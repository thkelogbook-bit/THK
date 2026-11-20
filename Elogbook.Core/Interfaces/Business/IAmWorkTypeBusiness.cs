using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmWorkTypeBusiness
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
        ResponseModel<AmWorkTypeModel> GetWorkTypeById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateWorkType(AmWorkTypeAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateWorkType(AmWorkTypeAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteWorkType(int id, int userID);

        /// <summary>
        /// Danh sách loại công việc theo nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        List<ComboboxDataModel> GetWorkTypeByFactoryID(int factoryID);
    }
}
