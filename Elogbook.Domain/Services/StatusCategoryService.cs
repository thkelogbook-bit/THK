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
    public class StatusCategoryService : IStatusCategoryService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public StatusCategoryService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetStatusCategories(SearchStatusCategoryModel requestModel)
        {
            // Filter data by condition
            var entities = _unitOfWork.ElogbookRepository<StatusCategory>().GetQuery(x => (requestModel.DataTypeID == CommonConstant.ALL_KEY || x.DataTypeID == requestModel.DataTypeID)
                                                                                && x.StatusName.Contains(requestModel.StatusName)
                                                                                && (requestModel.Active == CommonConstant.ALL_KEY || x.Active == requestModel.Active)
                                                                                && !x.IsDeleted);

            // Return and paging data
            var page = entities.Select(
                e => new StatusCategoryModel()
                {
                    ID = e.ID,
                    StatusName = e.StatusName,
                    DataType = new ValueLabelModel { Value = e.DataType.ID, Label = e.DataType.Description },
                    Active = e.Active,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(t => t.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public StatusCategoryModel GetStatusCategoryById(int id)
        {
            return _unitOfWork.ElogbookRepository<StatusCategory>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new StatusCategoryModel()
                {
                    ID = e.ID,
                    StatusName = e.StatusName,
                    DataType = new ValueLabelModel { Value = e.DataType.ID, Label = e.DataType.Description },
                    Active = e.Active,
                }).FirstOrDefault();
        }

        public StatusCategoryModel AddStatusCategory(AddEditStatusCategoryModel requestModel)
        {
            var entity = new StatusCategory
            {
                StatusName = requestModel.StatusName,
                DataTypeID = requestModel.DataTypeID,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<StatusCategory>().Add(entity);
            _unitOfWork.Complete();

            return GetStatusCategoryById(entity.ID);
        }

        public StatusCategoryModel UpdateStatusCategory(AddEditStatusCategoryModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<StatusCategory>().GetById(requestModel.ID);
            if (entity != null)
            {
                entity.StatusName = requestModel.StatusName;
                entity.DataTypeID = requestModel.DataTypeID;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<StatusCategory>().Update(entity);
                _unitOfWork.Complete();
                return GetStatusCategoryById(entity.ID);
            }

            return new StatusCategoryModel();
        }

        public bool DeleteStatusCategory(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<StatusCategory>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<StatusCategory>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckInUseStatusCategory(int id)
        {
            return false;
        }

        public bool CheckDuplicateData(int statusID, string statusName, int dataTypeID, bool isAddMode)
        {
            // Trường hợp thêm mới thì kiểm tra trạng thái trong cùng danh mục dữ liệu
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<StatusCategory>().GetQuery(x => x.DataTypeID == dataTypeID
                                                                                    && x.StatusName.Equals(statusName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì kiểm tra với những trạng thái khác
            else
            {
                return _unitOfWork.ElogbookRepository<StatusCategory>().GetQuery(x => x.DataTypeID == dataTypeID
                                                                                    && x.ID != statusID
                                                                                    && x.StatusName.Equals(statusName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }
    }
}
