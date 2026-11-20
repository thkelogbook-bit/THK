namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using Elogbook.Data.DataModels.Entities.WorkPermit;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctWorkTicket")]
    public partial class LctWorkTicket : EntityBase
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        [Required]
        [StringLength(100)]
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Lệnh đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual LctRegistrationTicket RegistrationTicket { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }
        [ForeignKey(nameof(FactoryID))]
        [MapperIgnore]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Đơn vị cấp lệnh (cấp phân xưởng)
        /// </summary>
        public int? ApprovedUnitID { get; set; }
        [ForeignKey(nameof(ApprovedUnitID))]
        [MapperIgnore]
        public virtual Organization ApprovedUnit { get; set; }

        /// <summary>
        /// Ngày cấp lệnh
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// Thời gian Người chỉ huy ký
        /// </summary>
        public DateTime? TimeCommanderSigned { get; set; }

        /// <summary>
        /// Loại lệnh
        /// </summary>
        public int WorkTicketType { get; set; }

        /// <summary>
        /// Trạng thái lệnh công tác
        /// </summary>
        public int WorkTicketStatus { get; set; }
        [ForeignKey(nameof(WorkTicketStatus))]
        [MapperIgnore]
        public virtual LctTicketStatus TicketStatus { get; set; }

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
        /// Phạm vi làm việc
        /// </summary>
        public string WorkScope { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Thông tin làm việc của nhân viên
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctWorkerInformation> WorkerInformations { get; set; }

        /// <summary>
        /// Thông tin địa điểm làm việc công tác hằng ngày
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctWorkPlacement> WorkPlacements { get; set; }

        /// <summary>
        /// Thông tin ban quản lý (người cấp lệnh, người cho phép, người chỉ huy, người giám sát, người cảnh giới, người lãnh đạo)
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctLeadershipUser> LeadershipUsers { get; set; }

        /// <summary>
        /// FIle đính kèm
        /// </summary>
        [StringLength(500)]
        public string Attachments { get; set; }

        /// <summary>
        /// Hình ảnh quá trình công tác
        /// </summary>
        public string WorkPictures { get; set; }

        /// <summary>
        /// Kết quả công việc
        /// </summary>
        public string WorkResult { get; set; }

        /// <summary>
        /// Cảnh báo nguy hiểm và biện pháp an toàn
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctWarnAndSafe> WarnAndSafes { get; set; }

        /// <summary>
        /// Các loại lệnh
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctWorkTicketFeatureLink> WorkTicketFeatureLinks { get; set; }

        /// <summary>
        /// Công tác hằng ngày
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctWorkDaily> WorkDailys { get; set; }

        /// <summary>
        /// Trạng thái lệnh công tác
        /// </summary>
        public int? WorkTicketFeatureID { get; set; }
        [ForeignKey(nameof(WorkTicketFeatureID))]
        [MapperIgnore]
        public virtual LctWorkTicketFeatures WorkTicketFeature { get; set; }

        /// <summary>
        /// Đánh dấu những PCT cần chú ý
        /// </summary>
        public bool IsFlagNote { get; set; }

        /// <summary>
        /// File lệnh đã cấp
        /// </summary>
        public int? TicketPdfID { get; set; }

        /// <summary>
        /// Danh sách nhân viên đơn vị công tác và thay đổi người (nếu có)
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctEmployeeByWorkUnit> EmployeeListByWorkUnit { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
