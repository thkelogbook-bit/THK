namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperationDiary")]
    public partial class OperationDiary : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Ngày ghi tạo nhật ký vận hành
        /// </summary>
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationPositionID))]
        public virtual OperationPosition OperationPosition { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int? OperationShiftID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationShiftID))]
        public virtual OperationShift OperationShift { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public int? OperationCrewID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationCrewID))]
        public virtual OperationCrew OperationCrew { get; set; }

        /// <summary>
        /// Danh sách cương vị trong ca
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<OperationDiaryPositionShift> OperationDiaryPositionInShift { get; set; }

        /// <summary>
        /// Nhân viên tập sự
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<ProbationaryStaff> ProbationaryStaffs { get; set; }

        /// <summary>
        /// 1.Tình hình vận hành
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<OperationSituation> OperationSituations { get; set; }

        /// <summary>
        /// Công tác đang thực hiện
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<ActiveWork> ActiveWorks { get; set; }

        /// <summary>
        /// 3.Phương thức giao ca
        /// </summary>

        /// <summary>
        /// 4.Tồn tại trong ca
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<ExistInShift> ExistInShifts { get; set; }

        /// <summary>
        /// 4.0 Trường hợp có tồn tại ca thì set là True, ngược lại set False
        /// </summary>
        public bool? IsExistsShift { get; set; }

        /// <summary>
        /// 5.Nhận xét thông số
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 6.Giao/Nhận ca
        /// </summary>
        [MapperIgnore]
        public ICollection<DeliveryReceiveShift> DeliveryReceiveShift { get; set; }

        /// <summary>
        /// Trạng thái nhận ca
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// Trạng thái giao ca
        /// </summary>
        public bool IsDelivered { get; set; }

        /// <summary>
        /// Có thể chỉnh sửa dữ liệu nhật ký vận hành
        /// </summary>
        [MapperIgnore]
        public bool IsCanEdit { get; set; }

        /// <summary>
        /// Ngày ghi tạo nhật ký vận hành
        /// </summary>
        public DateTime? TimeDelivery { get; set; }

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
        /// SyncDataKey
        /// </summary>
        [StringLength(100)]
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)

        /// <summary>
        /// App ID
        /// </summary>
        [StringLength(100)]
        public string AppID { get; set; }

        /// <summary>
        /// Tình hình lúc nhận ca
        /// </summary>
        public string ShiftReceiving { get; set; }

        /// <summary>
        /// PDd file path sau khi ký số
        /// </summary>
        public int? PdfExportId { get; set; }
    }
}
