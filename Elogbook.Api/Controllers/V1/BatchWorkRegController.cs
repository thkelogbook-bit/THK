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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý thông tin Phiếu đăng ký (đăng ký số lượng lớn)
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/batchWorkReg")]
    public class BatchWorkRegController : BaseApiController
    {
        private readonly IBatchWorkRegBusiness _batchWorkRegBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="batchWorkRegBusiness"></param>
        public BatchWorkRegController(IBatchWorkRegBusiness batchWorkRegBusiness)
        {
            _batchWorkRegBusiness = batchWorkRegBusiness;
        }

        /// <summary>
        /// Tải file mẫu Phiếu đăng ký Điện
        /// </summary>
        /// <returns>File</returns>
        [Route("downloadFileTemplateDien")]
        [HttpGet]
        [Permission(EnumUserPermission.VIEW_ĐKCTĐ)]
        public HttpResponseMessage DownloadFileTemplateDien()
        {
            return _batchWorkRegBusiness.DownloadFileTemplate("Dien");
        }

        /// <summary>
        /// Tải file mẫu Phiếu đăng ký Thủy cơ nhiệt hóa
        /// </summary>
        /// <returns>File</returns>
        [Route("downloadFileTemplateTcnh")]
        [HttpGet]
        [Permission(EnumUserPermission.VIEW_ĐKCTTCNH)]
        public HttpResponseMessage DownloadFileTemplateTcnh()
        {
            return _batchWorkRegBusiness.DownloadFileTemplate("TCNH");
        }

        /// <summary>
        /// Upload file chứa thông tin phiếu đăng ký, hệ thống sẽ thực hiện kiểm tra dữ liệu trong file
        /// Nếu dữ liệu có lỗi thì trả thông báo lỗi về cho người dùng
        /// Nếu dữ liệu hoàn toàn hợp lệ sẽ gọi chức năng CreateBatchWorkReg để đưa thông tin vào các bảng tạm trong hệ thống
        /// Sau đó job sẽ chạy theo lịch trình và dựa trên thời gian tạo phiếu đăng ký để trigger việc tạo phiếu
        /// </summary>
        /// <returns></returns>
        [Route("importWorkRegist")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_ĐKCTĐ, EnumUserPermission.ADD_ĐKCTTCNH)]
        public async Task<ResponseModel<WorkRegExcelDataValidationModel>> UploadFile([FromUri] int bacthWorkRegID = 0, int regisType = 0)
        {
            ResponseModel<WorkRegExcelDataValidationModel> responseModel = new ResponseModel<WorkRegExcelDataValidationModel>();

            try
            {
                var excelPath = Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, CommonConstant.BATCH_WORK_REG);
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
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BatchWorkRegBusiness.ToString(), CommonConstant.XLSX_EXTENTION_REQUIRED);
                        responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Data = null;
                        responseModel.ErrorMessages.Add(CommonConstant.XLSX_EXTENTION_REQUIRED);
                        return responseModel;
                    }
                    fileName = fileName.Remove(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
                    fileName = System.Text.RegularExpressions.Regex.Replace(fileName.Trim('"'), @"[\W]+", "_") + "_" + timeLable + "." + fileExtension;
                    var localFileName = file.LocalFileName;

                    WorkRegUploadInfoModel requestModel = new WorkRegUploadInfoModel()
                    {
                        RegisType = regisType,
                        BatchWorkRegID = bacthWorkRegID,
                        FileName = fileName,
                        LocalFileName = localFileName,
                        CurrentUserID = CurrentUser.ID,
                        FilePath = Path.Combine(excelPath, fileName)
                    };
                    return _batchWorkRegBusiness.UploadFile(requestModel);
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
        /// Thêm mới thông tin đợt tạo Phiếu đăng ký (đăng ký số lượng lớn)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_ĐKCTĐ, EnumUserPermission.ADD_ĐKCTTCNH)]
        public ResponseModel<int> CreateBatchWorkReg([FromBody] WorkRegAddEditModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _batchWorkRegBusiness.CreateBatchWorkReg(requestModel);
        }

    }
}
