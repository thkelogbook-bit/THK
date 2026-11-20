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
    public class FactoryBusiness : IFactoryBusiness
    {
        private readonly IFactoryService _factoryService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public FactoryBusiness(IFactoryService factoryService)
        {
            _factoryService = factoryService;
        }

        public ResponseBaseModel GetFactorys(SearchFactoryModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _factoryService.GetFactorys(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FactoryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<FactoryModel> GetFactoryById(int id)
        {
            var responseModel = new ResponseModel<FactoryModel>();

            try
            {
                responseModel.Data = _factoryService.GetFactoryById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FactoryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<int> AddFactory(AddEditFactoryModel requestModel)
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
                    if (!_factoryService.CheckDuplicateData(0, requestModel.FactoryName, requestModel.CompanyID, true))
                    {
                        responseModel.Data = _factoryService.AddFactory(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_FACTORY_COMPANY);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FactoryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateFactory(AddEditFactoryModel requestModel)
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
                    if (!_factoryService.CheckDuplicateData(requestModel.ID, requestModel.FactoryName, requestModel.CompanyID, false))
                    {
                        responseModel.Data = _factoryService.UpdateFactory(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_PRODUCTION_PLANT);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FactoryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteFactory(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (!_factoryService.CheckInUseFactory(id))
                {
                    responseModel.Data = _factoryService.DeleteFactory(id, userID);
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add("Bạn không thể xóa nhà máy vì nó đang được sử dụng.");
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FactoryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseFactory(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _factoryService.CheckInUseFactory(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.FactoryBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật Phân xưởng
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditFactoryModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.FactoryName))
            {
                _errorMessages.Add(CommonConstant.FACTORY_NAME_REQUIRED);
            }
            if (requestModel.CompanyID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_COMPANY_REQUIRED);
            }
            if (requestModel.ProductionPlantID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_PRODUCTIONPLANT_REQUIRED);
            }
            if (requestModel.CompanyID > 0 && requestModel.ProductionPlantID > 0 && !_factoryService.CheckExistData(requestModel.CompanyID, requestModel.ProductionPlantID))
            {
                _errorMessages.Add(CommonConstant.COMPANY_PRODUCTIONPLANT_NOTFOUND);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
