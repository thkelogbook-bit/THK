using DocumentFormat.OpenXml.Packaging;
using Elogbook.Core.Constants;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Elogbook.Core
{
    /// <summary>
    /// Triển khai các kỹ thuật để kiểm trả file trước khi upload vào server
    /// </summary>
    public static class FileHelper
    {
        //m4a, mov: not found
        //flv, mp4, wmv, mkv, 3gp, png, jpeg, jpg, xlsx, docx, xls, doc, pdf

        private readonly static byte[][] _flvSignature = new byte[][]
        {
            new byte[] { 0x46, 0x4c, 0x56 }
        };

        private readonly static byte[][] _mp4iosSignature = new byte[][]
        {
            new byte[] { 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D }
        };

        private readonly static byte[][] _mp4Signature = new byte[][]
        {
            new byte[] { 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56 }
        };

        private readonly static byte[][] _mkvSignature = new byte[][]
        {
            new byte[] { 0x1A, 0x45, 0xDF, 0xA3 }
        };

        private readonly static byte[][] _3gpSignature = new byte[][]
        {
            new byte[] { 0x66, 0x74, 0x79, 0x70, 0x33, 0x67 }
        };

        private readonly static byte[][] _pngSignature = new byte[][]
        {
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
        };

        private readonly static byte[][] _jpegjpg1Signature = new byte[][]
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }
        };

        private readonly static byte[][] _jpegjpg2Signature = new byte[][]
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 },
            new byte[] { 0x49, 0x46, 0x00, 0x01 }
        };

        private readonly static byte[][] _jpegjpg3Signature = new byte[][]
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xEE }
        };

        private readonly static byte[][] _jpgSignature = new byte[][]
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0  }
        };

        private readonly static byte[][] _xlsxSignature = new byte[][]
        {
            new byte[] { 0x50, 0x4B, 0x03, 0x04 },
            new byte[] { 0x50, 0x4B, 0x05, 0x06 },
            new byte[] { 0x50, 0x4B, 0x07, 0x08 }
        };

        private readonly static byte[][] _docxSignature = new byte[][]
        {
            new byte[] { 0x50, 0x4B, 0x03, 0x04 },
            new byte[] { 0x50, 0x4B, 0x05, 0x06 },
            new byte[] { 0x50, 0x4B, 0x07, 0x08 }
        };

        private readonly static byte[][] _xlsSignature = new byte[][]
        {
            new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
        };

        private readonly static byte[][] _docSignature = new byte[][]
        {
            new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
        };

        private readonly static byte[][] _pdfSignature = new byte[][]
        {
            new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }
        };

        private readonly static byte[][] _wmvSignature = new byte[][]
        {
            new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 },
            new byte[] { 0xA6, 0xD9, 0x00, 0xAA, 0x62, 0xCE, 0x6C}
        };

        /// <summary>
        /// 1. Tự động làm sạch các ký tự không cho phép trong tên tệp tin
        /// 2. Kiểm tra định dạng các đuôi tệp tin cho phép
        /// 3. Kiểm tra dung lượng tối thiểu tệp tin upload cho phép
        /// 4. Kiểm tra định dạng tệp tin File Signature: "flv,m4a,mkv,3gp,png,jpeg,jpg,xlsx,docx,xls,doc,pdf";
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileZise"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public static string ValidationFileUpload(ref string fileName, long fileZise, string localFilePath)
        {
            var errorMessage = string.Empty;

            try
            {
                //1. Tự động làm sạch các ký tự không cho phép trong tên tệp tin
                fileName = CleanedFileName(fileName);
                if (Path.GetFileNameWithoutExtension(fileName).Length > CommonConstant.FILENAME_LENGTH)
                {
                    return string.Format(CommonConstant.FILENAME_LENGTH_ERROR, CommonConstant.FILENAME_LENGTH);
                }

                //2. Kiểm tra định dạng các đuôi tệp tin cho phép
                var fileExtention = Path.GetExtension(fileName).Replace(".", "").ToLower();
                var fileExtentions = CommonConstant.ALLOWED_FILE_EXTENTION.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < fileExtentions.Length; i++)
                {
                    fileExtentions[i] = fileExtentions[i].Trim();
                }

                if (!fileExtentions.Contains(fileExtention))
                {
                    return string.Format(CommonConstant.FILE_EXTENTION_ERROR, CommonConstant.ALLOWED_FILE_EXTENTION);
                }

                //3. Kiểm tra dung lượng tối thiểu tệp tin upload cho phép
                if (fileZise > CommonConstant.ALLOWED_FILESIZE)
                {
                    return string.Format(CommonConstant.FILE_SIZE_ERROR, CommonConstant.ALLOWED_FILESIZE / 1024 / 1024);
                }

                //4. Kiểm tra định dạng tệp tin File Signature
                if (!string.IsNullOrWhiteSpace(CommonConstant.ALLOWED_FILE_EXTENTION))
                {
                    var fileExtentionTemp = $",{fileExtention},";
                    switch (fileExtentionTemp.ToLower())
                    {
                        case ",flv,":
                            if (!IsValidFileSignature(localFilePath, _flvSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",mp4,":
                            if (!IsValidFileSignature(localFilePath, _mp4iosSignature)
                                && !IsValidFileSignature(localFilePath, _mp4Signature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",mkv,":
                            if (!IsValidFileSignature(localFilePath, _mkvSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",3gp,":
                            if (!IsValidFileSignature(localFilePath, _3gpSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",png,":
                            if (!IsValidFileSignature(localFilePath, _pngSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",jpeg,":
                            if (!IsValidFileSignature(localFilePath, _jpegjpg1Signature)
                                && !IsValidFileSignature(localFilePath, _jpegjpg2Signature)
                                && !IsValidFileSignature(localFilePath, _jpegjpg3Signature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",jpg,":
                            if (!IsValidFileSignature(localFilePath, _jpgSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",xls,":
                            if (!IsValidFileSignature(localFilePath, _xlsSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",doc,":
                            if (!IsValidFileSignature(localFilePath, _docSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",xlsx,":
                            if (!IsValidFileSignature(localFilePath, _xlsxSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",docx,":
                            if (!IsValidFileSignature(localFilePath, _docxSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",pdf,":
                            if (!IsValidFileSignature(localFilePath, _pdfSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        case ",wmv,":
                            if (!IsValidFileSignature(localFilePath, _wmvSignature))
                            {
                                errorMessage = string.Format(CommonConstant.FILE_SIGNATURE_ERROR, fileExtention);
                            }
                            break;
                        default:
                            errorMessage = string.Empty;
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(errorMessage))
                    {
                        return errorMessage;
                    }
                }

                //Lọc các thẻ: javascript:,<</type/action/s/submitform/f,/s/launch/win,function(),<%@ include,<%@ page,<</type/action/s/java,<</S/JavaScript/JS,/S/URI/URI
                //Lọc các thẻ (?i:<!ENTITY|<!DOCTYPE|<!ELEMENT|<script|<include|xlink\:|xliff|XMLDecoder)
                switch (fileExtention.ToLower())
                {
                    case "doc":
                    case "docx":
                    case "xls":
                    case "xlsx":
                    case "pdf":
                        var fileContent = string.Empty;
                        using (FileStream fs = new FileStream(localFilePath, FileMode.Open))
                        {
                            using (StreamReader reader = new StreamReader(fs as Stream, Encoding.UTF8))
                            {
                                fileContent = reader.ReadToEnd();
                            }
                            fs.Close();
                        }

                        errorMessage = CheckFileContainSpecificTags(fileContent);
                        if (!string.IsNullOrWhiteSpace(errorMessage))
                        {
                            return errorMessage;
                        }
                        break;
                }

                //Đối với file .doc .docx cần thực hiện check TargetMode=”External”, OLEObject,  UpdateMode=""OnCall"" file document.xml.rels và document.xml trong cấu trúc XML Microsoft Office"
                switch (fileExtention.ToLower())
                {
                    case "doc":
                    case "docx":
                        errorMessage = CheckDocumentXml(localFilePath);
                        if (!string.IsNullOrWhiteSpace(errorMessage))
                        {
                            return errorMessage;
                        }
                        break;
                }
            }
            catch
            {
                errorMessage = CommonConstant.FILE_UPLOAD_ERROR;
            }

            return errorMessage;
        }

        private static bool IsValidFileSignature(string localFilePath, byte[][] allowedFileSignature)
        {
            try
            {
                using (FileStream fs = new FileStream(localFilePath, FileMode.Open))
                {
                    var maxFirstByte = allowedFileSignature.Max(x => x.Length);
                    byte[] buffer = new byte[maxFirstByte];

                    Stream uploadedFileStream = fs as Stream;
                    if (uploadedFileStream.CanRead && uploadedFileStream.Length > 0)
                    {
                        uploadedFileStream.Position = 0;
                        var bytes_read = uploadedFileStream.Read(buffer, 0, maxFirstByte);
                        if ((bytes_read != buffer.Length) || (allowedFileSignature.All(x => !x.SequenceEqual(buffer))))
                        {
                            return false;
                        }

                        return true;
                    }

                    fs.Close();
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Clean các ký tự đặc biệt không cho phép lưu file trên Windows
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string CleanedFileName(string fileName)
        {
            return Regex.Replace(fileName, @"[\\/:*?""<>|]", "");
        }

        /// <summary>
        /// Kiểm tra nội dung file có chứa các thẻ mã độc
        /// javascript:,<</type/action/s/submitform/f,/s/launch/win,function(),<%@ include,<%@ page,<</type/action/s/java,<</S/JavaScript/JS,/S/URI/URI
        /// (?i:<!ENTITY|<!DOCTYPE|<!ELEMENT|<script|<include|xlink\:|xliff|XMLDecoder)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string CheckFileContainSpecificTags(string content)
        {
            string[] patterns = {
                "javascript:",
                @"<</type/action/s/submitform/f",
                @"/s/launch/win",
                "function\\(\\)",
                "<%@ include",
                "<%@ page",
                "<</type/action/s/java",
                @"<</S/JavaScript/JS",
                @"/S/URI/URI",
                "<!ENTITY"
            };

            foreach (var pattern in patterns)
            {
                string escapedPattern = Regex.Escape(pattern);
                Regex regex = new Regex(escapedPattern);

                var isMatch = regex.IsMatch(content);
                if (isMatch)
                {
                    return $"{CommonConstant.MALICIOUS_CODE}: {pattern}.";
                }
            }

            string regexPattern = @"(?i:<!ENTITY|<!DOCTYPE|<!ELEMENT|<script|<include|xlink\:|xliff|XMLDecoder)";
            if (Regex.IsMatch(content, regexPattern))
            {
                return $"{CommonConstant.MALICIOUS_CODE}: <!ENTITY|<!DOCTYPE|<!ELEMENT|<script|<include|xlink\\:|xliff|XMLDecoder.";
            }

            return "";
        }

        private static string ExtractWordXmlContent(Stream uploadedFileStream)
        {
            // Mở tài liệu Word bằng Open XML SDK
            using (WordprocessingDocument doc = WordprocessingDocument.Open(uploadedFileStream, false))
            {
                // Trích xuất nội dung XML từ tài liệu Word
                MainDocumentPart mainPart = doc.MainDocumentPart;
                if (mainPart != null)
                {
                    using (var stream = mainPart.GetStream())
                    using (var reader = XmlReader.Create(stream))
                    {
                        // Đọc và trả về nội dung XML dưới dạng chuỗi
                        return reader.ReadOuterXml();
                    }
                }
            }

            return null;
        }

        private static string CheckDocumentXml(string localFilePath)
        {
            try
            {
                using (FileStream fs = new FileStream(localFilePath, FileMode.Open))
                {
                    var xmlContent = ExtractWordXmlContent(fs as Stream);
                    if (!string.IsNullOrWhiteSpace(xmlContent))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(xmlContent);

                        XmlNodeList targetModeExternalNodes = xmlDocument.SelectNodes("//TargetMode[@Val='External']");
                        if (targetModeExternalNodes.Count > 0)
                        {
                            return $"{CommonConstant.MALICIOUS_CODE}.";
                        }

                        XmlNodeList oleObjectNodes = xmlDocument.SelectNodes("//OLEObject");
                        if (oleObjectNodes.Count > 0)
                        {
                            return $"{CommonConstant.MALICIOUS_CODE}.";
                        }

                        XmlNodeList updateModeOnCallNodes = xmlDocument.SelectNodes("//UpdateMode[@Val='OnCall']");
                        if (updateModeOnCallNodes.Count > 0)
                        {
                            return $"{CommonConstant.MALICIOUS_CODE}.";
                        }
                    }

                    fs.Close();
                }
            }
            catch
            {
                //ignore error
            }

            return "";
        }

    }
}
