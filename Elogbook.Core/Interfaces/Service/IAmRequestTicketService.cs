using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// định nghĩa các phương thức cho mỗi phiếu yêu cầu
    /// </summary>
    public interface IAmRequestTicketService
    {
        /// <summary>
        /// kiểm tra trùng lặp phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(AmAddEditRequestTicketModel requestModel, bool isAddMode);

        /// <summary>
        /// Thêm mới phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        AmRequestTicketModel Add(AmAddEditRequestTicketModel requestModel);

        /// <summary>
        /// Trả về phiếu yêu cầu theo mã số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmRequestTicketModel GetById(int id);

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(AmSearchRequestTicketModel requestModel);

        /// <summary>
        /// Cập nhập phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        AmRequestTicketModel Update(AmAddEditRequestTicketModel requestModel);

        /// <summary>
        /// Xóa phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool Delete(int id, int currentUserId);

        /// <summary>
        /// Cập nhật trạng thái phiếu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool UpdateStatus(ChangeStatusRequestModel request);

        /// <summary>
        /// Đếm số phiếu yêu cầu
        /// trạng thái đang xử lý với PYC trưởng ca
        /// trạng thái đã duyệt với PYC đơn vị sửa chữa
        /// </summary>
        /// <returns></returns>
        CountNumberTicketStatusModel GetCountNumberTicket();

        /// <summary>
        /// lấy data tạo phiếu đăng ký Phiếu Điện/TCNH
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isCTD"></param>
        /// <returns></returns>
        DataRegistrationTicketModel GetDataRegistrationTicket(int Id, bool isCTD);

        /// <summary>
        /// lấy data lịch sử phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseBaseModel GetHistory(int id);

        /// <summary>
        /// lấy data thông tin liên quan phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseBaseModel GetRelateInfo(int id);

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindMobile(AmSearchRequestTicketMobileModel requestModel);

        /// <summary>
        /// Trả về tất cả số lượng phiếu yêu cầu theo điều kiện của mỗi tab (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        CountStatusRequestTicketMobileModel FindMobileCount(AmSearchRequestTicketMobileModel requestModel);        
    }
}
