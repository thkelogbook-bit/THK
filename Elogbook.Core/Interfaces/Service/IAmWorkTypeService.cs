using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmWorkTypeService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkTypes(AmWorkTypeSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmWorkTypeModel GetWorkTypeById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CreateWorkType(AmWorkTypeAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateWorkType(AmWorkTypeAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteWorkType(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isCreate"></param>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        bool CheckDuplicateWorkType(int id, string name, bool isCreate, int factoryID);

        /// <summary>
        /// Danh sách loại công việc theo nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        List<ComboboxDataModel> GetWorkTypeByFactoryID(int factoryID);
    }
}
