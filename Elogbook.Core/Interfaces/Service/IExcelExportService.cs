using Elogbook.Core.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức xuất dữ liệu excel
    /// </summary>
    public interface IExcelExportService
    {
        /// <summary>
        /// Xuất dữ liệu file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportExcelFile(ExportExcelRequestModel requestModel);
    }
}
