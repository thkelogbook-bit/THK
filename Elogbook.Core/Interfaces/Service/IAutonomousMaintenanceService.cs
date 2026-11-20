using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAutonomousMaintenanceService
    {
        /// <summary>
        /// Job Create AutonomousMaintenance
        /// </summary>
        /// <returns></returns>
        int CreateAutonomousMaintenance();

        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetAutonomousMaintenances(AutonomousMaintenanceSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AutonomousMaintenanceModel GetAutonomousMaintenanceById(int id);

        /// <summary>
        /// Không thực hiện bảo trì
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateAmStopMaintenance(AmStopMaintenanceAddEditModel requestModel);

        /// <summary>
        /// Thông tin chi tiết thực hiện công việc AM by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmWorkDetailModel GetAmWorkDetailById(int id);

        /// <summary>
        /// Thực hiện AM
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<AmWorkDetailModel> WorkProcessing(AmWorkingProgressAddModel requestModel);

        /// <summary>
        /// Thông tin chi tiết dừng hoặc tạm dừng công việc AM
        /// </summary>
        /// <param name="amWorkDetailID"></param>
        /// <returns></returns>
        AmWorkPendingModel GetAmWorkPendingById(int amWorkDetailID);

        /// <summary>
        /// Cập nhật trạng thái bất thường thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateDeviceAbnormal(AmDeviceAbnormalUpdateModel requestModel);

        /// <summary>
        /// Ký hoàn thành bảo trì tự quản
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> SignatureCompleteAm(AmSignatureCompleteModel requestModel);

        /// <summary>
        /// Xem lý do dừng bảo trì tự quản
        /// </summary>
        /// <param name="autonomousMaintenanceID"></param>
        /// <returns></returns>
        AmStopMaintenanceModel GetAmStopMaintenanceById(int autonomousMaintenanceID);

        /// <summary>
        /// Upload file
        /// </summary>
        /// <returns></returns>
        UploadFileModel UploadFile(FileInfoModel fileInfoModel);

        /// <summary>
        /// lịch sử vận hành sửa chữa
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        ResponseBaseModel FindHistory(int deviceId);

        /// <summary>
        /// Send mail notification
        /// </summary>
        /// <returns></returns>
        int SendMailNotificationAm();

        /// <summary>
        /// Tự động hoàn thành bảo trì tự quản
        /// </summary>
        /// <returns></returns>
        int AutoCompleteAm();

        /// <summary>
        /// Lưu hình ảnh trước và sau thực hiện bảo trì
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel SaveEvidenceAm(SaveImageInfoModel requestModel);
    }
}
