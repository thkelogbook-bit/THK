using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.InspectionReport;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.InspectionReport;
using Elogbook.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using static iTextSharp.text.pdf.AcroFields;

namespace Elogbook.Domain.Services
{
    public class InspectionReportService : IInspectionReportService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fBold = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fItalic = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesi.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly int tblWidth = 510;
        private readonly int TABLE_WIDTH = 510;

        public InspectionReportService(ElogbookUnitOfWork unitOfWork,
            ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        /// <summary>
        /// Tìm kiếm những BBKSHT theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetAll(SearchInspectionReportModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var entities = _unitOfWork.ElogbookRepository<InspectionReport>().GetQuery(x => !x.IsDeleted);

            //Số BBKSHT
            if (!string.IsNullOrWhiteSpace(requestModel.TicketNumber))
            {
                entities = entities.Where(x => x.TicketNumber.Contains(requestModel.TicketNumber));
            }

            //Trạng thái phiếu
            if (requestModel.StatusValue > 0)
            {
                entities = entities.Where(x => x.StatusValue == requestModel.StatusValue);
            }

            // Tìm kiếm theo đơn vị đăng ký
            if (requestModel.ProductionPlantID > 0)
            {
                var organizations = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && x.Active == (int)AccessStatus.Active
                                                                                    && x.ID == requestModel.ProductionPlantID).FirstOrDefault();
                if (organizations != null && organizations.Level.Equals("Tổ"))
                {
                    entities = entities.Where(x => x.ProductionPlant.ID == organizations.ParentID);
                }
                else
                {
                    entities = entities.Where(x => x.ProductionPlant.ID == requestModel.ProductionPlantID);
                }
            }

