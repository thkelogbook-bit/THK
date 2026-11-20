using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi Kíp vận hành
    /// </summary>
    public interface IOperationCrewBusiness
    {
        /// <summary>
        /// Tìm kiếm tất cả Kíp vận hành theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationCrews(SearchOperationCrewModel requestModel);

        /// <summary>
        /// Tìm kiếm một kíp vận hành theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<OperationCrewModel> GetOperationCrewById(int id);

        /// <summary>
        /// Thêm mới Kíp vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<OperationCrewModel> AddOperationCrew(AddEditOperationCrewModel requestModel);

        /// <summary>
        /// Cập nhật Kíp vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<OperationCrewModel> UpdateOperationCrew(AddEditOperationCrewModel requestModel);

        /// <summary>
        /// Xóa Kíp vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteOperationCrew(int id, int userID);

        /// <summary>
        /// Kiểm tra đã sử dụng kíp vận hành để nhập nhật ký vận hành nào chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseOperationCrew(int id);
    }
}
