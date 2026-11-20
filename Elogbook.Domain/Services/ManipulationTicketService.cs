using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Helpers;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseCommon;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Manipulation;
using Elogbook.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
using System.Text;
using Image = iTextSharp.text.Image;

namespace Elogbook.Domain.Services
{
    public class ManipulationTicketService : IManipulationTicketService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fBold = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fItalic = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesi.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly int tblWidth = 510;
        private readonly int TABLE_WIDTH = 510;

        public ManipulationTicketService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        #region public

        public DataCountModel FindMobileCount(SearchTicketMobileModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            //Phiếu đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => !x.IsDeleted);
            //Tìm kiếm theo Tên phiếu, Số phiếu, Người tạo phiếu, Người duyệt phiếu
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.TicketName.Contains(requestModel.SearchText)
                                                || x.TicketNumber.Contains(requestModel.SearchText)
                                                || x.CreatedUserTicket.Contains(requestModel.SearchText)
                                                || x.ApprovedUserTicket.Contains(requestModel.SearchText));
            }
            //Tìm kiếm theo Loại phiếu
            if (requestModel.WorkTicketType > 0)
            {
                entities = entities.Where(x => x.TicketType == requestModel.WorkTicketType);
            }
            //Tìm kiếm theo Trạng thái phiếu
            if (requestModel.WorkTicketStatus > 0)
            {
                entities = entities.Where(x => x.TicketStatusValue == requestModel.WorkTicketStatus);
            }
            //Ngày cấp (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= frDate.Date);
            }
            //Ngày cấp (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <= toDate.Date);
            }
            //Tìm kiếm theo Nhà máy
            if (requestModel.FactoryID != null && requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            var countData = new DataCountModel();
            var e1 = entities.Where(x => x.TicketStatusValue == 5 || x.TicketStatusValue == 6);
            countData.Count1 = e1.Count();

            var e2 = entities.Where(x => x.TicketStatusValue == 7 || x.TicketStatusValue == 8);
            countData.Count1 = e2.Count();

            var e3 = entities.Where(x => x.TicketStatusValue == 9);
            countData.Count1 = e3.Count();

            return countData;
        }

        public ResponseBaseModel<TicketSearchMobileModel> Find(SearchTicketMobileModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            //Phiếu đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => !x.IsDeleted);
            //Tìm kiếm theo Tên phiếu, Số phiếu, Người tạo phiếu, Người duyệt phiếu
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.TicketName.Contains(requestModel.SearchText)
                                                || x.TicketNumber.Contains(requestModel.SearchText)
                                                || x.CreatedUserTicket.Contains(requestModel.SearchText)
                                                || x.ApprovedUserTicket.Contains(requestModel.SearchText));
            }
            //Tìm kiếm theo Loại phiếu
            if (requestModel.WorkTicketType > 0)
            {
                entities = entities.Where(x => x.TicketType == requestModel.WorkTicketType);
            }
            //Tìm kiếm theo Trạng thái phiếu
            if (requestModel.WorkTicketStatus > 0)
            {
                entities = entities.Where(x => x.TicketStatusValue == requestModel.WorkTicketStatus);
            }
            //Ngày cấp (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= frDate.Date);
            }
            //Ngày cấp (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <= toDate.Date);
            }
            //Tìm kiếm theo Nhà máy
            if (requestModel.FactoryID != null && requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            if (requestModel.Tab == 1)
            {
                entities = entities.Where(x => x.TicketStatusValue == 5 || x.TicketStatusValue == 6);
            }
            else if (requestModel.Tab == 2)
            {
                entities = entities.Where(x => x.TicketStatusValue == 7 || x.TicketStatusValue == 8);
            }
            else if (requestModel.Tab == 3)
            {
                entities = entities.Where(x => x.TicketStatusValue == 9);
            }

            requestModel.SortColumn = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortColumn.ToUpper();
            requestModel.SortOrder = string.IsNullOrWhiteSpace(requestModel.SortOrder) ? "" : requestModel.SortOrder.ToUpper();
            if ("DESC".Equals(requestModel.SortOrder))
            {
                entities = entities.OrderByDescending(x => x.InsertedAt);
            }
            else
            {
                entities = entities.OrderBy(x => x.InsertedAt);
            }

            var page = entities.Select(
                    e => new TicketSearchMobileModel()
                    {
                        ID = e.ID,
                        TicketType = new ValueLabelModel { Value = e.TicketType, Label = e.TicketType == 1 ? "Kế hoạch" : "Đột xuất" },
                        TicketName = e.TicketName,
                        TicketNumber = e.TicketNumber,
                        ApprovedDate = e.CreatedDate,
                        CreatedDate = e.CreatedDate,
                        TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                    });

            IPagedList<TicketSearchMobileModel> pagelist = page.ToPagedList(requestModel.PageNumber, requestModel.PageSize);
            // Kiểm tra quyền edit
            if (pagelist != null && pagelist.TotalRecords > 0)
            {
                foreach (var entity in pagelist)
                {
                    entity.ApprovedUser = GetUserInTicket(entity.ID, (int)UserInTicketRoles.Approval);
                    entity.CreatedUser = GetUserInTicket(entity.ID, (int)UserInTicketRoles.Creator);
                }
                pagelist.GetEnumerator().Reset();
            }

            return HttpResponse.OkList(pagelist);
        }

        /// <summary>
        /// Tìm kiếm một Phiếu thao tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TicketDetailModel GetTicketById(int id)
        {
            var data = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.ID == id && !x.IsDeleted);
            if (data != null)
            {
                var entity = data.Select(e => new TicketDetailModel()
                {
                    ID = e.ID,
                    TicketType = new ValueLabelModel { Value = e.TicketType, Label = e.TicketType == 1 ? "Kế hoạch" : "Đột xuất" },
                    TicketName = e.TicketName,
                    TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                    TicketNumber = e.TicketNumber,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    DTCreatedDate = e.CreatedDate,
                    DTApprovedDate = e.CreatedDate,
                    Company = e.ApproveUnit.Parent != null ? new ValueLabelModel { Value = e.ApproveUnit.Parent.ID, Label = e.ApproveUnit.Parent.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                    ApprovedUnit = new ValueLabelModel { Value = e.ApproveUnit.ID, Label = e.ApproveUnit.Name },
                    Purpose = e.Purpose,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    RecommendedUnit = e.RecommendedUnit,
                    CaseAbnormal = e.CaseAbnormal,
                    DTWorkEndTime = e.WorkEndTime,
                    DTCompletedTime = e.CompletedTime,
                    ReferAttachmentIDs = e.ReferAttachmentIDs,
                    MapAttachmentIDs = e.MapAttachmentIDs,
                }).FirstOrDefault();

                if (entity != null)
                {
                    // Những người tham gia vào phiếu thao tác
                    var entitiesParticipant = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x => x.TicketID == id && !x.IsCreated && !x.IsDeleted).ToList();
                    if (entitiesParticipant.Count > 0)
                    {
                        entity.Participants = new List<ParticipantModel>();
                        entity.Workers = new List<ParticipantModel>();
                        entity.Suppervisors = new List<ParticipantModel>();
                        foreach (var e in entitiesParticipant)
                        {
                            ParticipantModel userInfo = new ParticipantModel()
                            {
                                ID = e.ID,
                                TicketID = e.TicketID.GetValueOrDefault(0),
                                IsCreated = e.IsCreated,
                                UserName = e.UserName,
                                FullName = e.FullName,
                                PositionName = e.PositionName,
                                Roles = e.Roles,
                                ElectricRankStr = e.ElectricRank,
                                ElectricRankCard = e.ElectricRankCard,
                                SignatureID = e.SignatureID,
                                SignatureContent = e.SignatureID != null ? _commonService.GetSignatureById(e.SignatureID.GetValueOrDefault(0)).SignatureContent : null,
                                IsExternal = e.IsExternal,
                                Order = e.Order,
                            };
                            entity.Participants.Add(userInfo);

                            if (userInfo.Roles == (int)UserInTicketRoles.Worker)
                            {
                                entity.Workers.Add(userInfo);
                            }
                            else if (userInfo.Roles == (int)UserInTicketRoles.Supervisor)
                            {
                                entity.Suppervisors.Add(userInfo);
                            }
                        }
                    }

                    //5. Điều kiện cần có thể thực hiện và 6. Lưu ý
                    var entitiesOtherRequest = _unitOfWork.ElogbookRepository<OtherRequest>().GetQuery(x => x.TicketID == id && !x.IsCreated && !x.IsDeleted);
                    if (entitiesOtherRequest != null)
                    {
                        entity.OtherRequests = entitiesOtherRequest.Select(e => new OtherRequestModel()
                        {
                            ID = e.ID,
                            IsCreated = e.IsCreated,
                            Description = e.Description,
                            Type = e.Type,
                        }).ToList();
                    }

                    //Danh sách tài liệu đính kèm
                    if (!string.IsNullOrWhiteSpace(entity.ReferAttachmentIDs))
                    {
                        entity.ReferAttachments = GetAttachedFile(entity.ReferAttachmentIDs);
                    }

                    //Giao nhận, nghiệm thu đường dây, thiết bị điện trước và sau khi thao tác
                    var entitiesDeliveryAccepDevices = _unitOfWork.ElogbookRepository<DeliveryAccepDevice>().GetQuery(x => x.TicketID == id && !x.IsDeleted);
                    if (entitiesDeliveryAccepDevices != null)
                    {
                        entity.DeliveryAccepDevices = entitiesDeliveryAccepDevices.Select(e => new DeliveryAccepDeviceModel()
                        {
                            ID = e.ID,
                            IsBeforeDelivery = e.IsBeforeDelivery,
                            DTDeliveryTime = e.DeliveryTime,
                            Unit = e.Unit,
                            FullName = e.FullName,
                            Content = e.Content
                        }).ToList();
                    }

                    //Trình tự hạng mục thao tác
                    var entitiesSequenceOperation = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => x.TicketID == id && !x.IsCreated && !x.IsDeleted);
                    if (entitiesSequenceOperation != null)
                    {
                        entity.SequenceOperations = entitiesSequenceOperation.Select(e => new SequenceOperationDetailModel()
                        {
                            ID = e.ID,
                            TicketID = e.TicketID != null ? e.TicketID.Value : 0,
                            IsCreated = e.IsCreated,
                            TableOfContents = e.TableOfContents,
                            Location = e.Location,
                            Step = e.Step,
                            Content = e.Content,
                            IsDone = e.IsDone,
                            ParentStep = e.ParentStep,
                            BeginTime = e.BeginTime != null ? e.BeginTime : null,
                            EndTime = e.EndTime != null ? e.EndTime : null,
                            LeaderUser = new ValueLabelModel { Value = e.LeaderUser.ID, Label = e.LeaderUser.FullName },
                            WorkerUser = new ValueLabelModel { Value = e.WorkerUser.ID, Label = e.WorkerUser.FullName },
                        }).OrderBy(x => x.Step).ToList();
                    }

                    //Danh sách sơ đồ
                    if (!string.IsNullOrWhiteSpace(entity.MapAttachmentIDs))
                    {
                        entity.MapAttachments = GetAttachedFile(entity.MapAttachmentIDs);
                    }

                    // Thông tin hủy phiếu
                    var entitiesCancelTicketInfo = _unitOfWork.ElogbookRepository<CancelTicketInfo>().GetQuery(x => x.TicketID == id && !x.IsDeleted).FirstOrDefault();
                    if (entitiesCancelTicketInfo != null)
                    {
                        CancelTicketInfoModel cancelTicketInfoModel = new CancelTicketInfoModel()
                        {
                            ID = entitiesCancelTicketInfo.ID,
                            Reason = entitiesCancelTicketInfo.Reason,
                            CancelTime = entitiesCancelTicketInfo.UpdatedAt.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM),
                            ReferAttachments = !string.IsNullOrEmpty(entitiesCancelTicketInfo.ReferAttachmentIDs) ? GetAttachedFile(entitiesCancelTicketInfo.ReferAttachmentIDs) : null,
                            InsertedAt = entitiesCancelTicketInfo.InsertedAt,
                            UpdatedAt = entitiesCancelTicketInfo.UpdatedAt,
                            AppID = entitiesCancelTicketInfo.AppID
                        };

                        entity.CancelTicketInfo = cancelTicketInfoModel;
                    }
                    entity.ApprovedUser = GetUserInTicket(entity.ID, (int)UserInTicketRoles.Approval);
                    entity.CreatorUser = GetUserInTicket(entity.ID, (int)UserInTicketRoles.Creator);
                }

                return entity;
            }

            return null;
        }

        public bool DeleteFinalTicket(int id, int currentUserID)
        {
            var manipulationTicket = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (manipulationTicket != null)
            {
                // Xóa lưu dữ liệu Người giám sát, Người thao tác
                var participants = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x => x.TicketID == manipulationTicket.ID && !x.IsDeleted);
                if (participants != null)
                {
                    _unitOfWork.ElogbookRepository<Participant>().Delete(participants, currentUserID);
                }

                // Xóa lưu dữ liệu 1/Điều kiện cần có thể thực hiện, 2/Lưu ý
                var otherRequests = _unitOfWork.ElogbookRepository<OtherRequest>().GetQuery(x => x.TicketID == manipulationTicket.ID && !x.IsDeleted);
                if (otherRequests != null)
                {
                    _unitOfWork.ElogbookRepository<OtherRequest>().Delete(otherRequests, currentUserID);
                }

                // Xóa lưu dữ liệu trình tự hạng mục thao tác
                var sequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => x.TicketID == manipulationTicket.ID && !x.IsDeleted);
                if (sequenceOperations != null)
                {
                    _unitOfWork.ElogbookRepository<SequenceOperation>().Delete(sequenceOperations, currentUserID);
                }

                var registID = manipulationTicket.RegistrationManipulationTicketID;
                var registrationManipulationTicket = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetById((int)registID);
                if (registrationManipulationTicket != null)
                {
                    // Xóa lưu dữ liệu lịch sử
                    var ticketStatusHistory = _unitOfWork.ElogbookRepository<TicketStatusHistory>().GetQuery(x => x.RegistrationManipulationID == registrationManipulationTicket.ID && !x.IsDeleted);

                    if (ticketStatusHistory != null)
                    {
                        _unitOfWork.ElogbookRepository<TicketStatusHistory>().Delete(ticketStatusHistory, currentUserID);
                    }

                    // Xóa phiếu đăng ký
                    _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().Delete(registrationManipulationTicket, currentUserID);
                }

                // Xóa phiếu thao tác
                _unitOfWork.ElogbookRepository<ManipulationTicket>().Delete(manipulationTicket, currentUserID);
                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        /// <summary>
        /// Thêm thông tin thao tác hoàn thành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool Update(UpdateSequenceOperationModel requestModel)
        {
            var data = _unitOfWork.ElogbookRepository<SequenceOperation>()
                                            .GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).First();

            if (requestModel.LeaderUser != null)
            {
                data.LeaderID = CreateSequenceOperationParticipant(requestModel.LeaderUser.UserName,
                        requestModel.LeaderUser.IsExternal, requestModel.CurrentUserID);
            }

            if (requestModel.WorkerUser != null)
            {
                data.WorkerID = CreateSequenceOperationParticipant(requestModel.WorkerUser.UserName,
                        requestModel.WorkerUser.IsExternal, requestModel.CurrentUserID);
            }
            data.BeginTime = requestModel.BeginDate;
            data.EndTime = requestModel.EndDate;
            data.IsDone = requestModel.IsDone;
            data.ParentStep = requestModel.ParentStep;
            data.LastUpdatedUserID = requestModel.CurrentUserID;

            _unitOfWork.ElogbookRepository<SequenceOperation>().Update(data);
            return _unitOfWork.Complete() > 0;
        }

        /// <summary>
        /// Hủy phiếu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool Cancel(CancelTicketModel requestModel)
        {
            var data = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.ID == requestModel.IdTicket && !x.IsDeleted)
                    .FirstOrDefault();
            if (data != null)
            {
                data.TicketStatusValue = (int)Core.Enums.TicketManipulationStatus.Cancel;
                data.LastUpdatedUserID = requestModel.CurrentUserID;
                _unitOfWork.ElogbookRepository<ManipulationTicket>().Update(data);
            }

            var cancelTicketInfo = new CancelTicketInfo()
            {
                Reason = requestModel.Reason,
                TicketID = requestModel.IdTicket,
                CreatedUserID = requestModel.CurrentUserID,
                ReferAttachmentIDs = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments) : null,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<CancelTicketInfo>().Add(cancelTicketInfo);

            return _unitOfWork.Complete() > 0;
        }

        /// <summary>
        /// Kết thúc phiếu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool CloseTicket(CloseTicketModel requestModel)
        {
            var data = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.ID == requestModel.IdTicket && !x.IsDeleted).FirstOrDefault();
            if (data != null)
            {
                data.TicketStatusValue = (int)Core.Enums.TicketManipulationStatus.WaitForConfirmation;
                data.LastUpdatedUserID = requestModel.CurrentUserID;
                data.WorkEndTime = DateTime.Now;
                data.CaseAbnormal = requestModel.Notes;

                if (requestModel.Supervisors != null && requestModel.Supervisors.Count > 0)
                {
                    foreach (var supervisor in requestModel.Supervisors)
                    {
                        var signatureModel = new SignatureModel()
                        {
                            SignatureContent = supervisor.Signature != null ? supervisor.Signature.SignatureContent : string.Empty,
                            Type = supervisor.Signature != null ? supervisor.Signature.Type : 0,
                            CurrentUserID = requestModel.CurrentUserID
                        };
                        AddSignature(requestModel.IdTicket, (int)UserInTicketRoles.Supervisor, supervisor.UserName, requestModel.CurrentUserID, signatureModel);
                    }
                }

                if (requestModel.Workers != null && requestModel.Workers.Count > 0)
                {
                    foreach (var worker in requestModel.Workers)
                    {
                        var signatureModel = new SignatureModel()
                        {
                            SignatureContent = worker.Signature != null ? worker.Signature.SignatureContent : string.Empty,
                            Type = worker.Signature != null ? worker.Signature.Type : 0,
                            CurrentUserID = requestModel.CurrentUserID
                        };
                        AddSignature(requestModel.IdTicket, (int)UserInTicketRoles.Worker, worker.UserName, requestModel.CurrentUserID, signatureModel);
                    }
                }

                _unitOfWork.ElogbookRepository<ManipulationTicket>().Update(data);

                return _unitOfWork.Complete() > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Trả về tất cả phiếu thao tác trên Web theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindManipulationWeb(SearchTicketWebModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var entities = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => !x.IsDeleted);

            // Tìm kiếm theo nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY && requestModel.FactoryID > 0)
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
            var page = entities.Select(
                    e => new TicketSearchWebModel()
                    {
                        ID = e.ID,
                        TicketNumber = e.TicketNumber,
                        TicketName = e.TicketName,
                        Factory = e.FactoryID.HasValue ? new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName } : null,
                        TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                        TicketTypeValue = e.TicketType,
                        Company = e.ApproveUnit.Parent != null ? new ValueLabelModel { Value = e.ApproveUnit.Parent.ID, Label = e.ApproveUnit.Parent.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                        ApproveUnit = e.ApproveUnitID.HasValue ? new ValueLabelModel { Value = e.ApproveUnit.ID, Label = e.ApproveUnit.Name } : null,
                        CreatedDate = e.InsertedAt,
                    }).OrderByDescending(t => t.CreatedDate)
                    .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Xác nhận hoàn thành phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> CompleteManipulationWeb(CompleteManipulationModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            var data = _unitOfWork.ElogbookRepository<ManipulationTicket>()
                .GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();

            if (data != null)
            {
                data.TicketStatusValue = (int)Core.Enums.TicketManipulationStatus.Completed;
                data.LastUpdatedUserID = requestModel.CurrentUserID;
                data.MapAttachmentIDs = (requestModel.MapAttachmentIDs != null) ? string.Join(",", requestModel.MapAttachmentIDs.ToArray()) : null;
                data.CompletedTime = DateTime.Now;
                data.UpdatedAt = DateTime.Now;
                if (requestModel.DeliveryAccepDevices != null || requestModel.DeliveryAccepDevices.Count > 0)
                {
                    var deliveryAccepDevices = requestModel.DeliveryAccepDevices.Select(e => new DeliveryAccepDevice()
                    {
                        TicketID = data.ID,
                        IsBeforeDelivery = false,
                        DeliveryTime = DateTimeConvert.ConvertDateAndTimeFromString(e.DeliveryTime),
                        Unit = !string.IsNullOrEmpty(e.Unit) ? e.Unit : null,
                        FullName = !string.IsNullOrEmpty(e.FullName) ? e.FullName : null,
                        Content = !string.IsNullOrEmpty(e.Content) ? e.Content : null,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        CreatedUserID = requestModel.CurrentUserID,
                    }).ToList();

                    _unitOfWork.ElogbookRepository<DeliveryAccepDevice>().AddRange(deliveryAccepDevices);
                }

                if (requestModel.SignerInfo != null)
                {
                    var user = requestModel.SignerInfo;
                    var signatureModel = new SignatureModel()
                    {
                        SignatureContent = user.Signature.SignatureContent,
                        Type = user.Signature.Type,
                        CurrentUserID = requestModel.CurrentUserID
                    };
                    AddSignature(requestModel.ID, (int)UserInTicketRoles.Approval, user.UserName, requestModel.CurrentUserID, signatureModel);
                }
                _unitOfWork.ElogbookRepository<ManipulationTicket>().Update(data);

                var success = _unitOfWork.Complete() > 0;
                if (success)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    responseModel.Data = true;
                    responseModel.ErrorMessages.Add("Xác nhận hoàn thành phiếu thành công");
                    return responseModel;
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.ErrorMessages.Add("Xác nhận hoàn thành phiếu thất bại");
                    return responseModel;
                }
            }

            responseModel.Data = false;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            responseModel.ErrorMessages.Add("Xác nhận hoàn thành phiếu thất bại");

            return responseModel;
        }

        /// <summary>
        /// Trả về trình tự thao tác
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<SequenceOperationModel> GetSequenceOperation(int ID)
        {
            return _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => !x.IsDeleted && x.TicketID == ID)
                 .Select(t => new SequenceOperationModel()
                 {
                     ID = t.ID,
                     TicketID = t.TicketID.Value,
                     IsCreated = t.IsCreated,
                     TableOfContents = t.TableOfContents,
                     Location = t.Location,
                     Step = t.Step,
                     Content = t.Content,
                     BeginTime = t.BeginTime,
                     EndTime = t.EndTime,
                     LeaderUser = t.LeaderUser != null ? new UserInTicketModel
                     {
                         ID = t.LeaderUser.ID,
                         TicketID = t.LeaderUser.TicketID,
                         UserName = t.LeaderUser.UserName,
                         IsExternal = t.LeaderUser.IsExternal ?? false,
                         FullName = t.LeaderUser.FullName,
                         Roles = t.LeaderUser.Roles,
                         ElectricRankStr = t.LeaderUser.ElectricRank,
                         ElectricRankCard = t.LeaderUser.ElectricRankCard,
                         PositionName = t.LeaderUser.PositionName
                     } : null,
                     WorkerUser = t.WorkerUser != null ? new UserInTicketModel
                     {
                         ID = t.WorkerUser.ID,
                         TicketID = t.WorkerUser.TicketID,
                         UserName = t.WorkerUser.UserName,
                         IsExternal = t.WorkerUser.IsExternal ?? false,
                         FullName = t.WorkerUser.FullName,
                         Roles = t.WorkerUser.Roles,
                         ElectricRankStr = t.WorkerUser.ElectricRank,
                         ElectricRankCard = t.WorkerUser.ElectricRankCard,
                         PositionName = t.WorkerUser.PositionName
                     } : null,
                     IsDone = t.IsDone,
                     ParentStep = t.ParentStep
                 }).OrderBy(x => x.Step).ToList();
        }

        /// <summary>
        /// Trả về danh sách users
        /// </summary>
        /// <param name="Role"></param>
        /// <param name="TicketID"></param>
        /// <returns></returns>
        public List<UserInTicketModel> GetUsersInTicket(int TicketID, int Role)
        {
            var participants = _unitOfWork.ElogbookRepository<Participant>()
                    .GetQuery(x => !x.IsDeleted && x.TicketID == TicketID && x.Roles == Role)
                    .Select(e => new UserInTicketModel()
                    {
                        ID = e.ID,
                        TicketID = e.TicketID != null ? e.TicketID.Value : 0,
                        UserName = e.UserName,
                        IsExternal = e.IsExternal ?? false,
                        FullName = e.FullName,
                        Roles = e.Roles,
                        ElectricRankStr = e.ElectricRank,
                        ElectricRankCard = e.ElectricRankCard,
                        SignatureID = e.SignatureID,
                        PositionName = e.PositionName
                    }).ToList();

            // Lấy chữ ký từ table Signature
            if (participants.Count > 0)
            {
                foreach (var item in participants)
                {
                    if (item.SignatureID != null)
                    {
                        var signtureInfo = _commonService.GetSignatureById((int)item.SignatureID);

                        if (signtureInfo != null)
                        {
                            if (signtureInfo.SignatureContent != null)
                            {
                                item.SignatureContent = signtureInfo.SignatureContent;
                                continue;
                            }

                            if (signtureInfo.SignatureContentByte != null)
                            {
                                item.SignatureContent = Encoding.UTF8.GetString(signtureInfo.SignatureContentByte);
                            }
                        }
                    }
                }
            }

            return participants;
        }

        /// <summary>
        /// In file PDF
        /// </summary>
        public HttpResponseMessage ExportPdfFile(int id)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            var ticketDetail = GetTicketById(id);

            if (ticketDetail == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // tạo thư mục lưu file pdf
            if (!Directory.Exists(Path.Combine(_rootPath, "ManipulationTicket", "PDF")))
            {
                Directory.CreateDirectory(Path.Combine(_rootPath, "ManipulationTicket", "PDF"));
            }
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = $"ManipulationTicket_{currentDate}.pdf";
            string outputFile = Path.Combine(_rootPath, "ManipulationTicket", "PDF", fileName);

            bool isCreatePDF = GeneratePdfFile(ticketDetail, outputFile);
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

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel SyncData(ManipulationTicketSyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            ManipulationTicketSyncDataResultModel result = new ManipulationTicketSyncDataResultModel();
            List<int> lstManipulationTicket = new List<int>();
            if (requestModel.ManipulationTicketSyncModels != null && requestModel.ManipulationTicketSyncModels.Count > 0)
            {
                using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    foreach (var model in requestModel.ManipulationTicketSyncModels)
                    {
                        try
                        {
                            if (model.IsNew)
                            {
                                var ticket = new ManipulationTicket()
                                {
                                    TicketType = model.TicketType != null ? model.TicketType.Value.Value : 0,
                                    RegistrationManipulationTicketID = model.RegistrationManipulationTicketID,
                                    FactoryID = model.FactoryID,
                                    CreatedDate = model.CreatedDate,
                                    ApproveUnitID = model.ApproveUnitID,
                                    ApproveUnitView = model.ApproveUnitView,
                                    TicketName = model.TicketName,
                                    TicketNumber = model.TicketNumber,
                                    CreatedUserTicket = model.CreatedUserTicket,
                                    ApprovedUserTicket = model.ApprovedUserTicket,
                                    Purpose = model.Purpose,
                                    StartTime = model.StartTime,
                                    EndTime = model.EndTime,
                                    RecommendedUnit = model.RecommendedUnit,
                                    ReferAttachmentIDs = model.ReferAttachmentIDs,
                                    TicketStatusValue = model.TicketStatusValue,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    SyncedDate = DateTime.Now
                                };

                                _unitOfWork.ElogbookRepository<ManipulationTicket>().Add(ticket);

                                var saveSuccess = _unitOfWork.Complete() > 0;
                                if (saveSuccess)
                                {
                                    lstManipulationTicket.Add(ticket.ID);
                                    // người giám sát và người thao tác
                                    List<Participant> dataParticipant = new List<Participant>();
                                    if (model.Participants != null && model.Participants.Count > 0)
                                    {
                                        foreach (var item in model.Participants)
                                        {
                                            var userInfo = _commonService.GetUserById(item.UserName, false);
                                            var participant = new Participant()
                                            {
                                                TicketID = ticket.ID,
                                                IsCreated = false,
                                                UserName = userInfo.UserName,
                                                FullName = userInfo.FullName,
                                                PositionName = userInfo.PositionName,
                                                Roles = item.Roles,
                                                ElectricRank = userInfo.ElectricRankStr,
                                                ElectricRankCard = userInfo.ElectricRankCard,
                                                IsExternal = userInfo.IsExternal,
                                                Order = item.Order,
                                                CreatedUserID = requestModel.CurrentUserID,
                                                LastUpdatedUserID = requestModel.CurrentUserID,
                                                SyncedDate = DateTime.Now
                                            };

                                            dataParticipant.Add(participant);
                                            _unitOfWork.ElogbookRepository<Participant>().Add(participant);
                                        }
                                    }

                                    //5.Điều kiện cần có thể thực hiện và 6.Lưu ý
                                    if (model.OtherRequests != null && model.OtherRequests.Count > 0)
                                    {
                                        foreach (var item in model.OtherRequests)
                                        {
                                            var otherRequest = new OtherRequest()
                                            {
                                                TicketID = ticket.ID,
                                                IsCreated = false,
                                                Description = item.Description,
                                                Type = item.Type,
                                                SyncedDate = DateTime.Now
                                            };

                                            _unitOfWork.ElogbookRepository<OtherRequest>().Add(otherRequest);
                                        }
                                    }

                                    // Trình tự hạng mục thao tác
                                    if (model.SequenceOperations != null && model.SequenceOperations.Count > 0)
                                    {
                                        foreach (var item in model.SequenceOperations)
                                        {
                                            var sequenceOperation = new SequenceOperation()
                                            {
                                                TicketID = ticket.ID,
                                                IsCreated = false,
                                                TableOfContents = item.TableOfContents,
                                                Location = item.Location,
                                                Step = item.Step,
                                                Content = item.Content,
                                                CreatedUserID = requestModel.CurrentUserID,
                                                LastUpdatedUserID = requestModel.CurrentUserID,
                                                BeginTime = item.BeginTime,
                                                EndTime = item.EndTime,
                                                IsDone = false,
                                                ParentStep = item.ParentStep,
                                                SyncedDate = DateTime.Now
                                            };

                                            if (item.LeaderUser != null)
                                            {
                                                sequenceOperation.LeaderID = CreateSequenceOperationParticipant(item.LeaderUser.UserName,
                                                        item.LeaderUser.IsExternal, requestModel.CurrentUserID);
                                            }

                                            if (item.WorkerUser != null)
                                            {
                                                sequenceOperation.WorkerID = CreateSequenceOperationParticipant(item.WorkerUser.UserName,
                                                        item.WorkerUser.IsExternal, requestModel.CurrentUserID);
                                            }

                                            _unitOfWork.ElogbookRepository<SequenceOperation>().Add(sequenceOperation);
                                        }
                                    }

                                    //Danh sách giao nhận nghiệm thu đường dây, thiết bị điện trước khi thao tác
                                    if (model.DeliveryAccepDeviceModels != null && model.DeliveryAccepDeviceModels.Count > 0)
                                    {
                                        foreach (var item in model.DeliveryAccepDeviceModels)
                                        {
                                            var deliveryAccepDeviceModels = new DeliveryAccepDevice()
                                            {
                                                TicketID = ticket.ID,
                                                DeliveryTime = DateTimeConvert.ConvertDateAndTimeFromString(item.DeliveryTime),
                                                Unit = item.Unit,
                                                FullName = item.FullName,
                                                Content = item.Content,
                                                IsBeforeDelivery = true,
                                                CreatedUserID = requestModel.CurrentUserID,
                                                LastUpdatedUserID = requestModel.CurrentUserID,
                                                SyncedDate = DateTime.Now
                                            };

                                            _unitOfWork.ElogbookRepository<DeliveryAccepDevice>().Add(deliveryAccepDeviceModels);
                                        }
                                    }

                                    if (model.CancelTicketSyncModels != null && model.CancelTicketSyncModels.Count > 0)
                                    {
                                        foreach (var item in model.CancelTicketSyncModels)
                                        {
                                            if (item.ID <= 0)
                                            {
                                                var cancelTicketInfo = new CancelTicketInfo()
                                                {
                                                    AppID = item.AppID,
                                                    TicketID = item.TicketID,
                                                    Reason = item.Reason,
                                                    ReferAttachmentIDs = item.ReferAttachmentIDs,
                                                    CreatedUserID = requestModel.CurrentUserID,
                                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                                    SyncedDate = DateTime.Now
                                                };

                                                _unitOfWork.ElogbookRepository<CancelTicketInfo>().Add(cancelTicketInfo);
                                            }

                                        }
                                    }

                                    _unitOfWork.Complete();

                                    // update Partician
                                    if (dataParticipant.Count > 0)
                                    {
                                        foreach (var item in dataParticipant)
                                        {
                                            if (item.Roles == 1 || item.Roles == 2)
                                            {
                                                var userInfo = _commonService.GetUserById(item.UserName, false);
                                                var signatureModel = new SignatureModel()
                                                {
                                                    CurrentUserID = requestModel.CurrentUserID
                                                };
                                                if (!string.IsNullOrEmpty(userInfo.Signature))
                                                {
                                                    signatureModel.Type = userInfo.SourceType;
                                                    signatureModel.SignatureContent = userInfo.Signature;
                                                }

                                                var signatureInfo = _commonService.CreateSignature(signatureModel);
                                                item.SignatureID = signatureInfo.ID;
                                                item.SyncedDate = DateTime.Now;
                                            }
                                        }

                                        _unitOfWork.ElogbookRepository<Participant>().Update(dataParticipant);
                                        _unitOfWork.Complete();
                                    }
                                }
                            }
                            else
                            {
                                lstManipulationTicket.Add(model.ID);
                                if (model.SequenceOperations != null && model.SequenceOperations.Count > 0)
                                {
                                    foreach (var item in model.SequenceOperations)
                                    {
                                        if (item.ID == 0)
                                        {
                                            var data = new SequenceOperation();
                                            if (item.LeaderUser != null)
                                            {
                                                data.LeaderID = CreateSequenceOperationParticipant(item.LeaderUser.UserName,
                                                        item.LeaderUser.IsExternal, requestModel.CurrentUserID);
                                            }

                                            if (item.WorkerUser != null)
                                            {
                                                data.WorkerID = CreateSequenceOperationParticipant(item.WorkerUser.UserName,
                                                        item.WorkerUser.IsExternal, requestModel.CurrentUserID);
                                            }
                                            data.BeginTime = item.BeginTime;
                                            data.EndTime = item.EndTime;
                                            data.IsDone = item.IsDone;
                                            data.ParentStep = item.ParentStep;
                                            data.LastUpdatedUserID = requestModel.CurrentUserID;
                                            data.SyncedDate = DateTime.Now;
                                            _unitOfWork.ElogbookRepository<SequenceOperation>().Add(data);
                                        }
                                        else
                                        {
                                            var data = _unitOfWork.ElogbookRepository<SequenceOperation>()
                                               .GetQuery(x => x.ID == item.ID && !x.IsDeleted).First();

                                            if (item.LeaderUser != null)
                                            {
                                                data.LeaderID = CreateSequenceOperationParticipant(item.LeaderUser.UserName,
                                                        item.LeaderUser.IsExternal, requestModel.CurrentUserID);
                                            }

                                            if (item.WorkerUser != null)
                                            {
                                                data.WorkerID = CreateSequenceOperationParticipant(item.WorkerUser.UserName,
                                                        item.WorkerUser.IsExternal, requestModel.CurrentUserID);
                                            }
                                            data.BeginTime = item.BeginTime;
                                            data.EndTime = item.EndTime;
                                            data.IsDone = item.IsDone;
                                            data.ParentStep = item.ParentStep;
                                            data.LastUpdatedUserID = requestModel.CurrentUserID;
                                            data.SyncedDate = DateTime.Now;
                                            _unitOfWork.ElogbookRepository<SequenceOperation>().Update(data);
                                        }
                                    }
                                }

                                if (model.OtherRequests != null && model.OtherRequests.Count > 0)
                                {
                                    foreach (var item in model.OtherRequests)
                                    {
                                        if (item.ID > 0)
                                        {
                                            var otherRequest = new OtherRequest()
                                            {
                                                TicketID = model.ID,
                                                IsCreated = false,
                                                Description = item.Description,
                                                Type = item.Type,
                                                SyncedDate = DateTime.Now
                                            };

                                            _unitOfWork.ElogbookRepository<OtherRequest>().Add(otherRequest);
                                        }
                                        else
                                        {
                                            var data = _unitOfWork.ElogbookRepository<OtherRequest>()
                                               .GetQuery(x => x.ID == item.ID && !x.IsDeleted).First();

                                            data.Description = item.Description;
                                            data.SyncedDate = DateTime.Now;
                                            _unitOfWork.ElogbookRepository<OtherRequest>().Update(data);
                                        }
                                    }
                                }

                                //Danh sách giao nhận nghiệm thu đường dây, thiết bị điện trước khi thao tác
                                if (model.DeliveryAccepDeviceModels != null && model.DeliveryAccepDeviceModels.Count > 0)
                                {
                                    foreach (var item in model.DeliveryAccepDeviceModels)
                                    {
                                        if (item.ID > 0)
                                        {
                                            var deliveryAccepDeviceModels = new DeliveryAccepDevice()
                                            {
                                                TicketID = model.ID,
                                                DeliveryTime = DateTimeConvert.ConvertDateAndTimeFromString(item.DeliveryTime),
                                                Unit = item.Unit,
                                                FullName = item.FullName,
                                                Content = item.Content,
                                                IsBeforeDelivery = true,
                                                CreatedUserID = requestModel.CurrentUserID,
                                                LastUpdatedUserID = requestModel.CurrentUserID,
                                                SyncedDate = DateTime.Now
                                            };

                                            _unitOfWork.ElogbookRepository<DeliveryAccepDevice>().Add(deliveryAccepDeviceModels);
                                        }
                                        else
                                        {
                                            var data = _unitOfWork.ElogbookRepository<DeliveryAccepDevice>()
                                               .GetQuery(x => x.ID == item.ID && !x.IsDeleted).First();

                                            data.DeliveryTime = DateTimeConvert.ConvertDateAndTimeFromString(item.DeliveryTime);
                                            data.Unit = item.Unit;
                                            data.FullName = item.FullName;
                                            data.Content = item.Content;
                                            data.IsBeforeDelivery = true;
                                            data.SyncedDate = DateTime.Now;
                                            _unitOfWork.ElogbookRepository<DeliveryAccepDevice>().Update(data);
                                        }
                                    }
                                }

                                if (model.CancelTicketSyncModels != null && model.CancelTicketSyncModels.Count > 0)
                                {
                                    foreach (var item in model.CancelTicketSyncModels)
                                    {
                                        if (item.ID == 0)
                                        {
                                            var cancelTicketInfo = new CancelTicketInfo()
                                            {
                                                AppID = item.AppID,
                                                TicketID = item.TicketID,
                                                Reason = item.Reason,
                                                ReferAttachmentIDs = item.ReferAttachmentIDs,
                                                CreatedUserID = requestModel.CurrentUserID,
                                                LastUpdatedUserID = requestModel.CurrentUserID,
                                                SyncedDate = DateTime.Now
                                            };
                                            _unitOfWork.ElogbookRepository<CancelTicketInfo>().Add(cancelTicketInfo);
                                        }
                                        else
                                        {
                                            var data = _unitOfWork.ElogbookRepository<CancelTicketInfo>()
                                               .GetQuery(x => x.ID == item.ID && !x.IsDeleted).First();
                                            data.AppID = item.AppID;
                                            data.Reason = item.Reason;
                                            data.ReferAttachmentIDs = item.ReferAttachmentIDs;
                                            data.SyncedDate = DateTime.Now;
                                            _unitOfWork.ElogbookRepository<CancelTicketInfo>().Update(data);
                                        }
                                    }
                                }

                                if (model.Suppervisors != null && model.Suppervisors.Count > 0)
                                {
                                    foreach (var supervisor in model.Suppervisors)
                                    {
                                        var signatureModel = new SignatureModel()
                                        {
                                            SignatureContent = supervisor.Signature != null ? supervisor.Signature.SignatureContent : string.Empty,
                                            Type = supervisor.Signature != null ? supervisor.Signature.Type : 0,
                                            CurrentUserID = requestModel.CurrentUserID
                                        };
                                        AddSignature(model.ID, (int)UserInTicketRoles.Supervisor, supervisor.UserName, requestModel.CurrentUserID, signatureModel);
                                    }
                                }

                                if (model.Workers != null && model.Workers.Count > 0)
                                {
                                    foreach (var worker in model.Workers)
                                    {
                                        var signatureModel = new SignatureModel()
                                        {
                                            SignatureContent = worker.Signature != null ? worker.Signature.SignatureContent : string.Empty,
                                            Type = worker.Signature != null ? worker.Signature.Type : 0,
                                            CurrentUserID = model.CurrentUserID
                                        };
                                        AddSignature(model.ID, (int)UserInTicketRoles.Worker, worker.UserName, requestModel.CurrentUserID, signatureModel);
                                    }
                                }


                                var manipulationTicket = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.ID == model.ID && !x.IsDeleted).FirstOrDefault();
                                if (manipulationTicket != null)
                                {
                                    manipulationTicket.TicketStatusValue = model.TicketStatusValue;
                                    manipulationTicket.LastUpdatedUserID = requestModel.CurrentUserID;
                                    manipulationTicket.SyncedDate = DateTime.Now;
                                    _unitOfWork.ElogbookRepository<ManipulationTicket>().Update(manipulationTicket);
                                }
                                _unitOfWork.Complete();
                                transaction.Commit();
                            }
                        }
                        catch
                        {
                            responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                            responseModel.Content = false;
                            responseModel.Message = "Đồng bộ dữ liệu thất bại, Vui lòng liên hệ Admin";
                            transaction.Rollback();
                        }
                    }
                }
            }

            if (lstManipulationTicket.Count > 0)
            {
                var lstData = new List<TicketDetailModel>();
                foreach (var id in lstManipulationTicket)
                {
                    lstData.Add(GetTicketById(id));
                }
                result.TicketDetailModels = lstData;
                responseModel.Content = result;
            }

            return responseModel;
        }

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

        #endregion

        #region private

        private bool GeneratePdfFile(TicketDetailModel ticketDetail, string outputFile)
        {
            try
            {
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    //Create document
                    Document pdfDocument = new Document(PageSize.A4);
                    pdfDocument.SetMargins(5, 5, 25, 5);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, fileStream);

                    var companyName = string.Empty;
                    var organization = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == ticketDetail.Company.Value).FirstOrDefault();
                    if (organization != null)
                    {
                        companyName = organization.Name;
                    }

                    var factoryName = string.Empty;
                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == ticketDetail.Factory.Value).FirstOrDefault();
                    if (factory != null)
                    {
                        factoryName = factory.FactoryName;
                    }

                    HeaderFooterManipulation tevent = new HeaderFooterManipulation(ticketDetail, companyName, factory.ProductionPlant?.Name);
                    pdfWriter.PageEvent = tevent;
                    // Open the document to enable you to write to the document
                    pdfDocument.Open();
                    try
                    {
                        PdfPTable mainTable = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
                        mainTable.DefaultCell.Border = Rectangle.NO_BORDER;
                        mainTable.SetWidths(new float[] { 2, 2, 2 });
                        mainTable.LockedWidth = true;
                        mainTable.HeaderRows = 2;
                        mainTable.AddCell(GetCellSign(_fBold, 16, "", 3, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        mainTable.AddCell(GetCellSign(_fBold, 16, "", 3, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        mainTable.AddCell(GetCellSign(_fBold, 16, "PHIẾU THAO TÁC", 3, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                        // tên phiếu thao tác
                        AddPhaseA(mainTable, ticketDetail);

                        // Lưu ý: (nếu có)
                        AddPhaseB(mainTable, ticketDetail);

                        //Giao nhận, nghiệm thu đường dây, thiết bị điện trước khi thao tác: (nếu có)
                        AddPhaseC(mainTable, ticketDetail);

                        // Trình tự hạng mục thao tác:
                        AddPhaseD(mainTable, ticketDetail);

                        //Giao nhận, nghiệm thu đường dây, thiết bị điện sau thao tác: (nếu có)
                        AddPhaseE(mainTable, ticketDetail);

                        // Người viết phiếu ký,  //Người duyệt phiếu ký
                        AddPhaseF(mainTable, ticketDetail);

                        //Các sự kiện bất thường trong thao tác, Người thực hiện thao tác
                        AddPhaseG(mainTable, ticketDetail);

                        // chèn chữ ký người giám sát
                        AddSigntureToPDF(mainTable, ticketDetail.Participants, (int)UserInTicketRoles.Supervisor, (int)UserInTicketRoles.Worker);

                        //Sơ đồ, (Kèm theo nếu cần)
                        AddPhaseH(mainTable);

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

        private void AddPhaseH(PdfPTable mainTable)
        {
            PdfPTable tblData = new PdfPTable(2);
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1, 9 });
            tblData.TotalWidth = tblWidth;
            int fSize = 13;

            //Sơ đồ
            tblData.AddCell(GetCellSign(_fBold, fSize, "Sơ đồ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, "(Kèm theo nếu cần)", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AddPhaseG(PdfPTable mainTable, TicketDetailModel ticketDetail)
        {
            PdfPTable tblData = new PdfPTable(1);
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1 });
            tblData.TotalWidth = tblWidth;
            tblData.LockedWidth = true;
            int fSize = 13;

            //Các sự kiện bất thường trong thao tác:
            tblData.AddCell(GetCellSign(_fBold, fSize, "Các sự kiện bất thường trong thao tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            if (!string.IsNullOrEmpty(ticketDetail.CaseAbnormal))
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, ticketDetail.CaseAbnormal, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "............................................................................................................................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, "............................................................................................................................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, "............................................................................................................................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, "............................................................................................................................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            }

            //Người thực hiện thao tác
            tblData.AddCell(GetCellSign(_fBold, fSize, "Người thực hiện thao tác", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (ticketDetail.DTCompletedTime != null)
            {
                DateTime dtCom = ticketDetail.DTCompletedTime ?? DateTime.Now;
                string completeTime = "Ngày " + dtCom.Day.ToString() + " tháng " + dtCom.Month.ToString() + " năm " + dtCom.Year.ToString();
                tblData.AddCell(GetCellSign(_fNomal, fSize, completeTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "Ngày ... tháng ... năm ....", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AddPhaseF(PdfPTable mainTable, TicketDetailModel ticketDetail)
        {
            PdfPTable tblData = new PdfPTable(2);
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 2 });
            tblData.TotalWidth = tblWidth;
            int fSize = 13;
            //Người viết phiếu ký
            string createTime = "Ngày " + ticketDetail.DTCreatedDate.Day.ToString() + " tháng " + ticketDetail.DTCreatedDate.Month.ToString() + " năm " + ticketDetail.DTCreatedDate.Year.ToString();
            tblData.AddCell(GetCellSign(_fNomal, fSize, createTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            //Người duyệt phiếu ký
            string approveTime = "Ngày " + ticketDetail.DTApprovedDate.Day.ToString() + " tháng " + ticketDetail.DTApprovedDate.Month.ToString() + " năm " + ticketDetail.DTApprovedDate.Year.ToString();
            tblData.AddCell(GetCellSign(_fNomal, fSize, approveTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fNomal, fSize, "Người viết phiếu", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, "Người duyệt phiếu", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(GetCellSign(_fNomal, fSize, "(Ký và ghi rõ họ tên)", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, "(Ký và ghi rõ họ tên)", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (ticketDetail.Participants != null)
            {
                string createUserName = " ";
                string approveUserName = " ";
                string createSignContent = string.Empty;

                // Chèn chữ ký người tạo phiếu ,
                foreach (var item in ticketDetail.Participants)
                {
                    if (item.Roles == (int)UserInTicketRoles.Creator)
                    {
                        createUserName = item.FullName;
                        createSignContent = item.SignatureContent;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(createSignContent))
                {
                    tblData.AddCell(GetCellImageSignatureB(createSignContent, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }
                else
                {
                    tblData.AddCell(GetCellSign(_fItalic, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }

                // Chèn chữ ký người duyệt phiếu
                string approveSignContent = string.Empty;
                foreach (var item in ticketDetail.Participants)
                {
                    if (item.Roles == (int)UserInTicketRoles.Approval)
                    {
                        approveUserName = item.FullName;
                        approveSignContent = item.SignatureContent;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(approveSignContent))
                {
                    tblData.AddCell(GetCellImageSignatureB(approveSignContent, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }
                else
                {
                    tblData.AddCell(GetCellSign(_fItalic, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }

                tblData.AddCell(GetCellSign(_fNomal, fSize, createUserName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(GetCellSign(_fNomal, fSize, approveUserName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AddPhaseE(PdfPTable mainTable, TicketDetailModel ticketDetail)
        {
            PdfPTable tblData = new PdfPTable(2);
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 7, 3 });
            tblData.TotalWidth = tblWidth; tblData.LockedWidth = true;
            int fSize = 13;

            //Giao nhận, nghiệm thu đường dây, thiết bị điện sau thao tác:
            tblData.AddCell(GetCellSign(_fBold, fSize, "Giao nhận, nghiệm thu đường dây, thiết bị điện sau thao tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fItalic, fSize, "(nếu có)", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            AppendDeliveryAccepDevicesToPdf(mainTable, ticketDetail.DeliveryAccepDevices, false);
        }

        private void AddPhaseD(PdfPTable mainTable, TicketDetailModel ticketDetail)
        {
            PdfPTable tblData = new PdfPTable(1);
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1 });
            tblData.TotalWidth = tblWidth;
            tblData.LockedWidth = true;
            int fSize = 13;

            //Trình tự hạng mục thao tác:: (nếu có)
            tblData.AddCell(GetCellSign(_fBold, fSize, "Trình tự hạng mục thao tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
            //Chèn table Trình tự hạng mục thao tác
            AppendSequenceOperationsToPdf(mainTable, ticketDetail.SequenceOperations);
        }

        private void AddPhaseC(PdfPTable mainTable, TicketDetailModel ticketDetail)
        {
            PdfPTable tblData = new PdfPTable(2);
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 8, 2 });
            tblData.TotalWidth = tblWidth;
            tblData.LockedWidth = true;
            int fSize = 13;

            //Giao nhận, nghiệm thu đường dây, thiết bị điện trước khi thao tác: (nếu có)
            tblData.AddCell(GetCellSign(_fBold, fSize, "Giao nhận, nghiệm thu đường dây, thiết bị điện trước khi thao tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fItalic, fSize, "(nếu có)", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
            AppendDeliveryAccepDevicesToPdf(mainTable, ticketDetail.DeliveryAccepDevices, true);
        }

        private void AddPhaseB(PdfPTable mainTable, TicketDetailModel ticketDetail)
        {
            PdfPTable tblData = new PdfPTable(2);
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1, 9 });
            tblData.TotalWidth = tblWidth;
            tblData.LockedWidth = true;
            int fSize = 13;

            // Lưu ý: (nếu có)
            tblData.AddCell(GetCellSign(_fBold, fSize, "Lưu ý:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fItalic, fSize, "(nếu có)", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            if (ticketDetail.DeliveryAccepDevices != null)
            {
                foreach (var item in ticketDetail.OtherRequests)
                {
                    if (item.Type == 2)
                    {
                        AppendOtherRequestsToPdf(mainTable, ticketDetail.OtherRequests, 2);
                        break;
                    }
                }
            }
        }

        private void AddPhaseA(PdfPTable mainTable, TicketDetailModel ticketDetail)
        {
            PdfPTable tblData = new PdfPTable(4) { TotalWidth = tblWidth };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2.5f, 3, 1, 3 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fBold, fSize, "Tên phiếu thao tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            string ticketName = !string.IsNullOrEmpty(ticketDetail.TicketName) ? ticketDetail.TicketName : "................";
            tblData.AddCell(GetCellSign(_fNomal, fSize, ticketName, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            string strDot = ".............................";

            if (ticketDetail.Participants != null)
            {
                // người viết phiếu
                foreach (var item in ticketDetail.Participants)
                {
                    if (item.Roles == (int)UserInTicketRoles.Creator)
                    {
                        string fullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, "  Người viết phiếu:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, fullName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        string positionName = !string.IsNullOrEmpty(item.PositionName) ? item.PositionName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, "Chức vụ:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, positionName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        break;
                    }
                }

                // người duyệt phiếu
                foreach (var item in ticketDetail.Participants)
                {
                    if (item.Roles == (int)UserInTicketRoles.Approval)
                    {
                        string fullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, "  Người duyệt phiếu:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, fullName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        string positionName = !string.IsNullOrEmpty(item.PositionName) ? item.PositionName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, "Chức vụ:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, positionName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        break;
                    }
                }

                // người giám sát
                int index = 1;
                foreach (var item in ticketDetail.Participants)
                {
                    if (item.Roles == (int)UserInTicketRoles.Supervisor)
                    {
                        string fullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, index == 1 ? "  Người giám sát:" : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, fullName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        string positionName = !string.IsNullOrEmpty(item.PositionName) ? item.PositionName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, index == 1 ? "Chức vụ:" : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, positionName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        index += 1;
                    }
                }

                // người thao tác
                index = 1;
                foreach (var item in ticketDetail.Participants)
                {
                    if (item.Roles == (int)UserInTicketRoles.Worker)
                    {
                        string fullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, index == 1 ? "  Người thao tác:" : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, fullName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        string positionName = !string.IsNullOrEmpty(item.PositionName) ? item.PositionName : strDot;
                        tblData.AddCell(GetCellSign(_fNomal, fSize, index == 1 ? "Chức vụ:" : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(GetCellSign(_fNomal, fSize, positionName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        index += 1;
                    }
                }
            }

            //Mục đích thao tác
            tblData.AddCell(GetCellSign(_fBold, fSize, "Mục đích thao tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, !string.IsNullOrEmpty(ticketDetail.Purpose) ? ticketDetail.Purpose : strDot, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Thời gian dự kiến
            tblData.AddCell(GetCellSign(_fBold, fSize, "Thời gian dự kiến:", 4, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Bắt đầu:
            DateTime st = ticketDetail.StartTime;
            string startTime = st.Hour.ToString("00") + "h" + st.Minute.ToString("00") + ", Ngày " + st.Day.ToString() + " tháng " + st.Month.ToString() + " năm " + st.Year.ToString();
            tblData.AddCell(GetCellSign(_fNomal, fSize, "  Bắt đầu:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, startTime, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Kết thúc:
            DateTime ed = ticketDetail.EndTime;
            string endTime = ed.Hour.ToString("00") + "h" + ed.Minute.ToString("00") + ", Ngày " + ed.Day.ToString() + " tháng " + ed.Month.ToString() + " năm " + ed.Year.ToString();
            tblData.AddCell(GetCellSign(_fNomal, fSize, "  Kết thúc:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, endTime, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            PdfPTable tblData1 = new PdfPTable(2) { TotalWidth = tblWidth };
            tblData1.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData1.SetWidths(new float[] { 3, 7 });
            tblData1.LockedWidth = true;
            //Đơn vị đề nghị thao tác:
            tblData1.AddCell(GetCellSign(_fBold, fSize, "Đơn vị đề nghị thao tác:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP));
            tblData1.AddCell(GetCellSign(_fNomal, fSize, !string.IsNullOrEmpty(ticketDetail.RecommendedUnit) ? ticketDetail.RecommendedUnit : strDot, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // Điều kiện cần có để thực hiện: (nếu có)
            tblData1.AddCell(GetCellSign(_fBold, fSize, "Điều kiện có để thực hiện:", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData1.AddCell(GetCellSign(_fItalic, fSize, "(nếu có)", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 5,
                Colspan = 3
            };

            cellMerge.AddElement(tblData1);
            mainTable.AddCell(cellMerge);
            if (ticketDetail.DeliveryAccepDevices != null)
            {
                foreach (var item in ticketDetail.OtherRequests)
                {
                    if (item.Type == 1)
                    {
                        AppendOtherRequestsToPdf(mainTable, ticketDetail.OtherRequests, 1);
                        break;
                    }
                }
            }
        }

        private void AddSigntureToPDF(PdfPTable mainTable, List<ParticipantModel> participants, int roles1, int roles2)
        {
            List<string> lstUser1 = new List<string>();
            List<string> lstUser2 = new List<string>();

            if (participants != null)
            {
                int index1 = 1;
                int index2 = 1;

                foreach (var item in participants)
                {
                    string title = (item.Roles == (int)UserInTicketRoles.Supervisor) ? "Người giám sát " : "Người thao tác ";
                    if (item.Roles == roles1)
                    {
                        lstUser1.Add(title + index1.ToString());
                        lstUser1.Add("(Ký và ghi rõ họ tên)");
                        lstUser1.Add(!string.IsNullOrWhiteSpace(item.SignatureContent) ? item.SignatureContent : " ");
                        lstUser1.Add(!string.IsNullOrWhiteSpace(item.FullName) ? item.FullName : " ");
                        index1++;
                    }
                    else if (item.Roles == roles2)
                    {
                        lstUser2.Add(title + index2.ToString());
                        lstUser2.Add("(Ký và ghi rõ họ tên)");
                        lstUser2.Add(!string.IsNullOrWhiteSpace(item.SignatureContent) ? item.SignatureContent : " ");
                        lstUser2.Add(!string.IsNullOrWhiteSpace(item.FullName) ? item.FullName : " ");
                        index2++;
                    }
                }

                if (lstUser1.Count > 0)
                {
                    PdfPTable tblData = new PdfPTable(2);
                    tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                    tblData.SetWidths(new float[] { 2, 2 });
                    tblData.TotalWidth = tblWidth;
                    tblData.LockedWidth = true;
                    int fSize = 11;
                    var index = 1;
                    for (int i = 0; i < lstUser1.Count; i++)
                    {
                        if (index == 3)
                        {
                            if (!string.IsNullOrWhiteSpace(lstUser1[i].Trim()))
                            {
                                tblData.AddCell(GetCellImageSignature(lstUser1[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
                            }
                            else
                            {
                                tblData.AddCell(GetCellSign(_fNomal, fSize, lstUser1[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                                tblData.AddCell(GetCellSign(_fNomal, fSize, lstUser1[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            }

                            if (lstUser2.Count > i)
                            {
                                if (!string.IsNullOrWhiteSpace(lstUser2[i].Trim()))
                                {
                                    tblData.AddCell(GetCellImageSignature(lstUser2[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
                                }
                                else
                                {
                                    tblData.AddCell(GetCellSign(_fNomal, fSize, lstUser2[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                                    tblData.AddCell(GetCellSign(_fNomal, fSize, lstUser2[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                                }
                            }
                            else
                            {
                                tblData.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                                tblData.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            }
                            index = 0;
                        }
                        else
                        {
                            tblData.AddCell(GetCellSign(_fNomal, fSize, lstUser1[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            if (lstUser2.Count > i)
                            {
                                tblData.AddCell(GetCellSign(_fNomal, fSize, lstUser2[i], 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            }
                            else
                            {
                                tblData.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            }
                            index += 1;
                        }
                    }

                    PdfPCell cellMerge = new PdfPCell
                    {
                        Border = Rectangle.NO_BORDER,
                        Padding = 0,
                        Colspan = 3
                    };
                    cellMerge.AddElement(tblData);
                    mainTable.AddCell(cellMerge);
                }
            }
        }

        private void AppendSequenceOperationsToPdf(PdfPTable mainTable, List<SequenceOperationDetailModel> sequenceOperations)
        {
            // add header
            PdfPTable tblData = new PdfPTable(9);
            tblData.SetWidths(new float[] { 1, 1, 1, 4, 1, 2, 2, 1, 1 });
            tblData.TotalWidth = tblWidth;
            tblData.LockedWidth = true;
            int fSize = 11;
            // Header row.
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Mục", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Địa điểm", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Trình tự thao tác", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Thời gian", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Người", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Bước", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Nội dung", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Đã thực hiện", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Bắt đầu", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Kết thúc", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Ra lệnh", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Nhận lệnh", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            int indexRow = 2;
            if (sequenceOperations != null && sequenceOperations.Count > 0)
            {
                foreach (var item in sequenceOperations)
                {
                    string startTime = " ";
                    if (item.BeginTime != null)
                    {
                        DateTime st = item.BeginTime ?? DateTime.Now;
                        startTime = st.Hour.ToString("00") + "h" + st.Minute.ToString("00") + Environment.NewLine + st.ToString("dd-MM-yyyy");
                    }
                    string endTime = " ";
                    if (item.EndTime != null)
                    {
                        DateTime ed = item.EndTime ?? DateTime.Now;
                        endTime = ed.Hour.ToString("00") + "h" + ed.Minute.ToString("00") + Environment.NewLine + ed.ToString("dd-MM-yyyy");
                    }

                    string leaderUser = !string.IsNullOrEmpty(item.LeaderUser.Label) ? item.LeaderUser.Label : " ";
                    string workerUser = !string.IsNullOrEmpty(item.WorkerUser.Label) ? item.WorkerUser.Label : " ";

                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, !string.IsNullOrEmpty(item.TableOfContents) ? item.TableOfContents : " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, !string.IsNullOrEmpty(item.Location) ? item.Location : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Step.ToString(), 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, !string.IsNullOrEmpty(item.Content) ? item.Content : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.IsDone ? "v" : " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, startTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, endTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, leaderUser, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, workerUser, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                    indexRow += 1;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                    indexRow += 1;
                }
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

        private void AppendDeliveryAccepDevicesToPdf(PdfPTable mainTable, List<DeliveryAccepDeviceModel> deliveryAccepDevices, bool isBefore)
        {
            // add header
            PdfPTable tblData = new PdfPTable(4);
            tblData.SetWidths(new float[] { 1, 1, 2, 3 });
            tblData.TotalWidth = tblWidth;
            tblData.LockedWidth = true;
            int fSize = 11;
            // Header row.
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Thời gian", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Đơn vị", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Họ tên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Nội dung", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // add content
            int indexRow = 1;
            bool isExist = false;
            if (deliveryAccepDevices != null && deliveryAccepDevices.Count > 0)
            {
                foreach (var item in deliveryAccepDevices)
                {
                    if (item.IsBeforeDelivery == isBefore)
                    {
                        string deliveryTime = item.DTDeliveryTime.Hour.ToString("00") + "h" + item.DTDeliveryTime.Minute.ToString("00") + Environment.NewLine + item.DTDeliveryTime.ToString("dd-MM-yyyy");
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, deliveryTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, !string.IsNullOrEmpty(item.Unit) ? item.Unit : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, !string.IsNullOrEmpty(item.FullName) ? item.FullName : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, !string.IsNullOrEmpty(item.Content) ? item.Content : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        indexRow += 1;
                        isExist = true;
                    }
                }
            }

            if (!isExist)
            {
                for (int i = 0; i < 2; i++)
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                    indexRow += 1;
                }
            }
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
                Colspan = 3
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendOtherRequestsToPdf(PdfPTable mainTable, List<OtherRequestModel> otherRequests, int type)
        {
            if (otherRequests != null && otherRequests.Count > 0)
            {
                // add header
                PdfPTable tblData = new PdfPTable(2);
                tblData.SetWidths(new float[] { 1, 6 });
                tblData.TotalWidth = tblWidth;
                tblData.LockedWidth = true;
                int fSize = 11;
                // Header row.
                string title = type == 1 ? "Điều kiện cần có thể thực hiện" : "Lưu ý";
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Stt", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, title, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                // add content
                int indexRow = 1;

                foreach (var item in otherRequests)
                {
                    if (item.Type == type)
                    {
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, indexRow.ToString(), 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, !string.IsNullOrEmpty(item.Description) ? item.Description : " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        indexRow += 1;
                    }
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
        }

        private void AddSignature(int ticketID, int role, string username, int currentUserID, SignatureModel signatureModel)
        {
            signatureModel.CurrentUserID = currentUserID;
            var signatureInfo = _commonService.CreateSignature(signatureModel);
            var dataUpdate = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x =>
                                                    x.TicketID == ticketID
                                                    && x.Roles == role
                                                    && x.UserName == username
                                                    && !x.IsDeleted)
                                         .FirstOrDefault();
            if (dataUpdate != null && signatureInfo != null)
            {
                dataUpdate.SignatureID = signatureInfo.ID;
                dataUpdate.LastUpdatedUserID = currentUserID;
                dataUpdate.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<Participant>().Update(dataUpdate);
                _unitOfWork.Complete();
            }
        }

        private int CreateSequenceOperationParticipant(string userName, bool isExternal, int currentUserID)
        {
            var userInfo = _commonService.GetUserById(userName, isExternal);

            var user = new SequenceOperationParticipant()
            {
                IsExternal = isExternal,
                UserName = userInfo.UserName,
                FullName = userInfo.FullName,
                PositionName = userInfo.PositionName,
                CreatedUserID = currentUserID,
                LastUpdatedUserID = currentUserID
            };
            _unitOfWork.ElogbookRepository<SequenceOperationParticipant>().Add(user);
            bool isCreated = _unitOfWork.Complete() > 0;
            return isCreated ? user.ID : -1;
        }

        private ParticipantModel GetUserInTicket(int ticketId, int roles)
        {
            var entity = _unitOfWork.ElogbookRepository<Participant>()
                                                    .GetQuery(x => x.TicketID == ticketId && x.Roles == roles && !x.IsDeleted)
                                                    .Select(e => new ParticipantModel()
                                                    {
                                                        ID = e.ID,
                                                        TicketID = e.TicketID != null ? e.TicketID.Value : 0,
                                                        UserName = e.UserName,
                                                        IsExternal = e.IsExternal ?? false,
                                                        FullName = e.FullName,
                                                        Roles = e.Roles,
                                                        ElectricRankStr = e.ElectricRank,
                                                        ElectricRankCard = e.ElectricRankCard,
                                                        SignatureID = e.SignatureID,
                                                        PositionName = e.PositionName
                                                    }).FirstOrDefault();

            return entity;
        }

        private List<AttachmentModel> GetAttachedFile(string lstID)
        {
            List<AttachmentModel> results = new List<AttachmentModel>();
            if (lstID != null)
            {
                var attackments = lstID.Split(',');
                if (attackments != null)
                {
                    foreach (var attachment in attackments)
                    {
                        if (int.TryParse(attachment, out int idItem))
                        {
                            var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == idItem).FirstOrDefault();
                            if (att != null)
                            {
                                results.Add(new AttachmentModel
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
            }

            return results;
        }

        public PdfPCell GetCellSign(BaseFont fBase, float fSize, string textContent, int colSpan, int rowSpan, int hAlignment = 1, int vAlignment = 1, int minHeight = 16)
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

        private PdfPCell GetCellImageSignatureB(string signatureContent, int colSpan, int rowSpan, int hAlignment, int vAlignment)
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
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Border = 0
            };
            imageCell.Border = Rectangle.NO_BORDER;

            return imageCell;
        }

        #endregion
    }
}
