using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Manipulation;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Phiếu thao tác
    /// </summary>
    public interface IManipulationTicketBusiness
    {
        /// <summary>
        /// Tìm kiếm phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel<TicketSearchMobileModel> Find(SearchTicketMobileModel requestModel);
        /// <summary>
        /// Lấy số đếm các PTT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<DataCountModel> FindMobileCount(SearchTicketMobileModel requestModel);

        /// <summary>
        /// Trả về một phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<TicketDetailModel> GetWorkTicketById(int id);

        /// <summary>
        /// Xóa PTT đã phê duyệt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteFinalTicket(int id, int currentUserID);

        /// <summary>
        /// Cập nhật thao tác hàng ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateWork(UpdateSequenceOperationModel requestModel);

        /// <summary>
        /// Từ chối cấp phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> Cancel(CancelTicketModel requestModel);

        /// <summary>
        /// Kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CloseTicket(CloseTicketModel requestModel);

        /// <summary>
        /// Xác nhận hoàn thành phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CompleteManipulationWeb(CompleteManipulationModel requestModel);

        /// <summary>
        /// Trả về tất cả phiếu thao tác trên Web theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindManipulationWeb(SearchTicketWebModel requestModel);

        /// <summary>
        /// Lấy trình tự thao tác
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        ResponseModel<List<SequenceOperationModel>> GetSequenceOperation(int ID);

        /// <summary>
        /// Lấy danh sách user trong ticket
        /// </summary>
        /// <param name="TicketID"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        ResponseModel<List<UserInTicketModel>> GetUsersInTicket(int TicketID, int Role);

        /// <summary>
        /// In tài liệu phiếu công tác ra file PDF
        /// </summary>
        /// <param name="id">ID phiếu công tác</param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);

        /// <summary>
        /// Đồng bộ PTT
        /// </summary>
        /// <param name="requestModel">Data PTT</param>
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
