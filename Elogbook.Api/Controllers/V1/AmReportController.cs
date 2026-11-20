using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý am report
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/amReportController")]
    public class AmReportController : BaseApiController
    {
        private readonly IAmReportBusiness _amReportBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amReportBusiness"></param>
        public AmReportController(IAmReportBusiness amReportBusiness)
        {
            _amReportBusiness = amReportBusiness;
        }

        /// <summary>
        /// Trả về tổng hợp báo cáo tuân thủ hoạt động theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTAM
        [Route("findGeneral")]
        [HttpPost]
        public ResponseBaseModel FindGeneralOperationalCompliance([FromBody] SearchOperationalComplianceModel requestModel)
        {
            return _amReportBusiness.FindGeneralOperationalCompliance(requestModel);
        }

        /// <summary>
        /// Trả về chi tiết báo cáo tuân thủ hoạt động theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTAM
        [Route("findDetail")]
        [HttpPost]
        public ResponseBaseModel FindDetailOperationalCompliance([FromBody] SearchOperationalComplianceModel requestModel)
        {
            return _amReportBusiness.FindDetailOperationalCompliance(requestModel);
        }

        /// <summary>
        /// In tài liệu tab tổng quan
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTAM
        [Route("exportGeneralExcel")]
        [HttpPost]
        public HttpResponseMessage ExportGeneral([FromBody] SearchOperationalComplianceModel requestModel)
        {
            return _amReportBusiness.ExportGeneral(requestModel);
        }

        /// <summary>
        /// In tài liệu tab chi tiết
        /// </summary>
        /// <param name="FactoryId"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="Status"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTAM
        [Route("exportDetailExcel/{FactoryId}/{Year}/{Month}/{Status}/{PageSize}/{PageNumber}")]
        [HttpGet]
        public HttpResponseMessage ExportDetail([FromUri] int FactoryId, int Year, int Month, int Status, int PageSize, int PageNumber)
        {
            SearchOperationalComplianceModel requestModel = new SearchOperationalComplianceModel
            {
                FactoryId = FactoryId,
                Year = Year,
                Month = Month,
                Status = Status,
                PageSize = PageSize,
                PageNumber = PageNumber
            };
            return _amReportBusiness.ExportDetail(requestModel);
        }

        /// <summary>
        /// Báo cáo tiến trình xử lý yêu cầu -Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTXL
        [Route("prtReportGeneral")]
        [HttpPost]
        public ResponseModel<ReportPrtGeneralModel> RptyReportGeneral([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RptyReportGeneral(requestModel);
        }

        /// <summary>
        /// Báo cáo tiến trình xử lý yêu cầu - Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTXL
        [Route("prtReportDetail")]
        [HttpPost]
        public ResponseBaseModel RptyReportDetail([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RptyReportDetail(requestModel);
        }

        /// <summary>
        /// Xuất file báo cáo tiến trình xử lý yêu cầu -Tổng hợp
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTXL
        [Route("prtReportGeneralExport")]
        [HttpGet]
        public HttpResponseMessage PrtReportGeneralExport(int factoryID, int year, int month)
        {
            return _amReportBusiness.PrtReportGeneralExport(factoryID, year, month);
        }

        /// <summary>
        /// Xuất file báo cáo tiến trình xử lý yêu cầu -Chi tiết
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TTXL
        [Route("prtReportDetailExport")]
        [HttpPost]
        public HttpResponseMessage PrtReportDetailExport([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.PrtReportDetailExport(requestModel);
        }

        /// <summary>
        /// Lấy trạng thái xử lý của báo cáo
        /// </summary>
        /// <param name="statusType">1: Trạng thái xử lý, 2: Trạng thái</param>
        /// <returns></returns>
        [Route("getReportStatus")]
        [HttpGet]
        public List<ValueLabelCheckedModel> GetReportStatus([FromUri] int statusType)
        {
            return _amReportBusiness.GetReportStatus(statusType);
        }

        /// <summary>
        /// Báo cáo yêu cầu bảo trì - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_YCBT
        [Route("requestAmReportGeneral")]
        [HttpPost]
        public ResponseBaseModel RequestAmReportGeneral([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RequestAmReportGeneral(requestModel);
        }

        /// <summary>
        /// Báo cáo yêu cầu bảo trì - Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_YCBT
        [Route("requestAmReportDetail")]
        [HttpPost]
        public ResponseBaseModel RequestAmReportDetail([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RequestAmReportDetail(requestModel);
        }

        /// <summary>
        /// Báo cáo yêu cầu bảo trì - Mức độ khẩn cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_YCBT
        [Route("requestAmReportEmergency")]
        [HttpPost]
        public ResponseModel<RequestAmReportEmergencyModel> RequestAmReportEmergency([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RequestAmReportEmergency(requestModel);
        }

        /// <summary>
        /// Xuất file Báo cáo yêu cầu bảo trì - Chi tiết
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_YCBT
        [Route("requestAmReportDetailExport")]
        [HttpPost]
        public HttpResponseMessage RequestAmReportDetailExport([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RequestAmReportDetailExport(requestModel);
        }

        /// <summary>
        /// Xuất file Báo cáo yêu cầu bảo trì - Mức độ khẩn cấp
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_YCBT
        [Route("requestAmReportEmergencyExport")]
        [HttpPost]
        public HttpResponseMessage RequestAmReportEmergencyExport([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RequestAmReportEmergencyExport(requestModel);
        }

        /// <summary>
        /// Xuất file Báo cáo yêu cầu bảo trì - Tổng hợp
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_YCBT
        [Route("requestAmReportGeneralExport")]
        [HttpPost]
        public HttpResponseMessage RequestAmReportGeneralExport([FromBody] SearchPrtReportModel requestModel)
        {
            return _amReportBusiness.RequestAmReportGeneralExport(requestModel);
        }

        /// <summary>
        /// Báo cáo thời gian xử lý bất thường - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TGXL
        [Route("findGeneralTimeHandle")]
        [HttpPost]
        public ResponseBaseModel FindGeneralTimeHandle([FromBody] SearchTimeHandleModel requestModel)
        {
            return _amReportBusiness.FindGeneralTimeHandle(requestModel);
        }

        /// <summary>
        /// Trả về chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TGXL
        [Route("findDetailTimeHandle")]
        [HttpPost]
        public ResponseBaseModel FindDetailTimeHandle([FromBody] SearchTimeHandleModel requestModel)
        {
            return _amReportBusiness.FindDetailTimeHandle(requestModel);
        }

        /// <summary>
        /// In tài liệu tab tổng quan báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="FactoryId"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TGXL
        [Route("exportGeneralTimeHandle/{FactoryId}/{Year}/{Month}")]
        [HttpGet]
        public HttpResponseMessage ExportGeneralTimeHandle([FromUri] int FactoryId, int Year, int Month)
        {
            SearchTimeHandleModel requestModel = new SearchTimeHandleModel
            {
                FactoryId = FactoryId,
                Year = Year,
                Month = Month
            };
            return _amReportBusiness.ExportGeneralTimeHandle(requestModel);
        }

        /// <summary>
        /// In tài liệu tab chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="FactoryId"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="Emergency"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_REPORT_TGXL
        [Route("exportDetailTimeHandle/{FactoryId}/{Year}/{Month}/{Emergency}/{PageSize}/{PageNumber}")]
        [HttpGet]
        public HttpResponseMessage ExportDetailTimeHandle([FromUri] int FactoryId, int Year, int Month, int Emergency, int PageSize, int PageNumber)
        {
            SearchTimeHandleModel requestModel = new SearchTimeHandleModel
            {
                FactoryId = FactoryId,
                Year = Year,
                Month = Month,
                Emergency = Emergency,
                PageSize = PageSize,
                PageNumber = PageNumber
            };
            return _amReportBusiness.ExportDetailTimeHandle(requestModel);
        }

        /// <summary>
        /// Xuất file danh sách Phiếu công tác Điện - Thủy cơ nhiệt thóa
        /// </summary>
        /// <returns></returns>
        [Route("pctExport")]
        [HttpPost]
        public HttpResponseMessage PctExport([FromBody] SearchWorkTicketModel requestModel)
        {
            return _amReportBusiness.PctExport(requestModel);
        }

        /// <summary>
        /// Xuất file danh sách Phiếu thao tác
        /// </summary>
        /// <returns></returns>
        [Route("pttExport")]
        [HttpPost]
        public HttpResponseMessage PttExport([FromBody] SearchTicketWebModel requestModel)
        {
            return _amReportBusiness.PttExport(requestModel);
        }

        /// <summary>
        /// Tự động hoàn thành bảo trì tự quản 
        /// </summary>
        [Route("sendMailReport")]
        [HttpPost]
        public int AutoCompleteAm()
        {
            return _amReportBusiness.SendMailReport();
        }
    }
}