using Elogbook.Core.Enums;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Quản lý tổ chức
    /// </summary>
    public interface IOrganizationService
    {
        /// <summary>
        /// Lấy danh sách tổ chức
        /// </summary>
        /// <returns></returns>
        List<OrganizationModel> GetOrganizations(SearchOrganizationModel requestModel);

        /// <summary>
        /// Thêm mới tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddOrganization(AddEditOrganizationModel requestModel);

        /// <summary>
        /// Cập nhật tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateOrganization(AddEditOrganizationModel requestModel);

        /// <summary>
        /// Xóa tổ chức
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        UpdateStatus DeleteOrganization(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại dữ liệu
        /// </summary>
        /// <param name="level"></param>
        /// <param name="name"></param>
        /// <param name="shortName"></param>
        /// <param name="id"></param>
        /// <param name="parrentID"></param>
        /// <param name="isAddMode"></param>
        bool CheckDuplicateData(string level, string name,string shortName, int id, int? parrentID, bool isAddMode);

        /// <summary>
        /// Thay đổi trạng thái tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        UpdateStatus ChangeStatusOrganization(ChangeOrganizationStatusModel requestModel);
    }
}
