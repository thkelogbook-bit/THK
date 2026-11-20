using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.OperationShift;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    public class OperationShiftBusiness : IOperationShiftBusiness
    {
        private readonly IOperationShiftService _operationShiftService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationShiftService"></param>
        public OperationShiftBusiness(IOperationShiftService operationShiftService)
        {
            _operationShiftService = operationShiftService;
        }

        /// <summary>
        /// Tìm kiếm tất cả Ca vận hành theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationShifts(SearchOperationShiftModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationShiftService.GetOperationShifts(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationShiftBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<OperationShiftModel> GetOperationShiftById(int id)
        {
            var responseModel = new ResponseModel<OperationShiftModel>();

            try
            {
                responseModel.Data = _operationShiftService.GetOperationShiftById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationShiftBusiness.ToString(), Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<OperationShiftModel> AddOperationShift(AddEditOperationShiftModel requestModel)
        {
            var responseModel = new ResponseModel<OperationShiftModel>();
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
                    // Kiểm tra Ca vận hành đã tồn tại trong cùng Nhà máy
                    if (!_operationShiftService.CheckDuplicateData(0, requestModel.OperationShiftName, requestModel.FactoryID, true))
                    {
                        if (requestModel.Active == (int)AccessStatus.Active)
                        {
                            // Kiểm tra tồn tại Ca vận hành có thời gian sử dụng trùng nhau
                            double startTime = ConvertStringToDouble(requestModel.StartTime);
                            double endTime = ConvertStringToDouble(requestModel.EndTime);
                            if (!_operationShiftService.CheckDuplicateTime(0, requestModel.FactoryID, startTime, endTime, true))
                            {
                                responseModel.Data = _operationShiftService.AddOperationShift(requestModel);
                            }
                            else
                            {
                                responseModel.ErrorMessages.Add(CommonConstant.TIME_IS_BEING_USED);
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                        }
                        else
                        {
                            responseModel.Data = _operationShiftService.AddOperationShift(requestModel);
                        }
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_OPERATION_SHIFT);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationShiftBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<OperationShiftModel> UpdateOperationShift(AddEditOperationShiftModel requestModel)
        {
            var responseModel = new ResponseModel<OperationShiftModel>();
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
                    // Kiểm tra Ca vận hành đã tồn tại trong cùng Nhà máy
                    if (!_operationShiftService.CheckDuplicateData(requestModel.ID, requestModel.OperationShiftName, requestModel.FactoryID, false))
                    {
                        if (requestModel.Active == (int)AccessStatus.Active)
                        {
                            // Kiểm tra tồn tại Ca vận hành có thời gian sử dụng trùng nhau
                            double startTime = ConvertStringToDouble(requestModel.StartTime);
                            double endTime = ConvertStringToDouble(requestModel.EndTime);
                            if (!_operationShiftService.CheckDuplicateTime(requestModel.ID, requestModel.FactoryID, startTime, endTime, false))
                            {
                                responseModel.Data = _operationShiftService.UpdateOperationShift(requestModel);
                            }
                            else
                            {
                                responseModel.ErrorMessages.Add(CommonConstant.TIME_IS_BEING_USED);
                                responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                            }
                        }
                        else
                        {
                            responseModel.Data = _operationShiftService.UpdateOperationShift(requestModel);
                        }
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_OPERATION_SHIFT);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationShiftBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteOperationShift(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationShiftService.DeleteOperationShift(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationShiftBusiness.ToString(), Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseOperationShift(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationShiftService.CheckInUseOperationShift(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationShiftBusiness.ToString(), Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AddEditOperationShiftModel requestModel)
        {
            // Tên ca vận hành là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.OperationShiftName))
            {
                _errorMessages.Add(CommonConstant.OPERATION_SHIFT_NAME_REQUIRED);
            }

            // Nhà máy là bắt buộc
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            // Thứ tự sắp xếp là bắt buộc
            if (requestModel.Position < 1)
            {
                _errorMessages.Add(CommonConstant.POSITION_REQUIRED);
            }

            // Thời gian bắt đầu ca
            if (string.IsNullOrWhiteSpace(requestModel.StartTime))
            {
                _errorMessages.Add(CommonConstant.OPERATION_SHIFT_START_TIME);
            }

            // Thời gian kết thúc ca
            if (string.IsNullOrWhiteSpace(requestModel.EndTime))
            {
                _errorMessages.Add(CommonConstant.OPERATION_SHIFT_END_TIME);
            }

            double startTime = ConvertStringToDouble(requestModel.StartTime);
            double endTime = ConvertStringToDouble(requestModel.EndTime);
            if (startTime < 0 || endTime < 0)
            {
                _errorMessages.Add(CommonConstant.FORMAT_TIME_INVALID);
            }

            return _errorMessages.Count <= 0;
        }

        // Nếu thời gian bắt đầu/thời gian kết thúc ca không đúng định dạng hh:mm hoặc lớn hơn thời gian cho phép trong ngày thì hiển thị lỗi
        private double ConvertStringToDouble(string strTime)
        {
            try
            {
                string[] time = strTime.Split(':');
                // Phút phải lớn bằng 0 và nhỏ hơn bằng 59
                if (short.Parse(time[1]) > 59 || short.Parse(time[1]) < 0)
                {
                    return -1;
                }

                // Giờ phải lớn bằng 0 và nhỏ hơn bằng 24
                if (short.Parse(time[0]) > 24 || short.Parse(time[0]) < 0)
                {
                    return -1;
                }
                double.TryParse(strTime.Replace(":", "."), out double dbTime);

                return dbTime;
            }
            catch
            {
                return -1;
            }
        }
    }
}
