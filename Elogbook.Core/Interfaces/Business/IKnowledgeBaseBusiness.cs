using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Quản lý tài liệu
    /// </summary>
    public interface IKnowledgeBaseBusiness
    {
        /// <summary>
        /// Lấy tất cả tài liệu đang active (dùng cho Sources list)
        /// </summary>
        /// <returns></returns>
        ResponseModel<List<KnowledgeBaseSourceItem>> GetAllSources();

        /// <summary>
        /// Tìm tất cả tài liệu full text
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SearchFullText(KnowledgeBaseRequestModel requestModel);

        /// <summary>
        /// Tìm kiếm danh sách tài liệu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetKnowledgeBases(KnowledgeBaseRequestModel requestModel);

        /// <summary>
        /// Tìm kiếm tài liệu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<KnowledgeBaseModel> GetKnowledgeBaseById(int id);

        /// <summary>
        /// Thêm tài liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddKnowledgeBase(AddEditKnowledgeBaseModel requestModel);

        /// <summary>
        /// Cập nhật tài liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateKnowledgeBase(AddEditKnowledgeBaseModel requestModel);

        /// <summary>
        /// Xoá tài liệu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteKnowledgeBase(int id, int userID);
    }
}
