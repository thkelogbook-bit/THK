using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Model for searching SLA configurations
    /// </summary>
    public class SearchSlaConfigModel : BaseListFilter
    {
        /// <summary>
        /// SLA configuration name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Factory ID
        /// </summary>
        public int FactoryID { get; set; } = -1;

        /// <summary>
        /// SLA category
        /// </summary>
        public int CategoryID { get; set; } = -1;

        /// <summary>
        /// Filter by active status
        /// </summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Model for adding/editing SLA configuration
    /// </summary>
    public class AddEditSlaConfigModel
    {
        /// <summary>
        /// SLA configuration ID (for editing)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy ID (Factory ID)
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Configuration category (e.g., 'PCT')
        /// </summary>
        public int CategoryID { get; set; }

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
        /// Danh sách cấu hình cấp độ SLA liên quan đến cấu hình SLA này
        /// </summary>
        public List<AddEditSlaLevelModel> Levels { get; set; } = new List<AddEditSlaLevelModel>();

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }

    /// <summary>
    /// Model for adding/editing SLA configuration
    /// </summary>
    public class AddEditSlaLevelModel
    {
        /// <summary>
        /// Level ID (for updates)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Cấu hình SLA
        /// </summary>
        public int SlaConfigID { get; set; }

        /// <summary>
        /// Thứ tự cấp độ (1, 2, 3...)
        /// </summary>
        public int LevelOrder { get; set; }

        /// <summary>
        /// Cấu hình điều kiện SLA
        /// </summary>
        public int SlaConditionID { get; set; }

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
        /// Danh sách vai trò SLA
        /// </summary>
        //public List<AddEditSlaRoleModel> Roles { get; set; } = new List<AddEditSlaRoleModel>();

        /// <summary>
        /// Có gửi Email thông báo hay không
        /// </summary>
        public bool SendEmail { get; set; }

        /// <summary>
        /// ID mẫu Email
        /// </summary>
        public int? EmailTemplateId { get; set; }

        /// <summary>
        /// Danh sách Nhóm người dùng
        /// </summary>
        public List<AddEditSlaUserGroupModel> UserGroups { get; set; } = new List<AddEditSlaUserGroupModel>();

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }

    /// <summary>
    /// Model for adding/editing SLA role
    /// </summary>
    public class AddEditSlaRoleModel
    {
        /// <summary>
        /// ID vai trò SLA
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên viết tắt vai trò SLA (e.g., 'CHTT', 'NCP')
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên đầy đủ vai trò SLA (e.g., 'Chỉ huy trực tiếp', 'Người cho phép')
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    ///Nhóm người dùng
    /// </summary>
    public class AddEditSlaUserGroupModel
    {
        /// <summary>
        /// ID nhóm người dùng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên nhóm người dùng
        /// </summary>
        public string Name { get; set; }
    }
}
