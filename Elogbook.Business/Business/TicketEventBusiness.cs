using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    /// <summary>
    /// TicketEventBusiness
    /// </summary>
    public class TicketEventBusiness : ITicketEventBusiness
    {
        private readonly ITicketEventService _ticketEventService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ticketEventService"></param>
        public TicketEventBusiness(ITicketEventService ticketEventService)
        {
            _ticketEventService = ticketEventService;
        }

        public ResponseModel<bool> CreateTicketEvent(AddEditTicketEventModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    // Kiểm tra Ticket event đã tồn tại trong cùng Nhà máy
                    if (!_ticketEventService.CheckDuplicateData(0, requestModel.DataType, requestModel.EventTypeID, requestModel.FactoryID, true))
                    {
                        responseModel.Data = _ticketEventService.CreateTicketEvent(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_TICKET_EVENT);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.TicketEventBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteTicketEvent(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _ticketEventService.DeleteTicketEvent(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.TicketEventBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<TicketEventModel> GetTicketEventById(int id)
        {
            var responseModel = new ResponseModel<TicketEventModel>();

            try
            {
                responseModel.Data = _ticketEventService.GetTicketEventById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.TicketEventBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetTicketEvents(SearchTicketEventModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _ticketEventService.GetTicketEvents(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.TicketEventBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateTicketEvent(AddEditTicketEventModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    // Kiểm tra Ticket event đã tồn tại trong cùng Nhà máy
                    if (!_ticketEventService.CheckDuplicateData(requestModel.ID, requestModel.DataType, requestModel.EventTypeID, requestModel.FactoryID, false))
                    {
                        responseModel.Data = _ticketEventService.UpdateTicketEvent(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_TICKET_EVENT);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.TicketEventBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AddEditTicketEventModel requestModel)
        {
            // Nhà máy là bắt buộc
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            // Loại phiếu là thông tin bắt buộc
            if (requestModel.DataType < 1)
            {
                _errorMessages.Add("Loại phiếu là thông tin bắt buộc.");
            }

            // Loại sự kiện là thông tin bắt buộc
            if (requestModel.EventTypeID < 1)
            {
                _errorMessages.Add(CommonConstant.TICKETTYPE_REQUIRED);
            }

            // Nội dung mẫu là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.EventContent))
            {
                _errorMessages.Add(CommonConstant.TICKET_CONTENT_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
