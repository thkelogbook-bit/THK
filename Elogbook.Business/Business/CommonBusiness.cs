using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Identity;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.OperationShift;
using Elogbook.Core.Models.Response.User;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    public class CommonBusiness : ICommonBusiness
    {
        private readonly ICommonService _commonService;
        private readonly IFactoryService _factoryService;
        private readonly IExportImportService _exportImportService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="factoryService"></param>
        public CommonBusiness(ICommonService commonService, IFactoryService factoryService, IExportImportService exportImportService)
        {
            _commonService = commonService;
            _factoryService = factoryService;
            _exportImportService = exportImportService;
        }

        public ResponseModel<SystemDataModel> GetSystemData(string[] dataTypes, int companyID, int productionPlantID, int userID)
        {
            var responseModel = new ResponseModel<SystemDataModel>();
            try
            {
                responseModel.Data = _commonService.GetSystemData(dataTypes, companyID, productionPlantID, userID);

                if (responseModel.Data == null)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<ValueLabelModel>> GetSystemDataByFilter(string dataType, string filterDescription, int companyID, int productionPlantID)
        {
            var responseModel = new ResponseModel<List<ValueLabelModel>>();
            try
            {
                responseModel.Data = _commonService.GetSystemDataByFilter(dataType, filterDescription, companyID, productionPlantID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public List<ValueLabelModel> GetProductionPlantByFactoryID(int factoryID)
        {
            try
            {
                return _commonService.GetProductionPlantByFactoryID(factoryID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        public List<ProductionPlantInfoModel> GetProductionPlantByCompanyID(int companyID)
        {
            try
            {
                return _commonService.GetProductionPlantByCompanyID(companyID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ProductionPlantInfoModel>();
            }
        }

        public List<ValueLabelModel> GetOrganizations(int organizationID, string level)
        {
            try
            {
                return _commonService.GetOrganizations(organizationID, level);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        public List<ValueLabelModel> GetFactories(int productionPlantID)
        {
            try
            {
                return _commonService.GetFactories(productionPlantID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        public ResponseModel<MasterDataModel> GetMasterData(string deviceID, int factoryID, int operationPositionID, bool force)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, $"{GetType()}, GetMasterData(), deviceID: {deviceID}, factoryID: {factoryID}, operationPositionID: {operationPositionID}, force: {force}");
            var responseModel = new ResponseModel<MasterDataModel>();

            try
            {
                responseModel.Data = _commonService.GetMasterData(deviceID, factoryID, operationPositionID, force);

                if (responseModel.Data == null)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<UserInfo>> GetUsersInfo(string fullName)
        {
            var responseModel = new ResponseModel<List<UserInfo>>();

            try
            {
                responseModel.Data = _commonService.GetUsersInfo(fullName);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<PositionInShiftInfo>> GetPositionShiftInfo(int operationPositionID)
        {
            var responseModel = new ResponseModel<List<PositionInShiftInfo>>();

            try
            {
                var positionInShifts = _commonService.GetPositionShiftInfo(operationPositionID);
                if (positionInShifts == null || positionInShifts.Count == 0)
                {
                    positionInShifts = new List<PositionInShiftInfo> {
                        new PositionInShiftInfo
                        {
                            PositionInShiftName = "Trưởng ca",
                            Position = 1
                        },
                        new PositionInShiftInfo
                        {
                            PositionInShiftName = "Trưởng kíp",
                            Position = 2
                        },
                        new PositionInShiftInfo
                        {
                            PositionInShiftName = "Vận hành viên",
                            Position = 3
                        },
                    };
                }

                responseModel.Data = positionInShifts;

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy danh sách cương vị trong ca và nhân sự phân bổ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<List<PositionInShiftInfo>> GetPositionShiftAndEmployeeInfo(PositionInShiftAndEmployeeModel requestModel)
        {
            var responseModel = new ResponseModel<List<PositionInShiftInfo>>();

            try
            {
                var positionInShifts = _commonService.GetPositionShiftAndEmployeeInfo(requestModel);
                if (positionInShifts == null || positionInShifts.Count == 0)
                {
                    positionInShifts = new List<PositionInShiftInfo> {
                        new PositionInShiftInfo
                        {
                            PositionInShiftName = "Trưởng ca",
                            Position = 1
                        },
                        new PositionInShiftInfo
                        {
                            PositionInShiftName = "Trưởng kíp",
                            Position = 2
                        },
                        new PositionInShiftInfo
                        {
                            PositionInShiftName = "Vận hành viên",
                            Position = 3
                        },
                    };
                }

                responseModel.Data = positionInShifts;

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<TicketInforModel>> GetTickets(int factoryID, string ticketNumber)
        {
            var responseModel = new ResponseModel<List<TicketInforModel>>();
            try
            {
                responseModel.Data = _commonService.GetTickets(factoryID, ticketNumber);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<TicketNumberInfo>> GetMobileTickets(int factoryID, string ticketNumber)
        {
            var responseModel = new ResponseModel<List<TicketNumberInfo>>();
            try
            {
                responseModel.Data = _commonService.GetMobileTickets(factoryID, ticketNumber);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<UserInfo> GetUserSignature(string username, string password, CustomIdentity currentUser)
        {
            var responseModel = new ResponseModel<UserInfo>();
            try
            {
                responseModel.Data = _commonService.GetUserSignature(username, password);
                if (responseModel.Data != null)
                {
                    responseModel.ErrorMessages.Add(responseModel.Data.ErrorMessage);
                    if (!string.IsNullOrWhiteSpace(responseModel.Data.ErrorMessage))
                    {
                        responseModel.Data = null;
                    }
                }

                if (username != currentUser.Username && (responseModel.ErrorMessages == null || (responseModel.ErrorMessages != null && responseModel.ErrorMessages.Count == 0)))
                {
                    responseModel.ErrorMessages.Add($"Vui lòng nhập đúng tên người dùng của tài khoản: {currentUser.FullName}.");
                }
            }
            catch (Exception ex)
            {
                responseModel.Data = null;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add("Đã có lỗi xảy ra trong quá trình thực hiện. Vui lòng liên hệ quản trị viên để kiểm tra.");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<ValueLabelModel>> GetPowerUnits(int factoryID)
        {
            var responseModel = new ResponseModel<List<ValueLabelModel>>();
            try
            {
                responseModel.Data = _commonService.GetPowerUnits(factoryID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<ValueLabelModel>> GetCycles(int factoryID)
        {
            var responseModel = new ResponseModel<List<ValueLabelModel>>();
            try
            {
                responseModel.Data = _commonService.GetCycles(factoryID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<CyclesModel>> GetOperationParameterCycles(int factoryID, int operationPositionID)
        {
            var responseModel = new ResponseModel<List<CyclesModel>>();
            try
            {
                responseModel.Data = _commonService.GetOperationParameterCycles(factoryID, operationPositionID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<MobileOperationPositionInfo>> GetOperationPositionsByFactoryIDMobile(int factoryID)
        {
            var responseModel = new ResponseModel<List<MobileOperationPositionInfo>>();
            try
            {
                responseModel.Data = _commonService.GetOperationPositionsByFactoryIDMobile(factoryID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<ComboboxOperationPositionModel>> GetOperationPositionsByFactoryID(int factoryID)
        {
            var responseModel = new ResponseModel<List<ComboboxOperationPositionModel>>();
            try
            {
                responseModel.Data = _commonService.GetOperationPositionsByFactoryID(factoryID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Danh sách cương vị trong nhà máy ngoại trừ cương vị hiện tại
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="currentPositionID"></param>
        /// <returns></returns>
        public ResponseModel<List<ComboboxOperationPositionModel>> GetPositionsWithoutCurrentPosition(int factoryID, int currentPositionID)
        {
            var responseModel = new ResponseModel<List<ComboboxOperationPositionModel>>();
            try
            {
                responseModel.Data = _commonService.GetPositionsWithoutCurrentPosition(factoryID, currentPositionID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<OperationShiftBaseModel>> GetOperationShiftsByFactoryID(int factoryID, string hour)
        {
            var responseModel = new ResponseModel<List<OperationShiftBaseModel>>();
            try
            {
                responseModel.Data = _commonService.GetOperationShiftsByFactoryID(factoryID, hour, true);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<ValueLabelModel>> GetOperationCrewsByFactoryID(int factoryID)
        {
            var responseModel = new ResponseModel<List<ValueLabelModel>>();
            try
            {
                responseModel.Data = _commonService.GetOperationCrewsByFactoryID(factoryID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<List<ValueLabelModel>> GetParameterGroupByOperationPositionID(int operationPositionID)
        {
            var responseModel = new ResponseModel<List<ValueLabelModel>>();
            try
            {
                responseModel.Data = _commonService.GetParameterGroupByOperationPositionID(operationPositionID);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public ResponseModel<CategoryModel> GetCategoryByFactoryID(int factoryID)
        {
            var responseModel = new ResponseModel<CategoryModel>();
            try
            {
                responseModel.Data = _commonService.GetCategoryByFactoryID(factoryID);

                if (responseModel.Data == null)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy danh sách setting cảnh báo
        /// </summary>
        public ResponseModel<AlarmSettingModel> GetAlarmSettings()
        {
            var responseModel = new ResponseModel<AlarmSettingModel>();
            try
            {
                responseModel.Data = _commonService.GetAlarmSettings();

                if (responseModel.Data == null)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy thông tin chữ ký
        /// </summary>
        /// <param name="id"> Id chữ ký </param>
        /// <returns></returns>
        public ResponseModel<SignatureInfo> GetSignatureById(int ID)
        {
            var responseModel = new ResponseModel<SignatureInfo>();
            try
            {
                responseModel.Data = _commonService.GetSignatureById(ID);

                if (responseModel.Data == null)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public UserInfo GetUserByUserName(string userName, bool isExternal)
        {
            try
            {
                if (isExternal)
                    return _commonService.GetExternalUserById(userName);
                else
                    return _commonService.GetInternalUserById(userName);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return null;
            }
        }

        public ResponseModel<UserModel> GetExternalUserById(int ID)
        {
            var responseModel = new ResponseModel<UserModel>();
            try
            {
                responseModel.Data = _commonService.GetExternalUserById(ID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm đơn vị đăng ký và đơn vị cấp phiếu
        /// </summary>
        /// <param name="isUnitRegistration"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetUnitRegistrationOrUnitApprove(bool isUnitRegistration)
        {
            try
            {
                return _commonService.GetUnitRegistrationOrUnitApprove(isUnitRegistration);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Tìm đơn vị đơn vị cấp phiếu (chỉ lấy phân xưởng vận hành)
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetUnitApprove()
        {
            try
            {
                return _commonService.GetUnitApprove();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Danh sách người dùng nội bộ/Bên ngoài đơn vị
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="searchText"></param>
        /// <param name="electricRank"></param>
        /// <param name="userType"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        public List<InternalExternalUserModel> GetInternalExternalUser(int factoryID, string searchText, string electricRank, int userType, string ticketType)
        {
            try
            {
                return _commonService.GetInternalExternalUser(factoryID, searchText, electricRank, userType, ticketType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<InternalExternalUserModel>();
            }
        }

        /// <summary>
        /// Danh sách người dùng nội bộ/Bên ngoài đơn vị
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseBaseModel GetInternalExternalUserMobile(GetInternalExternalUserModel request)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _commonService.GetInternalExternalUserMobile(request);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = ex.Message;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.RegistrationManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(request) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Lấy danh sách chữ ký theo id
        /// </summary>
        /// <param name="signatureIds"></param>
        /// <returns></returns>
        public List<SignatureInfo> GetSignatures(string signatureIds)
        {
            try
            {
                return _commonService.GetSignatures(signatureIds);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<SignatureInfo>();
            }
        }

        /// <summary>
        /// Lấy trạng thai phiếu công tács
        /// </summary>
        /// <param name="isRegistrationTicket"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetTicketStatus(bool isRegistrationTicket, string ticketType)
        {
            try
            {
                return _commonService.GetTicketStatus(isRegistrationTicket, ticketType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Lấy trạng thai phiếu công tác
        /// </summary>
        /// <param name="isRegistrationTicket"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetTicketManipulationStatus(bool isRegistrationTicket)
        {
            try
            {
                return _commonService.GetTicketManipulationStatus(isRegistrationTicket);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Tìm mã số đơn vị đăng ký
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public string GetRegistrationTicketNumber(int iD, string ticketType)
        {
            try
            {
                return _commonService.GetRegistrationTicketNumber(iD, ticketType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Tìm tên đơn vị đăng ký
        /// </summary>
        /// <returns></returns>
        public string GetRegistrationUnitView(int Id)
        {
            try
            {
                return _commonService.GetRegistrationUnitView(Id);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return string.Empty;
            }
        }

        public ResponseModel<List<ValueLabelModel>> GetFactoriesByUserRight(int userID, string filterDescription, bool isActive)
        {
            var responseModel = new ResponseModel<List<ValueLabelModel>>();
            try
            {
                responseModel.Data = _commonService.GetFactoriesByUserRight(userID, filterDescription, isActive);

                if (responseModel.Data == null || responseModel.Data.Count == 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        public UserRolesInfo GetUserRight(int userID)
        {
            try
            {
                return _commonService.GetUserRight(userID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + $"userID: {userID} " + Environment.NewLine + ex.ToString());
                return new UserRolesInfo();
            }
        }

        public List<ValueLabelModel> GetParameterGroupByFactoryIDAndPositionID(int factoryID, int operationPositionID)
        {
            try
            {
                return _commonService.GetParameterGroupByFactoryIDAndPositionID(factoryID, operationPositionID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + $"factoryID: {operationPositionID}, operationPositionID: {operationPositionID} " + Environment.NewLine + ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Tìm mã số đơn vị cấp phiếu của phiếu thao tác
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public string GetManipulationUnitApprover(int iD)
        {
            try
            {
                return _commonService.GetManipulationUnitApprover(iD);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Tìm số phiếu thao tác
        /// </summary>
        /// <param name="idManipulationTicket"></param>
        /// <returns></returns>
        public string GetTicketNumberManipulation(int idManipulationTicket)
        {
            try
            {
                return _commonService.GetTicketNumberManipulation(idManipulationTicket);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Danh sách nhà máy
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetFactoriesActive()
        {
            try
            {
                return _commonService.GetFactoriesActive();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Lấy danh sách đơn vị công tác theo công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetWorkUnitByCompany(int companyID)
        {
            try
            {
                return _commonService.GetWorkUnitByCompany(companyID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Lọc user
        /// </summary>
        /// <param name="filterUserModel"></param>
        /// <returns></returns>
        public List<InternalExternalUserModel> FilterUser(FilterUserModel filterUserModel)
        {
            try
            {
                return _commonService.FilterUser(filterUserModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<InternalExternalUserModel>();
            }
        }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetSigningMethods()
        {
            try
            {
                return _commonService.GetSigningMethods();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Danh sách email template
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetEmailTemplates()
        {
            try
            {
                return _commonService.GetEmailTemplates();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Chức năng hoạt động trên mobile
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetMobileActivities()
        {
            try
            {
                return _commonService.GetMobileActivities();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Lấy các sự kiện theo loại phiếu
        /// </summary>
        /// <param name="dataType">Loại phiếu PCT-Đ, PCT-TCNH</param>
        /// <returns></returns>
        public List<ValueLabelModel> GetTicketEventTypes(int dataType)
        {
            try
            {
                return _commonService.GetTicketEventTypes(dataType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }

        /// <summary>
        /// Lấy danh sách định dạng số phiếu
        /// <param name="type">PTT, PCTTCNH, PCTD</param>
        /// </summary>
        /// <returns></returns>
        public ResponseModel<TicketFormatDefaultModel> GetTicketFormatDefault(string type)
        {
            var responseModel = new ResponseModel<TicketFormatDefaultModel>();
            var data = new TicketFormatDefaultModel();
            try
            {
                switch (type)
                {
                    case "PTT":
                        data.Format = CommonConstant.PTT_FORMAT_DEFAULT;
                        data.Formats = _factoryService.ParseTicketFormat(CommonConstant.PTT_FORMAT_DEFAULT, CommonConstant.PTT);
                        break;
                    case "PCTTCNH":
                        data.Format = CommonConstant.PCTTCNH_FORMAT_DEFAULT;
                        data.Formats = _factoryService.ParseTicketFormat(CommonConstant.PCTTCNH_FORMAT_DEFAULT, CommonConstant.PCTTCNH);
                        break;
                    case "PCTD":
                        data.Format = CommonConstant.PCTD_FORMAT_DEFAULT;
                        data.Formats = _factoryService.ParseTicketFormat(CommonConstant.PCTD_FORMAT_DEFAULT, CommonConstant.PCTD);
                        break;
                }

                responseModel.Data = data;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return responseModel;
        }

        /// <summary>
        /// AM - Danh sách dữ liệu dùng chung:
        /// <param name="categoryNames"></param>
        /// <param name="factoryID"></param>
        /// <param name="companyID"></param>
        /// </summary>
        /// <returns></returns>
        public AmSystemDataModel GetAmCommonData(string categoryNames, int factoryID, int companyID)
        {
            try
            {
                return _commonService.GetAmCommonData(categoryNames, factoryID, companyID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new AmSystemDataModel();
            }
        }

        /// <summary>
        /// tìm mã số phiếu yêu cầu
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public string GetRequestTicketNumber(int iD)
        {
            try
            {
                return _commonService.GetRequestTicketNumber(iD);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Tìm mã số đơn vị đăng ký theo tháng năm
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ticketType"></param>
        /// <param name="registrationTicketID"></param>
        /// <returns></returns>
        public string GetWorkTicketNumber(int userID, string ticketType, int registrationTicketID)
        {
            try
            {
                return _commonService.GetWorkTicketNumber(userID, ticketType, registrationTicketID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Tìm mã số phiếu yêu cầu theo công ty
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        public string GetRequestTicketNumberByFactory(int factoryID)
        {
            try
            {
                return _commonService.GetRequestTicketNumberByFactory(factoryID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return string.Empty;
            }
        }

        public ResponseModel<bool> SyncData(DataSyncModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _commonService.SyncData(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckUserHasRight(int userID, string permissionKey)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _commonService.CheckUserHasRight(userID, permissionKey);
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = true;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel QuickAutoComplete(QuickAutoCompleteModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _commonService.QuickAutoComplete(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetAlertNotifications(AlertNotificationSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _commonService.GetAlertNotifications(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public int GetCountAlertNotice(int iD)
        {
            return _commonService.GetCountAlertNotice(iD);
        }

        public AlertNotificationModel GetAlertNoticeById(int alertId)
        {
            try
            {
                return _commonService.GetAlertNoticeById(alertId);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + $"userID: {alertId} " + Environment.NewLine + ex.ToString());
                return new AlertNotificationModel();
            }
        }

        public ResponseModel<bool> UpdateAlertNotice(UpdateAlertNoticeModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _commonService.UpdateAlertNotice(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public HttpResponseMessage ExportFileTemplate(int categoryType, int companyId)
        {
            try
            {
                return _exportImportService.ExportFileTemplate(categoryType, companyId);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public ResponseModel<bool> ImportDataFromExcelFile(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _exportImportService.ImportDataFromExcelFile(requestModel, categoryType);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<int> GetCountAlertNoticeForMobile(int userID)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();

            try
            {
                return _commonService.GetCountAlertNoticeForMobile(userID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy trạng thai lệnh công tács
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetLctTicketStatus(string ticketType)
        {
            try
            {
                return _commonService.GetLctTicketStatus(ticketType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                return new List<ValueLabelModel>();
            }
        }
    }
}
