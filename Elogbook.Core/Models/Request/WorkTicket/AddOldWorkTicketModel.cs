using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số để thêm mới Phiếu công tác
    /// </summary>
    public class AddOldWorkTicketModel
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [JsonIgnore]
        public int FactoryID { get; set; }

        /// <summary>
        /// Đơn vị cấp phiếu (Tổ)
        /// </summary>
        public int? ApprovedUnitID { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string ApprovedDate { get; set; }

        /// <summary>
        /// Đặc điểm phiếu (Thủy/ Cơ / Nhiệt/ Hóa)
        /// </summary>
        public int? WorkTicketFeatureID { get; set; }

        /// <summary>
        /// Nhân viên đơn vị công tác
        /// </summary>
        public int? UnitNumber { get; set; }

        /// <summary>
        /// Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Phạm vị công tác
        /// </summary>
        public string WorkScope { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Nguy hiểm và cảnh báo
        /// </summary>
        public List<AddWarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string Commander { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public AddLeadershipUserModel Approver { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public string Leader { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public List<string> AttachmentsID { get; set; }

        /// <summary>
        /// Thông tin cho phép
        /// </summary>
        public AddAllowedInfoModel AllowedInfo { get; set; }

        /// <summary>
        /// Thông tin tiếp nhận nơi làm việc
        /// </summary>
        public AddReceiveWorkplaceModel ReceiveWorkplace { get; set; }

        /// <summary>
        /// Danh sách thông tin làm việc của nhân viên
        /// </summary>
        public virtual List<AddEditWorkerInformationModel> WorkerInformations { get; set; }

        /// <summary>
        /// Công tác hằng ngày của thủy cơ nhiệt hóa
        /// </summary>
        public List<AddWorkDailyModel> WorkDailys { get; set; }

        /// <summary>
        /// Danh sách hình ảnh công tác
        /// </summary>
        public List<string> WorkPictures { get; set; }

        /// <summary>
        /// Kết quả thực hiện
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Thông tin hoàn thành công việc
        /// </summary>
        public AddWorkFinishModel WorkFinish { get; set; }

        /// <summary>
        /// Danh sách Thông tin địa điểm làm việc
        /// </summary>
        public virtual List<AddEditWorkPlacementModel> WorkPlacements { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public List<int> WorkTicketFeatures { get; set; }
    }
}
