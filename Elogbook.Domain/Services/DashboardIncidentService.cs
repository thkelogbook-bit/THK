using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Helpers;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Triển khai các phương thức báo cáo bất thường thiết bị
    /// </summary>
    public class DashboardIncidentService : IDashboardIncidentService
    {
        #region Properties field

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public DashboardIncidentService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Bất thường thiết bị
        public List<DashboardGroupItemModel> Find(AbnormalIncidentSearch requestModel)
        {
            return GetDeviceAbnormalDataGeneral(requestModel);
        }

        public ResponseBaseModel FindDetail(SearchPrtReportModel requestModel)
        {
            var rootData = GetDeviceAbnormalDataDetail(requestModel);

            return HttpResponse.OkList(rootData.ToPagedList(requestModel.PageNumber,requestModel.PageSize));
        }

        public HttpResponseMessage ExportDetailHandle(SearchPrtReportModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            try
            {
                requestModel.PageSize = int.MaxValue;
                requestModel.PageNumber = 1;
                var data = GetDeviceAbnormalDataDetail(requestModel);

                // tạo thư mục lưu file xlsx
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_DEVICE_ABNORMAL_DETAIL}_{currentDate}.xlsx");

                bool isCreatexlsx = CreateReportDetail(data, outputFile, requestModel);
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
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage ExportGeneralHandle(AbnormalIncidentSearch requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            try
            {
                var data = GetDeviceAbnormalDataGeneral(requestModel);

                // tạo thư mục lưu file xlsx
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_DEVICE_ABNORMAL_GENERAL}_{currentDate}.xlsx");

                bool isCreatexlsx = CreateReportGeneral(data, outputFile, requestModel);
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
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private bool CreateReportGeneral(List<DashboardGroupItemModel> dataInput, string outputFile, AbnormalIncidentSearch requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Bất thường thiết bị");

                // Title
                IRow title1 = sheet.CreateRow(1); //Excel Row Index = 2
                CreateCellTypeStr(sheet, 1, 2, 4, 11, title1, "BÁO CÁO TỔNG HỢP BẤT THƯỜNG THIẾT BỊ");
                SetBoldweight(sheet, 1, 4, true, false, 16);

                IRow title2 = sheet.CreateRow(3); //Excel Row Index = 4
                CreateCellTypeStr(sheet, 3, 3, 4, 11, title2, requestModel.Title);
                SetBoldweight(sheet, 3, 4, true, true, 11);

                if (dataInput.Count > 0)
                {
                    IRow headerRow1 = sheet.CreateRow(5); //Excel Row Index = 6
                    CreateCellTypeStr(sheet, 5, 6, 1, 3, headerRow1, "Nhà máy");

                    IRow headerRow2 = sheet.CreateRow(6); //Excel Row Index = 7

                    int stColBd = 1;
                    int edColBd = 2;
                    foreach (var item in dataInput[0].DataDetail)
                    {
                        edColBd += 2;
                        CreateCellTypeStr(sheet, 5, 5, edColBd, edColBd + 7, headerRow1, "Tháng " + item.Month);

                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Tổng hoạt động");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Số bất thường");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Đã tạo PYC");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Chưa tạo PYC");
                    }
                    edColBd += 2;
                    CreateCellTypeStr(sheet, 5, 6, edColBd, edColBd, headerRow1, "Tổng");
                    SetStyleTableHeader(sheet, new CellRangeAddress(5, 6, 1, edColBd), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBd = 7;
                    int edRowBd = 6;
                    foreach (var fac in dataInput)
                    {
                        edRowBd += 1;
                        int indexCol = 1;
                        IRow bodyRow = sheet.CreateRow(edRowBd);

                        CreateCellTypeStr(sheet, edRowBd, edRowBd, indexCol, indexCol + 2, bodyRow, fac.Factory.Label);
                        indexCol = 2;
                        foreach (var mon in fac.DataDetail)
                        {
                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.Total);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalAbnormal);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalOpenTicket);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalNotOpen);
                        }
                        indexCol += 2;
                        bodyRow.CreateCell(indexCol).SetCellValue(fac.TotalAbnormalSingelFactory);
                    }

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBd, edRowBd, stColBd, edColBd), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                    SetStyleFinish(sheet, stRowBd - 2, edRowBd, stColBd, edColBd, edRowBd + 50, edColBd + 50);
                }
                else
                {
                    IRow noneData = sheet.CreateRow(5); //Excel Row Index = 6
                    noneData.CreateCell(1).SetCellValue("Không tìm thấy dữ liệu tương ứng.");

                    SetStyleFinish(sheet, 1, 3, 4, 11, 61, 58);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "DashboardIncidentService: ExportGeneralHandle", ex.ToString());
                return false;
            }
        }

        private List<DashboardGroupItemModel> GetDeviceAbnormalDataGeneral(AbnormalIncidentSearch requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => facs.Contains(x.FactoryID));
            }

            // Năm
            if (requestModel.Year != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months) && !requestModel.Months.Equals("-1"))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();
                entities = entities.Where(x => mons.Contains(x.ActualDate.Month));
            }
            DateTime currentDate = DateTime.Now;

            entities = entities.Where(x => x.ActualDate <= currentDate);

            var rootData = entities.ToList();

            var facIds = rootData.Select(x => x.FactoryID).Distinct().ToList();
            var months = rootData.Select(x => x.ActualDate.Month).Distinct().ToList();

            List<DashboardGroupItemModel> lstdta = new List<DashboardGroupItemModel>();

            //Loop nhà máy
            foreach (var fId in facIds)
            {
                DashboardGroupItemModel newData = new DashboardGroupItemModel();

                // Thông tin nhà máy
                var facInfo = rootData.Where(x => x.FactoryID == fId).Select(x => x.Factory).FirstOrDefault();
                if (facInfo != null)
                {
                    newData.Factory = new ValueLabelCheckedModel { Value = fId, Label = facInfo.FactoryName };
                }

                // Loop từng tháng
                List<DataDetailSingelMonth> dataDetail = new List<DataDetailSingelMonth>();
                foreach (var mon in months)
                {
                    DataDetailSingelMonth newMon = new DataDetailSingelMonth();

                    // Tổng số hoạt động của từng tháng theo từng nhà máy
                    newMon.Total = rootData.Count(x => x.ActualDate.Month == mon && x.FactoryID == fId);

                    // Tổng số bất thường của từng tháng theo từng nhà máy
                    newMon.TotalAbnormal = rootData.Count(x => x.ActualDate.Month == mon && x.FactoryID == fId && x.AmDeviceAbnormalStatusID > 1);

                    // Tổng số PYC chưa tạo theo trang thái có bất thường
                    newMon.TotalNotOpen = rootData.Count(x => x.ActualDate.Month == mon && x.FactoryID == fId && x.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_NoCreate);

                    // Tổng số PYC đã tạo theo trạng thái có bất thường
                    newMon.TotalOpenTicket = newMon.TotalAbnormal - newMon.TotalNotOpen;

                    // Tháng
                    newMon.Month = mon;

                    // Tổng số bất thường của 1 nhà máy
                    newData.TotalAbnormalSingelFactory += newMon.TotalAbnormal;

                    dataDetail.Add(newMon);
                }

                newData.DataDetail = dataDetail;

                lstdta.Add(newData);
            }

            List<DataDetailSingelMonth> dataDetailTotal = new List<DataDetailSingelMonth>();
            DashboardGroupItemModel totalInfo = new DashboardGroupItemModel
            {
                Factory = new ValueLabelCheckedModel { Value = 0, Label = "Tổng" }
            };

            foreach (var mon in months)
            {
                DataDetailSingelMonth newMon = new DataDetailSingelMonth();
                newMon.Month = mon;
                foreach (var dtDetail in lstdta.Select(x => x.DataDetail))
                {
                    newMon.Total += dtDetail.Where(x => x.Month == mon).Select(x => x.Total).First();
                    newMon.TotalAbnormal += dtDetail.Where(x => x.Month == mon).Select(x => x.TotalAbnormal).First();
                    newMon.TotalOpenTicket += dtDetail.Where(x => x.Month == mon).Select(x => x.TotalOpenTicket).First();
                    newMon.TotalNotOpen += dtDetail.Where(x => x.Month == mon).Select(x => x.TotalNotOpen).First();
                }

                totalInfo.TotalAbnormalSingelFactory += newMon.TotalAbnormal;
                dataDetailTotal.Add(newMon);
            }
            
            totalInfo.DataDetail = dataDetailTotal;

            lstdta.Add(totalInfo);

            return lstdta;
        }

        private List<DeviceAbnormalDataDetailModel> GetDeviceAbnormalDataDetail(SearchPrtReportModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted && x.AmDeviceAbnormalStatusID > 1);

            // Năm
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            DateTime currentDate = DateTime.Now;
            entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= currentDate.Date);

            var amRootIds = entities.Select(x => x.ID).ToList();

            // Mức độ khẩn cấp
            if (requestModel.EmergencyID != CommonConstant.ALL_KEY)
            {
                var amRequest = _unitOfWork.ElogbookRepository<AmRequestTicket>()
                    .GetQuery(x => !x.IsDeleted && amRootIds.Contains(x.AutonomousMaintenanceID.Value) && x.AmEmergencyId == requestModel.EmergencyID)
                    .ToList();

                var amIds = amRequest.Select(x => x.AutonomousMaintenanceID).ToList();

                entities = entities.Where(x => amIds.Contains(x.ID));
            }

            amRootIds = entities.Select(x => x.ID).ToList();

            var dataResult = entities.Select(
                e => new DeviceAbnormalDataDetailModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    MaintenanceSchedule = new ValueLabelCheckedModel() { Value = e.AmMaintenanceSchedule.ID, Label = e.AmMaintenanceSchedule.MaintenanceScheduleName },
                    OperationPosition = new ValueLabelCheckedModel() { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    OperationShift = new ValueLabelCheckedModel() { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                    Device = new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc },
                    ActualDate = e.ActualDate,
                    AmRequestTicketID = e.AmRequestTicketID,
                    AmDeviceAbnormalStatusID = e.AmDeviceAbnormalStatusID,
                    StatusValue = 0,
                    ProcessStatusValue = 0,
                    CompleterUser = e.CompleterUserID != null ? new ValueLabelCheckedModel() { Value = e.CompleterUser.ID, Label = e.CompleterUser.FullName } : null,
                    CompleteDate = e.CompleteDate,
                    WorkDetails = e.AmWorkDetails.Select(t => new AmWorkDetailModel()
                    {
                        ID = t.ID,
                        Factory = new ValueLabelCheckedModel() { Value = t.Factory.ID, Label = t.Factory.FactoryName },
                        AutonomousMaintenance = new ValueLabelCheckedModel() { Value = t.AutonomousMaintenance.ID, Label = "" },
                        Work = new ValueLabelCheckedModel() { Value = t.AmWork.ID, Label = t.AmWork.WorkName },
                        WorkType = new ValueLabelCheckedModel() { Value = t.AmWorkType.ID, Label = t.AmWorkType.TypeDesc },
                        Status = new AmStatusInfoModel { ID = t.AmStatus.ID, Status = t.AmStatus.StatusName },
                        StatusID = t.AmStatusID,
                        Order = t.Order,
                        SLA = t.SLA,
                        Description = t.Description,
                        StickTime = t.StickTime,
                        UpdatedAt = t.UpdatedAt,
                        Implementer = t.ImplementerID != null ? new ValueLabelCheckedModel() { Value = t.Implementer.ID, Label = t.Implementer.FullName } : null
                    }).ToList(),
                })
                .OrderByDescending(t => t.ActualDate)
                .ToList();

            var rootData = dataResult.ToList();

            var amRequestDatas = _unitOfWork.ElogbookRepository<AmRequestTicket>()
                .GetQuery(x => !x.IsDeleted && amRootIds.Contains(x.AutonomousMaintenanceID.Value));

            var amRqIds = amRequestDatas.Select(x => x.ID).Distinct().ToList();

            var wtDatas = _unitOfWork.ElogbookRepository<WorkTicket>()
                        .GetQuery(x => !x.IsDeleted && amRqIds.Contains(x.RequestTicketID.Value) && x.WorkTicketStatus == (int)EnumTicketStatus.Approved).ToList();

            if (rootData.Count > 0)
            {
                foreach (var item in rootData)
                {
                    // Trạng thái Đã tạo PYC
                    if (item.AmDeviceAbnormalStatusID != (int)AmEnumDeAbStatus.Yes_NoCreate)
                    {
                        item.StatusValue = 1;
                    }

                    // Trạng thái Chưa tạo PYC
                    if (item.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_NoCreate)
                    {
                        item.StatusValue = 2;
                    }

                    // Trạng thái xử lý  "Đang xử lý"
                    if (item.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Created)
                    {
                        item.ProcessStatusValue = (int)AmEnumReportStatus.InProgessPYC;
                    }

                    // Trạng thái "Dừng xử lý"
                    if (item.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Stop)
                    {
                        item.ProcessStatusValue = (int)AmEnumReportStatus.StopPYC;
                    }

                    // Trạng thái "Đang thực hiện sửa chữa"
                    if (item.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_InProgress)
                    {
                        item.ProcessStatusValue = (int)AmEnumReportStatus.OnGoingPCT;
                    }

                    // Trạng thái "Đã hoàn thành sửa chữa"
                    if (item.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Finish)
                    {
                        item.ProcessStatusValue = (int)AmEnumReportStatus.CompletedPCT;
                    }

                    // Trường hợp Có bất thường - PYC đã được duyệt
                    if (item.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Approved)
                    {
                        var idAmRq0 = amRequestDatas.Where(x => x.AutonomousMaintenanceID == item.ID && x.AmStatusID == (int)AmEnumRequestTicket.Registered)
                            .Select(t => t.ID).FirstOrDefault();

                        if (idAmRq0 > 0)
                        {
                            var idWt = wtDatas.Where(x => x.WorkTicketStatus == (int)EnumTicketStatus.Approved && x.RequestTicketID == idAmRq0)
                                .Select(t => t.ID).FirstOrDefault();

                            // Trạng thái xử lý "Đã cấp phiếu"
                            if (idWt > 0)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.ApprovedPCT;
                            }
                            // Trạng thái xử lý "Đã đăng ký công tác"
                            else
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.RegisteredPCT;
                            }
                        }

                        // Trạng thái xử lý "Chờ đăng ký công tác"
                        var idAmRq1 = amRequestDatas.Where(x => x.AutonomousMaintenanceID == item.ID && x.AmStatusID == (int)AmEnumRequestTicket.Approved)
                            .Select(t => t.ID).FirstOrDefault();
                        if (idAmRq1 > 0)
                        {
                            item.ProcessStatusValue = (int)AmEnumReportStatus.WaitForRegistPCT;
                        }
                    }

                    // Người thực hiện
                    if (item.CompleterUser == null)
                    {
                        item.CompleterUser = item.WorkDetails.Where(x => x.StatusID != (int)AmEnumStatus.Waiting).Select(x => x.Implementer).FirstOrDefault();
                    }

                    // Ngày thực hiện
                    if (item.CompleteDate == null)
                    {
                        var dataGet = item.WorkDetails.Where(x => x.StatusID != (int)AmEnumStatus.Waiting).Select(x => x.Implementer).FirstOrDefault();
                        if (dataGet != null)
                        {
                            item.CompleteDate = item.WorkDetails.Where(x => x.StatusID != (int)AmEnumStatus.Waiting).Select(x => x.UpdatedAt).FirstOrDefault();
                        }
                    }
                }

                // Tìm kiếm theo trạng thái xử lý
                if (requestModel.StatusID != CommonConstant.ALL_KEY)
                {
                    rootData = rootData.FindAll(x => x.StatusValue == requestModel.StatusID);
                }

                // Tìm kiếm theo trạng thái xử lý
                if (requestModel.ProcessingStatusID != CommonConstant.ALL_KEY)
                {
                    rootData = rootData.FindAll(x => x.ProcessStatusValue == requestModel.ProcessingStatusID);
                }
            }

            return rootData;
        }

        private bool CreateReportDetail(List<DeviceAbnormalDataDetailModel> dataInput, string outputFile, SearchPrtReportModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();

            try
            {
                ISheet sheet = workbook.CreateSheet("Bất thường thiết bị");
                int indexRow = 0;
                int stColBdIndex = 1;
                int edColBdIndex = 27;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 8, 20, title1, "BÁO CÁO BẤT THƯỜNG THIẾT BỊ CHI TIẾT");
                SetBoldweight(sheet, indexRow, 8, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 8, 20, title2, requestModel.Title);
                SetBoldweight(sheet, indexRow, 8, true, true, 11);

                if (dataInput.Count > 0)
                {
                    indexRow += 2;//Excel Row Index = 6
                    IRow headerRow2 = sheet.CreateRow(indexRow);
                    headerRow2.CreateCell(1).SetCellValue("STT");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, headerRow2, "Ngày có hoạt động");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, headerRow2, "Nhà máy");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 10, headerRow2, "Cương vị");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, headerRow2, "Thiết bị");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 14, 16, headerRow2, "Lịch công tác");
                    headerRow2.CreateCell(17).SetCellValue("Ca");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 18, 19, headerRow2, "Trạng thái");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 20, 22, headerRow2, "Trạng thái xử lý");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 23, 25, headerRow2, "Người thực hiện");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 26, 27, headerRow2, "Ngày thực hiện");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBdIndex = indexRow;
                    int countStt = 0;
                    foreach (var item in dataInput)
                    {
                        indexRow += 1;
                        countStt += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(countStt.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, bodyRow, item.ActualDate.ToString("yyyy/MM/dd"));
                        CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, bodyRow, item.Factory.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 8, 10, bodyRow, item.OperationPosition.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, bodyRow, item.Device.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 14, 16, bodyRow, item.MaintenanceSchedule.Label);
                        bodyRow.CreateCell(17).SetCellValue(item.OperationShift.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 18, 19, bodyRow, item.Status.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 20, 22, bodyRow, item.ProcessingStatus.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 23, 25, bodyRow, item.CompleterUser != null ? item.CompleterUser.Label : string.Empty);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 26, 27, bodyRow, item.CompleteDate.HasValue ? item.CompleteDate.Value.ToString("yyyy/MM/dd") : string.Empty);
                    }
                    int edRowBdIndex = indexRow;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                    SetStyleFinish(sheet, stRowBdIndex, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    IRow noneData = sheet.CreateRow(5); //Excel Row Index = 6
                    noneData.CreateCell(1).SetCellValue("Không tìm thấy dữ liệu tương ứng.");

                    SetStyleFinish(sheet, 1, 3, 7, 18, 61, 58);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "DashboardIncidentService: ExportDetailHandle", ex.ToString());
                return false;
            }
        }
        #endregion

        ResponseBaseModel IDashboardIncidentService.AllActivity(AbnormalIncidentSearch requestModel)
        {
            var lstdta = new List<DashboardAllActivityGroupModel>();
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Năm
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => mons.Contains(x.ActualDate.Month));
            }

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => facs.Contains(x.FactoryID));
            }

            entities = entities.Where(x => x.ActualDate <= DateTime.Now);

            var dataResult = entities.GroupBy(x => new { x.ActualDate.Month, x.AmStatusID })
                .Select(n => new
                {
                    Month = n.Key.Month,
                    AmStatusID = n.Key.AmStatusID,
                    Count = n.Count()
                });

            var rootData = dataResult.ToList();
            if (rootData.Count > 0)
            {
                var mapCount1 = new Dictionary<int, DashboardAllActivityGroupModel>();
                foreach (var item in rootData)
                {
                    var itemGroup = new DashboardAllActivityGroupModel();

                    if (!mapCount1.TryGetValue(item.Month, out itemGroup))
                    {
                        itemGroup = new DashboardAllActivityGroupModel();
                    }

                    itemGroup.Year = requestModel.Year;
                    itemGroup.Month = item.Month;
                    if (item.AmStatusID == (int)AmEnumStatus.DoNotWork)
                    {
                        itemGroup.TotalUnProcess = item.Count;
                    }
                    else if (item.AmStatusID == (int)AmEnumStatus.Waiting)
                    {
                        itemGroup.TotalNotYetProcess = item.Count;
                    }
                    else if (item.AmStatusID == (int)AmEnumStatus.Completed)
                    {
                        itemGroup.TotalCompleted = item.Count;
                    }
                    else if (item.AmStatusID == (int)AmEnumStatus.OnGoing)
                    {
                        itemGroup.TotalOnGoing = item.Count;
                    }

                    if (!mapCount1.ContainsKey(item.Month))
                        mapCount1.Add(item.Month, itemGroup);
                }
                lstdta = mapCount1.Values.ToList();
            }
            return HttpResponse.OkList(lstdta);
        }

        ResponseBaseModel IDashboardIncidentService.AbnormalActivity(AbnormalIncidentSearch requestModel)
        {
            List<DashboardAbnormalActivityGroupModel> lstdta = new List<DashboardAbnormalActivityGroupModel>();
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Năm
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => mons.Contains(x.ActualDate.Month));
            }

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => facs.Contains(x.FactoryID));
            }
            entities = entities.Where(x => x.ActualDate <= DateTime.Now);
            ////////////
            var dataResult = entities
                .Where(x => x.AmDeviceAbnormalStatusID != (int)AmEnumDeAbStatus.No 
                        && x.AmDeviceAbnormalStatusID > 0)
                .GroupBy(x => new { x.ActualDate.Month, x.AmDeviceAbnormalStatusID })
                .Select(n => new
                {
                    Month = n.Key.Month,
                    AmDeviceAbnormalStatusID = n.Key.AmDeviceAbnormalStatusID,
                    Count = n.Count()
                });

            var rootData = dataResult.ToList();
            if (rootData.Count > 0)
            {
                var mapCount1 = new Dictionary<int, DashboardAbnormalActivityGroupModel>();
                foreach (var item in rootData)
                {
                    var itemGroup = new DashboardAbnormalActivityGroupModel();
                    if (!mapCount1.TryGetValue(item.Month, out itemGroup))
                    {
                        itemGroup = new DashboardAbnormalActivityGroupModel();
                    }

                    itemGroup.Year = requestModel.Year;
                    itemGroup.Month = item.Month;
                    if (item.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_NoCreate)
                    {
                        itemGroup.TotalNotCreate += item.Count;
                    }
                    else 
                    {
                        itemGroup.TotalCreated += item.Count;
                    }

                    if (!mapCount1.ContainsKey(item.Month))
                        mapCount1.Add(item.Month, itemGroup);
                }

                lstdta = mapCount1.Values.ToList();
            }

            return HttpResponse.OkList(lstdta);
        }

        ResponseBaseModel IDashboardIncidentService.AbnormalTicketInfo(AbnormalIncidentSearch requestModel)
        {
            List<DashboardAbnormalTicketGroupModel> lstdta = new List<DashboardAbnormalTicketGroupModel>();

            // Số yêu cầu đã tạo - Lấy tất cả danh sách các Phiếu yêu cầu đã được tạo trong AmRequestTicket
            var requestTicketEntities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);
            // Năm
            if (requestModel.Year > 1900)
            {
                requestTicketEntities = requestTicketEntities.Where(x => x.InsertedAt.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                requestTicketEntities = requestTicketEntities.Where(x => x.InsertedAt.Month == requestModel.Month);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();

                requestTicketEntities = requestTicketEntities.Where(x => mons.Contains(x.InsertedAt.Month));
            }

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                requestTicketEntities = requestTicketEntities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                requestTicketEntities = requestTicketEntities.Where(x => facs.Contains(x.FactoryID));
            }
            requestTicketEntities = requestTicketEntities.Where(x => x.InsertedAt <= DateTime.Now);
            var rootData = requestTicketEntities
                .GroupBy(x => new { x.InsertedAt.Month, x.AmStatusID })
                .Select(n => new
                {
                    Month = n.Key.Month,
                    AmStatusID = n.Key.AmStatusID,
                    Count = n.Count()
                }).ToList();


            if (rootData.Count > 0)
            {
                var mapCount1 = new Dictionary<int, DashboardAbnormalTicketGroupModel>();
                foreach (var item in rootData)
                {
                    var itemGroup = new DashboardAbnormalTicketGroupModel();
                    if (!mapCount1.TryGetValue(item.Month, out itemGroup))
                    {
                        itemGroup = new DashboardAbnormalTicketGroupModel();
                    }

                    itemGroup.Year = requestModel.Year;
                    itemGroup.Month = item.Month;
                    
                    if (item.AmStatusID == (int)AmEnumRequestTicket.UnSend)
                    {
                        itemGroup.TotalNotSubmited = item.Count;
                    }
                    else if (item.AmStatusID == (int)AmEnumRequestTicket.InProgress || item.AmStatusID == (int)AmEnumRequestTicket.Reject)
                    {
                        itemGroup.TotalInProgress += item.Count;
                    }
                    else if (item.AmStatusID == (int)AmEnumRequestTicket.Approved || item.AmStatusID == (int)AmEnumRequestTicket.Registered)
                    {
                        itemGroup.TotalTicketApproved += item.Count;
                    }
                    else if (item.AmStatusID == (int)AmEnumRequestTicket.Stop)
                    {
                        itemGroup.TotalTicketStopped += item.Count;
                    }

                    if (!mapCount1.ContainsKey(item.Month))
                        mapCount1.Add(item.Month, itemGroup);
                }
                lstdta = mapCount1.Values.ToList();
            }

            return HttpResponse.OkList(lstdta);
        }

        public ResponseBaseModel Top5User(AbnormalIncidentSearch requestModel)
        {
            List<DashboardAMUserGroupModel> resultData = new List<DashboardAMUserGroupModel>();
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Năm
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => mons.Contains(x.ActualDate.Month));
            }

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => facs.Contains(x.FactoryID));
            }
            entities = entities.Where(x => x.ActualDate <= DateTime.Now);

            var amIds = entities.Select(x => x.ID).ToList();

            // Số nhân sự tham gia hoạt động AM - Là những nhân viên có tham gia vào công việc của hoạt động AM
            var amWorkDetailEntities = _unitOfWork.ElogbookRepository<AmWorkDetail>()
                .GetQuery(x => amIds.Contains(x.AutonomousMaintenanceID) && !x.IsDeleted).ToList();

            var userIds = amWorkDetailEntities.Where(x => x.ImplementerID > 0).Select(t => t.ImplementerID).Distinct().ToList();

            if (userIds.Count <= 0)
            {
                return HttpResponse.OkList(resultData);
            }

            Dictionary<int, int> dictUser = new Dictionary<int, int>();

            foreach (var item in userIds)
            {
                var total = amWorkDetailEntities.Where(x => x.ImplementerID == item).Select(t => t.AutonomousMaintenanceID).Distinct().ToList();

                dictUser.Add((int)item, total.Count);
            }

            var sortedByValue = dictUser.OrderByDescending(pair => pair.Value);

            int count = 0;
            foreach (KeyValuePair<int, int> pair in sortedByValue)
            {
                count += 1;
                if (count > 5)
                {
                    break;
                }
                var newItem = new DashboardAMUserGroupModel();

                var user = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted && x.ID == pair.Key)
                    .Select(e => new DashboardUserModel()
                    {
                        ID = e.ID,
                        FullName = e.FullName,
                        UserName = e.UserName,
                        Factory = e.FactoryID.HasValue ? new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName } : null,
                    }).FirstOrDefault();

                newItem.UserInfo = user;
                newItem.TotalAm = pair.Value;

                resultData.Add(newItem);
            }
            return HttpResponse.OkList(resultData);
        }

        public DashboardSummaryGroupModel Summary(AbnormalIncidentSearch requestModel)
        {
            DashboardSummaryGroupModel result = new DashboardSummaryGroupModel();

            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Năm
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => mons.Contains(x.ActualDate.Month));
            }

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => facs.Contains(x.FactoryID));
            }

            DateTime currentDate = DateTime.Now;
            entities = entities.Where(x => x.ActualDate <= currentDate);

            var amDatas = entities.ToList();
            var amIds = amDatas.Select(x => x.ID).ToList();

            //Tổng các hoạt động AM
            AmSummaryInfo amSummaryInfo = new AmSummaryInfo
            {
                TotalWaiting = amDatas.Count(x => x.AmStatusID == (int)AmEnumStatus.Waiting),
                TotalDoNotWork = amDatas.Count(x => x.AmStatusID == (int)AmEnumStatus.DoNotWork),
                TotalOnGoing = amDatas.Count(x => x.AmStatusID == (int)AmEnumStatus.OnGoing),
                TotalCompleted = amDatas.Count(x => x.AmStatusID == (int)AmEnumStatus.Completed),
                Total = amDatas.Count
            };
            result.AmSummaryInfo = amSummaryInfo;

            //Số bất thường thiết bị - Là những hoạt động AM có phát sinh thiết bị bất thường
            var deviceAbnormalDatas = entities.Where(x => x.AmDeviceAbnormalStatusID > 1 && !x.IsDeleted).ToList();

            DeviceAbnormalSummaryInfo deviceAbnormalSummaryInfo = new DeviceAbnormalSummaryInfo
            {
                Total = deviceAbnormalDatas.Count,
                TotalUnCreated = deviceAbnormalDatas.Count(x => x.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_NoCreate)
            };
            deviceAbnormalSummaryInfo.TotalCreated = deviceAbnormalSummaryInfo.Total - deviceAbnormalSummaryInfo.TotalUnCreated;
            result.DeviceAbnormalSummaryInfo = deviceAbnormalSummaryInfo;

            // Số yêu cầu đã tạo - Lấy tất cả danh sách các Phiếu yêu cầu đã được tạo trong AmRequestTicket
            var requestTicketEntities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);
            // Năm
            if (requestModel.Year > 1900)
            {
                requestTicketEntities = requestTicketEntities.Where(x => x.InsertedAt.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                requestTicketEntities = requestTicketEntities.Where(x => x.InsertedAt.Month == requestModel.Month);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();

                requestTicketEntities = requestTicketEntities.Where(x => mons.Contains(x.InsertedAt.Month));
            }

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                requestTicketEntities = requestTicketEntities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                requestTicketEntities = requestTicketEntities.Where(x => facs.Contains(x.FactoryID));
            }

            var requestTicketDatas = requestTicketEntities.ToList();

            RequestTicketSummaryInfo requestTicketSummaryInfo = new RequestTicketSummaryInfo
            {
                TotalUnSentPYC = requestTicketDatas.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.UnSend),
                TotalInProgessPYC = requestTicketDatas.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.InProgress || x.AmStatusID == (int)AmEnumRequestTicket.Reject),
                TotalApprovePYC = requestTicketDatas.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved || x.AmStatusID == (int)AmEnumRequestTicket.Registered),
                TotalStopPYC = requestTicketDatas.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.Stop),
                Total = requestTicketDatas.Count
            };
            result.RequestTicketSummaryInfo = requestTicketSummaryInfo;

            // Số nhân sự tham gia hoạt động AM - Là những nhân viên có tham gia vào công việc của hoạt động AM
            var amWorkDetailEntities = _unitOfWork.ElogbookRepository<AmWorkDetail>()
                .GetQuery(x => amIds.Contains(x.AutonomousMaintenanceID) && !x.IsDeleted).ToList();

            var userIds = amWorkDetailEntities.Where(x => x.ImplementerID > 0).Select(t => t.ImplementerID).Distinct().ToList();

            NumberOfParticipantsInfo numberOfParticipantsInfo = new NumberOfParticipantsInfo
            {
                TotalUser = userIds.Count
            };
            result.NumberOfParticipantsInfo = numberOfParticipantsInfo;

            return result;
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

        private void SetStyleTableBody(ISheet sheet, CellRangeAddress cellRange, XSSFColor xSSFColor, BorderStyle borderStyle)
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

                    cellStyle.BorderTop = borderStyle;
                    cellStyle.BorderLeft = borderStyle;
                    cellStyle.BorderRight = borderStyle;
                    cellStyle.BorderBottom = borderStyle;

                    cellStyle.Alignment = HorizontalAlignment.Left;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    currentCell.CellStyle = cellStyle;
                }
            }
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
    }
}
