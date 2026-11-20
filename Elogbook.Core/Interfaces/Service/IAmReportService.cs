using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// khai báo interface service tuân thủ hoạt động
    /// </summary>
    public interface IAmReportService
    {
        /// <summary>
        /// Trả về tất cả phân loại thiết bị theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindGeneralOperationalCompliance(SearchOperationalComplianceModel requestModel);

        /// <summary>
        /// Trả về chi tiết báo cáo tuân thủ hoạt động theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindDetailOperationalCompliance(SearchOperationalComplianceModel requestModel);

        /// <summary>
        /// In tài liệu tab tổng quan
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportGeneral(SearchOperationalComplianceModel requestModel);

        /// <summary>
        /// In tài liệu tab chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportDetail(SearchOperationalComplianceModel requestModel);

        /// <summary>
        /// Báo cáo tiến trình xử lý yêu cầu - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ReportPrtGeneralModel RptyReportGeneral(SearchPrtReportModel requestModel);

        /// <summary>
        /// Báo cáo tiến trình xử lý yêu cầu - Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel RptyReportDetail(SearchPrtReportModel requestModel);

        /// <summary>
        /// Xuất file báo cáo tiến trình xử lý yêu cầu -Tổng hợp
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        HttpResponseMessage PrtReportGeneralExport(int factoryID, int year, int month);

        /// <summary>
        /// Xuất file báo cáo tiến trình xử lý yêu cầu -Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage PrtReportDetailExport(SearchPrtReportModel requestModel);

        /// <summary>
        /// Lấy trạng thái xử lý của báo cáo tiến trình xử lý yêu cầu
        /// </summary>
        /// <returns></returns>
        List<ValueLabelCheckedModel> GetReportStatus(int statusType);

        /// <summary>
        /// Báo cáo yêu cầu bảo trì - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel RequestAmReportGeneral(SearchPrtReportModel requestModel);

        /// <summary>
        /// Báo cáo yêu cầu bảo trì - Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel RequestAmReportDetail(SearchPrtReportModel requestModel);

        /// <summary>
        /// Báo cáo yêu cầu bảo trì - Mức độ khẩn cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        RequestAmReportEmergencyModel RequestAmReportEmergency(SearchPrtReportModel requestModel);

        /// <summary>
        /// Xuất file Báo cáo yêu cầu bảo trì - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage RequestAmReportDetailExport(SearchPrtReportModel requestModel);

        /// <summary>
        /// Xuất file Báo cáo yêu cầu bảo trì - Mức độ khẩn cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage RequestAmReportEmergencyExport(SearchPrtReportModel requestModel);

        /// <summary>
        /// Xuất file Báo cáo yêu cầu bảo trì - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage RequestAmReportGeneralExport(SearchPrtReportModel requestModel);

        /// <summary>
        /// Trả về tổng hợp báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindGeneralTimeHandle(SearchTimeHandleModel requestModel);

        /// <summary>
        /// Trả về chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindDetailTimeHandle(SearchTimeHandleModel requestModel);

        /// <summary>
        /// In tài liệu tab tổng quan báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportGeneralTimeHandle(SearchTimeHandleModel requestModel);

        /// <summary>
        /// In tài liệu tab chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportDetailTimeHandle(SearchTimeHandleModel requestModel);

        /// <summary>
        /// Xuất file danh sách Phiếu công tác Điện - Thủy cơ nhiệt thóa
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage PctExport(SearchWorkTicketModel requestModel);

        /// <summary>
        /// Xuất file danh sách Phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage PttExport(SearchTicketWebModel requestModel);
        
        /// <summary>
        /// Gửi mail report định kỳ
        /// </summary>
        /// <returns></returns>
        int SendMailReport();
    }
}
