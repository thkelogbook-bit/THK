namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DeliveryReceiveShift")]
    public partial class DeliveryReceiveShift : EntityBase
    {
        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationDiaryID))]
        public virtual OperationDiary OperationDiary { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        [StringLength(100)]
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)
        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// App ID
        /// </summary>
        [StringLength(100)]
        public string AppID { get; set; }

        /// <summary>
        /// Xác định cương vị trong ca được khai báo khi ghi nhật ký vận hành
        /// </summary>
        public int OperationDiaryPositionShiftID { get; set; }

        /// <summary>
        /// Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public int SupervisorID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(SupervisorID))]
        public virtual User Supervisor { get; set; }

        /// <summary>
        /// Xác định người giao ca/nhận ca, true thì người đó giao ca và ngược lại
        /// </summary>
        public bool IsDeliveryShift { get; set; }

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
