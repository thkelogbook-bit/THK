using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.MySign;
using Elogbook.Core.Models.Response;
using Elogbook.Domain.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ViettelFileSigner;
using VnptHashSignatures.Common;
using VnptHashSignatures.Interface;
using VnptHashSignatures.Pdf;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Service xu ly ky so PDF: chu ky tay (username/password), Viettel CA, VNPT SmartCA, Viettel MySign.
    /// </summary>
    public class PdfSigningService : IPdfSigningService
    {
        private readonly IVnptClientService _vnptClientService;
        private readonly IViettelMySignService _viettelMySignService;

        // Duong dan font Arial ho tro Unicode tieng Viet tren Windows
        private static readonly string ArialFontPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts", "arial.ttf");

        #region Constructor

        public PdfSigningService(IVnptClientService vnptClientService, IViettelMySignService viettelMySignService)
        {
            _vnptClientService    = vnptClientService;
            _viettelMySignService = viettelMySignService;
        }

        #endregion

        // =====================================================================================
        // PUBLIC: Ky chu ky tay (username/password) - chen anh chu ky va dien ten vao PDF
        // =====================================================================================

        /// <summary>
        /// Chen anh chu ky tay vao Sig_ field va dien ho ten vao Name_ field.
        /// Su dung append=true de bao toan cau truc AcroForm cho nguoi ky tiep theo.
        /// </summary>
        public ResponseModel<bool> SignDocumentAndFillName(
            UserInfo userSign, string originalPdfPath, string signatureFieldName, string signerName)
        {
            var responseModel = new ResponseModel<bool>();
            string directory = Path.GetDirectoryName(originalPdfPath);
            string tempPdfPath = Path.Combine(directory, "temp_" + Guid.NewGuid() + ".pdf");

            try
            {
                using (PdfReader reader = new PdfReader(originalPdfPath))
                using (FileStream os = new FileStream(tempPdfPath, FileMode.Create))
                {
                    // append=true: bao toan cac Sig_/Name_ field con lai cho nguoi ky sau
                    PdfStamper stamper = new PdfStamper(reader, os, '\0', true);
                    AcroFields form = stamper.AcroFields;

                    // --- 1. Chen anh chu ky tay vao Sig_ field ---
                    if (!string.IsNullOrWhiteSpace(userSign.Signature))
                    {
                        // Loai bo data URI prefix neu co (vd: "data:image/png;base64,...")
                        string sigBase64 = userSign.Signature.Contains(",")
                            ? userSign.Signature.Substring(userSign.Signature.IndexOf(",") + 1)
                            : userSign.Signature;

                        byte[] imgBytes = Convert.FromBase64String(sigBase64);
                        Image sigImg = Image.GetInstance(imgBytes);

                        var positions = form.GetFieldPositions(signatureFieldName);
                        if (positions != null && positions.Count > 0)
                        {
                            var pos = positions[0];

                            // Thu nho anh xuong 80% de khong bi sat le khung
                            float maxWidth  = pos.position.Width  * 0.8f;
                            float maxHeight = pos.position.Height * 0.8f;
                            sigImg.ScaleToFit(maxWidth, maxHeight);

                            // Can giua anh trong o
                            float absX = pos.position.Left   + (pos.position.Width  - sigImg.ScaledWidth)  / 2f;
                            float absY = pos.position.Bottom + (pos.position.Height - sigImg.ScaledHeight) / 2f;
                            sigImg.SetAbsolutePosition(absX, absY);

                            PdfContentByte cb = stamper.GetOverContent(pos.page);
                            cb.AddImage(sigImg);

                            // Xoa Sig_ field sau khi da ve anh thuc te len
                            form.RemoveField(signatureFieldName);
                        }
                    }

                    // --- 2. Dien ho ten vao Name_ field ---
                    string nameFieldName = signatureFieldName.Replace("Sig_", "Name_");
                    SetNameField(form, nameFieldName, signerName, stamper);

                    stamper.Close();
                }

                // Ghi de len file goc
                File.Delete(originalPdfPath);
                File.Move(tempPdfPath, originalPdfPath);

                responseModel.Data = true;
            }
            catch (Exception ex)
            {
                if (File.Exists(tempPdfPath)) File.Delete(tempPdfPath);
                responseModel.ErrorMessages = new List<string>
                {
                    "Loi ngoai le thuc thi luong ky chu ky tay: " + ex.Message
                };
            }

            return responseModel;
        }

        // =====================================================================================
        // PUBLIC: Ky so Viettel MobileCA
        // =====================================================================================

        /// <summary>
        /// Ky so bang Viettel MobileCA.
        ///
        /// Nguyen tac hoat dong cua ViettelFileSigner.SignPdfFile (da decompile IL):
        ///   - createHash(inputFile, ...) : doc inputFile, tao internal tmpFile co empty sig field
        ///     dung chuan (/ByteRange placeholder), luu state vao instance (tmpFile, hash, chain).
        ///   - insertSignature(token, signedFile, ...) : doc tu this.tmpFile, nhung token vao,
        ///     ghi output ra signedFile (file MOI, khac inputFile).
        ///
        /// Luu y quan trong:
        ///   - inputFile va signedFile PHAI la 2 file rieng biet.
        ///   - Sig_ field trong PDF goc chi la widget annotation (khong co /ByteRange),
        ///     phai xoa truoc de Viettel tu tao lai dung chuan qua emptySignature().
        /// </summary>
        public ResponseModel<bool> SignWithViettelMobileCA(
            UserInfo userSign, string dataDisplay, string originalPdfPath,
            string signatureFieldName, string signerName)
        {
            var responseModel = new ResponseModel<bool>();
            var errorMessages = new List<string>();

            // 2 file rieng biet theo pattern cua WorkTicketService.IsSignedPDF:
            //   inputFile  - createHash doc vao, KHONG bi thay doi
            //   signedFile - insertSignature ghi output chu ky ra day
            string inputFile  = originalPdfPath + ".input.pdf";
            string signedFile = originalPdfPath + ".signed.pdf";

            try
            {
                // --- Lay chung thu so Viettel ---
                string msisdn = BuildMsisdn(userSign.PhoneNumber);

                MobileCA mobileCA = new MobileCA();
                string[] certList = mobileCA.GetCertificatefromPrikey(
                    AppSettings.VIETTEL_APIID, msisdn, null, AppSettings.VIETTEL_KEY);

                if (certList == null || certList.Length == 0)
                    return ErrorResponse(errorMessages, "So dien thoai nay chua duoc dang ky ky so Viettel CA");

                var certParser    = new X509CertificateParser();
                X509Certificate userCert     = certParser.ReadCertificate(Convert.FromBase64String(certList[0]));
                X509Certificate viettelCACert = CertUtils.GetX509Cert(AppSettings.VIETTEL_CERT);
                var certChain = new X509Certificate[] { userCert, viettelCACert };

                // --- Buoc 1: Lay toa do Sig_ field tu file goc (truoc khi xu ly) ---
                var sigPos = GetFieldPosition(originalPdfPath, signatureFieldName);
                if (sigPos == null)
                    return ErrorResponse(errorMessages, $"Khong tim thay khung chu ky: {signatureFieldName}");

                // Thu nho vung hien thi chu ky so xuong 80% tranh full cell
                float dispWidth  = sigPos.Width  * 0.8f;
                float dispHeight = sigPos.Height * 0.8f;
                float dispX = sigPos.Left   + (sigPos.Width  - dispWidth)  / 2f;
                float dispY = sigPos.Bottom + (sigPos.Height - dispHeight) / 2f;

                // --- Buoc 2: Tao inputFile ---
                // Neu PDF chua co chu ky so nao: dung append=false (BuildCleanPdf) de tao file sach.
                //   - Xoa Sig_ widget annotation (khong co /ByteRange) de Viettel tao lai dung chuan.
                //   - Dien ten vao Name_ field.
                //   - append=false an toan vi chua co chu ky nao can bao ve.
                //
                // Neu PDF da co chu ky so truoc do: PHAI dung append=true (BuildAppendPdf).
                //   - append=false se rewrite toan bo file → pha byte-range cua chu ky cu → INVALID.
                //   - append=true chi them revision moi len tren, khong cham vao bytes cu.
                //   - Xoa Sig_ field bang append=true van duoc, Viettel doc revision moi nhat.
                string nameFieldName = signatureFieldName.Replace("Sig_", "Name_");
                byte[] rawBytes = File.ReadAllBytes(originalPdfPath);

                bool hasPriorSignature = PdfHasDigitalSignature(rawBytes);
                byte[] inputBytes = hasPriorSignature
                    ? BuildAppendPdf(rawBytes, nameFieldName, signerName, sigFieldToRemove: signatureFieldName)
                    : BuildCleanPdf(rawBytes, nameFieldName, signerName, sigFieldToRemove: signatureFieldName);

                File.WriteAllBytes(inputFile, inputBytes);

                // --- Buoc 3: Tao hash ---
                SignPdfFile pdfSig = new SignPdfFile();
                string sigImgPath = WorkTicketService.LoadBase64(StripDataUriPrefix(userSign.Signature));

                DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigImage(
                    sigPos.Page,
                    dispX, dispY,
                    (int)Math.Round(dispWidth), (int)Math.Round(dispHeight),
                    CertUtils.GetCN(certChain[0]), dataDisplay, "",
                    sigImgPath);
                displayConfig.IsDisplaySignature = true;

                string base64Hash = pdfSig.createHash(inputFile, certChain, displayConfig);
                if (string.IsNullOrEmpty(base64Hash))
                    return ErrorResponse(errorMessages, "Tao hash PDF that bai (createHash tra ve null).");

                LogCustom.LogMessage(TrackingLogLevel.INFO, "PdfSigningService",
                    $"[Viettel] createHash OK, hashLen={base64Hash.Length}, field={signatureFieldName}, hasPriorSig={hasPriorSignature}");

                // --- Buoc 4: Gui hash len Viettel, cho nguoi ky xac nhan OTP ---
                string signatureToken = mobileCA.SignSynchronouswithPrikey(
                    base64Hash, AppSettings.VIETTEL_APIID, msisdn, dataDisplay, AppSettings.VIETTEL_KEY);

                if (string.IsNullOrEmpty(signatureToken))
                    return ErrorResponse(errorMessages, "He thong Viettel khong tra ve token chu ky so.");

                // --- Buoc 5: Nhung chu ky vao signedFile (file MOI, khac inputFile) ---
                var timestampConfig = new TimestampConfig { UseTimestamp = false };
                bool ok = pdfSig.insertSignature(signatureToken, signedFile, timestampConfig);

                LogCustom.LogMessage(TrackingLogLevel.INFO, "PdfSigningService",
                    $"[Viettel] insertSignature={ok}, signedFile exists={File.Exists(signedFile)}");

                if (!ok)
                    return ErrorResponse(errorMessages, "Nhung token chu ky so Viettel vao PDF that bai.");

                // --- Buoc 6: Thay the file goc bang signedFile ---
                File.Delete(originalPdfPath);
                File.Move(signedFile, originalPdfPath);

                responseModel.Data = true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PdfSigningService",
                    $"[Viettel] Exception: {ex.Message}");
                errorMessages.Add("Loi ngoai le thuc thi luong ky so Viettel: " + ex.Message);
                responseModel.ErrorMessages = errorMessages;
            }
            finally
            {
                TryDelete(inputFile);
                TryDelete(signedFile);
            }

            return responseModel;
        }

        // =====================================================================================
        // PUBLIC: Ky so VNPT SmartCA
        // =====================================================================================

        /// <summary>
        /// Ky so bang VNPT SmartCA.
        ///
        /// Luong xu ly:
        ///   1. Lay chung thu so VNPT cua nguoi dung.
        ///   2. Lam sach PDF (append=false) va dien ten vao Name_ field.
        ///   3. Tao hash PDF, cau hinh hien thi chu ky.
        ///   4. Gui hash len VNPT, poll cho nguoi dung xac nhan tren app SmartCA.
        ///   5. Nhung chu ky vao PDF, ghi de file goc.
        /// </summary>
        public ResponseModel<bool> SignWithVnpt(
            UserInfo userSign, string dataDisplay, string originalPdfPath,
            string signatureFieldName, string signerName)
        {
            var responseModel = new ResponseModel<bool>();
            var errorMessages = new List<string>();

            string tempPdfPath = originalPdfPath + ".processing.pdf";

            try
            {
                // --- Buoc 1: Lay chung thu so VNPT ---
                var userCert = _vnptClientService.VnptGetCertificate(userSign);
                if (userCert == null)
                    return ErrorResponse(errorMessages, "Khong tim thay chung thu so VNPT SmartCA cho nguoi dung nay");

                // --- Buoc 2: Lam sach PDF va dien ten ---
                // Neu chua co chu ky nao: append=false (BuildCleanPdf) → PDF sach, byte-range chinh xac.
                // Neu da co chu ky truoc: append=true (BuildAppendPdf) → giu nguyen bytes cu,
                //   chi them revision moi → byte-range chu ky cu khong bi anh huong.
                byte[] rawBytes = File.ReadAllBytes(originalPdfPath);
                bool hasPriorSignature = PdfHasDigitalSignature(rawBytes);
                byte[] cleanedBytes = hasPriorSignature
                    ? BuildAppendPdf(rawBytes,
                        nameFieldName: signatureFieldName.Replace("Sig_", "Name_"),
                        signerName: signerName,
                        sigFieldToRemove: null)
                    : BuildCleanPdf(rawBytes,
                        nameFieldName: signatureFieldName.Replace("Sig_", "Name_"),
                        signerName: signerName,
                        sigFieldToRemove: null);

                // --- Buoc 3: Lay toa do Sig_ field va tinh vung hien thi 80% ---
                var sigPos = GetFieldPositionFromBytes(cleanedBytes, signatureFieldName);
                if (sigPos == null)
                    return ErrorResponse(errorMessages, $"Khong tim thay khung chu ky: {signatureFieldName}");

                float dispWidth  = sigPos.Width  * 0.8f;
                float dispHeight = sigPos.Height * 0.8f;
                float dispX = sigPos.Left   + (sigPos.Width  - dispWidth)  / 2f;
                float dispY = sigPos.Bottom + (sigPos.Height - dispHeight) / 2f;

                File.WriteAllBytes(tempPdfPath, cleanedBytes);

                // --- Buoc 4: Tao hash PDF ---
                IHashSigner signer = HashSignerFactory.GenerateSigner(
                    cleanedBytes, userCert.cert_data, null, HashSignerFactory.PDF);
                signer.SetHashAlgorithm(MessageDigestAlgorithm.SHA256);

                _vnptClientService.ConfigureSignatureView(
                    signer as PdfHashSigner,
                    userSign.Signature,
                    dispX, dispY, dispWidth, dispHeight,
                    sigPos.Page, dataDisplay);

                string dataToBeSign = _vnptClientService.HashToHexString(signer.GetSecondHashAsBase64());

                // --- Buoc 5: Gui hash len VNPT, poll xac nhan ---
                var dataSign = _vnptClientService.VnptSignHash(
                    dataToBeSign, userCert.serial_number, dataDisplay, userSign.NationalID);

                if (dataSign == null)
                    return ErrorResponse(errorMessages, "Gui yeu cau ky so VNPT that bai");
                
                // Check if response contains error message from VNPT
                if (!string.IsNullOrEmpty(dataSign.error_message))
                    return ErrorResponse(errorMessages, dataSign.error_message);
                
                if (string.IsNullOrEmpty(dataSign.transaction_id))
                    return ErrorResponse(errorMessages, "Gui yeu cau ky so VNPT that bai");

                string datasigned = _vnptClientService.VnptPollSignature(dataSign.transaction_id);
                if (datasigned == null)
                    return ErrorResponse(errorMessages, "Nguoi ky chua xac nhan tren ung dung VNPT SmartCA");

                if (!signer.CheckHashSignature(datasigned))
                    return ErrorResponse(errorMessages, "Chu ky so VNPT khong hop le");

                // --- Buoc 6: Nhung chu ky vao PDF, ghi de file goc ---
                File.WriteAllBytes(tempPdfPath, signer.Sign(datasigned));
                File.Delete(originalPdfPath);
                File.Move(tempPdfPath, originalPdfPath);

                responseModel.Data = true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PdfSigningService",
                    $"[VNPT] Exception: {ex.Message}");
                errorMessages.Add("Ky so VNPT SmartCA that bai: " + ex.Message);
                responseModel.ErrorMessages = errorMessages;
            }
            finally
            {
                TryDelete(tempPdfPath);
            }

            return responseModel;
        }

        // =====================================================================================
        // PUBLIC: Ky so Viettel MySign (bat dong bo)
        // =====================================================================================

        /// <summary>
        /// Ky so bang Viettel MySign (bat dong bo).
        ///
        /// Luong xu ly:
        ///   1. Login → access_token
        ///   2. GetCredentials → credential_id + cert_data (Base64 DER)
        ///   3. Tinh hash PDF bang VnptHashSignatures (SHA-256) — phan nay cung lam sach PDF va dien ten
        ///   4. SendHash → transactionId (push notification toi app MySign cua nguoi dung)
        ///   5. Polling cho den khi nguoi dung xac nhan → signature Base64
        ///   6. [TODO] Nhung chu ky vao PDF — se implement sau khi xac nhan format signature voi Viettel
        ///
        /// user_id su dung: userSign.NationalID (CCCD/CMND hoac MST tuy cau hinh voi Viettel).
        /// </summary>
        public ResponseModel<bool> SignWithViettelMySign(
            UserInfo userSign, string dataDisplay, string originalPdfPath,
            string signatureFieldName, string signerName)
        {
            var responseModel = new ResponseModel<bool>();
            var errorMessages = new List<string>();

            string tempPdfPath = originalPdfPath + ".mysign.processing.pdf";

            try
            {
                // --- Buoc 1: Login lay access_token ---
                string userId     = userSign.NationalID;
                string accessToken = _viettelMySignService.MySignLogin(userId);
                if (string.IsNullOrEmpty(accessToken))
                    return ErrorResponse(errorMessages, "[MySign] Đăng nhập thất bại — kiểm tra cấu hình VIETTEL_MYSIGN_CLIENT_ID / CLIENT_SECRET.");

                // --- Buoc 2: Lay credential_id va cert chain ---
                var credential = _viettelMySignService.MySignGetCertificateInfo(accessToken, userId);
                if (credential == null || string.IsNullOrEmpty(credential.CredentialId))
                    return ErrorResponse(errorMessages, "[MySign] Không tìm thấy chứng thư số MySign cho người dùng này.");

                string credentialId = credential.CredentialId;

                // CertData = cert.certificates[0] (end-entity cert, Base64 DER)
                // Dùng để VnptHashSignatures embed cert vào PDF signature field
                // → Adobe Reader hiển thị: Ký bởi <subjectDN> / Viettel-CA / Valid
                string certData = credential.CertData;
                if (string.IsNullOrEmpty(certData))
                    return ErrorResponse(errorMessages, "[MySign] Không lấy được certificate từ MySign — không thể tạo hash PDF.");

                // --- Buoc 3: Lam sach PDF, dien ten, tinh hash ---
                byte[] rawBytes = File.ReadAllBytes(originalPdfPath);
                bool hasPriorSignature = PdfHasDigitalSignature(rawBytes);

                byte[] cleanedBytes = hasPriorSignature
                    ? BuildAppendPdf(rawBytes,
                        nameFieldName: signatureFieldName.Replace("Sig_", "Name_"),
                        signerName: signerName,
                        sigFieldToRemove: null)
                    : BuildCleanPdf(rawBytes,
                        nameFieldName: signatureFieldName.Replace("Sig_", "Name_"),
                        signerName: signerName,
                        sigFieldToRemove: null);

                // Lay toa do Sig_ field de cau hinh vung hien thi chu ky
                var sigPos = GetFieldPositionFromBytes(cleanedBytes, signatureFieldName);
                if (sigPos == null)
                    return ErrorResponse(errorMessages, $"[MySign] Không tìm thấy khung chữ ký: {signatureFieldName}");

                float dispWidth  = sigPos.Width  * 0.8f;
                float dispHeight = sigPos.Height * 0.8f;
                float dispX = sigPos.Left   + (sigPos.Width  - dispWidth)  / 2f;
                float dispY = sigPos.Bottom + (sigPos.Height - dispHeight) / 2f;

                File.WriteAllBytes(tempPdfPath, cleanedBytes);

                // Tinh hash PDF bang VnptHashSignatures (SHA-256) voi cert cua nguoi ky
                IHashSigner signer = HashSignerFactory.GenerateSigner(
                    cleanedBytes, certData, null, HashSignerFactory.PDF);
                signer.SetHashAlgorithm(MessageDigestAlgorithm.SHA256);

                // Cau hinh hien thi chu ky (su dung lai logic cua VNPT)
                _vnptClientService.ConfigureSignatureView(
                    signer as PdfHashSigner,
                    userSign.Signature,
                    dispX, dispY, dispWidth, dispHeight,
                    sigPos.Page, dataDisplay);

                // Hash dang Base64 de gui len MySign
                string base64Hash = signer.GetSecondHashAsBase64();
                if (string.IsNullOrEmpty(base64Hash))
                    return ErrorResponse(errorMessages, "[MySign] Tạo hash PDF thất bại.");

                LogCustom.LogMessage(TrackingLogLevel.INFO, "PdfSigningService",
                    $"[MySign] Hash OK, hashLen={base64Hash.Length}, field={signatureFieldName}");

                // --- Buoc 4: Gui hash len MySign → nhan transactionId ---
                var hashResult = _viettelMySignService.MySignSendHash(
                    accessToken, credentialId, base64Hash, dataDisplay);

                if (hashResult == null || !string.IsNullOrEmpty(hashResult.ErrorMessage))
                    return ErrorResponse(errorMessages, hashResult?.ErrorMessage ?? "[MySign] Gửi hash thất bại.");

                // --- Buoc 5: Polling cho den khi nguoi ky xac nhan tren app MySign ---
                string signatureBase64 = _viettelMySignService.MySignPollSignature(
                    accessToken, hashResult.TransactionId);

                if (string.IsNullOrEmpty(signatureBase64))
                    return ErrorResponse(errorMessages, "[MySign] Người ký chưa xác nhận trên app MySign hoặc đã từ chối.");

                LogCustom.LogMessage(TrackingLogLevel.INFO, "PdfSigningService",
                    $"[MySign] Nhận được chữ ký, signatureLen={signatureBase64.Length}");

                // --- Buoc 6: Nhung chu ky vao PDF ---
                // MySign tra ve signature dang Base64, signer.Sign() nhan Hex string
                // → convert Base64 → hex (cung cach VNPT dung)
                // Khong goi CheckHashSignature vi day la check noi bo cua VNPT library,
                // khong ap dung cho chu ky tu Viettel MySign.
                string signatureHex = BitConverter.ToString(Convert.FromBase64String(signatureBase64))
                    .Replace("-", "").ToLower();

                byte[] signedPdfBytes = signer.Sign(signatureHex);
                if (signedPdfBytes == null || signedPdfBytes.Length == 0)
                    return ErrorResponse(errorMessages, "[MySign] Nhúng chữ ký vào PDF thất bại — signer.Sign() trả về rỗng.");

                LogCustom.LogMessage(TrackingLogLevel.INFO, "PdfSigningService",
                    $"[MySign] Nhúng chữ ký thành công, pdfSize={signedPdfBytes.Length}");

                File.WriteAllBytes(tempPdfPath, signedPdfBytes);
                File.Delete(originalPdfPath);
                File.Move(tempPdfPath, originalPdfPath);

                responseModel.Data = true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PdfSigningService",
                    $"[MySign] Exception: {ex.Message}");
                errorMessages.Add($"[MySign] Lỗi ngoại lệ: {ex.Message}");
                responseModel.ErrorMessages = errorMessages;
            }
            finally
            {
                TryDelete(tempPdfPath);
            }

            return responseModel;
        }

        // =====================================================================================
        // PRIVATE HELPERS
        // =====================================================================================

        /// <summary>
        /// Kiem tra PDF co chua chu ky so hop le (da duoc nhung /ByteRange) hay chua.
        /// Dung de quyet dinh dung append=false (chua ky) hay append=true (da co chu ky truoc).
        /// </summary>
        private static bool PdfHasDigitalSignature(byte[] pdfBytes)
        {
            try
            {
                using (PdfReader reader = new PdfReader(pdfBytes))
                {
                    var sigNames = reader.AcroFields.GetSignatureNames();
                    return sigNames != null && sigNames.Count > 0;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Them revision moi vao PDF bang append=true (KHONG rewrite file cu).
        /// Dung khi PDF da co chu ky so truoc do can bao toan.
        ///
        /// Thay vi dung SetField (AcroForm) de dien ten - cach nay loi font Unicode trong append mode -
        /// ham nay ve text truc tiep len OverContent bang PdfContentByte voi font Arial EMBEDDED.
        /// Day la cach dam bao font tieng Viet hien thi dung ma khong phu thuoc vao AcroForm machinery.
        /// </summary>
        private static byte[] BuildAppendPdf(
            byte[] rawPdfBytes, string nameFieldName, string signerName, string sigFieldToRemove)
        {
            using (var ms = new MemoryStream())
            {
                using (PdfReader reader = new PdfReader(rawPdfBytes))
                {
                    PdfStamper stamper = new PdfStamper(reader, ms, '\0', append: true);
                    try
                    {
                        AcroFields form = stamper.AcroFields;

                        // Xoa Sig_ field (chi dung cho Viettel)
                        if (!string.IsNullOrWhiteSpace(sigFieldToRemove)
                            && form.GetField(sigFieldToRemove) != null)
                        {
                            form.RemoveField(sigFieldToRemove);
                        }

                        SetNameField(form, nameFieldName, signerName, stamper);
                    }
                    finally
                    {
                        stamper.Close();
                    }
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Ghi lai PDF sach (append=false, khong co revision history), tuy chon:
        ///   - Dien ten vao Name_ field voi font Arial Unicode (ho tro tieng Viet).
        ///   - Xoa Sig_ field neu sigFieldToRemove != null (dung cho Viettel).
        ///
        /// Ly do can xoa Sig_ field khi ky Viettel:
        ///   PDF duoc tao bang PdfWriter.AddAnnotation nen Sig_ field chi la widget annotation
        ///   thong thuong, KHONG co /ByteRange placeholder chuan. Neu de nguyen, thu vien Viettel
        ///   tinh byte-range sai → chu ky INVALID. Phai xoa de Viettel tu tao lai dung chuan.
        ///
        /// Ly do dung append=false:
        ///   File co revision history (tu cac lan append truoc) se khien byte-range tinh sai,
        ///   hien thi Date: 1601/01/01. append=false tao ra 1 revision duy nhat, byte-range chinh xac.
        /// </summary>
        private static byte[] BuildCleanPdf(
            byte[] rawPdfBytes, string nameFieldName, string signerName, string sigFieldToRemove)
        {
            using (var ms = new MemoryStream())
            {
                using (PdfReader reader = new PdfReader(rawPdfBytes))
                {
                    PdfStamper stamper = new PdfStamper(reader, ms); // append=false
                    try
                    {
                        AcroFields form = stamper.AcroFields;

                        if (!string.IsNullOrWhiteSpace(sigFieldToRemove)
                            && form.GetField(sigFieldToRemove) != null)
                        {
                            form.RemoveField(sigFieldToRemove);
                        }

                        SetNameField(form, nameFieldName, signerName, stamper);
                    }
                    finally
                    {
                        stamper.Close();
                    }
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Dien ten vao AcroField kieu Text voi font Arial Unicode (ho tro tieng Viet).
        /// embedFont=true  : append=false - PDF rewrite hoan toan, font embed va AP stream chinh xac.
        /// embedFont=false : append=true  - set DA string truc tiep de viewer tu tim font he thong,
        ///                   tranh van de font resource khong duoc copy dung giua cac revision.
        /// </summary>
        private static void SetNameField(AcroFields form, string fieldName, string value,
            PdfStamper stamper = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName) || form.GetField(fieldName) == null)
                return;

            // Xoa READ_ONLY flag de SetField co the ghi duoc
            var fieldItem = form.GetFieldItem(fieldName);
            if (fieldItem != null)
            {
                for (int i = 0; i < fieldItem.Size; i++)
                {
                    PdfDictionary widget = fieldItem.GetWidget(i);
                    if (widget == null) continue;
                    PdfNumber ffNum = widget.GetAsNumber(PdfName.FF);
                    int ff = ffNum != null ? ffNum.IntValue : 0;
                    widget.Put(PdfName.FF, new PdfNumber(ff & ~1));
                }
            }

            form.SetField(fieldName, value);
            form.SetFieldProperty(fieldName, "setfflags", TextField.READ_ONLY, null);

            // Tao AP stream thu cong voi font Arial va gan vao widget.
            // GenerateAppearances trong append mode khong dam bao AP duoc flush.
            if (stamper != null && fieldItem != null && File.Exists(ArialFontPath))
            {
                for (int i = 0; i < fieldItem.Size; i++)
                {
                    PdfDictionary widget = fieldItem.GetWidget(i);
                    if (widget == null) continue;

                    PdfArray rectArray = widget.GetAsArray(PdfName.RECT);
                    if (rectArray == null || rectArray.Size < 4) continue;

                    float llx = rectArray.GetAsNumber(0).FloatValue;
                    float lly = rectArray.GetAsNumber(1).FloatValue;
                    float urx = rectArray.GetAsNumber(2).FloatValue;
                    float ury = rectArray.GetAsNumber(3).FloatValue;
                    float w   = Math.Abs(urx - llx);
                    float h   = Math.Abs(ury - lly);
                    if (w <= 0 || h <= 0) continue;

                    BaseFont bf       = BaseFont.CreateFont(ArialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    const float fs    = 9f;

                    PdfAppearance ap = PdfAppearance.CreateAppearance(stamper.Writer, w, h);
                    // Nen trang
                    ap.SetColorFill(BaseColor.WHITE);
                    ap.Rectangle(0, 0, w, h);
                    ap.Fill();
                    // Ve text can giua
                    ap.BeginText();
                    ap.SetFontAndSize(bf, fs);
                    ap.SetColorFill(BaseColor.BLACK);
                    float tw = bf.GetWidthPoint(value, fs);
                    ap.SetTextMatrix((w - tw) / 2f, (h - fs) / 2f);
                    ap.ShowText(value);
                    ap.EndText();

                    PdfDictionary apDict = new PdfDictionary();
                    apDict.Put(PdfName.N, ap.IndirectReference);
                    widget.Put(PdfName.AP, apDict);
                }
            }
            else
            {
                // Fallback neu khong co stamper
                form.GenerateAppearances = true;
            }
        }

        /// <summary>
        /// Lay vi tri (toa do, kich thuoc, so trang) cua AcroField tu file PDF tren disk.
        /// Tra ve null neu khong tim thay field.
        /// </summary>
        private static FieldRect GetFieldPosition(string filePath, string fieldName)
        {
            using (PdfReader reader = new PdfReader(filePath))
            {
                return ReadFieldRect(reader.AcroFields, fieldName);
            }
        }

        /// <summary>
        /// Lay vi tri cua AcroField tu byte array PDF (khong can ghi ra disk).
        /// </summary>
        private static FieldRect GetFieldPositionFromBytes(byte[] pdfBytes, string fieldName)
        {
            using (PdfReader reader = new PdfReader(pdfBytes))
            {
                return ReadFieldRect(reader.AcroFields, fieldName);
            }
        }

        private static FieldRect ReadFieldRect(AcroFields acroFields, string fieldName)
        {
            var positions = acroFields.GetFieldPositions(fieldName);
            if (positions == null || positions.Count == 0) return null;
            var p = positions[0];
            return new FieldRect
            {
                Left   = p.position.Left,
                Bottom = p.position.Bottom,
                Width  = p.position.Width,
                Height = p.position.Height,
                Page   = p.page
            };
        }

        /// <summary>
        /// Xay dung msisdn tu so dien thoai Viet Nam (loai bo so 0 dau, them prefix 84).
        /// </summary>
        private static string BuildMsisdn(string phoneNumber)
        {
            const string prefix = "84";
            if (string.IsNullOrWhiteSpace(phoneNumber)) return prefix;
            return prefix + phoneNumber.TrimStart('0');
        }

        /// <summary>
        /// Loai bo data URI prefix khi co (vd: "data:image/png;base64,...").
        /// </summary>
        private static string StripDataUriPrefix(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return base64;
            int comma = base64.IndexOf(',');
            return comma >= 0 ? base64.Substring(comma + 1) : base64;
        }

        /// <summary>
        /// Xoa file an toan, bo qua loi neu file khong ton tai.
        /// </summary>
        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch { /* bo qua loi xoa file tam */ }
        }

        private ResponseModel<bool> ErrorResponse(List<string> errorMessages, string message)
        {
            errorMessages.Add(message);
            return new ResponseModel<bool>
            {
                Data         = false,
                ErrorMessages = errorMessages,
                StatusCode   = (int)HttpStatusCode.BadRequest
            };
        }

        // DTO noi bo de truyen toa do field
        private class FieldRect
        {
            public float Left   { get; set; }
            public float Bottom { get; set; }
            public float Width  { get; set; }
            public float Height { get; set; }
            public int   Page   { get; set; }
        }
    }
}
