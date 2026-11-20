using Elogbook.Core.Constants;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Elogbook.Core
{
    /// <summary>
    /// EncryptDecrypt
    /// </summary>
    public static class EncryptDecrypt
    {
        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encrypt(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] b = ASCIIEncoding.ASCII.GetBytes(data);
                    TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider
                    {
                        Key = ASCIIEncoding.ASCII.GetBytes(CommonConstant.CRYPT_KEY),
                        Mode = CipherMode.ECB,
                        Padding = PaddingMode.PKCS7
                    };
                    ICryptoTransform trans = triple.CreateEncryptor();
                    byte[] resultArr = trans.TransformFinalBlock(b, 0, b.Length);
                    triple.Clear();
                    return Convert.ToBase64String(resultArr);
                }
                return "";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decrypt(string data)
        {
            byte[] b;

            try
            {
                b = Convert.FromBase64String(data);
                TripleDESCryptoServiceProvider tripleDec = new TripleDESCryptoServiceProvider
                {
                    Key = ASCIIEncoding.ASCII.GetBytes(CommonConstant.CRYPT_KEY),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform trans = tripleDec.CreateDecryptor();
                byte[] resultArr = trans.TransformFinalBlock(b, 0, b.Length);
                return ASCIIEncoding.ASCII.GetString(resultArr);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
