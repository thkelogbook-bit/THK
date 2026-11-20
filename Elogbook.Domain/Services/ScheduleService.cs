using CronExpressionDescriptor;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using Quartz;
using System.Linq;
using System.Threading.Tasks;

namespace Elogbook.Domain.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IScheduler _scheduler;
        public ScheduleService(ElogbookUnitOfWork unitOfWork, IScheduler scheduler)
        {
            _unitOfWork = unitOfWork;
            _scheduler = scheduler;
        }

        /// <summary>
        /// Kiểm tra cấu hình SLA đã tồn tại trong danh mục SLA
        /// </summary>
        /// <param name="slaConfigId"></param>
        /// <param name="slaCategoryId"></param>
        /// <returns></returns>
        public bool SlaConfigExists(int slaConfigId, int slaCategoryId)
        {
            if (slaConfigId <= 0 || slaCategoryId <= 0) return false;

            return _unitOfWork.ElogbookRepository<SlaConfig>()
                .GetQuery(x => x.ID == slaConfigId && x.SlaCategoryID == slaCategoryId && x.IsActive && !x.IsDeleted).Any();
        }

        public ResponseBaseModel GetSchedules(SearchScheduleRequestModel requestModel)
        {
            // Filter data by condition
            if (requestModel.Active <= 0) requestModel.Active = CommonConstant.ALL_KEY;
            var entities = _unitOfWork.ElogbookRepository<Schedule>().GetQuery(x => (x.ScheduleName.Contains(requestModel.SearchText) || x.ScheduleContent.Contains(requestModel.SearchText))
                                                                                && (requestModel.Active == CommonConstant.ALL_KEY || x.Active == requestModel.Active)
                                                                                && !x.IsDeleted);

            if (requestModel.SlaConfigID.HasValue && requestModel.SlaConfigID.Value > 0)
            {
                entities = entities.Where(x => x.SlaConfigID == requestModel.SlaConfigID);
            }

            if (requestModel.SlaCategoryID.HasValue && requestModel.SlaCategoryID.Value > 0)
            {
                entities = entities.Where(x => x.SlaCategoryID == requestModel.SlaCategoryID);
            }

            // Return and paging data
            var page = entities.Select(
                e => new ScheduleResponseModel()
                {
                    ID = e.ID,
                    ScheduleName = e.ScheduleName,
                    ScheduleContent = e.ScheduleContent,
                    ScheduleValue = e.ScheduleValue,
                    ScheduleDescription = e.ScheduleDescription,
                    Active = e.Active,
                    IsSystem = e.IsSystem,
                    IsSendEmail = e.IsSendEmail,
                    EmailTo = e.EmailTo,
                    EmailCC = e.EmailCC,
                    EmailSubject = e.EmailSubject,
                    EmailContent = e.EmailContent,
                    SlaConfig = e.SlaConfigID.HasValue ? new ScheduleSlaConfigModel()
                    {
                        ID = e.SlaConfig.ID,
                        SlaName = e.SlaConfig.Name,
                        SlaDescription = e.SlaConfig.Description,
                        IsActive = e.SlaConfig.IsActive,
                    } : null,
                    SlaCategory = e.SlaCategoryID.HasValue ? new ScheduleSlaCategoryModel()
                    {
                        ID = e.SlaCategory.ID,
                        SlaCategoryName = e.SlaCategory.Name,
                        SlaCategoryDescription = e.SlaCategory.Description,
                    } : null,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(t => t.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public ScheduleResponseModel GetScheduleById(int id)
        {
            return _unitOfWork.ElogbookRepository<Schedule>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                   e => new ScheduleResponseModel()
                   {
                       ID = e.ID,
                       ScheduleName = e.ScheduleName,
                       ScheduleContent = e.ScheduleContent,
                       ScheduleValue = e.ScheduleValue,
                       ScheduleDescription = e.ScheduleDescription,
                       Active = e.Active,
                       IsSystem = e.IsSystem,
                       IsSendEmail = e.IsSendEmail,
                       EmailTo = e.EmailTo,
                       EmailCC = e.EmailCC,
                       EmailSubject = e.EmailSubject,
                       EmailContent = e.EmailContent,
                       InsertedAt = e.InsertedAt,
                       SlaConfig = e.SlaConfigID.HasValue ? new ScheduleSlaConfigModel()
                       {
                           ID = e.SlaConfig.ID,
                           SlaName = e.SlaConfig.Name,
                           SlaDescription = e.SlaConfig.Description,
                           IsActive = e.SlaConfig.IsActive,
                       } : null,
                       SlaCategory = e.SlaCategoryID.HasValue ? new ScheduleSlaCategoryModel()
                       {
                           ID = e.SlaCategory.ID,
                           SlaCategoryName = e.SlaCategory.Name,
                           SlaCategoryDescription = e.SlaCategory.Description,
                       } : null
                   }).FirstOrDefault();
        }

        public ScheduleResponseModel AddSchedule(AddEditScheduleRequestModel requestModel)
        {
            var entity = new Schedule
            {
                ScheduleName = requestModel.ScheduleName,
                ScheduleContent = requestModel.ScheduleContent,
                ScheduleValue = requestModel.ScheduleValue,
                ScheduleDescription = requestModel.ScheduleDescription,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                IsSystem = requestModel.IsSystem,
                IsSendEmail = requestModel.IsSendEmail,
                EmailTo = requestModel.EmailTo,
                EmailCC = requestModel.EmailCC,
                EmailSubject = requestModel.EmailSubject,
                EmailContent = requestModel.EmailContent,
                SlaConfigID = requestModel.SlaConfigID > 0 ? requestModel.SlaConfigID : null,
                SlaCategoryID = requestModel.SlaCategoryID > 0 ? requestModel.SlaCategoryID : null,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<Schedule>().Add(entity);
            _unitOfWork.Complete();

            return GetScheduleById(entity.ID);
        }

        public async Task DeleteScheduleAM(string idAm, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<Schedule>().GetQuery(x => x.ScheduleName == "AM-Job-"+ idAm).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<Schedule>().Delete(entity, userID);
                _unitOfWork.Complete();

                JobKey Key = new JobKey("JOB-" + idAm, "groupAmTask");
                //Tell Quartz to schedule the job using our trigger
                await _scheduler.DeleteJob(Key);
            }
        }

        public ScheduleResponseModel UpdateSchedule(AddEditScheduleRequestModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Schedule>().GetById(requestModel.ID);
            if (entity != null)
            {
                if (!string.IsNullOrWhiteSpace(requestModel.ScheduleName))
                    entity.ScheduleName = requestModel.ScheduleName;
                entity.ScheduleContent = requestModel.ScheduleContent;
                entity.ScheduleValue = requestModel.ScheduleValue;
                entity.ScheduleDescription = requestModel.ScheduleDescription;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.IsSystem = requestModel.IsSystem;
                entity.IsSendEmail = requestModel.IsSendEmail;
                entity.EmailTo = requestModel.EmailTo;
                entity.EmailCC = requestModel.EmailCC;
                entity.EmailSubject = requestModel.EmailSubject;
                entity.EmailContent = requestModel.EmailContent;
                entity.SlaConfigID = requestModel.SlaConfigID > 0 ? requestModel.SlaConfigID : null;
                entity.SlaCategoryID = requestModel.SlaCategoryID > 0 ? requestModel.SlaCategoryID : null;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<Schedule>().Update(entity);
                _unitOfWork.Complete();

                return GetScheduleById(entity.ID);
            }

            return new ScheduleResponseModel();
        }

        public bool DeleteSchedule(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<Schedule>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<Schedule>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckInUseSchedule(int id)
        {
            return false;
        }

        public bool CheckDuplicateData(int scheduleID, string scheduleName, bool isAddMode)
        {
            //Check tồn tại schedule
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<Schedule>().GetQuery(x => x.ScheduleName == scheduleName
                                                                                    && !x.IsDeleted).Any();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<Schedule>().GetQuery(x => x.ScheduleName.Equals(scheduleName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && x.ID != scheduleID
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public string GetCronJobDescription(CronJobDescriptionRequestModel requestModel)
        {
            return ExpressionDescriptor.GetDescription(requestModel.CronJobValue, new Options() { Locale = "vi-VN" });
        }

    }
}
