using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Manipulation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class ManipulationTicketBusiness : IManipulationTicketBusiness
    {
        private readonly IManipulationTicketService _manipulationTicketService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manipulationTicketService"></param>
        public ManipulationTicketBusiness(IManipulationTicketService manipulationTicketService)
        {
            _manipulationTicketService = manipulationTicketService;
        }

        public ResponseBaseModel<TicketSearchMobileModel> Find(SearchTicketMobileModel requestModel)
        {
            var responseModel = new ResponseBaseModel<TicketSearchMobileModel>();

            try
            {
                return _manipulationTicketService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.Message = "Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.";
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<DataCountModel> FindMobileCount(SearchTicketMobileModel requestModel)
        {
            var responseModel = new ResponseModel<DataCountModel>();

            try
            {
                responseModel.Data = _manipulationTicketService.FindMobileCount(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<TicketDetailModel> GetWorkTicketById(int id)
        {
            var responseModel = new ResponseModel<TicketDetailModel>();

            try
            {
                responseModel.Data = _manipulationTicketService.GetTicketById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteFinalTicket(int id, int currentUserID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _manipulationTicketService.DeleteFinalTicket(id, currentUserID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Xóa phiếu thao tác không thành công.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(), $"{id}: " + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateWork(UpdateSequenceOperationModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _manipulationTicketService.Update(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Cập nhật dữ liệu thao tác hàng ngày không thành công.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> Cancel(CancelTicketModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _manipulationTicketService.Cancel(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Từ chối cấp phiếu đăng ký công tác không thành công.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CloseTicket(CloseTicketModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _manipulationTicketService.CloseTicket(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Kết thúc công tác không thành công.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Trả về tất cả phiếu thao tác trên Web theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindManipulationWeb(SearchTicketWebModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _manipulationTicketService.FindManipulationWeb(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseBaseModel.Message = "Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.";
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Xác nhận hoàn thành phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> CompleteManipulationWeb(CompleteManipulationModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();
            try
            {
                if (!IsValid(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel = _manipulationTicketService.CompleteManipulationWeb(requestModel);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Xác nhận hoàn thành phiếu thao tác không thành công.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValid(CompleteManipulationModel requestModel)
        {
            if (requestModel.SignerInfo == null)
            {
                _errorMessages.Add(CommonConstant.SIGNER_INFO);
            }

            if (requestModel.ID < 1)
            {
                _errorMessages.Add(CommonConstant.ID_MANIPULATION_TICKET);
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Trả về trình tự thao tác
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ResponseModel<List<SequenceOperationModel>> GetSequenceOperation(int ID)
        {
            ResponseModel<List<SequenceOperationModel>> responseBaseModel = new ResponseModel<List<SequenceOperationModel>>();

            try
            {
                responseBaseModel.Data = _manipulationTicketService.GetSequenceOperation(ID);
            }
            catch (Exception ex)
            {
                responseBaseModel.ErrorMessages.Add("Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.");
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + $"ID: {ID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về danh sách user
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ResponseModel<List<UserInTicketModel>> GetUsersInTicket(int TicketID, int Role)
        {
            ResponseModel<List<UserInTicketModel>> responseBaseModel = new ResponseModel<List<UserInTicketModel>>();

            try
            {
                responseBaseModel.Data = _manipulationTicketService.GetUsersInTicket(TicketID, Role);
            }
            catch (Exception ex)
            {
                responseBaseModel.ErrorMessages.Add("Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.");
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + $"ID: {TicketID} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// In tài liệu phiếu công tác ra file PDF
        /// </summary>
        /// <param name="id">ID phiếu công tác</param>
        /// <returns></returns>
        public HttpResponseMessage ExportPdfFile(int id)
        {
            try
            {
                return _manipulationTicketService.ExportPdfFile(id);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                                        Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Đồng bộ PTT
        /// </summary>
        /// <param name="requestModel">Data PTT</param>
        /// <returns></returns>
        public ResponseBaseModel SyncData(ManipulationTicketSyncDataModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _manipulationTicketService.SyncData(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseBaseModel.Message = "Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.";
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public HttpResponseMessage PttExport(SearchTicketWebModel requestModel)
        {
            try
            {
                return _manipulationTicketService.PttExport(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "Export",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}
