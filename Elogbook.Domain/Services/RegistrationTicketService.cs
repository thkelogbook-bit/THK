using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.WorkTicket.PDF;
using Elogbook.Core.Models.Response.WorkTicket.PDF881;
using Elogbook.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TicketStatus = Elogbook.Data.TicketStatus;

namespace Elogbook.Domain.Services
{
    public class RegistrationTicketService : IRegistrationTicketService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly IAmRequestTicketService _amRequestTicketService;
        private readonly IEmailService _emailService;
        private readonly IInspectionReportService _inspectionReportService;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fBold = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private PdfInfoModel _pdfInfo;
        private readonly int TABLE_WIDTH = 495;

        public RegistrationTicketService(ElogbookUnitOfWork unitOfWork,
            ICommonService commonService,
            IAmRequestTicketService amRequestTicketService,
            IEmailService emailService,
            IInspectionReportService inspectionReportService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _amRequestTicketService = amRequestTicketService;
            _emailService = emailService;
            _inspectionReportService = inspectionReportService;
        }

        public int AddRegistrationTicket(AddEditRegistrationTicketModel requestModel)
        {
            if (requestModel.IsSave)
            {
                requestModel.RegistrationDate = null;
            }
            else
            {
                requestModel.RegistrationDate = DateTime.Now;
            }

            // Get Factory Info
            var facInfo = _unitOfWork.ElogbookRepository<Factory>().GetById(requestModel.FactoryID);
            requestModel.ReceiveUnitID = facInfo.ProductionPlantID;

            // Get User Info
            var userEntity = _unitOfWork.ElogbookRepository<User>().GetById(requestModel.CurrentUserID);
            requestModel.ProductionPlantID = userEntity.ProductionPlantID.Value;
            requestModel.Email = userEntity.Email;
            requestModel.PhoneNumber = userEntity.PhoneNumber;
            requestModel.RegistrationUnitView = userEntity.ProductionPlant.Name;
            requestModel.Position = userEntity.PositionName;

            RegistrationTicket entity;
            switch (requestModel.RegistrationTicketType)
            {
                default:
                    entity = new RegistrationTicket
                    {
                        // Thông tin chung
                        TicketType = requestModel.TicketType,
                        FactoryID = requestModel.FactoryID,
                        TicketNumber = requestModel.TicketNumber,
                        RegistrationDate = requestModel.RegistrationDate,
                        ReceiveUnitID = requestModel.ReceiveUnitID,
                        RegulationNumber = requestModel.RegulationNumber,
                        RegistrationUnitID = requestModel.ProductionPlantID,
                        Position = requestModel.Position,
                        PhoneNumber = requestModel.PhoneNumber,
                        Email = requestModel.Email,
                        // Thông tin đăng ký
                        WorkContent = requestModel.WorkContent,
                        WorkPlacement = requestModel.WorkPlacement,
                        WorkCondition = requestModel.WorkCondition,
                        StartDate = requestModel.StartDate,
                        EndDate = requestModel.EndDate,
                        UnitNumberAndUnitStaff = requestModel.UnitNumberAndUnitStaff,
                        StatusValue = requestModel.IsSave ? (int)EnumTicketStatus.Unsent : (int)EnumTicketStatus.Waiting,
                        Attachments = requestModel.FileAttachments != null && requestModel.FileAttachments.Any() ? string.Join(",", requestModel.FileAttachments) : string.Empty,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        RegistrationTicketType = (int)TicketTypePCT.DIEN,
                        Units = string.Join(Environment.NewLine, requestModel.Units),
                        RegistrationUnitView = requestModel.RegistrationUnitView,
                        RequestTicketID = requestModel.RequestTicketID,
                        InspectionReportID = requestModel.InspectionReportID,
                    };

                    entity.RegistrationUnitView = entity.RegistrationUnitView ?? _commonService.GetRegistrationUnitView((int)entity.CreatedUserID);

                    // Người lãnh đạo
                    List<Stakeholders> stakeholders = new List<Stakeholders>();
                    if (requestModel.LeaderUser != null)
                    {
                        UserInfo userInfo = _commonService.GetUserById(requestModel.LeaderUser.UserName, requestModel.LeaderUser.IsExternal);
                        if (userInfo != null)
                        {
                            int? assignToUserID = null;
                            if (!requestModel.LeaderUser.IsExternal)
                                assignToUserID = userInfo.ID;

                            stakeholders.Add(new Stakeholders
                            {
                                AssignToUserID = assignToUserID,
                                FullName = userInfo.FullName,
                                ElectricRank = userInfo.ElectricRankStr,
                                LaborCard = userInfo.LaborCard,
                                LaborRank = userInfo.LaborRankStr,
                                UserName = userInfo.UserName,
                                IsExternal = requestModel.LeaderUser.IsExternal,
                                Roles = (int)RolesStakeholder.Leader,
                                PositionName = userInfo.PositionName,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            });
                        }
                    }

                    // Người giám sát an toàn điện (nếu có)
                    if (requestModel.Supervisors != null)
                    {
                        foreach (var item in requestModel.Supervisors)
                        {
                            UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                            if (userInfo != null)
                            {
                                int? assignToUserID = null;
                                if (!item.IsExternal)
                                    assignToUserID = userInfo.ID;

                                stakeholders.Add(new Stakeholders
                                {
                                    AssignToUserID = assignToUserID,
                                    FullName = userInfo.FullName,
                                    ElectricRank = userInfo.ElectricRankStr,
                                    LaborRank = userInfo.LaborRankStr,
                                    LaborCard = userInfo.LaborCard,
                                    UserName = userInfo.UserName,
                                    IsExternal = item.IsExternal,
                                    Roles = (int)RolesStakeholder.Supervisor,
                                    PositionName = userInfo.PositionName,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                });
                            }
                        }
                    }

                    // Người chỉ huy trực tiếp
                    if (requestModel.Commanders != null)
                    {
                        foreach (var item in requestModel.Commanders)
                        {
                            UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                            if (userInfo != null)
                            {
                                int? assignToUserID = null;
                                if (!item.IsExternal)
                                    assignToUserID = userInfo.ID;

                                stakeholders.Add(new Stakeholders
                                {
                                    AssignToUserID = assignToUserID,
                                    FullName = userInfo.FullName,
                                    UserName = userInfo.UserName,
                                    IsExternal = item.IsExternal,
                                    PositionName = userInfo.PositionName,
                                    ElectricRank = userInfo.ElectricRankStr,
                                    LaborRank = userInfo.LaborRankStr,
                                    LaborCard = userInfo.LaborCard,
                                    Roles = (int)RolesStakeholder.Commander,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                });
                            }
                        }
                    }

                    // Người cảnh giới
                    if (requestModel.GuardUser != null)
                    {
                        UserInfo userInfo = _commonService.GetUserById(requestModel.GuardUser.UserName, requestModel.GuardUser.IsExternal);
                        if (userInfo != null)
                        {
                            int? assignToUserID = null;
                            if (!requestModel.GuardUser.IsExternal)
                                assignToUserID = userInfo.ID;

                            stakeholders.Add(new Stakeholders
                            {
                                AssignToUserID = assignToUserID,
                                FullName = userInfo.FullName,
                                ElectricRank = userInfo.ElectricRankStr,
                                LaborCard = userInfo.LaborCard,
                                LaborRank = userInfo.LaborRankStr,
                                UserName = userInfo.UserName,
                                IsExternal = requestModel.GuardUser.IsExternal,
                                Roles = (int)RolesStakeholder.Guard,
                                PositionName = userInfo.PositionName,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            });
                        }
                    }

                    // Danh sách những người được cử để thực hiện công việc
                    if (requestModel.Workers != null)
                    {
                        foreach (var item in requestModel.Workers)
                        {
                            UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                            if (userInfo != null)
                            {
                                int? assignToUserID = null;
                                if (!item.IsExternal)
                                    assignToUserID = userInfo.ID;

                                stakeholders.Add(new Stakeholders
                                {
                                    AssignToUserID = assignToUserID,
                                    FullName = userInfo.FullName,
                                    UserName = userInfo.UserName,
                                    IsExternal = item.IsExternal,
                                    PositionName = userInfo.PositionName,
                                    ElectricRankCard = userInfo.ElectricRankCard,
                                    LaborCard = userInfo.LaborCard,
                                    LaborRank = userInfo.LaborRankStr,
                                    Mission = item.Mission,
                                    Roles = (int)RolesStakeholder.Worker,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                });
                            }
                        }
                    }
                    entity.Stakeholders = stakeholders;

                    // Những yêu cầu khác
                    List<OtherRequirements> otherRequirements = new List<OtherRequirements>();
                    if (requestModel.OtherRequirements != null)
                    {
                        foreach (var item in requestModel.OtherRequirements)
                        {
                            otherRequirements.Add(new OtherRequirements
                            {
                                Location = item.Location,
                                SafetyMeasures = item.SafetyMeasures,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            });
                        }
                    }
                    entity.OtherRequirements = otherRequirements;
                    break;
                case (int)TicketTypePCT.COTHUYNHIETHOA:
                    entity = new RegistrationTicket
                    {
                        TicketType = requestModel.TicketType,
                        // Thông tin chung
                        FactoryID = requestModel.FactoryID,
                        TicketNumber = requestModel.TicketNumber,
                        RegistrationDate = requestModel.RegistrationDate,
                        RegistrationUnitID = requestModel.ProductionPlantID,
                        ReceiveUnitID = requestModel.ReceiveUnitID,
                        Position = requestModel.Position,
                        PhoneNumber = requestModel.PhoneNumber,
                        Email = requestModel.Email,
                        // Thông tin đăng ký
                        WorkContent = requestModel.WorkContent,
                        LocationDevice = requestModel.LocationDevice,
                        Reason = requestModel.Reason,
                        ContentType = requestModel.ContentType,
                        StartDate = requestModel.StartDate,
                        EndDate = requestModel.EndDate,
                        IsWorkDaily = requestModel.IsWorkDaily,
                        StartTimeDaily = requestModel.StartTimeDaily,
                        EndTimeDaily = requestModel.EndTimeDaily,
                        OtherContents = requestModel.OtherContents,
                        StatusValue = requestModel.IsSave ? (int)EnumTicketStatus.Unsent : (int)EnumTicketStatus.Waiting,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        Attachments = requestModel.FileAttachments != null && requestModel.FileAttachments.Any() ? string.Join(",", requestModel.FileAttachments) : string.Empty,
                        RegistrationTicketType = (int)TicketTypePCT.COTHUYNHIETHOA,
                        RegulationNumber = requestModel.RegulationNumber,
                        WorkPlacement = requestModel.WorkPlacement,
                        WorkCondition = requestModel.WorkCondition,
                        RegistrationUnitView = requestModel.RegistrationUnitView,
                        RequestTicketID = requestModel.RequestTicketID,
                        InspectionReportID = requestModel.InspectionReportID,
                    };

                    entity.RegulationNumber = !string.IsNullOrWhiteSpace(requestModel.RegulationNumber) ? requestModel.RegulationNumber : string.Empty;
                    entity.WorkPlacement = !string.IsNullOrWhiteSpace(requestModel.WorkPlacement) ? requestModel.WorkPlacement : string.Empty;
                    entity.WorkCondition = !string.IsNullOrWhiteSpace(requestModel.WorkCondition) ? requestModel.WorkCondition : string.Empty;
                    entity.RegistrationUnitView = entity.RegistrationUnitView ?? _commonService.GetRegistrationUnitView((int)entity.CreatedUserID);
                    // Những người tham gia công việc
                    List<ContentPosition> contentPositions = new List<ContentPosition>();
                    foreach (var item in requestModel.ContentPositions)
                    {
                        var data = new ContentPosition
                        {
                            Position = item.Position,
                            RegistrationTicketID = entity.ID,
                        };
                        if (item.Stakeholders != null && item.Stakeholders.Count > 0)
                        {
                            List<Stakeholders> listStakeholders = new List<Stakeholders>();
                            foreach (var worker in item.Stakeholders)
                            {
                                UserInfo userInfo = _commonService.GetUserById(worker.UserName, worker.IsExternal);
                                int? assignToUserID = null;
                                if (userInfo != null)
                                {
                                    if (!worker.IsExternal)
                                        assignToUserID = userInfo.ID;

                                    listStakeholders.Add(new Stakeholders
                                    {
                                        AssignToUserID = assignToUserID,
                                        FullName = userInfo.FullName,
                                        ElectricRankCard = userInfo.LaborCard,
                                        ElectricRank = userInfo.ElectricRankStr,
                                        LaborRank = userInfo.LaborRankStr,
                                        LaborCard = userInfo.LaborCard,
                                        Roles = (int)RolesStakeholder.WorkParticipants,
                                        Mission = worker.Mission,
                                        CreatedUserID = requestModel.CurrentUserID,
                                        LastUpdatedUserID = requestModel.CurrentUserID,
                                        UserName = userInfo.UserName,
                                        IsExternal = userInfo.IsExternal,
                                        PositionName = userInfo.PositionName,
                                        RegistrationTicketID = entity.ID
                                    });
                                }
                            }
                            data.Stakeholders = listStakeholders;
                        }
                        contentPositions.Add(data);
                    }
                    entity.ContentPositions = contentPositions;

                    // Nhận diện mối nguy hại và biện pháp an toàn
                    List<IdentificationAndSafetyMeasures> identificationAndSafetyMeasures = new List<IdentificationAndSafetyMeasures>();
                    foreach (var item in requestModel.IdentificationAndSafetyMeasuresModels)
                    {
                        identificationAndSafetyMeasures.Add(new IdentificationAndSafetyMeasures
                        {
                            Identification = item.Identification,
                            SafetyMeasures = item.SafetyMeasures,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                        });
                    }
                    entity.IdentificationAndSafetyMeasures = identificationAndSafetyMeasures;
                    break;
            }

            // Tracking trạng thái thay đổi phiếu công tác
            var ticketStatus = requestModel.IsSave ? (int)EnumTicketStatus.Unsent : (int)EnumTicketStatus.Waiting;
            var description = requestModel.IsSave ? "Lưu chỉnh sửa phiếu đăng ký" : "Gửi phiếu đăng ký";
            List<TicketStatusHistory> ticketStatusHistories = new List<TicketStatusHistory>
            {
                AddTicketStatusHistory(0, requestModel.CurrentUserID, entity.UpdatedAt, ticketStatus, description,""),
            };
            entity.TicketStatusHistories = ticketStatusHistories;

            entity.MajorRepairID = requestModel.MajorRepairID;
            entity.WorkTicketID = requestModel.WorkTicketID;

            _unitOfWork.ElogbookRepository<RegistrationTicket>().Add(entity);

            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                //Add ticket number to tracking table
                try
                {
                    var arrNumTicket = requestModel.TicketNumber.Split('/');
                    int.TryParse(arrNumTicket[0], out int numTicket);
                    var ticketType = "DKPCTD";
                    if (requestModel.RegistrationTicketType == (int)TicketTypePCT.COTHUYNHIETHOA)
                    {
                        ticketType = "DKPTCNH";
                    }

                    var currentDate = DateTime.Now;
                    var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                    .GetQuery(x => x.InsertedAt.Year == currentDate.Year && x.TicketType.Equals(ticketType) && x.TicketNumber == numTicket).FirstOrDefault();
                    if (lstNumber == null)
                    {
                        var data = new ExclusiveData()
                        {
                            TicketNumber = numTicket,
                            TicketType = ticketType,
                            InsertedAt = currentDate,
                            UpdatedAt = currentDate,
                        };
                        _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
                    }
                    _unitOfWork.Complete();
                }
                catch
                {
                    //ignore error
                }

                // Cập nhật trạng thái PCT (Phiếu thuộc sửa chữa lớn)
                if (requestModel.WorkTicketID > 0 && requestModel.IsSave)
                {
                    var dataWorkTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
                    if (dataWorkTicket != null)
                    {
                        dataWorkTicket.RegistrationTicketID = entity.ID;
                        dataWorkTicket.WorkTicketStatus = (int)EnumTicketStatus.Waiting;
                        dataWorkTicket.UpdatedAt = DateTime.Now;
                        dataWorkTicket.LastUpdatedUserID = requestModel.CurrentUserID;
                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(dataWorkTicket);
                        _unitOfWork.Complete();
                    }
                }

                // cập nhật trạng thái phiếu yêu cầu
                if (requestModel.RequestTicketID.HasValue && requestModel.RequestTicketID.Value > 0)
                {
                    ChangeStatusRequestModel request = new ChangeStatusRequestModel
                    {
                        CurrentUserID = requestModel.CurrentUserID,
                        ID = requestModel.RequestTicketID.Value,
                        Status = (int)AmEnumRequestTicket.Registered,
                        RegistrationTicketID = entity.ID,
                    };
                    _amRequestTicketService.UpdateStatus(request);
                }

                if (!requestModel.IsSave)
                {
                    //[Notification-1 && 8]
                    var processEmailRequestModel = new ProcessEmailRequestModel
                    {
                        RegistrationTicketID = entity.ID,
                        TicketStatus = ticketStatus,
                        RegistrationTicketType = entity.RegistrationTicketType,
                        WorkTicketID = entity.WorkTicketID ?? 0,
                        IsPCT = false,
                        CurrentUserID = requestModel.CurrentUserID,
                        ModuleNotification = entity.RegistrationTicketType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                    };
                    _emailService.ProcessEmail(processEmailRequestModel);
                }

                return entity.ID;
            }
            else
            {
                return 0;
            }
        }

        public bool CheckUserIsUse(int userID)
        {
            //kiểm tra userId là lãnh đạo/người giám sát
            var workerIsUse = _unitOfWork.ElogbookRepository<Stakeholders>().GetQuery(x => x.AssignToUserID == userID && !x.IsDeleted).Any();

            return workerIsUse;
        }

        public RegistrationTicketModel GetRegistrationTicketById(int id)
        {
            var data = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ID == id && !x.IsDeleted);
            var entity = data.Select(
               e => new RegistrationTicketModel()
               {
                   // Thông tin chung
                   ID = e.ID,
                   TicketTypeValue = e.TicketType,
                   TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                   Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                   TicketNumber = e.TicketNumber,
                   RegistrationDate = e.RegistrationDate,
                   ReceiveUnit = new ValueLabelModel { Value = e.ReceiveUnit.ID, Label = e.ReceiveUnit.Name },
                   InsertedAt = e.InsertedAt,
                   CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                   RegulationNumber = e.RegulationNumber,
                   RegisterUnit = new ValueLabelModel { Value = e.RegistrationUnit.ID, Label = e.RegistrationUnit.Name },
                   RegisterUser = new UserInTicketModel
                   {
                       ID = e.CreatedUser.ID,
                       UserName = e.CreatedUser.UserName,
                       FullName = e.CreatedUser.FullName,
                       SignatureContent = e.CreatedUser.Signature,
                       CompanyName = e.CreatedUser.Company.Name,
                   },
                   Position = e.Position,
                   PhoneNumber = e.PhoneNumber,
                   Email = e.Email,
                   // Thông tin đăng ký
                   LocationDevice = e.LocationDevice,
                   WorkContent = e.WorkContent,
                   ContentTypeValue = e.ContentType,
                   IsWorkDaily = e.IsWorkDaily,
                   StartTimeDaily = e.StartTimeDaily,
                   EndTimeDaily = e.EndTimeDaily,
                   RegistrationUnitView = e.RegistrationUnitView,
                   WorkPlacement = e.WorkPlacement,
                   WorkCondition = e.WorkCondition,
                   StartDate = e.StartDate,
                   EndDate = e.EndDate,
                   UnitNumberAndUnitStaff = e.UnitNumberAndUnitStaff,
                   RegistrationTicketType = e.RegistrationTicketType,
                   WorkTicketID = e.WorkTicketID,
                   MajorRepairID = e.MajorRepairID,
                   RequestTicketID = e.RequestTicketID,
                   InspectionReportID = e.InspectionReportID,
                   // Nhận diện mối nguy và biện pháp an toàn
                   IdentificationAndSafetyMeasures = e.IdentificationAndSafetyMeasures.Where(x => !x.IsDeleted).Select(
                                            t => new IdentificationAndSafetyMeasuresModel()
                                            {
                                                ID = t.ID,
                                                Identification = t.Identification,
                                                SafetyMeasures = t.SafetyMeasures,
                                            }).ToList(),
                   OtherContents = e.OtherContents,

                   // Danh sách những người được đề cử để thực hiện công việc
                   // Các đơn vị QLVH khác có liên quan (nếu có)
                   Units = new List<string> { e.Units },

                   // Những yêu cầu khác (nếu có)
                   OtherRequirements = e.OtherRequirements.Where(x => !x.IsDeleted).Select(
                                            t => new OtherRequirementsModel()
                                            {
                                                ID = t.ID,
                                                Location = t.Location,
                                                SafetyMeasures = t.SafetyMeasures,
                                            }).ToList(),
                   // Tài liệu liên quan (nếu có)
                   Attackments = e.Attachments,
                   // Lịch sử thay đổi trạng thái phiếu công tác
                   TicketStatusHistories = e.TicketStatusHistories.Select(
                                            t => new TicketStatusHistoryModel()
                                            {
                                                ID = t.ID,
                                                InsertedAt = t.InsertedAt,
                                                CreatedUser = new ValueLabelModel { Value = t.CreatedUser.ID, Label = t.CreatedUser.FullName },
                                                Description = t.Description,
                                                Status = new ValueLabelModel { Value = t.TicketStatus.ID, Label = t.TicketStatus.StatusName },
                                            }).OrderBy(t => t.InsertedAt).ToList(),
               }).FirstOrDefault();

            if (entity != null && entity.ID > 0)
            {
                switch (entity.RegistrationTicketType)
                {
                    default:
                        // Các đơn vị QLVH khác có liên quan (nếu có)
                        if (entity.Units.Count > 0)
                        {
                            var dataUnit = entity.Units.FirstOrDefault();
                            if (dataUnit != null)
                            {
                                entity.Units = dataUnit.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                            }
                        }

                        // Người lãnh đạo (nếu có)
                        entity.LeaderUser = GetStakeholders(entity.ID, RolesStakeholder.Leader).FirstOrDefault();

                        // Người giám sát an toàn điện (nếu có)
                        entity.Supervisors = GetStakeholders(entity.ID, RolesStakeholder.Supervisor);

                        // Người chỉ huy trực tiếp
                        entity.Commanders = GetStakeholders(entity.ID, RolesStakeholder.Commander);

                        // Người giám sát (nếu có)
                        entity.GuardUser = GetStakeholders(entity.ID, RolesStakeholder.Guard).FirstOrDefault();

                        // Danh sách những người được đề cử để thực hiện công việc
                        entity.Workers = GetStakeholders(entity.ID, RolesStakeholder.Worker);
                        break;

                    case (int)TicketTypePCT.COTHUYNHIETHOA:
                        entity.ContentPositions = GetContentPositions(entity.ID);
                        break;
                }

                // Lấy thông tin đợt sửa chữa lớn
                var majorRepairEntity = _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => x.ID == entity.MajorRepairID && !x.IsDeleted).FirstOrDefault();
                if (majorRepairEntity != null)
                {
                    var status = _unitOfWork.ElogbookRepository<MajorRepairStatus>().GetById(majorRepairEntity.MajorRepairStatusID);
                    entity.MajorRepairInfo = new BasicDetailModel()
                    {
                        ID = majorRepairEntity.ID,
                        Title = majorRepairEntity.Title,
                        Status = status != null ? status.Status : string.Empty,
                    };
                }

                //Lấy thông tin phiếu yêu cầu
                var amRequestTicket = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.ID == entity.RequestTicketID && !x.IsDeleted).FirstOrDefault();
                if (amRequestTicket != null)
                {
                    var status = _unitOfWork.ElogbookRepository<AmRequestTicketStatus>().GetById(amRequestTicket.AmStatusID);
                    entity.RequestTicketInfo = new BasicDetailModel()
                    {
                        ID = amRequestTicket.ID,
                        Title = amRequestTicket.RequestTicketNumber,
                        Status = status != null ? status.StatusName : string.Empty,
                    };
                }

                // Lấy thông tin phiếu công tác
                var workTicketEntity = _unitOfWork.ElogbookRepository<WorkTicket>()
                    .GetQuery(x => x.RegistrationTicketID == entity.ID).FirstOrDefault();
                if (workTicketEntity != null)
                {
                    var status = _unitOfWork.ElogbookRepository<TicketStatus>().GetById(workTicketEntity.WorkTicketStatus);
                    entity.WorkTicketInfo = new BasicDetailModel()
                    {
                        ID = workTicketEntity.ID,
                        Title = workTicketEntity.WorkTicketNumber,
                        Status = status != null ? status.StatusName : string.Empty,
                    };

                    if (workTicketEntity.IsDeleted)
                    {
                        entity.WorkTicketDeleteInfo = new BasicDetailModel()
                        {
                            ID = workTicketEntity.ID,
                            Title = "Phiếu công tác: " + workTicketEntity.WorkTicketNumber + " đã được xóa bởi " +
                            workTicketEntity.LastUpdatedUser.FullName + " lúc " +
                            workTicketEntity.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                            Status = status != null ? status.StatusName : string.Empty,
                        };
                    }

                    entity.WorkTicketID = workTicketEntity.ID;
                }
            }

            if (entity != null && !string.IsNullOrWhiteSpace(entity.Attackments))
            {
                entity.FileAttachments = new List<AttachmentModel>();
                var attackments = entity.Attackments.Split(',');
                foreach (var item in attackments)
                {
                    if (int.TryParse(item, out int idItem))
                    {
                        var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted &&
                                                                        x.ID == idItem).FirstOrDefault();
                        if (att != null)
                        {
                            entity.FileAttachments.Add(new AttachmentModel
                            {
                                ID = att.ID,
                                FileName = att.FileName,
                                FilePath = _commonService.GetPathFile(att.FilePath),
                                PathThumbnail = _commonService.GetPathFile(att.PathThumbnail),
                                ScreenName = att.ScreenName,
                                FileType = att.FileType,
                            });
                        }
                    }
                }
            }

            if (entity != null && entity.InspectionReportID != null)
            {
                var inspectionRepor = _inspectionReportService.GetById(entity.InspectionReportID.Value);
                if (inspectionRepor != null)
                {
                    entity.InspectionReport = new ValueLabelModel { Value = inspectionRepor.ID, Label = inspectionRepor.TicketNumber };

                }
            }

            return entity;
        }

        public ResponseBaseModel GetRegistrationTickets(SearchRegistrationTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            //Mặc định không truyền data thì tìm kiếm theo PCTĐ
            if (requestModel.RegistrationTicketType.GetValueOrDefault(0) < 1) requestModel.RegistrationTicketType = 1;

            var entities = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketType == requestModel.RegistrationTicketType);

            //Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            //Số phiếu công tác
            if (!string.IsNullOrWhiteSpace(requestModel.TicketNumber))
            {
                entities = entities.Where(x => x.TicketNumber.Contains(requestModel.TicketNumber));
            }
            //Loại phiếu
            if (requestModel.TicketType > 0)
            {
                entities = entities.Where(x => x.TicketType == requestModel.TicketType);
            }
            //Trạng thái phiếu
            if (requestModel.StatusValue > 0)
            {
                entities = entities.Where(x => x.StatusValue == requestModel.StatusValue);
            }
            // Tìm kiếm theo đơn vị đăng ký
            if (requestModel.RegisterUnitID > 0)
            {
                var organizations = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && x.Active == (int)AccessStatus.Active
                                                                                    && x.ID == requestModel.RegisterUnitID).FirstOrDefault();
                if (organizations != null && organizations.Level.Equals("Tổ"))
                {
                    entities = entities.Where(x => x.RegistrationUnit.ID == organizations.ParentID);
                }
                else
                {
                    entities = entities.Where(x => x.RegistrationUnit.ID == requestModel.RegisterUnitID);
                }
            }
            //Ngày đăng ký phiếu (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.RegistrationDate) >= frDate.Date);
            }
            //Ngày đăng ký phiếu (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.RegistrationDate) <= toDate.Date);
            }

            // Return and paging data
            var page = entities.Select(
                e => new GeneralRegistrationTicketModel()
                {
                    ID = e.ID,
                    TicketNumber = e.TicketNumber,
                    RegistrationDate = e.RegistrationDate,
                    RegistrationUnitView = e.RegistrationUnitView,
                    WorkContent = e.WorkContent,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    RegisterUnit = new ValueLabelModel { Value = e.RegistrationUnit.ID, Label = e.RegistrationUnit.Name },
                    RegisterUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    TicketTypeValue = e.TicketType,
                    TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                })
                .OrderByDescending(t => t.RegistrationDate)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool CheckDuplicateData(int id, string ticketNumber, bool isAddMode, bool isTicketNumber)
        {
            // Trường hợp thêm mới thì kiểm tra Số phiếu đăng ký công tác
            if (isAddMode)
            {
                if (isTicketNumber)
                {
                    var currentDate = DateTime.Now;

                    var isExistCurrent = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.TicketNumber == ticketNumber
                                                                                        && !x.IsDeleted
                                                                                        && x.RegistrationTicketType == (int)TicketTypePCT.DIEN
                                                                                        && x.InsertedAt.Year == currentDate.Year).Any();

                    return isExistCurrent;
                }
                else
                {
                    return _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.RegulationNumber == ticketNumber
                                                                                        && !x.IsDeleted
                                                                                        && x.RegistrationTicketType == (int)TicketTypePCT.DIEN
                                                                                        && x.InsertedAt.Year == DateTime.Now.Year).Any();
                }
            }
            // Trường hợp cập nhật thì kiểm tra Số phiếu đăng ký công tác và id
            else
            {
                if (isTicketNumber)
                {
                    var currentDate = DateTime.Now;

                    var isExistCurrent = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ID != id
                                                                                    && x.TicketNumber == ticketNumber
                                                                                    && !x.IsDeleted && x.RegistrationTicketType == (int)TicketTypePCT.DIEN
                                                                                    && x.InsertedAt.Year == currentDate.Year).Any();

                    return isExistCurrent;
                }
                else
                {
                    return _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ID != id
                                                                                    && x.RegulationNumber == ticketNumber
                                                                                    && !x.IsDeleted && x.RegistrationTicketType == (int)TicketTypePCT.DIEN
                                                                                    && x.InsertedAt.Year == DateTime.Now.Year).Any();
                }
            }
        }

        /// <summary>
        /// kiểm tra trùng dữ liệu theo số phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>.
        public bool CheckDuplicateData(int id, string ticketNumber, bool isAddMode)
        {
            var currentDate = DateTime.Now;

            // Trường hợp thêm mới thì kiểm tra Số phiếu đăng ký công tác
            if (isAddMode)
            {
                var isExistCurrent = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.TicketNumber == ticketNumber
                                                                                    && !x.IsDeleted
                                                                                    && x.RegistrationTicketType == (int)TicketTypePCT.COTHUYNHIETHOA
                                                                                    && x.InsertedAt.Year == currentDate.Year).Any();

                return isExistCurrent;
            }
            // Trường hợp cập nhật thì kiểm tra Số phiếu đăng ký công tác và id
            else
            {
                var isExistCurrent = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ID != id
                                                                                    && x.TicketNumber == ticketNumber
                                                                                    && !x.IsDeleted && x.RegistrationTicketType == (int)TicketTypePCT.COTHUYNHIETHOA
                                                                                    && x.InsertedAt.Year == currentDate.Year).Any();

                return isExistCurrent;
            }
        }

        public bool UpdateStatusRegistrationTicket(int id, int statusValue, string description, int userID, string lstAttachment)
        {
            var entity = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(id);
            if (entity != null && entity.StatusValue != statusValue)
            {
                entity.StatusValue = statusValue;
                if (statusValue != (int)EnumTicketStatus.Approved)
                {
                    entity.RegistrationDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<RegistrationTicket>().Update(entity);

                var ticketStatusHistory = AddTicketStatusHistory(entity.ID, userID, entity.UpdatedAt, statusValue, description, lstAttachment);
                _unitOfWork.ElogbookRepository<TicketStatusHistory>().Add(ticketStatusHistory);

                if (statusValue == (int)EnumTicketStatus.Reject || statusValue == (int)EnumTicketStatus.Cancel)
                {
                    // Cập nhật trạng thái PCT (Phiếu thuộc sửa chữa lớn)
                    var dataWorkTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == entity.WorkTicketID
                                                                                               && x.MajorRepairID == entity.MajorRepairID
                                                                                               && !x.IsDeleted).FirstOrDefault();
                    if (dataWorkTicket != null)
                    {
                        dataWorkTicket.WorkTicketStatus = (statusValue == (int)EnumTicketStatus.Cancel) ? (int)EnumTicketStatus.WaitForRegist : (int)EnumTicketStatus.Reject;
                        dataWorkTicket.UpdatedAt = DateTime.Now;
                        dataWorkTicket.LastUpdatedUserID = userID;
                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(dataWorkTicket);
                    }
                }

                if (_unitOfWork.Complete() > 0)
                {
                    //[Notification-1-2-3-4 && 8-9-10-11]
                    var processEmailRequestModel = new ProcessEmailRequestModel
                    {
                        RegistrationTicketID = entity.ID,
                        TicketStatus = statusValue,
                        RegistrationTicketType = entity.RegistrationTicketType,
                        ModuleNotification = entity.RegistrationTicketType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH,
                        CurrentUserID = userID,
                        IsPCT = false
                    };
                    _emailService.ProcessEmail(processEmailRequestModel);

                    return true;
                }
            }

            return false;
        }

        public bool UpdateRegistrationTicket(AddEditRegistrationTicketModel requestModel)
        {
            if (requestModel.IsSave)
            {
                requestModel.RegistrationDate = null;
            }
            else if (!requestModel.RegistrationDate.HasValue)
            {
                requestModel.RegistrationDate = DateTime.Now;
            }

            var entity = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(requestModel.ID);
            if (entity != null)
            {
                var oldTicketNumber = entity.TicketNumber;
                var oldStatusValue = entity.StatusValue;
                var year = entity.InsertedAt.Year;

                entity.TicketType = requestModel.TicketType;
                entity.FactoryID = requestModel.FactoryID;
                entity.TicketNumber = requestModel.TicketNumber;
                entity.RegistrationDate = requestModel.RegistrationDate;
                entity.ReceiveUnitID = requestModel.ReceiveUnitID;
                entity.RegistrationUnitID = requestModel.ProductionPlantID;
                entity.Position = requestModel.Position;
                entity.PhoneNumber = requestModel.PhoneNumber;
                entity.Email = requestModel.Email;
                entity.ContentType = requestModel.ContentType;
                entity.IsWorkDaily = requestModel.IsWorkDaily;
                entity.StartTimeDaily = requestModel.StartTimeDaily;
                entity.EndTimeDaily = requestModel.EndTimeDaily;
                entity.OtherContents = requestModel.OtherContents;
                entity.WorkContent = requestModel.WorkContent;
                entity.StartDate = requestModel.StartDate;
                entity.EndDate = requestModel.EndDate;
                entity.UnitNumberAndUnitStaff = requestModel.UnitNumberAndUnitStaff;
                _commonService.DeleteAttachFileUnuse(entity.Attachments, requestModel.FileAttachments, requestModel.CurrentUserID);
                entity.Attachments = requestModel.FileAttachments != null && requestModel.FileAttachments.Any() ? string.Join(",", requestModel.FileAttachments) : string.Empty;
                entity.StatusValue = requestModel.IsSave ? (int)EnumTicketStatus.Unsent : (int)EnumTicketStatus.Waiting;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.Units = requestModel.Units == null ? string.Empty : string.Join(Environment.NewLine, requestModel.Units);
                entity.InspectionReportID = requestModel.InspectionReportID;

                switch (entity.RegistrationTicketType)
                {
                    default:
                        entity.RegulationNumber = requestModel.RegulationNumber;
                        entity.WorkPlacement = requestModel.WorkPlacement;
                        entity.WorkCondition = requestModel.WorkCondition;
                        break;

                    case (int)TicketTypePCT.COTHUYNHIETHOA:
                        entity.RegulationNumber = !string.IsNullOrWhiteSpace(requestModel.RegulationNumber) ? requestModel.RegulationNumber : string.Empty;
                        entity.WorkPlacement = !string.IsNullOrWhiteSpace(requestModel.WorkPlacement) ? requestModel.WorkPlacement : string.Empty;
                        entity.WorkCondition = !string.IsNullOrWhiteSpace(requestModel.WorkCondition) ? requestModel.WorkCondition : string.Empty;
                        entity.LocationDevice = !string.IsNullOrWhiteSpace(requestModel.LocationDevice) ? requestModel.LocationDevice : string.Empty;
                        break;
                }

                _unitOfWork.ElogbookRepository<RegistrationTicket>().Update(entity);

                var stakeholders = _unitOfWork.ElogbookRepository<Stakeholders>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();

                switch (requestModel.RegistrationTicketType)
                {
                    default:
                        #region Người lãnh đạo

                        var leaders = stakeholders.Where(x => x.Roles == (int)RolesStakeholder.Leader).ToList();
                        UpdateStakeholders(leaders, requestModel, RolesStakeholder.Leader);

                        #endregion

                        #region Người giám sát an toàn điện

                        var supervisors = stakeholders.Where(x => x.Roles == (int)RolesStakeholder.Supervisor).ToList();
                        UpdateStakeholders(supervisors, requestModel, RolesStakeholder.Supervisor);

                        #endregion

                        #region Người chỉ huy trực tiếp

                        var commanders = stakeholders.Where(x => x.Roles == (int)RolesStakeholder.Commander).ToList();
                        UpdateStakeholders(commanders, requestModel, RolesStakeholder.Commander);

                        #endregion

                        #region Danh sách những người được đề cử để thực hiện công việc

                        var workers = stakeholders.Where(x => x.Roles == (int)RolesStakeholder.Worker).ToList();
                        UpdateStakeholders(workers, requestModel, RolesStakeholder.Worker);

                        #endregion

                        #region Người cảnh giác

                        var guard = stakeholders.Where(x => x.Roles == (int)RolesStakeholder.Guard).ToList();
                        UpdateStakeholders(guard, requestModel, RolesStakeholder.Guard);

                        #endregion

                        #region Những yêu cầu khác

                        var entityOthers = _unitOfWork.ElogbookRepository<OtherRequirements>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();
                        foreach (var updateRow in entityOthers)
                        {
                            var existingRow = requestModel.OtherRequirements.Find(x => x.ID == updateRow.ID);
                            if (existingRow != null)
                            {
                                if (existingRow.Location != updateRow.Location || existingRow.SafetyMeasures != updateRow.SafetyMeasures)
                                {
                                    updateRow.Location = existingRow.Location;
                                    updateRow.SafetyMeasures = existingRow.SafetyMeasures;
                                    updateRow.LastUpdatedUserID = requestModel.CurrentUserID;

                                    _unitOfWork.ElogbookRepository<OtherRequirements>().Update(updateRow);
                                }
                            }
                            else
                            {
                                _unitOfWork.ElogbookRepository<OtherRequirements>().Delete(updateRow, requestModel.CurrentUserID);
                            }
                        }

                        if (requestModel.OtherRequirements != null)
                        {
                            foreach (var item in requestModel.OtherRequirements.Where(x => x.ID == 0))
                            {
                                var addRow = new OtherRequirements
                                {
                                    RegistrationTicketID = requestModel.ID,
                                    Location = item.Location,
                                    SafetyMeasures = item.SafetyMeasures,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };
                                _unitOfWork.ElogbookRepository<OtherRequirements>().Add(addRow);
                            }
                        }

                        #endregion
                        break;

                    case (int)TicketTypePCT.COTHUYNHIETHOA:
                        #region Người tham gia công việc
                        var contentPositions = _unitOfWork.ElogbookRepository<ContentPosition>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();
                        UpdateContentPositions(contentPositions, requestModel);
                        #endregion

                        #region Nhận diện mối nguy và biện pháp an toàn

                        var entityIdentificationAndSafetys = _unitOfWork.ElogbookRepository<IdentificationAndSafetyMeasures>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();
                        foreach (var updateRow in entityIdentificationAndSafetys)
                        {
                            var existingRow = requestModel.IdentificationAndSafetyMeasuresModels.Find(x => x.ID == updateRow.ID);
                            if (existingRow != null)
                            {
                                updateRow.Identification = existingRow.Identification;
                                updateRow.SafetyMeasures = existingRow.SafetyMeasures;
                                _unitOfWork.ElogbookRepository<IdentificationAndSafetyMeasures>().Update(updateRow);
                            }
                            else
                            {
                                _unitOfWork.ElogbookRepository<IdentificationAndSafetyMeasures>().Delete(updateRow, requestModel.CurrentUserID);
                            }
                        }

                        foreach (var item in requestModel.IdentificationAndSafetyMeasuresModels.Where(x => x.ID == 0))
                        {
                            var addRow = new IdentificationAndSafetyMeasures
                            {
                                RegistrationTicketID = requestModel.ID,
                                Identification = item.Identification,
                                SafetyMeasures = item.SafetyMeasures,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            };
                            _unitOfWork.ElogbookRepository<IdentificationAndSafetyMeasures>().Add(addRow);
                        }

                        #endregion
                        break;
                }

                // Tracking trạng thái thay đổi phiếu công tác
                TicketStatusHistory ticketStatusHistory = null;
                if (requestModel.IsSave)
                {
                    if (oldStatusValue != (int)EnumTicketStatus.Unsent)
                    {
                        ticketStatusHistory = AddTicketStatusHistory(entity.ID, requestModel.CurrentUserID, entity.UpdatedAt, (int)EnumTicketStatus.Unsent, "Lưu chỉnh sửa phiếu đăng ký", "");
                        _unitOfWork.ElogbookRepository<TicketStatusHistory>().Add(ticketStatusHistory);
                    }
                }
                else
                {
                    ticketStatusHistory = AddTicketStatusHistory(entity.ID, requestModel.CurrentUserID, entity.UpdatedAt, (int)EnumTicketStatus.Waiting, "Gửi phiếu đăng ký", "");
                    _unitOfWork.ElogbookRepository<TicketStatusHistory>().Add(ticketStatusHistory);
                }

                if (_unitOfWork.Complete() > 0)
                {
                    //Add ticket number to tracking table
                    try
                    {
                        if (oldTicketNumber?.Trim() != requestModel?.TicketNumber?.Trim())
                        {
                            var arrNumTicket = requestModel.TicketNumber.Split('/');
                            Int32.TryParse(arrNumTicket[0], out int numTicket);
                            var ticketType = "DKPCTD";
                            if (requestModel.RegistrationTicketType == (int)TicketTypePCT.COTHUYNHIETHOA)
                            {
                                ticketType = "DKPTCNH";
                            }

                            var currentDate = DateTime.Now;
                            var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                            .GetQuery(x => x.InsertedAt.Year == currentDate.Year && x.TicketType.Equals(ticketType) && x.TicketNumber == numTicket).FirstOrDefault();
                            if (lstNumber == null)
                            {
                                var data = new ExclusiveData()
                                {
                                    TicketNumber = numTicket,
                                    TicketType = ticketType,
                                    InsertedAt = currentDate,
                                    UpdatedAt = currentDate,
                                };
                                _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);

                                //Remove old data tracking ticket
                                var arrNumTicketOld = oldTicketNumber?.Split('/');
                                int.TryParse(arrNumTicketOld?[0], out int numTicketOld);

                                var oldExclusiveData = _unitOfWork.ElogbookRepository<ExclusiveData>()
                                    .GetQuery(x => x.InsertedAt.Year == year && x.TicketType.Equals(ticketType)
                                        && x.TicketNumber == numTicketOld).FirstOrDefault();
                                if (oldExclusiveData != null)
                                {
                                    _unitOfWork.ElogbookRepository<ExclusiveData>().Delete(oldExclusiveData, requestModel.CurrentUserID, true);
                                }
                            }
                            _unitOfWork.Complete();
                        }
                    }
                    catch
                    {
                        //ignore error
                    }

                    if (!requestModel.IsSave)
                    {
                        //[Notification-1 && 8]
                        var processEmailRequestModel = new ProcessEmailRequestModel
                        {
                            RegistrationTicketID = entity.ID,
                            TicketStatus = (int)EnumTicketStatus.Waiting,
                            RegistrationTicketType = entity.RegistrationTicketType,
                            WorkTicketID = entity.WorkTicketID ?? 0,
                            IsPCT = false,
                            CurrentUserID = requestModel.CurrentUserID,
                            ModuleNotification = entity.RegistrationTicketType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH,
                        };
                        _emailService.ProcessEmail(processEmailRequestModel);
                    }

                    return true;
                }
            }

            return false;
        }

        public bool DeleteRegistrationTicket(int id, int currentUserID)
        {
            var entity = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(id);

            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<Stakeholders>().Delete(entity.Stakeholders, currentUserID);
                _unitOfWork.ElogbookRepository<OtherRequirements>().Delete(entity.OtherRequirements, currentUserID);
                _unitOfWork.ElogbookRepository<TicketStatusHistory>().Delete(entity.TicketStatusHistories, currentUserID);
                _unitOfWork.ElogbookRepository<RegistrationTicket>().Delete(entity, currentUserID);
                _unitOfWork.ElogbookRepository<ContentPosition>().Delete(entity.ContentPositions, currentUserID);
                foreach (var item in entity.ContentPositions)
                {
                    _unitOfWork.ElogbookRepository<Stakeholders>().Delete(item.Stakeholders, currentUserID);
                }

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        private List<ContentPositionModel> GetContentPositions(int registrationTicketID)
        {
            List<ContentPositionModel> data = new List<ContentPositionModel>();
            var entities = _unitOfWork.ElogbookRepository<ContentPosition>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == registrationTicketID).ToList();

            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    List<StakeholdersModel> stakeholdersModels = new List<StakeholdersModel>();
                    foreach (var stakeholder in item.Stakeholders)
                    {
                        stakeholdersModels.Add(new StakeholdersModel
                        {
                            ID = stakeholder.ID,
                            AssignToUserID = stakeholder.AssignToUserID,
                            Mission = stakeholder.Mission,
                            UserName = stakeholder.UserName,
                            Roles = stakeholder.Roles,
                            IsExternal = stakeholder.IsExternal,
                            LaborCard = stakeholder.LaborCard,
                            LaborRankStr = stakeholder.LaborRank,
                            ElectricRankStr = stakeholder.ElectricRank,
                            ElectricRankCard = stakeholder.ElectricRankCard,
                            FullName = stakeholder.FullName,
                        });
                    }

                    data.Add(new ContentPositionModel
                    {
                        ID = item.ID,
                        Position = item.Position,
                        Stakeholders = stakeholdersModels
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// Lấy danh sách người trong ticket
        /// </summary>
        /// <param name="registrationTicketID"></param>
        /// <param name="poisitionStakeholder"></param>
        /// <returns></returns>
        public List<StakeholdersResponseModel> GetStakeholders(int registrationTicketID, RolesStakeholder rolesStakeholder)
        {
            return _unitOfWork.ElogbookRepository<Stakeholders>().GetQuery(x => !x.IsDeleted && x.Roles == (int)rolesStakeholder &&
                                                                            x.RegistrationTicketID == registrationTicketID)
                .Select(t => new StakeholdersResponseModel()
                {
                    ID = t.ID,
                    AssignToUserID = t.AssignToUserID,
                    FullName = t.AssignToUserID > 0 ? t.AssignToUser.FullName : t.FullName,
                    ElectricRankStr = t.ElectricRank,
                    ElectricRankCard = t.ElectricRankCard,
                    LaborRankStr = t.LaborRank,
                    LaborCard = t.LaborCard,
                    Roles = t.Roles,
                    Mission = t.Mission,
                    PositionName = t.PositionName,
                    UserName = t.UserName,
                    IsExternal = t.IsExternal,
                    Email = t.AssignToUser.Email
                }).ToList();
        }

        /// <summary>
        /// Cập nhật dữ liệu người liên quan đến phiếu công tác
        /// </summary>
        /// <param name="stakeholders"></param>
        /// <param name="requestModel"></param>
        /// <param name="position"></param>
        private void UpdateStakeholders(List<Stakeholders> stakeholders, AddEditRegistrationTicketModel requestModel, RolesStakeholder roles)
        {
            var isExistLeader = false;
            var isExistGuard = false;

            #region Cập nhật/xóa dữ liệu

            foreach (var updateRow in stakeholders)
            {
                StakeholdersModel existingRow = null;
                switch (roles)
                {
                    case RolesStakeholder.Leader:
                        isExistLeader = true;
                        if (requestModel.LeaderUser != null)
                        {
                            existingRow = requestModel.LeaderUser;
                        }
                        break;
                    case RolesStakeholder.Supervisor:
                        if (requestModel.Supervisors != null)
                        {
                            existingRow = requestModel.Supervisors.Find(x => x.ID == updateRow.ID);
                        }
                        break;
                    case RolesStakeholder.Commander:
                        if (requestModel.Commanders != null)
                        {
                            existingRow = requestModel.Commanders.Find(x => x.ID == updateRow.ID);
                        }
                        break;
                    case RolesStakeholder.Guard:
                        isExistGuard = true;
                        if (requestModel.GuardUser != null)
                        {
                            existingRow = requestModel.GuardUser;
                        }
                        break;
                    case RolesStakeholder.Worker:
                        if (requestModel.Workers != null)
                        {
                            existingRow = requestModel.Workers.Find(x => x.ID == updateRow.ID);
                        }
                        break;
                }

                // Tồn tại dòng cần cập nhật
                if (existingRow != null)
                {
                    var isUpdate = false;
                    UserInfo userInfo = _commonService.GetUserById(existingRow.UserName, existingRow.IsExternal);

                    // Xác định dữ liệu có thay đổi trước khi cập nhật
                    if (roles == RolesStakeholder.Worker)
                    {
                        if (existingRow.AssignToUserID != updateRow.AssignToUserID || userInfo.FullName != updateRow.FullName || userInfo.ElectricRankCard != updateRow.ElectricRankCard || existingRow.Mission != updateRow.Mission)
                        {
                            isUpdate = true;
                        }
                    }
                    else
                    {
                        if (existingRow.AssignToUserID != updateRow.AssignToUserID || userInfo.FullName != updateRow.FullName || userInfo.ElectricRankStr != updateRow.ElectricRank)
                        {
                            isUpdate = true;
                        }
                    }

                    // Nếu dữ liệu có thay đổi thì tiến hành cập nhật
                    if (isUpdate)
                    {
                        int? assignToUserID = null;
                        if (userInfo != null && !updateRow.IsExternal)
                            assignToUserID = userInfo.ID;

                        // Người thực hiện công việc
                        if (roles == RolesStakeholder.Worker)
                        {
                            if (userInfo != null)
                                updateRow.ElectricRankCard = userInfo.ElectricRankCard;
                            updateRow.Mission = existingRow.Mission;
                        }
                        // Người giám sát/chỉ huy
                        else
                        {
                            if (userInfo != null)
                                updateRow.ElectricRank = userInfo.ElectricRankStr;
                        }

                        // Thông tin chung
                        updateRow.AssignToUserID = assignToUserID;
                        updateRow.IsExternal = existingRow.IsExternal;
                        updateRow.LastUpdatedUserID = requestModel.CurrentUserID;

                        if (userInfo != null)
                        {
                            updateRow.FullName = userInfo.FullName;
                            updateRow.UserName = userInfo.UserName;
                            updateRow.ElectricRank = userInfo.ElectricRankStr;
                            updateRow.ElectricRankCard = userInfo.ElectricRankCard;
                            updateRow.PositionName = userInfo.PositionName;
                            updateRow.LaborRank = userInfo.LaborRankStr;
                            updateRow.LaborCard = userInfo.LaborCard;
                        }

                        _unitOfWork.ElogbookRepository<Stakeholders>().Update(updateRow);
                    }
                }
                else
                {
                    _unitOfWork.ElogbookRepository<Stakeholders>().Delete(updateRow, requestModel.CurrentUserID);
                }
            }

            #endregion

            #region Thêm mới dữ liệu

            // Danh sách dữ liệu thêm mới
            List<StakeholdersModel> newStakeholders = new List<StakeholdersModel>();
            switch (roles)
            {
                case RolesStakeholder.Leader:
                    if (requestModel.LeaderUser != null && !isExistLeader)
                    {
                        newStakeholders.Add(requestModel.LeaderUser);
                    }
                    break;
                case RolesStakeholder.Supervisor:
                    if (requestModel.Supervisors != null)
                    {
                        newStakeholders = requestModel.Supervisors.Where(x => x.ID == 0).ToList();
                    }
                    break;
                case RolesStakeholder.Commander:
                    if (requestModel.Commanders != null)
                    {
                        newStakeholders = requestModel.Commanders.Where(x => x.ID == 0).ToList();
                    }
                    break;
                case RolesStakeholder.Guard:
                    if (requestModel.GuardUser != null && !isExistGuard)
                    {
                        newStakeholders.Add(requestModel.GuardUser);
                    }
                    break;
                case RolesStakeholder.Worker:
                    if (requestModel.Workers != null)
                    {
                        newStakeholders = requestModel.Workers.Where(x => x.ID == 0).ToList();
                    }
                    break;
            }

            foreach (var item in newStakeholders)
            {
                UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                if (userInfo != null)
                {
                    int? assignToUserID = null;
                    if (!item.IsExternal)
                        assignToUserID = userInfo.ID;

                    var rowAdd = new Stakeholders
                    {
                        RegistrationTicketID = requestModel.ID,
                        AssignToUserID = assignToUserID,
                        FullName = userInfo.FullName,
                        Roles = (int)roles,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UserName = userInfo.UserName,
                        IsExternal = userInfo.IsExternal,
                        PositionName = userInfo.PositionName
                    };

                    // Người thực hiện công việc
                    if (roles == RolesStakeholder.Worker)
                    {
                        rowAdd.ElectricRankCard = userInfo.ElectricRankCard;
                        rowAdd.Mission = item.Mission;
                    }
                    // Người Lãnh đạo/giám sát/chỉ huy/cảnh giới
                    else
                    {
                        rowAdd.ElectricRankCard = userInfo.ElectricRankCard;
                    }

                    _unitOfWork.ElogbookRepository<Stakeholders>().Add(rowAdd);
                }
            }

            #endregion
        }

        private void UpdateContentPositions(List<ContentPosition> contentPositions, AddEditRegistrationTicketModel requestModel)
        {
            foreach (var updateRow in contentPositions)
            {
                _unitOfWork.ElogbookRepository<Stakeholders>().Delete(updateRow.Stakeholders, requestModel.CurrentUserID);
                _unitOfWork.ElogbookRepository<ContentPosition>().Delete(updateRow, requestModel.CurrentUserID);
            }

            foreach (var newItem in requestModel.ContentPositions)
            {
                var data = new ContentPosition
                {
                    Position = newItem.Position,
                    RegistrationTicketID = requestModel.ID,
                };

                if (newItem.Stakeholders != null && newItem.Stakeholders.Count > 0)
                {
                    List<Stakeholders> stakeholders = new List<Stakeholders>();
                    foreach (var worker in newItem.Stakeholders)
                    {
                        UserInfo userInfo = _commonService.GetUserById(worker.UserName, worker.IsExternal);
                        if (userInfo != null)
                        {
                            int? assignToUserID = null;
                            if (!worker.IsExternal)
                            {
                                assignToUserID = userInfo.ID;
                            }

                            stakeholders.Add(new Stakeholders
                            {
                                RegistrationTicketID = requestModel.ID,
                                AssignToUserID = assignToUserID,
                                FullName = userInfo.FullName,
                                ElectricRankCard = userInfo.ElectricRankCard,
                                Roles = (int)RolesStakeholder.WorkParticipants,
                                Mission = worker.Mission,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UserName = userInfo.UserName,
                                IsExternal = userInfo.IsExternal,
                                PositionName = userInfo.PositionName,
                            });
                        }
                    }
                    data.Stakeholders = stakeholders;
                }
                _unitOfWork.ElogbookRepository<ContentPosition>().Add(data);
            }
        }

        private TicketStatusHistory AddTicketStatusHistory(int registrationTicketID, int userID, DateTime insertedAt, int ticketStatus, string description, string lstAttachment)
        {
            var ticketStatusHistory = new TicketStatusHistory()
            {
                InsertedAt = insertedAt,
                StatusValue = ticketStatus,
                Description = description,
                Attachments = lstAttachment,
                CreatedUserID = userID,
                LastUpdatedUserID = userID,
            };

            if (ticketStatus == (int)EnumTicketStatus.Reject)
            {
                ticketStatusHistory.Description = "Từ chối phiếu đăng ký.";
                ticketStatusHistory.Reason = description;
            }
            else
            {
                ticketStatusHistory.Description = description;
                ticketStatusHistory.Reason = string.Empty;
            }

            if (registrationTicketID > 0)
                ticketStatusHistory.RegistrationTicketID = registrationTicketID;

            return ticketStatusHistory;
        }

        public ResponseBaseModel GetListTicketStatusHistoryById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<TicketStatusHistory>().GetQuery(x => x.RegistrationTicketID == id && !x.IsDeleted).ToList();
            List<TicketStatusHistoryModel> ticketStatusHistoryModels = new List<TicketStatusHistoryModel>();

            foreach (var item in entity)
            {
                List<int> listId = new List<int>();
                if (!string.IsNullOrWhiteSpace(item.Attachments))
                {
                    try
                    {
                        listId = item.Attachments?.Split(',').Select(int.Parse).ToList();
                    }
                    catch
                    {
                        // Bỏ qua trường hợp lỗi vì data không đúng
                    }
                }

                TicketStatusHistoryModel ticketStatus = new TicketStatusHistoryModel
                {
                    ID = item.ID,
                    Description = item.Description,
                    InsertedAt = item.InsertedAt,
                    FileAttachs = new List<AttachmentModel>(),
                    Reason = item.Reason
                };

                if (listId != null && listId.Count > 0)
                {
                    foreach (var attachmentId in listId)
                    {
                        var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted &&
                                                                            x.ID == attachmentId).FirstOrDefault();
                        if (att != null)
                        {
                            ticketStatus.FileAttachs.Add(new AttachmentModel
                            {
                                ID = att.ID,
                                FileName = att.FileName,
                                FilePath = _commonService.GetPathFile(att.FilePath),
                                PathThumbnail = _commonService.GetPathFile(att.PathThumbnail),
                                ScreenName = att.ScreenName,
                                FileType = att.FileType,
                            });
                        }
                    }
                }

                ticketStatus.CreatedUser = new ValueLabelModel { Value = item.CreatedUser.ID, Label = item.CreatedUser.FullName };
                ticketStatus.Status = new ValueLabelModel { Value = item.TicketStatus.ID, Label = item.TicketStatus.StatusName };
                ticketStatusHistoryModels.Add(ticketStatus);
            }

            // Return and paging data
            var page = ticketStatusHistoryModels.OrderBy(t => t.InsertedAt).ToList().ToPagedList();

            return HttpResponse.OkList(page);
        }

        public HttpResponseMessage ExportPdfFile(int id)
        {
            var rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            var registrationTicket = GetRegistrationTicketById(id);
            if (registrationTicket == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            var ticketType = registrationTicket.RegistrationTicketType;
            string[] arrTicketNumber = registrationTicket.TicketNumber.Split('/');
            string ticketNumber = string.Empty;
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (arrTicketNumber.Length > 0) ticketNumber = arrTicketNumber[0];

            // tạo thư mục lưu file pdf
            if (!Directory.Exists(Path.Combine(rootPath, "RegistrationTicket", "PDF")))
            {
                Directory.CreateDirectory(Path.Combine(rootPath, "RegistrationTicket", "PDF"));
            }
            string fileName = $"Phieu_dang_ky_cong_tac_{ticketNumber}_{currentDate}.pdf";
            string outputFile = Path.Combine(rootPath, "RegistrationTicket", "PDF", fileName);

            PdfInfoModel pdfInfoModel = null;

            if (ticketType == 2)
            {
                Pdf881RegistrationTicketModel pdfDataModel = CreatePdf881Data(registrationTicket);
                pdfInfoModel = GeneratePdf881File(pdfDataModel, outputFile);
            }
            else
            {
                // tạo file PDF rùi ký vào file đó
                PdfRegistrationTicketModel pdfDataModel = CreatePdfData(registrationTicket);
                pdfInfoModel = GeneratePdfFile(pdfDataModel, outputFile);
            }

            if (pdfInfoModel.Status)
            {
                var fileBytes = File.ReadAllBytes(outputFile);
                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);

        }

        private Pdf881RegistrationTicketModel CreatePdf881Data(RegistrationTicketModel workTicketDetail)
        {
            Pdf881RegistrationTicketModel pdfContentModel = new Pdf881RegistrationTicketModel
            {
                PdfHeader = new Pdf881HeaderRegistrationTicketModel()
                {
                    ReceiveUnit = workTicketDetail.ReceiveUnit.Label,
                    RegisterUnit = workTicketDetail.RegisterUnit.Label,
                    TicketNumber = workTicketDetail.TicketNumber,
                },
                PdfInfo = new Pdf881InfoRegistrationTicketModel()
                {
                    LocationDevice = workTicketDetail.LocationDevice,
                    TicketType = workTicketDetail.TicketType,
                    ContentType = workTicketDetail.ContentType,
                    WorkContent = workTicketDetail.WorkContent,
                    IdentificationAndSafetyMeasures = workTicketDetail.IdentificationAndSafetyMeasures,
                    StartDate = workTicketDetail.StartDate,
                    EndDate = workTicketDetail.EndDate,
                    IsWorkDaily = workTicketDetail.IsWorkDaily,
                    StartTimeDaily = workTicketDetail.StartTimeDaily,
                    EndTimeDaily = workTicketDetail.EndTimeDaily,
                    ContentPositions = workTicketDetail.ContentPositions,
                    RegisterUser = workTicketDetail.RegisterUser,
                    OtherContents = workTicketDetail.OtherContents,
                    Position = workTicketDetail.Position,
                    Email = workTicketDetail.Email,
                    PhoneNumber = workTicketDetail.PhoneNumber,
                    RegistrationDate = workTicketDetail.RegistrationDate ?? workTicketDetail.InsertedAt
                }
            };

            return pdfContentModel;
        }

        private PdfInfoModel GeneratePdf881File(Pdf881RegistrationTicketModel pdfDataModel, string outputFile)
        {
            _pdfInfo = new PdfInfoModel();
            try
            {
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    //Create document
                    Document pdfDocument = new Document(PageSize.A4);
                    pdfDocument.SetMargins(0, 0, 25, 25);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, fileStream);
                    // Open the document to enable you to write to the document
                    pdfDocument.Open();
                    try
                    {
                        PdfPTable tblData = new PdfPTable(1) { TotalWidth = TABLE_WIDTH };
                        tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                        tblData.SetWidths(new float[] { 1 });
                        tblData.LockedWidth = true;

                        // ------------PdfHeaderRegistrationTicket---------------//
                        AddHeaderToPdf881(tblData, pdfDataModel.PdfHeader);

                        AddBodyToPdf881(tblData, pdfDataModel.PdfInfo);

                        AddIdentificationAndSafetyMeasuresToPdf881(tblData, pdfDataModel.PdfInfo);

                        AddTimeWorkToPdf881(tblData, pdfDataModel.PdfInfo);

                        AddContentPositionsToPdf881(tblData, pdfDataModel.PdfInfo);

                        AddOtherRequirementsToPdf881(tblData, pdfDataModel.PdfInfo);

                        AddRegisterUserToPdf881(tblData, pdfDataModel.PdfInfo);

                        pdfDocument.Add(tblData);
                        pdfDocument.Close();
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                        _pdfInfo.Status = true;

                        return _pdfInfo;
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
                _pdfInfo.Status = false;
            }

            return _pdfInfo;
        }

        private void AddHeaderToPdf881(PdfPTable mainTable, Pdf881HeaderRegistrationTicketModel pdfHeaderWorkTicket)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1, 1 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Từ: {pdfHeaderWorkTicket.RegisterUnit}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Tới: {pdfHeaderWorkTicket.ReceiveUnit}", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fNomal, fSize, "Tel:                  Fax:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, "Tel:                  Fax:       ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fBold, fSize, "ĐĂNG KÝ CÔNG TÁC", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"Số: {pdfHeaderWorkTicket.TicketNumber}", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
        }

        /// <summary>
        /// Add info
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistrationTicket"></param>
        private void AddBodyToPdf881(PdfPTable mainTable, Pdf881InfoRegistrationTicketModel pdfRegistrationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"1. Địa điểm/Thiết bị: {pdfRegistrationTicket.LocationDevice}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"2. Lý do:         [{(pdfRegistrationTicket.TicketType.Value == 1 ? "X" : " ")}]    Kế hoạch [{(pdfRegistrationTicket.TicketType.Value == 2 ? "X" : " ")}]    Ngoài kế hoạch [{(pdfRegistrationTicket.TicketType.Value == 3 ? "X" : " ")}] Sự cố", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"3. Nội dung:    [{(pdfRegistrationTicket.ContentType.Value == 1 ? "X" : " ")}]    Sửa chữa [{(pdfRegistrationTicket.ContentType.Value == 2 ? "X" : " ")}]    Thí nghiệm     [{(pdfRegistrationTicket.ContentType.Value == 3 ? "X" : " ")}] Xây dựng", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"{pdfRegistrationTicket.WorkContent}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

        }

        /// <summary>
        /// 4. Nhận diện mối nguy và biện pháp an toàn:
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddIdentificationAndSafetyMeasuresToPdf881(PdfPTable mainTable, Pdf881InfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"4. Nhận diện mối nguy và biện pháp an toàn:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            if (pdfRegistationTicket.IdentificationAndSafetyMeasures != null && pdfRegistationTicket.IdentificationAndSafetyMeasures.Count > 0)
            {
                PdfPTable tblData = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
                tblData.SetWidths(new float[] { 1, 4, 6 });
                tblData.LockedWidth = true;
                fSize = 11;
                // Header row.
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "STT", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "Nhận diện mối nguy", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "Biện pháp an toàn", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                int index = 0;
                foreach (IdentificationAndSafetyMeasuresModel identificationAndSafety in pdfRegistationTicket.IdentificationAndSafetyMeasures)
                {
                    index++;
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{index.ToString()}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{identificationAndSafety.Identification}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{identificationAndSafety.SafetyMeasures}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }

                PdfPCell cellMerge = new PdfPCell
                {
                    Padding = 0,
                    Border = Rectangle.NO_BORDER,
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }

        }

        /// <summary>
        /// 7. Những yêu cầu khác:
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistrationTicket"></param>
        private void AddOtherRequirementsToPdf881(PdfPTable mainTable, Pdf881InfoRegistrationTicketModel pdfRegistrationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"7. Những yêu cầu khác: {pdfRegistrationTicket.OtherContents}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

        }

        /// <summary>
        /// 8. Người đăng ký công tác:
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistrationTicket"></param>
        private void AddRegisterUserToPdf881(PdfPTable mainTable, Pdf881InfoRegistrationTicketModel pdfRegistrationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "8. Người đăng ký công tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPTable tblData = new PdfPTable(4) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 1, 1, 2 });
            tblData.LockedWidth = true;

            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Họ tên: {pdfRegistrationTicket.RegisterUser.FullName}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            if (pdfRegistrationTicket.RegisterUser != null && !string.IsNullOrEmpty(pdfRegistrationTicket.RegisterUser.SignatureContent))
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, $"Chữ ký: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellImageSignature(pdfRegistrationTicket.RegisterUser.SignatureContent, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ………………", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            }

            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Ngày {pdfRegistrationTicket.RegistrationDate.Day} tháng {pdfRegistrationTicket.RegistrationDate.Month} năm {pdfRegistrationTicket.RegistrationDate.Year}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Chức vụ: {pdfRegistrationTicket.Position}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Số điện thoại: {pdfRegistrationTicket.PhoneNumber}", 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Email: {pdfRegistrationTicket.Email}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));


            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        /// <summary>
        /// 6. Danh sách người tham gia công việc:
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddContentPositionsToPdf881(PdfPTable mainTable, Pdf881InfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"6. Danh sách người tham gia công việc:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            if (pdfRegistationTicket.ContentPositions != null && pdfRegistationTicket.ContentPositions.Count > 0)
            {
                PdfPTable tblData = new PdfPTable(5) { TotalWidth = TABLE_WIDTH };
                tblData.SetWidths(new float[] { 1, 4, 2, 2, 5 });
                tblData.LockedWidth = true;
                fSize = 11;
                // Header row.
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "STT", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "Họ tên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "Số thẻ AT", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "Bậc thợ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellBorder(_fNomal, fSize, "Nhiệm vụ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                int indexPosition = 1;
                foreach (ContentPositionModel contentPosition in pdfRegistationTicket.ContentPositions)
                {
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{ToRoman(indexPosition)}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, contentPosition.Position, 4, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    int index = 0;
                    foreach (StakeholdersModel stakeholders in contentPosition.Stakeholders)
                    {
                        index++;
                        tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{index.ToString()}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{stakeholders.FullName}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{stakeholders.LaborCard}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{stakeholders.LaborRankStr}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellBorder(_fNomal, fSize, $"{stakeholders.Mission}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    }
                }

                PdfPCell cellMerge = new PdfPCell
                {
                    Padding = 0,
                    Border = Rectangle.NO_BORDER,
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }

        }

        /// <summary>
        /// 7. Những yêu cầu khác:
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddTimeWorkToPdf881(PdfPTable mainTable, Pdf881InfoRegistrationTicketModel pdfRegistrationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "5. Thời gian:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"     Từ: {pdfRegistrationTicket.StartDate.Hour} h {pdfRegistrationTicket.StartDate.Minute} ngày {pdfRegistrationTicket.StartDate.Day} tháng {pdfRegistrationTicket.StartDate.Month} năm {pdfRegistrationTicket.StartDate.Year}     [{(pdfRegistrationTicket.IsWorkDaily ? " " : "X")}] Liên tục trong thời gian đăng ký", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"     Đến: {pdfRegistrationTicket.EndDate.Hour} h {pdfRegistrationTicket.EndDate.Minute} ngày {pdfRegistrationTicket.EndDate.Day} tháng {pdfRegistrationTicket.EndDate.Month} năm {pdfRegistrationTicket.EndDate.Year}     [{(pdfRegistrationTicket.IsWorkDaily ? "X" : " ")}] Hằng ngày từ {(pdfRegistrationTicket.IsWorkDaily && pdfRegistrationTicket.StartTimeDaily != null ? pdfRegistrationTicket.StartTimeDaily.Replace(":", "h") : "...h...")} đến {(pdfRegistrationTicket.IsWorkDaily && pdfRegistrationTicket.EndTimeDaily != null ? pdfRegistrationTicket.EndTimeDaily.Replace(":", "h") : "...h...")}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

        }

        private string ToRoman(int number)
        {
            string[] thousands = { "", "M", "MM", "MMM" };
            string[] hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
            string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            string[] ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
            return thousands[number / 1000] + hundreds[number / 100 % 10] + tens[number / 10 % 10] + ones[number % 10];
        }

        private PdfRegistrationTicketModel CreatePdfData(RegistrationTicketModel workTicketDetail)
        {
            PdfRegistrationTicketModel pdfContentModel = new PdfRegistrationTicketModel
            {
                PdfHeader = new PdfHeaderRegistrationTicketModel()
                {
                    ReceiveUnit = workTicketDetail.ReceiveUnit.Label,
                    RegisterUnit = workTicketDetail.RegisterUnit.Label,
                    TicketNumber = workTicketDetail.TicketNumber,
                },
                PdfInfo = new PdfInfoRegistrationTicketModel()
                {
                    RegulationNumber = workTicketDetail.RegulationNumber,
                    RegisterUser = workTicketDetail.RegisterUser,
                    Position = workTicketDetail.Position,
                    PhoneNumber = workTicketDetail.PhoneNumber,
                    Email = workTicketDetail.Email,
                    WorkCondition = workTicketDetail.WorkCondition,
                    WorkContent = workTicketDetail.WorkContent,
                    WorkPlacement = workTicketDetail.WorkPlacement,
                    StartDate = workTicketDetail.StartDate,
                    EndDate = workTicketDetail.EndDate,
                    UnitNumberAndUnitStaff = workTicketDetail.UnitNumberAndUnitStaff,
                    Workers = workTicketDetail.Workers,
                    Units = workTicketDetail.Units,
                    OtherRequirements = workTicketDetail.OtherRequirements,
                    Commanders = workTicketDetail.Commanders,
                    LeaderUser = workTicketDetail.LeaderUser,
                    GuardUser = workTicketDetail.GuardUser,
                    Supervisors = workTicketDetail.Supervisors
                }
            };

            return pdfContentModel;
        }

        private PdfInfoModel GeneratePdfFile(PdfRegistrationTicketModel pdfDataModel, string outputFile)
        {
            _pdfInfo = new PdfInfoModel();
            try
            {
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    //Create document
                    Document pdfDocument = new Document(PageSize.A4);
                    pdfDocument.SetMargins(0, 0, 25, 25);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, fileStream);
                    // Open the document to enable you to write to the document
                    pdfDocument.Open();
                    try
                    {
                        PdfPTable tblData = new PdfPTable(1) { TotalWidth = TABLE_WIDTH };
                        tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                        tblData.SetWidths(new float[] { 1 });
                        tblData.LockedWidth = true;

                        // ------------PdfHeaderRegistrationTicket---------------//
                        AddHeaderToPdf(tblData, pdfDataModel.PdfHeader, pdfDataModel.PdfInfo.RegisterUser.CompanyName);

                        AddBodyToPdf(tblData, pdfDataModel);

                        AddLeaderUserToPdf(tblData, pdfDataModel.PdfInfo);

                        AddSupervisorsToPdf(tblData, pdfDataModel.PdfInfo);

                        AddCommandersToPdf(tblData, pdfDataModel.PdfInfo);

                        AddGuardUserToPdf(tblData, pdfDataModel.PdfInfo);

                        AddWorkerToPdf(tblData, pdfDataModel.PdfInfo);

                        AddUnitsToPdf(tblData, pdfDataModel.PdfInfo);

                        AddOtherRequirementsToPdf(tblData, pdfDataModel.PdfInfo);

                        AddSignatureToPdf(tblData, pdfDataModel.PdfInfo);

                        pdfDocument.Add(tblData);
                        pdfDocument.Close();
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                        _pdfInfo.Status = true;

                        return _pdfInfo;
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
                _pdfInfo.Status = false;
            }

            return _pdfInfo;
        }

        private void AddHeaderToPdf(PdfPTable mainTable, PdfHeaderRegistrationTicketModel pdfHeader, string companyName)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 3, 5 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fNomal, fSize, companyName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fBold, fSize, pdfHeader.RegisterUnit, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "Độc lập - Tự do - Hạnh phúc", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));


            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Số: {pdfHeader.TicketNumber}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, $"…………, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);

            mainTable.AddCell(cellMerge);
            mainTable.AddCell(GetCellSign(_fBold, fSize, "ĐĂNG KÝ CÔNG TÁC", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
        }

        /// <summary>
        /// Add info
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistrationTicket"></param>
        private void AddBodyToPdf(PdfPTable mainTable, PdfRegistrationTicketModel pdfRegistrationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"Kính gửi: {pdfRegistrationTicket.PdfHeader.ReceiveUnit}", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"Căn cứ {pdfRegistrationTicket.PdfInfo.RegulationNumber ?? "........................."}, Công ty (Chi nhánh) {pdfRegistrationTicket.PdfInfo.RegisterUser.CompanyName ?? "......."} do Ông (Bà) {pdfRegistrationTicket.PdfInfo.RegisterUser.FullName} đại diện, Chức vụ {pdfRegistrationTicket.PdfInfo.Position ?? "....."} Điện thoại (Email) {pdfRegistrationTicket.PdfInfo.PhoneNumber ?? pdfRegistrationTicket.PdfInfo.Email} đăng ký tiến hành công việc, cụ thể như sau:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"1. Nội dung công việc: {pdfRegistrationTicket.PdfInfo.WorkContent}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"2. Địa điểm (hoặc thiết bị) tiến hành công việc: {pdfRegistrationTicket.PdfInfo.WorkPlacement}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"3. Điều kiện cần có để thực hiện công việc: {pdfRegistrationTicket.PdfInfo.WorkCondition}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "4. Thời gian tiến hành công việc:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"     Bắt đầu: {pdfRegistrationTicket.PdfInfo.StartDate.Hour} giờ {pdfRegistrationTicket.PdfInfo.StartDate.Minute} ngày {pdfRegistrationTicket.PdfInfo.StartDate.Day} tháng {pdfRegistrationTicket.PdfInfo.StartDate.Month} năm {pdfRegistrationTicket.PdfInfo.StartDate.Year}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"     Kết thúc: {pdfRegistrationTicket.PdfInfo.EndDate.Hour} giờ {pdfRegistrationTicket.PdfInfo.EndDate.Minute} ngày {pdfRegistrationTicket.PdfInfo.EndDate.Day} tháng {pdfRegistrationTicket.PdfInfo.EndDate.Month} năm {pdfRegistrationTicket.PdfInfo.EndDate.Year}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            List<string> numberUnits = pdfRegistrationTicket.PdfInfo.UnitNumberAndUnitStaff == null ? new List<string>() : pdfRegistrationTicket.PdfInfo.UnitNumberAndUnitStaff.Split(',').ToList();
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"5. Số lượng Đơn vị công tác (nếu có): {numberUnits.Count} ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            mainTable.AddCell(GetCellSign(_fNomal, fSize, "6. Số lượng nhân viên 01 Đơn vị công tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            if (numberUnits.Count > 0)
            {
                PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
                tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                tblData.SetWidths(new float[] { 1, 1 });
                tblData.LockedWidth = true;

                int index = 0;
                foreach (string unit in numberUnits)
                {
                    index++;
                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"   6.{index}) {unit} /ng", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                if (numberUnits.Count % 2 > 0)
                {
                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                PdfPCell cellMerge = new PdfPCell
                {
                    Padding = 0,
                    Border = Rectangle.NO_BORDER,
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }
        }

        /// <summary>
        /// 7. Người lãnh đạo công việc
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddLeaderUserToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;

            if (pdfRegistationTicket.LeaderUser != null)
            {
                mainTable.AddCell(GetCellSign(_fNomal, fSize, $"7. Người lãnh đạo công việc (nếu có): {pdfRegistationTicket.LeaderUser.FullName}    Bậc ATĐ {pdfRegistationTicket.LeaderUser.ElectricRankStr}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            }
            else
            {
                mainTable.AddCell(GetCellSign(_fNomal, fSize, "7. Người lãnh đạo công việc (nếu có):", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            }

        }


        /// <summary>
        /// 8. Người giám sát ATĐ
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddSupervisorsToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"8. Người giám sát ATĐ (nếu có):", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            if (pdfRegistationTicket.Supervisors != null && pdfRegistationTicket.Supervisors.Count > 0)
            {
                PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
                tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                tblData.SetWidths(new float[] { 1, 1 });
                tblData.LockedWidth = true;

                int index = 0;
                foreach (StakeholdersResponseModel supervisor in pdfRegistationTicket.Supervisors)
                {
                    index++;
                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"   8.{index.ToString()}) {supervisor.FullName}    Bậc ATĐ {supervisor.ElectricRankStr}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                if (pdfRegistationTicket.Supervisors.Count % 2 > 0)
                {
                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                PdfPCell cellMerge = new PdfPCell
                {
                    Padding = 0,
                    Border = Rectangle.NO_BORDER,
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }

        }

        /// <summary>
        /// 9. Người chỉ huy trực tiếp
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddCommandersToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, $"9. Người chỉ huy trực tiếp:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            if (pdfRegistationTicket.Commanders != null && pdfRegistationTicket.Commanders.Count > 0)
            {
                PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
                tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                tblData.SetWidths(new float[] { 1, 1 });
                tblData.LockedWidth = true;

                int index = 0;
                foreach (StakeholdersResponseModel comander in pdfRegistationTicket.Commanders)
                {
                    index++;
                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"   9.{index}) {comander.FullName}    Bậc ATĐ {comander.ElectricRankStr}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                if (pdfRegistationTicket.Commanders.Count % 2 > 0)
                {
                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }

                PdfPCell cellMerge = new PdfPCell
                {
                    Padding = 0,
                    Border = Rectangle.NO_BORDER,
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }
        }

        /// <summary>
        /// Người cảnh giới
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfHeaderWorkTicket"></param>
        private void AddGuardUserToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;

            if (pdfRegistationTicket.GuardUser != null)
            {
                mainTable.AddCell(GetCellSign(_fNomal, fSize, $"10. Người cảnh giới (nếu có): {pdfRegistationTicket.GuardUser.FullName}    Bậc ATĐ {pdfRegistationTicket.GuardUser.ElectricRankStr}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            }
            else
            {
                mainTable.AddCell(GetCellSign(_fNomal, fSize, "10. Người cảnh giới (nếu có):", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            }

        }


        /// <summary>
        /// Danh sách những người được đề cử để thực hiện công việc
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfHeaderWorkTicket"></param>
        private void AddWorkerToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "11. Danh sách những người được cử để thực hiện công việc:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPTable tblData = new PdfPTable(4) { TotalWidth = TABLE_WIDTH };
            tblData.SetWidths(new float[] { 1, 3, 2, 4 });
            tblData.LockedWidth = true;
            fSize = 11;
            // Header row.
            tblData.AddCell(GetCellBorder(_fNomal, fSize, "TT", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellBorder(_fNomal, fSize, "Họ tên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellBorder(_fNomal, fSize, "Thẻ ATĐ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellBorder(_fNomal, fSize, "Nhiệm vụ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            int index = 0;
            if (pdfRegistationTicket.Workers != null && pdfRegistationTicket.Workers.Count > 0)
            {
                foreach (StakeholdersResponseModel stakeholders in pdfRegistationTicket.Workers)
                {
                    index++;
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, index.ToString(), 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, stakeholders.FullName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, stakeholders.ElectricRankCard, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, stakeholders.Mission, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        /// <summary>
        /// Các Đơn vị QLVH khác có liên quan đến công việc
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddUnitsToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "12. Các Đơn vị QLVH khác có liên quan đến công việc, bao gồm:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            if (pdfRegistationTicket.Units != null && pdfRegistationTicket.Units.Count > 0 && !(pdfRegistationTicket.Units.Count == 1 && pdfRegistationTicket.Units[0] == ""))
            {
                PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
                tblData.SetWidths(new float[] { 1, 1 });
                tblData.LockedWidth = true;
                tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                int index = 0;
                foreach (string unit in pdfRegistationTicket.Units)
                {
                    index++;

                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"   12.{index} {unit}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                if (pdfRegistationTicket.Units.Count % 2 > 0)
                {
                    tblData.AddCell(GetCellSign(_fNomal, fSize, $"", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                PdfPCell cellMerge = new PdfPCell
                {
                    Padding = 0,
                    Border = Rectangle.NO_BORDER,
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }
        }

        /// <summary>
        /// Những yêu cầu khác
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddOtherRequirementsToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "13. Những yêu cầu khác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPTable tblData = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblData.SetWidths(new float[] { 1, 3, 6 });
            tblData.LockedWidth = true;
            fSize = 11;
            // Header row.
            tblData.AddCell(GetCellBorder(_fNomal, fSize, "TT", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellBorder(_fNomal, fSize, "Vị trí", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellBorder(_fNomal, fSize, "Biện pháp an toàn/Tiếp đất di động", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            int index = 0;

            if (pdfRegistationTicket.OtherRequirements != null && pdfRegistationTicket.OtherRequirements.Count > 0)
            {
                foreach (OtherRequirementsModel otherRequirements in pdfRegistationTicket.OtherRequirements)
                {
                    index++;
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, index.ToString(), 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, otherRequirements.Location, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(GetCellBorder(_fNomal, fSize, otherRequirements.SafetyMeasures, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }

            }


            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="pdfRegistationTicket"></param>
        private void AddSignatureToPdf(PdfPTable mainTable, PdfInfoRegistrationTicketModel pdfRegistationTicket)
        {
            int fSize = 13;
            mainTable.AddCell(GetCellSign(_fBold, fSize, "", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1, 1 });
            tblData.LockedWidth = true;

            tblData.AddCell(GetCellSign(_fBold, fSize, "Nơi nhận:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize, "NGƯỜI ĐĂNG KÝ CÔNG TÁC", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fNomal, fSize, "- Như trên;", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, "(Ký tên, ghi rõ họ tên)", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fNomal, fSize, "- Lưu.", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, "", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fNomal, fSize, "", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            if (pdfRegistationTicket.RegisterUser != null && !string.IsNullOrEmpty(pdfRegistationTicket.RegisterUser.SignatureContent))
            {
                tblData.AddCell(GetCellImageSignature(pdfRegistationTicket.RegisterUser.SignatureContent, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }

            tblData.AddCell(GetCellSign(_fNomal, fSize, "", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            if (pdfRegistationTicket.RegisterUser != null && !string.IsNullOrEmpty(pdfRegistationTicket.RegisterUser.SignatureContent))
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, pdfRegistationTicket.RegisterUser.FullName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }


            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private PdfPCell GetCellImageSignature(string signatureContent,
            int colSpan, int rowSpan, int hAlignment,
            int vAlignment, int fixHeight = 21, bool isBorder = true)
        {
            if (string.IsNullOrWhiteSpace(signatureContent))
            {
                PdfPCell cell = new PdfPCell(new Phrase(" "))
                {
                    HorizontalAlignment = hAlignment,
                    VerticalAlignment = vAlignment,
                    Rowspan = rowSpan,
                    Colspan = colSpan,
                    FixedHeight = fixHeight,
                };

                return cell;
            }

            string signatureBase64 = signatureContent.Remove(0, signatureContent.IndexOf(',') + 1);
            byte[] imgBytes = Convert.FromBase64String(signatureBase64);

            Image img = Image.GetInstance(imgBytes);
            float imgHeight = 23;
            float imgWidth = (img.Width * imgHeight) / img.Height;
            img.ScaleAbsolute(imgWidth, imgHeight);

            PdfPCell imageCell = new PdfPCell(img)
            {
                Colspan = colSpan,
                Rowspan = rowSpan,
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Padding = 2,
            };
            if (!isBorder)
                imageCell.Border = 0;

            return imageCell;
        }

        private PdfPCell GetCellSign(BaseFont fBase, float fSize, string textContent, int colSpan, int rowSpan, int hAlignment = 1, int vAlignment = 1, int minHeight = 16)
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

        private PdfPCell GetCellBorder(BaseFont fBase, float fSize, string textContent, int colSpan, int rowSpan, int hAlignment = 1, int vAlignment = 1, int minHeight = 16)
        {
            Font font = new Font(fBase, fSize);
            PdfPCell cell = new PdfPCell(new Phrase(textContent, font))
            {
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Rowspan = rowSpan,
                Colspan = colSpan,
                MinimumHeight = minHeight,
                BorderColor = BaseColor.BLACK
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

        public HttpResponseMessage DownloadFileTemplate()
        {
            var pathCurrent = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var outputFile = Path.Combine(pathCurrent.Substring(6), "PCT_Template.xlsx");

            if (File.Exists(outputFile))
            {
                var fileBytes = File.ReadAllBytes(outputFile);

                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), "PCT_Template.xlsx");
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}
