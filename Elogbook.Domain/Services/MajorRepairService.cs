using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using TicketStatus = Elogbook.Data.TicketStatus;

namespace Elogbook.Domain.Services
{
    public class MajorRepairService : IMajorRepairService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private static readonly object _lockRequestTicket = new object();

        #endregion

        #region Contructor
        public MajorRepairService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        #endregion

        #region Public method
        public bool CheckDuplicateMajorRepair(int id, int factoryID, string title, bool isCreate)
        {
            // Trường hợp thêm mới
            if (isCreate)
            {
                return _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)
                                                                                && x.FactoryID == factoryID
                                                                                && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)
                                                                                && x.FactoryID == factoryID
                                                                                && x.ID != id
                                                                                && !x.IsDeleted).Any();
            }
        }

        public bool IsAvailableUpdateWorkTicket(int id)
        {
            return _unitOfWork.ElogbookRepository<MajorRepair>()
                .GetQuery(x => x.ID == id && x.MajorRepairStatusID != (int)MajorRepairStatusDefine.Doing && !x.IsDeleted).Any();
        }

        public int InsertWorkTicketFromSchedule()
        {
            // Lấy danh sách đợt sữa chữa
            var majorRepairs = _unitOfWork.ElogbookRepository<MajorRepair>()
                .GetQuery(x => !x.IsDeleted && (x.MajorRepairStatusID == (int)MajorRepairStatusDefine.Waiting
                || x.MajorRepairStatusID == (int)MajorRepairStatusDefine.Error) && x.PlanCreateTicketTime <= DateTime.Now).ToList();

            if (majorRepairs != null && majorRepairs.Count > 0)
            {
                //List ID đợt sửa chữa lớn
                var marjorRepairIds = majorRepairs.Select(x => x.ID).ToList();

                //List phiếu công tác từ table tạm
                var excelDatas = _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().GetQuery(x => !x.IsDeleted && marjorRepairIds.Contains(x.MajorRepairId)).ToList();

                if (excelDatas != null && excelDatas.Count > 0)
                {
                    foreach (var item in majorRepairs)
                    {
                        // Kiểm tra nếu đợt sửa chữa lớn đã kết thúc thì xóa những phiếu đang bị lỗi
                        if (item.EndDate < DateTime.Now)
                        {
                            _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().Delete(x => x.MajorRepairId == item.ID, 0, true);
                        }

                        var history = new ScheduleHistory()
                        {
                            ScheduleName = "Cấp phiếu hàng loạt cho đợt sửa chữa lớn: " + item.Title,
                            Status = 1,
                            ScheduleTime = DateTime.Now
                        };

                        int countSuccess = 0;
                        int countError = 0;

                        var excelDataOfMajorRepairs = excelDatas.Where(x => x.MajorRepairId == item.ID);
                        List<int> lstTempId = new List<int>();
                        if (excelDataOfMajorRepairs != null)
                        {
                            // Set status for MajorRepair
                            item.MajorRepairStatusID = (int)MajorRepairStatusDefine.Doing;
                            _unitOfWork.ElogbookRepository<MajorRepair>().Update(item);
                            _unitOfWork.Complete();

                            // Save log khi hệ thống tạo hàng loạt phiếu công tác
                            MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                            {
                                MajorRepairID = item.ID,
                                Content = MajorRepairConstant.MSG_SYS_CREATE_PCT,
                                AttachmentID = item.AttachmentID,
                                MajorRepairStatusID = (int)MajorRepairStatusDefine.Doing
                            };
                            _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);
                            _unitOfWork.Complete();
                            Thread.Sleep(200);

                            foreach (var it in excelDataOfMajorRepairs)
                            {
                                using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
                                {
                                    try
                                    {
                                        //Tạo phiếu công tác Điện , TCNH
                                        var newTicket = new WorkTicket
                                        {
                                            MajorRepairID = it.MajorRepairId,
                                            WorkTicketStatus = (int)EnumTicketStatus.WaitForRegist,
                                            WorkPlacement = it.WorkTicketLocation + '-' + it.WorkTicketUnitArea,
                                            WorkTicketType = (int)TicketType.InsidePlan,
                                            FactoryID = item.FactoryID,
                                            ScreenType = it.ScreenType,
                                            WorkContent = it.WorkContent,
                                            StartDate = item.StartDate,
                                            EndDate = item.EndDate,
                                            CreatedUserID = it.CreatedUserID,
                                            InsertedAt = DateTime.Now,
                                            LastUpdatedUserID = it.CreatedUserID,
                                            UpdatedAt = DateTime.Now,
                                            WorkTicketNumber = CreateTicketNumber(item.FactoryID, it.ScreenType)
                                        };

                                        // Add record to table WorkTicket
                                        _unitOfWork.ElogbookRepository<WorkTicket>().Add(newTicket);
                                        _unitOfWork.Complete();

                                        transaction.Commit();
                                        countSuccess++;
                                        lstTempId.Add(it.ID);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "MajorRepairService: InsertWorkTicketFromSchedule ID=" + it.ID.ToString(), Environment.NewLine + ex.ToString());
                                        transaction.Rollback();
                                        countError++;
                                    }
                                }
                            }

                            if (countError > 0)
                            {
                                item.MajorRepairStatusID = (int)MajorRepairStatusDefine.Error;
                                history.Status = 1;
                                majorRepairHistory.MajorRepairStatusID = (int)MajorRepairStatusDefine.Error;
                            }
                            else
                            {
                                history.Status = 2;
                                majorRepairHistory.MajorRepairStatusID = (int)MajorRepairStatusDefine.Finish;
                                item.MajorRepairStatusID = (int)MajorRepairStatusDefine.Finish;
                            }

                            // Save lịch sử khi tạo cấp phiếu hàng loạt
                            _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);

                            // Delete all record  to MajorRepairExcelDataTemp
                            if (lstTempId.Count > 0)
                            {
                                _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().Delete(x => lstTempId.Contains(x.ID), 0, true);
                            }

                            // Set status for MajorRepair
                            item.TotalSuccess += countSuccess;

                            item.UpdatedAt = DateTime.Now;
                            _unitOfWork.ElogbookRepository<MajorRepair>().Update(item);

                            // Save history
                            history.ScheduleName = history.ScheduleName + ". Thành công: " + countSuccess.ToString() + ". Thất bại: " + countError.ToString();
                            _unitOfWork.ElogbookRepository<ScheduleHistory>().Add(history);

                            _unitOfWork.Complete();
                        }
                    }
                }
            }

            return 0;
        }

        public int CreateMajorRepair(MajorRepairAddEditModel requestModel)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var entity = new MajorRepair
                    {
                        FactoryID = requestModel.FactoryID,
                        Title = requestModel.Title,
                        StartDate = requestModel.StartDate,
                        EndDate = requestModel.EndDate,
                        CreatedDate = DateTime.Now,
                        PlanCreateTicketTime = requestModel.PlanCreateTicketTime,
                        MajorRepairStatusID = (int)MajorRepairStatusDefine.Waiting,
                        TotalTicket = requestModel.MajorRepairExcelDatas.Count,
                        TotalSuccess = 0,
                        AttachmentIds = requestModel.AttachmentID.ToString(),
                        AttachmentID = requestModel.AttachmentID,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    };
                    _unitOfWork.ElogbookRepository<MajorRepair>().Add(entity);
                    var saveSuccess = _unitOfWork.Complete() > 0;

                    if (saveSuccess)
                    {
                        // Save dữ liệu tạo PCT vào table MajorRepairExcelDataTemp
                        InsertMajorRepairExcelDataTemps(requestModel, entity.ID);

                        // Save log khi tạo mới đợt sửa chữa lớn
                        MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                        {
                            MajorRepairID = entity.ID,
                            Content = MajorRepairConstant.MSG_CREATE_SCL,
                            AttachmentID = entity.AttachmentID,
                            MajorRepairStatusID = (int)MajorRepairStatusDefine.Waiting,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID
                        };
                        _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);
                        _unitOfWork.Complete();

                        transaction.Commit();
                        return entity.ID;
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "MajorRepairService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                }

                return 0;
            }
        }

        public ResponseModel<bool> DeleteMajorRepair(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<MajorRepair>()
                        .GetQuery(x => x.ID == id && x.MajorRepairStatusID == (int)MajorRepairStatusDefine.Waiting && !x.IsDeleted).FirstOrDefault();

                    if (entity != null)
                    {
                        var diffOfDates = entity.PlanCreateTicketTime - DateTime.Now;

                        if (diffOfDates.TotalMinutes <= 5)
                        {
                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                            responseModel.Data = false;
                            responseModel.ErrorMessages.Add("Đợt công tác sửa chữa lớn này đang được chuẩn bị tạo Phiếu công tác. Không được xóa");
                            return responseModel;
                        }

                        // Xóa đợt sửa chữa lớn
                        _unitOfWork.ElogbookRepository<MajorRepair>().Delete(entity, userID);

                        // Xóa phiếu tạm chưa được cấp
                        var excelData = _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().GetQuery(x => !x.IsDeleted && x.MajorRepairId == entity.ID);
                        if (excelData != null && excelData.Count() > 0)
                        {
                            _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().Delete(x => x.MajorRepairId == entity.ID, 0, true);
                        }

                        // Save log khi tạo mới đợt sửa chữa lớn
                        MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                        {
                            MajorRepairID = entity.ID,
                            Content = MajorRepairConstant.MSG_DELETE_SCL,
                            AttachmentID = entity.AttachmentID,
                            MajorRepairStatusID = (int)MajorRepairStatusDefine.Waiting,
                            CreatedUserID = userID,
                            LastUpdatedUserID = userID
                        };
                        _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);

                        _unitOfWork.Complete();

                        transaction.Commit();
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                        responseModel.Data = true;
                        return responseModel;
                    }
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add("Đối tượng cần xóa không tồn tại");
                    return responseModel;
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "MajorRepairService",
                    Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                    responseModel.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add("Đã có lỗi xảy ra trong quá trình xóa!");
                    return responseModel;
                }
            }
        }

        public HttpResponseMessage DownloadFile(int id)
        {
            var rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            // Lấy data cần tải
            var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (attachment != null)
            {
                var outputFile = Path.Combine(rootPath, attachment.FilePath);

                if (File.Exists(outputFile))
                {
                    var fileBytes = File.ReadAllBytes(outputFile);

                    if (fileBytes.Length > 0)
                    {
                        return Download(new ByteArrayContent(fileBytes), attachment.FileName);
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        public MajorRepairModel GetMajorRepairById(int id)
        {
            // Thông tin chi tiết của đợt sửa chữa lớn
            var entity = _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new MajorRepairModel()
              {
                  ID = e.ID,
                  Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  Title = e.Title,
                  MajorRepairStatus = new ValueLabelCheckedModel { Value = e.MajorRepairStatus.ID, Label = e.MajorRepairStatus.Status },
                  CreatedDate = e.CreatedDate,
                  StartDate = e.StartDate,
                  EndDate = e.EndDate,
                  PlanCreateTicketTime = e.PlanCreateTicketTime,
                  CreaterUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                  ExcelAttIds = e.AttachmentIds,
                  TotalTicket = e.TotalTicket,
                  TotalSuccess = e.TotalSuccess,
                  LastAttachmentId = e.AttachmentID,
                  ExcelPath = e.AttachmentFile.FilePath
              }).FirstOrDefault();

            if (entity != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.ExcelAttIds))
                {
                    var attSplit = entity.ExcelAttIds.Split(',');

                    var listofIDs = attSplit.Select(int.Parse).ToList();

                    if (listofIDs.Count > 0)
                    {
                        // Thông tin các file đã upload
                        entity.AttachmentExcelFile = GetAttachedFile(listofIDs);
                    }
                }

                // Dữ liệu file excel
                if (File.Exists(entity.ExcelPath))
                {
                    entity.MajorRepairExcelDatas = GetExcelDatas(entity.ExcelPath);
                }

                // Thông tin phiếu công tác điện
                entity.PCTDienInfo = GetWorkTicketInfo((int)TicketTypePCT.DIEN, entity.ID);

                // Thông tin phiếu công tác Thủy Cơ Nhiệt Hóa
                entity.PCTTCNHInfo = GetWorkTicketInfo((int)TicketTypePCT.COTHUYNHIETHOA, entity.ID);

                // Lịch sử thao tác
                entity.MajorRepairHistories = GetMajorRepairHistoriesByMajorRepairID(entity.ID);
            }

            return entity;
        }

        public ResponseBaseModel GetMajorRepairs(MajorRepairSearchModel requestModel)
        {
            DateTime stDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.StartDate, false);
            DateTime edDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.EndDate, false);
            DateTime crFromDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreatedFromDate, false);
            DateTime crToDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreatedToDate, false);

            // Tất cả thông số theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => !x.IsDeleted);

            //Tên đợt công tác
            if (!string.IsNullOrWhiteSpace(requestModel.Title))
            {
                entities = entities.Where(x => x.Title.Contains(requestModel.Title));
            }

            // Nhà máy
            if (requestModel.FactoryID > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Trạng thái các đợt sửa chữa lớn
            if (requestModel.MajorRepairStatusID > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.MajorRepairStatusID == requestModel.MajorRepairStatusID);
            }

            //Ngày dự kiến bắt đầu (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.StartDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.StartDate) >= stDate.Date
                                            || DbFunctions.TruncateTime(x.EndDate) >= stDate.Date);
            }

            //Ngày dự kiến kết thúc (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.EndDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.StartDate) <= edDate.Date
                                            || DbFunctions.TruncateTime(x.EndDate) <= edDate.Date);
            }

            //Ngày tạo đợt (from)
            if (!string.IsNullOrWhiteSpace(requestModel.CreatedFromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= crFromDate.Date);
            }

            //Ngày tạo đợt (to)
            if (!string.IsNullOrWhiteSpace(requestModel.CreatedToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <= crToDate.Date);
            }

            var page = entities.Select(
                e => new MajorRepairModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Title = e.Title,
                    MajorRepairStatus = new ValueLabelCheckedModel { Value = e.MajorRepairStatus.ID, Label = e.MajorRepairStatus.Status },
                    CreatedDate = e.CreatedDate,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    PlanCreateTicketTime = e.PlanCreateTicketTime,
                    CreaterUser = e.CreatedUserID.HasValue ? new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName } : null,
                })
                .OrderByDescending(t => t.CreatedDate)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public ResponseModel<bool> UpdateMajorRepair(MajorRepairAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            responseModel.Data = false;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                    if (entity != null && entity.MajorRepairStatusID == (int)MajorRepairStatusDefine.Waiting)
                    {
                        // Cập nhật SCL đối với trường hợp "Chưa thực hiện cấp phiếu"
                        int attID = int.Parse(entity.AttachmentIds);
                        entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                        entity.Title = !string.IsNullOrEmpty(requestModel.Title) ? requestModel.Title : entity.Title;
                        entity.StartDate = requestModel.StartDate > DateTime.MinValue ? requestModel.StartDate : entity.StartDate;
                        entity.EndDate = requestModel.EndDate < DateTime.MaxValue ? requestModel.EndDate : entity.EndDate;
                        entity.PlanCreateTicketTime = requestModel.PlanCreateTicketTime;
                        entity.TotalTicket = requestModel.MajorRepairExcelDatas.Count;
                        entity.AttachmentIds = (requestModel.AttachmentID > 0 && attID != requestModel.AttachmentID) ? requestModel.AttachmentID.ToString() : entity.AttachmentIds;
                        entity.AttachmentID = (requestModel.AttachmentID > 0 && attID != requestModel.AttachmentID) ? requestModel.AttachmentID : entity.AttachmentID;
                        entity.LastUpdatedUserID = requestModel.CurrentUserID;
                        entity.UpdatedAt = DateTime.Now;

                        _unitOfWork.ElogbookRepository<MajorRepair>().Update(entity);

                        _unitOfWork.Complete();

                        // Xóa file data cũ nếu có
                        if (attID != requestModel.AttachmentID && requestModel.AttachmentID > 0)
                        {
                            var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == attID && !x.IsDeleted).FirstOrDefault();
                            if (attachment != null)
                            {
                                // kiểm tra file đã tồn tại chưa và xoá
                                var filePath = Path.Combine(AppSettings.SERVER_DIR, attachment.FilePath);
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }
                                _unitOfWork.ElogbookRepository<Attachment>().Delete(attachment, requestModel.CurrentUserID);
                                _unitOfWork.Complete();
                            }
                        }

                        // Update excel data nếu có
                        if (requestModel.MajorRepairExcelDatas != null && requestModel.MajorRepairExcelDatas.Count > 0)
                        {
                            var excelData = _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().GetQuery(x => !x.IsDeleted && x.MajorRepairId == entity.ID);
                            if (excelData != null && excelData.Count() > 0)
                            {
                                _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().Delete(x => x.MajorRepairId == entity.ID, 0, true);
                                _unitOfWork.Complete();
                                InsertMajorRepairExcelDataTemps(requestModel, entity.ID);
                            }
                        }

                        // Save log khi tạo mới đợt sửa chữa lớn
                        MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                        {
                            MajorRepairID = entity.ID,
                            Content = MajorRepairConstant.MSG_UPDATE_SCL,
                            AttachmentID = entity.AttachmentID,
                            MajorRepairStatusID = (int)MajorRepairStatusDefine.Waiting,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID
                        };
                        _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);

                        transaction.Commit();
                        responseModel.Data = true;
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "MajorRepairService",
                    Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                }
            }
            return responseModel;
        }

        public MajorRepairExcelDataValidationModel UploadFile(MajorRepairUploadInfoModel requestModel)
        {
            // Lưu vào file tạm
            if (File.Exists(requestModel.FilePath))
            {
                File.Delete(requestModel.FilePath);
            }
            File.Move(requestModel.LocalFileName, requestModel.FilePath);

            FileInfo fileInfo = new FileInfo(requestModel.FilePath);
            long fileSizeInBytes = fileInfo.Length;
            long fileSizeCheck = 1024 * 1024 * 5;

            if (fileSizeInBytes > fileSizeCheck)
            {
                var results = new MajorRepairExcelDataValidationModel
                {
                    IsValid = false,
                    ErrorMessage = "Dung lượng file phải nhỏ hơn hoặc 5M"
                };
                return results;
            }

            var data = GetMajorRepairChildModels(requestModel.FilePath, requestModel.MajorRepairID);

            // valid
            if (!data.IsValid)
            {
                File.Delete(requestModel.FilePath);
                File.Delete(requestModel.LocalFileName);
                return data;
            }
            else
            {
                File.Delete(requestModel.LocalFileName);
                // lưu vào table Attachment
                var attachment = new Attachment
                {
                    FileName = requestModel.FileName,
                    FilePath = requestModel.FilePath,
                    ScreenName = CommonConstant.MAJOR_REPAIR,
                    FileType = "xlsx",
                    CreatedUserID = requestModel.CurrentUserID
                };

                _unitOfWork.ElogbookRepository<Attachment>().Add(attachment);
                if (_unitOfWork.Complete() > 0)
                {
                    // return id
                    data.AttachmentId = attachment.ID;
                    return data;
                }
            }

            return data;
        }

        public MajorRepairExcelDataValidationModel GetMajorRepairChildModels(string excelPath, int majorRepairID)
        {
            var results = new MajorRepairExcelDataValidationModel();
            List<MajorRepairExcelDataModel> majorRepairChilds;
            List<MajorRepairExcelDataModel> majorRepairChildErros;

            int totalRecord = 0;
            if (majorRepairID > 0)
            {
                var entity = _unitOfWork.ElogbookRepository<MajorRepair>()
                    .GetQuery(x => x.ID == majorRepairID && x.MajorRepairStatusID == (int)MajorRepairStatusDefine.Finish && !x.IsDeleted).FirstOrDefault();
                if (entity != null)
                {
                    totalRecord = entity.TotalTicket.HasValue ? (int)entity.TotalTicket : 0;
                }
            }

            // Đọc file excel data
            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    // Read the data from the Excel file
                    DataSet result = reader.AsDataSet();

                    // Access the first sheet
                    DataTable dataInput = result.Tables[0];

                    // Get the last row index
                    int lastRowIndex = dataInput.Rows.Count;

                    if (totalRecord > 0 && lastRowIndex - 1 <= totalRecord)
                    {
                        results.IsValid = false;
                        results.ErrorMessage = "Tổng số phiếu trong file excel mới phải lớn hơn số phiếu đã tồn tại";
                        return results;
                    }

                    if (!CheckTitleFile(dataInput))
                    {
                        results.IsValid = false;
                        results.ErrorMessage = "Nội dung file chưa đúng!";
                        return results;
                    }


                    majorRepairChilds = new List<MajorRepairExcelDataModel>();
                    majorRepairChildErros = new List<MajorRepairExcelDataModel>();
                    for (int j = totalRecord + 1; j < lastRowIndex; j++)
                    {
                        MajorRepairExcelDataModel majorRepairChild = new MajorRepairExcelDataModel();
                        if (!IsValidExcelData(dataInput, j, majorRepairChild))
                        {
                            majorRepairChildErros.Add(majorRepairChild);
                        }
                        else
                        {
                            majorRepairChilds.Add(majorRepairChild);
                        }
                    }
                }
            }

            if (majorRepairChildErros.Count > 0)
            {
                results.IsValid = false;
                results.MajorRepairExcelDatas = majorRepairChildErros;
            }
            else
            {
                results.IsValid = true;
                results.MajorRepairExcelDatas = majorRepairChilds;
            }

            return results;
        }

        private bool CheckTitleFile(DataTable dataInput)
        {
            // Check Title 1 (Miêu tả)
            string title1 = dataInput.Rows[0][MajorRepairConstant.WORK_CONTENT].ToString().Trim();
            if (!title1.Equals("Miêu tả"))
            {
                return false;
            }

            // Check Title 2 (Vị trí / KKS)
            string title2 = dataInput.Rows[0][MajorRepairConstant.WORK_TICKET_LOCATION].ToString().Trim();
            if (!title2.Equals("Vị trí / KKS"))
            {
                return false;
            }

            // Check Title 3 (Loại Phiếu (1: Điện, 2: TCNH))
            string title3 = dataInput.Rows[0][MajorRepairConstant.WORK_TICKET_TYPE].ToString().Trim();
            if (!title3.Equals("Loại Phiếu\n(1: Điện, 2: TCNH)"))
            {
                return false;
            }

            // Check Title 4 (Tổ Máy / Khu vực)
            string title4 = dataInput.Rows[0][MajorRepairConstant.TICKET_UNIT_AREA].ToString().Trim();
            if (!title4.Equals("Tổ Máy / Khu vực"))
            {
                return false;
            }

            return true;
        }

        public List<ValueLabelModel> GetMajorRepairStatus()
        {
            return _unitOfWork.ElogbookRepository<MajorRepairStatus>().GetQuery(x => !x.IsDeleted)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.Status
                                }).OrderBy(t => t.Value).ToList();
        }

        public ResponseBaseModel FindAllWorkTickets(MajorRepairSearchWorkTicketModel requestModel)
        {
            DateTime crDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreateDate, false);

            // Tất cả thông số theo điều kiện
            var workTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => !x.IsDeleted && x.MajorRepairID > 0);

            // Theo ID sửa chữa lớn
            if (requestModel.MajorRepairID != CommonConstant.ALL_KEY)
            {
                workTickets = workTickets.Where(x => x.MajorRepairID == requestModel.MajorRepairID);
            }

            // Loại phiếu (1:Điện/ 2:TCNH)
            if (requestModel.ScreenType > CommonConstant.ZERO_KEY_COMPARE)
            {
                workTickets = workTickets.Where(x => x.ScreenType == requestModel.ScreenType);
            }

            // Nhà máy
            if (requestModel.ApprovedUnitID != CommonConstant.ALL_KEY)
            {
                workTickets = workTickets.Where(x => x.ApprovedUnitID == requestModel.ApprovedUnitID);
            }

            //Trạng thái phiếu công tác
            if (requestModel.WorkTicketStatus > 0)
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus == requestModel.WorkTicketStatus);
            }

            //Trạng thái phiếu công tác
            if (requestModel.WorkTicketStatusList != null && requestModel.WorkTicketStatusList.Count > 0)
            {
                workTickets = workTickets.Where(x => requestModel.WorkTicketStatusList.Contains(x.WorkTicketStatus));
            }

            //Đánh dấu những PCT cần chú ý (Có xảy ra tai nạn, sự cố trong quá trình công tác)
            if (requestModel.IsFlagNote)
            {
                workTickets = workTickets.Where(x => x.IsFlagNote == requestModel.IsFlagNote);
            }

            //Ngày cấp phiếu
            if (!string.IsNullOrWhiteSpace(requestModel.CreateDate))
            {
                workTickets = workTickets.Where(x => DbFunctions.TruncateTime(x.InsertedAt) == crDate.Date);
            }

            // nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                workTickets = workTickets.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            //Thông tin phiếu công tác (Số phiếu/Địa điểm/Nội dung/Phạm vi công tác)
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                workTickets = workTickets.Where(x => x.WorkTicketNumber.Contains(requestModel.SearchText.Trim())
                                                    || x.WorkContent.Contains(requestModel.SearchText.Trim())
                                                    || x.WorkPlacement.Contains(requestModel.SearchText.Trim())
                                                    || x.WorkScope.Contains(requestModel.SearchText.Trim()));
            }

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
                MajorRepair = new ValueLabelModel { Value = e.MajorRepair.ID, Label = e.MajorRepair.Title },
                WorkContent = e.WorkContent,
                WorkPlacement = e.WorkPlacement,
                RegistrationTicket = new ValueLabelModel
                {
                    Value = e.RegistrationTicketID.HasValue ? e.RegistrationTicket.ID : 0,
                    Label = e.RegistrationTicketID.HasValue ? e.RegistrationTicket.TicketNumber : ""
                },
                WorkTicketFeatures = e.WorkTicketFeatureLinks.Select(z => new WorkTicketFeaturesModel()
                {
                    Label = z.WorkTicketFeatures.FeatureName,
                    FeatureID = z.FeatureID,
                }).ToList(),
                Approver = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Approver && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                Acceptor = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                CreaterUser = new ValueLabelModel
                {
                    Value = e.CreatedUser.ID,
                    Label = e.CreatedUser.FullName
                },
                InsertedAt = e.InsertedAt,
                CreatedDate = e.InsertedAt,
                UpdatedAt = e.UpdatedAt,
                IsFlagNote = e.IsFlagNote
            }).OrderBy(t => t.WorkTicketStatus.Value)
            .ThenByDescending(t => t.CreatedDate)
            .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Lây danh sách trạng thái Phiếu công tác tại SCL
        /// </summary>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        public List<ValueLabelCheckedModel> GetMajorRepairTicketStatus(string ticketType)
        {
            var lstId = new List<int> {
                (int)EnumTicketStatus.WaitForRegist,
                (int)EnumTicketStatus.Waiting,
                (int)EnumTicketStatus.Approved,
                (int)EnumTicketStatus.Allowed,
                (int)EnumTicketStatus.InProgress,
                (int)EnumTicketStatus.WaitForConfirmation,
                (int)EnumTicketStatus.Completed,
                (int)EnumTicketStatus.NotAllowed,
                (int)EnumTicketStatus.Annulled
            };

            if (ticketType.Equals("TCNH"))
            {
                lstId.Add((int)EnumTicketStatus.WaitingMaterials);
                lstId.Add((int)EnumTicketStatus.Skipped);
                lstId.Add((int)EnumTicketStatus.ExpiredLock);
            }

            return _unitOfWork.ElogbookRepository<TicketStatus>().GetQuery(x => !x.IsDeleted && lstId.Contains(x.ID))
                .Select(e => new ValueLabelCheckedModel() { Value = e.ID, Label = e.StatusName })
                .OrderBy(t => t.Value).ToList();
        }

        public HttpResponseMessage DownloadFileTemplate()
        {
            var pathCurrent = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var outputFile = Path.Combine(pathCurrent.Substring(6), "Template", "SCL_Template.xlsx");

            if (File.Exists(outputFile))
            {
                var fileBytes = File.ReadAllBytes(outputFile);

                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), "SCL_Template.xlsx");
                }
            }
            else
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + "File not found: " + outputFile);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        public ResponseBaseModel GetTitleMajorRepair(int factoryID)
        {
            var entities = _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => !x.IsDeleted);

            if (factoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == factoryID);
            }

            var dataResult = entities.Select(e => new ValueLabelCheckedModel()
            {
                Value = e.ID,
                Label = e.Title
            }).ToList();

            return HttpResponse.OkList(dataResult);
        }

        public ResponseModel<bool> InsertMultipleTicket(InsertNewTicketModel requestModel)
        {
            ResponseModel<bool> responseBaseModel = new ResponseModel<bool>();
            responseBaseModel.Data = false;

            var entity = _unitOfWork.ElogbookRepository<MajorRepair>().GetQuery(x => x.ID == requestModel.MajorRepairID && !x.IsDeleted).FirstOrDefault();

            // Cập nhật SCL đối với trường hợp "Hoàn thành cấp phiếu"
            if (entity != null && entity.MajorRepairStatusID == (int)MajorRepairStatusDefine.Finish && requestModel.MajorRepairExcelDatas.Count > 0)
            {
                // Set status for MajorRepair
                entity.MajorRepairStatusID = (int)MajorRepairStatusDefine.Doing;
                _unitOfWork.ElogbookRepository<MajorRepair>().Update(entity);
                _unitOfWork.Complete();

                // Save log khi hệ thống tạo hàng loạt phiếu công tác
                MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                {
                    MajorRepairID = entity.ID,
                    Content = MajorRepairConstant.MSG_UPDATE_SCL,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    AttachmentID = entity.AttachmentID,
                    MajorRepairStatusID = (int)MajorRepairStatusDefine.Doing
                };
                _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);
                _unitOfWork.Complete();
                Thread.Sleep(200);

                //Tạo phiếu công tác Điện , TCNH
                int countSuccess = 0;
                int countError = 0;
                var listMajorRepairExcelDataTemps = new List<MajorRepairExcelDataTemp>();
                foreach (var item in requestModel.MajorRepairExcelDatas)
                {
                    using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        try
                        {
                            var newTicket = new WorkTicket
                            {
                                MajorRepairID = entity.ID,
                                ScreenType = item.ScreenType,
                                WorkPlacement = item.WorkTicketLocation + '-' + item.WorkTicketUnitArea,
                                WorkContent = item.WorkContent,
                                FactoryID = entity.FactoryID,
                                WorkTicketType = (int)TicketType.InsidePlan,
                                WorkTicketStatus = (int)EnumTicketStatus.WaitForRegist,
                                StartDate = entity.StartDate,
                                EndDate = entity.EndDate,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                                WorkTicketNumber = CreateTicketNumber(entity.FactoryID, item.ScreenType)
                            };

                            // Add record to table WorkTicket
                            _unitOfWork.ElogbookRepository<WorkTicket>().Add(newTicket);
                            _unitOfWork.Complete();

                            transaction.Commit();
                            countSuccess++;
                        }
                        catch
                        {
                            var newTicket = new MajorRepairExcelDataTemp()
                            {
                                MajorRepairId = entity.ID,
                                RowIndex = item.RowIndex,
                                ScreenType = item.ScreenType,
                                WorkContent = ReplaceWhiteSpace(item.WorkContent),
                                WorkTicketLocation = ReplaceWhiteSpace(item.WorkTicketLocation),
                                WorkTicketUnitArea = ReplaceWhiteSpace(item.WorkTicketUnitArea),
                                UpdatedAt = DateTime.Now,
                                InsertedAt = DateTime.Now,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID
                            };

                            listMajorRepairExcelDataTemps.Add(newTicket);
                            countError++;
                        }
                    }
                }

                var history = new ScheduleHistory()
                {
                    ScheduleName = "Cấp phiếu bổ sung đợt sửa chữa lớn: " + entity.Title,
                    ScheduleTime = DateTime.Now,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID
                };

                if (countError > 0)
                {
                    entity.MajorRepairStatusID = (int)MajorRepairStatusDefine.Error;
                    history.Status = 1;
                    majorRepairHistory.MajorRepairStatusID = (int)MajorRepairStatusDefine.Error;
                }
                else
                {
                    history.Status = 2;
                    majorRepairHistory.MajorRepairStatusID = (int)MajorRepairStatusDefine.Finish;
                    entity.MajorRepairStatusID = (int)MajorRepairStatusDefine.Finish;
                }

                // Lưu lịch sử khi chỉnh sửa tạo thêm phiếu công tác
                _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);

                // Trường hợp có xảy ra lỗi bất thường thì lưu vào table tạm MajorRepairExcelDataTemp
                if (listMajorRepairExcelDataTemps.Count > 0)
                {
                    _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().AddRange(listMajorRepairExcelDataTemps);
                }

                // Save history
                history.ScheduleName = history.ScheduleName + ". Thành công: " + countSuccess.ToString() + ". Thất bại: " + countError.ToString();
                _unitOfWork.ElogbookRepository<ScheduleHistory>().Add(history);

                //Cập nhật trạng thái cấp phiếu
                entity.TotalTicket += requestModel.MajorRepairExcelDatas.Count;
                entity.TotalSuccess += countSuccess;
                entity.AttachmentIds = entity.AttachmentIds + "," + requestModel.AttachmentID.ToString();
                entity.AttachmentID = requestModel.AttachmentID;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;
                _unitOfWork.ElogbookRepository<MajorRepair>().Update(entity);

                _unitOfWork.Complete();

                responseBaseModel.Data = true;
                responseBaseModel.StatusCode = (int)HttpStatusCode.OK;
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> DeleteTicket(int workTicketID, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            var entity = _unitOfWork.ElogbookRepository<WorkTicket>()
                        .GetQuery(x => x.ID == workTicketID && x.WorkTicketStatus == (int)EnumTicketStatus.WaitForRegist && !x.IsDeleted).FirstOrDefault();

            if (entity != null)
            {
                int? createdUserID = entity.CreatedUserID;

                if (createdUserID != userID)
                {
                    responseModel.ErrorMessages.Add("Người dùng này không có quyền xóa đối với phiếu công tác này");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                _unitOfWork.ElogbookRepository<WorkTicket>().Delete(entity, userID);

                // Save log khi người dùng xóa phiếu công tác
                MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                {
                    MajorRepairID = entity.MajorRepairID,
                    WorkTicketID = entity.ID,
                    WorkTicketNumber = entity.WorkTicketNumber,
                    Content = MajorRepairConstant.MSG_DELETE_PCT,
                    CreatedUserID = userID,
                    LastUpdatedUserID = userID,
                };
                _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);

                if (_unitOfWork.Complete() > 0)
                {
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateTicket(MajorRepairUpdateWorkTicketModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            var entity = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID
                                                                                && x.WorkTicketStatus == (int)EnumTicketStatus.WaitForRegist
                                                                                && x.MajorRepairID == requestModel.MajorRepairID
                                                                                && x.ScreenType == requestModel.ScreenType
                                                                                && !x.IsDeleted).FirstOrDefault();

            if (entity != null)
            {
                int? createdUserID = entity.CreatedUserID;

                // Trường hợp người thao tác cập nhật không phải là người tạo ra phiếu thì báo lỗi
                if (createdUserID != requestModel.CurrentUserID)
                {
                    responseModel.ErrorMessages.Add("Người dùng này không có quyền cập nhật đối với phiếu công tác này");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // Cập nhật phiếu công tác điện
                if (requestModel.ScreenType == (int)TicketTypePCT.DIEN && requestModel.PctDienInfo != null)
                {
                    PctDienModel pctDienInfo = requestModel.PctDienInfo;

                    requestModel.LeadershipUsers = new List<AddNewLeadershipUserFetchedModel>();

                    // lấy thông tin người cho phép
                    if (pctDienInfo.Acceptor != null && pctDienInfo.Acceptor.UserName != null)
                    {
                        var acceptor = GetUserInfo(pctDienInfo.Acceptor?.UserName, pctDienInfo.Acceptor.IsExternal, (int)LeadershipUserRoles.Acceptor);
                        if (acceptor != null)
                        {
                            requestModel.LeadershipUsers.Add(acceptor);
                        }
                    }

                    // lấy thông tin người chỉ huy
                    if (pctDienInfo.Commander != null && pctDienInfo.Commander.UserName != null)
                    {
                        var commander = GetUserInfo(pctDienInfo.Commander?.UserName, pctDienInfo.Commander.IsExternal, (int)LeadershipUserRoles.Commander);
                        if (commander != null)
                        {
                            requestModel.LeadershipUsers.Add(commander);
                        }
                    }

                    // lấy thông tin người lãnh đạo
                    if (pctDienInfo.Leader != null && pctDienInfo.Leader.UserName != null)
                    {
                        var leader = GetUserInfo(pctDienInfo.Leader.UserName, pctDienInfo.Leader.IsExternal, (int)LeadershipUserRoles.Leader);
                        if (leader != null)
                        {
                            requestModel.LeadershipUsers.Add(leader);
                        }
                    }

                    // Add các nhân viên liên quan
                    if (requestModel.LeadershipUsers != null && requestModel.LeadershipUsers.Count > 0)
                    {
                        _unitOfWork.ElogbookRepository<LeadershipUser>().Delete(x => x.WorkTicketID == entity.ID, 0, true);

                        foreach (var e in requestModel.LeadershipUsers)
                        {
                            var leadershipUser = new LeadershipUser()
                            {
                                UserName = !string.IsNullOrWhiteSpace(e.UserName) ? e.UserName : null,
                                FullName = !string.IsNullOrWhiteSpace(e.FullName) ? e.FullName : null,
                                PositionName = !string.IsNullOrWhiteSpace(e.PositionName) ? e.PositionName : null,
                                IsExternal = e.IsExternal != null ? e.IsExternal : null,
                                Roles = e.Roles > 0 ? e.Roles : null,
                                ElectricRank = e.ElectricRank,
                                ElectricRankCard = !string.IsNullOrWhiteSpace(e.ElectricRankCard) ? e.ElectricRankCard : null,
                                LaborRank = e.LaborRank,
                                LaborCard = e.LaborCard,
                                Signature = !string.IsNullOrWhiteSpace(e.Signature) ? e.Signature : null,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                                WorkTicketID = entity.ID
                            };

                            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                        }
                    }
                    else
                    {
                        _unitOfWork.ElogbookRepository<LeadershipUser>().Delete(x => x.WorkTicketID == entity.ID, 0, true);
                    }

                    // Cập nhật thông tin khác
                    entity.UnitNumber = pctDienInfo.UnitNumber.HasValue ? pctDienInfo.UnitNumber : entity.UnitNumber;
                    entity.WorkPlacement = !string.IsNullOrEmpty(pctDienInfo.WorkPlacement) ? pctDienInfo.WorkPlacement : entity.WorkPlacement;
                    entity.WorkContent = !string.IsNullOrEmpty(pctDienInfo.WorkContent) ? pctDienInfo.WorkContent : entity.WorkContent;
                    entity.StartDate = pctDienInfo.StartDate != DateTime.MinValue ? pctDienInfo.StartDate : entity.StartDate;
                    entity.EndDate = pctDienInfo.EndDate != DateTime.MaxValue ? pctDienInfo.EndDate : entity.EndDate;
                    entity.WorkCondition = !string.IsNullOrEmpty(pctDienInfo.WorkCondition) ? pctDienInfo.WorkCondition : entity.WorkCondition;
                    entity.Attachments = (pctDienInfo.AttachmentsID != null) ? string.Join(",", pctDienInfo.AttachmentsID.ToArray()) : entity.Attachments;
                    entity.LastUpdatedUserID = requestModel.CurrentUserID;
                    entity.UpdatedAt = DateTime.Now;
                }

                // Cập nhật phiếu thủy cơ nhiệt hóa
                if (requestModel.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA && requestModel.PctTcnhInfo != null)
                {
                    PctTcnhModel pctTcnhInfo = requestModel.PctTcnhInfo;
                    requestModel.LeadershipUsers = new List<AddNewLeadershipUserFetchedModel>();

                    // lấy thông tin người cho phép
                    if (pctTcnhInfo.Acceptor != null && pctTcnhInfo.Acceptor.UserName != null)
                    {
                        var acceptorUser = GetUserInfo(pctTcnhInfo.Acceptor?.UserName, pctTcnhInfo.Acceptor.IsExternal, (int)LeadershipUserRoles.Acceptor);
                        if (acceptorUser != null)
                        {
                            requestModel.LeadershipUsers.Add(acceptorUser);
                        }
                    }

                    // Add các nhân viên liên quan
                    _unitOfWork.ElogbookRepository<LeadershipUser>().Delete(x => x.WorkTicketID == entity.ID, 0, true);
                    if (requestModel.LeadershipUsers != null && requestModel.LeadershipUsers.Count > 0)
                    {
                        foreach (var e in requestModel.LeadershipUsers)
                        {
                            var leadershipUser = new LeadershipUser()
                            {
                                UserName = !string.IsNullOrWhiteSpace(e.UserName) ? e.UserName : null,
                                FullName = !string.IsNullOrWhiteSpace(e.FullName) ? e.FullName : null,
                                PositionName = !string.IsNullOrWhiteSpace(e.PositionName) ? e.PositionName : null,
                                IsExternal = e.IsExternal != null ? e.IsExternal : null,
                                Roles = e.Roles > 0 ? e.Roles : null,
                                ElectricRank = e.ElectricRank,
                                ElectricRankCard = !string.IsNullOrWhiteSpace(e.ElectricRankCard) ? e.ElectricRankCard : null,
                                LaborRank = e.LaborRank,
                                LaborCard = e.LaborCard,
                                Signature = !string.IsNullOrWhiteSpace(e.Signature) ? e.Signature : null,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                                WorkTicketID = entity.ID
                            };

                            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                        }
                    }

                    // Add Loại phiếu
                    _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>().Delete(x => x.WorkTicketID == entity.ID, 0, true);
                    if (pctTcnhInfo.WorkTicketFeatures != null && pctTcnhInfo.WorkTicketFeatures.Count > 0)
                    {
                        foreach (var item in pctTcnhInfo.WorkTicketFeatures)
                        {
                            var dataNew = new WorkTicketFeatureLink()
                            {
                                WorkTicketID = entity.ID,
                                FeatureID = item,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };

                            _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>().Add(dataNew);
                        }
                    }

                    // Add Nhận diện mỗi nguy hiểm và biện pháp an toàn
                    _unitOfWork.ElogbookRepository<WarnAndSafe>().Delete(x => x.WorkTicketID == entity.ID, 0, true);
                    if (pctTcnhInfo.WarnAndSafes != null && pctTcnhInfo.WarnAndSafes.Count > 0)
                    {
                        foreach (var item in pctTcnhInfo.WarnAndSafes)
                        {
                            var dataNew = new WarnAndSafe()
                            {
                                WorkTicketID = entity.ID,
                                WarningContent = item.WarningContent,
                                SafetyMeasureContent = item.SafetyMeasureContent,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };

                            _unitOfWork.ElogbookRepository<WarnAndSafe>().Add(dataNew);
                        }
                    }

                    // Add Kiểm tra các biện pháp an toàn trước khi cho phép công tác
                    var entityAllowedInfo = _unitOfWork.ElogbookRepository<AllowedInfo>()
                                    .GetQuery(x => x.WorkTicketID == entity.ID && !x.IsDeleted).FirstOrDefault();

                    if (entityAllowedInfo != null)
                    {
                        _unitOfWork.ElogbookRepository<AllowedInfo>().Delete(entityAllowedInfo, requestModel.CurrentUserID);
                    }
                    if (pctTcnhInfo.AllowedInfoDetails != null && pctTcnhInfo.AllowedInfoDetails.Count > 0)
                    {
                        var allowedEntity = new AllowedInfo()
                        {
                            WorkTicketID = entity.ID,
                            AllowedTime = DateTime.Now,
                            TimeCommanderSigned = DateTime.Now,
                            AllowedInfoDetails = pctTcnhInfo.AllowedInfoDetails?.Select(e => new AllowedInfoDetail()
                            {
                                Content = !string.IsNullOrWhiteSpace(e.Content) ? e.Content : null,
                                Note = !string.IsNullOrWhiteSpace(e.Note) ? e.Note : null,
                                IsChecked = e.IsChecked != null ? e.IsChecked : false,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            }).ToList(),
                            CreatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            UpdatedAt = DateTime.Now,
                        };
                        _unitOfWork.ElogbookRepository<AllowedInfo>().Add(allowedEntity);
                    }

                    // Add Kiểm tra các biện pháp an toàn trước khi thực hiện công tác
                    var entityReceiveWorkplace = _unitOfWork.ElogbookRepository<ReceiveWorkplace>()
                                    .GetQuery(x => x.WorkTicketID == entity.ID && !x.IsDeleted).FirstOrDefault();

                    if (entityReceiveWorkplace != null)
                    {
                        _unitOfWork.ElogbookRepository<ReceiveWorkplace>().Delete(entityReceiveWorkplace, requestModel.CurrentUserID);
                    }

                    if (pctTcnhInfo.SafetyMeasures != null && pctTcnhInfo.SafetyMeasures.Count > 0)
                    {
                        var receiveWorkplace = new ReceiveWorkplace()
                        {
                            WorkTicketID = entity.ID,
                            StartWorkingDate = DateTime.Now,
                            SafetyMeasures = pctTcnhInfo.SafetyMeasures?.Select(e => new SafetyMeasure()
                            {
                                Content = !string.IsNullOrWhiteSpace(e.Content) ? e.Content : null,
                                Note = !string.IsNullOrWhiteSpace(e.Note) ? e.Note : null,
                                IsChecked = e.IsChecked != null ? e.IsChecked : false,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            }).ToList(),
                            CreatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            UpdatedAt = DateTime.Now,
                        };
                        _unitOfWork.ElogbookRepository<ReceiveWorkplace>().Add(receiveWorkplace);
                    }

                    // Cập nhật thông tin khác
                    entity.WorkPlacement = !string.IsNullOrEmpty(pctTcnhInfo.WorkPlacement) ? pctTcnhInfo.WorkPlacement : entity.WorkPlacement;
                    entity.WorkContent = !string.IsNullOrEmpty(pctTcnhInfo.WorkContent) ? pctTcnhInfo.WorkContent : entity.WorkContent;
                    entity.StartDate = pctTcnhInfo.StartDate != DateTime.MinValue ? pctTcnhInfo.StartDate : entity.StartDate;
                    entity.EndDate = pctTcnhInfo.EndDate != DateTime.MaxValue ? pctTcnhInfo.EndDate : entity.EndDate;
                    entity.WorkScope = !string.IsNullOrEmpty(pctTcnhInfo.WorkScope) ? pctTcnhInfo.WorkScope : entity.WorkScope;
                    entity.Attachments = (pctTcnhInfo.AttachmentsID != null && pctTcnhInfo.AttachmentsID.Count > 0) ? string.Join(",", pctTcnhInfo.AttachmentsID.ToArray()) : entity.Attachments;
                    entity.LastUpdatedUserID = requestModel.CurrentUserID;
                    entity.UpdatedAt = DateTime.Now;
                }

                _unitOfWork.ElogbookRepository<WorkTicket>().Update(entity);

                // Save log khi người dùng xóa phiếu công tác
                MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                {
                    MajorRepairID = entity.MajorRepairID,
                    WorkTicketID = entity.ID,
                    WorkTicketNumber = entity.WorkTicketNumber,
                    Content = MajorRepairConstant.MSG_UPDATE_PCT,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID
                };
                _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);

                _unitOfWork.Complete();

                responseModel.Data = true;
                responseModel.StatusCode = (int)HttpStatusCode.OK;
                return responseModel;
            }

            responseModel.Data = false;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            return responseModel;
        }

        public ResponseBaseModel GetMajorRepairHistories(MajorRepairHistorySearchModel requestModel)
        {
            DateTime stDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.StartDate, false);
            DateTime edDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.EndDate, false);

            // Tất cả thông số theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<MajorRepairHistory>().GetQuery(x => x.MajorRepairID == requestModel.MajorRepairID && !x.IsDeleted);

            //Tên đợt công tác
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.Content.Contains(requestModel.SearchText) || x.WorkTicketNumber.Contains(requestModel.SearchText));
            }

            //Ngày dự kiến bắt đầu (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.StartDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= stDate.Date);
            }

            //Ngày dự kiến kết thúc (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.EndDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= edDate.Date);
            }

            var page = entities.Select(
                e => new MajorRepairHistoryModel()
                {
                    ID = e.ID,
                    MajorRepair = e.MajorRepairID.HasValue ? new ValueLabelCheckedModel { Value = e.MajorRepair.ID, Label = e.MajorRepair.Title } : null,
                    WorkTicket = e.WorkTicketID.HasValue ? new ValueLabelCheckedModel { Value = e.WorkTicket.ID, Label = e.WorkTicket.WorkTicketNumber } : null,
                    MajorRepairStatus = e.MajorRepairStatusID.HasValue ? new ValueLabelCheckedModel { Value = e.MajorRepairStatus.ID, Label = e.MajorRepairStatus.Status } : null,
                    CreatedUser = e.CreatedUserID.HasValue ? new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName } : null,
                    Content = e.Content,
                    Comment = e.Comment,
                    InsertedAt = e.InsertedAt,
                    Attachment = e.AttachmentID.HasValue ? new ValueLabelCheckedModel { Value = e.Attachment.ID, Label = e.Attachment.FileName } : null,
                    MajorRepairStatusID = e.MajorRepairStatusID
                })
                .OrderByDescending(t => t.InsertedAt)
                .ThenByDescending(t => (int)t.MajorRepairStatusID)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public List<MajorRepairHistoryModel> GetMajorRepairHistoriesByMajorRepairID(int MajorRepairID)
        {
            var entities = _unitOfWork.ElogbookRepository<MajorRepairHistory>().GetQuery(x => x.MajorRepairID == MajorRepairID && !x.IsDeleted).Select(
                e => new MajorRepairHistoryModel()
                {
                    ID = e.ID,
                    MajorRepair = e.MajorRepairID.HasValue ? new ValueLabelCheckedModel { Value = e.MajorRepair.ID, Label = e.MajorRepair.Title } : null,
                    WorkTicket = e.WorkTicketID.HasValue ? new ValueLabelCheckedModel { Value = e.WorkTicket.ID, Label = e.WorkTicket.WorkTicketNumber } : null,
                    ScreenType = e.WorkTicketID.HasValue ? e.WorkTicket.ScreenType : null,
                    MajorRepairStatus = e.MajorRepairStatusID.HasValue ? new ValueLabelCheckedModel { Value = e.MajorRepairStatus.ID, Label = e.MajorRepairStatus.Status } : null,
                    CreatedUser = e.CreatedUserID.HasValue ? new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName } : null,
                    Content = e.Content,
                    Comment = e.Comment,
                    InsertedAt = e.InsertedAt,
                    MajorRepairStatusID = e.MajorRepairStatusID,
                    Attachment = e.AttachmentID.HasValue ? new ValueLabelCheckedModel { Value = e.Attachment.ID, Label = e.Attachment.FileName } : null,
                }).OrderByDescending(t => t.InsertedAt)
                .OrderByDescending(t => (int)t.MajorRepairStatusID)
                .ToList();

            return entities;
        }

        #endregion

        #region Private Method
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

        private void InsertMajorRepairExcelDataTemps(MajorRepairAddEditModel requestModel, int majorRepairId)
        {
            var listMajorRepairExcelDataTemps = new List<MajorRepairExcelDataTemp>();
            // Tạo phiếu con lưu vào bảng tạm
            foreach (var item in requestModel.MajorRepairExcelDatas)
            {
                var newTicket = new MajorRepairExcelDataTemp()
                {
                    MajorRepairId = majorRepairId,
                    RowIndex = item.RowIndex,
                    ScreenType = item.ScreenType,
                    WorkContent = ReplaceWhiteSpace(item.WorkContent),
                    WorkTicketLocation = ReplaceWhiteSpace(item.WorkTicketLocation),
                    WorkTicketUnitArea = ReplaceWhiteSpace(item.WorkTicketUnitArea),
                    UpdatedAt = DateTime.Now,
                    InsertedAt = DateTime.Now,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID
                };

                listMajorRepairExcelDataTemps.Add(newTicket);
            }
            _unitOfWork.ElogbookRepository<MajorRepairExcelDataTemp>().AddRange(listMajorRepairExcelDataTemps);
            _unitOfWork.Complete();
        }

        private string ReplaceWhiteSpace(string text)
        {
            StringBuilder rsText = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]))
                {
                    rsText.Append(" ");
                }
                else
                {
                    rsText.Append(text[i]);
                }
            }

            return rsText.ToString();
        }

        private bool IsValidExcelData(DataTable dataInput, int indexRow, MajorRepairExcelDataModel majorRepairChild)
        {
            majorRepairChild.Errors = new List<string>();
            majorRepairChild.RowIndex = indexRow;

            // Nội dung công tác
            if (string.IsNullOrWhiteSpace(dataInput.Rows[indexRow][MajorRepairConstant.WORK_CONTENT].ToString()))
            {
                majorRepairChild.Errors.Add("Nội dung công tác trống");
            }

            majorRepairChild.WorkContent = dataInput.Rows[indexRow][MajorRepairConstant.WORK_CONTENT].ToString();

            // Vị trí / KKS
            if (string.IsNullOrWhiteSpace(dataInput.Rows[indexRow][MajorRepairConstant.WORK_TICKET_LOCATION].ToString()))
            {
                majorRepairChild.Errors.Add("Vị trí / KKS trống");
            }
            majorRepairChild.WorkTicketLocation = dataInput.Rows[indexRow][MajorRepairConstant.WORK_TICKET_LOCATION].ToString();

            // Loại Phiếu (Sử dụng để tạo Phiếu Điện hoặc TCNH)
            if (string.IsNullOrWhiteSpace(dataInput.Rows[indexRow][MajorRepairConstant.WORK_TICKET_TYPE].ToString()))
            {
                majorRepairChild.Errors.Add("Loại phiếu công tác trống");
            }
            else
            {
                var workTicketType = int.Parse(dataInput.Rows[indexRow][MajorRepairConstant.WORK_TICKET_TYPE].ToString());

                // Loại Phiếu Không đúng định dạng Phiếu Điện hoặc TCNH)
                if (workTicketType != (int)MajorRepairWorkTicketType.PCTD && workTicketType != (int)MajorRepairWorkTicketType.PCTTCNH)
                {
                    majorRepairChild.Errors.Add("Sai loại phiếu công tác");
                }

                majorRepairChild.ScreenType = workTicketType;
            }

            // Vị trí / KKS
            if (string.IsNullOrWhiteSpace(dataInput.Rows[indexRow][MajorRepairConstant.TICKET_UNIT_AREA].ToString()))
            {
                majorRepairChild.Errors.Add("Tổ máy / Khu vực trống");
            }
            majorRepairChild.WorkTicketUnitArea = dataInput.Rows[indexRow][MajorRepairConstant.TICKET_UNIT_AREA].ToString();

            return majorRepairChild.Errors.Count <= 0;
        }

        private MajorRepairTicketInforModel GetWorkTicketInfo(int screenType, int majorRepairID)
        {
            var dataEntities = _unitOfWork.ElogbookRepository<WorkTicket>()
                .GetQuery(x => !x.IsDeleted && x.ScreenType == screenType && x.MajorRepairID == majorRepairID)
                .Select(e => new WorkTicketModel()
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
                    MajorRepair = new ValueLabelModel { Value = e.MajorRepair.ID, Label = e.MajorRepair.Title },
                    WorkTicketFeatures = e.WorkTicketFeatureLinks.Select(z => new WorkTicketFeaturesModel()
                    {
                        Label = z.WorkTicketFeatures.FeatureName,
                        FeatureID = z.FeatureID,
                    }).ToList(),
                    Approver = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Approver && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                    Acceptor = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                    InsertedAt = e.InsertedAt,
                    IsFlagNote = e.IsFlagNote
                }).OrderByDescending(t => t.WorkTicketStatus.Value)
                .ThenByDescending(t => t.ApprovedDateStyleDatetime)
                .ToList();

            if (dataEntities.Count > 0)
            {
                return new MajorRepairTicketInforModel
                {
                    TotalAll = new MRValueLabelModel
                    {
                        ID = 0,
                        Label = "Total",
                        Value = dataEntities.Count
                    },
                    TotalWaitForRegist = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.WaitForRegist,
                        Label = "Chờ đăng ký",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.WaitForRegist).ToList().Count
                    },
                    TotalApproved = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.Approved,
                        Label = "Đã cấp phiếu công tác",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.Approved).ToList().Count
                    },
                    TotalWaiting = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.Waiting,
                        Label = "Chớ cấp phiếu công tác",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.Waiting).ToList().Count
                    },
                    TotalAccepted = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.Allowed,
                        Label = "Đã cho phép",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.Allowed).ToList().Count
                    },
                    TotalInProgress = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.InProgress,
                        Label = "Đang thực hiện công tác",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.InProgress).ToList().Count
                    },
                    TotalWaitForConfirmation = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.WaitForConfirmation,
                        Label = "Chờ xác nhận hoàn thành",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.WaitForConfirmation).ToList().Count
                    },
                    TotalCompleted = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.Completed,
                        Label = "Đã hoàn thành",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.Completed).ToList().Count
                    },
                    TotalAnnulled = new MRValueLabelModel
                    {
                        ID = (int)Core.Enums.EnumTicketStatus.Annulled,
                        Label = "Đã hủy",
                        Value = dataEntities.Where(x => x.WorkTicketStatus.Value == (int)Core.Enums.EnumTicketStatus.Annulled).ToList().Count
                    },
                    WorkTickets = dataEntities
                };
            }

            return null;
        }

        private List<MajorRepairExcelDataModel> GetExcelDatas(string excelPath)
        {
            var majorRepairChilds = new List<MajorRepairExcelDataModel>();

            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    // Read the data from the Excel file
                    DataSet result = reader.AsDataSet();

                    // Access the first sheet
                    DataTable dataInput = result.Tables[0];

                    // Get the last row index
                    int lastRowIndex = dataInput.Rows.Count;

                    for (int j = 1; j < lastRowIndex; j++)
                    {
                        MajorRepairExcelDataModel majorRepairChild = new MajorRepairExcelDataModel
                        {
                            RowIndex = j,
                            WorkContent = dataInput.Rows[j][MajorRepairConstant.WORK_CONTENT].ToString(),
                            WorkTicketLocation = dataInput.Rows[j][MajorRepairConstant.WORK_TICKET_LOCATION].ToString(),
                            ScreenType = int.Parse(dataInput.Rows[j][MajorRepairConstant.WORK_TICKET_TYPE].ToString()),
                            WorkTicketUnitArea = dataInput.Rows[j][MajorRepairConstant.TICKET_UNIT_AREA].ToString()
                        };
                        majorRepairChilds.Add(majorRepairChild);
                    }
                }
            }

            return majorRepairChilds;
        }

        /// <summary>
        /// Tạo ticket number
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ticketType">1: PCTTCNH, 2: PCTD</param>
        /// <returns></returns>
        private string CreateTicketNumber(int factoryId, int ticketType)
        {
            lock (_lockRequestTicket)
            {
                var currentDate = DateTime.Now;
                var ticketTypeString = (ticketType == (int)TicketTypePCT.DIEN) ? "QLPCTD" : "QLPTCNH";

                // get dữ liệu ở bảng tạm
                var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == factoryId).FirstOrDefault();
                if (factory == null)
                {
                    return string.Empty;
                }
                var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                    .GetQuery(x => x.InsertedAt.Year == currentDate.Year && x.TicketType.Equals(ticketTypeString)).Select(x => x.TicketNumber).ToList();

                var availableMin = 0;
                var isMin = false;
                var ticketName = string.Empty;
                if (lstNumber.Count > 0)
                {
                    lstNumber.Sort();
                    foreach (var x in lstNumber)
                    {
                        availableMin += 1;
                        if (availableMin < x)
                        {
                            isMin = true;
                            break;
                        }
                    }

                    if (!isMin)
                    {
                        availableMin += 1;
                    }
                }
                else
                {
                    availableMin += 1;
                }

                var ticketFormat = string.Empty;
                switch (ticketTypeString)
                {
                    case "QLPTCNH":
                        if (!string.IsNullOrWhiteSpace(factory.PCTTCNHFormat))
                        {
                            ticketFormat = factory.PCTTCNHFormat;
                        }
                        else
                        {
                            ticketName = $"{availableMin}/{currentDate.Year}/{factory.Company?.ShortName}-{factory.ProductionPlant?.ShortName}";
                        }
                        break;
                    case "QLPCTD":
                        if (!string.IsNullOrWhiteSpace(factory.PCTDFormat))
                        {
                            ticketFormat = factory.PCTDFormat;
                        }
                        else
                        {
                            ticketName = $"{availableMin}/{currentDate.Year}/{factory.ProductionPlant?.ShortName}-{factory.Company?.ShortName}";
                        }
                        break;
                    default:
                        ticketName = string.Empty;
                        break;
                }

                if (!string.IsNullOrWhiteSpace(ticketFormat) && !string.IsNullOrWhiteSpace(factory.PCTDFormat))
                {
                    var ticketFormats = ticketFormat.Split(',');
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var item in ticketFormats)
                    {
                        switch (item)
                        {
                            case "1":
                                stringBuilder.Append($"{availableMin}");
                                break;
                            case "2":
                                stringBuilder.Append($"{currentDate.Year}");
                                break;
                            case "3":
                                stringBuilder.Append($"{factory.ShortName}");
                                break;
                            case "4":
                                stringBuilder.Append($"{factory.ProductionPlant?.ShortName}");
                                break;
                            case "5":
                                stringBuilder.Append($"{factory.Company?.ShortName}");
                                break;
                            case "6":
                                stringBuilder.Append(CommonConstant.SLASH);
                                break;
                            case "7":
                                stringBuilder.Append(CommonConstant.HYPHEN);
                                break;
                            default:
                                break;
                        }
                    }
                    ticketName = stringBuilder.ToString();
                }

                var data = new ExclusiveData()
                {
                    TicketNumber = availableMin,
                    TicketType = ticketTypeString,
                    InsertedAt = currentDate,
                    UpdatedAt = currentDate,
                };
                _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
                _unitOfWork.Complete();

                return ticketName;
            }
        }

        private List<ExcelDataFileInfo> GetAttachedFile(List<int> excelAttIds)
        {
            var results = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => excelAttIds.Contains(x.ID)).Select(e => new ExcelDataFileInfo()
            {
                ID = e.ID,
                FileName = e.FileName,
                FilePath = e.FilePath,
            }).ToList();

            return results;
        }

        private AddNewLeadershipUserFetchedModel GetUserInfo(string username, bool isExternal, int Roles)
        {
            UserInfo userInfo = _commonService.GetUserById(username, isExternal);
            if (userInfo != null)
            {
                return new AddNewLeadershipUserFetchedModel()
                {
                    FullName = userInfo.FullName,
                    UserName = userInfo.UserName,
                    ElectricRank = userInfo.ElectricRankStr ?? string.Empty,
                    ElectricRankCard = userInfo.ElectricRankCard,
                    Roles = Roles,
                    Signature = userInfo.Signature,
                    IsExternal = isExternal,
                    PositionName = userInfo.PositionName,
                    LaborRank = userInfo.LaborRankStr,
                    LaborCard = userInfo.LaborCard,
                };
            }
            else
            {
                return null;
            }
        }
        #endregion

    }
}
