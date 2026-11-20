using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.PMIS;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    public class PmisBusiness : IPmisBusiness
    {
        private readonly IPmisService _pmisService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public PmisBusiness(IPmisService pmisService)
        {
            _pmisService = pmisService;
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
                responseBaseModel = _pmisService.GetParameters(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetParameterPmiss(SearchParameterPmisModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _pmisService.GetParameterPmiss(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public List<SelectboxInfoModel> GetParameterGroups(bool isHasAll)
        {
            try
            {
                return _pmisService.GetParameterGroups(isHasAll);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS", ex.ToString());
                return new List<SelectboxInfoModel>();
            }
        }

        public ResponseModel<bool> UpdateParameter(AddEditParameterModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _pmisService.UpdateParameter(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> TestConnectionDatabaseSyncPMIS(DatabaseConnectionInfoModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _pmisService.TestConnectionDatabaseSyncPMIS(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Không thể kết nối đến database server PMIS, vui lòng kiểm tra lại thông đã cung cấp.");
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> SaveConnectionDatabasePMIS(DatabaseConnectionInfoModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _pmisService.SaveConnectionDatabasePMIS(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Không thể lưu thông tin, vui lòng kiểm tra log để xác định lỗi.");
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<DatabaseConnectionInfoModel> LoadConnectionDatabasePMIS()
        {
            var responseModel = new ResponseModel<DatabaseConnectionInfoModel>();

            try
            {
                responseModel.Data = _pmisService.LoadConnectionDatabasePMIS();
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add("Không thể lấy thông tin cấu hình database PMIS, vui lòng kiểm tra log để xác định lỗi.");
                responseModel.Data = null;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS", ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<int> AutoCreateTableByYear()
        {
            var responseModel = new ResponseModel<int>();

            try
            {
                responseModel.Data = _pmisService.AutoCreateTableByYear();
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS", "Không thể tạo kết nối tới server, vui lòng kiểm tra lại thông tin cấu hình database PMIS."
                    + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public void SycnDataForPMIS(int timeGetLatestPramaterUpdated)
        {
            try
            {
                _pmisService.SycnDataForPMIS(timeGetLatestPramaterUpdated);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "PMIS", "Đồng bộ dữ liệu không thành công, vui lòng kiểm tra lại thông tin cấu hình database PMIS."
                    + Environment.NewLine + ex.ToString());
            }
        }
    }
}
