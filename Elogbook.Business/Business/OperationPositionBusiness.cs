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
    public class OperationPositionBusiness : IOperationPositionBusiness
    {
        private readonly IOperationPositionService _operationPositionService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterGroupService"></param>
        public OperationPositionBusiness(IOperationPositionService operationPositionService)
        {
            _operationPositionService = operationPositionService;
        }

        /// <summary>
        /// Thêm mới Cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddOperationPosition(AddEditOperationPositionModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                var data = _operationPositionService.CheckDuplicateData(0, requestModel.OperationPositionName, requestModel.FactoryID, true, requestModel.JobTitle);
                if (data == null || data.Value == null)
                {
                    responseModel.Data = _operationPositionService.AddOperationPosition(requestModel);
                }
                else
                {
                    if (data.Value == -1)
                    {
                        responseModel.ErrorMessages.Add($"Tên cương vị đã tồn tại trong nhà máy {data.Label}.");
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(requestModel.JobTitle == (int)JobTitle.ShiftLeader
                            ? $"Nhà máy {data.Label} đã tồn tại cương vị trưởng ca."
                            : $"Nhà máy {data.Label} đã tồn tại cương vị trưởng kíp.");
                    }
                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationPositionBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra Cương vị đã có ghi nhận nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<bool> CheckInUseOperationPosition(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationPositionService.CheckInUseOperationPosition(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationPositionBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa Cương vị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteOperationPosition(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (!_operationPositionService.CheckInUseOperationPosition(id))
                {
                    responseModel.Data = _operationPositionService.DeleteOperationPosition(id, userID);
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.MSG_CANNOT_DELETE_OPERATION_POSITION);
                }

            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationPositionBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm một Cương vị theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<OperationPositionModel> GetOperationPositionById(int id)
        {
            var responseModel = new ResponseModel<OperationPositionModel>();

            try
            {
                responseModel.Data = _operationPositionService.GetOperationPositionById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationPositionBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm tất cả Cương vị theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationPositions(SearchOperationPositionModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationPositionService.GetOperationPositions(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationPositionBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Cập nhật cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateOperationPosition(AddEditOperationPositionModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
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
                    var data = _operationPositionService.CheckDuplicateData(requestModel.ID, requestModel.OperationPositionName, requestModel.FactoryID, false, requestModel.JobTitle);
                    if (data == null || data.Value == null)
                    {
                        responseModel.Data = _operationPositionService.UpdateOperationPosition(requestModel);
                    }
                    else
                    {
                        if (data.Value == -1)
                        {
                            responseModel.ErrorMessages.Add($"Tên cương vị đã tồn tại trong nhà máy {data.Label}.");
                        }
                        else
                        {
                            responseModel.ErrorMessages.Add(requestModel.JobTitle == (int)JobTitle.ShiftLeader
                                ? $"Nhà máy {data.Label} đã tồn tại cương vị trưởng ca."
                                : $"Nhà máy {data.Label} đã tồn tại cương vị trưởng kíp.");
                        }
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationPositionBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditOperationPositionModel requestModel)
        {
            // Tên cương vị là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.OperationPositionName))
            {
                _errorMessages.Add(CommonConstant.OPERATION_POSITION_NAME_REQUIRED);
            }

            // Nhà máy là bắt buộc
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            // Số lượng là bắt buộc
            if (requestModel.Quantity < 1)
            {
                _errorMessages.Add(CommonConstant.QUANTITY_REQUIRED);
            }

            // Kiểu xác thực là bắt buộc
            if (requestModel.AthenticationType < 0)
            {
                _errorMessages.Add(CommonConstant.ATHENTICATION_TYPE);
            }

            // Thứ tự sắp xếp là bắt buộc
            if (requestModel.Position < 1)
            {
                _errorMessages.Add("Thứ tự sắp xếp là thông tin bắt buộc.");
            }

            return _errorMessages.Count <= 0;
        }
    }
}
