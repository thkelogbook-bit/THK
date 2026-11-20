using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Quản lý tổ chức
    /// </summary>
    public interface IOrganizationBusiness
    {
        /// <summary>
        /// Lấy danh sách tổ chức
        /// </summary>
        /// <returns></returns>
        ResponseModel<List<OrganizationModel>> GetOrganizations(SearchOrganizationModel requestModel);

        /// <summary>
        /// Thêm mới tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddOrganization(AddEditOrganizationModel requestModel);

        /// <summary>
        /// Cập nhật tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateOrganization(AddEditOrganizationModel requestModel);

        /// <summary>
        /// Xóa tổ chức
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<int> DeleteOrganization(int id, int userID);

        /// <summary>
        /// Thay đổi trạng thái tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> ChangeStatusOrganization(ChangeOrganizationStatusModel requestModel);
    }
}
