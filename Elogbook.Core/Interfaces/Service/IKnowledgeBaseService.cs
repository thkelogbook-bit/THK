using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Quản lý tài liệu
    /// </summary>
    public interface IKnowledgeBaseService
    {
        /// <summary>
        /// Lấy tất cả tài liệu đang active (dùng cho Sources list)
        /// </summary>
        /// <returns></returns>
        List<KnowledgeBaseSourceItem> GetAllSources();

        /// <summary>
        /// Tìm tất cả tài liệu full text
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SearchFullText(KnowledgeBaseRequestModel requestModel);

        /// <summary>
        /// Tìm tất cả tài liệu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetKnowledgeBases(KnowledgeBaseRequestModel requestModel);

        /// <summary>
        /// Tìm kiếm tài liệu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KnowledgeBaseModel GetKnowledgeBaseById(int id);

        /// <summary>
        /// Thêm mới tài liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddKnowledgeBase(AddEditKnowledgeBaseModel requestModel);

        /// <summary>
        /// Cập nhật tài liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateKnowledgeBase(AddEditKnowledgeBaseModel requestModel);

        /// <summary>
        /// Xoá tài liệu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteKnowledgeBase(int id, int userID);
    }
}
