using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
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
    /// nhà cung cấp AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amSupplier")]
    public class AmSupplierController : BaseApiController
    {
        private readonly IAmSupplierBusiness _supplierBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SupplierBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmSupplierController(IAmSupplierBusiness SupplierBusiness,
            ICommonBusiness commonBusiness)
        {
            _supplierBusiness = SupplierBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm nhà cung cấp theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetSuppliers([FromBody] AmSupplierSearchModel requestModel)
        {
            return _supplierBusiness.GetSuppliers(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nhà cung cấp theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmSupplierModel> GetSupplierById([FromUri] int id)
        {
            return _supplierBusiness.GetSupplierById(id);
        }

        /// <summary>
        /// Thêm mới nhà cung cấp 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateSupplier([FromBody] AmSupplierAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLNSX)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _supplierBusiness.CreateSupplier(requestModel);
        }

        /// <summary>
        /// Cập nhập nhà cung cấp 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateSupplier([FromBody] AmSupplierAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLNSX)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _supplierBusiness.UpdateSupplier(requestModel);
        }

        /// <summary>
        /// Xóa nhà cung cấp 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteSupplier([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLNSX)).Data)
            //{
            //    responseModel.Message = CommonConstant.USER_RIGHT;
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            return _supplierBusiness.DeleteSupplier(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ PMIS
        /// </summary>
        /// <returns></returns>
        [Route("syncDataPmis")]
        [HttpPost]
        public ResponseBaseModel SyncDataPmis()
        {
            return _supplierBusiness.SyncDataPmis();
        }

        /// <summary>
        ///  Danh sách nhà cung cấp
        /// </summary>
        /// <returns></returns>
        [Route("getAllSupplier")]
        [HttpGet]
        public List<ComboboxDataModel> GetAllSupplier()
        {
            return _supplierBusiness.GetAllSupplier();
        }
    }
}
