using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Phương thức xử lý
    /// </summary>
    public interface IMajorRepairService
    {
        /// <summary>
        /// Tìm kiếm thông tin sửa chữa lớn theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetMajorRepairs(MajorRepairSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm thông tin sửa chữa lớn theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MajorRepairModel GetMajorRepairById(int id);

        /// <summary>
        /// Thêm mới thông tin sửa chữa lớn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int CreateMajorRepair(MajorRepairAddEditModel requestModel);

        /// <summary>
        /// Cập nhật thông tin sửa chữa lớn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateMajorRepair(MajorRepairAddEditModel requestModel);

        /// <summary>
        /// Xóa thông tin sửa chữa lớn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteMajorRepair(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <param name="factoryID"></param>
        /// <param name="title"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        bool CheckDuplicateMajorRepair(int id, int factoryID, string title, bool isCreate);

        /// <summary>
        /// Upload file dữ liệu thông tin sửa chữa lớn lên server
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        MajorRepairExcelDataValidationModel UploadFile(MajorRepairUploadInfoModel requestModel);

        /// <summary>
        /// Download file dữ liệu thông tin sửa chữa lớn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage DownloadFile(int id);

        /// <summary>
        /// Danh sách trạng thái các đợt sửa chữa lớn
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
        /// Kiểm tra available update work ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsAvailableUpdateWorkTicket(int id);

        /// <summary>
        /// Trả về trạng thái phiếu công tác thuộc đợt sửa chữa lớn
        /// </summary>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        List<ValueLabelCheckedModel> GetMajorRepairTicketStatus(string ticketType);

        /// <summary>
        /// Download file mẫu thông tin sửa chữa lớn
        /// </summary>
        /// <returns></returns>
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
