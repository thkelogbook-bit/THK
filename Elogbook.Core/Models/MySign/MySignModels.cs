using System.Collections.Generic;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.MySign
{
    // =========================================================================
    // LOGIN  —  POST /vtss/service/ras/v1/login
    // =========================================================================

    public class MySignLoginRequest
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("profile_id")]
        public string ProfileId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    public class MySignLoginResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }

    // =========================================================================
    // CERTIFICATE INFO  —  POST /vtss/service/certificates/info
    // Response thực tế là mảng các credential object, mỗi object chứa đầy đủ
    // credential_id, cert (bao gồm certificates[] chain), key, authMode...
    // =========================================================================

    public class MySignCertInfoRequest
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("profile_id")]
        public string ProfileId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("certificates")]
        public string Certificates { get; set; } = "chain";

        [JsonProperty("certInfo")]
        public bool CertInfo { get; set; } = true;

        [JsonProperty("authInfo")]
        public bool AuthInfo { get; set; } = false;
    }

    /// <summary>
    /// Một credential trong response của /vtss/service/certificates/info.
    /// API trả về mảng List&lt;MySignCredentialInfo&gt;.
    /// </summary>
    public class MySignCredentialInfo
    {
        [JsonProperty("credential_id")]
        public string CredentialId { get; set; }

        [JsonProperty("cert")]
        public MySignCertInfo Cert { get; set; }

        [JsonProperty("key")]
        public MySignKeyInfo Key { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("authMode")]
        public string AuthMode { get; set; }

        [JsonProperty("multisign")]
        public int Multisign { get; set; }

        [JsonProperty("SCAL")]
        public string SCAL { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("subscriber_id")]
        public string SubscriberId { get; set; }

        [JsonProperty("sign_package")]
        public string SignPackage { get; set; }

        [JsonProperty("main_product_code")]
        public string MainProductCode { get; set; }

        /// <summary>
        /// Trả về cert_data (Base64 DER) của end-entity certificate — index 0 trong chain.
        /// Dùng để truyền vào HashSignerFactory.GenerateSigner().
        /// </summary>
        [JsonIgnore]
        public string CertData => Cert?.Certificates != null && Cert.Certificates.Count > 0
            ? Cert.Certificates[0]
            : null;

        /// <summary>
        /// Cert còn hiệu lực không (status = "valid").
        /// </summary>
        [JsonIgnore]
        public bool IsCertValid => string.Equals(Cert?.Status, "valid",
            System.StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Key có thể dùng để ký không (status = "enabled").
        /// </summary>
        [JsonIgnore]
        public bool IsKeyEnabled => string.Equals(Key?.Status, "enabled",
            System.StringComparison.OrdinalIgnoreCase);
    }

    public class MySignCertInfo
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("certificates")]
        public List<string> Certificates { get; set; }

        [JsonProperty("issuerDN")]
        public string IssuerDN { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("subjectDN")]
        public string SubjectDN { get; set; }

        [JsonProperty("validFrom")]
        public string ValidFrom { get; set; }

        [JsonProperty("validTo")]
        public string ValidTo { get; set; }
    }

    public class MySignKeyInfo
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("algo")]
        public List<string> Algo { get; set; }

        [JsonProperty("len")]
        public int Len { get; set; }

        [JsonProperty("curve")]
        public string Curve { get; set; }
    }

    // Giữ lại để tránh break nếu có code cũ tham chiếu — alias sang model mới
    [System.Obsolete("Dùng MySignCertInfoRequest thay thế")]
    public class MySignCredentialsRequest : MySignCertInfoRequest { }

    // =========================================================================
    // SIGN HASH  —  POST /vtss/service/signHash   (async=1)
    // =========================================================================

    public class MySignDocument
    {
        [JsonProperty("document_id")]
        public string DocumentId { get; set; }

        /// <summary>
        /// Tên tài liệu hiển thị trên app MySign.
        /// Phải encode Base64 UTF-8, nội dung gốc chỉ gồm chữ cái, số, _, -, dấu cách.
        /// </summary>
        [JsonProperty("document_name")]
        public string DocumentName { get; set; }
    }

    public class MySignHashRequest
    {
        [JsonProperty("credentialID")]
        public string CredentialId { get; set; }

        [JsonProperty("numSignatures")]
        public int NumSignatures { get; set; } = 1;

        [JsonProperty("hash")]
        public List<string> Hash { get; set; }

        [JsonProperty("hashAlgo")]
        public string HashAlgo { get; set; } = "2.16.840.1.101.3.4.2.1"; // SHA-256

        [JsonProperty("signAlgo")]
        public string SignAlgo { get; set; } = "1.2.840.113549.1.1.1"; // RSA

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("documents")]
        public List<MySignDocument> Documents { get; set; }

        /// <summary>
        /// Chế độ bất đồng bộ: luôn = 1 cho MySign async.
        /// </summary>
        [JsonProperty("async")]
        public int Async { get; set; } = 1;
    }

    public class MySignHashResponse
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }

    // =========================================================================
    // GET REQUEST STATUS  —  POST /vtss/service/requests/status
    // =========================================================================

    public class MySignStatusRequest
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }
    }

    public class MySignStatusResponse
    {
        /// <summary>
        /// Danh sách chữ ký Base64. Chỉ có giá trị khi Status = "1".
        /// </summary>
        [JsonProperty("signatures")]
        public List<string> Signatures { get; set; }

        /// <summary>
        /// Mã trạng thái:
        ///   1     – Thành công
        ///   4000  – Chờ người dùng xác nhận
        ///   4001  – Timeout (hết thời gian chờ)
        ///   4002  – Người dùng từ chối
        ///   4004  – Ký thất bại
        ///   50000 – Lỗi hệ thống
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }

    // =========================================================================
    // Kết quả trả về từ IViettelMySignService.SignHash
    // =========================================================================

    public class MySignHashResult
    {
        /// <summary>
        /// transactionId để dùng khi polling.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Thông báo lỗi nếu gửi hash thất bại.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
