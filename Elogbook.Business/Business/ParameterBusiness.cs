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
    public class ParameterBusiness : IParameterBusiness
    {
        private readonly IParameterService _parameterService;
        private readonly IParameterGroupService _parameterGroupService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterService"></param>
        public ParameterBusiness(IParameterService parameterService, IParameterGroupService parameterGroupService)
        {
            _parameterService = parameterService;
            _parameterGroupService = parameterGroupService;
        }

        /// <summary>
        /// Lấy danh sách nhóm thông số và thông số của nhóm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetParameters(SearchParameterForStopModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _parameterService.GetParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tìm kiếm tất cả Thông số theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetParameters(SearchParameterModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _parameterService.GetParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tìm kiếm nhanh danh mục thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel QuickSearchParameters(SearchParameterModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _parameterService.QuickSearchParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy dữ liệu để vẽ chart cho TSVH
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ParametersChartModel GetParametersDrawChart(SearchParameterForChartModel requestModel)
        {
            try
            {
                return _parameterService.GetParametersDrawChart(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                return new ParametersChartModel();
            }
        }

        /// <summary>
        /// Tìm kiếm một Thông số theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<ParameterModel> GetParameterById(int id)
        {
            var responseModel = new ResponseModel<ParameterModel>();

            try
            {
                responseModel.Data = _parameterService.GetParameterById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<List<CommonDataComboboxList>> GetOperationPositions(int factoryID)
        {
            var responseModel = new ResponseModel<List<CommonDataComboboxList>>();

            try
            {
                responseModel.Data = _parameterService.GetOperationPositions(factoryID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + $"factoryID: {factoryID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Thêm mới Thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddParameter(AddEditParameterModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel.AlertDetails != null && requestModel.AlertDetails.Count > 0)
                {
                    Dictionary<int, string> operatorValue = new Dictionary<int, string>();
                    for (int i = 0; i < requestModel.AlertDetails.Count; i++)
                    {
                        AddEditAlertDetailModel item = requestModel.AlertDetails[i];
                        switch (item.Operator)
                        {
                            case "=":
                                if (!operatorValue.ContainsKey(1))
                                {
                                    operatorValue.Add(1, item.Operator);
                                }
                                else
                                {
                                    _errorMessages.Add("Trùng toán tử: " + item.Operator);
                                }
                                break;
                            case ">=":
                                if (!operatorValue.ContainsKey(2))
                                {
                                    operatorValue.Add(2, item.Operator);
                                }
                                else
                                {
                                    _errorMessages.Add("Trùng toán tử: " + item.Operator);
                                }
                                break;
                            case "<=":
                                if (!operatorValue.ContainsKey(3))
                                {
                                    operatorValue.Add(3, item.Operator);
                                }
                                else
                                {
                                    _errorMessages.Add("Trùng toán tử: " + item.Operator);
                                }
                                break;
                            case ">":
                                if (!operatorValue.ContainsKey(4))
                                {
                                    operatorValue.Add(4, item.Operator);
                                }
                                else
                                {
                                    _errorMessages.Add("Trùng toán tử: " + item.Operator);
                                }
                                break;
                            case "<":
                                if (!operatorValue.ContainsKey(5))
                                {
                                    operatorValue.Add(5, item.Operator);
                                }
                                else
                                {
                                    _errorMessages.Add("Trùng toán tử: " + item.Operator);
                                }
                                break;
                        }
                    }

                    if (_errorMessages.Count > 0)
                    {
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }
                }
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    requestModel.ID = 0;
                    if (!_parameterService.CheckDuplicateData(requestModel, true))
                    {
                        responseModel.Data = _parameterService.AddParameter(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_PARAMETER);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật Thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateParameter(AddEditParameterModel requestModel)
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
                    if (!_parameterService.CheckDuplicateData(requestModel, false))
                    {
                        ParameterGroupModel parameterGroupModel = _parameterGroupService.GetParameterGroupById(requestModel.ParameterGroupID.GetValueOrDefault());
                        var response = new ResponseModel<ParameterModel>
                        {
                            Data = _parameterService.GetParameterById(requestModel.ID)
                        };

                        if (parameterGroupModel != null && parameterGroupModel.Status == (int)AccessStatus.InActive
                            && response.Data != null
                            && response.Data.Status != requestModel.Status)
                        {
                            responseModel.ErrorMessages.Add("Không thể chuyển thông số sang trạng thái \"Sử dụng\" vì nhóm thông số đang ở trạng thái không sử dụng!");
                            responseModel.StatusCode = StatusCodeCustom.InValid;
                        }
                        else
                        {
                            responseModel.Data = _parameterService.UpdateParameter(requestModel);
                        }
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_PARAMETER);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa Thông số
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeleteParameter(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _parameterService.DeleteParameter(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + $"id: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra Thông số có đang sử dụng để cấu hình thông số không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<bool> CheckInUseParameter(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _parameterService.CheckInUseParameter(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ParameterBusiness.ToString(),
                    Environment.NewLine + $"id: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditParameterModel requestModel)
        {
            // Nhà máy là bắt buộc
            if (requestModel.FactoryID < 1 && !requestModel.IsGroup)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            // Cương vị là bắt buộc
            if (requestModel.OperationPositionID < 1 && !requestModel.IsGroup)
            {
                _errorMessages.Add(CommonConstant.OPERATION_POSITION_REQUIRED);
            }

            // Chu kỳ là bắt buộc
            if (requestModel.CycleID < 1 && !requestModel.IsGroup)
            {
                _errorMessages.Add(CommonConstant.CYCLE_REQUIRED);
            }

            // Tên Thông số là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.ParameterName))
            {
                _errorMessages.Add(CommonConstant.PARAMETERNAME_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
