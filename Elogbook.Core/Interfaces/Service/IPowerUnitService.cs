using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Tổ máy
    /// </summary>
    public interface IPowerUnitService
    {
        /// <summary>
        /// Tìm kiếm tất cả Tổ máy theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetPowerUnits(SearchPowerUnitModel requestModel);

        /// <summary>
        /// Tìm kiếm Tổ máy theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PowerUnitModel GetPowerUnitById(int id);

        /// <summary>
        /// Thêm mới Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        PowerUnitModel AddPowerUnit(AddEditPowerUnitModel requestModel);

        /// <summary>
        /// Cập nhật Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        PowerUnitModel UpdatePowerUnit(AddEditPowerUnitModel requestModel);

        /// <summary>
        /// Xóa Tổ máy
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeletePowerUnit(int id, int userID);

        /// <summary>
        /// Kiểm tra Tổ máy có đang dùng ở Cương vị không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUsePowerUnit(int id);

        /// <summary>
        /// Kiểu tra tồn tại Tổ máy trong Nhà máy
        /// </summary>
        /// <param name="powerUnitID"></param>
        /// <param name="powerUnitName"></param>
        /// <param name="factoryID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int powerUnitID, string powerUnitName, int factoryID, bool isAddMode);
    }
}
