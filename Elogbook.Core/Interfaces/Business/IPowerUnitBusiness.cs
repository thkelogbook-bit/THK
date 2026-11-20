using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi Tổ máy
    /// </summary>
    public interface IPowerUnitBusiness
    {
        /// <summary>
        /// Tìm kiếm tất cả Tổ máy theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetPowerUnits(SearchPowerUnitModel requestModel);

        /// <summary>
        /// Tìm kiếm một Tổ máy theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<PowerUnitModel> GetPowerUnitById(int id);

        /// <summary>
        /// Thêm mới Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<PowerUnitModel> AddPowerUnit(AddEditPowerUnitModel requestModel);

        /// <summary>
        /// Cập nhật Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<PowerUnitModel> UpdatePowerUnit(AddEditPowerUnitModel requestModel);

        /// <summary>
        /// Xóa Tổ máy
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeletePowerUnit(int id, int userID);

        /// <summary>
        /// Kiểm tra Tổ máy có đang dùng ở Cương vị không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUsePowerUnit(int id);
    }
}
