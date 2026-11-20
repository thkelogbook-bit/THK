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
    public class PositionInShiftService : IPositionInShiftService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public PositionInShiftService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int AddPositionInShift(AddEditPositionInShiftModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID == requestModel.OperatinPositionID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                var positionInShift = new PositionInShift
                {
                    OperationPositionID = requestModel.OperatinPositionID,
                    ParentID = requestModel.ParentID,
                    IsSystem = entity.JobTitle == 1 || entity.JobTitle == 2,
                    Position = requestModel.Position,
                    Quantity = requestModel.Quantity,
                    Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID
                };

                _unitOfWork.ElogbookRepository<PositionInShift>().Add(positionInShift);
                var saveSuccess = _unitOfWork.Complete() > 0;
                return saveSuccess ? entity.ID : 0;
            }

            return 0;
        }

        public bool CheckInUsePositionInShift(int id)
        {
            // Kiểm tra đã sử dụng cương vị đã có ghi nhận nhật ký vận hành thì hiển thị "Cương vị trong ca này đã được sử dụng không thể xóa"
            return _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.PositionInShiftID == id && !x.IsDeleted).Any();
        }

        public bool DeletePositionInShift(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<PositionInShift>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<PositionInShift>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public PositionInShiftModel GetPositionInShiftById(int id)
        {
            return _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new PositionInShiftModel()
                {
                    ID = e.ID,
                    PositionInShiftName = e.OperationPosition.OperationPositionName,
                    Position = e.Position,
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Active = e.Active,
                    Quantity = e.Quantity,
                    IsSystem = e.IsSystem,
                }).FirstOrDefault();
        }

        public ResponseBaseModel GetPositionInShifts(SearchPositionInShiftModel requestModel)
        {
            // Filter data by condition
            var entities = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.ParentID == requestModel.OperationPositionID
                                                                                && !x.IsDeleted);

            // Return and paging data
            var page = entities.Select(
                e => new PositionInShiftModel()
                {
                    ID = e.ID,
                    PositionInShiftName = e.OperationPosition.OperationPositionName,
                    Position = e.Position,
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Active = e.Active,
                    Quantity = e.Quantity,
                    IsSystem = e.IsSystem,
                })
                .OrderBy(t => t.Position)
                .ThenBy(t => t.PositionInShiftName)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public PositionInShiftModel UpdatePositionInShift(AddEditPositionInShiftModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<PositionInShift>().GetById(requestModel.ID);
            if (entity != null)
            {
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.Position = requestModel.Position;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                _unitOfWork.ElogbookRepository<PositionInShift>().Update(entity);
                _unitOfWork.Complete();

                return GetPositionInShiftById(entity.ID);
            }

            return new PositionInShiftModel();
        }

        public bool CheckDuplicateData(int operationPositionID, int parentID)
        {
            return _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.OperationPositionID == operationPositionID
                                                                                            && x.ParentID == parentID
                                                                                            && x.Active == (int)AccessStatus.Active
                                                                                            && !x.IsDeleted).Any();
        }
    }
}
