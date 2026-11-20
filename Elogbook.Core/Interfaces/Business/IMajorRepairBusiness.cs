using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức quản lý thông tin sửa chữa lớn
    /// </summary>
    public interface IMajorRepairBusiness
    {
        /// <summary>
        /// Tìm kiếm dữ liệu sửa chữa lớn theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetMajorRepairs(MajorRepairSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm dữ liệu sửa chữa lớn theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<MajorRepairModel> GetMajorRepairById(int id);

        /// <summary>
        /// Thêm mới dữ liệu sửa chữa lớn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> CreateMajorRepair(MajorRepairAddEditModel requestModel);

        /// <summary>
        /// Cập nhật dữ liệu sửa chữa lớn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateMajorRepair(MajorRepairAddEditModel requestModel);

        /// <summary>
        /// Xóa dữ liệu sửa chữa lớn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteMajorRepair(int id, int userID);

        /// <summary>
        /// Upload file dữ liệu sửa chữa lớn lên server
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<MajorRepairExcelDataValidationModel> UploadFile(MajorRepairUploadInfoModel requestModel);

        /// <summary>
        /// Download file dữ liệu sửa chữa lớn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage DownloadFile(int id);

        /// <summary>
        /// Lấy trạng thái các đợt sửa chữa lớn
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetMajorRepairStatus();

        /// <summary>
        /// Job insert Workticket from excel data
        /// </summary>
        /// <returns></returns>
        int InsertWorkTicketFromSchedule();

        /// <summary>
        /// Trả về tất cả phiếu công tác thuộc đợt công tác sửa chữa lớn theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindAllWorkTickets(MajorRepairSearchWorkTicketModel requestModel);

        /// <summary>
        /// Trả về trạng thái phiếu công tác thuộc đợt sửa chữa lớn
        /// </summary>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        List<ValueLabelCheckedModel> GetMajorRepairTicketStatus(string ticketType);

        /// <summary>
        /// Download file mẫu sửa chữa lớn
        /// </summary>
        HttpResponseMessage DownloadFileTemplate();

        /// <summary>
        /// Lấy danh sách title sửa chữa lớn
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseBaseModel GetTitleMajorRepair(int factoryID);

        /// <summary>
        /// Cập nhập tạo phiếu sau khi hoàn thành cấp phiếu hàng loạt
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> InsertMultipleTicket(InsertNewTicketModel requestModel);

        /// <summary>
        /// Xóa phiếu công tác điện, phiếu thủy cơ nhiệt hóa của đợt sửa chữa lớn
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteTicket(int workTicketID, int userID);

        /// <summary>
        /// Cập nhật phiếu công tác điện, phiếu thủy cơ nhiệt hóa của đợt sửa chữa lớn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateTicket(MajorRepairUpdateWorkTicketModel requestModel);

        /// <summary>
        /// Get all history
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetMajorRepairHistories(MajorRepairHistorySearchModel requestModel);
    }
}
