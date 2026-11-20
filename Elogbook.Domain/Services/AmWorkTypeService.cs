using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AmWorkTypeService : IAmWorkTypeService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public AmWorkTypeService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckDuplicateWorkType(int id, string name, bool isCreate, int factoryID)
        {
            // Trường hợp thêm mới
            if (isCreate)
            {
                return _unitOfWork.ElogbookRepository<AmWorkType>()
                    .GetQuery(x => x.TypeDesc.Equals(name, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted && x.FactoryID == factoryID).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<AmWorkType>()
                    .GetQuery(x => x.ID != id && x.TypeDesc.Equals(name, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted && x.FactoryID == factoryID).Any();
            }
        }

        public bool CreateWorkType(AmWorkTypeAddEditModel requestModel)
        {
            var entity = new AmWorkType
            {
                TypeDesc = requestModel.TypeDesc,
                FactoryID = requestModel.FactoryID,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<AmWorkType>().Add(entity);
            return _unitOfWork.Complete() > 0;
        }

        public ResponseBaseModel DeleteWorkType(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkType>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {

                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "Loại công việc"));
                }

                _unitOfWork.ElogbookRepository<AmWorkType>().Delete(entity, userID);

                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "loại công việc"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "loại công việc"));
                }
            }
            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "Loại công việc"));
        }

        public List<ComboboxDataModel> GetWorkTypeByFactoryID(int factoryID)
        {
            var amWorkTypes = new List<ComboboxDataModel>();

            var entities = _unitOfWork.ElogbookRepository<AmWorkType>()
                .GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

            if (factoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == factoryID);
            }

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.TypeDesc,
                }).ToList();

            if (listData.Count > 0)
            {
                amWorkTypes.AddRange(listData);
            }

            return amWorkTypes;
        }

        public AmWorkTypeModel GetWorkTypeById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkType>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmWorkTypeModel()
              {
                  ID = e.ID,
                  Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  Active = e.Active,
                  TypeDesc = e.TypeDesc,
              }).FirstOrDefault();

            return entity;
        }

        public ResponseBaseModel GetWorkTypes(AmWorkTypeSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmWorkType>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tên loại công việc
            if (!string.IsNullOrWhiteSpace(requestModel.TypeDesc))
            {
                entities = entities.Where(x => x.TypeDesc.Contains(requestModel.TypeDesc));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            var page = entities.Select(
                e => new AmWorkTypeModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                    TypeDesc = e.TypeDesc,
                    InsertedAt = e.InsertedAt,
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool UpdateWorkType(AmWorkTypeAddEditModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkType>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.TypeDesc = !string.IsNullOrWhiteSpace(requestModel.TypeDesc) ? requestModel.TypeDesc : entity.TypeDesc;
                entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AmWorkType>().Update(entity);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        private bool CheckDataIsUse(int id)
        {
            if (_unitOfWork.ElogbookRepository<AmWork>().GetQuery(x => x.AmWorkTypeID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }
    }
}
