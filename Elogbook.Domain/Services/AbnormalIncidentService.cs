using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseCommon;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;

namespace Elogbook.Domain.Services
{
    public class AbnormalIncidentService : IAbnormalIncidentService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        #endregion

        #region Contructor

        public AbnormalIncidentService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        #endregion

        #region Public methods

        public ResponseBaseModel Find(SearchAbnormalIncidentModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            var entities = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => // Chỉ lấy thông tin tồn tại trong cả của những NKVH đã hoàn tất việc giao/nhận ca
                                                                                        x.OperationDiary.IsDelivered
                                                                                        && !x.IsDeleted
                                                                                        && !x.IsDeleteAbnormalIncident);
            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.OperationDiary.FactoryID == requestModel.FactoryID);
            }
            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                entities = entities.Where(x => x.OperationDiary.OperationPositionID == requestModel.OperationPositionID);
            }
            // Trạng thái
            if (requestModel.StatusID >= 0)
            {
                entities = entities.Where(x => x.StatusID == requestModel.StatusID);
            }
            // Từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => (x.Description.Contains(requestModel.SearchText)
                                                || x.CreatedUser.FullName.Contains(requestModel.SearchText)
                                                || x.AbnormalIncidentDetails.Any(c => c.CreatedUser.FullName.Contains(requestModel.SearchText))));
            }
            // Thời gian giao ca (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.OperationDiary.TimeDelivery) >= frDate.Date);
            }
            // Thời gian giao ca (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.OperationDiary.TimeDelivery) <= toDate.Date);
            }

            requestModel.SortColumn = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortColumn.ToUpper();
            requestModel.SortOrder = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortOrder.ToUpper();
            if ("INSERTEDAT".Equals(requestModel.SortColumn))
            {
                if ("DESC".Equals(requestModel.SortOrder))
                {
                    entities = entities.OrderByDescending(x => x.InsertedAt).ThenBy(x => x.StatusID);
                }
                else
                {
                    entities = entities.OrderBy(x => x.InsertedAt).ThenBy(x => x.StatusID);
                }
            }
            else
            {
                entities = entities.OrderBy(x => x.StatusID).ThenByDescending(x => x.InsertedAt);
            }

            var page = entities.Select(
                e => new AbnormalIncidentModel()
                {
                    ID = e.ID,
                    InsertedAt = e.InsertedAt,
                    Description = e.Description,
                    TeamLeaderUser = new ValueLabelModel { Value = 1, Label = e.TeamLeader },
                    CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Status = e.StatusID,
                    OperationDiaryID = e.OperationDiaryID,
                    RegistrationTicket = new TicketInforModel { Value = e.RegistrationTicket.ID, Label = e.RegistrationTicket != null ? e.RegistrationTicket.WorkTicketNumber : e.RegistrationTicketName },
                    TeamLeader = e.TeamLeader,
                    Area = e.Area,
                    // Tài liệu liên quan (nếu có)
                    Attachments = e.Attachments,
                    FactoryID = e.OperationDiary.FactoryID.Value,
                    IsEditable = e.StatusID != 2,
                    SyncDataKey = e.SyncDataKey,
                    AppID = e.AppID,
                    //file đính kèm tình hình vận hành
                    AbnormalIncidentDetails = e.AbnormalIncidentDetails.Where(x => !x.IsDeleted).Select(
                                            t => new AbnormalIncidentDetailModel()
                                            {
                                                ID = t.ID,
                                                Content = t.Content,
                                                CreateDate = t.InsertedAt,
                                                CreatedUser = new ValueLabelModel
                                                {
                                                    Value = t.CreatedUser.ID,
                                                    Label = t.CreatedUser.FullName
                                                }
                                            }).OrderBy(t => t.CreateDate).ToList(),
                });
            IPagedList<AbnormalIncidentModel> pagelist = page.ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            // Kiểm tra quyền edit
            if (pagelist != null && pagelist.TotalRecords > 0)
            {
                foreach (var entity in pagelist)
                {
                    entity.IsEditable = CheckEditIssueOfShift(entity.ID, entity.FactoryID);
                }
                pagelist.GetEnumerator().Reset();
            }

            return HttpResponse.OkList(pagelist);
        }

        public ResponseBaseModel GetAbnormalIncidentById(int id, int userID)
        {
            var data = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new AbnormalIncidentModel()
                {
                    ID = e.ID,
                    InsertedAt = e.InsertedAt,
                    Description = e.Description,
                    TeamLeaderUser = new ValueLabelModel { Value = 1, Label = e.TeamLeader },
                    CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Status = e.StatusID,
                    OperationDiaryID = e.OperationDiaryID,
                    RegistrationTicket = new TicketInforModel { Value = e.RegistrationTicket.ID, Label = e.RegistrationTicket != null ? e.RegistrationTicket.WorkTicketNumber : e.RegistrationTicketName },
                    TeamLeader = e.TeamLeader,
                    FactoryID = e.OperationDiary.FactoryID.Value,
                    Area = e.Area,
                    SyncDataKey = e.SyncDataKey,
                    AppID = e.AppID,
                    // Tài liệu liên quan (nếu có)
                    Attachments = e.Attachments,
                    AbnormalIncidentDetails = e.AbnormalIncidentDetails.Where(x => !x.IsDeleted).Select(
                                            t => new AbnormalIncidentDetailModel()
                                            {
                                                ID = t.ID,
                                                Content = t.Content,
                                                CreateDate = t.InsertedAt,
                                                CreatedUser = new ValueLabelModel
                                                {
                                                    Value = t.CreatedUser.ID,
                                                    Label = t.CreatedUser.FullName
                                                },
                                                Attachments = t.Attachments
                                            }).OrderBy(t => t.CreateDate).ToList(),
                }).FirstOrDefault();
            // file đính kèm tồn tại trong ca
            if (data != null)
            {
                data.FileAttachments = new List<AttachmentModel>();
                var attachments = data?.Attachments?.Split(',');
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        int idItem = 0;
                        if (int.TryParse(attachment, out idItem))
                        {
                            var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted &&
                                                                            x.ID == idItem).FirstOrDefault();
                            if (att != null)
                            {
                                data.FileAttachments.Add(new AttachmentModel
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
                data.IsEditable = CheckEditIssueOfShift(data.ID, data.FactoryID);
            }
            // file đính kèm phần xử lý tồn tại trong ca
            if (data != null && data.AbnormalIncidentDetails != null)
            {
                foreach (var item in data.AbnormalIncidentDetails)
                {
                    item.FileAttachments = new List<AttachmentModel>();
                    var attachments = item?.Attachments?.Split(',');
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            int idItem = 0;
                            if (int.TryParse(attachment, out idItem))
                            {
                                var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted &&
                                                                                x.ID == idItem).FirstOrDefault();
                                if (att != null)
                                {
                                    item.FileAttachments.Add(new AttachmentModel
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
                    item.IsEditable = CheckEditIssueOfShiftDetail(item.ID, false, userID);

                }
            }
            return data != null ? HttpResponse.OK(data) : HttpResponse.Error("Không tìm thấy");
        }

        public ResponseBaseModel AddAbnormalIncidentDetail(AbnormalIncidentDetailAdd requestModel)
        {
            // Tạo xử lý tồn tại
            var entity = new AbnormalIncidentDetail()
            {
                AbnormalIncidentID = requestModel.AbnormalIncidentID,
                Content = requestModel.Content,
                Attachments = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments) : string.Empty,
                CreatedUserID = requestModel.CreatedUserID,
                LastUpdatedUserID = requestModel.CreatedUserID,
            };

            _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                UpdateStatusIssueOfShift(requestModel.AbnormalIncidentID, requestModel.CreatedUserID, requestModel.IsComplete);

                return HttpResponse.OK(GetAbnormalIncidentById(requestModel.AbnormalIncidentID, requestModel.CreatedUserID));
            }
            else
            {
                return HttpResponse.Error("Tạo không thành công");
            }
        }

        public ResponseBaseModel UpdateAbnormalIncidentDetail(AbnormalIncidentDetailUpdate requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().GetById(requestModel.ID);
            if (entity != null)
            {
                if (CheckEditIssueOfShiftDetail(entity.ID, false, requestModel.UpdatedUserID))
                {

                    if (requestModel.UpdatedUserID != entity.CreatedUserID)
                    {
                        return HttpResponse.Error("Người dùng không thể cập nhật nội dung xử lý.");
                    }
                    else
                    {
                        entity.Content = requestModel.Content;
                        entity.LastUpdatedUserID = requestModel.UpdatedUserID;
                        entity.IsComplete = requestModel.IsComplete ? 1 : 0;
                        entity.Attachments = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments) : string.Empty;
                        _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().Update(entity);
                        _unitOfWork.Complete();
                        // Cập nhật trạng thái tồn tại
                        UpdateStatusIssueOfShift(requestModel.AbnormalIncidentID, requestModel.UpdatedUserID, requestModel.IsComplete);

                        return HttpResponse.OK(GetAbnormalIncidentById(requestModel.AbnormalIncidentID, requestModel.UpdatedUserID));
                    }
                }
                else
                {
                    return HttpResponse.Error("Không thể cập nhật nội dung xử lý nếu bạn không có ca làm việc hiện tại.");
                }
            }
            return HttpResponse.Error("ID không tìm thấy.");
        }

        public ResponseBaseModel DeleteAbnormalIncident(int id, int userID)
        {
            var entityExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetById(id);
            if (entityExistInShift != null)
            {
                entityExistInShift.IsDeleteIssueOfShift = true;
                entityExistInShift.LastUpdatedUserID = userID;
                entityExistInShift.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<ExistInShift>().Update(entityExistInShift);

                var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetById(entityExistInShift.OperationDiaryID);
                if (operationDiary != null)
                {
                    operationDiary.LastUpdatedUserID = userID;
                    operationDiary.SyncedDate = DateTime.Now;
                    _unitOfWork.ElogbookRepository<OperationDiary>().Update(operationDiary);
                }

                _unitOfWork.Complete();
                return HttpResponse.OK("OK");
            }

            return HttpResponse.Error("ID không tìm thấy.");
        }

        public ResponseBaseModel DeteleAbnormalIncidentDetail(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().GetById(id);
            if (entity != null)
            {
                if (CheckEditIssueOfShiftDetail(entity.ID, false, userID))
                {
                    _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().Delete(entity, userID);

                    var entityExistInShift = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetById(entity.AbnormalIncidentID.GetValueOrDefault(0));
                    if (entityExistInShift != null)
                    {
                        entityExistInShift.LastUpdatedUserID = userID;
                        entityExistInShift.SyncedDate = DateTime.Now;
                        _unitOfWork.ElogbookRepository<AbnormalIncident>().Update(entityExistInShift);
                        var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetById(entityExistInShift.OperationDiaryID);
                        if (operationDiary != null)
                        {
                            operationDiary.LastUpdatedUserID = userID;
                            operationDiary.SyncedDate = DateTime.Now;
                            _unitOfWork.ElogbookRepository<OperationDiary>().Update(operationDiary);
                        }
                    }

                    _unitOfWork.Complete();
                    return HttpResponse.OK("OK");
                }
                else
                {
                    return HttpResponse.Error("Không thể xóa nội dung xử lý.");
                }
            }
            return HttpResponse.Error("ID không tìm thấy.");
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel SyncDataAbnormalIncidentDetail(AbnormalIncidentDetailSyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            AbnormalIncidentDetailSyncDataResultModel result = new AbnormalIncidentDetailSyncDataResultModel();
            List<int> lstAbnormalIncident = new List<int>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    LogCustom.LogMessage(TrackingLogLevel.INFO, "AbnormalIncidentService", Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel));

                    // Bất thường sự cố
                    if (requestModel.AbnormalIncidentSync != null && requestModel.AbnormalIncidentSync.Count > 0)
                    {
                        foreach (var eis in requestModel.AbnormalIncidentSync)
                        {
                            // Cập nhật bất thường sự cố
                            var abnormalIncidentUpdate = _unitOfWork.ElogbookRepository<AbnormalIncident>()
                                    .GetQuery(x => x.ID == eis.ID && x.SyncDataKey == eis.SyncDataKey && !x.IsDeleted).FirstOrDefault();

                            if (abnormalIncidentUpdate != null)
                            {
                                double diffTimeUpdate = (eis.UpdatedAt - abnormalIncidentUpdate.UpdatedAt).TotalMilliseconds;
                                //Chỉ cập nhất nếu thời gian thay đổi lớn hơn thời gian đã cập nhật trước đó
                                if (diffTimeUpdate > 0)
                                {
                                    abnormalIncidentUpdate.StatusID = eis.StatusID;
                                    abnormalIncidentUpdate.AppID = eis.AppID;
                                    if (eis.CompletedDateTime.Year >= 1753)
                                    {
                                        abnormalIncidentUpdate.CompletedDateTime = eis.CompletedDateTime;
                                    }
                                    if (eis.LastUpdatedUserID != null && eis.LastUpdatedUserID > 0)
                                    {
                                        abnormalIncidentUpdate.LastUpdatedUserID = eis.LastUpdatedUserID;
                                    }
                                    abnormalIncidentUpdate.UpdatedAt = eis.UpdatedAt;
                                    abnormalIncidentUpdate.IsDeleted = eis.IsDeleted;
                                    abnormalIncidentUpdate.Description = eis.Description;
                                    abnormalIncidentUpdate.Attachments = eis.Attachments;
                                    abnormalIncidentUpdate.Area = eis.Area;

                                    abnormalIncidentUpdate.SyncedDate = DateTime.Now;
                                    _unitOfWork.ElogbookRepository<AbnormalIncident>().Update(abnormalIncidentUpdate);
                                    _unitOfWork.Complete();
                                    lstAbnormalIncident.Add(abnormalIncidentUpdate.ID);
                                }
                            }
                            else
                            {
                                LogCustom.LogMessage(TrackingLogLevel.INFO, "AbnormalIncidentService",
                                    Environment.NewLine + " Ignore this case.");
                            }
                        }
                    }

                    // Chi tiết bất thường sự cố
                    if (requestModel.AbnormalIncidentSyncDetails != null && requestModel.AbnormalIncidentSyncDetails.Count > 0)
                    {
                        foreach (var item in requestModel.AbnormalIncidentSyncDetails)
                        {
                            var abnormalIncidentEntity = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => !x.IsDeleted && x.ID == item.AbnormalIncidentID).FirstOrDefault();
                            if (item.ID == 0)
                            {
                                if (abnormalIncidentEntity != null && abnormalIncidentEntity.ID > 0)
                                {
                                    AbnormalIncidentDetail abnormalIncidentDetail = new AbnormalIncidentDetail()
                                    {
                                        AppID = item.AppID,
                                        Content = item.Content,
                                        IsComplete = item.IsComplete,
                                        Attachments = item.Attachments,
                                        IsDeleted = item.IsDeleted,
                                        InsertedAt = item.InsertedAt,
                                        UpdatedAt = item.UpdatedAt,
                                    };

                                    if (item.CreatedUserID != null && item.CreatedUserID > 0)
                                        abnormalIncidentDetail.CreatedUserID = item.CreatedUserID;
                                    if (item.LastUpdatedUserID != null && item.LastUpdatedUserID > 0)
                                        abnormalIncidentDetail.LastUpdatedUserID = item.LastUpdatedUserID;

                                    abnormalIncidentDetail.AbnormalIncidentID = abnormalIncidentEntity.ID;
                                    _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().Add(abnormalIncidentDetail);
                                    _unitOfWork.Complete();
                                    lstAbnormalIncident.Add(abnormalIncidentEntity.ID);
                                }
                            }
                            else
                            {
                                var abnormalIncidentDetailEntity = _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>()
                                    .GetQuery(x => !x.IsDeleted && x.ID == item.ID && x.AbnormalIncidentID == item.AbnormalIncidentID).FirstOrDefault();

                                // trường hợp đã tồn tại thì cập nhật
                                if (abnormalIncidentDetailEntity != null)
                                {
                                    double diffTimeUpdate = (item.UpdatedAt - abnormalIncidentDetailEntity.UpdatedAt).TotalMilliseconds;
                                    //Chỉ cập nhất nếu thời gian thay đổi lớn hơn thời gian đã cập nhật trước đó
                                    if (diffTimeUpdate > 0)
                                    {
                                        abnormalIncidentDetailEntity.AppID = item.AppID;
                                        abnormalIncidentDetailEntity.Content = item.Content;
                                        abnormalIncidentDetailEntity.AbnormalIncidentID = item.AbnormalIncidentID;
                                        abnormalIncidentDetailEntity.IsComplete = item.IsComplete;
                                        abnormalIncidentDetailEntity.Attachments = item.Attachments;
                                        abnormalIncidentDetailEntity.IsDeleted = item.IsDeleted;
                                        abnormalIncidentDetailEntity.InsertedAt = item.InsertedAt;
                                        abnormalIncidentDetailEntity.UpdatedAt = item.UpdatedAt;
                                        if (item.AbnormalIncidentID != null && item.AbnormalIncidentID > 0) abnormalIncidentDetailEntity.AbnormalIncidentID = item.AbnormalIncidentID;
                                        if (item.CreatedUserID != null && item.CreatedUserID > 0) abnormalIncidentDetailEntity.CreatedUserID = item.CreatedUserID;
                                        if (item.LastUpdatedUserID != null && item.LastUpdatedUserID > 0) abnormalIncidentDetailEntity.LastUpdatedUserID = item.LastUpdatedUserID;

                                        _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().Update(abnormalIncidentDetailEntity);
                                        _unitOfWork.Complete();
                                    }
                                    lstAbnormalIncident.Add(abnormalIncidentEntity.ID);
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    responseModel.Message = "Đồng bộ dữ liệu thành công";
                }
                catch (Exception ex)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Content = false;
                    responseModel.Message = "Đồng bộ dữ liệu thất bại, Vui lòng liên hệ Admin";
                    transaction.Rollback();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "IssueOfShiftDetailService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    return responseModel;
                }
            }
            if (lstAbnormalIncident.Count > 0)
            {
                //1. Tồn tại trong ca
                result.AbnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => lstAbnormalIncident.Contains(x.ID))
                                        .Select(
                                            e => new MobileAbnormalIncidentInfo()
                                            {
                                                AppID = e.AppID,
                                                ID = e.ID,
                                                OperationDiaryID = e.OperationDiaryID,
                                                RegistrationTicketID = e.RegistrationTicketID,
                                                RegistrationTicketName = e.RegistrationTicketName,
                                                TeamLeader = e.TeamLeader,
                                                Area = e.Area,
                                                Description = e.Description,
                                                Attachments = e.Attachments,
                                                StatusID = e.StatusID,
                                                IsDeleteAbnormalIncident = e.IsDeleteAbnormalIncident,
                                                CompletedDateTime = e.CompletedDateTime,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt
                                            }).OrderBy(t => t.ID).ToList();
                // 2.Chi tiết tồn tại trong ca
                result.AbnormalIncidentDetail = _unitOfWork.ElogbookRepository<AbnormalIncidentDetail>().GetQuery(x => lstAbnormalIncident.Contains((int)x.AbnormalIncidentID))
                        .Select(
                            e => new MobileAbnormalIncidentDetailInfo()
                            {
                                AppID = e.AppID,
                                ID = e.ID,
                                Content = e.Content,
                                Attachments = e.Attachments,
                                IsComplete = e.IsComplete,
                                AbnormalIncidentID = e.AbnormalIncidentID,
                                IsDeleted = e.IsDeleted,
                                CreatedUserID = e.CreatedUserID,
                                InsertedAt = e.InsertedAt,
                                LastUpdatedUserID = e.LastUpdatedUserID,
                                UpdatedAt = e.UpdatedAt,
                            }).OrderBy(t => t.ID).ToList();

                responseModel.Content = result;
            }

            return responseModel;
        }

        #endregion

        #region Private methods

        private void UpdateStatusIssueOfShift(int existInShiftID, int updatedUserID, bool isComplete)
        {
            // Cập nhật trạng thái của tồn tại
            var entityExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetById(existInShiftID);
            if (entityExistInShift != null)
            {
                entityExistInShift.StatusID = isComplete ? (int)IssueOfShiftStatus.DONE : (int)IssueOfShiftStatus.IN_PROGRESS;
                entityExistInShift.LastUpdatedUserID = updatedUserID;
                if (isComplete)
                    entityExistInShift.CompletedDateTime = DateTime.Now;

                entityExistInShift.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<ExistInShift>().Update(entityExistInShift);

                var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetById(entityExistInShift.OperationDiaryID);
                if (operationDiary != null)
                {
                    operationDiary.LastUpdatedUserID = updatedUserID;
                    operationDiary.SyncedDate = DateTime.Now;
                    _unitOfWork.ElogbookRepository<OperationDiary>().Update(operationDiary);
                }

                _unitOfWork.Complete();
            }
        }

        private bool CheckEditIssueOfShift(int id, int factoryID)
        {
            var isEditable = false;
            var entity = _unitOfWork.ElogbookRepository<ExistInShift>().GetById(id);
            if (entity != null)
            {
                //Chưa giao nhận ca và đang trong khung giờ trực thì được cập nhật nội dung xử lý
                double time = (double)DateTime.Now.Minute / 60;
                double currentHour = DateTime.Now.Hour + time;

                List<ValueLabelModel> shifts = _commonService.GetOperationShiftsByFactoryID(factoryID, currentHour.ToString("F2"));

                if (shifts.Count > 0)
                {
                    int shiftId = shifts.First().Value.Value;

                    var operationShift = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == factoryID
                                                                                 && x.Active == (int)AccessStatus.Active
                                                                                 && x.ID == shiftId
                                                                                 && !x.IsDeleted).FirstOrDefault();
                    if (operationShift != null)
                    {
                        // Kiểm tra xem ca có phần qua ngày khác không
                        if (operationShift.StartTime > operationShift.EndTime)
                        {
                            // Nếu ngày hiện tại đang ở khung giờ trực ở ngày hôm sau
                            if (DateTime.Now.Date.Hour < operationShift.StartTime)
                            {
                                var yesterday = DateTime.Today.AddDays(-1);
                                // Danh sách NKVH đang trong ca
                                isEditable = _unitOfWork.ElogbookRepository<OperationDiary>()
                                    .GetQuery(t => t.OperationShift.ID == shiftId && DbFunctions.TruncateTime(t.CreatedDate) == yesterday).Any();
                            }
                            else
                            {
                                // Danh sách NKVH đang trong ca
                                isEditable = _unitOfWork.ElogbookRepository<OperationDiary>()
                                    .GetQuery(t => t.OperationShift.ID == shiftId && DbFunctions.TruncateTime(t.CreatedDate) == DateTime.Today.Date).Any();
                            }
                        }
                        else
                        {
                            // Danh sách NKVH đang trong ca
                            isEditable = _unitOfWork.ElogbookRepository<OperationDiary>()
                                .GetQuery(t => t.OperationShift.ID == shiftId && DbFunctions.TruncateTime(t.CreatedDate) == DateTime.Today.Date).Any();
                        }

                        if (isEditable && entity.StatusID == (int)IssueOfShiftStatus.DONE)
                        {
                            double timeComplete = (double)entity.CompletedDateTime.GetValueOrDefault().Minute / 60;
                            double hourComplete = entity.CompletedDateTime.GetValueOrDefault().Hour + timeComplete;
                            List<ValueLabelModel> shiftComplete = _commonService.GetOperationShiftsByFactoryID(factoryID, hourComplete.ToString("F2"));
                            // sau khi trạng thái là đã xử lý. Chỉ được edit nếu còn trong ca
                            if (shiftComplete.Count == 0
                                || shifts.First().Value.Value != shiftComplete.First().Value.Value
                                || DateTime.Now.Date.Date != entity.CompletedDateTime.GetValueOrDefault().Date.Date)
                            {
                                isEditable = false;
                            }
                        }
                    }
                }
            }

            return isEditable;

        }

        private bool CheckEditIssueOfShiftDetail(int id, bool isAdmin, int userID)
        {
            var isEditable = false;
            var entity = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().GetById(id);

            if (entity != null)
            {
                double time = (double)DateTime.Now.Minute / 60;
                double currentHour = DateTime.Now.Hour + time;

                List<ValueLabelModel> shiftsCurrent = _commonService.GetOperationShiftsByFactoryID(entity.ExistInShift.OperationDiary.FactoryID.Value, currentHour.ToString("F2"));

                if (shiftsCurrent.Count > 0 && (isAdmin || userID == entity.CreatedUserID))
                {
                    //Chưa giao nhận ca và đang trong khung giờ trực thì được cập nhật nội dung xử lý
                    double TimeCreated = (double)entity.InsertedAt.Minute / 60;
                    double HourCreated = entity.InsertedAt.Hour + TimeCreated;
                    List<ValueLabelModel> shifts = _commonService.GetOperationShiftsByFactoryID(entity.ExistInShift.OperationDiary.FactoryID.Value,
                        HourCreated.ToString("F2"));

                    if (shifts.Count > 0 && shifts.First().Value.Value == shiftsCurrent.First().Value.Value)
                    {
                        int shiftId = shifts.First().Value.Value;
                        var operationShift = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == entity.ExistInShift.OperationDiary.FactoryID.Value
                                                                                 && x.Active == (int)AccessStatus.Active
                                                                                 && x.ID == shiftId
                                                                                 && !x.IsDeleted).FirstOrDefault();
                        if (operationShift != null)
                        {
                            // Kiểm tra xem ca có phần qua ngày khác không
                            if (operationShift.StartTime > operationShift.EndTime)
                            {
                                // Nếu ngày hiện tại đang ở ngày khác thì kiểm tra nhật ký ở ngày trước
                                if ((DateTime.Today.Date - entity.InsertedAt).Days == 1)
                                {
                                    // Danh sách NKVH đang trong ca
                                    isEditable = _unitOfWork.ElogbookRepository<OperationDiary>()
                                        .GetQuery(t => t.OperationShift.ID == shiftId && DbFunctions.TruncateTime(t.CreatedDate) == entity.InsertedAt).Any();
                                    return isEditable;
                                }
                            }
                            isEditable = _unitOfWork.ElogbookRepository<OperationDiary>()
                                    .GetQuery(t => t.OperationShift.ID == shiftId && DbFunctions.TruncateTime(t.CreatedDate) == DateTime.Today.Date).Any();
                        }
                    }
                }
            }

            return isEditable;
        }

        #endregion
    }
}
