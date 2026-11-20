using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmReasonForStopService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetReasonForStops(AmReasonForStopSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmReasonForStopModel GetReasonForStopById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CreateReasonForStop(AmReasonForStopAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateReasonForStop(AmReasonForStopAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteReasonForStop(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isCreate"></param>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        bool CheckDuplicateReasonForStop(int id, string name, bool isCreate, int factoryID);

        /// <summary>
        /// CheckFactoryExist
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        bool CheckFactoryExist(int factoryID);
    }
}
