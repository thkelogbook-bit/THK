using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Lịch sử đồng bộ
    /// </summary>
    public interface IDashboardIncidentService
    {
        /// <summary>
        /// Tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        List<DashboardGroupItemModel> Find(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Tìm kiếm 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindDetail(SearchPrtReportModel requestModel);

        /// <summary>
        /// Tìm tất cả hoạt động 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AllActivity(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Tìm các bất thường sự cố 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AbnormalActivity(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Thông tin các phiếu xử lý bất thường sự cố 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AbnormalTicketInfo(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Thông tin các phiếu xử lý bất thường sự cố 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Top5User(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// Thông tin các phiếu xử lý bất thường sự cố 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        DashboardSummaryGroupModel Summary(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// In tài liệu bất thường 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportGeneralHandle(AbnormalIncidentSearch requestModel);

        /// <summary>
        /// In tài liệu bất thường - phần chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportDetailHandle(SearchPrtReportModel requestModel);
    }
}
