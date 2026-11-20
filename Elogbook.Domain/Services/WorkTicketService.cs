using Core.System.Runtime.Managers;
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
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Request.WorkTicket;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using Elogbook.Domain.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using ViettelFileSigner;

using HttpResponse = Elogbook.Core.Models.HttpResponse;

namespace Elogbook.Domain.Services
{
    public class WorkTicketService : IWorkTicketService
    {
        public class Result
        {
            public string Message { get; set; }
        }

        #region Private fields

        private readonly ICommonService _commonService;
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IRegistrationTicketService _registrationTicketService;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fBold = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private int posY = 815;
        private readonly string _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
        private PdfInfoModel _pdfInfo;
        private readonly int TABLE_WIDTH = 495;
        private readonly IEmailService _emailService;
        private readonly IPdfSigningService _pdfSigningService;
        private static readonly object _lockCompletionReminder = new object();

        #endregion

        #region Contructor

        public WorkTicketService(ElogbookUnitOfWork unitOfWork,
            ICommonService commonService,
            IRegistrationTicketService registrationTicketService,
            IEmailService emailService,
            IPdfSigningService pdfSigningService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _registrationTicketService = registrationTicketService;
            _emailService = emailService;
            _pdfSigningService = pdfSigningService;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Tìm kiếm một Phiếu công tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WorkTicketDetailModel GetWorkTicketById(int id)
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
                RegistTicketInfo = new BasicDetailModel
                {
                    ID = e.RegistrationTicketID.HasValue ? e.RegistrationTicket.ID : 0,
                    Title = e.RegistrationTicketID.HasValue ? e.RegistrationTicket.TicketNumber : "",
                    Status = e.RegistrationTicketID.HasValue ? e.RegistrationTicket.TicketStatus.StatusName : ""
                },
                MajorRepairInfo = new BasicDetailModel
                {
                    ID = e.MajorRepairID.HasValue ? e.MajorRepair.ID : 0,
                    Title = e.MajorRepairID.HasValue ? e.MajorRepair.Title : "",
                    Status = e.MajorRepairID.HasValue ? e.MajorRepair.MajorRepairStatus.Status : ""
                }
            }).FirstOrDefault();

