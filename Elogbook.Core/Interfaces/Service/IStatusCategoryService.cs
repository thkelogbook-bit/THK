using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của danh mục trạng thái
    /// </summary>
    public interface IStatusCategoryService
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
        StatusCategoryModel GetStatusCategoryById(int id);

        /// <summary>
        /// Thêm mới trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        StatusCategoryModel AddStatusCategory(AddEditStatusCategoryModel requestModel);

        /// <summary>
        /// Cập nhật trạng thái
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        StatusCategoryModel UpdateStatusCategory(AddEditStatusCategoryModel requestModel);

        /// <summary>
        /// Xóa trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteStatusCategory(int id, int userID);

        /// <summary>
        /// Kiểm tra trạng thái đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseStatusCategory(int id);

        /// <summary>
        /// Kiểm tra tồn tại trạng thái trong cùng loại dữ liệu
        /// </summary>
        /// <param name="statusID"></param>
        /// <param name="statusName"></param>
        /// <param name="dataTypeID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int statusID, string statusName, int dataTypeID, bool isAddMode);
    }
}
