using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.OperationShift;
using Elogbook.Data;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Elogbook.Domain.Services
{
    public class IssueOnShiftService : IIssueOfShiftService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        #endregion

        #region Contructor

        public IssueOnShiftService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        #endregion

        #region Public methods

        public ResponseBaseModel Find(SearchIssueOfShiftModel requestModel)
        {
            // Nhà máy
            if (requestModel.FactoryID <= 0)
            {
                requestModel.FactoryID = CommonConstant.ALL_KEY;
            }
            // Cương vị
            if (requestModel.OperationPositionID <= 0)
            {
                requestModel.OperationPositionID = CommonConstant.ALL_KEY;
            }
            // Trạng thái
            if (requestModel.StatusID < 0)
            {
                requestModel.StatusID = CommonConstant.ALL_KEY;
            }

            requestModel.SortColumn = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortColumn.ToUpper();
            requestModel.SortOrder = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortOrder.ToUpper();
            if (string.IsNullOrWhiteSpace(requestModel.SearchText)) requestModel.SearchText = "";

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("PageNumber", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageNumber,
                },

                new SqlParameter("PageSize", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageSize,
                },

                new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.FactoryID,
                },

                new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.OperationPositionID,
                },

                new SqlParameter("StatusID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.StatusID,
                },

                new SqlParameter("SearchText", System.Data.SqlDbType.NVarChar)
                {
                    Value = requestModel.SearchText,
                }
            };

            // Thời gian giao ca (từ ngày)
            if (string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                sqlParams.Add(new SqlParameter("FromDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DBNull.Value,
                });
            }
            else
            {
                sqlParams.Add(new SqlParameter("FromDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false),
                });
            }

            // Thời gian giao ca (đến ngày)
            if (string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                sqlParams.Add(new SqlParameter("ToDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DBNull.Value,
                });
            }
            else
            {
                sqlParams.Add(new SqlParameter("ToDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true),
                });
            }

            sqlParams.Add(new SqlParameter("SortColumn", System.Data.SqlDbType.NVarChar)
            {
                Value = requestModel.SortColumn,
            });

            sqlParams.Add(new SqlParameter("SortOrder", System.Data.SqlDbType.NVarChar)
            {
                Value = requestModel.SortOrder,
            });

            var datas = _unitOfWork.DataContext.Database
                .SqlQuery<SearchExistInShiftResult>("Search_TDTTC @PageNumber, @PageSize, @FactoryID, @OperationPositionID" +
                ", @StatusID, @SearchText, @FromDate, @ToDate, @SortColumn, @SortOrder", sqlParams.ToArray()).ToList();

            var totalRecords = 0;
            var totalPages = 0;
            List<ExistInShiftModel> results = new List<ExistInShiftModel>();
            if (datas.Count > 0)
            {
                foreach (var item in datas)
                {
                    var existInShiftInfo = new ExistInShiftModel()
                    {
                        ID = item.ID,
                        InsertedAt = item.InsertedAt,
                        Content = item.Content,
                        TeamLeaderUser = new ValueLabelModel { Value = 1, Label = item.TeamLeader },
                        CreatedUser = new ValueLabelModel { Value = item.CreatedUserID, Label = item.FullName },
                        Status = item.StatusID,
                        OperationDiaryID = item.OperationDiaryID,
                        RegistrationTicket = new TicketInforModel { Value = item.RegistrationTicketID, Label = item.RegistrationTicketName },
                        TeamLeader = item.TeamLeader,
                        Area = item.Area,
                        // Tài liệu liên quan (nếu có)
                        Attachments = item.Attachments,
                        FactoryID = item.FactoryID ?? 0,
                        IsEditable = item.StatusID != 2,
                        SyncDataKey = item.SyncDataKey,
                        AppID = item.AppID,
                        //file đính kèm tình hình vận hành
                        ExistInShiftDetails = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().GetQuery(x => x.ExistInShiftID == item.ID && !x.IsDeleted).Select(
                                            t => new IssueOfShiftDetailModel()
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
                    };
                    //Kiểm tra quyền edit
                    existInShiftInfo.IsEditable = CheckEditIssueOfShift(item.ID, item.FactoryID ?? 0);
                    results.Add(existInShiftInfo);
                }

                totalRecords = datas[0].TotalRecords;
                totalPages = totalRecords / requestModel.PageSize + (totalRecords % requestModel.PageSize > 0 ? 1 : 0);
            }

            return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
        }

        public ResponseBaseModel GetIssueOfShiftById(int id, int userID)
        {
            var data = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new ExistInShiftModel()
                {
                    ID = e.ID,
                    InsertedAt = e.InsertedAt,
                    Content = e.Content,
                    TeamLeaderUser = new ValueLabelModel { Value = 1, Label = e.TeamLeader },
                    CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    Status = e.StatusID,
                    OperationDiaryID = e.OperationDiaryID,
                    RegistrationTicket = new TicketInforModel { Value = e.RegistrationTicketID, Label = e.RegistrationTicketName },
                    TeamLeader = e.TeamLeader,
                    FactoryID = e.OperationDiary.FactoryID.Value,
                    Area = e.Area,
                    SyncDataKey = e.SyncDataKey,
                    AppID = e.AppID,
                    // Tài liệu liên quan (nếu có)
                    Attachments = e.Attachments,
                    ExistInShiftDetails = e.IssueOfShiftDetails.Where(x => !x.IsDeleted).Select(
                                            t => new IssueOfShiftDetailModel()
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
                        if (int.TryParse(attachment, out int idItem))
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
            if (data != null && data.ExistInShiftDetails != null)
            {
                foreach (var item in data.ExistInShiftDetails)
                {
                    item.FileAttachments = new List<AttachmentModel>();
                    var attachments = item?.Attachments?.Split(',');
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            if (int.TryParse(attachment, out int idItem))
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

        public ResponseBaseModel AddIssueOfShiftDetail(IssueOfShiftDetailAdd requestModel)
        {
            // Tạo xử lý tồn tại
            var entity = new IssueOfShiftDetail()
            {
                ExistInShiftID = requestModel.ExistInShiftID,
                Content = requestModel.Content,
                Attachments = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments) : string.Empty,
                CreatedUserID = requestModel.CreatedUserID,
                LastUpdatedUserID = requestModel.CreatedUserID,
            };

            _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                UpdateStatusIssueOfShift(requestModel.ExistInShiftID, requestModel.CreatedUserID, requestModel.IsComplete);

                return HttpResponse.OK(GetIssueOfShiftById(requestModel.ExistInShiftID, requestModel.CreatedUserID));
            }
            else
            {
                return HttpResponse.Error("Tạo không thành công");
            }
        }

        public ResponseBaseModel UpdateIssueOfShiftDetail(IssueOfShiftDetailUpdate requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().GetById(requestModel.ID);
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
                        _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().Update(entity);
                        _unitOfWork.Complete();
                        // Cập nhật trạng thái tồn tại
                        UpdateStatusIssueOfShift(requestModel.ExistInShiftID, requestModel.UpdatedUserID, requestModel.IsComplete);

                        return HttpResponse.OK(GetIssueOfShiftById(requestModel.ExistInShiftID, requestModel.UpdatedUserID));
                    }
                }
                else
                {
                    return HttpResponse.Error("Không thể cập nhật nội dung xử lý nếu bạn không có ca làm việc hiện tại.");
                }
            }
            return HttpResponse.Error("ID không tìm thấy.");
        }

        public ResponseBaseModel DeleteIssueOfShift(int id, int userID)
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

        public ResponseBaseModel DeteleIssueOfShiftDetail(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().GetById(id);
            if (entity != null)
            {
                if (CheckEditIssueOfShiftDetail(entity.ID, false, userID))
                {
                    _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().Delete(entity, userID);

                    var entityExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetById(entity.ExistInShiftID.GetValueOrDefault(0));
                    if (entityExistInShift != null)
                    {
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
        public ResponseBaseModel SyncDataIssueOfShiftDetail(IssueOfShiftDetailSyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            IssueOfShiftDetailSyncDataResultModel result = new IssueOfShiftDetailSyncDataResultModel();
            List<int> lstExistInShift = new List<int>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    LogCustom.LogMessage(TrackingLogLevel.INFO, "IssueOfShiftDetailService", Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel));

                    // Tồn tại trong ca
                    if (requestModel.ExistInShiftSyncs != null && requestModel.ExistInShiftSyncs.Count > 0)
                    {
                        foreach (var eis in requestModel.ExistInShiftSyncs)
                        {
                            // Cập nhật tồn tại ca
                            var existInShiftUpdate = _unitOfWork.ElogbookRepository<ExistInShift>()
                                    .GetQuery(x => x.ID == eis.ID && x.SyncDataKey == eis.SyncDataKey && !x.IsDeleted).FirstOrDefault();

                            if (existInShiftUpdate != null)
                            {
                                double diffTimeUpdate = (eis.UpdatedAt - existInShiftUpdate.UpdatedAt).TotalMilliseconds;
                                //Chỉ cập nhất nếu thời gian thay đổi lớn hơn thời gian đã cập nhật trước đó
                                if (diffTimeUpdate > 0)
                                {
                                    existInShiftUpdate.StatusID = eis.StatusID;
                                    existInShiftUpdate.AppID = eis.AppID;
                                    if (eis.CompletedDateTime.Year >= 1753)
                                    {
                                        existInShiftUpdate.CompletedDateTime = eis.CompletedDateTime;
                                    }
                                    if (eis.LastUpdatedUserID != null && eis.LastUpdatedUserID > 0)
                                    {
                                        existInShiftUpdate.LastUpdatedUserID = eis.LastUpdatedUserID;
                                    }
                                    existInShiftUpdate.UpdatedAt = eis.UpdatedAt;
                                    existInShiftUpdate.IsDeleted = eis.IsDeleted;
                                    existInShiftUpdate.Content = eis.Content;
                                    existInShiftUpdate.Attachments = eis.Attachments;
                                    existInShiftUpdate.Area = eis.Area;
                                    existInShiftUpdate.IsDeleteIssueOfShift = eis.IsDeleteIssueOfShift;

                                    existInShiftUpdate.SyncedDate = DateTime.Now;
                                    _unitOfWork.ElogbookRepository<ExistInShift>().Update(existInShiftUpdate);
                                    _unitOfWork.Complete();
                                    lstExistInShift.Add(existInShiftUpdate.ID);
                                }
                            }
                            else
                            {
                                LogCustom.LogMessage(TrackingLogLevel.INFO, "IssueOfShiftDetailService",
                                    Environment.NewLine + " Ignore this case.");
                            }
                        }
                    }

                    // Chi tiết tồn tại trong ca
                    if (requestModel.IssueOfShiftDetails != null && requestModel.IssueOfShiftDetails.Count > 0)
                    {
                        foreach (var item in requestModel.IssueOfShiftDetails)
                        {
                            var existInShiftEntity = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => !x.IsDeleted && x.ID == item.ExistInShiftID).FirstOrDefault();
                            if (item.ID == 0)
                            {
                                if (existInShiftEntity != null && existInShiftEntity.ID > 0)
                                {
                                    IssueOfShiftDetail issueOfShiftDetail = new IssueOfShiftDetail()
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
                                        issueOfShiftDetail.CreatedUserID = item.CreatedUserID;
                                    if (item.LastUpdatedUserID != null && item.LastUpdatedUserID > 0)
                                        issueOfShiftDetail.LastUpdatedUserID = item.LastUpdatedUserID;

                                    issueOfShiftDetail.ExistInShiftID = existInShiftEntity.ID;
                                    _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().Add(issueOfShiftDetail);
                                    _unitOfWork.Complete();
                                    lstExistInShift.Add(existInShiftEntity.ID);
                                }
                            }
                            else
                            {
                                var issueOfShiftDetailEntity = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>()
                                    .GetQuery(x => !x.IsDeleted && x.ID == item.ID && x.ExistInShiftID == item.ExistInShiftID).FirstOrDefault();

                                // trường hợp đã tồn tại thì cập nhật
                                if (issueOfShiftDetailEntity != null)
                                {
                                    double diffTimeUpdate = (item.UpdatedAt - issueOfShiftDetailEntity.UpdatedAt).TotalMilliseconds;
                                    //Chỉ cập nhất nếu thời gian thay đổi lớn hơn thời gian đã cập nhật trước đó
                                    if (diffTimeUpdate > 0)
                                    {
                                        issueOfShiftDetailEntity.AppID = item.AppID;
                                        issueOfShiftDetailEntity.Content = item.Content;
                                        issueOfShiftDetailEntity.ExistInShiftID = item.ExistInShiftID;
                                        issueOfShiftDetailEntity.IsComplete = item.IsComplete;
                                        issueOfShiftDetailEntity.Attachments = item.Attachments;
                                        issueOfShiftDetailEntity.IsDeleted = item.IsDeleted;
                                        issueOfShiftDetailEntity.InsertedAt = item.InsertedAt;
                                        issueOfShiftDetailEntity.UpdatedAt = item.UpdatedAt;
                                        if (item.ExistInShiftID != null && item.ExistInShiftID > 0) issueOfShiftDetailEntity.ExistInShiftID = item.ExistInShiftID;
                                        if (item.CreatedUserID != null && item.CreatedUserID > 0) issueOfShiftDetailEntity.CreatedUserID = item.CreatedUserID;
                                        if (item.LastUpdatedUserID != null && item.LastUpdatedUserID > 0) issueOfShiftDetailEntity.LastUpdatedUserID = item.LastUpdatedUserID;

                                        _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().Update(issueOfShiftDetailEntity);
                                        _unitOfWork.Complete();
                                    }

                                    if (existInShiftEntity != null)
                                    {
                                        lstExistInShift.Add(existInShiftEntity.ID);
                                    }
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
            if (lstExistInShift.Count > 0)
            {
                //1. Tồn tại trong ca
                result.ExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => lstExistInShift.Contains(x.ID))
                                        .Select(
                                            e => new MobileExistInShiftInfo()
                                            {
                                                AppID = e.AppID,
                                                ID = e.ID,
                                                OperationDiaryID = e.OperationDiaryID,
                                                RegistrationTicketID = e.RegistrationTicketID,
                                                RegistrationTicketName = e.RegistrationTicketName,
                                                TeamLeader = e.TeamLeader,
                                                Area = e.Area,
                                                Content = e.Content,
                                                Attachments = e.Attachments,
                                                StatusID = e.StatusID,
                                                IsDeleteIssueOfShift = e.IsDeleteIssueOfShift,
                                                CompletedDateTime = e.CompletedDateTime,
                                                //SyncDataKey = e.SyncDataKey,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt
                                            }).OrderBy(t => t.ID).ToList();
                // 2.Chi tiết tồn tại trong ca
                result.IssueOfShiftDetail = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().GetQuery(x => lstExistInShift.Contains((int)x.ExistInShiftID))
                        .Select(
                            e => new MobileIssueOfShiftDetailInfo()
                            {
                                AppID = e.AppID,
                                ID = e.ID,
                                Content = e.Content,
                                Attachments = e.Attachments,
                                IsComplete = e.IsComplete,
                                ExistInShiftID = e.ExistInShiftID,
                                IsDeleted = e.IsDeleted,
                                CreatedUserID = e.CreatedUserID,
                                InsertedAt = e.InsertedAt,
                                LastUpdatedUserID = e.LastUpdatedUserID,
                                UpdatedAt = e.UpdatedAt
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

                List<OperationShiftBaseModel> shifts = _commonService.GetOperationShiftsByFactoryID(factoryID, currentHour.ToString("F2"));

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
                            List<OperationShiftBaseModel> shiftComplete = _commonService.GetOperationShiftsByFactoryID(factoryID, hourComplete.ToString("F2"));
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

                List<OperationShiftBaseModel> shiftsCurrent = _commonService.GetOperationShiftsByFactoryID(entity.ExistInShift.OperationDiary.FactoryID.Value, currentHour.ToString("F2"));

                if (shiftsCurrent.Count > 0 && (isAdmin || userID == entity.CreatedUserID))
                {
                    //Chưa giao nhận ca và đang trong khung giờ trực thì được cập nhật nội dung xử lý
                    double TimeCreated = (double)entity.InsertedAt.Minute / 60;
                    double HourCreated = entity.InsertedAt.Hour + TimeCreated;
                    List<OperationShiftBaseModel> shifts = _commonService.GetOperationShiftsByFactoryID(entity.ExistInShift.OperationDiary.FactoryID.Value,
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
                            if (operationShift.StartTime > operationShift.EndTime && (DateTime.Today.Date - entity.InsertedAt).Days == 1)
                            {
                                // Danh sách NKVH đang trong ca
                                isEditable = _unitOfWork.ElogbookRepository<OperationDiary>()
                                    .GetQuery(t => t.OperationShift.ID == shiftId && DbFunctions.TruncateTime(t.CreatedDate) == entity.InsertedAt).Any();
                                return isEditable;
                            }
                            isEditable = _unitOfWork.ElogbookRepository<OperationDiary>()
                                    .GetQuery(t => t.OperationShift.ID == shiftId && DbFunctions.TruncateTime(t.CreatedDate) == DateTime.Today.Date).Any();
                        }
                    }
                }
            }

            return isEditable;
        }

        /// <summary>
        /// Xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportData(SearchIssueOfShiftModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var rootData = GetDataExport(requestModel);
                string fileName = "IssueOfShift";

                if (rootData.Count <= 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, "IssueOfShift")))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, "IssueOfShift"));
                }

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(_rootPath, "IssueOfShift", $"{fileName}_{currentDate}.xlsx");

                bool isCreatexlsx = GenerateExcelFile(rootData, outputFile, requestModel);

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "IssueOfShiftService: ExportData", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        private bool GenerateExcelFile(List<ExistInShiftModel> rootData, string outputFile, SearchIssueOfShiftModel requestModel)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Sheet1");

                int indexRow = 0;

                // Title
                indexRow += 1;//Excel Row Index = 2
                IRow title1 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 10, 16, title1, "THỐNG KÊ THEO DÕI TỒN TẠI TRONG CA");
                SetBoldweight(sheet, indexRow, 10, true, false, 14);

                indexRow += 2;//Excel Row Index = 4
                IRow title2 = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow, 10, 16, title2, $"Dữ liệu từ ngày {requestModel.FromDate:dd/MM/yyyy} - {requestModel.ToDate:dd/MM/yyyy}");
                SetBoldweight(sheet, indexRow, 10, true, true, 11); 

                indexRow += 2;//Excel Row Index = 6
                IRow headerRow = sheet.CreateRow(indexRow);
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 1, 1, headerRow, "STT");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 2, 3, headerRow, "Thời gian tạo");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 4, 5, headerRow, "Người tạo");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 6, 8, headerRow, "Phiếu công tác");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 9, 10, headerRow, "Chỉ huy trực tiếp");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 11, 12, headerRow, "Khu vực");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 13, 15, headerRow, "Nội dung tồn tại");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 16, 17, headerRow, "Tình trạng");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 18, 20, headerRow, "Nội dung xử lý");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 21, 22, headerRow, "Thời gian xử lý");
                CreateCellTypeStr(sheet, indexRow, indexRow + 1, 23, 24, headerRow, "Người xử lý");

                indexRow += 1;//Excel Row Index = 7
                IRow headerRow2 = sheet.CreateRow(indexRow);

                SetStyleTableHeader(sheet, new CellRangeAddress(indexRow - 1, indexRow, 1, 25), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                int stRowBdIndex = indexRow;
                int stColBdIndex = 1;

                if (rootData.Count > 0)
                {
                    int stt = 0;
                    foreach (var item in rootData)
                    {
                        stt++;
                        indexRow += 1;
                        IRow bodyRow = sheet.CreateRow(indexRow);

                        bodyRow.CreateCell(1).SetCellValue(stt.ToString());
                        CreateCellTypeStr(sheet, indexRow, indexRow, 2, 3, bodyRow, $"{item.InsertedAt:dd/MM/yyyy HH:mm}");
                        CreateCellTypeStr(sheet, indexRow, indexRow, 4, 5, bodyRow, item.CreatedUser?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 6, 8, bodyRow, item.RegistrationTicket?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 9, 10, bodyRow, item.TeamLeaderUser?.Label);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 11, 12, bodyRow, item.Area);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 13, 15, bodyRow, item.Content);
                        CreateCellTypeStr(sheet, indexRow, indexRow, 16, 17, bodyRow, item.StatusObject?.Label);
                        if (item.ExistInShiftDetails.Count > 0)
                        {
                            CreateCellTypeStr(sheet, indexRow, indexRow, 18, 20, bodyRow, item.ExistInShiftDetails[0].Content);
                            CreateCellTypeStr(sheet, indexRow, indexRow, 21, 22, bodyRow, $"{item.ExistInShiftDetails[0].CreateDate:dd/MM/yyyy HH:mm}");
                            CreateCellTypeStr(sheet, indexRow, indexRow, 23, 24, bodyRow, item.ExistInShiftDetails[0].CreatedUser?.Label);
                        }
                        else
                        {
                            CreateCellTypeStr(sheet, indexRow, indexRow, 18, 20, bodyRow, string.Empty);
                            CreateCellTypeStr(sheet, indexRow, indexRow, 21, 22, bodyRow, string.Empty);
                            CreateCellTypeStr(sheet, indexRow, indexRow, 23, 24, bodyRow, string.Empty);
                        }
                    }

                    int edRowBdIndex = indexRow;
                    int edColBdIndex = 24;

                    SetStyleTableBody(sheet, new CellRangeAddress(stRowBdIndex + 1, edRowBdIndex, 1, edColBdIndex), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin, HorizontalAlignment.Center);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "IssueOnShiftService: GenerateExcelFile", ex.ToString());
                return false;
            }
        }

        private List<ExistInShiftModel> GetDataExport(SearchIssueOfShiftModel requestModel)
        {
            // Nhà máy
            if (requestModel.FactoryID <= 0)
            {
                requestModel.FactoryID = CommonConstant.ALL_KEY;
            }
            // Cương vị
            if (requestModel.OperationPositionID <= 0)
            {
                requestModel.OperationPositionID = CommonConstant.ALL_KEY;
            }
            // Trạng thái
            if (requestModel.StatusID < 0)
            {
                requestModel.StatusID = CommonConstant.ALL_KEY;
            }

            requestModel.SortColumn = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortColumn.ToUpper();
            requestModel.SortOrder = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortOrder.ToUpper();
            if (string.IsNullOrWhiteSpace(requestModel.SearchText)) requestModel.SearchText = "";

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("PageNumber", System.Data.SqlDbType.Int)
                {
                    Value = CommonConstant.DEFAULT_PAGE_NUMBER,
                },

                new SqlParameter("PageSize", System.Data.SqlDbType.Int)
                {
                    Value = int.MaxValue,
                },

                new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.FactoryID,
                },

                new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.OperationPositionID,
                },

                new SqlParameter("StatusID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.StatusID,
                },

                new SqlParameter("SearchText", System.Data.SqlDbType.NVarChar)
                {
                    Value = requestModel.SearchText,
                }
            };

            // Thời gian giao ca (từ ngày)
            if (string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                sqlParams.Add(new SqlParameter("FromDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DBNull.Value,
                });
            }
            else
            {
                sqlParams.Add(new SqlParameter("FromDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false),
                });
            }

            // Thời gian giao ca (đến ngày)
            if (string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                sqlParams.Add(new SqlParameter("ToDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DBNull.Value,
                });
            }
            else
            {
                sqlParams.Add(new SqlParameter("ToDate", System.Data.SqlDbType.DateTime)
                {
                    Value = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true),
                });
            }

            sqlParams.Add(new SqlParameter("SortColumn", System.Data.SqlDbType.NVarChar)
            {
                Value = requestModel.SortColumn,
            });

            sqlParams.Add(new SqlParameter("SortOrder", System.Data.SqlDbType.NVarChar)
            {
                Value = requestModel.SortOrder,
            });

            var datas = _unitOfWork.DataContext.Database
                .SqlQuery<SearchExistInShiftResult>("Search_TDTTC @PageNumber, @PageSize, @FactoryID, @OperationPositionID" +
                ", @StatusID, @SearchText, @FromDate, @ToDate, @SortColumn, @SortOrder", sqlParams.ToArray()).ToList();

            List<ExistInShiftModel> results = new List<ExistInShiftModel>();
            if (datas.Count > 0)
            {
                foreach (var item in datas)
                {
                    var existInShiftInfo = new ExistInShiftModel()
                    {
                        ID = item.ID,
                        InsertedAt = item.InsertedAt,
                        Content = item.Content,
                        TeamLeaderUser = new ValueLabelModel { Value = 1, Label = item.TeamLeader },
                        CreatedUser = new ValueLabelModel { Value = item.CreatedUserID, Label = item.FullName },
                        Status = item.StatusID,
                        OperationDiaryID = item.OperationDiaryID,
                        RegistrationTicket = new TicketInforModel { Value = item.RegistrationTicketID, Label = item.RegistrationTicketName },
                        TeamLeader = item.TeamLeader,
                        Area = item.Area,
                        // Tài liệu liên quan (nếu có)
                        Attachments = item.Attachments,
                        FactoryID = item.FactoryID ?? 0,
                        IsEditable = item.StatusID != 2,
                        SyncDataKey = item.SyncDataKey,
                        AppID = item.AppID,
                        //file đính kèm tình hình vận hành
                        ExistInShiftDetails = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().GetQuery(x => x.ExistInShiftID == item.ID && !x.IsDeleted).Select(
                                            t => new IssueOfShiftDetailModel()
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
                    };
                    //Kiểm tra quyền edit
                    existInShiftInfo.IsEditable = CheckEditIssueOfShift(item.ID, item.FactoryID ?? 0);
                    results.Add(existInShiftInfo);
                }
            }

            return results;
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
    }

    /// <summary>
    /// Thông tin danh sách Tồn tại trong ca
    /// </summary>
    public class SearchExistInShiftResult
    {
        /// <summary>
        /// Mã tồn tại
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Nội dung tồn tại
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int CreatedUserID { get; set; }

        /// <summary>
        /// Họ và Tên người tạo
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Mã NKVH
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Mã PCT
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Số PCT
        /// </summary>
        public string RegistrationTicketName { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string TeamLeader { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Mã đồng bộ
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Mã tồn tại từ app
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Số dòng dữ liệu map với điều kiện tìm kiếm
        /// </summary>
        public int TotalRecords { get; set; }
    }
}
