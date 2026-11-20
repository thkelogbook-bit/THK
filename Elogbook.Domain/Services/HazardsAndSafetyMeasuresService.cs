using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Domain.Services
{
    public class HazardsAndSafetyMeasuresService : IHazardsAndSafetyMeasuresService
    {
        #region Private fields
        private readonly ElogbookUnitOfWork _unitOfWork;
        #endregion

        #region Contructor
        public HazardsAndSafetyMeasuresService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckDuplicate(int id, string contentText, int contentType, bool isCreate)
        {
            // Trường hợp thêm mới
            if (isCreate)
            {
                return _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>()
                    .GetQuery(x => x.ContentText.Equals(contentText, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted && x.ContentType == contentType).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>()
                    .GetQuery(x => x.ID != id && x.ContentText.Equals(contentText, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted && x.ContentType == contentType).Any();
            }
        }

        public bool Create(HazardsAndSafetyMeasuresAddEditModel requestModel)
        {
            var entity = new HazardsAndSafetyMeasures
            {
                ContentText = requestModel.ContentText,
                ContentType = requestModel.ContentType,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().Add(entity);
            return _unitOfWork.Complete() > 0;
        }

        public ResponseBaseModel Delete(int id, int userId)
        {
            var entity = _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "Mối nguy hoặc biện pháp an toàn"));
                }

                _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().Delete(entity, userId);
                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "mối nguy hoặc biện pháp an toàn"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "mối nguy hoặc biện pháp an toàn"));
                }
            }
            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "Mối nguy hoặc biện pháp an toàn"));
        }

        public List<ValueLabelCheckedModel> FindByContentType(int contentType)
        {
            var entities = _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().GetQuery(x => !x.IsDeleted);

            if (contentType > 0)
            {
                entities = entities.Where(x => x.ContentType == contentType);
            }

            var listData = entities.Select(e => new ValueLabelCheckedModel()
            {
                Value = e.ID,
                Label = e.ContentText
            }).ToList();

            return listData;
        }

        public ResponseBaseModel GetAlls(HazardsAndSafetyMeasuresSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().GetQuery(x => !x.IsDeleted);

            // Tên nguyên nhân
            if (!string.IsNullOrWhiteSpace(requestModel.ContentText))
            {
                entities = entities.Where(x => x.ContentText.Contains(requestModel.ContentText));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            // Loại
            if (requestModel.ContentType != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.ContentType == requestModel.ContentType);
            }

            var page = entities.Select(
                e => new HazardsAndSafetyMeasuresModel()
                {
                    ID = e.ID,
                    Active = e.Active,
                    TypeValue = e.ContentType,
                    ContentText = e.ContentText,
                })
                .OrderBy(t => t.TypeValue)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public HazardsAndSafetyMeasuresModel GetById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().GetQuery(x => x.ID == id).Select(
              e => new HazardsAndSafetyMeasuresModel()
              {
                  ID = e.ID,
                  Active = e.Active,
                  TypeValue = e.ContentType,
                  ContentText = e.ContentText
              }).FirstOrDefault();

            return entity;
        }

        public bool Update(HazardsAndSafetyMeasuresAddEditModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.ContentText = !string.IsNullOrWhiteSpace(requestModel.ContentText) ? requestModel.ContentText : entity.ContentText;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.ContentType = requestModel.ContentType > 0 ? requestModel.ContentType : entity.ContentType;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<HazardsAndSafetyMeasures>().Update(entity);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        private bool CheckDataIsUse(int id)
        {
            // Kiểm tra trong table Phiếu đăng ký công tác
            // Kiểm tra trong table Phiếu công tác

            return false;
        }

        #endregion
    }
}
