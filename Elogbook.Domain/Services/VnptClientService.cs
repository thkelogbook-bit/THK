using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.VNPT;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using VnptHashSignatures.Pdf;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Dịch vụ kết nối và xử lý các nghiệp vụ ký số từ xa qua Gateway của VNPT (SmartCA).
    /// </summary>
    public class VnptClientService : IVnptClientService
    {
        /// <summary>
        /// Lấy thông tin chứng thư số (Certificate) của người dùng từ hệ thống VNPT.
        /// </summary>
        /// <param name="user">Thông tin tài khoản người dùng cần lấy chứng thư.</param>
        /// <returns>Đối tượng <see cref="VnptUserCertificate"/> chứa thông tin chứng thư, hoặc <c>null</c> nếu thất bại.</returns>
        public VnptUserCertificate VnptGetCertificate(UserInfo user)
        {
            try
            {
                string uri = AppSettings.VNPT_GATEWAY_URL + "/credentials/get_certificate";
                var requestBody = new
                {
                    sp_id = AppSettings.VNPT_SP_ID,
                    sp_password = AppSettings.VNPT_SP_PASSWORD,
                    user_id = user.NationalID,
                    serial_number = "",
                    transaction_id = Guid.NewGuid().ToString()
                };

                var response = VnptPostRequest(requestBody, uri);
                if (response != null)
                {
                    var res = JsonConvert.DeserializeObject<VnptResGetCert>(response);
                    if (res?.data?.user_certificates?.Count > 0)
                        return res.data.user_certificates[0];
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "VnptClientService",
                    $"[Vnpt] Exception occurred while getting certificate from VNPT: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Cấu hình hiển thị vùng chữ ký hình ảnh lên file PDF.
        /// </summary>
        /// <param name="pdfSigner">Đối tượng ký số tệp PDF.</param>
        /// <param name="approverSignature">Chuỗi ảnh chữ ký dạng Base64 (có thể chứa prefix data:image).</param>
        /// <param name="signatureX">Tọa độ X (trục ngang) góc dưới bên trái vùng ký.</param>
        /// <param name="signatureY">Tọa độ Y (trục dọc) góc dưới bên trái vùng ký.</param>
        /// <param name="width">Chiều rộng vùng hiển thị chữ ký.</param>
        /// <param name="height">Chiều cao vùng hiển thị chữ ký.</param>
        /// <param name="pageNum">Trang PDF sẽ hiển thị chữ ký.</param>
        /// <param name="reason">Lý do ký nhận.</param>
        public void ConfigureSignatureView(PdfHashSigner pdfSigner, string approverSignature, float signatureX, float signatureY, float width, float height, int pageNum, string reason)
        {
            if (pdfSigner == null || string.IsNullOrEmpty(approverSignature)) return;

            string signatureBase64 = approverSignature.Contains(",")
                ? approverSignature.Substring(approverSignature.IndexOf(',') + 1)
                : approverSignature;

            pdfSigner.SetReason(reason);

            // =========================================================================
            // THAY ĐỔI TẠI ĐÂY: Chuyển sang RenderMode.GRAPHIC để ẩn hoàn toàn Text
            // =========================================================================
            pdfSigner.SetRenderingMode(PdfHashSigner.RenderMode.LOGO_ONLY);

            pdfSigner.SetCustomImage(Convert.FromBase64String(signatureBase64));

            int rightX = (int)Math.Round(signatureX + width);
            int topY = (int)Math.Round(signatureY + height);

            pdfSigner.AddSignatureView(new PdfSignatureView
            {
                Rectangle = string.Format("{0},{1},{2},{3}",
                    (int)Math.Round(signatureX),
                    (int)Math.Round(signatureY),
                    rightX,
                    topY),
                Page = pageNum
            });
        }

        /// <summary>
        /// Thực hiện vòng lặp gửi yêu cầu kiểm tra (Polling) để lấy chuỗi chữ ký số sau khi người dùng xác nhận trên App SmartCA.
        /// </summary>
        /// <param name="transactionId">Mã định danh giao dịch cần kiểm tra trạng thái.</param>
        /// <returns>Chuỗi giá trị chữ ký dạng Base64/Hex nếu thành công, ngược lại trả về <c>null</c> khi hết thời gian chờ.</returns>
        public string VnptPollSignature(string transactionId)
        {
            int maxPolling = AppSettings.VNPT_MAX_POLLING_COUNT;
            int pollingInterval = AppSettings.VNPT_POLLING_INTERVAL_MS;

            for (int i = 0; i < maxPolling; i++)
            {
                var status = VnptGetSignStatus(transactionId);
                if (status?.signatures?.Count > 0)
                    return status.signatures[0].signature_value;

                Thread.Sleep(pollingInterval);
            }
            return null;
        }

        /// <summary>
        /// Chuyển đổi chuỗi băm dạng Base64 sang chuỗi định dạng Hexadecimal (Thập lục phân viết thường, loại bỏ gạch nối).
        /// </summary>
        /// <param name="base64Hash">Chuỗi mã băm định dạng Base64.</param>
        /// <returns>Chuỗi ký tự Hex hoàn chỉnh.</returns>
        public string HashToHexString(string base64Hash)
        {
            return BitConverter.ToString(Convert.FromBase64String(base64Hash)).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Gửi yêu cầu ký một chuỗi mã băm (Hash) tài liệu lên cổng dịch vụ của VNPT.
        /// Bước này sẽ kích hoạt thông báo xác nhận (Push Notification) về ứng dụng di động SmartCA của người dùng.
        /// </summary>
        /// <param name="dataToBeSign">Chuỗi dữ liệu băm của văn bản cần ký.</param>
        /// <param name="serialNumber">Số Serial chứng thư số của người ký.</param>
        /// <param name="displayName">Tên hiển thị hoặc mô tả nội dung giao dịch ký văn bản.</param>
        /// <returns>Đối tượng dữ liệu phản hồi <see cref="VnptDataSign"/> chứa mã giao dịch ký từ VNPT, hoặc chứa error_message nếu có lỗi.</returns>
        public VnptDataSign VnptSignHash(string dataToBeSign, string serialNumber, string displayName, string userId)
        {
            try
            {
                string uri = AppSettings.VNPT_GATEWAY_URL + "/signatures/sign";
                var requestBody = new
                {
                    sp_id = AppSettings.VNPT_SP_ID,
                    sp_password = AppSettings.VNPT_SP_PASSWORD,
                    transaction_id = Guid.NewGuid().ToString(),
                    user_id = userId,
                    transaction_desc = displayName,
                    sign_files = new[]
                    {
                        new
                        {
                            data_to_be_signed = dataToBeSign,
                            doc_id = dataToBeSign,
                            file_type = "pdf",
                            sign_type = "hash"
                        }
                    },
                    serial_number = serialNumber
                };

                var response = VnptPostRequest(requestBody, uri);
                if (response != null)
                {
                    var res = JsonConvert.DeserializeObject<VnptResSign>(response);
                    if (res != null)
                    {
                        // Check if response indicates success
                        if (res.data != null && string.IsNullOrEmpty(res.data.error_message))
                            return res.data;
                        
                        // If data exists but status_code indicates failure, set error message
                        if (res.data != null && (res.status_code != 0 && res.status_code != 1))
                        {
                            res.data.error_message = res.message;
                            LogCustom.LogMessage(TrackingLogLevel.ERROR, "PdfSigningService", 
                                $"[VNPT] VnptSignHash failed - Status Code: {res.status_code}, Message: {res.message}");
                            return res.data;
                        }
                        
                        // If data is null, create error response object
                        if (res.data == null && res.status_code != 0 && res.status_code != 1)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.ERROR, "VnptClientService", 
                                $"[VNPT] VnptSignHash failed - Status Code: {res.status_code}, Message: {res.message}");
                            return new VnptDataSign { error_message = res.message };
                        }
                    }
                }
            }
            catch (Exception ex){
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "VnptClientService", $"[VNPT] Exception: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Gửi yêu cầu HTTP để kiểm tra tình trạng xử lý hiện tại của một giao dịch ký số qua API VNPT.
        /// </summary>
        /// <param name="transactionId">Mã định danh giao dịch cần truy vấn tình trạng.</param>
        /// <returns>Đối tượng <see cref="VnptDataTransaction"/> chứa danh sách chữ ký số và trạng thái nếu thành công.</returns>
        private VnptDataTransaction VnptGetSignStatus(string transactionId)
        {
            try
            {
                string uri = AppSettings.VNPT_GATEWAY_URL + "/signatures/sign/" + transactionId + "/status";
                var response = VnptPostRequest(new { }, uri);
                if (response != null)
                {
                    var res = JsonConvert.DeserializeObject<VnptResStatus>(response);
                    if (res?.data != null)
                        return res.data;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "VnptClientService",
                    $"[Vnpt] Exception VnptGetSignStatus : {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Hàm dùng chung thực hiện gửi yêu cầu HTTP POST sử dụng thư viện RestSharp tới đối tác VNPT.
        /// </summary>
        /// <param name="requestBody">Đối tượng chứa nội dung tham số cần gửi.</param>
        /// <param name="serviceUri">Địa chỉ URL API cần kết nối.</param>
        /// <returns>Chuỗi nội dung văn bản phản hồi (JSON string) từ Server nếu mã kết quả là HTTP 200 (OK).</returns>
        private string VnptPostRequest(object requestBody, string serviceUri)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

                RestClient client = new RestClient(serviceUri);
                RestRequest request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(requestBody), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                if (response?.StatusCode == HttpStatusCode.OK)
                    return response.Content;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "VnptClientService",
                    $"[Vnpt] Exception VnptPostRequest : {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Phương thức ủy quyền (Callback) luôn đồng ý chấp nhận chứng chỉ SSL của máy chủ từ xa mà không qua kiểm tra hệ thống.
        /// </summary>
        public static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            // Trả về true cho phép kết nối HTTPS thành công bất chấp chứng chỉ SSL hết hạn hoặc không an toàn
            return true;
        }

    }
}
