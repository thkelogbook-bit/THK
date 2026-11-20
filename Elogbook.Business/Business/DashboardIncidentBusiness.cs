using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    /// <summary>
    /// AM Dashboard
    /// </summary>
    public class DashboardIncidentBusiness : IDashboardIncidentBusiness
    {
        private readonly IDashboardIncidentService _dashboardIncidentService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dashboardIncidentService"></param>
        public DashboardIncidentBusiness(IDashboardIncidentService dashboardIncidentService)
        {
            _dashboardIncidentService = dashboardIncidentService;
        }

        public ResponseBaseModel Find(AbnormalIncidentSearch requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = HttpResponse.OkList(_dashboardIncidentService.Find(requestModel));
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.INCIDENT;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel FindDetail(SearchPrtReportModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _dashboardIncidentService.FindDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.INCIDENT;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        ResponseBaseModel IDashboardIncidentBusiness.AllActivity(AbnormalIncidentSearch requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _dashboardIncidentService.AllActivity(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.INCIDENT;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        ResponseBaseModel IDashboardIncidentBusiness.AbnormalActivity(AbnormalIncidentSearch requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _dashboardIncidentService.AbnormalActivity(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.INCIDENT;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        ResponseBaseModel IDashboardIncidentBusiness.AbnormalTicketInfo(AbnormalIncidentSearch requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _dashboardIncidentService.AbnormalTicketInfo(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.INCIDENT;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        ResponseBaseModel IDashboardIncidentBusiness.Top5User(AbnormalIncidentSearch requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _dashboardIncidentService.Top5User(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.INCIDENT;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<DashboardSummaryGroupModel> Summary(AbnormalIncidentSearch requestModel)
        {
            var responseModel = new ResponseModel<DashboardSummaryGroupModel>();

            try
            {
                 responseModel.Data = _dashboardIncidentService.Summary(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(CommonConstant.INCIDENT);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DashboardIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
        HttpResponseMessage IDashboardIncidentBusiness.ExportGeneralHandle(AbnormalIncidentSearch requestModel)
        {
            try
            {
                return _dashboardIncidentService.ExportGeneralHandle(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        HttpResponseMessage IDashboardIncidentBusiness.ExportDetailHandle(SearchPrtReportModel requestModel)
        {
            try
            {
                return _dashboardIncidentService.ExportDetailHandle(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

    }
}
