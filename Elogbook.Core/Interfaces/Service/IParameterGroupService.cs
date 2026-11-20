using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Nhóm thông số
    /// </summary>
    public interface IParameterGroupService
    {
        /// <summary>
        /// Tìm kiếm tất cả Nhóm thông số theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetParameterGroups(SearchParameterGroupModel requestModel);

        /// <summary>
        /// Tìm kiếm một Nhóm thông số theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ParameterGroupModel GetParameterGroupById(int id);

        /// <summary>
        /// Thêm mới Nhóm thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddParameterGroup(AddEditParameterGroupModel requestModel);

        /// <summary>
        /// Cập nhật Nhóm thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateParameterGroup(AddEditParameterGroupModel requestModel);

        /// <summary>
        /// Xóa Nhóm thông số
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteParameterGroup(int id, int userID);

        /// <summary>
        /// Kiểm tra Nhóm thông số có đang sử dụng để cấu hình thông số không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseParameterGroup(int id);

        /// <summary>
        /// Kiểu tra tồn tại Nhóm thông số trong Nhà máy
        /// </summary>
        /// <param name="parameterGroupID"></param>
        /// <param name="parameterGroupName"></param>
        /// <param name="operationPositionID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int parameterGroupID, string parameterGroupName, int operationPositionID, bool isAddMode);
    }
}
