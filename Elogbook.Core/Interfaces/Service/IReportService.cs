using Elogbook.Core.Models;
using Elogbook.Core.Models.Response.Report;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchReportModel searchModel);

        /// <summary>
        /// Find by module not dashboard
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        List<ReportViewModel> FindByModuleNotDashboard(string module);

        /// <summary>
        /// Thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        ResponseBaseModel GetPositionShiftStatusStatistics(FilterPositionShiftStatusModel requestModel);

        /// <summary>
        /// Thống kê tồn tại ca theo tình trạng xử lý của các nhà máy
        /// </summary>
        ResponseBaseModel GetPlantShiftStatusStatistics(FilterPlantShiftStatusModel requestModel);

        /// <summary>
        /// Bảng thống kê số lượng tồn tại ca theo tháng
        /// </summary>
        ResponseBaseModel GetMonthlyShiftCountStatistics(FilterMonthlyShiftCountModel requestModel);

        /// <summary>
        /// Thông số có giá trị vượt ngưỡng
        /// </summary>
        ResponseBaseModel GetThresholdExceededParameters(FilterThresholdExceededParametersModel requestModel);

        /// <summary>
        /// Báo cáo chi tiết lỗi thiết bị
        /// </summary>
        ResponseBaseModel GetDeviceErrorDetailReport(FilterGetDeviceErrorDetailReportModel requestModel);

        /// <summary>
        /// Export file for plant shift status
        /// </summary>
        HttpResponseMessage ExportPlantShiftStatus(FilterPlantShiftStatusModel requestModel);

        /// <summary>
        /// Export file for monthly shift count
        /// </summary>
        HttpResponseMessage ExportMonthlyShiftCount(FilterMonthlyShiftCountModel requestModel);

        /// <summary>
        /// Xuất file cho thông số vượt ngưỡng
        /// </summary>
        HttpResponseMessage ExportThresholdExceededParameters(FilterThresholdExceededParametersModel requestModel);

        /// <summary>
        /// Export file for machine downtime statistics
        /// </summary>
        HttpResponseMessage ExportDeviceErrorDetailReport(FilterGetDeviceErrorDetailReportModel requestModel);

        /// <summary>
        /// Export file for business trip and machine stop statistics
        /// </summary>
        HttpResponseMessage ExportBusyAndMachineStopStatistics(FilterBusyAndMachineStopStatisticsModel requestModel);

        /// <summary>
        /// Export file for position shift status
        /// </summary>
        HttpResponseMessage ExportPositionShiftStatus(FilterPositionShiftStatusModel requestModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addToDashboardModel"></param>
        /// <returns></returns>
        bool AddToDashboard(AddToDashboardModel addToDashboardModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        bool RemoveFromDashboard(int reportId, int currentUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addReportModel"></param>
        /// <returns></returns>
        ReportViewModel AddReport(AddEditReportModel addReportModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editReportModel"></param>
        /// <returns></returns>
        ReportViewModel EditReport(AddEditReportModel editReportModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool Delete(int reportId, int userID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="isActive"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool Active(int reportId, bool isActive, int userID);

        /// <summary>
        /// Bảng thống kê số lượng báo bận công tác và dừng máy 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetBusyAndMachineStopStatistics(FilterBusyAndMachineStopStatisticsModel requestModel);

        /// <summary>
        /// Lấy danh sách WidgetType và ChartType theo ReportId
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        List<ReportWidgetTypeModel> GetWidgetTypesByReportId(long reportId);
    }
}
