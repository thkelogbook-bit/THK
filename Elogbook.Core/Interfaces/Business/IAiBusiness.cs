using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// IAiBusiness
    /// </summary>
    public interface IAiBusiness
    {
        /// <summary>
        /// GetById
        /// </summary>
        /// <returns></returns>
        ResponseModel<ConfigWarningRuleResponseModel> GetConfigRuleDefaul();

        /// <summary>
        /// GetConfigDefaul
        /// </summary>
        /// <returns></returns>
        ResponseModel<ConfigWarningResponseModel> GetConfigDefaul();

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
