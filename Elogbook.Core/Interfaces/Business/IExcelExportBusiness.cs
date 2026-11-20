using Elogbook.Core.Models.Request;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức xuất dữ liệu
    /// </summary>
    public interface IExcelExportBusiness
    {
        /// <summary>
        /// Xuất dữ liệu file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportExcelFile(ExportExcelRequestModel requestModel);
    }
}
