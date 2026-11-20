using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Model for SLA configuration detail
    /// </summary>
    public class SlaConfigDetailModel
    {
        /// <summary>
        /// Configuration ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Factory information
        /// </summary>
        public SlaFactoryModel Factory { get; set; }

        /// <summary>
        /// Id nhà máy
        /// </summary>
        [JsonIgnore]
        public int FactoryID { get; set; }

        /// <summary>
        /// Configuration category
        /// </summary>
        public SlaCategoryModel Category { get; set; }

        /// <summary>
        /// Configuration name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Configuration description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether configuration is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// List of SLA levels for this configuration
        /// </summary>
        public List<SlaLevelModel> Levels { get; set; }

        /// <summary>
        /// Person who created the configuration
        /// </summary>
        public SlaUserModel CreatedUser { get; set; }

        /// <summary>
        /// Date configuration was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date configuration was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Model for SLA level
    /// </summary>
    public class SlaLevelModel
    {
        /// <summary>
        /// Level ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Level order
        /// </summary>
        public int LevelOrder { get; set; }

        /// <summary>
        /// Condition type: BEFORE / ON_DUE / AFTER
        /// </summary>
        public string ConditionType { get; set; }

        /// <summary>
        /// Condition information
        /// </summary>
        [JsonIgnore]
        public int SlaConditionID { get; set; }

        /// <summary>
        /// Offset days
        /// </summary>
        public int OffsetDays { get; set; }

        /// <summary>
        /// Offset hours
        /// </summary>
        public int OffsetHours { get; set; }

        /// <summary>
        /// Offset minutes
        /// </summary>
        public int OffsetMinutes { get; set; }

        /// <summary>
        /// Include holidays
        /// </summary>
        public bool IncludeHolidays { get; set; }

        /// <summary>
        /// Include weekends
        /// </summary>
        public bool IncludeWeekends { get; set; }

        /// <summary>
        /// Working hours only
        /// </summary>
        public bool WorkingHoursOnly { get; set; }

        /// <summary>
        /// Send email notification
        /// </summary>
        public bool SendEmail { get; set; }

        /// <summary>
        /// Email template ID
        /// </summary>
        public long? EmailTemplateId { get; set; }

        /// <summary>
        /// Role codes associated with this level
        /// </summary>
        public List<SlaRoleModel> RoleCodes { get; set; }

        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public List<SlaUserGroupModel> UserGroups { get; set; }
    }

    /// <summary>
    /// Model for SLA notification log
    /// </summary>
    public class SlaNotificationLogModel
    {
        /// <summary>
        /// Log ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Configuration ID
        /// </summary>
        public int SlaConfigId { get; set; }

        /// <summary>
        /// Level ID
        /// </summary>
        public int SlaLevelId { get; set; }

        /// <summary>
        /// PCT ID
        /// </summary>
        public long PctId { get; set; }

        /// <summary>
        /// Sent date
        /// </summary>
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Channel: EMAIL / APP_NOTIF
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Recipient user ID
        /// </summary>
        public long? RecipientUserId { get; set; }

        /// <summary>
        /// Notification status: SENT / FAILED / SKIPPED
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Model for SLA category
    /// </summary>
    public class SlaCategoryModel
    {
        /// <summary>
        /// ID phân loại SLA
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên viết tắt phân loại SLA (e.g., 'PCT', 'PTT', 'PYC')
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên đầy đủ phân loại SLA
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Model for SLA user
    /// </summary>
    public class SlaUserModel
    {
        /// <summary>
        /// ID người dùng SLA
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// Tên người dùng SLA
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Tên đầy đủ người dùng SLA
        /// </summary>
        public string FullName { get; set; }
    }

    /// <summary>
    /// Model for SLA factory
    /// </summary>
    public class SlaFactoryModel
    {
        /// <summary>
        /// ID nhà máy
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên nhà máy
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Model for SLA condition
    /// </summary>
    public class SlaConditionModel
    {
        /// <summary>
        /// ID điều kiện SLA
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên điều kiện SLA (Trước hạn / Đến hạn / Quá hạn)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả (nếu có)
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Model for SLA condition
    /// </summary>
    public class SlaRoleModel
    {
        /// <summary>
        /// ID vai trò SLA
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên vai trò SLA
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả (nếu có)
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Nhóm người dùng
    /// </summary>
    public class SlaUserGroupModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID nhóm người dùng
        /// </summary>
        public int UserGroupsID { get; set; }

        /// <summary>
        /// Tên nhóm người dùng
        /// </summary>
        public string UserGroupName { get; set; }
    }

    /// <summary>
    /// Model for SLA email template
    /// </summary>
    public class SlaEmailTemplateModel
    {
        /// <summary>
        /// ID mẫu email SLA
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Chủ đề email
        /// </summary>
        public string EmailSubject { get; set; }
    }

    /// <summary>
    /// Model for SLA notification template
    /// </summary>
    public class SlaNotificationTemplateModel
    {
        /// <summary>
        /// ID mẫu thông báo SLA
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Chủ đề thông báo
        /// </summary>
        public string NotificationSubject { get; set; }
    }
}
