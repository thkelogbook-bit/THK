using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AiService : IAiService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        public AiService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// GetConfigRuleById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConfigWarningRuleResponseModel GetConfigRuleDefaul()
        {
            var entity = _unitOfWork.ElogbookRepository<ConfigurationWarningRule>().GetQuery(x=> !x.IsDeleted)
                .Select(e => new ConfigWarningRuleResponseModel()
                {
                    ID = e.ID,
                    RuleTypeValue = e.RuleType,
                    ThresholdPercent1 = e.ThresholdPercent1,
                    ThresholdPercent2 = e.ThresholdPercent2,
                    ThresholdPercent3 = e.ThresholdPercent3,
                    ConsecutiveExceedCount1 = e.ConsecutiveExceedCount1,
                    ConsecutiveExceedCount2 = e.ConsecutiveExceedCount2,
                    ConsecutiveExceedCount3 = e.ConsecutiveExceedCount3,
                    IsActive = e.IsActive
                }).FirstOrDefault();

            return entity;
        }

        /// <summary>
        /// GetConfigById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConfigWarningResponseModel GetConfigDefaul()
        {
            var entity = _unitOfWork.ElogbookRepository<ConfigurationWarning>().GetQuery(x => !x.IsDeleted)
                .Select(e => new ConfigWarningResponseModel()
                {
                    ID = e.ID,
                    Days = e.Days,
                    IsActive = e.IsActive
                }).FirstOrDefault();

            return entity;
        }

        /// <summary>
        /// SaveConfigWarningRule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> SaveConfigWarningRule(AddEditConfigWarningRuleRequestModel requestModel)
        {
            var response = new ResponseModel<bool>();

            if (!Enum.IsDefined(typeof(EnumAiWarningRule), requestModel.RuleType))
            {
                response.ErrorMessages.Add("Quy tắc không hợp lệ");
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }

            if (requestModel.ID > 0)
            {
                var entity = _unitOfWork.ElogbookRepository<ConfigurationWarningRule>().GetById(requestModel.ID);
                if (entity == null)
                {
                    response.ErrorMessages.Add("Không tìm thấy mức độ cảnh báo cần chỉnh sửa");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                entity.ConfigurationWarningID = requestModel.ConfigurationWarningID;
                entity.RuleType = requestModel.RuleType;
                entity.ThresholdPercent1 = requestModel.ThresholdPercent1;
                entity.ThresholdPercent2 = requestModel.ThresholdPercent2;
                entity.ThresholdPercent3 = requestModel.ThresholdPercent3;
                entity.ConsecutiveExceedCount1 = requestModel.ConsecutiveExceedCount1;
                entity.ConsecutiveExceedCount2 = requestModel.ConsecutiveExceedCount2;
                entity.ConsecutiveExceedCount3 = requestModel.ConsecutiveExceedCount3;
                entity.IsActive = requestModel.IsActive;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<ConfigurationWarningRule>().Update(entity);
            }
            else
            {
                var entity = new ConfigurationWarningRule
                {
                    ConfigurationWarningID = requestModel.ConfigurationWarningID,
                    RuleType = requestModel.RuleType,
                    ThresholdPercent1 = requestModel.ThresholdPercent1,
                    ThresholdPercent2 = requestModel.ThresholdPercent2,
                    ThresholdPercent3 = requestModel.ThresholdPercent3,
                    ConsecutiveExceedCount1 = requestModel.ConsecutiveExceedCount1,
                    ConsecutiveExceedCount2 = requestModel.ConsecutiveExceedCount2,
                    ConsecutiveExceedCount3 = requestModel.ConsecutiveExceedCount3,
                    IsActive = requestModel.IsActive,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                };

                _unitOfWork.ElogbookRepository<ConfigurationWarningRule>().Add(entity);
            }

            var rs = _unitOfWork.Complete() > 0;
            response.Data = rs;
            return response;
        }

        /// <summary>
        /// SaveConfigWarning
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> SaveConfigWarning(AddEditConfigWarningRequestModel requestModel)
        {
            var response = new ResponseModel<bool>();

            if (requestModel.ID > 0)
            {
                var entity = _unitOfWork.ElogbookRepository<ConfigurationWarning>().GetById(requestModel.ID);
                if (entity == null)
                {
                    response.ErrorMessages.Add("Không tìm thấy cấu hình cần chỉnh sửa");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                entity.Days = requestModel.Days;
                entity.IsActive = requestModel.IsActive;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<ConfigurationWarning>().Update(entity);
            }
            else
            {
                var entity = new ConfigurationWarning
                {
                    Days = requestModel.Days,
                    IsActive = requestModel.IsActive,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                };

                _unitOfWork.ElogbookRepository<ConfigurationWarning>().Add(entity);
            }

            var rs = _unitOfWork.Complete() > 0;
            response.Data = rs;
            return response;
        }
    }
}
