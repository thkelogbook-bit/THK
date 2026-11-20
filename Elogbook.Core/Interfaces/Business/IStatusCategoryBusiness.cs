using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho danh mục trạng thái
    /// </summary>
    public interface IStatusCategoryBusiness
    {
        /// <summary>
        /// Danh sách trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetStatusCategories(SearchStatusCategoryModel requestModel);

        /// <summary>
        /// Chi tiết trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<StatusCategoryModel> GetStatusCategoryById(int id);

        /// <summary>
        /// Thêm mới trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<StatusCategoryModel> AddStatusCategory(AddEditStatusCategoryModel requestModel);

        /// <summary>
        /// Cập nhật trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<StatusCategoryModel> UpdateStatusCategory(AddEditStatusCategoryModel requestModel);

        /// <summary>
        /// Xóa trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteStatusCategory(int id, int userID);

        /// <summary>
        /// Kiểm tra trạng thái đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseStatusCategory(int id);
    }
}
