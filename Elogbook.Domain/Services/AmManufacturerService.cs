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
using System.Data;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AmManufacturerService : IAmManufacturerService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IPmisParameterTask _pmisParameterTask;

        public AmManufacturerService(ElogbookUnitOfWork unitOfWork, IPmisParameterTask pmisParameterTask)
        {
            _unitOfWork = unitOfWork;
            _pmisParameterTask = pmisParameterTask;
        }

        public bool CheckDuplicateManufacturer(string text, bool isCode)
        {
            if (isCode)
            {
                return _unitOfWork.ElogbookRepository<AmManufacturer>()
                    .GetQuery(x => x.ManufacturerCode.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<AmManufacturer>()
                    .GetQuery(x => x.ManufacturerName.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
        }

        public bool CreateManufacturer(AmManufacturerAddEditModel requestModel)
        {
            var entity = new AmManufacturer
            {
                ManufacturerName = requestModel.ManufacturerName,
                ManufacturerCode = requestModel.ManufacturerCode,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<AmManufacturer>().Add(entity);
            return _unitOfWork.Complete() > 0;
        }

        public ResponseBaseModel DeleteManufacturer(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmManufacturer>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "nhà sản xuất"));
                }

                _unitOfWork.ElogbookRepository<AmManufacturer>().Delete(entity, userID);

                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "nhà sản xuất"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "nhà sản xuất"));
                }
            }
            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "nhà sản xuất")); 
        }

        public AmManufacturerModel GetManufacturerById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmManufacturer>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmManufacturerModel()
              {
                  ID = e.ID,
                  ManufacturerName = e.ManufacturerName,
                  ManufacturerCode = e.ManufacturerCode,
                  Active = e.Active,
                  InsertedAt = e.InsertedAt,
                  UpdatedAt = e.UpdatedAt,
                  PMISId = e.PMISId,
                  CreatedUser = e.CreatedUserID != null ? new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.UserName } : null,
                  LastUpdatedUser = e.CreatedUserID != null ? new ValueLabelCheckedModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.UserName } : null
              }).FirstOrDefault();

            return entity;
        }

        public ResponseBaseModel GetManufacturers(AmManufacturerSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmManufacturer>().GetQuery(x => !x.IsDeleted);

            // Tên hoặc Mã nhà sản xuất
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.ManufacturerName.Contains(requestModel.SearchText) || x.ManufacturerCode.Contains(requestModel.SearchText));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            var page = entities.Select(
                e => new AmManufacturerModel()
                {
                    ID = e.ID,
                    ManufacturerName = e.ManufacturerName,
                    ManufacturerCode = e.ManufacturerCode,
                    Active = e.Active,
                    InsertedAt = e.InsertedAt,
                    UpdatedAt = e.UpdatedAt,
                    PMISId = e.PMISId,
                    CreatedUser = e.CreatedUserID != null ? new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.UserName } : null,
                    LastUpdatedUser = e.CreatedUserID != null ? new ValueLabelCheckedModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.UserName } : null
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool UpdateManufacturer(AmManufacturerAddEditModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmManufacturer>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.ManufacturerName = !string.IsNullOrWhiteSpace(requestModel.ManufacturerName) ? requestModel.ManufacturerName : entity.ManufacturerName;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AmManufacturer>().Update(entity);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            var entityJob = _unitOfWork.ElogbookRepository<PmisJobHistory>()
                     .GetQuery(e => !e.IsDeleted).OrderByDescending(t => t.LAST_RUN_DATE)
                     .FirstOrDefault();

            DateTime lastrun = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

            if (entityJob != null && entityJob.LAST_RUN_DATE != null)
            {
                lastrun = (DateTime)(entityJob.LAST_RUN_DATE ?? entityJob.LAST_RUN_DATE);
            }

            PmisJobHistory jobHistory = new PmisJobHistory
            {
                LAST_RUN_DATE = DateTime.Now,
                Status = (int)JobHistoryStatus.IN_PROGRES,
                SyncType = (int)JobHistoryType.Manual
            };

            _unitOfWork.ElogbookRepository<PmisJobHistory>().Add(jobHistory);
            _unitOfWork.Complete();

            jobHistory = _unitOfWork.ElogbookRepository<PmisJobHistory>().GetById(jobHistory.ID);

            jobHistory.Object = "AmManufacturer";
            try
            {
                _pmisParameterTask.Extract(jobHistory.ID, lastrun, PmisSyncType.AmManufacturer);
                _pmisParameterTask.Load(jobHistory.ID, PmisSyncType.AmManufacturer);
            }
            catch (Exception e)
            {
                jobHistory.Status = (int)JobHistoryStatus.FAIL;
                jobHistory.Logs = e.ToString();
                _unitOfWork.ElogbookRepository<PmisJobHistory>().Update(jobHistory);
                _unitOfWork.Complete();
            }

            if (jobHistory.Status != (int)JobHistoryStatus.FAIL)
            {
                jobHistory.Status = (int)JobHistoryStatus.SUCCESS;
                _unitOfWork.ElogbookRepository<PmisJobHistory>().Update(jobHistory);
                _unitOfWork.Complete();
            }
            return HttpResponse.OK(CommonConstant.MSG_SYNC_DATA_PMIS);
        }

        private bool CheckDataIsUse(int id)
        {
            if (_unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.AmManufacturerID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }

        public List<ComboboxDataModel> GetAllManufacturer()
        {
            var results = new List<ComboboxDataModel>
                            {
                                new ComboboxDataModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };

            var entities = _unitOfWork.ElogbookRepository<AmManufacturer>().GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.ManufacturerName,
                    Code = e.ManufacturerCode
                }).ToList();

            if (listData.Count > 0)
                results.AddRange(listData);

            return results;
        }
    }
}
