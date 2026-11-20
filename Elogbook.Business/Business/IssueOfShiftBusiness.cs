using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Domain.Services;
using System;
using System.Net;
using System.Net.Http;

namespace Elogbook.Business
{
    /// <summary>
    /// Tồn tại trong ca
    /// </summary>
    public class IssueOfShiftBusiness : IIssueOfShiftBusiness
    {
        private readonly IIssueOfShiftService _issueOfShiftService;

        public IssueOfShiftBusiness(IIssueOfShiftService issueOfShiftService)
        {
            _issueOfShiftService = issueOfShiftService;
        }

        public ResponseBaseModel Find(SearchIssueOfShiftModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _issueOfShiftService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ISSUE_OF_SHIFT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(), 
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel UpdateIssueOfShiftDetail(IssueOfShiftDetailUpdate requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _issueOfShiftService.UpdateIssueOfShiftDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ISSUE_OF_SHIFT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(), 
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel AddIssueOfShiftDetail(IssueOfShiftDetailAdd requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _issueOfShiftService.AddIssueOfShiftDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ISSUE_OF_SHIFT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(), 
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetIssueOfShift(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _issueOfShiftService.GetIssueOfShiftById(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ISSUE_OF_SHIFT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(), 
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel DeleteIssueOfShift(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _issueOfShiftService.DeleteIssueOfShift(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ISSUE_OF_SHIFT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(), 
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel DeteleIssueOfShiftDetail(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _issueOfShiftService.DeteleIssueOfShiftDetail(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ISSUE_OF_SHIFT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(), 
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel SyncDataIssueOfShiftDetail(IssueOfShiftDetailSyncDataModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _issueOfShiftService.SyncDataIssueOfShiftDetail(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.ISSUE_OF_SHIFT_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(), ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Xuất dữ liệu tồn tại trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportData(SearchIssueOfShiftModel requestModel)
        {
            try
            {
                return _issueOfShiftService.ExportData(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.IssueOfShift.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}
