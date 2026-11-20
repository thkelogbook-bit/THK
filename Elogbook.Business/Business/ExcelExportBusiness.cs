using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request;
using System;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class ExcelExportBusiness : IExcelExportBusiness
    {
        private readonly IExcelExportService _excelExportyService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="excelExportyService"></param>
        public ExcelExportBusiness(IExcelExportService excelExportyService)
        {
            _excelExportyService = excelExportyService;
        }

        /// <summary>
        /// Xuất dữ liệu file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportExcelFile(ExportExcelRequestModel requestModel)
        {
            try
            {
                return _excelExportyService.ExportExcelFile(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "Export",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}
