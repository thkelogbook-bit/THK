using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public EmailTemplateService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetEmailTemplates(SearchEmailTemplateRequestModel requestModel)
        {
            // Filter data by condition
            var entities = _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => !x.IsDeleted);

            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.EmailSubject))
            {
                entities = entities.Where(x => x.EmailSubject.Contains(requestModel.EmailSubject));
            }

            if (requestModel.ModuleID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ModuleID == requestModel.ModuleID);
            }

            if (requestModel.NotificationTypeID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.NotificationTypeID == requestModel.NotificationTypeID);
            }

            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            // Return and paging data
            var page = entities.Select(
                e => new EmailTemplateResponseModel()
                {
                    ID = e.ID,
                    NotificationType = new ValueLabelCheckedModel { Value = e.NotificationType.ID, Label = e.NotificationType.Description },
                    Module = new ValueLabelCheckedModel { Value = e.Module.ID, Label = e.Module.Description },
                    EmailSubject = e.EmailSubject,
                    EmailContent = e.EmailContent,
                    EmailCode = e.EmailCode,
                    Active = e.Active,
                    Factory = new ValueLabelCheckedModel
                    {
                        Value = e.Factory != null ? e.Factory.ID : 0,
                        Label = e.Factory != null ? e.Factory.FactoryName : string.Empty
                    },
                    InsertedAt = e.InsertedAt,
                    ModuleID = e.ModuleID,
                }).OrderBy(t => t.ID)
                .ThenBy(t => t.ModuleID)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public EmailTemplateResponseModel GetEmailTemplateById(int id)
        {
            return _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new EmailTemplateResponseModel()
                {
                    ID = e.ID,
                    NotificationType = new ValueLabelCheckedModel { Value = e.NotificationType.ID, Label = e.NotificationType.Description },
                    Module = new ValueLabelCheckedModel { Value = e.Module.ID, Label = e.Module.Description },
                    EmailSubject = e.EmailSubject,
                    EmailContent = e.EmailContent,
                    EmailCode = e.EmailCode,
                    Active = e.Active,
                    Factory = new ValueLabelCheckedModel
                    {
                        Value = e.Factory != null ? e.Factory.ID : 0,
                        Label = e.Factory != null ? e.Factory.FactoryName : string.Empty
                    },
                }).FirstOrDefault();
        }

        public EmailTemplateResponseModel AddEmailTemplate(AddEditEmailTemplateRequestModel requestModel)
        {
            var entity = new EmailTemplate
            {
                ModuleID = requestModel.ModuleID,
                NotificationTypeID = requestModel.NotificationTypeID,
                EmailSubject = requestModel.EmailSubject,
                EmailContent = System.Web.HttpUtility.HtmlDecode(requestModel.EmailContent),
                EmailCode = requestModel.EmailCode,
                FactoryID = requestModel.FactoryID,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<EmailTemplate>().Add(entity);
            _unitOfWork.Complete();

            return GetEmailTemplateById(entity.ID);
        }

        public EmailTemplateResponseModel UpdateEmailTemplate(AddEditEmailTemplateRequestModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<EmailTemplate>().GetById(requestModel.ID);
            if (entity != null)
            {
                entity.ModuleID = requestModel.ModuleID > 0 ? requestModel.ModuleID : entity.ModuleID;
                entity.NotificationTypeID = requestModel.NotificationTypeID > 0 ? requestModel.NotificationTypeID : entity.NotificationTypeID;
                entity.EmailSubject = !string.IsNullOrWhiteSpace(requestModel.EmailSubject) ? requestModel.EmailSubject : entity.EmailSubject;
                entity.EmailContent = !string.IsNullOrWhiteSpace(requestModel.EmailContent) ? System.Web.HttpUtility.HtmlDecode(requestModel.EmailContent) : entity.EmailContent;
                entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<EmailTemplate>().Update(entity);
                _unitOfWork.Complete();

                return GetEmailTemplateById(entity.ID);
            }

            return new EmailTemplateResponseModel();
        }

        public bool DeleteEmailTemplate(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<EmailTemplate>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<EmailTemplate>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckInUseEmailTemplate(int id)
        {
            return false;
        }

        public bool CheckDuplicateData(AddEditEmailTemplateRequestModel requestModel, bool isAddMode)
        {
            //Check tồn tại email template cùng event trong nhà máy
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => x.ModuleID == requestModel.ModuleID
                                                                                    && x.NotificationTypeID == requestModel.NotificationTypeID
                                                                                    && x.EmailCode == requestModel.EmailCode
                                                                                    && !x.IsDeleted).Any();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => x.ID != requestModel.ID
                                                                                    && x.ModuleID == requestModel.ModuleID
                                                                                    && x.NotificationTypeID == requestModel.NotificationTypeID
                                                                                    && !x.IsDeleted).Any();
            }
        }

        /// <summary>
        /// lấy email mẫu theo mã module và notification
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public EmailTemplateResponseModel GetTemplateByModuleAndNotification(int moduleId, int notificationId)
        {
            var entity = _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => x.ModuleID == moduleId && x.NotificationTypeID == notificationId).FirstOrDefault();
            if (entity != null)
            {
                return GetEmailTemplateById(entity.ID);
            }

            return null;
        }
    }
}
