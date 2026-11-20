using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Widget;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Report;
using Elogbook.Data;
using Elogbook.Data.DataModels.Entities.Report;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Elogbook.Domain.Services
{
    public class ReportService : IReportService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IWidgetService _widgetService;

        /// <summary>
        /// Folder Report AM
        /// </summary>
        private const string FOLDER_REPORT = "ReportNkvnTsvh";

        /// <summary>
        /// Thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        private const string FILE_NAME_01 = "Thong ke trang thai ca van hanh theo cuong vi";

        /// <summary>
        /// Thống kê tồn tại ca theo tình trạng xử lý của các nhà máy
        /// </summary>
        private const string FILE_NAME_02 = "Thong ke ton tai ca theo tinh trang xu ly cua cac nha may";

        /// <summary>
        /// Bảng thống kê số lượng tồn tại ca theo tháng
        /// </summary>
        private const string FILE_NAME_03 = "Bang thong ke so luong ton tai ca theo thang";

        /// <summary>
        /// Báo cáo thông số có giá trị vượt ngưỡng
        /// </summary>
        private const string FILE_NAME_04 = "Bao cao thong so co gia tri vuot nguong";

        /// <summary>
        /// Báo cáo thống kê chi tiết lỗi thiết bị
        /// </summary>
        private const string FILE_NAME_05 = "Bao cao thong ke chi tiet loi thiet bi";

        /// <summary>
        /// Bảng thống kê số lượng báo bận công tác và dừng máy 
        /// </summary>
        private const string FILE_NAME_06 = "Bang thong ke so luong bao ban cong tac va dung may";

        public ReportService(ElogbookUnitOfWork unitOfWork, IWidgetService widgetService)
        {
            _unitOfWork = unitOfWork;
            _widgetService = widgetService;
        }

        #region Thống kê trạng thái ca vận hành theo cương vị
        /// <summary>
        /// Thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetPositionShiftStatusStatistics(FilterPositionShiftStatusModel requestModel)
        {
            var results = GetPositionShiftStatusStatistics_Data(requestModel);
            return Core.Models.HttpResponse.OK(results);
        }

        /// <summary>
        /// Xuất báo cáo thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportPositionShiftStatus(FilterPositionShiftStatusModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetPositionShiftStatusStatistics_Data(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, FOLDER_REPORT)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, FOLDER_REPORT));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, FOLDER_REPORT, $"{FILE_NAME_01}_{currentDate}.xlsx");

                bool isCreatexlsx = GeneratePositionShiftStatusFile(rootData, outputFile, requestModel);

                if (isCreatexlsx)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: ExportPositionShiftStatus", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Xuất file thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        /// <param name="rootData"></param>
        /// <param name="outputFile"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool GeneratePositionShiftStatusFile(List<PositionShiftStatusModel> rootData, string outputFile, FilterPositionShiftStatusModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 14, title1, "THỐNG KÊ TRẠNG THÁI CA VẬN HÀNH THEO CƯƠNG VỊ");
                SetBoldweight(sheet, indexRow, 4, true, false, 14);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 6, 12, title2, $"Dữ liệu từ ngày {requestModel.From:dd/MM/yyyy} - {requestModel.To:dd/MM/yyyy}");
                SetBoldweight(sheet, indexRow, 6, true, true, 12);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 5, headerRow1, "Nhà máy");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 6, 10, headerRow1, "Cương vị");
                CreateCellTypeStr(sheet, indexRow, indexRow, 11, 16, headerRow1, "Trạng thái");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 17, 17, headerRow1, "Tổng");

                indexRow += 1;//Excel Row Index = 7
                IRow headerRow2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, headerRow2, "Đã giao ca");
                CreateCellTypeStr(sheet, indexRow, indexRow, 14, 16, headerRow2, "Chưa giao/nhận ca");
                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 17), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;

                int countIndex = 0;

                if (rootData.Count > 0)
                {
                    foreach (var item in rootData)
                    {
                        indexRow += 1;
                        countIndex++;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        if (countIndex == rootData.Count)
                        {
                            CreateCellTypeStr(sheet, indexRow, indexRow, 1, 10, bodyRow, item.Factory.Label);
                        }
                        else
                        {
                            CreateCellTypeStr(sheet, indexRow, indexRow, 1, 5, bodyRow, item.Factory.Label);
                            CreateCellTypeStr(sheet, indexRow, indexRow, 6, 10, bodyRow, item.OperationPosition.Label);
                        }
                        CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, bodyRow, item.TotalDelivered.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 14, 16, bodyRow, item.TotalReceived.ToString());
                        bodyRow.CreateCell(17).SetCellValue(item.Total.ToString());
                    }
                    int edRowBdIndex = indexRow;
                    int edColBdIndex = 17;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 1, 10), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Left);
                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 11, 17), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Center);

                    SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 17, bodyRow, "Không có dữ liệu");
                    SetAlignmentCennter(sheet, indexRow, 1);
                    SetStyleFinish(sheet, 1, 3, 7, 26, 61, 58);
                }

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: GeneratePositionShiftStatusFile", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Lấy dữ liệu thống kê trạng thái ca vận hành theo cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private List<PositionShiftStatusModel> GetPositionShiftStatusStatistics_Data(FilterPositionShiftStatusModel requestModel)
        {
            // Dữ liệu Nhật ký vận hành đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }

            // Tìm kiếm Nhật ký vận hành theo ngày tạo
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
                requestModel.From = frDate;
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= frDate.Date);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
                requestModel.To = toDate;
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <= toDate.Date);
            }

            var datas = entities
                .Select(e => new OperationDiaryModel
                {
                    CreateDate = e.CreatedDate,
                    FactoryID = e.FactoryID,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPositionID = e.OperationPositionID,
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    IsReceived = e.IsReceived,
                    IsDelivered = e.IsDelivered
                })
                .OrderBy(e => e.FactoryID)
                .ThenBy(e => e.OperationPositionID)
                .ToList();

            if (string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                requestModel.From = datas.Min(x => x.CreateDate);
            }

            if (string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                requestModel.To = datas.Max(x => x.CreateDate);
            }

            List<PositionShiftStatusModel> results = new List<PositionShiftStatusModel>();
            int t1 = 0;
            int t2 = 0;

            // Danh sách nhà máy có trong nhật ký vận hành
            var factoryIds = datas.Select(x => x.FactoryID).Distinct().ToList();

            foreach (var factoryId in factoryIds)
            {
                var factoryLabel = datas.FirstOrDefault(x => x.FactoryID == factoryId)?.Factory?.Label ?? string.Empty;

                // Danh sách cương vị có trong nhật ký vận hành cho từng nhà máy
                var positionIds = datas.Where(x => x.FactoryID == factoryId).Select(x => x.OperationPositionID).Distinct().ToList();

                foreach (var positionId in positionIds)
                {
                    var positionLabel = datas.FirstOrDefault(x => x.OperationPositionID == positionId && x.FactoryID == factoryId)?.OperationPosition?.Label ?? string.Empty;

                    var statistic = new PositionShiftStatusModel
                    {
                        // Include factory in the label so report shows grouping by Factory + Position
                        Factory = new ValueLabelModel { Value = factoryId, Label = factoryLabel },
                        OperationPosition = new ValueLabelModel { Value = positionId, Label = positionLabel },
                        TotalDelivered = datas.Count(x => x.FactoryID == factoryId && x.OperationPositionID == positionId && x.IsDelivered && x.IsReceived),
                        TotalReceived = datas.Count(x => x.FactoryID == factoryId && x.OperationPositionID == positionId && !(x.IsDelivered && x.IsReceived))
                    };

                    statistic.Total = statistic.TotalDelivered + statistic.TotalReceived;

                    t1 += statistic.TotalDelivered;
                    t2 += statistic.TotalReceived;

                    results.Add(statistic);
                }
            }
            

            results.Add(new PositionShiftStatusModel
            {
                Factory = new ValueLabelModel { Value = 0, Label = "Tổng" },
                OperationPosition = new ValueLabelModel { Value = 0, Label = "Tổng" },
                TotalDelivered = t1,
                TotalReceived = t2,
                Total = t1 + t2
            });

            return results;
        }

        #endregion

        #region Thống kê trạng thái ca vận hành theo nhà máy
        /// <summary>
        /// Thống kê trạng thái ca vận hành theo nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetPlantShiftStatusStatistics(FilterPlantShiftStatusModel requestModel)
        {
            var results = GetPlantShiftStatusStatistics_Data(requestModel);

            return Core.Models.HttpResponse.OK(results);
        }

        public HttpResponseMessage ExportPlantShiftStatus(FilterPlantShiftStatusModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetPlantShiftStatusStatistics_Data(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, FOLDER_REPORT)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, FOLDER_REPORT));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, FOLDER_REPORT, $"{FILE_NAME_02}_{currentDate}.xlsx");

                bool isCreatexlsx = GeneratePlantShiftStatusFile(rootData, outputFile, requestModel);

                if (isCreatexlsx)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: ExportPlantShiftStatus", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        private List<PlantShiftStatusModel> GetPlantShiftStatusStatistics_Data(FilterPlantShiftStatusModel requestModel)
        {
            var tempDatas = _unitOfWork.ElogbookRepository<ExistInShift>()
                .GetQuery(x => !x.IsDeleted && !x.IsDeleteIssueOfShift);

            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                tempDatas = tempDatas.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Ngày vận hành
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
                tempDatas = tempDatas.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date);
                requestModel.From = frDate;
            }

            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
                tempDatas = tempDatas.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date);
                requestModel.To = toDate;
            }

            var datas = tempDatas.OrderBy(x => x.FactoryID).ToList();

            if (string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                requestModel.From = datas.Min(x => x.InsertedAt);
            }

            if (string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                requestModel.To = datas.Max(x => x.InsertedAt);
            }

            List<PlantShiftStatusModel> results = new List<PlantShiftStatusModel>();
            int t1 = 0;
            int t2 = 0;
            int t3 = 0;
            if (datas.Count > 0)
            {
                // Thống kê tồn tại ca theo tình trạng xử lý của các nhà máy
                var factoryIds = datas.Select(x => x.FactoryID).Distinct().ToList();

                foreach (var factoryId in factoryIds)
                {
                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted && x.ID == factoryId).FirstOrDefault();
                    var statistic = new PlantShiftStatusModel
                    {
                        Factory = factory != null ? new ValueLabelModel { Value = factory.ID, Label = factory.FactoryName } : null,
                        TotalPendingCount = datas.Count(x => x.FactoryID == factoryId && x.StatusID == (int)IssueOfShiftStatus.NOT_PROCESS),
                        TotalProcessingCount = datas.Count(x => x.FactoryID == factoryId && x.StatusID == (int)IssueOfShiftStatus.IN_PROGRESS),
                        TotalProcessedCount = datas.Count(x => x.FactoryID == factoryId && x.StatusID == (int)IssueOfShiftStatus.DONE)
                    };

                    statistic.Total = statistic.TotalPendingCount + statistic.TotalProcessingCount + statistic.TotalProcessedCount;
                    t1 += statistic.TotalPendingCount;
                    t2 += statistic.TotalProcessingCount;
                    t3 += statistic.TotalProcessingCount;

                    results.Add(statistic);
                }
            }
            results.Add(new PlantShiftStatusModel
            {
                Factory = new ValueLabelModel { Value = 0, Label = "Tổng" },
                TotalPendingCount = t1,
                TotalProcessingCount = t2,
                TotalProcessedCount = t3,
                Total = t1 + t2 + t3
            });

            return results;
        }

        private bool GeneratePlantShiftStatusFile(List<PlantShiftStatusModel> rootData, string outputFile, FilterPlantShiftStatusModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 12, title1, "THỐNG KÊ TỒN TẠI CA THEO TÌNH TRẠNG XỬ LÝ CỦA CÁC NHÀ MÁY");
                SetBoldweight(sheet, indexRow, 2, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 4, 9, title2, $"Dữ liệu từ ngày {requestModel.From:dd/MM/yyyy} - {requestModel.To:dd/MM/yyyy}");
                SetBoldweight(sheet, indexRow, 4, true, true, 11);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 5, headerRow1, "Nhà máy");
                CreateCellTypeStr(sheet, indexRow, indexRow, 6, 11, headerRow1, "Tình trạng xử lý");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 12, 12, headerRow1, "Tổng");

                indexRow += 1;//Excel Row Index = 7
                IRow headerRow2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 6, 7, headerRow2, "Chưa xử lý");
                CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, headerRow2, "Đang xử lý");
                CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, headerRow2, "Đã xử lý");
                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 12), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;

                if (rootData.Count > 0)
                {
                    foreach (var item in rootData)
                    {
                        indexRow += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        CreateCellTypeStr(sheet, indexRow, indexRow, 1, 5, bodyRow, item.Factory.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 6, 7, bodyRow, item.TotalPendingCount.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, bodyRow, item.TotalProcessingCount.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, bodyRow, item.TotalProcessedCount.ToString());
                        bodyRow.CreateCell(12).SetCellValue(item.Total.ToString());
                    }
                    int edRowBdIndex = indexRow;
                    int edColBdIndex = 12;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 1, 5), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Left);
                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 6, 12), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Center);

                    SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 12, bodyRow, "Không có dữ liệu");
                    SetAlignmentCennter(sheet, indexRow, 1);
                    SetStyleFinish(sheet, 1, 3, 7, 26, 61, 58);
                }

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: GeneratePositionShiftStatusFile", ex.ToString());
                return false;
            }
        }

        #endregion

        #region Thống kê số lượng ca vận hành theo tháng
        /// <summary>
        /// Thống kê số lượng ca vận hành theo tháng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetMonthlyShiftCountStatistics(FilterMonthlyShiftCountModel requestModel)
        {
            var results = GetMonthlyShiftCountStatistics_Data(requestModel);
            return Core.Models.HttpResponse.OK(results);
        }

        public HttpResponseMessage ExportMonthlyShiftCount(FilterMonthlyShiftCountModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetMonthlyShiftCountStatistics_Data(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, FOLDER_REPORT)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, FOLDER_REPORT));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, FOLDER_REPORT, $"{FILE_NAME_03}_{currentDate}.xlsx");

                bool isCreatexlsx = GenerateMonthlyShiftCountFile(rootData, outputFile, requestModel);

                if (isCreatexlsx)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: ExportPositionShiftStatus", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        private List<MonthlyShiftCountModel> GetMonthlyShiftCountStatistics_Data(FilterMonthlyShiftCountModel requestModel)
        {
            var months = new List<int>();
            var tempDatas = _unitOfWork.ElogbookRepository<ExistInShift>()
                .GetQuery(x => !x.IsDeleted && !x.IsDeleteIssueOfShift);

            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                tempDatas = tempDatas.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // năm
            if (requestModel.Year > 1900)
            {
                tempDatas = tempDatas.Where(x => x.InsertedAt.Year == requestModel.Year);
            }
            else
            {
                tempDatas = tempDatas.Where(x => x.InsertedAt.Year == DateTime.Now.Year);
                requestModel.Year = DateTime.Now.Year;
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();
                months = mons;
                tempDatas = tempDatas.Where(x => mons.Contains(x.InsertedAt.Month));
            }
            else
            {
                months = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            }

            var datas = tempDatas.OrderBy(x => x.FactoryID).ToList();

            List<MonthlyShiftCountModel> results = new List<MonthlyShiftCountModel>();

            if (datas.Count > 0)
            {
                // Thống kê tồn tại ca theo tình trạng xử lý của các nhà máy
                var factoryIds = datas.Select(x => x.FactoryID).Distinct().ToList();
                int total = 0;
                foreach (var factoryId in factoryIds)
                {
                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted && x.ID == factoryId).FirstOrDefault();
                    var statistic = new MonthlyShiftCountModel()
                    {
                        Factory = new ValueLabelModel { Value = factory.ID, Label = factory.FactoryName },
                        MonthlyShiftCountDetails = new List<MonthlyShiftCountDetailsModel>()
                    };
                    int t1 = 0;
                    for (int i = 1; i <= 12; i++)
                    {
                        var detail = new MonthlyShiftCountDetailsModel()
                        {
                            Month = i,
                            TotalReceived = datas.Count(x => x.FactoryID == factoryId && x.InsertedAt.Month == i)
                        };
                        if (months.Contains(i)) detail.IsActive = true;
                        statistic.MonthlyShiftCountDetails.Add(detail);

                        t1 += detail.TotalReceived;
                    }

                    statistic.MonthlyShiftCountDetails.Add(new MonthlyShiftCountDetailsModel
                    {
                        Month = 13,
                        IsActive = true,
                        TotalReceived = t1
                    });

                    results.Add(statistic);

                    total += t1;
                }

                var totalEntity = new List<MonthlyShiftCountModel>();
                var totalStatistic = new MonthlyShiftCountModel()
                {
                    Factory = new ValueLabelModel { Value = 0, Label = "Tổng" },
                    MonthlyShiftCountDetails = new List<MonthlyShiftCountDetailsModel>()
                };

                for (int i = 1; i <= 13; i++)
                {
                    totalStatistic.MonthlyShiftCountDetails.Add(new MonthlyShiftCountDetailsModel
                    {
                        Month = i,
                        IsActive = results.Where(x => x.MonthlyShiftCountDetails.Any(y => y.Month == i))
                        .Select(x => x.MonthlyShiftCountDetails.First(y => y.Month == i).IsActive).FirstOrDefault(),
                        TotalReceived = results
                        .Where(x => x.MonthlyShiftCountDetails.Any(y => y.Month == i))
                        .Sum(x => x.MonthlyShiftCountDetails
                        .FirstOrDefault(y => y.Month == i).TotalReceived)
                    });
                }
                results.Add(totalStatistic);
            }

            return results;
        }

        private bool GenerateMonthlyShiftCountFile(List<MonthlyShiftCountModel> rootData, string outputFile, FilterMonthlyShiftCountModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                string titleMonth = string.Empty;
                if (string.IsNullOrWhiteSpace(requestModel.Months))
                {
                    titleMonth = $"Dữ liệu từ tháng 01/{requestModel.Year} ~ 12/{requestModel.Year}";
                }
                else
                {
                    titleMonth = $"Dữ liệu các tháng {requestModel.Months}/{requestModel.Year}";
                }
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 5, 14, title1, "BẢNG THỐNG KÊ SỐ LƯỢNG TỒN TẠI CA THEO THÁNG");
                SetBoldweight(sheet, indexRow, 5, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 7, 12, title2, titleMonth);
                SetBoldweight(sheet, indexRow, 7, true, true, 11);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 5, headerRow1, "Nhà máy");
                CreateCellTypeStr(sheet, indexRow, indexRow, 6, 17, headerRow1, "Số tồn tại ca");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 18, 18, headerRow1, "Tổng");

                indexRow += 1;//Excel Row Index = 7
                IRow headerRow2 = sheet.CreateRow(indexRow);
                headerRow2.CreateCell(6).SetCellValue("Tháng 1");
                headerRow2.CreateCell(7).SetCellValue("Tháng 2");
                headerRow2.CreateCell(8).SetCellValue("Tháng 3");
                headerRow2.CreateCell(9).SetCellValue("Tháng 4");
                headerRow2.CreateCell(10).SetCellValue("Tháng 5");
                headerRow2.CreateCell(11).SetCellValue("Tháng 6");
                headerRow2.CreateCell(12).SetCellValue("Tháng 7");
                headerRow2.CreateCell(13).SetCellValue("Tháng 8");
                headerRow2.CreateCell(14).SetCellValue("Tháng 9");
                headerRow2.CreateCell(15).SetCellValue("Tháng 10");
                headerRow2.CreateCell(16).SetCellValue("Tháng 11");
                headerRow2.CreateCell(17).SetCellValue("Tháng 12");

                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 18), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;

                if (rootData.Count > 0)
                {
                    foreach (var item in rootData)
                    {
                        indexRow += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        CreateCellTypeStr(sheet, indexRow, indexRow, 1, 5, bodyRow, item.Factory.Label);
                        for (int i = 0; i < item.MonthlyShiftCountDetails.Count; i++)
                        {
                            if (item.MonthlyShiftCountDetails[i].IsActive)
                            {
                                bodyRow.CreateCell(6 + i).SetCellValue(item.MonthlyShiftCountDetails[i].TotalReceived.ToString());
                            }
                            else
                            {
                                bodyRow.CreateCell(6 + i).SetCellValue(string.Empty);
                            }
                        }
                    }
                    int edRowBdIndex = indexRow;
                    int edColBdIndex = 18;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 1, 5), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Left);
                    if (string.IsNullOrWhiteSpace(requestModel.Months))
                    {
                        SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 6, 18), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Center);
                    }
                    else
                    {
                        for (int i = 0; i <= 12; i++)
                        {
                            var item = rootData[0].MonthlyShiftCountDetails[i];
                            if (item.IsActive)
                            {
                                SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 6 + i, 6 + i), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Center);
                            }
                            else
                            {
                                SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 6 + i, 6 + i), new XSSFColor(new byte[] { 179, 177, 175 }), BorderStyle.Thin, HorizontalAlignment.Center);
                            }
                        }
                    }
                    SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 18, bodyRow, "Không có dữ liệu");
                    SetAlignmentCennter(sheet, indexRow, 1);
                    SetStyleFinish(sheet, 1, 3, 7, 26, 61, 58);
                }

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: GeneratePositionShiftStatusFile", ex.ToString());
                return false;
            }
        }
        #endregion

        #region Báo cáo thông số có giá trị vượt ngưỡng
        /// <summary>
        /// Xuất file Excel thông số vượt ngưỡng
        /// </summary>
        public HttpResponseMessage ExportThresholdExceededParameters(FilterThresholdExceededParametersModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetThresholdExceededParameters_Data(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(Path.Combine(_rootPath, FOLDER_REPORT)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, FOLDER_REPORT));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, FOLDER_REPORT, $"{FILE_NAME_04}_{currentDate}.xlsx");

                bool isCreated = GenerateThresholdExceededParametersFile(rootData, outputFile, requestModel);

                if (isCreated)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: ExportThresholdExceededParameters", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Tạo file Excel báo cáo thông số vượt ngưỡng
        /// </summary>
        private bool GenerateThresholdExceededParametersFile(
            List<ThresholdExceededParametersModel> rootData,
            string outputFile,
            FilterThresholdExceededParametersModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // ── Tiêu đề chính ──────────────────────────────────────────────
                indexRow += 1; // row 1
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 9, title1,
                    "BÁO CÁO THÔNG SỐ CÓ GIÁ TRỊ VƯỢT NGƯỠNG");
                SetBoldweight(sheet, indexRow, 1, true, false, 16);

                // ── Dòng khoảng thời gian ────────────────────────────────────
                indexRow += 2; // row 3
                IRow title2 = sheet.CreateRow(indexRow);
                string fromLabel = requestModel.From.HasValue
                    ? requestModel.From.Value.ToString("dd/MM/yyyy")
                    : "---";
                string toLabel = requestModel.To.HasValue
                    ? requestModel.To.Value.ToString("dd/MM/yyyy")
                    : "---";
                CreateCellTypeStr(sheet, indexRow, indexRow, 3, 7, title2,
                    $"Dữ liệu từ ngày {fromLabel} - {toLabel}");
                SetBoldweight(sheet, indexRow, 3, true, true, 11);

                // ── Header bảng ───────────────────────────────────────────────
                indexRow += 2; // row 5
                IRow headerRow = sheet.CreateRow(indexRow);
                headerRow.CreateCell(1).SetCellValue("STT");
                CreateCellTypeStr(sheet, indexRow, indexRow, 2, 3, headerRow, "Nhà máy");
                headerRow.CreateCell(4).SetCellValue("Ngày ghi thông số");
                CreateCellTypeStr(sheet, indexRow, indexRow, 5, 6, headerRow, "Nhóm thông số");
                CreateCellTypeStr(sheet, indexRow, indexRow, 7, 8, headerRow, "Thông số");
                headerRow.CreateCell(9).SetCellValue("Mã KKS");
                headerRow.CreateCell(10).SetCellValue("Range");
                headerRow.CreateCell(11).SetCellValue("Giá trị vượt ngưỡng");

                SetStyleTableHeader(sheet,
                    new CellRangeAddress(indexRow, indexRow, 1, 11),
                    new XSSFColor(new byte[] { 169, 208, 142 }),
                    BorderStyle.Thin);

                int stRowBdIndex = indexRow;

                // ── Dữ liệu ──────────────────────────────────────────────────
                if (rootData.Count > 0)
                {
                    foreach (var item in rootData)
                    {
                        indexRow += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(item.RowIndex.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 3, bodyRow,
                            item.Factory?.Label ?? string.Empty);
                        bodyRow.CreateCell(4).SetCellValue(
                            item.CheckedDate.HasValue
                                ? item.CheckedDate.Value.ToString("dd/MM/yyyy")
                                : string.Empty);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 5, 6, bodyRow,
                            item.ParameterGroupName ?? string.Empty);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 7, 8, bodyRow,
                            item.ParameterName ?? string.Empty);
                        bodyRow.CreateCell(9).SetCellValue(item.KKSCode ?? string.Empty);
                        bodyRow.CreateCell(10).SetCellValue(item.Range ?? string.Empty);
                        bodyRow.CreateCell(11).SetCellValue(item.ExceededValue ?? string.Empty);
                    }

                    int edRowBdIndex = indexRow;

                    // Cột STT + Ngày + KKS + Range + Giá trị — căn giữa
                    SetStyleTableBody(sheet,
                        new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 1, 1),
                        new XSSFColor(new byte[] { 221, 235, 247 }),
                        BorderStyle.Thin, HorizontalAlignment.Center);
                    SetStyleTableBody(sheet,
                        new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 4, 4),
                        new XSSFColor(new byte[] { 221, 235, 247 }),
                        BorderStyle.Thin, HorizontalAlignment.Center);
                    SetStyleTableBody(sheet,
                        new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 9, 11),
                        new XSSFColor(new byte[] { 221, 235, 247 }),
                        BorderStyle.Thin, HorizontalAlignment.Center);

                    // Cột text — căn trái
                    SetStyleTableBody(sheet,
                        new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 2, 3),
                        new XSSFColor(new byte[] { 221, 235, 247 }),
                        BorderStyle.Thin, HorizontalAlignment.Left);
                    SetStyleTableBody(sheet,
                        new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 5, 8),
                        new XSSFColor(new byte[] { 221, 235, 247 }),
                        BorderStyle.Thin, HorizontalAlignment.Left);

                    SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, 1, 11, edRowBdIndex + 50, 60);
                }
                else
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 11, bodyRow, "Không có dữ liệu");
                    SetAlignmentCennter(sheet, indexRow, 1);
                    SetStyleFinish(sheet, 1, 3, 1, 11, 61, 60);
                }

                // ── Độ rộng cột ──────────────────────────────────────────────
                sheet.SetColumnWidth(1, 8 * 256);    // STT
                sheet.SetColumnWidth(2, 18 * 256);   // Nhà máy (col 2-3)
                sheet.SetColumnWidth(3, 18 * 256);
                sheet.SetColumnWidth(4, 18 * 256);   // Ngày ghi
                sheet.SetColumnWidth(5, 22 * 256);   // Nhóm thông số (col 5-6)
                sheet.SetColumnWidth(6, 22 * 256);
                sheet.SetColumnWidth(7, 28 * 256);   // Thông số (col 7-8)
                sheet.SetColumnWidth(8, 28 * 256);
                sheet.SetColumnWidth(9, 18 * 256);   // KKS
                sheet.SetColumnWidth(10, 14 * 256);  // Range
                sheet.SetColumnWidth(11, 18 * 256);  // Giá trị vượt ngưỡng

                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR,
                    "ReportService: GenerateThresholdExceededParametersFile", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Thống kê các thông số vượt ngưỡng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetThresholdExceededParameters(FilterThresholdExceededParametersModel requestModel)
        {
            var results = GetThresholdExceededParameters_Data(requestModel);
            var page = results.ToPagedList(requestModel.PageNumber, requestModel.PageSize);
            return Core.Models.HttpResponse.OkList(page);
        }

        /// <summary>
        /// Query dữ liệu thông số vượt ngưỡng (dùng chung cho Get và Export)
        /// </summary>
        private List<ThresholdExceededParametersModel> GetThresholdExceededParameters_Data(FilterThresholdExceededParametersModel requestModel)
        {
            // Lấy tất cả thông số đã ghi, kiểu số, có range định nghĩa
            var query = _unitOfWork.ElogbookRepository<OperationParameters>()
                .GetQuery(x => !x.IsDeleted
                               && x.IsChecked
                               && x.IsNumberType
                               && x.FromRange.HasValue
                               && x.ToRange.HasValue
                               && x.CurrentValue != null
                               && x.CurrentValue != "");

            // Lọc theo nhà máy
            if (requestModel.FactoryID > 0)
            {
                query = query.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Lọc theo từ ngày
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
                requestModel.From = frDate;
                query = query.Where(x => x.CheckedDate.HasValue
                                         && DbFunctions.TruncateTime(x.CheckedDate.Value) >= frDate.Date);
            }

            // Lọc theo đến ngày
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
                requestModel.To = toDate;
                query = query.Where(x => x.CheckedDate.HasValue
                                         && DbFunctions.TruncateTime(x.CheckedDate.Value) <= toDate.Date);
            }

            var rawList = query
                .OrderBy(x => x.FactoryID)
                .ThenBy(x => x.CheckedDate)
                .ThenBy(x => x.ParameterGroupID)
                .ThenBy(x => x.ID)
                .ToList();

            // Lọc các bản ghi có CurrentValue vượt ngoài [FromRange, ToRange]
            var exceededList = rawList.Where(p =>
            {
                if (!float.TryParse(p.CurrentValue, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float val))
                    return false;
                return val < p.FromRange.Value || val > p.ToRange.Value;
            }).ToList();

            // Load factory và parameter group một lần để tránh N+1
            var factoryIds = exceededList.Select(x => x.FactoryID).Distinct().ToList();
            var factories = _unitOfWork.ElogbookRepository<Factory>()
                .GetQuery(x => !x.IsDeleted && factoryIds.Contains(x.ID))
                .ToList();

            var groupIds = exceededList.Where(x => x.ParameterGroupID.HasValue)
                .Select(x => x.ParameterGroupID.Value).Distinct().ToList();
            var paramGroups = _unitOfWork.ElogbookRepository<ParameterGroup>()
                .GetQuery(x => !x.IsDeleted && groupIds.Contains(x.ID))
                .ToList();

            var results = new List<ThresholdExceededParametersModel>();
            int rowIndex = 1;

            foreach (var p in exceededList)
            {
                var factory = factories.FirstOrDefault(f => f.ID == p.FactoryID);
                var group = p.ParameterGroupID.HasValue
                    ? paramGroups.FirstOrDefault(g => g.ID == p.ParameterGroupID.Value)
                    : null;

                string range = $"{p.FromRange} : {p.ToRange}";

                results.Add(new ThresholdExceededParametersModel
                {
                    RowIndex        = rowIndex++,
                    Factory         = factory != null
                                        ? new ValueLabelModel { Value = factory.ID, Label = factory.FactoryName }
                                        : new ValueLabelModel { Value = p.FactoryID, Label = "N/A" },
                    CheckedDate     = p.CheckedDate,
                    ParameterGroupName = group?.ParameterGroupName ?? string.Empty,
                    ParameterName   = p.ParameterName,
                    KKSCode         = p.KKSCode,
                    Range           = range,
                    ExceededValue   = p.CurrentValue
                });
            }

            return results;
        }

        #endregion

        #region Báo cáo thống kê chi tiết lỗi thiết bị
        /// <summary>
        /// Báo cáo chi tiết lỗi thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetDeviceErrorDetailReport(FilterGetDeviceErrorDetailReportModel requestModel)
        {
            var results = GetDeviceErrorDetailReport_Data(requestModel);
            var page = results.ToPagedList(requestModel.PageNumber, requestModel.PageSize);
            return Core.Models.HttpResponse.OkList(page);
        }

        /// <summary>
        /// Get dữ liệu Báo cáo chi tiết lỗi thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private List<GetDeviceErrorDetailReportModel> GetDeviceErrorDetailReport_Data(FilterGetDeviceErrorDetailReportModel requestModel)
        {
            var operationParameters = _unitOfWork.ElogbookRepository<OperationParameters>()
                .GetQuery(x => !x.IsDeleted && x.ErrorCode != null && (x.ErrorCode == (int)DeviceErrorCode.Error || x.ErrorCode == (int)DeviceErrorCode.Stop || x.ErrorCode == (int)DeviceErrorCode.Sim || x.ErrorCode == (int)DeviceErrorCode.Jump));

            // Filter by factory
            if (requestModel.FactoryID > 0)
            {
                operationParameters = operationParameters.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Filter by position
            if (requestModel.OperationPositionID > 0)
            {
                operationParameters = operationParameters.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }

            // Filter by cycle
            if (requestModel.CycleID > 0)
            {
                operationParameters = operationParameters.Where(x => x.CycleID == requestModel.CycleID);
            }

            // Date range handling (From)
            DateTime frDate;
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
                if (frDate == DateTime.MinValue)
                {
                    if (requestModel.Year > 1990)
                    {
                        frDate = new DateTime(requestModel.Year, 1, 1);
                    }
                    else
                    {
                        frDate = new DateTime(DateTime.Now.Year, 1, 1);
                    }
                }
            }
            else
            {
                if (requestModel.Year > 1990)
                {
                    frDate = new DateTime(requestModel.Year, 1, 1);
                }
                else
                {
                    frDate = new DateTime(DateTime.Now.Year, 1, 1);
                }
            }
            operationParameters = operationParameters.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date);
            requestModel.From = frDate;

            // Date range handling (To)
            DateTime toDate;
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
                if (toDate == DateTime.MaxValue)
                {
                    if (requestModel.Year > 1990)
                    {
                        toDate = new DateTime(requestModel.Year, 12, 31);
                    }
                    else if (toDate == DateTime.MaxValue)
                    {
                        toDate = new DateTime(DateTime.Now.Year, 12, 31);
                    }
                }
            }
            else
            {
                if (requestModel.Year > 1990)
                {
                    toDate = new DateTime(requestModel.Year, 12, 31);
                }
                else
                {
                    toDate = new DateTime(DateTime.Now.Year, 12, 31);
                }
            }
            operationParameters = operationParameters.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date);
            requestModel.To = toDate;

            var results = operationParameters
                .Select(x => new GetDeviceErrorDetailReportModel
                {
                    Factory = new ReportKeyValueModel { Value = x.FactoryID, Label = x.Factory.FactoryName },
                    InsertedAt = x.InsertedAt,
                    OperationPosition = new ReportKeyValueModel { Value = x.OperationPositionID, Label = x.OperationPosition.OperationPositionName },
                    Cycle = new ReportKeyValueModel { Value = x.CycleID, Label = x.Cycle.CycleName },
                    OperationTimeLine = new ReportKeyValueModel { Value = x.OperationTimeLineID, Label = x.OperationTimeLine != null ? x.OperationTimeLine.TimeLineName.ToString() : string.Empty },
                    ParameterGroup = new ReportKeyValueModel { Value = x.ParameterGroupID, Label = x.ParameterGroup.ParameterGroupName },
                    Parameter = new ReportKeyValueModel { Value = x.ParameterID, Label = x.ParameterName },
                    KKSCode = x.KKSCode,
                    ErrorCode = x.ErrorCode ?? 0,
                    FactoryID = x.FactoryID,
                    OperationPositionID = x.OperationPositionID,
                    CycleID = x.CycleID,
                    OperationTimeLineID = x.OperationTimeLineID,
                    ParameterGroupID = x.ParameterGroupID,
                    ParameterID = x.ParameterID
                })
                .OrderBy(x => x.FactoryID)
                .ThenBy(x => x.InsertedAt)
                .ThenBy(x => x.OperationPositionID)
                .ThenBy(x => x.CycleID)
                .ThenBy(x => x.OperationTimeLineID)
                .ThenBy(x => x.ParameterGroupID)
                .ThenBy(x => x.ParameterID)
                .ToList();

            // Set sequence number
            int idx = 1;
            foreach (var r in results)
            {
                r.No = idx++;
            }

            return results;
        }

        /// <summary>
        /// Xuất báo cáo chi tiết lỗi thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportDeviceErrorDetailReport(FilterGetDeviceErrorDetailReportModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetDeviceErrorDetailReport_Data(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, FOLDER_REPORT)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, FOLDER_REPORT));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, FOLDER_REPORT, $"{FILE_NAME_05}_{currentDate}.xlsx");

                bool isCreatexlsx = GenerateExportDeviceErrorDetailReportFile(rootData, outputFile, requestModel);

                if (isCreatexlsx)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: ExportDeviceErrorDetailReport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Tạo file báo cáo chi tiết lỗi thiết bị
        /// </summary>
        /// <param name="rootData"></param>
        /// <param name="outputFile"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool GenerateExportDeviceErrorDetailReportFile(List<GetDeviceErrorDetailReportModel> rootData, string outputFile, FilterGetDeviceErrorDetailReportModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 10, 16, title1, "BÁO CÁO THỐNG KÊ CHI TIẾT LỖI THIẾT BỊ");
                SetBoldweight(sheet, indexRow, 10, true, false, 14);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 10, 16, title2, $"Dữ liệu từ ngày {requestModel.From:dd/MM/yyyy} - {requestModel.To:dd/MM/yyyy}");
                SetBoldweight(sheet, indexRow, 10, true, true, 11);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 1, headerRow, "STT");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 2, 5, headerRow, "Nhà máy");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 6, 6, headerRow, "Ngày ghi thông số");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 7, 10, headerRow, "Cương vị");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 11, 13, headerRow, "Chu kỳ");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 14, 14, headerRow, "Khung giờ");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 15, 18, headerRow, "Nhóm thông số");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 19, 22, headerRow, "Thông số");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 23, 23, headerRow, "Mã KKS");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 24, 25, headerRow, "Báo lỗi thiết bị");

                indexRow += 1;//Excel Row Index = 7
                IRow headerRow2 = sheet.CreateRow(indexRow);

                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 25), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;

                if (rootData.Count > 0)
                {
                    foreach (var item in rootData)
                    {
                        indexRow += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(item.No.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 5, bodyRow, item.Factory?.Label);
                        bodyRow.CreateCell(6).SetCellValue($"{item.InsertedAt:dd/MM/yyyy}");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 7, 10, bodyRow, item.OperationPosition?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, bodyRow, item.Cycle?.Label);
                        bodyRow.CreateCell(14).SetCellValue(item.OperationTimeLine?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, bodyRow, item.ParameterGroup?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 19, 22, bodyRow, item.Parameter?.Label);
                        bodyRow.CreateCell(23).SetCellValue(item.KKSCode);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 24, 25, bodyRow, item.DeviceError);
                    }

                    int edRowBdIndex = indexRow;
                    int edColBdIndex = 25;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 1, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Left);
                    SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 10, bodyRow, "Không có dữ liệu");
                    SetAlignmentCennter(sheet, indexRow, 1);
                    SetStyleFinish(sheet, 1, 3, 7, 26, 61, 58);
                }

                sheet.SetColumnWidth(0, 3 * 256);    // Col A blank
                sheet.SetColumnWidth(6, 10 * 256);    // Ngày ghi thông số

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: GenerateExportDeviceErrorDetailReportFile", ex.ToString());
                return false;
            }
        }

        #endregion

        #region Thống kê số lượng báo bận công tác và dừng máy 
        /// <summary>
        /// Thống kê số lượng báo bận công tác và dừng máy 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ResponseBaseModel GetBusyAndMachineStopStatistics(FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            var results = GetBusyAndMachineStopStatistics_Data(requestModel);
            var page = results.ToPagedList(requestModel.PageNumber, requestModel.PageSize);
            return Core.Models.HttpResponse.OkList(page);
        }

        /// <summary>
        /// Lấy dữ liệu thống kê số lượng báo bận công tác và dừng máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private List<BusyAndMachineStopStatisticsModel> GetBusyAndMachineStopStatistics_Data(FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            DateTime currentDate = DateTime.Now;
            DateTime firstDayOfYear = new DateTime(currentDate.Year, 1, 1);

            var operationParameters = _unitOfWork.ElogbookRepository<OperationParameters>()
                .GetQuery(x => !x.IsDeleted && (x.ErrorCode == (int)DeviceErrorCode.Stop || x.ErrorCode == (int)DeviceErrorCode.Busy));

            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                operationParameters = operationParameters.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                operationParameters = operationParameters.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }

            // Chu kỳ ghi thông số
            if (requestModel.CycleID > 0)
            {
                operationParameters = operationParameters.Where(x => x.CycleID == requestModel.CycleID);
            }

            // Ngày ghi thông số
            DateTime frDate;
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
                if (frDate == DateTime.MinValue)
                {
                    if (requestModel.Year > 1990)
                    {
                        frDate = new DateTime(requestModel.Year, 1, 1);
                    } else
                    {
                        frDate = new DateTime(DateTime.Now.Year, 1, 1);
                    }
                }
            }
            else
            {
                if (requestModel.Year > 1990)
                {
                    frDate = new DateTime(requestModel.Year, 1, 1);
                }
                else
                {
                    frDate = new DateTime(DateTime.Now.Year, 1, 1);
                }
            }
            operationParameters = operationParameters.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date);
            requestModel.From = frDate;

            DateTime toDate;
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

                if (toDate == DateTime.MaxValue)
                {
                    if (requestModel.Year > 1990)
                    {
                        toDate = new DateTime(requestModel.Year, 12, 31);
                    }
                    else
                    {
                        toDate = new DateTime(DateTime.Now.Year, 12, 31);
                    }
                }
            }
            else
            {
                if (requestModel.Year > 1990)
                {
                    toDate = new DateTime(requestModel.Year, 12, 31);
                }
                else
                {
                    toDate = new DateTime(DateTime.Now.Year, 12, 31);
                }
            }
            operationParameters = operationParameters.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date);
            requestModel.To = toDate;

            // Group by FactoryID, OperationPositionID, CycleID and exact InsertedAt (including time)
            var grouped = operationParameters
                .OrderBy(x=>x.FactoryID)
                .ThenBy(x => x.OperationPositionID)
                .ThenBy(x => x.InsertedAt)
                .ThenBy(x => x.CycleID)
                .GroupBy(x => new { x.FactoryID, x.OperationPositionID, x.InsertedAt, x.CycleID })
                .ToList();

            var resultDatas = grouped
                .Select(g =>
                {
                    //distinct OperationTimeLineIDs for Busy
                   var busyIds = g
                    .Where(x => x.ErrorCode == (int)DeviceErrorCode.Busy && x.OperationTimeLineID != null)
                    .GroupBy(x => x.OperationTimeLineID)
                    .Select(gr => new { OperationTimeLineID = gr.Key, OperationTimeLineName = gr.First().OperationTimeLine.TimeLineName })
                    .OrderBy(x => x.OperationTimeLineID)
                    .ToList();

                    // distinct OperationTimeLineIDs for Stop
                    var stopIds = g
                    .Where(x => x.ErrorCode == (int)DeviceErrorCode.Stop && x.OperationTimeLineID != null)
                    .GroupBy(x => x.OperationTimeLineID)
                    .Select(gr => new { OperationTimeLineID = gr.Key, OperationTimeLineName = gr.First().OperationTimeLine.TimeLineName })
                    .OrderBy(x => x.OperationTimeLineID)
                    .ToList();

                    return new BusyAndMachineStopStatisticsModel
                    {
                        Factory = new ReportKeyValueModel { Value = g.Key.FactoryID, Label = g.Select(x => x.Factory.FactoryName).FirstOrDefault() },
                        OperationPosition = new ReportKeyValueModel { Value = g.Key.OperationPositionID, Label = g.Select(x => x.OperationPosition.OperationPositionName).FirstOrDefault() },
                        Cycle = new ReportKeyValueModel { Value = g.Key.CycleID, Label = g.Select(x => x.Cycle.CycleName).FirstOrDefault() },
                        InsertedAt = g.Key.InsertedAt,
                        FactoryID = g.Key.FactoryID,
                        OperationPositionID = g.Key.OperationPositionID,
                        CycleID = g.Key.CycleID,
                        BusyOperationTimeLines = string.Join(";", busyIds.Select(x => x.OperationTimeLineName)),
                        StopOperationTimeLines = string.Join(";", stopIds.Select(x => x.OperationTimeLineName))
                    };
                })
                .OrderBy(x => x.FactoryID)
                .ThenBy(x => x.OperationPositionID)
                .ThenBy(x => x.InsertedAt)
                .ThenBy(x => x.CycleID)
                .ToList();

            return resultDatas;
        }

        /// <summary>
        /// Xuất file thống kê số lượng báo bận công tác và dừng máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportBusyAndMachineStopStatistics(FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetBusyAndMachineStopStatistics_Data(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, FOLDER_REPORT)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, FOLDER_REPORT));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, FOLDER_REPORT, $"{FILE_NAME_06}_{currentDate}.xlsx");

                bool isCreatexlsx = GenerateBusyAndMachineStopStatisticsFile(rootData, outputFile, requestModel);

                if (isCreatexlsx)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: ExportBusyAndMachineStopStatistics", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Tạo file thống kê số lượng báo bận công tác và dừng máy
        /// </summary>
        /// <param name="rootData"></param>
        /// <param name="outputFile"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool GenerateBusyAndMachineStopStatisticsFile(List<BusyAndMachineStopStatisticsModel> rootData, string outputFile, FilterBusyAndMachineStopStatisticsModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 5, 14, title1, "BẢNG THỐNG KÊ SỐ LƯỢNG BẬN CÔNG TÁC VÀ DỪNG MÁY");
                SetBoldweight(sheet, indexRow, 5, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 7, 12, title2, $"Dữ liệu từ ngày {requestModel.From:dd/MM/yyyy} - {requestModel.To:dd/MM/yyyy}");
                SetBoldweight(sheet, indexRow, 7, true, true, 11);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 1, headerRow1, "STT");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 2, 5, headerRow1, "Nhà máy");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 6, 9, headerRow1, "Cương vị");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 10, 11, headerRow1, "Ngày ghi  thông số");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 12, 14, headerRow1, "Chu kỳ");
                CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, headerRow1, "Khung giờ");

                indexRow += 1;//Excel Row Index = 7
                IRow headerRow2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 15, 16, headerRow2, "Bận công tác");
                CreateCellTypeStr(sheet, indexRow, indexRow, 17, 18, headerRow2, "Dừng máy");
                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 18), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;
                int countIndex = 0;

                if (rootData.Count > 0)
                {
                    foreach (var item in rootData)
                    {
                        indexRow += 1;
                        countIndex++;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(countIndex.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 5, bodyRow, item.Factory.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 6, 9, bodyRow, item.OperationPosition.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, bodyRow, $"{item.InsertedAt:dd/MM/yyyy}");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 12, 14, bodyRow, item.Cycle.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 15, 16, bodyRow, item.BusyOperationTimeLines);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 17, 18, bodyRow, item.StopOperationTimeLines);
                    }
                    int edRowBdIndex = indexRow;
                    int edColBdIndex = 18;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 1, 18), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Left);

                    SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 18, bodyRow, "Không có dữ liệu");
                    SetAlignmentCennter(sheet, indexRow, 1);
                    SetStyleFinish(sheet, 1, 3, 7, 26, 61, 58);
                }

                sheet.SetColumnWidth(0, 3 * 256);    // Col A blank

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ReportService: GenerateBusyAndMachineStopStatisticsFile", ex.ToString());
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Kích hoạt hoặc hủy kích hoạt báo cáo
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="isActive"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Active(int reportId, bool isActive, int userID)
        {
            var report = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.ID == reportId).FirstOrDefault();
            if (report != null)
            {
                report.IsActive = isActive;
                report.LastUpdatedUserID = userID;
                _unitOfWork.ElogbookRepository<Report>().Update(report);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        public ReportViewModel AddReport(AddEditReportModel addReportModel)
        {
            var report = new Report();
            MapToReport(report, addReportModel);
            _unitOfWork.ElogbookRepository<Report>().Add(report);
            _unitOfWork.Complete();
            return new ReportViewModel()
            {
                Id = report.ID,
                Name = report.Name,
                Description = report.Description,
                ModuleType = report.ModuleType,
                IsActive = report.IsActive,
                IsDefault = report.IsDefault,
                IsAddToDashboard = report.IsAddToDashboard,
                InsertedAt = report.InsertedAt,
            };
        }

        public bool AddToDashboard(AddToDashboardModel addToDashboardModel)
        {
            var report = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.ID == addToDashboardModel.Id).FirstOrDefault();
            if (report != null)
            {
                AddEditWidgetModel addEditWidgetModel = new AddEditWidgetModel();
                addEditWidgetModel.Name = report.Name;
                addEditWidgetModel.Description = report.Description;
                addEditWidgetModel.ChartType = addToDashboardModel.ChartType;
                addEditWidgetModel.WidgetType = addToDashboardModel.WidgetType;
                addEditWidgetModel.ReportId = addToDashboardModel.Id;
                addEditWidgetModel.ModuleType = report.ModuleType;
                _widgetService.Add(addEditWidgetModel);
                report.IsAddToDashboard = true;
                _unitOfWork.ElogbookRepository<Report>().Update(report);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        public bool RemoveFromDashboard(int reportId, int currentUserId)
        {
            var report = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.ID == reportId).FirstOrDefault();
            if (report != null)
            {
                _widgetService.DeleteByReportId(reportId, currentUserId);
                report.IsAddToDashboard = false;
                _unitOfWork.ElogbookRepository<Report>().Update(report);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        public bool Delete(int reportId, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.ID == reportId).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<Report>().Delete(entity, userID);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        public ReportViewModel EditReport(AddEditReportModel editReportModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.ID == editReportModel.Id).FirstOrDefault();
            if (entity != null)
            {
                var report = new Report();
                MapToReport(report, editReportModel);
                _unitOfWork.ElogbookRepository<Report>().Add(report);
                _unitOfWork.Complete();
                return new ReportViewModel()
                {
                    Id = report.ID,
                    Name = report.Name,
                    Description = report.Description,
                    ModuleType = report.ModuleType,
                    IsActive = report.IsActive,
                    IsDefault = report.IsDefault,
                    IsAddToDashboard = report.IsAddToDashboard,
                    InsertedAt = report.InsertedAt,
                };

            }
            return null;
        }

        public ResponseBaseModel Find(SearchReportModel searchModel)
        {
            // Filter data by condition
            var reports = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted);
            if (!string.IsNullOrWhiteSpace(searchModel.ModuleType))
            {
                reports = reports.Where(x => x.ModuleType == searchModel.ModuleType.Trim());
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Name))
            {
                reports = reports.Where(x => x.Name.Contains(searchModel.Name.Trim()));
            }

            if (searchModel.IsActive != null)
            {
                reports = reports.Where(x => x.IsActive == searchModel.IsActive);
            }
            // Return and paging data
            var page = reports.Select(
                e => new ReportViewModel()
                {
                    Id = e.ID,
                    Name = e.Name,
                    Description = e.Description,
                    ModuleType = e.ModuleType,
                    IsActive = e.IsActive,
                    IsDefault = e.IsDefault,
                    IsAddToDashboard = e.IsAddToDashboard,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(x => x.InsertedAt).ToPagedList(searchModel.PageNumber, searchModel.PageSize);

            return Core.Models.HttpResponse.OkList(page);
        }

        /// <summary>
        /// Find by module not dashboard
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public List<ReportViewModel> FindByModuleNotDashboard (string module)
        {
            // Filter data by condition
            var reports = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.IsActive && x.ModuleType == module && !x.IsAddToDashboard);
            return reports.Select(
                e => new ReportViewModel()
                {
                    Id = e.ID,
                    Name = e.Name,
                    Description = e.Description,
                    ModuleType = e.ModuleType,
                    IsActive = e.IsActive,
                    IsDefault = e.IsDefault,
                    IsAddToDashboard = e.IsAddToDashboard,
                    InsertedAt = e.InsertedAt,
                }).ToList();
        }

        private void MapToReport(Report report, AddEditReportModel editReportModel)
        {
            report.Name = editReportModel.Name;
            report.Description = editReportModel.Description;
            report.LastUpdatedUserID = editReportModel.CurrentUserID;
            if (editReportModel.Id == null)
            {
                report.CreatedUserID = editReportModel.CurrentUserID;
                report.ModuleType = editReportModel.ModuleType;
                report.IsDeleted = false;
                report.IsDefault = false;
                report.IsActive = true;
                report.IsAddToDashboard = false;
            }
        }

        #region Download File
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
        #endregion

        #region Excel Helper
        private void CreateCellTypeStr(ISheet sheet, int startRow, int endRow, int startCol, int endCol, IRow headerRow, string value)
        {
            headerRow.CreateCell(startCol).SetCellValue(value);
            MergeColumnCellsTypeStr(sheet, startRow, endRow, startCol, endCol);
        }

        private void SetStyleTableHeader(ISheet sheet, CellRangeAddress cellRange, XSSFColor xSSFColor, BorderStyle borderStyle)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

            for (int row = cellRange.FirstRow; row <= cellRange.LastRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                for (int col = cellRange.FirstColumn; col <= cellRange.LastColumn; col++)
                {
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    cellStyle.SetFillForegroundColor(xSSFColor);
                    cellStyle.FillPattern = FillPattern.SolidForeground;

                    var font = (XSSFFont)xssfWorkbook.CreateFont();
                    font.IsBold = true;
                    font.FontName = "Times New Roman";
                    cellStyle.SetFont(font);
                    cellStyle.WrapText = true;

                    cellStyle.BorderTop = borderStyle;
                    cellStyle.BorderLeft = borderStyle;
                    cellStyle.BorderRight = borderStyle;
                    cellStyle.BorderBottom = borderStyle;

                    cellStyle.Alignment = HorizontalAlignment.Center;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    currentCell.CellStyle = cellStyle;
                }
            }
        }

        private void SetStyleTableBody(ISheet sheet, CellRangeAddress cellRange, XSSFColor xSSFColor, BorderStyle borderStyle, HorizontalAlignment alignment)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

            for (int row = cellRange.FirstRow; row <= cellRange.LastRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                for (int col = cellRange.FirstColumn; col <= cellRange.LastColumn; col++)
                {
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    cellStyle.SetFillForegroundColor(xSSFColor);
                    cellStyle.FillPattern = FillPattern.SolidForeground;

                    var font = (XSSFFont)xssfWorkbook.CreateFont();
                    font.FontName = "Times New Roman";
                    cellStyle.SetFont(font);
                    cellStyle.WrapText = true;

                    cellStyle.BorderTop = borderStyle;
                    cellStyle.BorderLeft = borderStyle;
                    cellStyle.BorderRight = borderStyle;
                    cellStyle.BorderBottom = borderStyle;

                    cellStyle.Alignment = alignment;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    currentCell.CellStyle = cellStyle;
                }
            }
        }

        private void SetAlignmentCennter(ISheet sheet, int row, int col)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

            IRow currentRow = sheet.GetRow(row);
            ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);

            cellStyle.Alignment = HorizontalAlignment.Center;
            currentCell.CellStyle = cellStyle;
        }

        private void MergeColumnCellsTypeStr(ISheet sheet, int startRow, int endRow, int startCol, int endCol)
        {
            ICell cell = sheet.GetRow(startRow).GetCell(startCol, MissingCellPolicy.CREATE_NULL_AS_BLANK);

            if (cell != null)
            {
                cell.SetCellType(CellType.String);

                cell.SetCellValue(cell.StringCellValue);

                sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startCol, endCol));
            }
        }

        private void SetBoldweight(ISheet sheet, int row, int col, bool isBold = false, bool isItalic = false, int fontSize = 11)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();
            var font = (XSSFFont)xssfWorkbook.CreateFont();
            cellStyle.SetFillForegroundColor(new XSSFColor(new byte[] { 255, 255, 255 }));
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.WrapText = true;
            font.FontName = "Times New Roman";
            font.IsBold = isBold;
            font.IsItalic = isItalic;
            font.FontHeightInPoints = fontSize;
            cellStyle.SetFont(font);
            ICell currentCell = sheet.GetRow(row).GetCell(col);
            currentCell.CellStyle = cellStyle;
        }

        private void SetStyleFinish(ISheet sheet, int igonreStRow, int ignoreEdRow, int ignoreStCol, int ignoreEdCol, int endRow, int endCol)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();
            for (int row = 0; row < endRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                if (currentRow == null)
                {
                    currentRow = sheet.CreateRow(row);
                }
                for (int col = 0; col < endCol; col++)
                {
                    if (row >= igonreStRow && row <= ignoreEdRow && col >= ignoreStCol && col <= ignoreEdCol)
                    {
                        continue;
                    }
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    if (currentCell.StringCellValue == "")
                    {
                        var font = (XSSFFont)xssfWorkbook.CreateFont();
                        font.FontName = "Times New Roman";
                        cellStyle.SetFont(font);
                        cellStyle.WrapText = true;
                        cellStyle.SetFillForegroundColor(new XSSFColor(new byte[] { 255, 255, 255 }));
                        cellStyle.FillPattern = FillPattern.SolidForeground;
                        currentCell.CellStyle = cellStyle;
                    }
                }
            }
        }

        private void CreateCellTypeNum(ISheet sheet, int startRow, int endRow, int startCol, int endCol, IRow headerRow, int value)
        {
            headerRow.CreateCell(startCol).SetCellValue(value);
            MergeColumnCellsTypeNum(sheet, startRow, endRow, startCol, endCol);
        }

        private void MergeColumnCellsTypeNum(ISheet sheet, int startRow, int endRow, int startCol, int endCol)
        {
            ICell cell = sheet.GetRow(startRow).GetCell(startCol, MissingCellPolicy.CREATE_NULL_AS_BLANK);

            if (cell != null)
            {
                cell.SetCellType(CellType.Numeric);

                cell.SetCellValue(cell.NumericCellValue);

                sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startCol, endCol));
            }
        }

        #endregion

        /// <summary>
        /// Lấy danh sách WidgetType và ChartType theo ReportId
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public List<ReportWidgetTypeModel> GetWidgetTypesByReportId(long reportId)
        {
            return _unitOfWork.ElogbookRepository<ReportWidgetType>()
                .GetQuery(x => x.ReportId == reportId && !x.IsDeleted)
                .Select(x => new ReportWidgetTypeModel
                {
                    WidgetType = x.WidgetType,
                    ChartType = x.ChartType,
                })
                .ToList();
        }
    }
}

