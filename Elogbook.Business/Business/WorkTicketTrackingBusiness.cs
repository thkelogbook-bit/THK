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

namespace Elogbook.Business
{
    public class WorkTicketTrackingBusiness : IWorkTicketTrackingBusiness
    {
        private readonly IWorkTicketTrackingService _workTicketTrackingService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTicketTrackingService"></param>
        public WorkTicketTrackingBusiness(IWorkTicketTrackingService workTicketTrackingService)
        {
            _workTicketTrackingService = workTicketTrackingService;
        }

        public ResponseModel<bool> ApproveWorkTicketTracking(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _workTicketTrackingService.ApproveWorkTicketTracking(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CreateWorkTicketTracking(AddWorkTicketTrackingModel requestModel)
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
                    responseModel.Data = _workTicketTrackingService.CreateWorkTicketTracking(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteWorkTicketTracking(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _workTicketTrackingService.DeleteWorkTicketTracking(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> RejectWorkTicketTracking(int id, int userID, string reason)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _workTicketTrackingService.RejectWorkTicketTracking(id, userID, reason);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<WorkTicketTrackingModel> GetWorkTicketTrackingById(int id)
        {
            var responseModel = new ResponseModel<WorkTicketTrackingModel>();

            try
            {
                responseModel.Data = _workTicketTrackingService.GetWorkTicketTrackingById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm tất cả Phiếu công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetAllWorkTickets(SearchWorkTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTicketTrackingService.GetAllWorkTickets(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tìm kiếm một Phiếu công tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<WorkTicketDetailModel> GetDetailWorkTicket(int id)
        {
            var responseModel = new ResponseModel<WorkTicketDetailModel>();

            try
            {
                responseModel.Data = _workTicketTrackingService.GetDetailWorkTicket(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(),
                        Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy trạng thái thay đổi người cho phép
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetTrackingStatus()
        {
            try
            {
                return _workTicketTrackingService.GetTrackingStatus();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketTrackingBusiness.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        private bool IsValidData(AddWorkTicketTrackingModel requestModel)
        {
            // PCT là bắt buộc
            if (requestModel.WorkTicketID < 1)
            {
                _errorMessages.Add(CommonConstant.WORK_TICKET_REQUIRED);
            }

            // Người cho phép cần thay đổi là bắt buộc
            if (requestModel.NewAllowerUserID < 1)
            {
                _errorMessages.Add(CommonConstant.NEW_ALLOWR_USER_REQUIRED);
            }

            // Người yêu cầu là bắt buộc
            if (requestModel.RequesterUserID < 1)
            {
                _errorMessages.Add(CommonConstant.REQUESTER_USER_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

    }
}
