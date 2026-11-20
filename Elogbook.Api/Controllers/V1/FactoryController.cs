using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý Nhà máy
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/factory")]
    public class FactoryController : BaseApiController
    {
        private readonly IFactoryBusiness _factoryBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="factoryBusiness"></param>
        public FactoryController(IFactoryBusiness factoryBusiness)
        {
            _factoryBusiness = factoryBusiness;
        }

        /// <summary>
        /// Trả về tất cả Nhà máy theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_NM)]
        public ResponseBaseModel GetFactorys([FromBody] SearchFactoryModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Content = null;
                return responseModel;
            }

            return _factoryBusiness.GetFactorys(requestModel);
        }

        /// <summary>
        /// Trả về Nhà máy theo mã Nhà máy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_NM)]
        public ResponseModel<FactoryModel> GetFactoryById([FromUri] int id)
        {
            return _factoryBusiness.GetFactoryById(id);
        }

        /// <summary>
        /// Thêm mới Nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_NM)]
        public ResponseModel<int> AddFactory([FromBody] AddEditFactoryModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Data = 0;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _factoryBusiness.AddFactory(requestModel);
        }

        /// <summary>
        /// Cập nhập Nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_NM)]
        public ResponseModel<bool> UpdateFactory([FromBody] AddEditFactoryModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Data = false;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _factoryBusiness.UpdateFactory(requestModel);
        }

        /// <summary>
        /// Xóa Nhà máy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_NM)]
        public ResponseModel<bool> DeleteFactory([FromUri] int id)
        {
            return _factoryBusiness.DeleteFactory(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra Nhà máy có đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_NM)]
        public ResponseModel<bool> CheckInUseFactory([FromUri] int id)
        {
            return _factoryBusiness.CheckInUseFactory(id);
        }
    }
}