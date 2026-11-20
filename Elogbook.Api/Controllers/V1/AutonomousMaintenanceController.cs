using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Bảo trì tự quản - AutonomousMaintenance
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/autonomousMaintenance")]
    public class AutonomousMaintenanceController : BaseApiController
    {
        private readonly IAutonomousMaintenanceBusiness _autonomousMaintenanceBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autonomousMaintenanceBusiness"></param>
        public AutonomousMaintenanceController(IAutonomousMaintenanceBusiness autonomousMaintenanceBusiness)
        {
            _autonomousMaintenanceBusiness = autonomousMaintenanceBusiness;
        }

        /// <summary>
        /// Tạo bảo trì tự quản 
        /// </summary>
        [Route("")]
        [HttpPost]
        public int CreateAutonomousMaintenance()
        {
            return _autonomousMaintenanceBusiness.CreateAutonomousMaintenance();
        }

        /// <summary>
        /// Tự động hoàn thành bảo trì tự quản 
        /// </summary>
        [Route("autoCompleteAm")]
        [HttpPost]
        public int AutoCompleteAm()
        {
            return _autonomousMaintenanceBusiness.AutoCompleteAm();
        }

        /// <summary>
        /// Mail nhắc lịch thực hiện hoạt động AM
        /// </summary>
        [Route("sendMailNotificationAm")]
        [HttpPost]
        public int SendMailNotificationAm()
        {
            return _autonomousMaintenanceBusiness.SendMailNotificationAm();
        }

        /// <summary>
        /// Tìm kiếm bảo trì tự quản theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLBTTQ
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetAutonomousMaintenances([FromBody] AutonomousMaintenanceSearchModel requestModel)
        {
            return _autonomousMaintenanceBusiness.GetAutonomousMaintenances(requestModel);
        }

        /// <summary>
        /// Tìm kiếm bảo trì tự theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLBTTQ
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AutonomousMaintenanceModel> GetAutonomousMaintenanceId([FromUri] int id)
        {
            return _autonomousMaintenanceBusiness.GetAutonomousMaintenanceById(id);
        }

        /// <summary>
        /// Không thực hiện bảo trì tự quản
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.NO_PROCESS_QLBTTQ
        [Route("createAmStopMaintenance")]
        [HttpPost]
        public ResponseModel<bool> CreateAmStopMaintenance([FromBody] AmStopMaintenanceAddEditModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _autonomousMaintenanceBusiness.CreateAmStopMaintenance(requestModel);
        }

        /// <summary>
        /// Thực hiện công việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.PROCESS_QLBTTQ
        [Route("workProcessing")]
        [HttpPut]
        public ResponseModel<AmWorkDetailModel> WorkProcessing([FromBody] AmWorkingProgressAddModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _autonomousMaintenanceBusiness.WorkProcessing(requestModel);
        }

        /// <summary>
        /// Get Thông tin chi tiết thực hiện công việc theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLBTTQ
        [Route("getAmWorkDetailById")]
        [HttpGet]
        public ResponseModel<AmWorkDetailModel> GetAmWorkDetailById([FromUri] int id)
        {
            return _autonomousMaintenanceBusiness.GetAmWorkDetailById(id);
        }

        /// <summary>
        /// Xem lý do dừng hoặc tạm dừng công việc
        /// </summary>
        /// <param name="amWorkDetailID"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLBTTQ
        [Route("getAmWorkPendingById")]
        [HttpGet]
        public ResponseModel<AmWorkPendingModel> GetAmWorkPendingById([FromUri] int amWorkDetailID)
        {
            return _autonomousMaintenanceBusiness.GetAmWorkPendingById(amWorkDetailID);
        }

        /// <summary>
        /// Cập nhật trạng thái bất thường thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.PROCESS_QLBTTQ
        [Route("updateDeviceAbnormal")]
        [HttpPut]
        public ResponseModel<bool> UpdateDeviceAbnormal([FromBody] AmDeviceAbnormalUpdateModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _autonomousMaintenanceBusiness.UpdateDeviceAbnormal(requestModel);
        }

        /// <summary>
        /// Ký hoàn thành bảo trì tự quản
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.PROCESS_QLBTTQ
        [Route("signatureCompleteAm")]
        [HttpPut]
        public ResponseModel<bool> SignatureCompleteAm([FromBody] AmSignatureCompleteModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _autonomousMaintenanceBusiness.SignatureCompleteAm(requestModel);
        }

        /// <summary>
        /// Xem lý do Không thực hiện bảo trì tự quản
        /// </summary>
        /// <param name="autonomousMaintenanceID"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLBTTQ
        [Route("getAmStopMaintenanceById")]
        [HttpGet]
        public ResponseModel<AmStopMaintenanceModel> GetAmStopMaintenanceById([FromUri] int autonomousMaintenanceID)
        {
            return _autonomousMaintenanceBusiness.GetAmStopMaintenanceById(autonomousMaintenanceID);
        }

        /// <summary>
        /// Lưu tập tin lên server
        /// </summary>
        /// <returns></returns>
        [Route("uploadFile")]
        [HttpPost]
        public async Task<List<UploadFileModel>> UploadFile()
        {
            var rootPath = Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, CommonConstant.AUTONOMOUS_MAINTENANCE);

            var responseModel = new List<UploadFileModel>();
            var provider = new MultipartFormDataStreamProvider(Path.Combine(rootPath));
            var timeLable = $"_{DateTime.Now:yyyyMMddHHmmss}";
            try
            {
                // kiểm tra folder App_Data đã tồn tại chưa
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    var fileName = file.Headers.ContentDisposition.FileName;
                    var fileExtension = file.Headers.ContentDisposition.FileName.Trim().Split('.')[1].Trim('"').ToLower();
                    fileExtension = "." + fileExtension;
                    fileName = fileName.Remove(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
                    fileName = System.Text.RegularExpressions.Regex.Replace(fileName.Trim('"'), @"[\W]+", "_") + timeLable + fileExtension;
                    var fileType = CurrentUser.IsMobileSystem ? GetFileType(fileExtension) : file.Headers.ContentType.ToString();
                    var localFileName = file.LocalFileName;
                    FileInfoModel fileInfoModel = new FileInfoModel()
                    {
                        LocalFileName = localFileName,
                        RootPath = rootPath,
                        FileName = fileName,
                        FileExtension = fileExtension,
                        FileType = fileType,
                        CurrentUserID = CurrentUser.ID
                    };

                    var attachment = _autonomousMaintenanceBusiness.UploadFile(fileInfoModel);

                    responseModel.Add(attachment);
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

        private string GetFileType(string fileExtension)
        {
            string fileType = string.Empty;

            switch (fileExtension)
            {
                case ".jpg":
                    fileType = "image/jpeg";
                    break;
                case ".jpeg":
                    fileType = "image/jpeg";
                    break;
                case ".png":
                    fileType = "image/png";
                    break;
                case ".mp4":
                    fileType = "video/mp4";
                    break;
                case ".mov":
                    fileType = "video/quicktime";
                    break;
                case ".wmv":
                    fileType = "video/wmv";
                    break;
            }

            return fileType;
        }

        /// <summary>
        /// lịch sử vận hành sửa chữa
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLBTTQ
        [Route("findHistory/{deviceId}")]
        [HttpGet]
        public ResponseBaseModel FindHistory([FromUri] int deviceId)
        {
            return _autonomousMaintenanceBusiness.FindHistory(deviceId);
        }

        /// <summary>
        /// Lưu hình ảnh trước và sau thực hiện bảo trì
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.PROCESS_QLBTTQ
        [Route("saveEvidenceAm")]
        [HttpPut]
        public ResponseBaseModel SaveEvidenceAm([FromBody] SaveImageInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _autonomousMaintenanceBusiness.SaveEvidenceAm(requestModel);
        }
    }
}
