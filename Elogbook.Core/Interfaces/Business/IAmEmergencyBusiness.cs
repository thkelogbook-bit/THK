using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// định nghĩa các phương thức cho mỗi mức độ khẩn cấp
    /// </summary>
    public interface IAmEmergencyBusiness
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<AmEmergencyModel> Add(AmAddEditEmergencyModel requestModel);

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<AmEmergencyModel> GetById(int id);

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(AmSearchEmergencyModel requestModel);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<AmEmergencyModel> Update(AmAddEditEmergencyModel requestModel);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> Delete(int id, int currentUserId);
    }
}
