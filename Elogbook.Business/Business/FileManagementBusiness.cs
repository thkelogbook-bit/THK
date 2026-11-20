using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    /// <summary>
    /// Quản lý tệp tin
    /// </summary>
    public class FileManagementBusiness : IFileManagementBusiness
    {
        private readonly IFileManagementService _fileManagementService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileManagementService"></param>
        public FileManagementBusiness(IFileManagementService fileManagementService)
        {
            _fileManagementService = fileManagementService;
        }

        /// <summary>
        /// tải file về local
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public HttpResponseMessage DownloadFile(int id)
        {
            try
            {
                return _fileManagementService.DownloadFile(id);

            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FileManagement.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// lưu file từ local lên server
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="filePath"></param>
        /// <param name="name"></param>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public UploadFileModel UploadFile(string localFileName, string filePath, string filePathThumbnail, string name, string screenName, string fileType, string fileExtension, bool isLogoFactory, string moduleValue = null)
        {
            try
            {
                return _fileManagementService.UploadFile(localFileName, filePath, filePathThumbnail, name, screenName, fileType, fileExtension, isLogoFactory, moduleValue);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FileManagement.ToString(),
                    Environment.NewLine + $"filePath: {filePath} " + Environment.NewLine + ex.ToString());
                return new UploadFileModel()
                {
                    Description = ex.Message,
                    IsError = true
                };
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteFile(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _fileManagementService.DeleteFile(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FileManagement.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Zip file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileNames"></param>
        /// <param name="zipFileName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<UploadFileModel> ZipFiles(string filePath, List<string> fileNames, string zipFileName, int userID)
        {
            ResponseModel<UploadFileModel> responseModel = new ResponseModel<UploadFileModel>();

            try
            {
                responseModel.Data = _fileManagementService.ZipFiles(filePath, fileNames, zipFileName, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FileManagement.ToString(),
                    Environment.NewLine + $"filePath: {filePath} " + $"fileNames: {fileNames} " + $"zipFileName: {zipFileName} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
