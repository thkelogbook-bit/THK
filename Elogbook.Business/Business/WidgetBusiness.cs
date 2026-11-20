using Elogbook.Core.Enums;
using Elogbook.Core;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Widget;
using Elogbook.Core.Models.Response.Widget;
using System.Net;
using System;
using System.Collections.Generic;

namespace Elogbook.Business.Business
{
    /// <summary>
    /// 
    /// </summary>
    class WidgetBusiness : IWidgetBusiness
    {

        private readonly IWidgetService _widgetService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dashboardIncidentService"></param>
        public WidgetBusiness(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        public ResponseModel<WidgetViewModel> Add(AddEditWidgetModel addEditWidgetModel)
        {
            var responseModel = new ResponseModel<WidgetViewModel>();
            _errorMessages = new List<string>();

            try
            {
                responseModel.Data = _widgetService.Add(addEditWidgetModel);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WidgetBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(addEditWidgetModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> Delete(int id, int currentUserId)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _widgetService.Delete(id, currentUserId);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WidgetBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<WidgetViewModel> Edit(AddEditWidgetModel addEditWidgetModel)
        {
            var responseModel = new ResponseModel<WidgetViewModel>();
            _errorMessages = new List<string>();

            try
            {
                responseModel.Data = _widgetService.Edit(addEditWidgetModel);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WidgetBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(addEditWidgetModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<List<WidgetViewModel>> FindByModule(string module)
        {
            var responseBaseModel = new ResponseModel<List<WidgetViewModel>>();

            try
            {
                responseBaseModel.Data = _widgetService.FindByModule(module);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WidgetBusiness.ToString(),
                    Environment.NewLine + module + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<List<WidgetLabelModel>> FindByModuleNotOverview(string module)
        {
            var responseBaseModel = new ResponseModel<List<WidgetLabelModel>>();

            try
            {
                responseBaseModel.Data = _widgetService.FindByModuleNotOverview(module);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WidgetBusiness.ToString(),
                    Environment.NewLine + module + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> AddToOverview(int id, int currentUserId)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _widgetService.AddToOverview(id, currentUserId);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WidgetBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> RemoveFromOverview(int id, int currentUserId)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _widgetService.RemoveFromOverview(id, currentUserId);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WidgetBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
