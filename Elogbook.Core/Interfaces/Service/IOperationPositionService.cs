using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Cương vị
    /// </summary>
    public interface IOperationPositionService
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
        OperationPositionModel GetOperationPositionById(int id);

        /// <summary>
        /// Thêm mới Cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddOperationPosition(AddEditOperationPositionModel requestModel);

        /// <summary>
        /// Cập nhật Cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateOperationPosition(AddEditOperationPositionModel requestModel);

        /// <summary>
        /// Xóa Cương vị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteOperationPosition(int id, int userID);

        /// <summary>
        /// Kiểm tra Cương vị có đang ghi nhận nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseOperationPosition(int id);

        /// <summary>
        /// Kiểu tra tồn tại Cương vị có tồn tại trong nhà máy
        /// </summary>
        /// <param name="operationPositionID"></param>
        /// <param name="operationPositionName"></param>
        /// <param name="factoryID"></param>
        /// <param name="isAddMode"></param>
        /// <param name="jobTitle"></param>
        /// <returns></returns>
        ValueLabelModel CheckDuplicateData(int operationPositionID, string operationPositionName, int factoryID, bool isAddMode, int jobTitle);
    }
}
