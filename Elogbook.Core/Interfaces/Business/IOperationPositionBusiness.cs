using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi Cương vị
    /// </summary>
    public interface IOperationPositionBusiness
    {
        /// <summary>
        /// Tìm kiếm tất cả Cương vị theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationPositions(SearchOperationPositionModel requestModel);

        /// <summary>
        /// Tìm kiếm một Cương vị theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<OperationPositionModel> GetOperationPositionById(int id);

        /// <summary>
        /// Thêm mới Cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddOperationPosition(AddEditOperationPositionModel requestModel);

        /// <summary>
        /// Cập nhật Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateOperationPosition(AddEditOperationPositionModel requestModel);

        /// <summary>
        /// Xóa Cương vị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteOperationPosition(int id, int userID);

        /// <summary>
        /// Kiểm tra Cương vị có đang ghi nhận nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseOperationPosition(int id);
    }
}
