using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi Nhà máy
    /// </summary>
    public interface IFactoryBusiness
    {
        /// <summary>
        /// Tìm kiếm tất cả Nhà máy theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetFactorys(SearchFactoryModel requestModel);

        /// <summary>
        /// Tìm kiếm một Nhà máy theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<FactoryModel> GetFactoryById(int id);

        /// <summary>
        /// Thêm mới Nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddFactory(AddEditFactoryModel requestModel);

        /// <summary>
        /// Cập nhật Nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateFactory(AddEditFactoryModel requestModel);

        /// <summary>
        /// Xóa Nhà máy
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteFactory(int id, int userID);

        /// <summary>
        /// Kiểm tra Nhà máy có đang dùng ở Cương vị không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseFactory(int id);
    }
}
