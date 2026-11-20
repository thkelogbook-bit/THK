using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models.Request.Widget;
using Elogbook.Core.Models.Response.Widget;
using Elogbook.Data;
using Elogbook.Data.DataModels.Entities.Report;
using Elogbook.Data.DataModels.Entities.Widget;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services
{
    class WidgetService : IWidgetService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        public WidgetService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public WidgetViewModel Add(AddEditWidgetModel addEditWidgetModel)
        {
            Widget widget = new Widget();
            MapToWidget(widget, addEditWidgetModel);
            _unitOfWork.ElogbookRepository<Widget>().Add(widget);
            _unitOfWork.Complete();
            return new WidgetViewModel()
            {
                Id = widget.ID,
                Name = widget.Name,
                Description = widget.Description,
                WidgetType = widget.WidgetType,
                ChartType = widget.ChartType,
                ModuleType = widget.ModuleType,
                ReportId = widget.ReportId,
                Order = widget.Order,
            };
        }

        public bool Delete(int id, int currentUserId)
        {
            var entity = _unitOfWork.ElogbookRepository<Widget>().GetQuery(x => !x.IsDeleted && x.ID == id).FirstOrDefault();
            if (entity != null)
            {
                var report = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.ID == entity.ReportId).FirstOrDefault();
                if (report != null)
                {
                    report.IsAddToDashboard = false;
                    _unitOfWork.ElogbookRepository<Report>().Update(report);
                }
                _unitOfWork.ElogbookRepository<Widget>().Delete(entity, currentUserId);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        public bool DeleteByReportId(int reportId, int currentUserId)
        {
            var entity = _unitOfWork.ElogbookRepository<Widget>().GetQuery(x => !x.IsDeleted && x.ReportId == reportId).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<Widget>().Delete(entity, currentUserId);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }


        public WidgetViewModel Edit(AddEditWidgetModel addEditWidgetModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Widget>().GetQuery(x => !x.IsDeleted && x.ID == addEditWidgetModel.Id).FirstOrDefault();
            if (entity != null)
            {
                MapToWidget(entity, addEditWidgetModel);
                _unitOfWork.ElogbookRepository<Widget>().Update(entity);
                _unitOfWork.Complete();
                return new WidgetViewModel()
                {
                    Id = entity.ID,
                    Name = entity.Name,
                    Description = entity.Description,
                    WidgetType = entity.WidgetType,
                    ChartType = entity.ChartType,
                    ModuleType = entity.ModuleType,
                    ReportId = entity.ReportId,
                    Order = entity.Order,
                };
            }
            return null;

        }

        private void MapToWidget(Widget widget, AddEditWidgetModel addEditWidgetModel)
        {
            widget.Name = addEditWidgetModel.Name;
            widget.Description = addEditWidgetModel.Description;
            widget.WidgetType = addEditWidgetModel.WidgetType;
            widget.ChartType = addEditWidgetModel.ChartType;
            widget.ModuleType = addEditWidgetModel.ModuleType;
            widget.ReportId = addEditWidgetModel.ReportId;
            widget.LastUpdatedUserID = addEditWidgetModel.CurrentUserID;
            widget.IsInOverviewDashboard = addEditWidgetModel.IsInOverviewDashboard;
            if (addEditWidgetModel.Id == null)
            {
                widget.IsDeleted = false;
                widget.CreatedUserID = addEditWidgetModel.CurrentUserID;

            }
        }

        public List<WidgetViewModel> FindByModule(string module)
        {
            var entity = _unitOfWork.ElogbookRepository<Widget>().GetQuery(x => !x.IsDeleted && (x.ModuleType == module || (module.Equals("TQ") && x.IsInOverviewDashboard)));
            return entity.Select(e => new WidgetViewModel()
            {
                Id = e.ID,
                Name = e.Name,
                ChartType = e.ChartType,
                WidgetType = e.WidgetType,
                ModuleType = e.ModuleType,
                ReportId = e.ReportId,
                Order = e.Order
            }).OrderBy(e => e.Order).ToList();
        }

        public List<WidgetLabelModel> FindByModuleNotOverview(string module)
        {
            var entity = _unitOfWork.ElogbookRepository<Widget>().GetQuery(x => !x.IsDeleted && x.ModuleType == module && !module.Equals("TQ") && !x.IsInOverviewDashboard);
            return entity.Select(e => new WidgetLabelModel()
            {
                Id = e.ID,
                Name = e.Name
            }).ToList();
        }

        public bool AddToOverview(int id, int currentUserId)
        {
            var entity = _unitOfWork.ElogbookRepository<Widget>().GetQuery(x => !x.IsDeleted && x.ID == id).FirstOrDefault();
            if (entity != null)
            {
                entity.IsInOverviewDashboard = true;
                _unitOfWork.ElogbookRepository<Widget>().Update(entity);
                _unitOfWork.Complete();
                return true;
            }
            return false;
        }

        public bool RemoveFromOverview(int id, int currentUserId)
        {
            var entity = _unitOfWork.ElogbookRepository<Widget>().GetQuery(x => !x.IsDeleted && x.ID == id).FirstOrDefault();
            if (entity != null)
            {
                entity.IsInOverviewDashboard = false;
                _unitOfWork.ElogbookRepository<Widget>().Update(entity);
                _unitOfWork.Complete();
                return true;
            }
            return false;
        }
    }
}

