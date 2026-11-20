using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response.Manipulation;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    public class RegistrationManipulationBusiness : IRegistrationManipulationBusiness
    {
        private readonly IRegistrationManipulationService _registrationManipulationService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="registrationManipulationTicketService"></param>
        public RegistrationManipulationBusiness(IRegistrationManipulationService registrationManipulationService)
        {
            _registrationManipulationService = registrationManipulationService;
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel Find(SearchRegistrationManipulationTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _registrationManipulationService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Thêm mới phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> Add(AddEditTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                var isSave = false;
                // Khi nhấn nút "Gửi phiếu đăng ký"
                if (!requestModel.IsSave)
                {
                    // Kiểm tra dữ dữ liệu bắt buộc trước khi lưu
                    if (!IsValidData(requestModel))
                    {
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        isSave = true;
                    }
                }
                // Khi bấm nút "Lưu"
                else
                {
                    isSave = true;
                }

                // Nếu dữ liệu được nhập đầy đủ thì thực hiện
                if (isSave && responseModel.ErrorMessages.Count == 0)
                {
                    responseModel.Data = _registrationManipulationService.Add(requestModel);
                }

                // Nếu có lỗi trong quá trình lưu thì trả về lỗi để kiểm tra
                if (responseModel.Data < 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = 0;
                    responseModel.ErrorMessages.Add(CommonConstant.MANIPULATION_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra ràng buộc các trường được nhập
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditTicketModel requestModel)
        {
            _errorMessages = new List<string>();

            // Công ty là bắt buộc
            if (requestModel.FactoryID <= 0)
            {
                _errorMessages.Add(CommonConstant.FACTORY_COMPANY_REQUIRED);
            }

            // Tên phiếu thao tác
            if (string.IsNullOrEmpty(requestModel.TicketName))
            {
                _errorMessages.Add(CommonConstant.MANIPULATION_TICKET_NAME);
            }

            // Người duyệt phiếu là thông tin bắt buộc
            if (requestModel.ApprovedUserID <= 0)
            {
                _errorMessages.Add(CommonConstant.APPROVER_USER);
            }

            // kiểm tra thông tin người giám sát và  người thao tác
            if (requestModel.Participants.Count == 0)
            {
                _errorMessages.Add(CommonConstant.APPROVER_USER);
            }
            else
            {
                if (!requestModel.Participants.Exists(x => x.Roles == (int)UserInTicketRoles.Supervisor))
                {
                    _errorMessages.Add(CommonConstant.SUPPERVISOR_USER);
                }

                if (!requestModel.Participants.Exists(x => x.Roles == (int)UserInTicketRoles.Worker))
                {
                    _errorMessages.Add(CommonConstant.WORKERS_USER);
                }
            }

            // Mục đích thao tác
            if (string.IsNullOrEmpty(requestModel.Purpose))
            {
                _errorMessages.Add(CommonConstant.MANIPULATION_TICKET_NAME);
            }

            // thời gian dự kiến (bắt đầu)
            if (requestModel.StartTime == null)
            {
                _errorMessages.Add(CommonConstant.START_DATE_EXPECTED);
            }

            // thời gian dự kiến (kết thúc)
            if (requestModel.EndTime == null)
            {
                _errorMessages.Add(CommonConstant.END_DATE_EXPECTED);
            }

            // Đơn vị đề nghị thao tác
            if (string.IsNullOrEmpty(requestModel.RecommendedUnit))
            {
                _errorMessages.Add(CommonConstant.MANIPULATION_TICKET_NAME);
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// cập nhật phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> Update(AddEditTicketModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!requestModel.IsSave)
                {
                    if (!IsValidData(requestModel))
                    {
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        if (responseModel.ErrorMessages.Count == 0)
                            responseModel.Data = _registrationManipulationService.Update(requestModel);
                    }
                }
                else
                {
                    responseModel.Data = _registrationManipulationService.Update(requestModel);
                }

                if (!responseModel.Data)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<ManipulationTicketDetailModel> GetManipulationTicketDetailById(int id)
        {
            var responseModel = new ResponseModel<ManipulationTicketDetailModel>();

            try
            {
                responseModel.Data = _registrationManipulationService.GetManipulationTicketDetailById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusValue"></param>
        /// <param name="description"></param>
        /// <param name="attackments"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateStatus(int id, int statusValue, string description, string attackments, int userID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _registrationManipulationService.UpdateStatus(id, statusValue, description, userID, attackments);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xoá phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteTicket(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _registrationManipulationService.DeleteTicket(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm trạng thái phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetStatusById(int id)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _registrationManipulationService.GetStatusById(id);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Duyệt phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddApprovalTicket(AddManipulationTicketInfoModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (responseModel.ErrorMessages.Count == 0)
                    responseModel.Data = _registrationManipulationService.AddApprovalTicket(requestModel);

                // Nếu có lỗi trong quá trình lưu thì trả về lỗi để kiểm tra
                if (responseModel.Data < 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = 0;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm phiếu mẫu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindTemplate(Core.Models.Request.Manipulation.SearchTemplateTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _registrationManipulationService.FindTemplate(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// tạo phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddTemplateTicket(AddEditTemplateTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                // Kiểm tra dữ dữ liệu bắt buộc trước khi lưu
                if (!IsValidDataTemplate(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    // Kiểm tra tồn tại tên phiếu thao tác
                    if (_registrationManipulationService.CheckDuplicateDataTemplate(0, requestModel, true))
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_MANIPULATION_TEMPLATE_TICKET_NAME);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }

                    if (responseModel.ErrorMessages.Count == 0)
                        responseModel.Data = _registrationManipulationService.AddTemplateTicket(requestModel);
                }

                // Nếu có lỗi trong quá trình lưu thì trả về lỗi để kiểm tra
                if (responseModel.Data < 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = 0;
                    responseModel.ErrorMessages.Add(CommonConstant.MANIPULATION_TEMPLATE_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidDataTemplate(AddEditTemplateTicketModel requestModel)
        {
            _errorMessages = new List<string>();

            // Công ty là bắt buộc
            if (string.IsNullOrEmpty(requestModel.IdFactorysTemplateTicket))
            {
                _errorMessages.Add(CommonConstant.FACTORY_COMPANY_REQUIRED);
            }

            // Tên phiếu thao tác mẫu
            if (string.IsNullOrEmpty(requestModel.TemplateTicketName))
            {
                _errorMessages.Add(CommonConstant.MANIPULATION_TEMPLATE_TICKET_NAME);
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Xoá phiếu thao tác mẫu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteTemplateTicket(int id, int userId)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _registrationManipulationService.DeleteTemplateTicket(id, userId);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// cập nhật phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateTemplateTicket(AddEditTemplateTicketModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidDataTemplate(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    // Kiểm tra tồn tại tên phiếu thao tác
                    if (_registrationManipulationService.CheckDuplicateDataTemplate(requestModel.ID, requestModel, false))
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_MANIPULATION_TEMPLATE_TICKET_NAME);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }

                    if (responseModel.ErrorMessages.Count == 0)
                        responseModel.Data = _registrationManipulationService.UpdateTemplateTicket(requestModel);
                }

                if (!responseModel.Data)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm phiếu thao tác mẫu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<DetailTemplateTicketModel> FindTemplateId(int id)
        {
            var responseModel = new ResponseModel<DetailTemplateTicketModel>();

            try
            {
                responseModel.Data = _registrationManipulationService.FindTemplateId(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm phiếu mẫu đang active
        /// </summary>
        /// <returns></returns>
        public ResponseBaseModel FindTemplateActive()
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _registrationManipulationService.FindTemplateActive();
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + "FindTemplateActive" + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
