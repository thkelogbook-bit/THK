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
    /// Quản lý khu vực
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amLocation")]
    public class AmLocationController : BaseApiController
    {
        private readonly IAmLocationBusiness _locationBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locationBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmLocationController(IAmLocationBusiness locationBusiness,
            ICommonBusiness commonBusiness)
        {
            _locationBusiness = locationBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm địa điểm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLKVDD
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetLocations([FromBody] AmLocationSearchModel requestModel)
        {
            return _locationBusiness.GetLocations(requestModel);
        }

        /// <summary>
        /// Tìm kiếm địa điểm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLKVDD
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmLocationModel> GetLocationById([FromUri] int id)
        {
            return _locationBusiness.GetLocationById(id);
        }

        /// <summary>
        /// Thêm mới địa điểm 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateLocation([FromBody] AmLocationAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLKVDD)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _locationBusiness.CreateLocation(requestModel);
        }

        /// <summary>
        /// Cập nhập địa điểm 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateLocation([FromBody] AmLocationAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLKVDD)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _locationBusiness.UpdateLocation(requestModel);
        }

        /// <summary>
        /// Xóa địa điểm 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteLocation([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLKVDD)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _locationBusiness.DeleteLocation(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ PMIS
        /// </summary>
        /// <returns></returns>
        [Route("syncDataPmis")]
        [HttpPost]
        public ResponseBaseModel SyncDataPmis()
        {
            return _locationBusiness.SyncDataPmis();
        }

        /// <summary>
        ///  Danh sách Khu vực theo công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="isParent"></param>
        /// <returns></returns>
        [Route("getLocationByCompanyID")]
        [HttpGet]
        public List<ComboboxDataModel> GetLocationByCompanyID(int companyID, bool isParent)
        {
            return _locationBusiness.GetLocationByCompanyID(companyID, isParent);
        }

        /// <summary>
        /// Danh sách Khu vực cha
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        [Route("findParent")]
        [HttpGet]
        public List<ComboboxDataModel> GetParent(int companyID)
        {
            return _locationBusiness.GetParent(companyID);
        }
    }
}
