using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class MajorRepairBusiness : IMajorRepairBusiness
    {
        private readonly IMajorRepairService _majorRepairService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="majorRepairService"></param>
        public MajorRepairBusiness(IMajorRepairService majorRepairService)
        {
            _majorRepairService = majorRepairService;
        }

        public ResponseModel<int> CreateMajorRepair(MajorRepairAddEditModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (_majorRepairService.CheckDuplicateMajorRepair(0, requestModel.FactoryID, requestModel.Title, true))
                {
                    responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_MAJOR_REPAIR);
                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    return responseModel;
                }

                responseModel.Data = _majorRepairService.CreateMajorRepair(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteMajorRepair(int id, int userID)
        {
            try
            {
                return _majorRepairService.DeleteMajorRepair(id, userID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());

                return new ResponseModel<bool>()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Data = false,
                    ErrorMessages = new List<string> { ex.ToString() }
                };
            }
        }

        /// <summary>
        /// tải file về local
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public HttpResponseMessage DownloadFile(int id)
        {
            try
            {
                return _majorRepairService.DownloadFile(id);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public ResponseModel<MajorRepairModel> GetMajorRepairById(int id)
        {
            var responseModel = new ResponseModel<MajorRepairModel>();

            try
            {
                responseModel.Data = _majorRepairService.GetMajorRepairById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetMajorRepairs(MajorRepairSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _majorRepairService.GetMajorRepairs(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateMajorRepair(MajorRepairAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel.ID < 1)
                {
                    _errorMessages.Add(CommonConstant.MAJOR_REPAIR_ID_REQUIRED);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // Kiểm tra tồn tại
                if (_majorRepairService.CheckDuplicateMajorRepair(requestModel.ID, requestModel.FactoryID, requestModel.Title, false))
                {
                    responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_MAJOR_REPAIR);
                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    return responseModel;
                }

                // Kiểm tra nếu đang tạo work ticket thì không cho update
                if (!_majorRepairService.IsAvailableUpdateWorkTicket(requestModel.ID))
                {
                    responseModel.ErrorMessages.Add(CommonConstant.MAJOR_REPAIR_NOT_AVAILABLE_UPDATE);
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // Thời gian cấp phiếu hàng loạt
                if (requestModel.PlanCreateTicketTime < DateTime.Now)
                {
                    responseModel.ErrorMessages.Add(CommonConstant.PLAN_CREATE_TICKET_TIME);
                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    return responseModel;
                }

                return _majorRepairService.UpdateMajorRepair(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<MajorRepairExcelDataValidationModel> UploadFile(MajorRepairUploadInfoModel requestModel)
        {
            ResponseModel<MajorRepairExcelDataValidationModel> responseModel = new ResponseModel<MajorRepairExcelDataValidationModel>();

            try
            {
                responseModel.Data = _majorRepairService.UploadFile(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = null;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + $"ID: 0" + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy trạng thái các đợt sửa chữa lớn
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetMajorRepairStatus()
        {
            try
            {
                return _majorRepairService.GetMajorRepairStatus();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Tạo ticket from excel file
        /// </summary>
        /// <returns></returns>
        public int InsertWorkTicketFromSchedule()
        {
            try
            {
                return _majorRepairService.InsertWorkTicketFromSchedule();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(), ex.ToString());
                return 0;
            }
        }

        /// <summary>
        /// Trả về tất cả phiếu công tác thuộc đợt công tác sửa chữa lớn theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindAllWorkTickets(MajorRepairSearchWorkTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _majorRepairService.FindAllWorkTickets(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về trạng thái phiếu công tác thuộc đợt sửa chữa lớn
        /// </summary>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        public List<ValueLabelCheckedModel> GetMajorRepairTicketStatus(string ticketType)
        {
            try
            {
                return _majorRepairService.GetMajorRepairTicketStatus(ticketType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(), ex.ToString());
                return new List<ValueLabelCheckedModel>();
            }
        }

        /// <summary>
        /// Tải file mẫu về local
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage DownloadFileTemplate()
        {
            try
            {
                return _majorRepairService.DownloadFileTemplate();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public ResponseBaseModel GetTitleMajorRepair(int factoryID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _majorRepairService.GetTitleMajorRepair(factoryID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + $"FactoryID: {factoryID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        private bool IsValidData(MajorRepairAddEditModel requestModel)
        {
            // Tên đợt công tác SCL là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.Title))
            {
                _errorMessages.Add(CommonConstant.MAJOR_REPAIR_NAME_REQUIRED);
            }

            // Nhà máy là bắt buộc
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            // Ngày dự kiến bắt đầu
            if (requestModel.StartDate == DateTime.MinValue)
            {
                _errorMessages.Add(CommonConstant.START_DATE_EXPECTED);
            }

            // Ngày dự kiến kết thúc
            if (requestModel.EndDate == DateTime.MaxValue)
            {
                _errorMessages.Add(CommonConstant.END_DATE_EXPECTED);
            }

            // Ngày dự kiến bắt đầu
            if (requestModel.PlanCreateTicketTime <= DateTime.Now)
            {
                _errorMessages.Add(CommonConstant.PLAN_CREATE_TICKET_TIME);
            }

            // File excel đính kèm là bắt buộc
            if (requestModel.AttachmentID < 1)
            {
                _errorMessages.Add(CommonConstant.ATTACHTMENT_EXECL);
            }

            // File excel đính kèm là bắt buộc
            if (requestModel.MajorRepairExcelDatas == null || requestModel.MajorRepairExcelDatas.Count == 0)
            {
                _errorMessages.Add(CommonConstant.DATA_EXCEL_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        public ResponseModel<bool> InsertMultipleTicket(InsertNewTicketModel requestModel)
        {
            ResponseModel<bool> responseBaseModel = new ResponseModel<bool>();
            try
            {
                if (requestModel.MajorRepairID < 1)
                {
                    responseBaseModel.ErrorMessages.Add(CommonConstant.MAJOR_REPAIR_ID_REQUIRED);
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }

                if (requestModel.AttachmentID < 1)
                {
                    responseBaseModel.ErrorMessages.Add(CommonConstant.ATTACHTMENT_EXECL);
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }

                return _majorRepairService.InsertMultipleTicket(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> DeleteTicket(int workTicketID, int userID)
        {
            try
            {
                return _majorRepairService.DeleteTicket(workTicketID, userID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + $"WorkTicketID: {workTicketID} " + Environment.NewLine + ex.ToString());

                return new ResponseModel<bool>()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Data = false,
                    ErrorMessages = new List<string> { ex.ToString() }
                };
            }
        }

        public ResponseModel<bool> UpdateTicket(MajorRepairUpdateWorkTicketModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            try
            {
                return _majorRepairService.UpdateTicket(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetMajorRepairHistories(MajorRepairHistorySearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _majorRepairService.GetMajorRepairHistories(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.MajorRepairBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
