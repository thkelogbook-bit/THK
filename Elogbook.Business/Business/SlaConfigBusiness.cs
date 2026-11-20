using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    /// <summary>
    /// Business layer for SLA configuration management
    /// </summary>
    public class SlaConfigBusiness : ISlaConfigBusiness
    {
        private readonly ISlaConfigService _slaConfigService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        public SlaConfigBusiness(ISlaConfigService slaConfigService)
        {
            _slaConfigService = slaConfigService;
        }

        /// <summary>
        /// Get all SLA configurations with pagination and filtering
        /// </summary>
        public ResponseBaseModel GetSlaConfigs(SearchSlaConfigModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _slaConfigService.GetSlaConfigs(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Get SLA configuration details by ID
        /// </summary>
        public ResponseModel<SlaConfigDetailModel> GetSlaConfigById(int id)
        {
            var responseModel = new ResponseModel<SlaConfigDetailModel>();

            try
            {
                responseModel.Data = _slaConfigService.GetSlaConfigById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(),
                    Environment.NewLine + $"ID: {id}" + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Create new SLA configuration
        /// </summary>
        public ResponseModel<int> AddSlaConfig(AddEditSlaConfigModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidSlaConfigData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    // validate levels: hours/minutes ranges and template existence
                    if (requestModel.Levels != null && requestModel.Levels.Count > 0)
                    {
                        foreach (var lvl in requestModel.Levels)
                        {
                            if (lvl.OffsetHours < 0 || lvl.OffsetHours > 23)
                            {
                                _errorMessages.Add($"Giờ offset cho cấp độ {lvl.LevelOrder} không hợp lệ");
                            }

                            if (lvl.OffsetMinutes < 0 || lvl.OffsetMinutes > 59)
                            {
                                _errorMessages.Add($"Phút offset cho cấp độ {lvl.LevelOrder} không hợp lệ");
                            }

                            if (lvl.SendEmail)
                            {
                                if (!lvl.EmailTemplateId.HasValue || lvl.EmailTemplateId.Value <= 0)
                                {
                                    _errorMessages.Add($"EmailTemplateId là bắt buộc cho cấp độ {lvl.LevelOrder} khi SendEmail = true");
                                }
                                else if (!_slaConfigService.EmailTemplateExists(lvl.EmailTemplateId.Value))
                                {
                                    _errorMessages.Add($"Mẫu email {lvl.EmailTemplateId.Value} cho cấp độ {lvl.LevelOrder} không tồn tại");
                                }
                            }
                        }

                        if (_errorMessages.Count > 0)
                        {
                            responseModel.ErrorMessages = _errorMessages;
                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                            return responseModel;
                        }
                    }

                    responseModel.Data = _slaConfigService.AddSlaConfig(requestModel);
                    responseModel.StatusCode = (int)HttpStatusCode.Created;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Update existing SLA configuration
        /// </summary>
        public ResponseModel<bool> UpdateSlaConfig(AddEditSlaConfigModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel.ID <= 0)
                {
                    responseModel.ErrorMessages.Add("ID cấu hình là bắt buộc");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // Determine factory to check
                int factoryIdToCheck = requestModel.FactoryID;
                if (factoryIdToCheck <= 0)
                {
                    var existing = _slaConfigService.GetSlaConfigById(requestModel.ID);
                    if (existing != null && existing.Factory != null)
                        factoryIdToCheck = existing.Factory.ID;
                }

                // Check duplicate name + category + factory
                if (!string.IsNullOrWhiteSpace(requestModel.Name) && requestModel.CategoryID > 0 && factoryIdToCheck > 0)
                {
                    var exists = _slaConfigService.ExistsByNameCategoryFactory(requestModel.Name.Trim(), requestModel.CategoryID, factoryIdToCheck, requestModel.ID);
                    if (exists)
                    {
                        responseModel.ErrorMessages.Add("Tên cấu hình, phân loại và nhà máy đã tồn tại");
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }
                }

                responseModel.Data = _slaConfigService.UpdateSlaConfig(requestModel);

                if (!responseModel.Data)
                {
                    responseModel.ErrorMessages.Add("Không tìm thấy cấu hình SLA");
                    responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(),
                    Environment.NewLine + $"ID: {requestModel.ID}" + Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Delete SLA configuration
        /// </summary>
        public ResponseModel<bool> DeleteSlaConfig(int id, int currentUserID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _slaConfigService.DeleteSlaConfig(id, currentUserID);
                if (!responseModel.Data)
                {
                    responseModel.ErrorMessages.Add("Không tìm thấy cấu hình SLA");
                    responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(),
                    Environment.NewLine + $"ID: {id}" + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Toggle SLA configuration active status
        /// </summary>
        public ResponseModel<bool> ToggleSlaConfigStatus(int id, int currentUserID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _slaConfigService.ToggleSlaConfigStatus(id, currentUserID);
                if (!responseModel.Data)
                {
                    responseModel.ErrorMessages.Add("Không tìm thấy cấu hình SLA");
                    responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(),
                    Environment.NewLine + $"ID: {id}" + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Get all SLA conditions
        /// </summary>
        public List<SlaConditionModel> GetSlaConditions()
        {
            var results = new List<SlaConditionModel>();
            try
            {
                results = _slaConfigService.GetSlaConditions();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return results;
        }

        public List<SlaEmailTemplateModel> GetActiveEmailTemplates()
        {
            var results = new List<SlaEmailTemplateModel>();
            try
            {
                results = _slaConfigService.GetActiveEmailTemplates();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return results;
        }

        public ResponseModel<int> AddSlaLevel(AddEditSlaLevelModel model)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (model == null)
                {
                    responseModel.ErrorMessages.Add("Dữ liệu yêu cầu không hợp lệ");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // validate offsets
                if (model.OffsetHours < 0 || model.OffsetHours > 23)
                    _errorMessages.Add("Giờ offset không hợp lệ");
                if (model.OffsetMinutes < 0 || model.OffsetMinutes > 59)
                    _errorMessages.Add("Phút offset không hợp lệ");

                // validate templates
                if (model.SendEmail)
                {
                    if (!model.EmailTemplateId.HasValue || model.EmailTemplateId.Value <= 0)
                        _errorMessages.Add("EmailTemplateId là bắt buộc khi SendEmail = true");
                    else if (!_slaConfigService.EmailTemplateExists(model.EmailTemplateId.Value))
                        _errorMessages.Add($"Mẫu email {model.EmailTemplateId.Value} không tồn tại");
                }

                if (_errorMessages.Count > 0)
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel.Data = _slaConfigService.AddSlaLevel(model);
                responseModel.StatusCode = (int)HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateSlaLevel(int id, AddEditSlaLevelModel model)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (model == null)
                {
                    responseModel.ErrorMessages.Add("Dữ liệu yêu cầu không hợp lệ");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                model.ID = id;

                // validate id
                if (model.ID <= 0)
                {
                    responseModel.ErrorMessages.Add("ID cấp độ không hợp lệ");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                // validate offsets
                if (model.OffsetHours < 0 || model.OffsetHours > 23)
                    _errorMessages.Add("Giờ offset không hợp lệ");
                if (model.OffsetMinutes < 0 || model.OffsetMinutes > 59)
                    _errorMessages.Add("Phút offset không hợp lệ");

                // validate templates
                if (model.SendEmail)
                {
                    if (!model.EmailTemplateId.HasValue || model.EmailTemplateId.Value <= 0)
                        _errorMessages.Add("EmailTemplateId là bắt buộc khi SendEmail = true");
                    else if (!_slaConfigService.EmailTemplateExists(model.EmailTemplateId.Value))
                        _errorMessages.Add($"Mẫu email {model.EmailTemplateId.Value} không tồn tại");
                }

                if (_errorMessages.Count > 0)
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel.Data = _slaConfigService.UpdateSlaLevel(model);
                if (!responseModel.Data)
                {
                    responseModel.ErrorMessages.Add("Không tìm thấy cấp độ SLA");
                    responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Get all SLA roles
        /// </summary>
        public List<SlaRoleModel> GetSlaRoles()
        {
            var results = new List<SlaRoleModel>();
            try
            {
                results = _slaConfigService.GetSlaRoles();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return results;
        }

        /// <summary>
        /// Get all SLA categories as combobox
        /// </summary>
        public List<SlaCategoryModel> GetSlaCategories()
        {
            var results = new List<SlaCategoryModel>();
            try
            {
                results = _slaConfigService.GetSlaCategories();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return results;
        }

        /// <summary>
        /// Physically delete a SlaLevel by ID
        /// </summary>
        public ResponseModel<bool> DeleteSlaLevel(int id, int currentUserID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _slaConfigService.DeleteSlaLevel(id, currentUserID);
                if (!responseModel.Data)
                {
                    responseModel.ErrorMessages.Add("Không tìm thấy cấp độ SLA");
                    responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.SlaConfigBusiness.ToString(),
                    Environment.NewLine + $"ID: {id}" + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Validate SLA configuration data
        /// </summary>
        private bool IsValidSlaConfigData(AddEditSlaConfigModel requestModel, int? existingId = null)
        {
            if (requestModel == null)
            {
                _errorMessages.Add("Dữ liệu yêu cầu không được để trống");
                return false;
            }

            if (string.IsNullOrWhiteSpace(requestModel.Name))
            {
                _errorMessages.Add("Tên cấu hình là bắt buộc");
            }

            if (string.IsNullOrWhiteSpace(requestModel.Description))
            {
                _errorMessages.Add("Mô tả cấu hình là bắt buộc");
            }

            if (requestModel.CategoryID <= 0)
            {
                _errorMessages.Add("Phân loại là bắt buộc");
            }

            // For insert, FactoryID is required
            if (!existingId.HasValue)
            {
                if (requestModel.FactoryID <= 0)
                {
                    _errorMessages.Add("Nhà máy là bắt buộc");
                }
            }

            // Check duplicate name + category + factory
            if (!string.IsNullOrWhiteSpace(requestModel.Name) && requestModel.CategoryID > 0)
            {
                int factoryIdToCheck = requestModel.FactoryID;

                if (existingId.HasValue && factoryIdToCheck <= 0)
                {
                    // try to fetch existing config to get its factory
                    var existing = _slaConfigService.GetSlaConfigById(existingId.Value);
                    if (existing != null && existing.Factory != null)
                        factoryIdToCheck = existing.Factory.ID;
                }

                if (factoryIdToCheck > 0)
                {
                    var exists = _slaConfigService.ExistsByNameCategoryFactory(requestModel.Name.Trim(), requestModel.CategoryID, factoryIdToCheck, existingId);
                    if (exists)
                    {
                        _errorMessages.Add("Tên cấu hình, phân loại và nhà máy đã tồn tại");
                    }
                }
            }

            return _errorMessages.Count == 0;
        }
    }
}
