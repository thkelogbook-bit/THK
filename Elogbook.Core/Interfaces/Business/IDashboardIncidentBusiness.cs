using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Báo cáo bất thường sự cố
    /// </summary>
    public interface IDashboardIncidentBusiness
    {
        /// <summary>
        /// Lấy báo cáo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Lấy báo cáo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindDetail(SearchPrtReportModel requestModel);

        /// <summary>
        /// Lấy tất cả hoạt động
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AllActivity(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Lấy tất cả hoạt động
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AbnormalActivity(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Lấy tất cả thông tin bất thường có ticket
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AbnormalTicketInfo(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Lấy top 5 user
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Top5User(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Lấy top 5 user
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<DashboardSummaryGroupModel> Summary(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Xuất file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportGeneralHandle(AbnormalIncidentSearch requestModel);
        /// <summary>
        /// Xuất file excel - phần chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportDetailHandle(SearchPrtReportModel requestModel);
    }
}
