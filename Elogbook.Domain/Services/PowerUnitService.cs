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
    public class PowerUnitService : IPowerUnitService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public PowerUnitService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetPowerUnits(SearchPowerUnitModel requestModel)
        {
            // Filter data by condition
            var entities = _unitOfWork.ElogbookRepository<PowerUnit>().GetQuery(x => (requestModel.FactoryID == CommonConstant.ALL_KEY || x.FactoryID == requestModel.FactoryID)
                                                                                && x.PowerUnitName.Contains(requestModel.PowerUnitName)
                                                                                && (requestModel.Active == CommonConstant.ALL_KEY || x.Active == requestModel.Active)
                                                                                && !x.IsDeleted);

            // Return and paging data
            var page = entities.Select(
                e => new PowerUnitModel()
                {
                    ID = e.ID,
                    PowerUnitName = e.PowerUnitName,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(t => t.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public PowerUnitModel GetPowerUnitById(int id)
        {
            return _unitOfWork.ElogbookRepository<PowerUnit>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new PowerUnitModel()
                {
                    ID = e.ID,
                    PowerUnitName = e.PowerUnitName,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                }).FirstOrDefault();
        }

        public PowerUnitModel AddPowerUnit(AddEditPowerUnitModel requestModel)
        {
            var entity = new PowerUnit
            {
                PowerUnitName = requestModel.PowerUnitName,
                FactoryID = requestModel.FactoryID,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<PowerUnit>().Add(entity);
            _unitOfWork.Complete();

            return GetPowerUnitById(entity.ID);
        }

        public PowerUnitModel UpdatePowerUnit(AddEditPowerUnitModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<PowerUnit>().GetById(requestModel.ID);
            if (entity != null)
            {
                entity.PowerUnitName = requestModel.PowerUnitName;
                entity.FactoryID = requestModel.FactoryID;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<PowerUnit>().Update(entity);
                _unitOfWork.Complete();
                return GetPowerUnitById(entity.ID);
            }

            return new PowerUnitModel();
        }

        public bool DeletePowerUnit(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<PowerUnit>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<PowerUnit>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckInUsePowerUnit(int id)
        {
            return false;
        }

        public bool CheckDuplicateData(int powerUnitID, string powerUnitName, int factoryID, bool isAddMode)
        {
            // Trường hợp thêm mới thì check Tên Tổ máy và Nhà máy
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<PowerUnit>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.PowerUnitName.Equals(powerUnitName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì check Tên Tổ máy, Nhà máy và những Tổ máy khác Tổ máy đang cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<PowerUnit>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.ID != powerUnitID
                                                                                    && x.PowerUnitName.Equals(powerUnitName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }
    }
}
