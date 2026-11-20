using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response.WorkPermit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Elogbook.Business
{
    public class RegistrationWorkPermitBusiness : IRegistrationWorkPermitBusiness
    {
        private readonly IRegistrationWorkPermitService _registrationWorkPermitService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationShiftService"></param>
        public RegistrationWorkPermitBusiness(IRegistrationWorkPermitService registrationWorkPermitService)
        {
            _registrationWorkPermitService = registrationWorkPermitService;
        }

        /// <summary>
        /// Thêm lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddRegistrationTicket(LctAddEditRegistrationTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            requestModel.RegistrationTicketType = requestModel.RegistrationTicketType ?? (int)WorkPermitTypeLCT.DIEN;

            try
            {
                var isSave = false;
                // Khi nhấn nút "Gửi lệnh đăng ký"
                if (!requestModel.IsSave)
                {
                    // Kiểm tra dữ liệu bắt buộc trước khi lưu
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
                if (isSave)
                {
                    switch (requestModel.RegistrationTicketType)
                    {
                        case (int)WorkPermitTypeLCT.DIEN:
                            // Kiểm tra tồn tại Số lệnh đăng ký công tác
                            if (_registrationWorkPermitService.CheckDuplicateData(
                                id: 0,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: true,
                                isTicketNumber: true))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_WORK_PERMIT, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }

                            break;

                        case (int)WorkPermitTypeLCT.COTHUYNHIETHOA:
                            // Kiểm tra tồn tại Số lệnh đăng ký công tác
                            if (_registrationWorkPermitService.CheckDuplicateData(
                                id: 0,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: true))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_WORK_PERMIT, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                            break;
                    }

                    if (responseModel.ErrorMessages.Count == 0)
                        responseModel.Data = _registrationWorkPermitService.AddRegistrationTicket(requestModel);
                }

                // Nếu có lỗi trong quá trình lưu thì trả về lỗi để kiểm tra
                if (responseModel.Data < 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = 0;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_WORK_PERMIT);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationWorkPermitBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm những lệnh đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetRegistrationTickets(LctSearchRegistrationTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                requestModel.RegistrationTicketType = requestModel.RegistrationTicketType ?? (int)WorkPermitTypeLCT.DIEN;
                responseBaseModel = _registrationWorkPermitService.GetRegistrationTickets(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationWorkPermitBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// tìm kiếm lệnh đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<LctRegistrationTicketModel> GetRegistrationTicketById(int id)
        {
            var responseModel = new ResponseModel<LctRegistrationTicketModel>();

            try
            {
                responseModel.Data = _registrationWorkPermitService.GetRegistrationTicketById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationWorkPermitBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật trạng thái lệnh đăng ký công tác (Chờ cấp lệnh công tác/Rút lệnh đăng ký)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSendTicket"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateStatusRegistrationTicket(int id, bool isSendTicket, int userID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                if (isSendTicket)
                {
                    responseModel.Data = _registrationWorkPermitService.UpdateStatusRegistrationTicket(id, (int)WorkPermitStatus.Waiting, "Gửi lệnh đăng ký", userID, "");
                }
                else
                {
                    responseModel.Data = _registrationWorkPermitService.UpdateStatusRegistrationTicket(id, (int)WorkPermitStatus.Cancel, "Thu hồi lệnh đăng ký", userID, "");
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationWorkPermitBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật trạng thái lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateRegistrationTicket(LctAddEditRegistrationTicketModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                requestModel.RegistrationTicketType = requestModel.RegistrationTicketType ?? (int)WorkPermitTypeLCT.DIEN;
                if (!requestModel.IsSave)
                {
                    if (!IsValidData(requestModel))
                    {
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        switch (requestModel.RegistrationTicketType)
                        {
                            case (int)WorkPermitTypeLCT.DIEN:
                                // Kiểm tra tồn tại Số lệnh đăng ký công tác
                                if (_registrationWorkPermitService.CheckDuplicateData(
                                    id: requestModel.ID,
                                    ticketNumber: requestModel.TicketNumber,
                                    isAddMode: false,
                                    isTicketNumber: true))
                                {
                                    responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_WORK_PERMIT, requestModel.TicketNumber));
                                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                                }

                                // Kiểm tra tồn tại Căn cứ biên bản số
                                if (_registrationWorkPermitService.CheckDuplicateData(
                                    id: requestModel.ID,
                                    ticketNumber: requestModel.TicketNumber,
                                    isAddMode: false,
                                    isTicketNumber: false))
                                {
                                    responseModel.ErrorMessages.Add(CommonConstant.REGULATION_TICKET);
                                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                                }
                                break;

                            case (int)WorkPermitTypeLCT.COTHUYNHIETHOA:
                                if (_registrationWorkPermitService.CheckDuplicateData(
                                    id: requestModel.ID,
                                    ticketNumber: requestModel.TicketNumber,
                                    isAddMode: false))
                                {
                                    responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_WORK_PERMIT, requestModel.TicketNumber));
                                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                                }
                                break;
                        }

                        if (responseModel.ErrorMessages.Count == 0)
                            responseModel.Data = _registrationWorkPermitService.UpdateRegistrationTicket(requestModel);
                    }
                }
                else
                {
                    switch (requestModel.RegistrationTicketType)
                    {
                        case (int)WorkPermitTypeLCT.DIEN:
                            // Kiểm tra tồn tại Số lệnh đăng ký công tác
                            if (_registrationWorkPermitService.CheckDuplicateData(
                                id: requestModel.ID,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: false,
                                isTicketNumber: true))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_WORK_PERMIT, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                            break;

                        case (int)WorkPermitTypeLCT.COTHUYNHIETHOA:
                            // Kiểm tra tồn tại Số lệnh đăng ký công tác
                            if (_registrationWorkPermitService.CheckDuplicateData(
                                id: requestModel.ID,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: false))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_WORK_PERMIT, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                            break;
                    }

                    if (responseModel.ErrorMessages.Count == 0)
                        responseModel.Data = _registrationWorkPermitService.UpdateRegistrationTicket(requestModel);
                }

                if (!responseModel.Data)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_WORK_PERMIT);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationWorkPermitBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xoá lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteRegistrationTicket(int id, int currentUserID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _registrationWorkPermitService.DeleteRegistrationTicket(id, currentUserID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationWorkPermitBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm danh sách trạng thái lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetStatusRegistrationTicketById(int id)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _registrationWorkPermitService.GetListTicketStatusHistoryById(id);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationWorkPermitBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// kiểm tra ràng buộc giá trị được nhập vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(LctAddEditRegistrationTicketModel requestModel)
        {
            if (requestModel.RegistrationTicketType == (int)WorkPermitTypeLCT.DIEN)
            {
                // Số lệnh đăng ký công tác là bắt buộc
                if (string.IsNullOrWhiteSpace(requestModel.TicketNumber))
                {
                    _errorMessages.Add(CommonConstant.WORK_PERMIT_NUMBER);
                }

                // Đơn vị nhận lệnh
                if (requestModel.ReceiveUnitID < 0)
                {
                    _errorMessages.Add(CommonConstant.RECEIVE_WORK_PERMIT_UNIT);
                }

                // Căn cứ biên bản số
                if (string.IsNullOrWhiteSpace(requestModel.RegulationNumber))
                {
                    _errorMessages.Add(CommonConstant.DOCUMENT_TICKET);
                }

                // Nội dung công việc
                if (string.IsNullOrWhiteSpace(requestModel.WorkContent))
                {
                    _errorMessages.Add(CommonConstant.WORK_CONTENT);
                }

                // Địa điểm (hoặc thiết bị) tiến hành công tác
                if (string.IsNullOrWhiteSpace(requestModel.WorkPlacement))
                {
                    _errorMessages.Add(CommonConstant.WORK_ADDRESS);
                }

                // Điều kiện cần có để hoàn thành công việc
                if (string.IsNullOrWhiteSpace(requestModel.WorkCondition))
                {
                    _errorMessages.Add("Điều kiện cần có để thực hiện công việc là thông tin bắt buộc.");
                }

                // Thời gian tiến hành công việc (bắt đầu)
                if (requestModel.StartDate == DateTime.MinValue)
                {
                    _errorMessages.Add(CommonConstant.START_DATE);
                }

                // Thời gian tiến hành công việc (kết thúc)
                if (requestModel.EndDate == DateTime.MinValue)
                {
                    _errorMessages.Add(CommonConstant.END_DATE);
                }

                // kiểm tra số lượng nhân viên
                if (!string.IsNullOrWhiteSpace(requestModel.UnitNumberAndUnitStaff) && requestModel.UnitNumberAndUnitStaff.Split(',').Any())
                {
                    var unitNumberAndUnitStaffs = requestModel.UnitNumberAndUnitStaff.Split(',').Sum(x => int.Parse(x));
                    int total = 0;
                    //if (requestModel.LeaderUser != null) total++;
                    //total += requestModel.Supervisors != null ? requestModel.Supervisors.Count : 0;
                    //total += requestModel.Commanders != null ? requestModel.Commanders.Count : 0;
                    //if (requestModel.GuardUser != null) total++;
                    total += requestModel.Workers != null ? requestModel.Workers.Count : 0;

                    if (unitNumberAndUnitStaffs != total)
                    {
                        _errorMessages.Add(CommonConstant.UNIT_NUMBER_AND_UNIT_STAFFS);
                    }
                }

                // Người chỉ huy trực tiếp
                if (requestModel.Commanders == null || (requestModel.Commanders != null && requestModel.Commanders.Count == 0))
                {
                    _errorMessages.Add(CommonConstant.COMMANDER_USER);
                }

                // Danh sách những người được cử để thực hiện công việc
                if (requestModel.Workers == null || (requestModel.Workers != null && requestModel.Workers.Count == 0))
                {
                    _errorMessages.Add(CommonConstant.WORKER_USER);
                }
            }
            else if (requestModel.RegistrationTicketType == (int)WorkPermitTypeLCT.COTHUYNHIETHOA)
            {
                // Số lệnh đăng ký công tác là bắt buộc
                if (string.IsNullOrWhiteSpace(requestModel.TicketNumber))
                {
                    _errorMessages.Add(CommonConstant.WORK_PERMIT_NUMBER);
                }

                // Đơn vị nhận lệnh
                if (requestModel.ReceiveUnitID < 0)
                {
                    _errorMessages.Add(CommonConstant.RECEIVE_WORK_PERMIT_UNIT);
                }

                // Địa điểm/Thiết bị
                if (string.IsNullOrWhiteSpace(requestModel.LocationDevice))
                {
                    _errorMessages.Add(CommonConstant.LOCATION_DEVICE);
                }

                // Nội dung công việc
                if (string.IsNullOrWhiteSpace(requestModel.WorkContent))
                {
                    _errorMessages.Add(CommonConstant.WORK_CONTENT);
                }

                // Nhận diện mối nguy và biện pháp an toàn
                if (requestModel.IdentificationAndSafetyMeasuresModels == null || (requestModel.IdentificationAndSafetyMeasuresModels != null && requestModel.IdentificationAndSafetyMeasuresModels.Count == 0))
                {
                    _errorMessages.Add(CommonConstant.IDENTIFICATION_SAFETY);
                }

                // Thời gian (bắt đầu)
                if (requestModel.StartDate == DateTime.MinValue)
                {
                    _errorMessages.Add(CommonConstant.START_DATE);
                }

                // Thời gian (kết thúc)
                if (requestModel.EndDate == DateTime.MinValue)
                {
                    _errorMessages.Add(CommonConstant.END_DATE);
                }

                if (requestModel.IsWorkDaily && string.IsNullOrWhiteSpace(requestModel.StartTimeDaily))
                {
                    _errorMessages.Add(CommonConstant.START_TIME_DAILY);
                }

                if (requestModel.IsWorkDaily && string.IsNullOrWhiteSpace(requestModel.EndTimeDaily))
                {
                    _errorMessages.Add(CommonConstant.END_TIME_DAILY);
                }

                // Nhận diện mối nguy và biện pháp an toàn
                if (requestModel.ContentPositions == null || (requestModel.ContentPositions != null && requestModel.ContentPositions.Count == 0))
                {
                    _errorMessages.Add(CommonConstant.WORKER_USERS);
                }
            }

            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
