using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Request.WorkTicket;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class WorkTicketBusiness : IWorkTicketBusiness
    {
        private readonly IRegistrationTicketService _registrationTicketService;
        private readonly IWorkTicketService _workTicketService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTicketService"></param>
        public WorkTicketBusiness(IWorkTicketService workTicketService, IRegistrationTicketService registrationTicketService)
        {
            _workTicketService = workTicketService;
            _registrationTicketService = registrationTicketService;
        }

        /// <summary>
        /// Tìm kiếm một Phiếu công tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<WorkTicketDetailModel> GetWorkTicketById(int id)
        {
            var responseModel = new ResponseModel<WorkTicketDetailModel>();

            try
            {
                responseModel.Data = _workTicketService.GetWorkTicketById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm một Phiếu công tác theo ID (mobile)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isWaitingTicket"></param>
        /// <returns></returns>
        public ResponseModel<WorkTicketDetailMobileModel> GetWorkTicketByIdMobile(int id, bool isWaitingTicket)
        {
            var responseModel = new ResponseModel<WorkTicketDetailMobileModel>();

            try
            {
                responseModel.Data = _workTicketService.GetWorkTicketByIdMobile(id, isWaitingTicket);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Từ chối cấp Phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> RejectTicket(AddTicketStatusHistoryModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidRejectTicket(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                string lstAttachtment = "";
                if (requestModel.AttachmentsID != null)
                {
                    lstAttachtment = string.Join(",", requestModel.AttachmentsID.ToArray());
                }

                ResponseModel<bool> isChange = ChangeStatusTicket(requestModel.RegistrationTicketID, (int)EnumTicketStatus.Reject, requestModel.Description, requestModel.CurrentUserID, lstAttachtment);
                if (isChange.Data)
                {
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    _errorMessages.Add("Từ chối cấp phiếu thất bại");
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.Data = false;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Đồng ý cấp Phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> ApproveTicket(AddNewWorkTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();
            try
            {
                // validate input request
                if (!IsValidApproveTicket(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.WorkTicketID > 0 || requestModel.MajorRepairID > 0)
                {
                    responseModel = _workTicketService.SCLApproveTicket(requestModel, true);
                }
                else
                {
                    if (_workTicketService.CheckDuplicateData(requestModel.WorkTicketNumber))
                    {
                        _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_EXIST);
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }

                    responseModel = _workTicketService.AddWorkTicketNew(requestModel, true);
                }

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> ChangeStatusTicket(int registrationTicketID, int statusValue, string description, int userID, string lstAttachment)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                // kiểm tra phiếu đăng ký công tác tồn tại
                if (_workTicketService.CheckExistRegistrationTicketData(registrationTicketID))
                {
                    responseModel.Data = _registrationTicketService.UpdateStatusRegistrationTicket(registrationTicketID, statusValue, description, userID, lstAttachment);
                }
                else
                {
                    _errorMessages.Add(CommonConstant.DATA_DOES_NOT_EXIST);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.Data = false;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm tất cả Phiếu công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkTickets(SearchWorkTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.GetWorkTickets(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tìm kiếm một Phiếu công tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<DataCountModel> GetWorkTicketsCountMobile(SearchWorkTicketMobileModel requestModel)
        {
            var responseModel = new ResponseModel<DataCountModel>();

            try
            {
                responseModel.Data = _workTicketService.GetWorkTicketsCountMobile(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm tất cả Phiếu công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkTicketsMobile(SearchWorkTicketMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.GetWorkTicketsMobile(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Thêm mới phiếu công tác mà không cần phải có phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddWorkTicketNew(AddNewWorkTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();
            try
            {
                // validate input request
                if (!IsValidApproveTicket(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // check duplicate
                if (_workTicketService.CheckDuplicateData(requestModel.WorkTicketNumber))
                {
                    _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_EXIST);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel = _workTicketService.AddWorkTicketNew(requestModel, false);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Thêm thông tin PCT cho phép công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> AddAllowedInfo(AddAllowedInfoWithoutAcceptorModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workTicketService.AddAllowedInfo(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Thêm mới phiếu công tác bằng cách nhập liệu dữ liệu cũ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddWorkTicketOld(AddOldWorkTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();
            try
            {
                // Validate GeneralInformation
                if (!IsValidAddOld(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }
                // check duplicate
                if (_workTicketService.CheckDuplicateData(requestModel.WorkTicketNumber))
                {
                    _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_EXIST);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }
                responseModel = _workTicketService.AddWorkTicketOld(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm những phiếu đăng ký công tác đang chờ cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetRegistrationTickets(SearchRegistrationTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();
            try
            {
                responseBaseModel = _workTicketService.GetRegistrationTickets(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }
            return responseBaseModel;
        }

        /// <summary>
        /// Lấy thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<AllowedInfoModel> GetAcceptanceInfoTickets(int workTicketID, int? screenType)
        {
            var responseModel = new ResponseModel<AllowedInfoModel>();

            try
            {
                responseModel.Data = _workTicketService.GetAcceptanceInfoTickets(workTicketID, screenType);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {workTicketID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tạo thông tin tiếp nhận nơi làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel CreateAdmissionWorkplace(AddReceiveWorkplaceMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.CreateAdmissionWorkplace(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tạo thông tin công tác hàng ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel CreateDailyWorkInfo(AddDetailTicketMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.CreateDailyWorkInfo(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddEditWorkerInformation(AddEditWorkerInformationModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.AddEditWorkerInformation(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Xóa thông tin công tác hàng ngày
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteWorkerInformation(int ID, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.DeleteWorkerInformation(ID, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {ID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tạo Thông tin địa điểm công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddEditWorkPlacement(AddEditWorkPlacementModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.AddEditWorkPlacement(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Xóa  địa điểm làm việc
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteWorkPlacement(int ID, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.DeleteWorkPlacement(ID, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {ID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tạo chi tiết kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel CreateInfoEndTicket(AddEditWorkFinishMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.CreateInfoEndTicket(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về danh sách thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkerInformations(int workTicketID, int pageNumber, int pageSize, bool isMobile)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.GetWorkerInformations(workTicketID, pageNumber, pageSize, isMobile);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {workTicketID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
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
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.GetWorkPlacements(workTicketID, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {workTicketID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về danh sách thông tin địa điểm công tác hằng ngày
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkPlacements(int workTicketID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.GetWorkPlacements(workTicketID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {workTicketID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Hoàn thành phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateWorkFinish(UpdateWorkFinishModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workTicketService.UpdateWorkFinish(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác ở ngày cụ thể
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public ResponseBaseModel AddWorkerInformationForDate(int workTicketId, int currentUserID)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workTicketService.AddWorkerInformationForDate(workTicketId, currentUserID);
            }
            catch (Exception ex)
            {
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {workTicketId} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidRejectTicket(AddTicketStatusHistoryModel requestModel)
        {
            // lý do từ chối là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.Description))
            {
                _errorMessages.Add(CommonConstant.REASON_REQUIRED);
            }

            // phiếu đăng ký công tác là thông tin bắt buộc
            if (requestModel.RegistrationTicketID < 0)
            {
                _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        private bool IsValidApproveTicket(AddNewWorkTicketModel requestModel)
        {
            // số phiếu đăng ký công tác bắt buộc
            if (requestModel.MajorRepairID < 1 && string.IsNullOrWhiteSpace(requestModel.WorkTicketNumber))
            {
                _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_REQUIRED);
            }

            if (requestModel.Acceptor == null)
            {
                _errorMessages.Add(CommonConstant.ACCEPTOR_REQUIRED);
            }

            // địa điểm tiến hành công tác là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.WorkPlacement))
            {
                _errorMessages.Add(CommonConstant.WORK_PLACEMENT_REQUIRED);
            }

            // Loại phiếu
            if (requestModel.WorkTicketType < 1)
            {
                _errorMessages.Add(CommonConstant.TICKET_TYPE_REQUIRED);
            }

            // Nội dung công tác là thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.WorkContent))
            {
                _errorMessages.Add(CommonConstant.CONTENT_OF_WORK_REQUIRED);
            }

            // Thời gian tiến hành công việc (bắt đầu)
            if (string.IsNullOrWhiteSpace(requestModel.StartDate))
            {
                _errorMessages.Add(CommonConstant.START_DATE);
            }

            // Thời gian tiến hành công việc (kết thúc)
            if (string.IsNullOrWhiteSpace(requestModel.EndDate))
            {
                _errorMessages.Add(CommonConstant.END_DATE);
            }

            if (requestModel.ScreenType != null && requestModel.ScreenType == 2)
            {
                if (string.IsNullOrWhiteSpace(requestModel.WorkScope))
                {
                    _errorMessages.Add(CommonConstant.WORK_SCOPE_REQUIRED);
                }
                if (requestModel.WorkTicketFeatureID < 1)
                {
                    _errorMessages.Add(CommonConstant.WORK_TICKET_FEATURE_REQUIRED);
                }
            }

            if (requestModel.ScreenType == null && requestModel.ScreenType == 1 && requestModel.Commander == null)
            {
                _errorMessages.Add(CommonConstant.COMMANDER_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        private bool IsValidAddOld(AddOldWorkTicketModel requestModel)
        {
            // số phiếu đăng ký công tác bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.WorkTicketNumber))
            {
                _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_REQUIRED);
            }

            // địa điểm tiến hành công tác là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.WorkPlacement))
            {
                _errorMessages.Add(CommonConstant.WORK_PLACEMENT_REQUIRED);
            }

            // Nội dung công tác là thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.WorkContent))
            {
                _errorMessages.Add(CommonConstant.CONTENT_OF_WORK_REQUIRED);
            }

            // Thời gian tiến hành công việc (bắt đầu)
            if (string.IsNullOrWhiteSpace(requestModel.StartDate))
            {
                _errorMessages.Add(CommonConstant.START_DATE);
            }

            // Thời gian tiến hành công việc (kết thúc)
            if (string.IsNullOrWhiteSpace(requestModel.EndDate))
            {
                _errorMessages.Add(CommonConstant.END_DATE);
            }

            // Người cho phép là bắt buộc
            if (requestModel.AllowedInfo == null || requestModel.AllowedInfo?.Acceptor == null)
            {
                _errorMessages.Add(CommonConstant.ACCEPTOR_REQUIRED);
            }

            // Người chỉ huy là bắt buộc
            if (requestModel.Commander == null)
            {
                _errorMessages.Add(CommonConstant.COMMANDER_REQUIRED);
            }

            // Người cấp phiếu là bắt buộc
            if (requestModel.Approver == null)
            {
                _errorMessages.Add(CommonConstant.APPROVER_REQUIRED);
            }

            if ((requestModel.ScreenType == null || requestModel.ScreenType == 1) && string.IsNullOrWhiteSpace(requestModel.WorkCondition))
            {
                _errorMessages.Add(CommonConstant.CONDITION_TO_WORK_REQUIRED);
            }

            if (requestModel.ScreenType != null && requestModel.ScreenType == 2)
            {
                // phạm vi là bắt buộc
                if (string.IsNullOrWhiteSpace(requestModel.WorkScope))
                {
                    _errorMessages.Add(CommonConstant.WORK_SCOPE_REQUIRED);
                }

                // điều kiện tiến hành công việc bắt buộc
                if (requestModel.ApprovedUnitID < 1)
                {
                    _errorMessages.Add(CommonConstant.APPROVE_UNIT_REQUIRED);
                }

                // Lý do là bắt buộc
                if (requestModel.WorkTicketType < 1)
                {
                    _errorMessages.Add(CommonConstant.REASON_REQUIRED);
                }

                // Loại phiếu
                if (requestModel.WorkTicketFeatureID < 1)
                {
                    _errorMessages.Add(CommonConstant.TICKET_TYPE_REQUIRED);
                }

                // số lượng nhân viên
                if (string.IsNullOrEmpty(requestModel.ReceiveWorkplace?.AmountPersons))
                {
                    _errorMessages.Add(CommonConstant.AMOUNT_PERSON_REQUIRED);
                }
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Tải file pdf
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportPdfFile(int id)
        {
            try
            {
                return _workTicketService.ExportPdfFile(id);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public ResponseBaseModel AddSubAllowedInfo(AddSubAllowedInfoModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workTicketService.AddSubAllowedInfo(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel AddWorkDailyModel(AddWorkDailyMobileModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workTicketService.AddWorkDailyModel(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteSubAllowedInfo(int ID, int userID)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workTicketService.DeleteSubAllowedInfo(ID, userID);
            }
            catch (Exception ex)
            {
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {ID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel UpdateFlagNote(int workTicketID, bool isFlagNote, int currentUserID)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workTicketService.UpdateFlagNote(workTicketID, isFlagNote, currentUserID);
            }
            catch (Exception ex)
            {
                responseModel.Content = "Cập nhật không thành công!";
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"workTicketID: {workTicketID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
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
            var responseModel = new ResponseBaseModel();
            _errorMessages = new List<string>();
            try
            {
                return _workTicketService.GetWorkDailys(workTicketID, pageNumber, pageSize, isMobile);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseBaseModel GetWorker(int workTicketID)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workTicketService.GetStakeholders(workTicketID, RolesStakeholder.WorkParticipants);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"workTicketID: {workTicketID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tổ chức thực hiện (Thủy cơ nhiệt hóa)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddWorkPlanInfo(AddWorkPlanInfoModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workTicketService.AddWorkPlanInfo(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Trả về thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseModel<WorkPlanInfoModel> GetWorkPlanInfo(int workTicketID)
        {
            var responseModel = new ResponseModel<WorkPlanInfoModel>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workTicketService.GetWorkPlanInfo(workTicketID);
                return responseModel;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Trả về danh sách vị trí làm việc và danh sách nhân viên (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseModel<List<ContentPositionModel>> GetWorkLocations(int workTicketID)
        {
            var responseModel = new ResponseModel<List<ContentPositionModel>>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workTicketService.GetWorkLocations(workTicketID);
                return responseModel;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<WaitingNumberTicketModel> GetNumberWaitingRegistrationTicket(int productionPlantID)
        {
            var responseModel = new ResponseModel<WaitingNumberTicketModel>();

            try
            {
                responseModel.Data = _workTicketService.GetNumberWaitingRegistrationTicket(productionPlantID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Lấy dữ liệu không thành công!");
                responseModel.Data = new WaitingNumberTicketModel();
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"productionPlantID: {productionPlantID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Trả về chi tiết kết thúc công tác (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkFinish(int workTicketID)
        {
            var responseModel = new ResponseBaseModel();
            _errorMessages = new List<string>();
            try
            {
                return _workTicketService.GetWorkFinishDetail(workTicketID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa thông tin công tác hàng ngày
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteStaffInformation(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.DeleteStaffInformation(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách nhân viên trong và ngoài công ty
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetListStaff(SearchStaffModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            _errorMessages = new List<string>();
            try
            {
                return _workTicketService.GetListStaff(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.Content = ex.Message;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật chi tiết thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateAllowedInfoDetail(AddAllowedInfoDetailModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workTicketService.UpdateAllowedInfoDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

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
            try
            {
                responseModel = _workTicketService.UpdateAllowedInfo(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

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
            try
            {
                responseModel = _workTicketService.UpdateSafetyMeasure(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật tổ chức thực hiện (Cập nhật số lượng người và đơn vị công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateReceiveWorkplace(UpdateReceiveWorkplaceModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workTicketService.UpdateReceiveWorkplace(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa nhóm thông tin công tác hàng ngày
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseBaseModel DeleteStaffInformationGroup(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketService.DeleteStaffInformationGroup(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> CheckExistTicketNumber(string workTicketNumber, int screenType)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel.Data = _workTicketService.CheckExistTicketNumber(workTicketNumber, screenType);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"workTicketNumber: {workTicketNumber}, screenType: {screenType} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Reset số phiếu không sử dụng
        /// </summary>
        public void ResetTicketNumber()
        {
            try
            {
                _workTicketService.ResetTicketNumber();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                    Environment.NewLine + "Reset số phiếu không thành công!" + Environment.NewLine + ex.ToString());
            }
        }

        public ResponseModel<bool> DeleteWorkTicket(int workTicketId, int userId)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                return _workTicketService.DeleteWorkTicket(workTicketId, userId);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"workTicketId: {workTicketId}," + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateStatus(int workTicketId, EnumTicketStatus status, string description, int currentUserId)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                return _workTicketService.UpdateStatus(workTicketId, status, description, currentUserId);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + $"workTicketId: {workTicketId}," + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public void SendEmailWorkTicketCompletionReminder()
        {
            try
            {
                _workTicketService.SendEmailWorkTicketCompletionReminder();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                    Environment.NewLine + "Gửi email không thành công!" + Environment.NewLine + ex.ToString());
            }
        }
    }
}
