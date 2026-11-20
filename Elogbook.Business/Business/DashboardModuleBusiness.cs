using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business.Business
{
    /// <summary>
    /// Business layer cho danh mục module dashboard
    /// </summary>
    public class DashboardModuleBusiness : IDashboardModuleBusiness
    {
        private readonly IDashboardModuleService _dashboardModuleService;

        /// <summary>
        /// Constructor
        /// </summary>
        public DashboardModuleBusiness(IDashboardModuleService dashboardModuleService)
        {
            _dashboardModuleService = dashboardModuleService;
        }

        /// <summary>
        /// Lấy danh sách module đang active
        /// </summary>
        public ResponseModel<List<DashboardModuleModel>> GetActiveModules()
        {
            var responseModel = new ResponseModel<List<DashboardModuleModel>>();
            try
            {
                responseModel.Data = _dashboardModuleService.GetActiveModules();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }
    }
}
