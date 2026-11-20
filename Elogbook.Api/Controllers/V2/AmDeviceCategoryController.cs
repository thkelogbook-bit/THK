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

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Quản lý phân loại thiết bị
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/amDeviceCategory")]
    public class AmDeviceCategoryController : BaseApiController
    {
        private readonly IAmDeviceCategoryBusiness _amDeviceCategoryBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amDeviceCategoryBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmDeviceCategoryController(IAmDeviceCategoryBusiness amDeviceCategoryBusiness,
            ICommonBusiness commonBusiness)
        {
            _amDeviceCategoryBusiness = amDeviceCategoryBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Thêm mới phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<AmDeviceCategoryModel> Add([FromBody] AddEditAmDeviceCategoryModel requestModel)
        {
            ResponseModel<AmDeviceCategoryModel> responseModel = new ResponseModel<AmDeviceCategoryModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLPLTB)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _amDeviceCategoryBusiness.Add(requestModel);
        }

        /// <summary>
        /// Trả về phân loại thiết bị theo mã số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmDeviceCategoryModel> GetById([FromUri] int id)
        {
            return _amDeviceCategoryBusiness.GetById(id);
        }

        /// <summary>
        /// Trả về tất cả phân loại thiết bị theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel Find([FromBody] SearchAmDeviceCategoryModel requestModel)
        {
            return _amDeviceCategoryBusiness.Find(requestModel);
        }

        /// <summary>
        /// Trả về tất cả phân loại thiết bị cha theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findParent")]
        [HttpPost]
        public ResponseBaseModel FindParent([FromBody] SearchParentAmDeviceCategoryModel requestModel)
        {
            return _amDeviceCategoryBusiness.FindParent(requestModel);
        }

        /// <summary>
        /// Cập nhập phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<AmDeviceCategoryModel> Update([FromBody] AddEditAmDeviceCategoryModel requestModel)
        {
            ResponseModel<AmDeviceCategoryModel> responseModel = new ResponseModel<AmDeviceCategoryModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLPLTB)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _amDeviceCategoryBusiness.Update(requestModel);
        }

        /// <summary>
        /// Xóa phân loại thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> Delete([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLPLTB)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _amDeviceCategoryBusiness.Delete(id, CurrentUser.ID);
        }

        /// <summary>
        /// sync PMIS
        /// </summary>
        /// <returns></returns>
        [Route("syncPmis")]
        [HttpPost]
        public ResponseBaseModel SyncPmis()
        {
            return _amDeviceCategoryBusiness.SyncPmis();
        }

        /// <summary>
        ///  Danh sách theo Công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="isParent"></param>
        /// <returns></returns>
        [Route("getDeviceCategoriesByCompany")]
        [HttpGet]
        public List<ComboboxDataModel> GetDeviceCategoriesByCompany(int companyID, bool isParent)
        {
            return _amDeviceCategoryBusiness.GetDeviceCategoriesByCompany(companyID, isParent);
        }
    }
}