using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseCommon;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class AmDeviceCategoryService : IAmDeviceCategoryService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IPmisParameterTask _pmisParameterTask;

        public AmDeviceCategoryService(ElogbookUnitOfWork unitOfWork, IPmisParameterTask pmisParameterTask)
        {
            _unitOfWork = unitOfWork;
            _pmisParameterTask = pmisParameterTask;
        }

        /// <summary>
        /// Kiểm tra trùng dữ liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        public bool CheckDuplicateData(AddEditAmDeviceCategoryModel requestModel, bool isAddMode)
        {
            // Trường hợp thêm mới
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => x.CategoryName.Equals(requestModel.CategoryName, StringComparison.OrdinalIgnoreCase)
                                                                                    && x.CategoryCode.Equals(requestModel.CategoryCode, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => x.ID != requestModel.ID
                                                                                    && x.CategoryName.Equals(requestModel.CategoryName, StringComparison.OrdinalIgnoreCase)
                                                                                    && x.CategoryCode.Equals(requestModel.CategoryCode, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        /// <summary>
        /// Thêm mới phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public AmDeviceCategoryModel Add(AddEditAmDeviceCategoryModel requestModel)
        {
            var entity = new AmDeviceCategory
            {
                OrgID = requestModel.OrgID,
                CategoryParent = requestModel.CategoryParent,
                CategoryName = requestModel.CategoryName,
                CategoryCode = requestModel.CategoryCode,
                Active = requestModel.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<AmDeviceCategory>().Add(entity);
            _unitOfWork.Complete();

            return GetById(entity.ID);
        }

        /// <summary>
        /// Lấy data theo mã số
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public AmDeviceCategoryModel GetById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetById(id);
            if (entity != null)
            {
                var model = new AmDeviceCategoryModel()
                {
                    ID = entity.ID,
                    CategoryParent = entity.CategoryParent,
                    CategoryName = entity.CategoryName,
                    CategoryCode = entity.CategoryCode,
                    Active = entity.Active
                };

                if (entity.Organization != null)
                {
                    model.Organization = new ComboboxDataModel { Value = entity.Organization.ID, Label = entity.Organization.Name };
                }
                return model;
            }

            return null;
        }

        /// <summary>
        /// Trả về tất cả phân loại thiết bị theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel Find(SearchAmDeviceCategoryModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => !x.IsDeleted);

            //Nhà máy
            if (requestModel.OrgID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.OrgID == requestModel.OrgID);
            }

            //Phân loại thiết bị cha
            if (requestModel.ParentId != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.CategoryParent == requestModel.ParentId);
            }

            //Trạng thái sử dụng
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            //Mã / tên
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.CategoryCode.Contains(requestModel.SearchText) || x.CategoryName.Contains(requestModel.SearchText));
            }

            // Return and paging data
            IPagedList<AmDeviceCategoryModel> pagelist = entities.Select(
               e => new AmDeviceCategoryModel()
               {
                   ID = e.ID,
                   OrgID = e.OrgID,
                   Organization = new ComboboxDataModel
                   {
                       Value = e.Organization != null ? e.Organization.ID : 0,
                       Label = e.Organization != null ? e.Organization.Name : string.Empty,
                   },
                   CategoryParent = e.CategoryParent,
                   CategoryCode = e.CategoryCode,
                   CategoryName = e.CategoryName,
                   Active = e.Active,
                   InsertedAt = e.InsertedAt,
                   CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                   UpdatedAt = e.UpdatedAt,
                   LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName }
               }).OrderByDescending(x => x.InsertedAt)
               .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            if (pagelist != null && pagelist.TotalRecords > 0)
            {
                foreach (var item in pagelist)
                {
                    if (item.CategoryParent.GetValueOrDefault() > 0)
                    {
                        var parentData = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x=> x.ID == item.CategoryParent).FirstOrDefault();
                        if (parentData != null)
                        {
                            item.DeviceCategoryParent = new ComboboxDataModel { Value = parentData.ID, Label = parentData.CategoryName, Code = parentData.CategoryCode };
                        }
                        var arrow = GetNameWithParentID(item.CategoryParent.Value);
                        item.CategoryName = (!string.IsNullOrEmpty(arrow) ? arrow + " " : "") + item.CategoryName;
                    }
                }

                pagelist.GetEnumerator().Reset();

                return HttpResponse.OkList(pagelist, requestModel.PageNumber, requestModel.PageSize, pagelist.TotalRecords, pagelist.TotalPages);
            }

            return HttpResponse.OkList(new List<AmDeviceCategoryModel>(), requestModel.PageNumber, requestModel.PageSize);
        }

        /// <summary>
        /// Trả về tất cả phân loại thiết bị cha theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindParent(SearchParentAmDeviceCategoryModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => !x.IsDeleted);

            //Nhà máy
            if (requestModel.OrgID.GetValueOrDefault() > 0)
            {
                entities = entities.Where(x => x.OrgID == requestModel.OrgID);
            }

            //Mã / tên
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.CategoryName.Contains(requestModel.SearchText));
            }

            if (requestModel.AmDeviceCategoryID.HasValue)
            {
                List<int> IDs = new List<int>();
                IDs.Add(requestModel.AmDeviceCategoryID.Value);
                GetChildByID(requestModel.AmDeviceCategoryID.Value,ref IDs);

                if (IDs.Any())
                {
                    entities = entities.Where(x => !IDs.Contains(x.ID));
                }
            }

            // Return and paging data
            var page = entities.Select(
                e => new ValueLabelModel()
                {
                    Value = e.ID,
                    Label = e.CategoryCode + " - " + e.CategoryName
                }).ToList();

            return HttpResponse.OkList(page);
        }

        private void GetChildByID(int ID,ref List<int> IDs)
        {
            var listID = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => !x.IsDeleted && x.CategoryParent == ID).Select(x=>x.ID);
            if (listID.Any())
            {
                IDs.AddRange(listID);
                foreach(var item in listID)
                {
                    GetChildByID(item,ref IDs);
                }
            }
        }

        private string GetNameWithParentID(int? parent)
        {
            if (parent.HasValue && parent.Value > 0)
            {
                var entity = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => x.ID == parent && !x.IsDeleted).FirstOrDefault();
                if (entity != null)
                {
                    if (entity.CategoryParent.GetValueOrDefault() > 0)
                    {
                        return ">" + GetNameWithParentID(entity.CategoryParent.Value);
                    }
                    else
                    {
                        return ">";
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Cập nhập phân loại thiết bị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<AmDeviceCategoryModel> Update(AddEditAmDeviceCategoryModel requestModel)
        {
            var responseModel = new ResponseModel<AmDeviceCategoryModel>();

            if (requestModel.ID == requestModel.CategoryParent)
            {
                responseModel.ErrorMessages.Add("Lỗi - Phân loại thiết bị cha đang chọn không khả dụng");
                return responseModel;
            }

            var entity = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetById(requestModel.ID);
            if (entity != null)
            {
                if (requestModel.CategoryParent.HasValue)
                {
                    var dCateRef = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetById((int)requestModel.CategoryParent);

                    if (dCateRef != null && entity.ID == dCateRef.CategoryParent)
                    {
                        responseModel.ErrorMessages.Add("Lỗi - Phân loại thiết bị cha đang chọn không khả dụng");
                        return responseModel;
                    }
                }

                entity.OrgID = requestModel.OrgID;
                entity.CategoryParent = requestModel.CategoryParent;
                entity.CategoryName = requestModel.CategoryName;
                entity.Active = requestModel.Active;
                entity.CreatedUserID = requestModel.CurrentUserID;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<AmDeviceCategory>().Update(entity);
                if (_unitOfWork.Complete() > 0)
                {
                    responseModel.Data = GetById(entity.ID);
                    return responseModel;
                }
                responseModel.ErrorMessages.Add("Cập nhật phân loại thiết bị thất bại");
                return responseModel;
            }

            responseModel.ErrorMessages.Add("Đối tượng cập nhật không tồn tại");
            return responseModel;
        }

        /// <summary>
        /// Xóa phân loại thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public bool Delete(int id, int currentUserId)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<AmDeviceCategory>().Delete(entity, currentUserId);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sync pmis
        /// </summary>
        /// <returns></returns>
        public ResponseBaseModel SyncPmis()
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
                SyncType = (int)JobHistoryType.Auto
            };

            _unitOfWork.ElogbookRepository<PmisJobHistory>().Add(jobHistory);
            _unitOfWork.Complete();

            jobHistory = _unitOfWork.ElogbookRepository<PmisJobHistory>().GetById(jobHistory.ID);

            jobHistory.Object = "AmDeviceCategory";
            try
            {
                _pmisParameterTask.Extract(jobHistory.ID, lastrun, PmisSyncType.AmDeviceCategory);
                _pmisParameterTask.Load(jobHistory.ID, PmisSyncType.AmDeviceCategory);
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

        public bool GetOrgInf(int orgID)
        {
            return _unitOfWork.ElogbookRepository<Organization>().GetQuery(e =>e.ID == orgID && !e.IsDeleted).Any();
        }

        public bool CheckExistsParent(int categoryParent)
        {
            return _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(e => e.ID == categoryParent && !e.IsDeleted).Any();
        }

        public List<ComboboxDataModel> GetDeviceCategoriesByCompany(int companyID, bool isParent)
        {
            var deviceCategories = new List<ComboboxDataModel>();

            var entities = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => !x.IsDeleted);

            if (companyID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.OrgID == companyID);
            }

            if (isParent)
            {
                var amDevicesCategory = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => x.CategoryParent != null && !x.IsDeleted);

                if (companyID != CommonConstant.ALL_KEY)
                {
                    amDevicesCategory = amDevicesCategory.Where(x => x.OrgID == companyID);
                }
                var lstParent = amDevicesCategory.Select(x => x.CategoryParent).Distinct().ToList();

                entities = entities.Where(x => lstParent.Contains(x.ID));
            }
            else
            {
                entities = entities.Where(x => x.Active == (int)AmEnumUsageStatus.Apply);
            }

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.CategoryName,
                    Code = e.CategoryCode
                }).ToList();

            if (listData.Count > 0)
            {
                deviceCategories.AddRange(listData);
            }

            return deviceCategories;
        }
    }
}
