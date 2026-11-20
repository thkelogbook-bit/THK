using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Theo dõi tồn tại trong ca
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/issueOfShift")]
    public class IssueOfShiftManagementController : BaseApiController
    {
        private readonly IIssueOfShiftBusiness _issueOfShiftBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="issueOfShiftBusiness"></param>
        public IssueOfShiftManagementController(IIssueOfShiftBusiness issueOfShiftBusiness)
        {
            _issueOfShiftBusiness = issueOfShiftBusiness;
        }

        /// <summary>
        /// Tìm kiếm Tồn tại trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_TTTC)]
        public ResponseBaseModel Find([FromBody] SearchIssueOfShiftModel requestModel)
        {
            requestModel.UserID = CurrentUser.ID;
            return _issueOfShiftBusiness.Find(requestModel);
        }

        /// <summary>
        /// Thêm mới xử lý tồn tại trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        [Permission(EnumUserPermission.EDIT_TTTC)]
        public ResponseBaseModel Post([FromBody] IssueOfShiftDetailAdd requestModel)
        {
            requestModel.CreatedUserID = CurrentUser.ID;
            return _issueOfShiftBusiness.AddIssueOfShiftDetail(requestModel);
        }

        /// <summary>
        /// Chi tiết xử lý tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [Permission(EnumUserPermission.VIEW_TTTC, EnumUserPermission.EDIT_TTTC)]
        public ResponseBaseModel Get(int id)
        {
            return _issueOfShiftBusiness.GetIssueOfShift(id, CurrentUser.ID);
        }

        /// <summary>
        /// Cập nhật xử lý tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_TTTC)]
        public ResponseBaseModel Put(int id, [FromBody] IssueOfShiftDetailUpdate requestModel)
        {
            requestModel.ExistInShiftID = id;
            requestModel.UpdatedUserID = CurrentUser.ID;
            return _issueOfShiftBusiness.UpdateIssueOfShiftDetail(requestModel);
        }

        /// <summary>
        /// Xóa tồn tại trong ca
        /// </summary>
        /// <param name="existInShiftID"></param>
        /// <returns></returns>
        [Route("delete")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_TTTC)]
        public ResponseBaseModel Delete(int existInShiftID)
        {
            return _issueOfShiftBusiness.DeleteIssueOfShift(existInShiftID, CurrentUser.ID);
        }

        /// <summary>
        /// Xóa chi tiết xử lý tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deleteDetail")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_TTTC)]
        public ResponseBaseModel DeleteDetail(int id)
        {
            return _issueOfShiftBusiness.DeteleIssueOfShiftDetail(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.EDIT_TTTC
        [Route("syncDataIssueOfShiftDetail")]
        [HttpPost]
        public ResponseBaseModel SyncDataIssueOfShiftDetail([FromBody] IssueOfShiftDetailSyncDataModel requestModel)
        {
            return _issueOfShiftBusiness.SyncDataIssueOfShiftDetail(requestModel);
        }

        /// <summary>
        /// Xuất dữ liệu tồn tại trong ca ra file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("exportData")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_TTTC)]
        public HttpResponseMessage ExportData([FromBody] SearchIssueOfShiftModel requestModel)
        {
            return _issueOfShiftBusiness.ExportData(requestModel);
        }
    }
}