            if (entity != null && entity.ID > 0)
            {
                var approverUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Approver);
                var leaderUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Leader);
                var commanderUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Commander);
                var supervisorUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Supervisor);
                var representativeUnitUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.RepresentativeUnit);
                var acceptorUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Acceptor);
                var completeConfirmUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.CompletedConfirm);

                entity.ApproverUser = approverUserEntity;
                if (approverUserEntity != null)
                    entity.ApproverSignature = approverUserEntity.Signature;
                entity.LeaderUser = leaderUserEntity;
                entity.CommanderUser = commanderUserEntity;
                entity.AcceptorUser = acceptorUserEntity;
                if (completeConfirmUserEntity != null && !string.IsNullOrWhiteSpace(completeConfirmUserEntity.FullName))
                {
                    entity.CompleteConfirmUser = completeConfirmUserEntity;
                }
                else
                {
                    entity.CompleteConfirmUser = entity.ApproverUser;
                }

                // thông tin cho phép
                entity.AllowedInfo = GetAllowedInfo(id);
                if (entity.AllowedInfo != null)
                {
                    entity.AllowedInfo.Acceptor = acceptorUserEntity;
                    entity.AllowedInfo.Commander = commanderUserEntity;
                }
                // tiếp nhận nơi làm việc
                entity.ReceiveWorkplace = GetReceiveWorkplace(id);
                if (entity.ReceiveWorkplace != null)
                {
                    entity.ReceiveWorkplace.LeaderUser = leaderUserEntity;
                    entity.ReceiveWorkplace.CommanderUser = commanderUserEntity;
                    entity.ReceiveWorkplace.SupervisorUser = supervisorUserEntity;
                }
                // file đính kèm
                if (!string.IsNullOrWhiteSpace(entity.AttachmentsID))
                {
                    entity.Attachments = GetAttachedFile(entity.AttachmentsID);
                }

                // hình ảnh trong quá trình công tác
                if (!string.IsNullOrWhiteSpace(entity.WorkPicturesID))
                {
                    entity.WorkPictures = GetAttachedFile(entity.WorkPicturesID);
                }

                // kết thúc công tác trao trả nơi làm việc
                entity.WorkFinish = GetWorkFinish(id);
                if (entity.WorkFinish != null)
                {
                    // Thông tin "Cho đại diện đơn vị quản lý"
                    // Là thông tin hiển thị ở mục "6.2 Đã tiếp nhận và kiểm tra nơi làm việc, phiếu công tác đã khóa"
                    var representativeUnitInfo = new LeadershipUserModel()
                    {
                        FullName = entity.WorkFinish?.RepresentativeUnit,
                        Signature = entity.WorkFinish?.AllowedSignature?.SignatureContent,
                        PositionName = entity.WorkFinish?.RepresentativeUnitPositionName,
                    };
                    if (string.IsNullOrWhiteSpace(representativeUnitInfo.FullName))
                        representativeUnitInfo.Signature = null;

                    if (representativeUnitUserEntity != null)
                        entity.WorkFinish.RepresentativeUnitUser = representativeUnitUserEntity;
                    else
                        entity.WorkFinish.RepresentativeUnitUser = representativeUnitInfo;

                    entity.WorkFinish.LeaderUser = leaderUserEntity;
                    entity.WorkFinish.CommanderUser = commanderUserEntity;
                    // Thay đổi thông tin người cho phép hiển thị ở tab "Kêt thúc công tác"
                    entity.WorkFinish.AllowedUser = representativeUnitInfo;
                    entity.WorkFinish.ApproverUser = approverUserEntity;
                }

                // Danh sách những người được đề cử để thực hiện công việc
                if (entity.RegistrationTicket != null && entity.RegistrationTicket.Value.HasValue)
                {
                    entity.WorkerUsers = _registrationTicketService.GetStakeholders(entity.RegistrationTicket.Value.Value, RolesStakeholder.Worker);
                }

                // Người giám sát
                if (entity.RegistrationTicket != null && entity.RegistrationTicket.Value.HasValue)
                {
                    var supervisors = _registrationTicketService.GetStakeholders(entity.RegistrationTicket.Value.Value, RolesStakeholder.Supervisor);
                    entity.Supervisors = supervisors;
                }
            }

            return entity;
        }

        /// <summary>
        /// Tìm kiếm một Phiếu công tác theo ID (mobile)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isWaitingTicket"></param>
        /// <returns></returns>
        public WorkTicketDetailMobileModel GetWorkTicketByIdMobile(int id, bool isWaitingTicket)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.Common.ToString(), Environment.NewLine + "Start GetWorkTicketByIdMobile");
            var data = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (data != null)
            {
                var entity = new WorkTicketDetailMobileModel()
                {
                    ID = data.ID,
                    WorkTicketNumber = data.WorkTicketNumber,
                    ApprovedUnit = new ValueLabelModel { Value = data.ApprovedUnit.ID, Label = data.ApprovedUnit.Name },
                    ApprovedDate = (data.ApprovedDate != null && data.ApprovedDate != DateTime.MinValue) ? data.ApprovedDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                    WorkTicketStatus = new ValueLabelModel { Value = data.TicketStatus.ID, Label = data.TicketStatus.StatusName },
                    UnitNumber = data.UnitNumber ?? 0,
                    WorkPlacement = data.WorkPlacement,
                    WorkContent = data.WorkContent,
                    PlanTime = data.StartDate.ToString("dd/MM/yyyy HH:mm") + " - " + data.EndDate.ToString("dd/MM/yyyy HH:mm"),
                    WorkCondition = data.WorkCondition,
                    WorkTicketTypeValue = data.WorkTicketType,
                    WorkScope = data.WorkScope,
                    WorkTicketFeature = data.WorkTicketFeature != null ? new ValueLabelModel { Value = data.WorkTicketFeature?.ID, Label = data.WorkTicketFeature?.FeatureName } : new ValueLabelModel { Value = 0, Label = null },
                    WorkTicketFeatures = data.WorkTicketFeatureLinks.Select(t => new WorkTicketFeaturesModel()
                    {
                        Label = t.WorkTicketFeatures.FeatureName,
                        FeatureID = t.FeatureID,

                    }).ToList(),
                };

                var approverUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Approver);
                var leaderUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Leader);
                var commanderUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Commander);
                var acceptorUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Acceptor);
                var supervisorUserEntity = GetLeadershipById(id, (int)LeadershipUserRoles.Supervisor);

                entity.ApproverUser = approverUserEntity;
                entity.LeaderUser = leaderUserEntity;
                entity.CommanderUser = commanderUserEntity;
                entity.AcceptorUser = acceptorUserEntity;

                // danh sách WarnAndSafes
                var warnAndSafeEntities = _unitOfWork.ElogbookRepository<WarnAndSafe>().GetQuery(x => !x.IsDeleted && x.WorkTicketID == data.ID);
                if (warnAndSafeEntities != null)
                {
                    var warnAndSafe = warnAndSafeEntities.Select(e => new WarnAndSafeModel()
                    {
                        SafetyMeasureContent = e.SafetyMeasureContent,
                        WarningContent = e.WarningContent,
                    }).ToList();
                    entity.WarnAndSafes = warnAndSafe;
                }

                // thông tin cho phép
                entity.AllowedInfo = GetAllowedInfo(id);
                if (entity.AllowedInfo != null)
                {
                    entity.AllowedInfo.Acceptor = acceptorUserEntity;
                }

                // thông tin tổ chức thực hiện công việc
                entity.ReceiveWorkplace = GetReceiveWorkplace(id);
                if (entity.ReceiveWorkplace != null)
                {
                    entity.ReceiveWorkplace.LeaderUser = leaderUserEntity;
                    entity.ReceiveWorkplace.CommanderUser = commanderUserEntity;
                    entity.ReceiveWorkplace.SupervisorUser = supervisorUserEntity;
                }

                int rgTicketID = 0;
                if (data.RegistrationTicketID != null)
                    rgTicketID = data.RegistrationTicketID.GetValueOrDefault(0);

                // Thông tin chi tiết phiếu đăng ký
                RegistrationDetailModel registrationDetailModel = GetRegistrationDetail(rgTicketID);
                if (registrationDetailModel != null)
                {
                    entity.RegistrationDetails = registrationDetailModel;
                    entity.RequestTicketID = registrationDetailModel.RequestTicketID;
                }

                // hình ảnh trong quá trình công tác
                if (!string.IsNullOrWhiteSpace(data.WorkPictures))
                {
                    entity.WorkPictures = GetAttachedFile(data.WorkPictures);
                }

                // kêt quả công tác
                entity.WorkResult = data.WorkResult;

                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.Common.ToString(), Environment.NewLine + "End GetWorkTicketByIdMobile");

                return entity;
            }

            return null;
        }

        /// <summary>
        /// Trả về danh sách thông tin nhân viên đơn vị công tác (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkDailys(int workTicketID, int? pageNumber, int? pageSize, bool isMobile)
        {
            int? pNum = pageNumber != null ? pageNumber : CommonConstant.DEFAULT_PAGE_NUMBER;
            int? pSize = pageSize != null ? pageSize : int.MaxValue;

            var entities = _unitOfWork.ElogbookRepository<WorkDaily>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted);
            if (isMobile)
            {
                var page = entities.Select(e => new WorkDailyDetailModel()
                {
                    ID = e.ID,
                    WorkLocationText = e.WorkLocation != null ? e.WorkLocation.Position : e.WorkLocationText,
                    WorkLocationID = e.WorkLocationID ?? 0,
                    WorkDayTemp = e.WorkDay,
                    WorkDay = e.WorkDay,
                    StaffInformations = e.StaffInformations.Where(x => !x.IsDeleted).Select(t => new StaffInformationsModel()
                    {
                        ID = t.ID,
                        FullName = t.FullName,
                        UserName = t.UserName,
                        Mission = t.Mission,
                        PositionName = t.PositionName,
                        ElectricRankCard = t.ElectricRankCard,
                        CheckInTime = t.CheckInTime,
                        CheckInSignatureID = t.CheckInSignature,
                        CheckOutTime = t.CheckOutTime,
                        CheckOutSignatureID = t.CheckOutSignature,
                        IsExternal = t.IsExternal,
                    }).ToList(),
                }).OrderBy(t => t.WorkDay).ToList();

                return HttpResponse.OkList(page);
            }
            else
            {
                var masterData = entities;
                IPagedList<WorkDailyDetailModel> dataGroup = entities.Select(
                         e => new WorkDailyDetailModel()
                         {
                             GroupDate = DbFunctions.TruncateTime(e.WorkDay),
                         }).Distinct().OrderBy(x => x.GroupDate)
                         .ToPagedList((int)pNum, (int)pSize);

                List<BaseGroupDataModel<WorkDailyDetailModel>> results = new List<BaseGroupDataModel<WorkDailyDetailModel>>();
                foreach (WorkDailyDetailModel item in dataGroup)
                {
                    var groupDate = item.GroupDate.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM/yyyy");

                    var lstWorkDaily = masterData.Where(x => DbFunctions.TruncateTime(x.WorkDay) == item.GroupDate.Value)
                                    .Select(e => new WorkDailyDetailModel()
                                    {
                                        ID = e.ID,
                                        WorkLocationText = e.WorkLocation != null ? e.WorkLocation.Position : e.WorkLocationText,
                                        WorkLocationID = e.WorkLocationID ?? 0,
                                        WorkDayTemp = e.WorkDay,
                                        WorkDay = e.WorkDay,
                                        StaffInformations = e.StaffInformations.Where(x => !x.IsDeleted).Select(t => new StaffInformationsModel()
                                        {
                                            ID = t.ID,
                                            FullName = t.FullName,
                                            UserName = t.UserName,
                                            Mission = t.Mission,
                                            IsExternal = t.IsExternal,
                                            PositionName = t.PositionName,
                                            ElectricRankCard = t.ElectricRankCard,
                                            CheckInTime = t.CheckInTime,
                                            CheckInSignatureID = t.CheckInSignature,
                                            CheckOutTime = t.CheckOutTime,
                                            CheckOutSignatureID = t.CheckOutSignature,
                                        }).ToList(),
                                    }).ToList();

                    if (lstWorkDaily != null && lstWorkDaily.Count > 0)
                    {
                        foreach (var wd in lstWorkDaily)
                        {
                            List<StaffInformationsModel> lstStaff = wd.StaffInformations;

                            if (lstStaff != null && lstStaff.Count > 0)
                            {
                                wd.StaffInformations = new List<StaffInformationsModel>();
                                foreach (var staff in lstStaff)
                                {
                                    StaffInformationsModel staffInfo = new StaffInformationsModel()
                                    {
                                        ID = staff.ID,
                                        FullName = staff.FullName,
                                        UserName = staff.UserName,
                                        Mission = staff.Mission,
                                        PositionName = staff.PositionName,
                                        IsExternal = staff.IsExternal,
                                        ElectricRankCard = staff.ElectricRankCard,
                                        CheckInTime = staff.CheckInTime,
                                        CheckInSignatureID = staff.CheckInSignatureID,
                                        CheckInSignature = staff.CheckInSignatureID != null ? _commonService.GetSignatureById((int)staff.CheckInSignatureID).SignatureContent : null,
                                        CheckOutTime = staff.CheckOutTime,
                                        CheckOutSignatureID = staff.CheckOutSignatureID,
                                        CheckOutSignature = staff.CheckOutSignatureID != null ? _commonService.GetSignatureById((int)staff.CheckOutSignatureID).SignatureContent : null,
                                    };
                                    wd.StaffInformations.Add(staffInfo);
                                }
                            }
                        }
                    }
                    results.Add(new BaseGroupDataModel<WorkDailyDetailModel>()
                    {
                        GroupDate = groupDate,
                        MasterDetail = lstWorkDaily,
                    });
                }

                return HttpResponse.OkList(results, (int)pNum, (int)pSize, dataGroup.TotalRecords, dataGroup.TotalPages);
            }
        }

        /// <summary>
        /// Trả về danh sách thông tin nhân viên đơn vị công tác (959)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkerInformations(int workTicketID, int pageNumber, int pageSize, bool isMobile)
        {
            if (isMobile)
            {
                var entities = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted);

                var masterData = entities;
                IPagedList<WorkerInformationModel> dataGroup = entities.Select(
                         e => new WorkerInformationModel()
                         {
                             GroupDate = DbFunctions.TruncateTime(e.InsertedAt),
                         }).Distinct().OrderBy(x => x.GroupDate)
                         .ToPagedList(pageNumber, pageSize);

                List<BaseGroupDataModel<WorkerInformationModel>> results = new List<BaseGroupDataModel<WorkerInformationModel>>();
                foreach (WorkerInformationModel item in dataGroup)
                {
                    var groupDate = item.GroupDate.GetValueOrDefault(DateTime.MinValue).ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
                    results.Add(new BaseGroupDataModel<WorkerInformationModel>()
                    {
                        GroupDate = groupDate,
                        MasterDetail = masterData.Where(x => DbFunctions.TruncateTime(x.InsertedAt) == item.GroupDate.Value)
                                    .Select(e => new WorkerInformationModel()
                                    {
                                        ID = e.ID,
                                        FullName = e.FullName,
                                        CheckInDateTime = e.CheckInTime,
                                        CheckInSignatureID = e.CheckInSignature,
                                        CheckOutDateTime = e.CheckOutTime,
                                        CheckOutSignatureID = e.CheckOutSignature,
                                        IsExternal = e.IsExternal,
                                        UserName = e.UserName,
                                    }).ToList()
                    });
                }

                return HttpResponse.OkList(results, pageNumber, pageSize, dataGroup.TotalRecords, dataGroup.TotalPages);
            }
            else
            {

                var entities = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted);

                var masterData = entities;
                IPagedList<WorkerInformationModel> dataGroup = entities.Select(
                         e => new WorkerInformationModel()
                         {
                             GroupDate = DbFunctions.TruncateTime(e.InsertedAt),
                         }).Distinct().OrderBy(x => x.GroupDate)
                         .ToPagedList(pageNumber, pageSize);

                List<BaseGroupDataModel<WorkerInformationModel>> results = new List<BaseGroupDataModel<WorkerInformationModel>>();
                foreach (WorkerInformationModel item in dataGroup)
                {
                    var groupDate = item.GroupDate.GetValueOrDefault(DateTime.MinValue).ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);

                    var masterDetail = masterData.Where(x => DbFunctions.TruncateTime(x.InsertedAt) == item.GroupDate.Value)
                                    .Select(e => new WorkerInformationModel()
                                    {
                                        ID = e.ID,
                                        FullName = e.FullName,
                                        CheckInDateTime = e.CheckInTime,
                                        CheckInSignatureID = e.CheckInSignature,
                                        CheckOutDateTime = e.CheckOutTime,
                                        CheckOutSignatureID = e.CheckOutSignature,
                                        IsExternal = e.IsExternal,
                                        UserName = e.UserName,
                                    }).ToList();

                    foreach (var itm in masterDetail)
                    {
                        itm.CheckInSignature = itm.CheckInSignatureID.HasValue ? _commonService.GetSignatureById((int)itm.CheckInSignatureID).SignatureContent : null;
                        itm.CheckOutSignature = itm.CheckOutSignatureID.HasValue ? _commonService.GetSignatureById((int)itm.CheckOutSignatureID).SignatureContent : null;
                    }

                    results.Add(new BaseGroupDataModel<WorkerInformationModel>()
                    {
                        GroupDate = groupDate,
                        MasterDetail = masterDetail
                    });
                }

                return HttpResponse.OkList(results, pageNumber, pageSize, dataGroup.TotalRecords, dataGroup.TotalPages);
            }
        }

        /// <summary>
        /// Trả về danh sách thông tin địa điểm công tác hằng ngày
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkPlacements(int workTicketID, int pageNumber, int pageSize)
        {
            var entities = _unitOfWork.ElogbookRepository<WorkPlacement>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted);

            var masterData = entities;
            IPagedList<WorkPlacementModel> dataGroup = entities.Select(
                     e => new WorkPlacementModel()
                     {
                         GroupDate = DbFunctions.TruncateTime(e.InsertedAt),
                     }).Distinct().OrderBy(x => x.GroupDate)
                     .ToPagedList(pageNumber, pageSize);

            List<BaseGroupDataModel<WorkPlacementModel>> results = new List<BaseGroupDataModel<WorkPlacementModel>>();
            foreach (WorkPlacementModel item in dataGroup)
            {
                var groupDate = item.GroupDate.GetValueOrDefault(DateTime.MinValue).ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
                results.Add(new BaseGroupDataModel<WorkPlacementModel>()
                {
                    GroupDate = groupDate,
                    MasterDetail = masterData.Where(x => DbFunctions.TruncateTime(x.InsertedAt) == item.GroupDate.Value).Select(e => new WorkPlacementModel()
                    {
                        ID = e.ID,
                        Place = e.Place,
                        //Người chỉ huy trực tiếp
                        CommanderName = e.CommanderName,
                        StartTimeTemp = e.StartTime,
                        EndTimeTemp = e.EndTime,
                        CommanderSignatureID = e.CommanderSignature,
                        //Người cho phép
                        AllowerName = e.AcceptorName,
                        AllowerSignatureID = e.AcceptorSignature,
                    }).ToList()
                });
            }

            return HttpResponse.OkList(results, pageNumber, pageSize, dataGroup.TotalRecords, dataGroup.TotalPages);
        }

        /// <summary>
        /// Trả về danh sách thông tin địa điểm công tác hằng ngày ở mobile
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkPlacements(int workTicketID)
        {
            var entities = _unitOfWork.ElogbookRepository<WorkPlacement>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted)
                            .Select(
                                     e => new WorkPlacementModel()
                                     {
                                         ID = e.ID,
                                         Place = e.Place,
                                         //Người chỉ huy trực tiếp
                                         CommanderName = e.CommanderName,
                                         StartTimeTemp = e.StartTime,
                                         EndTimeTemp = e.EndTime,
                                         CommanderSignatureID = e.CommanderSignature,
                                         //Người chi phép
                                         AllowerName = e.AcceptorName,
                                         AllowerSignatureID = e.AcceptorSignature,
                                     })
                     .ToList();

            return HttpResponse.OkList(entities);
        }

        /// <summary>
        /// Tìm kiếm tất cả phiếu công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkTickets(SearchWorkTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var workTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.WaitForRegist
                                                                                        && x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.Waiting
                                                                                        && !x.IsDeleted);
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
                WorkContent = e.WorkContent,
                Note = e.WorkResult,
                ScreenType = e.ScreenType,
                Approver = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Approver && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                Acceptor = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                InsertedAt = e.InsertedAt,
                IsFlagNote = e.IsFlagNote,
            }).ToList();

            var lstId = page.Select(x => x.ID).ToList();

            var workTicketFeatureLinks = _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>()
                .GetQuery(x => !x.IsDeleted && lstId.Contains(x.WorkTicketID)).Select(e => new WorkTicketFeaturesModel()
                {
                    ID = e.ID,
                    WorkTicketID = e.WorkTicketID,
                    FeatureID = e.FeatureID,
                    Label = e.WorkTicketFeatures.FeatureName,
                }).ToList();

            var workFinishData = _unitOfWork.ElogbookRepository<WorkFinish>()
                .GetQuery(x => !x.IsDeleted && lstId.Contains(x.WorkTicketID)).ToList();

            if (page.Count > 0)
            {
                foreach (var item in page)
                {
                    item.WorkTicketFeatures = workTicketFeatureLinks.FindAll(x => x.WorkTicketID == item.ID);

                    var workFinish = workFinishData.Find(x => x.WorkTicketID == item.ID);
                    if (workFinish != null)
                    {
                        if (item.ScreenType.Value == (int)TicketTypePCT.COTHUYNHIETHOA)
                        {
                            item.CompletedTimeLocal = workFinish.CheckReceivedTime;
                        }
                        else
                        {
                            item.CompletedTimeLocal = workFinish.CheckReceivedTime;
                        }
                    }
                }

                // Mặc định sắp xếp giảm dần theo ngày cấp phiếu
                if (string.IsNullOrWhiteSpace(requestModel.SortColumn))
                {
                    List<WorkTicketModel> dataResult = page.OrderByDescending(x => x.ApprovedDateLocal.GetValueOrDefault().Date).ThenBy(t => t.WorkTicketNumber).ToList();
                    return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                }

                requestModel.SortColumn = string.IsNullOrWhiteSpace(requestModel.SortColumn) ? "" : requestModel.SortColumn.ToUpper();
                requestModel.SortOrder = string.IsNullOrWhiteSpace(requestModel.SortOrder) ? "" : requestModel.SortOrder.ToUpper();

                // Sắp xếp theo Ngày cấp phiếu
                if (requestModel.SortColumn.Equals("CREATEAT"))
                {
                    if ("DESC".Equals(requestModel.SortOrder))
                    {
                        List<WorkTicketModel> dataResult = page.OrderByDescending(x => x.ApprovedDateStyleDatetime).ToList();
                        return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                    }
                    else
                    {
                        List<WorkTicketModel> dataResult = page.OrderBy(x => x.ApprovedDateStyleDatetime).ToList();
                        return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                    }
                }

                // Sắp xếp theo Trạng thái
                if (requestModel.SortColumn.Equals("ACTIVE"))
                {
                    if ("DESC".Equals(requestModel.SortOrder))
                    {
                        List<WorkTicketModel> dataResult = page.OrderByDescending(x => x.WorkTicketStatus.Value).ToList();
                        return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                    }
                    else
                    {
                        List<WorkTicketModel> dataResult = page.OrderBy(x => x.WorkTicketStatus.Value).ToList();
                        return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                    }
                }

                // Sắp xếp theo Số phiếu công tác
                if (requestModel.SortColumn.Equals("TICKETNO"))
                {
                    if ("DESC".Equals(requestModel.SortOrder))
                    {
                        List<WorkTicketModel> dataResult = page.OrderByDescending(x => x.ApprovedDateStyleDatetime).ThenByDescending(x => x.WorkTicketNumber).ToList();
                        return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                    }
                    else
                    {
                        List<WorkTicketModel> dataResult = page.OrderBy(x => x.ApprovedDateStyleDatetime).ThenBy(x => x.WorkTicketNumber).ToList();
                        return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                    }
                }
            }

            return HttpResponse.OkList(page);
        }

        public DataCountModel GetWorkTicketsCountMobile(SearchWorkTicketMobileModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.RegisteredFromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.RegisteredToDate, true);
            // Mặc định tìm kiếm theo PCTĐ
            if (!requestModel.ScreenType.HasValue) requestModel.ScreenType = 1;

            var workTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.WaitForRegist && x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.Waiting && !x.IsDeleted);

            if (requestModel.WorkTicketFeature > 0)
            {
                workTickets = workTickets.Where(x => x.WorkTicketFeatureLinks.Any(y => y.FeatureID == requestModel.WorkTicketFeature));
            }
            if (requestModel.ScreenType.HasValue && requestModel.ScreenType > 0)
                workTickets = workTickets.Where(x => x.ScreenType == requestModel.ScreenType);

            if (requestModel.IsWaitingTicket)
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus == (int)Core.Enums.EnumTicketStatus.Approved);
            }
            else
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.Approved);
            }
            //Ngày đăng ký (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.RegisteredFromDate))
            {
                workTickets = workTickets.Where(x => DbFunctions.TruncateTime(x.ApprovedDate) >= frDate.Date);
            }
            //Ngày đăng ký (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.RegisteredToDate))
            {
                workTickets = workTickets.Where(x => DbFunctions.TruncateTime(x.ApprovedDate) <= toDate.Date);
            }

            if (requestModel.WorkTicketType > 0)
            {
                workTickets = workTickets.Where(x => x.WorkTicketType == requestModel.WorkTicketType);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.SearchKey))
            {
                workTickets = workTickets.Where(x => x.WorkTicketNumber.Contains(requestModel.SearchKey)
                                                || x.ApprovedUnit.Name.Contains(requestModel.SearchKey)
                                                || x.LeadershipUsers.Any(t => t.FullName.Contains(requestModel.SearchKey)));
            }

            var count1 = workTickets.Where(x => x.WorkTicketStatus == (int)EnumTicketStatus.Allowed).Count();

            var count2 = workTickets.Where(x => x.WorkTicketStatus == (int)EnumTicketStatus.InProgress).Count();

            var count3 = workTickets.Where(x => x.WorkTicketStatus == (int)EnumTicketStatus.Completed
                            || x.WorkTicketStatus == (int)EnumTicketStatus.WaitForConfirmation).Count();

            return new DataCountModel()
            {
                Count1 = count1,
                Count2 = count2,
                Count3 = count3
            };
        }

        public ResponseBaseModel GetWorkTicketsMobile(SearchWorkTicketMobileModel requestModel)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.Common.ToString(), Environment.NewLine + "Start GetWorkTicketsMobile");
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.Common.ToString(), Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel));

            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.RegisteredFromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.RegisteredToDate, true);
            // Mặc định tìm kiếm theo PCTĐ
            if (!requestModel.ScreenType.HasValue) requestModel.ScreenType = 1;

            var workTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.WaitForRegist && x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.Waiting && !x.IsDeleted);

            if (requestModel.WorkTicketFeature > 0)
            {
                workTickets = workTickets.Where(x => x.WorkTicketFeatureLinks.Any(y => y.FeatureID == requestModel.WorkTicketFeature));
            }
            if (requestModel.ScreenType.HasValue && requestModel.ScreenType > 0)
                workTickets = workTickets.Where(x => x.ScreenType == requestModel.ScreenType);

            if (requestModel.IsWaitingTicket)
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus == (int)Core.Enums.EnumTicketStatus.Approved);
            }
            else
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus != (int)Core.Enums.EnumTicketStatus.Approved);
            }
            //Ngày đăng ký (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.RegisteredFromDate))
            {
                workTickets = workTickets.Where(x => DbFunctions.TruncateTime(x.ApprovedDate) >= frDate.Date);
            }
            //Ngày đăng ký (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.RegisteredToDate))
            {
                workTickets = workTickets.Where(x => DbFunctions.TruncateTime(x.ApprovedDate) <= toDate.Date);
            }

            if (requestModel.WorkTicketType > 0)
            {
                workTickets = workTickets.Where(x => x.WorkTicketType == requestModel.WorkTicketType);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.SearchKey))
            {
                workTickets = workTickets.Where(x => x.WorkTicketNumber.Contains(requestModel.SearchKey)
                                                || x.ApprovedUnit.Name.Contains(requestModel.SearchKey)
                                                || x.LeadershipUsers.Any(t => t.FullName.Contains(requestModel.SearchKey)));
            }

            if (requestModel.Tab == 1)
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus == (int)EnumTicketStatus.Allowed);
            }
            else if (requestModel.Tab == 2)
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus == (int)EnumTicketStatus.InProgress);
            }
            else if (requestModel.Tab == 3)
            {
                workTickets = workTickets.Where(x => x.WorkTicketStatus == (int)EnumTicketStatus.Completed
                            || x.WorkTicketStatus == (int)EnumTicketStatus.WaitForConfirmation);
            }
            if (!requestModel.SortDescending)
            {
                var pageAscending = workTickets.Select(e => new WorkTicketModel()
                {
                    ID = e.ID,
                    WorkTicketNumber = e.WorkTicketNumber,
                    WorkTicketFeature = e.WorkTicketFeature != null ? new ValueLabelModel
                    {
                        Value = e.WorkTicketFeature.ID,
                        Label = e.WorkTicketFeature.FeatureName
                    } : new ValueLabelModel { Value = 0, Label = null },
                    WorkTicketTypeValue = e.WorkTicketType,
                    WorkTicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                    ApprovedDateLocal = e.ApprovedDate,
                    ApprovedDateStyleDatetime = e.ApprovedDate,
                    Approver = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Approver && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                    Acceptor = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                }).OrderBy(t => t.WorkTicketStatus.Label)
                        .ThenBy(t => t.ApprovedDateStyleDatetime)
                        .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                // Kiểm tra quyền edit
                if (pageAscending != null && pageAscending.TotalRecords > 0)
                {
                    foreach (var entity in pageAscending)
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

                    pageAscending.GetEnumerator().Reset();
                }

                return HttpResponse.OkList(pageAscending);
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
                WorkTicketTypeValue = e.WorkTicketType,
                WorkTicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                ApprovedDateLocal = e.ApprovedDate,
                ApprovedDateStyleDatetime = e.ApprovedDate,
                WorkContent = e.WorkContent,
                Approver = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Approver && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
                Acceptor = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Acceptor && !x.IsDeleted)
                                            .Select(z => new ValueLabelModel { Value = z.ID, Label = z.FullName }).FirstOrDefault(),
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

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.Common.ToString(), Environment.NewLine + "End GetWorkTicketsMobile");
            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// kiểm tra phiếu đăng ký tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckExistRegistrationTicketData(int id)
        {
            return _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ID == id && !x.IsDeleted).Any();
        }

        /// <summary>
        /// Tạo phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isApprove"></param>
        /// <returns></returns>
        public ResponseModel<int> AddWorkTicketNew(AddNewWorkTicketModel requestModel, bool isApprove)
        {
            var responseModel = new ResponseModel<int>();
            var _errorMessages = new List<string>();

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                if (CheckDuplicateData(requestModel.WorkTicketNumber))
                {
                    _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_EXIST);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.Data = -1;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

                    return responseModel;
                }

                try
                {
                    requestModel.LeadershipUsers = new List<AddNewLeadershipUserFetchedModel>();

                    // lấy thông tin người cấp phiếu
                    var approveUser = GetUserInfo(requestModel.CurrentUserID, false, (int)LeadershipUserRoles.Approver);
                    if (approveUser != null)
                        requestModel.LeadershipUsers.Add(approveUser);

                    // lấy thông tin người cho phép
                    if (requestModel.Acceptor != null && requestModel.Acceptor.UserName != null)
                    {
                        var acceptorUser = GetUserInfo(requestModel.Acceptor?.UserName, requestModel.Acceptor.IsExternal, (int)LeadershipUserRoles.Acceptor);
                        if (acceptorUser != null)
                            requestModel.LeadershipUsers.Add(acceptorUser);
                    }

                    // lấy thông tin người chỉ huy
                    if (requestModel.Commander != null && requestModel.Commander.UserName != null)
                    {
                        var commander = GetUserInfo(requestModel.Commander?.UserName, requestModel.Commander.IsExternal, (int)LeadershipUserRoles.Commander);
                        if (commander != null)
                            requestModel.LeadershipUsers.Add(commander);
                    }

                    // lấy thông tin người lãnh đạo
                    if (requestModel.Leader != null && requestModel.Leader.UserName != null)
                    {
                        var leaderUser = GetUserInfo(requestModel.Leader.UserName, requestModel.Leader.IsExternal, (int)LeadershipUserRoles.Leader);
                        if (leaderUser != null)
                            requestModel.LeadershipUsers.Add(leaderUser);
                    }

                    if (requestModel.RegistrationTicketID.GetValueOrDefault(0) > 0)
                    {
                        RegistrationDetailModel registrationDetailModel = GetRegistrationDetail(requestModel.RegistrationTicketID.GetValueOrDefault(0));
                        if (registrationDetailModel != null)
                        {
                            requestModel.FactoryID = registrationDetailModel.FactoryID;
                            requestModel.RequestTicketID = registrationDetailModel.RequestTicketID;
                        }
                    }
                    else
                    {
                        requestModel.FactoryID = GetFactory(requestModel.CurrentUserID, 0);
                    }

                    if (requestModel.FactoryID < 1)
                    {
                        _errorMessages.Add("Người dùng không tồn tại trong nhà máy");
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.Data = -1;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }

                    if (requestModel.ApprovedUnitID < 1)
                    {
                        _errorMessages.Add("Đơn vị cấp phiếu không tồn tại");
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.Data = -1;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }

                    // thêm mới phiếu công tác
                    if (requestModel.ApprovedDate == DateTime.MinValue)
                    {
                        requestModel.ApprovedDate = DateTime.Now;
                    }

                    var entity = CreateEntity(requestModel);

                    _unitOfWork.ElogbookRepository<WorkTicket>().Add(entity);

                    // Add Kiểm tra các biện pháp an toàn trước khi cho phép công tác
                    if (requestModel.AllowedInfoDetails != null && requestModel.AllowedInfoDetails.Count > 0)
                    {
                        var allowedEntity = new AllowedInfo()
                        {
                            WorkTicketID = entity.ID,
                            AllowedTime = DateTime.Now,
                            TimeCommanderSigned = DateTime.Now,
                            AllowedInfoDetails = requestModel.AllowedInfoDetails?.Select(e => new AllowedInfoDetail()
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
                    if (requestModel.SafetyMeasures != null && requestModel.SafetyMeasures.Count > 0)
                    {
                        var receiveWorkplace = new ReceiveWorkplace()
                        {
                            WorkTicketID = entity.ID,
                            StartWorkingDate = DateTime.Now,
                            SafetyMeasures = requestModel.SafetyMeasures?.Select(e => new SafetyMeasure()
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

                    var saveSuccess = _unitOfWork.Complete() > 0;
                    if (saveSuccess)
                    {
                        // Cập nhật số phiếu đã đăng ký vào ExclusiveData
                        string[] arrNumTicket = requestModel.WorkTicketNumber.Split('/');
                        Int32.TryParse(arrNumTicket[0], out int numTicket);
                        string ticketType = "QLPCTD";
                        if (entity.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                        {
                            ticketType = "QLPTCNH";
                        }

                        var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                            .GetQuery(x => x.InsertedAt.Year == DateTime.Now.Year && x.TicketType.Equals(ticketType) && x.TicketNumber == numTicket).FirstOrDefault();
                        if (lstNumber == null)
                        {
                            var data = new ExclusiveData()
                            {
                                TicketNumber = numTicket,
                                TicketType = ticketType
                            };
                            _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
                        }

                        var workTicketDetail = GetWorkTicketById(entity.ID);
                        if (workTicketDetail != null)
                        {
                            // tạo thư mục lưu file pdf
                            if (!Directory.Exists(Path.Combine(_rootPath, "WorkTicket", "PDF")))
                            {
                                Directory.CreateDirectory(Path.Combine(_rootPath, "WorkTicket", "PDF"));
                            }
                            var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                            var fileName = $"WorkTicket_{currentDate}.pdf";
                            var fileNameSigned = $"WorkTicket_{currentDate}_signed.pdf";
                            var outputFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileName);
                            var signedFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileNameSigned);

                            // Báo lỗi nếu không tạo được PDF
                            PdfInfoModel pdfInfoModel = null;
                            PdfWorkTicketModel pdfWorkTicketModel = null;
                            Pdf881ContentModel pdf881ContentModel = null;
                            if (requestModel.ScreenType == null || (int)requestModel.ScreenType == 1)
                            {
                                // tạo file PDF(Phiếu điện) rùi ký vào file đó
                                pdfWorkTicketModel = CreatePdfData(workTicketDetail);
                                pdfInfoModel = GeneratePdfFile(pdfWorkTicketModel, outputFile, false);
                            }
                            else if (requestModel.ScreenType == 2)
                            {
                                // tạo file PDF(Phiếu TCNH) rùi ký vào file đó
                                pdf881ContentModel = CreatePdf881Data(workTicketDetail);
                                pdfInfoModel = GeneratePdf881File(pdf881ContentModel, outputFile, false);
                            }

                            if (pdfInfoModel != null && pdfInfoModel.Status)
                            {
                                if (approveUser != null)
                                {
                                    UserInfo userApproved = _commonService.GetInternalUserById(approveUser.UserName);
                                    if (userApproved != null)
                                    {
                                        // Ký số PDF — hỗ trợ 4 phương thức: Tài khoản, Viettel CA, VNPT SmartCA, Viettel MySign
                                        ResponseModel<bool> signResult = SignPdfForTicket(userApproved, requestModel.WorkTicketNumber, outputFile, "Sig_Approver");
                                        if (!signResult.Data)
                                        {
                                            transaction.Rollback();
                                            responseModel.ErrorMessages = signResult.ErrorMessages;
                                            responseModel.Data = -1;
                                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                                            return responseModel;
                                        }

                                        // Lưu Attachment trỏ về outputFile (đã được PdfSigningService ghi đè chữ ký)
                                        var attachmentPdfCreated = new Attachment
                                        {
                                            FilePath = Path.Combine("WorkTicket", "PDF", fileName),
                                            FileName = fileName,
                                            ScreenName = "WorkTicket",
                                            FileType = "pdf",
                                        };
                                        _unitOfWork.ElogbookRepository<Attachment>().Add(attachmentPdfCreated);

                                        // Thực hiện lưu cấp PCT
                                        {
                                            saveSuccess = _unitOfWork.Complete() > 0;
                                            if (saveSuccess)
                                            {
                                                entity.TicketPdfID = attachmentPdfCreated.ID;
                                                _unitOfWork.Complete();
                                                transaction.Commit();

                                                // chỉ gửi mail cấp phiếu công tác cho trường hợp cấp mới (không thông qua phiếu đăng ký)
                                                if (entity.RegistrationTicketID == null || entity.RegistrationTicketID < 1)
                                                {
                                                    //[Notification-5 && 12]
                                                    var processEmailRequestModel = new ProcessEmailRequestModel
                                                    {
                                                        RegistrationTicketID = entity.RegistrationTicketID ?? 0,
                                                        TicketStatus = entity.WorkTicketStatus,
                                                        RegistrationTicketType = entity.ScreenType ?? 1,
                                                        WorkTicketID = entity.ID,
                                                        IsPCT = true,
                                                        CurrentUserID = requestModel.CurrentUserID,
                                                        ModuleNotification = entity.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                                                    };
                                                    _emailService.ProcessEmail(processEmailRequestModel);
                                                }

                                                if (isApprove)
                                                {
                                                    string desciption = "Đồng ý cấp phiếu công tác. Số phiếu: " + requestModel.WorkTicketNumber;
                                                    _registrationTicketService.UpdateStatusRegistrationTicket(requestModel.RegistrationTicketID.GetValueOrDefault(0),
                                                    (int)EnumTicketStatus.Approved,
                                                    desciption, requestModel.CurrentUserID,
                                                    attachmentPdfCreated.ID.ToString());
                                                    responseModel.Data = entity.ID;

                                                    return responseModel;
                                                }
                                                else
                                                {
                                                    responseModel.Data = entity.ID;
                                                    return responseModel;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            _errorMessages.Add("Cấp phiếu công tác thất bại");
            responseModel.ErrorMessages = _errorMessages;
            responseModel.Data = -1;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

            return responseModel;
        }

        /// <summary>
        /// Dispatch ký số PDF phiếu công tác theo SigningMethod của người ký.
        /// Hỗ trợ đủ 4 phương thức: Tài khoản người dùng, Viettel CA, VNPT SmartCA, Viettel MySign.
        /// PdfSigningService ghi đè trực tiếp lên <paramref name="pdfPath"/> sau khi ký.
        /// </summary>
        /// <param name="userApproved">Thông tin người ký (SigningMethod, PhoneNumber, NationalID, Signature)</param>
        /// <param name="ticketNumber">Số phiếu — hiển thị trên điện thoại khi ký</param>
        /// <param name="pdfPath">Đường dẫn file PDF (sẽ bị ghi đè bằng bản đã ký)</param>
        /// <param name="signatureFieldName">Tên AcroField chữ ký trong PDF (dạng "Sig_xxx")</param>
        /// <returns>ResponseModel với Data=true nếu ký thành công</returns>
        private ResponseModel<bool> SignPdfForTicket(
            UserInfo userApproved, string ticketNumber, string pdfPath, string signatureFieldName)
        {
            string dataDisplay = ("Ky Cap Phieu Cong Tac " + ticketNumber).Replace("Đ", "D");
            string signingMethod = userApproved?.SigningMethod?.Label ?? string.Empty;

            LogCustom.LogMessage(TrackingLogLevel.INFO, "WorkTicketService",
                $"[SignPdfForTicket] ticket={ticketNumber}, method={signingMethod}, field={signatureFieldName}");

            if (signingMethod.Equals(CommonConstant.SIGNING_USERNAME_PASSWORD, StringComparison.OrdinalIgnoreCase))
            {
                return _pdfSigningService.SignDocumentAndFillName(
                    userApproved, pdfPath, signatureFieldName, userApproved.FullName);
            }
            else if (signingMethod.Equals(CommonConstant.SIGNING_VIETTEL_CA, StringComparison.OrdinalIgnoreCase))
            {
                return _pdfSigningService.SignWithViettelMobileCA(
                    userApproved, dataDisplay, pdfPath, signatureFieldName, userApproved.FullName);
            }
            else if (signingMethod.Equals(CommonConstant.SIGNING_VNPT_SMARTCA, StringComparison.OrdinalIgnoreCase))
            {
                return _pdfSigningService.SignWithVnpt(
                    userApproved, dataDisplay, pdfPath, signatureFieldName, userApproved.FullName);
            }
            else if (signingMethod.Equals(CommonConstant.SIGNING_VIETTEL_MYSIGN, StringComparison.OrdinalIgnoreCase))
            {
                return _pdfSigningService.SignWithViettelMySign(
                    userApproved, dataDisplay, pdfPath, signatureFieldName, userApproved.FullName);
            }

            // Phương thức ký không xác định
            return new ResponseModel<bool>
            {
                Data = false,
                StatusCode = (int)HttpStatusCode.BadRequest,
                ErrorMessages = new List<string> { $"Phương thức ký không được hỗ trợ: {signingMethod}" }
            };
        }

        public static string LoadBase64(string base64)
        {
            var rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            //***Save Base64 Encoded string as Image File***//
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = $"signature_{currentDate}.png";
            //Convert Base64 Encoded string to Byte Array.
            byte[] imageBytes = Convert.FromBase64String(base64);

            //Save the Byte Array as Image File.
            string outputFile = Path.Combine(rootPath, "Image", "signature", fileName);
            if (!Directory.Exists(Path.Combine(rootPath, "Image", "signature")))
            {
                Directory.CreateDirectory(Path.Combine(rootPath, "Image", "signature"));
            }
            File.WriteAllBytes(outputFile, imageBytes);

            return outputFile;
        }

        /// <summary>
        /// Thêm thông tin PCT cho phép công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> AddAllowedInfo(AddAllowedInfoWithoutAcceptorModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            var data = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
            if (data != null)
            {
                var allowedEntity = new AllowedInfo()
                {
                    WorkTicketID = requestModel.WorkTicketID,
                    TimeCommanderSigned = DateTime.Now,
                    AllowedTime = !string.IsNullOrEmpty(requestModel.AllowedTime) ? DateTimeConvert.ConvertDateAndTimeFromString(requestModel.AllowedTime) : DateTime.Now,
                    AllowedInfoDetails = requestModel.AllowedInfos?.Select(e => new AllowedInfoDetail()
                    {
                        Content = !string.IsNullOrWhiteSpace(e.Content) ? e.Content : null,
                        Position = e.Position != null ? e.Position : null,
                        IsChecked = e.IsChecked != null ? e.IsChecked : false,
                        Note = !string.IsNullOrWhiteSpace(e.Note) ? e.Note : null,
                        Attachments = e.Attachments != null ? string.Join(",", e.Attachments.ToArray()) : null,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    }).ToList(),

                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                };

                // Nếu là phiếu thủy cơ nhiệt thì bổ sung chữ ký người chỉ huy trực tiếp
                if (data.ScreenType == 2)
                {
                    if (requestModel.Commander != null)
                    {
                        var signatureModel = new SignatureModel()
                        {
                            CurrentUserID = requestModel.CurrentUserID
                        };
                        if (requestModel.Commander.Signature != null)
                        {
                            signatureModel.Type = requestModel.Commander.Signature.Type;
                            signatureModel.SignatureContent = requestModel.Commander.Signature.SignatureContent;
                        }

                        var signatureInfo = _commonService.CreateSignature(signatureModel);
                        allowedEntity.CommanderSignature = signatureInfo.ID;
                    }

                    // lấy thông tin người cho phép
                    if (requestModel.Allower != null && requestModel.Allower.UserName != null && requestModel.Allower.IsExternal != null)
                    {
                        var acceptorUser = GetUserInfo(requestModel.Allower?.UserName, (bool)requestModel.Allower.IsExternal, (int)LeadershipUserRoles.Acceptor);
                        if (acceptorUser != null)
                        {
                            LeadershipUser leadershipUser = new LeadershipUser()
                            {
                                WorkTicketID = requestModel.WorkTicketID,
                                UserName = !string.IsNullOrWhiteSpace(acceptorUser.UserName) ? acceptorUser.UserName : null,
                                FullName = !string.IsNullOrWhiteSpace(acceptorUser.FullName) ? acceptorUser.FullName : null,
                                PositionName = !string.IsNullOrWhiteSpace(acceptorUser.PositionName) ? acceptorUser.PositionName : null,
                                IsExternal = acceptorUser.IsExternal,
                                Roles = acceptorUser.Roles > 0 ? acceptorUser.Roles : null,
                                ElectricRank = acceptorUser.ElectricRank,
                                ElectricRankCard = !string.IsNullOrWhiteSpace(acceptorUser.ElectricRankCard) ? acceptorUser.ElectricRankCard : null,
                                LaborCard = acceptorUser.LaborCard,
                                LaborRank = acceptorUser.LaborRank,
                                Signature = !string.IsNullOrWhiteSpace(acceptorUser.Signature) ? acceptorUser.Signature : null,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };
                            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                        }
                    }

                    // thêm người cho chỉ huy vào table LeadershipUser
                    if (requestModel.Commander != null && requestModel.Commander.UserName != null && requestModel.Commander.IsExternal != null)
                    {
                        var commanderUser = GetUserInfo(requestModel.Commander?.UserName, (bool)requestModel.Commander.IsExternal, (int)LeadershipUserRoles.Commander);
                        if (commanderUser != null)
                        {
                            string commanderSignature = null;

                            if (!string.IsNullOrWhiteSpace(commanderUser.Signature))
                            {
                                commanderSignature = commanderUser.Signature;
                            }
                            else
                            {
                                if (requestModel.Commander?.Signature != null)
                                {
                                    commanderSignature = requestModel.Commander.Signature.SignatureContent;
                                }
                            }

                            LeadershipUser leadershipUser = new LeadershipUser()
                            {
                                WorkTicketID = requestModel.WorkTicketID,
                                UserName = !string.IsNullOrWhiteSpace(commanderUser.UserName) ? commanderUser.UserName : null,
                                FullName = !string.IsNullOrWhiteSpace(commanderUser.FullName) ? commanderUser.FullName : null,
                                PositionName = !string.IsNullOrWhiteSpace(commanderUser.PositionName) ? commanderUser.PositionName : null,
                                IsExternal = commanderUser.IsExternal,
                                Roles = commanderUser.Roles > 0 ? commanderUser.Roles : null,
                                ElectricRank = commanderUser.ElectricRank,
                                ElectricRankCard = !string.IsNullOrWhiteSpace(commanderUser.ElectricRankCard) ? commanderUser.ElectricRankCard : null,
                                LaborCard = commanderUser.LaborCard,
                                LaborRank = commanderUser.LaborRank,
                                Signature = commanderSignature,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };
                            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                        }
                    }
                }

                _unitOfWork.ElogbookRepository<AllowedInfo>().Add(allowedEntity);
                data.WorkTicketStatus = (int)EnumTicketStatus.Allowed;
                data.LastUpdatedUserID = requestModel.CurrentUserID;
                _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);

                // Trường hợp thuộc phiếu yêu cầu(AmRequestTicket) thì cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                if (data.RequestTicketID.HasValue)
                {
                    UpdateAmDeviceAbnormalStatus((int)data.RequestTicketID, (int)AmEnumDeAbStatus.Yes_InProgress, requestModel.CurrentUserID);
                }

                responseModel.Data = _unitOfWork.Complete() > 0;

                //[Notification-6 && 13]
                var processEmailRequestModel = new ProcessEmailRequestModel
                {
                    RegistrationTicketID = data.RegistrationTicketID ?? 0,
                    TicketStatus = data.WorkTicketStatus,
                    RegistrationTicketType = data.ScreenType ?? 1,
                    WorkTicketID = data.ID,
                    IsPCT = true,
                    CurrentUserID = requestModel.CurrentUserID,
                    ModuleNotification = data.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                };
                _emailService.ProcessEmail(processEmailRequestModel);

                if (data.ScreenType == 2)
                {
                    //2 - Phần B: Khi  ký Cho phép PCT=> lưu nội dung:
                    //Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE} (Lưu sổ VHV).
                    _commonService.AsyncStatusAndContent((int)EventType.PCT_TCNH_ALLOWED, requestModel.CurrentUserID, requestModel.WorkTicketID, allowedEntity.AllowedTime, null, null);
                }
                else
                {
                    //2.Khi ký Cho phép PCT: Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE}.
                    _commonService.AsyncStatusAndContent((int)EventType.PCTD_ALLOWED, requestModel.CurrentUserID, requestModel.WorkTicketID, allowedEntity.AllowedTime, null, null);
                }

                return responseModel;
            }
            else
            {
                responseModel.ErrorMessages.Add("Không tìm thấy dữ liệu để thêm");
                responseModel.Data = false;

                return responseModel;
            }
        }

        /// <summary>
        /// kiểm tra user + pass
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        public bool IsExistUser(string userName, string passWord)
        {
            // verify user pass
            var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.UserName.Equals(userName.Trim(), StringComparison.OrdinalIgnoreCase)
                                                                                && !x.IsDeleted
                                                                                && x.Status == (int)AccessStatus.Active).FirstOrDefault();
            if (userInfo != null)
            {
                return RuntimeManager.ValidatePassword(passWord, userInfo.Password);
            }

            return false;
        }

        /// <summary>
        /// CheckDuplicateData
        /// </summary>
        /// <param name="numberWorkTicket"></param>
        /// <returns></returns>
        public bool CheckDuplicateData(string numberWorkTicket)
        {
            return _unitOfWork.ElogbookRepository<WorkTicket>()
                            .GetQuery(x => x.WorkTicketNumber == numberWorkTicket && !x.IsDeleted && x.InsertedAt.Year == DateTime.Now.Year).Any();
        }

        /// <summary>
        /// Thêm mới phiếu công tác bằng cách nhập liệu dữ liệu cũ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddWorkTicketOld(AddOldWorkTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            var _errorMessages = new List<string>();
            int factoryID = GetFactory(0, requestModel.ApprovedUnitID);

            // thêm mới phiếu công tác
            var workTicketEntity = new WorkTicket
            {
                ScreenType = requestModel.ScreenType != null ? requestModel.ScreenType : 1,
                WorkTicketNumber = requestModel.WorkTicketNumber,
                WorkTicketType = requestModel.WorkTicketType,
                WorkTicketFeatureID = requestModel.WorkTicketFeatureID != null ? requestModel.WorkTicketFeatureID : null,
                WorkTicketStatus = (int)EnumTicketStatus.Completed,
                UnitNumber = requestModel.UnitNumber != null ? requestModel.UnitNumber : null,
                WorkPlacement = !string.IsNullOrEmpty(requestModel.WorkPlacement) ? requestModel.WorkPlacement : null,
                WorkContent = !string.IsNullOrEmpty(requestModel.WorkContent) ? requestModel.WorkContent : null,
                WorkScope = !string.IsNullOrEmpty(requestModel.WorkScope) ? requestModel.WorkScope : null,
                WorkResult = !string.IsNullOrEmpty(requestModel.WorkResult) ? requestModel.WorkResult : null,
                StartDate = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.StartDate),
                EndDate = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.EndDate),
                WorkCondition = !string.IsNullOrEmpty(requestModel.WorkCondition) ? requestModel.WorkCondition : null,
                CreatedUserID = requestModel.CurrentUserID,
                InsertedAt = DateTime.Now,
                LastUpdatedUserID = requestModel.CurrentUserID,
                UpdatedAt = DateTime.Now,
                Attachments = (requestModel.AttachmentsID != null) ? string.Join(",", requestModel.AttachmentsID.ToArray()) : null,
                WorkPlacements = requestModel.WorkPlacements?.Select(e => new WorkPlacement()
                {
                    Place = !string.IsNullOrWhiteSpace(e.Place) ? e.Place : null,
                    StartTime = !string.IsNullOrEmpty(e.StartTime) ? DateTimeConvert.ConvertDateAndTimeFromString(e.StartTime) : (DateTime?)null,
                    EndTime = !string.IsNullOrEmpty(e.EndTime) ? DateTimeConvert.ConvertDateAndTimeFromString(e.EndTime) : (DateTime?)null,
                    CommanderName = e.CommanderName,
                    AcceptorName = e.AllowerName,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                }).ToList(),
                WorkerInformations = requestModel.WorkerInformations?.Select(e => new WorkerInformation()
                {
                    FullName = !string.IsNullOrWhiteSpace(e.FullName) ? e.FullName : string.Empty,
                    CheckInTime = DateTimeConvert.ConvertDateAndTimeFromString(e.CheckInTime),
                    CheckOutTime = DateTimeConvert.ConvertDateAndTimeFromString(e.CheckOutTime),
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                }).ToList(),
                WarnAndSafes = requestModel.WarnAndSafes?.Select(e => new WarnAndSafe()
                {
                    WarningContent = !string.IsNullOrWhiteSpace(e.WarningContent) ? e.WarningContent : null,
                    SafetyMeasureContent = !string.IsNullOrWhiteSpace(e.SafetyMeasureContent) ? e.SafetyMeasureContent : null,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                }).ToList(),
                WorkDailys = requestModel.WorkDailys?.Select(e => new WorkDaily()
                {
                    WorkDay = DateTimeConvert.ConvertDateTimeFromString(e.WorkDay, false),
                    WorkLocationID = e.WorkLocationID,
                    WorkLocationText = !string.IsNullOrEmpty(e.WorkLocationText) ? e.WorkLocationText : null,
                    StaffInformations = e.StaffInformations?.Select(t => new StaffInformation()
                    {
                        UserName = !string.IsNullOrEmpty(t.UserName) ? t.UserName : string.Empty,
                        IsExternal = t.IsExternal != null ? t.IsExternal : null,
                        FullName = !string.IsNullOrEmpty(t.FullName) ? t.FullName : string.Empty,
                        PositionName = !string.IsNullOrEmpty(t.PositionName) ? t.PositionName : null,
                        Mission = !string.IsNullOrEmpty(t.Mission) ? t.Mission : null,
                        ElectricRankCard = !string.IsNullOrEmpty(t.ElectricRankCard) ? t.ElectricRankCard : null,
                        CheckInTime = !string.IsNullOrEmpty(t.CheckInTime) ? t.CheckInTime : null,
                        CheckOutTime = !string.IsNullOrEmpty(t.CheckOutTime) ? t.CheckOutTime : null,
                        CreatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                    }).ToList(),
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                }).ToList(),
                WorkTicketFeatureLinks = requestModel.WorkTicketFeatures?.Select(id => new WorkTicketFeatureLink()
                {
                    FeatureID = id,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                }).ToList(),
                WorkPictures = requestModel.WorkPictures != null ? string.Join(",", requestModel.WorkPictures.ToArray()) : null,
            };

            if (factoryID > 0)
            {
                workTicketEntity.FactoryID = factoryID;
            }

            if (DateTimeConvert.ConvertDateTimeFromString(requestModel.ApprovedDate, true) == DateTime.MaxValue)
            {
                workTicketEntity.ApprovedDate = null;
            }
            else
            {
                workTicketEntity.ApprovedDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ApprovedDate, true);
            }

            if (requestModel.ApprovedUnitID != null && requestModel.ApprovedUnitID > 0)
            {
                workTicketEntity.ApprovedUnitID = requestModel.ApprovedUnitID;
            }

            _unitOfWork.ElogbookRepository<WorkTicket>().Add(workTicketEntity);

            // người lãnh đạo
            if (requestModel.Commander != null)
            {
                // người chỉ huy
                var commanderEntity = new LeadershipUser()
                {
                    WorkTicketID = workTicketEntity.ID,
                    Roles = (int)LeadershipUserRoles.Commander,
                    FullName = requestModel.Commander,
                    PositionName = requestModel.ReceiveWorkplace?.CommanderPositionName ?? null,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                };
                _unitOfWork.ElogbookRepository<LeadershipUser>().Add(commanderEntity);
            }

            // người lãnh đạo
            if (requestModel.Leader != null)
            {
                var leaderEntity = new LeadershipUser()
                {
                    WorkTicketID = workTicketEntity.ID,
                    Roles = (int)LeadershipUserRoles.Leader,
                    FullName = requestModel.Leader,
                    PositionName = requestModel.ReceiveWorkplace?.LeaderPositionName ?? null,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                };
                _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leaderEntity);
            }

            // người cấp phiếu
            var approverEntity = LeadershipUserEntity(requestModel.Approver, requestModel.CurrentUserID, (int)LeadershipUserRoles.Approver);
            approverEntity.WorkTicketID = workTicketEntity.ID;
            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(approverEntity);

            // thông tin cho phép
            var allowedEntity = new AllowedInfo()
            {
                WorkTicketID = workTicketEntity.ID,
                AllowedTime = !string.IsNullOrEmpty(requestModel.AllowedInfo?.AllowedTime) ? DateTimeConvert.ConvertDateAndTimeFromString(requestModel.AllowedInfo?.AllowedTime) : DateTime.Now,
                TimeCommanderSigned = !string.IsNullOrEmpty(requestModel.AllowedInfo?.TimeCommanderSigned) ? DateTimeConvert.ConvertDateAndTimeFromString(requestModel.AllowedInfo?.TimeCommanderSigned) : DateTime.Now,
                AllowedInfoDetails = requestModel.AllowedInfo.AllowedInfos?.Select(e => new AllowedInfoDetail()
                {
                    Content = !string.IsNullOrWhiteSpace(e.Content) ? e.Content : null,
                    Position = e.Position != null ? e.Position : null,
                    Note = !string.IsNullOrWhiteSpace(e.Note) ? e.Note : null,
                    IsChecked = e.IsChecked != null ? e.IsChecked : false,
                    Attachments = (e.Attachments != null) ? string.Join(",", e.Attachments.ToArray()) : null,
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

            // thêm người cho phép
            var acceptorEntity = LeadershipUserEntity(requestModel.AllowedInfo.Acceptor, requestModel.CurrentUserID, (int)LeadershipUserRoles.Acceptor);
            acceptorEntity.WorkTicketID = workTicketEntity.ID;
            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(acceptorEntity);

            // Thông tin tiếp nhận nơi làm việc
            if (requestModel.ReceiveWorkplace != null)
            {
                var receiveReq = requestModel.ReceiveWorkplace;
                var receiveWorkplace = new ReceiveWorkplace()
                {
                    WorkTicketID = workTicketEntity.ID,
                    WorkUnitName = !string.IsNullOrEmpty(receiveReq.WorkUnitName) ? receiveReq.WorkUnitName : null,
                    WorkUnitID = receiveReq.WorkUnitID != null ? receiveReq.WorkUnitID : null,
                    AmountPersons = !string.IsNullOrEmpty(receiveReq.AmountPersons) ? receiveReq.AmountPersons : null,
                    StartWorkingDate = !string.IsNullOrEmpty(receiveReq.StartWorkingDate) ? DateTimeConvert.ConvertDateAndTimeFromString(receiveReq.StartWorkingDate) : DateTime.Now,
                    SafetyMeasures = receiveReq.SafetyMeasures?.Select(e => new SafetyMeasure()
                    {
                        Content = !string.IsNullOrWhiteSpace(e.Content) ? e.Content : null,
                        Position = e.Position != null ? e.Position : null,
                        Note = !string.IsNullOrWhiteSpace(e.Note) ? e.Note : null,
                        IsChecked = e.IsChecked != null ? e.IsChecked : false,
                        Attachments = (e.Attachments != null && e.Attachments.Count > 0) ? string.Join(",", e.Attachments.ToArray()) : null,
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
                // thêm người giám sát
                if (requestModel.ReceiveWorkplace.Supervisor != null)
                {
                    var supervisorEntity = new LeadershipUser()
                    {
                        WorkTicketID = workTicketEntity.ID,
                        Roles = (int)LeadershipUserRoles.Supervisor,
                        FullName = !string.IsNullOrWhiteSpace(requestModel.ReceiveWorkplace.Supervisor) ? requestModel.ReceiveWorkplace.Supervisor : null,
                        PositionName = !string.IsNullOrWhiteSpace(requestModel.ReceiveWorkplace.SupervisorPositionName) ? requestModel.ReceiveWorkplace.SupervisorPositionName : null,
                        CreatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                    };
                    _unitOfWork.ElogbookRepository<LeadershipUser>().Add(supervisorEntity);
                }
            }

            // Kết thúc công tác
            if (requestModel.WorkFinish != null)
            {
                var workFinishModel = requestModel.WorkFinish;
                var workFinishEntity = new WorkFinish()
                {
                    WorkTicketID = workTicketEntity.ID,
                    WorkFinishPictures = workFinishModel.WorkFinishPictures?.Select(e => new WorkFinishPicture()
                    {
                        Attachments = (e.Attachments != null && e.Attachments.Count > 0) ? string.Join(",", e.Attachments.ToArray()) : null,
                        Position = e.Position != null ? e.Position : null,
                        IsChecked = e.IsChecked != null ? e.IsChecked : false,
                        CreatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                    }).ToList(),
                    OperatorConfirmed1 = workFinishModel.OperatorConfirmed1 != null ? workFinishModel.OperatorConfirmed1 : false,
                    OperatorConfirmed2 = workFinishModel.OperatorConfirmed2 != null ? workFinishModel.OperatorConfirmed2 : false,
                    OperatorConfirmed3 = workFinishModel.OperatorConfirmed3 != null ? workFinishModel.OperatorConfirmed3 : false,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                };

                //ReturnWorkplaceTime
                if (DateTimeConvert.ConvertDateAndTimeFromString(workFinishModel.ReturnWorkplaceTime) == DateTime.MaxValue)
                {
                    workFinishEntity.ReturnWorkplaceTime = null;
                }
                else
                {
                    workFinishEntity.ReturnWorkplaceTime = DateTimeConvert.ConvertDateAndTimeFromString(workFinishModel.ReturnWorkplaceTime);
                }

                //CheckReceivedTime
                if (DateTimeConvert.ConvertDateAndTimeFromString(workFinishModel.CheckReceivedTime) == DateTime.MaxValue)
                {
                    workFinishEntity.CheckReceivedTime = null;
                }
                else
                {
                    workFinishEntity.CheckReceivedTime = DateTimeConvert.ConvertDateAndTimeFromString(workFinishModel.CheckReceivedTime);
                }

                //CheckCompletedTime
                if (DateTimeConvert.ConvertDateAndTimeFromString(workFinishModel.CheckCompletedTime) == DateTime.MaxValue)
                {
                    workFinishEntity.CheckCompletedTime = null;
                }
                else
                {
                    workFinishEntity.CheckCompletedTime = DateTimeConvert.ConvertDateAndTimeFromString(workFinishModel.CheckCompletedTime);
                }

                // thêm người cho phép
                UserInfo userInfo = _commonService.GetUserById(requestModel.AllowedInfo.Acceptor.UserName, requestModel.AllowedInfo.Acceptor.IsExternal);
                if (userInfo != null)
                {
                    var signature = new Signature
                    {
                        Type = (int)SignatureType.Image,
                        SignatureContentByte = userInfo.Signature != null ? Encoding.UTF8.GetBytes(userInfo.Signature) : null,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    };

                    _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                    workFinishEntity.AllowedSignatureInfo = signature;
                }

                // thêm người cấp phiếu
                UserInfo approvalUserInfo = _commonService.GetUserById(requestModel.Approver.UserName, requestModel.Approver.IsExternal);
                if (approvalUserInfo != null && approvalUserInfo.Signature != null)
                {
                    var signature = new Signature
                    {
                        Type = (int)SignatureType.Image,
                        SignatureContentByte = Encoding.UTF8.GetBytes(approvalUserInfo.Signature),
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    };

                    _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                    workFinishEntity.ApproverSignatureInfo = signature;
                }
                _unitOfWork.ElogbookRepository<WorkFinish>().Add(workFinishEntity);

                // thêm người đại diện đơn vị quản lý
                if (workFinishModel.RepresentativeUnit != null)
                {
                    var representativeUnitEntity = new LeadershipUser()
                    {
                        WorkTicketID = workTicketEntity.ID,
                        Roles = (int)LeadershipUserRoles.RepresentativeUnit,
                        FullName = !string.IsNullOrWhiteSpace(requestModel.WorkFinish.RepresentativeUnit) ? requestModel.WorkFinish.RepresentativeUnit : null,
                        PositionName = !string.IsNullOrWhiteSpace(requestModel.WorkFinish.RepresentativeUnitPositionName) ? requestModel.WorkFinish.RepresentativeUnitPositionName : null,
                        CreatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                    };
                    _unitOfWork.ElogbookRepository<LeadershipUser>().Add(representativeUnitEntity);
                }
            }

            // Trường hợp thuộc phiếu yêu cầu(AmRequestTicket) thì cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
            if (workTicketEntity.RequestTicketID.HasValue)
            {
                UpdateAmDeviceAbnormalStatus((int)workTicketEntity.RequestTicketID, (int)AmEnumDeAbStatus.Yes_Finish, requestModel.CurrentUserID);
            }

            string[] arrNumTicket = requestModel.WorkTicketNumber.Split('/');
            Int32.TryParse(arrNumTicket[0], out int numTicket);
            string ticketType = "QLPCTD";
            if (workTicketEntity.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
            {
                ticketType = "QLPTCNH";
            }

            var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                .GetQuery(x => x.InsertedAt.Year == DateTime.Now.Year && x.TicketType.Equals(ticketType) && x.TicketNumber == numTicket).FirstOrDefault();
            if (lstNumber == null)
            {
                var data = new ExclusiveData()
                {
                    TicketNumber = numTicket,
                    TicketType = ticketType
                };
                _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
            }

            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                _errorMessages.Add("Cấp mới phiếu công tác thành công");
                responseModel.ErrorMessages = _errorMessages;
                responseModel.Data = workTicketEntity.ID;
                responseModel.StatusCode = (int)HttpStatusCode.OK;

                //[Notification-5 && 12]
                var processEmailRequestModel = new ProcessEmailRequestModel
                {
                    RegistrationTicketID = workTicketEntity.RegistrationTicketID ?? 0,
                    TicketStatus = (int)EnumTicketStatus.Approved,
                    RegistrationTicketType = workTicketEntity.ScreenType ?? 1,
                    WorkTicketID = workTicketEntity.ID,
                    IsPCT = true,
                    CurrentUserID = requestModel.CurrentUserID,
                    ModuleNotification = workTicketEntity.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                };
                _emailService.ProcessEmail(processEmailRequestModel);

                return responseModel;
            }
            else
            {
                _errorMessages.Add("Cấp mới phiếu công tác thất bại");
                responseModel.ErrorMessages = _errorMessages;
                responseModel.Data = -1;
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

                return responseModel;
            }
        }

        /// <summary>
        /// Tìm kiếm những phiếu đăng ký công tác đang chờ cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetRegistrationTickets(SearchRegistrationTicketModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            var entities = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.StatusValue == (int)Core.Enums.EnumTicketStatus.Waiting && !x.IsDeleted);
            //Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            //Ngày đăng ký (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.RegistrationDate) >= frDate.Date);
            }
            //Ngày đăng ký (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.RegistrationDate) <= toDate.Date);
            }
            //Loại phiếu
            if (requestModel.TicketType > 0)
            {
                entities = entities.Where(x => x.TicketType == requestModel.TicketType);
            }
            //Điều kiện lấy danh sách PCTĐ, PCT-TCNH, 1: PCTĐ, 2: PCT-TCNH
            if (requestModel.RegistrationTicketType.GetValueOrDefault(0) < 1)
                requestModel.RegistrationTicketType = 1;
            entities = entities.Where(x => x.RegistrationTicketType == requestModel.RegistrationTicketType);
            //Số phiếu đăng ký
            if (!string.IsNullOrWhiteSpace(requestModel.TicketNumber))
            {
                entities = entities.Where(x => x.TicketNumber.Contains(requestModel.TicketNumber.Trim()));
            }
            //Người đăng ký phiếu
            if (!string.IsNullOrWhiteSpace(requestModel.RegisterUser))
            {
                entities = entities.Where(x => x.CreatedUser.FullName.Contains(requestModel.RegisterUser.Trim()));
            }
            // Tìm kiếm theo đơn vị đăng ký
            if (requestModel.RegisterUnitID > 0)
            {
                var organizations = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                                && x.ID == requestModel.RegisterUnitID).FirstOrDefault();
                if (organizations != null && organizations.Level.Equals("Tổ"))
                {
                    entities = entities.Where(x => x.RegistrationUnit.ParentID == organizations.ParentID);
                }
                else
                {
                    entities = entities.Where(x => x.RegistrationUnit.ID == requestModel.RegisterUnitID);
                }
            }

            // Return and paging data
            var page = entities.Select(
                            e => new WorkTicketWaitingModel()
                            {
                                ID = e.ID,
                                TicketNumber = e.TicketNumber,
                                ParentUnit = e.RegistrationUnit.Parent != null ? new ValueLabelModel { Value = e.RegistrationUnit.Parent.ID, Label = e.RegistrationUnit.Parent.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                                RegistrationDateTemp = e.RegistrationDate,
                                RegistrationDateSort = e.RegistrationDate,
                                Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                                RegisterUnit = new ValueLabelModel { Value = e.RegistrationUnit.ID, Label = e.RegistrationUnit.Name },
                                RegisterUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                                TicketTypeValue = e.TicketType,
                                TicketTypeSort = e.TicketType,
                                FromDateTemp = e.StartDate,
                                ToDateTemp = e.EndDate,
                            })
                            .OrderByDescending(t => t.TicketTypeSort)
                            .ThenByDescending(t => t.RegistrationDateSort)
                            .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Lấy thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public AllowedInfoModel GetAcceptanceInfoTickets(int workTicketID, int? screenType)
        {
            var data = GetAllowedInfo(workTicketID);
            var acceptorUserEntity = GetLeadershipById(workTicketID, (int)LeadershipUserRoles.Acceptor);
            // thông tin cho phép
            if (data != null)
            {
                data.Acceptor = acceptorUserEntity;
                if (screenType != null && screenType == 2)
                {
                    var commanderUserEntity = GetLeadershipById(workTicketID, (int)LeadershipUserRoles.Commander);
                    data.Commander = commanderUserEntity;
                }
            }

            return data;
        }

        /// <summary>
        /// Tạo thông tin tiếp nhận nơi làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel CreateAdmissionWorkplace(AddReceiveWorkplaceMobileModel requestModel)
        {
            // Thông tin tiếp nhận nơi làm việc
            if (requestModel != null)
            {
                var receiveWorkplace = new ReceiveWorkplace()
                {
                    WorkTicketID = requestModel.WorkTicketID,
                    StartWorkingDate = requestModel.StartWorkingDate,
                    AmountPersons = !string.IsNullOrEmpty(requestModel.AmountPersons) ? requestModel.AmountPersons : null,
                    SafetyMeasures = requestModel.SafetyMeasures?.Select(e => new SafetyMeasure()
                    {
                        Note = !string.IsNullOrWhiteSpace(e.Note) ? e.Note : null,
                        IsChecked = e.IsChecked != null ? e.IsChecked : false,
                        Content = !string.IsNullOrWhiteSpace(e.Content) ? e.Content : null,
                        Position = e.Position,
                        Attachments = e.Attachments != null ? string.Join(",", e.Attachments.ToArray()) : null,
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

                // thêm người giám sát
                if (requestModel.Supervisor != null && requestModel.Supervisor.UserName != null && requestModel.Supervisor.IsExternal != null)
                {
                    UserInfo userInfo = _commonService.GetUserById(requestModel.Supervisor.UserName, (bool)requestModel.Supervisor.IsExternal);
                    if (userInfo != null)
                    {
                        var supervisorEntity = LeadershipUserEntity(userInfo, requestModel.CurrentUserID, (int)LeadershipUserRoles.Supervisor);
                        supervisorEntity.WorkTicketID = requestModel.WorkTicketID;
                        _unitOfWork.ElogbookRepository<LeadershipUser>().Add(supervisorEntity);
                    }

                    var signatureModel = new SignatureModel()
                    {
                        SignatureContent = requestModel.Supervisor.Signature.SignatureContent,
                        Type = requestModel.Supervisor.Signature.Type,
                        CurrentUserID = requestModel.CurrentUserID
                    };
                    var signatureInfo = _commonService.CreateSignature(signatureModel);
                    receiveWorkplace.SupervisorSignature = signatureInfo.ID;
                }

                // người chỉ huy
                if (requestModel.Commander != null)
                {
                    var signatureModel = new SignatureModel()
                    {
                        SignatureContent = requestModel.Commander.Signature.SignatureContent,
                        Type = requestModel.Commander.Signature.Type,
                        CurrentUserID = requestModel.CurrentUserID
                    };
                    var signatureInfo = _commonService.CreateSignature(signatureModel);
                    receiveWorkplace.CommanderSignature = signatureInfo.ID;
                }

                // người chỉ huy
                if (requestModel.Leader != null)
                {
                    var signatureModel = new SignatureModel()
                    {
                        SignatureContent = requestModel.Leader.Signature.SignatureContent,
                        Type = requestModel.Leader.Signature.Type,
                        CurrentUserID = requestModel.CurrentUserID
                    };
                    var signatureInfo = _commonService.CreateSignature(signatureModel);
                    receiveWorkplace.LeaderSignature = signatureInfo.ID;
                }
                _unitOfWork.ElogbookRepository<ReceiveWorkplace>().Add(receiveWorkplace);

                var data = _unitOfWork.ElogbookRepository<WorkTicket>()
                                .GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    data.WorkTicketStatus = (int)Core.Enums.EnumTicketStatus.InProgress;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);
                }
            }

            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                return HttpResponse.OK("Tạo thành công");
            }

            return HttpResponse.Error("Tạo không thành công");
        }

        /// <summary>
        /// Tạo Thông tin công tác hàng ngày ở ngày cụ thể
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel CreateDailyWorkInfo(AddDetailTicketMobileModel requestModel)
        {
            // Thông tin công tác hàng ngày
            if (requestModel != null)
            {
                var data = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
                if (data != null && data.WorkTicketStatus != (int)EnumTicketStatus.Completed)
                {
                    data.WorkPictures = requestModel.WorkPictures != null ? string.Join(",", requestModel.WorkPictures.ToArray()) : null;
                    data.WorkResult = !string.IsNullOrEmpty(requestModel.WorkResult) ? requestModel.WorkResult : null;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);
                }
            }

            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
                return HttpResponse.OK("Tạo thành công");

            return HttpResponse.Error("Tạo không thành công");
        }

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public ResponseBaseModel AddWorkerInformationForDate(int workTicketId, int currentUserID)
        {
            var existData = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketId
                        && DbFunctions.TruncateTime(x.InsertedAt) == DateTime.Today && !x.IsDeleted).Count();

            var groupDate = DateTime.Today.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);

            if (existData > 0)
            {
                return HttpResponse.OK(new BaseGroupDataModel<WorkerInformationModel>()
                {
                    GroupDate = groupDate,
                    MasterDetail = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketId
                        && DbFunctions.TruncateTime(x.InsertedAt) == DateTime.Today && !x.IsDeleted)
                                .Select(e => new WorkerInformationModel()
                                {
                                    ID = e.ID,
                                    FullName = e.FullName,
                                    CheckInDateTime = e.CheckInTime,
                                    CheckInSignatureID = e.CheckInSignature,
                                    CheckOutDateTime = e.CheckOutTime,
                                    CheckOutSignatureID = e.CheckOutSignature,
                                    IsExternal = e.IsExternal,
                                    UserName = e.UserName,
                                }).ToList()
                });
            }

            var data = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == workTicketId && !x.IsDeleted).FirstOrDefault();
            if (data != null && data.RegistrationTicketID.HasValue)
            {
                List<StakeholdersResponseModel> stakeholders = _registrationTicketService.GetStakeholders(data.RegistrationTicketID.Value, RolesStakeholder.Worker);
                if (stakeholders != null && stakeholders.Count > 0)
                {
                    var workerInformations = stakeholders.Select(e => new WorkerInformation()
                    {
                        WorkTicketID = workTicketId,
                        FullName = !string.IsNullOrWhiteSpace(e.FullName) ? e.FullName : string.Empty,
                        CheckInTime = DateTime.Today,
                        CheckOutTime = DateTime.Today,
                        CreatedUserID = currentUserID,
                        LastUpdatedUserID = currentUserID,
                        IsExternal = e.IsExternal,
                        UserName = e.UserName,
                    }).ToList();

                    _unitOfWork.ElogbookRepository<WorkerInformation>().AddRange(workerInformations);
                }
            }
            else
            {
                if (data != null && data.UnitNumber > 0)
                {
                    for (int i = 0; i < data.UnitNumber; i++)
                    {
                        var WorkerInformation = new WorkerInformation()
                        {
                            WorkTicketID = workTicketId,
                            FullName = string.Empty,
                            CheckInTime = DateTime.Today,
                            CheckOutTime = DateTime.Today,
                            CreatedUserID = currentUserID,
                            LastUpdatedUserID = currentUserID,
                        };

                        _unitOfWork.ElogbookRepository<WorkerInformation>().Add(WorkerInformation);
                    }
                }
            }

            if (_unitOfWork.Complete() > 0)
            {
                return HttpResponse.OK(new BaseGroupDataModel<WorkerInformationModel>()
                {
                    GroupDate = groupDate,
                    MasterDetail = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketId
                        && DbFunctions.TruncateTime(x.InsertedAt) == DateTime.Today && !x.IsDeleted)
                                .Select(e => new WorkerInformationModel()
                                {
                                    ID = e.ID,
                                    FullName = e.FullName,
                                    CheckInDateTime = e.CheckInTime,
                                    CheckInSignatureID = e.CheckInSignature,
                                    CheckOutDateTime = e.CheckOutTime,
                                    CheckOutSignatureID = e.CheckOutSignature,
                                    IsExternal = e.IsExternal,
                                    UserName = e.UserName,
                                }).ToList()
                });
            }
            else
            {
                return HttpResponse.OK(new BaseGroupDataModel<WorkerInformationModel>()
                {
                    GroupDate = groupDate,
                    MasterDetail = null,
                });
            }
        }

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddEditWorkerInformation(AddEditWorkerInformationModel requestModel)
        {
            var id = 0;
            var saveSuccess = false;
            // Thông tin công tác hàng ngày
            if (requestModel != null)
            {
                if (requestModel.ID > 0)
                {
                    id = requestModel.ID;
                    var data = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                    if (data != null)
                    {
                        data.FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName : string.Empty;
                        data.CheckInTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckInTime);
                        data.CheckOutTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckOutTime);
                        data.LastUpdatedUserID = requestModel.CurrentUserID;
                        data.UserName = !string.IsNullOrWhiteSpace(requestModel.UserName) ? requestModel.UserName : data.UserName;

                        if (requestModel.CheckInSignature != null && !string.IsNullOrWhiteSpace(requestModel.CheckInSignature.SignatureContent))
                        {
                            requestModel.CheckInSignature.CurrentUserID = requestModel.CurrentUserID;
                            var signatureInfo = _commonService.CreateSignature(requestModel.CheckInSignature);
                            data.CheckInSignature = signatureInfo.ID;
                        }

                        if (requestModel.CheckOutSignature != null && !string.IsNullOrWhiteSpace(requestModel.CheckOutSignature.SignatureContent))
                        {
                            requestModel.CheckOutSignature.CurrentUserID = requestModel.CurrentUserID;
                            var signatureInfoCheckout = _commonService.CreateSignature(requestModel.CheckOutSignature);
                            data.CheckOutSignature = signatureInfoCheckout.ID;
                        }

                        _unitOfWork.ElogbookRepository<WorkerInformation>().Update(data);
                        saveSuccess = _unitOfWork.Complete() > 0;
                    }
                }
                else
                {
                    var WorkerInformation = new WorkerInformation()
                    {
                        WorkTicketID = requestModel.WorkTicketID,
                        FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName : string.Empty,
                        CheckInTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckInTime),
                        CheckOutTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckOutTime),
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        IsExternal = requestModel.IsExternal,
                        UserName = requestModel.UserName,
                    };

                    if (requestModel.CheckInSignature != null && !string.IsNullOrWhiteSpace(requestModel.CheckInSignature.SignatureContent))
                    {
                        requestModel.CheckInSignature.CurrentUserID = requestModel.CurrentUserID;
                        var signatureInfo = _commonService.CreateSignature(requestModel.CheckInSignature);
                        WorkerInformation.CheckInSignature = signatureInfo.ID;
                    }

                    if (requestModel.CheckOutSignature != null && !string.IsNullOrWhiteSpace(requestModel.CheckOutSignature.SignatureContent))
                    {
                        requestModel.CheckOutSignature.CurrentUserID = requestModel.CurrentUserID;
                        var signatureInfoCheckout = _commonService.CreateSignature(requestModel.CheckOutSignature);
                        WorkerInformation.CheckOutSignature = signatureInfoCheckout.ID;
                    }

                    _unitOfWork.ElogbookRepository<WorkerInformation>().Add(WorkerInformation);
                    saveSuccess = _unitOfWork.Complete() > 0;
                    id = WorkerInformation.ID;
                }
            }

            if (saveSuccess)
            {
                var entity = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new WorkerInformationModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    CheckInDateTime = e.CheckInTime,
                    CheckInSignatureID = e.CheckInSignature,
                    CheckOutDateTime = e.CheckOutTime,
                    CheckOutSignatureID = e.CheckOutSignature,
                    IsExternal = e.IsExternal,
                    UserName = e.UserName,
                }).FirstOrDefault();

                return HttpResponse.OK(entity);
            }

            return HttpResponse.Error("Tạo không thành công");
        }

        /// <summary>
        /// Xóa thông tin công tác hàng ngày
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteWorkerInformation(int ID, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<WorkerInformation>().GetById(ID);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<WorkerInformation>().Delete(entity, userID);
                _unitOfWork.Complete();

                return HttpResponse.OK("Xóa thành công");
            }
            else
            {
                return HttpResponse.Error("ID không tìm thấy");
            }
        }

        /// <summary>
        /// Tạo Thông tin địa điểm công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddEditWorkPlacement(AddEditWorkPlacementModel requestModel)
        {
            var id = 0;
            var saveSuccess = false;
            // Thông tin địa điểm công tác
            if (requestModel != null)
            {
                if (requestModel.ID > 0)
                {
                    id = requestModel.ID;
                    var data = _unitOfWork.ElogbookRepository<WorkPlacement>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                    if (data != null)
                    {

                        data.Place = !string.IsNullOrWhiteSpace(requestModel.Place) ? requestModel.Place : null;
                        data.StartTime = !string.IsNullOrEmpty(requestModel.StartTime) ? DateTimeConvert.ConvertDateAndTimeFromString(requestModel.StartTime) : (DateTime?)null;
                        data.EndTime = !string.IsNullOrEmpty(requestModel.EndTime) ? DateTimeConvert.ConvertDateAndTimeFromString(requestModel.EndTime) : (DateTime?)null;
                        data.CommanderName = !string.IsNullOrWhiteSpace(requestModel.CommanderName) ? requestModel.CommanderName : null;
                        data.AcceptorName = !string.IsNullOrWhiteSpace(requestModel.AllowerName) ? requestModel.AllowerName : null;
                        data.LastUpdatedUserID = requestModel.CurrentUserID;
                        data.UpdatedAt = DateTime.Now;

                        if (requestModel.CommanderSignature != null && !string.IsNullOrWhiteSpace(requestModel.CommanderSignature.SignatureContent))
                        {
                            requestModel.CommanderSignature.CurrentUserID = requestModel.CurrentUserID;
                            var signatureInfo = _commonService.CreateSignature(requestModel.CommanderSignature);
                            data.CommanderSignature = signatureInfo.ID;
                        }

                        if (requestModel.AllowerSignature != null)
                        {
                            requestModel.AllowerSignature.CurrentUserID = requestModel.CurrentUserID;
                            var signatureInfo2 = _commonService.CreateSignature(requestModel.AllowerSignature);
                            data.AcceptorSignature = signatureInfo2.ID;
                        }

                        _unitOfWork.ElogbookRepository<WorkPlacement>().Update(data);
                        saveSuccess = _unitOfWork.Complete() > 0;
                    }
                }
                else
                {
                    var workPlacement = new WorkPlacement()
                    {
                        WorkTicketID = requestModel.WorkTicketID,
                        Place = !string.IsNullOrWhiteSpace(requestModel.Place) ? requestModel.Place : null,
                        StartTime = !string.IsNullOrEmpty(requestModel.StartTime) ? DateTimeConvert.ConvertDateAndTimeFromString(requestModel.StartTime) : (DateTime?)null,
                        EndTime = !string.IsNullOrEmpty(requestModel.EndTime) ? DateTimeConvert.ConvertDateAndTimeFromString(requestModel.EndTime) : (DateTime?)null,
                        CommanderName = !string.IsNullOrWhiteSpace(requestModel.CommanderName) ? requestModel.CommanderName : null,
                        AcceptorName = !string.IsNullOrWhiteSpace(requestModel.AllowerName) ? requestModel.AllowerName : null,
                        CreatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                    };

                    if (requestModel.CommanderSignature != null && !string.IsNullOrWhiteSpace(requestModel.CommanderSignature.SignatureContent))
                    {
                        requestModel.CommanderSignature.CurrentUserID = requestModel.CurrentUserID;
                        var signatureInfo = _commonService.CreateSignature(requestModel.CommanderSignature);
                        workPlacement.CommanderSignature = signatureInfo.ID;
                    }

                    if (requestModel.AllowerSignature != null)
                    {
                        requestModel.AllowerSignature.CurrentUserID = requestModel.CurrentUserID;
                        var signatureInfo2 = _commonService.CreateSignature(requestModel.AllowerSignature);
                        workPlacement.AcceptorSignature = signatureInfo2.ID;
                    }

                    _unitOfWork.ElogbookRepository<WorkPlacement>().Add(workPlacement);
                    saveSuccess = _unitOfWork.Complete() > 0;
                    id = workPlacement.ID;
                }
            }
            if (saveSuccess)
            {
                ////3. Khi ký Cho phép làm việc công tác hàng ngày, di chuyển nơi làm việc:
                ////PCT {NUMBER_TICKET} bắt đầu công tác hàng ngày lúc {DATE}. Số nhân viên đơn vị công tác: {NUMBER}.
                //if (isSignatureCheckin)
                //{
                //    _commonService.AsyncStatusAndContent((int)EventType.PCTD_START, requestModel.CurrentUserID, requestModel.WorkTicketID, null, startTime, null);
                //}

                ////4. Khi ký kết thúc công tác hàng ngày, di chuyển nơi làm việc:
                ////PCT {NUMBER_TICKET} kết thúc công tác hàng ngày lúc {DATE}. Số nhân viên đơn vị công tác: {NUMBER}.
                //if (isSignatureCheckout)
                //{
                //    _commonService.AsyncStatusAndContent((int)EventType.PCTD_END, requestModel.CurrentUserID, requestModel.WorkTicketID, null, null, endTime);
                //}

                var entities = _unitOfWork.ElogbookRepository<WorkPlacement>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new WorkPlacementModel()
                {
                    ID = e.ID,
                    Place = e.Place,
                    //Người chỉ huy trực tiếp
                    CommanderName = e.CommanderName,
                    StartTimeTemp = e.StartTime,
                    EndTimeTemp = e.EndTime,
                    CommanderSignatureID = e.CommanderSignature,
                    //Người cho phép
                    AllowerName = e.AcceptorName,
                    AllowerSignatureID = e.AcceptorSignature,
                }).FirstOrDefault();

                return HttpResponse.OK(entities);
            }

            return HttpResponse.Error("Tạo không thành công");
        }

        /// <summary>
        /// Xóa  địa điểm làm việc
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteWorkPlacement(int ID, int userID)
        {
            // xóa địa điểm làm việc
            if (ID > 0)
            {
                var entity = _unitOfWork.ElogbookRepository<WorkPlacement>().GetById(ID);
                if (entity != null)
                {
                    _unitOfWork.ElogbookRepository<WorkPlacement>().Delete(entity, userID);
                    _unitOfWork.Complete();

                    return HttpResponse.OK("Xóa thành công");
                }

                HttpResponse.Error("Xóa không thành công");
            }

            return HttpResponse.Error("ID không tìm thấy");
        }

        /// <summary>
        /// Tạo chi tiết kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel CreateInfoEndTicket(AddEditWorkFinishMobileModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.ApiVersion)) requestModel.ApiVersion = string.Empty;
            if ("V2".Equals(requestModel.ApiVersion, StringComparison.OrdinalIgnoreCase))
            {
                return CreateInfoEndTicketV2(requestModel);
            }
            else
            {
                LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, CreateInfoEndTicket().{Environment.NewLine}Request Data:{Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel)}");
                using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        if (requestModel != null)
                        {
                            var workFinishEntity = new WorkFinish()
                            {
                                WorkTicketID = requestModel.WorkTicketID,
                                RepresentativeUnit = requestModel.RepresentativeUnit,
                                RepresentativeUnitPositionName = requestModel.RepresentativeUnitPositionName,
                                WorkFinishPictures = requestModel.WorkFinishPictures?.Select(e => new WorkFinishPicture()
                                {
                                    Attachments = e.Attachments != null ? string.Join(",", e.Attachments.ToArray()) : null,
                                    IsChecked = e.IsChecked != null ? e.IsChecked : false,
                                    Position = e.Position != null ? e.Position : null,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    UpdatedAt = DateTime.Now,
                                }).ToList(),
                                OperatorConfirmed1 = requestModel.OperatorConfirmed1 != null ? requestModel.OperatorConfirmed1 : false,
                                OperatorConfirmed2 = requestModel.OperatorConfirmed2 != null ? requestModel.OperatorConfirmed2 : false,
                                OperatorConfirmed3 = requestModel.OperatorConfirmed3 != null ? requestModel.OperatorConfirmed3 : false,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };

                            //ReturnWorkplaceTime
                            if (DateTimeConvert.ConvertDateAndTimeFromString(requestModel.ReturnWorkplaceTime) == DateTime.MaxValue)
                            {
                                workFinishEntity.ReturnWorkplaceTime = null;
                            }
                            else
                            {
                                workFinishEntity.ReturnWorkplaceTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.ReturnWorkplaceTime);
                            }

                            //CheckReceivedTime
                            if (DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckReceivedTime) == DateTime.MaxValue)
                            {
                                workFinishEntity.CheckReceivedTime = null;
                            }
                            else
                            {
                                workFinishEntity.CheckReceivedTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckReceivedTime);
                            }

                            //CheckCompletedTime
                            if (DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckCompletedTime) == DateTime.MaxValue)
                            {
                                workFinishEntity.CheckCompletedTime = null;
                            }
                            else
                            {
                                workFinishEntity.CheckCompletedTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckCompletedTime);
                            }

                            // Lưu thông tin chữ ký người lãnh đạo công việc
                            // người lãnh đạo
                            if (requestModel.LeaderSignature != null && requestModel.LeaderSignature.SignatureContent != null)
                            {
                                var signature = new Signature
                                {
                                    Type = (int)SignatureType.Image,
                                    SignatureContentByte = Encoding.UTF8.GetBytes(requestModel.LeaderSignature.SignatureContent),
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };

                                _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                                workFinishEntity.LeaderSignatureInfo = signature;
                            }

                            // Lưu thông tin chữ ký người chỉ huy trực tiếp
                            // người chỉ huy
                            if (requestModel.CommanderSignature != null && requestModel.CommanderSignature.SignatureContent != null)
                            {
                                var signature = new Signature
                                {
                                    Type = (int)SignatureType.Image,
                                    SignatureContentByte = Encoding.UTF8.GetBytes(requestModel.CommanderSignature.SignatureContent),
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };

                                _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                                workFinishEntity.CommanderSignatureInfo = signature;
                            }

                            // chữ ký Người close (đại diện đơn vị quản lý)
                            if (requestModel.CloserSignature != null && requestModel.CloserSignature.SignatureContent != null)
                            {
                                var signature = new Signature
                                {
                                    Type = (int)SignatureType.Image,
                                    SignatureContentByte = Encoding.UTF8.GetBytes(requestModel.CloserSignature.SignatureContent),
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };
                                _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                                workFinishEntity.AllowedSignatureInfo = signature;
                            }
                            _unitOfWork.ElogbookRepository<WorkFinish>().Add(workFinishEntity);

                            if (_unitOfWork.Complete() > 0)
                            {
                                var data = _unitOfWork.ElogbookRepository<WorkTicket>()
                                                                        .GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();

                                if (data != null)
                                {
                                    if (data.ScreenType != null && data.ScreenType == 2)
                                    {
                                        // ký số tạo file pdf
                                        var workTicketDetail = GetWorkTicketById(data.ID);
                                        if (workTicketDetail != null)
                                        {
                                            // tạo thư mục lưu file pdf
                                            if (!Directory.Exists(Path.Combine(_rootPath, "WorkTicket", "PDF")))
                                            {
                                                Directory.CreateDirectory(Path.Combine(_rootPath, "WorkTicket", "PDF"));
                                            }
                                            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                                            string fileName = $"WorkTicket_{currentDate}.pdf";
                                            string fileNameSigned = $"WorkTicket_{currentDate}_signed.pdf";
                                            string outputFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileName);
                                            string signedFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileNameSigned);

                                            Pdf881ContentModel pdfDataModel = CreatePdf881Data(workTicketDetail);
                                            PdfInfoModel pdfInfoModel = GeneratePdf881File(pdfDataModel, outputFile, false);

                                            if (pdfInfoModel != null && pdfInfoModel.Status)
                                            {
                                                var acceptor = GetLeadershipById(data.ID, (int)LeadershipUserRoles.Acceptor);
                                                if (acceptor != null)
                                                {
                                                    UserInfo userApproved = _commonService.GetInternalUserById(acceptor.UserName);

                                                    // Ký số PDF — hỗ trợ 4 phương thức: Tài khoản, Viettel CA, VNPT SmartCA, Viettel MySign
                                                    ResponseModel<bool> signResult = SignPdfForTicket(userApproved, data.WorkTicketNumber, outputFile, "Sig_Completer");
                                                    if (!signResult.Data)
                                                    {
                                                        transaction.Rollback();
                                                        return HttpResponse.Error(string.Join(",", signResult.ErrorMessages));
                                                    }

                                                    var attachmentPdfCreated = new Attachment
                                                    {
                                                        FilePath = Path.Combine("WorkTicket", "PDF", fileName),
                                                        FileName = fileName,
                                                        ScreenName = "WorkTicket",
                                                        FileType = "pdf",
                                                    };
                                                    _unitOfWork.ElogbookRepository<Attachment>().Add(attachmentPdfCreated);

                                                    if (_unitOfWork.Complete() > 0)
                                                    {
                                                        data.TicketPdfID = attachmentPdfCreated.ID;
                                                        data.WorkTicketStatus = (int)EnumTicketStatus.Completed;
                                                        data.UpdatedAt = DateTime.Now;
                                                        data.LastUpdatedUserID = requestModel.CurrentUserID;
                                                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);

                                                        // Trường hợp thuộc phiếu yêu cầu(AmRequestTicket) thì cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                                                        if (data.RequestTicketID.HasValue)
                                                        {
                                                            UpdateAmDeviceAbnormalStatus((int)data.RequestTicketID, (int)AmEnumDeAbStatus.Yes_Finish, requestModel.CurrentUserID);
                                                        }

                                                        if (_unitOfWork.Complete() > 0)
                                                        {
                                                            //3-Phần D. Kết thúc công tác: Khi VHV ký Khóa kết thúc PCT => Lưu nội dung:
                                                            //Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ VHV).
                                                            _commonService.AsyncStatusAndContent((int)EventType.PCT_TCNH_COMPLETED, requestModel.CurrentUserID, requestModel.WorkTicketID, workFinishEntity.CheckReceivedTime, null, null);

                                                            transaction.Commit();

                                                            //[Notification-7 && 14]
                                                            var processEmailRequestModel = new ProcessEmailRequestModel
                                                            {
                                                                RegistrationTicketID = data.RegistrationTicketID ?? 0,
                                                                TicketStatus = data.WorkTicketStatus,
                                                                RegistrationTicketType = data.ScreenType ?? 1,
                                                                WorkTicketID = data.ID,
                                                                IsPCT = true,
                                                                CurrentUserID = requestModel.CurrentUserID,
                                                                ModuleNotification = data.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                                                            };
                                                            _emailService.ProcessEmail(processEmailRequestModel);

                                                            return HttpResponse.OK("Tạo kết thúc công tác thành công");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        data.UpdatedAt = DateTime.Now;
                                        data.LastUpdatedUserID = requestModel.CurrentUserID;
                                        data.WorkTicketStatus = (int)Core.Enums.EnumTicketStatus.WaitForConfirmation;
                                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);

                                        if (_unitOfWork.Complete() > 0)
                                        {
                                            //5.Kết thúc công tác: Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (sổ Trưởng ca).
                                            _commonService.AsyncStatusAndContent((int)EventType.PCTD_LOCKED, requestModel.CurrentUserID, requestModel.WorkTicketID, workFinishEntity.CheckReceivedTime, null, null);

                                            transaction.Commit();

                                            //[Notification-7 && 14]
                                            var processEmailRequestModel = new ProcessEmailRequestModel
                                            {
                                                RegistrationTicketID = data.RegistrationTicketID ?? 0,
                                                TicketStatus = data.WorkTicketStatus,
                                                RegistrationTicketType = data.ScreenType ?? 1,
                                                WorkTicketID = data.ID,
                                                IsPCT = true,
                                                CurrentUserID = requestModel.CurrentUserID,
                                                ModuleNotification = data.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                                            };
                                            _emailService.ProcessEmail(processEmailRequestModel);
                                            return HttpResponse.OK("Tạo kết thúc công tác thành công");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "CreateInfoEndTicket",
                            Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                        transaction.Rollback();
                        return HttpResponse.Error("Tạo không thành công");
                    }
                    transaction.Rollback();
                    return HttpResponse.Error("Tạo không thành công");
                }
            }
        }

        public ResponseBaseModel CreateInfoEndTicketV2(AddEditWorkFinishMobileModel requestModel)
        {
            LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, CreateInfoEndTicket().{Environment.NewLine}Request Data:{Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel)}");
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (requestModel != null)
                    {
                        var workFinishEntity = new WorkFinish()
                        {
                            WorkTicketID = requestModel.WorkTicketID,
                            WorkFinishPictures = requestModel.WorkFinishPictures?.Select(e => new WorkFinishPicture()
                            {
                                Attachments = e.Attachments != null ? string.Join(",", e.Attachments.ToArray()) : null,
                                IsChecked = e.IsChecked != null ? e.IsChecked : false,
                                Position = e.Position != null ? e.Position : null,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            }).ToList(),
                            OperatorConfirmed1 = requestModel.OperatorConfirmed1 != null ? requestModel.OperatorConfirmed1 : false,
                            OperatorConfirmed2 = requestModel.OperatorConfirmed2 != null ? requestModel.OperatorConfirmed2 : false,
                            OperatorConfirmed3 = requestModel.OperatorConfirmed3 != null ? requestModel.OperatorConfirmed3 : false,
                            CreatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            UpdatedAt = DateTime.Now,
                        };

                        //ReturnWorkplaceTime
                        if (DateTimeConvert.ConvertDateAndTimeFromString(requestModel.ReturnWorkplaceTime) == DateTime.MaxValue)
                        {
                            workFinishEntity.ReturnWorkplaceTime = null;
                        }
                        else
                        {
                            workFinishEntity.ReturnWorkplaceTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.ReturnWorkplaceTime);
                        }

                        //CheckReceivedTime
                        if (DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckReceivedTime) == DateTime.MaxValue)
                        {
                            workFinishEntity.CheckReceivedTime = null;
                        }
                        else
                        {
                            workFinishEntity.CheckReceivedTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckReceivedTime);
                        }

                        //CheckCompletedTime
                        if (DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckCompletedTime) == DateTime.MaxValue)
                        {
                            workFinishEntity.CheckCompletedTime = null;
                        }
                        else
                        {
                            workFinishEntity.CheckCompletedTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.CheckCompletedTime);
                        }

                        // Lưu thông tin chữ ký người lãnh đạo công việc
                        // người lãnh đạo
                        if (requestModel.LeaderSignature != null && requestModel.LeaderSignature.SignatureContent != null)
                        {

                            var signature = new Signature
                            {
                                Type = (int)SignatureType.Image,
                                SignatureContentByte = Encoding.UTF8.GetBytes(requestModel.LeaderSignature.SignatureContent),
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            };

                            _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                            workFinishEntity.LeaderSignatureInfo = signature;
                        }

                        // Lưu thông tin chữ ký người chỉ huy trực tiếp
                        // người chỉ huy
                        if (requestModel.CommanderSignature != null && requestModel.CommanderSignature.SignatureContent != null)
                        {
                            var signature = new Signature
                            {
                                Type = (int)SignatureType.Image,
                                SignatureContentByte = Encoding.UTF8.GetBytes(requestModel.CommanderSignature.SignatureContent),
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            };

                            _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                            workFinishEntity.CommanderSignatureInfo = signature;
                        }

                        // chữ ký Người close (đại diện đơn vị quản lý)
                        if (requestModel.CloserSignature != null && requestModel.CloserSignature.SignatureContent != null)
                        {
                            var signature = new Signature
                            {
                                Type = (int)SignatureType.Image,
                                SignatureContentByte = Encoding.UTF8.GetBytes(requestModel.CloserSignature.SignatureContent),
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            };
                            _unitOfWork.ElogbookRepository<Signature>().Add(signature);
                            workFinishEntity.AllowedSignatureInfo = signature;
                        }
                        _unitOfWork.ElogbookRepository<WorkFinish>().Add(workFinishEntity);

                        if (_unitOfWork.Complete() > 0)
                        {
                            var data = _unitOfWork.ElogbookRepository<WorkTicket>()
                                                                    .GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();

                            if (data != null)
                            {
                                if (data.ScreenType != null && data.ScreenType == 2)
                                {
                                    // ký số tạo file pdf
                                    var workTicketDetail = GetWorkTicketById(data.ID);

                                    if (workTicketDetail != null)
                                    {
                                        // tạo thư mục lưu file pdf
                                        if (!Directory.Exists(Path.Combine(_rootPath, "WorkTicket", "PDF")))
                                        {
                                            Directory.CreateDirectory(Path.Combine(_rootPath, "WorkTicket", "PDF"));
                                        }
                                        string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                                        string fileName = $"WorkTicket_{currentDate}.pdf";
                                        string fileNameSigned = $"WorkTicket_{currentDate}_signed.pdf";
                                        string outputFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileName);
                                        string signedFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileNameSigned);

                                        Pdf881ContentModel pdfDataModel = CreatePdf881Data(workTicketDetail);
                                        PdfInfoModel pdfInfoModel = GeneratePdf881File(pdfDataModel, outputFile, false);

                                        if (pdfInfoModel != null && pdfInfoModel.Status)
                                        {
                                            //Cập nhật thông tin người khóa phiếu là người được chọn
                                            var acceptor = GetLeadershipById(data.ID, (int)LeadershipUserRoles.RepresentativeUnit);
                                            if (acceptor != null)
                                            {
                                                UserInfo userApproved = _commonService.GetInternalUserById(acceptor.UserName);

                                                // Ký số PDF — hỗ trợ 4 phương thức: Tài khoản, Viettel CA, VNPT SmartCA, Viettel MySign
                                                ResponseModel<bool> signResult = SignPdfForTicket(userApproved, data.WorkTicketNumber, outputFile, "Sig_Completer");
                                                if (!signResult.Data)
                                                {
                                                    transaction.Rollback();
                                                    return HttpResponse.Error(string.Join(",", signResult.ErrorMessages));
                                                }

                                                var attachmentPdfCreated = new Attachment
                                                {
                                                    FilePath = Path.Combine("WorkTicket", "PDF", fileName),
                                                    FileName = fileName,
                                                    ScreenName = "WorkTicket",
                                                    FileType = "pdf",
                                                };
                                                _unitOfWork.ElogbookRepository<Attachment>().Add(attachmentPdfCreated);

                                                if (_unitOfWork.Complete() > 0)
                                                {
                                                    data.TicketPdfID = attachmentPdfCreated.ID;
                                                    data.WorkTicketStatus = (int)EnumTicketStatus.Completed;
                                                    data.UpdatedAt = DateTime.Now;
                                                    data.LastUpdatedUserID = requestModel.CurrentUserID;
                                                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);

                                                    // Trường hợp thuộc phiếu yêu cầu(AmRequestTicket) thì cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                                                    if (data.RequestTicketID.HasValue)
                                                    {
                                                        UpdateAmDeviceAbnormalStatus((int)data.RequestTicketID, (int)AmEnumDeAbStatus.Yes_Finish, requestModel.CurrentUserID);
                                                    }

                                                    if (_unitOfWork.Complete() > 0)
                                                    {
                                                        //3-Phần D. Kết thúc công tác: Khi VHV ký Khóa kết thúc PCT => Lưu nội dung:
                                                        //Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ VHV).
                                                        _commonService.AsyncStatusAndContent((int)EventType.PCT_TCNH_COMPLETED, requestModel.CurrentUserID, requestModel.WorkTicketID, workFinishEntity.CheckReceivedTime, null, null);

                                                        transaction.Commit();

                                                        //[Notification-7 && 14]
                                                        var processEmailRequestModel = new ProcessEmailRequestModel
                                                        {
                                                            RegistrationTicketID = data.RegistrationTicketID ?? 0,
                                                            TicketStatus = data.WorkTicketStatus,
                                                            RegistrationTicketType = data.ScreenType ?? 1,
                                                            WorkTicketID = data.ID,
                                                            IsPCT = true,
                                                            CurrentUserID = requestModel.CurrentUserID,
                                                            ModuleNotification = data.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                                                        };
                                                        _emailService.ProcessEmail(processEmailRequestModel);

                                                        return HttpResponse.OK("Tạo kết thúc công tác thành công");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    data.UpdatedAt = DateTime.Now;
                                    data.LastUpdatedUserID = requestModel.CurrentUserID;
                                    data.WorkTicketStatus = (int)Core.Enums.EnumTicketStatus.WaitForConfirmation;
                                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);

                                    if (_unitOfWork.Complete() > 0)
                                    {
                                        //5.Kết thúc công tác: Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (sổ Trưởng ca).
                                        _commonService.AsyncStatusAndContent((int)EventType.PCTD_LOCKED, requestModel.CurrentUserID, requestModel.WorkTicketID, workFinishEntity.CheckReceivedTime, null, null);

                                        transaction.Commit();

                                        //[Notification-7 && 14]
                                        var processEmailRequestModel = new ProcessEmailRequestModel
                                        {
                                            RegistrationTicketID = data.RegistrationTicketID ?? 0,
                                            TicketStatus = data.WorkTicketStatus,
                                            RegistrationTicketType = data.ScreenType ?? 1,
                                            WorkTicketID = data.ID,
                                            IsPCT = true,
                                            CurrentUserID = requestModel.CurrentUserID,
                                            ModuleNotification = data.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                                        };
                                        _emailService.ProcessEmail(processEmailRequestModel);

                                        return HttpResponse.OK("Tạo kết thúc công tác thành công");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "CreateInfoEndTicket",
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                    return HttpResponse.Error("Tạo không thành công");
                }
                transaction.Rollback();
                return HttpResponse.Error("Tạo không thành công");
            }
        }

        /// <summary>
        /// Xác nhận Hoàn thành phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateWorkFinish(UpdateWorkFinishModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            var _errorMessages = new List<string>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (requestModel != null && requestModel.WorkTicketID > 0)
                    {
                        var workTicketEntity = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
                        var workFinishEntity = _unitOfWork.ElogbookRepository<WorkFinish>().GetQuery(x => x.WorkTicketID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();

                        if (workTicketEntity != null && workFinishEntity != null)
                        {
                            // Lấy thông tin người xác nhận hoàn thành
                            var completedConfirmUserModel = GetUserInfo(requestModel.CurrentUserID, false, (int)LeadershipUserRoles.CompletedConfirm);

                            UserInfo userApproved = new UserInfo();

                            // Lưu thông tin người xác nhận hoàn thành
                            if (completedConfirmUserModel != null)
                            {
                                LeadershipUser leadershipUser = new LeadershipUser()
                                {
                                    WorkTicketID = requestModel.WorkTicketID,
                                    UserName = !string.IsNullOrWhiteSpace(completedConfirmUserModel.UserName) ? completedConfirmUserModel.UserName : null,
                                    FullName = !string.IsNullOrWhiteSpace(completedConfirmUserModel.FullName) ? completedConfirmUserModel.FullName : null,
                                    PositionName = !string.IsNullOrWhiteSpace(completedConfirmUserModel.PositionName) ? completedConfirmUserModel.PositionName : null,
                                    IsExternal = completedConfirmUserModel.IsExternal,
                                    Roles = completedConfirmUserModel.Roles > 0 ? completedConfirmUserModel.Roles : null,
                                    ElectricRank = completedConfirmUserModel.ElectricRank,
                                    ElectricRankCard = !string.IsNullOrWhiteSpace(completedConfirmUserModel.ElectricRankCard) ? completedConfirmUserModel.ElectricRankCard : null,
                                    LaborCard = completedConfirmUserModel.LaborCard,
                                    LaborRank = completedConfirmUserModel.LaborRank,
                                    Signature = !string.IsNullOrWhiteSpace(completedConfirmUserModel.Signature) ? completedConfirmUserModel.Signature : null,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                };
                                _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);

                                userApproved = _commonService.GetInternalUserById(completedConfirmUserModel.UserName);
                            }

                            workFinishEntity.UpdatedAt = DateTime.Now;
                            workFinishEntity.LastUpdatedUserID = requestModel.CurrentUserID;

                            if (DateTimeConvert.ConvertDateAndTimeFromString(requestModel.FinishDate) == DateTime.MaxValue)
                            {
                                workFinishEntity.CheckCompletedTime = null;
                            }
                            else
                            {
                                workFinishEntity.CheckCompletedTime = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.FinishDate);
                            }
                            _unitOfWork.ElogbookRepository<WorkFinish>().Update(workFinishEntity);

                            if (_unitOfWork.Complete() > 0)
                            {
                                var workTicketDetail = GetWorkTicketById(workTicketEntity.ID);
                                if (workTicketDetail != null)
                                {
                                    // tạo thư mục lưu file pdf
                                    if (!Directory.Exists(Path.Combine(_rootPath, "WorkTicket", "PDF")))
                                    {
                                        Directory.CreateDirectory(Path.Combine(_rootPath, "WorkTicket", "PDF"));
                                    }
                                    var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                                    var fileName = $"WorkTicket_{currentDate}.pdf";
                                    var fileNameSigned = $"WorkTicket_{currentDate}_signed.pdf";
                                    var outputFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileName);
                                    var signedFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileNameSigned);

                                    // tạo file PDF(Phiếu điện) rùi ký vào file đó
                                    PdfWorkTicketModel pdfDataModel = CreatePdfData(workTicketDetail);
                                    PdfInfoModel pdfInfoModel = GeneratePdfFile(pdfDataModel, outputFile, true);

                                    if (pdfInfoModel != null && pdfInfoModel.Status && userApproved != null)
                                    {
                                        // Ký số PDF — hỗ trợ 4 phương thức: Tài khoản, Viettel CA, VNPT SmartCA, Viettel MySign
                                        ResponseModel<bool> signResult = SignPdfForTicket(userApproved, workTicketEntity.WorkTicketNumber, outputFile, "Sig_Completer");
                                        if (!signResult.Data)
                                        {
                                            transaction.Rollback();
                                            _errorMessages.Add("Tạo kết thúc thất bại!");
                                            responseModel.ErrorMessages = signResult.ErrorMessages;
                                            responseModel.Data = false;
                                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                                            return responseModel;
                                        }

                                        var attachmentPdfCreated = new Attachment
                                        {
                                            FilePath = Path.Combine("WorkTicket", "PDF", fileName),
                                            FileName = fileName,
                                            ScreenName = "WorkTicket",
                                            FileType = "pdf",
                                        };
                                        _unitOfWork.ElogbookRepository<Attachment>().Add(attachmentPdfCreated);

                                        if (_unitOfWork.Complete() > 0)
                                        {
                                            workTicketEntity.TicketPdfID = attachmentPdfCreated.ID;
                                            workTicketEntity.WorkTicketStatus = (int)EnumTicketStatus.Completed;
                                            workTicketEntity.UpdatedAt = DateTime.Now;
                                            workTicketEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                            _unitOfWork.ElogbookRepository<WorkTicket>().Update(workTicketEntity);

                                            // Trường hợp thuộc phiếu yêu cầu(AmRequestTicket) thì cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                                            if (workTicketEntity.RequestTicketID.HasValue)
                                            {
                                                UpdateAmDeviceAbnormalStatus((int)workTicketEntity.RequestTicketID, (int)AmEnumDeAbStatus.Yes_Finish, requestModel.CurrentUserID);
                                            }

                                            if (_unitOfWork.Complete() > 0)
                                            {
                                                //6. Hoàn thành PCT: Khóa kết thúc công tác PCT (số PCT) lúc ... (sổ Trưởng ca)
                                                _commonService.AsyncStatusAndContent((int)EventType.PCTD_COMPLETED, requestModel.CurrentUserID, requestModel.WorkTicketID, workFinishEntity.CheckCompletedTime, null, null);

                                                transaction.Commit();

                                                //[Notification-7 && 14]
                                                var processEmailRequestModel = new ProcessEmailRequestModel
                                                {
                                                    RegistrationTicketID = workTicketEntity.RegistrationTicketID ?? 0,
                                                    TicketStatus = workTicketEntity.WorkTicketStatus,
                                                    RegistrationTicketType = workTicketEntity.ScreenType ?? 1,
                                                    WorkTicketID = workTicketEntity.ID,
                                                    IsPCT = true,
                                                    CurrentUserID = requestModel.CurrentUserID,
                                                    ModuleNotification = workTicketEntity.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                                                };
                                                _emailService.ProcessEmail(processEmailRequestModel);

                                                _errorMessages.Add("Tạo kết thúc công tác thành công!");
                                                responseModel.ErrorMessages = _errorMessages;
                                                responseModel.Data = true;
                                                responseModel.StatusCode = (int)HttpStatusCode.OK;
                                                return responseModel;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    transaction.Rollback();
                    _errorMessages.Add("Tạo không thành công!");
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.Data = false;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

                    return responseModel;
                }

                transaction.Rollback();
                _errorMessages.Add("Tạo không thành công!");
                responseModel.ErrorMessages = _errorMessages;
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

                return responseModel;
            }
        }

        /// <summary>
        /// In file PDF
        /// </summary>
        public HttpResponseMessage ExportPdfFile(int id)
        {
            var rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            var workTicketDetail = GetWorkTicketById(id);
            if (workTicketDetail == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            var ticketType = workTicketDetail.ScreenType ?? 1;
            string[] arrTicketNumber = workTicketDetail.WorkTicketNumber.Split('/');
            string ticketNumber = string.Empty;
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (arrTicketNumber.Length > 0) ticketNumber = arrTicketNumber[0];
            if (workTicketDetail.WorkTicketStatus.Value == (int)Elogbook.Core.Enums.EnumTicketStatus.Approved && workTicketDetail.TicketPdfID.HasValue && workTicketDetail.TicketPdfID > 0)
            {
                var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == workTicketDetail.TicketPdfID && !x.IsDeleted).FirstOrDefault();
                if (attachment != null)
                {
                    var outputFile = Path.Combine(rootPath, attachment.FilePath);
                    var fileBytes = File.ReadAllBytes(outputFile);

                    if (fileBytes.Length > 0)
                    {
                        string fileName = $"Phieu_cong_tac_{ticketNumber}_{currentDate}.pdf";
                        return Download(new ByteArrayContent(fileBytes), fileName);
                    }
                }
            }
            else
            {
                // tạo thư mục lưu file pdf
                if (!Directory.Exists(Path.Combine(rootPath, "WorkTicket", "PDF")))
                {
                    Directory.CreateDirectory(Path.Combine(rootPath, "WorkTicket", "PDF"));
                }
                string fileName = $"Phieu_cong_tac_{ticketNumber}_{currentDate}.pdf";
                string outputFile = Path.Combine(rootPath, "WorkTicket", "PDF", fileName);

                PdfInfoModel pdfInfoModel = null;
                if (ticketType == 2)
                {
                    Pdf881ContentModel pdfDataModel = CreatePdf881Data(workTicketDetail);
                    pdfInfoModel = GeneratePdf881File(pdfDataModel, outputFile, true);
                }
                else
                {
                    // tạo file PDF rùi ký vào file đó
                    PdfWorkTicketModel pdfDataModel = CreatePdfData(workTicketDetail);
                    pdfInfoModel = GeneratePdfFile(pdfDataModel, outputFile, true);
                }

                if (pdfInfoModel.Status)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
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

        public ResponseBaseModel AddSubAllowedInfo(AddSubAllowedInfoModel requestModel)
        {
            var id = 0;
            var saveSuccess = false;
            // Thông tin biện pháp công tác
            if (requestModel != null)
            {
                if (requestModel.Id > 0)
                {
                    var data = _unitOfWork.ElogbookRepository<AllowedInfoDetail>().GetQuery(x => x.ID == requestModel.Id && !x.IsDeleted).FirstOrDefault();

                    id = requestModel.Id;
                    if (data != null)
                    {
                        data.Content = !string.IsNullOrWhiteSpace(requestModel.Content) ? requestModel.Content : null;
                        data.Position = requestModel.Position;
                        data.Attachments = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments.ToArray()) : null;
                        data.LastUpdatedUserID = requestModel.CurrentUserID;
                        data.UpdatedAt = DateTime.Now;
                        data.IsChecked = requestModel.IsChecked;
                        _unitOfWork.ElogbookRepository<AllowedInfoDetail>().Update(data);
                        saveSuccess = _unitOfWork.Complete() > 0;
                    }
                }
                else
                {
                    var data = new AllowedInfoDetail()
                    {
                        Content = !string.IsNullOrWhiteSpace(requestModel.Content) ? requestModel.Content : null,
                        Position = requestModel.Position,
                        Attachments = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments.ToArray()) : null,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                        IsChecked = requestModel.IsChecked,
                    };

                    _unitOfWork.ElogbookRepository<AllowedInfoDetail>().Add(data);
                    saveSuccess = _unitOfWork.Complete() > 0;
                    id = data.ID;
                }
            }

            if (saveSuccess)
            {
                var entity = _unitOfWork.ElogbookRepository<AllowedInfoDetail>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new AllowedInfoDetailModel()
                {
                    ID = e.ID,
                    Note = e.Note,
                    Content = e.Content,
                    Position = e.Position,
                    IsChecked = e.IsChecked,
                    ListPictureID = e.Attachments,
                }).FirstOrDefault();

                // file đính kèm
                if (entity != null && !string.IsNullOrWhiteSpace(entity.ListPictureID))
                {
                    entity.PictureAttachments = GetAttachedFile(entity.ListPictureID);
                }

                return HttpResponse.OK(entity);
            }

            return HttpResponse.Error("Tạo không thành công");
        }

        /// <summary>
        /// Xóa biện pháp làm việc
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteSubAllowedInfo(int ID, int userID)
        {
            // xóa địa điểm làm việc
            if (ID > 0)
            {
                var entity = _unitOfWork.ElogbookRepository<AllowedInfoDetail>().GetById(ID);
                if (entity != null)
                {
                    _unitOfWork.ElogbookRepository<AllowedInfoDetail>().Delete(entity, userID);
                    _unitOfWork.Complete();

                    return HttpResponse.OK("Xóa thành công");
                }

                HttpResponse.Error("Xóa không thành công");
            }

            return HttpResponse.Error("ID không tìm thấy");
        }

        public ResponseBaseModel AddWorkDailyModel(AddWorkDailyMobileModel requestModel)
        {
            LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, AddWorkDailyModel().{Environment.NewLine}Request Data:{Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel)}");
            var id = 0;
            var saveSuccess = false;
            int dailyEntityID = 0;
            // Thông tin biện pháp công tác
            if (requestModel != null)
            {
                var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
                if (workTicket == null)
                {
                    return HttpResponse.Error("Phiếu công tác không tồn tại");
                }

                var oldWorkLocationID = 0;
                if (requestModel.ID > 0)
                {
                    var dailyEntity = _unitOfWork.ElogbookRepository<WorkDaily>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                    if (dailyEntity == null)
                    {
                        return HttpResponse.Error("Id Công tác hàng ngày không tồn tại");
                    }
                    else
                    {
                        oldWorkLocationID = dailyEntity.WorkLocationID ?? 0;
                        dailyEntity.WorkDay = requestModel.WorkDay;
                        dailyEntity.WorkLocationText = requestModel.WorkLocationText;
                        dailyEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                        if (requestModel.WorkLocationID > 0)
                            dailyEntity.WorkLocationID = requestModel.WorkLocationID;
                        _unitOfWork.ElogbookRepository<WorkDaily>().Update(dailyEntity);
                    }
                    dailyEntityID = dailyEntity.ID;
                }
                else
                {
                    var dailyEntity = new WorkDaily()
                    {
                        WorkTicketID = requestModel.WorkTicketID,
                        WorkDay = requestModel.WorkDay,
                        WorkLocationText = requestModel.WorkLocationText,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    };
                    if (requestModel.WorkLocationID > 0)
                        dailyEntity.WorkLocationID = requestModel.WorkLocationID;

                    _unitOfWork.ElogbookRepository<WorkDaily>().Add(dailyEntity);
                    saveSuccess = _unitOfWork.Complete() > 0;
                    dailyEntityID = dailyEntity.ID;
                }

                if (requestModel.StaffInformations != null && requestModel.StaffInformations.Count > 0)
                {
                    var staffInformations = _unitOfWork.ElogbookRepository<StaffInformation>().GetQuery(x => x.WorkDailyID == requestModel.ID).ToList();
                    if (staffInformations != null && staffInformations.Count > 0 && requestModel.WorkLocationID > 0 && requestModel.WorkLocationID != oldWorkLocationID)
                    {
                        _unitOfWork.ElogbookRepository<StaffInformation>().Delete(staffInformations, requestModel.CurrentUserID, true);
                    }

                    foreach (var staffInfo in requestModel.StaffInformations)
                    {
                        var staffInformation = _unitOfWork.ElogbookRepository<StaffInformation>()
                                .GetQuery(x => x.ID == staffInfo.ID && !x.IsDeleted).FirstOrDefault();
                        if (staffInformation != null)
                        {
                            if (requestModel.WorkLocationID > 0 && requestModel.WorkLocationID != oldWorkLocationID)
                            {
                                _unitOfWork.ElogbookRepository<StaffInformation>().Delete(staffInformation, requestModel.CurrentUserID, true);
                            }
                            else
                            {
                                staffInformation.UserName = !string.IsNullOrEmpty(staffInfo.UserName) ? staffInfo.UserName : staffInformation.UserName;
                                staffInformation.FullName = !string.IsNullOrEmpty(staffInfo.FullName) ? staffInfo.FullName : staffInformation.FullName;
                                staffInformation.IsExternal = staffInfo.IsExternal != null ? staffInfo.IsExternal : staffInformation.IsExternal;
                                staffInformation.Mission = !string.IsNullOrEmpty(staffInfo.Mission) ? staffInfo.Mission : staffInformation.Mission;
                                staffInformation.PositionName = !string.IsNullOrEmpty(staffInfo.PositionName) ? staffInfo.PositionName : staffInformation.PositionName;
                                staffInformation.ElectricRankCard = !string.IsNullOrEmpty(staffInfo.ElectricRankCard) ? staffInfo.ElectricRankCard : staffInformation.ElectricRankCard;
                                staffInformation.CheckInTime = !string.IsNullOrEmpty(staffInfo.CheckInTime) ? staffInfo.CheckInTime : staffInformation.CheckInTime;
                                staffInformation.CheckOutTime = !string.IsNullOrEmpty(staffInfo.CheckOutTime) ? staffInfo.CheckOutTime : staffInformation.CheckOutTime;
                                staffInformation.CreatedUserID = requestModel.CurrentUserID;
                                staffInformation.LastUpdatedUserID = requestModel.CurrentUserID;
                                if (staffInfo.CheckInSignature != null)
                                {
                                    staffInfo.CheckInSignature.CurrentUserID = requestModel.CurrentUserID;
                                    var signatureInfo = _commonService.CreateSignature(staffInfo.CheckInSignature);
                                    staffInformation.CheckInSignature = signatureInfo.ID;
                                }

                                if (staffInfo.CheckOutSignature != null)
                                {
                                    staffInfo.CheckOutSignature.CurrentUserID = requestModel.CurrentUserID;
                                    var signatureInfoCheckout = _commonService.CreateSignature(staffInfo.CheckOutSignature);
                                    staffInformation.CheckOutSignature = signatureInfoCheckout.ID;
                                }
                                _unitOfWork.ElogbookRepository<StaffInformation>().Update(staffInformation);
                            }
                        }
                        else
                        {
                            var staff = new StaffInformation()
                            {
                                WorkDailyID = dailyEntityID,
                                UserName = !string.IsNullOrEmpty(staffInfo.UserName) ? staffInfo.UserName : null,
                                FullName = !string.IsNullOrEmpty(staffInfo.FullName) ? staffInfo.FullName : null,
                                IsExternal = staffInfo.IsExternal != null ? staffInfo.IsExternal : null,
                                Mission = !string.IsNullOrEmpty(staffInfo.Mission) ? staffInfo.Mission : null,
                                PositionName = !string.IsNullOrEmpty(staffInfo.PositionName) ? staffInfo.PositionName : null,
                                ElectricRankCard = !string.IsNullOrEmpty(staffInfo.ElectricRankCard) ? staffInfo.ElectricRankCard : null,
                                CheckInTime = !string.IsNullOrEmpty(staffInfo.CheckInTime) ? staffInfo.CheckInTime : null,
                                CheckOutTime = !string.IsNullOrEmpty(staffInfo.CheckOutTime) ? staffInfo.CheckOutTime : null,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                            };

                            if (staffInfo.CheckInSignature != null)
                            {
                                staffInfo.CheckInSignature.CurrentUserID = requestModel.CurrentUserID;
                                var signatureInfo = _commonService.CreateSignature(staffInfo.CheckInSignature);
                                staff.CheckInSignature = signatureInfo.ID;
                            }

                            if (staffInfo.CheckOutSignature != null)
                            {
                                staffInfo.CheckOutSignature.CurrentUserID = requestModel.CurrentUserID;
                                var signatureInfoCheckout = _commonService.CreateSignature(staffInfo.CheckOutSignature);
                                staff.CheckOutSignature = signatureInfoCheckout.ID;
                            }
                            _unitOfWork.ElogbookRepository<StaffInformation>().Add(staff);
                        }
                    }

                    saveSuccess = _unitOfWork.Complete() > 0;
                }

                id = dailyEntityID;
            }

            if (saveSuccess)
            {
                var entity = _unitOfWork.ElogbookRepository<WorkDaily>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new WorkDailyModel()
                {
                    ID = e.ID,
                    WorkDay = e.WorkDay,
                    WorkLocationText = e.WorkLocationText,
                    WorkLocationID = e.WorkLocationID,
                    StaffInformations = e.StaffInformations.Where(x => !x.IsDeleted).Select(t => new StaffInformationsModel()
                    {
                        ID = t.ID,
                        FullName = t.FullName,
                        UserName = t.UserName,
                        Mission = t.Mission,
                        IsExternal = t.IsExternal,
                        PositionName = t.PositionName,
                        ElectricRankCard = t.ElectricRankCard,
                        CheckInTime = t.CheckInTime,
                        CheckInSignatureID = t.CheckInSignature,
                        CheckOutTime = t.CheckOutTime,
                        CheckOutSignatureID = t.CheckOutSignature,
                    }).ToList(),
                }).FirstOrDefault();

                return HttpResponse.OK(entity);
            }

            return HttpResponse.Error("Tạo không thành công");
        }

        public ResponseBaseModel UpdateFlagNote(int workTicketID, bool isFlagNote, int currentUserID)
        {
            var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == workTicketID && !x.IsDeleted).FirstOrDefault();
            if (workTicket != null)
            {
                workTicket.IsFlagNote = isFlagNote;
                workTicket.LastUpdatedUserID = currentUserID;
                _unitOfWork.ElogbookRepository<WorkTicket>().Update(workTicket);
                _unitOfWork.Complete();

                return HttpResponse.OK("Cập nhật thành công!");
            }
            else
            {
                return HttpResponse.Error("Không tìm thấy dữ liệu cập nhật!");
            }
        }

        public WaitingNumberTicketModel GetNumberWaitingRegistrationTicket(int productionPlantID)
        {
            var waitingNumberTicketModel = new WaitingNumberTicketModel();

            // Lấy những PCTĐ có Đơn vị nhận phiếu là phân xưởng vận hành của user đang login
            var pctd = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ReceiveUnitID == productionPlantID
                                                                                            && x.StatusValue == (int)Core.Enums.EnumTicketStatus.Waiting
                                                                                            && x.RegistrationTicketType == 1
                                                                                            && !x.IsDeleted).ToList();
            if (pctd.Count > 0)
            {
                waitingNumberTicketModel.PCTD = pctd.Count;
            }

            // Lấy những PCT-TCNH có Đơn vị nhận phiếu là phân xưởng vận hành của user đang login
            var pcttcnh = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ReceiveUnitID == productionPlantID
                                                                                            && x.StatusValue == (int)Core.Enums.EnumTicketStatus.Waiting
                                                                                            && x.RegistrationTicketType == 2
                                                                                            && !x.IsDeleted).ToList();
            if (pcttcnh.Count > 0)
            {
                waitingNumberTicketModel.PCT_TCNH = pcttcnh.Count;
            }

            // Lấy những PTT của user đang login
            var ptt = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => x.ApproveUnitID == productionPlantID
                                                                                            && (x.TicketStatusValue == (int)Core.Enums.TicketManipulationStatus.Waiting) && !x.IsDeleted).ToList();
            if (ptt.Count > 0)
            {
                waitingNumberTicketModel.PTT = ptt.Count;
            }

            // Đếm những phiếu điện có trạng thái chờ phê duyệt thay đổi người cho phép
            waitingNumberTicketModel.Total_PCT_Dien = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.TrackingStatusID == 1
                                                                                        && x.ScreenType == (int)TicketTypePCT.DIEN
                                                                                        && x.WorkTicketStatus == (int)Core.Enums.EnumTicketStatus.Approved
                                                                                        && !x.IsDeleted).ToList().Count;

            // Đếm những phiếu tcnh có trạng thái chờ phê duyệt thay đổi người cho phép
            waitingNumberTicketModel.Total_PCT_TCNH = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.TrackingStatusID == 1
                                                                                        && x.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA
                                                                                        && x.WorkTicketStatus == (int)Core.Enums.EnumTicketStatus.Approved
                                                                                        && !x.IsDeleted).ToList().Count;

            // Đếm những phiếu điện có trạng thái chờ đăng ký
            waitingNumberTicketModel.Total_PCT_Dien_CDK = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ScreenType == (int)TicketTypePCT.DIEN
                                                                                        && x.WorkTicketStatus == (int)Core.Enums.EnumTicketStatus.WaitForRegist
                                                                                        && !x.IsDeleted).ToList().Count;

            // Đếm những phiếu tcnh có trạng thái chờ đăng ký
            waitingNumberTicketModel.Total_PCT_TCNH_CDK = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA
                                                                                        && x.WorkTicketStatus == (int)Core.Enums.EnumTicketStatus.WaitForRegist
                                                                                        && !x.IsDeleted).ToList().Count;

            return waitingNumberTicketModel;
        }

        /// <summary>
        /// Tổ chức thực hiện (Thủy cơ nhiệt hóa)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddWorkPlanInfo(AddWorkPlanInfoModel requestModel)
        {
            var workTicketEntity = _unitOfWork.ElogbookRepository<WorkTicket>()
                                            .GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();

            if (workTicketEntity != null)
            {
                var receiveWorkplace = new ReceiveWorkplace()
                {
                    WorkTicketID = workTicketEntity.ID,
                    WorkUnitName = !string.IsNullOrEmpty(requestModel.WorkUnitName) ? requestModel.WorkUnitName : null,
                    WorkUnitID = requestModel.WorkUnitID != null ? requestModel.WorkUnitID : null,
                    AmountPersons = !string.IsNullOrEmpty(requestModel.AmountPersons) ? requestModel.AmountPersons : null,
                    StartWorkingDate = DateTime.Now,
                    SafetyMeasures = requestModel.SafetyMeasures?.Select(e => new SafetyMeasure()
                    {
                        Content = !string.IsNullOrWhiteSpace(e.Content) ? e.Content : null,
                        Note = !string.IsNullOrWhiteSpace(e.Note) ? e.Note : null,
                        IsChecked = e.IsChecked != null ? e.IsChecked : false,
                        Attachments = (e.Attachments != null && e.Attachments.Count > 0) ? string.Join(",", e.Attachments.ToArray()) : null,
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

                workTicketEntity.WorkTicketStatus = (int)Core.Enums.EnumTicketStatus.InProgress;
                workTicketEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                _unitOfWork.ElogbookRepository<WorkTicket>().Update(workTicketEntity);

                var save = _unitOfWork.Complete() > 0;
                if (save)
                    return HttpResponse.OK("Thêm tổ chức thực hiện thành công!");

                return HttpResponse.OK("Thêm tổ chức thực hiện thất bại!");
            }

            return HttpResponse.OK("Không tìm dữ liệu của phiếu công tác");
        }

        /// <summary>
        /// Trả về thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public WorkPlanInfoModel GetWorkPlanInfo(int workTicketID)
        {
            var entity = _unitOfWork.ElogbookRepository<ReceiveWorkplace>()
                                            .GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted).Select(e => new WorkPlanInfoModel()
                                            {
                                                SafetyMeasures = e.SafetyMeasures.Where(x => !x.IsDeleted).Select(t => new SafetyMeasureModel()
                                                {
                                                    Content = t.Content,
                                                    IsChecked = t.IsChecked != null ? t.IsChecked : null,
                                                    Note = t.Note,
                                                    AttachmentsID = t.Attachments
                                                }).ToList(),
                                                AmountPersons = e.AmountPersons,
                                                WorkUnitName = e.WorkUnitName,
                                                WorkUnitLv1 = e.WorkUnitID != null ? new ValueLabelModel { Value = e.WorkUnit.ID, Label = e.WorkUnit.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                                                WorkUnitLv2 = e.WorkUnit.Parent != null ? new ValueLabelModel { Value = e.WorkUnit.Parent.ID, Label = e.WorkUnit.Parent.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                                            }).FirstOrDefault();

            if (entity != null)
            {
                if (entity.SafetyMeasures != null)
                {
                    foreach (var item in entity.SafetyMeasures)
                    {
                        item.PictureAttachments = new List<AttachmentModel>();
                        var attackments = item.AttachmentsID?.Split(',');
                        if (attackments != null)
                        {
                            foreach (var attachment in attackments)
                            {
                                if (int.TryParse(attachment, out int idItem))
                                {
                                    var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == idItem).FirstOrDefault();
                                    if (att != null)
                                    {
                                        item.PictureAttachments.Add(new AttachmentModel
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

                return entity;
            }

            return new WorkPlanInfoModel();
        }

        /// <summary>
        /// Trả về danh sách vị trí làm việc và danh sách nhân viên (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public List<ContentPositionModel> GetWorkLocations(int workTicketID)
        {
            var workTicketEntity = _unitOfWork.ElogbookRepository<WorkTicket>()
                            .GetQuery(x => x.ID == workTicketID && !x.IsDeleted).FirstOrDefault();
            if (workTicketEntity != null)
            {
                int rgTicketID = 0;
                if (workTicketEntity.RegistrationTicketID != null) rgTicketID = (int)workTicketEntity.RegistrationTicketID;
                if (rgTicketID > 0)
                {
                    var entities = _unitOfWork.ElogbookRepository<ContentPosition>()
                    .GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == rgTicketID);

                    var page = entities.Select(e => new ContentPositionModel()
                    {
                        ID = e.ID,
                        Position = e.Position,
                        Stakeholders = e.Stakeholders.Where(x => !x.IsDeleted).Select(t => new StakeholdersModel()
                        {
                            ID = t.ID,
                            AssignToUserID = t.AssignToUserID,
                            FullName = t.FullName,
                            Mission = t.Mission,
                            UserName = t.UserName,
                            ElectricRankStr = t.ElectricRank,
                            ElectricRankCard = t.ElectricRankCard,
                            PositionName = t.PositionName,
                            Roles = t.Roles,
                            IsExternal = t.IsExternal,
                        }).ToList(),
                    }).ToList();

                    return page;
                }

                return new List<ContentPositionModel>();
            }

            return new List<ContentPositionModel>();
        }

        /// <summary>
        /// Trả về chi tiết kết thúc công tác (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkFinishDetail(int workTicketID)
        {
            var entity = GetWorkFinish(workTicketID);
            if (entity != null)
            {
                entity.CommanderUser = GetLeadershipById(workTicketID, (int)LeadershipUserRoles.Commander);
                entity.AllowedUser = GetLeadershipById(workTicketID, (int)LeadershipUserRoles.Acceptor);
                entity.RepresentativeUnitUser = GetLeadershipById(workTicketID, (int)LeadershipUserRoles.RepresentativeUnit);
            }

            if (entity == null)
                HttpResponse.OK("Không tìm dữ liệu!");

            return HttpResponse.OK(entity);
        }

        /// <summary>
        /// Xóa nhân viên công tác 881
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteStaffInformation(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<StaffInformation>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<StaffInformation>().Delete(entity, userID);
                _unitOfWork.Complete();

                return HttpResponse.OK("Xóa thành công");
            }
            else
            {
                return HttpResponse.Error("ID không tìm thấy");
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên trong và ngoài công ty
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetListStaff(SearchStaffModel requestModel)
        {
            List<StaffInfoModel> staffInfos = new List<StaffInfoModel>();
            if (string.IsNullOrWhiteSpace(requestModel.SearchText)) requestModel.SearchText = string.Empty;
            if (requestModel.TypeStaff == 0) requestModel.TypeStaff = CommonConstant.ALL_KEY;
            if (string.IsNullOrWhiteSpace(requestModel.LaborRank)) requestModel.LaborRank = string.Empty;

            // Nhân viên đang công tác
            var entitiesStaff = _unitOfWork.ElogbookRepository<StaffInformation>()
                .GetQuery(x => !x.IsDeleted && x.CheckInSignature != null && (x.CheckOutTime != null || x.CheckOutTime != string.Empty)).ToList();

            // Nhân viên trong công ty
            if (requestModel.TypeStaff == -1 || requestModel.TypeStaff == 1)
            {
                var entitiesInternal = _unitOfWork.ElogbookRepository<User>()
                    .GetQuery(x => (x.FullName.Contains(requestModel.SearchText) || x.ElectricRankCard.Contains(requestModel.SearchText)) && x.LaborRank.Contains(requestModel.LaborRank) && !x.IsDeleted).ToList();
                if (entitiesInternal != null)
                {
                    foreach (var item in entitiesInternal)
                    {
                        bool isReady = false;
                        if (item.CompanyID > 0)
                        {
                            var compnay = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == item.CompanyID && "Công ty sửa chữa độc lập".Equals(x.CompanyType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (compnay != null) isReady = true;
                        }

                        if (item.ProductionPlantID.GetValueOrDefault(0) > 0)
                        {
                            var productionPlant = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == item.ProductionPlantID && "Sửa chữa".Equals(x.ProductionPlantType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (productionPlant != null) isReady = true;
                        }

                        if (!isReady) continue;

                        bool isAvailable = true;
                        if (entitiesStaff != null)
                        {
                            foreach (var item1 in entitiesStaff)
                            {
                                if (item1.UserName == item.UserName && item1.IsExternal == false)
                                {
                                    isAvailable = false;
                                    break;
                                }
                            }
                        }

                        StaffInfoModel staffInfo = new StaffInfoModel()
                        {
                            UserName = item.UserName,
                            FullName = item.FullName,
                            DateOfBirth = item.DateOfBirth.HasValue ? item.DateOfBirth.ToString() : string.Empty,
                            ElectricRankCard = item.ElectricRankCard,
                            LaborRankStr = item.LaborRank,
                            IsExternal = false,
                            IsAvailable = isAvailable,
                        };
                        staffInfos.Add(staffInfo);
                    }

                    if (requestModel.TypeStaff == 1)
                        return HttpResponse.OK(staffInfos);
                }
            }

            // Nhân viên ngoài công ty
            var entitiesExternal = _unitOfWork.ElogbookRepository<GuestUser>()
                .GetQuery(x => (x.FullName.Contains(requestModel.SearchText) || x.ElectricRankCard.Contains(requestModel.SearchText)) && x.LaborRank.Contains(requestModel.LaborRank) && !x.IsDeleted).ToList();

            if (entitiesExternal != null)
            {
                foreach (var item in entitiesExternal)
                {
                    bool isAvailable = true;
                    if (entitiesStaff != null)
                    {
                        foreach (var item1 in entitiesStaff)
                        {
                            if (item1.UserName == item.UserName && item1.IsExternal == true)
                            {
                                isAvailable = false;
                                break;
                            }
                        }
                    }
                    StaffInfoModel staffInfo = new StaffInfoModel()
                    {
                        UserName = item.UserName,
                        FullName = item.FullName,
                        DateOfBirth = item.DateOfBirth.HasValue ? item.DateOfBirth.ToString() : string.Empty,
                        ElectricRankCard = item.ElectricRankCard,
                        LaborRankStr = item.LaborRank,
                        IsExternal = true,
                        IsAvailable = isAvailable,
                    };

                    staffInfos.Add(staffInfo);
                }

                if (requestModel.TypeStaff == 2)
                    return HttpResponse.OK(staffInfos);
            }

            return HttpResponse.OK(staffInfos);
        }

        /// <summary>
        /// Lấy danh sách nhân viên được đăng ký
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetStakeholders(int workTicketID, RolesStakeholder commander)
        {
            var data = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == workTicketID && !x.IsDeleted).FirstOrDefault();
            if (data != null)
            {
                int idReg = data.RegistrationTicketID != null ? (int)data.RegistrationTicketID : 0;
                var entities = _unitOfWork.ElogbookRepository<Stakeholders>()
                    .GetQuery(x => !x.IsDeleted && x.Roles == (int)commander && x.RegistrationTicketID == idReg)
                    .Select(t => new StakeholdersResponseModel()
                    {
                        ID = t.ID,
                        AssignToUserID = t.AssignToUserID,
                        FullName = t.AssignToUserID > 0 ? t.AssignToUser.FullName : t.FullName,
                        ElectricRankStr = t.ElectricRank,
                        ElectricRankCard = t.ElectricRankCard,
                        Roles = t.Roles,
                        Mission = t.Mission,
                        PositionName = t.PositionName,
                        UserName = t.UserName,
                        IsExternal = t.IsExternal,
                    }).ToList();

                return HttpResponse.OK(entities);
            }

            return HttpResponse.OK("Không tìm thấy dữ liệu");
        }

        /// <summary>
        /// Cập nhật chi tiết thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateAllowedInfoDetail(AddAllowedInfoDetailModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            var entity = _unitOfWork.ElogbookRepository<AllowedInfoDetail>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();

            if (entity != null)
            {
                entity.IsChecked = requestModel.IsChecked != null ? requestModel.IsChecked : false;
                entity.IsChecked = requestModel.IsChecked != null ? requestModel.IsChecked : false;
                entity.Attachments = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments.ToArray()) : null;
                entity.UpdatedAt = DateTime.Now;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<AllowedInfoDetail>().Update(entity);
                var saveSuccess = _unitOfWork.Complete() > 0;
                if (saveSuccess)
                {
                    responseModel.ErrorMessages.Add("Cập nhật dữ liệu thành công");
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    responseModel.ErrorMessages.Add("Cập nhật dữ liệu thất bại");
                    responseModel.Data = false;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                return responseModel;
            }

            responseModel.ErrorMessages.Add("Không tìm thấy dữ liệu để thêm");
            responseModel.Data = false;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

            return responseModel;
        }

        /// <summary>
        /// Cập nhật thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateAllowedInfo(UpdateAllowedInfoModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            var entityWorkTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
            if (entityWorkTicket != null)
            {
                var entity = _unitOfWork.ElogbookRepository<AllowedInfo>().GetQuery(x => x.WorkTicketID == entityWorkTicket.ID && !x.IsDeleted).FirstOrDefault();
                if (entity != null)
                {
                    //bổ sung chữ ký người chỉ huy trực tiếp
                    if (requestModel.Commander != null)
                    {
                        if (requestModel.Commander.Signature != null)
                        {
                            var signatureModel = new SignatureModel()
                            {
                                CurrentUserID = requestModel.CurrentUserID,
                                Type = requestModel.Commander.Signature.Type,
                                SignatureContent = requestModel.Commander.Signature.SignatureContent,
                            };
                            var signatureInfo = _commonService.CreateSignature(signatureModel);
                            entity.CommanderSignature = signatureInfo.ID;
                        }

                        // thêm người cho chỉ huy vào table LeadershipUser
                        if (requestModel.Commander != null && requestModel.Commander.UserName != null && requestModel.Commander.IsExternal != null)
                        {
                            var commanderUser = GetUserInfo(requestModel.Commander?.UserName, (bool)requestModel.Commander.IsExternal, (int)LeadershipUserRoles.Commander);
                            if (commanderUser != null)
                            {
                                string commanderSignature = null;

                                if (!string.IsNullOrWhiteSpace(commanderUser.Signature))
                                {
                                    commanderSignature = commanderUser.Signature;
                                }
                                else
                                {
                                    if (requestModel.Commander?.Signature != null)
                                    {
                                        commanderSignature = requestModel.Commander.Signature.SignatureContent;
                                    }
                                }

                                LeadershipUser leadershipUser = new LeadershipUser()
                                {
                                    WorkTicketID = requestModel.WorkTicketID,
                                    UserName = !string.IsNullOrWhiteSpace(commanderUser.UserName) ? commanderUser.UserName : null,
                                    FullName = !string.IsNullOrWhiteSpace(commanderUser.FullName) ? commanderUser.FullName : null,
                                    PositionName = !string.IsNullOrWhiteSpace(commanderUser.PositionName) ? commanderUser.PositionName : null,
                                    IsExternal = commanderUser.IsExternal,
                                    Roles = commanderUser.Roles > 0 ? commanderUser.Roles : null,
                                    ElectricRank = commanderUser.ElectricRank,
                                    ElectricRankCard = !string.IsNullOrWhiteSpace(commanderUser.ElectricRankCard) ? commanderUser.ElectricRankCard : null,
                                    LaborCard = commanderUser.LaborCard,
                                    LaborRank = commanderUser.LaborRank,
                                    Signature = commanderSignature,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    UpdatedAt = DateTime.Now,
                                };
                                _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                            }
                        }
                    }

                    // lấy thông tin người cho phép
                    if (requestModel.Allower != null && requestModel.Allower.UserName != null && requestModel.Allower.IsExternal != null)
                    {
                        var acceptorUser = GetUserInfo(requestModel.Allower?.UserName, (bool)requestModel.Allower.IsExternal, (int)LeadershipUserRoles.Acceptor);
                        if (acceptorUser != null)
                        {
                            LeadershipUser leadershipUser = new LeadershipUser()
                            {
                                WorkTicketID = requestModel.WorkTicketID,
                                UserName = !string.IsNullOrWhiteSpace(acceptorUser.UserName) ? acceptorUser.UserName : null,
                                FullName = !string.IsNullOrWhiteSpace(acceptorUser.FullName) ? acceptorUser.FullName : null,
                                PositionName = !string.IsNullOrWhiteSpace(acceptorUser.PositionName) ? acceptorUser.PositionName : null,
                                IsExternal = acceptorUser.IsExternal,
                                Roles = acceptorUser.Roles > 0 ? acceptorUser.Roles : null,
                                ElectricRank = acceptorUser.ElectricRank,
                                ElectricRankCard = !string.IsNullOrWhiteSpace(acceptorUser.ElectricRankCard) ? acceptorUser.ElectricRankCard : null,
                                LaborRank = acceptorUser.LaborRank,
                                LaborCard = acceptorUser.LaborCard,
                                Signature = !string.IsNullOrWhiteSpace(acceptorUser.Signature) ? acceptorUser.Signature : null,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };
                            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                        }
                    }
                    entity.AllowedTime = DateTime.Now;
                    entity.TimeCommanderSigned = DateTime.Now;
                    entity.UpdatedAt = DateTime.Now;
                    entity.LastUpdatedUserID = requestModel.CurrentUserID;

                    _unitOfWork.ElogbookRepository<AllowedInfo>().Update(entity);

                    entityWorkTicket.WorkTicketStatus = (int)EnumTicketStatus.Allowed;
                    entityWorkTicket.UpdatedAt = DateTime.Now;
                    entityWorkTicket.LastUpdatedUserID = requestModel.CurrentUserID;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(entityWorkTicket);

                    // Trường hợp thuộc phiếu yêu cầu(AmRequestTicket) thì cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                    if (entityWorkTicket.RequestTicketID.HasValue)
                    {
                        UpdateAmDeviceAbnormalStatus((int)entityWorkTicket.RequestTicketID, (int)AmEnumDeAbStatus.Yes_InProgress, requestModel.CurrentUserID);
                    }

                    var saveSuccess = _unitOfWork.Complete() > 0;

                    if (saveSuccess)
                    {
                        //[Notification-6 && 13]
                        var processEmailRequestModel = new ProcessEmailRequestModel
                        {
                            RegistrationTicketID = entityWorkTicket.RegistrationTicketID ?? 0,
                            TicketStatus = entityWorkTicket.WorkTicketStatus,
                            RegistrationTicketType = entityWorkTicket.ScreenType ?? 1,
                            WorkTicketID = entityWorkTicket.ID,
                            IsPCT = true,
                            CurrentUserID = requestModel.CurrentUserID,
                            ModuleNotification = entityWorkTicket.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                        };
                        _emailService.ProcessEmail(processEmailRequestModel);

                        if (entityWorkTicket.ScreenType == 2)
                        {
                            //2 - Phần B: Khi  ký Cho phép PCT=> lưu nội dung:
                            //Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE} (Lưu sổ VHV).
                            _commonService.AsyncStatusAndContent((int)EventType.PCT_TCNH_ALLOWED, requestModel.CurrentUserID, requestModel.WorkTicketID, entity.AllowedTime, null, null);
                        }
                        else
                        {
                            //2.Khi ký Cho phép PCT: Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE}.
                            _commonService.AsyncStatusAndContent((int)EventType.PCTD_ALLOWED, requestModel.CurrentUserID, requestModel.WorkTicketID, entity.AllowedTime, null, null);
                        }

                        responseModel.ErrorMessages.Add("Cập nhật dữ liệu thành công");
                        responseModel.Data = true;
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Cập nhật dữ liệu thất bại");
                        responseModel.Data = false;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }

                    return responseModel;
                }
                else
                {
                    var allowedEntity = new AllowedInfo()
                    {
                        WorkTicketID = requestModel.WorkTicketID,
                        TimeCommanderSigned = DateTime.Now,
                        AllowedTime = DateTime.Now,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    };

                    // Nếu là phiếu thủy cơ nhiệt thì bổ sung chữ ký người chỉ huy trực tiếp
                    if (entityWorkTicket.ScreenType == 2)
                    {
                        if (requestModel.Commander != null)
                        {
                            var signatureModel = new SignatureModel()
                            {
                                CurrentUserID = requestModel.CurrentUserID
                            };
                            if (requestModel.Commander.Signature != null)
                            {
                                signatureModel.Type = requestModel.Commander.Signature.Type;
                                signatureModel.SignatureContent = requestModel.Commander.Signature.SignatureContent;
                            }

                            var signatureInfo = _commonService.CreateSignature(signatureModel);
                            allowedEntity.CommanderSignature = signatureInfo.ID;
                        }

                        // lấy thông tin người cho phép
                        if (requestModel.Allower != null && requestModel.Allower.UserName != null && requestModel.Allower.IsExternal != null)
                        {
                            var acceptorUser = GetUserInfo(requestModel.Allower?.UserName, (bool)requestModel.Allower.IsExternal, (int)LeadershipUserRoles.Acceptor);
                            if (acceptorUser != null)
                            {
                                LeadershipUser leadershipUser = new LeadershipUser()
                                {
                                    WorkTicketID = requestModel.WorkTicketID,
                                    UserName = !string.IsNullOrWhiteSpace(acceptorUser.UserName) ? acceptorUser.UserName : null,
                                    FullName = !string.IsNullOrWhiteSpace(acceptorUser.FullName) ? acceptorUser.FullName : null,
                                    PositionName = !string.IsNullOrWhiteSpace(acceptorUser.PositionName) ? acceptorUser.PositionName : null,
                                    IsExternal = acceptorUser.IsExternal,
                                    Roles = acceptorUser.Roles > 0 ? acceptorUser.Roles : null,
                                    ElectricRank = acceptorUser.ElectricRank,
                                    ElectricRankCard = !string.IsNullOrWhiteSpace(acceptorUser.ElectricRankCard) ? acceptorUser.ElectricRankCard : null,
                                    LaborCard = acceptorUser.LaborCard,
                                    LaborRank = acceptorUser.LaborRank,
                                    Signature = !string.IsNullOrWhiteSpace(acceptorUser.Signature) ? acceptorUser.Signature : null,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    UpdatedAt = DateTime.Now,
                                };
                                _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                            }
                        }

                        // thêm người cho chỉ huy vào table LeadershipUser
                        if (requestModel.Commander != null && requestModel.Commander.UserName != null && requestModel.Commander.IsExternal != null)
                        {
                            var commanderUser = GetUserInfo(requestModel.Commander?.UserName, (bool)requestModel.Commander.IsExternal, (int)LeadershipUserRoles.Commander);
                            if (commanderUser != null)
                            {
                                string commanderSignature = null;

                                if (!string.IsNullOrWhiteSpace(commanderUser.Signature))
                                {
                                    commanderSignature = commanderUser.Signature;
                                }
                                else
                                {
                                    if (requestModel.Commander?.Signature != null)
                                    {
                                        commanderSignature = requestModel.Commander.Signature.SignatureContent;
                                    }
                                }

                                LeadershipUser leadershipUser = new LeadershipUser()
                                {
                                    WorkTicketID = requestModel.WorkTicketID,
                                    UserName = !string.IsNullOrWhiteSpace(commanderUser.UserName) ? commanderUser.UserName : null,
                                    FullName = !string.IsNullOrWhiteSpace(commanderUser.FullName) ? commanderUser.FullName : null,
                                    PositionName = !string.IsNullOrWhiteSpace(commanderUser.PositionName) ? commanderUser.PositionName : null,
                                    IsExternal = commanderUser.IsExternal,
                                    Roles = commanderUser.Roles > 0 ? commanderUser.Roles : null,
                                    ElectricRank = commanderUser.ElectricRank,
                                    ElectricRankCard = !string.IsNullOrWhiteSpace(commanderUser.ElectricRankCard) ? commanderUser.ElectricRankCard : null,
                                    LaborRank = commanderUser.LaborRank,
                                    LaborCard = commanderUser.LaborCard,
                                    Signature = commanderSignature,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    UpdatedAt = DateTime.Now,
                                };
                                _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                            }
                        }
                    }
                    _unitOfWork.ElogbookRepository<AllowedInfo>().Add(allowedEntity);
                    entityWorkTicket.WorkTicketStatus = (int)EnumTicketStatus.Allowed;
                    entityWorkTicket.LastUpdatedUserID = requestModel.CurrentUserID;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(entityWorkTicket);

                    // Trường hợp thuộc phiếu yêu cầu(AmRequestTicket) thì cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                    if (entityWorkTicket.RequestTicketID.HasValue)
                    {
                        UpdateAmDeviceAbnormalStatus((int)entityWorkTicket.RequestTicketID, (int)AmEnumDeAbStatus.Yes_InProgress, requestModel.CurrentUserID);
                    }

                    var saveSuccess = _unitOfWork.Complete() > 0;
                    if (saveSuccess)
                    {
                        //[Notification-6 && 13]
                        var processEmailRequestModel = new ProcessEmailRequestModel
                        {
                            RegistrationTicketID = entityWorkTicket.RegistrationTicketID ?? 0,
                            TicketStatus = entityWorkTicket.WorkTicketStatus,
                            RegistrationTicketType = entityWorkTicket.ScreenType ?? 1,
                            WorkTicketID = entityWorkTicket.ID,
                            IsPCT = true,
                            CurrentUserID = requestModel.CurrentUserID,
                            ModuleNotification = entityWorkTicket.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                        };
                        _emailService.ProcessEmail(processEmailRequestModel);

                        if (entityWorkTicket.ScreenType == 2)
                        {
                            //2 - Phần B: Khi  ký Cho phép PCT=> lưu nội dung:
                            //Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE} (Lưu sổ VHV).
                            _commonService.AsyncStatusAndContent((int)EventType.PCT_TCNH_ALLOWED, requestModel.CurrentUserID, requestModel.WorkTicketID, allowedEntity.AllowedTime, null, null);
                        }
                        else
                        {
                            //2.Khi ký Cho phép PCT: Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE}.
                            _commonService.AsyncStatusAndContent((int)EventType.PCTD_ALLOWED, requestModel.CurrentUserID, requestModel.WorkTicketID, allowedEntity.AllowedTime, null, null);
                        }

                        responseModel.ErrorMessages.Add("Cập nhật dữ liệu thành công");
                        responseModel.Data = true;
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Cập nhật dữ liệu thất bại");
                        responseModel.Data = false;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }

                    return responseModel;
                }
            }

            responseModel.ErrorMessages.Add("Không tìm thấy dữ liệu của phiếu công tác");
            responseModel.Data = false;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

            return responseModel;
        }

        /// <summary>
        /// Cập nhật chi tiết tổ chức thực hiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateSafetyMeasure(AddSafetyMeasureModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            var entity = _unitOfWork.ElogbookRepository<SafetyMeasure>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();

            if (entity != null)
            {
                entity.IsChecked = requestModel.IsChecked != null ? requestModel.IsChecked : false;
                entity.Attachments = requestModel.Attachments != null ? string.Join(",", requestModel.Attachments.ToArray()) : null;
                entity.UpdatedAt = DateTime.Now;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<SafetyMeasure>().Update(entity);
                var saveSuccess = _unitOfWork.Complete() > 0;
                if (saveSuccess)
                {
                    responseModel.ErrorMessages.Add("Cập nhật dữ liệu thành công");
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    responseModel.ErrorMessages.Add("Cập nhật dữ liệu thất bại");
                    responseModel.Data = false;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                return responseModel;
            }

            responseModel.ErrorMessages.Add("Không tìm thấy dữ liệu để thêm");
            responseModel.Data = false;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

            return responseModel;
        }

        /// <summary>
        /// Cập nhật tổ chức thực hiện (Cập nhật số lượng người và đơn vị công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateReceiveWorkplace(UpdateReceiveWorkplaceModel requestModel)
        {
            LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, UpdateReceiveWorkplace().{Environment.NewLine}Request Data:{Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel)}");
            var responseModel = new ResponseModel<bool>();

            var data = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID && !x.IsDeleted).FirstOrDefault();
            if (data != null)
            {
                var entity = _unitOfWork.ElogbookRepository<ReceiveWorkplace>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                var workUnit = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == requestModel.WorkUnitID).FirstOrDefault();

                if (entity != null)
                {
                    entity.WorkUnitName = !string.IsNullOrEmpty(requestModel.WorkUnitName) ? requestModel.WorkUnitName : null;
                    if (workUnit != null && workUnit.ID > 0) entity.WorkUnitID = workUnit.ID;
                    entity.AmountPersons = !string.IsNullOrEmpty(requestModel.AmountPersons) ? requestModel.AmountPersons : null;
                    entity.StartWorkingDate = DateTime.Now;
                    entity.LastUpdatedUserID = requestModel.CurrentUserID;

                    _unitOfWork.ElogbookRepository<ReceiveWorkplace>().Update(entity);

                    data.LastUpdatedUserID = requestModel.CurrentUserID;
                    data.WorkTicketStatus = (int)Core.Enums.EnumTicketStatus.InProgress;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);

                    var saveSuccess = _unitOfWork.Complete() > 0;
                    if (saveSuccess)
                    {
                        responseModel.Data = true;
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Cập nhật dữ liệu thất bại");
                        responseModel.Data = false;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }

                    return responseModel;
                }
                else
                {
                    var receiveWorkplace = new ReceiveWorkplace()
                    {
                        WorkTicketID = data.ID,
                        WorkUnitName = !string.IsNullOrEmpty(requestModel.WorkUnitName) ? requestModel.WorkUnitName : null,
                        AmountPersons = !string.IsNullOrEmpty(requestModel.AmountPersons) ? requestModel.AmountPersons : null,
                        StartWorkingDate = DateTime.Now,
                        CreatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                    };
                    if (workUnit != null && workUnit.ID > 0) receiveWorkplace.WorkUnitID = workUnit.ID;

                    _unitOfWork.ElogbookRepository<ReceiveWorkplace>().Add(receiveWorkplace);

                    data.WorkTicketStatus = (int)Core.Enums.EnumTicketStatus.InProgress;
                    data.LastUpdatedUserID = requestModel.CurrentUserID;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(data);

                    var saveSuccess = _unitOfWork.Complete() > 0;
                    if (saveSuccess)
                    {
                        responseModel.Data = true;
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Cập nhật dữ liệu thất bại");
                        responseModel.Data = false;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }

                    return responseModel;
                }
            }

            responseModel.ErrorMessages.Add("Không tìm thấy dữ liệu của phiếu công tác");
            responseModel.Data = false;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

            return responseModel;
        }

        /// <summary>
        /// Xóa nhóm nhân viên công tác 881
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteStaffInformationGroup(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<WorkDaily>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<WorkDaily>().Delete(entity, userID);
                _unitOfWork.Complete();

                return HttpResponse.OK("Xóa thành công");
            }
            else
            {
                return HttpResponse.Error("ID không tìm thấy");
            }
        }

        public bool CheckExistTicketNumber(string workTicketNumber, int screenType)
        {
            if (screenType <= 0) screenType = 1;
            if (string.IsNullOrWhiteSpace(workTicketNumber)) workTicketNumber = string.Empty;
            else workTicketNumber = workTicketNumber.Trim();

            //3: PTT
            if (screenType == 3)
            {
                return _unitOfWork.ElogbookRepository<ManipulationTicket>()
                    .GetQuery(x => x.TicketNumber.Equals(workTicketNumber, StringComparison.OrdinalIgnoreCase)).Any();
            }
            //1: PCTĐ, 2: PCTTCNH
            else
            {
                return _unitOfWork.ElogbookRepository<WorkTicket>()
                    .GetQuery(x => x.WorkTicketNumber.Equals(workTicketNumber, StringComparison.OrdinalIgnoreCase) && x.ScreenType == screenType).Any();
            }
        }

        public ResponseModel<int> SCLApproveTicket(AddNewWorkTicketModel requestModel, bool isApprove)
        {
            var responseModel = new ResponseModel<int>();
            var _errorMessages = new List<string>();

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == requestModel.WorkTicketID
                                                                                       && x.MajorRepairID == requestModel.MajorRepairID && !x.IsDeleted).FirstOrDefault();
                    if (entity == null)
                    {
                        _errorMessages.Add("Phiếu công tác không tồn tại");
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.Data = -1;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }

                    if (requestModel.ApprovedUnitID < 1)
                    {
                        _errorMessages.Add("Đơn vị cấp phiếu không tồn tại");
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.Data = -1;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }

                    requestModel.LeadershipUsers = new List<AddNewLeadershipUserFetchedModel>();

                    // lấy thông tin người cấp phiếu
                    var approveUser = GetUserInfo(requestModel.CurrentUserID, false, (int)LeadershipUserRoles.Approver);
                    if (approveUser != null)
                        requestModel.LeadershipUsers.Add(approveUser);

                    // lấy thông tin người chỉ huy
                    if (requestModel.Commander != null && requestModel.Commander.UserName != null)
                    {
                        var commander = GetUserInfo(requestModel.Commander?.UserName, requestModel.Commander.IsExternal, (int)LeadershipUserRoles.Commander);
                        if (commander != null)
                            requestModel.LeadershipUsers.Add(commander);
                    }

                    // lấy thông tin người lãnh đạo
                    if (requestModel.Leader != null && requestModel.Leader.UserName != null)
                    {
                        var leaderUser = GetUserInfo(requestModel.Leader.UserName, requestModel.Leader.IsExternal, (int)LeadershipUserRoles.Leader);
                        if (leaderUser != null)
                            requestModel.LeadershipUsers.Add(leaderUser);
                    }

                    // lấy thông tin người cho phép
                    if (requestModel.Acceptor != null && requestModel.Acceptor.UserName != null)
                    {
                        var acceptorUser = GetUserInfo(requestModel.Acceptor?.UserName, requestModel.Acceptor.IsExternal, (int)LeadershipUserRoles.Acceptor);
                        if (acceptorUser != null)
                            requestModel.LeadershipUsers.Add(acceptorUser);
                    }

                    // Cập nhật phiếu công tác
                    entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                    entity.RegistrationTicketID = requestModel.RegistrationTicketID > 0 ? requestModel.RegistrationTicketID : entity.RegistrationTicketID;
                    entity.WorkTicketFeatureID = requestModel.WorkTicketFeatureID > 0 ? requestModel.WorkTicketFeatureID : entity.WorkTicketFeatureID;
                    entity.ApprovedUnitID = requestModel.ApprovedUnitID;
                    entity.ApprovedDate = DateTime.Now;
                    entity.WorkTicketStatus = (int)EnumTicketStatus.Approved;
                    entity.UnitNumber = requestModel.UnitNumber != null ? requestModel.UnitNumber : entity.UnitNumber;
                    entity.WorkPlacement = !string.IsNullOrEmpty(requestModel.WorkPlacement) ? requestModel.WorkPlacement : entity.WorkPlacement;
                    entity.WorkContent = !string.IsNullOrEmpty(requestModel.WorkContent) ? requestModel.WorkContent : entity.WorkContent;
                    entity.StartDate = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.StartDate);
                    entity.EndDate = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.EndDate);
                    entity.WorkCondition = !string.IsNullOrEmpty(requestModel.WorkCondition) ? requestModel.WorkCondition : entity.WorkCondition;
                    entity.WorkScope = !string.IsNullOrEmpty(requestModel.WorkScope) ? requestModel.WorkScope : entity.WorkScope;
                    entity.Attachments = (requestModel.AttachmentsID != null) ? string.Join(";", requestModel.AttachmentsID.ToArray()) : entity.Attachments;
                    entity.CreatedUserID = requestModel.CurrentUserID;
                    entity.InsertedAt = DateTime.Now;
                    entity.LastUpdatedUserID = requestModel.CurrentUserID;
                    entity.UpdatedAt = DateTime.Now;

                    if (requestModel.LeadershipUsers != null && requestModel.LeadershipUsers.Count > 0)
                    {
                        foreach (var e in requestModel.LeadershipUsers)
                        {
                            var leadershipUser = new LeadershipUser()
                            {
                                WorkTicketID = entity.ID,
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
                            };
                            _unitOfWork.ElogbookRepository<LeadershipUser>().Add(leadershipUser);
                        }
                    }

                    //Nguy hiểm cảnh báo
                    if (requestModel.WarnAndSafes != null && requestModel.WarnAndSafes.Count > 0)
                    {
                        foreach (var e in requestModel.WarnAndSafes)
                        {
                            var warnAndSafe = new WarnAndSafe()
                            {
                                WorkTicketID = entity.ID,
                                WarningContent = e.WarningContent,
                                SafetyMeasureContent = e.SafetyMeasureContent,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now
                            };
                            _unitOfWork.ElogbookRepository<WarnAndSafe>().Add(warnAndSafe);
                        }
                    }

                    //Loại phiếu (TCNH)
                    if (requestModel.WorkTicketFeatures != null && requestModel.WorkTicketFeatures.Count > 0)
                    {
                        foreach (var item in requestModel.WorkTicketFeatures)
                        {
                            var workTicketFeatures = new WorkTicketFeatureLink()
                            {
                                WorkTicketID = entity.ID,
                                FeatureID = item,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };
                            _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>().Add(workTicketFeatures);
                        }
                    }

                    // Kiểm tra các biện pháp an toàn trước khi cho phép công tác
                    if (requestModel.AllowedInfoDetails != null && requestModel.AllowedInfoDetails.Count > 0)
                    {
                        var allowedEntity = new AllowedInfo()
                        {
                            WorkTicketID = entity.ID,
                            AllowedTime = DateTime.Now,
                            TimeCommanderSigned = DateTime.Now,
                            AllowedInfoDetails = requestModel.AllowedInfoDetails?.Select(e => new AllowedInfoDetail()
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

                    // Kiểm tra các biện pháp an toàn trước khi thực hiện công tác
                    if (requestModel.SafetyMeasures != null && requestModel.SafetyMeasures.Count > 0)
                    {
                        var receiveWorkplace = new ReceiveWorkplace()
                        {
                            WorkTicketID = entity.ID,
                            StartWorkingDate = DateTime.Now,
                            SafetyMeasures = requestModel.SafetyMeasures?.Select(e => new SafetyMeasure()
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

                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(entity);

                    var saveSuccess = _unitOfWork.Complete() > 0;

                    if (saveSuccess)
                    {
                        var workTicketDetail = GetWorkTicketById(entity.ID);
                        if (workTicketDetail != null)
                        {
                            // tạo thư mục lưu file pdf
                            if (!Directory.Exists(Path.Combine(_rootPath, "WorkTicket", "PDF")))
                            {
                                Directory.CreateDirectory(Path.Combine(_rootPath, "WorkTicket", "PDF"));
                            }

                            var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                            var fileName = $"WorkTicket_{currentDate}.pdf";
                            var fileNameSigned = $"WorkTicket_{currentDate}_signed.pdf";
                            var outputFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileName);
                            var signedFile = Path.Combine(_rootPath, "WorkTicket", "PDF", fileNameSigned);

                            // Báo lỗi nếu không tạo được PDF
                            PdfInfoModel pdfInfoModel = null;
                            PdfWorkTicketModel pdfWorkTicketModel = null;
                            Pdf881ContentModel pdf881ContentModel = null;
                            if (entity.ScreenType == null || (int)entity.ScreenType == 1)
                            {
                                // tạo file PDF(Phiếu điện) rùi ký vào file đó
                                pdfWorkTicketModel = CreatePdfData(workTicketDetail);
                                pdfInfoModel = GeneratePdfFile(pdfWorkTicketModel, outputFile, false);
                            }
                            else if (entity.ScreenType == 2)
                            {
                                // tạo file PDF(Phiếu TCNH) rùi ký vào file đó
                                pdf881ContentModel = CreatePdf881Data(workTicketDetail);
                                pdfInfoModel = GeneratePdf881File(pdf881ContentModel, outputFile, false);
                            }

                            if (pdfInfoModel != null && pdfInfoModel.Status)
                            {
                                if (approveUser != null)
                                {
                                    UserInfo userApproved = _commonService.GetInternalUserById(approveUser.UserName);
                                    if (userApproved != null)
                                    {
                                        // Ký số PDF — hỗ trợ 4 phương thức: Tài khoản, Viettel CA, VNPT SmartCA, Viettel MySign
                                        ResponseModel<bool> signResult = SignPdfForTicket(userApproved, entity.WorkTicketNumber, outputFile, "Sig_Approver");
                                        if (!signResult.Data)
                                        {
                                            transaction.Rollback();
                                            responseModel.ErrorMessages = signResult.ErrorMessages;
                                            responseModel.Data = -1;
                                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                                            return responseModel;
                                        }

                                        // Lưu Attachment trỏ về outputFile (đã được PdfSigningService ghi đè chữ ký)
                                        var attachmentPdfCreated = new Attachment
                                        {
                                            FilePath = Path.Combine("WorkTicket", "PDF", fileName),
                                            FileName = fileName,
                                            ScreenName = "WorkTicket",
                                            FileType = "pdf",
                                        };
                                        _unitOfWork.ElogbookRepository<Attachment>().Add(attachmentPdfCreated);

                                        // Thực hiện lưu cấp PCT
                                        {
                                            saveSuccess = _unitOfWork.Complete() > 0;
                                            if (saveSuccess)
                                            {
                                                entity.TicketPdfID = attachmentPdfCreated.ID;
                                                _unitOfWork.Complete();
                                                transaction.Commit();
                                                if (isApprove)
                                                {
                                                    string desciption = "Đồng ý cấp phiếu công tác. Số phiếu: " + entity.WorkTicketNumber;
                                                    _registrationTicketService.UpdateStatusRegistrationTicket(entity.RegistrationTicketID.GetValueOrDefault(0),
                                                                                                            (int)Core.Enums.EnumTicketStatus.Approved,
                                                                                                            desciption, (int)entity.CreatedUserID,
                                                                                                            attachmentPdfCreated.ID.ToString());
                                                    responseModel.Data = entity.ID;

                                                    return responseModel;
                                                }
                                                else
                                                {
                                                    responseModel.Data = entity.ID;
                                                    return responseModel;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        transaction.Rollback();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            _errorMessages.Add("Cấp phiếu công tác thất bại");
            responseModel.ErrorMessages = _errorMessages;
            responseModel.Data = -1;
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;

            return responseModel;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Cập nhật trạng thái bất thường thiết bị
        /// </summary>
        private void UpdateAmDeviceAbnormalStatus(int requestTicketID, int statusID, int userID)
        {
            var amEntity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                    .GetQuery(x => x.AmRequestTicketID == requestTicketID && !x.IsDeleted).FirstOrDefault();
            if (amEntity != null)
            {
                amEntity.AmDeviceAbnormalStatusID = statusID;
                amEntity.UpdatedAt = DateTime.Now;
                amEntity.LastUpdatedUserID = userID;
                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(amEntity);
            }
        }

        private PdfInfoModel GeneratePdf881File(Pdf881ContentModel pdfDataModel, string outputFile, bool isSigned)
        {
            _pdfInfo = new PdfInfoModel();
            try
            {
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    //Create document
                    Document pdfDocument = new Document(PageSize.A4);
                    pdfDocument.SetMargins(5, 5, 20, 20);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, fileStream);
                    // Open the document to enable you to write to the document
                    pdfDocument.Open();
                    try
                    {
                        PdfPTable tblData = new PdfPTable(1) { TotalWidth = TABLE_WIDTH };
                        tblData.DefaultCell.Border = Rectangle.NO_BORDER;
                        tblData.SetWidths(new float[] { 1 });
                        tblData.LockedWidth = true;
                        tblData.SplitLate = false;

                        // PdfHeaderWorkTicket
                        if (pdfDataModel.Header != null)
                        {
                            AddPdf881Header(tblData, pdfDataModel.Header);
                        }

                        //Phần A: Cấp phiếu công tác
                        if (pdfDataModel.PhaseA != null)
                        {
                            AddPdf881PhaseA(tblData, pdfDataModel.PhaseA, isSigned);
                        }

                        //Phần B: Cho phép công tác
                        if (pdfDataModel.PhaseB != null)
                        {
                            AddPdf881PhaseB(tblData, pdfDataModel.PhaseB);
                        }

                        //Phần C: Thực hiện công tác
                        if (pdfDataModel.PhaseC != null)
                        {
                            AddPdf881PhaseC(tblData, pdfDataModel.PhaseC);
                        }

                        if (pdfDataModel.PhaseD != null)
                        {
                            AddPdf881PhaseD(tblData, pdfDataModel.PhaseD);
                        }
                        pdfDocument.Add(tblData);
                        pdfDocument.Close();

                        posY = 815;
                        _pdfInfo.Status = true;
                        return _pdfInfo;
                    }
                    finally
                    {
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                        posY = 815;
                    }
                }
            }
            catch
            {
                _pdfInfo.Status = false;

            }
            return _pdfInfo;
        }

        private void AddPdf881PhaseD(PdfPTable mainTable, Pdf881PhaseDModel phaseD)
        {

            PdfPTable tblData1 = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
            tblData1.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData1.SetWidths(new float[] { 3.5f, 6.5f });
            tblData1.LockedWidth = true;
            int fSize = 13;

            //Phần D: Kết thúc công tác
            tblData1.AddCell(GetCellSign(_fBold, fSize, phaseD.Title, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Đơn vị công tác khẳng định:
            tblData1.AddCell(GetCellSign(_fNomal, fSize, phaseD.Title1, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Người, thiết bị, phương tiện đã rút hết
            tblData1.AddCell(GetCellSign(_fNomal, fSize, phaseD.WorkUnitConfirm1, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Thiết bị sẵn sàng đưa trở lại vận hành
            tblData1.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData1.AddCell(GetCellSign(_fNomal, fSize, phaseD.WorkUnitConfirm2, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));


            //Đã kết thúc công việc theo phiếu công tác này
            tblData1.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData1.AddCell(GetCellSign(_fNomal, fSize, phaseD.WorkUnitConfirm3, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER
            };
            cellMerge.AddElement(tblData1);
            mainTable.AddCell(cellMerge);

            PdfPTable tblSign1 = new PdfPTable(4) { TotalWidth = TABLE_WIDTH };
            tblSign1.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSign1.SetWidths(new float[] { 5, 1, 1, 3 });
            tblSign1.LockedWidth = true;
            //Người CHTT
            tblSign1.AddCell(GetCellSign(_fNomal, fSize, phaseD.CommanderName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            //Chữ ký
            tblSign1.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (phaseD.CommanderSignature != null)
            {
                tblSign1.AddCell(GetCellImageSignature(phaseD.CommanderSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM, 31));
            }
            else
            {
                tblSign1.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Completer", 0, 0, 0, 0, fixHeight: 31));
            }

            //Ngày giờ
            tblSign1.AddCell(GetCellSign(_fNomal, fSize, phaseD.SignedTime1, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));

            PdfPCell cellMerge1 = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge1.AddElement(tblSign1);
            mainTable.AddCell(cellMerge1);

            PdfPTable tblData2 = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
            tblData2.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData2.SetWidths(new float[] { 3.5f, 6.5f });
            tblData2.LockedWidth = true;
            //Đơn vị vận hành khẳng định:
            tblData2.AddCell(GetCellSign(_fNomal, fSize, phaseD.Title2, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Mọi biển báo, cảnh báo an toàn đã gỡ hết
            tblData2.AddCell(GetCellSign(_fNomal, fSize, phaseD.WorkOperatorConfirm1, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Thiết bị đủ điều kiện đưa trở lại vận hành
            tblData2.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData2.AddCell(GetCellSign(_fNomal, fSize, phaseD.WorkOperatorConfirm2, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Khóa phiếu công tác này
            tblData2.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData2.AddCell(GetCellSign(_fNomal, fSize, phaseD.WorkOperatorConfirm3, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge2 = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge2.AddElement(tblData2);
            mainTable.AddCell(cellMerge2);

            PdfPTable tblSign2 = new PdfPTable(4) { TotalWidth = TABLE_WIDTH };
            tblSign2.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSign2.SetWidths(new float[] { 5, 1, 1, 3 });
            tblSign2.LockedWidth = true;
            //Người cho phép
            tblSign2.AddCell(GetCellSign(_fNomal, fSize, phaseD.AllowerName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            //Chữ ký
            tblSign2.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (phaseD.AllowerSignature != null)
            {
                tblSign2.AddCell(GetCellImageSignature(phaseD.AllowerSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM, 31, false));
            }
            else
            {
                tblSign2.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Allower", 0, 0, 0, 0, fixHeight: 31));
            }

            //Ngày giờ
            tblSign2.AddCell(GetCellSign(_fNomal, fSize, phaseD.SignedTime2, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            PdfPCell cellMerge3 = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge3.AddElement(tblSign2);
            mainTable.AddCell(cellMerge3);
        }

        private void AddPdf881PhaseC(PdfPTable mainTable, Pdf881PhaseCModel phaseC)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 7, 3 });
            tblData.LockedWidth = true;
            int fSize = 13;

            //Phần C: Thực hiện công tác
            tblData.AddCell(GetCellSign(_fBold, fSize, phaseC.Title, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Đơn vị công tác:
            tblData.AddCell(GetCellSign(_fNomal, fSize, phaseC.WorkUnit, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            //Số lượng người
            tblData.AddCell(GetCellSign(_fNomal, fSize, phaseC.PersonNumber, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            // Danh sách các biện pháp an toàn
            AppendSafetyMeasuresToPdf(mainTable, phaseC.SafetyMeasures);

            //Danh sách Người tham gia thực hiện công tác đã biết, hiểu rõ
            PdfPTable tblData1 = new PdfPTable(1) { TotalWidth = TABLE_WIDTH };
            tblData1.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData1.SetWidths(new float[] { 1 });
            tblData1.LockedWidth = true;
            tblData1.AddCell(GetCellSign(_fNomal, fSize, phaseC.WorkDailyTitle, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge2 = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge2.AddElement(tblData1);
            mainTable.AddCell(cellMerge2);

            // Danh sách các biện pháp an toàn
            AppendWorkDailysToPdf(mainTable, phaseC.WorkDailys);

            // Kết quả thực hiện
            PdfPTable tblData2 = new PdfPTable(1) { TotalWidth = TABLE_WIDTH };
            tblData2.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData2.SetWidths(new float[] { 1 });
            tblData2.LockedWidth = true;
            tblData2.AddCell(GetCellSign(_fNomal, fSize, phaseC.WorkResult, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge3 = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge3.AddElement(tblData2);
            mainTable.AddCell(cellMerge3);
        }

        private void AddPdf881PhaseB(PdfPTable mainTable, Pdf881PhaseBModel phaseB)
        {
            PdfPTable tblData = new PdfPTable(1) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1 });
            tblData.LockedWidth = true;
            int fSize = 13;

            //Phần B: Cho phép công tác
            tblData.AddCell(GetCellSign(_fBold, fSize, phaseB.Title, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            // Danh sách các mục cho phép
            AppendAllowedInfosToPdf(mainTable, phaseB.AllowedInfos);

            PdfPTable tblSign = new PdfPTable(4) { TotalWidth = TABLE_WIDTH };
            tblSign.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSign.SetWidths(new float[] { 5, 1, 1, 3 });
            tblSign.LockedWidth = true;
            //Người cho phép
            tblSign.AddCell(GetCellSign(_fNomal, fSize, phaseB.AllowerName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            //Chữ ký
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (phaseB.AllowerSignature != null)
            {
                tblSign.AddCell(GetCellImageSignature(phaseB.AllowerSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM, 31, false));
            }
            else
            {
                tblSign.AddCell(GetCellSign(_fNomal, fSize, ".............", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }

            //Ngày giờ
            tblSign.AddCell(GetCellSign(_fNomal, fSize, phaseB.SignedTime1, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));

            //Người CHTT
            tblSign.AddCell(GetCellSign(_fNomal, fSize, phaseB.CommanderName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            //Chữ ký
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (phaseB.CommanderSignature != null)
            {
                tblSign.AddCell(GetCellImageSignature(phaseB.CommanderSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM, 31, false));
            }
            else
            {
                tblSign.AddCell(GetCellSign(_fNomal, fSize, ".............", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }

            //Ngày giờ
            tblSign.AddCell(GetCellSign(_fNomal, fSize, phaseB.SignedTime2, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));

            PdfPCell cellMerge2 = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge2.AddElement(tblSign);
            mainTable.AddCell(cellMerge2);
        }

        private void AddPdf881PhaseA(PdfPTable mainTable, Pdf881PhaseAModel phaseA, bool isSigned)
        {
            PdfPTable tblData = new PdfPTable(2) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            //Phần A: Cấp phiếu công tác
            tblData.AddCell(GetCellSign(_fBold, fSize, phaseA.Title, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Địa điểm
            tblData.AddCell(GetCellSign(_fNomal, fSize, phaseA.WorkLocaltion, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Nội dung:
            tblData.AddCell(GetCellSign(_fNomal, fSize, phaseA.WorkContent, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Phạm vi:
            tblData.AddCell(GetCellSign(_fNomal, fSize, phaseA.WorkScope, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Thời gian: Từ …… h …… ngày …… /…… /……… Đến …… h …… ngày ……… /……… / ………
            tblData.AddCell(GetCellSign(_fNomal, fSize, phaseA.StartTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, phaseA.EndTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            AppendWarnAndSafesToPdf(mainTable, phaseA.WarnAndSafes);

            PdfPTable tblSign = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblSign.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSign.SetWidths(new float[] { 10, 2, 2 });
            tblSign.LockedWidth = true;
            //Người cấp phiếu
            tblSign.AddCell(GetCellSign(_fNomal, fSize, phaseA.ApproverName, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Ngày cấp phiếu
            tblSign.AddCell(GetCellSign(_fNomal, fSize, phaseA.ApproveTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Chữ ký
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(phaseA.ApproverSignature))
            {
                _pdfInfo.SignatureX = 495;
                _pdfInfo.SignatureY = (int)mainTable.TotalHeight;
                _pdfInfo.ApproverSignature = phaseA.ApproverSignature;
                if (isSigned)
                {
                    tblSign.AddCell(GetCellImageSignature(phaseA.ApproverSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM, 25, false));
                }
                else
                {
                    // isSigned=false: tạo blank AcroField để SignPdfForTicket ký đè vào
                    tblSign.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Approver", 0, 0, 0, 0, fixHeight: 25));
                }
            }
            else
            {
                if (isSigned)
                {
                    tblSign.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
                }
                else
                {
                    tblSign.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Approver", 0, 0, 0, 0, fixHeight: 25));
                }
            }

            PdfPCell cellMerge2 = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge2.AddElement(tblSign);
            mainTable.AddCell(cellMerge2);
        }

        private void AddPdf881Header(PdfPTable mainTable, Pdf881HeaderModel header)
        {
            PdfPTable tblData = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 2, 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fNomal, fSize - 2, header.CompanyName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_TOP));
            tblData.AddCell(GetCellSign(_fBold, fSize + 3, "PHIẾU CÔNG TÁC", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize - 2, header.WorkTicketNumber, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_TOP));

            tblData.AddCell(GetCellSign(_fNomal, fSize - 2, header.ApprovedUnit, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_TOP));
            tblData.AddCell(GetCellSign(_fBold, fSize + 3, "", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize - 2, "", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_TOP));

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            PdfPTable tblTCNH = new PdfPTable(6) { TotalWidth = TABLE_WIDTH };
            tblTCNH.DefaultCell.Border = Rectangle.NO_BORDER;
            tblTCNH.SetWidths(new float[] { 2, 2, 2, 2, 2, 2 });
            tblTCNH.LockedWidth = true;

            tblTCNH.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblTCNH.AddCell(GetCellSign(_fNomal, fSize, header.Thuy, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblTCNH.AddCell(GetCellSign(_fNomal, fSize, header.Co, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblTCNH.AddCell(GetCellSign(_fNomal, fSize, header.Nhiet, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblTCNH.AddCell(GetCellSign(_fNomal, fSize, header.Hoa, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblTCNH.AddCell(GetCellSign(_fNomal, fSize, header.XayDung, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge2 = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge2.AddElement(tblTCNH);
            mainTable.AddCell(cellMerge2);
        }

        private void AppendWorkDailysToPdf(PdfPTable mainTable, List<WorkDailyDetailModel> workDailys)
        {
            // add header
            PdfPTable tblData = new PdfPTable(7) { TotalWidth = TABLE_WIDTH };
            tblData.SetWidths(new float[] { 1, 4, 2, 2, 3, 2, 3 });
            tblData.LockedWidth = true;
            int fSize = 11;

            // Header row.
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Stt", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Họ tên", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Số thẻ an toàn", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Vào vị trí làm việc", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Rút khỏi vị trí làm việc", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // Header row.
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Thời gian", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Chữ ký", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Thời gian", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Chữ ký", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            if (workDailys != null && workDailys.Count > 0)
            {
                foreach (var item in workDailys)
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.WorkDayText, 7, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                    if (item.StaffInformations != null && item.StaffInformations.Count > 0)
                    {
                        foreach (var u in item.StaffInformations)
                        {
                            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, u.No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 35));
                            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, u.FullName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, u.ElectricRankCard, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, u.CheckInTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            tblData.AddCell(GetCellImageSignature(u.CheckInSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 35));
                            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, u.CheckOutTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            tblData.AddCell(GetCellImageSignature(u.CheckOutSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 35));
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i < 4; i++)
                {
                    string No = i < 3 ? IntToRoman(i) : "X";
                    string workDayText = "(Vị trí/Ngày)";

                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, workDayText, 7, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

                    for (int j = 1; j < 11; j++)
                    {
                        string col2 = " ";
                        switch (j)
                        {
                            case 7:
                                col2 = "(Người LĐCV)";
                                break;
                            case 8:
                                col2 = "(Người CG)";
                                break;
                            case 9:
                                col2 = "(Người GSAT)";
                                break;
                            case 10:
                                col2 = "(Người CHTT)";
                                break;
                        }

                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, j < 4 ? j.ToString() : "...", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, col2, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    }
                }
            }

            tblData.SplitLate = false;
            PdfPCell cellMerge2 = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge2.AddElement(tblData);
            mainTable.AddCell(cellMerge2);
        }

        private void AppendSafetyMeasuresToPdf(PdfPTable mainTable, List<SafetyMeasureModel> safetyMeasures)
        {
            // add header
            PdfPTable tblData = new PdfPTable(4) { TotalWidth = TABLE_WIDTH };
            tblData.SetWidths(new float[] { 1, 7, 1.5f, 3.5f });
            tblData.LockedWidth = true;
            int fSize = 11;
            // Header row.
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Stt", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Kiểm tra các biện pháp an toàn đơn vị công tác thực hiện", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Đánh dấu", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Ghi chú", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // add content
            if (safetyMeasures != null && safetyMeasures.Count > 0)
            {
                foreach (var item in safetyMeasures)
                {
                    string strCheck = (item.IsChecked != null && (bool)item.IsChecked) ? "X" : "";
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Content, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, strCheck, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Note, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, i < 4 ? i.ToString() : "...", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendAllowedInfosToPdf(PdfPTable mainTable, List<AllowedInfoDetailModel> allowedInfos)
        {
            // add header
            PdfPTable tblData = new PdfPTable(4) { TotalWidth = TABLE_WIDTH };
            tblData.SetWidths(new float[] { 1, 7, 1.5f, 3.5f });
            tblData.LockedWidth = true;
            int fSize = 11;
            // Header row.
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Stt", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Kiểm tra các biện pháp an toàn đơn vị cho phép thực hiện", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Đánh dấu", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Ghi chú", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // add content
            if (allowedInfos != null && allowedInfos.Count > 0)
            {
                foreach (var item in allowedInfos)
                {
                    string strCheck = (item.IsChecked != null && (bool)item.IsChecked) ? "X" : "";
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Content, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, strCheck, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.Note, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, i < 4 ? i.ToString() : "...", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AppendWarnAndSafesToPdf(PdfPTable mainTable, List<WarnAndSafeModel> warnAndSafes)
        {
            // add header
            PdfPTable tblData = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblData.SetWidths(new float[] { 1, 6, 6 });
            tblData.LockedWidth = true;
            int fSize = 11;
            // Header row.
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Stt", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Nhận diện mối nguy", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Biện pháp an toàn", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // add content
            if (warnAndSafes != null && warnAndSafes.Count > 0)
            {
                foreach (var item in warnAndSafes)
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.WarningContent, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, item.SafetyMeasureContent, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, i < 4 ? i.ToString() : "...", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblData.AddCell(PdfHelper.GetCell(_fNomal, fSize, " ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
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

        private Pdf881ContentModel CreatePdf881Data(WorkTicketDetailModel workTicketDetail)
        {
            Pdf881ContentModel pdfContentModel = new Pdf881ContentModel
            {
                Header = GetPdf881Header(workTicketDetail),
                PhaseA = GetPdf881PhaseA(workTicketDetail),
                PhaseB = GetPdf881PhaseB(workTicketDetail),
                PhaseC = GetPdf881PhaseC(workTicketDetail),
                PhaseD = GetPdf881PhaseD(workTicketDetail)
            };

            return pdfContentModel;
        }

        private Pdf881HeaderModel GetPdf881Header(WorkTicketDetailModel workTicketDetail)
        {
            Pdf881HeaderModel pdfHeader = new Pdf881HeaderModel
            {
                Title = "PHIẾU CÔNG TÁC",
                ApprovedUnit = workTicketDetail.ApprovedUnit.Label,
                WorkTicketNumber = "Số phiếu: " + workTicketDetail.WorkTicketNumber,
            };

            for (int i = 0; i < workTicketDetail.WorkTicketFeatures.Count; i++)
            {
                WorkTicketFeaturesModel item = workTicketDetail.WorkTicketFeatures[i];
                if (item.FeatureID == (int)WorkTicketFeature.Thuy)
                {
                    pdfHeader.Thuy = "[x] Thủy";
                }
                else if (item.FeatureID == (int)WorkTicketFeature.Co)
                {
                    pdfHeader.Co = "[x] Cơ";
                }
                else if (item.FeatureID == (int)WorkTicketFeature.Nhiet)
                {
                    pdfHeader.Nhiet = "[x] Nhiệt";
                }
                else if (item.FeatureID == (int)WorkTicketFeature.Hoa)
                {
                    pdfHeader.Hoa = "[x] Hóa";
                }
                else if (item.FeatureID == (int)WorkTicketFeature.XayDung)
                {
                    pdfHeader.XayDung = "[x] Xây dựng";
                }
            }

            if (pdfHeader.Thuy == null)
                pdfHeader.Thuy = "[ ] Thủy";
            if (pdfHeader.Co == null)
                pdfHeader.Co = "[ ] Cơ";
            if (pdfHeader.Nhiet == null)
                pdfHeader.Nhiet = "[ ] Nhiệt";
            if (pdfHeader.Hoa == null)
                pdfHeader.Hoa = "[ ] Hóa";
            if (pdfHeader.XayDung == null)
                pdfHeader.XayDung = "[ ] Xây dựng";

            return pdfHeader;
        }

        private Pdf881PhaseAModel GetPdf881PhaseA(WorkTicketDetailModel workTicketDetail)
        {
            Pdf881PhaseAModel pdfPhaseA = new Pdf881PhaseAModel();

            string startTime = "Thời gian: Từ " +
                            workTicketDetail.EstimatedStartTime.Hour.ToString("00") + "h" +
                            workTicketDetail.EstimatedStartTime.Minute.ToString("00") + " ngày " +
                            workTicketDetail.EstimatedStartTime.Day.ToString() + "/" +
                            workTicketDetail.EstimatedStartTime.Month.ToString() + "/" +
                            workTicketDetail.EstimatedStartTime.Year.ToString();

            string endTime = "Đến " +
                            workTicketDetail.EstimatedEndTime.Hour.ToString("00") + "h" +
                            workTicketDetail.EstimatedEndTime.Minute.ToString("00") + " ngày " +
                            workTicketDetail.EstimatedEndTime.Day.ToString() + "/" +
                            workTicketDetail.EstimatedEndTime.Month.ToString() + "/" +
                            workTicketDetail.EstimatedEndTime.Year.ToString();

            string signTime = "Ngày .../.../... Giờ ...h...";
            if (workTicketDetail.SignDate.HasValue)
            {
                signTime = "Ngày " +
                            workTicketDetail.SignDate.Value.Day.ToString() + "/" +
                            workTicketDetail.SignDate.Value.Month.ToString() + "/" +
                            workTicketDetail.SignDate.Value.Year.ToString() + " Giờ " +
                            workTicketDetail.SignDate.Value.Hour.ToString("00") + "h" +
                            workTicketDetail.SignDate.Value.Minute.ToString("00");
            }

            pdfPhaseA.Title = "Phần A: Cấp phiếu công tác";
            pdfPhaseA.WorkLocaltion = !string.IsNullOrEmpty(workTicketDetail.WorkPlacement) ? "Địa điểm: " + workTicketDetail.WorkPlacement : "Địa điểm: .......................................................................................................................";
            pdfPhaseA.WorkContent = !string.IsNullOrEmpty(workTicketDetail.WorkContent) ? "Nội dung: " + workTicketDetail.WorkContent : "Nội dung: .......................................................................................................................";
            pdfPhaseA.WorkScope = !string.IsNullOrEmpty(workTicketDetail.WorkScope) ? "Phạm vi: " + workTicketDetail.WorkScope : "Phạm vi: .......................................................................................................................";
            pdfPhaseA.StartTime = startTime;
            pdfPhaseA.EndTime = endTime;
            pdfPhaseA.ApproverName = "Người cấp phiếu: " + workTicketDetail.ApproverUser.FullName;
            pdfPhaseA.ApproverSignature = workTicketDetail.ApproverSignature;
            pdfPhaseA.WarnAndSafes = GetPdf881WarnAndSafes(workTicketDetail.WarnAndSafes);
            pdfPhaseA.ApproveTime = signTime;

            return pdfPhaseA;
        }

        private Pdf881PhaseBModel GetPdf881PhaseB(WorkTicketDetailModel workTicketDetail)
        {
            Pdf881PhaseBModel pdfPhaseB = new Pdf881PhaseBModel
            {
                AllowerName = "Người cho phép: .............................................",
                CommanderName = "Người CHTT: .................................................",
                SignedTime1 = "Ngày ...\\...\\... Giờ ...h...",
                SignedTime2 = "Ngày ...\\...\\... Giờ ...h...",
                Title = "Phần B: Cho phép công tác"
            };

            if (workTicketDetail.AllowedInfo != null)
            {
                var data = workTicketDetail.AllowedInfo;

                string signTime1 = "Ngày " +
                                data.AllowerSigned.Day.ToString() + "/" +
                                data.AllowerSigned.Month.ToString() + "/" +
                                data.AllowerSigned.Year.ToString() + " Giờ " +
                                data.AllowerSigned.Hour.ToString("00") + "h" +
                                data.AllowerSigned.Minute.ToString("00");

                string signTime2 = "Ngày " +
                                data.CommanderSigned.Day.ToString() + "/" +
                                data.CommanderSigned.Month.ToString() + "/" +
                                data.CommanderSigned.Year.ToString() + " Giờ " +
                                data.CommanderSigned.Hour.ToString("00") + "h" +
                                data.CommanderSigned.Minute.ToString("00");

                if (data.Acceptor?.FullName != null) pdfPhaseB.AllowerName = "Người cho phép: " + data.Acceptor?.FullName;
                if (data.Commander?.FullName != null) pdfPhaseB.CommanderName = "Người CHTT: " + data.Commander?.FullName;
                pdfPhaseB.AllowerSignature = data.Acceptor?.Signature;
                pdfPhaseB.CommanderSignature = BytesToString(data.CommanderSignature?.SignatureContentByte);
                pdfPhaseB.AllowerSignature = BytesToString(data.AllowedSignature?.SignatureContentByte);
                pdfPhaseB.SignedTime1 = signTime1;
                pdfPhaseB.SignedTime2 = signTime2;
                pdfPhaseB.AllowedInfos = GetPdf881AllowedInfos(workTicketDetail.AllowedInfo?.AllowedInfoDetails);
            }

            return pdfPhaseB;
        }

        static string BytesToString(byte[] bytes)
        {
            return bytes != null ? Encoding.UTF8.GetString(bytes) : null;
        }

        private Pdf881PhaseCModel GetPdf881PhaseC(WorkTicketDetailModel workTicketDetail)
        {
            Pdf881PhaseCModel pdfPhaseC = new Pdf881PhaseCModel
            {
                Title = "Phần C: Thực hiện công tác",
                WorkUnit = "Đơn vị công tác: ...........................................................................",
                PersonNumber = "Số lượng người: ..........",
                WorkDailyTitle = "Danh sách Người tham gia thực hiện công tác đã biết, hiểu rõ (Phạm vi công việc; thiết bị cần sửa; an toàn cá nhân):",
                WorkResult = "Kết quả thực hiện: ........................................................................................................................"
            };
            if (workTicketDetail.ReceiveWorkplace != null)
            {
                var data = workTicketDetail.ReceiveWorkplace;
                var workUnit = string.Empty;
                if (!string.IsNullOrWhiteSpace(data.WorkUnitName))
                {
                    workUnit = "Đơn vị công tác: " + data.WorkUnitName;
                }
                else
                {
                    if (data.WorkUnitLv1.Value != 0 && data.WorkUnitLv2.Value != 0)
                    {
                        workUnit = $"Đơn vị công tác: {data.WorkUnitLv1.Label}>{data.WorkUnitLv2.Label}";
                    }

                    if (data.WorkUnitLv1.Value != 0 && data.WorkUnitLv2.Value == 0)
                    {
                        workUnit = $"Đơn vị công tác: {data.WorkUnitLv1.Label}";
                    }
                }
                pdfPhaseC.WorkUnit = workUnit;

                if (!string.IsNullOrEmpty(data.AmountPersons)) pdfPhaseC.PersonNumber = "Số lượng người: " + data.AmountPersons;
                if (!string.IsNullOrEmpty(workTicketDetail.WorkResult)) pdfPhaseC.WorkResult = "Kết quả thực hiện: " + workTicketDetail.WorkResult;
                pdfPhaseC.SafetyMeasures = GetPdf881SafetyMeasures(workTicketDetail.ReceiveWorkplace.SafetyMeasures);
                pdfPhaseC.WorkDailys = GetPdf881WorkDailys(workTicketDetail.ID);
            }

            return pdfPhaseC;
        }

        private Pdf881PhaseDModel GetPdf881PhaseD(WorkTicketDetailModel workTicketDetail)
        {
            Pdf881PhaseDModel pdfPhaseD = new Pdf881PhaseDModel
            {
                Title = "Phần D: Kết thúc công tác",
                Title1 = "Đơn vị công tác khẳng định:",
                Title2 = "Đơn vị vận hành khẳng định:",
                CommanderName = "Người CHTT: ......................................",
                AllowerName = "Người khóa phiếu: ....................................",
                SignedTime1 = "Ngày ...\\...\\... Giờ ...h...",
                SignedTime2 = "Ngày ...\\...\\... Giờ ...h...",
                WorkUnitConfirm1 = "[ ] Người, thiết bị, phương tiện đã rút hết",
                WorkUnitConfirm2 = "[ ] Thiết bị sẵn sàng đưa trở lại vận hành",
                WorkUnitConfirm3 = "[ ] Đã kết thúc công việc theo phiếu công tác này",
                WorkOperatorConfirm1 = "[ ] Mọi biển báo, cảnh báo an toàn đã gỡ hết",
                WorkOperatorConfirm2 = "[ ] Thiết bị đủ điều kiện đưa trở lại vận hành",
                WorkOperatorConfirm3 = "[ ] Khóa phiếu công tác này"
            };

            if (workTicketDetail.WorkFinish != null)
            {
                var data = workTicketDetail.WorkFinish;
                string signTime1 = "Ngày " +
                                (data.ReturnTime.HasValue ? data.ReturnTime.Value.Day.ToString() : "...") + "/" +
                                (data.ReturnTime.HasValue ? data.ReturnTime.Value.Month.ToString() : "...") + "/" +
                                (data.ReturnTime.HasValue ? data.ReturnTime.Value.Year.ToString() : "...") + " Giờ " +
                                (data.ReturnTime.HasValue ? data.ReturnTime.Value.Hour.ToString("00") : "...") + "h" +
                                (data.ReturnTime.HasValue ? data.ReturnTime.Value.Minute.ToString("00") : "...");

                string signTime2 = "Ngày " +
                                (data.CheckedTime.HasValue ? data.ReturnTime.Value.Day.ToString() : "...") + "/" +
                                (data.CheckedTime.HasValue ? data.ReturnTime.Value.Month.ToString() : "...") + "/" +
                                (data.CheckedTime.HasValue ? data.ReturnTime.Value.Year.ToString() : "...") + " Giờ " +
                                (data.CheckedTime.HasValue ? data.ReturnTime.Value.Hour.ToString("00") : "...") + "h" +
                                (data.CheckedTime.HasValue ? data.ReturnTime.Value.Minute.ToString("00") : "...");

                pdfPhaseD.CommanderName = "Người CHTT: " + data.CommanderUser?.FullName;
                pdfPhaseD.CommanderSignature = data.CommanderSignature?.SignatureContent;
                pdfPhaseD.AllowerName = "Người khóa phiếu: " + data.AllowedUser?.FullName;
                pdfPhaseD.AllowerSignature = data.AllowedSignature?.SignatureContent;
                pdfPhaseD.SignedTime1 = signTime1;
                pdfPhaseD.SignedTime2 = signTime2;

                foreach (var item in data.WorkFinishPictures)
                {
                    if (item.Position == 1 && (bool)item.IsChecked) pdfPhaseD.WorkUnitConfirm1 = "[x] Người, thiết bị, phương tiện đã rút hết";
                    if (item.Position == 2 && (bool)item.IsChecked) pdfPhaseD.WorkUnitConfirm2 = "[x] Thiết bị sẵn sàng đưa trở lại vận hành";
                    if (item.Position == 3 && (bool)item.IsChecked) pdfPhaseD.WorkUnitConfirm3 = "[x] Đã kết thúc công việc theo phiếu công tác này";
                }
                if ((bool)data.OperatorConfirmed1) pdfPhaseD.WorkOperatorConfirm1 = "[x]Mọi biển báo, cảnh báo an toàn đã gỡ hết";
                if ((bool)data.OperatorConfirmed2) pdfPhaseD.WorkOperatorConfirm2 = "[x]Mọi biển báo, cảnh báo an toàn đã gỡ hết";
                if ((bool)data.OperatorConfirmed3) pdfPhaseD.WorkOperatorConfirm3 = "[x]Khóa phiếu công tác này";
            }

            return pdfPhaseD;
        }

        private List<WarnAndSafeModel> GetPdf881WarnAndSafes(List<WarnAndSafeModel> warnAndSafes)
        {
            List<WarnAndSafeModel> lst = new List<WarnAndSafeModel>();
            if (warnAndSafes != null && warnAndSafes.Count > 0)
            {
                int index = 1;
                foreach (var item in warnAndSafes)
                {
                    WarnAndSafeModel newItem = new WarnAndSafeModel()
                    {
                        No = index.ToString(),
                        SafetyMeasureContent = item.SafetyMeasureContent,
                        WarningContent = item.WarningContent,
                    };
                    lst.Add(newItem);
                    index += 1;
                }
            }

            return lst;
        }

        private List<AllowedInfoDetailModel> GetPdf881AllowedInfos(List<AllowedInfoDetailModel> allowedInfos)
        {
            List<AllowedInfoDetailModel> lst = new List<AllowedInfoDetailModel>();
            if (allowedInfos != null && allowedInfos.Count > 0)
            {
                int index = 1;
                foreach (var item in allowedInfos)
                {
                    AllowedInfoDetailModel newItem = new AllowedInfoDetailModel()
                    {
                        No = index.ToString(),
                        Note = item.Note,
                        IsChecked = item.IsChecked != null ? item.IsChecked : false,
                        Content = item.Content
                    };
                    lst.Add(newItem);
                    index += 1;
                }
            }

            return lst;
        }

        private List<SafetyMeasureModel> GetPdf881SafetyMeasures(List<SafetyMeasureModel> safetyMeasures)
        {
            List<SafetyMeasureModel> lst = new List<SafetyMeasureModel>();
            if (safetyMeasures != null && safetyMeasures.Count > 0)
            {
                int index = 1;
                foreach (var item in safetyMeasures)
                {
                    SafetyMeasureModel newItem = new SafetyMeasureModel()
                    {
                        No = index.ToString(),
                        Note = item.Note,
                        IsChecked = item.IsChecked,
                        Content = item.Content
                    };
                    lst.Add(newItem);
                    index += 1;
                }
            }

            return lst;
        }

        private List<WorkDailyDetailModel> GetPdf881WorkDailys(int workTicketID)
        {
            var entities = _unitOfWork.ElogbookRepository<WorkDaily>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted);
            var data = entities.Select(e => new WorkDailyDetailModel()
            {
                WorkLocationText = e.WorkLocation != null ? e.WorkLocation.Position : e.WorkLocationText,
                WorkDayTemp = e.WorkDay,
                WorkDay = e.WorkDay,
                StaffInformations = e.StaffInformations.Where(x => !x.IsDeleted).Select(t => new StaffInformationsModel()
                {
                    FullName = t.FullName,
                    PositionName = t.PositionName,
                    ElectricRankCard = t.ElectricRankCard,
                    CheckInTime = t.CheckInTime,
                    CheckInSignatureID = t.CheckInSignature,
                    CheckOutTime = t.CheckOutTime,
                    CheckOutSignatureID = t.CheckOutSignature,
                    IsExternal = t.IsExternal,
                }).ToList(),
            }).OrderBy(t => t.WorkDay).ToList();

            List<WorkDailyDetailModel> lst = new List<WorkDailyDetailModel>();
            if (data.Count > 0)
            {
                int index = 1;
                foreach (var item in data)
                {
                    WorkDailyDetailModel workDailyDetailModel = new WorkDailyDetailModel()
                    {
                        No = IntToRoman(index),
                        WorkDayText = item.WorkLocationText + " - " + item.WorkDayText,
                    };
                    if (item.StaffInformations != null && item.StaffInformations.Count > 0)
                    {
                        List<StaffInformationsModel> staffInfoModels = new List<StaffInformationsModel>();
                        int index1 = 1;
                        foreach (var user in item.StaffInformations)
                        {
                            StaffInformationsModel staffInfo = new StaffInformationsModel()
                            {
                                No = index1.ToString(),
                                FullName = user.FullName,
                                ElectricRankCard = user.ElectricRankCard,
                                CheckInTime = user.CheckInTime,
                                CheckInSignature = user.CheckInSignatureID != null ? _commonService.GetSignatureById((int)user.CheckInSignatureID).SignatureContent : null,
                                CheckOutTime = user.CheckOutTime,
                                CheckOutSignature = user.CheckOutSignatureID != null ? _commonService.GetSignatureById((int)user.CheckOutSignatureID).SignatureContent : null,
                            };
                            staffInfoModels.Add(staffInfo);
                            index1 += 1;
                        }
                        workDailyDetailModel.StaffInformations = staffInfoModels;
                    }
                    lst.Add(workDailyDetailModel);
                    index += 1;
                }
            }

            return lst;
        }

        private string IntToRoman(int n)
        {
            string[] roman_symbol = { "MMM", "MM", "M", "CM", "DCCC", "DCC", "DC", "D", "CD", "CCC", "CC", "C", "XC", "LXXX", "LXX", "LX", "L", "XL", "XXX", "XX", "X", "IX", "VIII", "VII", "VI", "V", "IV", "III", "II", "I" };
            int[] int_value = { 3000, 2000, 1000, 900, 800, 700, 600, 500, 400, 300, 200, 100, 90, 80, 70, 60, 50, 40, 30, 20, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

            var roman_numerals = new StringBuilder();
            var index_num = 0;
            while (n != 0)
            {
                if (n >= int_value[index_num])
                {
                    n -= int_value[index_num];
                    roman_numerals.Append(roman_symbol[index_num]);
                }
                else
                {
                    index_num++;
                }
            }

            return roman_numerals.ToString();
        }

        private PdfWorkTicketModel CreatePdfData(WorkTicketDetailModel detailData)
        {
            PdfWorkTicketModel pdfWorkTicket = new PdfWorkTicketModel
            {
                PdfHeaderWorkTicket = CreatePdfHeader(detailData.Factory.Value, detailData.ApprovedUnit.Label, detailData.WorkTicketNumber),
                PdfApproveInfo = GetPdfApproveInfo(detailData),
                PdfAllowedInfo = GetPdfAllowedInfo(detailData),
                PdfReceiveWorkplace = GetPdfReceiveWorkplace(detailData),
                PdfStaffInfos = GetPdfStaffInfos(detailData.ID),
                PdfWorkPlacement = GetPdfWorkPlacements(detailData.ID),
                PdfWorkFinish = GetPdfWorkFinish(detailData),
                WorkResult = detailData.WorkResult,
            };

            return pdfWorkTicket;
        }

        private PdfHeaderWorkTicketModel CreatePdfHeader(int? factoryID, string approvedUnitName, string workTicketNumber)
        {
            PdfHeaderWorkTicketModel pdfHeaderWorkTicketModel = new PdfHeaderWorkTicketModel
            {
                CompanyName = " ",
                ApprovedUnit = approvedUnitName,
                Title = "PHIẾU CÔNG TÁC",
                WorkTicketNumber = "Số: " + workTicketNumber
            };

            if (factoryID != null && factoryID > 0)
            {
                pdfHeaderWorkTicketModel.CompanyName = GetCompanyName((int)factoryID);
            }

            return pdfHeaderWorkTicketModel;
        }

        private PdfApproveInfoModel GetPdfApproveInfo(WorkTicketDetailModel detailData)
        {
            string startTime = "Bắt đầu công việc: " + detailData.EstimatedStartTime.Hour.ToString("00") + " giờ " + detailData.EstimatedStartTime.Minute.ToString("00") + " phút," + " ngày " + detailData.EstimatedStartTime.Day.ToString() + " tháng " + detailData.EstimatedStartTime.Month.ToString() + " năm " + detailData.EstimatedStartTime.Year.ToString();
            string endTime = "Kết thúc công việc: " + detailData.EstimatedEndTime.Hour.ToString("00") + " giờ " + detailData.EstimatedEndTime.Minute.ToString("00") + " phút," + " ngày " + detailData.EstimatedEndTime.Day.ToString() + " tháng " + detailData.EstimatedEndTime.Month.ToString() + " năm " + detailData.EstimatedEndTime.Year.ToString();
            string approveTime = "Phiếu công tác cấp ngày "
                + (detailData.SignDate.HasValue ? detailData.SignDate.Value.Day.ToString() : string.Empty) + " tháng "
                + (detailData.SignDate.HasValue ? detailData.SignDate.Value.Month.ToString() : string.Empty) + " năm "
                + (detailData.SignDate.HasValue ? detailData.SignDate.Value.Year.ToString() : string.Empty);

            PdfApproveInfoModel pdfApproveInfoModel = new PdfApproveInfoModel
            {
                Title = "1. Cấp cho",
                WorkLeader = "1.1. Người lãnh đạo công việc(nếu có): " + detailData.LeaderUser?.FullName,
                WorkCommander = "1.2. Người chỉ huy trực tiếp: " + detailData.CommanderUser?.FullName,
                UnitNumber = "1.3. Nhân viên đơn vị công tác: " + (detailData.UnitNumber.GetValueOrDefault(0) != 0 ? detailData.UnitNumber.ToString() : string.Empty),
                WorkPlacement = "1.4. Địa điểm công tác: " + detailData.WorkPlacement,
                WorkContent = "1.5. Nội dung công tác: " + detailData.WorkContent,
                TitleTime = "1.6. Thời gian theo kế hoạch: ",
                EstimatedStartTime = startTime,
                EstimatedEndTime = endTime,
                WorkCondition = "1.7. Điều kiện để tiến hành công việc (ghi rõ cắt điện một phần hay hoàn toàn thiết bị, đường dây, đoạn đường dây): " + detailData.WorkCondition,
                ApprovedDate = approveTime,
                ApproverUser = "Người cấp phiếu: " + detailData.ApproverUser?.FullName,
                ApproverPosition = "Chức vụ: " + detailData.ApproverUser?.PositionName,
                ApproverSignature = detailData.ApproverSignature
            };
            if (string.IsNullOrEmpty(detailData.LeaderUser?.FullName))
            {
                pdfApproveInfoModel.WorkLeader = "1.1. Người lãnh đạo công việc(nếu có): ......................................................................................";
            }

            return pdfApproveInfoModel;
        }

        private PdfAllowedInfoModel GetPdfAllowedInfo(WorkTicketDetailModel detailData)
        {
            _ = new PdfAllowedInfoModel();
            PdfAllowedInfoModel pdfAllowedInfoModel;
            if (detailData.AllowedInfo != null)
            {
                string DeviceDisconnected = string.Empty;
                string GroundedAtLocations = string.Empty;
                string BarriersAndWarningPoster = string.Empty;
                string AllowableWorkingRange = string.Empty;
                string WarningsAndInstructions = string.Empty;

                foreach (var item in detailData.AllowedInfo.AllowedInfoDetails)
                {
                    switch (item.Position)
                    {
                        case 1:
                            DeviceDisconnected = item.Content;
                            break;
                        case 2:
                            GroundedAtLocations = item.Content;
                            break;
                        case 3:
                            BarriersAndWarningPoster = item.Content;
                            break;
                        case 4:
                            AllowableWorkingRange = item.Content;
                            break;
                        case 5:
                            WarningsAndInstructions = item.Content;
                            break;
                    }
                }
                string AllowedTime = "2.6. Cho phép Đơn vị công tác bắt đầu làm việc lúc " + detailData.AllowedInfo.AllowerSigned.Hour.ToString() + " giờ " + detailData.AllowedInfo.AllowerSigned.Minute.ToString() + " phút," + " ngày " + detailData.AllowedInfo.AllowerSigned.Day.ToString() + " tháng " + detailData.AllowedInfo.AllowerSigned.Month.ToString() + " năm " + detailData.AllowedInfo.AllowerSigned.Year.ToString();
                pdfAllowedInfoModel = new PdfAllowedInfoModel()
                {
                    Title = "2. Thủ tục cho phép công tác",
                    DeviceDisconnected = "2.1. Những thiết bị, đường dây, đoạn đường dây đã cắt điện: " + DeviceDisconnected,
                    GroundedAtLocations = "2.2. Đã tiếp đất tại các vị trí: " + GroundedAtLocations,
                    BarriersAndWarningPoster = "2.3. Đã làm rào chắn và treo biển báo tại: " + BarriersAndWarningPoster,
                    AllowableWorkingRange = "2.4. Phạm vi được phép làm việc: " + AllowableWorkingRange,
                    WarningsAndInstructions = "2.5. Cảnh báo, chỉ dẫn cần thiết: " + WarningsAndInstructions,
                    AllowedTime = AllowedTime,
                    AllowerName = "Người cho phép: " + detailData.AllowedInfo.Acceptor?.FullName,
                    AllowerPosition = "Chức vụ: " + detailData.AllowedInfo.Acceptor?.PositionName,
                    AllowerSignature = detailData.AllowedInfo.AllowedSignature?.SignatureContent,
                };
            }
            else
            {
                pdfAllowedInfoModel = new PdfAllowedInfoModel()
                {
                    Title = "2. Thủ tục cho phép công tác",
                    DeviceDisconnected = "2.1. Những thiết bị, đường dây, đoạn đường dây đã cắt điện: ....................................................",
                    GroundedAtLocations = "2.2. Đã tiếp đất tại các vị trí: .......................................................................................................",
                    BarriersAndWarningPoster = "2.3. Đã làm rào chắn và treo biển báo tại: ...................................................................................",
                    AllowableWorkingRange = "2.4. Phạm vi được phép làm việc: ...............................................................................................",
                    WarningsAndInstructions = "2.5. Cảnh báo, chỉ dẫn cần thiết: .................................................................................................",
                    AllowedTime = "2.6. Cho phép Đơn vị công tác bắt đầu làm việc lúc .... giờ .... phút, ngày .... tháng .... năm ....",
                    AllowerName = "Người cho phép: ...................................................................................................................",
                    AllowerPosition = "Chức vụ: ...................................................................................",
                };
            }

            return pdfAllowedInfoModel;
        }

        private PdfReceiveWorkplaceModel GetPdfReceiveWorkplace(WorkTicketDetailModel detailData)
        {
            _ = new PdfReceiveWorkplaceModel();
            PdfReceiveWorkplaceModel pdfReceiveWorkplaceModel;
            if (detailData.ReceiveWorkplace != null)
            {
                string CheckedSafetyMeasures = string.Empty;
                string AddedSafetyMeasures = string.Empty;
                foreach (var item in detailData.ReceiveWorkplace.SafetyMeasures)
                {
                    switch (item.Position)
                    {
                        case 1:
                            CheckedSafetyMeasures = item.Content;
                            break;
                        case 2:
                            AddedSafetyMeasures = item.Content;
                            break;
                    }
                }
                string StartWorkingTime = "Bắt đầu tiến hành công việc lúc " + detailData.ReceiveWorkplace.StartTime.Hour.ToString("00") + " giờ " + detailData.ReceiveWorkplace.StartTime.Minute.ToString("00") + " phút," + " ngày " + detailData.ReceiveWorkplace.StartTime.Day.ToString() + " tháng " + detailData.ReceiveWorkplace.StartTime.Month.ToString() + " năm " + detailData.ReceiveWorkplace.StartTime.Year.ToString();
                pdfReceiveWorkplaceModel = new PdfReceiveWorkplaceModel()
                {
                    Title = "3. Tiếp nhận nơi làm việc",
                    CheckedSafetyMeasures = "3.1. Đã kiểm tra những biện pháp an toàn tại hiện trường: " + CheckedSafetyMeasures,
                    AddedSafetyMeasures = "3.2. Đã làm thêm các biện pháp an toàn và tiếp đất tại: " + AddedSafetyMeasures,
                    StartWorkingTime = StartWorkingTime,
                    LeaderName = "Người lãnh đạo công việc: " + detailData.ReceiveWorkplace.LeaderUser?.FullName,
                    LeaderPosition = "Chức vụ: " + detailData.ReceiveWorkplace.LeaderUser?.PositionName,
                    LeaderSignature = detailData.ReceiveWorkplace.LeaderSignature?.SignatureContent,
                    CommanderName = "Người chỉ huy trực tiếp: " + detailData.ReceiveWorkplace.CommanderUser?.FullName,
                    CommanderPosition = "Chức vụ: " + detailData.ReceiveWorkplace.CommanderUser?.PositionName,
                    CommanderSignature = detailData.ReceiveWorkplace.CommanderSignature?.SignatureContent,
                    SupervisorName = "Người giám sát an toàn điện: " + detailData.ReceiveWorkplace.SupervisorUser?.FullName,
                    SupervisorPosition = "Chức vụ: " + detailData.ReceiveWorkplace.SupervisorUser?.PositionName,
                    SupervisorSignature = detailData.ReceiveWorkplace.SupervisorSignature?.SignatureContent,
                };
            }
            else
            {
                pdfReceiveWorkplaceModel = new PdfReceiveWorkplaceModel()
                {
                    Title = "3. Tiếp nhận nơi làm việc",
                    CheckedSafetyMeasures = "3.1. Đã kiểm tra những biện pháp an toàn tại hiện trường: ..........................................................",
                    AddedSafetyMeasures = "3.2. Đã làm thêm các biện pháp an toàn và tiếp đất tại: ...............................................................",
                    StartWorkingTime = "Bắt đầu tiến hành công việc lúc ... giờ ... phút, ngày ... tháng  ... năm ...",
                    LeaderName = "Người lãnh đạo công việc (nếu có): ......................................................................................",
                    LeaderPosition = "Chức vụ: ...................................................................................",
                    CommanderName = "Người chỉ huy trực tiếp: ........................................................................................................",
                    CommanderPosition = "Chức vụ: ...................................................................................",
                    SupervisorName = "Người giám sát an toàn điện (nếu có): ..................................................................................",
                    SupervisorPosition = "Chức vụ: ...................................................................................",
                };
            }

            return pdfReceiveWorkplaceModel;
        }

        private List<PdfStaffInfoModel> GetPdfStaffInfos(int workTicketID)
        {
            var entities = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted).ToList();
            List<PdfStaffInfoModel> pdfStaffInfos = new List<PdfStaffInfoModel>();
            if (entities.Count > 0)
            {
                int index = 1;
                foreach (var item in entities)
                {
                    PdfStaffInfoModel pdfStaffInfoModel = new PdfStaffInfoModel()
                    {
                        No = index.ToString(),
                        FullName = item.FullName,
                        CheckInDateTime = item.CheckInTime,
                        CheckInSignature = item.CheckInSignature != null ? _commonService.GetSignatureById((int)item.CheckInSignature).SignatureContent : null,
                        CheckOutDateTime = item.CheckOutTime,
                        CheckOutSignature = item.CheckOutSignature != null ? _commonService.GetSignatureById((int)item.CheckOutSignature).SignatureContent : null,
                    };
                    pdfStaffInfos.Add(pdfStaffInfoModel);
                    index += 1;
                }
            }
            else
            {
                pdfStaffInfos.Add(StaffInfoPdfEntity("1"));
                pdfStaffInfos.Add(StaffInfoPdfEntity(" "));
            }

            return pdfStaffInfos;
        }

        private List<PdfWorkPlacementModel> GetPdfWorkPlacements(int workTicketID)
        {
            var entities = _unitOfWork.ElogbookRepository<WorkPlacement>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted).ToList();
            List<PdfWorkPlacementModel> pdfWorkPlacements = new List<PdfWorkPlacementModel>();
            if (entities.Count > 0)
            {
                int index = 1;
                foreach (var item in entities)
                {
                    PdfWorkPlacementModel pdfWorkPlacementModel = new PdfWorkPlacementModel()
                    {
                        No = index.ToString(),
                        Place = item.Place,
                        StartTime = item.StartTime.HasValue ? item.StartTime.Value.ToString("HHH:mm dd/MM/yyyy") : null,
                        EndTime = item.EndTime.HasValue ? item.EndTime.Value.ToString("HHH:mm dd/MM/yyyy") : null,
                        CommanderName = item.CommanderName,
                        CommanderSignature = item.CommanderSignature != null ? _commonService.GetSignatureById((int)item.CommanderSignature).SignatureContent : null,
                        AllowerName = item.AcceptorName,
                        AllowerSignature = item.AcceptorSignature != null ? _commonService.GetSignatureById((int)item.AcceptorSignature).SignatureContent : null,
                    };
                    pdfWorkPlacements.Add(pdfWorkPlacementModel);
                    index += 1;
                }
            }
            else
            {
                pdfWorkPlacements.Add(WorkPlacementPdfModel("1"));
                pdfWorkPlacements.Add(WorkPlacementPdfModel(" "));
                pdfWorkPlacements.Add(WorkPlacementPdfModel(" "));
                pdfWorkPlacements.Add(WorkPlacementPdfModel(" "));
            }

            return pdfWorkPlacements;
        }

        private PdfWorkFinishModel GetPdfWorkFinish(WorkTicketDetailModel detailData)
        {
            _ = new PdfWorkFinishModel();
            PdfWorkFinishModel pdfWorkFinishModel;
            if (detailData.WorkFinish != null)
            {

                var userRepresentativeUnitFullName = detailData.WorkFinish.RepresentativeUnitUser != null ? detailData.WorkFinish.RepresentativeUnitUser.FullName : detailData.WorkFinish.RepresentativeUnit;
                var userPositionName = detailData.WorkFinish.RepresentativeUnitUser != null ? detailData.WorkFinish.RepresentativeUnitUser?.PositionName : detailData.WorkFinish.RepresentativeUnitPositionName;

                string ReturnWorkplaceTime = (detailData.WorkFinish.ReturnTime.HasValue ? detailData.WorkFinish.ReturnTime.Value.Hour.ToString("00") : "...") +
                    " giờ " + (detailData.WorkFinish.ReturnTime.HasValue ? detailData.WorkFinish.ReturnTime.Value.Minute.ToString("00") : "...") + " phút," +
                    " ngày " + (detailData.WorkFinish.ReturnTime.HasValue ? detailData.WorkFinish.ReturnTime.Value.Day.ToString() : "...") +
                    " tháng " + (detailData.WorkFinish.ReturnTime.HasValue ? detailData.WorkFinish.ReturnTime.Value.Month.ToString() : "...") +
                    " năm " + (detailData.WorkFinish.ReturnTime.HasValue ? detailData.WorkFinish.ReturnTime.Value.Year.ToString() : "...");

                string content = "6.1. Toàn bộ công tác đã kết thúc, dụng cụ đã thu dọn, người, tiếp đất và biện pháp an toàn do Đơn vị công tác làm đã rút hết bảo đảm an toàn đóng điện. Người chỉ huy trực tiếp trả lại nơi làm việc cho ông (bà): "
                                + userRepresentativeUnitFullName
                                + ". Chức vụ: " + userPositionName + " đại diện đơn vị quản lý lúc " + ReturnWorkplaceTime;

                string CheckReceivedContent = "6.2. Đã tiếp nhận và kiểm tra nơi làm việc, phiếu công tác đã khóa lúc " +
                    (detailData.WorkFinish.CheckedTime.HasValue ? detailData.WorkFinish.CheckedTime.Value.Hour.ToString("00") : "...") +
                    " giờ " + (detailData.WorkFinish.CheckedTime.HasValue ? detailData.WorkFinish.CheckedTime.Value.Minute.ToString("00") : "...") + " phút," +
                    " ngày " + (detailData.WorkFinish.CheckedTime.HasValue ? detailData.WorkFinish.CheckedTime.Value.Day.ToString() : "...") +
                    " tháng " + (detailData.WorkFinish.CheckedTime.HasValue ? detailData.WorkFinish.CheckedTime.Value.Month.ToString() : "...") +
                    " năm " + (detailData.WorkFinish.CheckedTime.HasValue ? detailData.WorkFinish.CheckedTime.Value.Year.ToString() : "...");

                string CheckCompletedContent = "Đã kiểm tra hoàn thành Phiếu công tác ngày " +
                    (detailData.WorkFinish.CompletedTime.HasValue ? detailData.WorkFinish.CompletedTime.Value.Day.ToString() : "...") +
                    " tháng " + (detailData.WorkFinish.CompletedTime.HasValue ? detailData.WorkFinish.CompletedTime.Value.Month.ToString() : "...") +
                    " năm " + (detailData.WorkFinish.CompletedTime.HasValue ? detailData.WorkFinish.CompletedTime.Value.Year.ToString() : "...");

                pdfWorkFinishModel = new PdfWorkFinishModel()
                {
                    Title = "6. Kết thúc công tác",
                    TitleContent = content,
                    CommanderName = "Người chỉ huy trực tiếp: " + detailData.WorkFinish.CommanderUser?.FullName,
                    CommanderSignature = detailData.WorkFinish.CommanderSignature?.SignatureContent,
                    LeaderName = "Người lãnh đạo công việc: " + detailData.WorkFinish.LeaderUser?.FullName,
                    LeaderSignature = detailData.WorkFinish.LeaderSignature?.SignatureContent,
                    CheckReceivedContent = CheckReceivedContent,
                    AllowerName = "Người cho phép: " + detailData.AllowedInfo.Acceptor?.FullName,
                    AllowerSignature = detailData.AllowedInfo.AllowedSignature?.SignatureContent,
                    CheckCompletedContent = CheckCompletedContent,
                    CompleteConfirmName = "Người xác nhận hoàn thành: " + detailData.CompleteConfirmUser?.FullName,
                    CompleteConfirmSignature = detailData.CompleteConfirmUser?.Signature,
                };
            }
            else
            {
                pdfWorkFinishModel = new PdfWorkFinishModel()
                {
                    Title = "6. Kết thúc công tác",
                    TitleContent = "6.1.Toàn bộ công tác đã kết thúc, dụng cụ đã thu dọn, người, tiếp đất và biện pháp an toàn do Đơn vị công tác làm đã rút hết bảo đảm an toàn đóng điện. Người chỉ huy trực tiếp trả lại nơi làm việc cho ông(bà) ... chức vụ ... đại diện đơn vị quản lý lúc ... giờ ... phút, ngày ... tháng ... năm ... ",
                    CommanderName = "Người chỉ huy trực tiếp (ký): ............................................................",
                    LeaderName = "Người lãnh đạo công việc (ký - nếu có): ..........................................",
                    CheckReceivedContent = "6.2. Đã tiếp nhận và kiểm tra nơi làm việc, phiếu công tác đã khóa lúc ... giờ ... phút, ngày ... tháng ... năm ...",
                    AllowerName = "Người cho phép (ký và ghi họ tên): ..................................................",
                    CheckCompletedContent = "Đã kiểm tra hoàn thành Phiếu công tác ngày ... tháng ... năm ...",
                    CompleteConfirmName = "Người xác nhận hoàn thành (ký và ghi họ tên): .......................................",
                };
            }

            return pdfWorkFinishModel;
        }

        private PdfInfoModel GeneratePdfFile(PdfWorkTicketModel pdfDataModel, string outputFile, bool isSigned)
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

                        // ------------PdfHeaderWorkTicket---------------//
                        AddHeaderToPdf(tblData, pdfDataModel.PdfHeaderWorkTicket);

                        // ------------PdfApproveInfo (Cấp cho)---------------//
                        AddPdfApproveInfo(tblData, pdfDataModel.PdfApproveInfo, isSigned);

                        // ------------PdfAllowedInfo (Thủ tục cho phép công tác)---------------//
                        AddPdfAllowedInfo(tblData, pdfDataModel.PdfAllowedInfo);

                        // ------------PdfReceiveWorkplaceModel (Tiếp nhận nơi làm việc)---------------//
                        AddPdfReceiveWorkplace(tblData, pdfDataModel.PdfReceiveWorkplace);

                        // ------------PdfStaffInfoModel (Danh sách Nhân viên đơn vị công tác)---------------//
                        AddPdfStaffInfos(tblData, pdfDataModel.PdfStaffInfos);

                        // ------------PdfWorkPlacementModel (Cho phép làm việc và kết thúc công tác hằng ngày, di chuyên nơi làm việc)---------------//
                        AddPdfWorkPlacement(tblData, pdfDataModel.PdfWorkPlacement);
                        //Kết quả thực hiện
                        int fSize = 13;
                        if (!string.IsNullOrEmpty(pdfDataModel.WorkResult))
                        {
                            tblData.AddCell(GetCellSign(_fNomal, fSize, $"Kết quả thực hiện: {pdfDataModel.WorkResult}", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        }
                        else
                        {
                            tblData.AddCell(GetCellSign(_fNomal, fSize, "Kết quả thực hiện: ........................................................................................................................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                        }

                        // ------------PdfWorkFinishModel (Kết thúc công tác)---------------//
                        AddPdfWorkFinish(tblData, pdfDataModel.PdfWorkFinish);

                        pdfDocument.Add(tblData);
                        pdfDocument.Close();
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                        posY = 815;
                        _pdfInfo.Status = true;

                        return _pdfInfo;
                    }
                    finally
                    {
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                        posY = 815;
                    }
                }
            }
            catch
            {
                _pdfInfo.Status = false;
            }

            return _pdfInfo;
        }

        private void AddPdfWorkFinish(PdfPTable mainTable, PdfWorkFinishModel pdfWorkFinish)
        {
            PdfPTable tblData = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 10, 2, 2 });
            tblData.LockedWidth = true;
            int fSize = 13;

            // 6. Kết thúc công tác
            tblData.AddCell(GetCellSign(_fBold, fSize + 1, pdfWorkFinish.Title, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // 6.1
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfWorkFinish.TitleContent, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // Tên Người chỉ huy trực tiếp
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfWorkFinish.CommanderName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chữ ký người chỉ huy trực tiếp
            tblData.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfWorkFinish.CommanderSignature))
            {
                tblData.AddCell(GetCellImageSignature(pdfWorkFinish.CommanderSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }

            // Tên người lãnh đạo công việc
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfWorkFinish.LeaderName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chữ ký người lãnh đạo công việc
            tblData.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfWorkFinish.LeaderSignature))
            {
                tblData.AddCell(GetCellImageSignature(pdfWorkFinish.LeaderSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }

            // 6.2. Đã tiếp nhận và kiểm tra nơi làm việc, phiếu công tác đã khóa lúc ... giờ ... phút, ngày ... tháng ... năm ...
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfWorkFinish.CheckReceivedContent, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // Tên người cho phép
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfWorkFinish.AllowerName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chữ ký người cho phép
            tblData.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfWorkFinish.AllowerSignature))
            {
                tblData.AddCell(GetCellImageSignature(pdfWorkFinish.AllowerSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblData.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }

            // Đã kiểm tra hoàn thành Phiếu công tác ngày ... tháng ... năm ...
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfWorkFinish.CheckCompletedContent, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Tên người xác nhận hoàn thành
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfWorkFinish.CompleteConfirmName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chữ ký người xác nhận hoàn thành
            tblData.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfWorkFinish.CompleteConfirmSignature))
            {
                tblData.AddCell(GetCellImageSignature(pdfWorkFinish.CompleteConfirmSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblData.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Completer", 0, 0, 0, 0, fixHeight: 31));
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
        }

        private void AddPdfReceiveWorkplace(PdfPTable mainTable, PdfReceiveWorkplaceModel pdfReceiveWorkplace)
        {
            int fSize = 13;

            // 3. Tiếp nhận nơi làm việc
            mainTable.AddCell(GetCellSign(_fBold, fSize + 1, pdfReceiveWorkplace.Title, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //3.1. Đã kiểm tra những biện pháp an toàn tại hiện trường:
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfReceiveWorkplace.CheckedSafetyMeasures, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // 3.2. Đã làm thêm các biện pháp an toàn và tiếp đất tại:
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfReceiveWorkplace.AddedSafetyMeasures, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            // Bắt đầu tiến hành công việc lúc:
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfReceiveWorkplace.StartWorkingTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPTable tblSign = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblSign.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSign.SetWidths(new float[] { 10, 2, 2 });
            tblSign.LockedWidth = true;

            // Tên người lãnh đạo
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfReceiveWorkplace.LeaderName, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chức vụ người lãnh đạo
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfReceiveWorkplace.LeaderPosition, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chữ ký người lãnh đạo
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfReceiveWorkplace.LeaderSignature))
            {
                tblSign.AddCell(GetCellImageSignature(pdfReceiveWorkplace.LeaderSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblSign.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }

            // Tên người chỉ huy trực tiếp
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfReceiveWorkplace.CommanderName, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chức vụ người chỉ huy trực tiếp
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfReceiveWorkplace.CommanderPosition, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chữ ký người chỉ huy trực tiếp
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfReceiveWorkplace.CommanderSignature))
            {
                tblSign.AddCell(GetCellImageSignature(pdfReceiveWorkplace.CommanderSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblSign.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }

            // Tên người giám sát an toàn điện
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfReceiveWorkplace.SupervisorName, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            // Chức vụ người giám sát an toàn điện
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfReceiveWorkplace.SupervisorPosition, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));

            // Chữ ký người giám sát an toàn điện
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfReceiveWorkplace.SupervisorSignature))
            {
                tblSign.AddCell(GetCellImageSignature(pdfReceiveWorkplace.SupervisorSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblSign.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
            };
            cellMerge.AddElement(tblSign);
            mainTable.AddCell(cellMerge);
        }

        private void AddPdfAllowedInfo(PdfPTable mainTable, PdfAllowedInfoModel pdfAllowedInfo)
        {
            PdfPTable tblData = new PdfPTable(1) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1 });
            tblData.LockedWidth = true;

            int fSize = 13;

            //2. Thủ tục cho phép công tác
            tblData.AddCell(GetCellSign(_fBold, fSize + 1, pdfAllowedInfo.Title, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //2.1. Những thiết bị, đường dây, đoạn đường dây đã cắt điện
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfAllowedInfo.DeviceDisconnected, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            //2.2.Đã tiếp đất tại các vị trí:
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfAllowedInfo.GroundedAtLocations, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //2.3. Đã làm rào chắn và treo biển báo tại:
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfAllowedInfo.BarriersAndWarningPoster, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //2.4. Phạm vi được phép làm việc:
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfAllowedInfo.AllowableWorkingRange, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //2.5. Cảnh báo, chỉ dẫn cần thiết:
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfAllowedInfo.WarningsAndInstructions, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //2.6. Cho phép Đơn vị công tác bắt đầu làm việc lúc ... giờ ... phút, ngày ... tháng ... năm ...
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfAllowedInfo.AllowedTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // Người cho phép
            tblData.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfAllowedInfo.AllowerName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);

            PdfPTable tblSign = new PdfPTable(3)
            {
                TotalWidth = TABLE_WIDTH,
                LockedWidth = true
            };
            tblSign.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSign.SetWidths(new float[] { 10, 2, 2 });
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfAllowedInfo.AllowerPosition, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfAllowedInfo.AllowerSignature))
            {
                tblSign.AddCell(GetCellImageSignature(pdfAllowedInfo.AllowerSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
            }
            else
            {
                tblSign.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            }
            PdfPCell cellMerge2 = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
            };
            cellMerge2.AddElement(tblSign);
            mainTable.AddCell(cellMerge2);
        }

        private void AddPdfApproveInfo(PdfPTable mainTable, PdfApproveInfoModel pdfApproveInfo, bool isSigned)
        {
            int fSize = 13;
            //1. Cấp cho
            mainTable.AddCell(GetCellSign(_fBold, fSize + 1, "1. Cấp cho", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // 1.1. Người lãnh đạo công việc
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfApproveInfo.WorkLeader, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //1.2. Người chỉ huy trực tiếp
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfApproveInfo.WorkCommander, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //1.3. Nhân viên đơn vị công tác
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfApproveInfo.UnitNumber, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //1.4.Địa điểm công tác
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfApproveInfo.WorkPlacement, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //1.5 Nội dung công tác
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfApproveInfo.WorkContent, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //1.6. Thời gian theo kế hoạch:
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfApproveInfo.TitleTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // Bắt đầu công việc
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfApproveInfo.EstimatedStartTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Kết thúc công việc
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfApproveInfo.EstimatedEndTime, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //1.7. Điều kiện để tiến hành công việc
            mainTable.AddCell(GetCellSign(_fNomal, fSize, pdfApproveInfo.WorkCondition, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // Phiếu công tác cấp ngày
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfApproveInfo.ApprovedDate, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            //Người cấp phiếu
            mainTable.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfApproveInfo.ApproverUser, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPTable tblSign = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblSign.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSign.SetWidths(new float[] { 10, 2, 2 });
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "       " + pdfApproveInfo.ApproverPosition, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
            tblSign.AddCell(GetCellSign(_fNomal, fSize, "Chữ ký: ", 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM));
            if (!string.IsNullOrEmpty(pdfApproveInfo.ApproverSignature))
            {
                _pdfInfo.SignatureX = 495;
                _pdfInfo.SignatureY = posY + 2;
                _pdfInfo.ApproverSignature = pdfApproveInfo.ApproverSignature;
                if (isSigned)
                {
                    tblSign.AddCell(GetCellImageSignature(pdfApproveInfo.ApproverSignature, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 31, false));
                }
                else
                {
                    // isSigned=false: tạo blank AcroField để SignPdfForTicket ký đè vào
                    tblSign.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Approver", 0, 0, 0, 0, fixHeight: 31));
                }
            }
            else
            {
                if (isSigned)
                {
                    tblSign.AddCell(GetCellSign(_fNomal, fSize, "....................", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM));
                }
                else
                {
                    tblSign.AddCell(PdfHelper.GetBlankSignatureCell("Sig_Approver", 0, 0, 0, 0, fixHeight: 31));
                }
            }
            PdfPCell cellMerge = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                Padding = 0,
            };
            cellMerge.AddElement(tblSign);
            mainTable.AddCell(cellMerge);
        }

        private void AddHeaderToPdf(PdfPTable mainTable, PdfHeaderWorkTicketModel pdfHeaderWorkTicket)
        {
            PdfPTable tblData = new PdfPTable(3) { TotalWidth = TABLE_WIDTH };
            tblData.DefaultCell.Border = Rectangle.NO_BORDER;
            tblData.SetWidths(new float[] { 1, 1, 1 });
            tblData.LockedWidth = true;
            int fSize = 13;

            tblData.AddCell(GetCellSign(_fNomal, fSize - 2, pdfHeaderWorkTicket.CompanyName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fBold, fSize + 3, "PHIẾU CÔNG TÁC", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, pdfHeaderWorkTicket.WorkTicketNumber, 1, 2, Element.ALIGN_CENTER, Element.ALIGN_TOP));

            tblData.AddCell(GetCellSign(_fNomal, fSize - 2, pdfHeaderWorkTicket.ApprovedUnit, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblData.AddCell(GetCellSign(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 0,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblData);
            mainTable.AddCell(cellMerge);
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

        private WorkFinishModel GetWorkFinish(int workTicketID)
        {
            var entity = _unitOfWork.ElogbookRepository<WorkFinish>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted)
                            .Select(e => new WorkFinishModel()
                            {
                                WorkFinishPictures = e.WorkFinishPictures.Where(x => !x.IsDeleted).Select(t => new WorkFinishPictureModel()
                                {
                                    Position = t.Position != null ? t.Position : null,
                                    PictureAttachmentsID = t.Attachments,
                                    IsChecked = t.IsChecked != null ? t.IsChecked : null
                                }).ToList(),
                                RepresentativeUnit = e.RepresentativeUnit,
                                RepresentativeUnitPositionName = e.RepresentativeUnitPositionName,
                                ReturnWorkplaceTimeTemp = e.ReturnWorkplaceTime,
                                ReturnTime = e.ReturnWorkplaceTime,
                                CheckReceivedTimeTemp = e.CheckReceivedTime,
                                CheckedTime = e.CheckReceivedTime,
                                CompletedTime = e.CheckCompletedTime,
                                CheckCompletedTimeTemp = e.CheckCompletedTime,
                                LeaderSignatureID = e.LeaderSignature != null ? e.LeaderSignature : null,
                                CommanderSignatureID = e.CommanderSignature != null ? e.CommanderSignature : null,
                                ApproverSignatureID = e.ApproverSignature != null ? e.ApproverSignature : null,
                                AllowedSignatureID = e.AllowedSignature != null ? e.AllowedSignature : null,
                                OperatorConfirmed1 = e.OperatorConfirmed1 != null ? e.OperatorConfirmed1 : null,
                                OperatorConfirmed2 = e.OperatorConfirmed2 != null ? e.OperatorConfirmed2 : null,
                                OperatorConfirmed3 = e.OperatorConfirmed3 != null ? e.OperatorConfirmed3 : null,
                            }).FirstOrDefault();

            if (entity != null)
            {
                // hình ảnh công tác của phiếu điện 959
                if (entity.WorkFinishPictures != null)
                {
                    foreach (var item in entity.WorkFinishPictures)
                    {
                        item.PictureAttachments = new List<AttachmentModel>();
                        var attackments = item.PictureAttachmentsID?.Split(',');
                        if (attackments != null)
                        {
                            foreach (var attachment in attackments)
                            {
                                if (int.TryParse(attachment, out int idItem))
                                {
                                    var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == idItem).FirstOrDefault();
                                    if (att != null)
                                    {
                                        item.PictureAttachments.Add(new AttachmentModel
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

                entity.LeaderSignature = _commonService.GetSignatureById(entity.LeaderSignatureID.GetValueOrDefault());
                var leadershipUser = GetLeadershipById(workTicketID, (int)LeadershipUserRoles.Leader);
                if (entity.LeaderSignature == null)
                {
                    entity.LeaderSignature = new SignatureInfo
                    {
                        SignatureContent = leadershipUser != null ? leadershipUser.Signature : string.Empty
                    };
                }
                else if (entity.LeaderSignature != null && string.IsNullOrWhiteSpace(entity.LeaderSignature.SignatureContent))
                {
                    entity.LeaderSignature.SignatureContent = leadershipUser != null ? leadershipUser.Signature : string.Empty;
                }

                if (entity.CommanderSignatureID != null) entity.CommanderSignature = _commonService.GetSignatureById((int)entity.CommanderSignatureID);
                if (entity.ApproverSignatureID != null) entity.ApproverSignature = _commonService.GetSignatureById((int)entity.ApproverSignatureID);
                if (entity.AllowedSignatureID != null) entity.AllowedSignature = _commonService.GetSignatureById((int)entity.AllowedSignatureID);
            }

            return entity;
        }

        private List<AttachmentModel> GetAttachedFile(string workPicturesID)
        {
            List<AttachmentModel> results = new List<AttachmentModel>();
            var attackments = workPicturesID.Split(',');
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

            return results;
        }

        private AllowedInfoModel GetAllowedInfo(int workTicketID)
        {
            var entity = _unitOfWork.ElogbookRepository<AllowedInfo>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted)
                            .Select(e => new AllowedInfoModel()
                            {
                                ID = e.ID,
                                AllowedInfoDetails = e.AllowedInfoDetails.Where(t => !t.IsDeleted).Select(t => new AllowedInfoDetailModel()
                                {
                                    ID = t.ID,
                                    Content = t.Content,
                                    Position = t.Position != null ? t.Position : null,
                                    IsChecked = t.IsChecked != null ? t.IsChecked : null,
                                    Note = t.Note,
                                    ListPictureID = t.Attachments,
                                }).ToList(),
                                AllowedTimeTemp = e.AllowedTime,
                                TimeCommanderSignedTemp = e.TimeCommanderSigned,
                                AllowerSigned = e.AllowedTime,
                                CommanderSigned = e.TimeCommanderSigned,
                                CommanderSignatureID = e.CommanderSignature != null ? e.CommanderSignature : null
                            }).FirstOrDefault();

            if (entity != null)
            {
                var allowedInfo = _unitOfWork.ElogbookRepository<AllowedInfo>().GetQuery(x => x.WorkTicketID == workTicketID
                                                                                        && !x.IsDeleted && x.CommanderSignature != null).FirstOrDefault();
                if (allowedInfo != null)
                {
                    entity.AllowerSigned = allowedInfo.AllowedTime;
                    entity.CommanderSigned = allowedInfo.TimeCommanderSigned;
                    entity.AllowedTime = allowedInfo.AllowedTime != DateTime.MinValue ? allowedInfo.AllowedTime.ToString("dd/MM/yyyy HH:mm") : string.Empty;
                    entity.TimeCommanderSigned = allowedInfo.TimeCommanderSigned != DateTime.MinValue ? allowedInfo.TimeCommanderSigned.ToString("dd/MM/yyyy HH:mm") : string.Empty;
                    entity.CommanderSignatureID = allowedInfo.CommanderSignature != null ? allowedInfo.CommanderSignature : null;
                }

                if (entity.AllowedInfoDetails != null)
                {
                    foreach (var item in entity.AllowedInfoDetails)
                    {
                        item.PictureAttachments = new List<AttachmentModel>();
                        var attackments = item?.ListPictureID?.Split(',');
                        if (attackments != null)
                        {
                            foreach (var attachment in attackments)
                            {
                                if (int.TryParse(attachment, out int idItem))
                                {
                                    var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(t => !t.IsDeleted && t.ID == idItem).FirstOrDefault();
                                    if (att != null)
                                    {
                                        item.PictureAttachments.Add(new AttachmentModel
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

                    var acceptorUserEntity = GetLeadershipById(workTicketID, (int)LeadershipUserRoles.Acceptor);
                    // chữ ký người cho phép
                    if (acceptorUserEntity != null && acceptorUserEntity.Signature != null)
                    {
                        SignatureInfo signatureInfo = new SignatureInfo
                        {
                            SignatureContentByte = Encoding.UTF8.GetBytes(acceptorUserEntity.Signature)
                        };
                        entity.AllowedSignature = signatureInfo;
                    }

                    // chữ ký người chỉ huy
                    if (entity.CommanderSignatureID != null)
                    {
                        entity.CommanderSignature = _commonService.GetSignatureById((int)entity.CommanderSignatureID);
                    }
                }
            }

            return entity;
        }

        private ReceiveWorkplaceModel GetReceiveWorkplace(int workTicketID)
        {
            var entity = _unitOfWork.ElogbookRepository<ReceiveWorkplace>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted)
                            .Select(e => new ReceiveWorkplaceModel()
                            {
                                ID = e.ID,
                                SafetyMeasures = e.SafetyMeasures.Where(t => !t.IsDeleted).Select(t => new SafetyMeasureModel()
                                {
                                    ID = t.ID,
                                    Content = t.Content,
                                    Position = t.Position != null ? t.Position : null,
                                    IsChecked = t.IsChecked != null ? t.IsChecked : null,
                                    Note = t.Note,
                                    AttachmentsID = t.Attachments
                                }).ToList(),
                                StartTime = e.StartWorkingDate,
                                StartWorkingDateTemp = e.StartWorkingDate,
                                LeaderSignatureID = e.LeaderSignature != null ? e.LeaderSignature : null,
                                CommanderSignatureID = e.CommanderSignature != null ? e.CommanderSignature : null,
                                SupervisorSignatureID = e.SupervisorSignature != null ? e.SupervisorSignature : null,
                                WorkUnitLv1 = e.WorkUnitID != null ? new ValueLabelModel { Value = e.WorkUnit.ID, Label = e.WorkUnit.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                                WorkUnitLv2 = e.WorkUnit.Parent != null ? new ValueLabelModel { Value = e.WorkUnit.Parent.ID, Label = e.WorkUnit.Parent.Name } : new ValueLabelModel { Value = 0, Label = "null" },
                                WorkUnitName = e.WorkUnitName,
                                AmountPersons = e.AmountPersons,
                            }).FirstOrDefault();

            if (entity != null && entity.SafetyMeasures != null)
            {
                foreach (var item in entity.SafetyMeasures)
                {
                    item.PictureAttachments = new List<AttachmentModel>();
                    var attackments = item.AttachmentsID?.Split(',');
                    if (attackments != null)
                    {
                        foreach (var attachment in attackments)
                        {
                            if (int.TryParse(attachment, out int idItem))
                            {
                                var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == idItem).FirstOrDefault();
                                if (att != null)
                                {
                                    item.PictureAttachments.Add(new AttachmentModel
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

                if (entity.LeaderSignatureID != null) entity.LeaderSignature = _commonService.GetSignatureById((int)entity.LeaderSignatureID);
                if (entity.CommanderSignatureID != null) entity.CommanderSignature = _commonService.GetSignatureById((int)entity.CommanderSignatureID);
                if (entity.SupervisorSignatureID != null) entity.SupervisorSignature = _commonService.GetSignatureById((int)entity.SupervisorSignatureID);
            }

            return entity;
        }

        private WorkTicket CreateEntity(AddNewWorkTicketModel requestModel)
        {
            var entity = new WorkTicket
            {
                ScreenType = requestModel.ScreenType != null ? requestModel.ScreenType : 1,
                WorkTicketNumber = requestModel.WorkTicketNumber,
                FactoryID = requestModel.FactoryID,
                RegistrationTicketID = requestModel.RegistrationTicketID > 0 ? requestModel.RegistrationTicketID : null,
                WorkTicketFeatureID = requestModel.WorkTicketFeatureID > 0 ? requestModel.WorkTicketFeatureID : null,
                ApprovedUnitID = requestModel.ApprovedUnitID,
                ApprovedDate = DateTime.Now,
                WorkTicketType = requestModel.WorkTicketType,
                WorkTicketStatus = (int)EnumTicketStatus.Approved,
                UnitNumber = requestModel.UnitNumber != null ? requestModel.UnitNumber : null,
                WorkPlacement = !string.IsNullOrEmpty(requestModel.WorkPlacement) ? requestModel.WorkPlacement : null,
                WorkContent = !string.IsNullOrEmpty(requestModel.WorkContent) ? requestModel.WorkContent : null,
                StartDate = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.StartDate),
                EndDate = DateTimeConvert.ConvertDateAndTimeFromString(requestModel.EndDate),
                WorkCondition = !string.IsNullOrEmpty(requestModel.WorkCondition) ? requestModel.WorkCondition : null,
                WorkScope = !string.IsNullOrEmpty(requestModel.WorkScope) ? requestModel.WorkScope : null,
                Attachments = (requestModel.AttachmentsID != null) ? string.Join(",", requestModel.AttachmentsID.ToArray()) : null,
                CreatedUserID = requestModel.CurrentUserID,
                InsertedAt = DateTime.Now,
                LastUpdatedUserID = requestModel.CurrentUserID,
                UpdatedAt = DateTime.Now,
                LeadershipUsers = requestModel.LeadershipUsers.Select(e => new LeadershipUser()
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
                }).ToList(),
            };
            if (requestModel.WarnAndSafes != null && requestModel.WarnAndSafes.Count > 0)
            {
                entity.WarnAndSafes = requestModel.WarnAndSafes.Select(e => new WarnAndSafe()
                {
                    WarningContent = e.WarningContent,
                    SafetyMeasureContent = e.SafetyMeasureContent,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                }).ToList();
            }

            if (requestModel.WorkTicketFeatures != null && requestModel.WorkTicketFeatures.Count > 0)
            {
                entity.WorkTicketFeatureLinks = requestModel.WorkTicketFeatures.Select(id => new WorkTicketFeatureLink()
                {
                    FeatureID = id,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                }).ToList();
            }

            entity.RequestTicketID = requestModel.RequestTicketID > 0 ? requestModel.RequestTicketID : null;

            return entity;
        }

        private LeadershipUser LeadershipUserEntity(AddLeadershipUserModel requestModel, int userID, int roles)
        {
            var entity = new LeadershipUser()
            {
                IsExternal = requestModel.IsExternal,
                Roles = roles,
                CreatedUserID = userID,
                InsertedAt = DateTime.Now,
                LastUpdatedUserID = userID,
                UpdatedAt = DateTime.Now,
            };

            AddNewLeadershipUserFetchedModel leadershipUserModel = GetUserInfo(requestModel.UserName, requestModel.IsExternal, roles);
            if (leadershipUserModel != null)
            {
                entity.UserName = !string.IsNullOrWhiteSpace(leadershipUserModel.UserName) ? leadershipUserModel.UserName : null;
                entity.FullName = !string.IsNullOrWhiteSpace(leadershipUserModel.FullName) ? leadershipUserModel.FullName : null;
                entity.PositionName = !string.IsNullOrWhiteSpace(leadershipUserModel.PositionName) ? leadershipUserModel.PositionName : null;
                entity.ElectricRank = leadershipUserModel.ElectricRank;
                entity.ElectricRankCard = !string.IsNullOrWhiteSpace(leadershipUserModel.ElectricRankCard) ? leadershipUserModel.ElectricRankCard : null;
                entity.Signature = !string.IsNullOrWhiteSpace(leadershipUserModel.Signature) ? leadershipUserModel.Signature : null;
                entity.LaborCard = !string.IsNullOrWhiteSpace(leadershipUserModel.LaborCard) ? leadershipUserModel.LaborCard : null;
                entity.LaborRank = leadershipUserModel.LaborRank;
            }

            return entity;
        }

        private LeadershipUser LeadershipUserEntity(UserInfo userInfo, int userID, int roles)
        {
            var entity = new LeadershipUser()
            {
                PositionName = userInfo.PositionName,
                FullName = !string.IsNullOrWhiteSpace(userInfo.FullName) ? userInfo.FullName : null,
                ElectricRank = userInfo.ElectricRankStr,
                ElectricRankCard = !string.IsNullOrWhiteSpace(userInfo.ElectricRankCard) ? userInfo.ElectricRankCard : null,
                Signature = !string.IsNullOrWhiteSpace(userInfo.Signature) ? userInfo.Signature : null,
                UserName = userInfo.UserName,
                IsExternal = false,
                CreatedUserID = userID,
                InsertedAt = DateTime.Now,
                LastUpdatedUserID = userID,
                UpdatedAt = DateTime.Now,
                Roles = roles,
            };

            return entity;
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

        private PdfStaffInfoModel StaffInfoPdfEntity(string No)
        {
            PdfStaffInfoModel stInfo = new PdfStaffInfoModel()
            {
                No = No,
                FullName = " ",
                CheckInDateTime = DateTime.MinValue,
                CheckOutDateTime = DateTime.MinValue,
            };

            return stInfo;
        }

        private PdfWorkPlacementModel WorkPlacementPdfModel(string No)
        {
            PdfWorkPlacementModel wp = new PdfWorkPlacementModel()
            {
                No = No,
                Place = " ",
                StartTime = " ",
                EndTime = " ",
                CommanderName = " ",
                AllowerName = " ",
            };

            return wp;
        }

        private void AddPdfStaffInfos(PdfPTable mainTable, List<PdfStaffInfoModel> listStaff)
        {
            int fSize = 13;

            //4. Danh sách nhân viên đơn vị công tác (và thay đổi người nếu có)
            mainTable.AddCell(GetCellSign(_fBold, fSize + 1, "4. Danh sách nhân viên đơn vị công tác (và thay đổi người nếu có)", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPTable tblWorker = new PdfPTable(6) { TotalWidth = TABLE_WIDTH };
            tblWorker.SetWidths(new float[] { 1, 5, 4, 4, 4, 4 });
            tblWorker.LockedWidth = true;
            fSize = 11;

            // Header row.
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "TT", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "Họ tên", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "Vào vị trí làm việc", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "Ra khỏi vị trí làm việc", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // Inner middle row.
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "Thời gian", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "Ký tên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "Thời gian", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblWorker.AddCell(GetCellBorder(_fNomal, fSize, "Ký tên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // add content
            foreach (var item in listStaff)
            {
                tblWorker.AddCell(GetCellBorder(_fNomal, fSize, item.No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblWorker.AddCell(GetCellBorder(_fNomal, fSize, item.FullName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                if (!string.IsNullOrEmpty(item.CheckInTime))
                {
                    tblWorker.AddCell(GetCellBorder(_fNomal, fSize, item.CheckInTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }
                else
                {
                    tblWorker.AddCell(GetCellBorder(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }

                if (!string.IsNullOrEmpty(item.CheckInSignature))
                {
                    tblWorker.AddCell(GetCellImageSignature(item.CheckInSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_CENTER, 35));
                }
                else
                {
                    tblWorker.AddCell(GetCellBorder(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }

                if (!string.IsNullOrEmpty(item.CheckOutTime))
                {
                    tblWorker.AddCell(GetCellBorder(_fNomal, fSize, item.CheckOutTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }
                else
                {
                    tblWorker.AddCell(GetCellBorder(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }

                if (!string.IsNullOrEmpty(item.CheckOutSignature))
                {
                    tblWorker.AddCell(GetCellImageSignature(item.CheckOutSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_CENTER, 35));
                }
                else
                {
                    tblWorker.AddCell(GetCellBorder(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER,
            };
            cellMerge.AddElement(tblWorker);
            mainTable.AddCell(cellMerge);
        }

        private void AddPdfWorkPlacement(PdfPTable mainTable, List<PdfWorkPlacementModel> listPlace)
        {
            int fSize = 13;

            //5. Cho phép làm việc và kết thúc công tác hàng ngày, di chuyển nơi làm việc
            mainTable.AddCell(GetCellSign(_fBold, fSize + 1, "5. Cho phép làm việc và kết thúc công tác hàng ngày, di chuyển nơi làm việc", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            PdfPTable tblPlace = new PdfPTable(8) { TotalWidth = TABLE_WIDTH };
            tblPlace.SetWidths(new float[] { 1, 5, 4, 4, 4, 4, 4, 4 });
            tblPlace.LockedWidth = true;
            fSize = 11;

            // Header row.
            tblPlace.AddCell(GetCellBorder(_fNomal, fSize, "TT", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblPlace.AddCell(GetCellBorder(_fNomal, fSize, "Địa điểm công tác", 1, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblPlace.AddCell(GetCellBorder(_fNomal, fSize, "Thời gian (giờ, ngày, tháng)", 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblPlace.AddCell(GetCellBorder(_fNomal, fSize, "Người chi huy trực tiếp\nKý hoặc ghi họ tên", 2, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblPlace.AddCell(GetCellBorder(_fNomal, fSize, "Người cho phép\nKý hoặc ghi họ tên", 2, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            // Inner middle row.
            tblPlace.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Bắt đầu", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblPlace.AddCell(PdfHelper.GetCell(_fNomal, fSize, "Kết thúc", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            foreach (var item in listPlace)
            {
                tblPlace.AddCell(GetCellBorder(_fNomal, fSize, item.No, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblPlace.AddCell(GetCellBorder(_fNomal, fSize, item.Place, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblPlace.AddCell(GetCellBorder(_fNomal, fSize, item.StartTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblPlace.AddCell(GetCellBorder(_fNomal, fSize, item.EndTime, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                tblPlace.AddCell(GetCellBorder(_fNomal, fSize, item.CommanderName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                if (!string.IsNullOrEmpty(item.CommanderSignature))
                {
                    tblPlace.AddCell(GetCellImageSignature(item.CommanderSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM, 35));
                }
                else
                {
                    tblPlace.AddCell(GetCellBorder(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }

                tblPlace.AddCell(GetCellBorder(_fNomal, fSize, item.AllowerName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                if (!string.IsNullOrEmpty(item.AllowerSignature))
                {
                    tblPlace.AddCell(GetCellImageSignature(item.AllowerSignature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_BOTTOM, 35));
                }
                else
                {
                    tblPlace.AddCell(GetCellBorder(_fNomal, fSize, " ", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                }
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Padding = 5,
                Border = Rectangle.NO_BORDER
            };
            cellMerge.AddElement(tblPlace);
            mainTable.AddCell(cellMerge);
        }

        private AddNewLeadershipUserFetchedModel GetUserInfo(int id, bool isExternal, int Roles)
        {
            if (!isExternal)
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
                    IsExternal = isExternal,
                    UserID = id,
                    PositionName = e.PositionName,

                }).FirstOrDefault();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new AddNewLeadershipUserFetchedModel()
                {
                    UserName = e.UserName,
                    FullName = e.FullName,
                    ElectricRank = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    LaborCard = e.LaborCard,
                    LaborRank = e.LaborRank,
                    Roles = Roles,
                    IsExternal = isExternal,
                    UserID = id,
                }).FirstOrDefault();
            }
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

        private UserInfo GetUserInfo(string FullName)
        {
            return _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted
                                                                    && x.FullName.Equals(FullName, StringComparison.OrdinalIgnoreCase))
                    .Select(
                        e => new UserInfo()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            UserName = e.UserName,
                            DateOfBirth = e.DateOfBirth,
                            NationalID = e.NationalID,
                            ElectricRankStr = e.ElectricRank,
                            ElectricRankCard = e.ElectricRankCard,
                            LaborRankStr = e.LaborRank,
                            LaborCard = e.LaborCard,
                            Signature = e.Signature,
                            CompanyID = e.CompanyID,
                            ProductionPlantID = e.ProductionPlantID,
                            TeamID = e.TeamID,
                            GroupID = e.GroupID,
                            FactoryID = e.FactoryID,
                            AccountType = e.AccountType,
                            SourceType = e.SourceType,
                            Status = e.Status,
                            IsExternal = false,
                            PositionName = e.PositionName,
                            PhoneNumber = e.PhoneNumber,
                            SigningMethod = new ValueLabelModel { Value = e.SigningMethod.ID, Label = e.SigningMethod.SigningMethodName },
                            SigningMethodID = e.SigningMethod.ID,
                        }).FirstOrDefault();
        }

        private string GetCompanyName(int factoryId)
        {
            var entity = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == factoryId && !x.IsDeleted).FirstOrDefault();
            if (entity != null && entity.Company != null)
            {
                return entity.Company.Name;
            }

            return string.Empty;
        }

        private int GetFactory(int? userID, int? organizationID)
        {
            if (userID > 0)
            {
                var userEntity = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == userID && !x.IsDeleted).FirstOrDefault();
                if (userEntity != null) return userEntity.FactoryID != null ? (int)userEntity.FactoryID : -1;
                return -1;
            }
            if (organizationID > 0)
            {
                var orgEntity = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == organizationID && !x.IsDeleted).FirstOrDefault();
                if (orgEntity != null && orgEntity.ParentID != null)
                {
                    var fatoryEntity = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.CompanyID == orgEntity.ParentID && x.ProductionPlantID == organizationID && !x.IsDeleted).FirstOrDefault();
                    if (fatoryEntity != null) return fatoryEntity.ID;
                    return -1;
                }

                return -1;
            }

            return -1;
        }

        private RegistrationDetailModel GetRegistrationDetail(int registrationID)
        {
            RegistrationDetailModel registrationDetailModel = new RegistrationDetailModel();
            var entity = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.ID == registrationID && !x.IsDeleted).FirstOrDefault();

            if (entity != null)
            {
                // Danh sách vị trí và nhân viên công tác
                var wtLocationEntities = _unitOfWork.ElogbookRepository<ContentPosition>()
                        .GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == registrationID);
                if (wtLocationEntities != null)
                {
                    var workTicketLocations = wtLocationEntities.Select(e => new ContentPositionModel()
                    {
                        ID = e.ID,
                        Position = e.Position,
                        Stakeholders = e.Stakeholders.Where(x => !x.IsDeleted).Select(t => new StakeholdersModel()
                        {
                            ID = t.ID,
                            AssignToUserID = t.AssignToUserID,
                            FullName = t.FullName,
                            Mission = t.Mission,
                            UserName = t.UserName,
                            ElectricRankStr = t.ElectricRank,
                            ElectricRankCard = t.ElectricRankCard,
                            LaborRankStr = t.LaborRank,
                            LaborCard = t.LaborCard,
                            PositionName = t.PositionName,
                            Roles = t.Roles,
                            IsExternal = t.IsExternal,
                        }).ToList(),
                    }).ToList();

                    registrationDetailModel.WorkTicketLocations = workTicketLocations;
                }

                // danh sách nhân viên tham gia công việc
                var stEntities = _unitOfWork.ElogbookRepository<Stakeholders>()
                        .GetQuery(x => !x.IsDeleted && x.RegistrationTicketID == registrationID);

                if (stEntities != null)
                {
                    var stakeholders = stEntities.Select(t => new StakeholdersResponseModel()
                    {
                        ID = t.ID,
                        AssignToUserID = t.AssignToUserID,
                        FullName = t.FullName,
                        Mission = t.Mission,
                        UserName = t.UserName,
                        ElectricRankStr = t.ElectricRank,
                        ElectricRankCard = t.ElectricRankCard,
                        PositionName = t.PositionName,
                        LaborRankStr = t.LaborRank,
                        LaborCard = t.LaborCard,
                        Roles = t.Roles,
                        IsExternal = t.IsExternal,
                    }).ToList();

                    registrationDetailModel.Stakeholders = stakeholders;
                }

                var receiveUnit = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == entity.ReceiveUnitID && !x.IsDeleted).FirstOrDefault();
                registrationDetailModel.FactoryID = entity.FactoryID;
                if (receiveUnit != null)
                {
                    // đơn vị cấp phiếu
                    registrationDetailModel.ReceiveUnitID = receiveUnit.ParentID != null ? (int)receiveUnit.ParentID : -1;
                }
                else
                {
                    registrationDetailModel.ReceiveUnitID = -1;
                }
                if (entity.RegistrationUnitID != null)
                {
                    // đơn vị công tác cấp phân xưởng
                    var registrationUnitLv1 = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == entity.RegistrationUnitID && !x.IsDeleted).FirstOrDefault();
                    if (registrationUnitLv1 != null)
                    {
                        registrationDetailModel.WorkUnitLv1 = new ValueLabelModel { Value = registrationUnitLv1.ID, Label = registrationUnitLv1.Name };
                    }

                    // đơn vị công tác cấp tổ
                    var registrationUnitLv2 = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == registrationUnitLv1.ParentID && !x.IsDeleted).FirstOrDefault();
                    if (registrationUnitLv2 != null)
                    {
                        registrationDetailModel.WorkUnitLv2 = new ValueLabelModel { Value = registrationUnitLv2.ID, Label = registrationUnitLv2.Name };
                    }
                    registrationDetailModel.RegistrationUnitView = entity.RegistrationUnitView;
                    registrationDetailModel.RegistrationTicketID = (int)entity.RegistrationUnitID;
                }

                registrationDetailModel.RequestTicketID = entity.RequestTicketID;
            }
            else
            {
                registrationDetailModel.RegistrationTicketID = 0;
                registrationDetailModel.FactoryID = -1;
                registrationDetailModel.ReceiveUnitID = -1;
            }

            return registrationDetailModel;
        }

        #endregion

        /// <summary>
        /// Reset số phiếu không sử dụng
        /// </summary>
        public void ResetTicketNumber()
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                _unitOfWork.DataContext.Database.CommandTimeout = 3600;
                try
                {
                    var result = _unitOfWork.DataContext.Database
                            .SqlQuery<Result>("PCT_ResetTicketNumber").ToList();

                    if (result.Count > 0 && result[0].Message == "SUCCESS")
                    {
                        //ignore error when create success
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkTicketService", "Reset số phiếu không thành công!");
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkTicketService",
                        "Reset số phiếu không thành công!" + Environment.NewLine + ex.ToString());
                }
            }
        }

        public ResponseModel<bool> DeleteWorkTicket(int workTicketId, int userId)
        {
            var responseModel = new ResponseModel<bool>();

            if (workTicketId < 1)
            {
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.ErrorMessages.Add("ID phiếu công tác là bắt buộc");
                return responseModel;
            }

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(workTicketId);

                    if (entity != null)
                    {
                        // Xóa phiếu công tác
                        _unitOfWork.ElogbookRepository<WorkTicket>().Delete(entity, userId);

                        // Xóa số phiếu công tác
                        string[] arrNumTicket = entity.WorkTicketNumber.Split('/');
                        Int32.TryParse(arrNumTicket[0], out int numTicket);
                        int screenType = entity.ScreenType.Value;
                        string ticketType = "QLPCTD";

                        if (screenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                        {
                            ticketType = "QLPTCNH";
                        }

                        var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                            .GetQuery(x => x.InsertedAt.Year == DateTime.Now.Year && x.TicketType.Equals(ticketType) && x.TicketNumber == numTicket).FirstOrDefault();

                        if (lstNumber != null)
                        {
                            _unitOfWork.ElogbookRepository<ExclusiveData>().Delete(x => x.ID == lstNumber.ID, 0, true);
                        }

                        if (_unitOfWork.Complete() > 0)
                        {
                            transaction.Commit();
                            responseModel.Data = true;
                            responseModel.ErrorMessages.Add("Xóa phiếu công tác thành công");
                        }
                        else
                        {
                            transaction.Rollback();
                            responseModel.Data = false;
                            responseModel.ErrorMessages.Add("Xóa phiếu công tác thất bại");
                        }
                    }
                    else
                    {
                        responseModel.Data = false;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.ErrorMessages.Add("Phiếu công tác không tồn tại");
                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkTicketService",
                        "Xóa phiếu công tác không thành công!" + Environment.NewLine + ex.ToString());
                }
            }
            return responseModel;
        }

        /// <summary>
        /// Không cho phép thực hiện phiếu công tác
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <param name="status"></param>
        /// <param name="description"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateStatus(int workTicketId, EnumTicketStatus status, string description, int currentUserId)
        {
            var responseModel = new ResponseModel<bool>();

            if (workTicketId < 1)
            {
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.ErrorMessages.Add("Mã phiếu công tác là thông tin bắt buộc!");

                return responseModel;
            }

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(workTicketId);
                    if (entity != null)
                    {
                        if (status == EnumTicketStatus.NotAllowed || status == EnumTicketStatus.Annulled)
                        {
                            if (status == EnumTicketStatus.NotAllowed && entity.WorkTicketStatus != (int)EnumTicketStatus.Approved)
                            {
                                responseModel.Data = false;
                                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                                responseModel.ErrorMessages.Add("Trạng thái phiếu công tác khác cấp phiếu - không thể không cho phép thực hiện.");

                                return responseModel;
                            }
                            entity.ReasonReject = description;
                        }

                        entity.WorkTicketStatus = (int)status;
                        entity.LastUpdatedUserID = currentUserId;
                        // Update phiếu công tác
                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(entity);

                        //Thêm lịch sử khi hủy phiếu công tác
                        if (status == EnumTicketStatus.Annulled)
                        {
                            MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                            {
                                MajorRepairID = entity.MajorRepairID,
                                WorkTicketID = entity.ID,
                                WorkTicketNumber = entity.WorkTicketNumber,
                                Content = MajorRepairConstant.ANNULLED_DELETE_PCT,
                                Comment = description,
                                CreatedUserID = currentUserId,
                                LastUpdatedUserID = currentUserId,
                            };
                            _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);
                        }

                        //Thêm lịch sử khi không cho phép công tác
                        if (status == EnumTicketStatus.NotAllowed)
                        {
                            MajorRepairHistory majorRepairHistory = new MajorRepairHistory
                            {
                                MajorRepairID = entity.MajorRepairID,
                                WorkTicketID = entity.ID,
                                WorkTicketNumber = entity.WorkTicketNumber,
                                Content = "Không cho phép thực hiện",
                                Comment = description,
                                CreatedUserID = currentUserId,
                                LastUpdatedUserID = currentUserId,
                            };
                            _unitOfWork.ElogbookRepository<MajorRepairHistory>().Add(majorRepairHistory);
                        }

                        if (_unitOfWork.Complete() > 0)
                        {
                            transaction.Commit();
                            responseModel.Data = true;
                            responseModel.ErrorMessages.Add("Cập nhật phiếu công tác thành công!");
                        }
                        else
                        {
                            transaction.Rollback();
                            responseModel.Data = false;
                            responseModel.ErrorMessages.Add("Cập nhật phiếu công tác thất bại!");
                        }
                    }
                    else
                    {
                        responseModel.Data = false;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.ErrorMessages.Add("Phiếu công tác không tồn tại!");

                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkTicketService",
                        "Cập nhật phiếu công tác không thành công!" + Environment.NewLine + ex.ToString());
                }
            }

            return responseModel;
        }

        #region Send Email Work Ticket Completion Reminder

        /// <summary>
        /// Gửi email nhắc nhở, cảnh báo phiếu công tác chưa hoàn thành theo kế hoạch
        /// </summary>
        public void SendEmailWorkTicketCompletionReminder()
        {
            lock (_lockCompletionReminder)
            {
                ProcessBeforeDue(EnumSlaCondition.BeforeDue);
                ProcessDueToday(EnumSlaCondition.DueToday);
                ProcessOverdue(EnumSlaCondition.Overdue);
            }
        }

        private void ProcessBeforeDue(EnumSlaCondition slaCondition)
        {
            var slaConfigs = GetSlaConfig(slaCondition);

            if (slaConfigs.Count > 0)
            {
                foreach (var slaConfig in slaConfigs)
                {
                    var slaLevels = slaConfig.Levels;
                    List<int> userGroupIds = new List<int>();

                    foreach (var slaLevel in slaLevels)
                    {
                        if (slaLevel.SlaConditionID == (int)slaCondition && slaLevel.SendEmail
                            && slaLevel.UserGroups != null && slaLevel.UserGroups.Any())
                        {

                            int notifyMinute = (slaLevel.OffsetDays * 24 * 60)
                                            + (slaLevel.OffsetHours * 60)
                                            + slaLevel.OffsetMinutes; // Chuyển thời gian sang phút

                            string timeDue = slaLevel.OffsetDays > 0 ? $"{slaLevel.OffsetDays} ngày " : string.Empty;
                            timeDue = slaLevel.OffsetHours > 0 ? timeDue + $"{slaLevel.OffsetHours} giờ " : timeDue;
                            timeDue = slaLevel.OffsetMinutes > 0 ? timeDue + $"{slaLevel.OffsetMinutes} phút " : timeDue;

                            userGroupIds.AddRange(slaLevel.UserGroups.Select(ug => ug.UserGroupsID));

                            ProcessBeforeDue(notifyMinute, slaConfig.FactoryID, timeDue, userGroupIds);
                        }
                    }
                }
            }
            //else
            //{
            //    var beforeHours =
            //    WorkPermitConfig
            //        .CompletionNotificationTimes
            //        .BeforeTimes
            //        .Select(x => Convert.ToInt32(x.TotalHours))
            //        .OrderByDescending(x => x)
            //        .ToList();

            //    foreach (var notifyHour in beforeHours)
            //    {
            //        ProcessBeforeDue(notifyHour * 60, 0, $"{notifyHour} giờ");
            //    }
            //}
        }

        private void ProcessBeforeDue(int notifyMinute, int factoryId, string timeDue, List<int> userGroupIds)
        {
            var now = DateTime.Now;
            var window = TimeSpan.FromMinutes(WorkPermitConfig.NotificationWindowMinutes);
            var startDate = now.Subtract(window).AddMinutes(notifyMinute);
            var endDate = now.AddMinutes(notifyMinute);

            var workTickets = _unitOfWork
                .ElogbookRepository<WorkTicket>()
                .GetQuery(x =>
                    x.WorkTicketStatus != (int)EnumTicketStatus.Completed
                    && !x.IsDeleted
                    && x.EndDate > startDate
                    && x.EndDate <= endDate);

            if (factoryId > 0)
            {
                workTickets = workTickets.Where(x => x.FactoryID == factoryId);
            }

            var workTicketList = workTickets.ToList();

            foreach (var ticket in workTicketList)
            {
                if (HasSentNotification(ticket.BeforeDueHistory, notifyMinute))
                {
                    continue;
                }

                // Push mail service
                var processEmailRequestModel = new ProcessEmailRequestModel
                {
                    TimeDue = timeDue,
                    UserGroupIds = userGroupIds,
                    // Đánh dấu thông báo gửi trước
                    NotificationTypeValue = 1,
                    WorkTicketID = ticket.ID,
                    CurrentUserID = ticket.CreatedUserID.GetValueOrDefault(),
                    FactoryID = ticket.FactoryID.GetValueOrDefault(),
                    ModuleNotification = ticket.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                };

                if (_emailService.ProcessEmailWorkTicketCompletion(processEmailRequestModel))
                {
                    var entity = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(ticket.ID);
                    if (entity != null)
                    {
                        entity.BeforeDueHistory = AddNotificationHistory(entity.BeforeDueHistory, notifyMinute);
                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(entity);
                        _unitOfWork.Complete();
                    }
                }
            }
        }

        private void ProcessDueToday(EnumSlaCondition slaCondition)
        {
            // Lấy danh sách cấu hình SLA cho từng nhà máy, để gửi email theo từng nhà máy
            var slaConfigs = GetSlaConfig(slaCondition);

            if (slaConfigs.Count > 0)
            {
                foreach (var slaConfig in slaConfigs)
                {
                    var slaLevels = slaConfig.Levels;
                    List<int> userGroupIds = new List<int>();
                    foreach (var slaLevel in slaLevels)
                    {
                        if (slaLevel.SlaConditionID == (int)slaCondition && slaLevel.SendEmail)
                        {
                            userGroupIds.AddRange(slaLevel.UserGroups.Select(ug => ug.UserGroupsID));
                            ProcessDueToday(0, slaConfig.FactoryID, userGroupIds);
                        }
                    }
                }
            }
        }

        private void ProcessDueToday(int notifyMinute, int factoryId, List<int> userGroupIds)
        {
            var now = DateTime.Now;
            var window = TimeSpan.FromMinutes(WorkPermitConfig.NotificationWindowMinutes);
            var startDate = now.Subtract(window).AddMinutes(notifyMinute);
            var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            var workTickets = _unitOfWork
                .ElogbookRepository<WorkTicket>()
                .GetQuery(x =>
                    x.WorkTicketStatus != (int)EnumTicketStatus.Completed
                    && !x.IsDeleted
                    && x.EndDate > startDate
                    && x.EndDate <= endDate);

            if (factoryId > 0)
            {
                workTickets = workTickets.Select(x => x).Where(x => x.FactoryID == factoryId);
            }

            var workTicketList = workTickets.ToList();

            foreach (var ticket in workTicketList)
            {
                if (HasSentNotification(ticket.DueTodayHistory, notifyMinute))
                {
                    continue;
                }

                // Push mail service
                var processEmailRequestModel = new ProcessEmailRequestModel
                {
                    // Đánh dấu thông báo gửi đúng hạn
                    NotificationTypeValue = 2,
                    UserGroupIds = userGroupIds,
                    WorkTicketID = ticket.ID,
                    CurrentUserID = ticket.CreatedUserID.GetValueOrDefault(),
                    FactoryID = ticket.FactoryID.GetValueOrDefault(),
                    ModuleNotification = ticket.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                };

                if (_emailService.ProcessEmailWorkTicketCompletion(processEmailRequestModel))
                {
                    var entity = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(ticket.ID);
                    if (entity != null)
                    {
                        entity.DueTodayHistory = AddNotificationHistory(entity.DueTodayHistory, notifyMinute);
                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(entity);
                        _unitOfWork.Complete();
                    }
                }
            }
        }

        private void ProcessOverdue(EnumSlaCondition slaCondition)
        {
            // Lấy danh sách cấu hình SLA cho từng nhà máy, để gửi email theo từng nhà máy
            var slaConfigs = GetSlaConfig(slaCondition);

            if (slaConfigs.Count > 0)
            {
                foreach (var slaConfig in slaConfigs)
                {
                    var slaLevels = slaConfig.Levels;
                    List<int> userGroupIds = new List<int>();
                    foreach (var slaLevel in slaLevels)
                    {
                        if (slaLevel.SlaConditionID == (int)slaCondition && slaLevel.SendEmail)
                        {
                            int notifyMinute = (slaLevel.OffsetDays * 24 * 60)
                                            + (slaLevel.OffsetHours * 60)
                                            + slaLevel.OffsetMinutes; // Chuyển thời gian sang phút

                            string timeDue = slaLevel.OffsetDays > 0 ? $"{slaLevel.OffsetDays} ngày " : string.Empty;
                            timeDue = slaLevel.OffsetHours > 0 ? timeDue + $"{slaLevel.OffsetHours} giờ " : timeDue;
                            timeDue = slaLevel.OffsetMinutes > 0 ? timeDue + $"{slaLevel.OffsetMinutes} phút " : timeDue;

                            userGroupIds.AddRange(slaLevel.UserGroups.Select(ug => ug.UserGroupsID));
                            ProcessOverdue(notifyMinute, slaConfig.FactoryID, timeDue, userGroupIds);
                        }
                    }
                }
            }
            //else
            //{
            //    var beforeHours =
            //    WorkPermitConfig
            //        .CompletionNotificationTimes
            //        .AfterTimes
            //        .Select(x => Convert.ToInt32(x.TotalHours))
            //        .OrderByDescending(x => x)
            //        .ToList();

            //    foreach (var notifyHour in beforeHours)
            //    {
            //        ProcessOverdue(notifyHour * 60, 0, $"{notifyHour} giờ");
            //    }
            //}
        }

        private void ProcessOverdue(int notifyMinute, int factoryId, string timeDue, List<int> userGroupIds)
        {
            var now = DateTime.Now;
            var window = TimeSpan.FromMinutes(WorkPermitConfig.NotificationWindowMinutes);
            var startDate = now.Subtract(window).AddMinutes(-notifyMinute);
            var endDate = now.AddMinutes(-notifyMinute);

            var workTickets = _unitOfWork
                .ElogbookRepository<WorkTicket>()
                .GetQuery(x =>
                    x.WorkTicketStatus != (int)EnumTicketStatus.Completed
                    && !x.IsDeleted
                    && x.EndDate > startDate
                    && x.EndDate <= endDate);

            if (factoryId > 0)
            {
                workTickets = workTickets.Select(x => x).Where(x => x.FactoryID == factoryId);
            }

            var workTicketList = workTickets.ToList();

            foreach (var ticket in workTicketList)
            {
                if (HasSentNotification(ticket.OverdueHistory, notifyMinute))
                {
                    continue;
                }

                // Push mail service
                var processEmailRequestModel = new ProcessEmailRequestModel
                {
                    TimeDue = timeDue,
                    // Đánh dấu thông báo gửi trễ hạn
                    NotificationTypeValue = 3,
                    UserGroupIds = userGroupIds,
                    WorkTicketID = ticket.ID,
                    CurrentUserID = ticket.CreatedUserID.GetValueOrDefault(),
                    FactoryID = ticket.FactoryID.GetValueOrDefault(),
                    ModuleNotification = ticket.ScreenType == (int)EnumModuleNotification.PCTD ? EnumModuleNotification.PCTD : EnumModuleNotification.TCNH
                };

                if (_emailService.ProcessEmailWorkTicketCompletion(processEmailRequestModel))
                {
                    var entity = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(ticket.ID);
                    if (entity != null)
                    {
                        entity.OverdueHistory = AddNotificationHistory(entity.OverdueHistory, notifyMinute);
                        _unitOfWork.ElogbookRepository<WorkTicket>().Update(entity);
                        _unitOfWork.Complete();
                    }
                }
            }
        }

        private static HashSet<int> GetHistoryHours(string history)
        {
            var result = new HashSet<int>();

            if (string.IsNullOrWhiteSpace(history))
            {
                return result;
            }

            foreach (var item in history.Split(
                new[] { '|' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(item.Trim(), out int hour))
                {
                    result.Add(hour);
                }
            }

            return result;
        }

        private static bool HasSentNotification(string history, int notifyHour)
        {
            return GetHistoryHours(history).Contains(notifyHour);
        }

        private static string AddNotificationHistory(string history, int notifyHour)
        {
            var hours = GetHistoryHours(history);

            hours.Add(notifyHour);

            return string.Join(
                "|",
                hours
                    .OrderByDescending(x => x));
        }

        private List<SlaConfigDetailModel> GetSlaConfig(EnumSlaCondition enumSlaCondition)
        {
            var enties = _unitOfWork.ElogbookRepository<SlaConfig>()
                .GetQuery(x => !x.IsDeleted && x.IsActive && x.SlaCategoryID == (int)EnumSlaCategory.PCT
                               && x.SlaLevels.Any(sl => !sl.IsDeleted && sl.SlaConditionID == (int)enumSlaCondition && sl.SendEmail))
                .Select(e => new SlaConfigDetailModel()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Description = e.Description,
                    IsActive = e.IsActive,
                    FactoryID = e.FactoryID,
                    Levels = e.SlaLevels.Where(sl => !sl.IsDeleted && sl.SlaConditionID == (int)enumSlaCondition && sl.SendEmail)
                    .Select(sl => new SlaLevelModel()
                    {
                        ID = sl.ID,
                        LevelOrder = sl.LevelOrder,
                        ConditionType = sl.SlaCondition.Name,
                        SlaConditionID = sl.SlaConditionID,
                        OffsetDays = sl.OffsetDays,
                        OffsetHours = sl.OffsetHours,
                        OffsetMinutes = sl.OffsetMinutes,
                        IncludeHolidays = sl.IncludeHolidays,
                        IncludeWeekends = sl.IncludeWeekends,
                        WorkingHoursOnly = sl.WorkingHoursOnly,
                        SendEmail = sl.SendEmail,
                        EmailTemplateId = sl.EmailTemplateID,
                        UserGroups = sl.SlaLevelUserGroups.Where(u => !u.IsDeleted).Select(r => new SlaUserGroupModel
                        {
                            ID = r.ID,
                            UserGroupsID = r.UserGroupsID,
                            UserGroupName = r.UserGroups.UserGroupName,
                        }).ToList()
                    }).OrderBy(sl => sl.SlaConditionID).ToList()
                }).OrderByDescending(x => x.ID).ToList();

            return enties;

        }

        #endregion
    }
}
