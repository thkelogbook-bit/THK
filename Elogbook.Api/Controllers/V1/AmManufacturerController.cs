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
    /// nhà chế tạo AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amManufacturer")]
    public class AmManufacturerController : BaseApiController
    {
        private readonly IAmManufacturerBusiness _manufacturerBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ManufacturerBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmManufacturerController(IAmManufacturerBusiness ManufacturerBusiness,
            ICommonBusiness commonBusiness)
        {
            _manufacturerBusiness = ManufacturerBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm nhà chế tạo theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetManufacturers([FromBody] AmManufacturerSearchModel requestModel)
        {
            return _manufacturerBusiness.GetManufacturers(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nhà chế tạo theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmManufacturerModel> GetManufacturerById([FromUri] int id)
        {
            return _manufacturerBusiness.GetManufacturerById(id);
        }

        /// <summary>
        /// Thêm mới nhà chế tạo 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateManufacturer([FromBody] AmManufacturerAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLNCC)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _manufacturerBusiness.CreateManufacturer(requestModel);
        }

        /// <summary>
        /// Cập nhập nhà chế tạo 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateManufacturer([FromBody] AmManufacturerAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLNCC)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _manufacturerBusiness.UpdateManufacturer(requestModel);
        }

        /// <summary>
        /// Xóa nhà chế tạo 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteManufacturer([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLNCC)).Data)
            //{
            //    responseModel.Message = CommonConstant.USER_RIGHT;
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            return _manufacturerBusiness.DeleteManufacturer(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ PMIS
        /// </summary>
        /// <returns></returns>
        [Route("syncDataPmis")]
        [HttpPost]
        public ResponseBaseModel SyncDataPmis()
        {
            return _manufacturerBusiness.SyncDataPmis();
        }

        /// <summary>
        ///  Danh sách nhà chế tạo
        /// </summary>
        /// <returns></returns>
        [Route("getAllManufacturer")]
        [HttpGet]
        public List<ComboboxDataModel> GetAllManufacturer()
        {
            return _manufacturerBusiness.GetAllManufacturer();
        }
    }
}
