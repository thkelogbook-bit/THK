using Org.BouncyCastle.X509;
using System;
using ViettelFileSigner;

namespace Elogbook.Domain.Models
{
    public class HashFilePdf
    {
        protected HashFilePdf() { }

        public static String GetHashTypeRectangleText(String src, X509Certificate[] certChain)
        {
            SignPdfFile pdfSig = new SignPdfFile();
            DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigRectangleText(0, 10, 10, 200, 80,
                    null, DisplayConfig.SIGN_TEXT_FORMAT_4, CertUtils.GetCN(certChain[0]), "Kiểm tra", "Hà Nội", DisplayConfig.DATE_FORMAT_1);
            displayConfig.IsDisplaySignature = false;
            String base64Hash = pdfSig.createHash(src, certChain, displayConfig);
            var session = System.Web.HttpContext.Current.Session;
            session["pdfSig"] = pdfSig;
            return base64Hash;
        }

        public static String GetHashTypeRectangleText2_ExistedSignatureField(String src, X509Certificate[] certChain, String displayText, String fieldName)
        {
            SignPdfFile pdfSig = new SignPdfFile();
            DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigRectangleText_ExistedSignatureField(1, 10, 10, 200, 80,
                    displayText, null, CertUtils.GetCN(certChain[0]), "Kiểm tra", "Hà Nội", DisplayConfig.DATE_FORMAT_1);
            String base64Hash = pdfSig.createHashExistedSignatureField(src, certChain, displayConfig, fieldName);
            var session = System.Web.HttpContext.Current.Session;
            session["pdfSig"] = pdfSig;
            return base64Hash;
        }
    }
}