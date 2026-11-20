using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System;
using System.Net;

namespace Elogbook.Business
{
    /// <summary>
    /// Bất thường sự cố
    /// </summary>
    public class AbnormalIncidentBusiness : IAbnormalIncidentBusiness
    {
        private readonly IAbnormalIncidentService _abnormalIncidentService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="abnormalIncidentService"></param>
        public AbnormalIncidentBusiness(IAbnormalIncidentService abnormalIncidentService)
        {
            _abnormalIncidentService = abnormalIncidentService;
        }

        public ResponseBaseModel Find(SearchAbnormalIncidentModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _abnormalIncidentService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ABNORMAL_INCIDENT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AbnormalIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel UpdateAbnormalIncidentDetail(AbnormalIncidentDetailUpdate requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _abnormalIncidentService.UpdateAbnormalIncidentDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ABNORMAL_INCIDENT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AbnormalIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel AddAbnormalIncidentDetail(AbnormalIncidentDetailAdd requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _abnormalIncidentService.AddAbnormalIncidentDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ABNORMAL_INCIDENT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AbnormalIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetAbnormalIncidentById(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _abnormalIncidentService.GetAbnormalIncidentById(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ABNORMAL_INCIDENT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AbnormalIncidentBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel DeleteAbnormalIncident(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _abnormalIncidentService.DeleteAbnormalIncident(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ABNORMAL_INCIDENT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AbnormalIncidentBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel DeteleAbnormalIncidentDetail(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _abnormalIncidentService.DeteleAbnormalIncidentDetail(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ABNORMAL_INCIDENT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AbnormalIncidentBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel SyncDataAbnormalIncidentDetail(AbnormalIncidentDetailSyncDataModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {

                responseBaseModel = _abnormalIncidentService.SyncDataAbnormalIncidentDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ABNORMAL_INCIDENT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AbnormalIncidentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
