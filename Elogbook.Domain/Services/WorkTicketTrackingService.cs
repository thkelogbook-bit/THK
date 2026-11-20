using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;                 
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class WorkTicketTrackingService : IWorkTicketTrackingService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public WorkTicketTrackingService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Public method

        public bool ApproveWorkTicketTracking(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<WorkTicketTracking>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                var entityLeadershipUser = _unitOfWork.ElogbookRepository<LeadershipUser>()
                    .GetQuery(x => x.WorkTicketID == entity.WorkTicketID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted).FirstOrDefault(); 

                if (entityLeadershipUser != null)
                {
                    var acceptorUser = GetUserInfo((int)entity.NewAllowerUserID, (int)LeadershipUserRoles.Acceptor);

                    if (acceptorUser != null)
                    {
                        // cập nhập người cho phép mới vào table LeadershipUser
                        entityLeadershipUser.WorkTicketID = entity.WorkTicketID;
                        entityLeadershipUser.UserName = !string.IsNullOrWhiteSpace(acceptorUser.UserName) ? acceptorUser.UserName : null;
                        entityLeadershipUser.FullName = !string.IsNullOrWhiteSpace(acceptorUser.FullName) ? acceptorUser.FullName : null;
                        entityLeadershipUser.PositionName = !string.IsNullOrWhiteSpace(acceptorUser.PositionName) ? acceptorUser.PositionName : null;
                        entityLeadershipUser.IsExternal = acceptorUser.IsExternal;
                        entityLeadershipUser.Roles = acceptorUser.Roles > 0 ? acceptorUser.Roles : null;
                        entityLeadershipUser.ElectricRank = acceptorUser.ElectricRank;
                        entityLeadershipUser.ElectricRankCard = !string.IsNullOrWhiteSpace(acceptorUser.ElectricRankCard) ? acceptorUser.ElectricRankCard : null;
                        entityLeadershipUser.LaborCard = acceptorUser.LaborCard;
                        entityLeadershipUser.LaborRank = acceptorUser.LaborRank;
                        entityLeadershipUser.Signature = !string.IsNullOrWhiteSpace(acceptorUser.Signature) ? acceptorUser.Signature : null;
                        entityLeadershipUser.LastUpdatedUserID = userID;
                        entityLeadershipUser.UpdatedAt = DateTime.Now;

                        _unitOfWork.ElogbookRepository<LeadershipUser>().Update(entityLeadershipUser);

                        // cập nhập thông tin vào table WorkTicketTracking
                        entity.RequesterUserID = entity.CurrentAllowerUserID;
                        entity.CurrentAllowerUserID = (int)entity.NewAllowerUserID;
                        entity.Reason = "Đã đồng ý thay đổi người cho phép";
                        entity.TrackingStatusID = (int)NCPStatus.APPROVED;
                        entity.UpdatedAt = DateTime.Now;
                        entity.LastUpdatedUserID = userID;

                        _unitOfWork.ElogbookRepository<WorkTicketTracking>().Update(entity);

                        // cập nhập thông tin vào table WorkTicket
                        var dataWorkTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == entity.WorkTicketID && !x.IsDeleted).FirstOrDefault();
                        if (dataWorkTicket != null)
                        {
                            dataWorkTicket.TrackingStatusID = (int)NCPStatus.APPROVED;
                            dataWorkTicket.LastUpdatedUserID = userID;
                            dataWorkTicket.UpdatedAt = DateTime.Now;
                            _unitOfWork.ElogbookRepository<WorkTicket>().Update(dataWorkTicket);

                            return _unitOfWork.Complete() > 0;
                        }
                    }
                }
            }

            return false;
        }

        public bool RejectWorkTicketTracking(int id, int userID, string reason)
        {
            var entity = _unitOfWork.ElogbookRepository<WorkTicketTracking>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.UpdatedAt = DateTime.Now;
                entity.LastUpdatedUserID = userID;
                entity.Reason = reason;
                entity.TrackingStatusID = (int)NCPStatus.REJECT;

                _unitOfWork.ElogbookRepository<WorkTicketTracking>().Update(entity);

                var dataWorkTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == entity.WorkTicketID && !x.IsDeleted).FirstOrDefault();
                if (dataWorkTicket !=null)
                {
                    dataWorkTicket.TrackingStatusID = (int)NCPStatus.REJECT;
                    dataWorkTicket.LastUpdatedUserID = userID;
                    dataWorkTicket.UpdatedAt = DateTime.Now;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(dataWorkTicket);

                    return _unitOfWork.Complete() > 0;
                }
            }

            return false;
        }

        public bool CreateWorkTicketTracking(AddWorkTicketTrackingModel requestModel)
        {
            var dataWorkTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();

            if (dataWorkTicket !=null)
            {
                var entityLeadershipUser = _unitOfWork.ElogbookRepository<LeadershipUser>()
                                            .GetQuery(x => x.WorkTicketID == requestModel.WorkTicketID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted).FirstOrDefault();

                if (entityLeadershipUser != null)
                {
                    var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted
                                                                       && x.UserName.Equals(entityLeadershipUser.UserName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (userInfo != null)
                    {
                        // Thay đổi trạng thái những yêu cầu trước đó chưa được duyệt thành từ chôi
                        var entityOld = _unitOfWork.ElogbookRepository<WorkTicketTracking>().GetQuery(x => x.WorkTicketID == requestModel.WorkTicketID 
                                                                                                            && !x.IsDeleted 
                                                                                                            && x.TrackingStatusID == (int)NCPStatus.WAITING).FirstOrDefault();
                        if (entityOld != null)
                        {
                            entityOld.UpdatedAt = DateTime.Now;
                            entityOld.LastUpdatedUserID = requestModel.CurrentUserID;
                            entityOld.Reason = "Tự động từ chối do có yêu cầu thay đổi người cho phép mới";
                            entityOld.TrackingStatusID = (int)NCPStatus.REJECT;

                            _unitOfWork.ElogbookRepository<WorkTicketTracking>().Update(entityOld);
                        }

                        // Thêm mới yêu cầu thay đổi NCP -> lưu vào table WorkTicketTracking
                        var entityNew = new WorkTicketTracking
                        {
                            WorkTicketID = requestModel.WorkTicketID,
                            RequesterUserID = userInfo.ID,
                            CurrentAllowerUserID = userInfo.ID,
                            NewAllowerUserID = requestModel.NewAllowerUserID,
                            Content = requestModel.Reason,
                            TrackingStatusID = (int)NCPStatus.WAITING,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                        };
                        _unitOfWork.ElogbookRepository<WorkTicketTracking>().Add(entityNew);

                        // Cập nhật trạng thái thay đổi NCP cho Phiếu công tác
                        dataWorkTicket.TrackingStatusID = (int)NCPStatus.WAITING;
                        dataWorkTicket.LastUpdatedUserID = requestModel.CurrentUserID;
                        dataWorkTicket.UpdatedAt = DateTime.Now;

                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(dataWorkTicket);

                        return _unitOfWork.Complete() > 0;
                    }
                }
            }
            
            return false;
        }

        public bool DeleteWorkTicketTracking(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<WorkTicketTracking>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<WorkTicketTracking>().Delete(entity, userID);
                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public WorkTicketTrackingModel GetWorkTicketTrackingById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<WorkTicketTracking>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
               e => new WorkTicketTrackingModel()
               {
                   ID = e.ID,
                   WorkTicket = new ValueLabelModel { Value = e.WorkTicket.ID, Label = e.WorkTicket.WorkTicketNumber },
                   NewAllowerUser = new ValueLabelModel { Value = e.NewAllowerUser.ID, Label = e.NewAllowerUser.FullName },
               }).FirstOrDefault();

            return entity;
        }

        public ResponseBaseModel GetAllWorkTickets(SearchWorkTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var workTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.WaitForRegist && x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.Waiting && !x.IsDeleted);

            // Trạng thái thay đổi người cho phép
            if (requestModel.TrackingStatusID > 0)
            {
                workTickets = workTickets.Where(x => x.TrackingStatusID == requestModel.TrackingStatusID);
            }
            else {
                workTickets = workTickets.Where(x => x.TrackingStatusID != null);
            }

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

            var page = workTickets.Select(e => new WorkTicketModel()
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
                Approver = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Approver && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                Acceptor = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                InsertedAt = e.InsertedAt,
                IsFlagNote = e.IsFlagNote,
                TrackingStatus = e.TrackingStatus != null ? new ValueLabelModel
                {
                    Value = e.TrackingStatus.ID,
                    Label = e.TrackingStatus.Status
                } : new ValueLabelModel { Value = 0, Label = null },
            }).OrderBy(t => t.WorkTicketStatus.Label)
                        .ThenByDescending(t => t.ApprovedDateStyleDatetime)
                        .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            // Kiểm tra quyền edit
            if (page != null && page.TotalRecords > 0)
            {
                foreach (var entity in page)
                {
                    var workTicketFeatureLinks = _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>().GetQuery(x => !x.IsDeleted
                                                                && x.WorkTicketID == entity.ID).ToList();

                    if (workTicketFeatureLinks.Count > 0)
                    {
                        entity.WorkTicketFeatures = workTicketFeatureLinks.Select(es => new WorkTicketFeaturesModel()
                        {
                            FeatureID = es.FeatureID,
                            Label = es.WorkTicketFeatures.FeatureName,
                        }).ToList();
                    }

                }
                page.GetEnumerator().Reset();
            }

            return HttpResponse.OkList(page);
        }

        public WorkTicketDetailModel GetDetailWorkTicket(int id)
        {
            var data = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == id && !x.IsDeleted);
            var entity = data.Select(e => new WorkTicketDetailModel()
            {
                ScreenType = e.ScreenType,
                RegistrationTicket = new ValueLabelModel { Value = e.RegistrationTicket.ID, Label = e.RegistrationTicket.TicketNumber },
                Factory = e.FactoryID != null ? new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName } : new ValueLabelModel { Value = 0, Label = " " },
                ID = e.ID,
                WorkTicketNumber = e.WorkTicketNumber,
                ApprovedUnit = e.ApprovedUnitID != null ? new ValueLabelModel { Value = e.ApprovedUnit.ID, Label = e.ApprovedUnit.Name } : new ValueLabelModel { Value = 0, Label = " " },
                DTDApprovedDate = e.ApprovedDate,
                SignDate = e.ApprovedDate,
                WorkScope = e.WorkScope,
                WorkTicketFeature = e.WorkTicketFeature != null ? new ValueLabelModel { Value = e.WorkTicketFeature.ID, Label = e.WorkTicketFeature.FeatureName } : new ValueLabelModel { Value = 0, Label = null },
                WorkTicketTypeValue = e.WorkTicketType,
                WorkTicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                UnitNumber = e.UnitNumber,
                WorkPlacement = e.WorkPlacement,
                WorkContent = e.WorkContent,
                DTStartDate = e.StartDate,
                EstimatedStartTime = e.StartDate,
                DTEndDate = e.EndDate,
                EstimatedEndTime = e.EndDate,
                WorkCondition = e.WorkCondition,
                AttachmentsID = e.Attachments,
                WorkResult = e.WorkResult,
                WorkPicturesID = e.WorkPictures,
                IsFlagNote = e.IsFlagNote,
                WarnAndSafes = e.WarnAndSafes.Select(t => new WarnAndSafeModel()
                {
                    SafetyMeasureContent = t.SafetyMeasureContent,
                    WarningContent = t.WarningContent,
                }).ToList(),
                WorkTicketFeatures = e.WorkTicketFeatureLinks.Select(t => new WorkTicketFeaturesModel()
                {
                    Label = t.WorkTicketFeatures.FeatureName,
                    FeatureID = t.FeatureID,
                }).ToList(),
                TicketPdfID = e.TicketPdfID,
                WorkTicketTrackings = e.WorkTicketTrackings.Select(t => new WorkTicketTrackingModel()
                {
                    ID = t.ID,
                    Content = t.Content,
                    Reason = t.Reason,
                    RequesterUser = new ValueLabelModel { Value = t.RequesterUser.ID, Label = t.RequesterUser.FullName },
                    CurrentAllowerUser = new ValueLabelModel { Value = t.CurrentAllowerUser.ID, Label = t.CurrentAllowerUser.FullName },
                    NewAllowerUser = new ValueLabelModel { Value = t.NewAllowerUser.ID, Label = t.NewAllowerUser.FullName },
                    CreatedUser = new ValueLabelModel { Value = t.CreatedUser.ID, Label = t.CreatedUser.FullName },
                    Status = new ValueLabelModel { Value = t.TrackingStatus.ID, Label = t.TrackingStatus.Status },
                    WorkTicket = new ValueLabelModel { Value = t.WorkTicket.ID, Label = t.WorkTicket.WorkTicketNumber },
                    CreateDate = t.InsertedAt,
                }).OrderBy(z => z.CreateDate).ToList(),
            }).FirstOrDefault();

            if (entity != null && entity.ID > 0)
            {
                var approverUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Approver);
                var leaderUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Leader);
                var commanderUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Commander);
                var representativeUnitUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.RepresentativeUnit);
                var acceptorUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Acceptor);
                var commanderLastUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Commander);

                entity.ApproverUser = approverUserEntity;
                if (approverUserEntity != null)
                    entity.ApproverSignature = approverUserEntity.Signature;
                entity.LeaderUser = leaderUserEntity;
                entity.CommanderUser = commanderUserEntity;
                entity.CommanderLastUser = commanderLastUserEntity;
                entity.AcceptorUser = acceptorUserEntity;
                entity.RepresentativeUnitUser = representativeUnitUserEntity;
            }

            return entity;
        }

        public List<ValueLabelModel> GetTrackingStatus()
        {
            return _unitOfWork.ElogbookRepository<TrackingStatus>().GetQuery(x => !x.IsDeleted)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.Status
                                }).OrderBy(t => t.Value).ToList();
        }

        #endregion

        #region Private Method

        private AddNewLeadershipUserFetchedModel GetUserInfo(int id, int Roles)
        {
            return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new AddNewLeadershipUserFetchedModel()
                {
                    FullName = e.FullName,
                    UserName = e.UserName,
                    ElectricRank = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    LaborRank = e.LaborRank,
                    LaborCard = e.LaborCard,
                    Roles = Roles,
                    Signature = e.Signature,
                    IsExternal = false,
                    UserID = id,
                    PositionName = e.PositionName,

                }).FirstOrDefault();
        }

        private LeadershipUserModel GetLeadershipById(int workTicketID, int roles)
        {
            var entity = _unitOfWork.ElogbookRepository<LeadershipUser>()
                            .GetQuery(x => x.WorkTicketID == workTicketID && x.Roles == roles && !x.IsDeleted)
                            .Select(e => new LeadershipUserModel()
                            {
                                ID = e.ID,
                                WorkTicketID = e.WorkTicketID,
                                UserName = e.UserName,
                                IsExternal = e.IsExternal ?? false,
                                FullName = e.FullName,
                                Roles = e.Roles,
                                ElectricRankStr = e.ElectricRank,
                                ElectricRankCard = e.ElectricRankCard,
                                LaborRankStr = e.LaborRank,
                                LaborCard = e.LaborCard,
                                Signature = e.Signature,
                                PositionName = e.PositionName,
                            }).FirstOrDefault();

            return entity;
        }

        #endregion
    }
}
