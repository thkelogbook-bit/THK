using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmLocationService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetLocations(AmLocationSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmLocationModel GetLocationById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CreateLocation(AmLocationAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateLocation(AmLocationAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteLocation(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isCode"></param>
        /// <returns></returns>
        bool CheckDuplicateLocation(string text, bool isCode);

        /// <summary>
        /// Đồng bộ Pmis
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel SyncDataPmis();

        /// <summary>
        /// Danh sách Khu vực theo công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="isParent"></param>
        /// <returns></returns>
        List<ComboboxDataModel> GetLocationByCompanyID(int companyID, bool isParent);

        /// <summary>
        /// get parent location
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        List<ComboboxDataModel> GetParent(int companyID);
    }
}
