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
    public class ParameterGroupBusiness : IParameterGroupBusiness
    {
        private readonly IParameterGroupService _parameterGroupService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterGroupService"></param>
        public ParameterGroupBusiness(IParameterGroupService parameterGroupService)
        {
            _parameterGroupService = parameterGroupService;
        }

        /// <summary>
        /// Tìm kiếm tất cả Nhóm thông số theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetParameterGroups(SearchParameterGroupModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _parameterGroupService.GetParameterGroups(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterGroupBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tìm kiếm một Nhóm thông số theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<ParameterGroupModel> GetParameterGroupById(int id)
        {
            var responseModel = new ResponseModel<ParameterGroupModel>();

            try
            {
                responseModel.Data = _parameterGroupService.GetParameterGroupById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterGroupBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Thêm mới Nhóm thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddParameterGroup(AddEditParameterGroupModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
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
                    if (!_parameterGroupService.CheckDuplicateData(0, requestModel.ParameterGroupName, requestModel.OperationPositionID, true))
                    {
                        responseModel.Data = _parameterGroupService.AddParameterGroup(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_PARAMETERGROUP);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterGroupBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật Nhóm thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateParameterGroup(AddEditParameterGroupModel requestModel)
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
                    if (!_parameterGroupService.CheckDuplicateData(requestModel.ID, requestModel.ParameterGroupName, requestModel.OperationPositionID, false))
                    {
                        responseModel.Data = _parameterGroupService.UpdateParameterGroup(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_PARAMETERGROUP);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterGroupBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa Nhóm thông số
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteParameterGroup(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (!_parameterGroupService.CheckInUseParameterGroup(id))
                {
                    responseModel.Data = _parameterGroupService.DeleteParameterGroup(id, userID);
                }
                else
                {
                    responseModel.ErrorMessages.Add(CommonConstant.MSG_CANNOT_DELETE_PARAMETER_GROUP);
                    responseModel.Data = false;
                }


            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterGroupBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra Nhóm thông số có đang sử dụng để cấu hình thông số không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<bool> CheckInUseParameterGroup(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _parameterGroupService.CheckInUseParameterGroup(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterGroupBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditParameterGroupModel requestModel)
        {
            // Tên Nhóm thông số là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.ParameterGroupName))
            {
                _errorMessages.Add(CommonConstant.PARAMETERGROUPNAME_REQUIRED);
            }

            // Nhà máy là bắt buộc
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            // Cương vị
            if (requestModel.OperationPositionID < 1)
            {
                _errorMessages.Add(CommonConstant.OPERATION_POSITION_REQUIRED);
            }

            // Chu kỳ
            if (requestModel.CycleID < 1)
            {
                _errorMessages.Add(CommonConstant.CYCLE_REQUIRED);
            }

            // Thứ tự sắp xếp là bắt buộc
            if (requestModel.Position < 1)
            {
                _errorMessages.Add(CommonConstant.POSITION_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
