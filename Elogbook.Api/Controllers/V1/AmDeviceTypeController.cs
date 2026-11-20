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
    /// Loại thiết bị AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amDeviceType")]
    public class AmDeviceTypeController : BaseApiController
    {
        private readonly IAmDeviceTypeBusiness _deviceTypeBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceTypeBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmDeviceTypeController(IAmDeviceTypeBusiness deviceTypeBusiness,
            ICommonBusiness commonBusiness)
        {
            _deviceTypeBusiness = deviceTypeBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm loại thiết bị theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLLTB
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetDeviceTypes([FromBody] AmDeviceTypeSearchModel requestModel)
        {
            return _deviceTypeBusiness.GetDeviceTypes(requestModel);
        }

        /// <summary>
        /// Tìm kiếm loại thiết bị theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLLTB
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmDeviceTypeModel> GetDeviceTypeById([FromUri] int id)
        {
            return _deviceTypeBusiness.GetDeviceTypeById(id);
        }

        /// <summary>
        /// Thêm mới loại thiết bị 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateDeviceType([FromBody] AmDeviceTypeAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLLTB)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _deviceTypeBusiness.CreateDeviceType(requestModel);
        }

        /// <summary>
        /// Cập nhập loại thiết bị 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateDeviceType([FromBody] AmDeviceTypeAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLLTB)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _deviceTypeBusiness.UpdateDeviceType(requestModel);
        }

        /// <summary>
        /// Xóa loại thiết bị 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteDeviceType([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLLTB)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _deviceTypeBusiness.DeleteDeviceType(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ PMIS
        /// </summary>
        /// <returns></returns>
        [Route("syncDataPmis")]
        [HttpPost]
        public ResponseBaseModel SyncDataPmis()
        {
            return _deviceTypeBusiness.SyncDataPmis();
        }

        /// <summary>
        ///  Danh sách Loai thiết bị
        /// </summary>
        /// <returns></returns>
        [Route("getAllDeviceType")]
        [HttpGet]
        public List<ComboboxDataModel> GetAllDeviceType()
        {
            return _deviceTypeBusiness.GetAllDeviceType();
        }
    }
}
