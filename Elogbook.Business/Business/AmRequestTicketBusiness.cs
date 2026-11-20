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
    class AmRequestTicketBusiness : IAmRequestTicketBusiness
    {
        private readonly IAmRequestTicketService _amRequestTicketService;
        private readonly ICommonService _commonService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public AmRequestTicketBusiness(IAmRequestTicketService amRequestTicketService, ICommonService commonService)
        {
            _amRequestTicketService = amRequestTicketService;
            _commonService = commonService;
        }

        /// <summary>
        /// Thêm mới phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<AmRequestTicketModel> Add(AmAddEditRequestTicketModel requestModel)
        {
            var responseModel = new ResponseModel<AmRequestTicketModel>();
            _errorMessages = new List<string>();

            try
            {
                if (string.IsNullOrEmpty(requestModel.RequestTicketNumber))
                {
                    requestModel.RequestTicketNumber = _commonService.GetRequestTicketNumberByFactory(requestModel.FactoryID);
                }

                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    if (!_amRequestTicketService.CheckDuplicateData(requestModel, true))
                    {
                        responseModel.Data = _amRequestTicketService.Add(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_REQUEST_TICKET);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// kiểu tra data tìm kiếm có hợp lệ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AmAddEditRequestTicketModel requestModel)
        {

            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            if (requestModel.EmergencyId < 1)
            {
                _errorMessages.Add(CommonConstant.EMERGENCY_MANAGEMENT_REQUIRED);
            }

            if (requestModel.OrganizationId < 1)
            {
                _errorMessages.Add("Đơn vị công tác là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.RequestContent))
            {
                _errorMessages.Add(CommonConstant.REQUEST_CONTENT_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Trả về phiếu yêu cầu theo mã số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<AmRequestTicketModel> GetById(int id)
        {
            var responseModel = new ResponseModel<AmRequestTicketModel>();

            try
            {
                responseModel.Data = _amRequestTicketService.GetById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel Find(AmSearchRequestTicketModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amRequestTicketService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindMobile(AmSearchRequestTicketMobileModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amRequestTicketService.FindMobile(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về tất cả số lượng phiếu yêu cầu theo điều kiện của mỗi tab (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<CountStatusRequestTicketMobileModel> FindMobileCount(AmSearchRequestTicketMobileModel requestModel)
        {
            ResponseModel<CountStatusRequestTicketMobileModel> responseModel = new ResponseModel<CountStatusRequestTicketMobileModel>();

            try
            {
                responseModel.Data = _amRequestTicketService.FindMobileCount(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhập phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<AmRequestTicketModel> Update(AmAddEditRequestTicketModel requestModel)
        {
            var responseModel = new ResponseModel<AmRequestTicketModel>();
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
                    if (!_amRequestTicketService.CheckDuplicateData(requestModel, false))
                    {
                        responseModel.Data = _amRequestTicketService.Update(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_REQUEST_TICKET);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateStatus(ChangeStatusRequestModel request)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _amRequestTicketService.UpdateStatus(request);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {request.ID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public ResponseModel<bool> Delete(int id, int currentUserId)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _amRequestTicketService.Delete(id, currentUserId);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Đếm số phiếu yêu cầu
        /// trạng thái đang xử lý với PYC trưởng ca
        /// trạng thái đã duyệt với PYC đơn vị sửa chữa
        /// </summary>
        /// <returns></returns>
        public ResponseModel<CountNumberTicketStatusModel> GetCountNumberTicket()
        {
            ResponseModel<CountNumberTicketStatusModel> responseModel = new ResponseModel<CountNumberTicketStatusModel>();

            try
            {
                responseModel.Data = _amRequestTicketService.GetCountNumberTicket();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// lấy data tạo phiếu đăng ký Phiếu Điện/TCNH
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isCTD"></param>
        /// <returns></returns>
        public ResponseModel<DataRegistrationTicketModel> GetDataRegistrationTicket(int Id, bool isCTD)
        {
            ResponseModel<DataRegistrationTicketModel> responseModel = new ResponseModel<DataRegistrationTicketModel>();

            try
            {
                responseModel.Data = _amRequestTicketService.GetDataRegistrationTicket(Id, isCTD);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// lấy data lịch sử phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetHistory(int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                responseModel = _amRequestTicketService.GetHistory(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// lấy data thông tin liên quan phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetRelateInfo(int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                responseModel = _amRequestTicketService.GetRelateInfo(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RequestTicketBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
