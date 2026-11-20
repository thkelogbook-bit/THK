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
    public class AmDeviceTypeService : IAmDeviceTypeService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IPmisParameterTask _pmisParameterTask;

        public AmDeviceTypeService(ElogbookUnitOfWork unitOfWork, IPmisParameterTask pmisParameterTask)
        {
            _unitOfWork = unitOfWork;
            _pmisParameterTask = pmisParameterTask;
        }

        public bool CheckDuplicateDeviceType(string text, bool isCode)
        {
            if (isCode)
            {
                return _unitOfWork.ElogbookRepository<AmDeviceType>()
                    .GetQuery(x => x.TypeCode.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<AmDeviceType>()
                    .GetQuery(x => x.TypeDesc.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
        }

        public bool CreateDeviceType(AmDeviceTypeAddEditModel requestModel)
        {
            var entity = new AmDeviceType
            {
                TypeDesc = requestModel.TypeDesc,
                TypeCode = requestModel.TypeCode,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<AmDeviceType>().Add(entity);
            return _unitOfWork.Complete() > 0;
        }

        public ResponseBaseModel DeleteDeviceType(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDeviceType>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "Loại thiết bị"));
                }

                _unitOfWork.ElogbookRepository<AmDeviceType>().Delete(entity, userID);

                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "loại thiết bị"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "loại thiết bị"));
                }
            }
            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "Loại thiết bị")); 
        }

        public AmDeviceTypeModel GetDeviceTypeById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDeviceType>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmDeviceTypeModel()
              {
                  ID = e.ID,
                  Active = e.Active,
                  TypeDesc = e.TypeDesc,
                  TypeCode = e.TypeCode,
              }).FirstOrDefault();

            return entity;
        }

        public ResponseBaseModel GetDeviceTypes(AmDeviceTypeSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmDeviceType>().GetQuery(x => !x.IsDeleted);

            // Tên hoặc Mã loại thiết bị
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.TypeDesc.Contains(requestModel.SearchText) || x.TypeCode.Contains(requestModel.SearchText));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            var page = entities.Select(
                e => new AmDeviceTypeModel()
                {
                    ID = e.ID,
                    Active = e.Active,
                    TypeDesc = e.TypeDesc,
                    TypeCode = e.TypeCode,
                    InsertedAt = e.InsertedAt
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool UpdateDeviceType(AmDeviceTypeAddEditModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDeviceType>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.TypeDesc = !string.IsNullOrWhiteSpace(requestModel.TypeDesc) ? requestModel.TypeDesc : entity.TypeDesc;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AmDeviceType>().Update(entity);
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

            jobHistory.Object = "DeviceType";
            try
            {
                _pmisParameterTask.Extract(jobHistory.ID, lastrun, PmisSyncType.AmDeviceType);
                _pmisParameterTask.Load(jobHistory.ID, PmisSyncType.AmDeviceType);
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
            if (_unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.AmDeviceTypeID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }

        public List<ComboboxDataModel> GetAllDeviceType()
        {
            var results = new List<ComboboxDataModel>
                            {
                                new ComboboxDataModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };

            var entities = _unitOfWork.ElogbookRepository<AmDeviceType>().GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.TypeDesc,
                    Code = e.TypeCode
                }).ToList();

            if (listData.Count > 0)
                results.AddRange(listData);

            return results;
        }
    }
}
