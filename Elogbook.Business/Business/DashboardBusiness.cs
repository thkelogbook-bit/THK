using Elogbook.Core.Enums;
using Elogbook.Core;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response.Dashboard;
using System.Net;
using System;
using System.Collections.Generic;
using Elogbook.Core.Models.Request;

namespace Elogbook.Business.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class DashboardBusiness : IDashboardBusiness
    {

        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dashboardIncidentService"></param>
        public DashboardBusiness(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public ResponseModel<List<ListDashboardModuleView>> GetDashboards(int userId, bool includeInactive = false)
        {
            var responseModel = new ResponseModel<List<ListDashboardModuleView>>();

            try
            {
                responseModel.Data = _dashboardService.GetDashboards(userId, includeInactive);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<DashboardViewModel> GetInfo(int reportId)
        {
            var responseModel = new ResponseModel<DashboardViewModel>();

            try
            {
                responseModel.Data = _dashboardService.GetInfo(reportId);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(reportId) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<ListDashboardModuleView> Update(EditDashboardModel dashboardModel)
        {
            var responseModel = new ResponseModel<ListDashboardModuleView>();

            try
            {
                responseModel.Data = _dashboardService.Update(dashboardModel);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(dashboardModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
