using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.MySign;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Dịch vụ kết nối Viettel MySign — ký bất đồng bộ (async) qua app MySign trên điện thoại.
    ///
    /// Luồng sử dụng:
    ///   1. MySignLogin(userId)                              → access_token
    ///   2. MySignGetCredentials(token, userId)              → credential_id + cert_data
    ///   3. MySignSendHash(token, credentialId, hash, name)  → transactionId
    ///   4. MySignPollSignature(token, transactionId)        → chữ ký Base64
    ///
    /// Ghi chú: access_token hết hạn sau 3600 giây — không cache lâu dài giữa các request.
    /// </summary>
    public class ViettelMySignService : IViettelMySignService
    {
        private const string LOG_TAG = "ViettelMySignService";

        // =====================================================================
        // Bước 1 — Login
        // =====================================================================

        /// <summary>
        /// Đăng nhập lấy access_token.
        /// POST {BASE_URL}/vtss/service/ras/v1/login
        /// </summary>
        public string MySignLogin(string userId)
        {
            try
            {
                string uri = AppSettings.VIETTEL_MYSIGN_BASE_URL + "/vtss/service/ras/v1/login";

                var body = new MySignLoginRequest
                {
                    ClientId     = AppSettings.VIETTEL_MYSIGN_CLIENT_ID,
                    ClientSecret = AppSettings.VIETTEL_MYSIGN_CLIENT_SECRET,
                    ProfileId    = AppSettings.VIETTEL_MYSIGN_PROFILE_ID,
                    UserId       = userId
                };

                string responseJson = PostJson(uri, body, bearerToken: null);
                if (string.IsNullOrEmpty(responseJson)) return null;

                var response = JsonConvert.DeserializeObject<MySignLoginResponse>(responseJson);
                if (response == null || string.IsNullOrEmpty(response.AccessToken))
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                        $"[MySign] Login thất bại — userId={userId}, error={response?.Error}, desc={response?.ErrorDescription}");
                    return null;
                }

                LogCustom.LogMessage(TrackingLogLevel.INFO, LOG_TAG,
                    $"[MySign] Login thành công — userId={userId}");
                return response.AccessToken;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                    $"[MySign] Exception tại MySignLogin — userId={userId}: {ex.Message}");
                return null;
            }
        }

        // =====================================================================
        // Bước 2 — Lấy certificate info (credential_id + cert chain)
        // =====================================================================

        /// <summary>
        /// Lấy danh sách credential kèm certificate chain của người ký.
        /// POST {BASE_URL}/vtss/service/certificates/info
        ///
        /// Response là mảng List&lt;MySignCredentialInfo&gt;. Method tự động chọn
        /// credential hợp lệ đầu tiên (cert valid + key enabled).
        /// Nếu không có credential nào hợp lệ, trả về credential đầu tiên trong danh sách.
        /// </summary>
        public MySignCredentialInfo MySignGetCertificateInfo(string accessToken, string userId)
        {
            try
            {
                string uri = AppSettings.VIETTEL_MYSIGN_BASE_URL + "/vtss/service/certificates/info";

                var body = new MySignCertInfoRequest
                {
                    ClientId     = AppSettings.VIETTEL_MYSIGN_CLIENT_ID,
                    ClientSecret = AppSettings.VIETTEL_MYSIGN_CLIENT_SECRET,
                    ProfileId    = AppSettings.VIETTEL_MYSIGN_PROFILE_ID,
                    UserId       = userId,
                    Certificates = "chain",
                    CertInfo     = true,
                    AuthInfo     = false
                };

                string responseJson = PostJson(uri, body, bearerToken: accessToken);
                if (string.IsNullOrEmpty(responseJson)) return null;

                var credentials = JsonConvert.DeserializeObject<List<MySignCredentialInfo>>(responseJson);
                if (credentials == null || credentials.Count == 0)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                        $"[MySign] GetCertificateInfo thất bại — userId={userId}, response rỗng");
                    return null;
                }

                // Ưu tiên credential có cert valid + key enabled
                var selected = credentials.Find(c => c.IsCertValid && c.IsKeyEnabled)
                            ?? credentials[0];

                LogCustom.LogMessage(TrackingLogLevel.INFO, LOG_TAG,
                    $"[MySign] GetCertificateInfo thành công — userId={userId}, " +
                    $"total={credentials.Count}, selected={selected.CredentialId}, " +
                    $"certStatus={selected.Cert?.Status}, keyStatus={selected.Key?.Status}");

                return selected;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                    $"[MySign] Exception tại MySignGetCertificateInfo — userId={userId}: {ex.Message}");
                return null;
            }
        }

        // =====================================================================
        // Bước 3 — Gửi hash (async = 1)
        // =====================================================================

        /// <summary>
        /// Gửi hash tài liệu để người dùng xác nhận ký trên app MySign (bất đồng bộ).
        /// POST {BASE_URL}/vtss/service/signHash
        ///
        /// documentName sẽ được encode Base64 UTF-8 tự động trước khi gửi (theo yêu cầu API).
        /// Nội dung gốc chỉ được chứa chữ cái a-z A-Z, số 0-9, dấu _, -, dấu cách.
        /// </summary>
        public MySignHashResult MySignSendHash(string accessToken, string credentialId, string base64Hash, string documentName)
        {
            try
            {
                string uri = AppSettings.VIETTEL_MYSIGN_BASE_URL + "/vtss/service/signHash";

                // documentName phải encode Base64 UTF-8 theo tài liệu API Viettel MySign
                string encodedDocName  = EncodeBase64Utf8(SanitizeDocumentName(documentName));
                string encodedDesc     = encodedDocName; // dùng chung cho description

                var body = new MySignHashRequest
                {
                    CredentialId   = credentialId,
                    NumSignatures  = 1,
                    Hash           = new List<string> { base64Hash },
                    Description    = encodedDesc,
                    Documents      = new List<MySignDocument>
                    {
                        new MySignDocument
                        {
                            DocumentId   = Guid.NewGuid().ToString(),
                            DocumentName = encodedDocName
                        }
                    },
                    Async = 1
                };

                string responseJson = PostJson(uri, body, bearerToken: accessToken);
                if (string.IsNullOrEmpty(responseJson))
                    return new MySignHashResult { ErrorMessage = "[MySign] SendHash — không nhận được response từ server." };

                var response = JsonConvert.DeserializeObject<MySignHashResponse>(responseJson);
                if (response == null || string.IsNullOrEmpty(response.TransactionId))
                {
                    string errMsg = $"[MySign] SendHash thất bại — error={response?.Error}, desc={response?.ErrorDescription}";
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG, errMsg);
                    return new MySignHashResult { ErrorMessage = errMsg };
                }

                LogCustom.LogMessage(TrackingLogLevel.INFO, LOG_TAG,
                    $"[MySign] SendHash thành công — transactionId={response.TransactionId}");
                return new MySignHashResult { TransactionId = response.TransactionId };
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                    $"[MySign] Exception tại MySignSendHash: {ex.Message}");
                return new MySignHashResult { ErrorMessage = $"[MySign] Exception SendHash: {ex.Message}" };
            }
        }

        // =====================================================================
        // Bước 4 — Polling trạng thái
        // =====================================================================

        /// <summary>
        /// Polling trạng thái giao dịch cho đến khi người dùng xác nhận hoặc hết thời gian chờ.
        /// POST {BASE_URL}/vtss/service/requests/status
        ///
        /// Status codes:
        ///   "1"     — Thành công → trả về signatures[0] (Base64)
        ///   "4000"  — Đang chờ   → tiếp tục polling
        ///   "4001"  — Timeout
        ///   "4002"  — Người dùng từ chối
        ///   "4004"  — Ký thất bại
        ///   "50000" — Lỗi hệ thống
        /// </summary>
        public string MySignPollSignature(string accessToken, string transactionId)
        {
            int maxPolling       = AppSettings.VIETTEL_MYSIGN_MAX_POLLING_COUNT;
            int pollingIntervalMs = AppSettings.VIETTEL_MYSIGN_POLLING_INTERVAL_MS;

            for (int i = 0; i < maxPolling; i++)
            {
                var status = GetRequestStatus(accessToken, transactionId);

                if (status == null)
                {
                    LogCustom.LogMessage(TrackingLogLevel.WARN, LOG_TAG,
                        $"[MySign] Polling lần {i + 1}/{maxPolling} — không nhận được response, tiếp tục...");
                    Thread.Sleep(pollingIntervalMs);
                    continue;
                }

                switch (status.Status)
                {
                    case "1":
                        // Thành công
                        if (status.Signatures != null && status.Signatures.Count > 0)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.INFO, LOG_TAG,
                                $"[MySign] Polling thành công sau {i + 1} lần — transactionId={transactionId}");
                            return status.Signatures[0];
                        }
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                            $"[MySign] Status=1 nhưng signatures rỗng — transactionId={transactionId}");
                        return null;

                    case "4000":
                        // Đang chờ người dùng xác nhận — tiếp tục polling
                        LogCustom.LogMessage(TrackingLogLevel.INFO, LOG_TAG,
                            $"[MySign] Polling lần {i + 1}/{maxPolling} — đang chờ người dùng xác nhận...");
                        Thread.Sleep(pollingIntervalMs);
                        continue;

                    case "4001":
                        LogCustom.LogMessage(TrackingLogLevel.WARN, LOG_TAG,
                            $"[MySign] Yêu cầu ký hết thời gian chờ (Timeout) — transactionId={transactionId}");
                        return null;

                    case "4002":
                        LogCustom.LogMessage(TrackingLogLevel.WARN, LOG_TAG,
                            $"[MySign] Người dùng từ chối ký — transactionId={transactionId}");
                        return null;

                    case "4004":
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                            $"[MySign] Ký thất bại (4004) — transactionId={transactionId}");
                        return null;

                    default:
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                            $"[MySign] Trạng thái không xác định: {status.Status} — transactionId={transactionId}");
                        return null;
                }
            }

            LogCustom.LogMessage(TrackingLogLevel.WARN, LOG_TAG,
                $"[MySign] Hết {maxPolling} lần polling mà không có kết quả — transactionId={transactionId}");
            return null;
        }

        // =====================================================================
        // Private helpers
        // =====================================================================

        /// <summary>
        /// Gọi GET REQUEST STATUS để kiểm tra trạng thái 1 giao dịch.
        /// POST {BASE_URL}/vtss/service/requests/status
        /// </summary>
        private MySignStatusResponse GetRequestStatus(string accessToken, string transactionId)
        {
            try
            {
                string uri = AppSettings.VIETTEL_MYSIGN_BASE_URL + "/vtss/service/requests/status";
                var body = new MySignStatusRequest { TransactionId = transactionId };

                string responseJson = PostJson(uri, body, bearerToken: accessToken);
                if (string.IsNullOrEmpty(responseJson)) return null;

                return JsonConvert.DeserializeObject<MySignStatusResponse>(responseJson);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                    $"[MySign] Exception tại GetRequestStatus — transactionId={transactionId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gửi HTTP POST với body JSON, tuỳ chọn Authorization Bearer.
        /// Dùng WebClient thay vì RestSharp để tránh thêm dependency.
        /// </summary>
        private static string PostJson(string uri, object body, string bearerToken)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += IgnoreCertErrors;

                string json = JsonConvert.SerializeObject(body);
                byte[] data = Encoding.UTF8.GetBytes(json);

                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method      = "POST";
                request.ContentType = "application/json";
                request.Accept      = "application/json";
                request.ContentLength = data.Length;

                if (!string.IsNullOrEmpty(bearerToken))
                    request.Headers.Add("Authorization", "Bearer " + bearerToken);

                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    return reader.ReadToEnd();
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse httpResponse)
            {
                // Đọc body lỗi để log (400/401 trả về JSON error)
                using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    string errorBody = reader.ReadToEnd();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                        $"[MySign] HTTP {(int)httpResponse.StatusCode} — uri={uri}, body={errorBody}");
                }
                return null;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, LOG_TAG,
                    $"[MySign] Exception PostJson — uri={uri}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Encode chuỗi sang Base64 UTF-8 theo yêu cầu API Viettel MySign
        /// (dùng cho document_name và description).
        /// </summary>
        private static string EncodeBase64Utf8(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Làm sạch tên tài liệu — chỉ giữ chữ cái a-z A-Z, số 0-9, dấu _, -, dấu cách.
        /// Sau đó cắt còn tối đa 50 ký tự để Base64 < 100 ký tự (theo giới hạn Viettel).
        /// </summary>
        private static string SanitizeDocumentName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Tai lieu ky";
            var sb = new StringBuilder();
            foreach (char c in name)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') || c == '_' || c == '-' || c == ' ')
                    sb.Append(c);
            }
            string result = sb.ToString().Trim();
            if (result.Length > 50) result = result.Substring(0, 50);
            return string.IsNullOrEmpty(result) ? "Tai lieu ky" : result;
        }

        /// <summary>
        /// Callback bỏ qua lỗi SSL — tương tự VnptClientService.ValidateRemoteCertificate.
        /// </summary>
        private static bool IgnoreCertErrors(object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors errors) => true;
    }
}
