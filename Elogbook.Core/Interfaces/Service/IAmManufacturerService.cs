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
    public interface IAmManufacturerService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetManufacturers(AmManufacturerSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmManufacturerModel GetManufacturerById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CreateManufacturer(AmManufacturerAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateManufacturer(AmManufacturerAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteManufacturer(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isCode"></param>
        /// <returns></returns>
        bool CheckDuplicateManufacturer(string text, bool isCode);

        /// <summary>
        /// Đồng bộ Pmis
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel SyncDataPmis();
        
        /// <summary>
        /// Danh sách
        /// </summary>
        /// <returns></returns>
        List<ComboboxDataModel> GetAllManufacturer();
    }
}
