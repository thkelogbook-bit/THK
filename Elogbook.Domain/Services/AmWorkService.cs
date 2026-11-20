using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Linq;
using System.Net;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AmWorkService : IAmWorkService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        public AmWorkService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckDuplicateWork(int id, string name, bool isCreate, int factoryID)
        {
            // Trường hợp thêm mới
            if (isCreate)
            {
                return _unitOfWork.ElogbookRepository<AmWork>()
                    .GetQuery(x => x.WorkName.Equals(name, StringComparison.OrdinalIgnoreCase) && x.FactoryID == factoryID && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<AmWork>()
                    .GetQuery(x => x.ID != id && x.WorkName.Equals(name, StringComparison.OrdinalIgnoreCase) && x.FactoryID == factoryID && !x.IsDeleted).Any();
            }
        }

        public ResponseModel<bool> CreateWork(AmWorkAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            var isExists = _unitOfWork.ElogbookRepository<AmWork>()
                    .GetQuery(x => x.WorkName.Equals(requestModel.WorkName, StringComparison.OrdinalIgnoreCase) 
                    && x.AmWorkTypeID == requestModel.WorkTypeID 
                    && x.FactoryID == requestModel.FactoryID 
                    && !x.IsDeleted).Any();

            if (isExists)
            {
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "công việc"));
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                return responseModel;
            }

            var entity = new AmWork
            {
                WorkName = requestModel.WorkName,
                FactoryID = requestModel.FactoryID,
                AmWorkTypeID = requestModel.WorkTypeID,
                WorkDesc = requestModel.WorkDesc,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<AmWork>().Add(entity);
            var success = _unitOfWork.Complete() > 0;

            if (success)
            {
                responseModel.Data = true;
                responseModel.StatusCode = (int)HttpStatusCode.OK;
                return responseModel;
            }
            responseModel.Data = false;
            responseModel.ErrorMessages.Add("Tạo công việc thất bại");
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            return responseModel;
        }

        public ResponseBaseModel DeleteWork(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWork>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "Công việc"));
                }

                _unitOfWork.ElogbookRepository<AmWork>().Delete(entity, userID);

                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "công việc"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "công việc"));
                }
            }
            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "Công việc"));
        }

        public AmWorkModel GetWorkById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWork>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmWorkModel()
              {
                  ID = e.ID,
                  WorkName = e.WorkName,
                  Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  WorkType = new ValueLabelCheckedModel { Value = e.AmWorkType.ID, Label = e.AmWorkType.TypeDesc },
                  Active = e.Active,
                  WorkDesc = e.WorkDesc
              }).FirstOrDefault();

            return entity;
        }

        public ResponseBaseModel GetWorkByType(int workTypeId, int factoryID)
        {
            var entities = _unitOfWork.ElogbookRepository<AmWork>().GetQuery(x => !x.IsDeleted && x.AmWorkTypeID == workTypeId && x.FactoryID == factoryID)
                .Select(e => new ValueLabelCheckedModel()
                {
                    Value = e.ID,
                    Label = e.WorkName
                }).ToList();

            return HttpResponse.OkList(entities);
        }

        public ResponseBaseModel GetWorks(AmWorkSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmWork>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tên Công việc
            if (!string.IsNullOrWhiteSpace(requestModel.WorkName))
            {
                entities = entities.Where(x => x.WorkName.Contains(requestModel.WorkName));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            // Loại công việc
            if (requestModel.WorkTypeID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmWorkTypeID == requestModel.WorkTypeID);
            }

            var page = entities.Select(
                e => new AmWorkModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    WorkType = new ValueLabelCheckedModel() { Value = e.AmWorkType.ID, Label = e.AmWorkType.TypeDesc },
                    WorkName = e.WorkName,
                    Active = e.Active,
                    WorkDesc = e.WorkDesc,
                    InsertedAt = e.InsertedAt,
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public ResponseModel<bool> UpdateWork(AmWorkAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            var entity = _unitOfWork.ElogbookRepository<AmWork>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                var isExists = _unitOfWork.ElogbookRepository<AmWork>()
                    .GetQuery(x => x.ID != entity.ID && 
                    x.WorkName.Equals(requestModel.WorkName, StringComparison.OrdinalIgnoreCase)
                    && x.AmWorkTypeID == requestModel.WorkTypeID
                    && x.FactoryID == requestModel.FactoryID
                    && !x.IsDeleted).Any();

                if (isExists)
                {
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "công việc"));
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }
                entity.WorkName = !string.IsNullOrWhiteSpace(requestModel.WorkName) ? requestModel.WorkName : entity.WorkName;
                entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                entity.AmWorkTypeID = requestModel.WorkTypeID > 0 ? requestModel.WorkTypeID : entity.AmWorkTypeID;
                entity.WorkDesc = !string.IsNullOrWhiteSpace(requestModel.WorkDesc) ? requestModel.WorkDesc : entity.WorkDesc;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AmWork>().Update(entity);

                var success = _unitOfWork.Complete() > 0;
                if (success)
                {
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                responseModel.Data = false;
                responseModel.ErrorMessages.Add("Tạo công việc thất bại");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                return responseModel;
            }
            responseModel.Data = false;
            responseModel.ErrorMessages.Add("Công việc cần cập nhật không tồn tại");
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            return responseModel;
        }

        private bool CheckDataIsUse(int id)
        {
            if (_unitOfWork.ElogbookRepository<AmMaintenanceWork>().GetQuery(x => x.AmWorkID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }
    }
}
