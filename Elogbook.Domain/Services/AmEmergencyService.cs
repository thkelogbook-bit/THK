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
    public class AmEmergencyService : IAmEmergencyService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public AmEmergencyService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckDuplicateData(AmAddEditEmergencyModel requestModel, bool isAddMode)
        {
            // Trường hợp thêm mới
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<AmEmergency>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                    && x.Name.Equals(requestModel.Name, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<AmEmergency>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                    && x.ID != requestModel.ID
                                                                                    && x.Name.Equals(requestModel.Name, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public AmEmergencyModel Add(AmAddEditEmergencyModel requestModel)
        {
            var entity = new AmEmergency
            {
                Name = requestModel.Name,
                FactoryID = requestModel.FactoryID,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<AmEmergency>().Add(entity);
            _unitOfWork.Complete();

            return GetById(entity.ID);
        }

        public AmEmergencyModel GetById(int id)
        {
            return _unitOfWork.ElogbookRepository<AmEmergency>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new AmEmergencyModel()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Avtive = e.Active
                }).FirstOrDefault();
        }

        public ResponseBaseModel Find(AmSearchEmergencyModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmEmergency>().GetQuery(x => !x.IsDeleted);

            //Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            //Trạng thái sử dụng
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            //Tên mức độ khẩn cấp
            if (!string.IsNullOrWhiteSpace(requestModel.Name))
            {
                entities = entities.Where(x => x.Name.Contains(requestModel.Name));
            }

            // Return and paging data
            var page = entities.Select(
                e => new AmEmergencyModel()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Avtive = e.Active,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(t => t.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public AmEmergencyModel Update(AmAddEditEmergencyModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmEmergency>().GetById(requestModel.ID);
            if (entity != null)
            {
                entity.Name = !string.IsNullOrWhiteSpace(requestModel.Name) ? requestModel.Name : entity.Name;
                entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<AmEmergency>().Update(entity);
                _unitOfWork.Complete();
                return GetById(entity.ID);
            }

            return new AmEmergencyModel();
        }

        public bool Delete(int id, int currentUserId)
        {
            var entity = _unitOfWork.ElogbookRepository<AmEmergency>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<AmEmergency>().Delete(entity, currentUserId);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckDataIsUse(int id)
        {
            if (_unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.AmEmergencyId == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }
    }
}
