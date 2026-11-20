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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý thông tin đợt sửa chữa lớn
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/majorRepair")]
    public class MajorRepairController : BaseApiController
    {
        private readonly IMajorRepairBusiness _majorRepairBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="majorRepairBusiness"></param>
        /// <param name="commonBusiness"></param>
        public MajorRepairController(IMajorRepairBusiness majorRepairBusiness,
            ICommonBusiness commonBusiness)
        {
            _majorRepairBusiness = majorRepairBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm thông tin đợt SCL theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSCL
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetMajorRepairs([FromBody] MajorRepairSearchModel requestModel)
        {
            return _majorRepairBusiness.GetMajorRepairs(requestModel);
        }

        /// <summary>
        /// Tìm kiếm thông tin đợt SCL theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSCL
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<MajorRepairModel> GetMajorRepairById([FromUri] int id)
        {
            return _majorRepairBusiness.GetMajorRepairById(id);
        }

        /// <summary>
        /// Thêm mới thông tin đợt SCL
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<int> CreateMajorRepair([FromBody] MajorRepairAddEditModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLSCL)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _majorRepairBusiness.CreateMajorRepair(requestModel);
        }

        /// <summary>
        /// Cập nhập thông tin đợt SCL
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateMajorRepair([FromBody] MajorRepairAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLSCL)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _majorRepairBusiness.UpdateMajorRepair(requestModel);
        }

        /// <summary>
        /// Xóa thông tin đợt SCL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteMajorRepair([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLSCL)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _majorRepairBusiness.DeleteMajorRepair(id, CurrentUser.ID);
        }

        /// <summary>
        /// Upload file tài liệu thông tin đợt SCL lên server
        /// </summary>
        /// <returns></returns>
        [Route("importWorkTicket")]
        [HttpPost]
        public async Task<ResponseModel<MajorRepairExcelDataValidationModel>> UploadFile([FromUri] int majorRepairID)
        {
            ResponseModel<MajorRepairExcelDataValidationModel> responseModel = new ResponseModel<MajorRepairExcelDataValidationModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLSCL)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            try
            {
                var excelPath = Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, CommonConstant.MAJOR_REPAIR);
                var provider = new MultipartFormDataStreamProvider(Path.Combine(excelPath));
                string timeLable = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (!Directory.Exists(excelPath))
                {
                    Directory.CreateDirectory(excelPath);
                }

                await Request.Content.ReadAsMultipartAsync(provider);
                if (provider.FileData.Count > 0)
                {
                    var file = provider.FileData[0];
                    var fileName = file.Headers.ContentDisposition.FileName;
                    string[] arrFileName = file.Headers.ContentDisposition.FileName.Trim().Split('.');
                    var fileExtension = arrFileName[arrFileName.Length - 1].Trim('"').ToLower();

                    if (!fileExtension.Equals("xlsx"))
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(), CommonConstant.XLSX_EXTENTION_REQUIRED);
                        responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Data = null;
                        responseModel.ErrorMessages.Add(CommonConstant.XLSX_EXTENTION_REQUIRED);
                        return responseModel;
                    }
                    fileName = fileName.Remove(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
                    fileName = System.Text.RegularExpressions.Regex.Replace(fileName.Trim('"'), @"[\W]+", "_") + "_" + timeLable + "." + fileExtension;
                    var localFileName = file.LocalFileName;

                    MajorRepairUploadInfoModel requestModel = new MajorRepairUploadInfoModel()
                    {
                        MajorRepairID = majorRepairID,
                        FileName = fileName,
                        LocalFileName = localFileName,
                        CurrentUserID = CurrentUser.ID,
                        FilePath = Path.Combine(excelPath, fileName)
                    };
                    return _majorRepairBusiness.UploadFile(requestModel);
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(), ex.ToString());
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = null;
                responseModel.ErrorMessages.Add(ex.Message);
                return responseModel;
            }

            return responseModel;
        }

        /// <summary>
        /// Tải file dữ liệu thông tin SCL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("downloadFile")]
        [HttpGet]
        public HttpResponseMessage DownloadFile([FromUri] int id)
        {
            return _majorRepairBusiness.DownloadFile(id);
        }

        /// <summary>
        /// Lấy trạng thái các đợt SCL
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSCL
        [Route("getMajorRepairStatus")]
        [HttpGet]
        public List<ValueLabelModel> GetMajorRepairStatus()
        {
            return _majorRepairBusiness.GetMajorRepairStatus();
        }

        /// <summary>
        /// Trả về tất cả phiếu công tác thuộc đợt công tác SCL theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSCL
        [Route("findAllWorkTickets")]
        [HttpPost]
        public ResponseBaseModel FindAllWorkTickets([FromBody] MajorRepairSearchWorkTicketModel requestModel)
        {
            return _majorRepairBusiness.FindAllWorkTickets(requestModel);
        }

        /// <summary>
        /// Trả về trạng thái phiếu công tác thuộc đợt SCL
        /// </summary>
        /// <param name="ticketType">Loại phiếu công tác (TCNH,PCTD)</param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSCL
        [Route("getMajorRepairTicketStatus")]
        [HttpGet]
        public List<ValueLabelCheckedModel> GetMajorRepairTicketStatus(string ticketType = "PCTD")
        {
            return _majorRepairBusiness.GetMajorRepairTicketStatus(ticketType);
        }

        /// <summary>
        /// Tải file mẫu thông tin đợt SCL
        /// </summary>
        /// <returns>File</returns>
        /// EnumUserPermission.VIEW_QLSCL
        [Route("downloadFileTemplate")]
        [HttpGet]
        public HttpResponseMessage DownloadFileTemplate()
        {
            return _majorRepairBusiness.DownloadFileTemplate();
        }

        /// <summary>
        /// Lấy danh sách title sửa chữa lớn
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLSCL
        [Route("getTitleMajorRepair")]
        [HttpGet]
        public ResponseBaseModel GetTitleMajorRepair([FromUri] int factoryID)
        {
            return _majorRepairBusiness.GetTitleMajorRepair(factoryID);
        }

        /// <summary>
        /// Cấp phiếu hàng loạt
        /// </summary>
        [Route("createWorkTicketAll")]
        [HttpPost]
        public int InsertWorkTicketFromSchedule()
        {
            return _majorRepairBusiness.InsertWorkTicketFromSchedule();
        }

        /// <summary>
        /// Cập nhập tạo phiếu sau khi hoàn thành cấp phiếu hàng loạt
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("insertNewTickets")]
        [HttpPut]
        public ResponseModel<bool> InsertNewTicket([FromBody] InsertNewTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _majorRepairBusiness.InsertMultipleTicket(requestModel);
        }

        /// <summary>
        /// Xóa phiếu công tác điện, phiếu thủy cơ nhiệt hóa của đợt sửa chữa lớn
        /// </summary>
        /// <returns></returns>
        [Route("deleteTicket")]
        [HttpDelete]
        public ResponseModel<bool> DeleteTicket([FromUri] int workTicketID)
        {
            return _majorRepairBusiness.DeleteTicket(workTicketID, CurrentUser.ID);
        }

        /// <summary>
        /// Cập nhật phiếu công tác điện, phiếu thủy cơ nhiệt hóa của đợt sửa chữa lớn
        /// </summary>
        /// <returns></returns>
        [Route("updateTicket")]
        [HttpPut]
        public ResponseModel<bool> UpdateTicket([FromBody] MajorRepairUpdateWorkTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _majorRepairBusiness.UpdateTicket(requestModel);
        }

        /// <summary>
        /// Lấy danh sách tracking thao tác SCL
        /// </summary>
        /// <returns></returns>
        [Route("getMajorRepairHistories")]
        [HttpPost]
        public ResponseBaseModel GetMajorRepairHistories([FromBody] MajorRepairHistorySearchModel requestModel)
        {
            return _majorRepairBusiness.GetMajorRepairHistories(requestModel);
        }
    }
}
