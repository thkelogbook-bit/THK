using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý Thông số vận hành
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/operationParameters")]
    public class OperationParametersController : BaseApiController
    {
        private readonly IOperationParametersBusiness _operationParametersBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationParametersBusiness"></param>
        /// <param name="commonBusiness"></param>
        public OperationParametersController(IOperationParametersBusiness operationParametersBusiness,
            ICommonBusiness commonBusiness)
        {
            _operationParametersBusiness = operationParametersBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền EDIT_TSVH hoặc là người tạo khung giờ
        /// </summary>
        private bool CanOperateTimeLine(int operationTimeLineID)
        {
            var hasRight = _commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_TSVH)).Data;
            return hasRight || _operationParametersBusiness.IsCreator(operationTimeLineID, CurrentUser.ID);
        }

        /// <summary>
        /// Trả về tất cả chu kỳ của nhà máy tương ứng với người dùng hiện tại
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getCyclesManagements")]
        [HttpGet]
        public ResponseBaseModel GetCyclesManagements([FromUri] int factoryID, [FromUri] int operationPositionID)
        {
            return _operationParametersBusiness.GetCyclesManagements(factoryID, operationPositionID);
        }

        /// <summary>
        /// Cập nhật Thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateOperationParameters")]
        [HttpPut]
        public ResponseModel<bool> UpdateOperationParameters([FromBody] EditOperationParameterModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!CanOperateTimeLine(requestModel.OperationTimeLineID))
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.UpdateOperationParameters(requestModel);
        }

        /// <summary>
        /// Lấy danh sách khung giờ theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getOperationTimeLines")]
        [HttpPost]
        public ResponseBaseModel GetOperationTimeLines([FromBody] SearchOperationTimeLineModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.GetOperationTimeLines(requestModel);
        }

        /// <summary>
        /// Lấy danh sách khung giờ theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getOperationTimeLinesMobile")]
        [HttpPost]
        public ResponseBaseModel GetOperationTimeLinesMobile([FromBody] SearchOperationTimeLineModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.GetOperationTimeLinesMobile(requestModel);
        }

        /// <summary>
        /// Cập nhật khung giờ khi báo bận công tác hoặc báo dừng máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateOperationTimeLine")]
        [HttpPut]
        public ResponseModel<bool> UpdateOperationTimeLine([FromBody] EditOperationTimeLineModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_TSVH)).Data
                && (requestModel.ListID == null || !requestModel.ListID.Exists(id => _operationParametersBusiness.IsCreator(id, CurrentUser.ID))))
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.UpdateOperationTimeLine(requestModel);
        }

        /// <summary>
        /// Cập nhật khung giờ khi báo bận công tác hoặc báo dừng máy từng phần
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateOperationTimeLinePartial")]
        [HttpPut]
        public ResponseModel<bool> UpdateOperationTimeLinePartial([FromBody] StopOperationTimeLineModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_TSVH)).Data
                && (requestModel.OperationTimeLineIds == null || !requestModel.OperationTimeLineIds.Exists(id => _operationParametersBusiness.IsCreator(id, CurrentUser.ID))))
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.UpdateOperationTimeLine(requestModel);
        }

        /// <summary>
        /// Danh sách nhóm thông số và số thông số trong nhóm
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <param name="cycleID"></param>
        /// <param name="operationTimeLineID"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getParameterGroups")]
        [HttpGet]
        public ResponseBaseModel GetParameterGroups([FromUri] int factoryID, [FromUri] int operationPositionID, [FromUri] int cycleID, [FromUri] int operationTimeLineID)
        {
            return _operationParametersBusiness.GetParameterGroups(factoryID, operationPositionID, cycleID, operationTimeLineID);
        }

        /// <summary>
        /// Lấy danh sách thông số cần ghi thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getParameters")]
        [HttpPost]
        public ResponseBaseModel GetParameters([FromBody] SearchParametersModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.GetParameters(requestModel);
        }

        /// <summary>
        /// Lấy danh sách thông số vận hành theo khung giờ (mode Mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getOperationParametersModeMobile")]
        [HttpPost]
        public ResponseBaseModel GetOperationParameters([FromBody] SearchOperationParametersModel requestModel)
        {
            return _operationParametersBusiness.GetOperationParameters(requestModel);
        }

        /// <summary>
        /// Xem thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEWONLY_TSVH
        /// <returns></returns>
        [Route("viewOperationParameters")]
        [HttpPost]
        public ResponseBaseModel ViewOperationParameters(ViewOperationParametersModel requestModel)
        {
            return _operationParametersBusiness.ViewOperationParameters(requestModel);
        }

        /// <summary>
        /// Lấy danh sách thông số vận hành của 1 chu kỳ theo ngày (mode portal)
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getOperationParametersPortal")]
        [HttpPost]
        public ResponseBaseModel GetOperationParametersPortal([FromBody] SearchOperationParametersModel requestModel)
        {
            return _operationParametersBusiness.GetOperationParametersPortal(requestModel);
        }

        /// <summary>
        /// In tài liệu thông số vận hành ra file PDF
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <param name="createDate"></param>
        /// <returns></returns>
        [Route("exportPdfFile")]
        [HttpGet]
        public HttpResponseMessage ExportPdfFile([FromUri] int factoryID, int operationPositionID, string createDate)
        {
            PdfExportOPModel requestModel = new PdfExportOPModel()
            {
                FactoryID = factoryID,
                OperationPositionID = operationPositionID,
                CreateDate = createDate,
                CurrentUserID = CurrentUser.ID
            };
            return _operationParametersBusiness.ExportPdfFile(requestModel);
        }

        /// <summary>
        /// Export operation parameters to Excel file
        /// </summary>
        [Route("exportOperationParametersExcel")]
        [HttpGet]
        [Permission(EnumUserPermission.VIEWONLY_TSVH, EnumUserPermission.VIEW_TSVH)]
        public HttpResponseMessage ExportOperationParametersExcel([FromUri] int factoryID, int operationPositionID, string createDate)
        {
            PdfExportOPModel requestModel = new PdfExportOPModel()
            {
                FactoryID = factoryID,
                OperationPositionID = operationPositionID,
                CreateDate = createDate,
                CurrentUserID = CurrentUser.ID
            };
            return _operationParametersBusiness.ExportOperationParametersExcel(requestModel);
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("operationParameterSyncData")]
        [HttpPost]
        public ResponseBaseModel OperationParameterSyncData([FromBody] OperationParametersSyncDataModel requestModel)
        {
            return _operationParametersBusiness.OperationParameterSyncData(requestModel);
        }

        /// <summary>
        /// Tạo thông số theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addOperationParameterByTime")]
        [HttpPost]
        public ResponseBaseModel AddOperationParameterByTime([FromBody] AddOperationParameterByTimeModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.AddOperationParameterByTime(requestModel);
        }

        /// <summary>
        /// Lấy danh sách nhãn khun giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getTimeLineLabels")]
        [HttpPost]
        public ResponseBaseModel GetTimeLineLabels([FromBody] SearchOperationTimeLineModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.GetTimeLineLabels(requestModel);
        }

        /// <summary>
        /// Lấy danh sách thông số theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_TSVH
        /// <returns></returns>
        [Route("getTimeLineDetail")]
        [HttpPost]
        public ResponseBaseModel GetTimeLineDetail([FromBody] SearchOperationTimeLineDetailModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationParametersBusiness.GetTimeLineDetail(requestModel);
        }

        /// <summary>
        /// Lấy danh sách thông vượt ngưỡng giá trị bị cảnh báo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getOperationParameterWarning")]
        [HttpPost]
        public ResponseBaseModel GetOperationParameterWarning([FromBody] SearchOperationParameterWarningModel requestModel)
        {
            return _operationParametersBusiness.GetOperationParameterWarning(requestModel);
        }

        /// <summary>
        /// Lấy thông tin chi tiết cảnh báo AI của 1 thông số vận hành.
        /// Trả về: thông tin thiết bị (nhà máy, cương vị, mã KKS) và lịch sử giá trị ghi nhận
        /// (thời gian, khung giờ, range ngưỡng, giá trị thực tế).
        /// </summary>
        /// <param name="parameterId">ID thông số cần xem chi tiết</param>
        /// <returns></returns>
        [Route("getOperationParameterWarningDetail")]
        [HttpGet]
        public ResponseBaseModel GetOperationParameterWarningDetail([FromUri] int parameterId)
        {
            return _operationParametersBusiness.GetOperationParameterWarningDetail(parameterId);
        }
    }
}