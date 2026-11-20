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
    /// nước sản xuất AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amCountryManufacture")]
    public class AmCountryManufactureController : BaseApiController
    {
        private readonly IAmCountryManufactureBusiness _countryManufactureBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CountryManufactureBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmCountryManufactureController(IAmCountryManufactureBusiness CountryManufactureBusiness,
            ICommonBusiness commonBusiness)
        {
            _countryManufactureBusiness = CountryManufactureBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm nước sản xuất theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetCountryManufactures([FromBody] AmCountryManufactureSearchModel requestModel)
        {
            return _countryManufactureBusiness.GetCountryManufactures(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nước sản xuất theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmCountryManufactureModel> GetCountryManufactureById([FromUri] int id)
        {
            return _countryManufactureBusiness.GetCountryManufactureById(id);
        }

        /// <summary>
        /// Thêm mới nước sản xuất 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateCountryManufacture([FromBody] AmCountryManufactureAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLNCC)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _countryManufactureBusiness.CreateCountryManufacture(requestModel);
        }

        /// <summary>
        /// Cập nhập nước sản xuất 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateCountryManufacture([FromBody] AmCountryManufactureAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLNCC)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _countryManufactureBusiness.UpdateCountryManufacture(requestModel);
        }

        /// <summary>
        /// Xóa nước sản xuất 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteCountryManufacture([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLNCC)).Data)
            //{
            //    responseModel.Message = CommonConstant.USER_RIGHT;
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            return _countryManufactureBusiness.DeleteCountryManufacture(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ PMIS
        /// </summary>
        /// <returns></returns>
        [Route("syncDataPmis")]
        [HttpPost]
        public ResponseBaseModel SyncDataPmis()
        {
            return _countryManufactureBusiness.SyncDataPmis();
        }

        /// <summary>
        ///  Danh sách nước sản xuất
        /// </summary>
        /// <returns></returns>
        [Route("getAllCountryManufacture")]
        [HttpGet]
        public List<ComboboxDataModel> GetAllCountryManufacture()
        {
            return _countryManufactureBusiness.GetAllCountryManufacture();
        }
    }
}
