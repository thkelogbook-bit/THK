using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Nhà máy
    /// </summary>
    public interface IFactoryService
    {
        /// <summary>
        /// Tìm kiếm tất cả Nhà máy theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetFactorys(SearchFactoryModel requestModel);

        /// <summary>
        /// Tìm kiếm Nhà máy theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FactoryModel GetFactoryById(int id);

        /// <summary>
        /// Thêm mới Nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddFactory(AddEditFactoryModel requestModel);

        /// <summary>
        /// Cập nhật Nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateFactory(AddEditFactoryModel requestModel);

        /// <summary>
        /// Xóa Nhà máy
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteFactory(int id, int userID);

        /// <summary>
        /// Kiểm tra Nhà máy có đang dùng ở Cương vị không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseFactory(int id);

        /// <summary>
        /// Kiểm tra công ty có tồn tại trong hệ thống
        /// Kiểm tra phân xưởng có nằm trong hệ thống và thuộc công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="productionPlantID"></param>
        /// <returns></returns>
        bool CheckExistData(int companyID, int productionPlantID);

        /// <summary>
        /// Kiểu tra tồn tại Nhà máy trong Công ty
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="factoryName"></param>
        /// <param name="companyID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int factoryID, string factoryName, int companyID, bool isAddMode);

        /// <summary>
        /// Parse ticket format
        /// </summary>
        /// <param name="ticketFormat"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<ValueLabelModel> ParseTicketFormat(string ticketFormat, string type);
    }
}
