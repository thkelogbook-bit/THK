using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    /// <summary>
    /// Nhật ký vận hành
    /// </summary>
    public class OperationDiaryBusiness : IOperationDiaryBusiness
    {
        private readonly IOperationDiaryService _operationDiaryService;
        private readonly ICommonService _commonService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public OperationDiaryBusiness(ICommonService commonService, IOperationDiaryService operationDiaryService)
        {
            _operationDiaryService = operationDiaryService;
            _commonService = commonService;
        }

        public ResponseBaseModel GetOperationDiarys(SearchOperationDiaryModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.FactoryID == 0) requestModel.FactoryID = CommonConstant.ALL_KEY;
                if (requestModel.OperationPositionID == 0) requestModel.OperationPositionID = CommonConstant.ALL_KEY;
                if (requestModel.OperationShiftID == 0) requestModel.OperationShiftID = CommonConstant.ALL_KEY;
                if (requestModel.OperationCrewID == 0) requestModel.OperationCrewID = CommonConstant.ALL_KEY;
                if (string.IsNullOrWhiteSpace(requestModel.FullName)) requestModel.FullName = string.Empty;
                responseBaseModel = _operationDiaryService.GetOperationDiarys(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.OPERATION_DIARY_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetOperationDiarysWeb(SearchOperationDiaryWebModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.FactoryID == 0) requestModel.FactoryID = CommonConstant.ALL_KEY;
                if (requestModel.OperationPositionID == 0) requestModel.OperationPositionID = CommonConstant.ALL_KEY;
                if (requestModel.OperationShiftID == 0) requestModel.OperationShiftID = CommonConstant.ALL_KEY;
                if (requestModel.OperationCrewID == 0) requestModel.OperationCrewID = CommonConstant.ALL_KEY;
                if (string.IsNullOrWhiteSpace(requestModel.FullName)) requestModel.FullName = string.Empty;
                responseBaseModel = _operationDiaryService.GetOperationDiarysWeb(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.OPERATION_DIARY_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel QuickSearchOperationDiarys(QuickSearchOperationDiaryModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationDiaryService.QuickSearchOperationDiarys(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.OPERATION_DIARY_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<ViewOpeartionDiaryModel> ViewOperationDiary(ViewOperationDiaryRequestModel requestModel)
        {
            var responseModel = new ResponseModel<ViewOpeartionDiaryModel>();

            try
            {
                responseModel.Data = _operationDiaryService.ViewOperationDiary(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<ViewOpeartionDiaryModel> GetOperationDiaryById(int id)
        {
            var responseModel = new ResponseModel<ViewOpeartionDiaryModel>();

            try
            {
                responseModel.Data = _operationDiaryService.GetOperationDiaryById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<int> AddOperationDiary(AddOperationDiary requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
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
                    if (!_operationDiaryService.CheckDuplicateData(requestModel.CreatedDate, requestModel.OperationShiftID, requestModel.FactoryID, requestModel.OperationPositionID))
                    {
                        var id = _operationDiaryService.AddOperationDiary(requestModel);
                        if (id > 0)
                        {
                            responseModel.Data = id;
                        }
                        else
                        {
                            responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                            responseModel.Data = 0;
                            responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                        }
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_OPERATION_DIARY);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = 0;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateOperationDiary(EditOperationDiary requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationDiaryService.UpdateOperationDiary(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public bool IsCreator(int operationDiaryID, int userID)
        {
            return _operationDiaryService.IsCreator(operationDiaryID, userID);
        }

        public bool IsCreatorByOperationSituation(int id, int userID)
        {
            return _operationDiaryService.IsCreatorByOperationSituation(id, userID);
        }

        public bool IsCreatorByActiveWork(int id, int userID)
        {
            return _operationDiaryService.IsCreatorByActiveWork(id, userID);
        }

        public bool IsCreatorByExistInShift(int id, int userID)
        {
            return _operationDiaryService.IsCreatorByExistInShift(id, userID);
        }

        public ResponseModel<DeliveryReceiveShiftModel> DeliveryReceiveShift(AddEditDeliveryReceiveShiftModel requestModel)
        {
            ResponseModel<DeliveryReceiveShiftModel> responseModel = new ResponseModel<DeliveryReceiveShiftModel>();

            try
            {
                if (requestModel.UserName == null) requestModel.UserName = string.Empty;
                if (requestModel.Password == null) requestModel.Password = string.Empty;

                var result = _operationDiaryService.DeliveryReceiveShift(requestModel);
                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                {
                    responseModel.Data = null;
                    responseModel.ErrorMessages.Add(result.ErrorMessage);
                    if (result.ErrorMessage == CommonConstant.OPERATION_DIARY_NOT_EXISTS)
                    {
                        responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    else
                    {
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    responseModel.Data = result;
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = null;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteOperationDiary(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationDiaryService.DeleteOperationDiary(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteOperationSituation(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationDiaryService.DeleteOperationSituation(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteActiveWork(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationDiaryService.DeleteActiveWork(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<ValidateOpenShiftModel> ValidateOpenShift(int factoryID, int operationPositionID)
        {
            ResponseModel<ValidateOpenShiftModel> responseModel = new ResponseModel<ValidateOpenShiftModel>();

            try
            {
                responseModel.Data = _operationDiaryService.ValidateOpenShift(factoryID, operationPositionID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = null;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"factoryID: {factoryID} ,operationPositionID: {operationPositionID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteExistInShift(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationDiaryService.DeleteExistInShift(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public HttpResponseMessage ExportFile(int id, int curentUserID)
        {
            try
            {
                return _operationDiaryService.ExportFile(id, curentUserID);

            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Sync dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel OperationDiarySyncData(OperationDiarySyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                responseModel = _operationDiaryService.OperationDiarySyncData(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Content = false;
                responseModel.Message = CommonConstant.OPERATION_DIARY_COMMON;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString() + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AddOperationDiary requestModel)
        {
            // Kiểm tra thông tin Nhà máy
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }
            else if (!_commonService.CheckExistData("factory", requestModel.FactoryID))
            {
                _errorMessages.Add(CommonConstant.FACTORY_NOT_EXISTS);
            }

            // Kiểm tra thông tin ngày tạo Nhật ký vận hành
            if (requestModel.CreatedDate == DateTime.MinValue)
            {
                _errorMessages.Add(CommonConstant.SIGNATUREDATE);
            }

            // Kiểm tra thông tin Cương vị
            if (requestModel.OperationPositionID < 1)
            {
                _errorMessages.Add(CommonConstant.OPERATION_POSITION_REQUIRED);
            }
            else if (!_commonService.CheckExistData("operationposition", requestModel.OperationPositionID))
            {
                _errorMessages.Add(CommonConstant.OPERATION_POSITION_NOT_EXISTS);
            }

            // Kiếm tra thông tin Ca vận hành
            if (requestModel.OperationShiftID < 1)
            {
                _errorMessages.Add(CommonConstant.OPERATION_SHIFT_REQUIRED);
            }
            else if (!_commonService.CheckExistData("operationshift", requestModel.OperationShiftID))
            {
                _errorMessages.Add(CommonConstant.OPERATION_SHIFT_NOT_EXISTS);
            }

            // Kiếm tra thông tin Kíp vận hành
            if (requestModel.OperationCrewID < 1)
            {
                _errorMessages.Add(CommonConstant.OPERATION_CREW_REQUIRED);
            }
            else if (!_commonService.CheckExistData("operationcrew", requestModel.OperationCrewID))
            {
                _errorMessages.Add(CommonConstant.OPERATION_CREW_NOT_EXISTS);
            }

            if (requestModel.OperationDiaryPositionShift?.Count > 0)
            {
                foreach (var item in requestModel.OperationDiaryPositionShift)
                {
                    // Kiểm tra thông tin Cương vị trong ca
                    if (item.PositionInShiftID.GetValueOrDefault(0) < 1)
                    {
                        _errorMessages.Add(CommonConstant.POSITION_IN_SHIFT_REQUIRED);
                    }
                    else if (!_commonService.CheckExistData("positioninshift", item.PositionInShiftID.GetValueOrDefault(0)))
                    {
                        _errorMessages.Add(CommonConstant.POSITION_IN_SHIFT_NOT_EXISTS);
                    }

                    // Kiểm tra thông tin người thực hiện
                    if (item.AssignedUserID < 1)
                    {
                        _errorMessages.Add(CommonConstant.ASSIGNED_USER_REQUIRED);
                    }
                    else if (item.AssignedUserID.GetValueOrDefault(0) > 0 && !_commonService.CheckExistData("user", item.AssignedUserID.GetValueOrDefault(0)))
                    {
                        _errorMessages.Add(CommonConstant.ASSIGNED_USER_NOT_EXISTS);
                    }
                }
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Lấy nhãn nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationDiaryLabels(SearchOperationDiaryWebModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.FactoryID == 0) requestModel.FactoryID = CommonConstant.ALL_KEY;
                if (requestModel.OperationPositionID == 0) requestModel.OperationPositionID = CommonConstant.ALL_KEY;
                if (requestModel.OperationShiftID == 0) requestModel.OperationShiftID = CommonConstant.ALL_KEY;
                if (requestModel.OperationCrewID == 0) requestModel.OperationCrewID = CommonConstant.ALL_KEY;
                if (string.IsNullOrWhiteSpace(requestModel.FullName)) requestModel.FullName = string.Empty;
                responseBaseModel = _operationDiaryService.GetOperationDiaryLabels(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.OPERATION_DIARY_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy chi tiết nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationDiaryDetails(SearchOperationDiaryWebModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.FactoryID == 0) requestModel.FactoryID = CommonConstant.ALL_KEY;
                if (requestModel.OperationPositionID == 0) requestModel.OperationPositionID = CommonConstant.ALL_KEY;
                if (requestModel.OperationShiftID == 0) requestModel.OperationShiftID = CommonConstant.ALL_KEY;
                if (requestModel.OperationCrewID == 0) requestModel.OperationCrewID = CommonConstant.ALL_KEY;
                if (string.IsNullOrWhiteSpace(requestModel.FullName)) requestModel.FullName = string.Empty;
                responseBaseModel = _operationDiaryService.GetOperationDiaryDetails(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.OPERATION_DIARY_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> ExportOperationDiary(ExportOperationDiaryRequestModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel.Data = _operationDiaryService.ExportOperationDiary(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }

        public ResponseBaseModel GetExportOperationDiary(BaseListFilter requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            try
            {
                responseModel = _operationDiaryService.GetExportOperationDiary(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }

        public HttpResponseMessage DownloadExportOperationDiaryFile(int exportOperationDiaryId)
        {
            try
            {
                return _operationDiaryService.DownloadExportOperationDiaryFile(exportOperationDiaryId);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    $"Error downloading ExportOperationDiary file ID {exportOperationDiaryId}: {ex.ToString()}");

                var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Có lỗi xảy ra khi tải file. Vui lòng liên hệ Admin.")
                };
                return errorResponse;
            }
        }

        public ResponseModel<bool> DeleteExportOperationDiary(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationDiaryService.DeleteExportOperationDiary(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.OPERATION_DIARY_COMMON);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());

            }

            return responseModel;
        }

        public void DeleteReportOperationDiary()
        {
            try
            {
                _operationDiaryService.DeleteReportOperationDiary();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + "Xóa dữ liệu báo cáo nhật ký vận hành không thành công!" + Environment.NewLine + ex.ToString());
            }
        }

        public HttpResponseMessage ExportPersonnelData(ExportOperationDiaryModel requestModel)
        {
            try
            {
                return _operationDiaryService.ExportPersonnelData(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationDiaryBusiness.ToString(),
                    Environment.NewLine + "Xuất dữ liệu nhân sự ca vận hành không thành công!" + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new System.Net.Http.StringContent(ex.Message)
                };
            }
        }
    }
}
