using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class OperationParametersBusiness : IOperationParametersBusiness
    {
        private readonly IOperationParametersService _operationParametersService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterGroupService"></param>
        public OperationParametersBusiness(IOperationParametersService operationParametersService)
        {
            _operationParametersService = operationParametersService;
        }

        /// <summary>
        /// Tìm kiếm tất cả Chu kỳ và danh sách nhóm thông số, số lượng thông số
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetCyclesManagements(int factoryID, int operationPositionID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (factoryID < 1)
                {
                    responseBaseModel.Message = CommonConstant.FACTORY_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }
                if (operationPositionID < 1)
                {
                    responseBaseModel.Message = CommonConstant.OPERATION_POSITION_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }
                responseBaseModel = _operationParametersService.GetCyclesManagements(factoryID, operationPositionID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + $"factoryID: {factoryID} ,operationPositionID: {operationPositionID}" + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Cập nhật Thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateOperationParameters(EditOperationParameterModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                responseModel.Data = _operationParametersService.UpdateOperationParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy danh sách khung giờ theo ngày từ ngày , tới ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationTimeLines(SearchOperationTimeLineModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.CycleID < 1)
                {
                    responseBaseModel.Message = CommonConstant.CYCLE_NAME_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }
                responseBaseModel = _operationParametersService.GetOperationTimeLines(requestModel);

            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách khung giờ theo ngày từ ngày , tới ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationTimeLinesMobile(SearchOperationTimeLineModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.CycleID < 1)
                {
                    responseBaseModel.Message = CommonConstant.CYCLE_NAME_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }
                responseBaseModel = _operationParametersService.GetOperationTimeLinesMobile(requestModel);

            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Cập nhật khung giờ khi báo bận
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateOperationTimeLine(EditOperationTimeLineModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                if (requestModel.ListID.Count < 1)
                {
                    responseModel.ErrorMessages.Add("Danh sách khung giờ id là bắt buộc");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }
                responseModel.Data = _operationParametersService.UpdateOperationTimeLine(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật khung giờ khi báo bận
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateOperationTimeLine(StopOperationTimeLineModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            try
            {
                if (requestModel.OperationTimeLineIds.Count < 1)
                {
                    responseModel.ErrorMessages.Add("Danh sách khung giờ id là bắt buộc");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.StopParameterIds.Count < 1)
                {
                    responseModel.ErrorMessages.Add("Danh sách parameter id là bắt buộc");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel.Data = _operationParametersService.UpdateOperationTimeLine(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy Nhóm thông số và số lượng thông số
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <param name="cycleID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetParameterGroups(int factoryID, int operationPositionID, int cycleID, int operationTimeLineID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (cycleID < 1)
                {
                    responseBaseModel.Message = CommonConstant.CYCLE_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }
                responseBaseModel = _operationParametersService.GetParameterGroups(factoryID, operationPositionID, cycleID, operationTimeLineID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationCrewBusiness.ToString(),
                    Environment.NewLine + $"factoryID: {factoryID} ,operationPositionID: {operationPositionID} ,cycleID: {cycleID} ,operationTimeLineID: {operationTimeLineID}" + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách thông số cần ghi thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetParameters(SearchParametersModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationParametersService.GetParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách thông số vận hành đã ghi theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationParameters(SearchOperationParametersModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.OperationTimeLineID < 1)
                {
                    responseBaseModel.Message = CommonConstant.OPERATION_TIMELINEID_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }

                if (requestModel.CycleID < 1)
                {
                    responseBaseModel.Message = CommonConstant.CYCLE_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }

                if (requestModel.OperationPositionID < 1)
                {
                    responseBaseModel.Message = CommonConstant.OPERATION_POSITION_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }

                responseBaseModel = _operationParametersService.GetOperationParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Xem thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel ViewOperationParameters(ViewOperationParametersModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationParametersService.ViewOperationParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách thông số vận hành theo portal
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationParametersPortal(SearchOperationParametersModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.CycleID < 1)
                {
                    responseBaseModel.Message = CommonConstant.CYCLE_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }

                if (requestModel.OperationPositionID < 1)
                {
                    responseBaseModel.Message = CommonConstant.OPERATION_POSITION_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }

                responseBaseModel = _operationParametersService.GetOperationParametersPortal(requestModel);

            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tải file pdf
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportPdfFile(PdfExportOPModel requestModel)
        {
            try
            {
                return _operationParametersService.ExportPdfFile(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Tải file excel
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportOperationParametersExcel(PdfExportOPModel requestModel)
        {
            try
            {
                return _operationParametersService.ExportOperationParametersExcel(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
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
        public ResponseBaseModel OperationParameterSyncData(OperationParametersSyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                responseModel = _operationParametersService.OperationParameterSyncData(requestModel);
                if (responseModel.StatusCode == (int)HttpStatusCode.OK)
                {
                    _operationParametersService.UpdateCountData(requestModel);
                    return _operationParametersService.GetDataAfterSyncing((List<int>)responseModel.Content);
                }
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Message = "Đã có lỗi xảy ra trong quá trình đồng bộ dữ liệu";
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString() + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tạo danh sách thông số vận hành theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel AddOperationParameterByTime(AddOperationParameterByTimeModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationParametersService.ValidateOperationParameterByTime(requestModel);
                if (responseBaseModel.StatusCode == (int)HttpStatusCode.OK)
                {
                    _operationParametersService.AddOperationParameterByTime(requestModel);
                }
                else
                    return responseBaseModel;

            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách nhãn khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetTimeLineLabels(SearchOperationTimeLineModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.CycleID < 1)
                {
                    responseBaseModel.Message = CommonConstant.CYCLE_NAME_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }
                responseBaseModel = _operationParametersService.GetTimeLineLabels(requestModel);

            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetTimeLineDetail(SearchOperationTimeLineDetailModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                if (requestModel.CycleID < 1)
                {
                    responseBaseModel.Message = CommonConstant.CYCLE_NAME_REQUIRED;
                    responseBaseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseBaseModel;
                }
                responseBaseModel = _operationParametersService.GetTimeLineDetail(requestModel);

            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Đồng bộ dữ liệu TSVH Elogbok sang PMIS
        /// </summary>
        public void SyncParametersPmis()
        {
            try
            {
                _operationParametersService.SyncParametersPmis();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + "Đồng bộ dữ liệu PMIS không thành công!" + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Lấy danh sách thông vượt ngưỡng giá trị bị cảnh báo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationParameterWarning(SearchOperationParameterWarningModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationParametersService.GetOperationParameterWarning(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Đồng bộ dữ liệu thông số để hiển thị cảnh báo
        /// </summary>
        public void SyncDataOperationParameterWarning()
        {
            try
            {
                _operationParametersService.SyncDataOperationParameterWarning();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + "Đồng bộ dữ liệu thông số không thành công!" + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết cảnh báo AI của 1 thông số vận hành
        /// </summary>
        public ResponseBaseModel GetOperationParameterWarningDetail(int parameterId)
        {
            try
            {
                return _operationParametersService.GetOperationParameterWarningDetail(parameterId);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
                return HttpResponse.Error(ex.Message);
            }
        }

        public bool IsCreator(int operationTimeLineID, int userID)
        {
            return _operationParametersService.IsCreator(operationTimeLineID, userID);
        }

    }
}
