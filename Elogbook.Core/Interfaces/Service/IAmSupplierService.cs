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
    public interface IAmSupplierService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetSuppliers(AmSupplierSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmSupplierModel GetSupplierById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CreateSupplier(AmSupplierAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateSupplier(AmSupplierAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteSupplier(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isCode"></param>
        /// <returns></returns>
        bool CheckDuplicateSupplier(string text, bool isCode);

        /// <summary>
        /// Đồng bộ Pmis
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel SyncDataPmis();
        
        /// <summary>
        /// Danh sách
        /// </summary>
        /// <returns></returns>
        List<ComboboxDataModel> GetAllSupplier();
    }
}
