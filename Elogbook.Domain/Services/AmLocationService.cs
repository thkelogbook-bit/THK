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
using System.Net;
using System.Text;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AmLocationService : IAmLocationService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IPmisParameterTask _pmisParameterTask;

        #endregion

        #region Contructor

        public AmLocationService(ElogbookUnitOfWork unitOfWork, IPmisParameterTask pmisParameterTask)
        {
            _unitOfWork = unitOfWork;
            _pmisParameterTask = pmisParameterTask;
        }

        #endregion

        #region Public method

        public bool CheckDuplicateLocation(string text, bool isCode)
        {
            if (isCode)
            {
                return _unitOfWork.ElogbookRepository<AmLocation>()
                    .GetQuery(x => x.SiteCode.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<AmLocation>()
                    .GetQuery(x => x.SiteDesc.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
        }

        public bool CreateLocation(AmLocationAddEditModel requestModel)
        {
            string siteParent = null;
            if (requestModel.SiteParent.HasValue)
            {
                siteParent = _unitOfWork.ElogbookRepository<AmLocation>()
                    .GetQuery(x => x.ID == requestModel.SiteParent && !x.IsDeleted).Select(x => x.SiteCode).FirstOrDefault();
            }
            var entity = new AmLocation
            {
                SiteParent = siteParent,
                OrganizationID = requestModel.OrganizationID,
                SiteDesc = !string.IsNullOrWhiteSpace(requestModel.SiteDesc) ? requestModel.SiteDesc : null,
                SiteCode = !string.IsNullOrWhiteSpace(requestModel.SiteCode) ? requestModel.SiteCode : null,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<AmLocation>().Add(entity);

            return _unitOfWork.Complete() > 0;
        }

        public ResponseBaseModel DeleteLocation(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "Khu vực"));
                }

                _unitOfWork.ElogbookRepository<AmLocation>().Delete(entity, userID);

                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "khu vực"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "khu vực"));
                }
            }

            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "Khu vực"));
        }

        public AmLocationModel GetLocationById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmLocationModel()
              {
                  ID = e.ID,
                  Organization = new ValueLabelCheckedModel { Value = e.Organization.ID, Label = e.Organization.Name },
                  Active = e.Active,
                  SiteParentStr = e.SiteParent,
                  SiteDesc = e.SiteDesc,
                  SiteCode = e.SiteCode,
              }).FirstOrDefault();

            if (entity != null && !string.IsNullOrWhiteSpace(entity.SiteParentStr))
            {
                var parentData = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => x.SiteCode.Equals(entity.SiteParentStr) && !x.IsDeleted).FirstOrDefault();
                if (parentData != null)
                {
                    entity.SiteParent = new ComboboxDataModel { Value = parentData.ID, Label = parentData.SiteDesc, Code = parentData.SiteCode };
                }
            }

            return entity;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            var entityJob = _unitOfWork.ElogbookRepository<PmisJobHistory>()
                     .GetQuery(e => !e.IsDeleted).OrderByDescending(t => t.LAST_RUN_DATE)
                     .FirstOrDefault();

            DateTime lastrun = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (entityJob != null && entityJob.LAST_RUN_DATE.HasValue)
            {
                lastrun = (DateTime)(entityJob.LAST_RUN_DATE ?? entityJob.LAST_RUN_DATE);
            }

            PmisJobHistory jobHistory = new PmisJobHistory
            {
                LAST_RUN_DATE = DateTime.Now,
                Status = (int)JobHistoryStatus.IN_PROGRES,
                SyncType = (int)JobHistoryType.Auto
            };

            _unitOfWork.ElogbookRepository<PmisJobHistory>().Add(jobHistory);
            _unitOfWork.Complete();
            jobHistory.Object = "AmLocation";

            jobHistory = _unitOfWork.ElogbookRepository<PmisJobHistory>().GetById(jobHistory.ID);
            try
            {
                _pmisParameterTask.Extract(jobHistory.ID, lastrun, PmisSyncType.AmLocation);
                _pmisParameterTask.Load(jobHistory.ID, PmisSyncType.AmLocation);
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

        public ResponseBaseModel GetLocations(AmLocationSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => !x.IsDeleted);

            // Công ty
            if (requestModel.OrganizationID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.OrganizationID == requestModel.OrganizationID);
            }

            // Khu vực cha
            if (requestModel.SiteParent != CommonConstant.ALL_KEY)
            {
                var siteCode = _unitOfWork.ElogbookRepository<AmLocation>()
                    .GetQuery(x => x.ID == requestModel.SiteParent && !x.IsDeleted).Select(t => t.SiteCode).FirstOrDefault();
                entities = entities.Where(x => x.SiteParent.Equals(siteCode));
            }

            // Tên hoặc mã khu vực
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.SiteDesc.Contains(requestModel.SearchText) || x.SiteCode.Contains(requestModel.SearchText));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            var dataResult = entities.Select(
                e => new AmLocationModel()
                {
                    ID = e.ID,
                    Organization = new ValueLabelCheckedModel { Value = e.Organization.ID, Label = e.Organization.Name },
                    Active = e.Active,
                    SiteParentStr = e.SiteParent,
                    SiteDesc = e.SiteDesc,
                    SiteCode = e.SiteCode,
                    InsertedAt = e.InsertedAt
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            var rootData = dataResult.ToList();
            if (rootData.Count > 0)
            {
                foreach (var item in rootData)
                {
                    if (!string.IsNullOrWhiteSpace(item.SiteParentStr))
                    {
                        var parentData = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => x.SiteCode.Equals(item.SiteParentStr) && !x.IsDeleted).FirstOrDefault();
                        if (parentData != null)
                        {
                            item.SiteParent = new ComboboxDataModel { Value = parentData.ID, Label = parentData.SiteDesc, Code = parentData.SiteCode };

                            item.SiteDesc = SetTitleParent(rootData, item.SiteParentStr) + item.SiteDesc;
                        }
                    }
                }

                return HttpResponse.OkList(rootData, requestModel.PageNumber, requestModel.PageSize, dataResult.TotalRecords, dataResult.TotalPages);
            }

            return HttpResponse.OkList(new List<AmLocationModel>(), requestModel.PageNumber, requestModel.PageSize);
        }

        public ResponseModel<bool> UpdateLocation(AmLocationAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            if (requestModel.ID == requestModel.SiteParent)
            {
                responseModel.Data = false;
                responseModel.ErrorMessages.Add("Lỗi - Khu vực cha đang chọn không khả dụng");
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                return responseModel;
            }
            var entity = _unitOfWork.ElogbookRepository<AmLocation>().GetById(requestModel.ID);
            if (entity != null)
            {
                string siteParent = null;
                if (requestModel.SiteParent.HasValue)
                {
                    var siteRefer = _unitOfWork.ElogbookRepository<AmLocation>()
                        .GetQuery(x => x.ID == requestModel.SiteParent && !x.IsDeleted).FirstOrDefault();
                    if (siteRefer != null )
                    {
                        siteParent = siteRefer.SiteCode;
                        if (entity.SiteCode.Equals(siteRefer.SiteParent))
                        {
                            responseModel.Data = false;
                            responseModel.ErrorMessages.Add("Lỗi - Khu vực cha đang chọn không khả dụng");
                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                            return responseModel;
                        }
                    }
                }

                entity.SiteParent = siteParent;
                entity.OrganizationID = requestModel.OrganizationID > 0 ? requestModel.OrganizationID : entity.OrganizationID;
                entity.SiteDesc = !string.IsNullOrWhiteSpace(requestModel.SiteDesc) ? requestModel.SiteDesc : entity.SiteDesc;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AmLocation>().Update(entity);
                if(_unitOfWork.Complete() > 0)
                {
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                responseModel.Data = false;
                responseModel.ErrorMessages.Add("Cập nhật khu vực thất bại");
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                return responseModel;
            }

            responseModel.Data = false;
            responseModel.ErrorMessages.Add("Đối tượng cập nhật không tồn tại");
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            return responseModel;
        }

        public List<ComboboxDataModel> GetLocationByCompanyID(int companyID, bool isParent)
        {
            var locations = new List<ComboboxDataModel>
                            {
                                new ComboboxDataModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };

            var entities = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => !x.IsDeleted);


            if (companyID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.OrganizationID == companyID);
            }

            if(isParent)
            {
                var amLocations = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => x.SiteParent != null && !x.IsDeleted);

                if (companyID != CommonConstant.ALL_KEY)
                {
                    amLocations = amLocations.Where(x => x.OrganizationID == companyID);
                }
                var lstParent = amLocations.Select(x => x.SiteParent).Distinct().ToList();

                entities = entities.Where(x => lstParent.Contains(x.SiteCode));
            }
            else
            {
                entities = entities.Where(x => x.Active == (int)AmEnumUsageStatus.Apply);
            }

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.SiteDesc,
                    Code = e.SiteCode
                }).ToList();

            if (listData.Count > 0)
                locations.AddRange(listData);

            return locations;
        }

        public List<ComboboxDataModel> GetParent(int companyID)
        {
            var locations = new List<ComboboxDataModel>();

            var entities = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

            if (companyID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.OrganizationID == companyID);
            }

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.SiteDesc,
                    Code = e.SiteCode
                }).ToList();

            if (listData.Count > 0)
            {
                locations.AddRange(listData);
            }

            return locations;
        }
        #endregion

        #region Private Method
        private void GetChildByID(int amLocationID, ref List<int> IDs)
        {
            var locationCurrent = _unitOfWork.ElogbookRepository<AmLocation>().GetById(amLocationID);
            var listID = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => !x.IsDeleted && x.SiteParent == locationCurrent.SiteCode).Select(x => x.ID);
            if (listID.Any())
            {
                IDs.AddRange(listID);
                foreach (var item in listID)
                {
                    GetChildByID(item, ref IDs);
                }
            }
        }

        private string SetTitleParent(List<AmLocationModel> rootData, string childSite)
        {
            var amLocationEntity = rootData.Find(x => x.SiteCode.Equals(childSite));

            if (amLocationEntity != null)
            {
                string parentSite = amLocationEntity.SiteParentStr;

                if (string.IsNullOrWhiteSpace(parentSite))
                {
                    return "> ";
                }

                StringBuilder strTxt = new StringBuilder();

                while (!string.IsNullOrWhiteSpace(parentSite))
                {
                    strTxt.Append("> ");
                    childSite = parentSite;
                    parentSite = rootData.Where(x => x.SiteCode == childSite).Select(x => x.SiteParentStr).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(parentSite))
                    {
                        strTxt.Append("> ");
                        break;
                    }
                }

                return strTxt.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private bool CheckDataIsUse(int id)
        {
            if (_unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.AmLocationID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            if (_unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().GetQuery(x => x.AmLocationID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            if (_unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.AmLocationId == id && !x.IsDeleted).Any())
            {
                return true;
            }

            if (_unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.AmLocationID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
