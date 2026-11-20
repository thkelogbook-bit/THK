using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response.Report;
using Hangfire;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Report
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/report")]
    public class ReportController : BaseApiController
    {
        private readonly IReportBusiness _reportBusiness;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="reportBusiness"></param>
        public ReportController(IReportBusiness reportBusiness)
        {
            _reportBusiness = reportBusiness;
        }


        /// <summary>
        /// Lấy thông tin dashboard
        /// </summary>
        /// <param name="searchReportModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel Find(SearchReportModel searchReportModel)
        {
            return _reportBusiness.Find(searchReportModel);
        }

        /// <summary>
        /// Find by module not in dashboard
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [Route("findNotDashboard")]
        [HttpGet]
        public List<ReportViewModel> FindByModuleNotDashboard([FromParameter("module")] string module)
        {
            return _reportBusiness.FindByModuleNotDashboard(module);
        }

        /// <summary>
        /// Add to dashboard
        /// </summary>
        /// <param name="addToDashboardModel"></param>
        /// <returns></returns>
        [Route("addToDashboard")]
        [HttpPost]
        public ResponseModel<bool> AddToDashboard(AddToDashboardModel addToDashboardModel)
        {
            return _reportBusiness.AddToDashboard(addToDashboardModel);
        }

        /// <summary>
        /// Thêm dashboard
        /// </summary>
        /// <param name="addReportModel"></param>
        /// <returns></returns>
        [Route]
        [HttpPost]
        public ResponseModel<ReportViewModel> AddReport(AddEditReportModel addReportModel)
        {
            addReportModel.CurrentUserID = CurrentUser.ID;
            return _reportBusiness.AddReport(addReportModel);
        }

        /// <summary>
        /// Chỉnh sữa dashboard
        /// </summary>
        /// <param name="editReportModel"></param>
        /// <returns></returns>
        [Route]
        [HttpPut]
        public ResponseModel<ReportViewModel> EditReport(AddEditReportModel editReportModel)
        {
            editReportModel.CurrentUserID = CurrentUser.ID;
            return _reportBusiness.EditReport(editReportModel);
        }

        /// <summary>
        /// Delete dashboard
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> Delete([FromParameter("id")] int id)
        {
            return _reportBusiness.Delete(id, CurrentUser.ID);
        }

        /// <summary>
        /// Active dashboard
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [Route("active/{id}")]
        [HttpDelete]
        public ResponseModel<bool> Active([FromParameter("id")] int id, [FromParameter("isActive")] bool isActive)
        {
            return _reportBusiness.Active(id, isActive, CurrentUser.ID);
        }

        [Route("removeFromDashboard/{id}")]
        [HttpPut]
        public ResponseModel<bool> RemoveFromDashboard([FromParameter("id")] int id)
        {
            return _reportBusiness.RemoveFromDashboard(id, CurrentUser.ID);
        }

        [Route("exportFile")]
        [HttpGet]
        public HttpResponseMessage ExportFile([FromUri] int id)
        {
            return ExportFiledData(id);
        }

        private HttpResponseMessage ExportFiledData(int id)
        {
            var pathCurrent = HostingEnvironment.MapPath("~/bin/Template");
            var fileName = string.Empty;

            switch (id)
            {
                case 1:
                    fileName = "Thong ke trang thai ca van hanh theo cuong vi.xlsx";
                    break;

                case 2:
                    fileName = "Thong ke ton tai ca theo tinh trang xu ly cua cac nha may.xlsx";
                    break;

                case 3:
                    fileName = "Bang thong ke so luong ton tai ca theo thang.xlsx";
                    break;

                case 4:
                    fileName = "Bao cao thong so co gia tri vuot nguong.xlsx";
                    break;

                case 5:
                    fileName = "Bang thong ke so luong bao ban cong tac va dung may.xlsx";
                    break;

                default:
                    break;
            }

            var inputFile = Path.Combine(pathCurrent, fileName);
            if (File.Exists(inputFile))
            {
                var fileBytes = File.ReadAllBytes(inputFile);

                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), fileName);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("nkvh/positionShiftStatus")]
        [HttpPost]
        public ResponseBaseModel GetPositionShiftStatusStatistics([FromBody] FilterPositionShiftStatusModel requestModel)
        {
            return _reportBusiness.GetPositionShiftStatusStatistics(requestModel);
        }

        /// <summary>
        /// Thống kê tồn tại ca theo tình trạng xử lý của các nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("nkvh/plantShiftStatus")]
        [HttpPost]
        public ResponseBaseModel GetPlantShiftStatusStatistics([FromBody] FilterPlantShiftStatusModel requestModel)
        {
            return _reportBusiness.GetPlantShiftStatusStatistics(requestModel);
        }

        /// <summary>
        /// Bảng thống kê số lượng tồn tại ca theo tháng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("nkvh/monthlyShiftCount")]
        [HttpPost]
        public ResponseBaseModel GetMonthlyShiftCountStatistics([FromBody] FilterMonthlyShiftCountModel requestModel)
        {
            return _reportBusiness.GetMonthlyShiftCountStatistics(requestModel);
        }

        /// <summary>
        /// Thông số có giá trị vượt ngưỡng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("tsvh/thresholdExceededParameters")]
        [HttpPost]
        public ResponseBaseModel GetThresholdExceededParameters([FromBody] FilterThresholdExceededParametersModel requestModel)
        {
            return _reportBusiness.GetThresholdExceededParameters(requestModel);
        }

        /// <summary>
        /// Báo cáo thống kê chi tiết lỗi thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("tsvh/getDeviceErrorDetailReport")]
        [HttpPost]
        public ResponseBaseModel GetDeviceErrorDetailReport([FromBody] FilterGetDeviceErrorDetailReportModel requestModel)
        {
            return _reportBusiness.GetDeviceErrorDetailReport(requestModel);
        }

        /// <summary>
        /// Bảng thống kê số lượng báo bận công tác và dừng máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("tsvh/getBusyAndMachineStopStatistics")]
        [HttpPost]
        public ResponseBaseModel GetBusyAndMachineStopStatistics([FromBody] FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            return _reportBusiness.GetBusyAndMachineStopStatistics(requestModel);
        }

        /// <summary>
        /// Xuất file danh sách Thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        /// <returns></returns>
        [Route("nkvh/export/positionShiftStatus")]
        [HttpPost]
        public HttpResponseMessage ExportPositionShiftStatus([FromBody] FilterPositionShiftStatusModel requestModel)
        {
            return _reportBusiness.ExportPositionShiftStatus(requestModel);
        }

        /// <summary>
        /// Xuất file danh sách Thống kê trạng thái ca theo nhà máy
        /// </summary>
        [Route("nkvh/export/plantShiftStatus")]
        [HttpPost]
        public HttpResponseMessage ExportPlantShiftStatus([FromBody] FilterPlantShiftStatusModel requestModel)
        {
            return _reportBusiness.ExportPlantShiftStatus(requestModel);
        }

        /// <summary>
        /// Xuất file bảng thống kê số lượng tồn tại ca theo tháng
        /// </summary>
        [Route("nkvh/export/monthlyShiftCount")]
        [HttpPost]
        public HttpResponseMessage ExportMonthlyShiftCount([FromBody] FilterMonthlyShiftCountModel requestModel)
        {
            return _reportBusiness.ExportMonthlyShiftCount(requestModel);
        }

        /// <summary>
        /// Xuất file thông số có giá trị vượt ngưỡng
        /// </summary>
        [Route("tsvh/export/thresholdExceededParameters")]
        [HttpPost]
        public HttpResponseMessage ExportThresholdExceededParameters([FromBody] FilterThresholdExceededParametersModel requestModel)
        {
            return _reportBusiness.ExportThresholdExceededParameters(requestModel);
        }

        /// <summary>
        /// Xuất file báo cáo chi tiết lỗi thiết bị
        /// </summary>
        [Route("tsvh/export/getDeviceErrorDetailReport")]
        [HttpPost]
        public HttpResponseMessage ExportDeviceErrorDetailReport([FromBody] FilterGetDeviceErrorDetailReportModel requestModel)
        {
            return _reportBusiness.ExportDeviceErrorDetailReport(requestModel);
        }

        /// <summary>
        /// Xuất file thống kê công tác đi công tác và dừng máy
        /// </summary>
        [Route("tsvh/export/busyAndMachineStopStatistics")]
        [HttpPost]
        public HttpResponseMessage ExportBusyAndMachineStopStatistics([FromBody] FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            return _reportBusiness.ExportBusyAndMachineStopStatistics(requestModel);
        }

        /// <summary>
        /// Lấy danh sách WidgetType và ChartType theo ReportId
        /// </summary>
        /// <param name="reportId">ID của report</param>
        /// <returns>Danh sách WidgetType và ChartType</returns>
        [Route("widgetTypes")]
        [HttpGet]
        public List<ReportWidgetTypeModel> GetWidgetTypesByReportId([FromUri] long reportId)
        {
            return _reportBusiness.GetWidgetTypesByReportId(reportId);
        }

    }
}