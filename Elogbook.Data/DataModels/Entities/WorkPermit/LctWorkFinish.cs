namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctWorkFinish")]
    public partial class LctWorkFinish : EntityBase
    {
        /// <summary>
        /// Mã lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual LctWorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Danh sách hình ảnh
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<LctWorkFinishPicture> WorkFinishPictures { get; set; }

        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public DateTime? ReturnWorkplaceTime { get; set; }

        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa lệnh
        /// </summary>
        public DateTime? CheckReceivedTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành lệnh công tác
        /// </summary>
        public DateTime? CheckCompletedTime { get; set; }

        /// <summary>
        /// Chữ ký người lãnh đạo
        /// </summary>
        public int? LeaderSignature { get; set; }
        /// <summary>
        /// Chữ ký người lãnh đạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LeaderSignature))]
        public virtual Signature LeaderSignatureInfo { get; set; }

        /// <summary>
        /// Chữ ký người chỉ hủy
        /// </summary>
        public int? CommanderSignature { get; set; }
        /// <summary>
        /// Chữ ký người chỉ hủy
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CommanderSignature))]
        public virtual Signature CommanderSignatureInfo { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public int? AllowedSignature { get; set; }
        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(AllowedSignature))]
        public virtual Signature AllowedSignatureInfo { get; set; }

        /// <summary>
        /// Chữ ký người cấp lệnh
        /// </summary>
        public int? ApproverSignature { get; set; }

        /// <summary>
        /// Chữ ký người cấp lệnh
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(ApproverSignature))]
        public virtual Signature ApproverSignatureInfo { get; set; }

        /// <summary>
        /// Người đại diện đơn vị quản lý
        /// </summary>
        [StringLength(255)]
        public string RepresentativeUnit { get; set; }

        /// <summary>
        /// Chức vụ Người đại diện đơn vị quản lý
        /// </summary>
        [StringLength(255)]
        public string RepresentativeUnitPositionName { get; set; }

        /// <summary>
        /// Chữ ký  Người đại diện đơn vị quản lý
        /// </summary>
        public int? RepresentativeUnitSignature { get; set; }

        /// <summary>
        /// Chữ ký  Người đại diện đơn vị quản lý
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(RepresentativeUnitSignature))]
        public virtual Signature RepresentativeUnitSignatureInfo { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chức vụ người chỉ huy trực tiếp
        /// </summary>
        public string CommanderPositionName { get; set; }

        /// <summary>
        /// Mọi biển báo, cảnh báo an toàn đã gỡ hết
        /// </summary>
        public bool? OperatorConfirmed1 { get; set; }

        /// <summary>
        /// Thiết bị đủ điều kiện đưa trở lại vận hành
        /// </summary>
        public bool? OperatorConfirmed2 { get; set; }

        /// <summary>
        /// Khóa lệnh công tác này
        /// </summary>
        public bool? OperatorConfirmed3 { get; set; }

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
