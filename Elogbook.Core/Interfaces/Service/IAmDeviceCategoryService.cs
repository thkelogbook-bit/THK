using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// định nghĩa phương thức service của PLTB
    /// </summary>
    public interface IAmDeviceCategoryService
    {
        /// <summary>
        /// kiểm tra trùng data
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(AddEditAmDeviceCategoryModel requestModel, bool isAddMode);

        /// <summary>
        /// Thêm mới phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        AmDeviceCategoryModel Add(AddEditAmDeviceCategoryModel requestModel);

        /// <summary>
        /// Trả về phân loại thiết bị theo mã số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmDeviceCategoryModel GetById(int id);

        /// <summary>
        /// Trả về tất cả phân loại thiết bị theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchAmDeviceCategoryModel requestModel);

        /// <summary>
        /// xóa phân loại thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool Delete(int id, int currentUserId);

        /// <summary>
        /// Cập nhập phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<AmDeviceCategoryModel> Update(AddEditAmDeviceCategoryModel requestModel);

        /// <summary>
        /// Trả về tất cả phân loại thiết bị cha theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindParent(SearchParentAmDeviceCategoryModel requestModel);

        /// <summary>
        /// sync pmis
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel SyncPmis();
        
        /// <summary>
        /// Check công ty
        /// </summary>
        /// <param name="orgID"></param>
        /// <returns></returns>
        bool GetOrgInf(int orgID);

        /// <summary>
        /// CheckExistsParent
        /// </summary>
        /// <param name="categoryParent"></param>
        /// <returns></returns>
        bool CheckExistsParent(int categoryParent);

        /// <summary>
        /// Get danh sách phân loại thiết bị theo công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="isParent"></param>
        /// <returns></returns>
        List<ComboboxDataModel> GetDeviceCategoriesByCompany(int companyID, bool isParent);
    }
}
