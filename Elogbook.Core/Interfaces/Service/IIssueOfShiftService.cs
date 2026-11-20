using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Tồn tại trong ca
    /// </summary>
    public interface IIssueOfShiftService
    {
        /// <summary>
        /// Tìm kiếm Tồn tại trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchIssueOfShiftModel requestModel);

        /// <summary>
        /// Cập nhật thông tin xử lý Tồn tại trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel UpdateIssueOfShiftDetail(IssueOfShiftDetailUpdate requestModel);

        /// <summary>
        /// Tạo thông tin xử lý Tồn tại trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddIssueOfShiftDetail(IssueOfShiftDetailAdd requestModel);

        /// <summary>
        /// Lấy chi tiết Tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel GetIssueOfShiftById(int id, int userID);
        /// <summary>
        /// Xóa tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteIssueOfShift(int id, int userID);

        /// <summary>
        /// Xóa nội dung xử lý tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeteleIssueOfShiftDetail(int id, int userID);

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SyncDataIssueOfShiftDetail(IssueOfShiftDetailSyncDataModel requestModel);

        /// <summary>
        /// Xuất dữ liệu tồn tại trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportData(SearchIssueOfShiftModel requestModel);
    }
}
