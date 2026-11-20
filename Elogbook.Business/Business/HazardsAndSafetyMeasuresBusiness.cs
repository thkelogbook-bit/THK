using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Business
{
    public class HazardsAndSafetyMeasuresBusiness : IHazardsAndSafetyMeasuresBusiness
    {
        private readonly IHazardsAndSafetyMeasuresService _hazardsAndSafetyMeasures;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="am"></param>
        public HazardsAndSafetyMeasuresBusiness(IHazardsAndSafetyMeasuresService hazardsAndSafetyMeasures)
        {
            _hazardsAndSafetyMeasures = hazardsAndSafetyMeasures;
        }

        public ResponseModel<bool> Create(HazardsAndSafetyMeasuresAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
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
                    if (!_hazardsAndSafetyMeasures.CheckDuplicate(0, requestModel.ContentText, requestModel.ContentType, true))
                    {
                        responseModel.Data = _hazardsAndSafetyMeasures.Create(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Lỗi đã tồn tại");
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.HazardsAndSafetyMeasuresBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel Delete(int id, int userId)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _hazardsAndSafetyMeasures.Delete(id, userId);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.HazardsAndSafetyMeasuresBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public List<ValueLabelCheckedModel> FindByContentType(int contentType)
        {
            try
            {
                return _hazardsAndSafetyMeasures.FindByContentType(contentType);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.HazardsAndSafetyMeasuresBusiness.ToString(),
                    Environment.NewLine + $"ID: {contentType} " + Environment.NewLine + ex.ToString());

                return new List<ValueLabelCheckedModel>();
            }
        }

        public ResponseBaseModel GetAlls(HazardsAndSafetyMeasuresSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _hazardsAndSafetyMeasures.GetAlls(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.HazardsAndSafetyMeasuresBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<HazardsAndSafetyMeasuresModel> GetById(int id)
        {
            var responseModel = new ResponseModel<HazardsAndSafetyMeasuresModel>();

            try
            {
                responseModel.Data = _hazardsAndSafetyMeasures.GetById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.HazardsAndSafetyMeasuresBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> Update(HazardsAndSafetyMeasuresAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel.ContentType > 2)
                {
                    _errorMessages.Add("Phân loại không hợp lệ");
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.Active > 2)
                {
                    _errorMessages.Add("Trạng thái không hợp lệ");
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.ID < 1)
                {
                    _errorMessages.Add("ID là bắt buộc");
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                bool isExists = _hazardsAndSafetyMeasures.CheckDuplicate(requestModel.ID, requestModel.ContentText, requestModel.ContentType, true);
                if (isExists)
                {
                    responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "mối nguy hoặc biện pháp an toàn đã tồn tại"));
                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel.Data = _hazardsAndSafetyMeasures.Update(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.HazardsAndSafetyMeasuresBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(HazardsAndSafetyMeasuresAddEditModel requestModel)
        {
            // Nội dung là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.ContentText))
            {
                _errorMessages.Add("Nội dung là bắt buộc");
            }

            if (requestModel.ContentType > 2 || requestModel.ContentType < 1)
            {
                _errorMessages.Add("Phân loại là bắt buộc");
            }

            if (requestModel.Active > 2 || requestModel.Active < 1)
            {
                _errorMessages.Add("Trạng thái không hợp lệ");
            }

            return _errorMessages.Count <= 0;
        }
    }
}
