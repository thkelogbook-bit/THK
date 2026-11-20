using Core.System.Runtime.Managers;
using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Helpers;
using Elogbook.Core.Interfaces;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseCommon;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.OperationDiary;
using Elogbook.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Elogbook.Domain.Services
{
    public class OperationDiaryService : IOperationDiaryService
    {
        #region Private fields

        private readonly ICommonService _commonService;
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fBold = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly IPdfSigningService _pdfSigningService;

        #endregion

        #region Contructor

        public OperationDiaryService(ElogbookUnitOfWork unitOfWork, ICommonService commonService, IPdfSigningService pdfSigningService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _pdfSigningService = pdfSigningService;
        }

        #endregion

        #region Public methods

        public ResponseBaseModel GetOperationDiarys(SearchOperationDiaryModel requestModel)
        {
            // Lấy toàn bộ dữ liệu theo cương vị
            var entities = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => !x.IsDeleted);
            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }

            // Tìm kiếm theo điều kiện
            if (requestModel.IsFilter)
            {
                // Tên người tạo
                if (!string.IsNullOrWhiteSpace(requestModel.FullName))
                {
                    entities = entities.Where(x => x.CreatedUser.FullName.Contains(requestModel.FullName));
                }
                // Ca vận hành
                if (requestModel.OperationShiftID > 0)
                {
                    entities = entities.Where(x => x.OperationShiftID == requestModel.OperationShiftID);
                }
                // Kíp vận hành
                if (requestModel.OperationCrewID > 0)
                {
                    entities = entities.Where(x => x.OperationCrewID == requestModel.OperationCrewID);
                }

                DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
                DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

                if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
                    entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= frDate.Date);

                if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
                    entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <= toDate.Date);
            }

            var results = entities.Select(
                e => new OperationDiaryModel()
                {
                    ID = e.ID,
                    CreateDate = e.CreatedDate,
                    Description = e.OperationShift.OperationShiftName + " - " + e.OperationCrew.OperationCrewName + " - " + e.CreatedUser.FullName,
                    Position = e.OperationShift != null ? e.OperationShift.Position : 0,
                    IsDelivered = e.IsDelivered,
                    IsReceived = e.IsReceived,
                    SyncDataKey = e.SyncDataKey,
                    AppID = e.AppID,
                }).OrderByDescending(x => x.CreateDate)
                .ThenBy(t => t.Position)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(results);
        }

        public ResponseBaseModel GetOperationDiarysWeb(SearchOperationDiaryWebModel requestModel)
        {
            // Dữ liệu Nhật ký vận hành đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => !x.IsDeleted);
            // Tên người tạo
            if (!string.IsNullOrWhiteSpace(requestModel.FullName))
            {
                entities = entities.Where(x => x.CreatedUser.FullName.Contains(requestModel.FullName));
            }
            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }
            // Ca vận hành
            if (requestModel.OperationShiftID > 0)
            {
                entities = entities.Where(x => x.OperationShiftID == requestModel.OperationShiftID);
            }
            // Kíp vận hành
            if (requestModel.OperationCrewID > 0)
            {
                entities = entities.Where(x => x.OperationCrewID == requestModel.OperationCrewID);
            }

            // Dữ liệu chi tiết của nhóm
            var masterDetail = entities;

            // Tìm kiếm Nhật ký vận hành theo ngày tạo
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= frDate.Date);

            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
                entities = entities.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <= toDate.Date);

            // Lấy dữ liệu nhóm sắp xếp theo ngày tạo giảm dần
            IPagedList<OperationDiaryModel> dataGroup = entities.Select(
                e => new OperationDiaryModel()
                {
                    GroupDate = DbFunctions.TruncateTime(e.CreatedDate),
                }).Distinct().OrderByDescending(x => x.GroupDate)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            var userRolesInfo = _commonService.GetUserRight(requestModel.CurrentUserID);
            List<BaseGroupDataModel<OperationDiaryModel>> results = new List<BaseGroupDataModel<OperationDiaryModel>>();
            foreach (var item in dataGroup)
            {
                var groupDate = item.GroupDate.GetValueOrDefault(DateTime.MinValue).ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
                var operationDiaryDetail = new BaseGroupDataModel<OperationDiaryModel>()
                {
                    GroupDate = groupDate,
                    MasterDetail = masterDetail.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == item.GroupDate.Value).Select(
                    e => new OperationDiaryModel()
                    {
                        ID = e.ID,
                        CreateDate = e.CreatedDate,
                        OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                        OperationShift = new ValueLabelModel { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                        OperationCrew = new ValueLabelModel { Value = e.OperationCrew.ID, Label = e.OperationCrew.OperationCrewName },
                        CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                        Position = e.OperationShift != null ? e.OperationShift.Position : 0,
                        IsReceived = e.IsReceived,
                        IsDelivered = e.IsDelivered,
                        OperationDiaryPositionShifts = e.OperationDiaryPositionInShift.Where(x => !x.IsDeleted).Select(
                                            t => new OperationDiaryPositionShiftModel()
                                            {
                                                PositionInShiftID = t.PositionInShiftID.HasValue ? t.PositionInShiftID.Value : 0,
                                                AssignedUser = new ValueLabelModel { Value = t.AssignedUser.ID, Label = t.AssignedUser.FullName },
                                                Position = t.Position,
                                            }).OrderBy(z => z.Position).ToList(),
                    }).OrderBy(x => x.Position).ToList()
                };

                // Lấy danh sách quyền của người dùng
                var isAllowEditDelete = false;
                var isAllowEdit = false;
                var isAllowDelete = false;
                if (userRolesInfo != null && userRolesInfo.UsersRight != null && userRolesInfo.UsersRight.Count > 0)
                {
                    // Kiểm tra người dùng có quyền cập nhật/xóa NKVH khi đã giao/nhận ca không
                    if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "EDIT_DELETE_NKVH"))
                    {
                        isAllowEditDelete = true;
                    }

                    // Kiểm tra người dùng có quyền cập nhật NKVH
                    if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "EDIT_NKVH"))
                    {
                        isAllowEdit = true;
                    }

                    // Kiểm tra người dùng có quyền xóa NKVH
                    if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "DELETE_NKVH"))
                    {
                        isAllowDelete = true;
                    }
                }
                var isAdmin = false;
                if (userRolesInfo != null)
                {
                    isAdmin = userRolesInfo.IsAdmin;
                }
                var shifts = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery().ToList();
                var position = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.JobTitle == (int)JobTitle.ShiftLeader).ToList();

                foreach (var detailItem in operationDiaryDetail.MasterDetail)
                {
                    // Danh sách id của những cương vị trong ca được khai báo trong ca ghi NKVH
                    List<int> lstPositionInShiftID = detailItem.OperationDiaryPositionShifts.Select(e => e.PositionInShiftID).ToList();
                    // Danh sách cương vị trong ca
                    var positionInShifts = shifts.Where(x => lstPositionInShiftID.Contains(x.ID)).ToList();
                    // Xác định Trưởng ca
                    var isHasLeaderShift = false;
                    // Id cương vị trong ca
                    var positionInShiftID = 0;
                    // Xác định danh sách cương vị trong ca có tồn tại Trưởng ca
                    foreach (var positionInShift in positionInShifts)
                    {
                        // Lấy cương vị Trưởng ca
                        OperationPosition operationPosition = null;
                        if (position != null)
                        {
                            operationPosition = position.Find(x => x.ID == positionInShift.OperationPositionID);
                        }
                        // Nếu có cương vị Trưởng ca
                        if (operationPosition != null)
                        {
                            isHasLeaderShift = true;
                            positionInShiftID = positionInShift.ID;
                            break;
                        }
                    }

                    // Lấy người dùng thuộc cương vị Trưởng ca
                    var assignedUserID = detailItem.OperationDiaryPositionShifts.Where(x => x.PositionInShiftID == positionInShiftID).Select(e => e.AssignedUser.Value).FirstOrDefault();
                    // Cần xác định ai là người được sửa/xóa NKVH, ví dụ:
                    // 1. Người đó không có tên trong danh sách ca trực thì có được cập nhật ca đó không
                    // 2. hoặc người đó không phải là người tạo NKVH
                    //(!detailItem.IsDelivered && isHasLeaderShift && assignedUserID == requestModel.CurrentUserID) -> chỉ trưởng ca của Ca đó mới được cập nhật
                    //-> thay đổi yêu cầu, nếu người dùng được phân quyền thì sẽ được add/update/delete NKVH
                    // Nếu cương vị Trưởng và người khai báo trùng với người login thì mới cho chỉnh sửa NKVH
                    var isCreator = detailItem.CreatedUser != null && detailItem.CreatedUser.Value == requestModel.CurrentUserID;
                    if (isAdmin
                        || (isAllowEdit && !detailItem.IsDelivered)
                        || (detailItem.IsDelivered && isHasLeaderShift && assignedUserID == requestModel.CurrentUserID && isAllowEditDelete)
                        || (isCreator && !detailItem.IsDelivered))
                    {
                        detailItem.IsCanEdit = true;
                    }

                    if (isAdmin
                        || (isAllowDelete && !detailItem.IsDelivered)
                        || (detailItem.IsDelivered && isHasLeaderShift && assignedUserID == requestModel.CurrentUserID && isAllowEditDelete)
                        || (isCreator && !detailItem.IsDelivered))
                    {
                        detailItem.IsCanDelete = true;
                    }
                }

                results.Add(operationDiaryDetail);
            }

            return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, dataGroup.TotalRecords, dataGroup.TotalPages);
        }

        public ResponseBaseModel QuickSearchOperationDiarys(QuickSearchOperationDiaryModel requestModel)
        {
            if (requestModel.FactoryID <= 0) requestModel.FactoryID = CommonConstant.ALL_KEY;
            if (string.IsNullOrWhiteSpace(requestModel.SearchText))
                requestModel.SearchText = string.Empty;
            else
                requestModel.SearchText = requestModel.SearchText.Trim();

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("PageNumber", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageNumber,
                },
                new SqlParameter("PageSize", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageSize,
                },
                new SqlParameter("Year", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.Year,
                },
                new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.FactoryID,
                },
                new SqlParameter("SearchText", System.Data.SqlDbType.NVarChar)
                {
                    Value = requestModel.SearchText,
                }
            };

            var results = _unitOfWork.DataContext.Database
                .SqlQuery<QuickSearchOperationDiaryResponseModel>("Quick_Search_NKVH @PageNumber, @PageSize, @Year, @FactoryID, @SearchText", sqlParams).ToList();

            var totalRecords = 0;
            var totalPages = 0;
            if (results != null && results.Count > 0)
            {
                foreach (var item in results)
                {
                    item.Description = item.HeaderContent + "\n" + item.Description;
                }

                totalRecords = results[0].TotalRecords;
                totalPages = totalRecords / requestModel.PageSize + (totalRecords % requestModel.PageSize > 0 ? 1 : 0);
            }

            return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
        }

        public ViewOpeartionDiaryModel ViewOperationDiary(ViewOperationDiaryRequestModel requestModel)
        {
            DateTime createdDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreatedDate, false);
            var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && x.OperationShiftID == requestModel.OperationShiftID
                                                                                        && DbFunctions.TruncateTime(x.CreatedDate) == createdDate.Date
                                                                                        && !x.IsDeleted).FirstOrDefault();
            if (operationDiary != null && operationDiary.ID > 0)
            {
                return GetOperationDiaryById(operationDiary.ID);
            }
            else
            {
                return null;
            }
        }

        public ViewOpeartionDiaryModel GetOperationDiaryById(int id)
        {
            var data = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new ViewOpeartionDiaryModel()
                {
                    ID = e.ID,
                    CreatedDate = e.CreatedDate,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    OperationShift = new ValueLabelModel { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                    OperationShiftInfo = new OperationShiftModel()
                    {
                        StartTime = SqlFunctions.StringConvert(e.OperationShift.StartTime, 6, 2).Trim().Replace(".", ":"),
                        EndTime = SqlFunctions.StringConvert(e.OperationShift.EndTime, 6, 2).Trim().Replace(".", ":"),
                        StartTimeNumber = e.OperationShift.StartTime,
                        EndTimeNumber = e.OperationShift.EndTime,
                    },
                    OperationCrew = new ValueLabelModel { Value = e.OperationCrew.ID, Label = e.OperationCrew.OperationCrewName },
                    CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    InsertedAt = e.InsertedAt,
                    LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
                    UpdatedAt = e.UpdatedAt,
                    PositionInShifts = e.OperationDiaryPositionInShift.Where(x => !x.IsDeleted).Select(
                                            t => new OperationDiaryPositionShiftModel()
                                            {
                                                ID = t.ID,
                                                PositionInShiftID = t.PositionInShift.ID,
                                                Label = t.PositionInShiftID.HasValue && t.PositionInShiftID.Value > 0 ? t.PositionInShift.OperationPosition.OperationPositionName : t.Label,
                                                AssignedUser = t.AssignedUser != null ? new ValueLabelModel { Value = t.AssignedUser.ID, Label = t.AssignedUser.FullName } : null,
                                                AvailableUser = new InternalExternalUserModel
                                                {
                                                    ID = t.AssignedUser != null ? t.AssignedUser.ID : 0,
                                                    FullName = t.AssignedUser != null ? t.AssignedUser.FullName : string.Empty,
                                                    TempDateOfBirth = t.AssignedUser != null ? t.AssignedUser.DateOfBirth : null,
                                                    ElectricRankValueStr = t.AssignedUser != null ? t.AssignedUser.ElectricRank : null,
                                                    ElectricRankCard = t.AssignedUser != null ? t.AssignedUser.ElectricRankCard : null,
                                                    UserType = "Nội bộ",
                                                    Unit = t.AssignedUser != null ? new ValueLabelModel
                                                    {
                                                        Value = t.AssignedUser.TeamID,
                                                        Label = t.AssignedUser.Team != null ? t.AssignedUser.Team.Name : string.Empty
                                                    } : null,
                                                    UserName = t.AssignedUser != null ? t.AssignedUser.UserName : string.Empty,
                                                },
                                                Position = t.Position,
                                                OperationPositionID = e.OperationPosition != null ? e.OperationPosition.ID : 0,
                                                PositionShiftID = t.PositionInShift != null ? t.PositionInShift.OperationPositionID : 0,
                                            }).OrderBy(z => z.Position).ToList(),
                    // 1.Tình hình vận hành
                    OperationSituations = e.OperationSituations.Where(x => !x.IsDeleted).Select(
                                            t => new OperationSituationModel()
                                            {
                                                ID = t.ID,
                                                OperationDiaryID = t.OperationDiaryID,
                                                Hour = SqlFunctions.StringConvert(t.Hour, 6, 2).Trim().Replace(".", ":"),
                                                HourFloat = t.Hour,
                                                AssignedFrom = new ValueLabelModel { Value = t.AssignedFrom.ID, Label = t.AssignedFrom != null ? t.AssignedFrom.FullName : t.AssignedFromName },
                                                AssignedTo = new ValueLabelModel { Value = t.AssignedTo.ID, Label = t.AssignedTo != null ? t.AssignedTo.FullName : t.AssignedToName },
                                                Description = t.Description,
                                                // Tài liệu liên quan (nếu có)
                                                Attackments = t.Attachments,
                                                InsertedAt = e.InsertedAt,
                                            }).OrderBy(t => t.InsertedAt).ToList(),
                    // Công tác đang thực hiện
                    //ActiveWorks = e.ActiveWorks.Where(x => !x.IsDeleted).Select(
                    //                        t => new ActiveWorkModel()
                    //                        {
                    //                            ID = t.ID,
                    //                            OperationDiaryID = t.OperationDiaryID,
                    //                            StartHour = SqlFunctions.StringConvert(t.StartHour, 6, 2).Trim().Replace(".", ":"),
                    //                            HourFloat = t.StartHour,
                    //                            EndHour = SqlFunctions.StringConvert(t.EndHour, 6, 2).Trim().Replace(".", ":"),
                    //                            Description = t.Description,
                    //                            // Tài liệu liên quan (nếu có)
                    //                            Attackments = t.Attachments,
                    //                            InsertedAt = e.InsertedAt,
                    //                        }).OrderBy(t => t.InsertedAt).ToList(),
                    // 2.Bất thường sự cố
                    // 3.Phương thức giao ca
                    // 4.Tồn tại trong ca
                    ExistInShiftInfo = e.ExistInShifts.Where(x => !x.IsDeleted).Select(
                                            t => new ExistInShiftModel()
                                            {
                                                ID = t.ID,
                                                OperationDiaryID = t.OperationDiaryID,
                                                RegistrationTicket = new TicketInforModel
                                                {
                                                    Value = t.RegistrationTicket.ID,
                                                    Label = t.RegistrationTicket != null ? t.RegistrationTicket.WorkTicketNumber : t.RegistrationTicketName
                                                },
                                                TeamLeader = t.TeamLeader,
                                                Area = t.Area,
                                                Content = t.Content,
                                                InsertedAt = t.InsertedAt,
                                                CreatedUser = new ValueLabelModel { Value = t.CreatedUser.ID, Label = t.CreatedUser.FullName },
                                                Status = t.StatusID,
                                                // Tài liệu liên quan (nếu có)
                                                Attachments = t.Attachments,
                                                ExistInShiftDetails = t.IssueOfShiftDetails.Where(x => !x.IsDeleted).Select(
                                                    v => new IssueOfShiftDetailModel()
                                                    {
                                                        ID = v.ID,
                                                        Content = v.Content,
                                                        CreateDate = v.InsertedAt,
                                                        CreatedUser = new ValueLabelModel
                                                        {
                                                            Value = v.CreatedUser.ID,
                                                            Label = v.CreatedUser.FullName
                                                        },
                                                        Attachments = v.Attachments
                                                    }).OrderBy(v => v.CreateDate).ToList()
                                            }).OrderBy(t => t.InsertedAt).ToList(),
                    // 5.Nhận xét thông số
                    Comments = e.Comments,
                    // 6.Giao/Nhận ca
                    DeliveryReceiveShiftInfo = e.DeliveryReceiveShift.Where(x => !x.IsDeleted).Select(
                                            t => new DeliveryReceiveShiftModel()
                                            {
                                                ID = t.ID,
                                                OperationDiaryID = t.OperationDiaryID,
                                                PositionShiftID = t.OperationDiaryPositionShiftID,
                                                Supervisor = new ValueLabelModel { Value = t.Supervisor.ID, Label = t.Supervisor.FullName },
                                                IsDeliveryShift = t.IsDeliveryShift,
                                                Signature = t.Supervisor != null ? t.Supervisor.Signature : string.Empty,
                                                InsertedAt = t.InsertedAt,
                                            }).ToList(),
                    IsReceived = e.IsReceived,
                    IsDelivered = e.IsDelivered,
                    TimeDeliveryCaching = e.TimeDelivery,
                    ShiftReceiving = e.ShiftReceiving,
                    PdfExportId = e.PdfExportId
                }).FirstOrDefault();

            // 1.Tình hình vận hành
            GetOperationSituationInfo(ref data);

            // Công tác đang thực hiện
            //GetActiveWorkInfo(ref data);

            // 2.Bất thường sự cố
            if (data != null)
            {
                // 2.Bất thường sự cố
                data.AbnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => x.OperationDiaryID == data.ID && !x.IsDeleted)
                    .Select(e => new AbnormalIncidentModel()
                    {
                        ID = e.ID,
                        Description = e.Description,
                        OperationDiaryID = e.OperationDiaryID,
                        Attackments = e.Attachments,
                    }).FirstOrDefault();

                if (data.AbnormalIncident != null)
                {
                    data.AbnormalIncident.FileAttachments = new List<AttachmentModel>();
                    var attachments = data.AbnormalIncident.Attackments?.Split(',');
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
                                    data.AbnormalIncident.FileAttachments.Add(new AttachmentModel
                                    {
                                        ID = att.ID,
                                        FileName = att.FileName,
                                        FilePath = _commonService.GetPathFile(att.FilePath),
                                        PathThumbnail = _commonService.GetPathFile(att.PathThumbnail),
                                        ScreenName = att.ScreenName,
                                        FileType = att.FileType,
                                        Description = att.Description,
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // 3.Phương thức giao ca
            if (data != null)
            {
                GetDeliveryShiftMethodInfo(ref data, data.ID);
            }

            // 4.Tồn tại trong ca
            GetExistInShiftInfo(ref data);

            // 6.Giao/Nhận ca
            GetDeliveryReceiveShiftInfo(ref data);

            // Cương vị trong ca
            GetPositionInShiftInfo(ref data);

            if (data != null && data.TimeDeliveryCaching != null)
            {
                data.TimeDelivery = data.TimeDeliveryCaching.GetValueOrDefault(DateTime.MinValue).ToString("HH:mm dd/MM/yyyy");
            }

            return data;
        }

        #region Thông tin chi tiết nhật ký vận hành

        /// <summary>
        /// Tình hình vận hành
        /// </summary>
        /// <param name="data"></param>
        private void GetOperationSituationInfo(ref ViewOpeartionDiaryModel data)
        {
            if (data != null && data.OperationSituations != null)
            {
                var isConcurrentlyInShift = false;
                var operationPosition = _unitOfWork.ElogbookRepository<OperationPosition>().GetById(data.OperationPosition.Value.GetValueOrDefault(0));
                if (operationPosition != null)
                    isConcurrentlyInShift = operationPosition.IsConcurrentlyInShift;
                data.OperationPosition.IsConcurrentlyInShift = isConcurrentlyInShift;
                var operationSituationsTemp = data.OperationSituations;
                foreach (var item in operationSituationsTemp)
                {
                    //Ca vận hành có thời gian bắt đầu nhỏ hơn thời gian kết thúc là Ca được thực hiện trong ngày
                    if (data.OperationShiftInfo.StartTimeNumber < data.OperationShiftInfo.EndTimeNumber)
                    {
                        //Nếu tình hình vận hành có thời gian nhỏ hơn thời gian bắt đầu của Ca thì xếp trên cùng
                        if (item.HourFloat < data.OperationShiftInfo.StartTimeNumber)
                        {
                            item.SortOrder = 1;
                        }
                        //Nếu tình hình vận hành nằm trong thời gian Ca thì xếp ở giữa
                        else if (item.HourFloat >= data.OperationShiftInfo.StartTimeNumber && item.HourFloat <= data.OperationShiftInfo.EndTimeNumber)
                        {
                            item.SortOrder = 2;
                        }
                        //Nếu tình hình vận hành lớn hơn thời gian kết thúc Ca thì xếp sau cùng
                        else
                        {
                            item.SortOrder = 3;
                        }
                    }
                    //Ca vận hành kéo dài qua ngày hôm sau
                    else
                    {
                        //Nếu tình hình vận hành nhỏ hơn thời gian bắt đầu Ca thì xếp trên cùng
                        if (item.HourFloat <= data.OperationShiftInfo.StartTimeNumber
                            && item.HourFloat > data.OperationShiftInfo.StartTimeNumber - data.OperationShiftInfo.EndTimeNumber)
                        {
                            item.SortOrder = 1;
                        }
                        //Nếu tình hình vận hành lớn hơn thời gian bắt đầu Ca và nhỏ hơn 24(chưa qua ngày) thì sắp xếp ở giữa
                        else if (item.HourFloat > data.OperationShiftInfo.StartTimeNumber && item.HourFloat < 24)
                        {
                            item.SortOrder = 2;
                        }
                        //Nếu tình hình vận hành đã qua ngày mới thì sắp xếp sau cùng
                        else
                        {
                            item.SortOrder = 3;
                        }
                    }
                }

                data.OperationSituations = new List<OperationSituationModel>();
                data.OperationSituations = operationSituationsTemp.OrderBy(x => x.SortOrder).ThenBy(x => x.HourFloat).ToList();

                foreach (var item in data.OperationSituations)
                {
                    item.FileAttachments = new List<AttachmentModel>();
                    var attachments = item?.Attackments?.Split(',');
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
                }
            }
        }

        /// <summary>
        /// Công tác đang thực hiện
        /// </summary>
        /// <param name="data"></param>
        private void GetActiveWorkInfo(ref ViewOpeartionDiaryModel data)
        {
            if (data != null && data.ActiveWorks != null)
            {
                foreach (var item in data.ActiveWorks)
                {
                    item.FileAttachments = new List<AttachmentModel>();
                    var attachments = item.Attackments?.Split(',');
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
                }
            }
        }

        /// <summary>
        /// Phương thức giao ca
        /// </summary>
        /// <param name="data"></param>
        private void GetDeliveryShiftMethodInfo(ref ViewOpeartionDiaryModel data, int operationDiaryID)
        {
            data.DeliveryShiftMethod = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().GetQuery(x => x.OperationDiaryID == operationDiaryID && !x.IsDeleted)
                .Select(e => new DeliveryShiftMethodModel()
                {
                    ID = e.ID,
                    Description = e.Description,
                    OperationDiaryID = e.OperationDiaryID,
                    Attackments = e.Attachments,
                }).FirstOrDefault();

            if (data.DeliveryShiftMethod != null)
            {
                data.DeliveryShiftMethod.FileAttachments = new List<AttachmentModel>();
                var attachments = data.DeliveryShiftMethod.Attackments?.Split(',');
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
                                data.DeliveryShiftMethod.FileAttachments.Add(new AttachmentModel
                                {
                                    ID = att.ID,
                                    FileName = att.FileName,
                                    FilePath = _commonService.GetPathFile(att.FilePath),
                                    PathThumbnail = _commonService.GetPathFile(att.PathThumbnail),
                                    ScreenName = att.ScreenName,
                                    FileType = att.FileType,
                                    Description = att.Description,
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        /// <param name="data"></param>
        private void GetExistInShiftInfo(ref ViewOpeartionDiaryModel data)
        {
            // file đính kèm tồn tại trong ca
            if (data != null && data.ExistInShiftInfo != null)
            {
                foreach (var item in data.ExistInShiftInfo)
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

                    if (item.RegistrationTicket.Value != null && item.RegistrationTicket.Value > 0)
                    {
                        item.RegistrationTicket.Commander = _unitOfWork.ElogbookRepository<LeadershipUser>()
                            .GetQuery(x => x.WorkTicketID == item.RegistrationTicket.Value && x.Roles == (int)LeadershipUserRoles.Commander && !x.IsDeleted)
                            .Select(z => z.FullName).FirstOrDefault();
                    }
                }
            }
        }

        /// <summary>
        /// Thông tin giao/nhận ca
        /// </summary>
        /// <param name="data"></param>
        private void GetDeliveryReceiveShiftInfo(ref ViewOpeartionDiaryModel data)
        {
            if (data != null && data.DeliveryReceiveShiftInfo != null)
            {
                data.DeliveryReceiveShiftInfo = data.DeliveryReceiveShiftInfo.Select(
                                                t => new DeliveryReceiveShiftModel()
                                                {
                                                    ID = t.ID,
                                                    OperationDiaryID = t.OperationDiaryID,
                                                    PositionShiftID = t.PositionShiftID,
                                                    Supervisor = t.Supervisor,
                                                    IsDeliveryShift = t.IsDeliveryShift,
                                                    Signature = t.Signature,
                                                    SignatureMobile = t.Signature != null ? t.Signature.Replace("data:image/jpeg;base64,", "") : string.Empty,
                                                    DeliverDate = t.InsertedAt.ToString("HH:mm dd/MM/yyyy"),
                                                }).ToList();
            }
        }

        /// <summary>
        /// Thông tin cương vị trong ca
        /// </summary>
        /// <param name="data"></param>
        private void GetPositionInShiftInfo(ref ViewOpeartionDiaryModel data)
        {
            if (data != null && data.PositionInShifts != null)
            {
                #region Danh sách các Cương vị ký giao/nhận ca

                var operationDiaryPositionShifts = new List<OperationDiaryPositionShiftModel>();
                var i = 1;
                var entities = data.PositionInShifts.Where(x => x.OperationPositionID == x.PositionShiftID);

                foreach (var item in entities)
                {
                    //if (entities.Count() == 2 && item.Label.ToLower().Trim() == "Trưởng ca".ToLower())
                    //{
                    //    //Cập nhật tên Cương vị thêm hậu tố (1, 2...) trong trường hợp Cương vị có nhiều người trực
                    //    operationDiaryPositionShifts.Add(new OperationDiaryPositionShiftModel()
                    //    {
                    //        ID = item.ID,
                    //        Label = i == 1 ? "Trưởng ca" : "Trưởng kíp",
                    //        AssignedUser = item.AssignedUser,
                    //        Position = i,
                    //    });
                    //}
                    //else
                    //{
                    //    //Cập nhật tên Cương vị thêm hậu tố (1, 2...) trong trường hợp Cương vị có nhiều người trực
                    //    operationDiaryPositionShifts.Add(new OperationDiaryPositionShiftModel()
                    //    {
                    //        ID = item.ID,
                    //        Label = entities.Count() > 1 ? item.Label + $" ({i})" : item.Label,
                    //        AssignedUser = item.AssignedUser,
                    //        Position = i,
                    //    });
                    //}
                    //Cập nhật tên Cương vị thêm hậu tố (1, 2...) trong trường hợp Cương vị có nhiều người trực
                    operationDiaryPositionShifts.Add(new OperationDiaryPositionShiftModel()
                    {
                        ID = item.ID,
                        Label = entities.Count() > 1 ? item.Label + $" ({i})" : item.Label,
                        AssignedUser = item.AssignedUser,
                        Position = i,
                    });
                    i++;
                }
                data.OperationDiaryPositionShifts = operationDiaryPositionShifts;

                #endregion Danh sách các Cương vị ký giao/nhận ca

                //Danh sách người dùng được cấu hình để ký giao ca
                if (data.PositionInShifts.Count > 0)
                    data.AvailableUsers = data.PositionInShifts.Select(x => x.AvailableUser).ToList();

                foreach (var item in data.PositionInShifts)
                {
                    //Cập nhật tên Cương vị thêm hậu tố (1, 2...) trong trường hợp Cương vị có nhiều người trực để hiển thị ở phần thông tin chung
                    var label = operationDiaryPositionShifts.Where(x => x.ID == item.ID).Select(t => t.Label).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(label))
                        item.Label = label;
                }
            }
        }

        #endregion Thông tin chi tiết nhật ký vận hành

        public int AddOperationDiary(AddOperationDiary requestModel)
        {
            var entity = new OperationDiary()
            {
                CreatedDate = requestModel.CreatedDate,
                OperationShiftID = requestModel.OperationShiftID,
                OperationCrewID = requestModel.OperationCrewID,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            // Mobile lúc ghi cần xác định được cương vị đó thuộc nhà máy nào
            var templateDeliveryShiftMethod = string.Empty;
            entity.FactoryID = requestModel.FactoryID;
            if (requestModel.OperationPositionID > 0)
            {
                entity.OperationPositionID = requestModel.OperationPositionID;
                var entityOperationPosition = _unitOfWork.ElogbookRepository<OperationPosition>().GetById(requestModel.OperationPositionID);
                if (entityOperationPosition != null)
                {
                    templateDeliveryShiftMethod = entityOperationPosition.TemplateDeliveryShiftMethod;
                    entity.ShiftReceiving = entityOperationPosition.ShiftReceivingTemplate;
                }
            }
            string syncDataKey = requestModel.CreatedDate.ToString("ddMMyyy") +
                                "_" + requestModel.FactoryID +
                                "_" + requestModel.OperationPositionID +
                                "_" + requestModel.OperationShiftID;

            // Danh sách cương vị trong ca vận hành
            entity.OperationDiaryPositionInShift = requestModel.OperationDiaryPositionShift
                 .Select(e => new OperationDiaryPositionShift()
                 {
                     SyncDataKey = syncDataKey,
                     PositionInShiftID = e.PositionInShiftID.GetValueOrDefault(0) > 0 ? e.PositionInShiftID : null,
                     Label = e.Label,
                     AssignedUserID = e.AssignedUserID > 0 ? e.AssignedUserID : null,
                     IsDefault = e.IsDefault,
                     Position = e.Position,
                     CreatedUserID = requestModel.CurrentUserID,
                     LastUpdatedUserID = requestModel.CurrentUserID,
                     SyncedDate = DateTime.Now,
                 }).OrderBy(z => z.Position).ToList();

            entity.SyncDataKey = syncDataKey;
            entity.SyncedDate = DateTime.Now;
            _unitOfWork.ElogbookRepository<OperationDiary>().Add(entity);

            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                var deliveryShiftMethod = new DeliveryShiftMethod()
                {
                    SyncDataKey = syncDataKey,
                    OperationDiaryID = entity.ID,
                    Description = templateDeliveryShiftMethod,
                    Attachments = null,
                    SyncedDate = DateTime.Now,
                    FactoryID = requestModel.FactoryID,
                };
                _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Add(deliveryShiftMethod);
                _unitOfWork.Complete();

                return entity.ID;
            }
            else
            {
                return 0;
            }
        }

        public bool IsCreator(int operationDiaryID, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationDiary>().GetById(operationDiaryID);
            return entity != null && entity.CreatedUserID == userID;
        }

        public bool IsCreatorByOperationSituation(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationSituation>().GetById(id);
            return entity != null && entity.CreatedUserID == userID;
        }

        public bool IsCreatorByActiveWork(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<ActiveWork>().GetById(id);
            return entity != null && entity.CreatedUserID == userID;
        }

        public bool IsCreatorByExistInShift(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<ExistInShift>().GetById(id);
            return entity != null && entity.CreatedUserID == userID;
        }

        public bool UpdateOperationDiary(EditOperationDiary requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationDiary>().GetById(requestModel.OperationDiaryID);
            if (entity != null)
            {
                switch (requestModel.TabIndex)
                {
                    //Cập nhật khai báo cương vị trong ca
                    case 0:
                        foreach (var item in requestModel.PositionInShifts)
                        {
                            var positionShift = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetById(item.ID);
                            if (positionShift != null)
                            {
                                if (item.AssignedUser != null)
                                {
                                    positionShift.AssignedUserID = item.AssignedUser.Value > 0 ? item.AssignedUser.Value : null;
                                }
                                else
                                {
                                    positionShift.AssignedUserID = null;
                                }
                                positionShift.LastUpdatedUserID = requestModel.CurrentUserID;
                                positionShift.SyncedDate = DateTime.Now;
                                _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().Update(positionShift);
                            }
                        }
                        break;

                    // 1.Cập nhật tình hình vận hành
                    case (int)OperationDiaryTabName.Situation:
                        if (requestModel.OperationSituation != null)
                        {
                            // Cập nhật tình hình vận hành
                            if (requestModel.OperationSituation.ID > 0)
                            {
                                var updateEntity = entity.OperationSituations.FirstOrDefault(t => t.ID == requestModel.OperationSituation.ID);
                                if (updateEntity?.ID > 0)
                                {
                                    double.TryParse(requestModel.OperationSituation.Hour.Replace(":", "."), out double hour);
                                    updateEntity.Hour = hour;
                                    updateEntity.AssignedToID = requestModel.OperationSituation.AssignedToID.GetValueOrDefault(0) > 0
                                        ? requestModel.OperationSituation.AssignedToID : null;

                                    updateEntity.AssignedToName = !string.IsNullOrWhiteSpace(requestModel.OperationSituation.AssignedToName)
                                        ? requestModel.OperationSituation.AssignedToName.Trim() : requestModel.OperationSituation.AssignedToName;

                                    updateEntity.AssignedFromID = requestModel.OperationSituation.AssignedFromID.GetValueOrDefault(0) > 0
                                        ? requestModel.OperationSituation.AssignedFromID : null;
                                    updateEntity.AssignedFromName = !string.IsNullOrWhiteSpace(requestModel.OperationSituation.AssignedFromName)
                                        ? requestModel.OperationSituation.AssignedFromName.Trim() : requestModel.OperationSituation.AssignedFromName;
                                    updateEntity.Description = requestModel.OperationSituation.Description;
                                    updateEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                    _commonService.DeleteAttachFileUnuse(updateEntity.Attachments, requestModel.OperationSituation.Attachments, requestModel.CurrentUserID);
                                    updateEntity.Attachments = requestModel.OperationSituation.Attachments != null ? string.Join(",", requestModel.OperationSituation.Attachments) : string.Empty;
                                    updateEntity.SyncedDate = DateTime.Now;
                                    updateEntity.FactoryID = entity.FactoryID;
                                    _unitOfWork.ElogbookRepository<OperationSituation>().Update(updateEntity);
                                }
                            }
                            // Thêm mới tình hình vận hành
                            else
                            {
                                double.TryParse(requestModel.OperationSituation.Hour.Replace(":", "."), out double hour);
                                var addEntity = new OperationSituation()
                                {
                                    SyncDataKey = entity.SyncDataKey,
                                    OperationDiaryID = requestModel.OperationDiaryID,
                                    Hour = hour,
                                    AssignedFromID = requestModel.OperationSituation.AssignedFromID.GetValueOrDefault(0) > 0
                                        ? requestModel.OperationSituation.AssignedFromID : null,
                                    AssignedFromName = !string.IsNullOrWhiteSpace(requestModel.OperationSituation.AssignedFromName)
                                        ? requestModel.OperationSituation.AssignedFromName.Trim() : requestModel.OperationSituation.AssignedFromName,
                                    AssignedToID = requestModel.OperationSituation.AssignedToID.GetValueOrDefault(0) > 0
                                        ? requestModel.OperationSituation.AssignedToID : null,

                                    AssignedToName = !string.IsNullOrWhiteSpace(requestModel.OperationSituation.AssignedToName)
                                        ? requestModel.OperationSituation.AssignedToName.Trim() : requestModel.OperationSituation.AssignedToName,

                                    Description = requestModel.OperationSituation.Description,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    Attachments = requestModel.OperationSituation.Attachments != null ? string.Join(",", requestModel.OperationSituation.Attachments) : string.Empty
                                };
                                addEntity.SyncedDate = DateTime.Now;
                                addEntity.FactoryID = entity.FactoryID;
                                _unitOfWork.ElogbookRepository<OperationSituation>().Add(addEntity);
                            }
                        }
                        break;

                    // 2.Bất thường sự cố
                    case (int)OperationDiaryTabName.Incident:
                        if (requestModel.AbnormalIncident != null)
                        {
                            // Cập nhật bất thường sự cố
                            if (requestModel.AbnormalIncident.ID > 0)
                            {
                                var updateEntity = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(t => t.ID == requestModel.AbnormalIncident.ID).FirstOrDefault();
                                if (updateEntity?.ID > 0)
                                {
                                    updateEntity.Description = requestModel.AbnormalIncident.Description;
                                    updateEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                    updateEntity.SyncedDate = DateTime.Now;
                                    _commonService.DeleteAttachFileUnuse(updateEntity.Attachments, requestModel.AbnormalIncident.Attachments, requestModel.CurrentUserID);
                                    updateEntity.Attachments = requestModel.AbnormalIncident.Attachments != null ? string.Join(",", requestModel.AbnormalIncident.Attachments) : string.Empty;
                                    updateEntity.FactoryID = entity.FactoryID;
                                    _unitOfWork.ElogbookRepository<AbnormalIncident>().Update(updateEntity);
                                }
                            }
                            // Thêm mới bất thường sự cố
                            else
                            {
                                var abnormalIncident = new AbnormalIncident
                                {
                                    SyncDataKey = entity.SyncDataKey,
                                    OperationDiaryID = requestModel.OperationDiaryID,
                                    Description = requestModel.AbnormalIncident.Description,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    Attachments = requestModel.AbnormalIncident.Attachments != null ? string.Join(",", requestModel.AbnormalIncident.Attachments) : string.Empty,
                                    SyncedDate = DateTime.Now,
                                    FactoryID = entity.FactoryID
                                };
                                _unitOfWork.ElogbookRepository<AbnormalIncident>().Add(abnormalIncident);
                            }
                        }
                        break;

                    // 3.Phương thức giao ca
                    case (int)OperationDiaryTabName.DeliveryShiftMethod:
                        if (requestModel.DeliveryShiftMethod != null)
                        {
                            //cập nhật
                            if (requestModel.DeliveryShiftMethod.ID > 0)
                            {
                                var updateEntity = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().GetQuery(x => x.ID == requestModel.DeliveryShiftMethod.ID
                                    && x.OperationDiaryID == requestModel.OperationDiaryID && !x.IsDeleted).FirstOrDefault();
                                if (updateEntity != null)
                                {
                                    updateEntity.Description = requestModel.DeliveryShiftMethod.Description;
                                    updateEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                    _commonService.DeleteAttachFileUnuse(updateEntity.Attachments, requestModel.DeliveryShiftMethod.Attachments, requestModel.CurrentUserID);
                                    updateEntity.Attachments = requestModel.DeliveryShiftMethod.Attachments != null ? string.Join(",", requestModel.DeliveryShiftMethod.Attachments) : string.Empty;
                                    updateEntity.SyncedDate = DateTime.Now;
                                    updateEntity.FactoryID = entity.FactoryID;
                                    //update Phương thức giao ca
                                    _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Update(updateEntity);
                                }
                            }
                            // thêm mới
                            else
                            {
                                var addEntity = new DeliveryShiftMethod()
                                {
                                    SyncDataKey = entity.SyncDataKey,
                                    OperationDiaryID = requestModel.OperationDiaryID,
                                    Description = requestModel.DeliveryShiftMethod.Description,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    Attachments = requestModel.DeliveryShiftMethod.Attachments != null ? string.Join(",", requestModel.DeliveryShiftMethod.Attachments) : string.Empty
                                };
                                addEntity.SyncedDate = DateTime.Now;
                                addEntity.FactoryID = entity.FactoryID;
                                _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Add(addEntity);
                            }
                        }
                        break;

                    // 4.Tồn tại trong ca
                    case (int)OperationDiaryTabName.ExistInShift:
                        if (requestModel.ExistInShift != null)
                        {
                            // Cập nhật tồn tại trong ca
                            if (requestModel.ExistInShift.ID > 0)
                            {
                                var updateEntity = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(t => t.ID == requestModel.ExistInShift.ID).FirstOrDefault();
                                if (updateEntity?.ID > 0)
                                {
                                    updateEntity.RegistrationTicketID = requestModel.ExistInShift.RegistrationTicketID.GetValueOrDefault(0) > 0
                                        ? requestModel.ExistInShift.RegistrationTicketID : null;
                                    updateEntity.RegistrationTicketName = !string.IsNullOrWhiteSpace(requestModel.ExistInShift.RegistrationTicketName)
                                        ? requestModel.ExistInShift.RegistrationTicketName.Trim() : requestModel.ExistInShift.RegistrationTicketName;
                                    updateEntity.TeamLeader = requestModel.ExistInShift.TeamLeader;
                                    updateEntity.Area = requestModel.ExistInShift.Area;
                                    updateEntity.Content = requestModel.ExistInShift.Content;
                                    updateEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                    _commonService.DeleteAttachFileUnuse(updateEntity.Attachments, requestModel.ExistInShift.Attachments, requestModel.CurrentUserID);
                                    updateEntity.Attachments = requestModel.ExistInShift.Attachments != null ? string.Join(",", requestModel.ExistInShift.Attachments) : string.Empty;
                                    updateEntity.SyncedDate = DateTime.Now;
                                    updateEntity.FactoryID = entity.FactoryID;
                                    _unitOfWork.ElogbookRepository<ExistInShift>().Update(updateEntity);
                                }
                            }
                            // Thêm mới tồn tại trong ca
                            else
                            {
                                var existInShift = new ExistInShift
                                {
                                    SyncDataKey = entity.SyncDataKey,
                                    OperationDiaryID = requestModel.OperationDiaryID,
                                    RegistrationTicketID = requestModel.ExistInShift.RegistrationTicketID.GetValueOrDefault(0) > 0
                                        ? requestModel.ExistInShift.RegistrationTicketID : null,
                                    RegistrationTicketName = !string.IsNullOrWhiteSpace(requestModel.ExistInShift.RegistrationTicketName)
                                        ? requestModel.ExistInShift.RegistrationTicketName.Trim() : requestModel.ExistInShift.RegistrationTicketName,
                                    TeamLeader = requestModel.ExistInShift.TeamLeader,
                                    Area = requestModel.ExistInShift.Area,
                                    Content = requestModel.ExistInShift.Content,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    Attachments = requestModel.ExistInShift.Attachments != null ? string.Join(",", requestModel.ExistInShift.Attachments) : string.Empty,
                                };
                                existInShift.SyncedDate = DateTime.Now;
                                existInShift.FactoryID = entity.FactoryID;
                                _unitOfWork.ElogbookRepository<ExistInShift>().Add(existInShift);
                            }
                        }
                        break;

                    // 5.Nhận xét thông số
                    case (int)OperationDiaryTabName.Reviewed:
                        entity.Comments = requestModel.Comments;
                        break;

                    // Tình hình lúc nhận ca
                    case (int)OperationDiaryTabName.ShiftReceiving:
                        entity.ShiftReceiving = requestModel.ShiftReceiving;
                        break;

                    // Công tác đang thực hiện
                    //case (int)OperationDiaryTabName.ActiveWork:
                    //    if (requestModel.ActiveWork != null)
                    //    {
                    //        // Cập nhật công tác đang thực hiện
                    //        if (requestModel.ActiveWork.ID > 0)
                    //        {
                    //            var updateEntity = entity.ActiveWorks.FirstOrDefault(t => t.ID == requestModel.ActiveWork.ID);
                    //            if (updateEntity?.ID > 0)
                    //            {
                    //                double.TryParse(requestModel.ActiveWork.StartHour.Replace(":", "."), out double startHour);
                    //                updateEntity.StartHour = startHour;
                    //                double.TryParse(requestModel.ActiveWork.EndHour.Replace(":", "."), out double endHour);
                    //                updateEntity.EndHour = endHour;
                    //                updateEntity.Description = requestModel.ActiveWork.Description;
                    //                updateEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                    //                _commonService.DeleteAttachFileUnuse(updateEntity.Attachments, requestModel.ActiveWork.Attachments, requestModel.CurrentUserID);
                    //                updateEntity.Attachments = requestModel.ActiveWork.Attachments != null ? string.Join(",", requestModel.ActiveWork.Attachments) : string.Empty;
                    //                updateEntity.SyncedDate = DateTime.Now;
                    //                updateEntity.FactoryID = entity.FactoryID;
                    //                _unitOfWork.ElogbookRepository<ActiveWork>().Update(updateEntity);
                    //            }
                    //        }
                    //        // Thêm mới công tác đang thực hiện
                    //        else
                    //        {
                    //            double.TryParse(requestModel.ActiveWork.StartHour.Replace(":", "."), out double startHour);
                    //            double.TryParse(requestModel.ActiveWork.EndHour.Replace(":", "."), out double endHour);
                    //            var addEntity = new ActiveWork
                    //            {
                    //                SyncDataKey = entity.SyncDataKey,
                    //                OperationDiaryID = requestModel.OperationDiaryID,
                    //                StartHour = startHour,
                    //                EndHour = endHour,
                    //                Description = requestModel.ActiveWork.Description,
                    //                CreatedUserID = requestModel.CurrentUserID,
                    //                LastUpdatedUserID = requestModel.CurrentUserID,
                    //                Attachments = requestModel.ActiveWork.Attachments != null ? string.Join(",", requestModel.ActiveWork.Attachments) : string.Empty,
                    //                SyncedDate = DateTime.Now,
                    //                FactoryID = entity.FactoryID
                    //            };
                    //            _unitOfWork.ElogbookRepository<ActiveWork>().Add(addEntity);
                    //        }
                    //    }
                    //    break;
                }

                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<OperationDiary>().Update(entity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public DeliveryReceiveShiftModel DeliveryReceiveShift(AddEditDeliveryReceiveShiftModel requestModel)
        {
            DeliveryReceiveShiftModel responseModel = new DeliveryReceiveShiftModel();
            // Nếu đã có transaction bên ngoài (ví dụ từ SyncDeliveryReceiveShifts / OperationDiarySyncDataV2),
            // tái sử dụng transaction đó thay vì mở transaction mới (EF6 không hỗ trợ nested transaction).
            bool ownsTransaction = _unitOfWork.DataContext.Database.CurrentTransaction == null;
            var transaction = ownsTransaction
                ? _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable)
                : _unitOfWork.DataContext.Database.CurrentTransaction;
            try
            {
                var diary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.ID == requestModel.OperationDiaryID).FirstOrDefault();
                if (diary != null)
                {
                    if (requestModel.IsDeliveryShift)
                    {
                        if (diary.IsDelivered)
                        {
                            responseModel.ErrorMessage = CommonConstant.DELIVERED_COMPLETED;
                            return responseModel;
                        }
                    }
                    else
                    {
                        if (diary.IsReceived)
                        {
                            responseModel.ErrorMessage = CommonConstant.RECEIVED_COMPLETED;
                            return responseModel;
                        }
                    }

                    var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.UserName.Equals(requestModel.UserName, StringComparison.OrdinalIgnoreCase)
                                                                            && !x.IsDeleted
                                                                            && x.Status == (int)AccessStatus.Active).FirstOrDefault();
                    if (userInfo == null)
                    {
                        responseModel.ErrorMessage = CommonConstant.ASSIGNED_USER_NOT_EXISTS;
                        return responseModel;
                    }

                    if (userInfo.SourceType == (int)UserSource.ADUser)
                    {
                        var aDUserInfo = ADHelper.ValidateADUser(requestModel.UserName, requestModel.Password);
                        if (aDUserInfo != null)
                        {
                            userInfo.UserName = aDUserInfo.UserName;
                            userInfo.FullName = aDUserInfo.FullName;
                            userInfo.Password = RuntimeManager.GeneratePasswordHash(requestModel.Password);
                            _unitOfWork.ElogbookRepository<User>().Update(userInfo);
                            _unitOfWork.Complete();
                        }
                        else
                        {
                            responseModel.ErrorMessage = CommonConstant.PASSWORD_INVALID;
                            return responseModel;
                        }
                    }

                    if (requestModel.SkipPasswordValidation || !userInfo.SigningMethod.SigningMethodName.Equals(CommonConstant.SIGNING_USERNAME_PASSWORD, StringComparison.OrdinalIgnoreCase) || RuntimeManager.ValidatePassword(requestModel.Password, userInfo.Password))
                    {
                        var deliveryReceiveUsers = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.OperationDiaryID == requestModel.OperationDiaryID
                                                                                                                  && !x.IsDeleted).ToList();
                        var operationDiaryPositionShift = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.OperationDiaryID == requestModel.OperationDiaryID
                                                                                                                            && x.PositionInShift.OperationPositionID == diary.OperationPositionID
                                                                                                                            && !x.IsDeleted).ToList();
                        // Nhân viên ký giao ca không trùng với nhân viên được khai báo khi tạo nhật ký vận hành
                        if (requestModel.IsDeliveryShift)
                        {
                            if (deliveryReceiveUsers.Any(x => x.ID == requestModel.PositionShiftID && x.IsDeliveryShift))
                            {
                                responseModel.ErrorMessage = CommonConstant.USER_DELIVERED_COMPLETED;
                                return responseModel;
                            }

                            var deliveryInfo = operationDiaryPositionShift.FirstOrDefault(x => x.ID == requestModel.PositionShiftID);
                            if (deliveryInfo == null || deliveryInfo.AssignedUserID != userInfo.ID)
                            {
                                responseModel.ErrorMessage = CommonConstant.DELIVERY_NOTMAP_USER_DEFINE;
                                return responseModel;
                            }
                        }
                        else
                        {
                            if (deliveryReceiveUsers.Any(x => x.ID == requestModel.PositionShiftID && !x.IsDeliveryShift))
                            {
                                responseModel.ErrorMessage = CommonConstant.USER_RECEIVED_COMPLETED;
                                return responseModel;
                            }

                            // Nhân viên nhận ca không được trùng với nhân viên giao ca
                            if (operationDiaryPositionShift.Any(x => x.AssignedUserID == userInfo.ID))
                            {
                                responseModel.ErrorMessage = CommonConstant.RECEIVE_DELIVERY_USER;
                                return responseModel;
                            }

                            // Nhân viên nhận ca không được phép trùng nhau
                            if (deliveryReceiveUsers.Any(x => !x.IsDeliveryShift && x.SupervisorID == userInfo.ID))
                            {
                                responseModel.ErrorMessage = CommonConstant.RECEIVE_DUPLICATE_USER;
                                return responseModel;
                            }
                        }
                        // Khởi tạo giá trị mặc định là thời gian hiện tại
                        DateTime signDate = DateTime.Now;

                        if (!string.IsNullOrEmpty(requestModel.SignTime) && TimeSpan.TryParse(requestModel.SignTime, out var parsedTime))
                        {
                            // Nếu parse thành công, lấy Ngày hôm nay + Giờ truyền vào
                            signDate = DateTime.Today.Add(parsedTime);
                        }
                        var entity = new DeliveryReceiveShift()
                        {
                            SyncDataKey = diary.SyncDataKey,
                            OperationDiaryID = requestModel.OperationDiaryID,
                            OperationDiaryPositionShiftID = requestModel.PositionShiftID,
                            SupervisorID = userInfo.ID,
                            IsDeliveryShift = requestModel.IsDeliveryShift,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            SyncedDate = signDate,
                        };

                        _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().Add(entity);
                        if (diary != null)
                        {
                            var totalAssignedUser = operationDiaryPositionShift.Count(x => x.AssignedUserID > 0 && x.AssignedUserID != null);
                            var totalReceivedUser = deliveryReceiveUsers.Count(x => !x.IsDeliveryShift);
                            if (!requestModel.IsDeliveryShift)
                                totalReceivedUser++;
                            var totalDeliveredUser = deliveryReceiveUsers.Count(x => x.IsDeliveryShift);
                            if (requestModel.IsDeliveryShift)
                                totalDeliveredUser++;

                            if (!requestModel.IsDeliveryShift)
                                diary.IsReceived = totalReceivedUser == totalAssignedUser;
                            if (requestModel.IsDeliveryShift)
                                diary.IsDelivered = totalDeliveredUser == totalAssignedUser;
                            if (diary.IsDelivered)
                                diary.TimeDelivery = DateTime.Now;
                            diary.LastUpdatedUserID = requestModel.CurrentUserID;
                            diary.SyncedDate = DateTime.Now;
                        }

                        if (deliveryReceiveUsers.Count > 0 && (diary.PdfExportId == null || diary.PdfExportId <= 0))
                        {
                            //Không làm gì khi đã có chữ ký trước đó rồi, và k có tạo file pdf
                            _unitOfWork.Complete();
                        }
                        else
                        {
                            //Export file PDF khi ký giao ca và lưu lại thông tin file PDF đã ký vào database
                            if (diary.PdfExportId == null || diary.PdfExportId <= 0)
                            {
                                int attId = ExportFilePdfApproval(requestModel.OperationDiaryID, userInfo.UserName);
                                diary.PdfExportId = attId;
                            }
                            SafeUpdate<OperationDiary>(diary);
                            _unitOfWork.Complete();
                            var operationDiary = GetOperationDiaryById(diary.ID);
                            int position = operationDiary.OperationDiaryPositionShifts.Where(x => x.ID == requestModel.PositionShiftID).Select(x => x.Position).FirstOrDefault();
                            var isSigned = SignPdfFile(requestModel.OperationDiaryID, userInfo.UserName, requestModel.IsDeliveryShift, position);
                            if (!isSigned.Data)
                            {
                                // Trường hợp hàm SignPdfFile trả về kết quả false (Thất bại)
                                if (ownsTransaction) transaction.Rollback(); // Bắt buộc rollback toàn bộ dữ liệu đã Add/Update ở trên
                                responseModel.ErrorMessage = isSigned.ErrorMessages.FirstOrDefault() ?? ("Ký file PDF thất bại");
                                return responseModel;
                            }
                        }

                        responseModel = new DeliveryReceiveShiftModel()
                        {
                            ID = entity.ID,
                            OperationDiaryID = requestModel.OperationDiaryID,
                            PositionShiftID = requestModel.PositionShiftID,
                            Supervisor = new ValueLabelModel { Value = userInfo.ID, Label = userInfo.FullName },
                            IsDeliveryShift = entity.IsDeliveryShift,
                            Signature = !string.IsNullOrWhiteSpace(userInfo.Signature) ? userInfo.Signature.Replace("data:image/jpeg;base64,", "") : string.Empty,
                            DeliverDate = entity.InsertedAt.ToString("HH:mm dd/MM/yyyy"),
                        };
                        if (ownsTransaction) transaction.Commit();
                    }
                    else
                    {
                        responseModel.ErrorMessage = CommonConstant.PASSWORD_INVALID;
                    }
                }
                else
                {
                    responseModel.ErrorMessage = CommonConstant.OPERATION_DIARY_NOT_EXISTS;
                }
            }
            finally
            {
                if (ownsTransaction) transaction.Dispose();
            }
            return responseModel;
        }

        public bool DeleteOperationDiary(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationDiary>().GetById(id);
            if (entity != null)
            {
                // Danh sách cương vị trong ca
                var opPositionShifts = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(t => t.OperationDiaryID == id);
                foreach (var item in opPositionShifts)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().Delete(opPositionShifts, userID);

                // Tình hình vận hành
                var operationSituations = _unitOfWork.ElogbookRepository<OperationSituation>().GetQuery(t => t.OperationDiaryID == id);
                foreach (var item in operationSituations)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<OperationSituation>().Delete(operationSituations, userID);

                // Công tác đang thực hiện
                //var activeWorks = _unitOfWork.ElogbookRepository<ActiveWork>().GetQuery(t => t.OperationDiaryID == id);
                //foreach (var item in activeWorks)
                //{
                //    item.SyncedDate = DateTime.Now;
                //}
                //_unitOfWork.ElogbookRepository<ActiveWork>().Delete(activeWorks, userID);

                // Giao/nhận ca
                var deliveryReceiveShift = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(t => t.OperationDiaryID == id);
                foreach (var item in deliveryReceiveShift)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().Delete(deliveryReceiveShift, userID);

                // Tồn tại trong ca
                var existInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(t => t.OperationDiaryID == id);
                foreach (var item in existInShift)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<ExistInShift>().Delete(existInShift, userID);

                //Bất thường sự cố
                var abnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(t => t.OperationDiaryID == id);
                foreach (var item in abnormalIncident)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<AbnormalIncident>().Delete(abnormalIncident, userID);

                //Phương thức giao ca
                var deliveryShiftMethod = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().GetQuery(t => t.OperationDiaryID == id);
                foreach (var item in deliveryShiftMethod)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Delete(deliveryShiftMethod, userID);

                // Nhật ký vận hành
                entity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<OperationDiary>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteOperationSituation(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationSituation>().GetById(id);
            if (entity != null)
            {
                entity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<OperationSituation>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteActiveWork(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<ActiveWork>().GetById(id);
            if (entity != null)
            {
                entity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<ActiveWork>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteExistInShift(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<ExistInShift>().GetById(id);
            if (entity != null)
            {
                entity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<ExistInShift>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool CheckDuplicateData(DateTime createdDate, int operationShiftID, int factoryID, int operationPositionID)
        {
            var currentDate = DateTimeConvert.ConvertDateTimeFromString(createdDate.ToString("dd/MM/yyyy").Replace("-", "/"), true);
            return _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationShiftID == operationShiftID
                                                                                    && x.FactoryID == factoryID
                                                                                    && x.OperationPositionID == operationPositionID
                                                                                    && DbFunctions.TruncateTime(x.CreatedDate) == currentDate.Date
                                                                                    && !x.IsDeleted).Any();
        }

        public ValidateOpenShiftModel ValidateOpenShift(int factoryID, int operationPositionID)
        {
            var validateOpenShiftModel = new ValidateOpenShiftModel();

            var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationPositionID == operationPositionID
                                                                                   && x.FactoryID == factoryID
                                                                                   && !x.IsDeleted).OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ID).FirstOrDefault();
            if (operationDiary != null && !operationDiary.IsDelivered)
            {
                validateOpenShiftModel.OperationShiftName = operationDiary.OperationShift.OperationShiftName;
                validateOpenShiftModel.CreatedDate = $"{operationDiary.InsertedAt:dd/MM/yyyy}";
            }

            return validateOpenShiftModel;
        }

        public HttpResponseMessage ExportFile(int id, int curentUserID)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            var operationDiary = GetOperationDiaryById(id);
            if (operationDiary == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (operationDiary.PdfExportId.HasValue && operationDiary.PdfExportId > 0)
            {
                try
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == operationDiary.PdfExportId && !x.IsDeleted).FirstOrDefault();
                    if (attachment != null)
                    {
                        var outputFileAtt = Path.Combine(_rootPath, attachment.FilePath);
                        var fileAttBytes = File.ReadAllBytes(outputFileAtt);

                        if (fileAttBytes.Length > 0)
                        {
                            return Download(new ByteArrayContent(fileAttBytes), attachment.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService", $"Error exporting PDF PdfExportId {operationDiary.PdfExportId}: {ex.Message}");
                }
            }

            // tạo file PDF rùi ký vào file đó
            OperationDiaryPdfModel pdfDataModel = CreateDataPdf(operationDiary);
            if (!Directory.Exists(Path.Combine(_rootPath, "PDF")))
            {
                Directory.CreateDirectory(Path.Combine(_rootPath, "PDF"));
            }
            string fileName = $"Nhat_ky_van_hanh_{operationDiary.CreatedDate:yyyyMMdd}_{StringHelper.ToSnakeCaseSlug(operationDiary.OperationShift.Label)}_{StringHelper.ToSnakeCaseSlug(operationDiary.OperationCrew.Label)}_{operationDiary.ID}.pdf";
            string outputFile = Path.Combine(_rootPath, "PDF", fileName);
            bool isCreatePDF = CreatePDFContent(pdfDataModel, outputFile, true);

            if (!isCreatePDF) return new HttpResponseMessage(HttpStatusCode.NoContent);

            outputFile = outputFile.Replace(AppSettings.SERVER_DIR, AppSettings.FILE_STORAGE_SERVER_DIR);

            var fileBytes = File.ReadAllBytes(outputFile);
            if (fileBytes.Length > 0)
            {
                return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);

        }

        private ResponseModel<bool> SignPdfFile(int id, string username, bool isDeliveryShift, int position)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, "Signing PDF OperationDiaryService");
                var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
                if (operationDiary == null)
                {
                    responseModel.Data = false;
                }
                if (operationDiary.PdfExportId.HasValue || operationDiary.PdfExportId > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == operationDiary.PdfExportId && !x.IsDeleted).FirstOrDefault();
                    if (attachment != null)
                    {
                        var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
                        UserInfo user = _commonService.GetInternalUserById(username);
                        var outputFile = Path.Combine(_rootPath, attachment.FilePath);
                        if (user.SigningMethod.Label.Equals(CommonConstant.SIGNING_USERNAME_PASSWORD, StringComparison.OrdinalIgnoreCase))
                        {
                            LogCustom.LogMessage(TrackingLogLevel.INFO, "Signing PDF SIGNING_USERNAME_PASSWORD");
                            responseModel = _pdfSigningService.SignDocumentAndFillName(user, outputFile, isDeliveryShift ? $"Sig_Delivery_{position}" : $"Sig_Receive_{position}", user.FullName);
                        }
                        else if (user.SigningMethod.Label.Equals(CommonConstant.SIGNING_VIETTEL_CA, StringComparison.OrdinalIgnoreCase))
                        {
                            LogCustom.LogMessage(TrackingLogLevel.INFO, "Signing PDF SIGNING_VIETTEL_CA");
                            responseModel = _pdfSigningService.SignWithViettelMobileCA(user, $"Nhat ky van hanh {DateTime.Now:dd/MM/yyyy}-THK", outputFile, isDeliveryShift ? $"Sig_Delivery_{position}" : $"Sig_Receive_{position}", user.FullName);
                        }
                        else if (user.SigningMethod.Label.Equals(CommonConstant.SIGNING_VNPT_SMARTCA, StringComparison.OrdinalIgnoreCase))
                        {
                            LogCustom.LogMessage(TrackingLogLevel.INFO, "Signing PDF SIGNING_VNPT_SMARTCA ");
                            responseModel = _pdfSigningService.SignWithVnpt(user, $"Nhat ky van hanh {DateTime.Now:dd/MM/yyyy}-THK", outputFile, isDeliveryShift ? $"Sig_Delivery_{position}" : $"Sig_Receive_{position}", user.FullName);
                        }
                        else if (user.SigningMethod.Label.Equals(CommonConstant.SIGNING_VIETTEL_MYSIGN, StringComparison.OrdinalIgnoreCase))
                        {
                            LogCustom.LogMessage(TrackingLogLevel.INFO, "Signing PDF SIGNING_VIETTEL_MYSIGN");
                            responseModel = _pdfSigningService.SignWithViettelMySign(user, $"Nhat ky van hanh {DateTime.Now:dd/MM/yyyy}-THK", outputFile, isDeliveryShift ? $"Sig_Delivery_{position}" : $"Sig_Receive_{position}", user.FullName);
                        }
                        else
                        {
                            responseModel.Data = false;
                        }
                    }
                }
                else
                {
                    responseModel.ErrorMessages = new List<string> { "Lỗi tạo file pdf" };
                    responseModel.Data = false;
                }

            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService", $"Error signing PDF: {ex.Message}");
                responseModel.ErrorMessages = new List<string> { $"Error signing PDF: {ex.Message}" };
                responseModel.Data = false;
            }
            return responseModel;
        }

        private int ExportFilePdfApproval(int id, string username)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            var operationDiary = GetOperationDiaryById(id);

            if (operationDiary == null) return 0;

            // tạo file PDF rùi ký vào file đó
            OperationDiaryPdfModel pdfDataModel = CreateDataPdf(operationDiary);

            if (!Directory.Exists(Path.Combine(_rootPath, "PDF")))
            {
                Directory.CreateDirectory(Path.Combine(_rootPath, "PDF"));
            }

            string fileName = $"Nhat_ky_van_hanh_{operationDiary.CreatedDate:yyyyMMdd}_{StringHelper.ToSnakeCaseSlug(operationDiary.OperationShift.Label)}_{StringHelper.ToSnakeCaseSlug(operationDiary.OperationCrew.Label)}_{operationDiary.ID}.pdf";
            string outputFile = Path.Combine(_rootPath, "OperationDiary", "PDF", fileName);
            bool isCreatePDF = CreatePDFContent(pdfDataModel, outputFile);
            if (!isCreatePDF) return 0;
            outputFile = outputFile.Replace(AppSettings.SERVER_DIR, AppSettings.FILE_STORAGE_SERVER_DIR);
            var attachmentPdfCreated = new Attachment
            {
                FilePath = Path.Combine("OperationDiary", "PDF", fileName),
                FileName = fileName,
                ScreenName = "OperationDiary",
                FileType = "pdf",
            };
            _unitOfWork.ElogbookRepository<Attachment>().Add(attachmentPdfCreated);
            _unitOfWork.Complete();
            return attachmentPdfCreated.ID;
        }

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

        private ResponseBaseModel OperationDiarySyncDataV1(OperationDiarySyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            List<int> lstOperationDiaryID = new List<int>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (requestModel != null && requestModel.OperationDiarySyncs != null && requestModel.OperationDiarySyncs.Count > 0)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.DEBUG, "OperationDiaryService", Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel));

                        var listOperationDiaryAdd = new List<OperationDiarySyncModel>();
                        var listOperationDiaryUpdate = new List<OperationDiarySyncModel>();
                        foreach (var item in requestModel.OperationDiarySyncs)
                        {
                            if (item.ID > 0)
                            {
                                var itemDb = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.ID == item.ID && !x.IsDeleted).FirstOrDefault();
                                if (itemDb == null)
                                {
                                    listOperationDiaryAdd.Add(item);
                                }
                                else
                                {
                                    listOperationDiaryUpdate.Add(item);

                                    // Cập nhật sync key ở db
                                    if (item.SyncDataKey != itemDb.SyncDataKey)
                                    {
                                        itemDb.SyncDataKey = item.SyncDataKey;
                                        itemDb.OperationShiftID = item.OperationShiftID;
                                        itemDb.OperationCrewID = item.OperationCrewID;
                                        itemDb.SyncedDate = DateTime.Now;
                                        _unitOfWork.ElogbookRepository<OperationDiary>().Update(itemDb);
                                        _unitOfWork.Complete();
                                    }
                                }
                            }
                            else
                            {
                                listOperationDiaryAdd.Add(item);
                            }
                        }

                        if (listOperationDiaryAdd != null && listOperationDiaryAdd.Count > 0)
                        {
                            foreach (var item in listOperationDiaryAdd)
                            {
                                var itemDB = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.SyncDataKey.Equals(item.SyncDataKey) && !x.IsDeleted).FirstOrDefault();
                                // Nếu đã tồn tại thì không thêm mới
                                if (itemDB != null)
                                {
                                    item.ID = itemDB.ID;
                                    if (!lstOperationDiaryID.Contains(itemDB.ID))
                                        lstOperationDiaryID.Add(itemDB.ID);
                                    listOperationDiaryUpdate.Add(item);
                                    continue;
                                }

                                var operationDiary = new OperationDiary
                                {
                                    AppID = item.AppID,
                                    FactoryID = item.FactoryID,
                                    CreatedDate = item.CreatedDate,
                                    OperationPositionID = item.OperationPositionID,
                                    OperationShiftID = item.OperationShiftID,
                                    OperationCrewID = item.OperationCrewID,
                                    Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : null,
                                    IsCanEdit = item.IsCanEdit,
                                    TimeDelivery = item.TimeDelivery,
                                    SyncDataKey = item.SyncDataKey,
                                    IsDeleted = item.IsDeleted,
                                    CreatedUserID = item.CreatedUserID,
                                    InsertedAt = item.InsertedAt,
                                    LastUpdatedUserID = item.LastUpdatedUserID,
                                    UpdatedAt = item.UpdatedAt,
                                    SyncedDate = DateTime.Now,
                                    ShiftReceiving = !string.IsNullOrEmpty(item.ShiftReceiving) ? item.ShiftReceiving : null,
                                };
                                _unitOfWork.ElogbookRepository<OperationDiary>().Add(operationDiary);
                                _unitOfWork.Complete();
                                if (!lstOperationDiaryID.Contains(operationDiary.ID))
                                    lstOperationDiaryID.Add(operationDiary.ID);

                                // Danh sách cương vị trong ca
                                if (requestModel.OperationDiaryPositionShiftSyncs != null && requestModel.OperationDiaryPositionShiftSyncs.Count > 0)
                                {
                                    var operationDiaryPositionShiftAdd = requestModel.OperationDiaryPositionShiftSyncs.Where(x => x.SyncDataKey == item.SyncDataKey && !x.IsDeleted && x.AssignedUserID > 0).ToList();

                                    if (operationDiaryPositionShiftAdd != null && operationDiaryPositionShiftAdd.Count > 0)
                                    {
                                        foreach (var odps in operationDiaryPositionShiftAdd)
                                        {
                                            OperationDiaryPositionShift operationDiaryPositionShift = new OperationDiaryPositionShift
                                            {
                                                AppID = odps.AppID,
                                                OperationDiaryID = operationDiary.ID,
                                                PositionInShiftID = odps.PositionInShiftID != null ? odps.PositionInShiftID : null,
                                                Label = !string.IsNullOrEmpty(odps.Label) ? odps.Label : null,
                                                IsDefault = odps.IsDefault,
                                                Position = odps.Position,
                                                SyncDataKey = odps.SyncDataKey,
                                                IsDeleted = odps.IsDeleted,
                                                CreatedUserID = odps.CreatedUserID,
                                                InsertedAt = odps.InsertedAt,
                                                LastUpdatedUserID = odps.LastUpdatedUserID,
                                                UpdatedAt = odps.UpdatedAt,
                                                SyncedDate = DateTime.Now
                                            };

                                            if (odps.AssignedUserID > 0)
                                                operationDiaryPositionShift.AssignedUserID = odps.AssignedUserID;
                                            _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().Add(operationDiaryPositionShift);
                                        }
                                        _unitOfWork.Complete();
                                    }
                                }

                                // Bất thường sự cố
                                if (requestModel.AbnormalIncidentSyncs != null && requestModel.AbnormalIncidentSyncs.Count > 0)
                                {
                                    var abnormalIncidentAdd = requestModel.AbnormalIncidentSyncs.FirstOrDefault(x => x.SyncDataKey == item.SyncDataKey && !x.IsDeleted);
                                    if (abnormalIncidentAdd != null)
                                    {
                                        AbnormalIncident abnormalIncident = new AbnormalIncident
                                        {
                                            AppID = abnormalIncidentAdd.AppID,
                                            OperationDiaryID = operationDiary.ID,
                                            Description = !string.IsNullOrEmpty(abnormalIncidentAdd.Description) ? abnormalIncidentAdd.Description : null,
                                            Attachments = abnormalIncidentAdd.Attachments != null ? string.Join(",", abnormalIncidentAdd.Attachments) : string.Empty,
                                            SyncDataKey = abnormalIncidentAdd.SyncDataKey,
                                            IsDeleted = abnormalIncidentAdd.IsDeleted,
                                            CreatedUserID = abnormalIncidentAdd.CreatedUserID,
                                            InsertedAt = abnormalIncidentAdd.InsertedAt,
                                            LastUpdatedUserID = abnormalIncidentAdd.LastUpdatedUserID,
                                            UpdatedAt = abnormalIncidentAdd.UpdatedAt,
                                            SyncedDate = DateTime.Now,
                                            FactoryID = operationDiary.FactoryID,
                                        };
                                        _unitOfWork.ElogbookRepository<AbnormalIncident>().Add(abnormalIncident);
                                        _unitOfWork.Complete();
                                    }
                                }

                                // Phương thức giao ca
                                if (requestModel.DeliveryShiftMethodSyncs != null && requestModel.DeliveryShiftMethodSyncs.Count > 0)
                                {
                                    var deliveryShiftMethodAdd = requestModel.DeliveryShiftMethodSyncs.FirstOrDefault(x => x.SyncDataKey == item.SyncDataKey && !x.IsDeleted);
                                    if (deliveryShiftMethodAdd != null)
                                    {
                                        DeliveryShiftMethod deliveryShiftMethod = new DeliveryShiftMethod
                                        {
                                            AppID = deliveryShiftMethodAdd.AppID,
                                            OperationDiaryID = operationDiary.ID,
                                            Description = !string.IsNullOrEmpty(deliveryShiftMethodAdd.Description) ? deliveryShiftMethodAdd.Description : null,
                                            Attachments = deliveryShiftMethodAdd.Attachments != null ? string.Join(",", deliveryShiftMethodAdd.Attachments) : string.Empty,
                                            SyncDataKey = deliveryShiftMethodAdd.SyncDataKey,
                                            IsDeleted = deliveryShiftMethodAdd.IsDeleted,
                                            CreatedUserID = deliveryShiftMethodAdd.CreatedUserID,
                                            InsertedAt = deliveryShiftMethodAdd.InsertedAt,
                                            LastUpdatedUserID = deliveryShiftMethodAdd.LastUpdatedUserID,
                                            UpdatedAt = deliveryShiftMethodAdd.UpdatedAt,
                                            SyncedDate = DateTime.Now,
                                            FactoryID = operationDiary.FactoryID,
                                        };
                                        _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Add(deliveryShiftMethod);
                                        _unitOfWork.Complete();
                                    }
                                }

                                // Giao/nhận ca
                                if (requestModel.DeliveryReceiveShiftSyncs != null && requestModel.DeliveryReceiveShiftSyncs.Count > 0)
                                {
                                    var deliveryReceiveShifts = requestModel.DeliveryReceiveShiftSyncs.Where(x => x.SyncDataKey == item.SyncDataKey && x.IsNew && !x.IsDeleted).ToList();
                                    if (deliveryReceiveShifts != null && deliveryReceiveShifts.Count > 0)
                                    {
                                        var syncError = SyncDeliveryReceiveShifts(requestModel.DeliveryReceiveShiftSyncs, operationDiary, item.SyncDataKey);
                                        if (!string.IsNullOrEmpty(syncError))
                                            throw new InvalidOperationException(syncError);
                                    }
                                }

                                // Tồn tại trong ca
                                if (requestModel.ExistInShiftSyncs != null && requestModel.ExistInShiftSyncs.Count > 0)
                                {
                                    var existInShifts = requestModel.ExistInShiftSyncs.Where(x => x.SyncDataKey == item.SyncDataKey && x.IsNew && !x.IsDeleted).ToList();
                                    if (existInShifts != null && existInShifts.Count > 0)
                                    {
                                        foreach (var eis in existInShifts)
                                        {
                                            ExistInShift existInShift = new ExistInShift
                                            {
                                                AppID = eis.AppID,
                                                OperationDiaryID = operationDiary.ID,
                                                RegistrationTicketName = !string.IsNullOrEmpty(eis.RegistrationTicketName) ? eis.RegistrationTicketName : null,
                                                TeamLeader = !string.IsNullOrEmpty(eis.TeamLeader) ? eis.TeamLeader : null,
                                                Area = !string.IsNullOrEmpty(eis.Area) ? eis.Area : null,
                                                Content = !string.IsNullOrEmpty(eis.Content) ? eis.Content : null,
                                                Attachments = eis.Attachments != null ? string.Join(",", eis.Attachments) : string.Empty,
                                                StatusID = eis.StatusID,
                                                IsDeleteIssueOfShift = eis.IsDeleteIssueOfShift,
                                                SyncDataKey = eis.SyncDataKey,
                                                IsDeleted = eis.IsDeleted,
                                                CreatedUserID = eis.CreatedUserID,
                                                InsertedAt = eis.InsertedAt,
                                                LastUpdatedUserID = eis.LastUpdatedUserID,
                                                UpdatedAt = eis.UpdatedAt,
                                                SyncedDate = DateTime.Now,
                                                FactoryID = operationDiary.FactoryID,
                                            };

                                            if (eis.CompletedDateTime != null && eis.CompletedDateTime.Year >= 1753)
                                            {
                                                existInShift.CompletedDateTime = eis.CompletedDateTime;
                                            }
                                            if (eis.RegistrationTicketID.GetValueOrDefault() > 0)
                                                existInShift.RegistrationTicketID = eis.RegistrationTicketID;

                                            _unitOfWork.ElogbookRepository<ExistInShift>().Add(existInShift);
                                        }
                                        _unitOfWork.Complete();
                                    }
                                }

                                // Tình hình vận hành
                                if (requestModel.OperationSituationSyncs != null && requestModel.OperationSituationSyncs.Count > 0)
                                {
                                    var operationSituations = requestModel.OperationSituationSyncs.Where(x => x.SyncDataKey == item.SyncDataKey && x.IsNew && !x.IsDeleted).ToList();
                                    if (operationSituations != null && operationSituations.Count > 0)
                                    {
                                        foreach (var os in operationSituations)
                                        {
                                            double.TryParse(os.Hour.Replace(":", "."), out double hour);
                                            OperationSituation operationSituation = new OperationSituation
                                            {
                                                AppID = os.AppID,
                                                OperationDiaryID = operationDiary.ID,
                                                Hour = hour,
                                                AssignedFromID = os.AssignedFromID != null ? os.AssignedFromID : null,
                                                AssignedFromName = !string.IsNullOrEmpty(os.AssignedFromName) ? os.AssignedFromName : null,
                                                AssignedToID = os.AssignedToID != null ? os.AssignedToID : null,
                                                AssignedToName = !string.IsNullOrEmpty(os.AssignedToName) ? os.AssignedToName : null,
                                                Description = !string.IsNullOrEmpty(os.Description) ? os.Description : null,
                                                Attachments = os.Attachments != null ? string.Join(",", os.Attachments) : string.Empty,
                                                SyncDataKey = os.SyncDataKey,
                                                IsDeleted = os.IsDeleted,
                                                CreatedUserID = os.CreatedUserID,
                                                InsertedAt = os.InsertedAt,
                                                LastUpdatedUserID = os.LastUpdatedUserID,
                                                UpdatedAt = os.UpdatedAt,
                                                SyncedDate = DateTime.Now,
                                                FactoryID = operationDiary.FactoryID,
                                            };

                                            _unitOfWork.ElogbookRepository<OperationSituation>().Add(operationSituation);
                                        }
                                        _unitOfWork.Complete();
                                    }
                                }
                            }
                        }

                        if (listOperationDiaryUpdate != null && listOperationDiaryUpdate.Count > 0)
                        {
                            foreach (var item in listOperationDiaryUpdate)
                            {
                                // Cập nhật thông tin chung nhật ký vận hành
                                var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.ID == item.ID && !x.IsDeleted).FirstOrDefault();
                                if (operationDiary != null)
                                {
                                    operationDiary.AppID = item.AppID;
                                    operationDiary.FactoryID = item.FactoryID;
                                    operationDiary.CreatedDate = item.CreatedDate;
                                    operationDiary.OperationPositionID = item.OperationPositionID;
                                    operationDiary.OperationShiftID = item.OperationShiftID;
                                    operationDiary.OperationCrewID = item.OperationCrewID;
                                    operationDiary.Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : null;
                                    operationDiary.IsReceived = item.IsReceived;
                                    operationDiary.IsDelivered = item.IsDelivered;
                                    operationDiary.IsCanEdit = item.IsCanEdit;
                                    operationDiary.TimeDelivery = item.TimeDelivery;
                                    operationDiary.IsDeleted = item.IsDeleted;
                                    operationDiary.LastUpdatedUserID = item.LastUpdatedUserID;
                                    operationDiary.UpdatedAt = item.UpdatedAt;
                                    operationDiary.SyncedDate = DateTime.Now;
                                    operationDiary.ShiftReceiving = !string.IsNullOrEmpty(item.ShiftReceiving) ? item.ShiftReceiving : null;
                                    _unitOfWork.ElogbookRepository<OperationDiary>().Update(operationDiary);
                                    _unitOfWork.Complete();
                                    if (!lstOperationDiaryID.Contains(operationDiary.ID))
                                        lstOperationDiaryID.Add(operationDiary.ID);

                                    // Cập nhật bất thường sự cố
                                    if (requestModel.AbnormalIncidentSyncs != null && requestModel.AbnormalIncidentSyncs.Count > 0)
                                    {
                                        var abnormalIncidentUpdate = requestModel.AbnormalIncidentSyncs.FirstOrDefault(x => x.OperationDiaryID == operationDiary.ID);
                                        var abnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>()
                                            .GetQuery(x => x.OperationDiaryID == operationDiary.ID).FirstOrDefault();
                                        // Cập nhật nếu tồn tại
                                        if (abnormalIncident != null && abnormalIncidentUpdate != null)
                                        {
                                            abnormalIncident.AppID = abnormalIncidentUpdate.AppID;
                                            abnormalIncident.OperationDiaryID = operationDiary.ID;
                                            abnormalIncident.Description = !string.IsNullOrEmpty(abnormalIncidentUpdate.Description) ? abnormalIncidentUpdate.Description : null;
                                            abnormalIncident.Attachments = abnormalIncidentUpdate.Attachments != null ? string.Join(",", abnormalIncidentUpdate.Attachments) : string.Empty;
                                            abnormalIncident.IsDeleted = abnormalIncidentUpdate.IsDeleted;
                                            abnormalIncident.LastUpdatedUserID = abnormalIncidentUpdate.LastUpdatedUserID;
                                            abnormalIncident.UpdatedAt = abnormalIncidentUpdate.UpdatedAt;
                                            abnormalIncident.SyncedDate = DateTime.Now;
                                            abnormalIncident.FactoryID = operationDiary.FactoryID;
                                            _unitOfWork.ElogbookRepository<AbnormalIncident>().Update(abnormalIncident);
                                            _unitOfWork.Complete();
                                        }

                                        if (abnormalIncident == null && abnormalIncidentUpdate != null)
                                        {
                                            AbnormalIncident abnormalIncidentAdd = new AbnormalIncident
                                            {
                                                AppID = abnormalIncidentUpdate.AppID,
                                                OperationDiaryID = operationDiary.ID,
                                                Description = !string.IsNullOrEmpty(abnormalIncidentUpdate.Description) ? abnormalIncidentUpdate.Description : null,
                                                Attachments = abnormalIncidentUpdate.Attachments != null ? string.Join(",", abnormalIncidentUpdate.Attachments) : string.Empty,
                                                SyncDataKey = abnormalIncidentUpdate.SyncDataKey,
                                                IsDeleted = abnormalIncidentUpdate.IsDeleted,
                                                CreatedUserID = abnormalIncidentUpdate.CreatedUserID,
                                                InsertedAt = abnormalIncidentUpdate.InsertedAt,
                                                LastUpdatedUserID = abnormalIncidentUpdate.LastUpdatedUserID,
                                                UpdatedAt = abnormalIncidentUpdate.UpdatedAt,
                                                SyncedDate = DateTime.Now,
                                                FactoryID = operationDiary.FactoryID,
                                            };
                                            _unitOfWork.ElogbookRepository<AbnormalIncident>().Add(abnormalIncidentAdd);
                                            _unitOfWork.Complete();
                                        }
                                    }

                                    // Cập nhật phương thức giao ca
                                    if (requestModel.DeliveryShiftMethodSyncs != null && requestModel.DeliveryShiftMethodSyncs.Count > 0)
                                    {
                                        var deliveryShiftMethodUpdate = requestModel.DeliveryShiftMethodSyncs.FirstOrDefault(x => x.OperationDiaryID == operationDiary.ID);
                                        var deliveryShiftMethod = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>()
                                            .GetQuery(x => x.OperationDiaryID == operationDiary.ID).FirstOrDefault();

                                        if (deliveryShiftMethod != null && deliveryShiftMethodUpdate != null)
                                        {
                                            deliveryShiftMethod.AppID = deliveryShiftMethodUpdate.AppID;
                                            deliveryShiftMethod.OperationDiaryID = operationDiary.ID;
                                            deliveryShiftMethod.Description = !string.IsNullOrEmpty(deliveryShiftMethodUpdate.Description) ? deliveryShiftMethodUpdate.Description : null;
                                            deliveryShiftMethod.Attachments = deliveryShiftMethodUpdate.Attachments != null ? string.Join(",", deliveryShiftMethodUpdate.Attachments) : string.Empty;
                                            deliveryShiftMethod.IsDeleted = deliveryShiftMethodUpdate.IsDeleted;
                                            deliveryShiftMethod.LastUpdatedUserID = deliveryShiftMethodUpdate.LastUpdatedUserID;
                                            deliveryShiftMethod.UpdatedAt = deliveryShiftMethodUpdate.UpdatedAt;
                                            deliveryShiftMethod.SyncedDate = DateTime.Now;
                                            deliveryShiftMethod.FactoryID = operationDiary.FactoryID;
                                            _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Update(deliveryShiftMethod);
                                            _unitOfWork.Complete();
                                        }

                                        if (deliveryShiftMethod == null && deliveryShiftMethodUpdate != null)
                                        {
                                            DeliveryShiftMethod deliveryShiftMethodAdd = new DeliveryShiftMethod
                                            {
                                                AppID = deliveryShiftMethodUpdate.AppID,
                                                OperationDiaryID = operationDiary.ID,
                                                Description = !string.IsNullOrEmpty(deliveryShiftMethodUpdate.Description) ? deliveryShiftMethodUpdate.Description : null,
                                                Attachments = deliveryShiftMethodUpdate.Attachments != null ? string.Join(",", deliveryShiftMethodUpdate.Attachments) : string.Empty,
                                                SyncDataKey = deliveryShiftMethodUpdate.SyncDataKey,
                                                IsDeleted = deliveryShiftMethodUpdate.IsDeleted,
                                                CreatedUserID = deliveryShiftMethodUpdate.CreatedUserID,
                                                InsertedAt = deliveryShiftMethodUpdate.InsertedAt,
                                                LastUpdatedUserID = deliveryShiftMethodUpdate.LastUpdatedUserID,
                                                UpdatedAt = deliveryShiftMethodUpdate.UpdatedAt,
                                                SyncedDate = DateTime.Now,
                                                FactoryID = operationDiary.FactoryID,
                                            };
                                            _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Add(deliveryShiftMethodAdd);
                                            _unitOfWork.Complete();
                                        }
                                    }

                                    // Giao/nhận ca
                                    if (requestModel.DeliveryReceiveShiftSyncs != null && requestModel.DeliveryReceiveShiftSyncs.Count > 0)
                                    {
                                        var deliveryReceiveShifts = requestModel.DeliveryReceiveShiftSyncs.Where(x => x.OperationDiaryID == operationDiary.ID && x.IsNew && !x.IsDeleted).ToList();
                                        if (deliveryReceiveShifts != null && deliveryReceiveShifts.Count > 0)
                                        {
                                            var deliveryReceiveUsers = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.OperationDiaryID == operationDiary.ID
                                                                                                              && !x.IsDeleted).ToList();
                                            var totalReceivedUser = deliveryReceiveUsers.Count(x => !x.IsDeliveryShift);
                                            var totalDeliveredUser = deliveryReceiveUsers.Count(x => x.IsDeliveryShift);
                                            var listID = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.OperationDiaryID == operationDiary.ID
                                                                                                                                                && x.PositionInShift.OperationPositionID == operationDiary.OperationPositionID
                                                                                                                                                && !x.IsDeleted).Select(x => x.ID).ToList();
                                            var totalAssignedUser = listID.Count;
                                            //Nhận ca
                                            int index = 0;
                                            var receiveShifts = deliveryReceiveShifts.Where(x => !x.IsDeliveryShift);
                                            foreach (var receiveItem in receiveShifts)
                                            {
                                                if (receiveItem.OperationDiaryPositionShiftID == 0)
                                                {
                                                    try
                                                    {
                                                        receiveItem.OperationDiaryPositionShiftID = listID[index];
                                                    }
                                                    catch
                                                    {
                                                        //Ignore case
                                                    }
                                                }
                                                index++;

                                                DeliveryReceiveShift deliveryReceiveShift = new DeliveryReceiveShift
                                                {
                                                    AppID = receiveItem.AppID,
                                                    OperationDiaryID = operationDiary.ID,
                                                    OperationDiaryPositionShiftID = receiveItem.OperationDiaryPositionShiftID,
                                                    SupervisorID = receiveItem.SupervisorID,
                                                    IsDeliveryShift = receiveItem.IsDeliveryShift,
                                                    SyncDataKey = receiveItem.SyncDataKey,
                                                    IsDeleted = receiveItem.IsDeleted,
                                                    CreatedUserID = receiveItem.CreatedUserID,
                                                    InsertedAt = receiveItem.InsertedAt,
                                                    LastUpdatedUserID = receiveItem.LastUpdatedUserID,
                                                    UpdatedAt = receiveItem.UpdatedAt,
                                                    SyncedDate = DateTime.Now
                                                };
                                                _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().Add(deliveryReceiveShift);

                                                if (!receiveItem.IsDeliveryShift)
                                                    totalReceivedUser++;
                                                if (!receiveItem.IsDeliveryShift)
                                                    operationDiary.IsReceived = totalReceivedUser == totalAssignedUser;
                                                operationDiary.LastUpdatedUserID = item.LastUpdatedUserID;
                                                operationDiary.SyncedDate = DateTime.Now;
                                                _unitOfWork.ElogbookRepository<OperationDiary>().Update(operationDiary);
                                            }

                                            //Giao ca
                                            var deliveryShifts = deliveryReceiveShifts.Where(x => x.IsDeliveryShift);
                                            index = 0;
                                            foreach (var deliveryItem in deliveryShifts)
                                            {
                                                if (deliveryItem.OperationDiaryPositionShiftID == 0)
                                                {
                                                    try
                                                    {
                                                        deliveryItem.OperationDiaryPositionShiftID = listID[index];
                                                    }
                                                    catch
                                                    {
                                                        //Ignore case
                                                    }
                                                }
                                                index++;

                                                DeliveryReceiveShift deliveryReceiveShift = new DeliveryReceiveShift
                                                {
                                                    AppID = deliveryItem.AppID,
                                                    OperationDiaryID = operationDiary.ID,
                                                    OperationDiaryPositionShiftID = deliveryItem.OperationDiaryPositionShiftID,
                                                    SupervisorID = deliveryItem.SupervisorID,
                                                    IsDeliveryShift = deliveryItem.IsDeliveryShift,
                                                    SyncDataKey = deliveryItem.SyncDataKey,
                                                    IsDeleted = deliveryItem.IsDeleted,
                                                    CreatedUserID = deliveryItem.CreatedUserID,
                                                    InsertedAt = deliveryItem.InsertedAt,
                                                    LastUpdatedUserID = deliveryItem.LastUpdatedUserID,
                                                    UpdatedAt = deliveryItem.UpdatedAt,
                                                    SyncedDate = DateTime.Now
                                                };
                                                _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().Add(deliveryReceiveShift);

                                                if (deliveryItem.IsDeliveryShift)
                                                    totalDeliveredUser++;

                                                if (deliveryItem.IsDeliveryShift)
                                                    operationDiary.IsDelivered = totalDeliveredUser == totalAssignedUser;
                                                if (operationDiary.IsDelivered
                                                    && (operationDiary.TimeDelivery == null || operationDiary.TimeDelivery == DateTime.MinValue
                                                        || item.TimeDelivery == null || item.TimeDelivery == DateTime.MinValue))
                                                {
                                                    operationDiary.TimeDelivery = DateTime.Now;
                                                }
                                                operationDiary.LastUpdatedUserID = item.LastUpdatedUserID;
                                                operationDiary.SyncedDate = DateTime.Now;
                                                _unitOfWork.ElogbookRepository<OperationDiary>().Update(operationDiary);
                                            }
                                            _unitOfWork.Complete();
                                        }
                                    }

                                    // Tồn tại trong ca
                                    if (requestModel.ExistInShiftSyncs != null && requestModel.ExistInShiftSyncs.Count > 0)
                                    {
                                        var existInShifts = requestModel.ExistInShiftSyncs.Where(x => x.OperationDiaryID == operationDiary.ID).ToList();
                                        if (existInShifts != null && existInShifts.Count > 0)
                                        {
                                            foreach (var eis in existInShifts)
                                            {
                                                // Thêm mới
                                                if (eis.IsNew)
                                                {
                                                    ExistInShift existInShift = new ExistInShift
                                                    {
                                                        AppID = eis.AppID,
                                                        OperationDiaryID = operationDiary.ID,
                                                        RegistrationTicketName = !string.IsNullOrEmpty(eis.RegistrationTicketName) ? eis.RegistrationTicketName : null,
                                                        TeamLeader = !string.IsNullOrEmpty(eis.TeamLeader) ? eis.TeamLeader : null,
                                                        Area = !string.IsNullOrEmpty(eis.Area) ? eis.Area : null,
                                                        Content = !string.IsNullOrEmpty(eis.Content) ? eis.Content : null,
                                                        Attachments = eis.Attachments != null ? string.Join(",", eis.Attachments) : string.Empty,
                                                        StatusID = eis.StatusID,
                                                        IsDeleteIssueOfShift = eis.IsDeleteIssueOfShift,
                                                        SyncDataKey = eis.SyncDataKey,
                                                        IsDeleted = eis.IsDeleted,
                                                        CreatedUserID = eis.CreatedUserID,
                                                        InsertedAt = eis.InsertedAt,
                                                        LastUpdatedUserID = eis.LastUpdatedUserID,
                                                        UpdatedAt = eis.UpdatedAt,
                                                        SyncedDate = DateTime.Now,
                                                        FactoryID = operationDiary.FactoryID,
                                                    };

                                                    if (eis.CompletedDateTime != null && eis.CompletedDateTime.Year >= 1753)
                                                    {
                                                        existInShift.CompletedDateTime = eis.CompletedDateTime;
                                                    }
                                                    if (eis.RegistrationTicketID.GetValueOrDefault() > 0)
                                                        existInShift.RegistrationTicketID = eis.RegistrationTicketID;

                                                    _unitOfWork.ElogbookRepository<ExistInShift>().Add(existInShift);
                                                    _unitOfWork.Complete();
                                                }
                                                // Cập nhật
                                                else
                                                {
                                                    var existInShiftUpdate = _unitOfWork.ElogbookRepository<ExistInShift>()
                                                        .GetQuery(x => x.OperationDiaryID == operationDiary.ID && x.ID == eis.ID && !x.IsDeleted).FirstOrDefault();

                                                    if (existInShiftUpdate != null)
                                                    {
                                                        existInShiftUpdate.AppID = eis.AppID;
                                                        existInShiftUpdate.OperationDiaryID = operationDiary.ID;
                                                        existInShiftUpdate.RegistrationTicketName = !string.IsNullOrEmpty(eis.RegistrationTicketName) ? eis.RegistrationTicketName : null;
                                                        existInShiftUpdate.TeamLeader = !string.IsNullOrEmpty(eis.TeamLeader) ? eis.TeamLeader : null;
                                                        existInShiftUpdate.Area = !string.IsNullOrEmpty(eis.Area) ? eis.Area : null;
                                                        existInShiftUpdate.Content = !string.IsNullOrEmpty(eis.Content) ? eis.Content : null;
                                                        existInShiftUpdate.Attachments = eis.Attachments != null ? string.Join(",", eis.Attachments) : string.Empty;
                                                        existInShiftUpdate.StatusID = eis.StatusID;
                                                        existInShiftUpdate.IsDeleteIssueOfShift = eis.IsDeleteIssueOfShift;
                                                        existInShiftUpdate.IsDeleted = eis.IsDeleted;
                                                        existInShiftUpdate.LastUpdatedUserID = eis.LastUpdatedUserID;
                                                        existInShiftUpdate.UpdatedAt = eis.UpdatedAt;
                                                        existInShiftUpdate.SyncedDate = DateTime.Now;
                                                        if (eis.CompletedDateTime != null && eis.CompletedDateTime.Year >= 1753)
                                                        {
                                                            existInShiftUpdate.CompletedDateTime = eis.CompletedDateTime;
                                                        }
                                                        if (eis.RegistrationTicketID.GetValueOrDefault() > 0)
                                                            existInShiftUpdate.RegistrationTicketID = eis.RegistrationTicketID;

                                                        existInShiftUpdate.FactoryID = operationDiary.FactoryID;
                                                        _unitOfWork.ElogbookRepository<ExistInShift>().Update(existInShiftUpdate);
                                                        _unitOfWork.Complete();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Tình hình vận hành
                                    if (requestModel.OperationSituationSyncs != null && requestModel.OperationSituationSyncs.Count > 0)
                                    {
                                        var operationSituations = requestModel.OperationSituationSyncs.Where(x => x.OperationDiaryID == operationDiary.ID).ToList();
                                        if (operationSituations != null && operationSituations.Count > 0)
                                        {
                                            foreach (var os in operationSituations)
                                            {
                                                if (os.IsNew)
                                                {
                                                    double.TryParse(os.Hour.Replace(":", "."), out double hour);
                                                    OperationSituation operationSituation = new OperationSituation
                                                    {
                                                        AppID = os.AppID,
                                                        OperationDiaryID = operationDiary.ID,
                                                        Hour = hour,
                                                        AssignedFromID = os.AssignedFromID != null ? os.AssignedFromID : null,
                                                        AssignedFromName = !string.IsNullOrEmpty(os.AssignedFromName) ? os.AssignedFromName : null,
                                                        AssignedToID = os.AssignedToID != null ? os.AssignedToID : null,
                                                        AssignedToName = os.AssignedToName,
                                                        Description = !string.IsNullOrEmpty(os.Description) ? os.Description : null,
                                                        Attachments = os.Attachments != null ? string.Join(",", os.Attachments) : string.Empty,
                                                        SyncDataKey = os.SyncDataKey,
                                                        IsDeleted = os.IsDeleted,
                                                        CreatedUserID = os.CreatedUserID,
                                                        InsertedAt = os.InsertedAt,
                                                        LastUpdatedUserID = os.LastUpdatedUserID,
                                                        UpdatedAt = os.UpdatedAt,
                                                        SyncedDate = DateTime.Now,
                                                        FactoryID = operationDiary.FactoryID,
                                                    };

                                                    _unitOfWork.ElogbookRepository<OperationSituation>().Add(operationSituation);
                                                    _unitOfWork.Complete();
                                                }
                                                else
                                                {
                                                    var operationSituationUpdate = _unitOfWork.ElogbookRepository<OperationSituation>()
                                                        .GetQuery(x => x.OperationDiaryID == operationDiary.ID && x.ID == os.ID && !x.IsDeleted).FirstOrDefault();
                                                    if (operationSituationUpdate != null)
                                                    {
                                                        double.TryParse(os.Hour.Replace(":", "."), out double hour);
                                                        operationSituationUpdate.AppID = os.AppID;
                                                        operationSituationUpdate.OperationDiaryID = operationDiary.ID;
                                                        operationSituationUpdate.Hour = hour;
                                                        operationSituationUpdate.AssignedFromID = os.AssignedFromID != null ? os.AssignedFromID : null;
                                                        operationSituationUpdate.AssignedFromName = !string.IsNullOrEmpty(os.AssignedFromName) ? os.AssignedFromName : null;
                                                        operationSituationUpdate.AssignedToID = os.AssignedToID != null ? os.AssignedToID : null;
                                                        operationSituationUpdate.AssignedToName = !string.IsNullOrEmpty(os.AssignedToName) ? os.AssignedToName : null;
                                                        operationSituationUpdate.Description = !string.IsNullOrEmpty(os.Description) ? os.Description : null;
                                                        operationSituationUpdate.Attachments = os.Attachments != null ? string.Join(",", os.Attachments) : string.Empty;
                                                        operationSituationUpdate.IsDeleted = os.IsDeleted;
                                                        operationSituationUpdate.LastUpdatedUserID = os.LastUpdatedUserID;
                                                        operationSituationUpdate.UpdatedAt = os.UpdatedAt;
                                                        operationSituationUpdate.SyncedDate = DateTime.Now;
                                                        operationSituationUpdate.FactoryID = operationDiary.FactoryID;
                                                        _unitOfWork.ElogbookRepository<OperationSituation>().Update(operationSituationUpdate);
                                                        _unitOfWork.Complete();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Danh sách cương vị trong ca
                                    if (requestModel.OperationDiaryPositionShiftSyncs != null && requestModel.OperationDiaryPositionShiftSyncs.Count > 0)
                                    {
                                        var operationDiaryPositionShifts = requestModel.OperationDiaryPositionShiftSyncs.Where(x => x.SyncDataKey == item.SyncDataKey && !x.IsDeleted && x.AssignedUserID > 0).ToList();

                                        if (operationDiaryPositionShifts != null && operationDiaryPositionShifts.Count > 0)
                                        {
                                            foreach (var odps in operationDiaryPositionShifts)
                                            {
                                                if (odps.ID > 0)
                                                {
                                                    var odpsExist = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.ID == odps.ID
                                                                        && !x.IsDeleted).FirstOrDefault();

                                                    if (odpsExist != null)
                                                    {
                                                        if (odps.AssignedUserID > 0)
                                                            odpsExist.AssignedUserID = odps.AssignedUserID;
                                                        odpsExist.SyncedDate = DateTime.Now;
                                                        _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().Update(odpsExist);
                                                        _unitOfWork.Complete();
                                                    }
                                                }
                                                else
                                                {
                                                    OperationDiaryPositionShift operationDiaryPositionShift = new OperationDiaryPositionShift
                                                    {
                                                        AppID = odps.AppID,
                                                        OperationDiaryID = operationDiary.ID,
                                                        PositionInShiftID = odps.PositionInShiftID != null ? odps.PositionInShiftID : null,
                                                        Label = !string.IsNullOrEmpty(odps.Label) ? odps.Label : null,
                                                        IsDefault = odps.IsDefault,
                                                        Position = odps.Position,
                                                        SyncDataKey = odps.SyncDataKey,
                                                        IsDeleted = odps.IsDeleted,
                                                        CreatedUserID = odps.CreatedUserID,
                                                        InsertedAt = odps.InsertedAt,
                                                        LastUpdatedUserID = odps.LastUpdatedUserID,
                                                        UpdatedAt = odps.UpdatedAt,
                                                        SyncedDate = DateTime.Now
                                                    };

                                                    if (odps.AssignedUserID > 0)
                                                        operationDiaryPositionShift.AssignedUserID = odps.AssignedUserID;
                                                    _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().Add(operationDiaryPositionShift);
                                                }
                                            }
                                            _unitOfWork.Complete();
                                        }
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                        responseModel.Message = "Đồng bộ dữ liệu thành công";
                    }
                }
                catch (Exception ex)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Content = false;
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    responseModel.Message = "Đồng bộ dữ liệu thất bại. Vui lòng liên hệ quản trị viên để kiểm tra lỗi.\n" + ex.ToString();
                    transaction.Rollback();
                    return responseModel;
                }
            }

            OperationDiarySyncDataResultModel masterDataModel = new OperationDiarySyncDataResultModel();

            if (lstOperationDiaryID != null && lstOperationDiaryID.Count > 0)
            {
                // Nhật ký vận hành
                masterDataModel.OperationDiary = _unitOfWork.ElogbookRepository<OperationDiary>()
                            .GetQuery(x => lstOperationDiaryID.Contains(x.ID))
                            .Select(
                                e => new MobileOperationDiaryInfo()
                                {
                                    AppID = e.AppID,
                                    SyncDataKey = e.SyncDataKey,
                                    ID = e.ID,
                                    FactoryID = e.FactoryID,
                                    CreatedDate = e.CreatedDate,
                                    OperationPositionID = e.OperationPositionID,
                                    OperationShiftID = e.OperationShiftID,
                                    OperationCrewID = e.OperationCrewID,
                                    Comments = e.Comments,
                                    IsReceived = e.IsReceived,
                                    IsDelivered = e.IsDelivered,
                                    IsCanEdit = e.IsCanEdit,
                                    TimeDelivery = e.TimeDelivery,
                                    IsDeleted = e.IsDeleted,
                                    CreatedUserID = e.CreatedUserID,
                                    InsertedAt = e.InsertedAt,
                                    LastUpdatedUserID = e.LastUpdatedUserID,
                                    UpdatedAt = e.UpdatedAt,
                                    ShiftReceiving = e.ShiftReceiving,
                                }).OrderBy(t => t.ID).ToList();
                // Cương vị trong ca
                masterDataModel.OperationDiaryPositionShift = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                    .Select(
                        e => new MobileOperationDiaryPositionShiftInfo()
                        {
                            AppID = e.AppID,
                            SyncDataKey = e.SyncDataKey,
                            ID = e.ID,
                            OperationDiaryID = e.OperationDiaryID,
                            PositionInShiftID = e.PositionInShiftID,
                            Label = e.Label,
                            AssignedUserID = e.AssignedUserID.HasValue ? e.AssignedUserID.Value : 0,
                            IsDefault = e.IsDefault,
                            Position = e.Position,
                            IsDeleted = e.IsDeleted,
                            CreatedUserID = e.CreatedUserID,
                            InsertedAt = e.InsertedAt,
                            LastUpdatedUserID = e.LastUpdatedUserID,
                            UpdatedAt = e.UpdatedAt
                        }).OrderBy(t => t.ID).ToList();

                // 1.Tình hình vận hành
                masterDataModel.OperationSituation = _unitOfWork.ElogbookRepository<OperationSituation>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileOperationSituationInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        Hour = e.Hour,
                                        AssignedFromID = e.AssignedFromID,
                                        AssignedFromName = e.AssignedFromName,
                                        AssignedToID = e.AssignedToID,
                                        AssignedToName = e.AssignedToName,
                                        Description = e.Description,
                                        Attachments = e.Attachments,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                // 2.Bất thường sự cố
                masterDataModel.AbnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileAbnormalIncidentInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        Description = e.Description,
                                        Attachments = e.Attachments,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                // 3.Phương thức giao ca
                masterDataModel.DeliveryShiftMethod = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileDeliveryShiftMethodInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        Description = e.Description,
                                        Attachments = e.Attachments,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                // 4.Tồn tại trong ca
                masterDataModel.ExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileExistInShiftInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        RegistrationTicketID = e.RegistrationTicketID,
                                        RegistrationTicketName = e.RegistrationTicketName,
                                        TeamLeader = e.TeamLeader,
                                        Area = e.Area,
                                        Content = e.Content,
                                        Attachments = e.Attachments,
                                        StatusID = e.StatusID,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                // 5.Giao/nhận ca
                masterDataModel.DeliveryReceiveShift = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileDeliveryReceiveShiftInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        OperationDiaryPositionShiftID = e.OperationDiaryPositionShiftID,
                                        SupervisorID = e.SupervisorID,
                                        IsDeliveryShift = e.IsDeliveryShift,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();
            }

            responseModel.Content = masterDataModel;
            responseModel.StatusCode = (int)HttpStatusCode.OK;

            return responseModel;
        }

        #region Cập nhập Api với app version 1.4.4

        public ResponseBaseModel OperationDiarySyncData(OperationDiarySyncDataModel requestModel)
        {
            switch (requestModel.ApiVersion)
            {
                case "V1":
                default:
                    return OperationDiarySyncDataV1(requestModel);
                case "V2":
                    return OperationDiarySyncDataV2(requestModel);
            }
        }

        private ResponseBaseModel OperationDiarySyncDataV2(OperationDiarySyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            List<int> lstOperationDiaryID = new List<int>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (requestModel != null && requestModel.OperationDiarySyncs != null && requestModel.OperationDiarySyncs.Count > 0)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.DEBUG, "OperationDiaryService", Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel));

                        var listOperationDiaryAdd = new List<OperationDiarySyncModel>();
                        var listOperationDiaryUpdate = new List<OperationDiarySyncModel>();
                        foreach (var item in requestModel.OperationDiarySyncs)
                        {
                            var existOperationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).FirstOrDefault();
                            //Kiểm tra tồn tại NKVH
                            if (existOperationDiary != null)
                            {
                                if (!listOperationDiaryUpdate.Contains(item))
                                    listOperationDiaryUpdate.Add(item);

                                if (!lstOperationDiaryID.Contains(existOperationDiary.ID))
                                    lstOperationDiaryID.Add(existOperationDiary.ID);
                            }
                            else
                            {
                                if (!listOperationDiaryAdd.Contains(item))
                                    listOperationDiaryAdd.Add(item);
                            }
                        }

                        if (listOperationDiaryAdd != null && listOperationDiaryAdd.Count > 0)
                        {
                            foreach (var item in listOperationDiaryAdd)
                            {
                                var existOperationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).FirstOrDefault();
                                // Kiểm tra lại để chăc chắn NKVH đã tồn tại hay chưa
                                if (existOperationDiary != null)
                                {
                                    item.ID = existOperationDiary.ID;

                                    if (!lstOperationDiaryID.Contains(existOperationDiary.ID))
                                        lstOperationDiaryID.Add(existOperationDiary.ID);

                                    if (!listOperationDiaryUpdate.Contains(item))
                                        listOperationDiaryUpdate.Add(item);

                                    continue;
                                }

                                var operationDiary = new OperationDiary
                                {
                                    AppID = item.AppID,
                                    FactoryID = item.FactoryID,
                                    CreatedDate = item.CreatedDate,
                                    OperationPositionID = item.OperationPositionID,
                                    OperationShiftID = item.OperationShiftID,
                                    OperationCrewID = item.OperationCrewID,
                                    Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : null,
                                    IsCanEdit = item.IsCanEdit,
                                    TimeDelivery = item.TimeDelivery,
                                    SyncDataKey = item.SyncDataKey,
                                    IsDeleted = item.IsDeleted,
                                    CreatedUserID = item.CreatedUserID,
                                    InsertedAt = item.InsertedAt,
                                    LastUpdatedUserID = item.LastUpdatedUserID,
                                    UpdatedAt = item.UpdatedAt,
                                    SyncedDate = DateTime.Now,
                                    ShiftReceiving = !string.IsNullOrEmpty(item.ShiftReceiving) ? item.ShiftReceiving : null,
                                };

                                _unitOfWork.ElogbookRepository<OperationDiary>().Add(operationDiary);
                                _unitOfWork.Complete();

                                if (!lstOperationDiaryID.Contains(operationDiary.ID))
                                    lstOperationDiaryID.Add(operationDiary.ID);

                                // Danh sách cương vị trong ca
                                if (requestModel.OperationDiaryPositionShiftSyncs != null && requestModel.OperationDiaryPositionShiftSyncs.Count > 0)
                                {
                                    var operationDiaryPositionShiftAdd = requestModel.OperationDiaryPositionShiftSyncs.Where(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                                                && !x.IsDeleted).ToList();
                                    if (operationDiaryPositionShiftAdd != null && operationDiaryPositionShiftAdd.Count > 0)
                                    {
                                        foreach (var odps in operationDiaryPositionShiftAdd)
                                        {
                                            odps.OperationDiaryID = operationDiary.ID;
                                            AddOperationDiaryPositionShift(odps);
                                        }
                                        _unitOfWork.Complete();
                                    }
                                }

                                // Tồn tại trong ca
                                if (requestModel.AbnormalIncidentSyncs != null && requestModel.AbnormalIncidentSyncs.Count > 0)
                                {
                                    var abnormalIncidentAdd = requestModel.AbnormalIncidentSyncs.FirstOrDefault(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted);
                                    if (abnormalIncidentAdd != null)
                                    {
                                        abnormalIncidentAdd.OperationDiaryID = operationDiary.ID;
                                        AddAbnormalIncident(abnormalIncidentAdd, operationDiary.FactoryID);
                                    }
                                }

                                // Phương thức giao ca
                                if (requestModel.DeliveryShiftMethodSyncs != null && requestModel.DeliveryShiftMethodSyncs.Count > 0)
                                {
                                    var deliveryShiftMethodAdd = requestModel.DeliveryShiftMethodSyncs.FirstOrDefault(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted);
                                    if (deliveryShiftMethodAdd != null)
                                    {
                                        deliveryShiftMethodAdd.OperationDiaryID = operationDiary.ID;
                                        AddDeliveryShiftMethod(deliveryShiftMethodAdd, operationDiary.FactoryID, requestModel.DeviceID);
                                    }
                                }

                                // Công tác đang thực hiện
                                //if (requestModel.ActiveWorkSyncs != null && requestModel.ActiveWorkSyncs.Count > 0)
                                //{
                                //    var activeWorks = requestModel.ActiveWorkSyncs.Where(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)).ToList();
                                //    if (activeWorks != null && activeWorks.Count > 0)
                                //    {
                                //        foreach (var os in activeWorks)
                                //        {
                                //            os.OperationDiaryID = operationDiary.ID;
                                //            AddActiveWork(os, operationDiary.FactoryID);
                                //            _unitOfWork.Complete();
                                //        }
                                //    }
                                //}

                                // Tồn tại trong ca
                                if (requestModel.ExistInShiftSyncs != null && requestModel.ExistInShiftSyncs.Count > 0)
                                {
                                    var existInShifts = requestModel.ExistInShiftSyncs.Where(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                && x.IsNew && !x.IsDeleted).ToList();
                                    if (existInShifts != null && existInShifts.Count > 0)
                                    {
                                        foreach (var eis in existInShifts)
                                        {
                                            eis.OperationDiaryID = operationDiary.ID;
                                            AddExistInShift(eis, operationDiary.FactoryID);
                                        }
                                        _unitOfWork.Complete();
                                    }
                                }

                                // Tình hình vận hành
                                if (requestModel.OperationSituationSyncs != null && requestModel.OperationSituationSyncs.Count > 0)
                                {
                                    var operationSituations = requestModel.OperationSituationSyncs.Where(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                            && x.IsNew && !x.IsDeleted).ToList();
                                    if (operationSituations != null && operationSituations.Count > 0)
                                    {
                                        foreach (var os in operationSituations)
                                        {
                                            os.OperationDiaryID = operationDiary.ID;
                                            AddOperationSituation(os, operationDiary.FactoryID);
                                        }
                                        _unitOfWork.Complete();
                                    }
                                }
                                _unitOfWork.Complete();

                                // Giao/nhận ca
                                if (requestModel.DeliveryReceiveShiftSyncs != null && requestModel.DeliveryReceiveShiftSyncs.Count > 0)
                                {
                                    var syncError = SyncDeliveryReceiveShifts(requestModel.DeliveryReceiveShiftSyncs, operationDiary, item.SyncDataKey);
                                    if (!string.IsNullOrEmpty(syncError))
                                        throw new InvalidOperationException(syncError);
                                }

                            }
                        }

                        if (listOperationDiaryUpdate != null && listOperationDiaryUpdate.Count > 0)
                        {
                            foreach (var item in listOperationDiaryUpdate)
                            {
                                // Cập nhật thông tin chung nhật ký vận hành
                                var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                                    && !x.IsDeleted).FirstOrDefault();
                                if (operationDiary != null)
                                {
                                    double diffTimeUpdate = (item.UpdatedAt - operationDiary.UpdatedAt).TotalMilliseconds;
                                    if (diffTimeUpdate > 0)
                                    {
                                        operationDiary.AppID = item.AppID;
                                        operationDiary.FactoryID = item.FactoryID;
                                        operationDiary.CreatedDate = item.CreatedDate;
                                        operationDiary.OperationPositionID = item.OperationPositionID;
                                        operationDiary.OperationShiftID = item.OperationShiftID;
                                        operationDiary.OperationCrewID = item.OperationCrewID;
                                        operationDiary.Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : operationDiary.Comments;
                                        operationDiary.IsReceived = item.IsReceived;
                                        operationDiary.IsDelivered = item.IsDelivered;
                                        operationDiary.IsCanEdit = item.IsCanEdit;
                                        operationDiary.TimeDelivery = item.TimeDelivery;
                                        operationDiary.IsDeleted = item.IsDeleted;
                                        operationDiary.CreatedUserID = item.CreatedUserID;
                                        operationDiary.InsertedAt = item.InsertedAt;
                                        operationDiary.LastUpdatedUserID = item.LastUpdatedUserID;
                                        operationDiary.UpdatedAt = item.UpdatedAt;
                                        operationDiary.SyncedDate = DateTime.Now;
                                        operationDiary.ShiftReceiving = !string.IsNullOrEmpty(item.ShiftReceiving) ? item.ShiftReceiving : operationDiary.ShiftReceiving;
                                    }

                                    if (!lstOperationDiaryID.Contains(operationDiary.ID))
                                        lstOperationDiaryID.Add(operationDiary.ID);

                                    // Danh sách cương vị trong ca
                                    if (requestModel.OperationDiaryPositionShiftSyncs != null && requestModel.OperationDiaryPositionShiftSyncs.Count > 0)
                                    {
                                        var operationDiaryPositionShifts = requestModel.OperationDiaryPositionShiftSyncs.Where(x => x.SyncDataKey.Equals(item.SyncDataKey, StringComparison.OrdinalIgnoreCase)).ToList();

                                        if (operationDiaryPositionShifts != null && operationDiaryPositionShifts.Count > 0)
                                        {
                                            foreach (var odps in operationDiaryPositionShifts)
                                            {
                                                var odpsExist = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.SyncDataKey.Equals(odps.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                        && x.PositionInShiftID == odps.PositionInShiftID
                                                                        && x.Label.Equals(odps.Label, StringComparison.OrdinalIgnoreCase)
                                                                        && !x.IsDeleted).FirstOrDefault();
                                                if (odpsExist != null)
                                                {
                                                    odpsExist.OperationDiaryID = operationDiary.ID;
                                                    odpsExist.PositionInShiftID = odps.PositionInShiftID;
                                                    odpsExist.Label = odps.Label;
                                                    if (odps.AssignedUserID > 0)
                                                        odpsExist.AssignedUserID = odps.AssignedUserID;
                                                    odpsExist.IsDefault = odps.IsDefault;
                                                    odpsExist.Position = odps.Position;
                                                    odpsExist.IsDeleted = odps.IsDeleted;
                                                    odpsExist.LastUpdatedUserID = odps.LastUpdatedUserID;
                                                    odpsExist.UpdatedAt = odps.UpdatedAt;
                                                    odpsExist.SyncDataKey = odps.SyncDataKey;
                                                    odpsExist.AppID = odps.AppID;
                                                    odpsExist.SyncedDate = DateTime.Now;

                                                    _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().Update(odpsExist);
                                                }
                                                else
                                                {
                                                    odps.OperationDiaryID = operationDiary.ID;
                                                    AddOperationDiaryPositionShift(odps);
                                                }
                                            }
                                            _unitOfWork.Complete();
                                        }
                                    }

                                    // Cập nhật bất thường sự cố
                                    UpdateAbnormalIncident(requestModel, operationDiary.ID, operationDiary.SyncDataKey, operationDiary.FactoryID);
                                    // Cập nhật phương thức giao ca
                                    UpdateDeliveryShiftMethod(requestModel, operationDiary.ID, operationDiary.SyncDataKey, operationDiary.FactoryID);
                                    _unitOfWork.Complete();

                                    // Tồn tại trong ca
                                    if (requestModel.ExistInShiftSyncs != null && requestModel.ExistInShiftSyncs.Count > 0)
                                    {
                                        var existInShifts = requestModel.ExistInShiftSyncs.Where(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)).ToList();
                                        if (existInShifts != null && existInShifts.Count > 0)
                                        {
                                            foreach (var eis in existInShifts)
                                            {
                                                var existInShiftUpdate = _unitOfWork.ElogbookRepository<ExistInShift>()
                                                       .GetQuery(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase) && x.ID == eis.ID && !x.IsDeleted).FirstOrDefault();

                                                // Thêm mới
                                                if (existInShiftUpdate == null)
                                                {
                                                    eis.OperationDiaryID = operationDiary.ID;
                                                    AddExistInShift(eis, operationDiary.FactoryID);
                                                    _unitOfWork.Complete();
                                                }
                                                // Cập nhật
                                                else
                                                {
                                                    if (existInShiftUpdate != null)
                                                    {
                                                        existInShiftUpdate.AppID = eis.AppID;
                                                        existInShiftUpdate.OperationDiaryID = operationDiary.ID;
                                                        existInShiftUpdate.RegistrationTicketName = !string.IsNullOrEmpty(eis.RegistrationTicketName) ? eis.RegistrationTicketName : null;
                                                        existInShiftUpdate.TeamLeader = !string.IsNullOrEmpty(eis.TeamLeader) ? eis.TeamLeader : null;
                                                        existInShiftUpdate.Area = !string.IsNullOrEmpty(eis.Area) ? eis.Area : null;
                                                        existInShiftUpdate.Content = !string.IsNullOrEmpty(eis.Content) ? eis.Content : null;
                                                        existInShiftUpdate.Attachments = CombinedAttachments(existInShiftUpdate.Attachments, eis.Attachments);
                                                        existInShiftUpdate.StatusID = eis.StatusID;
                                                        existInShiftUpdate.IsDeleteIssueOfShift = eis.IsDeleteIssueOfShift;
                                                        existInShiftUpdate.IsDeleted = eis.IsDeleted;
                                                        existInShiftUpdate.LastUpdatedUserID = eis.LastUpdatedUserID;
                                                        existInShiftUpdate.UpdatedAt = eis.UpdatedAt;
                                                        existInShiftUpdate.SyncedDate = DateTime.Now;
                                                        if (eis.CompletedDateTime != null && eis.CompletedDateTime.Year >= 1753)
                                                        {
                                                            existInShiftUpdate.CompletedDateTime = eis.CompletedDateTime;
                                                        }
                                                        if (eis.RegistrationTicketID.GetValueOrDefault() > 0)
                                                            existInShiftUpdate.RegistrationTicketID = eis.RegistrationTicketID;

                                                        existInShiftUpdate.FactoryID = operationDiary.FactoryID;
                                                        _unitOfWork.ElogbookRepository<ExistInShift>().Update(existInShiftUpdate);
                                                        _unitOfWork.Complete();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Tình hình vận hành
                                    if (requestModel.OperationSituationSyncs != null && requestModel.OperationSituationSyncs.Count > 0)
                                    {
                                        var operationSituations = requestModel.OperationSituationSyncs.Where(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)).ToList();
                                        if (operationSituations != null && operationSituations.Count > 0)
                                        {
                                            foreach (var os in operationSituations)
                                            {
                                                var operationSituationUpdate = _unitOfWork.ElogbookRepository<OperationSituation>()
                                                        .GetQuery(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase) && x.ID == os.ID && !x.IsDeleted).FirstOrDefault();
                                                if (operationSituationUpdate == null)
                                                {
                                                    os.OperationDiaryID = operationDiary.ID;
                                                    AddOperationSituation(os, operationDiary.FactoryID);
                                                    _unitOfWork.Complete();
                                                }
                                                else
                                                {
                                                    if (operationSituationUpdate != null)
                                                    {
                                                        double.TryParse(os.Hour.Replace(":", "."), out double hour);
                                                        operationSituationUpdate.AppID = os.AppID;
                                                        operationSituationUpdate.OperationDiaryID = operationDiary.ID;
                                                        operationSituationUpdate.Hour = hour;
                                                        operationSituationUpdate.AssignedFromID = os.AssignedFromID != null ? os.AssignedFromID : null;
                                                        operationSituationUpdate.AssignedFromName = !string.IsNullOrEmpty(os.AssignedFromName) ? os.AssignedFromName : null;
                                                        operationSituationUpdate.AssignedToID = os.AssignedToID != null ? os.AssignedToID : null;
                                                        operationSituationUpdate.AssignedToName = !string.IsNullOrEmpty(os.AssignedToName) ? os.AssignedToName : null;
                                                        operationSituationUpdate.Description = !string.IsNullOrEmpty(os.Description) ? os.Description : null;
                                                        operationSituationUpdate.Attachments = CombinedAttachments(operationSituationUpdate.Attachments, os.Attachments);
                                                        operationSituationUpdate.IsDeleted = os.IsDeleted;
                                                        operationSituationUpdate.LastUpdatedUserID = os.LastUpdatedUserID;
                                                        operationSituationUpdate.UpdatedAt = os.UpdatedAt;
                                                        operationSituationUpdate.SyncedDate = DateTime.Now;
                                                        operationSituationUpdate.FactoryID = operationDiary.FactoryID;
                                                        _unitOfWork.ElogbookRepository<OperationSituation>().Update(operationSituationUpdate);
                                                        _unitOfWork.Complete();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Công tác đang thực hiện
                                    //if (requestModel.ActiveWorkSyncs != null && requestModel.ActiveWorkSyncs.Count > 0)
                                    //{
                                    //    var activeWorks = requestModel.ActiveWorkSyncs.Where(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)).ToList();
                                    //    if (activeWorks != null && activeWorks.Count > 0)
                                    //    {
                                    //        foreach (var os in activeWorks)
                                    //        {
                                    //            var activeWorkUpdate = _unitOfWork.ElogbookRepository<ActiveWork>()
                                    //                    .GetQuery(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase) && x.ID == os.ID && !x.IsDeleted).FirstOrDefault();
                                    //            if (activeWorkUpdate == null)
                                    //            {
                                    //                os.OperationDiaryID = operationDiary.ID;
                                    //                AddActiveWork(os, operationDiary.FactoryID);
                                    //                _unitOfWork.Complete();
                                    //            }
                                    //            else
                                    //            {
                                    //                if (activeWorkUpdate != null)
                                    //                {
                                    //                    double.TryParse(!string.IsNullOrWhiteSpace(os.StartHour) ? os.StartHour.Replace(":", ".") : "0", out double startHour);
                                    //                    double.TryParse(!string.IsNullOrWhiteSpace(os.EndHour) ? os.EndHour.Replace(":", ".") : "0", out double endHour);
                                    //                    activeWorkUpdate.AppID = os.AppID;
                                    //                    activeWorkUpdate.OperationDiaryID = operationDiary.ID;
                                    //                    activeWorkUpdate.StartHour = startHour;
                                    //                    activeWorkUpdate.EndHour = endHour;
                                    //                    activeWorkUpdate.Description = !string.IsNullOrEmpty(os.Description) ? os.Description : null;
                                    //                    activeWorkUpdate.Attachments = CombinedAttachments(activeWorkUpdate.Attachments, os.Attachments);
                                    //                    activeWorkUpdate.IsDeleted = os.IsDeleted;
                                    //                    activeWorkUpdate.LastUpdatedUserID = os.LastUpdatedUserID;
                                    //                    activeWorkUpdate.UpdatedAt = os.UpdatedAt;
                                    //                    activeWorkUpdate.SyncedDate = DateTime.Now;
                                    //                    activeWorkUpdate.FactoryID = operationDiary.FactoryID;
                                    //                    _unitOfWork.ElogbookRepository<ActiveWork>().Update(activeWorkUpdate);
                                    //                    _unitOfWork.Complete();
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}

                                    // Giao/nhận ca
                                    if (requestModel.DeliveryReceiveShiftSyncs != null && requestModel.DeliveryReceiveShiftSyncs.Count > 0)
                                    {
                                        var deliveryReceiveShifts = requestModel.DeliveryReceiveShiftSyncs.Where(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)).ToList();
                                        if (deliveryReceiveShifts != null && deliveryReceiveShifts.Count > 0)
                                        {
                                            var deliveryReceiveUsers = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                              && !x.IsDeleted).ToList();
                                            //Nhận ca
                                            var receiveShifts = deliveryReceiveShifts.Where(x => !x.IsDeliveryShift);
                                            foreach (var receiveItem in receiveShifts)
                                            {
                                                var operationDiaryPositionShiftID = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>()
                                                        .GetQuery(x => x.SyncDataKey.Equals(receiveItem.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                    && (x.AppID.Equals(receiveItem.ReferAppID, StringComparison.OrdinalIgnoreCase) || x.ID == receiveItem.OperationDiaryPositionShiftID)
                                                                    && !x.IsDeleted).Select(x => x.ID).FirstOrDefault();

                                                var updateEntity = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.OperationDiaryPositionShiftID == operationDiaryPositionShiftID
                                                                                                    && x.SyncDataKey.Equals(receiveItem.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                    && !x.IsDeliveryShift
                                                                                                    && !x.IsDeleted).FirstOrDefault();
                                                if (updateEntity != null)
                                                {
                                                    double totalMilliseconds = (receiveItem.UpdatedAt - updateEntity.UpdatedAt).TotalMilliseconds;
                                                    if (totalMilliseconds > 0)
                                                    {
                                                        updateEntity.AppID = receiveItem.AppID;
                                                        updateEntity.SupervisorID = receiveItem.SupervisorID;
                                                        updateEntity.IsDeliveryShift = receiveItem.IsDeliveryShift;
                                                        updateEntity.LastUpdatedUserID = receiveItem.LastUpdatedUserID;
                                                        updateEntity.UpdatedAt = receiveItem.UpdatedAt;
                                                        updateEntity.IsDeleted = receiveItem.IsDeleted;
                                                        updateEntity.SyncedDate = DateTime.Now;
                                                        _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().Update(updateEntity);
                                                    }
                                                }
                                                else
                                                {
                                                    var syncError = SyncDeliveryReceiveShifts(new List<DeliveryReceiveShiftSyncModel>() { receiveItem }, operationDiary, item.SyncDataKey);
                                                    if (!string.IsNullOrEmpty(syncError))
                                                        throw new InvalidOperationException(syncError);
                                                }

                                                operationDiary.LastUpdatedUserID = item.LastUpdatedUserID;
                                                operationDiary.SyncedDate = DateTime.Now;
                                            }

                                            //Giao ca
                                            var deliveryShifts = deliveryReceiveShifts.Where(x => x.IsDeliveryShift);
                                            foreach (var deliveryItem in deliveryShifts)
                                            {
                                                var operationDiaryPositionShiftID = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>()
                                                        .GetQuery(x => x.SyncDataKey.Equals(deliveryItem.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                    && (x.AppID.Equals(deliveryItem.ReferAppID, StringComparison.OrdinalIgnoreCase) || x.ID == deliveryItem.OperationDiaryPositionShiftID)
                                                                    && !x.IsDeleted).Select(x => x.ID).FirstOrDefault();

                                                var updateEntity = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.OperationDiaryPositionShiftID == operationDiaryPositionShiftID
                                                                                                    && x.SyncDataKey.Equals(deliveryItem.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                    && x.IsDeliveryShift
                                                                                                    && !x.IsDeleted).FirstOrDefault();
                                                if (updateEntity != null)
                                                {
                                                    double totalMilliseconds = (deliveryItem.UpdatedAt - updateEntity.UpdatedAt).TotalMilliseconds;
                                                    if (totalMilliseconds > 0)
                                                    {
                                                        updateEntity.AppID = deliveryItem.AppID;
                                                        updateEntity.SupervisorID = deliveryItem.SupervisorID;
                                                        updateEntity.IsDeliveryShift = deliveryItem.IsDeliveryShift;
                                                        updateEntity.LastUpdatedUserID = deliveryItem.LastUpdatedUserID;
                                                        updateEntity.UpdatedAt = deliveryItem.UpdatedAt;
                                                        updateEntity.IsDeleted = deliveryItem.IsDeleted;
                                                        updateEntity.SyncedDate = DateTime.Now;
                                                        _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().Update(updateEntity);
                                                    }
                                                }
                                                else
                                                {
                                                    var syncError = SyncDeliveryReceiveShifts(new List<DeliveryReceiveShiftSyncModel>() { deliveryItem }, operationDiary, item.SyncDataKey);
                                                    if (!string.IsNullOrEmpty(syncError))
                                                        throw new InvalidOperationException(syncError);
                                                }

                                                operationDiary.LastUpdatedUserID = item.LastUpdatedUserID;
                                                operationDiary.SyncedDate = DateTime.Now;
                                            }

                                            _unitOfWork.Complete();
                                        }
                                    }
                                    var deliveryReceiveInfo = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                             && !x.IsDeleted).ToList();
                                    var totalReceived = deliveryReceiveInfo.Count(x => !x.IsDeliveryShift);
                                    var totalDelivered = deliveryReceiveInfo.Count(x => x.IsDeliveryShift);
                                    var totalAssigned = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.SyncDataKey.Equals(operationDiary.SyncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                                        && x.PositionInShift.OperationPositionID == operationDiary.OperationPositionID
                                                                                                                        && x.AssignedUserID > 0 && x.AssignedUserID != null
                                                                                                                        && !x.IsDeleted).Select(x => x.ID).Count();
                                    if (operationDiary != null)
                                    {
                                        operationDiary.IsReceived = totalReceived == totalAssigned;
                                        operationDiary.IsDelivered = totalDelivered == totalAssigned;
                                        if (operationDiary.IsDelivered && (operationDiary.TimeDelivery == null || operationDiary.TimeDelivery == DateTime.MinValue))
                                        {
                                            if (item.TimeDelivery == null || item.TimeDelivery == DateTime.MinValue)
                                            {
                                                operationDiary.TimeDelivery = DateTime.Now;
                                            }
                                            else
                                            {
                                                operationDiary.TimeDelivery = item.TimeDelivery;
                                            }
                                        }

                                        if (operationDiary.TimeDelivery.HasValue && operationDiary.TimeDelivery.Value > DateTime.MinValue)
                                        {
                                            operationDiary.IsReceived = operationDiary.IsDelivered = true;
                                        }

                                        operationDiary.SyncedDate = DateTime.Now;
                                        SafeUpdate<OperationDiary>(operationDiary);
                                        _unitOfWork.Complete();
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                        responseModel.Message = "Đồng bộ dữ liệu thành công";
                    }
                }
                catch (Exception ex)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Content = false;
                    responseModel.Message = "Đồng bộ dữ liệu thất bại. Vui lòng liên hệ quản trị viên để kiểm tra lỗi.\n" + ex.ToString();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                    return responseModel;
                }
            }

            OperationDiarySyncDataResultModel masterDataModel = new OperationDiarySyncDataResultModel();

            if (lstOperationDiaryID != null && lstOperationDiaryID.Count > 0)
            {
                // Nhật ký vận hành
                masterDataModel.OperationDiary = _unitOfWork.ElogbookRepository<OperationDiary>()
                            .GetQuery(x => lstOperationDiaryID.Contains(x.ID))
                            .Select(
                                e => new MobileOperationDiaryInfo()
                                {
                                    AppID = e.AppID,
                                    SyncDataKey = e.SyncDataKey,
                                    ID = e.ID,
                                    FactoryID = e.FactoryID,
                                    CreatedDate = e.CreatedDate,
                                    OperationPositionID = e.OperationPositionID,
                                    OperationShiftID = e.OperationShiftID,
                                    OperationCrewID = e.OperationCrewID,
                                    Comments = e.Comments,
                                    IsReceived = e.IsReceived,
                                    IsDelivered = e.IsDelivered,
                                    IsCanEdit = e.IsCanEdit,
                                    TimeDelivery = e.TimeDelivery,
                                    IsDeleted = e.IsDeleted,
                                    CreatedUserID = e.CreatedUserID,
                                    InsertedAt = e.InsertedAt,
                                    LastUpdatedUserID = e.LastUpdatedUserID,
                                    UpdatedAt = e.UpdatedAt,
                                    ShiftReceiving = e.ShiftReceiving,
                                }).OrderBy(t => t.ID).ToList();
                // Cương vị trong ca
                masterDataModel.OperationDiaryPositionShift = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                    .Select(
                        e => new MobileOperationDiaryPositionShiftInfo()
                        {
                            AppID = e.AppID,
                            SyncDataKey = e.SyncDataKey,
                            ID = e.ID,
                            OperationDiaryID = e.OperationDiaryID,
                            PositionInShiftID = e.PositionInShiftID,
                            Label = e.Label,
                            AssignedUserID = e.AssignedUserID.HasValue ? e.AssignedUserID.Value : 0,
                            IsDefault = e.IsDefault,
                            Position = e.Position,
                            IsDeleted = e.IsDeleted,
                            CreatedUserID = e.CreatedUserID,
                            InsertedAt = e.InsertedAt,
                            LastUpdatedUserID = e.LastUpdatedUserID,
                            UpdatedAt = e.UpdatedAt
                        }).OrderBy(t => t.ID).ToList();

                // 1.Tình hình vận hành
                masterDataModel.OperationSituation = _unitOfWork.ElogbookRepository<OperationSituation>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileOperationSituationInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        Hour = e.Hour,
                                        AssignedFromID = e.AssignedFromID,
                                        AssignedFromName = e.AssignedFromName,
                                        AssignedToID = e.AssignedToID,
                                        AssignedToName = e.AssignedToName,
                                        Description = e.Description,
                                        Attachments = e.Attachments,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                // Công tác đang thực hiện
                //masterDataModel.ActiveWork = _unitOfWork.ElogbookRepository<ActiveWork>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                //                .Select(
                //                    e => new MobileActiveWorkInfo()
                //                    {
                //                        AppID = e.AppID,
                //                        SyncDataKey = e.SyncDataKey,
                //                        ID = e.ID,
                //                        OperationDiaryID = e.OperationDiaryID,
                //                        StartHour = e.StartHour,
                //                        EndHour = e.EndHour,
                //                        Description = e.Description,
                //                        Attachments = e.Attachments,
                //                        IsDeleted = e.IsDeleted,
                //                        CreatedUserID = e.CreatedUserID,
                //                        InsertedAt = e.InsertedAt,
                //                        LastUpdatedUserID = e.LastUpdatedUserID,
                //                        UpdatedAt = e.UpdatedAt
                //                    }).OrderBy(t => t.ID).ToList();

                // 2.Bất thường sự cố
                masterDataModel.AbnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileAbnormalIncidentInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        Description = e.Description,
                                        Attachments = e.Attachments,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                // 3.Phương thức giao ca
                masterDataModel.DeliveryShiftMethod = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileDeliveryShiftMethodInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        Description = e.Description,
                                        Attachments = e.Attachments,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt,
                                        OperationDiaryID = e.OperationDiaryID,
                                    }).OrderBy(t => t.ID).ToList();

                // 4.Tồn tại trong ca
                masterDataModel.ExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileExistInShiftInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        RegistrationTicketID = e.RegistrationTicketID,
                                        RegistrationTicketName = e.RegistrationTicketName,
                                        TeamLeader = e.TeamLeader,
                                        Area = e.Area,
                                        Content = e.Content,
                                        Attachments = e.Attachments,
                                        StatusID = e.StatusID,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                // 5.Giao/nhận ca
                masterDataModel.DeliveryReceiveShift = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => lstOperationDiaryID.Contains(x.OperationDiaryID))
                                .Select(
                                    e => new MobileDeliveryReceiveShiftInfo()
                                    {
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        ID = e.ID,
                                        OperationDiaryID = e.OperationDiaryID,
                                        OperationDiaryPositionShiftID = e.OperationDiaryPositionShiftID,
                                        SupervisorID = e.SupervisorID,
                                        IsDeliveryShift = e.IsDeliveryShift,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();
            }

            responseModel.Content = masterDataModel;
            responseModel.StatusCode = (int)HttpStatusCode.OK;

            return responseModel;
        }

        /// <summary>
        /// Đồng bộ giao/nhận ca. Trả về null nếu thành công, trả về chuỗi lỗi nếu thất bại.
        /// </summary>
        private string SyncDeliveryReceiveShifts(List<DeliveryReceiveShiftSyncModel> deliveryReceiveShiftSyncs, OperationDiary operationDiary, string syncDataKey)
        {
            var deliveryReceiveShifts = deliveryReceiveShiftSyncs.Where(x => x.SyncDataKey.Equals(syncDataKey, StringComparison.OrdinalIgnoreCase)
                                                                                                                && x.IsNew && !x.IsDeleted).ToList();
            if (deliveryReceiveShifts != null && deliveryReceiveShifts.Count > 0)
            {
                var deliveryReceiveUsers = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.OperationDiaryID == operationDiary.ID
                                          && !x.IsDeleted).ToList();
                var listID = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.OperationDiaryID == operationDiary.ID
                                                                                                                    && x.PositionInShift.OperationPositionID == operationDiary.OperationPositionID
                                                                                                                    && !x.IsDeleted).Select(x => x.ID).ToList();
                //Nhận ca
                int index = 0;
                var receiveShifts = deliveryReceiveShifts.Where(x => !x.IsDeliveryShift);
                foreach (var receiveItem in receiveShifts)
                {
                    if (receiveItem.OperationDiaryPositionShiftID == 0)
                    {
                        try
                        {
                            receiveItem.OperationDiaryPositionShiftID = listID[index];
                        }
                        catch
                        {
                            //Ignore case
                        }
                    }
                    index++;
                    var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == receiveItem.SupervisorID).FirstOrDefault();
                    AddEditDeliveryReceiveShiftModel deliveryReceiveShiftModel = new AddEditDeliveryReceiveShiftModel
                    {
                        OperationDiaryID = operationDiary.ID,
                        IsDeliveryShift = receiveItem.IsDeliveryShift,
                        PositionShiftID = receiveItem.positionShiftID > 0 ? receiveItem.positionShiftID : receiveItem.OperationDiaryPositionShiftID,
                        CurrentUserID = receiveItem.CreatedUserID.Value,
                        UserName = userInfo.UserName,
                        Password = userInfo.Password,
                        SignTime = receiveItem.SignTime,
                        SkipPasswordValidation = true, // Password trong DB đã hash, không verify lại khi sync
                    };

                    var result = DeliveryReceiveShift(deliveryReceiveShiftModel);
                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                        return $"Nhận ca (SyncDataKey={receiveItem.SyncDataKey}): {result.ErrorMessage}";
                }

                //Giao ca
                var deliveryShifts = deliveryReceiveShifts.Where(x => x.IsDeliveryShift);
                index = 0;
                foreach (var deliveryItem in deliveryShifts)
                {
                    if (deliveryItem.OperationDiaryPositionShiftID == 0)
                    {
                        try
                        {
                            deliveryItem.OperationDiaryPositionShiftID = listID[index];
                        }
                        catch
                        {
                            //Ignore case
                        }
                    }
                    index++;
                    var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == deliveryItem.SupervisorID).FirstOrDefault();
                    AddEditDeliveryReceiveShiftModel deliveryReceiveShiftModel = new AddEditDeliveryReceiveShiftModel
                    {
                        OperationDiaryID = operationDiary.ID,
                        IsDeliveryShift = deliveryItem.IsDeliveryShift,
                        PositionShiftID = deliveryItem.positionShiftID > 0 ? deliveryItem.positionShiftID : deliveryItem.OperationDiaryPositionShiftID,
                        CurrentUserID = deliveryItem.CreatedUserID.Value,
                        UserName = userInfo.UserName,
                        Password = userInfo.Password,
                        SignTime = deliveryItem.SignTime,
                        SkipPasswordValidation = true, // Password trong DB đã hash, không verify lại khi sync
                    };

                    var result = DeliveryReceiveShift(deliveryReceiveShiftModel);
                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                        return $"Giao ca (SyncDataKey={deliveryItem.SyncDataKey}): {result.ErrorMessage}";
                }
                _unitOfWork.Complete();
            }
            return null;
        }

        /// <summary>
        /// Safe update cho EF tracked entity trong SyncData:
        /// Nếu entity đã được track (Unchanged/Modified) thì chỉ set State = Modified,
        /// tránh lỗi "Attaching an entity... already has the same primary key" khi entity
        /// vừa được load bằng GetQuery() trong cùng DbContext/transaction.
        /// </summary>
        private void SafeUpdate<TEntity>(TEntity entity) where TEntity : class, IEntityBase
        {
            entity.UpdatedAt = DateTime.Now;
            var entry = _unitOfWork.DataContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                // Kiểm tra xem một entity cùng key có đang được track không
                var tracked = _unitOfWork.DataContext.Set<TEntity>().Local
                    .FirstOrDefault(e => e.ID == entity.ID);
                if (tracked != null)
                {
                    // Copy giá trị sang tracked entity, không Attach lại
                    _unitOfWork.DataContext.Entry(tracked).CurrentValues.SetValues(entity);
                    _unitOfWork.DataContext.Entry(tracked).State = EntityState.Modified;
                    return;
                }
                _unitOfWork.DataContext.Set<TEntity>().Attach(entity);
            }
            entry.State = EntityState.Modified;
        }

        private void AddAbnormalIncident(AbnormalIncidentSyncModel abnormalIncidentRequest, int? factoryID)
        {
            AbnormalIncident abnormalIncidentAdd = new AbnormalIncident
            {
                AppID = abnormalIncidentRequest.AppID,
                OperationDiaryID = abnormalIncidentRequest.OperationDiaryID,
                Description = !string.IsNullOrEmpty(abnormalIncidentRequest.Description) ? abnormalIncidentRequest.Description : null,
                SyncDataKey = abnormalIncidentRequest.SyncDataKey,
                IsDeleted = abnormalIncidentRequest.IsDeleted,
                CreatedUserID = abnormalIncidentRequest.CreatedUserID,
                InsertedAt = abnormalIncidentRequest.InsertedAt,
                LastUpdatedUserID = abnormalIncidentRequest.LastUpdatedUserID,
                UpdatedAt = abnormalIncidentRequest.UpdatedAt,
                SyncedDate = DateTime.Now,
                FactoryID = factoryID,
            };

            abnormalIncidentAdd.Attachments = !string.IsNullOrWhiteSpace(abnormalIncidentRequest.Attachments) ? abnormalIncidentRequest.Attachments : string.Empty;
            _unitOfWork.ElogbookRepository<AbnormalIncident>().Add(abnormalIncidentAdd);
        }

        private void UpdateAbnormalIncident(OperationDiarySyncDataModel requestModel, int operationDiaryID, string syncDataKey, int? factoryID)
        {
            if (requestModel.AbnormalIncidentSyncs != null && requestModel.AbnormalIncidentSyncs.Count > 0)
            {
                var abnormalIncidentRequest = requestModel.AbnormalIncidentSyncs.FirstOrDefault(x => x.SyncDataKey.Equals(syncDataKey, StringComparison.OrdinalIgnoreCase));

                var abnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>()
                    .GetQuery(x => x.SyncDataKey.Equals(syncDataKey, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                // Cập nhật nếu tồn tại
                if (abnormalIncident != null && abnormalIncidentRequest != null)
                {
                    double diffTimeUpdate = (abnormalIncidentRequest.UpdatedAt - abnormalIncident.UpdatedAt).TotalMilliseconds;
                    if (diffTimeUpdate > 0)
                    {
                        abnormalIncident.AppID = abnormalIncidentRequest.AppID;
                        abnormalIncident.OperationDiaryID = operationDiaryID;
                        abnormalIncident.Description = !string.IsNullOrEmpty(abnormalIncidentRequest.Description) ? abnormalIncidentRequest.Description : null;
                        abnormalIncident.Attachments = CombinedAttachments(abnormalIncident.Attachments, abnormalIncidentRequest.Attachments);
                        abnormalIncident.IsDeleted = abnormalIncidentRequest.IsDeleted;
                        abnormalIncident.LastUpdatedUserID = abnormalIncidentRequest.LastUpdatedUserID;
                        abnormalIncident.UpdatedAt = abnormalIncidentRequest.UpdatedAt;
                        abnormalIncident.SyncedDate = DateTime.Now;
                        abnormalIncident.FactoryID = factoryID;
                        _unitOfWork.ElogbookRepository<AbnormalIncident>().Update(abnormalIncident);
                    }
                }
                else if (abnormalIncident == null && abnormalIncidentRequest != null)
                {
                    abnormalIncidentRequest.OperationDiaryID = operationDiaryID;
                    AddAbnormalIncident(abnormalIncidentRequest, factoryID);
                }
            }
        }

        private void AddDeliveryShiftMethod(DeliveryShiftMethodSyncModel deliveryShiftMethodRequest, int? factoryID, string deviceID)
        {
            DeliveryShiftMethod deliveryShiftMethodAdd = new DeliveryShiftMethod
            {
                AppID = deliveryShiftMethodRequest.AppID,
                OperationDiaryID = deliveryShiftMethodRequest.OperationDiaryID,
                Description = !string.IsNullOrEmpty(deliveryShiftMethodRequest.Description) ? deliveryShiftMethodRequest.Description : null,
                SyncDataKey = deliveryShiftMethodRequest.SyncDataKey,
                IsDeleted = deliveryShiftMethodRequest.IsDeleted,
                CreatedUserID = deliveryShiftMethodRequest.CreatedUserID,
                InsertedAt = deliveryShiftMethodRequest.InsertedAt,
                LastUpdatedUserID = deliveryShiftMethodRequest.LastUpdatedUserID,
                UpdatedAt = deliveryShiftMethodRequest.UpdatedAt,
                SyncedDate = DateTime.Now,
                FactoryID = factoryID,
                DeviceID = deviceID,
            };

            deliveryShiftMethodAdd.Attachments = !string.IsNullOrWhiteSpace(deliveryShiftMethodRequest.Attachments) ? deliveryShiftMethodRequest.Attachments : string.Empty;
            _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Add(deliveryShiftMethodAdd);
        }

        private void UpdateDeliveryShiftMethod(OperationDiarySyncDataModel requestModel, int operationDiaryID, string syncDataKey, int? factoryID)
        {
            if (requestModel.DeliveryShiftMethodSyncs != null && requestModel.DeliveryShiftMethodSyncs.Count > 0)
            {
                var deliveryShiftMethodRequest = requestModel.DeliveryShiftMethodSyncs.FirstOrDefault(x => x.SyncDataKey.Equals(syncDataKey, StringComparison.OrdinalIgnoreCase));

                var deliveryShiftMethod = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>()
                    .GetQuery(x => x.SyncDataKey.Equals(syncDataKey, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (deliveryShiftMethod != null && deliveryShiftMethodRequest != null)
                {
                    double diffTimeUpdate = (deliveryShiftMethodRequest.UpdatedAt - deliveryShiftMethod.UpdatedAt).TotalMilliseconds;
                    if (diffTimeUpdate > 0)
                    {
                        deliveryShiftMethod.AppID = deliveryShiftMethodRequest.AppID;
                        deliveryShiftMethod.OperationDiaryID = operationDiaryID;

                        var entity = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == factoryID && !x.IsDeleted).FirstOrDefault();
                        if (entity != null)
                        {
                            if (entity.FactoryName.Equals("Bà Rịa", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrWhiteSpace(deliveryShiftMethod.DeviceID))
                                {
                                    if (requestModel.DeviceID.Equals(deliveryShiftMethod.DeviceID, StringComparison.OrdinalIgnoreCase))
                                    {
                                        deliveryShiftMethod.Description = !string.IsNullOrEmpty(deliveryShiftMethodRequest.Description) ? deliveryShiftMethodRequest.Description : deliveryShiftMethod.Description;
                                    }
                                    else
                                    {
                                        deliveryShiftMethod.Description = deliveryShiftMethod.Description + "\n" + deliveryShiftMethodRequest.Description;
                                    }
                                }
                                else
                                {
                                    deliveryShiftMethod.Description = !string.IsNullOrEmpty(deliveryShiftMethodRequest.Description) ? deliveryShiftMethodRequest.Description : deliveryShiftMethod.Description;
                                }
                            }
                            else
                            {
                                deliveryShiftMethod.Description = !string.IsNullOrEmpty(deliveryShiftMethodRequest.Description) ? deliveryShiftMethodRequest.Description : deliveryShiftMethod.Description;
                            }
                        }
                        else
                        {
                            deliveryShiftMethod.Description = !string.IsNullOrEmpty(deliveryShiftMethodRequest.Description) ? deliveryShiftMethodRequest.Description : deliveryShiftMethod.Description;
                        }

                        deliveryShiftMethod.DeviceID = requestModel.DeviceID;
                        deliveryShiftMethod.Attachments = CombinedAttachments(deliveryShiftMethod.Attachments, deliveryShiftMethodRequest.Attachments);
                        deliveryShiftMethod.IsDeleted = deliveryShiftMethodRequest.IsDeleted;
                        deliveryShiftMethod.LastUpdatedUserID = deliveryShiftMethodRequest.LastUpdatedUserID;
                        deliveryShiftMethod.UpdatedAt = deliveryShiftMethodRequest.UpdatedAt;
                        deliveryShiftMethod.SyncedDate = DateTime.Now;
                        deliveryShiftMethod.FactoryID = factoryID;
                        _unitOfWork.ElogbookRepository<DeliveryShiftMethod>().Update(deliveryShiftMethod);
                    }
                }
                else if (deliveryShiftMethod == null && deliveryShiftMethodRequest != null)
                {
                    deliveryShiftMethodRequest.OperationDiaryID = operationDiaryID;
                    AddDeliveryShiftMethod(deliveryShiftMethodRequest, factoryID, requestModel.DeviceID);
                }
            }
        }

        private string CombinedAttachments(string originalAttachments, string newAttachments)
        {
            var attachments = string.Empty;
            //Nếu list chưa có giá trị thì lấy giá trị của list mới
            if (string.IsNullOrWhiteSpace(originalAttachments))
            {
                attachments = newAttachments;
            }
            else
            {
                //Nếu list đã có giá trị, và list mới cũng có giá trị thì merge giá trị của 2 list lại
                if (!string.IsNullOrWhiteSpace(newAttachments))
                {
                    var newAttchmentIDs = newAttachments.Split(',');
                    var oldAttchmentIDs = originalAttachments.Split(',');
                    List<string> additionAttachments = new List<string>();
                    if (newAttchmentIDs != null && newAttchmentIDs.Length > 0)
                    {
                        foreach (var item in newAttchmentIDs)
                        {
                            int.TryParse(item, out int attchmentID);
                            if (!oldAttchmentIDs.Contains(item) && !additionAttachments.Contains(item) && attchmentID > 0)
                            {
                                additionAttachments.Add(item);
                            }
                        }
                    }

                    if (additionAttachments.Count > 0)
                    {
                        if (originalAttachments.EndsWith(","))
                        {
                            attachments = originalAttachments + string.Join(",", additionAttachments.ToArray());
                        }
                        else
                        {
                            attachments = $"{originalAttachments}," + string.Join(",", additionAttachments.ToArray());
                        }
                    }
                }
                else
                {
                    //Nếu list mới không có giá trị thì mặc định là giá trị của list cũ
                    attachments = originalAttachments;
                }
            }

            return attachments;
        }

        private void AddOperationDiaryPositionShift(OperationDiaryPositionShiftSyncModel odps)
        {
            OperationDiaryPositionShift operationDiaryPositionShift = new OperationDiaryPositionShift
            {
                AppID = odps.AppID,
                OperationDiaryID = odps.OperationDiaryID,
                PositionInShiftID = odps.PositionInShiftID != null ? odps.PositionInShiftID : null,
                Label = !string.IsNullOrEmpty(odps.Label) ? odps.Label : null,
                IsDefault = odps.IsDefault,
                Position = odps.Position,
                SyncDataKey = odps.SyncDataKey,
                IsDeleted = odps.IsDeleted,
                CreatedUserID = odps.CreatedUserID,
                InsertedAt = odps.InsertedAt,
                LastUpdatedUserID = odps.LastUpdatedUserID,
                UpdatedAt = odps.UpdatedAt,
                SyncedDate = DateTime.Now
            };

            if (odps.AssignedUserID > 0)
                operationDiaryPositionShift.AssignedUserID = odps.AssignedUserID;
            _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().Add(operationDiaryPositionShift);
        }

        private void AddDeliveryReceiveShift(DeliveryReceiveShiftSyncModel receiveItem)
        {
            DeliveryReceiveShift deliveryReceiveShift = new DeliveryReceiveShift
            {
                AppID = receiveItem.AppID,
                OperationDiaryID = receiveItem.OperationDiaryID,
                OperationDiaryPositionShiftID = receiveItem.OperationDiaryPositionShiftID,
                SupervisorID = receiveItem.SupervisorID,
                IsDeliveryShift = receiveItem.IsDeliveryShift,
                SyncDataKey = receiveItem.SyncDataKey,
                IsDeleted = receiveItem.IsDeleted,
                CreatedUserID = receiveItem.CreatedUserID,
                InsertedAt = receiveItem.InsertedAt,
                LastUpdatedUserID = receiveItem.LastUpdatedUserID,
                UpdatedAt = receiveItem.UpdatedAt,
                SyncedDate = DateTime.Now
            };
            _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().Add(deliveryReceiveShift);
        }

        private void AddOperationSituation(OperationSituationSyncModel os, int? factoryID)
        {
            double.TryParse(os.Hour.Replace(":", "."), out double hour);
            OperationSituation operationSituation = new OperationSituation
            {
                AppID = os.AppID,
                OperationDiaryID = os.OperationDiaryID,
                Hour = hour,
                AssignedFromID = os.AssignedFromID != null ? os.AssignedFromID : null,
                AssignedFromName = !string.IsNullOrEmpty(os.AssignedFromName) ? os.AssignedFromName : null,
                AssignedToID = os.AssignedToID != null ? os.AssignedToID : null,
                AssignedToName = !string.IsNullOrEmpty(os.AssignedToName) ? os.AssignedToName : null,
                Description = !string.IsNullOrEmpty(os.Description) ? os.Description : null,
                SyncDataKey = os.SyncDataKey,
                IsDeleted = os.IsDeleted,
                CreatedUserID = os.CreatedUserID,
                InsertedAt = os.InsertedAt,
                LastUpdatedUserID = os.LastUpdatedUserID,
                UpdatedAt = os.UpdatedAt,
                SyncedDate = DateTime.Now,
                FactoryID = factoryID,
            };

            operationSituation.Attachments = !string.IsNullOrWhiteSpace(os.Attachments) ? os.Attachments : string.Empty;
            _unitOfWork.ElogbookRepository<OperationSituation>().Add(operationSituation);
        }

        private void AddActiveWork(ActiveWorkSyncModel os, int? factoryID)
        {
            double.TryParse(!string.IsNullOrWhiteSpace(os.StartHour) ? os.StartHour.Replace(":", ".") : "0", out double startHour);
            double.TryParse(!string.IsNullOrWhiteSpace(os.EndHour) ? os.EndHour.Replace(":", ".") : "0", out double endHour);
            ActiveWork activeWork = new ActiveWork
            {
                AppID = os.AppID,
                OperationDiaryID = os.OperationDiaryID,
                StartHour = startHour,
                EndHour = endHour,
                Description = !string.IsNullOrEmpty(os.Description) ? os.Description : null,
                SyncDataKey = os.SyncDataKey,
                IsDeleted = os.IsDeleted,
                CreatedUserID = os.CreatedUserID,
                InsertedAt = os.InsertedAt,
                LastUpdatedUserID = os.LastUpdatedUserID,
                UpdatedAt = os.UpdatedAt,
                SyncedDate = DateTime.Now,
                FactoryID = factoryID,
            };

            activeWork.Attachments = !string.IsNullOrWhiteSpace(os.Attachments) ? os.Attachments : string.Empty;
            _unitOfWork.ElogbookRepository<ActiveWork>().Add(activeWork);
        }

        private void AddExistInShift(ExistInShiftSyncModel eis, int? factoryID)
        {
            ExistInShift existInShift = new ExistInShift
            {
                AppID = eis.AppID,
                OperationDiaryID = eis.OperationDiaryID,
                RegistrationTicketName = !string.IsNullOrEmpty(eis.RegistrationTicketName) ? eis.RegistrationTicketName : null,
                TeamLeader = !string.IsNullOrEmpty(eis.TeamLeader) ? eis.TeamLeader : null,
                Area = !string.IsNullOrEmpty(eis.Area) ? eis.Area : null,
                Content = !string.IsNullOrEmpty(eis.Content) ? eis.Content : null,
                Attachments = eis.Attachments != null ? string.Join(",", eis.Attachments) : string.Empty,
                StatusID = eis.StatusID,
                IsDeleteIssueOfShift = eis.IsDeleteIssueOfShift,
                SyncDataKey = eis.SyncDataKey,
                IsDeleted = eis.IsDeleted,
                CreatedUserID = eis.CreatedUserID,
                InsertedAt = eis.InsertedAt,
                LastUpdatedUserID = eis.LastUpdatedUserID,
                UpdatedAt = eis.UpdatedAt,
                SyncedDate = DateTime.Now,
                FactoryID = factoryID,
            };

            existInShift.Attachments = !string.IsNullOrWhiteSpace(eis.Attachments) ? eis.Attachments : string.Empty;
            if (eis.CompletedDateTime != null && eis.CompletedDateTime.Year >= 1753)
            {
                existInShift.CompletedDateTime = eis.CompletedDateTime;
            }
            if (eis.RegistrationTicketID.GetValueOrDefault() > 0)
                existInShift.RegistrationTicketID = eis.RegistrationTicketID;

            _unitOfWork.ElogbookRepository<ExistInShift>().Add(existInShift);
        }

        #endregion

        public ResponseBaseModel GetOperationDiaryLabels(SearchOperationDiaryWebModel requestModel)
        {
            // Tên người tạo
            if (string.IsNullOrWhiteSpace(requestModel.FullName))
            {
                requestModel.FullName = "%";
            }
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
            // Ca vận hành
            if (requestModel.OperationShiftID <= 0)
            {
                requestModel.OperationShiftID = CommonConstant.ALL_KEY;
            }
            // Kíp vận hành
            if (requestModel.OperationCrewID <= 0)
            {
                requestModel.OperationCrewID = CommonConstant.ALL_KEY;
            }

            // Tìm kiếm Nhật ký vận hành theo ngày tạo
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("PageNumber", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageNumber,
                },
                new SqlParameter("PageSize", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageSize,
                },
                new SqlParameter("FullName", System.Data.SqlDbType.NVarChar)
                {
                    Value = requestModel.FullName,
                },
                new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.FactoryID,
                },
                new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.OperationPositionID,
                },
                new SqlParameter("OperationShiftID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.OperationShiftID,
                },
                new SqlParameter("OperationCrewID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.OperationCrewID,
                },
                new SqlParameter("FromDate", System.Data.SqlDbType.DateTime)
                {
                    Value = frDate,
                },
                new SqlParameter("ToDate", System.Data.SqlDbType.DateTime)
                {
                    Value = toDate,
                }
            };

            var datas = _unitOfWork.DataContext.Database
                .SqlQuery<SearchOperationDiaryResult>("Search_NKVH @PageNumber, @PageSize, @FullName, @FactoryID, " +
                "@OperationPositionID, @OperationShiftID, @OperationCrewID, @FromDate, @ToDate", sqlParams).ToList();

            var totalRecords = 0;
            var totalPages = 0;
            if (datas.Count > 0)
            {
                totalRecords = datas[0].TotalRecords;
                totalPages = totalRecords / requestModel.PageSize + (totalRecords % requestModel.PageSize > 0 ? 1 : 0);
            }

            List<BaseGroupDataModel<OperationDiaryModel>> results = new List<BaseGroupDataModel<OperationDiaryModel>>();
            foreach (var item in datas)
            {
                var groupDate = item.GroupDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
                var operationDiaryDetail = new BaseGroupDataModel<OperationDiaryModel>()
                {
                    GroupDate = groupDate,
                };
                results.Add(operationDiaryDetail);
            }

            return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
        }

        public ResponseBaseModel GetOperationDiaryDetails(SearchOperationDiaryWebModel requestModel)
        {
            // Dữ liệu Nhật ký vận hành đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => !x.IsDeleted);
            // Tên người tạo
            if (!string.IsNullOrWhiteSpace(requestModel.FullName))
            {
                entities = entities.Where(x => x.CreatedUser.FullName.Contains(requestModel.FullName));
            }
            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }
            // Ca vận hành
            if (requestModel.OperationShiftID > 0)
            {
                entities = entities.Where(x => x.OperationShiftID == requestModel.OperationShiftID);
            }
            // Kíp vận hành
            if (requestModel.OperationCrewID > 0)
            {
                entities = entities.Where(x => x.OperationCrewID == requestModel.OperationCrewID);
            }

            // Dữ liệu chi tiết của nhóm
            var masterDetail = entities;

            List<BaseGroupDataModel<OperationDiaryModel>> results = new List<BaseGroupDataModel<OperationDiaryModel>>();
            DateTime dateGroup = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateGroup, false);
            var operationDiaryDetail = new BaseGroupDataModel<OperationDiaryModel>()
            {
                MasterDetail = masterDetail.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == dateGroup).Select(
                    e => new OperationDiaryModel()
                    {
                        ID = e.ID,
                        CreateDate = e.CreatedDate,
                        OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                        OperationShift = new ValueLabelModel { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                        OperationCrew = new ValueLabelModel { Value = e.OperationCrew.ID, Label = e.OperationCrew.OperationCrewName },
                        CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                        Position = e.OperationShift != null ? e.OperationShift.Position : 0,
                        IsReceived = e.IsReceived,
                        IsDelivered = e.IsDelivered,
                        OperationDiaryPositionShifts = e.OperationDiaryPositionInShift.Where(x => !x.IsDeleted).Select(
                                            t => new OperationDiaryPositionShiftModel()
                                            {
                                                PositionInShiftID = t.PositionInShiftID.HasValue ? t.PositionInShiftID.Value : 0,
                                                AssignedUser = new ValueLabelModel { Value = t.AssignedUser.ID, Label = t.AssignedUser.FullName },
                                                Position = t.Position,
                                            }).OrderBy(z => z.Position).ToList(),
                    }).OrderBy(x => x.Position).ToList()
            };

            // Lấy danh sách quyền của người dùng
            var userRolesInfo = _commonService.GetUserRight(requestModel.CurrentUserID);
            var isAllowEditDelete = false;
            var isAllowEdit = false;
            var isAllowDelete = false;
            if (userRolesInfo != null && userRolesInfo.UsersRight != null && userRolesInfo.UsersRight.Count > 0)
            {
                // Kiểm tra người dùng có quyền cập nhật/xóa NKVH khi đã giao/nhận ca không
                if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "EDIT_DELETE_NKVH"))
                {
                    isAllowEditDelete = true;
                }

                // Kiểm tra người dùng có quyền cập nhật NKVH
                if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "EDIT_NKVH"))
                {
                    isAllowEdit = true;
                }

                // Kiểm tra người dùng có quyền xóa NKVH
                if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "DELETE_NKVH"))
                {
                    isAllowDelete = true;
                }
            }
            var isAdmin = false;
            if (userRolesInfo != null)
            {
                isAdmin = userRolesInfo.IsAdmin;
            }
            var shifts = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery().ToList();
            var position = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.JobTitle == (int)JobTitle.ShiftLeader).ToList();

            foreach (var detailItem in operationDiaryDetail.MasterDetail)
            {
                // Danh sách id của những cương vị trong ca được khai báo trong ca ghi NKVH
                List<int> lstPositionInShiftID = detailItem.OperationDiaryPositionShifts.Select(e => e.PositionInShiftID).ToList();
                // Danh sách cương vị trong ca
                var positionInShifts = shifts.Where(x => lstPositionInShiftID.Contains(x.ID)).ToList();
                // Xác định Trưởng ca
                var isHasLeaderShift = false;
                // Id cương vị trong ca
                var positionInShiftID = 0;
                // Xác định danh sách cương vị trong ca có tồn tại Trưởng ca
                foreach (var positionInShift in positionInShifts)
                {
                    // Lấy cương vị Trưởng ca
                    OperationPosition operationPosition = null;
                    if (position != null)
                    {
                        operationPosition = position.Find(x => x.ID == positionInShift.OperationPositionID);
                    }
                    // Nếu có cương vị Trưởng ca
                    if (operationPosition != null)
                    {
                        isHasLeaderShift = true;
                        positionInShiftID = positionInShift.ID;
                        break;
                    }
                }

                // Lấy người dùng thuộc cương vị Trưởng ca
                var assignedUserID = detailItem.OperationDiaryPositionShifts.Where(x => x.PositionInShiftID == positionInShiftID).Select(e => e.AssignedUser.Value).FirstOrDefault();
                // Cần xác định ai là người được sửa/xóa NKVH, ví dụ:
                // 1. Người đó không có tên trong danh sách ca trực thì có được cập nhật ca đó không
                // 2. hoặc người đó không phải là người tạo NKVH
                //(!detailItem.IsDelivered && isHasLeaderShift && assignedUserID == requestModel.CurrentUserID) -> chỉ trưởng ca của Ca đó mới được cập nhật
                //-> thay đổi yêu cầu, nếu người dùng được phân quyền thì sẽ được add/update/delete NKVH
                // Nếu cương vị Trưởng và người khai báo trùng với người login thì mới cho chỉnh sửa NKVH
                var isCreator = detailItem.CreatedUser != null && detailItem.CreatedUser.Value == requestModel.CurrentUserID;
                if (isAdmin
                    || (isAllowEdit && !detailItem.IsDelivered)
                    || (detailItem.IsDelivered && isHasLeaderShift && assignedUserID == requestModel.CurrentUserID && isAllowEditDelete)
                    || (isCreator && !detailItem.IsDelivered))
                {
                    detailItem.IsCanEdit = true;
                }

                if (isAdmin
                    || (isAllowDelete && !detailItem.IsDelivered)
                    || (detailItem.IsDelivered && isHasLeaderShift && assignedUserID == requestModel.CurrentUserID && isAllowEditDelete)
                    || (isCreator && !detailItem.IsDelivered))
                {
                    detailItem.IsCanDelete = true;
                }
            }

            results.Add(operationDiaryDetail);

            return HttpResponse.OkList(results);
        }

        public bool ExportOperationDiary(ExportOperationDiaryRequestModel requestModel)
        {
            DateTime fromDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, true);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            // Create ExportOperationDiary entity with Waiting status and save immediately
            var entity = new ExportOperationDiary
            {
                FactoryID = requestModel.FactoryID,
                OperationPositionID = requestModel.OperationPositionID,
                FromDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false),
                ToDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true),
                CreatedUserID = requestModel.CurrentUserID,
                Status = ExportStatus.Waiting,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<ExportOperationDiary>().Add(entity);
            var saved = _unitOfWork.Complete() > 0;

            if (!saved)
            {
                return false;
            }

            // Run export processing in background so caller receives immediate response
            Task.Factory.StartNew(() => StartExportOperationDiaryBackground(entity.ID, requestModel, fromDate, toDate), TaskCreationOptions.LongRunning);

            return true;
        }

        /// <summary>
        /// Background worker to process export operation diary: create zip, save attachment and update export record
        /// </summary>
        /// <param name="exportId"></param>
        /// <param name="requestModel"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        private void StartExportOperationDiaryBackground(int exportId, ExportOperationDiaryRequestModel requestModel, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var ctx = new ElogbookDataContext())
                using (var uow = new ElogbookUnitOfWork(ctx))
                {
                    // Re-query operation diaries in background scope
                    var operationDiarys = uow.ElogbookRepository<OperationDiary>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                                && x.OperationPositionID == requestModel.OperationPositionID
                                                                                                && DbFunctions.TruncateTime(x.CreatedDate) >= fromDate.Date
                                                                                                && DbFunctions.TruncateTime(x.CreatedDate) <= toDate.Date
                                                                                                && !x.IsDeleted).ToList();

                    var backgroundService = new OperationDiaryService(uow, _commonService, _pdfSigningService);

                    if (operationDiarys.Count == 0)
                    {
                        var exportEntity = uow.ElogbookRepository<ExportOperationDiary>().GetById(exportId);
                        if (exportEntity != null)
                        {
                            exportEntity.Status = ExportStatus.Failed;
                            exportEntity.Message = "Không có dữ liệu để xuất file.";
                            uow.ElogbookRepository<ExportOperationDiary>().Update(exportEntity);
                            uow.Complete();
                        }
                        return;
                    }

                    try
                    {
                        // Create zip and attachment using background service and its unit of work
                        Attachment attachment = backgroundService.CreateExportOperationDiaryZip(operationDiarys);
                        // Persist attachment and update export entity
                        uow.Complete();

                        var exportEntity = uow.ElogbookRepository<ExportOperationDiary>().GetById(exportId);
                        if (exportEntity != null)
                        {
                            if (attachment != null && attachment.ID > 0)
                            {
                                exportEntity.AttachmentID = attachment.ID;
                                exportEntity.Status = ExportStatus.Success;
                            }
                            else if (attachment != null && !string.IsNullOrEmpty(attachment.FilePath))
                            {
                                // attachment added but not saved - try to get by file path
                                var att = uow.ElogbookRepository<Attachment>().GetQuery(x => x.FilePath == attachment.FilePath).FirstOrDefault();
                                if (att != null)
                                {
                                    exportEntity.AttachmentID = att.ID;
                                    exportEntity.Status = ExportStatus.Success;
                                }
                                else
                                {
                                    exportEntity.Status = ExportStatus.Failed;
                                    exportEntity.Message = "Tạo file thất bại.";
                                }
                            }
                            else
                            {
                                exportEntity.Status = ExportStatus.Failed;
                                exportEntity.Message = "Tạo file thất bại.";
                            }

                            uow.ElogbookRepository<ExportOperationDiary>().Update(exportEntity);
                            uow.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportOperationDiary.Background", ex.ToString());
                        var exportEntity = uow.ElogbookRepository<ExportOperationDiary>().GetById(exportId);
                        if (exportEntity != null)
                        {
                            exportEntity.Status = ExportStatus.Failed;
                            exportEntity.Message = "Lỗi hệ thống. Vui lòng thử lại";
                            uow.ElogbookRepository<ExportOperationDiary>().Update(exportEntity);
                            uow.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportOperationDiary.BackgroundWorker", ex.ToString());
            }
        }

        public HttpResponseMessage DownloadExportOperationDiaryFile(int exportOperationDiaryId)
        {
            try
            {
                var exportOperationDiary = _unitOfWork.ElogbookRepository<ExportOperationDiary>()
                    .GetQuery(x => x.ID == exportOperationDiaryId && !x.IsDeleted)
                    .Include(x => x.AttachmentFile)
                    .FirstOrDefault();

                if (exportOperationDiary == null || exportOperationDiary.AttachmentFile == null)
                {
                    var errorResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("ExportOperationDiary hoặc Attachment không tồn tại.")
                    };
                    return errorResponse;
                }

                var filePath = Path.Combine(
                    AppSettings.FILE_STORAGE_SERVER_DIR,
                    exportOperationDiary.AttachmentFile.FilePath
                );

                if (!File.Exists(filePath))
                {
                    var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("File không tồn tại trên server.")
                    };
                    return notFoundResponse;
                }
                var fileBytes = File.ReadAllBytes(filePath);
                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), Path.GetFileName(filePath));
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService.DownloadExportOperationDiaryFile",
                    $"Error downloading file for ExportOperationDiary ID {exportOperationDiaryId}: {ex}");

                var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Có lỗi xảy ra khi tải file. Vui lòng liên hệ Admin.")
                };
                return errorResponse;
            }
        }

        public Attachment CreateExportOperationDiaryZip(List<OperationDiary> operationDiarys)
        {
            // Create PDFs in parallel using separate DbContext/UnitOfWork per task to avoid DbContext thread-safety issues
            List<string> filePaths = new List<string>();

            if (operationDiarys != null && operationDiarys.Count > 0)
            {
                var tasks = new List<Task<string>>();

                foreach (OperationDiary operationDiary in operationDiarys)
                {
                    if (operationDiary == null) continue;

                    var odId = operationDiary.ID;
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            // Use a fresh context and unit of work per task
                            using (var ctx = new ElogbookDataContext())
                            using (var uow = new ElogbookUnitOfWork(ctx))
                            {
                                var svc = new OperationDiaryService(uow, _commonService, _pdfSigningService);
                                return svc.CreateFilePdf(odId);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.ERROR, "CreateExportOperationDiaryZip", ex.ToString());
                            return null;
                        }
                    }));
                }
                Task.WaitAll(tasks.ToArray());
                foreach (var t in tasks)
                {
                    if (t.Status == TaskStatus.RanToCompletion && !string.IsNullOrWhiteSpace(t.Result))
                    {
                        filePaths.Add(t.Result);
                    }
                }
            }
            string fileName = $"Nhat_ky_van_hanh_{DateTime.Now:yyyyMMddHHmmss}.zip";
            string fileZipPath = CreateZipFile(filePaths, fileName);
            var attachment = new Attachment
            {
                FileName = fileName,
                FilePath = fileZipPath,
                ScreenName = CommonConstant.OPERATION_DIARY,
                FileType = "zip",
            };
            _unitOfWork.ElogbookRepository<Attachment>().Add(attachment);
            return attachment;

        }

        public string CreateZipFile(List<string> filePaths, string fileName)
        {
            try
            {
                if (filePaths == null || filePaths.Count == 0)
                {
                    return null;
                }

                // Tạo thư mục ZIP nếu không tồn tại
                string zipDirectory = Path.Combine(
                    AppSettings.FILE_STORAGE_SERVER_DIR,
                    CommonConstant.OPERATION_DIARY,
                    "ZIP"
                );

                if (!Directory.Exists(zipDirectory))
                {
                    Directory.CreateDirectory(zipDirectory);
                }

                // Tạo tên file zip
                string zipFilePath = Path.Combine(zipDirectory, fileName);

                // Tạo file zip
                using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var filePath in filePaths)
                    {
                        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                        {
                            // Lấy tên file từ đường dẫn
                            string sourceFileName = Path.GetFileName(filePath);

                            // Thêm file vào zip với tên gốc
                            zipArchive.CreateEntryFromFile(filePath, sourceFileName, CompressionLevel.Optimal);
                        }
                    }
                }

                // Trả về đường dẫn tương đối của file zip
                return Path.Combine(CommonConstant.OPERATION_DIARY, "ZIP", fileName);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService", ex.ToString());
                return string.Empty;
            }
        }

        private string CreateFilePdf(int id)
        {

            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            var operationDiary = GetOperationDiaryById(id);
            if (operationDiary == null)
            {
                return null;
            }

            if (operationDiary.PdfExportId.HasValue && operationDiary.PdfExportId > 0)
            {
                var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == operationDiary.PdfExportId && !x.IsDeleted).FirstOrDefault();
                if (attachment != null)
                {
                    return Path.Combine(_rootPath, attachment.FilePath);
                }
            }

            // tạo file PDF rùi ký vào file đó
            OperationDiaryPdfModel pdfDataModel = CreateDataPdf(operationDiary);
            if (!Directory.Exists(Path.Combine(_rootPath, "PDF")))
            {
                Directory.CreateDirectory(Path.Combine(_rootPath, "PDF"));
            }
            string fileName = $"Nhat_ky_van_hanh_{operationDiary.CreatedDate:yyyyMMdd}_{StringHelper.ToSnakeCaseSlug(operationDiary.OperationShift.Label)}_{StringHelper.ToSnakeCaseSlug(operationDiary.OperationCrew.Label)}_{operationDiary.ID}.pdf";
            string outputFile = Path.Combine(_rootPath, "PDF", fileName);
            CreatePDFContent(pdfDataModel, outputFile, true);
            return outputFile;
        }

        public ResponseBaseModel GetExportOperationDiary(BaseListFilter requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<ExportOperationDiary>().GetQuery(x => !x.IsDeleted).OrderByDescending(t => t.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);
            var dataResult = entities.Select(
                e => new ExportOperationDiaryResponseModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPosition = e.OperationPosition == null ? null : new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    OperationShift = e.OperationShift == null ? null : new ValueLabelModel { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                    FromDate = e.FromDate,
                    ToDate = e.ToDate,
                    Status = new ValueLabelModel { Value = (int?)e.Status, Label = EnumExtensions.GetEnumDescription(e.Status) },
                    InsertedAt = e.InsertedAt,
                    Message = e.Message
                }).ToList();
            return HttpResponse.OkList(dataResult, entities.PageNo, entities.PageSize, entities.TotalRecords, entities.TotalPages);
        }

        public bool DeleteExportOperationDiary(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<ExportOperationDiary>().GetById(id);
            if (entity != null)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "DeleteExportOperationDiary1", entity.AttachmentID.GetValueOrDefault());
                if (entity.AttachmentID.GetValueOrDefault() > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetById(entity.AttachmentID.Value);
                    if (attachment != null && !string.IsNullOrWhiteSpace(attachment.FilePath))
                    {
                        try
                        {
                            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
                            var relativeFilePath = attachment.FilePath.Trim().TrimStart('\\', '/');

                            var fullPath = Path.Combine(_rootPath, relativeFilePath);
                            if (File.Exists(fullPath))
                            {
                                File.Delete(fullPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.ERROR, "DeleteExportOperationDiary", ex.ToString());
                        }

                        _unitOfWork.ElogbookRepository<Attachment>().Delete(attachment, userID, true);
                    }
                }

                _unitOfWork.ElogbookRepository<ExportOperationDiary>().Delete(entity, userID, true);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public void DeleteReportOperationDiary()
        {
            var dateTime = DateTime.Now.AddDays(AppSettings.LAST_TIME_DELETE_REPORT_NKVH);
            var entities = _unitOfWork.ElogbookRepository<ExportOperationDiary>().GetQuery(x => !x.IsDeleted
                && x.InsertedAt < dateTime).ToList();

            foreach (var item in entities)
            {
                DeleteExportOperationDiary(item.ID, 0);
            }
        }

        /// <summary>
        /// Xuất dữ liệu nhân sự ca vận hành ra file Excel
        /// </summary>
        public HttpResponseMessage ExportPersonnelData(ExportOperationDiaryModel requestModel)
        {
            var rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                var data = GetPersonnelExportData(requestModel);

                if (!Directory.Exists(Path.Combine(rootPath, "OperationDiaryPersonnel")))
                    Directory.CreateDirectory(Path.Combine(rootPath, "OperationDiaryPersonnel"));

                string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string outputFile = Path.Combine(rootPath, "OperationDiaryPersonnel", $"NhanSuCaVanHanh_{currentDate}.xlsx");

                bool success = GeneratePersonnelExcelFile(data, outputFile, requestModel);
                LogCustom.LogMessage(TrackingLogLevel.INFO, "OperationDiaryService: ExportPersonnelData",
                    $"GenerateExcel success={success}, fileExists={File.Exists(outputFile)}, fileSize={(File.Exists(outputFile) ? new FileInfo(outputFile).Length : 0)}");

                if (success)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, "OperationDiaryService: ExportPersonnelData",
                        $"fileBytes.Length={fileBytes.Length}");
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                            File.Delete(outputFile);
                        return DownloadFile(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService: ExportPersonnelData", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        #endregion

        #region Private methods

        private OperationDiaryPdfModel CreateDataPdf(ViewOpeartionDiaryModel operationDiary)
        {
            OperationDiaryPdfModel model = new OperationDiaryPdfModel();

            model.PdfHeaderOperationDiaryModel = GetPdfHeaderInfo(operationDiary);

            string[] arrStartTime = operationDiary.OperationShiftInfo.StartTime.Split(':');
            string[] arrEndTime = operationDiary.OperationShiftInfo.EndTime.Split(':');
            string time = string.Empty;
            if (arrStartTime.Length > 1 && arrEndTime.Length > 1)
            {
                time = "Từ " + arrStartTime[0] + " giờ " + arrStartTime[1] + " đến " + arrEndTime[0] + " giờ " + arrEndTime[1];
            }
            var datePrint = operationDiary.CreatedDate;
            if (!operationDiary.Factory.Label.Equals("Bà Rịa", StringComparison.OrdinalIgnoreCase) && int.Parse(arrStartTime[0]) > int.Parse(arrEndTime[0]))
            {
                datePrint = datePrint.AddDays(1);
            }
            time = time + " ngày " + datePrint.Day + " tháng " + datePrint.Month + " năm " + datePrint.Year;
            model.PdfHeaderOperationDiaryModel.CreateDate = time;

            // danh sách cương vị trong ca
            model.WorkerModel = new List<WorkerModel>();
            if (operationDiary.OperationDiaryPositionShifts != null)
            {
                foreach (var item in operationDiary.OperationDiaryPositionShifts.OrderBy(x => x.Position))
                {
                    model.WorkerModel.Add(new WorkerModel()
                    {
                        WorkName = item.Label,
                        FullName = item.AssignedUser?.Label
                    });
                }
            }
            model.Comment = operationDiary.Comments;

            // Tồn tại trong ca
            model.ExistInShifts = new List<ExistInShiftModel>();
            if (operationDiary.ExistInShiftInfo.Count > 0)
            {
                foreach (var item in operationDiary.ExistInShiftInfo)
                {
                    model.ExistInShifts.Add(new ExistInShiftModel()
                    {
                        RegistrationTicket = item.RegistrationTicket,
                        TeamLeader = item.TeamLeader,
                        TeamLeaderUser = item.TeamLeaderUser,
                        Area = item.Area,
                        Content = item.Content,
                        ExistInShiftDetails = item.ExistInShiftDetails,
                        StatusObject = item.StatusObject,
                    });
                }
            }

            // Ca vận hành
            model.OperationSituations = new List<OperationSituationModel>();
            if (operationDiary.OperationSituations.Count > 0)
            {
                foreach (var item in operationDiary.OperationSituations)
                {
                    model.OperationSituations.Add(new OperationSituationModel()
                    {
                        Hour = item.Hour,
                        Description = item.Description,
                        AssignedFrom = item.AssignedFrom,
                        AssignedTo = item.AssignedTo
                    });
                }
            }

            //model.ActiveWorks = new List<ActiveWorkModel>();
            //if (operationDiary.ActiveWorks.Count > 0)
            //{
            //    foreach (var item in operationDiary.ActiveWorks)
            //    {
            //        model.ActiveWorks.Add(new ActiveWorkModel()
            //        {
            //            StartHour = item.StartHour,
            //            EndHour = item.EndHour,
            //            Description = item.Description,
            //        });
            //    }
            //}

            // tình hình bất thường hay sự cố
            model.AbnormalIncident = operationDiary.AbnormalIncident?.Description;

            // tình hình nhà máy lúc giao ca
            model.DeliveryShiftMethod = operationDiary.DeliveryShiftMethod?.Description;

            // giao/nhận ca
            model.DeliveryReceiveShiftInfo = new List<DeliveryReceiveShiftModel>();
            if (operationDiary.DeliveryReceiveShiftInfo.Count > 0)
            {
                model.DeliveryReceiveShiftInfo.AddRange(operationDiary.DeliveryReceiveShiftInfo);
            }
            model.OperationDiaryPositionShifts = operationDiary.OperationDiaryPositionShifts;

            //Danh sách Cương vị trong ca
            if (operationDiary.PositionInShifts != null && operationDiary.PositionInShifts.Count > 0)
            {
                List<List<string>> PositionInShifts = new List<List<string>>();
                List<string> labelValue = new List<string>();
                foreach (var item in operationDiary.PositionInShifts)
                {
                    if (labelValue.Count < 4)
                    {
                        labelValue.Add("  " + item.Label + ":");
                        if (item.AssignedUser != null)
                        {
                            labelValue.Add(item.AssignedUser.Label);
                        }
                        else
                        {
                            labelValue.Add(string.Empty);
                        }
                        continue;
                    }
                    if (labelValue.Count == 4)
                    {
                        PositionInShifts.Add(labelValue);
                        labelValue = new List<string>();
                        labelValue.Add("  " + item.Label + ":");
                        if (item.AssignedUser != null)
                        {
                            labelValue.Add(item.AssignedUser.Label);
                        }
                        else
                        {
                            labelValue.Add(string.Empty);
                        }
                    }
                }
                if (labelValue.Count > 0)
                {
                    PositionInShifts.Add(labelValue);
                }
                model.PositionInShifts = PositionInShifts;
            }

            model.ShiftReceiving = operationDiary.ShiftReceiving;

            return model;
        }

        /// <summary>
        /// Lấy dữ liệu cho bảng Header
        /// </summary>
        public PdfHeaderOperationDiaryModel GetPdfHeaderInfo(ViewOpeartionDiaryModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID == requestModel.OperationPosition.Value && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                PdfHeaderOperationDiaryModel pdfHeaderModel = new PdfHeaderOperationDiaryModel()
                {
                    OperationPositionName = entity.OperationPositionName,
                    FactoryName = entity.Factory.FactoryName,
                    ItemISO = "",
                    VersionRelease = "",
                    EffectiveDate = "",
                    DocumentCode = "",
                    OperationShiftName = requestModel.OperationShift.Label + " - " + requestModel.OperationCrew.Label,
                    PathLogo = GetOrganizationLogo(requestModel.Factory.Value)
                };
                return pdfHeaderModel;
            }
            return null;
        }

        private string GetOrganizationLogo(int? factoryId)
        {
            if (factoryId != null)
            {
                var entity = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == factoryId && !x.IsDeleted).FirstOrDefault();
                if (entity != null && entity.Company != null && entity.Company.AttachmentFile != null)
                {
                    return entity.Company.AttachmentFile.FilePath;
                }
            }

            return string.Empty;
        }

        public PdfPCell createCell(String content, float borderWidth, int colspan, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content));
            cell.BorderWidth = (borderWidth);
            cell.Colspan = (colspan);
            cell.HorizontalAlignment = (alignment);
            return cell;
        }

        private void CreateTablePDF(string outputFile, OperationDiaryPdfModel pdfDataModel, bool isPdfSignUserpassAll = false)
        {
            using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
            {
                //Create document
                Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                document.SetMargins(5, 5, 5, 5);
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, fileStream);

                HeaderFooter tevent = new HeaderFooter();
                pdfWriter.PageEvent = tevent;
                document.Open();
                PdfPTable table = new PdfPTable(5);
                float[] floatArray = { 1.2f, 1, 1, 1, 1.2f };
                table.SetWidths(floatArray);
                table.TotalWidth = 600;
                table.LockedWidth = true;
                table.HeaderRows = 5;

                AppendHeaderTitle(table, pdfDataModel.PdfHeaderOperationDiaryModel);

                // chèn danh sách cương vị trong ca vào file pdf
                AppendPositionInShifts(table, pdfDataModel);

                // tình hình lúc nhận ca
                AppendShiftReceiving(table, pdfDataModel);

                // Tình hình vận hành
                AppendOperationSituations(table, pdfDataModel);

                // Công tác đang thực hiện
                //AppendActiveWorks(table, pdfDataModel);

                // Nhận xét thông số
                //AppendComment(table, pdfDataModel);

                // tình hình bất thường hay sự cố
                AppendAbnormalIncident(table, pdfDataModel);

                // Tồn tại trong ca
                AppendExistInShiftsHeader(table);
                AppendExistInShifts(table, pdfDataModel);

                // Nhận xét thông số
                AppendComment(table, pdfDataModel);

                // Phương thức giao ca
                AppendDeliveryReceiveShiftInfo(table, pdfDataModel, isPdfSignUserpassAll);

                document.Add(table);
                document.Close();
            }
        }

        private bool CreatePDFContent(OperationDiaryPdfModel pdfDataModel, string outputFile, bool isPdfSignUserpassAll = false)
        {
            try
            {
                CreateTablePDF(outputFile, pdfDataModel, isPdfSignUserpassAll);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(pdfDataModel) + Environment.NewLine + ex.ToString());
                return false;
            }

            return true;
        }

        private void AppendPositionInShifts(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel)
        {
            if (pdfDataModel.PositionInShifts != null && pdfDataModel.PositionInShifts.Count > 0)
            {
                PdfPTable tblData = new PdfPTable(4);
                tblData.SetWidths(new float[] { 1, 1, 1, 1 });
                tblData.TotalWidth = 600;
                tblData.LockedWidth = (true);
                int fSize = 11;
                foreach (var lstPositionInShifts in pdfDataModel.PositionInShifts)
                {
                    foreach (var text in lstPositionInShifts)
                    {
                        if (text.Contains(":"))
                        {
                            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, text, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        }
                        else
                        {
                            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize, text, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        }

                    }
                    tblData.CompleteRow();
                }

                PdfPCell cellMerge = new PdfPCell();
                cellMerge.Colspan = 5;
                cellMerge.Padding = 0;
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);

                PdfPTable tblEmpty = new PdfPTable(1);
                tblEmpty.TotalWidth = 600;
                tblEmpty.LockedWidth = (true);
                tblEmpty.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                PdfPCell cellMerge2 = new PdfPCell();
                cellMerge2.Colspan = 5;
                cellMerge2.Padding = 0;
                cellMerge2.AddElement(tblEmpty);
                mainTable.AddCell(cellMerge2);
            }
        }

        private void AppendAbnormalIncident(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel)
        {
            PdfPTable tblData = new PdfPTable(1);
            tblData.SetWidths(new float[] { 1 });
            tblData.TotalWidth = 600;
            tblData.LockedWidth = true;
            int fSize = 9;
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "TÌNH HÌNH BẤT THƯỜNG HAY SỰ CỐ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (string.IsNullOrWhiteSpace(pdfDataModel.AbnormalIncident))
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 105));
            }
            else
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, pdfDataModel.AbnormalIncident, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP));
            }
            PdfPCell cellMerge = new PdfPCell();
            cellMerge.Colspan = 5;
            cellMerge.Padding = 0;
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendShiftReceiving(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel)
        {
            PdfPTable tblData = new PdfPTable(1);
            tblData.SetWidths(new float[] { 1 });
            tblData.TotalWidth = 600;
            tblData.LockedWidth = true;
            int fSize = 9;
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "TÌNH HÌNH LÚC NHẬN CA", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (string.IsNullOrWhiteSpace(pdfDataModel.ShiftReceiving))
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 105));
            }
            else
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, pdfDataModel.ShiftReceiving, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP));
            }
            PdfPCell cellMerge = new PdfPCell();
            cellMerge.Colspan = 5;
            cellMerge.Padding = 0;
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendComment(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel)
        {
            PdfPTable tblData = new PdfPTable(1);
            tblData.SetWidths(new float[] { 1 });
            tblData.TotalWidth = 600;
            tblData.LockedWidth = true;
            int fSize = 9;
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "NHẬN XÉT THÔNG SỐ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (string.IsNullOrWhiteSpace(pdfDataModel.AbnormalIncident))
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 105));
            }
            else
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, pdfDataModel.Comment, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP));
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Colspan = 5,
                Padding = 0
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendExistInShiftsHeader(PdfPTable mainTable)
        {
            PdfPTable tblData = new PdfPTable(1);
            tblData.SetWidths(new float[] { 1 });
            tblData.TotalWidth = 600;
            tblData.LockedWidth = true;
            int fSize = 9;
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "TỒN TẠI TRONG CA", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Colspan = 5,
                Padding = 0
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendExistInShifts(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel)
        {
            PdfPTable tblData = new PdfPTable(6);
            tblData.SetWidths(new float[] { 1, 1.2f, 1.2f, 4, 4, 1 });
            tblData.TotalWidth = 600;
            tblData.LockedWidth = true;
            int fSize = 9;

            if (pdfDataModel.ExistInShifts.Count > 0)
            {
                tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Phiếu công tác", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Chỉ huy trực tiếp", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Khu vực", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Nội dung công tác", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Nội dung xử lý", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Trạng thái", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                if (pdfDataModel.ExistInShifts != null && pdfDataModel.ExistInShifts.Count > 0)
                {
                    foreach (var item in pdfDataModel.ExistInShifts)
                    {
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.RegistrationTicket.Label, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.TeamLeader, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Area, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Content, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                        var content = string.Empty;
                        try
                        {
                            if (item.ExistInShiftDetails != null && item.ExistInShiftDetails.Count > 0)
                            {
                                foreach (var childItem in item.ExistInShiftDetails)
                                {
                                    if (string.IsNullOrWhiteSpace(content))
                                    {
                                        content = $"{childItem.CreatedUser.Label} - {childItem.CreateDate:dd/MM/yyyy HH:mm}";
                                        content = $"{content}\n {childItem.Content}";
                                    }
                                    else
                                    {
                                        content = $"{content}\n\n{childItem.CreatedUser.Label} - {childItem.CreateDate:dd/MM/yyyy HH:mm}";
                                        content = $"{content}\n {childItem.Content}";
                                    }
                                }
                            }
                        }
                        catch
                        {
                            //ignore error
                        }

                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, content, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.StatusObject.Label, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.CompleteRow();
                    }
                }
                else
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }

                PdfPCell cellMerge = new PdfPCell
                {
                    Colspan = 5,
                    Padding = 0
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }
            else
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Không có tồn tại mới trong ca", 6, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                PdfPCell cellMerge = new PdfPCell
                {
                    Colspan = 5,
                    Padding = 0
                };
                cellMerge.AddElement(tblData);
                mainTable.AddCell(cellMerge);
            }
        }

        private void AppendDeliveryReceiveShiftInfo(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel, bool isPdfSignUserpassAll = false)
        {
            // Phương thúc giao ca
            #region Phương thúc giao ca

            PdfPTable tblData1 = new PdfPTable(1);
            tblData1.SetWidths(new float[] { 1 });
            tblData1.TotalWidth = 600;
            tblData1.LockedWidth = true;
            int fSize = 9;

            tblData1.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "PHƯƠNG THỨC GIAO CA", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (string.IsNullOrWhiteSpace(pdfDataModel.DeliveryShiftMethod))
            {
                tblData1.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 105));
            }
            else
            {
                tblData1.AddCell(PdfHelper.GetCell(_fNomal, fSize, pdfDataModel.DeliveryShiftMethod, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP));
            }
            PdfPCell cellMerge = new PdfPCell
            {
                Colspan = 5,
                Padding = 0
            };
            cellMerge.AddElement(tblData1);
            mainTable.AddCell(cellMerge);

            #endregion

            if (isPdfSignUserpassAll)
            {
                //Giao ca, nhận ca
                #region Giao ca, nhận ca

                PdfPTable tblData2 = new PdfPTable(3);
                tblData2.SetWidths(new float[] { 1, 1, 2 });
                tblData2.TotalWidth = 600;
                tblData2.LockedWidth = true;

                //header
                tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Người nhận ca", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Người giao ca", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Nhận xét của cấp trên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                if (pdfDataModel.DeliveryReceiveShiftInfo != null && pdfDataModel.DeliveryReceiveShiftInfo.Count > 0)
                {
                    var deliverys = pdfDataModel.DeliveryReceiveShiftInfo.Where(x => x.IsDeliveryShift).ToList();
                    var receive = pdfDataModel.DeliveryReceiveShiftInfo.Where(x => !x.IsDeliveryShift).ToList();
                    for (int i = 0; i < deliverys.Count; i++)
                    {
                        tblData2.AddCell(PdfHelper.GetCellImageSignatureSetBorder(receive[i].Signature, 1, 1, 1, 0, 0, 0, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        if (receive[i] != null)
                        {
                            tblData2.AddCell(PdfHelper.GetCellImageSignatureSetBorder(deliverys[i].Signature, 1, 1, 1, 0, 0, 0, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        }

                        tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize, " ", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                        tblData2.AddCell(PdfHelper.GetCellSetBorder(_fNomal, fSize, receive[i].Supervisor.Label, 1, 1, 1, 0, 0, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        if (receive[i] != null)
                        {
                            tblData2.AddCell(PdfHelper.GetCellSetBorder(_fNomal, fSize, deliverys[i].Supervisor.Label, 1, 1, 1, 0, 0, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        }
                    }
                }
                else
                {
                    tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 63));
                    tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 63));
                    tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 63));
                }
                PdfPCell cellMerge2 = new PdfPCell();
                cellMerge2.Colspan = 5;
                cellMerge2.Padding = 0;
                cellMerge2.AddElement(tblData2);
                mainTable.AddCell(cellMerge2);

                #endregion
            }
            else
            {
                //Giao ca, nhận ca
                #region Giao ca, nhận ca

                PdfPTable tblData2 = new PdfPTable(3);
                tblData2.SetWidths(new float[] { 1, 1, 2 });
                tblData2.TotalWidth = 600;
                tblData2.LockedWidth = true;

                //header
                tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Người nhận ca", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Người giao ca", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData2.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Nhận xét của cấp trên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                foreach (var operationDiaryPosition in pdfDataModel.OperationDiaryPositionShifts)
                {
                    // 2. KHỐI NGƯỜI NHẬN CA (CỘT 1) - Tạo cố định, không loop
                    PdfPTable tblRecGroup = new PdfPTable(1);
                    tblRecGroup.WidthPercentage = 100;
                    // Ô trên: Khung chữ ký trống (Tên trường cố định: "Sig_Receive")
                    tblRecGroup.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Receive_" + operationDiaryPosition.Position, 1, 1, 1, 0, 55));
                    // Ô dưới: Khung họ tên trống (Tên trường cố định: "Name_Receive")
                    tblRecGroup.AddCell(PdfHelper.GetBlankTextField("Name_Receive_" + operationDiaryPosition.Position, _fNomal, fSize, 0, 0, 1, 0, 22));

                    PdfPCell cellGroupRec = new PdfPCell { Padding = 0, Border = Rectangle.NO_BORDER };
                    cellGroupRec.AddElement(tblRecGroup);
                    tblData2.AddCell(cellGroupRec);

                    // 3. KHỐI NGƯỜI GIAO CA (CỘT 2) - Tạo cố định, không loop
                    PdfPTable tblDelGroup = new PdfPTable(1);
                    tblDelGroup.WidthPercentage = 100;
                    // Ô trên: Khung chữ ký trống (Tên trường cố định: "Sig_Delivery")
                    tblDelGroup.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Delivery_" + operationDiaryPosition.Position, 1, 1, 1, 0, 55));
                    // Ô dưới: Khung họ tên trống (Tên trường cố định: "Name_Delivery")
                    tblDelGroup.AddCell(PdfHelper.GetBlankTextField("Name_Delivery_" + operationDiaryPosition.Position, _fNomal, fSize, 0, 0, 1, 0, 22));

                    PdfPCell cellGroupDel = new PdfPCell { Padding = 0, Border = Rectangle.NO_BORDER };
                    cellGroupDel.AddElement(tblDelGroup);
                    tblData2.AddCell(cellGroupDel);

                    // 4. Ô NHẬN XÉT CỦA CẤP TRÊN (CỘT 3)
                    tblData2.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 77));
                }

                PdfPCell cellMerge2 = new PdfPCell();
                cellMerge2.Colspan = 5;
                cellMerge2.Padding = 0;
                cellMerge2.AddElement(tblData2);
                mainTable.AddCell(cellMerge2);

                #endregion
            }
        }

        private void AppendOperationSituations(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel)
        {
            PdfPTable tblData = new PdfPTable(4);
            tblData.SetWidths(new float[] { 1, 1.2f, 1.2f, 9 });
            tblData.TotalWidth = 600;
            tblData.LockedWidth = true;
            int fSize = 9;

            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Giờ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Người ra lệnh", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Người nhận lệnh", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "TÌNH HÌNH VẬN HÀNH", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (pdfDataModel.OperationSituations != null && pdfDataModel.OperationSituations.Count > 0)
            {
                foreach (var item in pdfDataModel.OperationSituations)
                {
                    var descCell = PdfHelper.GetCell(_fNomal, fSize, item.Description, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);

                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Hour, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.AssignedFrom.Label, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.AssignedTo.Label, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(descCell);
                    tblData.CompleteRow();
                }
            }
            else
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }

            PdfPCell cellMerge = new PdfPCell();
            cellMerge.Colspan = 5;
            cellMerge.Padding = 0;
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendActiveWorks(PdfPTable mainTable, OperationDiaryPdfModel pdfDataModel)
        {
            PdfPTable tblData = new PdfPTable(3);
            tblData.SetWidths(new float[] { 1.6f, 1.6f, 9 });
            tblData.TotalWidth = 600;
            tblData.LockedWidth = true;
            int fSize = 9;

            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Giờ, phút bắt đầu", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "Giờ, phút kết thúc", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fBold, fSize + 2, "CÔNG TÁC ĐANG THỰC HIỆN", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (pdfDataModel.ActiveWorks != null && pdfDataModel.ActiveWorks.Count > 0)
            {
                foreach (var item in pdfDataModel.ActiveWorks)
                {
                    var descCell = PdfHelper.GetCell(_fNomal, fSize, item.Description, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);

                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.StartHour, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.EndHour, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(descCell);
                    tblData.CompleteRow();
                }
            }
            else
            {
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Colspan = 5,
                Padding = 0
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendHeaderTitle(PdfPTable table, PdfHeaderOperationDiaryModel model)
        {
            int fontSizeNormal = 9;
            int fontSizeHeader = 10;

            table.AddCell(PdfHelper.GetCellImageLogo(model.PathLogo, 1, 4, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 60, false));
            table.AddCell(PdfHelper.GetCell(_fBold, fontSizeNormal, model.OperationPositionName + " - " + model.FactoryName, 3, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeHeader, "Mã tài liệu", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            table.AddCell(PdfHelper.GetCell(_fBold, fontSizeNormal, model.DocumentCode, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeHeader, "Mục ISO: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeHeader, "Ban hành lần thứ: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeHeader, "Ngày hiệu lực: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeHeader, "Trang số: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeNormal, model.ItemISO, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeNormal, model.VersionRelease, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeNormal, model.EffectiveDate, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCell(_fNomal, fontSizeNormal, "", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCellSetBorder(_fBold, fontSizeNormal, model.OperationShiftName, 2, 1, 1, 0, 0, 0, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            table.AddCell(PdfHelper.GetCellSetBorder(_fBold, fontSizeNormal, model.CreateDate, 3, 1, 0, 0, 1, 0, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
        }

        #region Excel helpers for personnel export

        /// <summary>
        /// Gọi SP NKVH_GetPersonnelExport, trả về DataTable với các cột động:
        ///   WorkDate | FactoryID | FactoryName | PositionName | PositionOrder | [Ca1] | [Ca2] | ...
        /// </summary>
        private DataTable GetPersonnelExportData(ExportOperationDiaryModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            LogCustom.LogMessage(TrackingLogLevel.INFO, "OperationDiaryService: GetPersonnelExportData",
                $"FromDate input='{requestModel.FromDate}' parsed={frDate:yyyy-MM-dd} | ToDate input='{requestModel.ToDate}' parsed={toDate:yyyy-MM-dd} | FactoryID={requestModel.FactoryID}");

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("FactoryID", SqlDbType.Int)
                {
                    Value = requestModel.FactoryID
                },
                new SqlParameter("FromDate", SqlDbType.Date)
                {
                    Value = (frDate == DateTime.MinValue || frDate == System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                        ? (object)DBNull.Value : (object)frDate.Date
                },
                new SqlParameter("ToDate", SqlDbType.Date)
                {
                    Value = toDate == DateTime.MaxValue
                        ? (object)DBNull.Value : (object)toDate.Date
                },
            };

            var connStr = _unitOfWork.DataContext.Database.Connection.ConnectionString;
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            using (var adapter = new SqlDataAdapter(
                "NKVH_GetPersonnelExport @FactoryID, @FromDate, @ToDate", conn))
            {
                adapter.SelectCommand.Parameters.AddRange(sqlParams);
                adapter.SelectCommand.CommandTimeout = 120;
                adapter.Fill(dt);
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, "OperationDiaryService: GetPersonnelExportData",
                $"DataTable rows={dt.Rows.Count}, cols={dt.Columns.Count}");

            return dt;
        }

        private bool GeneratePersonnelExcelFile(DataTable data, string outputFile, ExportOperationDiaryModel requestModel)
        {
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            try
            {
                // SP trả flat data: WorkDate | FactoryID | FactoryName | ShiftLabel | ShiftOrder | PositionName | PositionOrder | FullName
                // C# tự pivot: group theo (WorkDate, Factory) → sheet; cột = ShiftLabel

                // Lấy danh sách ca theo thứ tự ShiftOrder
                var shiftCols = data.AsEnumerable()
                    .Select(r => new { Label = r["ShiftLabel"]?.ToString() ?? "", Order = Convert.ToInt32(r["ShiftOrder"]) })
                    .Distinct()
                    .OrderBy(x => x.Order)
                    .Select(x => x.Label)
                    .Distinct()
                    .ToList();

                int colTT = 1;
                int colPosition = 2;
                int colShiftStart = 3;
                int colLast = colShiftStart + shiftCols.Count - 1;

                // Group theo ngày → sheet
                var dateGroups = data.AsEnumerable()
                    .GroupBy(r => { try { return Convert.ToDateTime(r["WorkDate"]).Date; } catch { return DateTime.MinValue; } })
                    .OrderBy(g => g.Key);

                if (!dateGroups.Any())
                {
                    var emptyWs = workbook.CreateSheet("Không có dữ liệu");
                    var er = emptyWs.CreateRow(1);
                    PersonnelCreateCellStr(emptyWs, 1, 1, 1, 3, er, "Không có dữ liệu trong khoảng thời gian đã chọn.");
                }
                else
                {
                    foreach (var dateGroup in dateGroups)
                    {
                        DateTime workDate = dateGroup.Key;
                        var ws = workbook.CreateSheet(workDate.ToString("dd.MM"));
                        int rowIdx = 0;

                        // Group theo nhà máy → block
                        var factoryGroups = dateGroup
                            .GroupBy(r => new { ID = Convert.ToInt32(r["FactoryID"]), Name = r["FactoryName"]?.ToString() ?? "" })
                            .OrderBy(g => g.Key.Name);

                        foreach (var factoryGroup in factoryGroups)
                        {
                            string factoryName = factoryGroup.Key.Name;

                            // Block title
                            rowIdx++;
                            var blockTitleRow = ws.CreateRow(rowIdx);
                            PersonnelCreateCellStr(ws, rowIdx, rowIdx, colTT, colLast, blockTitleRow,
                                $"NGÀY {workDate:dd/MM/yyyy} - {factoryName.ToUpper()}");
                            PersonnelSetStyleBlockTitle(ws, rowIdx, colTT, colLast, workbook);

                            // Header
                            rowIdx++;
                            var headerRow = ws.CreateRow(rowIdx);
                            headerRow.CreateCell(colTT).SetCellValue("TT");
                            headerRow.CreateCell(colPosition).SetCellValue("CƯƠNG VỊ");
                            for (int i = 0; i < shiftCols.Count; i++)
                                headerRow.CreateCell(colShiftStart + i).SetCellValue(shiftCols[i].ToUpper());
                            PersonnelSetStyleHeader(ws,
                                new NPOI.SS.Util.CellRangeAddress(rowIdx, rowIdx, colTT, colLast),
                                new NPOI.XSSF.UserModel.XSSFColor(new byte[] { 169, 208, 142 }),
                                NPOI.SS.UserModel.BorderStyle.Thin);

                            int dataStartRow = rowIdx + 1;

                            // Pivot: group theo (PositionOrder, PositionName)
                            var positionGroups = factoryGroup
                                .GroupBy(r => new
                                {
                                    Order = Convert.ToInt32(r["PositionOrder"]),
                                    Name = r["PositionName"]?.ToString() ?? ""
                                })
                                .OrderBy(g => g.Key.Order);

                            int stt = 0;
                            foreach (var posGroup in positionGroups)
                            {
                                stt++;
                                rowIdx++;
                                var bodyRow = ws.CreateRow(rowIdx);
                                bodyRow.CreateCell(colTT).SetCellValue(stt);
                                bodyRow.CreateCell(colPosition).SetCellValue(posGroup.Key.Name);

                                // Với mỗi ca, lấy FullName tương ứng
                                for (int i = 0; i < shiftCols.Count; i++)
                                {
                                    var shiftName = shiftCols[i];
                                    var match = posGroup.FirstOrDefault(r => r["ShiftLabel"]?.ToString() == shiftName);
                                    bodyRow.CreateCell(colShiftStart + i).SetCellValue(match?["FullName"]?.ToString() ?? "");
                                }
                            }

                            PersonnelSetStyleBody(ws,
                                new NPOI.SS.Util.CellRangeAddress(dataStartRow, rowIdx, colTT, colLast),
                                new NPOI.XSSF.UserModel.XSSFColor(new byte[] { 221, 235, 247 }),
                                NPOI.SS.UserModel.BorderStyle.Thin);

                            rowIdx += 2; // khoảng trắng giữa 2 nhà máy
                        }

                        for (int c = colTT; c <= colLast; c++)
                            ws.AutoSizeColumn(c);
                    }
                }

                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    workbook.Write(fs);

                workbook.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationDiaryService: GeneratePersonnelExcelFile", ex.ToString());
                return false;
            }
        }

        private void PersonnelCreateCellStr(NPOI.SS.UserModel.ISheet sheet, int startRow, int endRow, int startCol, int endCol, NPOI.SS.UserModel.IRow row, string value)
        {
            row.CreateCell(startCol).SetCellValue(value);
            if (startRow != endRow || startCol != endCol)
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, endRow, startCol, endCol));
        }

        private void PersonnelSetStyleHeader(NPOI.SS.UserModel.ISheet sheet, NPOI.SS.Util.CellRangeAddress range, NPOI.XSSF.UserModel.XSSFColor color, NPOI.SS.UserModel.BorderStyle border)
        {
            var wb = (NPOI.XSSF.UserModel.XSSFWorkbook)sheet.Workbook;
            for (int r = range.FirstRow; r <= range.LastRow; r++)
            {
                var row = sheet.GetRow(r) ?? sheet.CreateRow(r);
                for (int c = range.FirstColumn; c <= range.LastColumn; c++)
                {
                    var cell = row.GetCell(c, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    var style = (NPOI.XSSF.UserModel.XSSFCellStyle)wb.CreateCellStyle();
                    style.SetFillForegroundColor(color);
                    style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                    var font = (NPOI.XSSF.UserModel.XSSFFont)wb.CreateFont();
                    font.IsBold = true;
                    font.FontName = "Times New Roman";
                    style.SetFont(font);
                    style.WrapText = true;
                    style.BorderTop = border;
                    style.BorderLeft = border;
                    style.BorderRight = border;
                    style.BorderBottom = border;
                    style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    cell.CellStyle = style;
                }
            }
        }

        /// <summary>
        /// Style cho dòng block title "NGÀY DD/MM/YYYY - TÊN NHÀ MÁY" — nền xanh dương nhạt, chữ đậm căn giữa
        /// </summary>
        private void PersonnelSetStyleBlockTitle(NPOI.SS.UserModel.ISheet sheet, int rowIdx, int colFirst, int colLast, NPOI.XSSF.UserModel.XSSFWorkbook wb)
        {
            var style = (NPOI.XSSF.UserModel.XSSFCellStyle)wb.CreateCellStyle();
            var font = (NPOI.XSSF.UserModel.XSSFFont)wb.CreateFont();
            font.FontName = "Times New Roman";
            font.IsBold = true;
            font.FontHeightInPoints = 12;
            // Màu xanh dương nhạt giống template
            style.SetFillForegroundColor(new NPOI.XSSF.UserModel.XSSFColor(new byte[] { 189, 215, 238 }));
            style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            style.SetFont(font);
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            var row = sheet.GetRow(rowIdx);
            for (int c = colFirst; c <= colLast; c++)
            {
                var cell = row.GetCell(c, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK);
                cell.CellStyle = style;
            }
        }

        private void PersonnelSetStyleBody(NPOI.SS.UserModel.ISheet sheet, NPOI.SS.Util.CellRangeAddress range, NPOI.XSSF.UserModel.XSSFColor color, NPOI.SS.UserModel.BorderStyle border)
        {
            var wb = (NPOI.XSSF.UserModel.XSSFWorkbook)sheet.Workbook;
            for (int r = range.FirstRow; r <= range.LastRow; r++)
            {
                var row = sheet.GetRow(r) ?? sheet.CreateRow(r);
                for (int c = range.FirstColumn; c <= range.LastColumn; c++)
                {
                    var cell = row.GetCell(c, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    var style = (NPOI.XSSF.UserModel.XSSFCellStyle)wb.CreateCellStyle();
                    style.SetFillForegroundColor(color);
                    style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                    var font = (NPOI.XSSF.UserModel.XSSFFont)wb.CreateFont();
                    font.FontName = "Times New Roman";
                    style.SetFont(font);
                    style.WrapText = true;
                    style.BorderTop = border;
                    style.BorderLeft = border;
                    style.BorderRight = border;
                    style.BorderBottom = border;
                    style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    cell.CellStyle = style;
                }
            }
        }

        private HttpResponseMessage DownloadFile(HttpContent content, string fileName)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return response;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Thông tin danh sách Nhật ký vận hành
    /// </summary>
    public class SearchOperationDiaryResult
    {
        /// <summary>
        /// Ngày tạo Nhật ký vận hành
        /// </summary>
        public DateTime GroupDate { get; set; }

        /// <summary>
        /// Số dòng dữ liệu map với điều kiện tìm kiếm
        /// </summary>
        public int TotalRecords { get; set; }
    }
}
