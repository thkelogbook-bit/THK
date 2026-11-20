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
    public class AmDeviceCategoryBusiness: IAmDeviceCategoryBusiness
    {
        private readonly IAmDeviceCategoryService _amDeviceCategoryService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public AmDeviceCategoryBusiness(IAmDeviceCategoryService amDeviceCategoryService)
        {
            _amDeviceCategoryService = amDeviceCategoryService;
        }

        /// <summary>
        /// Thêm mới phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<AmDeviceCategoryModel> Add(AddEditAmDeviceCategoryModel requestModel)
        {
            var responseModel = new ResponseModel<AmDeviceCategoryModel>();
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
                    if (!_amDeviceCategoryService.CheckDuplicateData(requestModel, true))
                    {
                        responseModel.Data = _amDeviceCategoryService.Add(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Phân loại thiết bị đã tồn tại trong Nhà máy.");
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AddEditAmDeviceCategoryModel requestModel)
        {
            if (string.IsNullOrEmpty(requestModel.CategoryName))
            {
                _errorMessages.Add(CommonConstant.NAME_DEVICE_CATEGORY_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(requestModel.CategoryCode))
            {
                _errorMessages.Add(CommonConstant.CODE_DEVICE_CATEGORY_REQUIRED);
            }

            if (requestModel.Active > 2 || requestModel.Active < 1)
            {
                _errorMessages.Add("Trạng thái không hợp lệ");
            }

            if(requestModel.CategoryParent > 0)
            {
                bool existsParent = _amDeviceCategoryService.CheckExistsParent(requestModel.CategoryParent.Value);

                if (!existsParent)
                {
                    _errorMessages.Add("Phân loại thiết bị cha không hợp lệ");
                }
            }

            if (requestModel.OrgID > 1)
            {
                bool isExist = _amDeviceCategoryService.GetOrgInf(requestModel.OrgID.Value);

                if (!isExist)
                {
                    _errorMessages.Add("Mã công ty không hợp lệ");
                }
            }

            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Trả về phân loại thiết bị theo mã số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<AmDeviceCategoryModel> GetById(int id)
        {
            var responseModel = new ResponseModel<AmDeviceCategoryModel>();

            try
            {
                responseModel.Data = _amDeviceCategoryService.GetById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Trả về tất cả phân loại thiết bị theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel Find(SearchAmDeviceCategoryModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amDeviceCategoryService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Trả về tất cả phân loại thiết bị cha theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindParent(SearchParentAmDeviceCategoryModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amDeviceCategoryService.FindParent(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(), ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Cập nhập phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<AmDeviceCategoryModel> Update(AddEditAmDeviceCategoryModel requestModel)
        {
            var responseModel = new ResponseModel<AmDeviceCategoryModel>();
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
                    if (!_amDeviceCategoryService.CheckDuplicateData(requestModel, false))
                    {
                        return _amDeviceCategoryService.Update(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Phân loại thiết bị đã tồn tại trong Nhà máy.");
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }

        /// <summary>
        /// xóa phân loại thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="iD"></param>
        /// <returns></returns>
        public ResponseModel<bool> Delete(int id, int currentUserId)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _amDeviceCategoryService.Delete(id, currentUserId);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// sync PMIS
        /// </summary>
        /// <returns></returns>
        public ResponseBaseModel SyncPmis()
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amDeviceCategoryService.SyncPmis();
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(), ex.ToString());
            }

            return responseBaseModel;
        }

        public List<ComboboxDataModel> GetDeviceCategoriesByCompany(int companyID, bool isParent)
        {
            try
            {
                return _amDeviceCategoryService.GetDeviceCategoriesByCompany(companyID, isParent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.DeviceCategoryBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }
    }
}
