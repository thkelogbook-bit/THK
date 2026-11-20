namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RegistrationTicket")]
    public partial class RegistrationTicket : EntityBase
    {
        /// Loại phiếu
        /// 1/kế hoạch 2/Ngoài kế hoạch 3/Xử lý sự cố
        /// </summary>
        [Required]
        public int TicketType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [Required]
        public int FactoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Số phiếu đăng ký công tác
        /// </summary>
        [Required]
        [StringLength(255)]
        public string TicketNumber { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Đơn vị nhận phiếu
        /// </summary>
        [Required]
        public int ReceiveUnitID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(ReceiveUnitID))]
        public virtual Organization ReceiveUnit { get; set; }

        /// <summary>
        /// Căn cứ biên bản số
        /// </summary>
        [StringLength(255)]
        public string RegulationNumber { get; set; }

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public int? RegistrationUnitID { get; set; }
        [ForeignKey(nameof(RegistrationUnitID))]
        [MapperIgnore]
        public virtual Organization RegistrationUnit { get; set; }

        /// <summary>
        /// Hiển thị đơn vị công tác đăng ký
        /// </summary>
        [StringLength(500)]
        public string RegistrationUnitView { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        [StringLength(500)]
        public string Position { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [StringLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// 1/ Nội dung công việc
        /// </summary>
        [Required]
        public string WorkContent { get; set; }

        /// <summary>
        /// 2/ Địa điểm (hoặc thiết bị) tiến hành công tác
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string WorkPlacement { get; set; }

        /// <summary>
        /// 3/ Điều kiện cần có để thực hiện công việc
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string WorkCondition { get; set; }

        /// <summary>
        /// 4/ Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// 4/ Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// 5-6/ Số lượng đơn vị công tác/Số lượng nhân viên đơn vị công tác
        /// </summary>
        [StringLength(255)]
        public string UnitNumberAndUnitStaff { get; set; }

        /// <summary>
        /// 7-8-9-10-11/  Người giám sát an toàn điện (nếu có)/Người chỉ huy trực tiếp
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<Stakeholders> Stakeholders { get; set; }

        /// <summary>
        /// 12/ Các đơn vị QLVH khác có liên quan
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 13/ Những yêu cầu khác
        /// </summary>
        public virtual ICollection<OtherRequirements> OtherRequirements { get; set; }

        /// <summary>
        /// Trạng thái phiếu đăng ký
        /// </summary>
        public int StatusValue { get; set; }
        [ForeignKey(nameof(StatusValue))]
        [MapperIgnore]
        public virtual TicketStatus TicketStatus { get; set; }

        public virtual ICollection<TicketStatusHistory> TicketStatusHistories { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
        [StringLength(500)]
        public string Attachments { get; set; }

        /// <summary>
        /// Địa điểm/Thiết bị
        /// </summary>
        public string LocationDevice { get; set; }

        /// <summary>
        /// lý do
        /// </summary>
        public int Reason { get; set; }

        /// <summary>
        /// Loại nội dung
        /// </summary>
        public int ContentType { get; set; }

        /// <summary>
        /// Nhận diện mối nguy hại và biện pháp an toàn
        /// </summary>
        public virtual ICollection<IdentificationAndSafetyMeasures> IdentificationAndSafetyMeasures { get; set; }

        /// <summary>
        /// True: hằng ngày --  False: Liên tục trong thời gian đăng ký
        /// </summary>
        public bool IsWorkDaily { get; set; }

        /// <summary>
        /// Chọn thời gian làm việc hàng ngày (bắt đầu)
        /// </summary>
        [StringLength(100)]
        public string StartTimeDaily { get; set; }

        /// <summary>
        /// Chọn thời gian làm việc hàng ngày (kết thúc
        /// </summary>
        [StringLength(100)]
        public string EndTimeDaily { get; set; }

        /// <summary>
        /// Những yêu cầu khác
        /// </summary>
        public string OtherContents { get; set; }

        /// <summary>
        /// True: là phiếu đăng ký công tác   False: phiếu đăng ký công tác thuỷ cơ nhiệt hoá
        /// </summary>
        public bool IsCollaborateTicket { get; set; }

        /// <summary>
        /// 1/ là phiếu đăng ký công tác - 2/ phiếu đăng ký công tác thuỷ cơ nhiệt hoá
        /// </summary>
        public int RegistrationTicketType { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc (PCT thuỷ cơ nhiệt hoá)
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<ContentPosition> ContentPositions { get; set; }

        /// <summary>
        /// Mã BBKSHT
        /// </summary>
        public int? InspectionReportID { get; set; }

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

        /// <summary>
        /// ID PCT
        /// </summary>
        public int? WorkTicketID { get; set; }

        /// <summary>
        /// ID Sửa chửa lớn
        /// </summary>
        public int? MajorRepairID { get; set; }
        [ForeignKey(nameof(MajorRepairID))]
        [MapperIgnore]
        public virtual MajorRepair MajorRepair { get; set; }

        /// <summary>
        /// id phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }

        /// <summary>
        /// ID Phiếu đăng ký công tác số lượng lớn
        /// </summary>
        public int? BatchWorkRegID { get; set; }
        [ForeignKey(nameof(BatchWorkRegID))]
        [MapperIgnore]
        public virtual BatchWorkReg BatchWorkReg { get; set; }
    }
}
