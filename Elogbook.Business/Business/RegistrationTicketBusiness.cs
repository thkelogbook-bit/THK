using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class RegistrationTicketBusiness : IRegistrationTicketBusiness
    {
        private readonly IRegistrationTicketService _registrationTicketService;
        private readonly IInspectionReportService _inspectionReportService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationShiftService"></param>
        public RegistrationTicketBusiness(IRegistrationTicketService registrationTicketService, IInspectionReportService inspectionReportService)
        {
            _registrationTicketService = registrationTicketService;
            _inspectionReportService = inspectionReportService;
        }

        /// <summary>
        /// Thêm phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddRegistrationTicket(AddEditRegistrationTicketModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            requestModel.RegistrationTicketType = requestModel.RegistrationTicketType ?? (int)TicketTypePCT.DIEN;

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
                if (isSave)
                {
                    switch (requestModel.RegistrationTicketType)
                    {
                        case (int)TicketTypePCT.DIEN:
                            // Kiểm tra tồn tại Số phiếu đăng ký công tác
                            if (_registrationTicketService.CheckDuplicateData(
                                id: 0,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: true,
                                isTicketNumber: true))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_TICKET, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }

                            break;

                        case (int)TicketTypePCT.COTHUYNHIETHOA:
                            // Kiểm tra tồn tại Số phiếu đăng ký công tác
                            if (_registrationTicketService.CheckDuplicateData(
                                id: 0,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: true))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_TICKET, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                            break;
                    }

                    if (responseModel.ErrorMessages.Count == 0)
                        responseModel.Data = _registrationTicketService.AddRegistrationTicket(requestModel);
                }

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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm những phiếu đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetRegistrationTickets(SearchRegistrationTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                requestModel.RegistrationTicketType = requestModel.RegistrationTicketType ?? (int)TicketTypePCT.DIEN;
                responseBaseModel = _registrationTicketService.GetRegistrationTickets(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// tìm kiếm phiếu đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<RegistrationTicketModel> GetRegistrationTicketById(int id)
        {
            var responseModel = new ResponseModel<RegistrationTicketModel>();

            try
            {
                responseModel.Data = _registrationTicketService.GetRegistrationTicketById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu đăng ký công tác (Chờ cấp phiếu công tác/Rút phiếu đăng ký)
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
                    responseModel.Data = _registrationTicketService.UpdateStatusRegistrationTicket(id, (int)EnumTicketStatus.Waiting, "Gửi phiếu đăng ký", userID, "");
                }
                else
                {
                    responseModel.Data = _registrationTicketService.UpdateStatusRegistrationTicket(id, (int)EnumTicketStatus.Cancel, "Thu hồi phiếu đăng ký", userID, "");
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateRegistrationTicket(AddEditRegistrationTicketModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                requestModel.RegistrationTicketType = requestModel.RegistrationTicketType ?? (int)TicketTypePCT.DIEN;
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
                            case (int)TicketTypePCT.DIEN:
                                // Kiểm tra tồn tại Số phiếu đăng ký công tác
                                if (_registrationTicketService.CheckDuplicateData(
                                    id: requestModel.ID,
                                    ticketNumber: requestModel.TicketNumber,
                                    isAddMode: false,
                                    isTicketNumber: true))
                                {
                                    responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_TICKET, requestModel.TicketNumber));
                                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                                }

                                // Kiểm tra tồn tại Căn cứ biên bản số
                                if (_registrationTicketService.CheckDuplicateData(
                                    id: requestModel.ID,
                                    ticketNumber: requestModel.TicketNumber,
                                    isAddMode: false,
                                    isTicketNumber: false))
                                {
                                    responseModel.ErrorMessages.Add(CommonConstant.REGULATION_TICKET);
                                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                                }
                                break;

                            case (int)TicketTypePCT.COTHUYNHIETHOA:
                                if (_registrationTicketService.CheckDuplicateData(
                                    id: requestModel.ID,
                                    ticketNumber: requestModel.TicketNumber,
                                    isAddMode: false))
                                {
                                    responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_TICKET, requestModel.TicketNumber));
                                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                                }
                                break;
                        }

                        if (responseModel.ErrorMessages.Count == 0)
                            responseModel.Data = _registrationTicketService.UpdateRegistrationTicket(requestModel);
                    }
                }
                else
                {
                    switch (requestModel.RegistrationTicketType)
                    {
                        case (int)TicketTypePCT.DIEN:
                            // Kiểm tra tồn tại Số phiếu đăng ký công tác
                            if (_registrationTicketService.CheckDuplicateData(
                                id: requestModel.ID,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: false,
                                isTicketNumber: true))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_TICKET, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                            break;

                        case (int)TicketTypePCT.COTHUYNHIETHOA:
                            // Kiểm tra tồn tại Số phiếu đăng ký công tác
                            if (_registrationTicketService.CheckDuplicateData(
                                id: requestModel.ID,
                                ticketNumber: requestModel.TicketNumber,
                                isAddMode: false))
                            {
                                responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_REGISTRAITON_TICKET, requestModel.TicketNumber));
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                            break;
                    }

                    if (responseModel.ErrorMessages.Count == 0)
                        responseModel.Data = _registrationTicketService.UpdateRegistrationTicket(requestModel);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xoá phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteRegistrationTicket(int id, int currentUserID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _registrationTicketService.DeleteRegistrationTicket(id, currentUserID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm danh sách trạng thái phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetStatusRegistrationTicketById(int id)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _registrationTicketService.GetListTicketStatusHistoryById(id);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// In file PDF phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportPdfFile(int id)
        {
            try
            {
                return _registrationTicketService.ExportPdfFile(id);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// kiểm tra ràng buộc giá trị được nhập vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditRegistrationTicketModel requestModel)
        {
            if (requestModel.RegistrationTicketType == (int)TicketTypePCT.DIEN)
            {
                // Số phiếu đăng ký công tác là bắt buộc
                if (string.IsNullOrWhiteSpace(requestModel.TicketNumber))
                {
                    _errorMessages.Add(CommonConstant.TICKET_NUMBER);
                }

                // Đơn vị nhận phiếu
                if (requestModel.ReceiveUnitID < 0)
                {
                    _errorMessages.Add(CommonConstant.RECEIVE_TICKET_UNIT);
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
                    _errorMessages.Add(CommonConstant.WORK_ADDRESS);
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
                        _errorMessages.Add(string.Format(CommonConstant.UNIT_NUMBER_AND_UNIT_STAFFS, total, unitNumberAndUnitStaffs));
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
            else if (requestModel.RegistrationTicketType == (int)TicketTypePCT.COTHUYNHIETHOA)
            {
                // Số phiếu đăng ký công tác là bắt buộc
                if (string.IsNullOrWhiteSpace(requestModel.TicketNumber))
                {
                    _errorMessages.Add(CommonConstant.TICKET_NUMBER);
                }

                // Đơn vị nhận phiếu
                if (requestModel.ReceiveUnitID < 0)
                {
                    _errorMessages.Add(CommonConstant.RECEIVE_TICKET_UNIT);
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

            if (requestModel.InspectionReportID != null)
            {
                var inspectionReport = _inspectionReportService.GetById(requestModel.InspectionReportID.Value);
                if (inspectionReport == null)
                {
                    _errorMessages.Add(CommonConstant.BBKSHT_NOT_FOUND);
                }
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Tải file mẫu về local
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage DownloadFileTemplate()
        {
            try
            {
                return _registrationTicketService.DownloadFileTemplate();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationTicketBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}
