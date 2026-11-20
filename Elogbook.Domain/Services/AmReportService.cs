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
using Elogbook.Core.Models.Response.EmailNotification;
using Elogbook.Core.Models.Response.EmailService;
using Elogbook.Core.Models.Response.User;
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
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// khai báo service tuân thủ hoạt động
    /// </summary>
    public class AmReportService : IAmReportService
    {
        #region Private fields
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ExcelHelper excel = new ExcelHelper();
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailConfigurationService _emailConfigurationService;
        private readonly ISystemConfigurationService _systemConfigurationService;
        #endregion

        #region Contructor
        public AmReportService(ElogbookUnitOfWork unitOfWork,
            IEmailTemplateService emailTemplateService,
            IEmailConfigurationService emailConfigurationService,
            ISystemConfigurationService systemConfigurationService)
        {
            _unitOfWork = unitOfWork;
            _emailTemplateService = emailTemplateService;
            _emailConfigurationService = emailConfigurationService;
            _systemConfigurationService = systemConfigurationService;
        }
        #endregion

        #region Public Method

        #region Tiến trình xử lý
        public List<ValueLabelCheckedModel> GetReportStatus(int statusType)
        {
            var lstStatus = new List<ValueLabelCheckedModel>
            {
                new ValueLabelCheckedModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
            };

            if (statusType == 3)
            {
                lstStatus.Add(new ValueLabelCheckedModel
                {
                    Value = (int)AmEnumReportStatus.InProgessPYC,
                    Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.InProgessPYC)
                });
                lstStatus.Add(new ValueLabelCheckedModel
                {
                    Value = (int)AmEnumReportStatus.StopPYC,
                    Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.StopPYC)
                });

                statusType = 1;
            }

            var entities = _unitOfWork.ElogbookRepository<AmReportStatus>().GetQuery(x => x.StatusType == statusType && !x.IsDeleted);

            var listData = entities.Select(
                e => new ValueLabelCheckedModel()
                {
                    Value = e.ID,
                    Label = e.StatusName,
                }).ToList();

            if (listData.Count > 0)
            {
                lstStatus.AddRange(listData);
            }

            return lstStatus;
        }

        public ReportPrtGeneralModel RptyReportGeneral(SearchPrtReportModel requestModel)
        {
            requestModel.SortOrder = "ASC";
            requestModel.PageSize = int.MaxValue;
            requestModel.PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER;

            var rootData = GetDataRptyReportDetail(requestModel);

            if (rootData.Count > 0)
            {
                var result = GetDataRptyReportGeneral(rootData);
                return result;
            }

            return null;
        }

        public ResponseBaseModel RptyReportDetail(SearchPrtReportModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm kiếm theo trạng thái chờ đăng ký và đã đăng ký
            entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Registered || x.AmStatusID == (int)AmEnumRequestTicket.Approved);

            // Tìm kiếm theo nhà máy
            entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);

            // Tìm kiếm theo năm tạo
            entities = entities.Where(x => x.InsertedAt.Year == requestModel.Year);

            // Tìm kiếm theo tháng tạo
            entities = entities.Where(x => x.InsertedAt.Month == requestModel.Month);

            // Tìm kiếm theo mức độ khẩn cấp
            if (requestModel.EmergencyID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmEmergencyId == requestModel.EmergencyID);
            }

            var dataResult = entities.Select(
                e => new PrtReportDetailModel()
                {
                    ID = e.ID,
                    RequestTicketNumber = e.RequestTicketNumber,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    FactoryID = e.FactoryID,
                    Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : null,
                    CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                    CreateDate = e.InsertedAt,
                    Status = new ValueLabelCheckedModel { Value = 0, Label = "Đã được duyệt" },
                    StatusValue = e.AmStatusID,
                    ProcessStatusValue = 0
                })
                .OrderByDescending(t => t.CreateDate)
                .ToList();

            var rootData = dataResult.ToList();

            if (rootData.Count > 0)
            {
                var lstIds = rootData.Select(x => x.ID);
                var lstWtStatuss = new List<int>()
                {
                    (int)EnumTicketStatus.Approved,
                    (int)EnumTicketStatus.Allowed,
                    (int)EnumTicketStatus.InProgress,
                    (int)EnumTicketStatus.WaitForConfirmation,
                    (int)EnumTicketStatus.Completed
                };

                var dataWorkTickets = _unitOfWork.ElogbookRepository<WorkTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && lstWtStatuss.Contains(x.WorkTicketStatus) && !x.IsDeleted).ToList();

                var dataRegistTickets = _unitOfWork.ElogbookRepository<RegistrationTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && !x.IsDeleted).ToList();

                foreach (var item in rootData)
                {
                    // Trạng thái phiếu yêu cầu là Chờ đăng ký
                    if (item.StatusValue == (int)AmEnumRequestTicket.Approved)
                    {
                        item.ProcessStatusValue = (int)AmEnumReportStatus.WaitForRegistPCT;
                    }

                    // Trường hợp phiếu công tác có trạng thái từ cấp phiếu tới lúc hoàn thành
                    if (item.StatusValue == (int)AmEnumRequestTicket.Registered)
                    {
                        // Tìm PYC trong PCT
                        var wTicket = dataWorkTickets.Find(x => x.RequestTicketID == item.ID);

                        if (wTicket != null)
                        {
                            // Đã duyệt phiếu yêu cầu (Đã đăng ký)- Đã tạo phiếu đăng ký công tác - Đã cấp phiếu (PCT)
                            if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Approved)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.ApprovedPCT;
                                continue;
                            }

                            // Đã duyệt phiếu yêu cầu (Đã đăng ký)- Đã tạo phiếu đăng ký công tác - Hoàn thành sửa chữa (PCT)
                            else if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Completed)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.CompletedPCT;
                                continue;
                            }

                            // Đã duyệt phiếu yêu cầu (Đã đăng ký)- Đã tạo phiếu đăng ký công tác - Đang thực hiện sửa chữa (PCT)
                            else
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.OnGoingPCT;
                                continue;
                            }
                        }

                        //Đã duyệt phiếu yêu cầu - Đã tạo phiếu đăng ký công tác - Chưa được cấp phiếu công tác
                        // Tìm PYC trong PĐK
                        var rTicket = dataRegistTickets.Find(x => x.RequestTicketID == item.ID);
                        if (rTicket != null)
                        {
                            item.ProcessStatusValue = (int)AmEnumReportStatus.RegisteredPCT;
                        }
                    }
                }

                // Tìm kiếm theo trạng thái xử lý
                if (requestModel.ProcessingStatusID != CommonConstant.ALL_KEY)
                {
                    rootData = rootData.FindAll(x => x.ProcessStatusValue == requestModel.ProcessingStatusID);
                }
            }

            return HttpResponse.OkList(rootData.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
        }

        public HttpResponseMessage PrtReportGeneralExport(int factoryID, int year, int month)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                SearchPrtReportModel requestModel = new SearchPrtReportModel()
                {
                    FactoryID = factoryID,
                    Year = year,
                    Month = month
                };
                var rootData = GetDataRptyReportDetail(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                var dataInput = GetDataRptyReportGeneral(rootData);

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_RPT_GENERAL}_{currentDate}.xlsx");

                var factoryName = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == requestModel.FactoryID).Select(t => t.FactoryName).FirstOrDefault();
                bool isCreatexlsx = RptyReportGeneralGenerateFile(requestModel, dataInput, outputFile, factoryName);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: PrtReportGeneralExport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage PrtReportDetailExport(SearchPrtReportModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                requestModel.SortOrder = "ASC";
                requestModel.PageSize = int.MaxValue;
                requestModel.PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER;

                var dataInput = GetDataRptyReportDetail(requestModel);

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_RPT_DETAIL}_{currentDate}.xlsx");

                var factoryName = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == requestModel.FactoryID).Select(t => t.FactoryName).FirstOrDefault();

                bool isCreatexlsx = RptyReportDetailGenerateFile(requestModel, dataInput, outputFile, factoryName);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: PrtReportGeneralExport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region Yêu cầu bảo trì
        public ResponseBaseModel RequestAmReportGeneral(SearchPrtReportModel requestModel)
        {
            var result = GetDataRequestAmReportGeneral(requestModel);
            return HttpResponse.OkList(result);
        }

        public ResponseBaseModel RequestAmReportDetail(SearchPrtReportModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm kiếm theo nhà máy
            entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);

            // Tìm kiếm theo năm tạo
            entities = entities.Where(x => x.InsertedAt.Year == requestModel.Year);

            // Tìm kiếm theo tháng tạo
            entities = entities.Where(x => x.InsertedAt.Month == requestModel.Month);

            // Tìm kiếm theo mức độ khẩn cấp
            if (requestModel.EmergencyID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmEmergencyId == requestModel.EmergencyID);
            }

            // Tìm kiếm theo trạng thái "Chưa gửi phiếu"
            if (requestModel.StatusID == (int)AmEnumReportStatus.UnSentPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.UnSend);
            }

            // Tìm kiếm theo trạng thái "Đang xử lý"
            if (requestModel.StatusID == (int)AmEnumReportStatus.InProgessPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.InProgress || x.AmStatusID == (int)AmEnumRequestTicket.Reject);
            }

            // Tìm kiếm theo trạng thái "Dừng xử lý"
            if (requestModel.StatusID == (int)AmEnumReportStatus.StopPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Stop);
            }

            // Tìm kiếm theo trạng thái "Đã được duyệt"
            if (requestModel.StatusID == (int)AmEnumReportStatus.ApprovePYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved || x.AmStatusID == (int)AmEnumRequestTicket.Registered);
            }

            List<RequestAmReportDetailModel> rootData = null;

            if (requestModel.SortColumn == "emergency")
            {
                if (requestModel.SortOrder == "desc")
                {
                    var dataResult = entities.Select(
                        e => new RequestAmReportDetailModel()
                        {
                            ID = e.ID,
                            RequestTicketNumber = e.RequestTicketNumber,
                            Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                            Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : null,
                            CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                            Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                            CreateDate = e.InsertedAt,
                            AmStatusID = e.AmStatusID,
                            ProcessStatusValue = 0
                        })
                        .OrderByDescending(t => t.Emergency != null ? t.Emergency.Label : "")
                        .ToList();

                    rootData = dataResult.ToList();
                }
                else
                {
                    var dataResult = entities.Select(
                        e => new RequestAmReportDetailModel()
                        {
                            ID = e.ID,
                            RequestTicketNumber = e.RequestTicketNumber,
                            Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                            Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : null,
                            CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                            Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                            CreateDate = e.InsertedAt,
                            AmStatusID = e.AmStatusID,
                            ProcessStatusValue = 0
                        })
                        .OrderBy(t => t.Emergency != null ? t.Emergency.Label : "")
                        .ToList();

                    rootData = dataResult.ToList();
                }
            }
            else
            {
                var dataResult = entities.Select(
                    e => new RequestAmReportDetailModel()
                    {
                        ID = e.ID,
                        RequestTicketNumber = e.RequestTicketNumber,
                        Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                        Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : null,
                        CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                        Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                        CreateDate = e.InsertedAt,
                        AmStatusID = e.AmStatusID,
                        ProcessStatusValue = 0
                    }).OrderByDescending(t => t.ID)
                    .ToList();

                rootData = dataResult.ToList();
            }

            if (rootData.Count > 0)
            {
                var lstIds = rootData.Select(x => x.ID);
                var lstWtStatuss = new List<int>()
                {
                    (int)EnumTicketStatus.Approved,
                    (int)EnumTicketStatus.Allowed,
                    (int)EnumTicketStatus.InProgress,
                    (int)EnumTicketStatus.WaitForConfirmation,
                    (int)EnumTicketStatus.Completed
                };

                var dataWorkTickets = _unitOfWork.ElogbookRepository<WorkTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && lstWtStatuss.Contains(x.WorkTicketStatus) && !x.IsDeleted).ToList();

                var dataRegistTickets = _unitOfWork.ElogbookRepository<RegistrationTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && !x.IsDeleted).ToList();

                foreach (var item in rootData)
                {
                    // Trạng thái phiếu yêu cầu là Chưa gửi phiếu
                    if (item.AmStatusID == (int)AmEnumRequestTicket.UnSend)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.UnSentPYC;
                        item.ProcessStatusValue = 0;
                    }

                    // Trạng thái phiếu yêu cầu là Đang xử lý or Từ chối
                    if (item.AmStatusID == (int)AmEnumRequestTicket.InProgress || item.AmStatusID == (int)AmEnumRequestTicket.Reject)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.InProgessPYC;
                        item.ProcessStatusValue = 0;
                    }

                    // Trạng thái phiếu yêu cầu là Dừng xử lý
                    if (item.AmStatusID == (int)AmEnumRequestTicket.Stop)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.StopPYC;
                        item.ProcessStatusValue = 0;
                    }

                    // Trạng thái phiếu yêu cầu là Chờ đăng ký
                    if (item.AmStatusID == (int)AmEnumRequestTicket.Approved)
                    {
                        item.ProcessStatusValue = (int)AmEnumReportStatus.WaitForRegistPCT;
                        item.StatusValue = (int)AmEnumReportStatus.ApprovePYC;
                    }

                    // Trạng thái phiếu yêu cầu là Đã đăng ký
                    if (item.AmStatusID == (int)AmEnumRequestTicket.Registered)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.ApprovePYC;

                        // Tìm PYC trong PCT
                        var wTicket = dataWorkTickets.Find(x => x.RequestTicketID == item.ID);

                        if (wTicket != null)
                        {
                            // Đã duyệt PYC - Hoàn thành sửa chữa (PCT)
                            if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Completed)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.CompletedPCT;
                                item.WorkTicket = new ValueLabelCheckedModel()
                                {
                                    Value = wTicket.ID,
                                    Label = wTicket.WorkTicketNumber
                                };
                                continue;
                            }

                            // Đã duyệt PYC - Đã cấp phiếu (PCT)
                            else if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Approved)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.ApprovedPCT;
                                item.WorkTicket = new ValueLabelCheckedModel()
                                {
                                    Value = wTicket.ID,
                                    Label = wTicket.WorkTicketNumber
                                };
                                continue;
                            }

                            // Đã duyệt PYC - Đang thực hiện sửa chữa (PCT)
                            else
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.OnGoingPCT;
                                item.WorkTicket = new ValueLabelCheckedModel()
                                {
                                    Value = wTicket.ID,
                                    Label = wTicket.WorkTicketNumber
                                };
                                continue;
                            }
                        }

                        //Đã duyệt phiếu yêu cầu - Đã tạo phiếu đăng ký công tác - Chưa được cấp phiếu công tác
                        // Tìm PYC trong PĐK
                        var rTicket = dataRegistTickets.Find(x => x.RequestTicketID == item.ID);
                        if (rTicket != null)
                        {
                            item.ProcessStatusValue = (int)AmEnumReportStatus.RegisteredPCT;
                            item.RegisterTicket = new ValueLabelCheckedModel()
                            {
                                Value = rTicket.ID,
                                Label = rTicket.TicketNumber
                            };
                        }
                    }
                }

                // Tìm kiếm theo trạng thái xử lý
                if (requestModel.ProcessingStatusID != CommonConstant.ALL_KEY)
                {
                    rootData = rootData.FindAll(x => x.ProcessStatusValue == requestModel.ProcessingStatusID);
                }
            }

            return HttpResponse.OkList(rootData.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
        }

        public RequestAmReportEmergencyModel RequestAmReportEmergency(SearchPrtReportModel requestModel)
        {
            var result = GetDataRequestAmReportEmergency(requestModel);
            return result;
        }

        public HttpResponseMessage RequestAmReportDetailExport(SearchPrtReportModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                requestModel.SortOrder = "ASC";
                requestModel.PageSize = int.MaxValue;
                requestModel.PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER;

                var dataInput = GetDataRequestAmReportDetail(requestModel);

                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_RPT_DETAIL}_{currentDate}.xlsx");

                bool isCreatexlsx = RequestAmReportDetailGenerateFile(requestModel, dataInput, outputFile);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RequestAmReportDetailExport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage RequestAmReportEmergencyExport(SearchPrtReportModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                requestModel.SortOrder = "ASC";
                requestModel.PageSize = int.MaxValue;
                requestModel.PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER;

                var dataInput = GetDataRequestAmReportEmergency(requestModel);

                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_R_AM_EMERGENCY}_{currentDate}.xlsx");

                bool isCreatexlsx = RequestAmReportEmergencyGenerateFile(requestModel, dataInput, outputFile);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RequestAmReportEmergencyExport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage RequestAmReportGeneralExport(SearchPrtReportModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var dataInput = GetDataRequestAmReportGeneral(requestModel);

                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_R_AM_GENERAL}_{currentDate}.xlsx");

                bool isCreatexlsx = RequestAmReportGeneralGenerateFile(requestModel, dataInput, outputFile);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RequestAmReportEmergencyExport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region Tuân thủ hoạt động
        /// <summary>
        /// Trả về tất cả phân loại thiết bị theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindGeneralOperationalCompliance(SearchOperationalComplianceModel requestModel)
        {
            var result = GroupDataSearchModels(requestModel);
            return HttpResponse.OkList(result);
        }

        /// <summary>
        /// Trả về chi tiết báo cáo tuân thủ hoạt động theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindDetailOperationalCompliance(SearchOperationalComplianceModel requestModel)
        {
            var result = DetailOperationalComplianceModels(requestModel).ToPagedList(requestModel.PageNumber, requestModel.PageSize);
            return HttpResponse.OkList(result);
        }

        /// <summary>
        /// Export Tuẩn thủ hoạt động AM - Tổng hợp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportGeneral(SearchOperationalComplianceModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            try
            {
                var data = GroupDataSearchModels(requestModel);

                // tạo thư mục lưu file xlsx
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx")))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx"));
                }
                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = $"Bao cao tuan thu hoat dong am tong hop_{currentDate}.xlsx";
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx", fileName);

                bool isCreatexlsx = CreateGeneralFile(data, outputFile);
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

        /// <summary>
        /// Export Tuẩn thủ hoạt động AM - Chi tiết
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportDetail(SearchOperationalComplianceModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            try
            {
                var data = DetailOperationalComplianceModels(requestModel);

                // tạo thư mục lưu file xlsx
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx")))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx"));
                }
                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = $"Bao cao tuan thu hoat dong am chi tiet_{currentDate}.xlsx";
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx", fileName);

                bool isCreatexlsx = CreateDetailFile(data, outputFile);
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
        #endregion

        #region Thời gian xử lý bất thường
        /// <summary>
        /// Trả về tổng hợp báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindGeneralTimeHandle(SearchTimeHandleModel requestModel)
        {
            var result = GroupDataGeneralTimeHandleModel(requestModel);
            return HttpResponse.OkList(result);
        }

        /// <summary>
        /// Trả về chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindDetailTimeHandle(SearchTimeHandleModel requestModel)
        {
            // Get tất cả các bảo trì tự quản có bất thường thiết bị đã hoàn thành sửa chữa
            var amEntiies = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                .GetQuery(x => !x.IsDeleted && x.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Finish);

            // Nhà máy
            if (requestModel.FactoryId != CommonConstant.ALL_KEY)
            {
                amEntiies = amEntiies.Where(x => x.FactoryID == requestModel.FactoryId);
            }

            // Năm
            if (requestModel.Year > 1900)
            {
                amEntiies = amEntiies.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                amEntiies = amEntiies.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            DateTime currentDate = DateTime.Now;
            if (currentDate.Year == requestModel.Year)
            {
                amEntiies = amEntiies.Where(x => x.ActualDate.Month <= currentDate.Month);
            }

            // Mức độ khẩn cấp
            if (requestModel.Emergency != CommonConstant.ALL_KEY)
            {
                var amIds = amEntiies.Select(x => x.ID).ToList();
                if (amIds.Count > 0)
                {
                    var reqEntiies = _unitOfWork.ElogbookRepository<AmRequestTicket>()
                    .GetQuery(x => amIds.Contains((int)x.AutonomousMaintenanceID) && x.AmEmergencyId == requestModel.Emergency).ToList();

                    if (reqEntiies.Count > 0)
                    {
                        var reqIds = reqEntiies.Select(x => x.ID).ToList();

                        amEntiies = amEntiies.Where(x => reqIds.Contains(x.AmRequestTicketID.Value));
                    }
                    else
                    {
                        return HttpResponse.OkList(new List<GroupDataDetailTimeHandleModel>(), requestModel.PageNumber, requestModel.PageSize);
                    }
                }
                else
                {
                    return HttpResponse.OkList(new List<GroupDataDetailTimeHandleModel>(), requestModel.PageNumber, requestModel.PageSize);
                }
            }

            var page = amEntiies.Select(
                e => new GroupDataDetailTimeHandleModel()
                {
                    ID = e.ID,
                    DateActivity = e.ActualDate,
                    Factory = e.Factory.FactoryName,
                    OperationPosition = e.OperationPosition.OperationPositionName,
                    Device = new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc },
                    AmMaintenanceSchedule = new ValueLabelCheckedModel { Value = e.AmMaintenanceSchedule.ID, Label = e.AmMaintenanceSchedule.MaintenanceScheduleName },
                    OperationShift = e.OperationShift.OperationShiftName,
                    Status = "Đã tạo PYC",
                    HandleStatus = "Đã hoàn thành sửa chữa",
                    ActualUser = e.LastUpdatedUser.FullName,
                    ActualDate = e.CompleteDate
                })
                .OrderByDescending(t => t.ActualDate)
                .ThenBy(t => t.OperationShift)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// In tài liệu tab tổng quan báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportGeneralTimeHandle(SearchTimeHandleModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            try
            {
                var data = GroupDataGeneralTimeHandleModel(requestModel);

                // tạo thư mục lưu file xlsx
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx")))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx"));
                }
                var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = $"Bao cao tong hop thoi gian xu ly bat thuong_{currentDate}.xlsx";
                var outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx", fileName);

                var isCreatexlsx = CreateGeneralTimeHandleFile(requestModel, data, outputFile);
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

        /// <summary>
        /// In tài liệu tab chi tiết báo cáo thời gian xử lý bất thường
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportDetailTimeHandle(SearchTimeHandleModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            try
            {
                var data = GroupDataDetailTimeHandleModel(requestModel);

                // tạo thư mục lưu file xlsx
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx")))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx"));
                }
                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = $"ChiTietBaoThoiGianXuLyBatThuong_{currentDate}.xlsx";
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx", fileName);

                bool isCreatexlsx = CreateDetailTimeHandleFile(data, outputFile);
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
        #endregion

        #region Phiếu công tác điện - thủy cơ nhiệt hóa

        public HttpResponseMessage PctExport(SearchWorkTicketModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var datas = GetDataPct(requestModel);

                if (datas.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_PCT_DIEN}_{currentDate}.xlsx");
                if (requestModel.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                {
                    outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_PCT_TCNH}_{currentDate}.xlsx");
                }

                bool isCreatexlsx = false;
                if (requestModel.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                {
                    isCreatexlsx = PctTcnhGenerateFile(datas, outputFile);
                }
                else
                {
                    isCreatexlsx = PctDienGenerateFile(datas, outputFile);
                }

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: PrtReportGeneralExport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private List<WorkTicketModel> GetDataPct(SearchWorkTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var workTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.WaitForRegist
                                                                                        && x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.Waiting
                                                                                        && !x.IsDeleted);
            //Phiếu có xảy ra tai nạn, sự cố trong quá trình công tác
            if (requestModel.IsFlagNote)
            {
                workTickets = workTickets.Where(x => x.IsFlagNote == requestModel.IsFlagNote);
            }
            //Nhà máy
            if (requestModel.FactoryID > 0)
            {
                workTickets = workTickets.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            //Ngày duyệt phiếu (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                workTickets = workTickets.Where(x => DbFunctions.TruncateTime(x.ApprovedDate) >= frDate.Date);
            }
            //Ngày duyệt phiếu (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                workTickets = workTickets.Where(x => DbFunctions.TruncateTime(x.ApprovedDate) <= toDate.Date);
            }
            //Đơn vị duyệt phiếu
            if (requestModel.ApprovedUnitID > 0)
            {
                workTickets = workTickets.Where(x => x.ApprovedUnitID == requestModel.ApprovedUnitID);
            }
            //Trạng thái phiếu công tác
            if (requestModel.WorkTicketStatus > 0)
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus == requestModel.WorkTicketStatus);
            }
            //Loại phiếu
            if (requestModel.WorkTicketType > 0)
            {
                workTickets = workTickets.Where(x => x.WorkTicketType == requestModel.WorkTicketType);
            }
            //Số phiếu công tác
            if (!string.IsNullOrWhiteSpace(requestModel.WorkTicketNumber))
            {
                workTickets = workTickets.Where(x => x.WorkTicketNumber.Contains(requestModel.WorkTicketNumber.Trim()));
            }
            //Loại màn hình
            //Mặc định nếu không truyền giá trị filter thì gán tìm kiếm theo PCTĐ
            if (requestModel.ScreenType.GetValueOrDefault(0) < 1)
                requestModel.ScreenType = 1;
            workTickets = workTickets.Where(x => x.ScreenType == requestModel.ScreenType);

            var datas = workTickets.Select(e => new WorkTicketModel()
            {
                ID = e.ID,
                WorkTicketNumber = e.WorkTicketNumber,
                WorkTicketFeature = e.WorkTicketFeature != null ? new ValueLabelModel
                {
                    Value = e.WorkTicketFeature.ID,
                    Label = e.WorkTicketFeature.FeatureName
                } : new ValueLabelModel { Value = 0, Label = null },
                Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                ApproveUnit = new ValueLabelModel { Value = e.ApprovedUnit.ID, Label = e.ApprovedUnit.Name },
                WorkTicketTypeValue = e.WorkTicketType,
                WorkTicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                ApprovedDateLocal = e.ApprovedDate,
                ApprovedDateStyleDatetime = e.ApprovedDate,
                WorkContent = e.WorkContent,
                Note = e.WorkResult,
                ScreenType = e.ScreenType,
                Approver = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Approver && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                Acceptor = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                InsertedAt = e.InsertedAt,
                IsFlagNote = e.IsFlagNote,
            }).ToList();

            var lstId = datas.Select(x => x.ID).ToList();

            var workTicketFeatureLinks = _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>()
                .GetQuery(x => !x.IsDeleted && lstId.Contains(x.WorkTicketID)).Select(e => new WorkTicketFeaturesModel()
                {
                    ID = e.ID,
                    WorkTicketID = e.WorkTicketID,
                    FeatureID = e.FeatureID,
                    Label = e.WorkTicketFeatures.FeatureName,
                }).ToList();

            var workFinishData = _unitOfWork.ElogbookRepository<WorkFinish>()
                .GetQuery(x => !x.IsDeleted && lstId.Contains(x.WorkTicketID)).ToList();

            List<WorkTicketModel> dataResult = new List<WorkTicketModel>();

            if (datas.Count > 0)
            {
                foreach (var item in datas)
                {
                    item.WorkTicketFeatures = workTicketFeatureLinks.FindAll(x => x.WorkTicketID == item.ID);
                    var workFinish = workFinishData.Find(x => x.WorkTicketID == item.ID);
                    if (workFinish != null)
                    {
                        if (item.ScreenType.Value == (int)TicketTypePCT.COTHUYNHIETHOA)
                        {
                            item.CompletedTimeLocal = workFinish.CheckReceivedTime;
                        }
                        else
                        {
                            item.CompletedTimeLocal = workFinish.CheckReceivedTime;
                        }
                    }
                }

                // Mặc định sắp xếp giảm dần theo ngày cấp phiếu
                if (string.IsNullOrWhiteSpace(requestModel.SortColumn))
                {
                    dataResult = datas.OrderByDescending(x => x.ApprovedDateLocal.GetValueOrDefault().Date).ThenBy(t => t.WorkTicketNumber).ToList();
                }

                requestModel.SortColumn = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortColumn.ToUpper();
                requestModel.SortOrder = string.IsNullOrWhiteSpace(requestModel.SortOrder) ? "" : requestModel.SortOrder.ToUpper();

                // Sắp xếp theo Ngày cấp phiếu
                if (requestModel.SortColumn.Equals("CREATEAT"))
                {
                    if ("DESC".Equals(requestModel.SortOrder))
                    {
                        dataResult = datas.OrderByDescending(x => x.ApprovedDateStyleDatetime).ToList();
                    }
                    else
                    {
                        dataResult = datas.OrderBy(x => x.ApprovedDateStyleDatetime).ToList();
                    }
                }

                // Sắp xếp theo Trạng thái
                if (requestModel.SortColumn.Equals("ACTIVE"))
                {
                    if ("DESC".Equals(requestModel.SortOrder))
                    {
                        dataResult = datas.OrderByDescending(x => x.WorkTicketStatus.Value).ToList();
                    }
                    else
                    {
                        dataResult = datas.OrderBy(x => x.WorkTicketStatus.Value).ToList();
                    }
                }

                // Sắp xếp theo Số phiếu công tác
                if (requestModel.SortColumn.Equals("TICKETNO"))
                {
                    if ("DESC".Equals(requestModel.SortOrder))
                    {
                        dataResult = datas.OrderByDescending(x => x.ApprovedDateStyleDatetime).ThenByDescending(x => x.WorkTicketNumber).ToList();
                    }
                    else
                    {
                        dataResult = datas.OrderBy(x => x.ApprovedDateStyleDatetime).ThenBy(x => x.WorkTicketNumber).ToList();
                    }
                }
            }

            return dataResult;
        }

        private bool PctTcnhGenerateFile(List<WorkTicketModel> datas, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("PCT Thủy Cơ Nhiệt Hóa");

                int indexRow = 0;
                int stColBdIndex = 1;
                int edColBdIndex = 36;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 7, 20, title1, "DANH SÁCH PHIẾU CÔNG TÁC THỦY CƠ NHIỆT HÓA");
                SetBoldweight(sheet, indexRow, 7, true, false, 16);

                if (datas.Count > 0)
                {
                    indexRow += 2;//Excel Row Index = 6
                    IRow headerRow2 = sheet.CreateRow(indexRow);
                    headerRow2.CreateCell(1).SetCellValue("STT");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, headerRow2, "Số phiếu công tác");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 5, 6, headerRow2, "Loại phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 7, 8, headerRow2, "Ngày cấp");

                    CreateCellTypeStr(sheet, indexRow, indexRow, 9, 12, headerRow2, "Nội dung");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 13, 14, headerRow2, "Ngày khóa");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, headerRow2, "Kết quả thực hiện");

                    CreateCellTypeStr(sheet, indexRow, indexRow, 19, 22, headerRow2, "Nhà máy");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 23, 26, headerRow2, "Đơn vị cấp phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 27, 29, headerRow2, "Người cấp phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 30, 32, headerRow2, "Người cho phép");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 33, 34, headerRow2, "Lý do");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 35, 36, headerRow2, "Trạng thái");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBdIndex = indexRow;
                    int countStt = 0;
                    foreach (var item in datas)
                    {
                        indexRow += 1;
                        countStt += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        StringBuilder sbLoaiPhieu = new StringBuilder();
                        string strLoaiPhieu = string.Empty;
                        if (item.WorkTicketFeatures != null && item.WorkTicketFeatures.Count > 0)
                        {
                            foreach (var typeW in item.WorkTicketFeatures)
                            {
                                sbLoaiPhieu.Append(typeW.Label);
                                sbLoaiPhieu.Append(";");
                            }

                            strLoaiPhieu = sbLoaiPhieu.ToString();
                            strLoaiPhieu = strLoaiPhieu.Substring(0, strLoaiPhieu.Length - 1);
                        }

                        bodyRow.CreateCell(1).SetCellValue(countStt.ToString());
                        SetAlignmentCennter(sheet, indexRow, 1);

                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, bodyRow, item.WorkTicketNumber);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 5, 6, bodyRow, strLoaiPhieu);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 7, 8, bodyRow, item.ApprovedDate);
                        SetAlignmentCennter(sheet, indexRow, 7);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 9, 12, bodyRow, item.WorkContent);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 13, 14, bodyRow, item.CompletedTimeLocal?.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY));
                        CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, bodyRow, item.Note);

                        CreateCellTypeStr(sheet, indexRow, indexRow, 19, 22, bodyRow, item.Factory?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 23, 26, bodyRow, item.ApproveUnit?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 27, 29, bodyRow, item.Approver?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 30, 32, bodyRow, item.Acceptor?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 33, 34, bodyRow, item.WorkTicketType?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 35, 36, bodyRow, item.WorkTicketStatus?.Label);
                    }
                    int edRowBdIndex = indexRow;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                    SetStyleFinish(sheet, stRowBdIndex, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    IRow noneData = sheet.CreateRow(5); //Excel Row Index = 6
                    noneData.CreateCell(1).SetCellValue("Không tìm thấy dữ liệu tương ứng.");

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                return false;
            }
        }

        private bool PctDienGenerateFile(List<WorkTicketModel> datas, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("PCT Điện");

                int indexRow = 0;
                int stColBdIndex = 1;
                int edColBdIndex = 34;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 7, 20, title1, "DANH SÁCH PHIẾU CÔNG TÁC ĐIỆN");
                SetBoldweight(sheet, indexRow, 7, true, false, 16);

                if (datas.Count > 0)
                {
                    indexRow += 2;//Excel Row Index = 6
                    IRow headerRow2 = sheet.CreateRow(indexRow);
                    headerRow2.CreateCell(1).SetCellValue("STT");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, headerRow2, "Số phiếu công tác");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 5, 6, headerRow2, "Ngày cấp");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 7, 10, headerRow2, "Nội dung");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 11, 12, headerRow2, "Ngày khóa");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 13, 16, headerRow2, "Kết quả thực hiện");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 17, 20, headerRow2, "Nhà máy");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 21, 24, headerRow2, "Đơn vị cấp phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 25, 27, headerRow2, "Người cấp phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 28, 30, headerRow2, "Người cho phép");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 31, 32, headerRow2, "Loại phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 33, 34, headerRow2, "Trạng thái");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBdIndex = indexRow;
                    int countStt = 0;
                    foreach (var item in datas)
                    {
                        indexRow += 1;
                        countStt += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(countStt.ToString());
                        SetAlignmentCennter(sheet, indexRow, 1);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, bodyRow, item.WorkTicketNumber);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 5, 6, bodyRow, item.ApprovedDate);
                        SetAlignmentCennter(sheet, indexRow, 5);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 7, 10, bodyRow, item.WorkContent);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 11, 12, bodyRow, item.CompletedTimeLocal?.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY));
                        CreateCellTypeStr(sheet, indexRow, indexRow, 13, 16, bodyRow, item.Note);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 17, 20, bodyRow, item.Factory?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 21, 24, bodyRow, item.ApproveUnit?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 25, 27, bodyRow, item.Approver?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 28, 30, bodyRow, item.Acceptor?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 31, 32, bodyRow, item.WorkTicketType?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 33, 34, bodyRow, item.WorkTicketStatus?.Label);
                    }
                    int edRowBdIndex = indexRow;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                    SetStyleFinish(sheet, stRowBdIndex, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    IRow noneData = sheet.CreateRow(5); //Excel Row Index = 6
                    noneData.CreateCell(1).SetCellValue("Không tìm thấy dữ liệu tương ứng.");

                    SetStyleFinish(sheet, 1, 3, 7, 24, 61, 58);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                return false;
            }
        }

        #endregion

        #region Phiếu thao tác
        public HttpResponseMessage PttExport(SearchTicketWebModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetDataPtt(requestModel);

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, $"{CommonConstant.FILE_NAME_PTT}_{currentDate}.xlsx");

                bool isCreatexlsx = PttGenerateFile(rootData, outputFile);

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: PrtReportGeneralExport", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private bool PttGenerateFile(List<TicketSearchWebModel> rootData, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Phiếu thao tác");

                int indexRow = 0;
                int stColBdIndex = 1;
                int edColBdIndex = 22;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 7, 20, title1, "DANH SÁCH PHIẾU THAO TÁC");
                SetBoldweight(sheet, indexRow, 7, true, false, 16);

                if (rootData.Count > 0)
                {
                    indexRow += 2;//Excel Row Index = 6
                    IRow headerRow2 = sheet.CreateRow(indexRow);
                    headerRow2.CreateCell(1).SetCellValue("STT");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, headerRow2, "Số phiếu thao tác");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, headerRow2, "Tên phiếu thao tác");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, headerRow2, "Ngày cấp");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 10, 13, headerRow2, "Nhà máy");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 14, 17, headerRow2, "Đơn vị cấp phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 18, 20, headerRow2, "Loại phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 21, 22, headerRow2, "Trạng thái");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBdIndex = indexRow;
                    int countStt = 0;
                    foreach (var item in rootData)
                    {
                        indexRow += 1;
                        countStt += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(countStt.ToString());
                        SetAlignmentCennter(sheet, indexRow, 1);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, bodyRow, item.TicketNumber);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, bodyRow, item.TicketName);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, bodyRow, item.ApprovedDate);
                        SetAlignmentCennter(sheet, indexRow, 8);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 10, 13, bodyRow, item.Factory.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 14, 17, bodyRow, item.ApproveUnit.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 18, 20, bodyRow, item.TicketType.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 21, 22, bodyRow, item.TicketStatus.Label);
                    }
                    int edRowBdIndex = indexRow;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                    SetStyleFinish(sheet, stRowBdIndex, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }
                else
                {
                    IRow noneData = sheet.CreateRow(5); //Excel Row Index = 6
                    noneData.CreateCell(1).SetCellValue("Không tìm thấy dữ liệu tương ứng.");

                    SetStyleFinish(sheet, 1, 3, 7, 22, 61, 58);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: PttGenerateFile", ex.ToString());
                return false;
            }
        }

        private List<TicketSearchWebModel> GetDataPtt(SearchTicketWebModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var entities = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm kiếm theo nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm kiếm theo loại phiếu
            if (requestModel.TicketType != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.TicketType == requestModel.TicketType);
            }

            // Tìm kiếm theo đơn vị cấp phiếu
            if (requestModel.ApproveUnitID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ApproveUnitID == requestModel.ApproveUnitID);
            }

            // Tìm kiếm theo trạng thái phiếu
            if (requestModel.TicketStatusID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.TicketStatusValue == requestModel.TicketStatusID);
            }

            // Tìm kiếm theo ngày bắt đầu
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= frDate.Date);
            }

            // Tìm kiếm theo ngày kết thúc
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <= toDate.Date);
            }

            // Tìm kiếm theo key
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.TicketName.Contains(requestModel.SearchText)
                                               || x.TicketNumber.Contains(requestModel.SearchText)
                                               || x.CreatedUserTicket.Contains(requestModel.SearchText)
                                               || x.ApprovedUserTicket.Contains(requestModel.SearchText));
            }

            // Return and paging data
            var rsData = entities.Select(e => new TicketSearchWebModel()
            {
                ID = e.ID,
                TicketNumber = e.TicketNumber,
                TicketName = e.TicketName,
                Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                TicketType = new ValueLabelModel { Value = e.TicketType, Label = e.TicketType == 1 ? "Kế hoạch" : "Đột xuất" },
                Company = e.ApproveUnit.Parent != null ? new ValueLabelModel { Value = e.ApproveUnit.Parent.ID, Label = e.ApproveUnit.Parent.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                ApproveUnit = new ValueLabelModel { Value = e.ApproveUnit.ID, Label = e.ApproveUnit.Name },
                CreatedDate = e.InsertedAt,
            }).OrderByDescending(t => t.CreatedDate).ToList();

            return rsData;
        }
        #endregion

        #region Gửi mail định kỳ
        public int SendMailReport()
        {
            List<string> fileAttchments = CreateReportFile();
            if (fileAttchments.Count > 0)
            {
                ProcessMailAndNotification(fileAttchments);
                return 1;
            }
            return -1;
        }
        #endregion

        #endregion

        #region Private methor
        #region Thời gian xử lý bất thường
        private List<GroupDataGeneralTimeHandleModel> GroupDataGeneralTimeHandleModel(SearchTimeHandleModel requestModel)
        {
            // Get tất cả các bảo trì tự quản có bất thường thiết bị đã hoàn thành sửa chữa
            var amEntiies = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                .GetQuery(x => !x.IsDeleted && x.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Finish);

            // Tìm theo 1 Nhà máy
            if (requestModel.FactoryId != CommonConstant.ALL_KEY)
            {
                amEntiies = amEntiies.Where(x => x.FactoryID == requestModel.FactoryId);
            }

            // Năm
            if (requestModel.Year > 1900)
            {
                amEntiies = amEntiies.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                amEntiies = amEntiies.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            DateTime currentDate = DateTime.Now;

            amEntiies = amEntiies.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= currentDate);

            var amIds = amEntiies.Select(x => x.ID).ToList();

            if (amIds.Count <= 0)
            {
                return new List<GroupDataGeneralTimeHandleModel>();
            }
            // Get tất cả PYC được tạo theo bảo trì tự quản
            var amReqTickets = _unitOfWork.ElogbookRepository<AmRequestTicket>()
                .GetQuery(x => amIds.Contains((int)x.AutonomousMaintenanceID) && !x.IsDeleted)
                .Select(e => new AmRequestTicketModel()
                {
                    ID = e.ID,
                    AmEmergencyID = e.AmEmergencyId ?? 0,
                    InsertedAt = e.InsertedAt
                }).ToList();

            if (amReqTickets.Count <= 0)
            {
                return new List<GroupDataGeneralTimeHandleModel>();
            }

            var amEmeIds = amReqTickets.Select(x => x.AmEmergencyID).Distinct().ToList();
            var amEmeEntities = _unitOfWork.ElogbookRepository<AmEmergency>().GetQuery(x => amEmeIds.Contains(x.ID) && !x.IsDeleted).Select(
                e => new ValueLabelCheckedModel() { Value = e.ID, Label = e.Name }).ToList();

            var rqTicketIds = amReqTickets.Select(x => x.ID).ToList();
            var wtEntities = _unitOfWork.ElogbookRepository<WorkTicket>()
                .GetQuery(x => rqTicketIds.Contains((int)x.RequestTicketID) && x.WorkTicketStatus == (int)EnumTicketStatus.Completed)
                .Select(e => new WorkTicketModel()
                {
                    ID = e.ID,
                    UpdatedAt = e.UpdatedAt,
                    RequestTicketID = e.RequestTicketID,
                    WorkTicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                }).ToList();

            if (wtEntities.Count <= 0)
            {
                return new List<GroupDataGeneralTimeHandleModel>();
            }

            List<GroupDataGeneralTimeHandleModel> resultFinal = new List<GroupDataGeneralTimeHandleModel>();

            if (amEmeIds.Count > 0)
            {
                foreach (var idEm in amEmeIds)
                {
                    var newData = new GroupDataGeneralTimeHandleModel
                    {
                        Emergency = amEmeEntities.Find(x => x.Value == idEm),
                        TotalEmergency = amReqTickets.Count(x => x.AmEmergencyID == idEm),
                        Month = requestModel.Month,
                        FactoryID = requestModel.FactoryId
                    };

                    var reqIds = amReqTickets.FindAll(x => x.AmEmergencyID == idEm);
                    int totalDays = 0;

                    foreach (var item in reqIds)
                    {
                        var wtEntity = wtEntities.Find(x => x.RequestTicketID == item.ID);

                        if (wtEntity != null)
                        {
                            DateTime wtDatetime = wtEntity.UpdatedAt;

                            var diffOfDay = wtDatetime.Date - item.InsertedAt.Date;

                            totalDays += (diffOfDay.Days + 1);
                        }
                    }

                    newData.TotalDate = totalDays;
                    newData.AverageTime = Math.Round((double)totalDays / newData.TotalEmergency, 2);

                    resultFinal.Add(newData);
                }

                var totalData = new GroupDataGeneralTimeHandleModel
                {
                    Emergency = new ValueLabelCheckedModel { Value = 0, Label = "Tổng" },
                    TotalEmergency = resultFinal.Sum(x => x.TotalEmergency),
                    TotalDate = resultFinal.Sum(x => x.TotalDate)
                };
                totalData.AverageTime = Math.Round((double)totalData.TotalDate / totalData.TotalEmergency, 2);

                resultFinal.Add(totalData);
            }

            return resultFinal;
        }

        private List<GroupDataDetailTimeHandleModel> GroupDataDetailTimeHandleModel(SearchTimeHandleModel requestModel)
        {
            var requestIds = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => !x.IsDeleted && x.WorkTicketStatus == (int)Core.Enums.EnumTicketStatus.Completed &&
            (x.RegistrationTicketID.HasValue && x.RegistrationTicketID.Value > 0 && x.RegistrationTicket.RequestTicketID.HasValue && x.RegistrationTicket.RequestTicketID.Value > 0) ||
            x.RequestTicketID > 0).Select(x => x.RequestTicketID).Distinct().ToList();

            var pyc = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => requestIds.Contains(x.ID));
            if (requestModel.Emergency > 0)
            {
                pyc = pyc.Where(x => x.AmEmergencyId == requestModel.Emergency);
            }

            var listIdRequestTicket = pyc.Select(x => x.ID).Distinct().ToList();

            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.AmRequestTicketID.HasValue &&
            x.AmRequestTicketID.Value > 0 && listIdRequestTicket.Contains(x.AmRequestTicketID.Value) &&
            x.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Finish);

            // Nhà máy
            if (requestModel.FactoryId > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryId);
            }

            // năm
            if (requestModel.Year > 0)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // tháng
            if (requestModel.Month > 0)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            List<GroupDataDetailTimeHandleModel> result = new List<GroupDataDetailTimeHandleModel>();

            foreach (var item in entities)
            {
                result.Add(new GroupDataDetailTimeHandleModel
                {
                    ID = item.ID,
                    DateActivity = item.ActualDate,
                    Factory = item.Factory.FactoryName,
                    OperationPosition = item.OperationPosition.OperationPositionName,
                    Device = new ValueLabelCheckedModel { Value = item.AmDevice.ID, Label = item.AmDevice.AssetDesc },
                    AmMaintenanceSchedule = new ValueLabelCheckedModel { Value = item.AmMaintenanceSchedule.ID, Label = item.AmMaintenanceSchedule.MaintenanceScheduleName },
                    OperationShift = item.OperationShift.OperationShiftName,
                    Status = "Đã tạo PYC",
                    HandleStatus = "Đã hoàn thành sửa chữa",
                    ActualUser = item.CreatedUser.FullName,
                    ActualDate = item.CompleteDate,

                });
            }

            return result;
        }

        private bool CreateGeneralTimeHandleFile(SearchTimeHandleModel requestModel, List<GroupDataGeneralTimeHandleModel> data, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Thời gian xử lý bất thường");
                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 11, title, "BÁO CÁO TỔNG HỢP THỜI GIAN XỬ LÝ BẤT THƯỜNG");
                SetBoldweight(sheet, indexRow, 4, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                var factoryName = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == requestModel.FactoryId).Select(t => t.FactoryName).FirstOrDefault();
                CreateCellTypeStr(sheet, indexRow, indexRow, 3, 12, title2, $"Dữ liệu năm {requestModel.Year} - Tháng {requestModel.Month} - {factoryName}");
                SetBoldweight(sheet, indexRow, 3, true, true, 11);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 1, 2, headerRow, "Mức độ khẩn cấp");
                CreateCellTypeStr(sheet, indexRow, indexRow, 3, 7, headerRow, "Tổng số bất thường đã hoàn thành sửa chữa");
                CreateCellTypeStr(sheet, indexRow, indexRow, 8, 14, headerRow, "Tổng số ngày xử lý (Từ ngày tạo PYC - ngày hoàn thành PYC)");
                CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, headerRow, "Thời gian trung bình (ngày/bất thường)");
                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, 1, 18), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;
                foreach (var item in data)
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);

                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 2, bodyRow, item.Emergency.Label);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 3, 7, bodyRow, item.TotalEmergency.ToString());
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 14, bodyRow, item.TotalDate.ToString());
                    CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, bodyRow, item.AverageTime.ToString());
                }
                int edRowBdIndex = indexRow;
                int edColBdIndex = 18;

                SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                SetStyleFinish(sheet, stRowBdIndex, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                return false;
            }
        }

        private bool CreateDetailTimeHandleFile(List<GroupDataDetailTimeHandleModel> data, string outputFile)
        {
            try
            {
                excel.WorkbookCreate();
                excel.Worksheet_SelectByIndex(0);

                //create header
                excel.Cell_WriteByIndex(1, 1, "STT");
                excel.Cell_WriteByIndex(1, 2, "Ngày có hoạt động");
                excel.Cell_WriteByIndex(1, 3, "nhà máy");
                excel.Cell_WriteByIndex(1, 4, "Cương vị");
                excel.Cell_WriteByIndex(1, 5, "Thiết bị");
                excel.Cell_WriteByIndex(1, 6, "Lịch công tác");
                excel.Cell_WriteByIndex(1, 7, "Ca");
                excel.Cell_WriteByIndex(1, 8, "Trạng thái");
                excel.Cell_WriteByIndex(1, 9, "Trạng thái xử lý");
                excel.Cell_WriteByIndex(1, 10, "Người thực hiện");
                excel.Cell_WriteByIndex(1, 11, "Ngày thực hiện");

                //tạo datatable
                var dt = new DataTable();

                //add column
                for (int i = 1; i <= 11; i++)
                {
                    dt.Columns.Add("");
                }

                //add row
                for (int i = 1; i <= data.Count; i++)
                {
                    dt.Rows.Add(i, data[i - 1].DateActivity.ToString("yyyy/MM/dd"),
                        data[i - 1].Factory,
                        data[i - 1].OperationPosition,
                        data[i - 1].Device.Label,
                        data[i - 1].AmMaintenanceSchedule.Label,
                        data[i - 1].OperationShift,
                        data[i - 1].Status,
                        data[i - 1].HandleStatus,
                        data[i - 1].ActualUser,
                        data[i - 1].ActualDate);
                }

                // write data table to sheet
                excel.Cell_WriteRange(2, 1, dt, false);

                //save
                excel.Workbook_Save(outputFile);
                excel.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<GroupDataGeneralTimeHandleModel> GetDataGeneralTimeHandle()
        {
            // Get tất cả các bảo trì tự quản có bất thường thiết bị đã hoàn thành sửa chữa
            var amEntiies = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                .GetQuery(x => !x.IsDeleted && x.AmDeviceAbnormalStatusID == (int)AmEnumDeAbStatus.Yes_Finish);

            DateTime currentDate = DateTime.Now;
            DateTime bef1Date = currentDate.AddDays(-1);
            DateTime bef7Date = bef1Date.AddDays(-6);
            amEntiies = amEntiies.Where(x => DbFunctions.TruncateTime(x.ActualDate) >= bef7Date.Date);
            amEntiies = amEntiies.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= bef1Date.Date);

            var rootData = amEntiies.ToList();
            var factories = rootData.Select(x => x.FactoryID).Distinct().ToList();
            List<GroupDataGeneralTimeHandleModel> resultFinal = new List<GroupDataGeneralTimeHandleModel>();

            if (factories.Count > 0)
            {
                foreach (var facId in factories)
                {
                    amEntiies = amEntiies.Where(x => x.FactoryID == facId);
                    var amIds = amEntiies.Select(x => x.ID).ToList();

                    // Get tất cả PYC được tạo theo bảo trì tự quản
                    var amReqTickets = _unitOfWork.ElogbookRepository<AmRequestTicket>()
                        .GetQuery(x => amIds.Contains((int)x.AutonomousMaintenanceID) && !x.IsDeleted)
                        .Select(e => new AmRequestTicketModel()
                        {
                            ID = e.ID,
                            AmEmergencyID = e.AmEmergencyId ?? 0,
                            InsertedAt = e.InsertedAt
                        }).ToList();

                    if (amReqTickets.Count <= 0)
                    {
                        continue;
                    }

                    var amEmeIds = amReqTickets.Select(x => x.AmEmergencyID).Distinct().ToList();
                    var amEmeEntities = _unitOfWork.ElogbookRepository<AmEmergency>().GetQuery(x => amEmeIds.Contains(x.ID) && !x.IsDeleted).Select(
                        e => new ValueLabelCheckedModel() { Value = e.ID, Label = e.Name }).ToList();
                    if (amEmeEntities.Count <= 0)
                    {
                        continue;
                    }
                    var rqTicketIds = amReqTickets.Select(x => x.ID).ToList();
                    var wtEntities = _unitOfWork.ElogbookRepository<WorkTicket>()
                        .GetQuery(x => rqTicketIds.Contains((int)x.RequestTicketID) && x.WorkTicketStatus == (int)EnumTicketStatus.Completed)
                        .Select(e => new WorkTicketModel()
                        {
                            ID = e.ID,
                            UpdatedAt = e.UpdatedAt,
                            RequestTicketID = e.RequestTicketID,
                            WorkTicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                        }).ToList();

                    if (amEmeIds.Count > 0)
                    {
                        foreach (var idEm in amEmeIds)
                        {
                            var newData = new GroupDataGeneralTimeHandleModel
                            {
                                Emergency = amEmeEntities.Find(x => x.Value == idEm),
                                TotalEmergency = amReqTickets.Count(x => x.AmEmergencyID == idEm),
                                FactoryID = facId
                            };

                            var reqIds = amReqTickets.FindAll(x => x.AmEmergencyID == idEm);
                            int totalDays = 0;

                            foreach (var item in reqIds)
                            {
                                var wtEntity = wtEntities.Find(x => x.RequestTicketID == item.ID);

                                if (wtEntity != null)
                                {
                                    DateTime wtDatetime = wtEntity.UpdatedAt;

                                    var diffOfDay = wtDatetime.Date - item.InsertedAt.Date;

                                    totalDays += (diffOfDay.Days + 1);
                                }
                            }

                            newData.TotalDate = totalDays;
                            newData.AverageTime = Math.Round((double)totalDays / newData.TotalEmergency, 2);

                            resultFinal.Add(newData);
                        }

                        var totalData = new GroupDataGeneralTimeHandleModel
                        {
                            Emergency = new ValueLabelCheckedModel { Value = 0, Label = "Tổng" },
                            TotalEmergency = resultFinal.Sum(x => x.TotalEmergency),
                            TotalDate = resultFinal.Sum(x => x.TotalDate)
                        };
                        totalData.AverageTime = Math.Round((double)totalData.TotalDate / totalData.TotalEmergency, 2);

                        resultFinal.Add(totalData);
                    }
                }
            }

            return resultFinal;
        }

        private bool CreateGeneralTimeHandle(List<GroupDataGeneralTimeHandleModel> dataInput, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                var lstFacs = dataInput.Select(x => x.FactoryID).Distinct().ToList();

                foreach (var facId in lstFacs)
                {
                    var factoryName = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == facId).Select(t => t.FactoryName).FirstOrDefault();

                    var dataByFactory = dataInput.FindAll(x => x.FactoryID == facId).ToList();

                    ISheet sheet = workbook.CreateSheet(factoryName);
                    int indexRow = 0;

                    // Title
                    indexRow += 1;//Excel Row Index = 2
                    IRow title = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 11, title, "BÁO CÁO TỔNG HỢP THỜI GIAN XỬ LÝ BẤT THƯỜNG");
                    SetBoldweight(sheet, indexRow, 4, true, false, 16);

                    indexRow += 2;//Excel Row Index = 4
                    IRow title2 = sheet.CreateRow(indexRow);

                    CreateCellTypeStr(sheet, indexRow, indexRow, 3, 12, title2, $"Dữ liệu từ ngày {DateTime.Now.AddDays(-7):dd/MM/yyyy} - {DateTime.Now.AddDays(-1):dd/MM/yyyy} - {factoryName}");
                    SetBoldweight(sheet, indexRow, 3, true, true, 11);

                    indexRow += 2;//Excel Row Index = 6
                    IRow headerRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 2, headerRow, "Mức độ khẩn cấp");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 3, 7, headerRow, "Tổng số bất thường đã hoàn thành sửa chữa");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 14, headerRow, "Tổng số ngày xử lý (Từ ngày tạo PYC - ngày hoàn thành PYC)");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, headerRow, "Thời gian trung bình (ngày/bất thường)");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, 1, 18), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBdIndex = indexRow;
                    int stColBdIndex = 1;
                    foreach (var item in dataByFactory)
                    {
                        indexRow += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        CreateCellTypeStr(sheet, indexRow, indexRow, 1, 2, bodyRow, item.Emergency.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 3, 7, bodyRow, item.TotalEmergency.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 8, 14, bodyRow, item.TotalDate.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, bodyRow, item.AverageTime.ToString());
                    }
                    int edRowBdIndex = indexRow;
                    int edColBdIndex = 18;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                    SetStyleFinish(sheet, stRowBdIndex, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                }

                if (dataInput.Count == 0)
                {
                    ISheet sheet = workbook.CreateSheet("Sheet1");
                    int indexRow = 0;

                    // Title
                    indexRow += 1;//Excel Row Index = 2
                    IRow title = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 11, title, "BÁO CÁO TỔNG HỢP THỜI GIAN XỬ LÝ BẤT THƯỜNG");
                    SetBoldweight(sheet, indexRow, 4, true, false, 16);

                    indexRow += 2;//Excel Row Index = 4
                    IRow title2 = sheet.CreateRow(indexRow);

                    CreateCellTypeStr(sheet, indexRow, indexRow, 3, 12, title2, $"Dữ liệu từ ngày {DateTime.Now.AddDays(-7):dd/MM/yyyy} - {DateTime.Now.AddDays(-1):dd/MM/yyyy}");
                    SetBoldweight(sheet, indexRow, 3, true, true, 11);

                    indexRow += 2;//Excel Row Index = 6
                    IRow headerRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 2, headerRow, "Mức độ khẩn cấp");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 3, 7, headerRow, "Tổng số bất thường đã hoàn thành sửa chữa");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 14, headerRow, "Tổng số ngày xử lý (Từ ngày tạo PYC - ngày hoàn thành PYC)");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 15, 18, headerRow, "Thời gian trung bình (ngày/bất thường)");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, 1, 18), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                return false;
            }
        }
        #endregion

        #region Tuân thủ hoạt động
        private List<GroupDataSearchModel> GroupDataSearchModels(SearchOperationalComplianceModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => facs.Contains(x.FactoryID));
            }

            // Nhà máy
            if (requestModel.FactoryId != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryId);
            }

            // năm
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var mons = requestModel.Months.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => mons.Contains(x.ActualDate.Month));
            }

            // tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            DateTime currentDate = DateTime.Now;
            entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= currentDate);

            if (requestModel.IsMailReport)
            {
                DateTime bef1Date = currentDate.AddDays(-1);
                DateTime bef7Date = bef1Date.AddDays(-6);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) >= bef7Date.Date);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= bef1Date.Date);
            }

            // Return data
            var reneral = entities.Select(x => new GeneralOperationalComplianceModel
            {
                Month = x.ActualDate.Month,
                Factory = new ComboboxDataModel { Value = x.Factory.ID, Label = x.Factory.FactoryName },
                Status = new ValueLabelModel { Value = x.AmStatus.ID, Label = x.AmStatus.StatusName }
            });
            var factories = reneral.GroupBy(x => x.Factory).Select(s => s.Key).ToList();
            var months = reneral.Select(x => x.Month).Distinct();

            List<GroupDataSearchModel> result = new List<GroupDataSearchModel>();

            foreach (var fac in factories)
            {
                foreach (var month in months)
                {
                    var data = reneral.Where(x => x.Factory.Value == fac.Value && x.Month == month).ToList();
                    //count
                    var group = new GroupDataSearchModel
                    {
                        Factory = fac,
                        FactoryID = fac.Value,
                        Month = month,
                        DoneMadeStatus = data.Count(x => x.Status.Value.HasValue && x.Status.Value.Value == (int)AmEnumStatus.Completed),
                        DoNotMadeStatus = data.Count(x => x.Status.Value.HasValue && x.Status.Value.Value == (int)AmEnumStatus.DoNotWork),
                        UnfulfilledStatus = data.Count(x => x.Status.Value.HasValue && x.Status.Value.Value == (int)AmEnumStatus.Waiting),
                        InprogressStatus = data.Count(x => x.Status.Value.HasValue && x.Status.Value.Value == (int)AmEnumStatus.OnGoing)
                    };
                    group.TotalStatus = group.DoneMadeStatus + group.DoNotMadeStatus + group.UnfulfilledStatus + group.InprogressStatus;
                    result.Add(group);
                }
            }

            var total = new List<GroupDataSearchModel>();
            // add total row
            foreach (var month in months)
            {
                total.Add(new GroupDataSearchModel
                {
                    Factory = new ComboboxDataModel { Value = 0, Label = "Tổng" },
                    FactoryID = 0,
                    Month = month,
                    TotalStatus = result.Where(x => x.Month == month).Sum(x => x.TotalStatus),
                    DoneMadeStatus = result.Where(x => x.Month == month).Sum(x => x.DoneMadeStatus),
                    DoNotMadeStatus = result.Where(x => x.Month == month).Sum(x => x.DoNotMadeStatus),
                    UnfulfilledStatus = result.Where(x => x.Month == month).Sum(x => x.UnfulfilledStatus),
                    InprogressStatus = result.Where(x => x.Month == month).Sum(x => x.InprogressStatus)
                });
            }
            result.AddRange(total);

            return result;
        }

        private bool CreateGeneralFile(List<GroupDataSearchModel> groupDataSearchModels, string outputFile)
        {
            try
            {
                excel.WorkbookCreate();
                excel.Worksheet_SelectByIndex(0);

                #region header
                //create header
                excel.Worksheet.Cells["A1:A2"].Merge = true;
                excel.Cell_WriteByIndex(1, 1, "Nhà máy");

                List<int> months = groupDataSearchModels.Select(x => x.Month).Distinct().ToList();
                for (int i = 1; i <= months.Count; i++)
                {
                    excel.Worksheet.Cells[1, 2 + (i - 1) * 5, 1, 6 + (i - 1) * 5].Merge = true;
                    excel.Cell_WriteByIndex(1, 2 + (i - 1) * 5, "Tháng " + months[i - 1]);

                    excel.Cell_WriteByIndex(2, 2 + (i - 1) * 5, "Tổng hoạt động");
                    excel.Cell_WriteByIndex(2, 3 + (i - 1) * 5, "Đã thực hiện");
                    excel.Cell_WriteByIndex(2, 4 + (i - 1) * 5, "Không thực hiện");
                    excel.Cell_WriteByIndex(2, 5 + (i - 1) * 5, "Chưa thực hiện");
                    excel.Cell_WriteByIndex(2, 6 + (i - 1) * 5, "Đang thực hiện");
                }

                // column tổng
                excel.Worksheet.Cells[1, 7 + (months.Count - 1) * 5, 2, 7 + (months.Count - 1) * 5].Merge = true;
                excel.Cell_WriteByIndex(1, 7 + (months.Count - 1) * 5, "Tổng");
                #endregion

                //create datatable
                List<DataTable> dataTables = new List<DataTable>();
                for (int i = 0; i <= months.Count + 1; i++)
                {
                    dataTables.Add(new DataTable());
                }

                var factory = groupDataSearchModels.Select(x => x.Factory.Label).Distinct().ToList();
                for (int i = 0; i < factory.Count; i++)
                {
                    dataTables[0].Columns.Add("");
                    dataTables[0].Rows.Add(factory[i]);
                    for (int mon = 1; mon <= months.Count; mon++)
                    {
                        var row = groupDataSearchModels.Where(x => x.Factory.Label == factory[i] && x.Month == months[mon - 1]);

                        dataTables[mon].Columns.Add("");
                        dataTables[mon].Columns.Add("");
                        dataTables[mon].Columns.Add("");
                        dataTables[mon].Columns.Add("");
                        dataTables[mon].Columns.Add("");

                        if (row.Any())
                        {
                            dataTables[mon].Rows.Add(row.First().TotalStatus, row.First().DoneMadeStatus, row.First().DoNotMadeStatus, row.First().UnfulfilledStatus, row.First().InprogressStatus);
                        }
                    }
                    if (dataTables[months.Count + 1].Columns.Count == 0)
                    {
                        dataTables[months.Count + 1].Columns.Add("");
                    }
                    dataTables[months.Count + 1].Rows.Add(groupDataSearchModels.Where(x => x.Factory.Label == factory[i]).Sum(x => x.TotalStatus));
                }

                // write data table to sheet
                excel.Cell_WriteRange(3, 1, dataTables[0], false);
                for (int i = 1; i <= months.Count; i++)
                {
                    excel.Cell_WriteRangeNumberValue(3, 2 + 5 * (i - 1), dataTables[i], false);
                }
                excel.Cell_WriteRangeNumberValue(3, 2 + 5 * months.Count, dataTables[months.Count + 1], false);

                //save
                excel.Workbook_Save(outputFile);
                excel.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CreateDetailFile(List<DetailOperationalComplianceModel> data, string outputFile)
        {
            try
            {
                excel.WorkbookCreate();
                excel.Worksheet_SelectByIndex(0);

                //create header
                excel.Cell_WriteByIndex(1, 1, "STT");
                excel.Cell_WriteByIndex(1, 2, "Ngày có hoạt động");
                excel.Cell_WriteByIndex(1, 3, "Nhà máy");
                excel.Cell_WriteByIndex(1, 4, "Cương vị");
                excel.Cell_WriteByIndex(1, 5, "Thiết bị");
                excel.Cell_WriteByIndex(1, 6, "Lịch công tác");
                excel.Cell_WriteByIndex(1, 7, "Ca");
                excel.Cell_WriteByIndex(1, 8, "Trạng thái");
                excel.Cell_WriteByIndex(1, 9, "Thông tin thêm");
                excel.Cell_WriteByIndex(1, 10, "Người thực hiện");
                excel.Cell_WriteByIndex(1, 11, "Ngày thực hiện");
                excel.Cell_WriteByIndex(1, 12, "Tình trạng công việc Tổng/Hoàn thành/Dừng/Fail SLA");

                // list data print
                List<ExportExcelDetailModel> exportExcelDetailModels = new List<ExportExcelDetailModel>();
                for (int i = 1; i <= data.Count; i++)
                {
                    exportExcelDetailModels.Add(new ExportExcelDetailModel
                    {
                        STT = i,
                        ActualDate = data[i - 1].ActualDate.ToString("yyyy/MM/dd"),
                        Factory = data[i - 1].Factory?.Label,
                        OperationPosition = data[i - 1].OperationPosition?.Label,
                        Device = data[i - 1].Device?.Label,
                        MaintenanceSchedule = data[i - 1].MaintenanceSchedule?.Label,
                        OperationShift = data[i - 1].OperationShift?.Label,
                        Status = data[i - 1].Status?.Status,
                        DeviceAbnormalStatus = data[i - 1].DeviceAbnormalStatus?.Label,
                        CompleterUser = data[i - 1].CompleterUser?.Label,
                        CompleteDate = data[i - 1].CompleteDate?.ToString("yyyy/MM/dd"),
                        WorkStatus = data[i - 1].WorkStatus
                    });
                }

                // write data table to sheet
                excel.Cell_WriteRange(2, 1, exportExcelDetailModels, false);

                //save
                excel.Workbook_Save(outputFile);
                excel.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<DetailOperationalComplianceModel> DetailOperationalComplianceModels(SearchOperationalComplianceModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryId != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryId);
            }

            // năm
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.ActualDate.Year == requestModel.Year);
            }

            // tháng
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ActualDate.Month == requestModel.Month);
            }

            // trạng thái
            if (requestModel.Status != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmStatusID == requestModel.Status);
            }

            DateTime currentDate = DateTime.Now;
            entities = entities.Where(x => x.ActualDate <= currentDate);

            var page = entities.Select(
                e => new DetailOperationalComplianceModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    MaintenanceSchedule = new ValueLabelCheckedModel() { Value = e.AmMaintenanceSchedule.ID, Label = e.AmMaintenanceSchedule.MaintenanceScheduleName },
                    OperationPosition = new ValueLabelCheckedModel() { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    OperationShift = new ValueLabelCheckedModel() { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                    Status = new AmStatusInfoModel { ID = e.AmStatus.ID, Status = e.AmStatus.StatusName },
                    Device = new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc },
                    DeviceAbnormalStatus = e.AmDeviceAbnormalStatusID.HasValue ? new ValueLabelCheckedModel { Value = e.AmDeviceAbnormalStatus.ID, Label = e.AmDeviceAbnormalStatus.Name } : null,
                    ActualDate = e.ActualDate,
                    StatusID = e.AmStatusID,
                    InsertedAt = e.InsertedAt,
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
                        Order = t.Order,
                        SLA = t.SLA,
                        Description = t.Description,
                        StickTime = t.StickTime,
                        UpdatedAt = t.UpdatedAt,
                        Implementer = t.ImplementerID != null ? new ValueLabelCheckedModel() { Value = t.Implementer.ID, Label = t.Implementer.FullName } : null
                    }).ToList(),
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToList();

            foreach (var item in page)
            {
                item.WorkStatus = GetWorkStatus(item.WorkDetails);

                if (item.StatusID != (int)AmEnumStatus.Waiting)
                {
                    if (item.CompleterUser == null)
                    {
                        item.CompleterUser = item.WorkDetails.Where(x => x.StatusID != (int)AmEnumStatus.Waiting).Select(x => x.Implementer).FirstOrDefault();
                    }

                    if (item.CompleteDate == null)
                    {
                        var dataGet = item.WorkDetails.Where(x => x.StatusID != (int)AmEnumStatus.Waiting).Select(x => x.Implementer).FirstOrDefault();
                        if (dataGet != null)
                        {
                            item.CompleteDate = item.WorkDetails.Where(x => x.StatusID != (int)AmEnumStatus.Waiting).Select(x => x.UpdatedAt).FirstOrDefault();
                        }
                    }

                    item.ReasonStopInfo = GetAmStop(item.ID);
                }
            }

            return page;
        }

        private string GetWorkStatus(List<AmWorkDetailModel> workDetails)
        {
            var complete = workDetails.Count(x => x.Status.ID == (int)AmEnumStatus.Completed);
            var stop = workDetails.Count(x => x.Status.ID == (int)AmEnumStatus.Stop);
            var workDetailModels = workDetails.Where(x => x.Status.ID == (int)AmEnumStatus.Completed || x.Status.ID == (int)AmEnumStatus.Stop).ToList();
            int sla = 0;
            foreach (var item in workDetailModels)
            {
                if (item.StickTime != null && item.SLA.HasValue)
                {
                    var splipTime = item.StickTime.Split(':');
                    if (Convert.ToInt32(splipTime[0]) > item.SLA.Value)
                    {
                        sla += 1;
                    }
                }
            }

            return workDetails.Count + "/" + complete + "/" + stop + "/" + sla;
        }

        private ValueLabelCheckedModel GetAmStop(int amId)
        {
            var amStop = _unitOfWork.ElogbookRepository<AmStopMaintenance>().GetQuery(x => x.AutonomousMaintenanceID == amId).FirstOrDefault();

            if (amStop != null)
            {
                return new ValueLabelCheckedModel { Value = amStop.AmReasonForStop.ID, Label = amStop.AmReasonForStop.ReasonDesc };
            }
            return null;
        }

        private bool CreateGeneralActiveAmFile(List<GroupDataSearchModel> groupDataSearchModels, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                // Title
                IRow title1 = sheet.CreateRow(1); //Excel Row Index = 2
                CreateCellTypeStr(sheet, 1, 2, 4, 11, title1, "BÁO CÁO TỔNG HỢP TUÂN THỦ HOẠT ĐỘNG AM");
                SetBoldweight(sheet, 1, 4, true, false, 16);

                IRow title2 = sheet.CreateRow(3); //Excel Row Index = 4
                CreateCellTypeStr(sheet, 3, 3, 4, 11, title2, $"Dữ liệu từ ngày {DateTime.Now.AddDays(-7):dd/MM/yyyy} - {DateTime.Now.AddDays(-1):dd/MM/yyyy}");
                SetBoldweight(sheet, 3, 4, true, true, 11);

                if (groupDataSearchModels.Count > 0)
                {
                    IRow headerRow1 = sheet.CreateRow(5); //Excel Row Index = 6
                    CreateCellTypeStr(sheet, 5, 6, 1, 3, headerRow1, "Nhà máy");

                    IRow headerRow2 = sheet.CreateRow(6); //Excel Row Index = 7

                    int stColBd = 1;
                    int edColBd = 2;
                    List<int> months = groupDataSearchModels.Select(x => x.Month).Distinct().ToList();
                    foreach (var item in months)
                    {
                        edColBd += 2;
                        CreateCellTypeStr(sheet, 5, 5, edColBd, edColBd + 9, headerRow1, "Tháng " + item);

                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Tổng hoạt động");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Đã thực hiện");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Không thực hiện");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Chưa thực hiện");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Đang thực hiện");
                    }
                    edColBd += 2;
                    CreateCellTypeStr(sheet, 5, 6, edColBd, edColBd, headerRow1, "Tổng");
                    SetStyleTableHeader(sheet, new CellRangeAddress(5, 6, 1, edColBd), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBd = 7;
                    int edRowBd = 6;
                    var factory = groupDataSearchModels.Select(x => x.Factory.Label).Distinct().ToList();
                    foreach (var fac in factory)
                    {
                        edRowBd += 1;
                        int indexCol = 1;
                        IRow bodyRow = sheet.CreateRow(edRowBd);

                        CreateCellTypeStr(sheet, edRowBd, edRowBd, indexCol, indexCol + 2, bodyRow, fac);
                        indexCol = 2;
                        int totalMon = 0;
                        foreach (var mon in months)
                        {
                            var dataSet = groupDataSearchModels.Find(x => x.Month == mon && x.Factory.Label == fac);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, dataSet.TotalStatus);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, dataSet.DoneMadeStatus);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, dataSet.DoNotMadeStatus);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, dataSet.UnfulfilledStatus);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, dataSet.InprogressStatus);
                            int singMon = dataSet.TotalStatus + dataSet.DoneMadeStatus + dataSet.DoNotMadeStatus + dataSet.UnfulfilledStatus + dataSet.InprogressStatus;
                            totalMon += singMon;
                        }
                        indexCol += 2;
                        bodyRow.CreateCell(indexCol).SetCellValue(totalMon);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: CreateGeneralActiveAmFile", ex.ToString());
                return false;
            }
        }
        #endregion

        #region Tiến trình xử lý yêu cầu
        private bool RptyReportGeneralGenerateFile(SearchPrtReportModel requestModel, ReportPrtGeneralModel dataInput, string outputFile, string factoryName)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 11, title1, "BÁO CÁO TỔNG HỢP TIẾN TRÌNH XỬ LÝ YÊU CẦU");
                SetBoldweight(sheet, indexRow, 4, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 3, 12, title2, $"Dữ liệu năm {requestModel.Year} - Tháng {requestModel.Month} - {factoryName}");
                SetBoldweight(sheet, indexRow, 3, true, true, 11);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 3, headerRow1, "Mức độ khẩn cấp");
                CreateCellTypeStr(sheet, indexRow, indexRow, 4, 13, headerRow1, "Trạng thái xử lý");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 14, 14, headerRow1, "Tổng");

                indexRow += 1;//Excel Row Index = 7
                IRow headerRow2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 4, 5, headerRow2, "Chờ đăng ký");
                CreateCellTypeStr(sheet, indexRow, indexRow, 6, 7, headerRow2, "Đã đăng ký");
                CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, headerRow2, "Đã cấp phiếu");
                CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, headerRow2, "Đang thực hiện");
                CreateCellTypeStr(sheet, indexRow, indexRow, 12, 13, headerRow2, "Đã hoàn thành");
                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 14), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;
                foreach (var item in dataInput.DataResult)
                {
                    indexRow += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);

                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 3, bodyRow, item.Emergency.Label);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 4, 5, bodyRow, item.TotalWaitForRegist.ToString());
                    CreateCellTypeStr(sheet, indexRow, indexRow, 6, 7, bodyRow, item.TotalRegistered.ToString());
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, bodyRow, item.TotalApproved.ToString());
                    CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, bodyRow, item.TotalInProgress.ToString());
                    CreateCellTypeStr(sheet, indexRow, indexRow, 12, 13, bodyRow, item.TotalCompleted.ToString());
                    bodyRow.CreateCell(14).SetCellValue(item.TotalAll.ToString());
                }
                int edRowBdIndex = indexRow;
                int edColBdIndex = 14;

                SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                return false;
            }
        }

        private bool RptyReportDetailGenerateFile(SearchPrtReportModel requestModel, List<PrtReportDetailModel> dataInput, string outputFile, string factoryName)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;
                int stColBdIndex = 1;
                int edColBdIndex = 23;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 8, 17, title1, "BÁO CÁO CHI TIẾT TIẾN TRÌNH XỬ LÝ YÊU CẦU");
                SetBoldweight(sheet, indexRow, 8, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 7, 18, title2, $"Dữ liệu năm {requestModel.Year} - Tháng {requestModel.Month} - {factoryName}");
                SetBoldweight(sheet, indexRow, 7, true, true, 11);

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow2 = sheet.CreateRow(indexRow);
                headerRow2.CreateCell(1).SetCellValue("STT");
                CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, headerRow2, "Số phiếu yêu cầu");
                CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, headerRow2, "Nhà máy");
                CreateCellTypeStr(sheet, indexRow, indexRow, 8, 10, headerRow2, "Thiết bị");
                CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, headerRow2, "Người tạo");
                CreateCellTypeStr(sheet, indexRow, indexRow, 14, 15, headerRow2, "Ngày tạo");
                CreateCellTypeStr(sheet, indexRow, indexRow, 16, 18, headerRow2, "Mức độ khẩn cấp");
                CreateCellTypeStr(sheet, indexRow, indexRow, 19, 20, headerRow2, "Trạng thái");
                CreateCellTypeStr(sheet, indexRow, indexRow, 21, 23, headerRow2, "Trạng thái xử lý");
                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int countStt = 0;
                foreach (var item in dataInput)
                {
                    indexRow += 1;
                    countStt += 1;
                    IRow bodyRow = sheet.CreateRow(indexRow);

                    bodyRow.CreateCell(1).SetCellValue(countStt.ToString());
                    CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, bodyRow, item.RequestTicketNumber);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, bodyRow, item.Factory.Label);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 10, bodyRow, item.Device.Label);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, bodyRow, item.CreaterUser.Label);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 14, 15, bodyRow, item.CreateDate.ToString("yyyy/MM/dd"));
                    CreateCellTypeStr(sheet, indexRow, indexRow, 16, 18, bodyRow, item.Emergency.Label);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 19, 20, bodyRow, item.Status.Label);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 21, 23, bodyRow, item.ProcessingStatus.Label);
                }
                int edRowBdIndex = indexRow;

                SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                SetStyleFinish(sheet, stRowBdIndex, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                throw;
            }
        }

        private List<PrtReportDetailModel> GetDataRptyReportDetail(SearchPrtReportModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm kiếm theo trạng thái chờ đăng ký và đã đăng ký
            entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved || x.AmStatusID == (int)AmEnumRequestTicket.Registered);

            // Tìm kiếm theo nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm kiếm theo năm tạo
            if (requestModel.Year > 1900)
            {
                entities = entities.Where(x => x.InsertedAt.Year == requestModel.Year);
            }

            // Tìm kiếm theo tháng tạo
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.InsertedAt.Month == requestModel.Month);
            }

            // Tìm kiếm theo mức độ khẩn cấp
            if (requestModel.EmergencyID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmEmergencyId == requestModel.EmergencyID);
            }

            DateTime currentDate = DateTime.Now;
            if (requestModel.IsMailReport)
            {
                DateTime bef1Date = currentDate.AddDays(-1);
                DateTime bef7Date = bef1Date.AddDays(-6);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= bef7Date.Date);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= bef1Date.Date);
            }

            var rootData = entities.Select(
                e => new PrtReportDetailModel()
                {
                    ID = e.ID,
                    RequestTicketNumber = e.RequestTicketNumber,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    FactoryID = e.FactoryID,
                    Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : null,
                    CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                    CreateDate = e.InsertedAt,
                    Status = new ValueLabelCheckedModel { Value = 0, Label = "Đã được duyệt" },
                    StatusValue = e.AmStatusID,
                    ProcessStatusValue = 0
                }).ToList();

            if (rootData.Count > 0)
            {
                var lstIds = rootData.Select(x => x.ID);
                var lstWtStatuss = new List<int>()
                {
                    (int)EnumTicketStatus.Approved,
                    (int)EnumTicketStatus.Allowed,
                    (int)EnumTicketStatus.InProgress,
                    (int)EnumTicketStatus.WaitForConfirmation,
                    (int)EnumTicketStatus.Completed
                };

                var dataWorkTickets = _unitOfWork.ElogbookRepository<WorkTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && lstWtStatuss.Contains(x.WorkTicketStatus) && !x.IsDeleted).ToList();

                var dataRegistTickets = _unitOfWork.ElogbookRepository<RegistrationTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && !x.IsDeleted).ToList();

                foreach (var item in rootData)
                {
                    // Trường hợp phiếu công tác có trạng thái từ cấp phiếu tới lúc hoàn thành
                    if (item.StatusValue == (int)AmEnumRequestTicket.Registered)
                    {
                        // Tìm PYC trong PCT
                        var wTicket = dataWorkTickets.Find(x => x.RequestTicketID == item.ID);

                        if (wTicket != null)
                        {
                            // Đã duyệt phiếu yêu cầu (Đã đăng ký)- Đã tạo phiếu đăng ký công tác - Đã cấp phiếu (PCT)
                            if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Approved)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.ApprovedPCT;
                                continue;
                            }

                            // Đã duyệt phiếu yêu cầu (Đã đăng ký)- Đã tạo phiếu đăng ký công tác - Hoàn thành sửa chữa (PCT)
                            else if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Completed)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.CompletedPCT;
                                continue;
                            }

                            // Đã duyệt phiếu yêu cầu (Đã đăng ký)- Đã tạo phiếu đăng ký công tác - Đang thực hiện sửa chữa (PCT)
                            else
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.OnGoingPCT;
                                continue;
                            }
                        }

                        // Tìm trong PĐK
                        var rTicket = dataRegistTickets.Find(x => x.RequestTicketID == item.ID);
                        if (rTicket != null)
                        {
                            item.ProcessStatusValue = (int)AmEnumReportStatus.RegisteredPCT;
                            continue;
                        }
                    }

                    // PYC đã được duyệt, phiếu đăng ký công tác chưa được tạo
                    if (item.StatusValue == (int)AmEnumRequestTicket.Approved)
                    {
                        item.ProcessStatusValue = (int)AmEnumReportStatus.WaitForRegistPCT;
                    }
                }

                // Tìm kiếm theo trạng thái xử lý
                if (requestModel.ProcessingStatusID != CommonConstant.ALL_KEY)
                {
                    rootData = rootData.FindAll(x => x.ProcessStatusValue == requestModel.ProcessingStatusID);
                }
            }

            return rootData;
        }

        private ReportPrtGeneralModel GetDataRptyReportGeneral(List<PrtReportDetailModel> rootData)
        {
            var emgIds = rootData.Select(x => x.Emergency.Value).Distinct();
            ReportPrtGeneralModel reportPrtGeneral = new ReportPrtGeneralModel();
            PrtStatisticsReport totalAll = new PrtStatisticsReport
            {
                Emergency = new ValueLabelCheckedModel() { Value = 0, Label = "Tổng" }
            };
            List<PrtStatisticsReport> dataCount = new List<PrtStatisticsReport>();
            foreach (var emgId in emgIds)
            {
                PrtStatisticsReport prtStatisticsReport = new PrtStatisticsReport
                {
                    Emergency = rootData.Find(x => x.Emergency.Value == emgId).Emergency,
                    TotalWaitForRegist = rootData.Count(x => x.ProcessStatusValue == (int)AmEnumReportStatus.WaitForRegistPCT && x.Emergency.Value == emgId),
                    TotalRegistered = rootData.Count(x => x.ProcessStatusValue == (int)AmEnumReportStatus.RegisteredPCT && x.Emergency.Value == emgId),
                    TotalApproved = rootData.Count(x => x.ProcessStatusValue == (int)AmEnumReportStatus.ApprovedPCT && x.Emergency.Value == emgId),
                    TotalInProgress = rootData.Count(x => x.ProcessStatusValue == (int)AmEnumReportStatus.OnGoingPCT && x.Emergency.Value == emgId),
                    TotalCompleted = rootData.Count(x => x.ProcessStatusValue == (int)AmEnumReportStatus.CompletedPCT && x.Emergency.Value == emgId),
                };
                prtStatisticsReport.TotalAll = prtStatisticsReport.TotalWaitForRegist
                                                + prtStatisticsReport.TotalRegistered
                                                + prtStatisticsReport.TotalApproved
                                                + prtStatisticsReport.TotalInProgress
                                                + prtStatisticsReport.TotalCompleted;

                totalAll.TotalWaitForRegist += prtStatisticsReport.TotalWaitForRegist;
                totalAll.TotalRegistered += prtStatisticsReport.TotalRegistered;
                totalAll.TotalApproved += prtStatisticsReport.TotalApproved;
                totalAll.TotalInProgress += prtStatisticsReport.TotalInProgress;
                totalAll.TotalCompleted += prtStatisticsReport.TotalCompleted;
                totalAll.TotalAll += prtStatisticsReport.TotalAll;

                dataCount.Add(prtStatisticsReport);
            }
            dataCount.Add(totalAll);
            reportPrtGeneral.DataResult = dataCount;

            return reportPrtGeneral;
        }

        private bool ReportGeneralGenerateFile(List<PrtReportDetailModel> dataInput, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                var lstFacs = dataInput.Select(x => x.FactoryID).Distinct().ToList();

                foreach (var facId in lstFacs)
                {
                    var dataByFactory = dataInput.FindAll(x => x.FactoryID == facId).ToList();
                    var dataExport = GetDataRptyReportGeneral(dataByFactory);
                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == facId).FirstOrDefault();
                    if (factory != null)
                    {
                        ISheet sheet = workbook.CreateSheet(factory.FactoryName);

                        int indexRow = 0;

                        // Title
                        indexRow += 1;//Excel Row Index = 2
                        IRow title1 = sheet.CreateRow(indexRow);
                        CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 11, title1, "BÁO CÁO TỔNG HỢP TIẾN TRÌNH XỬ LÝ YÊU CẦU");
                        SetBoldweight(sheet, indexRow, 4, true, false, 16);

                        indexRow += 2;//Excel Row Index = 4
                        IRow title2 = sheet.CreateRow(indexRow);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 3, 12, title2, $"Dữ liệu từ ngày {DateTime.Now.AddDays(-7):dd/MM/yyyy} - {DateTime.Now.AddDays(-1):dd/MM/yyyy} - {factory.FactoryName}");
                        SetBoldweight(sheet, indexRow, 3, true, true, 11);

                        indexRow += 2;//Excel Row Index = 6
                        IRow headerRow1 = sheet.CreateRow(indexRow);
                        CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 3, headerRow1, "Mức độ khẩn cấp");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 4, 13, headerRow1, "Trạng thái xử lý");
                        CreateCellTypeStr(sheet, indexRow, indexRow + 1, 14, 14, headerRow1, "Tổng");

                        indexRow += 1;//Excel Row Index = 7
                        IRow headerRow2 = sheet.CreateRow(indexRow);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 4, 5, headerRow2, "Chờ đăng ký");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 6, 7, headerRow2, "Đã đăng ký");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, headerRow2, "Đã cấp phiếu");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, headerRow2, "Đang thực hiện");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 12, 13, headerRow2, "Đã hoàn thành");
                        SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 14), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                        int stRowBdIndex = indexRow;
                        int stColBdIndex = 1;
                        foreach (var item in dataExport.DataResult)
                        {
                            indexRow += 1;
                            IRow bodyRow = sheet.CreateRow(indexRow);

                            CreateCellTypeStr(sheet, indexRow, indexRow, 1, 3, bodyRow, item.Emergency.Label);
                            CreateCellTypeStr(sheet, indexRow, indexRow, 4, 5, bodyRow, item.TotalWaitForRegist.ToString());
                            CreateCellTypeStr(sheet, indexRow, indexRow, 6, 7, bodyRow, item.TotalRegistered.ToString());
                            CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, bodyRow, item.TotalApproved.ToString());
                            CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, bodyRow, item.TotalInProgress.ToString());
                            CreateCellTypeStr(sheet, indexRow, indexRow, 12, 13, bodyRow, item.TotalCompleted.ToString());
                            bodyRow.CreateCell(14).SetCellValue(item.TotalAll.ToString());
                        }
                        int edRowBdIndex = indexRow;
                        int edColBdIndex = 14;

                        SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                        SetStyleFinish(sheet, stRowBdIndex - 1, edRowBdIndex, stColBdIndex, edColBdIndex, edRowBdIndex + 50, edColBdIndex + 50);
                    }
                }

                if (dataInput.Count == 0)
                {
                    ISheet sheet = workbook.CreateSheet("Sheet1");
                    int indexRow = 0;

                    // Title
                    indexRow += 1;//Excel Row Index = 2
                    IRow title1 = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 11, title1, "BÁO CÁO TỔNG HỢP TIẾN TRÌNH XỬ LÝ YÊU CẦU");
                    SetBoldweight(sheet, indexRow, 4, true, false, 16);

                    indexRow += 2;//Excel Row Index = 4
                    IRow title2 = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 3, 12, title2, $"Dữ liệu từ ngày {DateTime.Now.AddDays(-7):dd/MM/yyyy} - {DateTime.Now.AddDays(-1):dd/MM/yyyy}");
                    SetBoldweight(sheet, indexRow, 3, true, true, 11);

                    indexRow += 1;//Excel Row Index = 7
                    IRow headerRow2 = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 4, 5, headerRow2, "Chờ đăng ký");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 6, 7, headerRow2, "Đã đăng ký");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 9, headerRow2, "Đã cấp phiếu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 10, 11, headerRow2, "Đang thực hiện");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 12, 13, headerRow2, "Đã hoàn thành");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 14), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                return false;
            }
        }

        #endregion

        #region Yêu cầu bảo trì
        private List<RequestAmDetailModel> GetDataRequestAmReportGeneral(SearchPrtReportModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm theo nhiều nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryIDs))
            {
                var facs = requestModel.FactoryIDs.Split(';').Select(int.Parse).ToList();
                entities = entities.Where(x => facs.Contains(x.FactoryID));
            }

            // Tìm kiếm theo nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tìm kiếm theo năm tạo
            if (requestModel.Year > 1990)
            {
                entities = entities.Where(x => x.InsertedAt.Year == requestModel.Year);
            }

            // Tìm theo nhiều tháng
            if (!string.IsNullOrWhiteSpace(requestModel.Months))
            {
                var monthsQuery = requestModel.Months.Split(';').Select(int.Parse).ToList();

                entities = entities.Where(x => monthsQuery.Contains(x.InsertedAt.Month));
            }

            DateTime currentDate = DateTime.Now;
            if (requestModel.IsMailReport)
            {
                DateTime bef1Date = currentDate.AddDays(-1);
                DateTime bef7Date = bef1Date.AddDays(-6);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= bef7Date.Date);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= bef1Date.Date);
            }

            // Tìm kiếm theo tháng tạo
            if (requestModel.Month != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.InsertedAt.Month == requestModel.Month);
            }

            var rootData = entities.Select(
                e => new PrtReportDetailModel()
                {
                    ID = e.ID,
                    RequestTicketNumber = e.RequestTicketNumber,
                    FactoryID = e.FactoryID,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : null,
                    CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                    CreateDate = e.InsertedAt,
                    StatusValue = e.AmStatusID,
                    ProcessStatusValue = 0,
                    Month = e.InsertedAt.Month
                }).OrderBy(t => t.CreateDate)
                .ToList();

            // Danh sách các nhà máy
            var lstFactoryID = rootData.Select(x => x.Factory.Value).Distinct().ToList();

            // Dữ liệu chính response
            List<RequestAmDetailModel> lstResult = new List<RequestAmDetailModel>();
            var months = rootData.Select(x => x.Month).Distinct().ToList();
            if (lstFactoryID.Count > 0)
            {
                foreach (var fId in lstFactoryID)
                {
                    RequestAmDetailModel factoryDetail = new RequestAmDetailModel
                    {
                        Months = string.Join(",", months),
                        Factory = rootData.Find(x => x.FactoryID == fId).Factory
                    };

                    List<RequestAmByMonthModel> monthDetails = new List<RequestAmByMonthModel>();
                    // Danh sách các tháng
                    if (requestModel.Month == CommonConstant.ALL_KEY)
                    {
                        foreach (var month in months)
                        {
                            RequestAmByMonthModel monthDetail = new RequestAmByMonthModel()
                            {
                                TotalUnSent = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && x.StatusValue == (int)AmEnumRequestTicket.UnSend),
                                TotalOnGoing = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && (x.StatusValue == (int)AmEnumRequestTicket.InProgress || x.StatusValue == (int)AmEnumRequestTicket.Reject)),
                                TotalStop = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && x.StatusValue == (int)AmEnumRequestTicket.Stop),
                                TotalApproved = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && (x.StatusValue == (int)AmEnumRequestTicket.Approved || x.StatusValue == (int)AmEnumRequestTicket.Registered)),
                                TotalRequest = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month),
                                Month = month
                            };
                            monthDetails.Add(monthDetail);
                            factoryDetail.Total += monthDetail.TotalRequest;
                        }

                        factoryDetail.MonthDetails = monthDetails;
                    }
                    else
                    {
                        int month = requestModel.Month;
                        RequestAmByMonthModel monthDetail = new RequestAmByMonthModel()
                        {
                            TotalUnSent = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && x.StatusValue == (int)AmEnumRequestTicket.UnSend),
                            TotalOnGoing = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && (x.StatusValue == (int)AmEnumRequestTicket.InProgress || x.StatusValue == (int)AmEnumRequestTicket.Reject)),
                            TotalStop = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && x.StatusValue == (int)AmEnumRequestTicket.Stop),
                            TotalApproved = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month && (x.StatusValue == (int)AmEnumRequestTicket.Approved || x.StatusValue == (int)AmEnumRequestTicket.Registered)),
                            TotalRequest = rootData.Count(x => x.Factory.Value == fId && x.CreateDate.Month == month),
                            Month = month
                        };
                        monthDetails.Add(monthDetail);
                        factoryDetail.Total += monthDetail.TotalRequest;

                        factoryDetail.MonthDetails = monthDetails;
                    }
                    lstResult.Add(factoryDetail);
                }

                // Xử lý dữ liệu tổng yêu cầu của tổng các nhà máy
                // Record tổng của các nhà máy
                RequestAmDetailModel totalFactoryDetail = new RequestAmDetailModel
                {
                    Factory = new ValueLabelCheckedModel { Value = 0, Label = "Tổng" }
                };

                // Danh sách dữ liệu tổng của các nhà máy theo từng tháng
                List<RequestAmByMonthModel> totalFactoryMonthDetail = new List<RequestAmByMonthModel>();

                if (requestModel.Month == CommonConstant.ALL_KEY)
                {
                    foreach (var month in months)
                    {
                        RequestAmByMonthModel monthDetail = new RequestAmByMonthModel();
                        foreach (var item in lstResult.Select(x => x.MonthDetails))
                        {
                            monthDetail.TotalUnSent += item.Find(x => x.Month == month).TotalUnSent;
                            monthDetail.TotalOnGoing += item.Find(x => x.Month == month).TotalOnGoing;
                            monthDetail.TotalStop += item.Find(x => x.Month == month).TotalStop;
                            monthDetail.TotalApproved += item.Find(x => x.Month == month).TotalApproved;
                            monthDetail.TotalRequest += item.Find(x => x.Month == month).TotalRequest;
                        }
                        monthDetail.Month = month;
                        totalFactoryMonthDetail.Add(monthDetail);
                        totalFactoryDetail.Total += monthDetail.TotalRequest;
                    }
                    totalFactoryDetail.MonthDetails = totalFactoryMonthDetail;

                    lstResult.Add(totalFactoryDetail);
                }
                else
                {
                    RequestAmByMonthModel monthDetail = new RequestAmByMonthModel();
                    foreach (var item in lstResult.Select(x => x.MonthDetails))
                    {
                        monthDetail.TotalUnSent += item.Find(x => x.Month == requestModel.Month).TotalUnSent;
                        monthDetail.TotalOnGoing += item.Find(x => x.Month == requestModel.Month).TotalOnGoing;
                        monthDetail.TotalStop += item.Find(x => x.Month == requestModel.Month).TotalStop;
                        monthDetail.TotalApproved += item.Find(x => x.Month == requestModel.Month).TotalApproved;
                        monthDetail.TotalRequest += item.Find(x => x.Month == requestModel.Month).TotalRequest;
                    }
                    monthDetail.Month = requestModel.Month;
                    totalFactoryDetail.Total = monthDetail.TotalRequest;
                    totalFactoryMonthDetail.Add(monthDetail);
                    totalFactoryDetail.MonthDetails = totalFactoryMonthDetail;

                    lstResult.Add(totalFactoryDetail);
                }
            }

            return lstResult;
        }

        private List<RequestAmReportDetailModel> GetDataRequestAmReportDetail(SearchPrtReportModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm kiếm theo nhà máy
            entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);

            // Tìm kiếm theo năm tạo
            entities = entities.Where(x => x.InsertedAt.Year == requestModel.Year);

            // Tìm kiếm theo tháng tạo
            entities = entities.Where(x => x.InsertedAt.Month == requestModel.Month);

            // Tìm kiếm theo mức độ khẩn cấp
            if (requestModel.EmergencyID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmEmergencyId == requestModel.EmergencyID);
            }

            // Tìm kiếm theo trạng thái "Chưa gửi phiếu"
            if (requestModel.StatusID == (int)AmEnumReportStatus.UnSentPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.UnSend);
            }

            // Tìm kiếm theo trạng thái "Đang xử lý"
            if (requestModel.StatusID == (int)AmEnumReportStatus.InProgessPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.InProgress || x.AmStatusID == (int)AmEnumRequestTicket.Reject);
            }

            // Tìm kiếm theo trạng thái "Dừng xử lý"
            if (requestModel.StatusID == (int)AmEnumReportStatus.StopPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Stop);
            }

            // Tìm kiếm theo trạng thái "Đã được duyệt"
            if (requestModel.StatusID == (int)AmEnumReportStatus.ApprovePYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved || x.AmStatusID == (int)AmEnumRequestTicket.Registered);
            }

            var dataResult = entities.Select(
                e => new RequestAmReportDetailModel()
                {
                    ID = e.ID,
                    RequestTicketNumber = e.RequestTicketNumber,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : new ValueLabelCheckedModel { Value = 0, Label = string.Empty },
                    CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                    CreateDate = e.InsertedAt,
                    AmStatusID = e.AmStatusID,
                    ProcessStatusValue = 0
                }).OrderByDescending(t => t.CreateDate).ToList();

            var rootData = dataResult.ToList();

            if (rootData.Count > 0)
            {
                var lstIds = rootData.Select(x => x.ID);
                var lstWtStatuss = new List<int>()
                {
                    (int)EnumTicketStatus.Approved,
                    (int)EnumTicketStatus.Allowed,
                    (int)EnumTicketStatus.InProgress,
                    (int)EnumTicketStatus.WaitForConfirmation,
                    (int)EnumTicketStatus.Completed
                };

                var dataWorkTickets = _unitOfWork.ElogbookRepository<WorkTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && lstWtStatuss.Contains(x.WorkTicketStatus) && !x.IsDeleted).ToList();

                var dataRegistTickets = _unitOfWork.ElogbookRepository<RegistrationTicket>()
                    .GetQuery(x => lstIds.Contains((int)x.RequestTicketID) && !x.IsDeleted).ToList();

                foreach (var item in rootData)
                {
                    // Trạng thái phiếu yêu cầu là Chưa gửi phiếu
                    if (item.AmStatusID == (int)AmEnumRequestTicket.UnSend)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.UnSentPYC;
                        item.ProcessStatusValue = 0;
                    }

                    // Trạng thái phiếu yêu cầu là Đang xử lý or Từ chối
                    if (item.AmStatusID == (int)AmEnumRequestTicket.InProgress || item.AmStatusID == (int)AmEnumRequestTicket.Reject)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.InProgessPYC;
                        item.ProcessStatusValue = 0;
                    }

                    // Trạng thái phiếu yêu cầu là Dừng xử lý
                    if (item.AmStatusID == (int)AmEnumRequestTicket.Stop)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.StopPYC;
                        item.ProcessStatusValue = 0;
                    }

                    // Trạng thái phiếu yêu cầu là Chờ đăng ký
                    if (item.AmStatusID == (int)AmEnumRequestTicket.Approved)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.ApprovePYC;
                        item.ProcessStatusValue = (int)AmEnumReportStatus.WaitForRegistPCT;
                    }

                    // Trạng thái phiếu yêu cầu là Đã đăng ký
                    if (item.AmStatusID == (int)AmEnumRequestTicket.Registered)
                    {
                        item.StatusValue = (int)AmEnumReportStatus.ApprovePYC;

                        // Tìm PYC trong PCT
                        var wTicket = dataWorkTickets.Find(x => x.RequestTicketID == item.ID);

                        if (wTicket != null)
                        {
                            // Đã duyệt PYC - Hoàn thành sửa chữa (PCT)
                            if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Completed)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.CompletedPCT;
                                item.WorkTicket = new ValueLabelCheckedModel()
                                {
                                    Value = wTicket.ID,
                                    Label = wTicket.WorkTicketNumber
                                };
                                continue;
                            }

                            // Đã duyệt PYC - Đã cấp phiếu (PCT)
                            else if (wTicket.WorkTicketStatus == (int)EnumTicketStatus.Approved)
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.ApprovedPCT;
                                item.WorkTicket = new ValueLabelCheckedModel()
                                {
                                    Value = wTicket.ID,
                                    Label = wTicket.WorkTicketNumber
                                };
                                continue;
                            }

                            // Đã duyệt PYC - Đang thực hiện sửa chữa (PCT)
                            else
                            {
                                item.ProcessStatusValue = (int)AmEnumReportStatus.OnGoingPCT;
                                item.WorkTicket = new ValueLabelCheckedModel()
                                {
                                    Value = wTicket.ID,
                                    Label = wTicket.WorkTicketNumber
                                };
                                continue;
                            }
                        }

                        //Đã duyệt phiếu yêu cầu - Đã tạo phiếu đăng ký công tác - Chưa được cấp phiếu công tác
                        // Tìm PYC trong PĐK
                        var rTicket = dataRegistTickets.Find(x => x.RequestTicketID == item.ID);
                        if (rTicket != null)
                        {
                            item.ProcessStatusValue = (int)AmEnumReportStatus.RegisteredPCT;
                            item.RegisterTicket = new ValueLabelCheckedModel()
                            {
                                Value = rTicket.ID,
                                Label = rTicket.TicketNumber
                            };
                        }
                    }
                }

                // Tìm kiếm theo trạng thái xử lý
                if (requestModel.ProcessingStatusID != CommonConstant.ALL_KEY)
                {
                    rootData = rootData.FindAll(x => x.ProcessStatusValue == requestModel.ProcessingStatusID);
                }
            }
            return rootData;
        }

        private RequestAmReportEmergencyModel GetDataRequestAmReportEmergency(SearchPrtReportModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm kiếm theo nhà máy
            entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);

            // Tìm kiếm theo năm tạo
            entities = entities.Where(x => x.InsertedAt.Year == requestModel.Year);

            // Tìm kiếm theo tháng tạo
            entities = entities.Where(x => x.InsertedAt.Month == requestModel.Month);

            // Tìm kiếm theo trạng thái "Chưa gửi phiếu"
            if (requestModel.StatusID == (int)AmEnumReportStatus.UnSentPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.UnSend);
            }

            // Tìm kiếm theo trạng thái "Đang xử lý"
            if (requestModel.StatusID == (int)AmEnumReportStatus.InProgessPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.InProgress || x.AmStatusID == (int)AmEnumRequestTicket.Reject);
            }

            // Tìm kiếm theo trạng thái "Dừng xử lý"
            if (requestModel.StatusID == (int)AmEnumReportStatus.StopPYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Stop);
            }

            // Tìm kiếm theo trạng thái "Đã được duyệt"
            if (requestModel.StatusID == (int)AmEnumReportStatus.ApprovePYC)
            {
                entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved || x.AmStatusID == (int)AmEnumRequestTicket.Registered);
            }

            var dataResult = entities.Select(
                e => new PrtReportDetailModel()
                {
                    ID = e.ID,
                    RequestTicketNumber = e.RequestTicketNumber,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    FactoryID = e.FactoryID,
                    Device = e.AmDeviceId.HasValue ? new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc } : null,
                    CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Emergency = e.AmEmergencyId.HasValue ? new ValueLabelCheckedModel { Value = e.AmEmergency.ID, Label = e.AmEmergency.Name } : null,
                    EmergencyID = e.AmEmergencyId,
                    CreateDate = e.InsertedAt,
                    StatusValue = e.AmStatusID,
                    ProcessStatusValue = 0
                }).ToList();

            var lstEmergencyID = dataResult.Select(x => x.EmergencyID).Distinct().ToList();

            var rsData = new RequestAmReportEmergencyModel
            {
                UnSentInfo = new List<ReportEmergency>(),
                OnGoingInfo = new List<ReportEmergency>(),
                StopInfo = new List<ReportEmergency>(),
                ApprovedInfo = new List<ReportEmergency>(),
                TotalInfo = new List<ReportEmergency>(),
                TotalEmergencyID = lstEmergencyID.Count,
                Month = requestModel.Month
            };
            if (lstEmergencyID.Count > 0)
            {
                foreach (var item in lstEmergencyID)
                {
                    // Chưa gửi phiếu
                    rsData.TotalUnSent += dataResult.Count(x => x.EmergencyID == item && x.StatusValue == (int)AmEnumRequestTicket.UnSend);
                    rsData.UnSentInfo.Add(new ReportEmergency()
                    {
                        Total = dataResult.Count(x => x.EmergencyID == item && x.StatusValue == (int)AmEnumRequestTicket.UnSend),
                        Emergency = dataResult.Find(x => x.EmergencyID == item).Emergency
                    });

                    // Đang xử lý
                    rsData.TotalOnGoing += dataResult.Count(x => x.EmergencyID == item && (x.StatusValue == (int)AmEnumRequestTicket.InProgress || x.StatusValue == (int)AmEnumRequestTicket.Reject));
                    rsData.OnGoingInfo.Add(new ReportEmergency()
                    {
                        Total = dataResult.Count(x => x.EmergencyID == item && (x.StatusValue == (int)AmEnumRequestTicket.InProgress || x.StatusValue == (int)AmEnumRequestTicket.Reject)),
                        Emergency = dataResult.Find(x => x.EmergencyID == item).Emergency
                    });

                    // Dừng xử lý
                    rsData.TotalStop += dataResult.Count(x => x.EmergencyID == item && x.StatusValue == (int)AmEnumRequestTicket.Stop);
                    rsData.StopInfo.Add(new ReportEmergency()
                    {
                        Total = dataResult.Count(x => x.EmergencyID == item && x.StatusValue == (int)AmEnumRequestTicket.Stop),
                        Emergency = dataResult.Find(x => x.EmergencyID == item).Emergency
                    });

                    // Đã được duyệt
                    rsData.TotalApproved += dataResult.Count(x => x.EmergencyID == item && (x.StatusValue == (int)AmEnumRequestTicket.Approved || x.StatusValue == (int)AmEnumRequestTicket.Registered));
                    rsData.ApprovedInfo.Add(new ReportEmergency()
                    {
                        Total = dataResult.Count(x => x.EmergencyID == item && (x.StatusValue == (int)AmEnumRequestTicket.Approved || x.StatusValue == (int)AmEnumRequestTicket.Registered)),
                        Emergency = dataResult.Find(x => x.EmergencyID == item).Emergency
                    });

                    // Tổng
                    rsData.TotalTotal += dataResult.Count(x => x.EmergencyID == item);
                    rsData.TotalInfo.Add(new ReportEmergency()
                    {
                        Total = dataResult.Count(x => x.EmergencyID == item),
                        Emergency = dataResult.Find(x => x.EmergencyID == item).Emergency
                    });
                }
            }
            return rsData;
        }

        private bool RequestAmReportDetailGenerateFile(SearchPrtReportModel requestModel, List<RequestAmReportDetailModel> dataInput, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;
                int stColBdIndex = 1;
                int edColBdIndex = 23;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 7, 18, title1, "BÁO CÁO TIẾN TRÌNH XỬ LÝ CHI TIẾT");
                SetBoldweight(sheet, indexRow, 7, true, false, 16);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 7, 18, title2, requestModel.Title);
                SetBoldweight(sheet, indexRow, 7, true, true, 11);

                if (dataInput.Count > 0)
                {
                    indexRow += 2;//Excel Row Index = 6
                    IRow headerRow2 = sheet.CreateRow(indexRow);
                    headerRow2.CreateCell(1).SetCellValue("STT");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, headerRow2, "Số phiếu yêu cầu");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, headerRow2, "Nhà máy");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 8, 10, headerRow2, "Thiết bị");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, headerRow2, "Người tạo");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 14, 15, headerRow2, "Ngày tạo");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 16, 18, headerRow2, "Mức độ khẩn cấp");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 19, 20, headerRow2, "Trạng thái");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 21, 23, headerRow2, "Trạng thái xử lý");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColBdIndex, edColBdIndex), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    int stRowBdIndex = indexRow;
                    int countStt = 0;
                    foreach (var item in dataInput)
                    {
                        indexRow += 1;
                        countStt += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(countStt.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 4, bodyRow, item.RequestTicketNumber);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 5, 7, bodyRow, item.Factory.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 8, 10, bodyRow, item.Device.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 11, 13, bodyRow, item.CreaterUser.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 14, 15, bodyRow, item.CreateDate.ToString("yyyy/MM/dd"));
                        CreateCellTypeStr(sheet, indexRow, indexRow, 16, 18, bodyRow, item.Emergency.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 19, 20, bodyRow, item.Status.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 21, 23, bodyRow, item.ProcessingStatus.Label);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RptyReportGeneralGenerateFile", ex.ToString());
                return false;
            }
        }

        private bool RequestAmReportEmergencyGenerateFile(SearchPrtReportModel requestModel, RequestAmReportEmergencyModel dataInput, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                // Title
                IRow title1 = sheet.CreateRow(1); //Excel Row Index = 2
                CreateCellTypeStr(sheet, 1, 2, 1, 8, title1, "BÁO CÁO MỨC ĐỘ KHẨN CẤP YÊU CẦU BẢO TRÌ");
                SetBoldweight(sheet, 1, 1, true, false, 16);

                IRow title2 = sheet.CreateRow(3); //Excel Row Index = 4
                CreateCellTypeStr(sheet, 3, 3, 1, 8, title2, requestModel.Title);
                SetBoldweight(sheet, 3, 1, true, true, 11);

                if (dataInput.TotalEmergencyID > 0)
                {
                    IRow headerRow1 = sheet.CreateRow(5); //Excel Row Index = 6
                    CreateCellTypeStr(sheet, 5, 6, 1, 2, headerRow1, "Trạng thái");

                    int edColBd = 3; //Excel Col Index = 3
                    CreateCellTypeStr(sheet, 5, 5, 3, (dataInput.TotalEmergencyID * 3) + 2, headerRow1, "Tháng " + dataInput.Month.ToString());

                    IRow headerRow2 = sheet.CreateRow(6); //Excel Row Index = 7

                    foreach (var item in dataInput.TotalInfo)
                    {
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 2, headerRow2, item.Emergency.Label);
                        edColBd += 3;
                    }
                    CreateCellTypeStr(sheet, 5, 6, edColBd, edColBd, headerRow1, "Tổng");

                    SetStyleTableHeader(sheet, new CellRangeAddress(5, 6, 1, edColBd), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                    if (requestModel.StatusID == CommonConstant.ALL_KEY)
                    {
                        IRow body1 = sheet.CreateRow(7);
                        CreateCellTypeStr(sheet, 7, 7, 1, 2, body1, "Chưa gửi phiếu");
                        body1.CreateCell(edColBd).SetCellValue(dataInput.TotalUnSent);

                        IRow body2 = sheet.CreateRow(8);
                        CreateCellTypeStr(sheet, 8, 8, 1, 2, body2, "Đang xử lý");
                        body2.CreateCell(edColBd).SetCellValue(dataInput.TotalOnGoing);

                        IRow body3 = sheet.CreateRow(9);
                        CreateCellTypeStr(sheet, 9, 9, 1, 2, body3, "Dừng xử lý");
                        body3.CreateCell(edColBd).SetCellValue(dataInput.TotalStop);

                        IRow body4 = sheet.CreateRow(10);
                        CreateCellTypeStr(sheet, 10, 10, 1, 2, body4, "Đã được duyệt");
                        body4.CreateCell(edColBd).SetCellValue(dataInput.TotalApproved);

                        IRow body5 = sheet.CreateRow(11);
                        CreateCellTypeStr(sheet, 11, 11, 1, 2, body5, "Tổng");
                        body5.CreateCell(edColBd).SetCellValue(dataInput.TotalTotal);

                        int indexCol = 3;
                        for (int i = 1; i <= dataInput.TotalEmergencyID; i++)
                        {
                            CreateCellTypeNum(sheet, 7, 7, indexCol, indexCol + 2, body1, dataInput.UnSentInfo[i - 1].Total);
                            CreateCellTypeNum(sheet, 8, 8, indexCol, indexCol + 2, body2, dataInput.OnGoingInfo[i - 1].Total);
                            CreateCellTypeNum(sheet, 9, 9, indexCol, indexCol + 2, body3, dataInput.StopInfo[i - 1].Total);
                            CreateCellTypeNum(sheet, 10, 10, indexCol, indexCol + 2, body4, dataInput.ApprovedInfo[i - 1].Total);
                            CreateCellTypeNum(sheet, 11, 11, indexCol, indexCol + 2, body5, dataInput.TotalInfo[i - 1].Total);

                            indexCol += 3;
                        }

                        SetStyleTableBody(sheet, new CellRangeAddress(7, 11, 1, edColBd), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                        SetStyleFinish(sheet, 5, 11, 1, edColBd, 61, edColBd + 50);
                    }
                    else
                    {
                        string title = string.Empty;
                        switch (requestModel.StatusID)
                        {
                            case (int)AmEnumReportStatus.UnSentPYC:
                                title = EnumExtensions.GetEnumDescription(AmEnumReportStatus.UnSentPYC);
                                break;
                            case (int)AmEnumReportStatus.InProgessPYC:
                                title = EnumExtensions.GetEnumDescription(AmEnumReportStatus.InProgessPYC);
                                break;
                            case (int)AmEnumReportStatus.StopPYC:
                                title = EnumExtensions.GetEnumDescription(AmEnumReportStatus.StopPYC);
                                break;
                            case (int)AmEnumReportStatus.ApprovePYC:
                                title = EnumExtensions.GetEnumDescription(AmEnumReportStatus.ApprovePYC);
                                break;
                        }

                        IRow body1 = sheet.CreateRow(7);
                        CreateCellTypeStr(sheet, 7, 7, 1, 2, body1, title);
                        body1.CreateCell(edColBd).SetCellValue(dataInput.TotalUnSent.ToString());

                        IRow body2 = sheet.CreateRow(8);
                        CreateCellTypeStr(sheet, 8, 8, 1, 2, body2, "Tổng");
                        body2.CreateCell(edColBd).SetCellValue(dataInput.TotalOnGoing.ToString());

                        int indexCol = 3;
                        for (int i = 1; i <= dataInput.TotalEmergencyID; i++)
                        {
                            switch (requestModel.StatusID)
                            {
                                case (int)AmEnumReportStatus.UnSentPYC:
                                    CreateCellTypeNum(sheet, 7, 7, indexCol, indexCol + 2, body1, dataInput.UnSentInfo[i - 1].Total);
                                    break;
                                case (int)AmEnumReportStatus.InProgessPYC:
                                    CreateCellTypeNum(sheet, 7, 7, indexCol, indexCol + 2, body1, dataInput.OnGoingInfo[i - 1].Total);
                                    break;
                                case (int)AmEnumReportStatus.StopPYC:
                                    CreateCellTypeNum(sheet, 7, 7, indexCol, indexCol + 2, body1, dataInput.StopInfo[i - 1].Total);
                                    break;
                                case (int)AmEnumReportStatus.ApprovePYC:
                                    CreateCellTypeNum(sheet, 7, 7, indexCol, indexCol + 2, body1, dataInput.ApprovedInfo[i - 1].Total);
                                    break;
                            }
                            CreateCellTypeStr(sheet, 8, 8, indexCol, indexCol + 2, body2, dataInput.TotalInfo[i - 1].Total.ToString());
                            indexCol += 3;
                        }

                        SetStyleTableBody(sheet, new CellRangeAddress(7, 8, 1, edColBd), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);

                        SetStyleFinish(sheet, 5, 8, 1, edColBd, 61, edColBd + 50);
                    }
                }
                else
                {
                    IRow noneData = sheet.CreateRow(5); //Excel Row Index = 6
                    noneData.CreateCell(1).SetCellValue("Không tìm thấy dữ liệu tương ứng.");

                    SetStyleFinish(sheet, 1, 3, 1, 8, 61, 58);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RequestAmReportEmergencyGenerateFile", ex.ToString());
                return false;
            }
        }

        private bool RequestAmReportGeneralGenerateFile(SearchPrtReportModel requestModel, List<RequestAmDetailModel> dataInput, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                // Title
                IRow title1 = sheet.CreateRow(1); //Excel Row Index = 2
                CreateCellTypeStr(sheet, 1, 2, 4, 11, title1, "BÁO CÁO TỔNG HỢP YÊU CẦU BẢO TRÌ");
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
                    foreach (var item in dataInput[0].MonthDetails)
                    {
                        edColBd += 2;
                        CreateCellTypeStr(sheet, 5, 5, edColBd, edColBd + 9, headerRow1, "Tháng " + item.Month);

                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Tổng số yêu cầu");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Chưa gửi phiếu");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Đang xử lý");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Dừng xử lý");

                        edColBd += 2;
                        CreateCellTypeStr(sheet, 6, 6, edColBd, edColBd + 1, headerRow2, "Đã được duyệt");
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
                        foreach (var mon in fac.MonthDetails)
                        {
                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalRequest);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalUnSent);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalOnGoing);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalStop);

                            indexCol += 2;
                            CreateCellTypeNum(sheet, edRowBd, edRowBd, indexCol, indexCol + 1, bodyRow, mon.TotalApproved);
                        }
                        indexCol += 2;
                        bodyRow.CreateCell(indexCol).SetCellValue(fac.Total);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmReportService: RequestAmReportGeneralGenerateFile", ex.ToString());
                return false;
            }
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

        #region Xử lý send mail và notification
        private List<string> CreateReportFile()
        {
            List<string> fileAttchments = new List<string>();

            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            DateTime currentDate = DateTime.Now;
            string folderReportName = Path.Combine(CommonConstant.FOLDER_REPORT_AM, currentDate.ToString("ddMMyyyy"));
            string folderReportPath = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, currentDate.ToString("ddMMyyyy"));

            // Tạo thư mục lưu report
            if (!Directory.Exists(folderReportPath))
            {
                Directory.CreateDirectory(folderReportPath);
            }

            // Tạo file báo cáo tổng hợp Tuân thủ hoạt động AM
            string fileName1 = Path.Combine(folderReportName, $"Bao_cao_tong_hop_tuan_thu_hoat_dong_am_{currentDate:ddMMyyyy}.xlsx");
            string filePath1 = Path.Combine(_rootPath, fileName1);
            SearchOperationalComplianceModel searchOperationalComplianceModel = new SearchOperationalComplianceModel
            {
                Year = currentDate.Year,
                PageSize = int.MaxValue,
                PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER,
                IsMailReport = true,
            };

            var data1 = GroupDataSearchModels(searchOperationalComplianceModel);
            bool isCreateReport1 = CreateGeneralActiveAmFile(data1, filePath1);
            if (isCreateReport1)
            {
                fileAttchments.Add(filePath1);
            }

            // Tạo file báo cáo tổng hợp Bất thường thiết bị
            string fileName2 = Path.Combine(folderReportName, $"Bao_cao_tong_hop_bat_thuong_thiet_bị_{currentDate:ddMMyyyy}.xlsx");
            string filePath2 = Path.Combine(_rootPath, fileName2);
            AbnormalIncidentSearch abnormalIncidentSearch = new AbnormalIncidentSearch()
            {
                Year = currentDate.Year,
                PageSize = int.MaxValue,
                PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER,
                IsMailReport = true,
                Title = $"Dữ liệu từ ngày {DateTime.Now.AddDays(-7):dd/MM/yyyy} - {DateTime.Now.AddDays(-1):dd/MM/yyyy}",
            };
            var data2 = GetDeviceAbnormalDataGeneral(abnormalIncidentSearch);
            bool isCreateReport2 = CreateReportGeneral(data2, filePath2, abnormalIncidentSearch);
            if (isCreateReport2)
            {
                fileAttchments.Add(filePath2);
            }

            // Tạo file báo cáo tổng hợp Thời gian xử lý bất thường
            string fileName3 = Path.Combine(folderReportName, $"Bao_cao_tong_hop_thoi_gian_xu_ly_bat_thuong_{currentDate:ddMMyyyy}.xlsx");
            string filePath3 = Path.Combine(_rootPath, fileName3);
            var data3 = GetDataGeneralTimeHandle();
            bool isCreateReport3 = CreateGeneralTimeHandle(data3, filePath3);

            if (isCreateReport3)
            {
                fileAttchments.Add(filePath3);
            }

            // Tạo file báo cáo tổng hợp Yêu cầu bảo trì
            string fileName4 = Path.Combine(folderReportName, $"Bao_cao_tong_hop_yeu_cau_bao_tri_{currentDate:ddMMyyyy}.xlsx");
            string filePath4 = Path.Combine(_rootPath, fileName4);
            SearchPrtReportModel searchPrtReportModel = new SearchPrtReportModel()
            {
                Year = currentDate.Year,
                PageSize = int.MaxValue,
                PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER,
                IsMailReport = true,
                Title = $"Dữ liệu từ ngày {DateTime.Now.AddDays(-7):dd/MM/yyyy} - {DateTime.Now.AddDays(-1):dd/MM/yyyy}",
            };
            var data4 = GetDataRequestAmReportGeneral(searchPrtReportModel);
            bool isCreateReport4 = RequestAmReportGeneralGenerateFile(searchPrtReportModel, data4, filePath4);
            if (isCreateReport4)
            {
                fileAttchments.Add(filePath4);
            }

            // Tạo file báo cáo tổng hợp Tiến trình xử lý
            string fileName5 = Path.Combine(folderReportName, $"Bao_cao_tong_hop_tien_trinh_xu_ly_{currentDate:ddMMyyyy}.xlsx");
            string filePath5 = Path.Combine(_rootPath, fileName5);
            SearchPrtReportModel requestModel = new SearchPrtReportModel
            {
                SortOrder = "ASC",
                PageSize = int.MaxValue,
                PageNumber = CommonConstant.DEFAULT_PAGE_NUMBER,
                IsMailReport = true
            };
            var data5 = GetDataRptyReportDetail(requestModel);
            bool isCreateReport5 = ReportGeneralGenerateFile(data5, filePath5);
            if (isCreateReport5)
            {
                fileAttchments.Add(filePath5);
            }

            return fileAttchments;
        }

        private void ProcessMailAndNotification(List<string> fileAttchments)
        {
            //[Notification-17]
            //get system data
            string module = EnumExtensions.GetEnumDescription(EnumModuleNotification.AM);
            string notification = EnumExtensions.GetEnumDescription(EnumNotificationType.AmStatistic);
            int? moduleId = _unitOfWork.ElogbookRepository<SystemData>().GetQuery(x => x.Description == module && x.DataType == 2).Select(x => x.ID).FirstOrDefault();
            int? notificationId = _unitOfWork.ElogbookRepository<SystemData>().GetQuery(x => x.Description == notification && x.DataType == 1).Select(x => x.ID).FirstOrDefault();

            // get mail template
            if (moduleId > 0 && notificationId > 0)
            {
                var mailTemplate = _emailTemplateService.GetTemplateByModuleAndNotification(moduleId.Value, notificationId.Value);
                if (mailTemplate != null)
                {
                    var mailConfig = _emailConfigurationService.GetEmailConfiguration();

                    //create Email Transmissions
                    EmailTransmissionModel transmissionsModel = new EmailTransmissionModel()
                    {
                        EmailReplyToAddr = mailConfig.EmailAddress,
                        Subject = mailTemplate.EmailSubject,
                        MessageBody = mailTemplate.EmailContent,
                        IsBodyHtml = true,
                        DetailFailure = "",
                    };

                    List<UserModel> lstUser = new List<UserModel>();
                    List<string> listPermission = new List<string> { "VIEW_DASHBOARD" };
                    foreach (var permission in listPermission)
                    {
                        var users = GetRelatedUsers(permission);
                        if (users.Any())
                        {
                            foreach (var user in users)
                            {
                                var isExistUser = lstUser.Find(x => x.ID == user.ID);
                                if (!string.IsNullOrWhiteSpace(user.Email) && isExistUser == null)
                                {
                                    lstUser.Add(user);
                                }
                            }
                        }
                    }

                    // Send mail
                    SendEmail(transmissionsModel, lstUser, fileAttchments);

                    // Add Notification
                    if (lstUser.Count > 0)
                    {
                        var system = _systemConfigurationService.GetAppVersion();
                        DateTime currentDate = DateTime.Now;
                        foreach (var userInfo in lstUser)
                        {
                            if (userInfo.ID > 0)
                            {
                                string titleNotice = transmissionsModel.Subject.Replace(CommonConstant.MAIL_MERGE_TITLE + " - ", string.Empty);
                                string body = transmissionsModel.MessageBody.Replace("[Account User]", userInfo.FullName);

                                if (system != null)
                                {
                                    body = body.Replace("[He thong Elogbook]", system.AppTitle);
                                }
                                body = body.Replace("[Account User]", userInfo.FullName);
                                body = body.Replace("[thang gui bao cao]", currentDate.Month.ToString());
                                body = body.Replace("[Ngay gui bao cao]", currentDate.ToString("dd/MM/yyyy"));
                                body = body.Replace("[Link he thong]", AppSettings.DOMAIN);

                                var entityAlertNotification = new AlertNotification()
                                {
                                    NoticeTitle = titleNotice + " - Ngày" + currentDate.ToString("dd/MM/yyyy"),
                                    NoticeContent = body,
                                    UserID = userInfo.ID,
                                    Status = (int)EnumNotification.Unread,
                                    IsImportant = false,
                                    InsertedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now
                                };

                                _unitOfWork.ElogbookRepository<AlertNotification>().Add(entityAlertNotification);
                            }
                        }
                        _unitOfWork.Complete();
                    }
                }
            }
        }

        private List<UserModel> GetRelatedUsers(string permissionKey)
        {
            var roleIDs = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.Permissions.Contains(permissionKey)).Select(x => x.ID).ToList();
            var userIDs = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => roleIDs.Contains(x.UserRoleID)).Select(x => x.UserID).ToList();

            List<UserModel> userModels = _unitOfWork.ElogbookRepository<User>()
                .GetQuery(x => userIDs.Contains(x.ID) && !x.IsDeleted).Select(
                e => new UserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    Email = e.Email,
                    UserName = e.UserName
                }).ToList();

            return userModels;
        }

        private void SendEmail(EmailTransmissionModel item, List<UserModel> users, List<string> fileAttchments)
        {
            var dataEmailSetting = GetEmailConfiguration();
            DateTime currentDate = DateTime.Now;
            var smtpSend = new SmtpClient
            {
                Host = dataEmailSetting.EmailServer,
                Port = dataEmailSetting.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(dataEmailSetting.UserName, dataEmailSetting.PassWord),
                EnableSsl = true,
                Timeout = 20000
            };
            var system = _systemConfigurationService.GetAppVersion();
            //replace subject
            item.Subject = item.Subject.Replace("[He thong Elogbook]", system?.AppTitle);
            var bodyRoot = item.MessageBody;

            if (system != null)
            {
                bodyRoot = bodyRoot.Replace("[He thong Elogbook]", system.AppTitle);
            }

            // Gửi mail cho từng nhân viên
            foreach (var us in users)
            {
                if (!string.IsNullOrWhiteSpace(us.Email))
                {
                    var body = bodyRoot.Replace("[Account User]", us.FullName);
                    body = body.Replace("[thang gui bao cao]", currentDate.Month.ToString());
                    body = body.Replace("[Ngay gui bao cao]", currentDate.ToString("dd/MM/yyyy"));
                    body = body.Replace("[Link he thong]", AppSettings.DOMAIN);

                    // Send mail
                    using (var mailMessage = new MailMessage())
                    {
                        try
                        {
                            mailMessage.Sender = new MailAddress(dataEmailSetting.EmailAddress);
                            mailMessage.From = new MailAddress(dataEmailSetting.EmailAddress);

                            //Subject & Body
                            mailMessage.Subject = item.Subject;
                            mailMessage.Body = body;
                            mailMessage.BodyEncoding = Encoding.UTF8;
                            mailMessage.IsBodyHtml = true;
                            mailMessage.To.Add(us.Email);

                            // Attchment
                            if (fileAttchments.Count > 0)
                            {
                                foreach (var attachment in fileAttchments)
                                {
                                    if (!string.IsNullOrWhiteSpace(attachment))
                                    {
                                        System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet);
                                        ContentDisposition disposition = attach.ContentDisposition;
                                        disposition.CreationDate = File.GetCreationTime(attachment);
                                        disposition.ModificationDate = File.GetLastWriteTime(attachment);
                                        disposition.ReadDate = File.GetLastAccessTime(attachment);
                                        disposition.FileName = Path.GetFileName(attachment);
                                        disposition.Size = new FileInfo(attachment).Length;
                                        disposition.DispositionType = DispositionTypeNames.Attachment;
                                        mailMessage.Attachments.Add(attach);
                                    }
                                }
                            }

                            smtpSend.Send(mailMessage);
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.INFO, "AM Report: Send mail report", "Eror send mail: " + us.Email
                                + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }

            // Gửi mail cho nhà máy
            var factorys = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted).ToList();
            if (factorys.Count > 0)
            {
                foreach (var factory in factorys)
                {
                    if (!string.IsNullOrWhiteSpace(factory.Email))
                    {
                        var body = bodyRoot.Replace("[Account User]", factory.FactoryName);
                        body = body.Replace("[thang gui bao cao]", currentDate.Month.ToString());
                        body = body.Replace("[Ngay gui bao cao]", currentDate.ToString("dd/MM/yyyy"));
                        body = body.Replace("[Link he thong]", AppSettings.DOMAIN);

                        // Send mail
                        using (var mailMessage = new MailMessage())
                        {
                            try
                            {
                                mailMessage.Sender = new MailAddress(dataEmailSetting.EmailAddress);
                                mailMessage.From = new MailAddress(dataEmailSetting.EmailAddress);

                                //Subject & Body
                                mailMessage.Subject = item.Subject;
                                mailMessage.Body = body;
                                mailMessage.BodyEncoding = Encoding.UTF8;
                                mailMessage.IsBodyHtml = true;
                                mailMessage.To.Add(factory.Email);

                                // Attchment
                                if (fileAttchments.Count > 0)
                                {
                                    foreach (var attachment in fileAttchments)
                                    {
                                        if (!string.IsNullOrWhiteSpace(attachment))
                                        {
                                            System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet);
                                            ContentDisposition disposition = attach.ContentDisposition;
                                            disposition.CreationDate = File.GetCreationTime(attachment);
                                            disposition.ModificationDate = File.GetLastWriteTime(attachment);
                                            disposition.ReadDate = File.GetLastAccessTime(attachment);
                                            disposition.FileName = Path.GetFileName(attachment);
                                            disposition.Size = new FileInfo(attachment).Length;
                                            disposition.DispositionType = DispositionTypeNames.Attachment;
                                            mailMessage.Attachments.Add(attach);
                                        }
                                    }
                                }

                                smtpSend.Send(mailMessage);
                            }
                            catch (Exception ex)
                            {
                                LogCustom.LogMessage(TrackingLogLevel.INFO, "AM Report: Send mail report", "Eror send mail: " + factory.Email
                                    + Environment.NewLine + ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private EmailConfigurationResponseModel GetEmailConfiguration()
        {
            var data = _unitOfWork.ElogbookRepository<EmailConfiguration>().GetQuery().Select(
                        e => new EmailConfigurationResponseModel()
                        {
                            ID = e.ID,
                            EmailAddress = e.EmailAddress,
                            EmailServer = e.EmailServer,
                            Port = e.Port,
                            EmailType = e.EmailType,
                            EmailTypeInfo = new ValueLabelModel { Value = e.EmailType, Label = "SMTP" },
                            UserName = e.UserName,
                            PassWord = e.PassWord,
                        }).FirstOrDefault();
            if (data != null)
            {
                data.PassWord = EncryptDecrypt.Decrypt(data.PassWord);
            }

            return data;
        }
        #endregion

        #region Bất thường thiết bị
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

            if (requestModel.IsMailReport)
            {
                DateTime bef1Date = currentDate.AddDays(-1);
                DateTime bef7Date = bef1Date.AddDays(-6);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) >= bef7Date.Date);
                entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= bef1Date.Date);
            }

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
        #endregion

        #endregion
    }
}
