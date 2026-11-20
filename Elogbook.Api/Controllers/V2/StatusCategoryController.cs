using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Danh mục trạng thái
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/statusCategory")]
    public class StatusCategoryController : BaseApiController
    {
        private readonly IStatusCategoryBusiness _statusCategoryBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusCategoryBusiness"></param>
        public StatusCategoryController(IStatusCategoryBusiness statusCategoryBusiness)
        {
            _statusCategoryBusiness = statusCategoryBusiness;
        }

        /// <summary>
        /// Danh sách trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetStatusCategories([FromBody] SearchStatusCategoryModel requestModel)
        {
            return _statusCategoryBusiness.GetStatusCategories(requestModel);
        }

        /// <summary>
        /// Chi tiết trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<StatusCategoryModel> GetStatusCategoryById([FromUri] int id)
        {
            return _statusCategoryBusiness.GetStatusCategoryById(id);
        }

        /// <summary>
        /// Thêm mới trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<StatusCategoryModel> AddStatusCategory([FromBody] AddEditStatusCategoryModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _statusCategoryBusiness.AddStatusCategory(requestModel);
        }

        /// <summary>
        /// Cập nhập trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<StatusCategoryModel> UpdateStatusCategory([FromBody] AddEditStatusCategoryModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _statusCategoryBusiness.UpdateStatusCategory(requestModel);
        }

        /// <summary>
        /// Xóa trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteStatusCategory([FromUri] int id)
        {
            return _statusCategoryBusiness.DeleteStatusCategory(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra trạng thái đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        public ResponseModel<bool> CheckInUseStatusCategory([FromUri] int id)
        {
            return _statusCategoryBusiness.CheckInUseStatusCategory(id);
        }
    }
}
