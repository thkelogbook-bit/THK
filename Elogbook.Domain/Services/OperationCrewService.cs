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
    public class OperationCrewService : IOperationCrewService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public OperationCrewService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OperationCrewModel AddOperationCrew(AddEditOperationCrewModel requestModel)
        {
            var entity = new OperationCrew
            {
                OperationCrewName = requestModel.OperationCrewName,
                FactoryID = requestModel.FactoryID,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<OperationCrew>().Add(entity);
            _unitOfWork.Complete();

            return GetOperationCrewById(entity.ID);
        }

        public bool CheckDuplicateData(int operationCrewID, string operationCrewName, int factoryID, bool isAddMode)
        {
            // Trường hợp thêm mới thì check Tên kíp vận hành và Nhà máy
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<OperationCrew>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.OperationCrewName.Equals(operationCrewName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì check Tên kíp vận hành, Nhà máy và những Kíp vận hành khác kíp vận hành đang cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<OperationCrew>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.ID != operationCrewID
                                                                                    && x.OperationCrewName.Equals(operationCrewName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public bool CheckInUseOperationCrew(int id)
        {
            // Kiểm tra đã sử dụng kíp vận hành để nhập nhật ký vận hành nào chưa, nếu có thì không cho xóa, hiển thị "Kíp vận hành này đã được sử dụng không thể xóa"
            return _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationCrewID == id && !x.IsDeleted).Any();
        }

        public bool DeleteOperationCrew(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationCrew>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<OperationCrew>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public OperationCrewModel GetOperationCrewById(int id)
        {
            return _unitOfWork.ElogbookRepository<OperationCrew>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new OperationCrewModel()
                {
                    ID = e.ID,
                    OperationCrewName = e.OperationCrewName,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                }).FirstOrDefault();
        }

        public ResponseBaseModel GetOperationCrews(SearchOperationCrewModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<OperationCrew>().GetQuery(x => !x.IsDeleted);
            //Nhà máy
            if (requestModel.FactoryID > 0)
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            //Trạng thái sử dụng
            if (requestModel.Active > 0)
                entities = entities.Where(x => x.Active == requestModel.Active);
            //Tên kíp vận hành
            if (!string.IsNullOrWhiteSpace(requestModel.OperationCrewName))
                entities = entities.Where(x => x.OperationCrewName.Contains(requestModel.OperationCrewName));

            // Return and paging data
            var page = entities.Select(
                e => new OperationCrewModel()
                {
                    ID = e.ID,
                    OperationCrewName = e.OperationCrewName,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(t => t.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public OperationCrewModel UpdateOperationCrew(AddEditOperationCrewModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationCrew>().GetById(requestModel.ID);
            if (entity != null)
            {
                entity.OperationCrewName = requestModel.OperationCrewName;
                entity.FactoryID = requestModel.FactoryID;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<OperationCrew>().Update(entity);
                _unitOfWork.Complete();
                return GetOperationCrewById(entity.ID);
            }

            return new OperationCrewModel();
        }
    }
}
