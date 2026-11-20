using Elogbook.Core;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Elogbook.Domain.Models
{
    public class MobileCA
    {
        private const string PROCESS_CODE_SIGN = "000001";
        private const string PROCESS_CODE_QUERYCER = "000006";
        private const string MODE_SYNC = "SYNC";
        private const string FORMAT_DATE = "{0:yyyyMMddHHmmss}";
        private const string MSSP_ID = "Viettel";
        private const int FORCE_PIN = 1;
        private const int RUN_IMM = 0; // ko yeu cau ky ngay
        private const int SIG_TYPE = 0; // ky van ban
        private const bool FORCE_PIN_SPECIFIED = true;
        private const bool SIG_TYPE_SPECIFIED = true;
        private const bool REQUEST_ID_SPECIFIED = true;
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();

        public MobileCA()
        {
        }

        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }

        public long GenerateRequestId()
        {
            int seq = GetRandomNumber(1, 999999);
            string strTrace = seq.ToString();
            while (strTrace.Length < 6)
            {
                strTrace = "0" + strTrace;
            }

            return long.Parse(DateTime.Now.ToString("yyMMdd") + strTrace);
        }

        public string[] GetCertificatefromPrikey(string apId, string msisdn, string certSerial, string prikey)
        {
            try
            {
                MobileCAWS.APWsImplService service = new MobileCAWS.APWsImplService();
                var url = AppSettings.VIETTEL_CA_DOTNET_WEB_MobileCAWS_APWsImplService;
                if (string.IsNullOrWhiteSpace(url))
                {
                    service.Url = url;
                }

                MobileCAWS.transactionInfo transInfo = new MobileCAWS.transactionInfo
                {
                    apId = apId,
                    msspId = MSSP_ID,
                    processCode = PROCESS_CODE_QUERYCER,
                    msisdn = msisdn
                };
                if (certSerial != null)
                {
                    transInfo.certSerial = certSerial;
                }

                transInfo.requestId = GenerateRequestId();
                transInfo.requestIdSpecified = REQUEST_ID_SPECIFIED;
                transInfo.reqDate = string.Format(FORMAT_DATE, DateTime.Now);
                string strCombine = transInfo.apId + transInfo.msspId
                    + transInfo.processCode + transInfo.requestId.ToString() + transInfo.msisdn;
                transInfo.mac = RsaSignWithPrivateFromJava(strCombine, prikey);
                transInfo = service.certificateQuery(transInfo);

                return transInfo.certList;
            }
            catch
            {
                //ignore error
                return new string[0];
            }
        }

        private string RsaSignWithPrivateFromJava(string clearText, string privateKey)
        {
            byte[] keyData = Base64.Decode(privateKey);

            AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(keyData);

            RsaPrivateCrtKeyParameters rsaKeyParameters = (RsaPrivateCrtKeyParameters)privKey;
            RSAParameters rsaParameters = new RSAParameters
            {
                Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
                P = rsaKeyParameters.P.ToByteArrayUnsigned(),
                Q = rsaKeyParameters.Q.ToByteArrayUnsigned(),
                DP = rsaKeyParameters.DP.ToByteArrayUnsigned(),
                DQ = rsaKeyParameters.DQ.ToByteArrayUnsigned(),
                InverseQ = rsaKeyParameters.QInv.ToByteArrayUnsigned(),
                D = rsaKeyParameters.Exponent.ToByteArrayUnsigned(),
                Exponent = rsaKeyParameters.PublicExponent.ToByteArrayUnsigned()
            };

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParameters);

                // Hash the data
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] dataToByte = encoding.GetBytes(clearText);
                    byte[] hash = sha1.ComputeHash(dataToByte);
                    // Sign the hash
                    return Convert.ToBase64String(rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
                }
            }
        }

        public string SignSynchronouswithPrikey(string dataSignByteSHa1, string apId, string msisdn, string dataDisplay, string prikey)
        {
            try
            {
                MobileCAWS.APWsImplService service = new MobileCAWS.APWsImplService();
                var url = AppSettings.VIETTEL_CA_DOTNET_WEB_MobileCAWS_APWsImplService;
                if (string.IsNullOrWhiteSpace(url))
                {
                    service.Url = url;
                }

                MobileCAWS.transactionInfo transInfo = new MobileCAWS.transactionInfo
                {
                    apId = apId,
                    msspId = MSSP_ID,
                    processCode = PROCESS_CODE_SIGN,
                    msisdn = msisdn,
                    msgMode = MODE_SYNC, // co che dong bo
                    forcePIN = FORCE_PIN, // bat buoc nhap PIN
                    forcePINSpecified = FORCE_PIN_SPECIFIED,
                    runImm = RUN_IMM, // ko yeu cau ky ngay
                    sigType = SIG_TYPE, // ky van ban
                    sigTypeSpecified = SIG_TYPE_SPECIFIED,
                    dataDisplay = dataDisplay,
                    requestIdSpecified = REQUEST_ID_SPECIFIED,
                    reqDate = string.Format(FORMAT_DATE, DateTime.Now),
                    requestId = GenerateRequestId(),
                    dataSign = dataSignByteSHa1
                };

                string strCombine = transInfo.apId + transInfo.msspId + transInfo.processCode + transInfo.requestId.ToString() +
                    transInfo.msisdn + transInfo.dataSign + transInfo.dataDisplay + transInfo.sigType.ToString();

                transInfo.mac = RsaSignWithPrivateFromJava(strCombine, prikey);

                if (transInfo.mac == null)
                {
                    return null;
                }

                transInfo = service.signature(transInfo);

                if (transInfo.errorCode != "00")
                {
                    return null;
                }

                return transInfo.signature;
            }
            catch
            {
                return null;
            }
        }

        private byte[] SignSynchronouswithPrikey(byte[] data, string apId, string msisdn, string dataDisplay, string prikey)
        {
            try
            {
                MobileCAWS.APWsImplService service = new MobileCAWS.APWsImplService();
                var url = AppSettings.VIETTEL_CA_DOTNET_WEB_MobileCAWS_APWsImplService;
                if (string.IsNullOrWhiteSpace(url))
                {
                    service.Url = url;
                }
                MobileCAWS.transactionInfo transInfo = new MobileCAWS.transactionInfo
                {
                    apId = apId,
                    msspId = MSSP_ID,
                    processCode = PROCESS_CODE_SIGN,
                    msisdn = msisdn,
                    msgMode = MODE_SYNC, // co che dong bo
                    forcePIN = FORCE_PIN, // bat buoc nhap PIN
                    forcePINSpecified = FORCE_PIN_SPECIFIED,
                    runImm = RUN_IMM, // ko yeu cau ky ngay
                    sigType = SIG_TYPE, // ky van ban
                    sigTypeSpecified = SIG_TYPE_SPECIFIED,
                    dataDisplay = dataDisplay,
                    requestIdSpecified = REQUEST_ID_SPECIFIED,
                    reqDate = string.Format(FORMAT_DATE, DateTime.Now),
                    requestId = GenerateRequestId()
                };

                SHA1 sha = new SHA1Managed();
                byte[] dataSignByteSHa1 = sha.ComputeHash(data);
                transInfo.dataSign = Convert.ToBase64String(dataSignByteSHa1);

                string strCombine = transInfo.apId + transInfo.msspId + transInfo.processCode + transInfo.requestId.ToString() +
                    transInfo.msisdn + transInfo.dataSign + transInfo.dataDisplay + transInfo.sigType.ToString();

                transInfo.mac = RsaSignWithPrivateFromJava(strCombine, prikey);

                if (transInfo.mac == null)
                {
                    return new byte[0];
                }

                transInfo = service.signature(transInfo);
                if (transInfo.errorCode != "00")
                {
                    return new byte[0];
                }

                return Convert.FromBase64String(transInfo.signature);
            }
            catch
            {
                return new byte[0];
            }
        }
    }
}