using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.InspectionReport;
using Elogbook.Core.Models.Response.InspectionReport;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức BBKSHT
    /// </summary>
    public interface IInspectionReportBusiness
    {
        /// <summary>
        /// Tìm kiếm những BBKSHT theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetAll(SearchInspectionReportModel requestModel);

        /// <summary>
        /// Tạo mới BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<InspectionReportDetailsModel> Add(AddEditInspectionReporModel requestModel);

        /// <summary>
        /// Update BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<InspectionReportDetailsModel> Update(AddEditInspectionReporModel requestModel);

        /// <summary>
        /// Delete BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> Delete(int id, int currentUserId);

        /// <summary>
        /// Get detail BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<InspectionReportDetailsModel> Details(int id);

        /// <summary>
        /// Ký số
        /// </summary>
        /// <param name="signgingModel"></param>
        /// <returns></returns>
        ResponseModel<UserSigningModel> Signging(SigngingModel signgingModel);

        /// <summary>
        /// In BBKSHT
        /// </summary>
        /// <param name="id">ID BBKSHT</param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);
    }
}
