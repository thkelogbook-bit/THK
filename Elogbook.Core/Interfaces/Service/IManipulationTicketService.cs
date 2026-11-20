using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Manipulation;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Phiếu thao tác
    /// </summary>
    public interface IManipulationTicketService
    {
        /// <summary>
        /// tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel<TicketSearchMobileModel> Find(SearchTicketMobileModel requestModel);

        /// <summary>
        /// Tìm kiếm PTT trên mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        DataCountModel FindMobileCount(SearchTicketMobileModel requestModel);

        /// <summary>
        /// Tìm kiếm một Phiếu thao tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TicketDetailModel GetTicketById(int id);

        /// <summary>
        /// Xóa PTT đã phê duyệt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        bool DeleteFinalTicket(int id, int currentUserID);

        /// <summary>
        /// Thêm thông tin thao tác hoàn thành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool Update(UpdateSequenceOperationModel requestModel);

        /// <summary>
        /// Hủy phiếu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool Cancel(CancelTicketModel requestModel);

        /// <summary>
        /// Kết thúc phiếu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CloseTicket(CloseTicketModel requestModel);

        /// <summary>
        /// Trả về tất cả phiếu thao tác trên Web theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindManipulationWeb(SearchTicketWebModel requestModel);

        /// <summary>
        /// Xác nhận hoàn thành phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CompleteManipulationWeb(CompleteManipulationModel requestModel);

        /// <summary>
        /// Trả về danh sách trình tự thao tác
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        List<SequenceOperationModel> GetSequenceOperation(int ID);
        /// <summary>
        /// Lấy danh sách user trong ticket
        /// </summary>
        /// <param name="TicketID"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        List<UserInTicketModel> GetUsersInTicket(int TicketID, int Role);

        /// <summary>
        /// In tài liệu phiếu công tác ra file PDF
        /// </summary>
        /// <param name="id">ID phiếu công tác</param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);
        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SyncData(ManipulationTicketSyncDataModel requestModel);

        /// <summary>
        /// Xuất file danh sách Phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage PttExport(SearchTicketWebModel requestModel);
    }
}
