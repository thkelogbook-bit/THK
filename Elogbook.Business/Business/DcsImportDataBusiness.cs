using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using System;
using System.Net;

namespace Elogbook.Business
{
    public class DcsImportDataBusiness : IDcsImportDataBusiness
    {
        private readonly IDcsImportDataService _dcsImportDataService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public DcsImportDataBusiness(IDcsImportDataService dcsImportDataService)
        {
            _dcsImportDataService = dcsImportDataService;
        }

        public ResponseModel<bool> ImportDataFromExcelFiles(int day)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _dcsImportDataService.ImportDataFromExcelFiles(day);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "DcsImportDataBusiness", Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
