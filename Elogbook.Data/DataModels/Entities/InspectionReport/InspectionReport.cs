namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("InspectionReport")]
    public partial class InspectionReport : EntityBase
    {
        /// <summary>
        /// Số BBKSHT
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [Required]
        public int? FactoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Tên đơn vị công tác
        /// </summary>
        public string ProductionPlantName { get; set; }

        /// <summary>
        /// Tên đơn vị công tác
        /// </summary>
        public int? ProductionPlantID { get; set; }
        [ForeignKey(nameof(ProductionPlantID))]
        [MapperIgnore]
        public virtual Organization ProductionPlant { get; set; }

        /// <summary>
        /// Thời gian khảo sát
        /// </summary>
        public DateTime? SurveyTime { get; set; }

        /// <summary>
        /// , tại .....................................................................................................
        /// </summary>
        public string Address { get; set; }

        //1. Đại diện Đơn vị công tác
        //2. Đại diện (các) Đơn vị QLVH
        //3. Đại diện các đơn vị có liên quan
        [MapperIgnore]
        public virtual ICollection<RelatedUser> RelatedUsers { get; set; }

        /// <summary>
        /// 4. Địa điểm (hoặc thiết bị) thực hiện công việc
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// 5. Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 6. Phạm vi làm việc
        /// </summary>
        public string InspectionScope { get; set; }

        /// <summary>
        /// 7. Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime? PlanStartDate { get; set; }

        /// <summary>
        /// 7. Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime? PlanEndDate { get; set; }

        /// <summary>
        /// 8. Trách nhiệm của các đơn vị liên quan
        /// a/ Đối với (các) Đơn vị QLVH
        /// </summary>
        public string OperationUnitResponsibilities { get; set; }

        /// <summary>
        /// b/ Đối với Đơn vị công tác
        /// </summary>
        public string WorkUnitResponsibilities { get; set; }

        /// <summary>
        /// c/ Đối với đơn vị điều độ
        /// </summary>
        public string DispatchUnitResponsibilities { get; set; }

        /// <summary>
        /// d/ Những nội dung khác có liên quan đến công việc
        /// </summary>
        public string OtherWorkRelatedMatters { get; set; }

        /// <summary>
        /// Thời gian xác nhận hoàn thành biên bản khảo sát hiện trường
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public int StatusValue { get; set; }

        /// <summary>
        /// Danh sách tệp đính kèm
        /// </summary>
        public string Attachments { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}