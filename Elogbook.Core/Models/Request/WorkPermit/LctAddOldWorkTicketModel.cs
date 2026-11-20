using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Tham số để thêm mới lệnh công tác
    /// </summary>
    public class LctAddOldWorkTicketModel
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Loại lệnh
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [JsonIgnore]
        public int FactoryID { get; set; }

        /// <summary>
        /// Đơn vị cấp lệnh (Tổ)
        /// </summary>
        public int? ApprovedUnitID { get; set; }

        /// <summary>
        /// Ngày cấp lệnh
        /// </summary>
        public string ApprovedDate { get; set; }

        /// <summary>
        /// Đặc điểm lệnh (Thủy/ Cơ / Nhiệt/ Hóa)
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
        public List<LctAddWarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string Commander { get; set; }

        /// <summary>
        /// Thời gian Người chỉ huy ký
        /// </summary>
        public string TimeCommanderSigned { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public LctAddLeadershipUserModel Approver { get; set; }

        /// <summary>
        /// Danh sách nhân viên đơn vị công tác và thay đổi người (nếu có)
        /// </summary>
        public List<LctAddLeadershipUserModel> EmployeeListByWorkUnit { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public string Leader { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public List<string> AttachmentsID { get; set; }

        ///// <summary>
        ///// Thông tin cho phép
        ///// </summary>
        //public LctAddAllowedInfoModel AllowedInfo { get; set; }

        /// <summary>
        /// Thông tin tiếp nhận nơi làm việc
        /// </summary>
        public LctAddReceiveWorkplaceModel ReceiveWorkplace { get; set; }

        /// <summary>
        /// Danh sách thông tin làm việc của nhân viên
        /// </summary>
        public virtual List<LctAddEditWorkerInformationModel> WorkerInformations { get; set; }

        /// <summary>
        /// Công tác hằng ngày của thủy cơ nhiệt hóa
        /// </summary>
        public List<LctAddWorkDailyModel> WorkDailys { get; set; }

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
        public LctAddWorkFinishModel WorkFinish { get; set; }

        /// <summary>
        /// Danh sách Thông tin địa điểm làm việc
        /// </summary>
        public virtual List<LctAddEditWorkPlacementModel> WorkPlacements { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Loại lệnh
        /// </summary>
        public List<int> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Thông tin thi hành lệnh/ Công tác hàng ngày
        /// </summary>
        public LctAddWorkDailyInfoModel LctAddWorkDailyInfo { get; set; }
    }
}
