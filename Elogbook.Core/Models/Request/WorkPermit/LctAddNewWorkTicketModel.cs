using Elogbook.Core.Mapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một lệnh công tác
    /// </summary>
    public class LctAddNewWorkTicketModel
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [JsonIgnore]
        public int? FactoryID { get; set; }

        /// <summary>
        /// lệnh đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Đơn vị cấp lệnh (phân xưởng)
        /// </summary>
        public int ApprovedUnitID { get; set; }

        /// <summary>
        /// Ngày cấp lệnh
        /// </summary>
        public DateTime ApprovedDate { get; set; }

        /// <summary>
        /// Loại lệnh
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public LctAddLeadershipUserModel Leader { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public LctAddLeadershipUserModel Commander { get; set; }

        /// <summary>
        /// Danh sách Thông tin ban quản lý (người cấp lệnh, người cho phép, người chỉ huy, người giám sát, người cảnh giới, người lãnh đạo)
        /// </summary>
        [JsonIgnore]
        public virtual List<LctAddNewLeadershipUserFetchedModel> LeadershipUsers { get; set; }

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
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// File đính kèm nếu có
        /// </summary>
        public List<string> AttachmentsID { get; set; }

        /// <summary>
        /// Phạm vi
        /// </summary>
        public string WorkScope { get; set; }

        /// <summary>
        /// Tính năng
        /// </summary>
        public int? WorkTicketFeatureID { get; set; }

        /// <summary>
        /// Nguy hiểm và cảnh báo
        /// </summary>
        public List<LctAddWarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Kiểm tra các biện pháp an toàn trước khi thực hiện công tác
        /// </summary>
        public List<LctAddSafetyMeasureModel> SafetyMeasures { get; set; }

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
        /// Danh sách nhân viên đơn vị công tác và thay đổi người (nếu có)
        /// </summary>
        public List<LctAddLeadershipUserModel> EmployeeListByWorkUnit { get; set; }

        /// <summary>
        /// Tên đơn vị công tác
        /// </summary>
        public string WorkUnitName { get; set; }

        /// <summary>
        /// Đơn vị công tác
        /// </summary>
        public int? WorkUnitID { get; set; }

    }
}
