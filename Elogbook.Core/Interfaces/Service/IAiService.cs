using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// IAiService
    /// </summary>
    public interface IAiService
    {
        /// <summary>
        /// GetConfigRuleDefaul
        /// </summary>
        /// <returns></returns>
        ConfigWarningRuleResponseModel GetConfigRuleDefaul();

        /// <summary>
        /// GetConfigDefaul
        /// </summary>
        /// <returns></returns>
        ConfigWarningResponseModel GetConfigDefaul();

        /// <summary>
        /// SaveConfigWarningRule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> SaveConfigWarningRule(AddEditConfigWarningRuleRequestModel requestModel);

        /// <summary>
        /// SaveConfigWarning
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> SaveConfigWarning(AddEditConfigWarningRequestModel requestModel);
    }
}
