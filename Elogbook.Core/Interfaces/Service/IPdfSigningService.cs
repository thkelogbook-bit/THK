using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPdfSigningService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalPdfPath"></param>
        /// <param name="signatureFieldName"></param>
        /// <param name="signerName"></param>
        /// <param name="userSign"></param>
        ResponseModel<bool> SignDocumentAndFillName(UserInfo userSign, string originalPdfPath, string signatureFieldName, string signerName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userSign"></param>
        /// <param name="dataDisplay"></param>
        /// <param name="originalPdfPath"></param>
        /// <param name="signatureFieldName"></param>
        /// <param name="signerName"></param>
        /// <returns></returns>
        ResponseModel<bool> SignWithViettelMobileCA(UserInfo userSign, string dataDisplay, string originalPdfPath, string signatureFieldName, string signerName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userApproved"></param>
        /// <param name="dataDisplay"></param>
        /// <param name="originalPdfPath"></param>
        /// <param name="signatureFieldName"></param>
        /// <param name="signerName"></param>
        /// <returns></returns>
        ResponseModel<bool> SignWithVnpt(UserInfo userApproved, string dataDisplay, string originalPdfPath, string signatureFieldName, string signerName);

        /// <summary>
        /// Ký số bằng Viettel MySign (bất đồng bộ).
        /// Luồng: Login → GetCredentials → tính hash PDF → SendHash (push notification tới app MySign)
        /// → Polling cho đến khi người dùng xác nhận → nhúng chữ ký vào PDF.
        /// </summary>
        /// <param name="userSign">Thông tin người ký — NationalID dùng làm user_id Viettel.</param>
        /// <param name="dataDisplay">Mô tả tài liệu hiển thị trên app MySign.</param>
        /// <param name="originalPdfPath">Đường dẫn file PDF gốc (sẽ bị ghi đè bằng bản đã ký).</param>
        /// <param name="signatureFieldName">Tên AcroField chữ ký (dạng Sig_xxx).</param>
        /// <param name="signerName">Họ tên người ký (điền vào field Name_xxx).</param>
        /// <returns><see cref="ResponseModel{bool}"/> — Data=true nếu ký thành công.</returns>
        ResponseModel<bool> SignWithViettelMySign(UserInfo userSign, string dataDisplay, string originalPdfPath, string signatureFieldName, string signerName);

    }
}
 