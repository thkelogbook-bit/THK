using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Manipulation;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class RegistrationManipulationService : IRegistrationManipulationService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        public RegistrationManipulationService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        /// <summary>
        /// Thêm mới phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public int Add(AddEditTicketModel requestModel)
        {
            var entity = new RegistrationManipulationTicket
            {
                TicketTypeValue = (int)TicketTypePTT.DotXuat,
                FactoryID = requestModel.FactoryID,
                CreateDate = requestModel.CreateDate,
                InsertedAt = requestModel.CreateDate,
                ApproveUnitID = requestModel.ApproveUnitID,
                ApproveUnitView = requestModel.ApproveUnitView,
                TicketName = requestModel.TicketName,
                CreatedUserPositionName = requestModel.CreatedUserPositionName,
                ApprovedUserID = requestModel.ApprovedUserID,
                ApprovedUserPositionName = requestModel.ApprovedUserPositionName,
                Purpose = requestModel.Purpose,
                StartTime = requestModel.StartTime ?? requestModel.StartTime.Value,
                EndTime = requestModel.EndTime ?? requestModel.EndTime.Value,
                RecommendedUnit = requestModel.RecommendedUnit,
                ReferAttachmentIDs = requestModel.ReferAttachmentIDs,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
                TicketStatusValue = requestModel.IsSave ? (int)Core.Enums.TicketManipulationStatus.Unsent : (int)Core.Enums.TicketManipulationStatus.Waiting
            };

            if (!requestModel.IsSave)
                entity.RegistrationDate = requestModel.CreateDate;

            // Tracking trạng thái thay đổi phiếu công tác
            var ticketStatus = requestModel.IsSave ? (int)Core.Enums.TicketManipulationStatus.Unsent : (int)Core.Enums.TicketManipulationStatus.Waiting;
            var description = requestModel.IsSave ? "Lưu chỉnh sửa phiếu thao tác" : "Gửi phiếu thao tác";
            List<TicketStatusHistory> ticketStatusHistories = new List<TicketStatusHistory>
            {
                AddTicketStatusHistory(0, requestModel.CurrentUserID, entity.UpdatedAt, ticketStatus, description,""),
            };
            entity.TicketStatusHistories = ticketStatusHistories;

            _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                // người giám sát và người thao tác
                if (requestModel.Participants != null)
                {
                    foreach (var item in requestModel.Participants)
                    {
                        var participant = new Participant()
                        {
                            RegistrationManipulationID = entity.ID,
                            IsCreated = true,
                            UserName = item.UserName,
                            FullName = item.FullName,
                            PositionName = item.PositionName,
                            Roles = item.Roles,
                            ElectricRank = item.ElectricRank,
                            ElectricRankCard = item.ElectricRankCard,
                            SignatureID = item.SignatureID,
                            IsExternal = item.IsExternal,
                            Order = item.Order
                        };
                        _unitOfWork.ElogbookRepository<Participant>().Add(participant);
                    }
                }

                //5.Điều kiện cần có thể thực hiện và 6.Lưu ý
                if (requestModel.OtherRequests != null)
                {
                    foreach (var item in requestModel.OtherRequests)
                    {
                        var otherRequest = new OtherRequest()
                        {
                            RegistrationManipulationID = entity.ID,
                            IsCreated = true,
                            Description = item.Description,
                            Type = item.Type
                        };

                        _unitOfWork.ElogbookRepository<OtherRequest>().Add(otherRequest);
                    }
                }

                // Trình tự hạng mục thao tác
                if (requestModel.SequenceOperations != null)
                {
                    foreach (var item in requestModel.SequenceOperations)
                    {
                        var sequenceOperation = new SequenceOperation()
                        {
                            RegistrationManipulationID = entity.ID,
                            IsCreated = true,
                            TableOfContents = item.TableOfContents,
                            Location = item.Location,
                            Step = item.Step,
                            Content = item.Content,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            BeginTime = item.BeginTime,
                            EndTime = item.EndTime,
                            LeaderID = item.LeaderID,
                            WorkerID = item.WorkerID,
                            IsDone = item.IsDone,
                            ParentStep = item.ParentStep
                        };
                        _unitOfWork.ElogbookRepository<SequenceOperation>().Add(sequenceOperation);
                    }
                }

                _unitOfWork.Complete();

                return entity.ID;
            }
            else
            {
                return 0;
            }
        }

        public ResponseBaseModel Find(SearchRegistrationManipulationTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            // Phiếu thao tác đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => !x.IsDeleted);
            // Tên phiếu thao tác, tên người tạo, tên người duyệt
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
                entities = entities.Where(x => x.TicketName.Contains(requestModel.SearchText)
                                            || x.CreatedUser.FullName.Contains(requestModel.SearchText)
                                            || x.ApprovedUser.FullName.Contains(requestModel.SearchText));
            // Nhà máy
            if (requestModel.FactoryID > 0)
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            // Đơn vị cấp phiếu
            if (requestModel.ApproveUnitID > 0)
                entities = entities.Where(x => x.ApproveUnitID == requestModel.ApproveUnitID);
            // Ngày tạo phiếu
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate) || !string.IsNullOrWhiteSpace(requestModel.ToDate))
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreateDate) >= frDate.Date && DbFunctions.TruncateTime(x.CreateDate) <= toDate.Date);
            // Loại phiếu
            if (requestModel.TicketType > 0)
                entities = entities.Where(x => x.TicketTypeValue == requestModel.TicketType);
            // Trạng thái phiếu
            if (!requestModel.IsRegistrationTicket)
                requestModel.TicketStatusValue = (int)Core.Enums.TicketManipulationStatus.Waiting;
            if (requestModel.TicketStatusValue > 0)
                entities = entities.Where(x => x.TicketStatusValue == requestModel.TicketStatusValue);

            // Return and paging data
            var page = entities.Select(
                e => new RegistrationTicketSearchModel()
                {
                    ID = e.ID,
                    TicketName = e.TicketName,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    ApproveUnit = new ValueLabelModel { Value = e.ApproveUnit.ID, Label = e.ApproveUnit.Name },
                    ApproveUnitView = e.ApproveUnitView,
                    CreatedTicket = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    ApprovedTicket = new ValueLabelModel { Value = e.ApprovedUser.ID, Label = e.ApprovedUser.FullName },
                    TicketTypeValue = e.TicketTypeValue,
                    TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                    CreatedDateLocal = e.CreateDate,
                })
                .OrderByDescending(t => t.CreatedDateLocal)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        private TicketStatusHistory AddTicketStatusHistory(int ticketID, int userID, DateTime insertedAt, int ticketStatus, string description, string lstAttachment)
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

            if (ticketID > 0)
                ticketStatusHistory.RegistrationManipulationID = ticketID;

            switch (ticketStatus)
            {
                case (int)Core.Enums.TicketManipulationStatus.Unsent:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.Unsent);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.Waiting:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.Waiting);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.Recovered:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.Recovered);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.Reject:
                    ticketStatusHistory.Description = "Từ chối thao tác.";
                    ticketStatusHistory.Reason = description;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.Approved:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.Approved);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.Unfulfilled:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.Unfulfilled);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.WaitForConfirmation:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.WaitForConfirmation);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.Completed:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.Completed);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
                case (int)Core.Enums.TicketManipulationStatus.Cancel:
                    ticketStatusHistory.Description = EnumExtensions.GetEnumDescription(Core.Enums.TicketManipulationStatus.Cancel);
                    ticketStatusHistory.Reason = string.Empty;
                    break;
            }

            return ticketStatusHistory;
        }

        /// <summary>
        /// kiểm tra dữ liệu có trùng lặp
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketName"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        public bool CheckDuplicateData(int id, string ticketName, bool isAddMode, bool isTicketNumber)
        {
            if (!isTicketNumber)
            {
                // Trường hợp thêm mới thì kiểm tra Số phiếu đăng ký công tác
                if (isAddMode)
                {
                    return _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => x.TicketName == ticketName
                            && !x.IsDeleted).Any();

                }
                // Trường hợp cập nhật thì kiểm tra Số phiếu đăng ký công tác và id
                else
                {
                    return _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => x.ID != id
                                                                                        && x.TicketName == ticketName
                                                                                        && !x.IsDeleted).Any();
                }
            }
            else
            {
                // Trường hợp thêm mới thì kiểm tra Số phiếu đăng ký công tác
                if (isAddMode)
                {
                    return _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.TicketNumber == ticketName
                            && !x.IsDeleted).Any();

                }
                // Trường hợp cập nhật thì kiểm tra Số phiếu đăng ký công tác và id
                else
                {
                    return _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.ID != id
                                                                                        && x.TicketNumber == ticketName
                                                                                        && !x.IsDeleted).Any();
                }
            }
        }

        /// <summary>
        /// cập nhật phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool Update(AddEditTicketModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                var oldStatusValue = entity.TicketStatusValue;
                entity.TicketTypeValue = requestModel.TicketType > 0 ? requestModel.TicketType : entity.TicketTypeValue;
                entity.FactoryID = requestModel.FactoryID;
                entity.CreateDate = requestModel.CreateDate;
                entity.InsertedAt = requestModel.CreateDate;
                entity.ApproveUnitID = requestModel.ApproveUnitID;
                entity.ApproveUnitView = requestModel.ApproveUnitView;
                entity.TicketName = requestModel.TicketName;
                entity.CreatedUserPositionName = requestModel.CreatedUserPositionName;
                entity.ApprovedUserID = requestModel.ApprovedUserID;
                entity.ApprovedUserPositionName = requestModel.ApprovedUserPositionName;
                entity.Purpose = requestModel.Purpose;
                entity.StartTime = requestModel.StartTime ?? requestModel.StartTime.Value;
                entity.EndTime = requestModel.EndTime ?? requestModel.EndTime.Value;
                entity.RecommendedUnit = requestModel.RecommendedUnit;
                entity.ReferAttachmentIDs = requestModel.ReferAttachmentIDs;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.TicketStatusValue = requestModel.IsSave ? (int)Core.Enums.TicketManipulationStatus.Unsent : (int)Core.Enums.TicketManipulationStatus.Waiting;

                if (!requestModel.IsSave)
                    entity.RegistrationDate = requestModel.CreateDate;

                _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().Update(entity);

                // người giám sát và người thao tác
                if (requestModel.Participants != null)
                {
                    var participants = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x => x.RegistrationManipulationID == requestModel.ID && !x.IsDeleted).ToList();
                    UpdateParticipants(participants, requestModel);
                }

                //5.Điều kiện cần có thể thực hiện và 6.Lưu ý
                if (requestModel.OtherRequests != null)
                {
                    var otherRequests = _unitOfWork.ElogbookRepository<OtherRequest>().GetQuery(x => x.RegistrationManipulationID == requestModel.ID && !x.IsDeleted).ToList();
                    UpdateOtherRequests(otherRequests, requestModel);
                }

                // Cập nhật trình tự thao tác
                if (requestModel.SequenceOperations != null)
                {
                    var sequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => x.RegistrationManipulationID == requestModel.ID && !x.IsDeleted).ToList();
                    UpdateSequenceOperations(sequenceOperations, requestModel);
                }

                // Tracking trạng thái thay đổi phiếu công tác
                TicketStatusHistory ticketStatusHistory = null;
                if (requestModel.IsSave)
                {
                    if (oldStatusValue != (int)Core.Enums.TicketManipulationStatus.Unsent)
                    {
                        ticketStatusHistory = AddTicketStatusHistory(entity.ID, requestModel.CurrentUserID, entity.UpdatedAt, (int)Core.Enums.TicketManipulationStatus.Unsent, "", "");
                        _unitOfWork.ElogbookRepository<TicketStatusHistory>().Add(ticketStatusHistory);
                    }
                }
                else
                {
                    ticketStatusHistory = AddTicketStatusHistory(entity.ID, requestModel.CurrentUserID, entity.UpdatedAt, (int)Core.Enums.TicketManipulationStatus.Waiting, "", "");
                    _unitOfWork.ElogbookRepository<TicketStatusHistory>().Add(ticketStatusHistory);
                }

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        private void UpdateSequenceOperations(ICollection<SequenceOperation> sequenceOperations, AddEditTicketModel request)
        {
            //cập nhật
            foreach (var item in sequenceOperations)
            {
                var existingRow = request.SequenceOperations.Find(x => x.ID == item.ID);
                if (existingRow != null)
                {
                    item.TableOfContents = existingRow.TableOfContents;
                    item.Location = existingRow.Location;
                    item.Step = existingRow.Step;
                    item.Content = existingRow.Content;
                    item.LastUpdatedUserID = request.CurrentUserID;
                    item.BeginTime = existingRow.BeginTime;
                    item.EndTime = existingRow.EndTime;
                    item.LeaderID = existingRow.LeaderID;
                    item.WorkerID = existingRow.WorkerID;
                    item.ParentStep = existingRow.ParentStep;

                    _unitOfWork.ElogbookRepository<SequenceOperation>().Update(item);
                }
                else // xoá
                {
                    _unitOfWork.ElogbookRepository<SequenceOperation>().Delete(item, request.CurrentUserID);
                }
            }

            // thêm mới
            var newDatas = request.SequenceOperations.Where(x => x.ID <= 0);
            foreach (var item in newDatas)
            {
                var otherRequest = new SequenceOperation()
                {
                    RegistrationManipulationID = request.ID,
                    IsCreated = true,
                    TableOfContents = item.TableOfContents,
                    Location = item.Location,
                    Step = item.Step,
                    Content = item.Content,
                    CreatedUserID = request.CurrentUserID,
                    LastUpdatedUserID = request.CurrentUserID,
                    BeginTime = item.BeginTime,
                    EndTime = item.EndTime,
                    LeaderID = item.LeaderID,
                    WorkerID = item.WorkerID,
                    IsDone = item.IsDone,
                    ParentStep = item.ParentStep,
                };
                _unitOfWork.ElogbookRepository<SequenceOperation>().Add(otherRequest);
            }
        }

        private void UpdateOtherRequests(ICollection<OtherRequest> otherRequests, AddEditTicketModel request)
        {
            //cập nhật
            foreach (var item in otherRequests)
            {
                var existingRow = request.OtherRequests.Find(x => x.ID == item.ID);
                if (existingRow != null)
                {
                    item.Description = existingRow.Description;
                    item.LastUpdatedUserID = request.CurrentUserID;
                    _unitOfWork.ElogbookRepository<OtherRequest>().Update(item);
                }
                else // xoá
                {
                    _unitOfWork.ElogbookRepository<OtherRequest>().Delete(item, request.CurrentUserID);
                }
            }

            // thêm mới
            var newDatas = request.OtherRequests.Where(x => x.ID <= 0);
            foreach (var item in newDatas)
            {
                var otherRequest = new OtherRequest()
                {
                    RegistrationManipulationID = request.ID,
                    IsCreated = true,
                    Description = item.Description,
                    Type = item.Type,
                    CreatedUserID = request.CurrentUserID,
                    LastUpdatedUserID = request.CurrentUserID
                };
                _unitOfWork.ElogbookRepository<OtherRequest>().Add(otherRequest);
            }
        }

        private void UpdateParticipants(List<Participant> participantsEntity, AddEditTicketModel request)
        {
            //cập nhật
            foreach (var item in participantsEntity)
            {
                var existingRow = request.Participants.Find(x => x.ID == item.ID);
                if (existingRow != null)
                {
                    item.UserName = existingRow.UserName;
                    item.FullName = existingRow.FullName;
                    item.PositionName = existingRow.PositionName;
                    item.ElectricRank = existingRow.ElectricRank;
                    item.ElectricRankCard = existingRow.ElectricRankCard;
                    item.SignatureID = existingRow.SignatureID;
                    item.IsExternal = existingRow.IsExternal;
                    item.Order = existingRow.Order;
                    item.LastUpdatedUserID = request.CurrentUserID;

                    _unitOfWork.ElogbookRepository<Participant>().Update(item);
                }
                else // xoá
                {
                    _unitOfWork.ElogbookRepository<Participant>().Delete(item, request.CurrentUserID);
                }
            }

            // thêm mới
            var newDatas = request.Participants.Where(x => x.ID <= 0);
            foreach (var item in newDatas)
            {
                var participant = new Participant()
                {
                    RegistrationManipulationID = request.ID,
                    IsCreated = true,
                    UserName = item.UserName,
                    FullName = item.FullName,
                    PositionName = item.PositionName,
                    Roles = item.Roles,
                    ElectricRank = item.ElectricRank,
                    ElectricRankCard = item.ElectricRankCard,
                    SignatureID = item.SignatureID,
                    IsExternal = item.IsExternal,
                    Order = item.Order,
                    CreatedUserID = request.CurrentUserID,
                    LastUpdatedUserID = request.CurrentUserID
                };
                _unitOfWork.ElogbookRepository<Participant>().Add(participant);
            }
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ManipulationTicketDetailModel GetManipulationTicketDetailById(int id)
        {
            var data = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => x.ID == id && !x.IsDeleted);
            var entity = data.Select(e => new ManipulationTicketDetailModel()
            {
                ID = e.ID,
                TicketTypeValue = e.TicketTypeValue,
                Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                CreateDate = e.CreateDate,
                ApproveUnit = new ValueLabelModel { Value = e.ApproveUnit.ID, Label = e.ApproveUnit.Name },
                ApproveUnitView = e.ApproveUnitView,
                TicketName = e.TicketName,
                CreatedUserPositionName = e.CreatedUserPositionName,
                ApprovedUser = new ValueLabelModel { Value = e.ApprovedUser.ID, Label = e.ApprovedUser.FullName },
                ApprovedUserPositionName = e.ApprovedUserPositionName,
                Purpose = e.Purpose,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                RecommendedUnit = e.RecommendedUnit,
                ReferAttachmentIDs = e.ReferAttachmentIDs,
                CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
                TicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                RegistrationDate = e.RegistrationDate
            }).FirstOrDefault();

            if (entity != null && entity.ID > 0)
            {
                //Người giám sát & người thao tác
                entity.Participants = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x => !x.IsDeleted && x.RegistrationManipulationID == entity.ID)
                .Select(t => new ParticipantModel()
                {
                    ID = t.ID,
                    RegistrationManipulationID = entity.ID,
                    IsCreated = true,
                    UserName = t.UserName,
                    FullName = t.FullName,
                    PositionName = t.PositionName,
                    Roles = t.Roles,
                    ElectricRankStr = t.ElectricRank,
                    ElectricRankCard = t.ElectricRankCard,
                    SignatureID = t.SignatureID,
                    IsExternal = t.IsExternal,
                    Order = t.Order
                }).ToList();

                foreach (var item in entity.Participants)
                {
                    //Fix bug display created ticket info not correct
                    if (item.Roles == 1)
                    {
                        if (entity.CreatedUser != null && !string.IsNullOrWhiteSpace(entity.CreatedUser.Label))
                        {
                            item.FullName = entity.CreatedUser.Label;
                        }
                        if (!string.IsNullOrWhiteSpace(entity.CreatedUserPositionName))
                        {
                            item.PositionName = entity.CreatedUserPositionName;
                        }
                        break;
                    }
                }

                //5.Điều kiện cần có thể thực hiện và 6.Lưu ý
                entity.OtherRequests = _unitOfWork.ElogbookRepository<OtherRequest>().GetQuery(x => !x.IsDeleted && x.RegistrationManipulationID == entity.ID)
                .Select(t => new OtherRequestModel()
                {
                    ID = t.ID,
                    RegistrationManipulationID = entity.ID,
                    IsCreated = t.IsCreated,
                    Description = t.Description,
                    Type = t.Type
                }).ToList();

                // Trình tự hạng mục thao tác
                entity.SequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => !x.IsDeleted && x.RegistrationManipulationID == entity.ID)
                .Select(t => new SequenceOperationModel()
                {
                    ID = t.ID,
                    RegistrationManipulationID = entity.ID,
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

                if (!string.IsNullOrWhiteSpace(entity.ReferAttachmentIDs))
                {
                    entity.FileAttachments = new List<AttachmentModel>();
                    var attackments = entity.ReferAttachmentIDs.Split(',');
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
            }

            return entity;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusValue"></param>
        /// <param name="description"></param>
        /// <param name="userID"></param>
        /// <param name="lstAttachment"></param>
        /// <returns></returns>
        public bool UpdateStatus(int id, int statusValue, string description, int userID, string lstAttachment)
        {
            var entity = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetById(id);
            if (entity != null && entity.TicketStatusValue != statusValue)
            {
                entity.TicketStatusValue = statusValue;
                entity.UpdatedAt = DateTime.Now;
                _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().Update(entity);

                var ticketStatusHistory = AddTicketStatusHistory(entity.ID, userID, entity.UpdatedAt, statusValue, description, lstAttachment);
                _unitOfWork.ElogbookRepository<TicketStatusHistory>().Add(ticketStatusHistory);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        /// <summary>
        /// Xoá phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool DeleteTicket(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetById(id);

            if (entity != null)
            {
                // Xóa lưu dữ liệu Người giám sát, Người thao tác
                var participants = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x => x.TicketID == id && !x.IsDeleted);
                if (participants != null)
                {
                    _unitOfWork.ElogbookRepository<Participant>().Delete(participants, userID);
                }

                // Xóa lưu dữ liệu 1/Điều kiện cần có thể thực hiện, 2/Lưu ý
                var otherRequests = _unitOfWork.ElogbookRepository<OtherRequest>().GetQuery(x => x.TicketID == id && !x.IsDeleted);
                if (otherRequests != null)
                {
                    _unitOfWork.ElogbookRepository<OtherRequest>().Delete(otherRequests, userID);
                }

                // Xóa lưu dữ liệu trình tự hạng mục thao tác
                var sequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => x.TicketID == id && !x.IsDeleted);
                if (sequenceOperations != null)
                {
                    _unitOfWork.ElogbookRepository<SequenceOperation>().Delete(sequenceOperations, userID);
                }

                // Xóa lưu dữ liệu lịch sử
                var ticketStatusHistory = _unitOfWork.ElogbookRepository<TicketStatusHistory>().GetQuery(x => x.RegistrationManipulationID == id && !x.IsDeleted);

                if (ticketStatusHistory != null)
                {
                    _unitOfWork.ElogbookRepository<TicketStatusHistory>().Delete(ticketStatusHistory, userID);
                }

                _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        /// <summary>
        /// Tìm kiếm trạng thái phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetStatusById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<TicketStatusHistory>().GetQuery(x => x.RegistrationManipulationID == id && !x.IsDeleted).ToList();

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
                    Reason = item.Reason,
                    FileAttachs = new List<AttachmentModel>()
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

        /// <summary>
        /// Duyệt phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public int AddApprovalTicket(AddManipulationTicketInfoModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => x.ID == requestModel.RegistrationManipulationTicketID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                var ticket = new ManipulationTicket()
                {
                    TicketType = entity.TicketTypeValue,
                    RegistrationManipulationTicketID = entity.ID,
                    FactoryID = entity.FactoryID,
                    CreatedDate = DateTime.Now,
                    ApproveUnitID = entity.ApproveUnitID,
                    ApproveUnitView = entity.ApproveUnitView,
                    TicketName = entity.TicketName,
                    TicketNumber = requestModel.TicketNumber,
                    CreatedUserTicket = entity.CreatedUser.FullName,
                    ApprovedUserTicket = entity.ApprovedUser.FullName,
                    Purpose = entity.Purpose,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                    RecommendedUnit = entity.RecommendedUnit,
                    ReferAttachmentIDs = entity.ReferAttachmentIDs,
                    TicketStatusValue = (int)Core.Enums.TicketManipulationStatus.Unfulfilled,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID
                };

                if (requestModel.StartTime.HasValue)
                    ticket.StartTime = requestModel.StartTime.Value;
                if (requestModel.EndTime.HasValue)
                    ticket.EndTime = requestModel.EndTime.Value;

                _unitOfWork.ElogbookRepository<ManipulationTicket>().Add(ticket);

                var saveSuccess = _unitOfWork.Complete() > 0;
                if (saveSuccess)
                {
                    // người giám sát và người thao tác
                    var participants = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x => x.RegistrationManipulationID == requestModel.RegistrationManipulationTicketID && !x.IsDeleted);
                    List<Participant> dataParticipant = new List<Participant>();
                    if (participants != null && participants.Count() > 0)
                    {
                        foreach (var item in participants)
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
                                LastUpdatedUserID = requestModel.CurrentUserID
                            };

                            dataParticipant.Add(participant);
                            _unitOfWork.ElogbookRepository<Participant>().Add(participant);
                        }
                    }

                    //5.Điều kiện cần có thể thực hiện và 6.Lưu ý
                    var otherRequests = _unitOfWork.ElogbookRepository<OtherRequest>().GetQuery(x => x.RegistrationManipulationID == requestModel.RegistrationManipulationTicketID && !x.IsDeleted);
                    if (otherRequests != null && otherRequests.Count() > 0)
                    {
                        foreach (var item in otherRequests)
                        {
                            var otherRequest = new OtherRequest()
                            {
                                TicketID = ticket.ID,
                                IsCreated = false,
                                Description = item.Description,
                                Type = item.Type
                            };

                            _unitOfWork.ElogbookRepository<OtherRequest>().Add(otherRequest);
                        }
                    }

                    // Trình tự hạng mục thao tác
                    var sequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => x.RegistrationManipulationID == requestModel.RegistrationManipulationTicketID && !x.IsDeleted);
                    if (sequenceOperations != null && sequenceOperations.Count() > 0)
                    {
                        foreach (var item in sequenceOperations)
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
                                LeaderID = item.LeaderID,
                                WorkerID = item.WorkerID,
                                IsDone = false,
                                ParentStep = item.ParentStep
                            };
                            _unitOfWork.ElogbookRepository<SequenceOperation>().Add(sequenceOperation);
                        }
                    }

                    //Danh sách giao nhận nghiệm thu đường dây, thiết bị điện trước khi thao tác
                    if (requestModel.DeliveryAccepDeviceModels != null && requestModel.DeliveryAccepDeviceModels.Count > 0)
                    {
                        foreach (var item in requestModel.DeliveryAccepDeviceModels)
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
                                LastUpdatedUserID = requestModel.CurrentUserID
                            };

                            _unitOfWork.ElogbookRepository<DeliveryAccepDevice>().Add(deliveryAccepDeviceModels);
                        }
                    }

                    var registrationManipulationTicketUpdate = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetById(requestModel.RegistrationManipulationTicketID);
                    registrationManipulationTicketUpdate.TicketStatusValue = (int)Core.Enums.TicketManipulationStatus.Approved;
                    _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().Update(registrationManipulationTicketUpdate);
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
                            }
                        }
                        _unitOfWork.ElogbookRepository<Participant>().Update(dataParticipant);
                        _unitOfWork.Complete();
                    }

                    return ticket.ID;
                }
            }

            return 0;
        }

        /// <summary>
        /// Tìm kiếm phiếu mẫu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindTemplate(Core.Models.Request.Manipulation.SearchTemplateTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            // Phiếu thao tác đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().GetQuery(x => !x.IsDeleted);
            // Tên phiếu thao tác mẫu
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
                entities = entities.Where(x => x.TemplateTicketName.Contains(requestModel.SearchText));

            // Ngày tạo phiếu
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate) || !string.IsNullOrWhiteSpace(requestModel.ToDate))
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date && DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date);

            if (requestModel.TicketStatusValue >= 0)
                entities = entities.Where(x => x.Status == requestModel.TicketStatusValue);

            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                var results = entities.ToList();
                var data = results.Where(x => x.IdFactorysTemplateTicket.Split(',').Select(Int32.Parse).ToList().Contains(requestModel.FactoryID)).ToList();
                // Return and paging data
                var pageData = data.Select(
                    e => new Core.Models.Response.Manipulation.SearchTemplateTicketModel()
                    {
                        ID = e.ID,
                        TemplateTicketName = e.TemplateTicketName,
                        FactorysTemplateTicket = e.FactorysTemplateTicket,
                        Status = e.Status,
                        CreateDate = e.InsertedAt,
                        CreateUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    })
                    .OrderByDescending(t => t.CreateDate).ToList()
                    .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                return HttpResponse.OkList(pageData);
            }

            // Return and paging data
            var page = entities.Select(
                e => new Core.Models.Response.Manipulation.SearchTemplateTicketModel()
                {
                    ID = e.ID,
                    TemplateTicketName = e.TemplateTicketName,
                    FactorysTemplateTicket = e.FactorysTemplateTicket,
                    Status = e.Status,
                    CreateDate = e.InsertedAt,
                    CreateUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                })
                .OrderByDescending(t => t.CreateDate)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// kiểm tra dữ liệu phiếu mẫu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        public bool CheckDuplicateDataTemplate(int id, AddEditTemplateTicketModel requestModel, bool isAddMode)
        {
            // Trường hợp thêm mới thì kiểm tra Số phiếu đăng ký công tác
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().GetQuery(x => x.TemplateTicketName == requestModel.TemplateTicketName
                                                                                                    && x.IdFactorysTemplateTicket == requestModel.IdFactorysTemplateTicket
                                                                                                    && !x.IsDeleted).Any();

            }
            // Trường hợp cập nhật thì kiểm tra Số phiếu đăng ký công tác và id
            else
            {
                return _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().GetQuery(x => x.ID != id && x.TemplateTicketName == requestModel.TemplateTicketName
                                                                                                    && x.IdFactorysTemplateTicket == requestModel.IdFactorysTemplateTicket
                                                                                                    && !x.IsDeleted).Any();
            }
        }

        /// <summary>
        /// tạo phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public int AddTemplateTicket(AddEditTemplateTicketModel requestModel)
        {
            var entity = new ManipulationTemplateTicket
            {
                TemplateTicketName = requestModel.TemplateTicketName,
                IdFactorysTemplateTicket = requestModel.IdFactorysTemplateTicket,
                InsertedAt = DateTime.Now,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
                Status = requestModel.Status
            };

            if (!string.IsNullOrEmpty(entity.IdFactorysTemplateTicket))
            {
                var factorys = GetFactorys(entity.IdFactorysTemplateTicket);
                entity.FactorysTemplateTicket = string.Join(",", factorys.Select(x => x.FactoryName).ToList());
            }

            _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                // Trình tự hạng mục thao tác
                if (requestModel.SequenceOperations != null)
                {
                    foreach (var item in requestModel.SequenceOperations)
                    {
                        var sequenceOperation = new SequenceOperation()
                        {
                            ManipulationTemplateTicketID = entity.ID,
                            IsCreated = true,
                            TableOfContents = item.ParentStep > 0 ? string.Empty : item.TableOfContents,
                            Location = item.ParentStep > 0 ? string.Empty : item.Location,
                            Step = item.Step,
                            Content = item.Content,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            BeginTime = item.BeginTime,
                            EndTime = item.EndTime,
                            LeaderID = item.LeaderID,
                            WorkerID = item.WorkerID,
                            ParentStep = item.ParentStep,
                        };
                        _unitOfWork.ElogbookRepository<SequenceOperation>().Add(sequenceOperation);
                    }
                }

                _unitOfWork.Complete();

                return entity.ID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Xoá phiếu thao tác mẫu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool DeleteTemplateTicket(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().GetById(id);

            if (entity != null)
            {
                var sequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => x.ManipulationTemplateTicketID == id && !x.IsDeleted);
                _unitOfWork.ElogbookRepository<SequenceOperation>().Delete(sequenceOperations, userID);

                _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        /// <summary>
        /// cập nhật phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool UpdateTemplateTicket(AddEditTemplateTicketModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.TemplateTicketName = requestModel.TemplateTicketName;
                entity.IdFactorysTemplateTicket = requestModel.IdFactorysTemplateTicket;
                entity.InsertedAt = DateTime.Now;
                entity.CreatedUserID = requestModel.CurrentUserID;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.Status = requestModel.Status;
                entity.UpdatedAt = DateTime.Now;

                if (!string.IsNullOrEmpty(entity.IdFactorysTemplateTicket))
                {
                    var factorys = GetFactorys(entity.IdFactorysTemplateTicket);
                    entity.FactorysTemplateTicket = string.Join(",", factorys.Select(x => x.FactoryName).ToList());
                }

                _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().Update(entity);

                // Cập nhật trình tự thao tác
                if (requestModel.SequenceOperations != null)
                {
                    var sequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => x.ManipulationTemplateTicketID == requestModel.ID && !x.IsDeleted).ToList();
                    UpdateSequenceOperationsTemplate(sequenceOperations, requestModel);
                }

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        private void UpdateSequenceOperationsTemplate(List<SequenceOperation> sequenceOperations, AddEditTemplateTicketModel request)
        {
            //cập nhật
            foreach (var item in sequenceOperations)
            {
                var existingRow = request.SequenceOperations.Find(x => x.ID == item.ID);
                if (existingRow != null)
                {
                    item.TableOfContents = existingRow.ParentStep > 0 ? string.Empty : existingRow.TableOfContents;
                    item.Location = existingRow.ParentStep > 0 ? string.Empty : existingRow.Location;
                    item.Step = existingRow.Step;
                    item.Content = existingRow.Content;
                    item.CreatedUserID = request.CurrentUserID;
                    item.LastUpdatedUserID = request.CurrentUserID;
                    item.BeginTime = existingRow.BeginTime;
                    item.EndTime = existingRow.EndTime;
                    item.LeaderID = existingRow.LeaderID;
                    item.WorkerID = existingRow.WorkerID;
                    item.ParentStep = existingRow.ParentStep;

                    _unitOfWork.ElogbookRepository<SequenceOperation>().Update(item);
                }
                else // xoá
                {
                    _unitOfWork.ElogbookRepository<SequenceOperation>().Delete(item, request.CurrentUserID);
                }
            }

            // thêm mới
            var newDatas = request.SequenceOperations.Where(x => x.ID <= 0);
            foreach (var item in newDatas)
            {
                var otherRequest = new SequenceOperation()
                {
                    ManipulationTemplateTicketID = request.ID,
                    IsCreated = true,
                    TableOfContents = item.ParentStep > 0 ? string.Empty : item.TableOfContents,
                    Location = item.ParentStep > 0 ? string.Empty : item.Location,
                    Step = item.Step,
                    Content = item.Content,
                    CreatedUserID = request.CurrentUserID,
                    LastUpdatedUserID = request.CurrentUserID,
                    BeginTime = item.BeginTime,
                    EndTime = item.EndTime,
                    LeaderID = item.LeaderID,
                    WorkerID = item.WorkerID,
                    ParentStep = item.ParentStep,
                    IsDone = item.IsDone,
                };
                _unitOfWork.ElogbookRepository<SequenceOperation>().Add(otherRequest);
            }
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác mẫu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DetailTemplateTicketModel FindTemplateId(int id)
        {
            var data = _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().GetQuery(x => x.ID == id && !x.IsDeleted);
            var entity = data.Select(e => new DetailTemplateTicketModel()
            {
                ID = e.ID,
                TemplateTicketName = e.TemplateTicketName,
                IdFactorysTemplateTicket = e.IdFactorysTemplateTicket,
                Status = e.Status,
                CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
            }).FirstOrDefault();

            if (entity != null && entity.ID > 0)
            {
                // Danh sách nhà máy
                if (!string.IsNullOrEmpty(entity.IdFactorysTemplateTicket))
                {
                    entity.Factorys = GetFactorys(entity.IdFactorysTemplateTicket);
                }

                // Trình tự hạng mục thao tác
                entity.SequenceOperations = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => !x.IsDeleted && x.ManipulationTemplateTicketID == entity.ID)
                                            .Select(t => new SequenceOperationModel()
                                            {
                                                ID = t.ID,
                                                ManipulationTemplateTicketID = t.ManipulationTemplateTicketID,
                                                IsCreated = t.IsCreated,
                                                TableOfContents = t.TableOfContents,
                                                Location = t.Location,
                                                Step = t.Step,
                                                Content = t.Content,
                                                BeginTime = t.BeginTime,
                                                ParentStep = t.ParentStep,
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
                                                CreatedUser = new ValueLabelModel { Value = t.CreatedUser.ID, Label = t.CreatedUser.FullName },
                                                LastUpdatedUser = new ValueLabelModel { Value = t.LastUpdatedUser.ID, Label = t.LastUpdatedUser.FullName },
                                            }).OrderBy(x => x.Step).ToList();
            }

            return entity;
        }

        private List<FactoryModel> GetFactorys(string idFactorysTemplateTicket)
        {
            try
            {
                var ids = idFactorysTemplateTicket.Split(',');
                List<FactoryModel> factorys = new List<FactoryModel>();
                foreach (var item in ids)
                {
                    if (int.TryParse(item, out int idItem))
                    {
                        var fac = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == idItem && !x.IsDeleted).Select(
                                         e => new FactoryModel()
                                         {
                                             ID = e.ID,
                                             FactoryName = e.FactoryName,
                                             Company = new ValueLabelModel { Value = e.Company.ID, Label = e.Company.Name },
                                             ProductionPlant = new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                                             Status = e.Status,
                                         }).FirstOrDefault();
                        factorys.Add(fac);
                    }
                }

                return factorys;
            }
            catch
            {
                return new List<FactoryModel>();
            }
        }

        /// <summary>
        /// Tìm kiếm phiếu mẫu đang active
        /// </summary>
        /// <returns></returns>
        public ResponseBaseModel FindTemplateActive()
        {
            // Phiếu thao tác đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<ManipulationTemplateTicket>().GetQuery(x => !x.IsDeleted && x.Status == 1);

            // Return and paging data
            var page = entities.Select(
                e => new ValueLabelModel()
                {
                    Value = e.ID,
                    Label = e.TemplateTicketName
                }).ToList();

            return HttpResponse.OkList(page);
        }
    }

}
