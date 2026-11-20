using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Service interface for SLA configuration management
    /// </summary>
    public interface ISlaConfigService
    {
        /// <summary>
        /// Get all SLA configurations with pagination and filtering
        /// </summary>
        ResponseBaseModel GetSlaConfigs(SearchSlaConfigModel requestModel);

        /// <summary>
        /// Get SLA configuration details by ID
        /// </summary>
        SlaConfigDetailModel GetSlaConfigById(int id);

        /// <summary>
        /// Create new SLA configuration
        /// </summary>
        int AddSlaConfig(AddEditSlaConfigModel requestModel);

        /// <summary>
        /// Update existing SLA configuration
        /// </summary>
        bool UpdateSlaConfig(AddEditSlaConfigModel requestModel);

        /// <summary>
        /// Delete SLA configuration
        /// </summary>
        bool DeleteSlaConfig(int id, int currentUserID);

        /// <summary>
        /// Toggle SLA configuration active status
        /// </summary>
        bool ToggleSlaConfigStatus(int id, int currentUserID);

        /// <summary>
        /// Check whether an SLA configuration with the same name, category and factory exists.
        /// excludeId: optional ID to exclude (useful when updating)
        /// </summary>
        bool ExistsByNameCategoryFactory(string name, int categoryId, int factoryId, int? excludeId = null);

        /// <summary>
        /// Physically delete a SlaLevel by ID
        /// </summary>
        bool DeleteSlaLevel(int id, int currentUserID);

        /// <summary>
        /// Add a SlaLevel to a SlaConfig
        /// </summary>
        int AddSlaLevel(AddEditSlaLevelModel model);

        /// <summary>
        /// Update a SlaLevel
        /// </summary>
        bool UpdateSlaLevel(AddEditSlaLevelModel model);

        /// <summary>
        /// Get all SLA conditions
        /// </summary>
        List<SlaConditionModel> GetSlaConditions();

        /// <summary>
        /// Check whether an EmailTemplate exists and active
        /// </summary>
        bool EmailTemplateExists(int id);

        /// <summary>
        /// Get active email templates for SLA
        /// </summary>
        List<SlaEmailTemplateModel> GetActiveEmailTemplates();

        /// <summary>
        /// Get all SLA roles
        /// </summary>
        List<SlaRoleModel> GetSlaRoles();

        /// <summary>
        /// Get all SLA categories as combobox
        /// </summary>
        List<SlaCategoryModel> GetSlaCategories();
    }
}
