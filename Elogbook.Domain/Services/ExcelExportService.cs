using Elogbook.Core;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models.Request;
using Elogbook.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Triển khai các phương thức xuất dữ liệu
    /// </summary>
    public class ExcelExportService : IExcelExportService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ExcelExportService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Xuất dữ liệu file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportExcelFile(ExportExcelRequestModel requestModel)
        {
            var startMonth = 0;
            var endMonth = 0;
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            string folderName;

            switch (requestModel.DateType)
            {
                // Ngày/Từ ngày - Đến ngày
                case 1:
                    return WriteExcelFile(requestModel);

                // Tháng
                case 2:
                    var month = requestModel.Month.Value.Month;
                    var year = requestModel.Month.Value.Year;
                    var firstDayOfMonth = new DateTime(year, month, 1);
                    var lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                    requestModel.FromDate = firstDayOfMonth.ToString("d/M/yyyy");
                    requestModel.ToDate = lastDayOfMonth.ToString("d/M/yyyy");

                    return WriteExcelFile(requestModel);

                // Quý
                case 3:
                    // Kiểm tra quý để gán tháng bắt đầu/kết thúc của quý
                    switch (requestModel.Quarter)
                    {
                        case 1:
                            startMonth = 1;
                            endMonth = 3;
                            break;

                        case 2:
                            startMonth = 4;
                            endMonth = 6;
                            break;

                        case 3:
                            startMonth = 7;
                            endMonth = 9;
                            break;

                        case 4:
                            startMonth = 10;
                            endMonth = 12;
                            break;
                    }

                    folderName = $"Thông số vận hành_{DateTime.Now:yyyyMMddHHmmss}";
                    // Tạo thư mục chưa file xuất dữ liệu
                    if (!Directory.Exists(Path.Combine(_rootPath, folderName)))
                    {
                        Directory.CreateDirectory(Path.Combine(_rootPath, folderName));
                    }

                    // Duyệt qua các tháng để xuất dữ liệu
                    for (int i = startMonth; i <= endMonth; i++)
                    {
                        ExportFileByMonth(requestModel, i, _rootPath, folderName);
                    }
                    //Parallel.For(startMonth, endMonth + 1, i =>
                    //{
                    //    ExportFileByMonth(requestModel, i, _rootPath, folderName);
                    //});

                    return GetZipFile(_rootPath, folderName);

                // Năm
                case 4:
                    // Xuất dữ liệu từ tháng đầu tiên trong năm
                    startMonth = 1;

                    // Nếu năm xuất dữ liệu bằng năm hiện tại, thì chỉ xuất dữ liệu tới tháng hiện tại
                    // Ngược lại thì xuất dữ liệu 12 tháng của năm
                    if (requestModel.Year == DateTime.Now.Year)
                    {
                        endMonth = DateTime.Now.Month;
                    }
                    else
                    {
                        endMonth = 12;
                    }

                    folderName = $"Thông số vận hành_{DateTime.Now:yyyyMMddHHmmss}";
                    // Tạo thư mục chưa file xuất dữ liệu
                    if (!Directory.Exists(Path.Combine(_rootPath, folderName)))
                    {
                        Directory.CreateDirectory(Path.Combine(_rootPath, folderName));
                    }

                    // Duyệt qua các tháng để xuất dữ liệu
                    for (int i = startMonth; i <= endMonth; i++)
                    {
                        ExportFileByMonth(requestModel, i, _rootPath, folderName);
                    }
                    //Parallel.For(startMonth, endMonth + 1, i =>
                    //{
                    //    ExportFileByMonth(requestModel, i, _rootPath, folderName);
                    //});

                    return GetZipFile(_rootPath, folderName);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private HttpResponseMessage WriteExcelFile(ExportExcelRequestModel requestModel)
        {
            IWorkbook wb = ProcessExportData(requestModel);

            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            var folderName = "Thông số vận hành";
            // Tạo thư mục chưa file xuất dữ liệu
            if (!Directory.Exists(Path.Combine(_rootPath, folderName)))
            {
                Directory.CreateDirectory(Path.Combine(_rootPath, folderName));
            }

            // Định nghĩa tên file xuất dữ liệu
            var fileName = $"{requestModel.FileName}_{DateTime.Now:yyyyMMddHHmmss}.xls";
            string outputFile = Path.Combine(_rootPath, folderName, fileName);

            // Lưu file ra thư mục
            using (FileStream outFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                wb.Write(outFileStream);
            }

            // Trả thông tin file cho người dùng
            var fileBytes = File.ReadAllBytes(outputFile);
            if (fileBytes.Length > 0)
            {
                return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private void WriteExcelFileYear(ExportExcelRequestModel requestModel, int month, string _rootPath, string folderName)
        {
            IWorkbook wb = ProcessExportData(requestModel);

            // Định nghĩa tên file xuất dữ liệu
            var fileName = $"{requestModel.FileName}_Thang {month}_{DateTime.Now:yyyyMMddHHmmss}.xls";
            string outputFile = Path.Combine(_rootPath, folderName, fileName);

            // Lưu file ra thư mục
            using (FileStream outFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                wb.Write(outFileStream);
            }
        }

        private HttpResponseMessage GetZipFile(string _rootPath, string folderName)
        {
            string fullFolderPath = Path.Combine(_rootPath, folderName);
            string fullZipFolderPath = Path.Combine(_rootPath, folderName) + ".zip";

            ZipFile.CreateFromDirectory(fullFolderPath, fullZipFolderPath);

            var fileBytes = File.ReadAllBytes(fullZipFolderPath);
            if (fileBytes.Length > 0)
            {
                if (File.Exists(fullZipFolderPath))
                {
                    File.Delete(fullZipFolderPath);
                }

                if (File.Exists(fullFolderPath))
                {
                    File.Delete(fullFolderPath);
                }

                return Download(new ByteArrayContent(fileBytes), Path.GetFileName(fullZipFolderPath));
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private void ExportFileByMonth(ExportExcelRequestModel requestModel, int month, string _rootPath, string folderName)
        {
            var firstDayOfMonth = new DateTime(requestModel.Year.Value, month, 1);
            var lastDayOfMonth = new DateTime(requestModel.Year.Value, month, DateTime.DaysInMonth(requestModel.Year.Value, month));

            requestModel.FromDate = firstDayOfMonth.ToString("d/M/yyyy");
            requestModel.ToDate = lastDayOfMonth.ToString("d/M/yyyy");

            WriteExcelFileYear(requestModel, month, _rootPath, folderName);
        }

        private IWorkbook ProcessExportData(ExportExcelRequestModel requestModel)
        {
            // Mã chu kỳ 'Chu kỳ 01 giờ (1-2-3-4)'
            requestModel.CycleID = 1;
            var parameterGroupIDs = string.Empty;
            switch (requestModel.TemplateType)
            {
                case 1:
                    parameterGroupIDs = "1;2";
                    break;
                case 2:
                    parameterGroupIDs = "3";
                    break;
                case 3:
                    parameterGroupIDs = "4";
                    break;
                case 4:
                    parameterGroupIDs = "5";
                    break;
            }

            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            List<ExcelDataModel> results;
            using (var context = new ElogbookDataContext())
            {
                var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                    {
                        Value = requestModel.FactoryID,
                    },
                    new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                    {
                        Value = requestModel.OperationPositionID,
                    },
                    new SqlParameter("CycleID", System.Data.SqlDbType.Int)
                    {
                        Value = requestModel.CycleID,
                    },
                    new SqlParameter("ParameterGroupIDs", System.Data.SqlDbType.NVarChar)
                    {
                        Value = parameterGroupIDs,
                    },
                    new SqlParameter("FromDate", System.Data.SqlDbType.DateTime)
                    {
                        Value = frDate,
                    },
                    new SqlParameter("ToDate", System.Data.SqlDbType.DateTime)
                    {
                        Value = toDate,
                    }
                };

                results = context.Database.SqlQuery<ExcelDataModel>("TSVH_GetDataExportExcel @FactoryID, @CycleID, @OperationPositionID, @ParameterGroupIDs, @FromDate, @ToDate", sqlParams).ToList();
            }

            var fromDateNumber = frDate.Day;
            var toDateNumber = toDate.Day;
            var month = frDate.Month;
            var year = frDate.Year;

            var excelTemplateRootPath = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\ExportTemplates\");
            var excelTemplateFullPath = string.Empty;

            if (!string.IsNullOrWhiteSpace(excelTemplateRootPath))
            {
                switch (requestModel.TemplateType)
                {
                    case 1:
                        requestModel.FileName = "Thong So Do Rung - Dao";
                        excelTemplateFullPath = $"{excelTemplateRootPath}{requestModel.FileName}.xls";
                        break;
                    case 2:
                        requestModel.FileName = "Thong So To May H1";
                        excelTemplateFullPath = $"{excelTemplateRootPath}{requestModel.FileName}.xls";
                        break;
                    case 3:
                        requestModel.FileName = "Thong So To May H2";
                        excelTemplateFullPath = $"{excelTemplateRootPath}{requestModel.FileName}.xls";
                        break;
                    case 4:
                        requestModel.FileName = "Thong So Trung Tam";
                        excelTemplateFullPath = $"{excelTemplateRootPath}{requestModel.FileName}.xls";
                        break;
                }
            }

            IWorkbook wb;
            using (var tempateFileStream = new FileStream(excelTemplateFullPath, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(tempateFileStream);

                // Cập nhật tên Sheet đầu tiên nếu chỉ xuất dữ liệu 1 ngày
                if (toDateNumber - fromDateNumber == 0)
                {
                    switch (requestModel.TemplateType)
                    {
                        case 1:
                            WriteDataRDForSheet(wb, 0, fromDateNumber, month, year, results, frDate, requestModel.FactoryID, requestModel.OperationPositionID);
                            break;
                        case 2:
                        case 3:
                            WriteDataTHForSheet(wb, 0, fromDateNumber, month, year, results, frDate, requestModel.FactoryID, requestModel.OperationPositionID);
                            break;
                        case 4:
                            WriteDataTTForSheet(wb, 0, fromDateNumber, month, year, results, frDate, requestModel.FactoryID, requestModel.OperationPositionID);
                            break;
                    }
                }
                else
                {
                    var sheetNameIndex = fromDateNumber;
                    // Nhân bản các sheet theo ngày yêu cầu
                    for (int i = fromDateNumber; i <= toDateNumber - 1; i++)
                    {
                        wb.CloneSheet(0);
                        sheetNameIndex++;
                    }

                    var index = 0;
                    for (int i = frDate.Day; i <= toDate.Day; i++)
                    {
                        switch (requestModel.TemplateType)
                        {
                            case 1:
                                WriteDataRDForSheet(wb, index, i, month, year, results, frDate.AddDays(index), requestModel.FactoryID, requestModel.OperationPositionID);
                                break;
                            case 2:
                            case 3:
                                WriteDataTHForSheet(wb, index, i, month, year, results, frDate.AddDays(index), requestModel.FactoryID, requestModel.OperationPositionID);
                                break;
                            case 4:
                                WriteDataTTForSheet(wb, index, i, month, year, results, frDate.AddDays(index), requestModel.FactoryID, requestModel.OperationPositionID);
                                break;
                        }
                        index++;
                    }
                }
            }

            // Bật các hàm tính toán trong file excel
            XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

            return wb;
        }

        private void WriteDataRDForSheet(IWorkbook wb, int sheetIndex, int fromDateNumber, int month, int year, List<ExcelDataModel> results,
            DateTime createdDate, int factoryID, int operationPositionID)
        {
            // Đổi tên sheet
            wb.SetSheetName(sheetIndex, $"{fromDateNumber:00}");
            // Mapping dữ liệu
            ISheet sheet = wb.GetSheetAt(sheetIndex);

            // Thay đổi thông tin ngày xuất dữ liệu trên sheet
            var headerRowIndex = 1;
            var headerCellDateIndex = 21;
            var cellDate = sheet.GetRow(headerRowIndex).GetCell(headerCellDateIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            if (cellDate != null)
            {
                cellDate.SetCellType(CellType.String);
                cellDate.SetCellValue($"Ngày {fromDateNumber:00} tháng {month:00} năm {year:0000}");
            }

            // Map dữ liệu cho thông số
            var rowIndex = 0;
            for (int i = 6; i <= 29; i++)
            {
                rowIndex++;
                List<double> rowData = results.Where(x => x.TimeLineName == rowIndex && x.InsertedAt.Date == createdDate.Date).OrderBy(y => y.ExcelAIColIndex).Select(e => e.CurrentValue).ToList();

                if (rowData.Count > 0)
                {
                    var h1Index = -1;
                    for (int j = 1; j <= 12; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = sheet.GetRow(i).GetCell(j, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        h1Index++;
                        if (cell != null)
                        {
                            cell.SetCellType(CellType.String);
                            try
                            {
                                if (rowData[h1Index] != 0)
                                {
                                    cell.SetCellValue(rowData[h1Index]);
                                }
                            }
                            catch
                            {
                                //ignore error
                            }
                        }
                    }

                    var h2Index = 11;
                    for (int t = 15; t <= 26; t++)
                    {
                        h2Index++;
                        NPOI.SS.UserModel.ICell cell = sheet.GetRow(i).GetCell(t, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        if (cell != null)
                        {
                            cell.SetCellType(CellType.String);
                            try
                            {
                                if (rowData[h2Index] != 0)
                                {
                                    cell.SetCellValue(rowData[h2Index]);
                                }
                            }
                            catch
                            {
                                //ignore error
                            }
                        }
                    }
                }
            }

            // Map thông tin chữ ký của trưởng ca vận hành
            var userInfoList = GetSignatureLeader(createdDate, factoryID, operationPositionID);
            // Ca ngày
            SetSignature(userInfoList, sheet, 36, 8, 7, true);
            // Ca đêm
            SetSignature(userInfoList, sheet, 36, 16, 15, false);
        }

        private void WriteDataTHForSheet(IWorkbook wb, int sheetIndex, int fromDateNumber, int month, int year, List<ExcelDataModel> results,
            DateTime createdDate, int factoryID, int operationPositionID)
        {
            // Đổi tên sheet
            wb.SetSheetName(sheetIndex, $"{fromDateNumber:00}");
            // Mapping dữ liệu
            ISheet sheet = wb.GetSheetAt(sheetIndex);

            // Thay đổi thông tin ngày xuất dữ liệu trên sheet
            var headerRowIndex = 1;
            var headerCellDateIndex = 43;
            var cellDate = sheet.GetRow(headerRowIndex).GetCell(headerCellDateIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            if (cellDate != null)
            {
                cellDate.SetCellType(CellType.String);
                cellDate.SetCellValue($"Ngày {fromDateNumber:00} tháng {month:00} năm {year:0000}");
                cellDate.CellStyle.Alignment = HorizontalAlignment.Center;
                cellDate.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            }

            // Map dữ liệu cho thông số
            var rowIndex = 0;
            for (int i = 6; i <= 29; i++)
            {
                rowIndex++;
                List<double> rowData = results.Where(x => x.TimeLineName == rowIndex && x.InsertedAt.Date == createdDate.Date).OrderBy(y => y.ExcelAIColIndex).Select(e => e.CurrentValue).ToList();

                if (rowData.Count > 0)
                {
                    var h1Index = -1;
                    for (int j = 1; j <= 51; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = sheet.GetRow(i).GetCell(j, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        h1Index++;
                        if (cell != null)
                        {
                            cell.SetCellType(CellType.String);
                            try
                            {
                                if (rowData[h1Index] != 0)
                                {
                                    cell.SetCellValue(rowData[h1Index]);
                                }
                            }
                            catch
                            {
                                //ignore error
                            }
                        }
                    }
                }
            }

            // Map thông tin chữ ký của trưởng ca vận hành
            var userInfoList = GetSignatureLeader(createdDate, factoryID, operationPositionID);
            // Ca ngày
            SetSignature(userInfoList, sheet, 35, 12, 10, true);
            // Ca đêm
            SetSignature(userInfoList, sheet, 35, 24, 22, false);
        }

        private void WriteDataTTForSheet(IWorkbook wb, int sheetIndex, int fromDateNumber, int month, int year, List<ExcelDataModel> results,
            DateTime createdDate, int factoryID, int operationPositionID)
        {
            // Đổi tên sheet
            wb.SetSheetName(sheetIndex, $"{fromDateNumber:00}");
            // Mapping dữ liệu
            ISheet sheet = wb.GetSheetAt(sheetIndex);

            // Thay đổi thông tin ngày xuất dữ liệu trên sheet
            var headerRowIndex = 1;
            var headerCellDateIndex = 25;
            var cellDate = sheet.GetRow(headerRowIndex).GetCell(headerCellDateIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            if (cellDate != null)
            {
                cellDate.SetCellType(CellType.String);
                cellDate.SetCellValue($"Ngày {fromDateNumber:00} tháng {month:00} năm {year:0000}");
                cellDate.CellStyle.Alignment = HorizontalAlignment.Center;
                cellDate.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            }

            // Map dữ liệu cho thông số
            var rowIndex = 0;
            for (int i = 7; i <= 30; i++)
            {
                rowIndex++;
                List<double> rowData = results.Where(x => x.TimeLineName == rowIndex && x.InsertedAt.Date == createdDate.Date).OrderBy(y => y.ExcelAIColIndex).Select(e => e.CurrentValue).ToList();

                if (rowData.Count > 0)
                {
                    var h1Index = -1;
                    for (int j = 1; j <= 32; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = sheet.GetRow(i).GetCell(j, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        h1Index++;
                        if (cell != null)
                        {
                            cell.SetCellType(CellType.String);
                            try
                            {
                                if (rowData[h1Index] != 0)
                                {
                                    cell.SetCellValue(rowData[h1Index]);
                                }
                            }
                            catch
                            {
                                //ignore error
                            }
                        }
                    }
                }
            }

            // Map thông tin chữ ký của trưởng ca vận hành
            var userInfoList = GetSignatureLeader(createdDate, factoryID, operationPositionID);
            // Ca ngày
            SetSignature(userInfoList, sheet, 35, 10, 9, true);
            // Ca đêm
            SetSignature(userInfoList, sheet, 35, 18, 17, false);
        }

        private void SetSignature(List<UserInfoExportModel> userInfoList, ISheet sheet, int rowIndex, int cellImage, int cellName, bool isFirst)
        {
            if (userInfoList != null)
            {
                var user = isFirst ? userInfoList.Find(x => x.ShiftType) : userInfoList.Find(x => !x.ShiftType);
                if (user != null)
                {
                    //InsertImage(sheet, user.Signature, rowIndex, cellImage);

                    NPOI.SS.UserModel.ICell cell = sheet.GetRow(rowIndex).GetCell(cellName, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    if (cell != null)
                    {
                        cell.SetCellValue(user.FullName);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                }
            }
        }

        private List<UserInfoExportModel> GetSignatureLeader(DateTime createdDate, int factoryID, int operationPositionID)
        {
            List<UserInfoExportModel> results;
            using (var context = new ElogbookDataContext())
            {
                var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                    {
                        Value = factoryID,
                    },
                    new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                    {
                        Value = operationPositionID,
                    },
                    new SqlParameter("OperationShiftID", System.Data.SqlDbType.NVarChar)
                    {
                        Value = 0,
                    },
                    new SqlParameter("CreatedDate", System.Data.SqlDbType.DateTime)
                    {
                        Value = createdDate,
                    },
                };

                results = context.Database.SqlQuery<UserInfoExportModel>("TSVH_GetUserInfoExport @FactoryID, @OperationPositionID, @OperationShiftID, @CreatedDate", sqlParams).ToList();
            }

            return results;
        }

        private void InsertImage(ISheet sheet, string signature, int rowIndex, int cellIndex)
        {
            HSSFWorkbook xssfWorkbook = (HSSFWorkbook)sheet.Workbook;

            signature = signature.Remove(0, signature.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(signature);
            iTextSharp.text.Image imageOld = iTextSharp.text.Image.GetInstance(data);

            float width = imageOld.Width;
            float height = imageOld.Height;
            using (MemoryStream memoryStream = new MemoryStream(data, 0, data.Length))
            {
                using (Image image = Image.FromStream(memoryStream))
                {
                    // Resize height
                    int h = 0;
                    int reductWith = 0;
                    if (height < 90)
                    {
                        h = (int)height;
                    }
                    else
                    {
                        h = 90;
                        reductWith = (int)height - 90;
                    }

                    // Resize width
                    int w = 0;
                    if (width < 120)
                    {
                        w = (int)width;
                    }
                    else
                    {
                        w = (int)width - reductWith;
                    }

                    using (Bitmap bitmap = new Bitmap(image, new Size(w, h)))
                    {
                        using (MemoryStream memoryStreamOut = new MemoryStream())
                        {
                            bitmap.Save(memoryStreamOut, System.Drawing.Imaging.ImageFormat.Png);
                            data = memoryStreamOut.ToArray();
                        }
                    }
                }
            }

            int pictureIndex = xssfWorkbook.AddPicture(data, NPOI.SS.UserModel.PictureType.PNG);
            ICreationHelper helper = xssfWorkbook.GetCreationHelper();
            NPOI.SS.UserModel.IDrawing drawing = sheet.CreateDrawingPatriarch();
            IClientAnchor anchor = helper.CreateClientAnchor();
            anchor.Col1 = cellIndex;
            anchor.Row1 = rowIndex;
            anchor.Dx1 = 1000;
            anchor.Dx2 = 1000;
            anchor.Dy1 = 10;
            anchor.Dy2 = 10;
            anchor.AnchorType = AnchorType.MoveDontResize;
            NPOI.SS.UserModel.IPicture picture = drawing.CreatePicture(anchor, pictureIndex);
            picture.Resize();
        }

        /// <summary>
        /// Tải file
        /// </summary>
        protected HttpResponseMessage Download(HttpContent content, string fileName = "")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return response;
        }
    }

    /// <summary>
    /// Dữ liệu thông số vận hành
    /// </summary>
    public class ExcelDataModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Column mapping
        /// </summary>
        public int ExcelAIColIndex { get; set; }

        /// <summary>
        /// Mã thông số
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Giá trị thông số
        /// </summary>
        public double CurrentValue { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int TimeLineName { get; set; }

        /// <summary>
        /// Ngày ghi dữ liệu thông số
        /// </summary>
        public DateTime InsertedAt { get; set; }
    }

    /// <summary>
    /// Thông tin trưởng ca nhật ký vận hành
    /// </summary>
    public class UserInfoExportModel
    {
        /// <summary>
        /// Mã trưởng ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Họ và Tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Ca ngày/đêm
        /// </summary>
        public bool ShiftType { get; set; }
    }
}
