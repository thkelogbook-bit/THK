using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmWorkService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorks(AmWorkSearchModel requestModel);

        /// <summary>
        /// Lấy danh sách công việc theo loại công việc
        /// </summary>
        /// <param name="workTypeId"></param>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkByType(int workTypeId, int factoryID);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmWorkModel GetWorkById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateWork(AmWorkAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateWork(AmWorkAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteWork(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isCreate"></param>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        bool CheckDuplicateWork(int id, string name, bool isCreate, int factoryID);
    }
}
