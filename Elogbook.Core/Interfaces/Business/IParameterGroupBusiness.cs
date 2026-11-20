using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi Nhóm thông số
    /// </summary>
    public interface IParameterGroupBusiness
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
        ResponseModel<ParameterGroupModel> GetParameterGroupById(int id);

        /// <summary>
        /// Thêm mới Nhóm thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddParameterGroup(AddEditParameterGroupModel requestModel);

        /// <summary>
        /// Cập nhật Nhóm thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateParameterGroup(AddEditParameterGroupModel requestModel);

        /// <summary>
        /// Xóa Nhóm thông số
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteParameterGroup(int id, int userID);

        /// <summary>
        /// Kiểm tra Nhóm thông số có đang sử dụng để cấu hình thông số không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseParameterGroup(int id);
    }
}