            //Ngày đăng ký phiếu (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date);
            }
            //Ngày đăng ký phiếu (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date);
            }


            if (requestModel.IsAssigned != null)
            {
                var tickets = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => !x.IsDeleted && x.InspectionReportID != null).Select(x => x.InspectionReportID).ToList();
                if (requestModel.IsAssigned.Value)
                {
                    entities = entities.Where(x => !tickets.Contains(x.ID));
                }
                else
                {
                    entities = entities.Where(x => !tickets.Contains(x.ID));
                }
            }
            // Return and paging data
            var page = entities.Select(
                e => new GeneralInspectionReportModel()
                {
                    ID = e.ID,
                    TicketNumber = e.TicketNumber,
                    RegistrationDate = e.InsertedAt,
                    CompletedDateTime = e.CompletedDate,
                    ProductionPlantName = e.ProductionPlant.Name,
                    CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    StatusValue = (InspectionReportStatus)e.StatusValue
                })
                .OrderByDescending(t => t.RegistrationDate)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Tạo mới BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public InspectionReportDetailsModel Add(AddEditInspectionReporModel requestModel)
        {
            // Get User Info
            var user = _unitOfWork.ElogbookRepository<User>().GetById(requestModel.CurrentUserID);
            if (user != null)
            {
                requestModel.ProductionPlantID = user.ProductionPlant.ID;
                requestModel.ProductionPlantName = user.ProductionPlant.Name;
            }

            InspectionReport entity;
            int currentYear = DateTime.Now.Year;
            var ticketNumber = $"1/{currentYear}/BBKSHT";
            var inspectionReport = _unitOfWork.ElogbookRepository<InspectionReport>().GetQuery(x => x.InsertedAt.Year == currentYear)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();
            if (inspectionReport != null)
            {
                var arrNumTicket = inspectionReport.TicketNumber.Split('/');
                int.TryParse(arrNumTicket[0], out int numTicket);
                ticketNumber = $"{numTicket + 1}/{currentYear}/BBKSHT";
            }
            entity = new InspectionReport
            {
                TicketNumber = ticketNumber,
                FactoryID = requestModel.FactoryID,
                SurveyTime = requestModel.SurveyTime,
                ProductionPlantName = requestModel.ProductionPlantName,
                ProductionPlantID = requestModel.ProductionPlantID,
                Address = requestModel.Address,
                WorkPlacement = requestModel.WorkPlacement,
                WorkContent = requestModel.WorkContent,
                InspectionScope = requestModel.InspectionScope,
                PlanStartDate = requestModel.PlanStartDate,
                PlanEndDate = requestModel.PlanEndDate,
                OperationUnitResponsibilities = requestModel.OperationUnitResponsibilities,
                WorkUnitResponsibilities = requestModel.WorkUnitResponsibilities,
                DispatchUnitResponsibilities = requestModel.DispatchUnitResponsibilities,
                OtherWorkRelatedMatters = requestModel.OtherWorkRelatedMatters,
                //Trạng thái: 1/Mới tạo
                StatusValue = 1,
                Attachments = requestModel.FileAttachments != null && requestModel.FileAttachments.Any() ? string.Join(",", requestModel.FileAttachments) : string.Empty,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            // 1 Đại diện Đơn vị công tác
            List<RelatedUser> relatedUsers = new List<RelatedUser>();
            if (requestModel.UnitRepresentatives != null)
            {
                relatedUsers.AddRange(MappingToRelatedUser(requestModel.UnitRepresentatives, 1, requestModel.CurrentUserID));
            }

            // 2 Đại diện (các) Đơn vị QLVH
            if (requestModel.OmUnitRepresentatives != null)
            {
                relatedUsers.AddRange(MappingToRelatedUser(requestModel.OmUnitRepresentatives, 2, requestModel.CurrentUserID));

            }

            // 3. Đại diện các đơn vị có liên quan
            if (requestModel.RelatedUnitRepresentatives != null && requestModel.RelatedUnitRepresentatives.Count > 0)
            {
                relatedUsers.AddRange(MappingToRelatedUser(requestModel.RelatedUnitRepresentatives, 3, requestModel.CurrentUserID));
            }

            if (relatedUsers.Count > 0)
            {
                entity.RelatedUsers = relatedUsers;
            }
            _unitOfWork.ElogbookRepository<InspectionReport>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            return GetById(entity.ID);

        }

        /// <summary>
        /// Lấy thông tin BBKSHT theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InspectionReportDetailsModel GetById(int id)
        {
            var inspectionReport = _unitOfWork.ElogbookRepository<InspectionReport>().GetById(id);
            if (inspectionReport != null)
            {
                return MappingToInspectionReportDetailsModel(inspectionReport);
            }
            return null;
        }

        /// <summary>
        /// Cập nhật BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public InspectionReportDetailsModel Update(AddEditInspectionReporModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<InspectionReport>().GetById(requestModel.ID);
            // Get User Info
            var user = _unitOfWork.ElogbookRepository<User>().GetById(requestModel.CurrentUserID);
            if (user != null)
            {
                requestModel.ProductionPlantID = user.ProductionPlant.ID;
                requestModel.ProductionPlantName = user.ProductionPlant.Name;
            }

            entity.FactoryID = requestModel.FactoryID;
            entity.SurveyTime = requestModel.SurveyTime;
            entity.ProductionPlantName = requestModel.ProductionPlantName;
            entity.ProductionPlantID = requestModel.ProductionPlantID;
            entity.Address = requestModel.Address;
            entity.WorkPlacement = requestModel.WorkPlacement;
            entity.WorkContent = requestModel.WorkContent;
            entity.InspectionScope = requestModel.InspectionScope;
            entity.PlanStartDate = requestModel.PlanStartDate;
            entity.PlanEndDate = requestModel.PlanEndDate;
            entity.OperationUnitResponsibilities = requestModel.OperationUnitResponsibilities;
            entity.WorkUnitResponsibilities = requestModel.WorkUnitResponsibilities;
            entity.DispatchUnitResponsibilities = requestModel.DispatchUnitResponsibilities;
            entity.OtherWorkRelatedMatters = requestModel.OtherWorkRelatedMatters;
            //Trạng thái: 1/Mới tạo
            entity.StatusValue = 2;
            entity.LastUpdatedUserID = requestModel.CurrentUserID;

            // Đại diện Đơn vị công tác
            List<RelatedUser> relatedUsers = new List<RelatedUser>();
            if (requestModel.UnitRepresentatives != null && requestModel.UnitRepresentatives.Count > 0)
            {
                relatedUsers.AddRange(MappingToRelatedUser(requestModel.UnitRepresentatives.Where(u => !entity.RelatedUsers.Any(r => r.UserName == u.UserName && r.Roles == 1)).ToList(), 1, requestModel.CurrentUserID));
                relatedUsers.AddRange(entity.RelatedUsers.Where(u => requestModel.UnitRepresentatives.Any(r => r.UserName == u.UserName && u.Roles == 1)).ToList());
            }

            // Đại diện (các) Đơn vị QLVH
            if (requestModel.OmUnitRepresentatives != null && requestModel.OmUnitRepresentatives.Count > 0)
            {
                relatedUsers.AddRange(MappingToRelatedUser(requestModel.OmUnitRepresentatives.Where(u => !entity.RelatedUsers.Any(r => r.UserName == u.UserName && r.Roles == 2)).ToList(), 2, requestModel.CurrentUserID));
                relatedUsers.AddRange(entity.RelatedUsers.Where(u => requestModel.OmUnitRepresentatives.Any(r => r.UserName == u.UserName && u.Roles == 2)).ToList());

            }

            // 3. Đại diện các đơn vị có liên quan
            if (requestModel.RelatedUnitRepresentatives != null && requestModel.RelatedUnitRepresentatives.Count > 0)
            {
                relatedUsers.AddRange(MappingToRelatedUser(requestModel.RelatedUnitRepresentatives.Where(u => !entity.RelatedUsers.Any(r => r.UserName == u.UserName && r.Roles == 3)).ToList(), 3, requestModel.CurrentUserID));
                relatedUsers.AddRange(entity.RelatedUsers.Where(u => requestModel.RelatedUnitRepresentatives.Any(r => r.UserName == u.UserName && u.Roles == 3)).ToList());
            }
            List<RelatedUser> relatedUsersDels = entity.RelatedUsers.Where(u => !relatedUsers.Any(r => r.UserName == u.UserName && r.Roles == u.Roles)).ToList();
            if (relatedUsersDels.Count > 0)
            {
                _unitOfWork.ElogbookRepository<RelatedUser>().Delete(relatedUsersDels, requestModel.CurrentUserID, true);
            }
            if (relatedUsers.Count > 0)
            {
                entity.RelatedUsers = relatedUsers;
            }

            _unitOfWork.ElogbookRepository<InspectionReport>().Update(entity);
            _unitOfWork.Complete();
            return GetById(requestModel.ID);
        }

        /// <summary>
        /// Xóa BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public bool Delete(int id, int currentUserID)
        {
            var entity = _unitOfWork.ElogbookRepository<InspectionReport>().GetById(id);
            if (entity != null)
            {
                entity.LastUpdatedUserID = currentUserID;
                entity.IsDeleted = true;
                _unitOfWork.ElogbookRepository<InspectionReport>().Update(entity);
                _unitOfWork.Complete();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ký số
        /// </summary>
        /// <param name="signgingModel"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public UserSigningModel Signging(SigngingModel signgingModel)
        {
            var entity = _unitOfWork.ElogbookRepository<InspectionReport>().GetById(signgingModel.InspectionReportId);
            if (entity != null && entity.RelatedUsers != null && entity.RelatedUsers.Any(r => r.UserName.ToLower() == signgingModel.UserName.ToLower() && r.Roles == signgingModel.Roles))
            {
                var relatedUser = entity.RelatedUsers.Where(r => r.UserName.ToLower() == signgingModel.UserName.ToLower() && r.Roles == signgingModel.Roles).First();
                UserInfo userInfo = _commonService.GetUserById(signgingModel.UserName, relatedUser.IsExternal);
                relatedUser.IsSigning = true;
                if (userInfo.IsExternal)
                {
                    relatedUser.Signature = signgingModel.Signature;
                }
                else
                {
                    relatedUser.Signature = userInfo.Signature;
                }
                entity.LastUpdatedUserID = userInfo.ID;
                if (entity.RelatedUsers.Any(r => r.Roles == 1 && r.IsSigning)
                    && entity.RelatedUsers.Any(r => r.Roles == 2 && r.IsSigning))
                {
                    if (entity.RelatedUsers.Any(r => r.Roles == 3))
                    {
                        if (entity.RelatedUsers.Any(r => r.Roles == 3 && r.IsSigning))
                        {
                            entity.CompletedDate = DateTime.UtcNow;
                            entity.StatusValue = (int)InspectionReportStatus.Complete;
                        }
                    }
                    else
                    {
                        entity.CompletedDate = DateTime.UtcNow;
                        entity.StatusValue = (int)InspectionReportStatus.Complete;
                    }

                }
                _unitOfWork.ElogbookRepository<InspectionReport>().Update(entity);
                _unitOfWork.Complete();
                if (userInfo != null)
                {
                    return new UserSigningModel
                    {
                        RelatedId = entity.ID,
                        UserId = userInfo.ID,
                        FullName = userInfo.FullName,
                        Signature = relatedUser.Signature,
                        InspectionReportStatus = (InspectionReportStatus)entity.StatusValue,
                        CreatedDateTime = relatedUser.InsertedAt
                    };
                }

            }
            return null;
        }

        /// <summary>
        /// In BBKSHT
        /// </summary>
        /// <param name="id">ID BBKSHT</param>
        /// <returns></returns>
        public HttpResponseMessage ExportPdfFile(int id)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            var details = GetById(id);

            if (details == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // tạo thư mục lưu file pdf
            if (!Directory.Exists(Path.Combine(_rootPath, "InspectionReport", "PDF")))
            {
                Directory.CreateDirectory(Path.Combine(_rootPath, "InspectionReport", "PDF"));
            }
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = $"InspectionReport_{currentDate}.pdf";
            string outputFile = Path.Combine(_rootPath, "InspectionReport", "PDF", fileName);

            bool isCreatePDF = GeneratePdfFile(details, outputFile);
            if (isCreatePDF)
            {
                var fileBytes = File.ReadAllBytes(outputFile);
                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Mapping data entity Inspection Report to view details
        /// </summary>
        /// <param name="inspectionReport"></param>
        /// <returns></returns>
        private InspectionReportDetailsModel MappingToInspectionReportDetailsModel(InspectionReport inspectionReport)
        {
            var inspectionReportDetailsModel = new InspectionReportDetailsModel()
            {
                ID = inspectionReport.ID,
                TicketNumber = inspectionReport.TicketNumber,
                FactoryID = inspectionReport.FactoryID,
                SurveyDateTime = inspectionReport.SurveyTime,
                ProductionPlantName = inspectionReport.ProductionPlantName,
                ProductionPlantID = inspectionReport.ProductionPlantID,
                Address = inspectionReport.Address,
                WorkPlacement = inspectionReport.WorkPlacement,
                WorkContent = inspectionReport.WorkContent,
                InspectionScope = inspectionReport.InspectionScope,
                PlanStartDate = inspectionReport.PlanStartDate,
                PlanEndDate = inspectionReport.PlanEndDate,
                OperationUnitResponsibilities = inspectionReport.OperationUnitResponsibilities,
                WorkUnitResponsibilities = inspectionReport.WorkUnitResponsibilities,
                DispatchUnitResponsibilities = inspectionReport.DispatchUnitResponsibilities,
                OtherWorkRelatedMatters = inspectionReport.OtherWorkRelatedMatters,
                StatusValue = (InspectionReportStatus)inspectionReport.StatusValue,
                RegistrationDate = inspectionReport.InsertedAt,
                CompletedDateTime = inspectionReport.CompletedDate,
                ProductionPlant = inspectionReport.ProductionPlant == null ? null : new ValueLabelModel { Value = inspectionReport.ProductionPlant.ID, Label = inspectionReport.ProductionPlant.Name },
                Factory = inspectionReport.Factory == null ? null : new ValueLabelModel { Value = inspectionReport.Factory.ID, Label = inspectionReport.Factory.FactoryName },
                FileAttachments = !string.IsNullOrWhiteSpace(inspectionReport.Attachments) ? inspectionReport.Attachments.Split(',').Select(int.Parse).ToList() : new List<int>(),
                CreatedUser = new ValueLabelModel { Value = inspectionReport.CreatedUser.ID, Label = inspectionReport.CreatedUser.FullName },
                UpdatedUser = new ValueLabelModel { Value = inspectionReport.LastUpdatedUser.ID, Label = inspectionReport.LastUpdatedUser.FullName },
            };
            List<RelatedUser> relatedUsers = new List<RelatedUser>();
            if (inspectionReport.RelatedUsers != null && inspectionReport.RelatedUsers.Count > 0)
            {
                // Đại diện Đơn vị công tác
                inspectionReportDetailsModel.UnitRepresentatives.AddRange(MappingToReRelatedUserModel(inspectionReport.RelatedUsers.Where(r => r.Roles == 1).ToList()));
                // Đại diện (các) Đơn vị QLVH
                inspectionReportDetailsModel.OmUnitRepresentatives.AddRange(MappingToReRelatedUserModel(inspectionReport.RelatedUsers.Where(r => r.Roles == 2).ToList()));
                // 3. Đại diện các đơn vị có liên quan
                inspectionReportDetailsModel.RelatedUnitRepresentatives.AddRange(MappingToReRelatedUserModel(inspectionReport.RelatedUsers.Where(r => r.Roles == 3).ToList()));

                inspectionReportDetailsModel.UnitRepresentativesSigning.AddRange(MappingToUserSigningModel(inspectionReport.RelatedUsers.Where(r => r.Roles == 1 && r.IsSigning).ToList()));
                inspectionReportDetailsModel.OmUnitRepresentativesSigning.AddRange(MappingToUserSigningModel(inspectionReport.RelatedUsers.Where(r => r.Roles == 2 && r.IsSigning).ToList()));
                inspectionReportDetailsModel.RelatedUnitRepresentativesSigning.AddRange(MappingToUserSigningModel(inspectionReport.RelatedUsers.Where(r => r.Roles == 3 && r.IsSigning).ToList()));
            }
            if (inspectionReportDetailsModel.FileAttachments != null && inspectionReportDetailsModel.FileAttachments.Count > 0)
            {
                var attachments = _unitOfWork.ElogbookRepository<Data.Attachment>().GetQuery(x => !x.IsDeleted && inspectionReportDetailsModel.FileAttachments.Contains(x.ID)).ToList();
                if (attachments.Count > 0)
                {
                    var attachmentModels = new List<AttachmentModel>();
                    foreach (var attachment in attachments)
                    {
                        attachmentModels.Add(new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        });
                    }
                    inspectionReportDetailsModel.Attachments = attachmentModels;
                }
            }
            return inspectionReportDetailsModel;
        }

        /// <summary>
        /// Mapping data related user to user signing model
        /// </summary>
        /// <param name="relatedUsers"></param>
        /// <returns></returns>
        private List<UserSigningModel> MappingToUserSigningModel(List<RelatedUser> relatedUsers)
        {
            List<UserSigningModel> userSignings = new List<UserSigningModel>();
            foreach (var item in relatedUsers)
            {
                UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                if (userInfo != null)
                {
                    var userSign = new UserSigningModel
                    {
                        RelatedId = item.ID,
                        UserId = userInfo.ID,
                        FullName = userInfo.FullName,
                        Signature = userInfo.Signature,
                        CreatedDateTime = item.InsertedAt
                    };
                    if (item.IsExternal)
                    {
                        userSign.Signature = item.Signature;
                    }
                    userSignings.Add(userSign);
                }
            }
            return userSignings;
        }

        /// <summary>
        /// Mapping data model of related user to entity
        /// </summary>
        /// <param name="relatedUserModels"></param>
        /// <param name="type"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        private List<RelatedUser> MappingToRelatedUser(List<RelatedUserModel> relatedUserModels, int type, int currentUserId)
        {
            List<RelatedUser> relatedUsers = new List<RelatedUser>();
            foreach (var item in relatedUserModels)
            {
                UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                if (userInfo != null)
                {
                    relatedUsers.Add(new RelatedUser
                    {
                        AssignToUserID = userInfo.ID,
                        FullName = userInfo.FullName,
                        ElectricRank = userInfo.ElectricRankStr,
                        LaborRank = userInfo.LaborRankStr,
                        LaborCard = userInfo.LaborCard,
                        UserName = userInfo.UserName,
                        IsExternal = item.IsExternal,
                        Roles = type,
                        PositionName = userInfo.PositionName,
                        CreatedUserID = currentUserId,
                        LastUpdatedUserID = currentUserId,
                    });
                }
            }
            return relatedUsers;
        }

        /// <summary>
        /// Mapping data from entity to user related model
        /// </summary>
        /// <param name="relatedUsers"></param>
        /// <returns></returns>
        private List<RelatedUserModel> MappingToReRelatedUserModel(List<RelatedUser> relatedUsers)
        {
            List<RelatedUserModel> relatedUserModels = new List<RelatedUserModel>();
            foreach (var item in relatedUsers)
            {
                UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                if (userInfo != null)
                {
                    relatedUserModels.Add(new RelatedUserModel
                    {
                        ID = item.ID,
                        AssignToUserID = userInfo.ID,
                        FullName = userInfo.FullName,
                        ElectricRank = userInfo.ElectricRankStr,
                        LaborRank = userInfo.LaborRankStr,
                        LaborCard = userInfo.LaborCard,
                        UserName = userInfo.UserName,
                        IsExternal = item.IsExternal,
                        PositionName = userInfo.PositionName,
                        ProductionPlant = userInfo.ProductionPlant,
                        Roles = relatedUsers.First().Roles,
                    });
                }
            }
            return relatedUserModels;
        }

        private bool GeneratePdfFile(InspectionReportDetailsModel inspectionReportDetails, string outputFile)
        {
            try
            {
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    //Create document
                    Document pdfDocument = new Document(PageSize.A4);
                    pdfDocument.SetMargins(5, 5, 25, 5);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, fileStream);

                    var factoryName = string.Empty;
                    var companyName = string.Empty;
                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == inspectionReportDetails.FactoryID).FirstOrDefault();
                    if (factory != null)
                    {
                        factoryName = factory.FactoryName;
                        companyName = factory.Company == null ? string.Empty : factory.Company.Name;
                    }

                    pdfDocument.Open();
                    try
                    {
                        PdfPTable mainTable = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
                        mainTable.DefaultCell.Border = Rectangle.NO_BORDER;
                        mainTable.SetWidths(new float[] { 2, 2 });
                        mainTable.LockedWidth = true;
                        mainTable.HeaderRows = 1;
                        mainTable.AddCell(GetCellSign(_fBold, 16, "", 3, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                        AddHeader(mainTable, companyName, factoryName, inspectionReportDetails.ProductionPlantName, inspectionReportDetails);
                        AddUnitRepresentatives(mainTable, inspectionReportDetails);
                        AddOmUnitRepresentatives(mainTable, inspectionReportDetails);
                        AddRelatedUnitRepresentatives(mainTable, inspectionReportDetails);
                        AddContent(mainTable, inspectionReportDetails);
                        AddSigntureToPDF(mainTable, inspectionReportDetails);
                        pdfDocument.Add(mainTable);
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();

                        return true;
                    }
                    catch
                    {
                        //ignore exception
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                        return false;
                    }
                    finally
                    {
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void AddHeader(PdfPTable mainTable, string companyName, string factoryName, string productionPlantName, InspectionReportDetailsModel inspectionReportDetails)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = tblWidth };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 3 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fBold, fSize, companyName + "\n" + productionPlantName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM\nĐộc lập - Tự do - Hạnh phúc", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            string surveyTime = inspectionReportDetails.SurveyDateTime.Value.ToString("HH'h'mm’ 'ngày' d/M/yyyy");
            tblData.AddCell(GetCellSign(_fItalic, fSize, $"Gia Lai, ngày {inspectionReportDetails.SurveyDateTime.Value.Day} tháng {inspectionReportDetails.SurveyDateTime.Value.Month} năm {inspectionReportDetails.SurveyDateTime.Value.Year}", 5, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fItalic, 10, "", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fBold, 14, "BIÊN BẢN KHẢO SÁT HIỆN TRƯỜNG", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);

        }

        private void AddUnitRepresentatives(PdfPTable mainTable, InspectionReportDetailsModel inspectionReportDetails)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = tblWidth };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            string surveyTime = inspectionReportDetails.SurveyDateTime.Value.ToString("HH'h'mm’ 'ngày' d/M/yyyy");
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Vào hồi {surveyTime}, tại {inspectionReportDetails.Address}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, "Chúng tôi gồm:", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "1. Đại diện Đơn vị công tác:", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            int stt = 0;
            foreach (RelatedUserModel relatedUser in inspectionReportDetails.UnitRepresentatives)
            {
                stt++;
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"1.{stt}. Ông (Bà): {relatedUser.FullName}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"Chức vụ: {relatedUser.PositionName}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            }
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);
        }

        private void AddOmUnitRepresentatives(PdfPTable mainTable, InspectionReportDetailsModel inspectionReportDetails)
        {
            PdfPTable tblData = new PdfPTable(3) { TotalWidth = tblWidth };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 3, 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fBold, fSize, "2. Đại diện (các) Đơn vị QLVH:", 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            int stt = 0;
            foreach (RelatedUserModel relatedUser in inspectionReportDetails.OmUnitRepresentatives)
            {
                stt++;
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"2.{stt}. Ông (Bà): {relatedUser.FullName}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"Chức vụ: {relatedUser.PositionName}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"Đơn vị: {(relatedUser.ProductionPlant != null ? relatedUser.ProductionPlant.Label : "")}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            }
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);
        }

        private void AddRelatedUnitRepresentatives(PdfPTable mainTable, InspectionReportDetailsModel inspectionReportDetails)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = tblWidth };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fBold, fSize, $"3. Đại diện các đơn vị có liên quan:{(inspectionReportDetails.RelatedUnitRepresentatives != null && inspectionReportDetails.RelatedUnitRepresentatives.Count > 0 ? "" : "..................................")}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            int stt = 0;
            foreach (RelatedUserModel relatedUser in inspectionReportDetails.RelatedUnitRepresentatives)
            {
                stt++;
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"3.{stt}. Ông (Bà): {relatedUser.FullName}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"Bậc ATĐ: {relatedUser.ElectricRank}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            }
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);
        }

        private void AddContent(PdfPTable mainTable, InspectionReportDetailsModel inspectionReportDetails)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = tblWidth };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fNomal, fSize, "   Cùng nhau khảo sát thực tế, trao đổi và thống nhất phân công trách nhiệm thực hiện những nội dung để đảm bảo an toàn về điện cho Đơn vị công tác khi tiến hành công việc, cụ thể như sau:", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"4. Địa điểm (hoặc thiết bị) thực hiện công việc: {inspectionReportDetails.WorkPlacement}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"5. Nội dung công việc: {inspectionReportDetails.WorkContent}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"6. Phạm vi làm việc: {inspectionReportDetails.InspectionScope}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            string startTime = inspectionReportDetails.PlanStartDate.Value.ToString("HH'h'mm 'ngày' d/M/yyyy");
            string endTime = inspectionReportDetails.PlanEndDate.Value.ToString("HH'h'mm 'ngày' d/M/yyyy");
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"7. Thời gian tiến hành công việc: từ {startTime} đến {endTime}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"8.Trách nhiệm của các đơn vị liên quan:", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"a/ Đối với (các) Đơn vị QLVH: {inspectionReportDetails.OperationUnitResponsibilities}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"b/ Đối với đơn vị công tác: {inspectionReportDetails.WorkUnitResponsibilities}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"c/ Đối với đơn vị điều độ: {inspectionReportDetails.DispatchUnitResponsibilities}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"d/ Những nội dung khác có liên quan đến công việc: {inspectionReportDetails.OtherWorkRelatedMatters}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"   Biên bản này được lập thành 02 bản và được tất cả mọi người dự họp là đại diện của các đơn vị có liên quan đến công việc đồng ý, thông qua để làm cơ sở tiến hành công việc sau này (nếu không thay đổi về những nội dung chính) và ký tên dưới đây.", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);
        }

        private void AddSigntureToPDF(PdfPTable mainTable, InspectionReportDetailsModel inspectionReportDetails)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = tblWidth };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fBold, fSize, "ĐƠN VỊ CÔNG TÁC", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "ĐƠN VỊ QLVH", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            string nameUnitRepresentativesSigning = string.Empty;
            string nameOmUnitRepresentativesSigning = string.Empty;
            if (inspectionReportDetails.UnitRepresentativesSigning != null && inspectionReportDetails.UnitRepresentativesSigning.Count > 0)
            {
                tblData.AddCell(GetCellImageSignature(inspectionReportDetails.UnitRepresentativesSigning[0].Signature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
                nameUnitRepresentativesSigning = inspectionReportDetails.UnitRepresentativesSigning[0].FullName;
            }
            else
            {
                tblData.AddCell(GetCellSign(_fBold, fSize, "", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            }

            if (inspectionReportDetails.OmUnitRepresentativesSigning != null && inspectionReportDetails.OmUnitRepresentativesSigning.Count > 0)
            {
                tblData.AddCell(GetCellImageSignature(inspectionReportDetails.OmUnitRepresentativesSigning[0].Signature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
                nameOmUnitRepresentativesSigning = inspectionReportDetails.OmUnitRepresentativesSigning[0].FullName;
            }
            else
            {
                tblData.AddCell(GetCellSign(_fBold, fSize, "", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            }

            tblData.AddCell(GetCellSign(_fNomal, fSize, nameUnitRepresentativesSigning, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
            tblData.AddCell(GetCellSign(_fBold, fSize, nameOmUnitRepresentativesSigning, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));

            tblData.AddCell(GetCellSign(_fBold, fSize, "", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fBold, fSize, "CÁC ĐƠN VỊ LIÊN QUAN KHÁC", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "SƠ ĐỒ MỘT SỢI KẾT NỐI THIẾT BỊ, LƯỚI ĐIỆN NƠI LÀM VIỆC", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fItalic, fSize, "(Kèm theo Biên bản khảo sát hiện trường)", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);
        }

        private PdfPCell GetCellImageSignature(string signatureContent, int colSpan, int rowSpan, int hAlignment, int vAlignment)
        {
            string signatureBase64 = signatureContent.Remove(0, signatureContent.IndexOf(',') + 1);
            byte[] imgBytes = Convert.FromBase64String(signatureBase64);

            Image img = Image.GetInstance(imgBytes);
            float imgHeight = 30;
            float imgWidth = (img.Width * imgHeight) / img.Height;
            img.ScaleAbsolute(imgWidth, imgHeight);

            PdfPCell imageCell = new PdfPCell(img)
            {
                Colspan = colSpan,
                Rowspan = rowSpan,
                BorderColor = BaseColor.GRAY,
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Border = 0
            };
            imageCell.BorderColor = BaseColor.WHITE;

            return imageCell;
        }

        public PdfPCell GetCellSign(BaseFont fBase, float fSize, string textContent, int colSpan, int rowSpan, int hAlignment = 1, int vAlignment = 1, int minHeight = 20)
        {
            Font font = new Font(fBase, fSize);
            PdfPCell cell = new PdfPCell(new Phrase(textContent, font))
            {
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Rowspan = rowSpan,
                Colspan = colSpan,
                MinimumHeight = minHeight,
                Border = 0
            };

            return cell;
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
}
