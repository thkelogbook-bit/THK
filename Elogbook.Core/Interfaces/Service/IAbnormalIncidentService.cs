using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Bất thường sự cố
    /// </summary>
    public interface IAbnormalIncidentService
    {
        /// <summary>
        /// Tìm kiếm bất thường sự cố
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchAbnormalIncidentModel requestModel);

        /// <summary>
        /// Cập nhật thông tin xử lý bất thường sự cố
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel UpdateAbnormalIncidentDetail(AbnormalIncidentDetailUpdate requestModel);

        /// <summary>
        /// Tạo thông tin xử lý bất thường sự cố
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddAbnormalIncidentDetail(AbnormalIncidentDetailAdd requestModel);

        /// <summary>
        /// Lấy chi tiết bất thường sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel GetAbnormalIncidentById(int id, int userID);

        /// <summary>
        /// Xóa bất thường sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteAbnormalIncident(int id, int userID);

        /// <summary>
        /// Xóa nội dung xử lý bất thường sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeteleAbnormalIncidentDetail(int id, int userID);

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SyncDataAbnormalIncidentDetail(AbnormalIncidentDetailSyncDataModel requestModel);
    }
}
