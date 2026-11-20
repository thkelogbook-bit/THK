using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.OperationShift;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Quản lý ca vận hành
    /// </summary>
    public interface IOperationShiftService
    {
        /// <summary>
        /// Tìm kiếm ca vận hành theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationShifts(SearchOperationShiftModel requestModel);

        /// <summary>
        /// Tìm kiếm ca vận hành theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OperationShiftModel GetOperationShiftById(int id);

        /// <summary>
        /// Thêm mới ca vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        OperationShiftModel AddOperationShift(AddEditOperationShiftModel requestModel);

        /// <summary>
        /// Cập nhật ca vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        OperationShiftModel UpdateOperationShift(AddEditOperationShiftModel requestModel);

        /// <summary>
        /// Xóa ca vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteOperationShift(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại ca vận hành trong nhà máy
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operationShiftName"></param>
        /// <param name="factoryID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int id, string operationShiftName, int factoryID, bool isAddMode);

        /// <summary>
        /// Kiểm trùng thời gian sử dụng Ca vận hành trong nhà máy
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isAddMode"></param>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        bool CheckDuplicateTime(int id, int factoryID, double startTime, double endTime, bool isAddMode);

        /// <summary>
        /// Kiểm tra Nhóm thông số có đang sử dụng để cấu hình thông số không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseOperationShift(int id);
    }
}
