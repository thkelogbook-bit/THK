using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.PMIS;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using ExcelDataReader;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.Http;
using Excel = Microsoft.Office.Interop.Excel;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// PMIS
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/pmis")]
    public class PmisController : BaseApiController
    {
        private readonly IPmisBusiness _pmisBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pmisBusiness"></param>
        /// <param name="commonBusiness"></param>
        public PmisController(IPmisBusiness pmisBusiness,
            ICommonBusiness commonBusiness)
        {
            _pmisBusiness = pmisBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Trả về tất cả Thông số theo điều kiện truyền vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getParameters")]
        [HttpPost]
        public ResponseBaseModel GetParameters([FromBody] SearchParameterModel requestModel)
        {
            return _pmisBusiness.GetParameters(requestModel);
        }

        /// <summary>
        /// Danh sách nhóm thông số PMIS
        /// </summary>
        /// <param name="isHasAll"></param>
        /// <returns></returns>
        [Route("getParameterGroups/{isHasAll}")]
        [HttpGet]
        public List<SelectboxInfoModel> GetParameterGroups(bool isHasAll)
        {
            return _pmisBusiness.GetParameterGroups(isHasAll);
        }

        /// <summary>
        /// Danh sách thông số PMIS
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_PMIS
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetParameterPmiss([FromBody] SearchParameterPmisModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Content = null;
                return responseModel;
            }

            return _pmisBusiness.GetParameterPmiss(requestModel);
        }

        /// <summary>
        /// Cập nhật mã QLKT cho thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateParameter([FromBody] AddEditParameterModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_PMIS)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _pmisBusiness.UpdateParameter(requestModel);
        }

        /// <summary>
        /// Test kết nối đến database server PMIS
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("testConnectionDatabaseSyncPMIS")]
        [HttpPost]
        public ResponseModel<bool> TestConnectionDatabaseSyncPMIS([FromBody] DatabaseConnectionInfoModel requestModel)
        {
            return _pmisBusiness.TestConnectionDatabaseSyncPMIS(requestModel);
        }

        /// <summary>
        /// Lưu thông tin connection database PMIS
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("saveConnectionDatabasePMIS")]
        [HttpPost]
        public ResponseModel<bool> SaveConnectionDatabasePMIS([FromBody] DatabaseConnectionInfoModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_CH_PMIS)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _pmisBusiness.SaveConnectionDatabasePMIS(requestModel);
        }

        /// <summary>
        /// Load thông tin kết nối database server PMIS
        /// </summary>
        /// <returns></returns>
        [Route("loadConnectionDatabasePMIS")]
        [HttpGet]
        public ResponseModel<DatabaseConnectionInfoModel> LoadConnectionDatabasePMIS()
        {
            return _pmisBusiness.LoadConnectionDatabasePMIS();
        }

        /// <summary>
        /// Read data from excel file
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [Route("readDataExcel")]
        [HttpGet]
        [AllowAnonymous]
        public string ReadDataExcel(int type, string filename)
        {
            var result = string.Empty;
            try
            {
                switch (type)
                {
                    //Read data by lib ExcelDataReader
                    case 1:
                        using (var stream = File.Open(filename, FileMode.Open))
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            reader.Read(); // Skip the header

                            int rowValueIndex = 0;
                            while (reader.Read() && rowValueIndex <= 24)
                            {
                                // GET DATE
                                var time = reader.GetValue(0)?.ToString();
                                if (!string.IsNullOrWhiteSpace(time))
                                {
                                    rowValueIndex++;
                                }
                            }

                            result = $"Total cell data (ExcelDataReader): {rowValueIndex}";
                        }
                        break;

                    //Read data by Microsoft.ACE.OLEDB.12.0
                    case 2:
                        var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filename};Extended Properties='Excel 12.0 XML;HDR=NO;IMEX=1;'";
                        using (OleDbConnection connection = new OleDbConnection(connectionString))
                        {
                            connection.Open();

                            DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null) ?? throw new Exception("No sheets found in the Excel file.");
                            var sheetName = schema.Rows[0]["TABLE_NAME"].ToString();
                            string query = $"SELECT * FROM [{sheetName}{"A2"}:{"Y25"}]";

                            using (OleDbCommand command = new OleDbCommand(query, connection))
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                            {
                                DataSet dataSet = new DataSet();
                                adapter.Fill(dataSet);
                                result = CommonFunction.SerializeObjectToString(dataSet);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(result))
                            result = $"Total cell data (Microsoft.ACE.OLEDB.12.0): 0";
                        break;

                    //Read data by Microsoft.Jet.OLEDB.4.0
                    case 3:
                        var connectionStringX = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filename};Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'";
                        using (OleDbConnection connection = new OleDbConnection(connectionStringX))
                        {
                            connection.Open();

                            DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null) ?? throw new Exception("No sheets found in the Excel file.");
                            var sheetName = schema.Rows[0]["TABLE_NAME"].ToString();
                            string query = $"SELECT * FROM [{sheetName}{"A2"}:{"Y25"}]";

                            using (OleDbCommand command = new OleDbCommand(query, connection))
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                            {
                                DataSet dataSet = new DataSet();
                                adapter.Fill(dataSet);
                                result = CommonFunction.SerializeObjectToString(dataSet);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(result))
                            result = $"Total cell data (Microsoft.Jet.OLEDB.4.0): 0";
                        break;

                    //Read data by Microsoft.Office.Interop.Excel
                    case 4:
                        Excel.Application excelApp = new Excel.Application();
                        Excel.Workbook excelWorkbook = null;
                        Excel.Worksheet excelWorksheet = null;
                        int rowValueIndexX = 0;

                        try
                        {
                            excelWorkbook = excelApp.Workbooks.Open(filename, FileMode.Open);
                            excelWorksheet = excelWorkbook.Sheets[1] as Excel.Worksheet;

                            for (int row = 2; row <= 26; row++) // Assuming the data is in rows 2 to 26
                            {
                                var timeCell = excelWorksheet.Cells[row, 1] as Excel.Range;
                                var time = timeCell.Value2?.ToString();

                                if (!string.IsNullOrWhiteSpace(time))
                                {
                                    rowValueIndexX++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                                            Environment.NewLine + $"Không để đọc dữ liệu file: " + filename
                                            + Environment.NewLine + $"Error: {ex}");
                        }
                        finally
                        {
                            excelApp.Quit();

                            // Cleanup COM objects
                            Marshal.ReleaseComObject(excelWorksheet);
                            Marshal.ReleaseComObject(excelWorkbook);
                            Marshal.ReleaseComObject(excelApp);
                            System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite).Dispose();
                        }

                        result = $"Total cell data (Microsoft.Office.Interop.Excel): {rowValueIndexX}";
                        break;

                    //Read data by Microsoft.Office.Interop.Excel (xlsx)
                    case 5:
                        var app = new Excel.Application();
                        var wb = app.Workbooks.Open(filename);
                        var xlsxFile = filename + "x";
                        if (File.Exists(xlsxFile))
                        {
                            File.Delete(xlsxFile); // override save file
                        }
                        wb.SaveAs(Filename: xlsxFile, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook);
                        wb.Close();
                        app.Quit();

                        filename = xlsxFile;
                        int rowValueIndexxx = 0;

                        using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            reader.Read(); // Skip the header

                            while (reader.Read() && rowValueIndexxx <= 24)
                            {
                                // GET DATE
                                var time = reader.GetValue(0)?.ToString();
                                if (!string.IsNullOrWhiteSpace(time))
                                {
                                    rowValueIndexxx++;
                                }
                            }
                        }

                        result = $"Total cell data (Microsoft.Office.Interop.Excel(xlsx)): {rowValueIndexxx}";
                        break;
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }

            return result;
        }
    }
}