using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa phương thức export/import
    /// </summary>
    public interface IExportImportService
    {
        /// <summary>
        /// Tải file mẫu
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        HttpResponseMessage ExportFileTemplate(int categoryType, int companyId);

        /// <summary>
        /// Import data
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        ResponseModel<bool> ImportDataFromExcelFile(FileUploadInfoModel requestModel, int categoryType);
    }
}
