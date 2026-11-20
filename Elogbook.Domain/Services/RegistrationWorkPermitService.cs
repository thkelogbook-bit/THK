using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response.WorkPermit;
using Elogbook.Data;
using Elogbook.Data.WorkPermit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class RegistrationWorkPermitService : IRegistrationWorkPermitService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        public RegistrationWorkPermitService(ElogbookUnitOfWork unitOfWork,
            ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        public int AddRegistrationTicket(LctAddEditRegistrationTicketModel requestModel)
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

            LctRegistrationTicket entity;
            switch (requestModel.RegistrationTicketType)
            {
                default:
                    entity = new LctRegistrationTicket
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
                        StatusValue = requestModel.IsSave ? (int)WorkPermitStatus.Unsent : (int)WorkPermitStatus.Waiting,
                        Attachments = requestModel.FileAttachments != null && requestModel.FileAttachments.Any() ? string.Join(",", requestModel.FileAttachments) : string.Empty,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        RegistrationTicketType = (int)WorkPermitTypeLCT.DIEN,
                        Units = string.Join(Environment.NewLine, requestModel.Units),
                        RegistrationUnitView = requestModel.RegistrationUnitView,
                    };

                    entity.RegistrationUnitView = entity.RegistrationUnitView ?? _commonService.GetRegistrationUnitView((int)entity.CreatedUserID);

                    // Người lãnh đạo
                    List<LctStakeholders> stakeholders = new List<LctStakeholders>();
                    if (requestModel.LeaderUser != null)
                    {
                        Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(requestModel.LeaderUser.UserName, requestModel.LeaderUser.IsExternal);
                        if (userInfo != null)
                        {
                            int? assignToUserID = null;
                            if (!requestModel.LeaderUser.IsExternal)
                                assignToUserID = userInfo.ID;

                            stakeholders.Add(new LctStakeholders
                            {
                                AssignToUserID = assignToUserID,
                                FullName = userInfo.FullName,
                                ElectricRank = userInfo.ElectricRankStr,
                                ElectricRankCard = userInfo.ElectricRankCard,
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
                            Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                            if (userInfo != null)
                            {
                                int? assignToUserID = null;
                                if (!item.IsExternal)
                                    assignToUserID = userInfo.ID;

                                stakeholders.Add(new LctStakeholders
                                {
                                    AssignToUserID = assignToUserID,
                                    FullName = userInfo.FullName,
                                    ElectricRank = userInfo.ElectricRankStr,
                                    ElectricRankCard = userInfo.ElectricRankCard,
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
                            Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                            if (userInfo != null)
                            {
                                int? assignToUserID = null;
                                if (!item.IsExternal)
                                    assignToUserID = userInfo.ID;

                                stakeholders.Add(new LctStakeholders
                                {
                                    AssignToUserID = assignToUserID,
                                    FullName = userInfo.FullName,
                                    UserName = userInfo.UserName,
                                    IsExternal = item.IsExternal,
                                    PositionName = userInfo.PositionName,
                                    ElectricRank = userInfo.ElectricRankStr,
                                    ElectricRankCard = userInfo.ElectricRankCard,
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
                        Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(requestModel.GuardUser.UserName, requestModel.GuardUser.IsExternal);
                        if (userInfo != null)
                        {
                            int? assignToUserID = null;
                            if (!requestModel.GuardUser.IsExternal)
                                assignToUserID = userInfo.ID;

                            stakeholders.Add(new LctStakeholders
                            {
                                AssignToUserID = assignToUserID,
                                FullName = userInfo.FullName,
                                ElectricRank = userInfo.ElectricRankStr,
                                ElectricRankCard = userInfo.ElectricRankCard,
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
                            Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                            if (userInfo != null)
                            {
                                int? assignToUserID = null;
                                if (!item.IsExternal)
                                    assignToUserID = userInfo.ID;

                                stakeholders.Add(new LctStakeholders
                                {
                                    AssignToUserID = assignToUserID,
                                    FullName = userInfo.FullName,
                                    UserName = userInfo.UserName,
                                    IsExternal = item.IsExternal,
                                    PositionName = userInfo.PositionName,
                                    ElectricRank = userInfo.ElectricRankStr,
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
                    List<LctOtherRequirements> otherRequirements = new List<LctOtherRequirements>();
                    if (requestModel.OtherRequirements != null)
                    {
                        foreach (var item in requestModel.OtherRequirements)
                        {
                            otherRequirements.Add(new LctOtherRequirements
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
                case (int)WorkPermitTypeLCT.COTHUYNHIETHOA:
                    entity = new LctRegistrationTicket
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
                        StatusValue = requestModel.IsSave ? (int)WorkPermitStatus.Unsent : (int)WorkPermitStatus.Waiting,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        Attachments = requestModel.FileAttachments != null && requestModel.FileAttachments.Any() ? string.Join(",", requestModel.FileAttachments) : string.Empty,
                        RegistrationTicketType = (int)WorkPermitTypeLCT.COTHUYNHIETHOA,
                        RegulationNumber = requestModel.RegulationNumber,
                        WorkPlacement = requestModel.WorkPlacement,
                        WorkCondition = requestModel.WorkCondition,
                        RegistrationUnitView = requestModel.RegistrationUnitView,
                    };

                    entity.RegulationNumber = !string.IsNullOrWhiteSpace(requestModel.RegulationNumber) ? requestModel.RegulationNumber : string.Empty;
                    entity.WorkPlacement = !string.IsNullOrWhiteSpace(requestModel.WorkPlacement) ? requestModel.WorkPlacement : string.Empty;
                    entity.WorkCondition = !string.IsNullOrWhiteSpace(requestModel.WorkCondition) ? requestModel.WorkCondition : string.Empty;
                    entity.RegistrationUnitView = entity.RegistrationUnitView ?? _commonService.GetRegistrationUnitView((int)entity.CreatedUserID);
                    // Những người tham gia công việc
                    List<LctContentPosition> contentPositions = new List<LctContentPosition>();
                    foreach (var item in requestModel.ContentPositions)
                    {
                        var data = new LctContentPosition
                        {
                            Position = item.Position,
                            RegistrationTicketID = entity.ID,
                        };
                        if (item.Stakeholders != null && item.Stakeholders.Count > 0)
                        {
                            List<LctStakeholders> listStakeholders = new List<LctStakeholders>();
                            foreach (var worker in item.Stakeholders)
                            {
                                Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(worker.UserName, worker.IsExternal);
                                int? assignToUserID = null;
                                if (userInfo != null)
                                {
                                    if (!worker.IsExternal)
                                        assignToUserID = userInfo.ID;

                                    listStakeholders.Add(new LctStakeholders
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
                    List<LctIdentificationAndSafetyMeasures> identificationAndSafetyMeasures = new List<LctIdentificationAndSafetyMeasures>();
                    foreach (var item in requestModel.IdentificationAndSafetyMeasuresModels)
                    {
                        identificationAndSafetyMeasures.Add(new LctIdentificationAndSafetyMeasures
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

            // Tracking trạng thái thay đổi lệnh công tác
            var ticketStatus = requestModel.IsSave ? (int)WorkPermitStatus.Unsent : (int)WorkPermitStatus.Waiting;
            var description = requestModel.IsSave ? "Lưu chỉnh sửa lệnh đăng ký" : "Gửi lệnh đăng ký";
            List<LctTicketStatusHistory> ticketStatusHistories = new List<LctTicketStatusHistory>
            {
                AddTicketStatusHistory(0, requestModel.CurrentUserID, entity.UpdatedAt, ticketStatus, description,""),
            };
            entity.TicketStatusHistories = ticketStatusHistories;

            _unitOfWork.ElogbookRepository<LctRegistrationTicket>().Add(entity);

            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                //Add ticket number to tracking table
                try
                {
                    var arrNumTicket = requestModel.TicketNumber.Split('/');
                    int.TryParse(arrNumTicket[0], out int numTicket);
                    var ticketType = "DKLCTD";
                    if (requestModel.RegistrationTicketType == (int)WorkPermitTypeLCT.COTHUYNHIETHOA)
                    {
                        ticketType = "DKLTCNH";
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
            var workerIsUse = _unitOfWork.ElogbookRepository<LctStakeholders>().GetQuery(x => x.AssignToUserID == userID && !x.IsDeleted).Any();

            return workerIsUse;
        }

        public LctRegistrationTicketModel GetRegistrationTicketById(int id)
        {
            var data = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => x.ID == id && !x.IsDeleted);
            var entity = data.Select(
               e => new LctRegistrationTicketModel()
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
                   RegisterUser = new LctUserInTicketModel
                   {
                       ID = e.CreatedUser.ID,
                       UserName = e.CreatedUser.UserName,
                       FullName = e.CreatedUser.FullName,
                       SignatureContent = e.CreatedUser.Signature
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
                   // Nhận diện mối nguy và biện pháp an toàn
                   IdentificationAndSafetyMeasures = e.IdentificationAndSafetyMeasures.Where(x => !x.IsDeleted).Select(
                                            t => new LctIdentificationAndSafetyMeasuresModel()
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
                                            t => new LctOtherRequirementsModel()
                                            {
                                                ID = t.ID,
                                                Location = t.Location,
                                                SafetyMeasures = t.SafetyMeasures,
                                            }).ToList(),
                   // Tài liệu liên quan (nếu có)
                   Attackments = e.Attachments,
                   // Lịch sử thay đổi trạng thái lệnh công tác
                   TicketStatusHistories = e.TicketStatusHistories.Select(
                                            t => new LctTicketStatusHistoryModel()
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

                    case (int)WorkPermitTypeLCT.COTHUYNHIETHOA:
                        entity.ContentPositions = GetContentPositions(entity.ID);
                        break;
                }

                // Lấy thông tin lệnh công tác
                var workTicketEntity = _unitOfWork.ElogbookRepository<LctWorkTicket>()
                    .GetQuery(x => x.RegistrationTicketID == entity.ID).FirstOrDefault();
                if (workTicketEntity != null)
                {
                    entity.WorkTicketID = workTicketEntity.ID;
                }
            }

            if (entity != null && !string.IsNullOrWhiteSpace(entity.Attackments))
            {
                entity.FileAttachments = new List<Core.Models.Response.AttachmentModel>();
                var attackments = entity.Attackments.Split(',');
                foreach (var item in attackments)
                {
                    if (int.TryParse(item, out int idItem))
                    {
                        var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted &&
                                                                        x.ID == idItem).FirstOrDefault();
                        if (att != null)
                        {
                            entity.FileAttachments.Add(new Core.Models.Response.AttachmentModel
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

            return entity;
        }

        public ResponseBaseModel GetRegistrationTickets(LctSearchRegistrationTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            //Mặc định không truyền data thì tìm kiếm theo PCTĐ
            if (requestModel.RegistrationTicketType.GetValueOrDefault(0) < 1) requestModel.RegistrationTicketType = 1;

            var entities = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketType == requestModel.RegistrationTicketType);

            //Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            //Số lệnh công tác
            if (!string.IsNullOrWhiteSpace(requestModel.TicketNumber))
            {
                entities = entities.Where(x => x.TicketNumber.Contains(requestModel.TicketNumber));
            }
            //Loại lệnh
            if (requestModel.TicketType > 0)
            {
                entities = entities.Where(x => x.TicketType == requestModel.TicketType);
            }
            //Trạng thái lệnh
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
            //Ngày đăng ký lệnh (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.RegistrationDate) >= frDate.Date);
            }
            //Ngày đăng ký lệnh (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.RegistrationDate) <= toDate.Date);
            }

            // Return and paging data
            var page = entities.Select(
                e => new LctGeneralRegistrationTicketModel()
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
            // Trường hợp thêm mới thì kiểm tra Số lệnh đăng ký công tác
            if (isAddMode)
            {
                if (isTicketNumber)
                {
                    var currentDate = DateTime.Now;

                    var isExistCurrent = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => x.TicketNumber == ticketNumber
                                                                                        && !x.IsDeleted
                                                                                        && x.RegistrationTicketType == (int)WorkPermitTypeLCT.DIEN
                                                                                        && x.InsertedAt.Year == currentDate.Year).Any();

                    return isExistCurrent;
                }
                else
                {
                    return _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => x.RegulationNumber == ticketNumber
                                                                                        && !x.IsDeleted
                                                                                        && x.RegistrationTicketType == (int)WorkPermitTypeLCT.DIEN
                                                                                        && x.InsertedAt.Year == DateTime.Now.Year).Any();
                }
            }
            // Trường hợp cập nhật thì kiểm tra Số lệnh đăng ký công tác và id
            else
            {
                if (isTicketNumber)
                {
                    var currentDate = DateTime.Now;

                    var isExistCurrent = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => x.ID != id
                                                                                    && x.TicketNumber == ticketNumber
                                                                                    && !x.IsDeleted && x.RegistrationTicketType == (int)WorkPermitTypeLCT.DIEN
                                                                                    && x.InsertedAt.Year == currentDate.Year).Any();

                    return isExistCurrent;
                }
                else
                {
                    return _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => x.ID != id
                                                                                    && x.RegulationNumber == ticketNumber
                                                                                    && !x.IsDeleted && x.RegistrationTicketType == (int)WorkPermitTypeLCT.DIEN
                                                                                    && x.InsertedAt.Year == DateTime.Now.Year).Any();
                }
            }
        }

        /// <summary>
        /// kiểm tra trùng dữ liệu theo số lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>.
        public bool CheckDuplicateData(int id, string ticketNumber, bool isAddMode)
        {
            var currentDate = DateTime.Now;

            // Trường hợp thêm mới thì kiểm tra Số lệnh đăng ký công tác
            if (isAddMode)
            {
                var isExistCurrent = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => x.TicketNumber == ticketNumber
                                                                                    && !x.IsDeleted
                                                                                    && x.RegistrationTicketType == (int)WorkPermitTypeLCT.COTHUYNHIETHOA
                                                                                    && x.InsertedAt.Year == currentDate.Year).Any();

                return isExistCurrent;
            }
            // Trường hợp cập nhật thì kiểm tra Số lệnh đăng ký công tác và id
            else
            {
                var isExistCurrent = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetQuery(x => x.ID != id
                                                                                    && x.TicketNumber == ticketNumber
                                                                                    && !x.IsDeleted && x.RegistrationTicketType == (int)WorkPermitTypeLCT.COTHUYNHIETHOA
                                                                                    && x.InsertedAt.Year == currentDate.Year).Any();

                return isExistCurrent;
            }
        }

        public bool UpdateStatusRegistrationTicket(int id, int statusValue, string description, int userID, string lstAttachment)
        {
            var entity = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetById(id);
            if (entity != null && entity.StatusValue != statusValue)
            {
                entity.StatusValue = statusValue;
                if (statusValue != (int)WorkPermitStatus.Approved)
                {
                    entity.RegistrationDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<LctRegistrationTicket>().Update(entity);

                var ticketStatusHistory = AddTicketStatusHistory(entity.ID, userID, entity.UpdatedAt, statusValue, description, lstAttachment);
                _unitOfWork.ElogbookRepository<LctTicketStatusHistory>().Add(ticketStatusHistory);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool UpdateRegistrationTicket(LctAddEditRegistrationTicketModel requestModel)
        {
            if (requestModel.IsSave)
            {
                requestModel.RegistrationDate = null;
            }
            else if (!requestModel.RegistrationDate.HasValue)
            {
                requestModel.RegistrationDate = DateTime.Now;
            }

            var entity = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetById(requestModel.ID);
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
                entity.StatusValue = requestModel.IsSave ? (int)WorkPermitStatus.Unsent : (int)WorkPermitStatus.Waiting;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.Units = requestModel.Units == null ? string.Empty : string.Join(Environment.NewLine, requestModel.Units);

                switch (entity.RegistrationTicketType)
                {
                    default:
                        entity.RegulationNumber = requestModel.RegulationNumber;
                        entity.WorkPlacement = requestModel.WorkPlacement;
                        entity.WorkCondition = requestModel.WorkCondition;
                        break;

                    case (int)WorkPermitTypeLCT.COTHUYNHIETHOA:
                        entity.RegulationNumber = !string.IsNullOrWhiteSpace(requestModel.RegulationNumber) ? requestModel.RegulationNumber : string.Empty;
                        entity.WorkPlacement = !string.IsNullOrWhiteSpace(requestModel.WorkPlacement) ? requestModel.WorkPlacement : string.Empty;
                        entity.WorkCondition = !string.IsNullOrWhiteSpace(requestModel.WorkCondition) ? requestModel.WorkCondition : string.Empty;
                        entity.LocationDevice = !string.IsNullOrWhiteSpace(requestModel.LocationDevice) ? requestModel.LocationDevice : string.Empty;
                        break;
                }

                _unitOfWork.ElogbookRepository<LctRegistrationTicket>().Update(entity);

                var stakeholders = _unitOfWork.ElogbookRepository<LctStakeholders>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();

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

                        var entityOthers = _unitOfWork.ElogbookRepository<LctOtherRequirements>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();
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

                                    _unitOfWork.ElogbookRepository<LctOtherRequirements>().Update(updateRow);
                                }
                            }
                            else
                            {
                                _unitOfWork.ElogbookRepository<LctOtherRequirements>().Delete(updateRow, requestModel.CurrentUserID);
                            }
                        }

                        if (requestModel.OtherRequirements != null)
                        {
                            foreach (var item in requestModel.OtherRequirements.Where(x => x.ID == 0))
                            {
                                var addRow = new LctOtherRequirements
                                {
                                    RegistrationTicketID = requestModel.ID,
                                    Location = item.Location,
                                    SafetyMeasures = item.SafetyMeasures,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };
                                _unitOfWork.ElogbookRepository<LctOtherRequirements>().Add(addRow);
                            }
                        }

                        #endregion
                        break;

                    case (int)WorkPermitTypeLCT.COTHUYNHIETHOA:
                        #region Người tham gia công việc
                        var contentPositions = _unitOfWork.ElogbookRepository<LctContentPosition>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();
                        UpdateContentPositions(contentPositions, requestModel);
                        #endregion

                        #region Nhận diện mối nguy và biện pháp an toàn

                        var entityIdentificationAndSafetys = _unitOfWork.ElogbookRepository<LctIdentificationAndSafetyMeasures>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == requestModel.ID).ToList();
                        foreach (var updateRow in entityIdentificationAndSafetys)
                        {
                            var existingRow = requestModel.IdentificationAndSafetyMeasuresModels.Find(x => x.ID == updateRow.ID);
                            if (existingRow != null)
                            {
                                updateRow.Identification = existingRow.Identification;
                                updateRow.SafetyMeasures = existingRow.SafetyMeasures;
                                _unitOfWork.ElogbookRepository<LctIdentificationAndSafetyMeasures>().Update(updateRow);
                            }
                            else
                            {
                                _unitOfWork.ElogbookRepository<LctIdentificationAndSafetyMeasures>().Delete(updateRow, requestModel.CurrentUserID);
                            }
                        }

                        foreach (var item in requestModel.IdentificationAndSafetyMeasuresModels.Where(x => x.ID == 0))
                        {
                            var addRow = new LctIdentificationAndSafetyMeasures
                            {
                                RegistrationTicketID = requestModel.ID,
                                Identification = item.Identification,
                                SafetyMeasures = item.SafetyMeasures,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            };
                            _unitOfWork.ElogbookRepository<LctIdentificationAndSafetyMeasures>().Add(addRow);
                        }

                        #endregion
                        break;
                }

                // Tracking trạng thái thay đổi lệnh công tác
                LctTicketStatusHistory ticketStatusHistory = null;
                if (requestModel.IsSave)
                {
                    if (oldStatusValue != (int)WorkPermitStatus.Unsent)
                    {
                        ticketStatusHistory = AddTicketStatusHistory(entity.ID, requestModel.CurrentUserID, entity.UpdatedAt, (int)WorkPermitStatus.Unsent, "Lưu chỉnh sửa lệnh đăng ký", "");
                        _unitOfWork.ElogbookRepository<LctTicketStatusHistory>().Add(ticketStatusHistory);
                    }
                }
                else
                {
                    ticketStatusHistory = AddTicketStatusHistory(entity.ID, requestModel.CurrentUserID, entity.UpdatedAt, (int)WorkPermitStatus.Waiting, "Gửi lệnh đăng ký", "");
                    _unitOfWork.ElogbookRepository<LctTicketStatusHistory>().Add(ticketStatusHistory);
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
                            var ticketType = "DKLCTD";
                            if (requestModel.RegistrationTicketType == (int)WorkPermitTypeLCT.COTHUYNHIETHOA)
                            {
                                ticketType = "DKLTCNH";
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

                    return true;
                }
            }

            return false;
        }

        public bool DeleteRegistrationTicket(int id, int currentUserID)
        {
            var entity = _unitOfWork.ElogbookRepository<LctRegistrationTicket>().GetById(id);

            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<LctStakeholders>().Delete(entity.Stakeholders, currentUserID);
                _unitOfWork.ElogbookRepository<LctOtherRequirements>().Delete(entity.OtherRequirements, currentUserID);
                _unitOfWork.ElogbookRepository<LctTicketStatusHistory>().Delete(entity.TicketStatusHistories, currentUserID);
                _unitOfWork.ElogbookRepository<LctRegistrationTicket>().Delete(entity, currentUserID);
                _unitOfWork.ElogbookRepository<LctContentPosition>().Delete(entity.ContentPositions, currentUserID);
                foreach (var item in entity.ContentPositions)
                {
                    _unitOfWork.ElogbookRepository<LctStakeholders>().Delete(item.Stakeholders, currentUserID);
                }

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        private List<LctContentPositionModel> GetContentPositions(int registrationTicketID)
        {
            List<LctContentPositionModel> data = new List<LctContentPositionModel>();
            var entities = _unitOfWork.ElogbookRepository<LctContentPosition>().GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == registrationTicketID).ToList();

            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    List<LctStakeholdersModel> stakeholdersModels = new List<LctStakeholdersModel>();
                    foreach (var stakeholder in item.Stakeholders)
                    {
                        stakeholdersModels.Add(new LctStakeholdersModel
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

                    data.Add(new LctContentPositionModel
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
        public List<LctStakeholdersResponseModel> GetStakeholders(int registrationTicketID, RolesStakeholder rolesStakeholder)
        {
            return _unitOfWork.ElogbookRepository<LctStakeholders>().GetQuery(x => !x.IsDeleted && x.Roles == (int)rolesStakeholder &&
                                                                            x.RegistrationTicketID == registrationTicketID)
                .Select(t => new LctStakeholdersResponseModel()
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
        /// Cập nhật dữ liệu người liên quan đến lệnh công tác
        /// </summary>
        /// <param name="stakeholders"></param>
        /// <param name="requestModel"></param>
        /// <param name="position"></param>
        private void UpdateStakeholders(List<LctStakeholders> stakeholders, LctAddEditRegistrationTicketModel requestModel, RolesStakeholder roles)
        {
            var isExistLeader = false;
            var isExistGuard = false;

            #region Cập nhật/xóa dữ liệu

            foreach (var updateRow in stakeholders)
            {
                LctStakeholdersModel existingRow = null;
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
                    Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(existingRow.UserName, existingRow.IsExternal);

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
                        {
                            assignToUserID = userInfo.ID;
                        }

                        // Người thực hiện công việc
                        if (roles == RolesStakeholder.Worker)
                        {
                            if (userInfo != null)
                            {
                                updateRow.ElectricRankCard = userInfo.ElectricRankCard;
                            }
                            updateRow.Mission = existingRow.Mission;
                        }
                        // Người giám sát/chỉ huy
                        else
                        {
                            if (userInfo != null)
                            {
                                updateRow.ElectricRank = userInfo.ElectricRankStr;
                            }
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

                        _unitOfWork.ElogbookRepository<LctStakeholders>().Update(updateRow);
                    }
                }
                else
                {
                    _unitOfWork.ElogbookRepository<LctStakeholders>().Delete(updateRow, requestModel.CurrentUserID);
                }
            }

            #endregion

            #region Thêm mới dữ liệu

            // Danh sách dữ liệu thêm mới
            List<LctStakeholdersModel> newStakeholders = new List<LctStakeholdersModel>();
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
                Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(item.UserName, item.IsExternal);
                if (userInfo != null)
                {
                    int? assignToUserID = null;
                    if (!item.IsExternal)
                        assignToUserID = userInfo.ID;

                    var rowAdd = new LctStakeholders
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

                    _unitOfWork.ElogbookRepository<LctStakeholders>().Add(rowAdd);
                }
            }

            #endregion
        }

        private void UpdateContentPositions(List<LctContentPosition> contentPositions, LctAddEditRegistrationTicketModel requestModel)
        {
            foreach (var updateRow in contentPositions)
            {
                _unitOfWork.ElogbookRepository<LctStakeholders>().Delete(updateRow.Stakeholders, requestModel.CurrentUserID);
                _unitOfWork.ElogbookRepository<LctContentPosition>().Delete(updateRow, requestModel.CurrentUserID);
            }

            foreach (var newItem in requestModel.ContentPositions)
            {
                var data = new LctContentPosition
                {
                    Position = newItem.Position,
                    RegistrationTicketID = requestModel.ID,
                };

                if (newItem.Stakeholders != null && newItem.Stakeholders.Count > 0)
                {
                    List<LctStakeholders> stakeholders = new List<LctStakeholders>();
                    foreach (var worker in newItem.Stakeholders)
                    {
                        Core.Models.Response.UserInfo userInfo = _commonService.GetUserById(worker.UserName, worker.IsExternal);
                        if (userInfo != null)
                        {
                            int? assignToUserID = null;
                            if (!worker.IsExternal)
                            {
                                assignToUserID = userInfo.ID;
                            }

                            stakeholders.Add(new LctStakeholders
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
                _unitOfWork.ElogbookRepository<LctContentPosition>().Add(data);
            }
        }

        private LctTicketStatusHistory AddTicketStatusHistory(int registrationTicketID, int userID, DateTime insertedAt, int ticketStatus, string description, string lstAttachment)
        {
            var ticketStatusHistory = new LctTicketStatusHistory()
            {
                InsertedAt = insertedAt,
                StatusValue = ticketStatus,
                Description = description,
                Attachments = lstAttachment,
                CreatedUserID = userID,
                LastUpdatedUserID = userID,
            };

            if (ticketStatus == (int)WorkPermitStatus.Reject)
            {
                ticketStatusHistory.Description = "Từ chối lệnh đăng ký.";
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
            var entity = _unitOfWork.ElogbookRepository<LctTicketStatusHistory>().GetQuery(x => x.RegistrationTicketID == id && !x.IsDeleted).ToList();
            List<LctTicketStatusHistoryModel> ticketStatusHistoryModels = new List<LctTicketStatusHistoryModel>();

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

                LctTicketStatusHistoryModel ticketStatus = new LctTicketStatusHistoryModel
                {
                    ID = item.ID,
                    Description = item.Description,
                    InsertedAt = item.InsertedAt,
                    FileAttachs = new List<Core.Models.Response.AttachmentModel>(),
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
                            ticketStatus.FileAttachs.Add(new Core.Models.Response.AttachmentModel
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
    }
}
