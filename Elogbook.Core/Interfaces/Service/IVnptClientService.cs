using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.VNPT;
using VnptHashSignatures.Pdf;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVnptClientService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        VnptUserCertificate VnptGetCertificate(UserInfo user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdfSigner"></param>
        /// <param name="approverSignature"></param>
        /// <param name="signatureX"></param>
        /// <param name="signatureY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pageNum"></param>
        /// <param name="reason"></param>
        void ConfigureSignatureView(PdfHashSigner pdfSigner, string approverSignature, float signatureX, float signatureY, float width, float height, int pageNum, string reason);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        string VnptPollSignature(string transactionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64Hash"></param>
        /// <returns></returns>
        string HashToHexString(string base64Hash);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataToBeSign"></param>
        /// <param name="serialNumber"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        VnptDataSign VnptSignHash(string dataToBeSign, string serialNumber, string displayName, string userId);
    }
}
