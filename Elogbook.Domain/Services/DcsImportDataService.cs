using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request;
using Elogbook.Data;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Quản lý import data DCS
    /// </summary>
    public class DcsImportDataService : IDcsImportDataService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IOperationParametersService _operationParametersService;
        private const int TMH1 = 1;
        private const int TMH2 = 2;
        private const int TRUNG_TAM = 3;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="operationParametersService"></param>
        public DcsImportDataService(ElogbookUnitOfWork unitOfWork, IOperationParametersService operationParametersService)
        {
            _unitOfWork = unitOfWork;
            _operationParametersService = operationParametersService;
        }

        /// <summary>
        /// Xử lý import dữ liệu từ files excel
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool ImportDataFromExcelFiles(int day)
        {
            try
            {
                var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted).FirstOrDefault();
                var operationPosition = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => !x.IsDeleted).FirstOrDefault();
                var cycleManagement = _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => !x.IsDeleted).FirstOrDefault();
                var user = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted).FirstOrDefault();

                var requestModel = new AddOperationParameterByTimeModel
                {
                    FactoryID = factory != null ? factory.ID : 0,
                    CycleID = cycleManagement != null ? cycleManagement.ID : 0,
                    OperationPositionID = operationPosition != null ? operationPosition.ID : 0,
                    DateCreate = DateTime.Now.AddDays(day),
                    CurrentUserID = user != null ? user.ID : 0,
                };

                var responseBaseModel = _operationParametersService.ValidateOperationParameterByTime(requestModel);
                if (responseBaseModel.StatusCode == (int)HttpStatusCode.OK)
                {
                    _operationParametersService.AddOperationParameterByTime(requestModel);
                }

                var listOperationTimeLineID = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                            && x.OperationPositionID == requestModel.OperationPositionID
                                                                                            && DbFunctions.TruncateTime(x.InsertedAt) == requestModel.DateCreate.Date
                                                                                            && x.FactoryID == requestModel.FactoryID
                                                                                            && !x.IsDeleted).Select(x => x.ID).ToList();

                var operationParameters = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                                        && x.CycleID == requestModel.CycleID
                                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                                        && listOperationTimeLineID.Contains(x.OperationTimeLineID != null ? (int)x.OperationTimeLineID : 0)
                                                                                                        && !x.IsDeleted).ToList();

                if (operationParameters.Count > 0)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var excelFilePaths = HandleFolderExcel(day);

                    var count = excelFilePaths.Count;
                    if (count == 0)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                          Environment.NewLine + $"Không tìm thấy file excel trong ngày: {DateTime.Now.AddDays(day):yyyyMMdd}");

                        return false;
                    }

                    var queryUpdate = new StringBuilder();
                    List<int> listTimeLineIDUpdate = new List<int>();
                    for (int i = 0; i < count; i++)
                    {
                        var query = ReadAndSaveExcelFile(excelFilePaths[i], operationParameters, requestModel, ref listTimeLineIDUpdate);
                        if (!string.IsNullOrWhiteSpace(query))
                        {
                            queryUpdate.Append(query);
                        }
                        else
                        {
                            LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                                Environment.NewLine + $"Không để đọc dữ liệu file: " + excelFilePaths[i]);
                        }
                    }

                    if (queryUpdate.Length > 0)
                    {
                        var sqOperationParameters = new SqlParameter[]
                        {
                            new SqlParameter("CurrentDate",SqlDbType.DateTime)
                            {
                                Value = DateTime.Now,
                            },
                            new SqlParameter("ErrorCode", SqlDbType.Int)
                            {
                                Value = 0,
                            },
                            new SqlParameter("IsChecked", SqlDbType.Bit)
                            {
                                Value = 1,
                            },
                        };

                        try
                        {
                            _unitOfWork.DataContext.Database.ExecuteSqlCommand(queryUpdate.ToString(), sqOperationParameters);

                            var query = new StringBuilder();
                            foreach (var id in listTimeLineIDUpdate)
                            {
                                query.Append($"UPDATE dbo.OperationTimeLine SET TotalParametersChecked = (SELECT count(1) FROM dbo.OperationParameters WITH(NOLOCK) WHERE OperationTimeLineID = {id} and IsChecked = 1), TotalParametersSynchronized = (SELECT count(1) FROM dbo.OperationParameters WITH(NOLOCK) WHERE OperationTimeLineID = {id} and IsChecked = 1), SyncedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {id};");
                            }

                            if (query.Length > 0)
                            {
                                _unitOfWork.DataContext.Database.ExecuteSqlCommand(query.ToString(), sqOperationParameters);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                            Environment.NewLine + $"Lỗi xảy ra trong khi cập nhật dữ liệu: " + ex.Message);
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    stopwatch.Stop();
                    TimeSpan elapsedTime = stopwatch.Elapsed;

                    LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                           Environment.NewLine + $"Thời gian thực thi: {elapsedTime.TotalSeconds}s");
                }

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                            Environment.NewLine + $"Lỗi xảy ra trong khi cập nhật dữ liệu: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Đọc file và tạo query cập nhật dữ liệu
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <param name="requestModel"></param>
        /// <param name="listTimeLineIDUpdate"></param>
        /// <returns></returns>
        private string ReadAndSaveExcelFile(string filePath, List<OperationParameters> operationParameters, AddOperationParameterByTimeModel requestModel, ref List<int> listTimeLineIDUpdate)
        {
            var query = "";

            try
            {
                List<int> parameterGroupIDs = new List<int>();

                if (filePath.Contains(AppSettings.EXCEL_FILENAME_RUNG_DAO))
                {
                    //Thuộc nhóm 'Độ rung - Độ đảo Tổ máy H1' và 'Độ rung - Độ đảo Tổ máy H2'
                    var doRungDoDaoParameters = operationParameters.Where(x => x.ParameterGroupID == 1 || x.ParameterGroupID == 2).ToList();
                    parameterGroupIDs.Add(1);
                    parameterGroupIDs.Add(2);
                    //query = ReadExcelFileThongSoDoRungDoDao(filePath, doRungDoDaoParameters);
                    query = ReadExcelFileThongSoDoRungDoDao_Hour(filePath, doRungDoDaoParameters, requestModel, parameterGroupIDs, ref listTimeLineIDUpdate);
                    //Nếu đọc file excel từ Microsoft.ACE.OLEDB lỗi thì thực hiện được từ Microsoft.Office.Interop.Excel
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        //query = ReadExcelFileThongSoDoRungDoDao_New(filePath, doRungDoDaoParameters);
                        query = ReadExcelFileThongSoDoRungDoDao_New_Hour(filePath, doRungDoDaoParameters, requestModel, parameterGroupIDs, ref listTimeLineIDUpdate);
                    }
                }

                if (filePath.Contains(AppSettings.EXCEL_FILENAME_TRUNG_TAM))
                {
                    //Thuộc nhóm 'Thiết bị trung tâm'
                    var toMayTrungTamParameters = operationParameters.Where(x => x.ParameterGroupID == 5).ToList();
                    parameterGroupIDs.Add(5);
                    //query = ReadFileExcelThongSoToMayH1H2(filePath, toMayTrungTamParameters);
                    query = ReadFileExcelThongSoToMayH1H2_Hour(filePath, toMayTrungTamParameters, requestModel, parameterGroupIDs, TRUNG_TAM, ref listTimeLineIDUpdate);
                    //Nếu đọc file excel từ Microsoft.ACE.OLEDB lỗi thì thực hiện được từ Microsoft.Office.Interop.Excel
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        //query = ReadFileExcelThongSoToMayH1H2_New(filePath, toMayTrungTamParameters);
                        query = ReadFileExcelThongSoToMayH1H2_New_Hour(filePath, toMayTrungTamParameters, requestModel, parameterGroupIDs, TRUNG_TAM, ref listTimeLineIDUpdate);
                    }
                }

                if (filePath.Contains(AppSettings.EXCEL_FILENAME_TMH1))
                {
                    //Thuộc nhóm 'Thông số vận hành Tổ máy H1'
                    var toMayH1Parameters = operationParameters.Where(x => x.ParameterGroupID == 3).ToList();
                    parameterGroupIDs.Add(3);
                    //query = ReadFileExcelThongSoToMayH1H2(filePath, toMayH1Parameters);
                    query = ReadFileExcelThongSoToMayH1H2_Hour(filePath, toMayH1Parameters, requestModel, parameterGroupIDs, TMH1, ref listTimeLineIDUpdate);
                    //Nếu đọc file excel từ Microsoft.ACE.OLEDB lỗi thì thực hiện được từ Microsoft.Office.Interop.Excel
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        //query = ReadFileExcelThongSoToMayH1H2_New(filePath, toMayH1Parameters);
                        query = ReadFileExcelThongSoToMayH1H2_New_Hour(filePath, toMayH1Parameters, requestModel, parameterGroupIDs, TMH1, ref listTimeLineIDUpdate);
                    }
                }

                if (filePath.Contains(AppSettings.EXCEL_FILENAME_TMH2))
                {
                    //Thuộc nhóm 'Thông số vận hành Tổ máy H2'
                    var toMayH2Parameters = operationParameters.Where(x => x.ParameterGroupID == 4).ToList();
                    parameterGroupIDs.Add(4);
                    //query = ReadFileExcelThongSoToMayH1H2(filePath, toMayH2Parameters);
                    query = ReadFileExcelThongSoToMayH1H2_Hour(filePath, toMayH2Parameters, requestModel, parameterGroupIDs, TMH2, ref listTimeLineIDUpdate);
                    //Nếu đọc file excel từ Microsoft.ACE.OLEDB lỗi thì thực hiện được từ Microsoft.Office.Interop.Excel
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        //query = ReadFileExcelThongSoToMayH1H2_New(filePath, toMayH2Parameters);
                        query = ReadFileExcelThongSoToMayH1H2_New_Hour(filePath, toMayH2Parameters, requestModel, parameterGroupIDs, TMH2, ref listTimeLineIDUpdate);
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                       Environment.NewLine + $"Lỗi trong khi đọc dữ liệu từ file excel: " + ex.ToString());
            }

            return query;
        }

        /// <summary>
        /// Tạo thư mục chưa files
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private List<string> HandleFolderExcel(int day)
        {
            var destinationFolder = AppSettings.EXCEL_DESTINATION_FOLDER_PATH;
            var rootFolder = AppSettings.EXCEL_ROOT_FOLDER_PATH;

            // Lấy ngày hôm nay dưới dạng "yyyyMMdd"
            var todayFolderName = DateTime.Now.AddDays(day).ToString("yyyyMMdd");

            // Tạo đường dẫn đến thư mục mới
            var todayFolderPath = Path.Combine(destinationFolder, todayFolderName);

            // Tạo thư mục mới nếu nó không tồn tại
            if (!Directory.Exists(todayFolderPath))
            {
                Directory.CreateDirectory(todayFolderPath);
            }

            // Gọi hàm để tìm tệp trong thư mục gốc và các thư mục con
            //return FindFileInSubFoldersAndCopy(rootFolder, todayFolderPath, day);
            return FindFileInSubFoldersAndCopy_Hour(rootFolder, todayFolderPath, day);
        }

        /// <summary>
        /// Copy files theo ngày
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private List<string> FindFileInSubFoldersAndCopy(string folderPath, string destinationFolder, int day)
        {
            var destinationFilePaths = new List<string>();
            try
            {
                var subfolders = Directory.GetDirectories(folderPath);
                var now = DateTime.Now.AddDays(day).ToString("yyyyMMdd");
                var fileNameH1 = $"{AppSettings.EXCEL_FILENAME_TMH1}_{now}";
                var fileNameH2 = $"{AppSettings.EXCEL_FILENAME_TMH2}_{now}";
                var fileNameRungDao = $"{AppSettings.EXCEL_FILENAME_RUNG_DAO}_{now}";
                var fileNameTrungTam = $"{AppSettings.EXCEL_FILENAME_TRUNG_TAM}_{now}";

                foreach (string subfolder in subfolders)
                {
                    // Lấy duy nhất một tệp có tên chứa ngày hiện tại
                    var matchingFile = Directory.GetFiles(subfolder)
                                                   .FirstOrDefault(file => Path.GetFileName(file).Contains(fileNameH1)
                                                    || Path.GetFileName(file).Contains(fileNameH2)
                                                    || Path.GetFileName(file).Contains(fileNameRungDao)
                                                    || Path.GetFileName(file).Contains(fileNameTrungTam)); // Lấy tệp đầu tiên nếu có

                    if (!string.IsNullOrWhiteSpace(matchingFile))
                    {
                        // Định dạng đường dẫn đến tệp gốc và đích
                        var sourcePath = matchingFile;
                        var destinationPath = Path.Combine(destinationFolder, Path.GetFileName(matchingFile));

                        // Sao chép tệp
                        File.Copy(sourcePath, destinationPath, true);

                        destinationFilePaths.Add(destinationPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                       Environment.NewLine + $"Không tìm thấy file: " + ex.ToString());
            }

            return destinationFilePaths;
        }

        /// <summary>
        /// Copy files theo giờ
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private List<string> FindFileInSubFoldersAndCopy_Hour(string folderPath, string destinationFolder, int day)
        {
            var destinationFilePaths = new List<string>();
            try
            {
                var subfolders = Directory.GetDirectories(folderPath);
                var now = DateTime.Now.AddDays(day).ToString("yyyyMMdd");
                var fileNameH1 = $"{AppSettings.EXCEL_FILENAME_TMH1}_{now}";
                var fileNameH2 = $"{AppSettings.EXCEL_FILENAME_TMH2}_{now}";
                var fileNameRungDao = $"{AppSettings.EXCEL_FILENAME_RUNG_DAO}_{now}";
                var fileNameTrungTam = $"{AppSettings.EXCEL_FILENAME_TRUNG_TAM}_{now}";

                foreach (string subfolder in subfolders)
                {
                    // Lấy duy nhất một tệp có tên chứa ngày hiện tại
                    var files = Directory.GetFiles(subfolder)
                                                   .Where(file => Path.GetFileName(file).Contains(fileNameH1)
                                                    || Path.GetFileName(file).Contains(fileNameH2)
                                                    || Path.GetFileName(file).Contains(fileNameRungDao)
                                                    || Path.GetFileName(file).Contains(fileNameTrungTam));

                    foreach (var fileName in files)
                    {
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            // Định dạng đường dẫn đến tệp gốc và đích
                            var sourcePath = fileName;
                            var destinationPath = Path.Combine(destinationFolder, Path.GetFileName(fileName));

                            // Sao chép tệp
                            File.Copy(sourcePath, destinationPath, true);

                            destinationFilePaths.Add(destinationPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                       Environment.NewLine + $"Không tìm thấy file: " + ex.ToString());
            }

            return destinationFilePaths;
        }

        #region Read excel OLEDB

        /// <summary>
        /// Đọc file theo ngày
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <returns></returns>
        private string ReadExcelFileThongSoDoRungDoDao(string filePath, List<OperationParameters> operationParameters)
        {
            string colRowStart = "A2";
            string colRowEnd = "Y25";
            var queryUpdate = new StringBuilder();
            DataTable dataTable = GetExcelDataTable(filePath, colRowStart, colRowEnd);

            int rowValueIndex = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                rowValueIndex++;

                // Lấy dòng đầu tiên
                var ops = operationParameters
                    .Where(x => x.OperationTimeLine.TimeLineName == rowValueIndex)
                    .OrderBy(x => x.ExcelAIColIndex)
                    .ToList();

                Parallel.ForEach(ops, item =>
                {
                    var cellValue = dataRow.Field<string>(item.ExcelAIColIndex.Value);
                    if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                    {
                        lock (queryUpdate)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                });
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Lấy dữ liệu thông số cần cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="createDate"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        private List<OperationParameters> GetOperationParametersByHour(AddOperationParameterByTimeModel requestModel, DateTime createDate, int hour, List<int> parameterGroupIDs)
        {
            createDate = createDate.AddDays(-1);
            var listOperationTimeLineID = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                            && x.OperationPositionID == requestModel.OperationPositionID
                                                                                            && DbFunctions.TruncateTime(x.InsertedAt) == createDate.Date
                                                                                            && x.TimeLineName == hour
                                                                                            && x.FactoryID == requestModel.FactoryID
                                                                                            && !x.IsDeleted).Select(x => x.ID).ToList();

            var oppData = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                                    && x.CycleID == requestModel.CycleID
                                                                                                    && x.OperationPositionID == requestModel.OperationPositionID
                                                                                                    && listOperationTimeLineID.Contains(x.OperationTimeLineID != null ? (int)x.OperationTimeLineID : 0)
                                                                                                    && parameterGroupIDs.Contains(x.ParameterGroupID != null ? (int)x.ParameterGroupID : 0)
                                                                                                    && !x.IsDeleted).ToList();

            return oppData;
        }

        /// <summary>
        /// Đọc file theo giờ
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <param name="requestModel"></param>
        /// <param name="listTimeLineIDUpdate"></param>
        /// <returns></returns>
        private string ReadExcelFileThongSoDoRungDoDao_Hour(string filePath, List<OperationParameters> operationParameters, AddOperationParameterByTimeModel requestModel, List<int> parameterGroupIDs, ref List<int> listTimeLineIDUpdate)
        {
            string colRowStart = "A2";
            string colRowEnd = "Y2";
            var queryUpdate = new StringBuilder();
            DataTable dataTable = GetExcelDataTable(filePath, colRowStart, colRowEnd);

            int timeLineName = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var cellTimeValue = dataRow.Field<string>(0);
                DateTime dateTime = Convert.ToDateTime(cellTimeValue);
                if (dateTime.Hour == 0 && dateTime.ToString().ToUpper().EndsWith("AM"))
                {
                    timeLineName = 24;
                    var oppData = GetOperationParametersByHour(requestModel, dateTime, timeLineName, parameterGroupIDs);

                    // Lấy dòng đầu tiên
                    var ops = oppData
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                    .ToList();

                    if (ops.Count > 0)
                    {
                        var firstItem = ops.FirstOrDefault();
                        if (firstItem != null && firstItem.OperationTimeLineID.GetValueOrDefault() > 0)
                        {
                            var operationTimeLineID = firstItem.OperationTimeLineID.GetValueOrDefault();
                            if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                            {
                                listTimeLineIDUpdate.Add(operationTimeLineID);
                            }
                        }
                    }

                    Parallel.ForEach(ops, item =>
                    {
                        var isUpdate = true;

                        //Lấy giá trị cột hiện tại
                        var cellValue = dataRow.Field<string>(item.ExcelAIColIndex.Value);
                        float.TryParse(cellValue, out float numericValue);

                        //Lấy giá trị cột 1U_AI07 (index = 1)
                        var cellValue1 = dataRow.Field<string>(1);
                        float.TryParse(cellValue1, out float numericValue1);

                        //Lấy giá trị cột 2U_AI07 (index = 13)
                        var cellValue13 = dataRow.Field<string>(13);
                        float.TryParse(cellValue13, out float numericValue13);

                        //1U_AI07 (index = 1), 2U_AI07 (index = 13)
                        if (item.ExcelAIColIndex.GetValueOrDefault() == 1 || item.ExcelAIColIndex.GetValueOrDefault() == 13)
                        {
                            if (numericValue < 0)
                            {
                                isUpdate = false;
                            }
                        }
                        //1U_AI08 (index = 2)
                        else if (item.ExcelAIColIndex.GetValueOrDefault() == 2)
                        {
                            if (numericValue1 < 0)
                            {
                                isUpdate = false;
                            }
                            else if (numericValue1 > 0)
                            {
                                isUpdate = true;
                            }
                        }
                        //1U_AI31, 1U_AI32, 1U_AI33, 1U_AI34, 1U_AI35, 1U_AI36, 1U_AI37, 1U_AI38, 1U_AI39, 1U_AI40
                        else if (item.ExcelAIColIndex.GetValueOrDefault() < 13)
                        {
                            if (numericValue1 < 0 || (numericValue < 0 && numericValue1 > 0))
                            {
                                isUpdate = false;
                            }
                        }
                        //2U_AI08 (index = 14)
                        else if (item.ExcelAIColIndex.GetValueOrDefault() == 14)
                        {
                            if (numericValue13 < 0)
                            {
                                isUpdate = false;
                            }
                            else if (numericValue13 > 0)
                            {
                                isUpdate = true;
                            }
                        }
                        //2U_AI31, 2U_AI32, 2U_AI33, 2U_AI34, 2U_AI35, 2U_AI36, 2U_AI37, 2U_AI38, 2U_AI39, 2U_AI40
                        else if (item.ExcelAIColIndex.GetValueOrDefault() > 14
                            && (numericValue13 < 0 || (numericValue < 0 && numericValue13 > 0)))
                        {
                            isUpdate = false;
                        }

                        if (isUpdate && !item.IsChecked)
                        {
                            lock (queryUpdate)
                            {
                                queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                            }
                        }
                    });
                }
                else
                {
                    timeLineName = dateTime.Hour;
                    if (timeLineName == 1 && dateTime.Date != requestModel.DateCreate.Date)
                    {
                        continue;
                    }

                    // Lấy dòng đầu tiên
                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                    .ToList();

                    if (ops.Count > 0)
                    {
                        var firstItem = ops.FirstOrDefault();
                        if (firstItem != null && firstItem.OperationTimeLineID.GetValueOrDefault() > 0)
                        {
                            var operationTimeLineID = firstItem.OperationTimeLineID.GetValueOrDefault();
                            if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                            {
                                listTimeLineIDUpdate.Add(operationTimeLineID);
                            }
                        }
                    }

                    Parallel.ForEach(ops, item =>
                    {
                        var isUpdate = true;

                        //Lấy giá trị cột hiện tại
                        var cellValue = dataRow.Field<string>(item.ExcelAIColIndex.Value);
                        float.TryParse(cellValue, out float numericValue);

                        //Lấy giá trị cột 1U_AI07 (index = 1)
                        var cellValue1 = dataRow.Field<string>(1);
                        float.TryParse(cellValue1, out float numericValue1);

                        //Lấy giá trị cột 2U_AI07 (index = 13)
                        var cellValue13 = dataRow.Field<string>(13);
                        float.TryParse(cellValue13, out float numericValue13);

                        //1U_AI07 (index = 1), 2U_AI07 (index = 13)
                        if (item.ExcelAIColIndex.GetValueOrDefault() == 1 || item.ExcelAIColIndex.GetValueOrDefault() == 13)
                        {
                            if (numericValue < 0)
                            {
                                isUpdate = false;
                            }
                        }
                        //1U_AI08 (index = 2)
                        else if (item.ExcelAIColIndex.GetValueOrDefault() == 2)
                        {
                            if (numericValue1 < 0)
                            {
                                isUpdate = false;
                            }
                            else if (numericValue1 > 0)
                            {
                                isUpdate = true;
                            }
                        }
                        //1U_AI31, 1U_AI32, 1U_AI33, 1U_AI34, 1U_AI35, 1U_AI36, 1U_AI37, 1U_AI38, 1U_AI39, 1U_AI40
                        else if (item.ExcelAIColIndex.GetValueOrDefault() < 13)
                        {
                            if (numericValue1 < 0 || (numericValue < 0 && numericValue1 > 0))
                            {
                                isUpdate = false;
                            }
                        }
                        //2U_AI08 (index = 14)
                        else if (item.ExcelAIColIndex.GetValueOrDefault() == 14)
                        {
                            if (numericValue13 < 0)
                            {
                                isUpdate = false;
                            }
                            else if (numericValue13 > 0)
                            {
                                isUpdate = true;
                            }
                        }
                        //2U_AI31, 2U_AI32, 2U_AI33, 2U_AI34, 2U_AI35, 2U_AI36, 2U_AI37, 2U_AI38, 2U_AI39, 2U_AI40
                        else if (item.ExcelAIColIndex.GetValueOrDefault() > 14
                            && (numericValue13 < 0 || (numericValue < 0 && numericValue13 > 0)))
                        {
                            isUpdate = false;
                        }

                        if (isUpdate && !item.IsChecked)
                        {
                            lock (queryUpdate)
                            {
                                queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                            }
                        }
                    });
                }
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Đọc file theo ngày
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <returns></returns>
        private string ReadFileExcelThongSoToMayH1H2(string filePath, List<OperationParameters> operationParameters)
        {
            int rowIndex = 0;
            int rowValueIndex = 0;
            string colRowStart = "A11";
            string colRowEnd = "CY57";
            var queryUpdate = new StringBuilder();
            DataTable dataTable = GetExcelDataTable(filePath, colRowStart, colRowEnd);

            // Bắt đầu đọc data
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (rowIndex % 2 != 0)
                {
                    rowIndex++;
                    continue;
                }

                var time = dataRow.Field<string>(1);
                if (!string.IsNullOrWhiteSpace(time))
                {
                    rowValueIndex++;
                }

                var ops = operationParameters
                    .Where(x => x.OperationTimeLine.TimeLineName == rowValueIndex)
                    .OrderBy(x => x.ExcelAIColIndex)
                    .ToList();

                Parallel.ForEach(ops, item =>
                {
                    var cellValue = dataRow.Field<string>(item.ExcelAIColIndex.Value);
                    if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                    {
                        lock (queryUpdate)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                });

                rowIndex++;
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Đọc file theo giờ
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <param name="requestModel"></param>
        /// <param name="listTimeLineIDUpdate"></param>
        /// <returns></returns>
        private string ReadFileExcelThongSoToMayH1H2_Hour(string filePath, List<OperationParameters> operationParameters, AddOperationParameterByTimeModel requestModel, List<int> parameterGroupIDs, int type, ref List<int> listTimeLineIDUpdate)
        {
            int rowIndex = 0;
            string colRowStart = "A11";
            string colRowEnd = "CY11";
            var queryUpdate = new StringBuilder();
            DataTable dataTable = GetExcelDataTable(filePath, colRowStart, colRowEnd);
            // Bắt đầu đọc data
            int timeLineName = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (rowIndex % 2 != 0)
                {
                    rowIndex++;
                    continue;
                }

                var cellTimeValue = dataRow.Field<string>(1);
                DateTime dateTime = Convert.ToDateTime(cellTimeValue);
                if (dateTime.Hour == 0 && dateTime.ToString().ToUpper().EndsWith("AM"))
                {
                    timeLineName = 24;
                    var oppData = GetOperationParametersByHour(requestModel, dateTime, timeLineName, parameterGroupIDs);

                    // Lấy dòng đầu tiên
                    var ops = oppData
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                    .ToList();

                    if (ops.Count > 0)
                    {
                        var firstItem = ops.FirstOrDefault();
                        if (firstItem != null && firstItem.OperationTimeLineID.GetValueOrDefault() > 0)
                        {
                            var operationTimeLineID = firstItem.OperationTimeLineID.GetValueOrDefault();
                            if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                            {
                                listTimeLineIDUpdate.Add(operationTimeLineID);
                            }
                        }
                    }

                    Parallel.ForEach(ops, item =>
                    {
                        switch (type)
                        {
                            case TMH1:
                                var cellValueH1 = dataRow.Field<string>(item.ExcelAIColIndex.GetValueOrDefault());
                                float.TryParse(cellValueH1, out float numericValueH1);
                                //1U_AI07 (index = 2), 1U_AI10 (index = 4), 1U_AI23 (index = 6)
                                if (item.ExcelAIColIndex.GetValueOrDefault() == 4 || item.ExcelAIColIndex.GetValueOrDefault() == 6)
                                {
                                    //Lấy giá trị cột 1U_AI07
                                    var cellValueH1_2 = dataRow.Field<string>(2);
                                    float.TryParse(cellValueH1_2, out float numericValueH1_2);
                                    if (numericValueH1_2 < 0 || (numericValueH1 < 0 && numericValueH1_2 > 0))
                                        break;
                                }
                                //1U_AI07, 1U_AI27 and other
                                else
                                {
                                    if (numericValueH1 < 0)
                                        break;
                                }

                                if (!item.IsChecked)
                                {
                                    lock (queryUpdate)
                                    {
                                        queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValueH1}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                                    }
                                }
                                break;

                            case TMH2:
                                var cellValueH2 = dataRow.Field<string>(item.ExcelAIColIndex.GetValueOrDefault());
                                float.TryParse(cellValueH2, out float numericValueH2);
                                //2U_AI07 (index = 2), 2U_AI10 (index = 4), 2U_AI23 (index = 6)
                                if (item.ExcelAIColIndex.GetValueOrDefault() == 4 || item.ExcelAIColIndex.GetValueOrDefault() == 6)
                                {
                                    //Lấy giá trị cột 2U_AI07
                                    var cellValueH2_2 = dataRow.Field<string>(2);
                                    float.TryParse(cellValueH2_2, out float numericValueH2_2);
                                    if (numericValueH2_2 < 0 || (numericValueH2 < 0 && numericValueH2_2 > 0))
                                        break;
                                }
                                //2U_AI07, 2U_AI27 and other
                                else
                                {
                                    if (numericValueH2 < 0)
                                        break;
                                }

                                if (!item.IsChecked)
                                {
                                    lock (queryUpdate)
                                    {
                                        queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValueH2}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                                    }
                                }
                                break;

                            case TRUNG_TAM:
                                //Lấy giá trị cột hiện tại
                                var cellValue = dataRow.Field<string>(item.ExcelAIColIndex.Value);
                                float.TryParse(cellValue, out float numericValue);
                                //Lấy giá trị cột 1U_AI07 (index = 2)
                                var cellValue2 = dataRow.Field<string>(2);
                                float.TryParse(cellValue2, out float numericValue2);
                                //Lấy giá trị cột 2U_AI07 (index = 14)
                                var cellValue14 = dataRow.Field<string>(14);
                                float.TryParse(cellValue14, out float numericValue14);

                                //1U_AI08 (index = 4)
                                if (item.ExcelAIColIndex.GetValueOrDefault() == 4)
                                {
                                    if (numericValue2 < 0)
                                        break;
                                }
                                //1U_AI04 (index = 6), 1U_AI01 (index = 8), 1U_AI17 (index = 10), 1U_AI18 (index = 12)
                                else if (item.ExcelAIColIndex.GetValueOrDefault() == 6
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 8
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 10
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 12)
                                {
                                    if (numericValue2 < 0 || (numericValue < 0 && numericValue2 > 0))
                                        break;
                                }
                                //2U_AI08 (index = 16)
                                else if (item.ExcelAIColIndex.GetValueOrDefault() == 16)
                                {
                                    if (numericValue14 < 0)
                                        break;
                                }
                                //2U_AI04 (index = 18), 2U_AI01 (index = 20), 2U_AI17 (index = 22), 2U_AI18 (index = 24)
                                else if (item.ExcelAIColIndex.GetValueOrDefault() == 18
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 20
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 22
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 24)
                                {
                                    if (numericValue14 < 0 || (numericValue < 0 && numericValue14 > 0))
                                        break;
                                }
                                else
                                {
                                    //SC_AI07 (index = 26), SC_AI08 (index = 28), SC_AI16 (index = 36), SC_AI17 (index = 38) nhỏ hơn không vẫn đưa vào Elogbook
                                    if (numericValue < 0 && item.ExcelAIColIndex.GetValueOrDefault() != 26
                                        && item.ExcelAIColIndex.GetValueOrDefault() != 28
                                        && item.ExcelAIColIndex.GetValueOrDefault() != 36
                                        && item.ExcelAIColIndex.GetValueOrDefault() != 38)
                                        break;
                                }

                                if (!item.IsChecked)
                                {
                                    lock (queryUpdate)
                                    {
                                        queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                                    }
                                }
                                break;
                        }
                    });
                }
                else
                {
                    timeLineName = dateTime.Hour;
                    if(timeLineName == 1 && dateTime.Date != requestModel.DateCreate.Date)
                    {
                        continue;
                    }

                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    if (ops.Count > 0)
                    {
                        var firstItem = ops.FirstOrDefault();
                        if (firstItem != null && firstItem.OperationTimeLineID.GetValueOrDefault() > 0)
                        {
                            var operationTimeLineID = firstItem.OperationTimeLineID.GetValueOrDefault();
                            if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                            {
                                listTimeLineIDUpdate.Add(operationTimeLineID);
                            }
                        }
                    }

                    Parallel.ForEach(ops, item =>
                    {
                        switch (type)
                        {
                            case TMH1:
                                var cellValueH1 = dataRow.Field<string>(item.ExcelAIColIndex.GetValueOrDefault());
                                float.TryParse(cellValueH1, out float numericValueH1);
                                //1U_AI07 (index = 2), 1U_AI10 (index = 4), 1U_AI23 (index = 6)
                                if (item.ExcelAIColIndex.GetValueOrDefault() == 4 || item.ExcelAIColIndex.GetValueOrDefault() == 6)
                                {
                                    //Lấy giá trị cột 1U_AI07
                                    var cellValueH1_2 = dataRow.Field<string>(2);
                                    float.TryParse(cellValueH1_2, out float numericValueH1_2);
                                    if (numericValueH1_2 < 0 || (numericValueH1 < 0 && numericValueH1_2 > 0))
                                        break;
                                }
                                //1U_AI07, 1U_AI27 and other
                                else
                                {
                                    if (numericValueH1 < 0)
                                        break;
                                }

                                if (!item.IsChecked)
                                {
                                    lock (queryUpdate)
                                    {
                                        queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValueH1}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                                    }
                                }
                                break;

                            case TMH2:
                                var cellValueH2 = dataRow.Field<string>(item.ExcelAIColIndex.GetValueOrDefault());
                                float.TryParse(cellValueH2, out float numericValueH2);
                                //2U_AI07 (index = 2), 2U_AI10 (index = 4), 2U_AI23 (index = 6)
                                if (item.ExcelAIColIndex.GetValueOrDefault() == 4 || item.ExcelAIColIndex.GetValueOrDefault() == 6)
                                {
                                    //Lấy giá trị cột 2U_AI07
                                    var cellValueH2_2 = dataRow.Field<string>(2);
                                    float.TryParse(cellValueH2_2, out float numericValueH2_2);
                                    if (numericValueH2_2 < 0 || (numericValueH2 < 0 && numericValueH2_2 > 0))
                                        break;
                                }
                                //2U_AI07, 2U_AI27 and other
                                else
                                {
                                    if (numericValueH2 < 0)
                                        break;
                                }

                                if (!item.IsChecked)
                                {
                                    lock (queryUpdate)
                                    {
                                        queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValueH2}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                                    }
                                }
                                break;

                            case TRUNG_TAM:
                                //Lấy giá trị cột hiện tại
                                var cellValue = dataRow.Field<string>(item.ExcelAIColIndex.Value);
                                float.TryParse(cellValue, out float numericValue);
                                //Lấy giá trị cột 1U_AI07 (index = 2)
                                var cellValue2 = dataRow.Field<string>(2);
                                float.TryParse(cellValue2, out float numericValue2);
                                //Lấy giá trị cột 2U_AI07 (index = 14)
                                var cellValue14 = dataRow.Field<string>(14);
                                float.TryParse(cellValue14, out float numericValue14);

                                //1U_AI08 (index = 4)
                                if (item.ExcelAIColIndex.GetValueOrDefault() == 4)
                                {
                                    if (numericValue2 < 0)
                                        break;
                                }
                                //1U_AI04 (index = 6), 1U_AI01 (index = 8), 1U_AI17 (index = 10), 1U_AI18 (index = 12)
                                else if (item.ExcelAIColIndex.GetValueOrDefault() == 6
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 8
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 10
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 12)
                                {
                                    if (numericValue2 < 0 || (numericValue < 0 && numericValue2 > 0))
                                        break;
                                }
                                //2U_AI08 (index = 16)
                                else if (item.ExcelAIColIndex.GetValueOrDefault() == 16)
                                {
                                    if (numericValue14 < 0)
                                        break;
                                }
                                //2U_AI04 (index = 18), 2U_AI01 (index = 20), 2U_AI17 (index = 22), 2U_AI18 (index = 24)
                                else if (item.ExcelAIColIndex.GetValueOrDefault() == 18
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 20
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 22
                                    || item.ExcelAIColIndex.GetValueOrDefault() == 24)
                                {
                                    if (numericValue14 < 0 || (numericValue < 0 && numericValue14 > 0))
                                        break;
                                }
                                else
                                {
                                    //SC_AI07 (index = 26), SC_AI08 (index = 28), SC_AI16 (index = 36), SC_AI17 (index = 38) nhỏ hơn không vẫn đưa vào Elogbook
                                    if (numericValue < 0 && item.ExcelAIColIndex.GetValueOrDefault() != 26
                                        && item.ExcelAIColIndex.GetValueOrDefault() != 28
                                        && item.ExcelAIColIndex.GetValueOrDefault() != 36
                                        && item.ExcelAIColIndex.GetValueOrDefault() != 38)
                                        break;
                                }

                                if (!item.IsChecked)
                                {
                                    lock (queryUpdate)
                                    {
                                        queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                                    }
                                }
                                break;
                        }
                    });
                }

                rowIndex++;
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'"
        /// $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 XML;HDR=NO;IMEX=1;'"
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="colRowStart"></param>
        /// <param name="colRowEnd"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private DataTable GetExcelDataTable(string filePath, string colRowStart, string colRowEnd)
        {
            var connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch
                {
                    connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 XML;HDR=NO;IMEX=1;'";
                }
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch
                {
                    return new DataTable();
                }

                DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null) ?? throw new Exception("No sheets found in the Excel file.");
                var sheetName = schema.Rows[0]["TABLE_NAME"].ToString();
                string query = $"SELECT * FROM [{sheetName}{colRowStart}:{colRowEnd}]";

                using (OleDbCommand command = new OleDbCommand(query, connection))
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    return dataSet.Tables[0];
                }
            }
        }

        #endregion

        #region Read XSL using interop excel

        /// <summary>
        /// Đọc file theo ngày bằng Interop
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <returns></returns>
        private string ReadExcelFileThongSoDoRungDoDao_New(string filePath, List<OperationParameters> operationParameters)
        {
            var queryUpdate = new StringBuilder();

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = null;
            Excel.Worksheet excelWorksheet = null;

            try
            {
                excelWorkbook = excelApp.Workbooks.Open(filePath, FileMode.Open);
                excelWorksheet = excelWorkbook.Sheets[1] as Excel.Worksheet;

                int rowValueIndex = 0;
                for (int row = 2; row <= 26; row++) // Assuming the data is in rows 2 to 26
                {
                    var timeCell = excelWorksheet.Cells[row, 1] as Excel.Range;
                    var time = timeCell.Value2?.ToString();

                    if (!string.IsNullOrWhiteSpace(time))
                    {
                        rowValueIndex++;
                    }

                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == rowValueIndex)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    foreach (var item in ops)
                    {
                        var cell = excelWorksheet.Cells[row, item.ExcelAIColIndex.Value + 1] as Excel.Range;
                        var cellValue = cell.Value2?.ToString();

                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                                Environment.NewLine + $"Không để đọc dữ liệu file: " + filePath
                                + Environment.NewLine + $"Error: {ex}");
            }
            finally
            {
                excelApp.Quit();

                // Cleanup COM objects
                Marshal.ReleaseComObject(excelWorksheet);
                Marshal.ReleaseComObject(excelWorkbook);
                Marshal.ReleaseComObject(excelApp);
                System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite).Dispose();
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Đọc file theo giờ bằng Interop
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <param name="requestModel"></param>
        /// <param name="listTimeLineIDUpdate"></param>
        /// <returns></returns>
        private string ReadExcelFileThongSoDoRungDoDao_New_Hour(string filePath, List<OperationParameters> operationParameters, AddOperationParameterByTimeModel requestModel, List<int> parameterGroupIDs, ref List<int> listTimeLineIDUpdate)
        {
            var queryUpdate = new StringBuilder();

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = null;
            Excel.Worksheet excelWorksheet = null;

            try
            {
                excelWorkbook = excelApp.Workbooks.Open(filePath, FileMode.Open);
                excelWorksheet = excelWorkbook.Sheets[1] as Excel.Worksheet;

                int startColumnIndex = 1;
                int startRowIndex = 2;
                int timeLineName = 0;
                var cellTimeValue = excelWorksheet.Cells[startRowIndex, startColumnIndex] as Excel.Range;
                DateTime dateTime = Convert.ToDateTime(cellTimeValue);
                if (dateTime.Hour == 0 && dateTime.ToString().ToUpper().EndsWith("AM"))
                {
                    timeLineName = 24;
                    var oppData = GetOperationParametersByHour(requestModel, dateTime, timeLineName, parameterGroupIDs);

                    // Lấy dòng đầu tiên
                    var ops = oppData
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    foreach (var item in ops)
                    {
                        var operationTimeLineID = item.OperationTimeLineID.GetValueOrDefault();
                        if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                        {
                            listTimeLineIDUpdate.Add(operationTimeLineID);
                        }

                        var cell = excelWorksheet.Cells[startRowIndex, item.ExcelAIColIndex.Value + 1] as Excel.Range;
                        var cellValue = cell.Value2?.ToString();

                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                }
                else
                {
                    timeLineName = dateTime.Hour;

                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    foreach (var item in ops)
                    {
                        var operationTimeLineID = item.OperationTimeLineID.GetValueOrDefault();
                        if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                        {
                            listTimeLineIDUpdate.Add(operationTimeLineID);
                        }

                        var cell = excelWorksheet.Cells[startRowIndex, item.ExcelAIColIndex.Value + 1] as Excel.Range;
                        var cellValue = cell.Value2?.ToString();

                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                                Environment.NewLine + $"Không để đọc dữ liệu file: " + filePath
                                + Environment.NewLine + $"Error: {ex}");
            }
            finally
            {
                excelApp.Quit();

                // Cleanup COM objects
                Marshal.ReleaseComObject(excelWorksheet);
                Marshal.ReleaseComObject(excelWorkbook);
                Marshal.ReleaseComObject(excelApp);
                System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite).Dispose();
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Đọc file theo ngày bằng Interop
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <returns></returns>
        private string ReadFileExcelThongSoToMayH1H2_New(string filePath, List<OperationParameters> operationParameters)
        {
            var queryUpdate = new StringBuilder();

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = null;
            Excel.Worksheet excelWorksheet = null;

            try
            {
                excelWorkbook = excelApp.Workbooks.Open(filePath);
                excelWorksheet = excelWorkbook.Sheets[1] as Excel.Worksheet;

                int rowValueIndex = 0;
                int skipRowIndex = 11;
                int lastRowIndex = 57;

                for (int row = 1; row <= lastRowIndex; row++)
                {
                    if (row < skipRowIndex || row % 2 == 0)
                    {
                        continue;
                    }

                    var timeCell = excelWorksheet.Cells[row, 2] as Excel.Range;
                    var time = timeCell.Value2?.ToString();

                    if (!string.IsNullOrWhiteSpace(time))
                    {
                        rowValueIndex++;
                    }

                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == rowValueIndex)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    foreach (var item in ops)
                    {
                        var cell = excelWorksheet.Cells[row, item.ExcelAIColIndex.Value + 1] as Excel.Range;
                        var cellValue = cell.Value2?.ToString();

                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                                Environment.NewLine + $"Không để đọc dữ liệu file: " + filePath
                                + Environment.NewLine + $"Error: {ex}");
            }
            finally
            {
                excelApp.Quit();

                // Cleanup COM objects
                Marshal.ReleaseComObject(excelWorksheet);
                Marshal.ReleaseComObject(excelWorkbook);
                Marshal.ReleaseComObject(excelApp);
                System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite).Dispose();
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Đọc file theo giờ bằng Interop
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <param name="requestModel"></param>
        /// <param name="listTimeLineIDUpdate"></param>
        /// <returns></returns>
        private string ReadFileExcelThongSoToMayH1H2_New_Hour(string filePath, List<OperationParameters> operationParameters, AddOperationParameterByTimeModel requestModel, List<int> parameterGroupIDs, int type, ref List<int> listTimeLineIDUpdate)
        {
            var queryUpdate = new StringBuilder();

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = null;
            Excel.Worksheet excelWorksheet = null;

            try
            {
                excelWorkbook = excelApp.Workbooks.Open(filePath);
                excelWorksheet = excelWorkbook.Sheets[1] as Excel.Worksheet;

                int startColumnIndex = 2;
                int startRowIndex = 11;
                int timeLineName = 0;

                var cellTimeValue = excelWorksheet.Cells[startRowIndex, startColumnIndex] as Excel.Range;
                DateTime dateTime = Convert.ToDateTime(cellTimeValue);
                if (dateTime.Hour == 0 && dateTime.ToString().ToUpper().EndsWith("AM"))
                {
                    timeLineName = 24;
                    var oppData = GetOperationParametersByHour(requestModel, dateTime, timeLineName, parameterGroupIDs);

                    // Lấy dòng đầu tiên
                    var ops = oppData
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    foreach (var item in ops)
                    {
                        var operationTimeLineID = item.OperationTimeLineID.GetValueOrDefault();
                        if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                        {
                            listTimeLineIDUpdate.Add(operationTimeLineID);
                        }

                        var cell = excelWorksheet.Cells[startRowIndex, item.ExcelAIColIndex.Value + 1] as Excel.Range;
                        var cellValue = cell.Value2?.ToString();

                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                }
                else
                {
                    timeLineName = dateTime.Hour;

                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == timeLineName)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    foreach (var item in ops)
                    {
                        var operationTimeLineID = item.OperationTimeLineID.GetValueOrDefault();
                        if (!listTimeLineIDUpdate.Contains(operationTimeLineID))
                        {
                            listTimeLineIDUpdate.Add(operationTimeLineID);
                        }

                        var cell = excelWorksheet.Cells[startRowIndex, item.ExcelAIColIndex.Value + 1] as Excel.Range;
                        var cellValue = cell.Value2?.ToString();

                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                                Environment.NewLine + $"Không để đọc dữ liệu file: " + filePath
                                + Environment.NewLine + $"Error: {ex}");
            }
            finally
            {
                excelApp.Quit();

                // Cleanup COM objects
                Marshal.ReleaseComObject(excelWorksheet);
                Marshal.ReleaseComObject(excelWorkbook);
                Marshal.ReleaseComObject(excelApp);
                System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite).Dispose();
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Chuyển đổi file từ xls sang xlsx
        /// </summary>
        /// <param name="xlsFilePath"></param>
        /// <param name="xlsxFilePath"></param>
        public void ConvertXlsToXlsx(string xlsFilePath, string xlsxFilePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = null;

            try
            {
                // Open the XLS file
                excelWorkbook = excelApp.Workbooks.Open(xlsFilePath);

                // Save it as XLSX
                excelWorkbook.SaveAs(xlsxFilePath, Excel.XlFileFormat.xlOpenXMLWorkbook, ConflictOption.OverwriteChanges);

                // Close the workbook
                excelWorkbook.Close(false);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DcsImportDataService.ToString(),
                                Environment.NewLine + $"Không thể chuyển đổi file: " + xlsFilePath
                                + Environment.NewLine + $"Error: {ex}");
            }
            finally
            {
                // Cleanup COM objects
                Marshal.ReleaseComObject(excelWorkbook);
                Marshal.ReleaseComObject(excelApp);
            }
        }

        #endregion

        #region ExcelReader dll

        /// <summary>
        /// Đọc file bằng lib ExcelReaderFactory
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <returns></returns>
        public string ReadExcelFileThongSoDoRungDoDao_Old(string filePath, List<OperationParameters> operationParameters)
        {
            var queryUpdate = new StringBuilder();
            using (var stream = File.Open(filePath, FileMode.Open))
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

                    // Lấy dòng đầu tiên 
                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == rowValueIndex)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    Parallel.ForEach(ops, item =>
                    {
                        var cellValue = reader.GetValue(item.ExcelAIColIndex.Value)?.ToString();
                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            lock (queryUpdate)
                            {
                                queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                            }
                        }
                    });
                }
            }

            return queryUpdate.ToString();
        }

        /// <summary>
        /// Đọc file bằng lib ExcelReaderFactory
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="operationParameters"></param>
        /// <returns></returns>
        public string ReadFileExcelThongSoToMayH1H2_Old(string filePath, List<OperationParameters> operationParameters)
        {
            var queryUpdate = new StringBuilder();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int rowIndex = 0;
                int rowValueIndex = 0;
                int skipRowIndex = 10;
                int lastRowIndex = 57;

                // Bắt đầu đọc data
                while (reader.Read() && rowIndex < lastRowIndex)
                {
                    if (rowIndex < skipRowIndex || rowIndex % 2 != 0)
                    {
                        rowIndex++;
                        continue;
                    }

                    string time = reader.GetValue(1)?.ToString();
                    if (!string.IsNullOrWhiteSpace(time))
                    {
                        rowValueIndex++;
                    }

                    var ops = operationParameters
                        .Where(x => x.OperationTimeLine.TimeLineName == rowValueIndex)
                        .OrderBy(x => x.ExcelAIColIndex)
                        .ToList();

                    Parallel.ForEach(ops, item =>
                    {
                        var cellValue = reader.GetValue(item.ExcelAIColIndex.Value)?.ToString();
                        if (float.TryParse(cellValue, out float numericValue) && numericValue >= 1 && !item.IsChecked)
                        {
                            lock (queryUpdate)
                            {
                                queryUpdate.Append($"UPDATE dbo.OperationParameters SET CurrentValue = '{cellValue}', ErrorCode = @ErrorCode, IsChecked = @IsChecked, SyncedDate = @CurrentDate, CheckedDate = @CurrentDate, UpdatedAt = @CurrentDate WHERE ID = {item.ID};");
                            }
                        }
                    });

                    rowIndex++;
                }
            }

            return queryUpdate.ToString();
        }

        #endregion
    }

}
