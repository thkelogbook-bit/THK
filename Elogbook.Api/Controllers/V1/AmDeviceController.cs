using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Thiết bị AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amDevice")]
    public class AmDeviceController : BaseApiController
    {
        private readonly IAmDeviceBusiness _deviceBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmDeviceController(IAmDeviceBusiness deviceBusiness,
            ICommonBusiness commonBusiness)
        {
            _deviceBusiness = deviceBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm thiết bị theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLTB
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetDevices([FromBody] AmDeviceSearchModel requestModel)
        {
            requestModel.CompanyID = CurrentUser.CompanyID;
            return _deviceBusiness.GetDevices(requestModel);
        }

        /// <summary>
        /// Tìm kiếm thiết bị theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLTB
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmDeviceModel> GetDeviceById([FromUri] int id)
        {
            return _deviceBusiness.GetDeviceById(id);
        }

        /// <summary>
        /// Thêm mới thiết bị 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateDevice([FromBody] AmDeviceAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLTB)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _deviceBusiness.CreateDevice(requestModel);
        }

        /// <summary>
        /// Cập nhập thiết bị 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateDevice([FromBody] AmDeviceAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLTB)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _deviceBusiness.UpdateDevice(requestModel);
        }

        /// <summary>
        /// Xóa thiết bị 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteDevice([FromUri] int id)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLTB)).Data)
            {
                responseBaseModel.Message = CommonConstant.USER_RIGHT;
                responseBaseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseBaseModel;
            }

            return _deviceBusiness.DeleteDevice(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ PMIS
        /// </summary>
        /// <returns></returns>
        [Route("syncDataPmis")]
        [HttpPost]
        public ResponseBaseModel SyncDataPmis()
        {
            return _deviceBusiness.SyncDataPmis();
        }

        /// <summary>
        ///  Danh sách theo Khu vực
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        [Route("getDeviceByLocationID")]
        [HttpGet]
        public List<ComboboxDataModel> GetDeviceByLocationID(int locationID)
        {
            return _deviceBusiness.GetDeviceByLocationID(locationID);
        }

        /// <summary>
        /// Danh sách Thiết bị cha 
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="deviceTypeID"></param>
        /// <returns></returns>
        [Route("findParent")]
        [HttpGet]
        public List<ComboboxDataModel> GetParent(int locationID, int deviceTypeID)
        {
            return _deviceBusiness.GetParent(locationID, deviceTypeID);
        }

        /// <summary>
        /// Tìm kiếm nhanh thông tin Thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("quickSearchDevices")]
        [HttpPost]
        public ResponseBaseModel QuickSearchDevices([FromBody] AmDeviceQuickSearchModel requestModel)
        {
            requestModel.CompanyID = CurrentUser.CompanyID;
            return _deviceBusiness.QuickSearchDevices(requestModel);
        }

        /// <summary>
        /// Tìm kiếm thiết bị theo cấu trúc phân cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findHierarchy")]
        [HttpPost]
        public ResponseBaseModel GetDevicesHierarchy([FromBody] AmDeviceSearchModel requestModel)
        {
            requestModel.CompanyID = CurrentUser.CompanyID;
            return _deviceBusiness.GetDevicesHierarchy(requestModel);
        }

        /// <summary>
        /// Tìm kiếm thiết bị cha cao nhất
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findRootDevices")]
        [HttpPost]
        public ResponseBaseModel GetRootDevices([FromBody] AmDeviceSearchModel requestModel)
        {
            requestModel.CompanyID = CurrentUser.CompanyID;
            return _deviceBusiness.GetRootDevices(requestModel);
        }

        /// <summary>
        /// Lấy cây phân cấp khu vực - thiết bị
        /// </summary>
        /// <returns></returns>
        [Route("locationDeviceHierarchy")]
        [HttpGet]
        public ResponseModel<List<BaseHierachyModel>> GetLocationDeviceHierarchy()
        {
            return _deviceBusiness.GetLocationDeviceHierarchy();
        }

        /// <summary>
        /// Thay đổi parent của Device hoặc Location
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("changeParent")]
        [HttpPost]
        public ResponseBaseModel ChangeParent([FromBody] ChangeParentLocationModel requestModel)
        {
            return _deviceBusiness.ChangeParent(requestModel);
        }

        /// <summary>
        /// Lấy danh sách thiết bị con
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/children")]
        [HttpGet]
        public ResponseModel<List<AmDeviceModel>> GetDeviceChildren([FromUri] int id)
        {
            return _deviceBusiness.GetDeviceChildren(id);
        }
    }
}
