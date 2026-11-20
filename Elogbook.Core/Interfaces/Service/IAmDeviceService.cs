using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.AmDevice;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmDeviceService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetDevices(AmDeviceSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmDeviceModel GetDeviceById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateDevice(AmDeviceAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateDevice(AmDeviceAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteDevice(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isCode"></param>
        /// <returns></returns>
        bool CheckDuplicateDevice(string text, bool isCode);

        /// <summary>
        /// Đồng bộ Pmis
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel SyncDataPmis();

        /// <summary>
        /// Danh sách thiết bị cha theo khu vực
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        List<ComboboxDataModel> GetDeviceByLocationID(int locationID);

        /// <summary>
        /// Danh sách Thiết bị cha
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="deviceTypeID"></param>
        /// <returns></returns>
        List<ComboboxDataModel> GetParent(int locationID, int deviceTypeID);
        
        /// <summary>
        /// Tìm kiếm nhanh thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel QuickSearchDevices(AmDeviceQuickSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm thiết bị theo cấu trúc phân cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetDevicesHierarchy(AmDeviceSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm thiết bị cha cao nhất
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetRootDevices(AmDeviceSearchModel requestModel);

        /// <summary>
        /// Lấy cây phân cấp khu vực - thiết bị
        /// </summary>
        /// <returns></returns>
        List<BaseHierachyModel> GetLocationDeviceHierarchy();

        /// <summary>
        /// Thay đổi parent của Device hoặc Location
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel ChangeParent(ChangeParentLocationModel requestModel);

        /// <summary>
        /// Lấy danh sách thiết bị con
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<AmDeviceModel> GetDeviceChildren(int id);
    }
}
