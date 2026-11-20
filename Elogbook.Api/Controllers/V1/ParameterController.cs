using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý Thông số
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/parameter")]
    public class ParameterController : BaseApiController
    {
        private readonly IParameterBusiness _parameterBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterBusiness"></param>
        public ParameterController(IParameterBusiness parameterBusiness)
        {
            _parameterBusiness = parameterBusiness;
        }

        /// <summary>
        /// Trả về tất cả Thông số theo điều kiện truyền vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_TS)]
        public ResponseBaseModel GetParameters([FromBody] SearchParameterModel requestModel)
        {
            return _parameterBusiness.GetParameters(requestModel);
        }

        /// <summary>
        /// Lấy danh sách nhóm thông số và thông số của nhóm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("groupDetail")]
        [HttpPost]
        public ResponseBaseModel GetParameterGroupDetails([FromBody] SearchParameterForStopModel requestModel)
        {
            return _parameterBusiness.GetParameters(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nhanh danh mục thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("QuickSearchParameters")]
        [HttpPost]
        public ResponseBaseModel QuickSearchParameters([FromBody] SearchParameterModel requestModel)
        {
            return _parameterBusiness.QuickSearchParameters(requestModel);
        }

        /// <summary>
        /// Lấy dữ liệu để vẽ chart cho TSVH
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getParametersDrawChart")]
        [HttpPost]
        public ParametersChartModel GetParametersDrawChart(SearchParameterForChartModel requestModel)
        {
            return _parameterBusiness.GetParametersDrawChart(requestModel);
        }

        /// <summary>
        /// Trả về một Thông số theo mã Thông số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_TS)]
        public ResponseModel<ParameterModel> GetParameterById([FromUri] int id)
        {
            return _parameterBusiness.GetParameterById(id);
        }

        /// <summary>
        /// Danh sách cương vị trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("getOperationPositions/{factoryID}")]
        [HttpGet]
        public ResponseModel<List<CommonDataComboboxList>> GetOperationPositions([FromUri] int factoryID)
        {
            return _parameterBusiness.GetOperationPositions(factoryID);
        }

        /// <summary>
        /// Thêm mới một Thống số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_TS)]
        public ResponseModel<int> AddParameter([FromBody] AddEditParameterModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _parameterBusiness.AddParameter(requestModel);
        }

        /// <summary>
        /// Cập nhập một Thống số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_TS)]
        public ResponseModel<bool> UpdateParameter([FromBody] AddEditParameterModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _parameterBusiness.UpdateParameter(requestModel);
        }

        /// <summary>
        /// Xóa một Thống số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_TS)]
        public ResponseModel<bool> DeleteParameter([FromUri] int id)
        {
            return _parameterBusiness.DeleteParameter(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra Thông số có đang sử dụng hay chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_TS)]
        public ResponseModel<bool> CheckInUseParameter([FromUri] int id)
        {
            return _parameterBusiness.CheckInUseParameter(id);
        }
    }
}