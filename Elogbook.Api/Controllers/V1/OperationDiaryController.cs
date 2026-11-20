using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Nhật ký vận hành
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/operationDiary")]
    public class OperationDiaryController : BaseApiController
    {
        private readonly IOperationDiaryBusiness _operationDiaryBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationDiaryBusiness"></param>
        /// <param name="commonBusiness"></param>
        public OperationDiaryController(IOperationDiaryBusiness operationDiaryBusiness,
            ICommonBusiness commonBusiness)
        {
            _operationDiaryBusiness = operationDiaryBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền thao tác NKVH hoặc là người tạo
        /// </summary>
        private bool CanOperateDiary(int operationDiaryID, string permissionKey)
        {
            var hasRight = _commonBusiness.CheckUserHasRight(CurrentUser.ID, permissionKey).Data
                        || _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_DELETE_NKVH)).Data;
            return hasRight || _operationDiaryBusiness.IsCreator(operationDiaryID, CurrentUser.ID);
        }

        /// <summary>
        /// Tìm kiếm nhật ký vận hành theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_NKVH)]
        public ResponseBaseModel GetOperationDiarys([FromBody] SearchOperationDiaryModel requestModel)
        {
            return _operationDiaryBusiness.GetOperationDiarys(requestModel);
        }

        /// <summary>
        /// Xem thông tin nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_NKVH
        /// <returns></returns>
        [Route("viewOperationDiary")]
        [HttpPost]
        public ResponseModel<ViewOpeartionDiaryModel> ViewOperationDiary(ViewOperationDiaryRequestModel requestModel)
        {
            return _operationDiaryBusiness.ViewOperationDiary(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nhật ký vận hành theo điều kiện (Web)
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_NKVH
        /// <returns></returns>
        [Route("findWeb")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_NKVH)]
        public ResponseBaseModel GetOperationDiarysWeb([FromBody] SearchOperationDiaryWebModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationDiaryBusiness.GetOperationDiarysWeb(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nhanh thông tin NKVH
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_NKVH
        /// <returns></returns>
        [Route("quickSearchOperationDiarys")]
        [HttpPost]
        public ResponseBaseModel QuickSearchOperationDiarys([FromBody] QuickSearchOperationDiaryModel requestModel)
        {
            return _operationDiaryBusiness.QuickSearchOperationDiarys(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nhật ký vận hành theo id
        /// </summary>
        /// <param name="id"></param>
        /// EnumUserPermission.VIEW_NKVH
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<ViewOpeartionDiaryModel> GetOperationDiaryById([FromUri] int id)
        {
            return _operationDiaryBusiness.GetOperationDiaryById(id);
        }

        /// <summary>
        /// Thêm mới nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_NKVH)]
        public ResponseModel<int> AddOperationDiary([FromBody] AddOperationDiary requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationDiaryBusiness.AddOperationDiary(requestModel);
        }

        /// <summary>
        /// Cập nhật nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_NKVH)]
        public ResponseModel<bool> UpdateOperationDiary([FromBody] EditOperationDiary requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationDiaryBusiness.UpdateOperationDiary(requestModel);
        }

        /// <summary>
        /// Ký giao/nhận ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("deliveryReceiveShift")]
        [HttpPost]
        public ResponseModel<DeliveryReceiveShiftModel> DeliveryReceiveShift([FromBody] AddEditDeliveryReceiveShiftModel requestModel)
        {
            ResponseModel<DeliveryReceiveShiftModel> responseModel = new ResponseModel<DeliveryReceiveShiftModel>();
            if (!CanOperateDiary(requestModel.OperationDiaryID, nameof(EnumUserPermission.EDIT_NKVH)))
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationDiaryBusiness.DeliveryReceiveShift(requestModel);
        }

        /// <summary>
        /// Xóa nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_NKVH)]
        public ResponseModel<bool> DeleteOperationDiary([FromUri] int id)
        {
            return _operationDiaryBusiness.DeleteOperationDiary(id, CurrentUser.ID);
        }

        /// <summary>
        /// Xóa tình hình vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deleteOperationSituation/{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteOperationSituation([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            var hasRight = _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_NKVH)).Data
                        || _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_DELETE_NKVH)).Data;
            if (!hasRight && !_operationDiaryBusiness.IsCreatorByOperationSituation(id, CurrentUser.ID))
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _operationDiaryBusiness.DeleteOperationSituation(id, CurrentUser.ID);
        }

        /// <summary>
        /// Xóa công tác đang thực hiện
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deleteActiveWork/{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteActiveWork([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            var hasRight = _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_NKVH)).Data
                        || _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_DELETE_NKVH)).Data;
            if (!hasRight && !_operationDiaryBusiness.IsCreatorByActiveWork(id, CurrentUser.ID))
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _operationDiaryBusiness.DeleteActiveWork(id, CurrentUser.ID);
        }

        /// <summary>
        /// Xóa tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deleteExistInShift/{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteExistInShift([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            var hasRight = _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_NKVH)).Data
                        || _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_DELETE_NKVH)).Data;
            if (!hasRight && !_operationDiaryBusiness.IsCreatorByExistInShift(id, CurrentUser.ID))
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _operationDiaryBusiness.DeleteExistInShift(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra tồn tại ca ghi nhật ký vận hành đang mở
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// EnumUserPermission.VIEW_NKVH
        /// <returns></returns>
        [Route("validateOpenShift")]
        [HttpGet]
        public ResponseModel<ValidateOpenShiftModel> ValidateOpenShift([FromUri] int factoryID, [FromUri] int operationPositionID)
        {
            return _operationDiaryBusiness.ValidateOpenShift(factoryID, operationPositionID);
        }

        /// <summary>
        /// In tài liệu nhật ký vận hành ra file PDF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("exportFilePDF")]
        [HttpGet]
        public HttpResponseMessage ExportFile([FromUri] int id)
        {
            return _operationDiaryBusiness.ExportFile(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.EDIT_NKVH
        /// <returns></returns>
        [Route("operationDiarySyncData")]
        [HttpPost]
        public ResponseBaseModel OperationDiarySyncData([FromBody] OperationDiarySyncDataModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.ApiVersion))
                requestModel.ApiVersion = "V1";
            return _operationDiaryBusiness.OperationDiarySyncData(requestModel);
        }

        /// <summary>
        /// Lấy nhãn nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_NKVH
        /// <returns></returns>
        [Route("getOperationDiaryLabels")]
        [HttpPost]
        public ResponseBaseModel GetOperationDiaryLabels([FromBody] SearchOperationDiaryWebModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationDiaryBusiness.GetOperationDiaryLabels(requestModel);
        }

        /// <summary>
        /// Lấy chi tiết nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEWONLY_NKVH
        /// <returns></returns>
        [Route("getOperationDiaryDetails")]
        [HttpPost]
        public ResponseBaseModel GetOperationDiaryDetails([FromBody] SearchOperationDiaryWebModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationDiaryBusiness.GetOperationDiaryDetails(requestModel);
        }

        /// <summary>
        /// Tạo yêu cầu export nhật ký vận hành
        /// </summary>
        [Route("exportOperationDiary")]
        [HttpPost]
        [Permission(EnumUserPermission.DOWNLOAD_EXPORT_NKVH)]
        public ResponseModel<bool> ExportOperationDiary([FromBody] ExportOperationDiaryRequestModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationDiaryBusiness.ExportOperationDiary(requestModel);
        }

        /// <summary>
        /// Lấy danh sách yêu cầu export nhật ký vận hành
        /// </summary>
        [Route("getExportOperationDiary")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_EXPORT_NKVH)]
        public ResponseBaseModel GetExportOperationDiary([FromBody] BaseListFilter requestModel)
        {
            return _operationDiaryBusiness.GetExportOperationDiary(requestModel);
        }

        /// <summary>
        /// Download file từ ExportOperationDiary
        /// </summary>
        /// <param name="exportOperationDiaryId">Id của ExportOperationDiary</param>
        /// <returns>File được tải xuống</returns>
        [Route("downloadExportOperationDiaryFile/{exportOperationDiaryId}")]
        [HttpGet]
        [Permission(EnumUserPermission.VIEW_EXPORT_NKVH)]
        public HttpResponseMessage DownloadExportOperationDiaryFile([FromUri] int exportOperationDiaryId)
        {
            return _operationDiaryBusiness.DownloadExportOperationDiaryFile(exportOperationDiaryId);
        }

        /// <summary>
        /// Xóa thông tin export nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("exportOperationDiary/{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_EXPORT_NKVH)]
        public ResponseModel<bool> DeleteExportOperationDiary([FromUri] int id)
        {
            return _operationDiaryBusiness.DeleteExportOperationDiary(id, CurrentUser.ID);
        }

        /// <summary>
        /// Xuất dữ liệu nhân sự ca vận hành ra file Excel
        /// </summary>
        [Route("exportData")]
        [HttpPost]
        public HttpResponseMessage ExportFile([FromBody] ExportOperationDiaryModel requestModel)
        {
            return _operationDiaryBusiness.ExportPersonnelData(requestModel);
        }

    }
}
