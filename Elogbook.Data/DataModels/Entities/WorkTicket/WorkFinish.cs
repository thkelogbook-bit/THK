namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WorkFinish")]
    public partial class WorkFinish : EntityBase
    {
        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkTicketID))]
        public virtual WorkTicket WorkTicket { get; set; }

        /// <summary>
        /// Danh sách hình ảnh
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<WorkFinishPicture> WorkFinishPictures { get; set; }

        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public DateTime? ReturnWorkplaceTime { get; set; }

        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa phiếu
        /// </summary>
        public DateTime? CheckReceivedTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành phiếu công tác
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
        /// Chữ ký người cấp phiếu
        /// </summary>
        public int? ApproverSignature { get; set; }

        /// <summary>
        /// Chữ ký người cấp phiếu
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
        /// Mọi biển báo, cảnh báo an toàn đã gỡ hết
        /// </summary>
        public bool? OperatorConfirmed1 { get; set; }

        /// <summary>
        /// Thiết bị đủ điều kiện đưa trở lại vận hành
        /// </summary>
        public bool? OperatorConfirmed2 { get; set; }

        /// <summary>
        /// Khóa phiếu công tác này
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
