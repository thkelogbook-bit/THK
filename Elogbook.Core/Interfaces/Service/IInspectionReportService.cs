using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.InspectionReport;
using Elogbook.Core.Models.Response.InspectionReport;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức BBKSHT
    /// </summary>
    public interface IInspectionReportService
    {
        /// <summary>
        /// Tìm kiếm những BBKSHT theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetAll(SearchInspectionReportModel requestModel);

        /// <summary>
        /// Lấy thông tin BBKSHT theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        InspectionReportDetailsModel GetById(int id);

        /// <summary>
        /// Tạo mới BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        InspectionReportDetailsModel Add(AddEditInspectionReporModel requestModel);

        /// <summary>
        /// Cập nhật BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        InspectionReportDetailsModel Update(AddEditInspectionReporModel requestModel);

        /// <summary>
        /// Xóa BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        bool Delete(int id, int currentUserID);

        /// <summary>
        /// Ký số
        /// </summary>
        /// <param name="signgingModel"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        UserSigningModel Signging(SigngingModel signgingModel);

        /// <summary>
        /// In BBKSHT
        /// </summary>
        /// <param name="id">ID BBKSHT</param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);

    }
}
