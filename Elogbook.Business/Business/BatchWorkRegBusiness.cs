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
    public class BatchWorkRegBusiness : IBatchWorkRegBusiness
    {
        private readonly IBatchWorkRegService _batchWorkRegService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="batchWorkRegService"></param>
        public BatchWorkRegBusiness(IBatchWorkRegService batchWorkRegService)
        {
            _batchWorkRegService = batchWorkRegService;
        }

        /// <summary>
        /// Tao batch work registration
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> CreateBatchWorkReg(WorkRegAddEditModel requestModel)
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

                responseModel.Data = _batchWorkRegService.CreateBatchWorkReg(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BatchWorkRegBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(WorkRegAddEditModel requestModel)
        {
            // Loại màn hình là bắt buộc
            if (requestModel.ScreenType < 1)
            {
                _errorMessages.Add("Loại phiếu đăng ký là bắt buộc");
            }

            // Nhà máy là bắt buộc
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
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
            if (requestModel.ScreenType == (int)TicketTypePCT.DIEN
                && requestModel.WorkRegDienExcelDatas == null
                && requestModel.WorkRegDienExcelDatas.Count == 0)
            {
                _errorMessages.Add(CommonConstant.DATA_EXCEL_REQUIRED);
            }

            if (requestModel.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA
                && requestModel.WorkRegTcnhExcelDatas == null
                && requestModel.WorkRegTcnhExcelDatas.Count == 0)
            {
                _errorMessages.Add(CommonConstant.DATA_EXCEL_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Tải file
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HttpResponseMessage DownloadFileTemplate(string type)
        {
            try
            {
                return _batchWorkRegService.DownloadFileTemplate(type);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BatchWorkRegBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Upload file excel work registration
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<WorkRegExcelDataValidationModel> UploadFile(WorkRegUploadInfoModel requestModel)
        {
            ResponseModel<WorkRegExcelDataValidationModel> responseModel = new ResponseModel<WorkRegExcelDataValidationModel>();

            try
            {
                responseModel.Data = _batchWorkRegService.UploadFile(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = null;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BatchWorkRegBusiness.ToString(),
                    Environment.NewLine + $"ID: 0" + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// InsertWorkRegFromSchedule
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int InsertWorkRegFromSchedule()
        {
            try
            {
                return _batchWorkRegService.InsertWorkRegFromSchedule();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BatchWorkRegBusiness.ToString(), ex.ToString());
                return 0;
            }
        }
    }
}
