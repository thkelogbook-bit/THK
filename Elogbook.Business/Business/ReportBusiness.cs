using Elogbook.Core.Enums;
using Elogbook.Core;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response.Report;
using System.Net;
using System.Net.Http;
using System;
using Elogbook.Core.Interfaces.Service;
using System.Collections.Generic;

namespace Elogbook.Business.Business
{
    public class ReportBusiness : IReportBusiness
    {
        private readonly IReportService _reportService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reportService"></param>
        public ReportBusiness(IReportService reportService)
        {
            _reportService = reportService;
        }

        public ResponseModel<bool> Active(int reportId, bool isActive, int userID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _reportService.Active(reportId, isActive, userID);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(reportId) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<ReportViewModel> AddReport(AddEditReportModel addReportModel)
        {
            var responseModel = new ResponseModel<ReportViewModel>();

            try
            {
                responseModel.Data = _reportService.EditReport(addReportModel);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(addReportModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }


        public ResponseModel<ReportViewModel> EditReport(AddEditReportModel editReportModel)
        {
            var responseModel = new ResponseModel<ReportViewModel>();

            try
            {
                responseModel.Data = _reportService.EditReport(editReportModel);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(editReportModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> AddToDashboard(AddToDashboardModel addToDashboardModel)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _reportService.AddToDashboard(addToDashboardModel);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(addToDashboardModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> RemoveFromDashboard(int reportId, int currentUserId)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _reportService.RemoveFromDashboard(reportId, currentUserId);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(reportId) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> Delete(int reportId, int userID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _reportService.Delete(reportId, userID);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(reportId) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel Find(SearchReportModel searchModel)
        {
            var responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _reportService.Find(searchModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(searchModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }


        public List<ReportViewModel> FindByModuleNotDashboard(string module)
        {
            var reportViews = new List<ReportViewModel>();

            try
            {
                reportViews = _reportService.FindByModuleNotDashboard(module);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(module) + Environment.NewLine + ex.ToString());
            }

            return reportViews;
        }

        /// <summary>
        /// Báo cáo thống kê trạng thái ca của vị trí
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetPositionShiftStatusStatistics(FilterPositionShiftStatusModel requestModel)
        {
            var response = new ResponseBaseModel();
            try
            {
                response = _reportService.GetPositionShiftStatusStatistics(requestModel);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Báo cáo thống kê trạng thái ca của nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetPlantShiftStatusStatistics(FilterPlantShiftStatusModel requestModel)
        {
            var response = new ResponseBaseModel();
            try
            {
                response = _reportService.GetPlantShiftStatusStatistics(requestModel);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Báo cáo thống kê số ca làm việc trong tháng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetMonthlyShiftCountStatistics(FilterMonthlyShiftCountModel requestModel)
        {
            var response = new ResponseBaseModel();
            try
            {
                response = _reportService.GetMonthlyShiftCountStatistics(requestModel);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Báo cáo thống kê các thông số vượt ngưỡng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetThresholdExceededParameters(FilterThresholdExceededParametersModel requestModel)
        {
            var response = new ResponseBaseModel();
            try
            {
                response = _reportService.GetThresholdExceededParameters(requestModel);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Báo cáo chi tiết lỗi thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetDeviceErrorDetailReport(FilterGetDeviceErrorDetailReportModel requestModel)
        {
            var response = new ResponseBaseModel();
            try
            {
                response = _reportService.GetDeviceErrorDetailReport(requestModel);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <summary>
        ///  Bảng thống kê số lượng báo bận công tác và dừng máy 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetBusyAndMachineStopStatistics(FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            var response = new ResponseBaseModel();
            try
            {
                response = _reportService.GetBusyAndMachineStopStatistics(requestModel);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Xuất báo cáo thống kê trạng thái ca của nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportPlantShiftStatus(FilterPlantShiftStatusModel requestModel)
        {
            try
            {
                return _reportService.ExportPlantShiftStatus(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Xuất báo cáo thống kê số ca làm việc trong tháng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportMonthlyShiftCount(FilterMonthlyShiftCountModel requestModel)
        {
            try
            {
                return _reportService.ExportMonthlyShiftCount(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Xuất báo cáo thống kê các thông số vượt ngưỡng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportThresholdExceededParameters(FilterThresholdExceededParametersModel requestModel)
        {
            try
            {
                return _reportService.ExportThresholdExceededParameters(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Xuất báo cáo chi tiết lỗi thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportDeviceErrorDetailReport(FilterGetDeviceErrorDetailReportModel requestModel)
        {
            try
            {
                return _reportService.ExportDeviceErrorDetailReport(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Xuất báo cáo thống kê thời gian dừng máy và bận
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportBusyAndMachineStopStatistics(FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            try
            {
                return _reportService.ExportBusyAndMachineStopStatistics(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Xuất báo cáo thống kê trạng thái ca của vị trí
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportPositionShiftStatus(FilterPositionShiftStatusModel requestModel)
        {
            try
            {
                return _reportService.ExportPositionShiftStatus(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Lấy danh sách WidgetType và ChartType theo ReportId
        /// </summary>
        public List<ReportWidgetTypeModel> GetWidgetTypesByReportId(long reportId)
        {
            try
            {
                return _reportService.GetWidgetTypesByReportId(reportId);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ReportBusiness.ToString() + Environment.NewLine + ex.ToString());
                return new List<ReportWidgetTypeModel>();
            }
        }
    }
}
