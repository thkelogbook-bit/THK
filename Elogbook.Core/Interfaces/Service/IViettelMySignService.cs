using Elogbook.Core.Models.MySign;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Dịch vụ kết nối Viettel MySign — ký bất đồng bộ (async) qua app MySign.
    /// </summary>
    public interface IViettelMySignService
    {
        /// <summary>
        /// Bước 1 — Đăng nhập lấy access_token.
        /// POST /vtss/service/ras/v1/login
        /// </summary>
        /// <param name="userId">User ID của người ký trong hệ thống Viettel (CCCD hoặc MST tuỳ cấu hình).</param>
        /// <returns>Access token dạng Bearer, hoặc null nếu thất bại.</returns>
        string MySignLogin(string userId);

        /// <summary>
        /// Bước 2 — Lấy danh sách credential kèm certificate chain của người ký.
        /// POST /vtss/service/certificates/info
        /// Response thực tế là mảng List&lt;MySignCredentialInfo&gt;, mỗi object chứa
        /// credential_id, cert.certificates[] (chain), key status, v.v.
        /// Method tự động chọn credential hợp lệ đầu tiên (cert valid + key enabled).
        /// </summary>
        /// <param name="accessToken">Access token từ bước Login.</param>
        /// <param name="userId">User ID của người ký.</param>
        /// <returns>
        /// <see cref="MySignCredentialInfo"/> của credential hợp lệ đầu tiên,
        /// hoặc null nếu không tìm thấy hoặc thất bại.
        /// </returns>
        MySignCredentialInfo MySignGetCertificateInfo(string accessToken, string userId);

        /// <summary>
        /// Bước 3 — Gửi hash tài liệu lên MySign để người dùng xác nhận trên app (bất đồng bộ).
        /// POST /vtss/service/signHash  (async = 1)
        /// </summary>
        /// <param name="accessToken">Access token từ bước Login.</param>
        /// <param name="credentialId">credential_id từ bước Credentials.</param>
        /// <param name="base64Hash">Hash tài liệu dạng Base64 (SHA-256).</param>
        /// <param name="documentName">Tên tài liệu hiển thị trên app MySign (plain text, chưa encode).</param>
        /// <returns>
        /// <see cref="MySignHashResult"/> chứa <c>TransactionId</c> nếu thành công,
        /// hoặc <c>ErrorMessage</c> nếu thất bại.
        /// </returns>
        MySignHashResult MySignSendHash(string accessToken, string credentialId, string base64Hash, string documentName);

        /// <summary>
        /// Bước 4 — Polling trạng thái giao dịch cho đến khi người dùng xác nhận hoặc hết thời gian chờ.
        /// POST /vtss/service/requests/status
        /// </summary>
        /// <param name="accessToken">Access token từ bước Login.</param>
        /// <param name="transactionId">transactionId từ bước SendHash.</param>
        /// <returns>
        /// Chữ ký dạng Base64 nếu người dùng xác nhận thành công (status = "1"),
        /// hoặc null nếu timeout / từ chối / lỗi.
        /// </returns>
        string MySignPollSignature(string accessToken, string transactionId);
    }
}
