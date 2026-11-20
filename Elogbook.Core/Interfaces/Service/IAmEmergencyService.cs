using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// định nghĩa các phương thức cho mỗi mức độ khẩn cấp
    /// </summary>
    public interface IAmEmergencyService
    {
        /// <summary>
        /// CheckDuplicateData
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(AmAddEditEmergencyModel requestModel, bool isAddMode);

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        AmEmergencyModel Add(AmAddEditEmergencyModel requestModel);

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmEmergencyModel GetById(int id);

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
        AmEmergencyModel Update(AmAddEditEmergencyModel requestModel);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool Delete(int id, int currentUserId);

        /// <summary>
        /// CheckDataIsUse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckDataIsUse(int id);
    }
}
