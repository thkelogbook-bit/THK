using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseCommon;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class JobHistoryService : IJobHistoryService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public JobHistoryService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Public methods

        public ResponseBaseModel Find(SearchJobHistoryModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<PmisJobHistory>().GetQuery(x => !x.IsDeleted);

            var page = entities.Select(
                    e => new JobHistoryModel()
                    {
                        ID = e.ID,
                        Duration = e.Duration,
                        EndTime = e.EndTime,
                        NumRecord = e.NumRecord,
                        Object = e.Object,
                        StartTime = e.StartTime,
                        StatusID = e.Status,
                        SyncTypeID = e.SyncType,
                        UserID = e.UserID,
                        Logs = e.Logs
                    })
                .OrderBy(t => t.ID).ToList();
            IPagedList<JobHistoryModel> pagelist = page.ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            // Kiểm tra quyền edit
            if (pagelist != null && pagelist.TotalRecords > 0)
            {
                foreach (var entity in pagelist)
                {
                    switch (entity.StatusID)
                    {
                        case (int)JobHistoryStatus.FAIL:
                            entity.Status = new ValueLabelModel
                            {
                                Value = entity.StatusID,
                                Label = EnumExtensions.GetEnumDescription(JobHistoryStatus.FAIL)
                            };
                            break;
                        case (int)JobHistoryStatus.IN_PROGRES:
                            entity.Status = new ValueLabelModel
                            {
                                Value = entity.StatusID,
                                Label = EnumExtensions.GetEnumDescription(JobHistoryStatus.IN_PROGRES)
                            };
                            break;
                        case (int)JobHistoryStatus.SUCCESS:
                            entity.Status = new ValueLabelModel
                            {
                                Value = entity.StatusID,
                                Label = EnumExtensions.GetEnumDescription(JobHistoryStatus.SUCCESS)
                            };
                            break;
                    }

                    switch (entity.SyncTypeID)
                    {
                        case (int)JobHistoryType.Auto:
                            entity.SyncType = new ValueLabelModel
                            {
                                Value = entity.SyncTypeID,
                                Label = EnumExtensions.GetEnumDescription(JobHistoryType.Auto)
                            };
                            break;
                        case (int)JobHistoryType.Manual:
                            entity.SyncType = new ValueLabelModel
                            {
                                Value = entity.SyncTypeID,
                                Label = EnumExtensions.GetEnumDescription(JobHistoryType.Manual)
                            };
                            break;
                    }
                }
                pagelist.GetEnumerator().Reset();
            }

            return HttpResponse.OkList(pagelist);
        }

        #endregion

        #region Private methods

        #endregion
    }
}
