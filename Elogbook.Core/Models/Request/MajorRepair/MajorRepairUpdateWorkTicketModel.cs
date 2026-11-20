using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin cập nhật phiếu công tác thuộc đợt sửa chữa lớn
    /// </summary>
    public class MajorRepairUpdateWorkTicketModel
    {
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Mã PCT
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Mã Đợt sửa chữa lớn
        /// </summary>
        public int MajorRepairID { get; set; }

        /// <summary>
        /// Thông tin phiếu công tác điện
        /// </summary>
        public PctDienModel PctDienInfo { get; set; }

        /// <summary>
        /// Thông tin phiếu thủy cơ nhiệt hóa
        /// </summary>
        public PctTcnhModel PctTcnhInfo { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Danh sách Thông tin ban quản lý (người cấp phiếu, người cho phép, người chỉ huy, người giám sát, người cảnh giới, người lãnh đạo)
        /// </summary>
        [JsonIgnore]
        public virtual List<AddNewLeadershipUserFetchedModel> LeadershipUsers { get; set; }
    }

    /// <summary>
    /// Thông tin phiếu công tác điện
    /// </summary>
    public class PctDienModel
    {
        /// <summary>
        /// 1.1 Người lãnh đạo công việc
        /// </summary>
        public AddLeadershipUserModel Leader { get; set; }

        /// <summary>
        /// 1.2 Người chỉ huy trực tiếp
        /// </summary>
        public AddLeadershipUserModel Commander { get; set; }

        /// <summary>
        /// 1.3 Nhân viên đơn vị công tác
        /// </summary>
        public int? UnitNumber { get; set; }

        /// <summary>
        /// 1.4 Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// 1.5 Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 1.6 Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 1.6 Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// 1.7 Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Người thực hiện cho phép
        /// </summary>
        public AddLeadershipUserModel Acceptor { get; set; }

        /// <summary>
        /// File đính kèm nếu có
        /// </summary>
        public List<string> AttachmentsID { get; set; }
    }

    /// <summary>
    /// Thông tin phiếu công tác Thủy Cơ Nhiệt Hóa
    /// </summary>
    public class PctTcnhModel
    {
        /// <summary>
        /// Loại phiếu (Thủy, Cơ, Nhiệt, Hóa)
        /// </summary>
        public List<int> WorkTicketFeatures { get; set; }

        /// <summary>
        /// 1. Địa điểm
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// 2. Nội dung
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 3. Phạm vi
        /// </summary>
        public string WorkScope { get; set; }

        /// <summary>
        /// 4 Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 4 Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Nguy hiểm và cảnh báo
        /// </summary>
        public List<AddWarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Kiểm tra các biện pháp an toàn trước khi cho phép công tác
        /// </summary>
        public List<AddAllowedInfoDetailModel> AllowedInfoDetails { get; set; }

        /// <summary>
        /// Kiểm tra các biện pháp an toàn trước khi thực hiện công tác
        /// </summary>
        public List<AddSafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Người thực hiện cho phép
        /// </summary>
        public AddLeadershipUserModel Acceptor { get; set; }

        /// <summary>
        /// File đính kèm nếu có
        /// </summary>
        public List<string> AttachmentsID { get; set; }
    }
}
