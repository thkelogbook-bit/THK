using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    /// <summary>
    /// khai báo business tuân thủ hoạt động
    /// </summary>
    public class AmReportBusiness: IAmReportBusiness
    {
        private readonly IAmReportService _amReportService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public AmReportBusiness(IAmReportService amReportService)
        {
            _amReportService= amReportService;
        }

        /// <summary>
        /// Trả về tất cả phân loại thiết bị theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindGeneralOperationalCompliance(SearchOperationalComplianceModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReportService.FindGeneralOperationalCompliance(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về chi tiết báo cáo tuân thủ hoạt động theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindDetailOperationalCompliance(SearchOperationalComplianceModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReportService.FindDetailOperationalCompliance(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// In tài liệu tab tổng quan
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportGeneral(SearchOperationalComplianceModel requestModel)
        {
            try
            {
                return _amReportService.ExportGeneral(requestModel);
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

        /// <summary>
        /// In tài liệu tab chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportDetail(SearchOperationalComplianceModel requestModel)
        {
            try
            {
                return _amReportService.ExportDetail(requestModel);
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

        /// <summary>
        /// Báo cáo tiến trình xử lý yêu cầu - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<ReportPrtGeneralModel> RptyReportGeneral(SearchPrtReportModel requestModel)
        {
            ResponseModel<ReportPrtGeneralModel> responseBaseModel = new ResponseModel<ReportPrtGeneralModel>();

            try
            {
                responseBaseModel.Data = _amReportService.RptyReportGeneral(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Báo cáo tiến trình xử lý yêu cầu - Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel RptyReportDetail(SearchPrtReportModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReportService.RptyReportDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Xuất file báo cáo tiến trình xử lý yêu cầu -Tổng hợp
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage PrtReportGeneralExport(int factoryID, int year, int month)
        {
            try
            {
                return _amReportService.PrtReportGeneralExport(factoryID, year, month);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Xuất file báo cáo tiến trình xử lý yêu cầu -Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage PrtReportDetailExport(SearchPrtReportModel requestModel)
        {
            try
            {
                return _amReportService.PrtReportDetailExport(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public List<ValueLabelCheckedModel> GetReportStatus(int statusType)
        {
            try
            {
                return _amReportService.GetReportStatus(statusType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(), ex.ToString());
                return new List<ValueLabelCheckedModel>();
            }
        }

        public ResponseBaseModel RequestAmReportGeneral(SearchPrtReportModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReportService.RequestAmReportGeneral(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel RequestAmReportDetail(SearchPrtReportModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReportService.RequestAmReportDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<RequestAmReportEmergencyModel> RequestAmReportEmergency(SearchPrtReportModel requestModel)
        {
            ResponseModel<RequestAmReportEmergencyModel> responseBaseModel = new ResponseModel<RequestAmReportEmergencyModel>();

            try
            {
                responseBaseModel.Data = _amReportService.RequestAmReportEmergency(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public HttpResponseMessage RequestAmReportDetailExport(SearchPrtReportModel requestModel)
        {
            try
            {
                return _amReportService.RequestAmReportDetailExport(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public HttpResponseMessage RequestAmReportEmergencyExport(SearchPrtReportModel requestModel)
        {
            try
            {
                return _amReportService.RequestAmReportEmergencyExport(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public HttpResponseMessage RequestAmReportGeneralExport(SearchPrtReportModel requestModel)
        {
            try
            {
                return _amReportService.RequestAmReportGeneralExport(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Trả về tổng hợp báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindGeneralTimeHandle(SearchTimeHandleModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReportService.FindGeneralTimeHandle(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindDetailTimeHandle(SearchTimeHandleModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReportService.FindDetailTimeHandle(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// In tài liệu tab tổng quan báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportGeneralTimeHandle(SearchTimeHandleModel requestModel)
        {
            try
            {
                return _amReportService.ExportGeneralTimeHandle(requestModel);
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

        /// <summary>
        /// In tài liệu tab chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportDetailTimeHandle(SearchTimeHandleModel requestModel)
        {
            try
            {
                return _amReportService.ExportDetailTimeHandle(requestModel);
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

        public HttpResponseMessage PctExport(SearchWorkTicketModel requestModel)
        {
            try
            {
                return _amReportService.PctExport(requestModel);
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

        public HttpResponseMessage PttExport(SearchTicketWebModel requestModel)
        {
            try
            {
                return _amReportService.PttExport(requestModel);
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

        public int SendMailReport()
        {
            try
            {
                return _amReportService.SendMailReport();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(), ex.ToString());
                return 0;
            }
        }
    }
}
