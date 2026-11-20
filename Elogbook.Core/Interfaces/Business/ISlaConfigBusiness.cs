using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Business interface for SLA configuration management
    /// </summary>
    public interface ISlaConfigBusiness
    {
        /// <summary>
        /// Get all SLA configurations with pagination and filtering
        /// </summary>
        ResponseBaseModel GetSlaConfigs(SearchSlaConfigModel requestModel);

        /// <summary>
        /// Get SLA configuration details by ID
        /// </summary>
        ResponseModel<SlaConfigDetailModel> GetSlaConfigById(int id);

        /// <summary>
        /// Create new SLA configuration
        /// </summary>
        ResponseModel<int> AddSlaConfig(AddEditSlaConfigModel requestModel);

        /// <summary>
        /// Update existing SLA configuration
        /// </summary>
        ResponseModel<bool> UpdateSlaConfig(AddEditSlaConfigModel requestModel);

        /// <summary>
        /// Delete SLA configuration
        /// </summary>
        ResponseModel<bool> DeleteSlaConfig(int id, int currentUserID);

        /// <summary>
        /// Toggle SLA configuration active status
        /// </summary>
        ResponseModel<bool> ToggleSlaConfigStatus(int id, int currentUserID);

        /// <summary>
        /// Physically delete a SlaLevel by ID
        /// </summary>
        ResponseModel<bool> DeleteSlaLevel(int id, int currentUserID);

        /// <summary>
        /// Get all SLA conditions
        /// </summary>
        List<SlaConditionModel> GetSlaConditions();

        /// <summary>
        /// Get active email templates for SLA
        /// </summary>
        List<SlaEmailTemplateModel> GetActiveEmailTemplates();

        /// <summary>
        /// Add a SlaLevel to a SlaConfig
        /// </summary>
        ResponseModel<int> AddSlaLevel(AddEditSlaLevelModel model);

        /// <summary>
        /// Update a SlaLevel
        /// </summary>
        ResponseModel<bool> UpdateSlaLevel(int id, AddEditSlaLevelModel model);

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
