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
    /// Quản lý tài liệu
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/knowledgeBase")]
    public class KnowledgeBaseController : BaseApiController
    {
        private readonly IKnowledgeBaseBusiness _knowledgeBaseBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="knowledgeBaseBusiness"></param>
        public KnowledgeBaseController(IKnowledgeBaseBusiness knowledgeBaseBusiness)
        {
            _knowledgeBaseBusiness = knowledgeBaseBusiness;
        }

        /// <summary>
        /// Lấy tất cả tài liệu đang active — dùng cho Sources list ở UI Hỏi đáp AI
        /// </summary>
        [Route("sources")]
        [HttpGet]
        public ResponseModel<List<KnowledgeBaseSourceItem>> GetAllSources()
        {
            return _knowledgeBaseBusiness.GetAllSources();
        }

        /// <summary>
        /// Tìm tất cả tài liệu full text
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("searchFullText")]
        [HttpPost]
        public ResponseBaseModel SearchFullText([FromBody] KnowledgeBaseRequestModel requestModel)
        {
            return _knowledgeBaseBusiness.SearchFullText(requestModel);
        }

        /// <summary>
        /// Tìm kiếm tài liệu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetKnowledgeBases([FromBody] KnowledgeBaseRequestModel requestModel)
        {
            return _knowledgeBaseBusiness.GetKnowledgeBases(requestModel);
        }

        /// <summary>
        /// Tìm kiếm tài liệu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<KnowledgeBaseModel> GetKnowledgeBaseById([FromUri] int id)
        {
            return _knowledgeBaseBusiness.GetKnowledgeBaseById(id);
        }

        /// <summary>
        /// Thêm tài liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<int> AddKnowledgeBase([FromBody] AddEditKnowledgeBaseModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _knowledgeBaseBusiness.AddKnowledgeBase(requestModel);
        }

        /// <summary>
        /// Cập nhật thông tin tài liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateKnowledgeBase([FromBody] AddEditKnowledgeBaseModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _knowledgeBaseBusiness.UpdateKnowledgeBase(requestModel);
        }

        /// <summary>
        /// Xoá tài liệu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteKnowledgeBase([FromUri] int id)
        {
            return _knowledgeBaseBusiness.DeleteKnowledgeBase(id, CurrentUser.ID);
        }
    }
}