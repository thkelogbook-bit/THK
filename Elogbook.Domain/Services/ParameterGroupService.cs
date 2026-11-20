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
    public class ParameterGroupService : IParameterGroupService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public ParameterGroupService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int AddParameterGroup(AddEditParameterGroupModel requestModel)
        {
            var entity = new ParameterGroup
            {
                ParameterGroupName = requestModel.ParameterGroupName.Trim(),
                FactoryID = requestModel.FactoryID,
                OperationPositionID = requestModel.OperationPositionID,
                CycleID = requestModel.CycleID,
                Position = requestModel.Position,
                Status = requestModel.Status <= 0 ? (int)AccessStatus.InActive : requestModel.Status,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<ParameterGroup>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                return entity.ID;
            }
            else
            {
                return 0;
            }
        }

        public bool CheckDuplicateData(int parameterGroupID, string parameterGroupName, int operationPositionID, bool isAddMode)
        {
            // Trường hợp thêm mới thì check Tên Nhóm thông số không được trùng và Nhà máy có tồn tại
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.OperationPositionID == operationPositionID
                                                                                    && x.ParameterGroupName.Equals(parameterGroupName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì check Tên Nhóm thông số, Nhà máy và những Tổ máy khác Tổ máy đang cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.OperationPositionID == operationPositionID
                                                                                    && x.ID != parameterGroupID
                                                                                    && x.ParameterGroupName.Equals(parameterGroupName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public bool CheckInUseParameterGroup(int id)
        {
            //Kiểm tra đã sử dụng nhóm thông số để cấu hình thông số ghi thông số thì không cho xóa, hiển thị "Nhóm thông số này đã được sử dụng, không thể xóa."
            return _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ParameterGroupID == id && !x.IsDeleted).Any();
        }

        public bool DeleteParameterGroup(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.ID == id && !x.IsDeleted);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<ParameterGroup>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public ParameterGroupModel GetParameterGroupById(int id)
        {
            return _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new ParameterGroupModel()
                {
                    ID = e.ID,
                    ParameterGroupName = e.ParameterGroupName,
                    Position = e.Position,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Status = e.Status,
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                }).FirstOrDefault();
        }

        public ResponseBaseModel GetParameterGroups(SearchParameterGroupModel requestModel)
        {
            // Filter data by condition
            var entities = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => !x.IsDeleted);
            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }
            // Tên nhóm thông số
            if (!string.IsNullOrWhiteSpace(requestModel.ParameterGroupName))
            {
                entities = entities.Where(x => x.ParameterGroupName.Contains(requestModel.ParameterGroupName));
            }
            //Trạng thái sử dụng
            if (requestModel.Status > 0)
            {
                entities = entities.Where(x => x.Status == requestModel.Status);
            }

            // Return and paging data
            var page = entities.Select(
                e => new ParameterGroupModel()
                {
                    ID = e.ID,
                    ParameterGroupName = e.ParameterGroupName,
                    Position = e.Position,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Status = e.Status,
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                    InsertedAt = e.InsertedAt,
                })
                .OrderBy(t => t.Position)
                .ThenBy(t => t.ParameterGroupName)
                .ThenByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            foreach (ParameterGroupModel item in page)
            {
                item.TotalParameter = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => !x.IsDeleted && x.ParameterGroupID == item.ID).Count();
            }
            page.GetEnumerator().Reset();
            return HttpResponse.OkList(page);
        }

        public bool UpdateParameterGroup(AddEditParameterGroupModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (entity.OperationPositionID != requestModel.OperationPositionID)
                {
                    var parameters = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => !x.IsDeleted && x.ParameterGroupID == entity.ID).ToList();
                    foreach (Parameter item in parameters)
                    {
                        item.OperationPositionID = requestModel.OperationPositionID;
                        item.CycleID = requestModel.CycleID;
                        item.FactoryID = requestModel.FactoryID;
                        // Thay đổi trạng thái thông số theo trạng thái nhóm thông số
                        item.Status = requestModel.Status;
                        item.LastUpdatedUserID = requestModel.CurrentUserID;
                    }
                    _unitOfWork.ElogbookRepository<Parameter>().Update(parameters);
                }
                else
                {
                    if (entity.Status != requestModel.Status)
                    {
                        var parameters = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => !x.IsDeleted && x.ParameterGroupID == entity.ID).ToList();
                        foreach (Parameter item in parameters)
                        {
                            // Thay đổi trạng thái thông số theo trạng thái nhóm thông số
                            item.Status = requestModel.Status;
                            item.LastUpdatedUserID = requestModel.CurrentUserID;
                        }
                        _unitOfWork.ElogbookRepository<Parameter>().Update(parameters);
                    }
                }

                entity.ParameterGroupName = requestModel.ParameterGroupName.Trim();
                entity.FactoryID = requestModel.FactoryID;
                entity.Position = requestModel.Position;
                entity.Status = requestModel.Status <= 0 ? (int)AccessStatus.InActive : requestModel.Status;
                entity.OperationPositionID = requestModel.OperationPositionID;
                entity.CycleID = requestModel.CycleID;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<ParameterGroup>().Update(entity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }
    }
}
