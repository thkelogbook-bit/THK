using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.WorkPermit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class WorkPermitBusiness : IWorkPermitBusiness
    {
        private readonly IRegistrationWorkPermitService _registrationWorkPermitService;
        private readonly IWorkPermitService _workPermitService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTicketService"></param>
        public WorkPermitBusiness(IWorkPermitService workPermitService, IRegistrationWorkPermitService registrationWorkPermitService)
        {
            _workPermitService = workPermitService;
            _registrationWorkPermitService = registrationWorkPermitService;
        }

        /// <summary>
        /// Tìm kiếm một lệnh công tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<LctWorkTicketDetailModel> GetWorkTicketById(int id)
        {
            var responseModel = new ResponseModel<LctWorkTicketDetailModel>();

            try
            {
                responseModel.Data = _workPermitService.GetWorkTicketById(id);
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
        /// Tìm kiếm một lệnh công tác theo ID (mobile)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isWaitingTicket"></param>
        /// <returns></returns>
        public ResponseModel<LctWorkTicketDetailMobileModel> GetWorkTicketByIdMobile(int id, bool isWaitingTicket)
        {
            var responseModel = new ResponseModel<LctWorkTicketDetailMobileModel>();

            try
            {
                responseModel.Data = _workPermitService.GetWorkTicketByIdMobile(id, isWaitingTicket);
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
        /// Từ chối cấp lệnh công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> RejectTicket(LctAddTicketStatusHistoryModel requestModel)
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

                ResponseModel<bool> isChange = ChangeStatusTicket(requestModel.RegistrationTicketID, (int)TicketStatus.Reject, requestModel.Description, requestModel.CurrentUserID, lstAttachtment);
                if (isChange.Data)
                {
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    _errorMessages.Add("Từ chối cấp lệnh thất bại");
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
        /// Đồng ý cấp lệnh công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> ApproveTicket(LctAddNewWorkTicketModel requestModel)
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

                // check does exist registration
                if (requestModel.RegistrationTicketID <= 0)
                {
                    _errorMessages.Add(CommonConstant.REGISTRAITON_TICKET_REQUIRED);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (_workPermitService.CheckDuplicateData(requestModel.WorkTicketNumber))
                {
                    _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_EXIST);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel = _workPermitService.AddWorkTicketNew(requestModel, true);

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
        /// Cập nhật trạng thái lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> ChangeStatusTicket(int registrationTicketID, int statusValue, string description, int userID, string lstAttachment)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                // kiểm tra lệnh đăng ký công tác tồn tại
                if (_workPermitService.CheckExistRegistrationTicketData(registrationTicketID))
                {
                    responseModel.Data = _registrationWorkPermitService.UpdateStatusRegistrationTicket(registrationTicketID, statusValue, description, userID, lstAttachment);
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
        /// Tìm kiếm tất cả lệnh công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkTickets(LctSearchWorkTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.GetWorkTickets(requestModel);
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
        /// Tìm kiếm một lệnh công tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<DataCountModel> GetWorkTicketsCountMobile(LctSearchWorkTicketMobileModel requestModel)
        {
            var responseModel = new ResponseModel<DataCountModel>();

            try
            {
                responseModel.Data = _workPermitService.GetWorkTicketsCountMobile(requestModel);
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
        /// Tìm kiếm tất cả lệnh công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetWorkTicketsMobile(LctSearchWorkTicketMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.GetWorkTicketsMobile(requestModel);
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
        /// Thêm mới lệnh công tác mà không cần phải có lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddWorkTicketNew(LctAddNewWorkTicketModel requestModel)
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
                if (_workPermitService.CheckDuplicateData(requestModel.WorkTicketNumber))
                {
                    _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_EXIST);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel = _workPermitService.AddWorkTicketNew(requestModel, false);

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
        public ResponseModel<bool> AddAllowedInfo(LctAddAllowedInfoWithoutAcceptorModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workPermitService.AddAllowedInfo(requestModel);
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
        /// Thêm mới lệnh công tác bằng cách nhập liệu dữ liệu cũ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddWorkTicketOld(LctAddOldWorkTicketModel requestModel)
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
                if (_workPermitService.CheckDuplicateData(requestModel.WorkTicketNumber))
                {
                    _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_EXIST);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }
                responseModel = _workPermitService.AddWorkTicketOld(requestModel);
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
        /// Tìm kiếm những lệnh đăng ký công tác đang chờ cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetRegistrationTickets(LctSearchRegistrationTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();
            try
            {
                responseBaseModel = _workPermitService.GetRegistrationTickets(requestModel);
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
        public ResponseModel<LctAllowedInfoModel> GetAcceptanceInfoTickets(int workTicketID, int? screenType)
        {
            var responseModel = new ResponseModel<LctAllowedInfoModel>();

            try
            {
                responseModel.Data = _workPermitService.GetAcceptanceInfoTickets(workTicketID, screenType);
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
        public ResponseBaseModel CreateAdmissionWorkplace(LctAddReceiveWorkplaceMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.CreateAdmissionWorkplace(requestModel);
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
        public ResponseBaseModel CreateDailyWorkInfo(LctAddDetailTicketMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.CreateDailyWorkInfo(requestModel);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.WorkTicketBusiness.ToString(),
                                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + "Tạo thông tin công tác hàng ngày");
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
        public ResponseBaseModel AddEditWorkerInformation(LctAddEditWorkerInformationModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.AddEditWorkerInformation(requestModel);
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
                responseBaseModel = _workPermitService.DeleteWorkerInformation(ID, userID);
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
        public ResponseBaseModel AddEditWorkPlacement(LctAddEditWorkPlacementModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.AddEditWorkPlacement(requestModel);
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
                responseBaseModel = _workPermitService.DeleteWorkPlacement(ID, userID);
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
        public ResponseBaseModel CreateInfoEndTicket(LctAddEditWorkFinishMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.CreateInfoEndTicket(requestModel);
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
                responseBaseModel = _workPermitService.GetWorkerInformations(workTicketID, pageNumber, pageSize, isMobile);
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
                responseBaseModel = _workPermitService.GetWorkPlacements(workTicketID, pageNumber, pageSize);
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
                responseBaseModel = _workPermitService.GetWorkPlacements(workTicketID);
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
        /// Hoàn thành lệnh công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateWorkFinish(LctUpdateWorkFinishModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workPermitService.UpdateWorkFinish(requestModel);
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
                return _workPermitService.AddWorkerInformationForDate(workTicketId, currentUserID);
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

        private bool IsValidRejectTicket(LctAddTicketStatusHistoryModel requestModel)
        {
            // lý do từ chối là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.Description))
            {
                _errorMessages.Add(CommonConstant.REASON_REQUIRED);
            }

            // lệnh đăng ký công tác là thông tin bắt buộc
            if (requestModel.RegistrationTicketID < 0)
            {
                _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        private bool IsValidApproveTicket(LctAddNewWorkTicketModel requestModel)
        {
            // số lệnh đăng ký công tác bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.WorkTicketNumber))
            {
                _errorMessages.Add(CommonConstant.NUMBER_WORD_TICKET_REQUIRED);
            }

            // địa điểm tiến hành công tác là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.WorkPlacement))
            {
                _errorMessages.Add(CommonConstant.WORK_PLACEMENT_REQUIRED);
            }

            // Loại lệnh
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

        private bool IsValidAddOld(LctAddOldWorkTicketModel requestModel)
        {
            // số lệnh đăng ký công tác bắt buộc
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
            //if (requestModel.AllowedInfo == null || requestModel.AllowedInfo?.Acceptor == null)
            //{
            //    _errorMessages.Add(CommonConstant.ACCEPTOR_REQUIRED);
            //}

            // Người chỉ huy là bắt buộc
            //if (requestModel.Commander == null)
            //{
            //    _errorMessages.Add(CommonConstant.COMMANDER_REQUIRED);
            //}

            // Người cấp lệnh là bắt buộc
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
                    _errorMessages.Add(CommonConstant.REASON_TYPE_REQUIRED);
                }

                // Loại lệnh
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
                return _workPermitService.ExportPdfFile(id);
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

        public ResponseBaseModel AddSubAllowedInfo(LctAddSubAllowedInfoModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workPermitService.AddSubAllowedInfo(requestModel);
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

        public ResponseBaseModel AddWorkDailyModel(LctAddWorkDailyMobileModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workPermitService.AddWorkDailyModel(requestModel);
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
                return _workPermitService.DeleteSubAllowedInfo(ID, userID);
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
                return _workPermitService.UpdateFlagNote(workTicketID, isFlagNote, currentUserID);
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
                return _workPermitService.GetWorkDailys(workTicketID, pageNumber, pageSize, isMobile);
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
                return _workPermitService.GetStakeholders(workTicketID, RolesStakeholder.WorkParticipants);
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
        public ResponseBaseModel AddWorkPlanInfo(LctAddWorkPlanInfoModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            try
            {
                return _workPermitService.AddWorkPlanInfo(requestModel);
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
        public ResponseModel<LctWorkPlanInfoModel> GetWorkPlanInfo(int workTicketID)
        {
            var responseModel = new ResponseModel<LctWorkPlanInfoModel>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workPermitService.GetWorkPlanInfo(workTicketID);
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
        public ResponseModel<List<LctContentPositionModel>> GetWorkLocations(int workTicketID)
        {
            var responseModel = new ResponseModel<List<LctContentPositionModel>>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workPermitService.GetWorkLocations(workTicketID);
                return responseModel;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<LctWaitingNumberTicketModel> GetNumberWaitingRegistrationTicket(int productionPlantID)
        {
            var responseModel = new ResponseModel<LctWaitingNumberTicketModel>();

            try
            {
                responseModel.Data = _workPermitService.GetNumberWaitingRegistrationTicket(productionPlantID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Lấy dữ liệu không thành công!");
                responseModel.Data = new LctWaitingNumberTicketModel();
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
                return _workPermitService.GetWorkFinishDetail(workTicketID);
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
                responseBaseModel = _workPermitService.DeleteStaffInformation(id, userID);
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
        public ResponseBaseModel GetListStaff(LctSearchStaffModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            _errorMessages = new List<string>();
            try
            {
                return _workPermitService.GetListStaff(requestModel);
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
        public ResponseModel<bool> UpdateAllowedInfoDetail(LctAddAllowedInfoDetailModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workPermitService.UpdateAllowedInfoDetail(requestModel);
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
        public ResponseModel<bool> UpdateAllowedInfo(LctUpdateAllowedInfoModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workPermitService.UpdateAllowedInfo(requestModel);
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
        public ResponseModel<bool> UpdateSafetyMeasure(LctAddSafetyMeasureModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workPermitService.UpdateSafetyMeasure(requestModel);
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
        public ResponseModel<bool> UpdateReceiveWorkplace(LctUpdateReceiveWorkplaceModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel = _workPermitService.UpdateReceiveWorkplace(requestModel);
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
                responseBaseModel = _workPermitService.DeleteStaffInformationGroup(id, userID);
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
                responseModel.Data = _workPermitService.CheckExistTicketNumber(workTicketNumber, screenType);
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
        /// Reset số lệnh không sử dụng
        /// </summary>
        public void ResetTicketNumber()
        {
            try
            {
                _workPermitService.ResetTicketNumber();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                    Environment.NewLine + "Reset số lệnh không thành công!" + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Trả về thông tin thi hành lệnh
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseModel<LctOrderExecutionMobileModel> GetOrderExecutionMobile(int workTicketID)
        {
            var responseModel = new ResponseModel<LctOrderExecutionMobileModel>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workPermitService.GetOrderExecutionMobile(workTicketID);
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
        /// Tạo thông tin thi hành lệnh
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddEditOrderExecutionMobile(AddLctOrderExecutionMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.AddEditOrderExecutionMobile(requestModel);
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
        /// Trả về thông tin kết thúc công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        public ResponseModel<LctdWorkFinishMobileModel> GetWorkFinishMobile(int workTicketID)
        {
            var responseModel = new ResponseModel<LctdWorkFinishMobileModel>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workPermitService.GetWorkFinishMobile(workTicketID);
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
        /// Tạo thông tin kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddEditWorkFinishMobile(LctdAddEditWorkFinishMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.AddEditWorkFinishMobile(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<List<LctStakeholdersResponseModel>> GetLCTDTaskParticipants(int workTicketID)
        {
            var responseModel = new ResponseModel<List<LctStakeholdersResponseModel>>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workPermitService.GetLCTDTaskParticipants(workTicketID);
                return responseModel;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(), ex.ToString());
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<LctContentPositionModel>> GetLCTTCNHTaskParticipants(int workTicketID)
        {
            var responseModel = new ResponseModel<List<LctContentPositionModel>>();
            _errorMessages = new List<string>();
            try
            {
                responseModel.Data = _workPermitService.GetLCTTCNHTaskParticipants(workTicketID);
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
        /// Tạo thông tin tổ chức thực hiện mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel CreateExecutingOrgLCTDMobile(LctAddExecutingOrgMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.CreateExecutingOrgLCTDMobile(requestModel);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.WorkTicketBusiness.ToString(),
                                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + "Tạo thông tin tổ chức thực hiện LCT Điện");
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
        /// Tạo thông tin Trình tự công việc và điều kiện an toàn 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddEditWorkProcessSafetyMobile(AddEditWorkProcessSafetyMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workPermitService.AddEditWorkProcessSafetyMobile(requestModel);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.WorkTicketBusiness.ToString(),
                                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + "Tạo thông tin Trình tự công việc và điều kiện an toàn");
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
