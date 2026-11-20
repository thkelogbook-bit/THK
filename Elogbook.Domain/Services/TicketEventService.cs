using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Data;
using System;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class TicketEventService : ITicketEventService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public TicketEventService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Public methods

        public bool CheckDuplicateData(int id, int dataType, int eventTypeID, int factoryID, bool isAddMode)
        {
            // Trường hợp thêm mới thì check Loại ticket event và Nhà máy
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<TicketEvent>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.DataType == dataType
                                                                                    && x.EventTypeID == eventTypeID
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì check Loại ticket event , Nhà máy khác Ticket Event đang cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<TicketEvent>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.ID != id
                                                                                    && x.DataType == dataType
                                                                                    && x.EventTypeID == eventTypeID
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public bool CreateTicketEvent(AddEditTicketEventModel requestModel)
        {
            var entity = new TicketEvent
            {
                FactoryID = requestModel.FactoryID,
                DataType = requestModel.DataType,
                EventTypeID = requestModel.EventTypeID,
                EventContent = requestModel.EventContent,
                Active = (int)AccessStatus.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<TicketEvent>().Add(entity);

            return _unitOfWork.Complete() > 0;
        }

        public bool DeleteTicketEvent(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<TicketEvent>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<TicketEvent>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public TicketEventModel GetTicketEventById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<TicketEvent>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
               e => new TicketEventModel()
               {
                   ID = e.ID,
                   Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                   DataType = e.DataType,
                   DataTypeInfo = new ValueLabelModel { Value = e.DataType, Label = e.DataType == 5 ? "Phiếu công tác điện" : "Phiếu công tác thủy-cơ-nhiệt-hóa" },
                   EventTypeID = e.EventTypeID,
                   EventType = new ValueLabelModel { Value = e.EventType.ID, Label = e.EventType.Description },
                   EventContent = e.EventContent,
                   IsSystem = e.IsSystem,
               }).FirstOrDefault();

            return entity;
        }

        public ResponseBaseModel GetTicketEvents(SearchTicketEventModel requestModel)
        {
            // Tất cả thông số theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<TicketEvent>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            //Loại phiếu
            if (requestModel.DataType > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.DataType == requestModel.DataType);
            }

            //Loại ticket event
            if (requestModel.EventTypeID > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.EventTypeID == requestModel.EventTypeID);
            }

            //Nội dung mẫu
            if (!string.IsNullOrWhiteSpace(requestModel.EventContent))
            {
                entities = entities.Where(x => x.EventContent.Contains(requestModel.EventContent));
            }

            var page = entities.Select(
                e => new TicketEventModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    DataType = e.DataType,
                    DataTypeInfo = new ValueLabelModel { Value = e.DataType, Label = e.DataType == 5 ? "Phiếu công tác điện" : "Phiếu công tác thủy-cơ-nhiệt-hóa" },
                    EventTypeID = e.EventTypeID,
                    EventType = new ValueLabelModel { Value = e.EventType.ID, Label = e.EventType.Description },
                    EventContent = e.EventContent,
                    InsertedAt = e.InsertedAt,
                    IsSystem = e.IsSystem,
                })
                .OrderBy(t => t.ID)
                .ThenBy(t => t.Factory.Value)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool UpdateTicketEvent(AddEditTicketEventModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<TicketEvent>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.FactoryID = requestModel.FactoryID;
                entity.DataType = requestModel.DataType;
                entity.EventTypeID = requestModel.EventTypeID;
                entity.EventContent = requestModel.EventContent;
                entity.Active = (int)AccessStatus.Active;
                entity.UpdatedAt = DateTime.Now;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<TicketEvent>().Update(entity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        #endregion
    }
}
