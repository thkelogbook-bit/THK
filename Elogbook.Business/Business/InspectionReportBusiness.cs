using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.InspectionReport;
using Elogbook.Core.Models.Response.InspectionReport;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class InspectionReportBusiness : IInspectionReportBusiness
    {
        private readonly IInspectionReportService _inspectionReportService;
        private readonly ICommonService _commonService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inspectionReportService"></param>
        public InspectionReportBusiness(IInspectionReportService inspectionReportService,
            ICommonService commonService)
        {
            _inspectionReportService = inspectionReportService;
            _commonService = commonService;
        }

        /// <summary>
        /// Tìm kiếm BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetAll(SearchInspectionReportModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _inspectionReportService.GetAll(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.InspectionReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Tạo mới BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<InspectionReportDetailsModel> Add(AddEditInspectionReporModel requestModel)
        {
            var responseModel = new ResponseModel<InspectionReportDetailsModel>();
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
                    responseModel.Data = _inspectionReportService.Add(requestModel);
                }

                // Nếu có lỗi trong quá trình lưu thì trả về lỗi để kiểm tra
                if (responseModel.Data == null)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.InspectionReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Update BBKSHT
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<InspectionReportDetailsModel> Update(AddEditInspectionReporModel requestModel)
        {
            var responseModel = new ResponseModel<InspectionReportDetailsModel>();
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
                    responseModel.Data = _inspectionReportService.Update(requestModel);
                }

                // Nếu có lỗi trong quá trình lưu thì trả về lỗi để kiểm tra
                if (responseModel.Data == null)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.InspectionReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Delete BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<bool> Delete(int id, int currentUserId)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                responseModel.Data = _inspectionReportService.Delete(id, currentUserId);

                // Nếu có lỗi trong quá trình lưu thì trả về lỗi để kiểm tra
                if (!responseModel.Data)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_TICKET_COMMON);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.InspectionReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(id) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Get detail BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<InspectionReportDetailsModel> Details(int id)
        {
            var responseModel = new ResponseModel<InspectionReportDetailsModel>();
            InspectionReportDetailsModel inspectionReportDetails = _inspectionReportService.GetById(id);
            if (inspectionReportDetails != null)
            {
                responseModel.Data = inspectionReportDetails;
            }
            else
            {
                responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                responseModel.ErrorMessages.Add(CommonConstant.BBKSHT_NOT_FOUND);
            }
            return responseModel;
        }

        /// <summary>
        /// Ký xác nhận BBKSHT
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ResponseModel<UserSigningModel> Signging(SigngingModel signgingModel)
        {
            _errorMessages = new List<string>();
            var responseModel = new ResponseModel<UserSigningModel>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidSigning(signgingModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseModel.Data = _inspectionReportService.Signging(signgingModel);

                    // Nếu có lỗi trong quá trình ký xác nhận
                    if (responseModel.Data == null)
                    {
                        responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.ErrorMessages.Add(CommonConstant.REGISTRATION_TICKET_COMMON);
                    }
                }

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.InspectionReportBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(signgingModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// In BBKSHT
        /// </summary>
        /// <param name="id">ID BBKSHT</param>
        /// <returns></returns>
        public HttpResponseMessage ExportPdfFile(int id)
        {
            try
            {
                return _inspectionReportService.ExportPdfFile(id);
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
        /// Validate chữ ký
        /// </summary>
        /// <param name="signgingModel"></param>
        /// <returns></returns>
        private bool IsValidSigning(SigngingModel signgingModel)
        {
            InspectionReportDetailsModel inspectionReportDetails = _inspectionReportService.GetById(signgingModel.InspectionReportId);
            if (inspectionReportDetails == null)
            {
                _errorMessages.Add("Biên bản khảo sát hiện trường không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(signgingModel.UserName))
            {
                _errorMessages.Add("Tên đăng nhập là thông tin bắt buộc.");
            }

            //if (string.IsNullOrWhiteSpace(signgingModel.Password))
            //{
            //    _errorMessages.Add("Mật khẩu là thông tin bắt buộc.");
            //}

            if (signgingModel.Roles < 1 || signgingModel.Roles > 3)
            {
                _errorMessages.Add("Vai trò người dùng không đúng.");
            }

            if (signgingModel.Roles == 1 && inspectionReportDetails != null)
            {
                if (inspectionReportDetails.UnitRepresentatives.Count == 0 || !inspectionReportDetails.UnitRepresentatives.Exists(u => u.UserName.ToLower().Equals(signgingModel.UserName.ToLower())))
                {
                    _errorMessages.Add("Người ký không tồn tại.");
                }

                if (inspectionReportDetails.UnitRepresentativesSigning != null && inspectionReportDetails.UnitRepresentativesSigning.Count > 0)
                {
                    _errorMessages.Add("Đại diện đơn vị công tác đã ký.");
                }
            }

            if (signgingModel.Roles == 2 && inspectionReportDetails != null)
            {
                if (inspectionReportDetails.OmUnitRepresentatives.Count == 0 || !inspectionReportDetails.OmUnitRepresentatives.Exists(u => u.UserName.ToLower().Equals(signgingModel.UserName.ToLower())))
                {
                    _errorMessages.Add("Người ký không tồn tại.");
                }

                if (inspectionReportDetails.OmUnitRepresentativesSigning != null && inspectionReportDetails.OmUnitRepresentativesSigning.Count > 0)
                {
                    _errorMessages.Add("Đại diện đơn vị QLVH đã ký.");
                }
            }

            if (signgingModel.Roles == 3 && inspectionReportDetails != null)
            {
                if (inspectionReportDetails.RelatedUnitRepresentatives.Count == 0 || !inspectionReportDetails.RelatedUnitRepresentatives.Exists(u => u.UserName.ToLower().Equals(signgingModel.UserName.ToLower())))
                {
                    _errorMessages.Add("Đại diện các đơn vị có liên quan không tồn tại.");
                }

                if (inspectionReportDetails.RelatedUnitRepresentativesSigning != null && inspectionReportDetails.RelatedUnitRepresentativesSigning.Count > 0)
                {
                    _errorMessages.Add("Đại diện các đơn vị có liên quan đã ký.");
                }
            }

            if (string.IsNullOrEmpty(signgingModel.Signature) && _commonService.IsValidUserAndPassword(signgingModel.UserName, signgingModel.Password))
            {
                _errorMessages.Add(CommonConstant.USERNAME_PASS_NOT_CORRECT);
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Kiểm tra ràng buộc giá trị được nhập vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditInspectionReporModel requestModel)
        {
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            if (requestModel.SurveyTime == DateTime.MinValue)
            {
                _errorMessages.Add("Thời gian vào khảo sát là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.Address))
            {
                _errorMessages.Add("Địa chỉ là thông tin bắt buộc.");
            }

            if (requestModel.UnitRepresentatives == null || (requestModel.UnitRepresentatives != null && requestModel.UnitRepresentatives.Count == 0))
            {
                _errorMessages.Add("Đại diện đơn vị công tác là thông tin bắt buộc.");
            }

            if (requestModel.OmUnitRepresentatives == null || (requestModel.OmUnitRepresentatives != null && requestModel.OmUnitRepresentatives.Count == 0))
            {
                _errorMessages.Add("Đại diện (các) đơn vị QLVH là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.WorkPlacement))
            {
                _errorMessages.Add("Địa điểm (hoặc thiết bị) thực hiện công việc là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.WorkContent))
            {
                _errorMessages.Add("Nội dung công việc là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.InspectionScope))
            {
                _errorMessages.Add("Phạm vi làm việc là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.InspectionScope))
            {
                _errorMessages.Add("Phạm vi làm việc là thông tin bắt buộc.");
            }

            if (requestModel.PlanEndDate == DateTime.MinValue)
            {
                _errorMessages.Add("Thời gian tiến hành công việc (bắt đầu) là thông tin bắt buộc.");
            }

            if (requestModel.PlanEndDate == DateTime.MinValue)
            {
                _errorMessages.Add("Thời gian tiến hành công việc (kết thúc) là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.OperationUnitResponsibilities))
            {
                _errorMessages.Add("Đối với (các) Đơn vị QLVH là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.WorkUnitResponsibilities))
            {
                _errorMessages.Add("Đối với Đơn vị công tác là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.WorkUnitResponsibilities))
            {
                _errorMessages.Add("Đối với đơn vị điều độ là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.WorkUnitResponsibilities))
            {
                _errorMessages.Add("Những nội dung khác có liên quan đến công việc là thông tin bắt buộc.");
            }
            return _errorMessages.Count <= 0;
        }
    }
}
