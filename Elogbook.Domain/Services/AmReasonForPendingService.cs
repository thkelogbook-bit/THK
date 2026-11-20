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

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AmReasonForPendingService : IAmReasonForPendingService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public AmReasonForPendingService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckDuplicateReasonForPending(int id, string name, bool isCreate, int factoryID)
        {
            // Trường hợp thêm mới
            if (isCreate)
            {
                return _unitOfWork.ElogbookRepository<AmReasonForPending>()
                    .GetQuery(x => x.ReasonDesc.Equals(name, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted && x.FactoryID == factoryID).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<AmReasonForPending>()
                    .GetQuery(x => x.ID != id && x.ReasonDesc.Equals(name, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted && x.FactoryID == factoryID).Any();
            }
        }

        public bool CreateReasonForPending(AmReasonForPendingAddEditModel requestModel)
        {
            var entity = new AmReasonForPending
            {
                ReasonDesc = requestModel.ReasonDesc,
                FactoryID = requestModel.FactoryID,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<AmReasonForPending>().Add(entity);
            return _unitOfWork.Complete() > 0;
        }

        public ResponseBaseModel DeleteReasonForPending(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmReasonForPending>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "Nguyên nhân"));
                }
                _unitOfWork.ElogbookRepository<AmReasonForPending>().Delete(entity, userID);
                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "nguyên nhân"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "nguyên nhân"));
                }
            }
            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "Nguyên nhân"));
        }

        public AmReasonForPendingModel GetReasonForPendingById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmReasonForPending>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmReasonForPendingModel()
              {
                  ID = e.ID,
                  Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  Active = e.Active,
                  ReasonDesc = e.ReasonDesc,
              }).FirstOrDefault();

            return entity;

        }

        public ResponseBaseModel GetReasonForPendings(AmReasonForPendingSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmReasonForPending>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Tên nguyên nhân tạm dừng công việc
            if (!string.IsNullOrWhiteSpace(requestModel.ReasonDesc))
            {
                entities = entities.Where(x => x.ReasonDesc.Contains(requestModel.ReasonDesc));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            var page = entities.Select(
                e => new AmReasonForPendingModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                    ReasonDesc = e.ReasonDesc,
                    InsertedAt = e.InsertedAt,
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool UpdateReasonForPending(AmReasonForPendingAddEditModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmReasonForPending>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.ReasonDesc = !string.IsNullOrWhiteSpace(requestModel.ReasonDesc) ? requestModel.ReasonDesc : entity.ReasonDesc;
                entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AmReasonForPending>().Update(entity);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        private bool CheckDataIsUse(int id)
        {
            if (_unitOfWork.ElogbookRepository<AmWorkPending>().GetQuery(x => x.AmReasonForPendingID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }
    }
}
