using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Cấu hình cấp độ SLA
    /// </summary>
    [Table("SlaLevels")]
    public partial class SlaLevel : EntityBase
    {
        public SlaLevel()
        {
            SlaLevelRoles = new HashSet<SlaLevelRole>();
            SlaLevelUserGroups = new HashSet<SlaLevelUserGroup>();
        }

        /// <summary>
        /// Cấu hình SLA
        /// </summary>
        public int SlaConfigID { get; set; }
        [ForeignKey(nameof(SlaConfigID))]
        [MapperIgnore]
        public virtual SlaConfig SlaConfig { get; set; }

        /// <summary>
        /// Thứ tự cấp độ (1, 2, 3...)
        /// </summary>
        public int LevelOrder { get; set; }

        /// <summary>
        /// Cấu hình điều kiện SLA
        /// </summary>
        public int SlaConditionID { get; set; }
        [ForeignKey(nameof(SlaConditionID))]
        [MapperIgnore]
        public virtual SlaCondition SlaCondition { get; set; }

        /// <summary>
        /// Số ngày offset
        /// </summary>
        public int OffsetDays { get; set; }

        /// <summary>
        /// Số giờ offset
        /// </summary>
        public int OffsetHours { get; set; }

        /// <summary>
        /// Số phút offset
        /// </summary>
        public int OffsetMinutes { get; set; }

        /// <summary>
        /// Tính cả ngày Lễ khi tính thời gian
        /// </summary>
        public bool IncludeHolidays { get; set; }

        /// <summary>
        /// Tính cả thứ 7 và chủ nhật khi tính thời gian
        /// </summary>
        public bool IncludeWeekends { get; set; }

        /// <summary>
        /// Chỉ tính trong giờ làm việc
        /// </summary>
        public bool WorkingHoursOnly { get; set; }

        /// <summary>
        /// Có gửi Email thông báo hay không
        /// </summary>
        public bool SendEmail { get; set; }

        /// <summary>
        /// ID mẫu Email
        /// </summary>
        public int? EmailTemplateID { get; set; }

        [ForeignKey(nameof(EmailTemplateID))]
        [MapperIgnore]
        public virtual EmailTemplate EmailTemplate { get; set; }

        /// <summary>
        /// Các vai trò liên quan đến cấp độ này
        /// </summary>
        public virtual ICollection<SlaLevelRole> SlaLevelRoles { get; set; }

        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public virtual ICollection<SlaLevelUserGroup> SlaLevelUserGroups { get; set; }
    }
}
