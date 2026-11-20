using Elogbook.Core;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using Elogbook.Data.DataModels.Entities.BackupData;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Quản lý tập tin
    /// </summary>
    public class FileManagementService : IFileManagementService
    {
        private const string MP4_EXTENTION = ".mp4";
        private const string MOV_EXTENTION = ".mov";
        private const string ZIP_FILE = "zip";

        private readonly ElogbookUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public FileManagementService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage DownloadFile(int id)
        {
            // Lấy data cần tải
            var data = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(e => new AttachmentModel()
            {
                FilePath = e.FilePath,
                PathThumbnail = e.PathThumbnail,
                FileName = e.FileName,
                FileType = e.FileType,
            }).FirstOrDefault();

            if (data != null)
            {
                var fileBytes = File.ReadAllBytes(AppSettings.FILE_STORAGE_SERVER_DIR + data.FilePath);
                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), Path.GetFileName(data.FilePath));
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected HttpResponseMessage Download(HttpContent content, string fileName = "")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return response;
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="filePath"></param>
        /// <param name="filePathThumbnail"></param>
        /// <param name="name"></param>
        /// <param name="screenName"></param>
        /// <param name="fileType"></param>
        /// <param name="fileExtension"></param>
        /// <param name="isLogoFactory"></param>
        /// <returns></returns>
        public UploadFileModel UploadFile(string localFileName, string filePath, string filePathThumbnail, string name, string screenName, string fileType, string fileExtension, bool isLogoFactory, string moduleValue = null)
        {
            UploadFileModel attachmentModel = new UploadFileModel();
            var path = isLogoFactory ? Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, "Logo") : AppSettings.FILE_STORAGE_SERVER_DIR;

            var prefix = string.Empty;
            if (!string.IsNullOrWhiteSpace(moduleValue))
            {
                prefix = moduleValue.Trim();
                path = Path.Combine(path, prefix);
            }
            if ("KNOWLEDGEBASE".Equals(screenName))
            {
                var year = DateTime.Now.Year;
                prefix = $"KnowledgeBase\\{year}";
                path = Path.Combine(path, prefix);
            }

            //move file main
            //if file type is mov from iOS then convert to mp4 for support read file on web
            var fileExtention = Path.GetExtension(filePath);
            if (MOV_EXTENTION.Equals(fileExtention, StringComparison.OrdinalIgnoreCase))
            {
                File.Move(localFileName, Path.ChangeExtension(Path.Combine(path, filePath), MP4_EXTENTION));
                fileType = "video/mp4";
                fileExtension = "mp4";
                name = name.Replace(MOV_EXTENTION, MP4_EXTENTION);
                filePath = filePath.Replace(MOV_EXTENTION, MP4_EXTENTION);
            }
            else
            {
                File.Move(localFileName, Path.Combine(path, filePath));
            }

            //copy file thumbnail
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/Icon");
            string[] files = Directory.GetFiles(root);

            // Copy the files and overwrite destination files if they already exist.
            List<string> fileExtensions = new List<string>() { "png", "jpg", "jpeg" };
            List<string> docPdfExtensions = new List<string>() { "doc", "docx", "pdf" };
            if (!fileExtensions.Contains(fileExtension))
            {
                if (docPdfExtensions.Contains(fileExtension))
                {
                    foreach (var s in from string s in files
                                      where s.Contains("docs.png")
                                      select s)
                    {
                        File.Copy(s, Path.Combine(path, filePathThumbnail), true);
                    }
                }
                else
                {
                    foreach (var s in from string s in files
                                      where s.Contains("video-icon.png")
                                      select s)
                    {
                        File.Copy(s, Path.Combine(path, filePathThumbnail), true);
                    }
                }
            }

            var attachment = new Attachment
            {
                FileName = name,
                FilePath = isLogoFactory ? @"Logo\" + Path.Combine(prefix, filePath) : Path.Combine(prefix, filePath),
                PathThumbnail = isLogoFactory ? @"Logo\" + Path.Combine(prefix, filePathThumbnail) : Path.Combine(prefix, filePathThumbnail),
                ScreenName = screenName,
                FileType = fileType,
                ModuleValue = moduleValue,
            };

            _unitOfWork.ElogbookRepository<Attachment>().Add(attachment);
            _unitOfWork.Complete();
            attachmentModel = new UploadFileModel
            {
                ID = attachment.ID,
                FilePath = AppSettings.SERVER_DIR + attachment.FilePath.Replace(@"Logo\", ""),
                PathThumbnail = AppSettings.SERVER_DIR + attachment.PathThumbnail.Replace(@"Logo\", ""),
                IsError = false,
                FileType = attachment.FileType
            };

            return attachmentModel;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool DeleteFile(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<Attachment>().GetById(id);
            if (entity != null)
            {
                // kiểm tra file đã tồn tại chưa và xoá
                var filePath = Path.Combine(AppSettings.SERVER_DIR, entity.FilePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // kiểm tra file Thumbnail đã tồn tại chưa và xoá
                var filePathThumbnail = Path.Combine(AppSettings.SERVER_DIR, entity.PathThumbnail);
                if (File.Exists(filePathThumbnail))
                {
                    File.Delete(filePathThumbnail);
                }

                _unitOfWork.ElogbookRepository<Attachment>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        /// <summary>
        /// Zip files
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileNames"></param>
        /// <param name="zipFileName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UploadFileModel ZipFiles(string filePath, List<string> fileNames, string zipFileName, int? userID)
        {
            UploadFileModel result = new UploadFileModel();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                result.ErrorMessage = "File path cannot be empty.";
                return result;
            }

            if (!Directory.Exists(filePath))
            {
                result.ErrorMessage = "Directory not found.";
                return result;
            }

            // Nếu danh sách rỗng → lấy tất cả file trong folder
            if (fileNames == null || fileNames.Count == 0)
                fileNames = new List<string>(Directory.GetFiles(filePath));

            // Đảm bảo danh sách file là đường dẫn đầy đủ
            var filesToZip = new List<string>();
            foreach (var name in fileNames)
            {
                string fullPath = Path.IsPathRooted(name) ? name : Path.Combine(filePath, name);
                if (File.Exists(fullPath))
                    filesToZip.Add(fullPath);
            }

            if (filesToZip.Count == 0)
            {
                result.ErrorMessage = "No files found to zip.";
                return result;
            }

            // Tạo file zip
            var path = Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, "BackupData");
            string zipFullPath = Path.Combine(path, zipFileName);
            if (File.Exists(zipFullPath))
                File.Delete(zipFullPath);
            string dir = Path.GetDirectoryName(zipFullPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var zip = ZipFile.Open(zipFullPath, ZipArchiveMode.Create))
            {
                foreach (var file in filesToZip)
                {
                    string entryName = Path.GetFileName(file);
                    zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                }
            }
            var attachment = new Attachment
            {
                FileName = zipFileName,
                FilePath = "BackupData\\" + zipFileName,
                ScreenName = "",
                FileType = ZIP_FILE,
            };

            _unitOfWork.ElogbookRepository<Attachment>().Add(attachment);
            _unitOfWork.Complete();
            var backupData = new BackupData
            {
                Name = zipFileName,
                Status = "Thành công",
                AttachmentId = attachment.ID,
            };
            _unitOfWork.ElogbookRepository<BackupData>().Add(backupData);
            _unitOfWork.Complete();
            //var fileInfo = new FileInfo(zipFullPath);
            return result;
        }
    }
}
