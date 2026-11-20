using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Controller for manage Download and Upload File
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/fileManagement")]
    public class FileManagementController : BaseApiController
    {
        private readonly IFileManagementBusiness _fileManagementBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileManagementBusiness"></param>
        public FileManagementController(IFileManagementBusiness fileManagementBusiness)
        {
            _fileManagementBusiness = fileManagementBusiness;
        }

        /// <summary>
        /// Tải tập tin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("download")]
        [HttpGet]
        public HttpResponseMessage DownloadFile([FromUri] int id)
        {
            return _fileManagementBusiness.DownloadFile(id);
        }

        /// <summary>
        /// Lưu tập tin lên server
        /// </summary>
        /// <param name="screenName">Tên màn hình (RegistrationTicket/phiếu đăng ký công tác,\n WorkTicket/quản lý phiếu công tác,\n OperationDiary: Nhật ký vận hành)</param>
        /// <param name="isLogoFactory"></param>
        /// <param name="moduleValue">Giá trị module xác định file thuộc module nào (không bắt buộc)</param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public async Task<List<UploadFileModel>> UploadFile([FromUri] string screenName, bool isLogoFactory = false, string moduleValue = null)
        {
            screenName = !string.IsNullOrWhiteSpace(screenName) ? screenName.Trim() : string.Empty;

            var root = isLogoFactory ? Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, "Logo") : AppSettings.FILE_STORAGE_SERVER_DIR;
            var lastdigit = root.Trim().LastOrDefault();

            if (!string.IsNullOrWhiteSpace(moduleValue))
            {
                root = Path.Combine(root, moduleValue.Trim());
            }

            if ("KNOWLEDGEBASE".Equals(screenName))
            {
                var year = DateTime.Now.Year;
                root = Path.Combine(root, $"KnowledgeBase\\{year}");
            }

            if (!lastdigit.Equals("\\") && !lastdigit.Equals("/"))
            {
                root += "/";
            }
            var responseModel = new List<UploadFileModel>();
            var provider = new MultipartFormDataStreamProvider(Path.Combine(root));
            var timeLable = $"_{DateTime.Now:yyyyMMddHHmmss}";
            try
            {
                // kiểm tra folder App_Data đã tồn tại chưa
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    var fileExtension = file.Headers.ContentDisposition.FileName.Trim().Split('.').Last().Trim('"').ToLower();
                    var name = file.Headers.ContentDisposition.FileName;
                    name = name.Remove(name.LastIndexOf('.'), name.Length - name.LastIndexOf('.'));
                    name = System.Text.RegularExpressions.Regex.Replace(name.Trim('"'), @"[\W]+", "_") + "." + fileExtension;
                    var fileType = CurrentUser.IsMobileSystem ? GetFileType(fileExtension) : file.Headers.ContentType.ToString();
                    var localFileName = file.LocalFileName;

                    string filePath = name.Insert(name.LastIndexOf('.'), timeLable);

                    var fileName = file.Headers.ContentDisposition.FileName;
                    long fileSizeInBytes = new FileInfo(localFileName).Length;
                    var errorMessage = FileHelperValidate.ValidationFileUpload(ref fileName,
                        fileSizeInBytes,
                        localFileName);

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        List<string> imageExtensions = new List<string>() { "png", "jpg", "jpeg" };
                        List<string> docPdfExtensions = new List<string>() { "doc", "docx", "pdf" };
                        var filePathThumbnail = "thumbnail" + name.Insert(name.LastIndexOf('.'), timeLable);
                        if (!imageExtensions.Contains(fileExtension))
                        {
                            if (docPdfExtensions.Contains(fileExtension))
                            {
                                filePathThumbnail = "thumbnail" + "docs.png";
                            }
                            else
                            {
                                filePathThumbnail = "thumbnail" + "video_icon.png";
                            }
                        }
                        var attachment = _fileManagementBusiness.UploadFile(localFileName, filePath, filePathThumbnail, name, screenName, fileType.ToLower(), fileExtension, isLogoFactory, moduleValue);

                        if (imageExtensions.Contains(fileExtension.ToLower()) && !attachment.IsError)
                        {
                            Image thumb = Image.FromFile(Path.Combine(root, filePath)).GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                            thumb.Save(Path.ChangeExtension(root + (isLogoFactory ? "\\" : "") + filePathThumbnail, fileExtension.ToLower()));
                        }
                        responseModel.Add(attachment);
                    }
                    else
                    {
                        File.Delete(localFileName);

                        UploadFileModel result = new UploadFileModel
                        {
                            IsError = true,
                            ErrorMessage = errorMessage,
                            Description = errorMessage,
                        };
                        responseModel.Add(result);

                        LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DownloadAndUpload.ToString(), errorMessage);
                        return responseModel;
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DownloadAndUpload.ToString(), ex.ToString());
                responseModel.Add(new UploadFileModel()
                {
                    Description = ex.Message,
                    IsError = true
                });
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("")]
        [HttpDelete]
        public ResponseModel<bool> DeleteFile([FromUri] int id)
        {
            return _fileManagementBusiness.DeleteFile(id, CurrentUser.ID);
        }

        private string GetFileType(string fileExtension)
        {
            string fileType = string.Empty;

            switch (fileExtension)
            {
                case "jpg":
                    fileType = "image/jpeg";
                    break;
                case "jpeg":
                    fileType = "image/jpeg";
                    break;
                case "png":
                    fileType = "image/png";
                    break;
                case "mp4":
                    fileType = "video/mp4";
                    break;
                case "mov":
                    fileType = "video/quicktime";
                    break;
                case "wmv":
                    fileType = "video/wmv";
                    break;
                case "docx":
                case "docm":
                    fileType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "xlsx":
                case "xlsm":
                    fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.";
                    break;
            }

            return fileType;
        }
    }
}
