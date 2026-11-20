using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmCountryManufactureBusiness
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetCountryManufactures(AmCountryManufactureSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<AmCountryManufactureModel> GetCountryManufactureById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateCountryManufacture(AmCountryManufactureAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateCountryManufacture(AmCountryManufactureAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteCountryManufacture(int id, int userID);

        /// <summary>
        /// Đồng bộ Pmis
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel SyncDataPmis();

        /// <summary>
        /// Combobox
        /// </summary>
        /// <returns></returns>
        List<ComboboxDataModel> GetAllCountryManufacture();
    }
}
