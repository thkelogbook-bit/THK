using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.AmDevice;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AmDeviceService : IAmDeviceService
    {
        #region Private fields
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly IPmisParameterTask _pmisParameterTask;
        #endregion

        #region Contructor
        public AmDeviceService(ElogbookUnitOfWork unitOfWork, ICommonService commonService, IPmisParameterTask pmisParameterTask)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _pmisParameterTask = pmisParameterTask;
        }
        #endregion

        #region Public method
        public bool CheckDuplicateDevice(string text, bool isCode)
        {
            if (isCode)
            {
                return _unitOfWork.ElogbookRepository<AmDevice>()
                    .GetQuery(x => x.AssetCode.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<AmDevice>()
                    .GetQuery(x => x.AssetDesc.Equals(text, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }
        }

        public ResponseModel<bool> CreateDevice(AmDeviceAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            string assetParent = null;
            if (requestModel.AssetParent.HasValue)
            {
                assetParent = _unitOfWork.ElogbookRepository<AmDevice>()
                    .GetQuery(x => x.ID == requestModel.AssetParent && !x.IsDeleted).Select(x => x.AssetCode).FirstOrDefault();
            }
            var entity = new AmDevice
            {
                AmLocationID = requestModel.SiteID,
                AssetParent = assetParent,
                AssetDesc = requestModel.AssetDesc,
                AssetCode = requestModel.AssetCode,
                AmDeviceStatusID = requestModel.DeviceStatusID,
                Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                AmDeviceTypeID = requestModel.TypeDescID.HasValue ? requestModel.TypeDescID : null,
                AmManufacturerID = requestModel.ManufacturerID.HasValue ? requestModel.ManufacturerID : null,
                AmSupplierID = requestModel.SupplierID.HasValue ? requestModel.SupplierID : null,
                AmCountryManufactureID = requestModel.CountryManufactureID.HasValue ? requestModel.CountryManufactureID : null,
                OrganizationID = requestModel.OrganizationID.HasValue ? requestModel.OrganizationID : null,
                SerialNum = !string.IsNullOrWhiteSpace(requestModel.SerialNum) ? requestModel.SerialNum : null,
                AssetType = !string.IsNullOrWhiteSpace(requestModel.AssetType) ? requestModel.AssetType : null,
                Meter = !string.IsNullOrWhiteSpace(requestModel.Meter) ? requestModel.Meter : null,
                Kss = !string.IsNullOrWhiteSpace(requestModel.Kss) ? requestModel.Kss : null,
                Khvh = !string.IsNullOrWhiteSpace(requestModel.Khvh) ? requestModel.Khvh : null,
                Note = !string.IsNullOrWhiteSpace(requestModel.Note) ? requestModel.Note : null,
                Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                Drawing = !string.IsNullOrWhiteSpace(requestModel.Drawing) ? requestModel.Drawing : null,
                Images = !string.IsNullOrWhiteSpace(requestModel.Images) ? requestModel.Images : null,
                InstallDate = requestModel.InstallDate.HasValue ? requestModel.InstallDate : null,
                OperatingDate = requestModel.OperatingDate.HasValue ? requestModel.OperatingDate : null,
                AmDeviceCategoryID = requestModel.CategoryID.HasValue ? requestModel.CategoryID : null,
                GoogleMapUrl = requestModel.GoogleMapUrl,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<AmDevice>().Add(entity);

            responseModel.Data = _unitOfWork.Complete() > 0;

            return responseModel;
        }

        public ResponseBaseModel DeleteDevice(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                if (CheckDataIsUse(id))
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_WARNING_DELETE, "Thiết bị"));
                }

                _unitOfWork.ElogbookRepository<AmDevice>().Delete(entity, userID);

                if (_unitOfWork.Complete() > 0)
                {
                    return HttpResponse.OK(string.Format(CommonConstant.MSG_DELETE_SUCCESS, "thiết bị"));
                }
                else
                {
                    return HttpResponse.Error(string.Format(CommonConstant.MSG_DELETE_FAILD, "thiết bị"));
                }
            }
            return HttpResponse.Error(string.Format(CommonConstant.MSG_DOES_NOT_EXISTS, "Thiết bị"));
        }

        public AmDeviceModel GetDeviceById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmDeviceModel()
              {
                  ID = e.ID,
                  SiteDesc = new ComboboxDataModel { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc, Code = e.AmLocation.SiteCode },
                  AssetParentStr = e.AssetParent,
                  AssetDesc = e.AssetDesc,
                  AssetCode = e.AssetCode,
                  DeviceStatus = new ValueLabelCheckedModel { Value = e.AmDeviceStatus.ID, Label = e.AmDeviceStatus.StatusDesc },
                  Active = e.Active,
                  TypeDesc = e.AmDeviceTypeID != null ? new ComboboxDataModel { Value = e.AmDeviceType.ID, Label = e.AmDeviceType.TypeDesc, Code = e.AmDeviceType.TypeCode } : null,
                  Manufacturer = e.AmManufacturerID != null ? new ComboboxDataModel { Value = e.AmManufacturer.ID, Label = e.AmManufacturer.ManufacturerName, Code = e.AmManufacturer.ManufacturerCode } : null,
                  Supplier = e.AmSupplierID != null ? new ComboboxDataModel { Value = e.AmSupplier.ID, Label = e.AmSupplier.SupplierName, Code = e.AmSupplier.SupplierCode } : null,
                  CountryManufacture = e.AmCountryManufactureID != null ? new ComboboxDataModel { Value = e.AmCountryManufacture.ID, Label = e.AmCountryManufacture.CountryName, Code = e.AmCountryManufacture.CountryCode } : null,
                  Organization = e.OrganizationID != null ? new ValueLabelCheckedModel { Value = e.Organization.ID, Label = e.Organization.Name } : null,
                  DeviceCategory = e.AmDeviceCategoryID != null ? new ComboboxDataModel { Value = e.AmDeviceCategory.ID, Label = e.AmDeviceCategory.CategoryName, Code = e.AmDeviceCategory.CategoryCode } : null,
                  SerialNum = e.SerialNum,
                  AssetType = e.AssetType,
                  Meter = e.Meter,
                  Kss = e.Kss,
                  Khvh = e.Khvh,
                  Note = e.Note,
                  Description = e.Description,
                  Drawing = e.Drawing,
                  ListImageIds = e.Images,
                  InstallDate = e.InstallDate,
                  OperatingDate = e.OperatingDate,
                  GoogleMapUrl = e.GoogleMapUrl
              }).FirstOrDefault();

            // Hình ảnh file đính kèm
            if (entity != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.ListImageIds))
                {
                    var attackmentIds = entity.ListImageIds.Split(',').Select(int.Parse).ToList();

                    var dataImages = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && attackmentIds.Contains(x.ID)).ToList();

                    if (dataImages.Count > 0)
                    {
                        var images = new List<AttachmentModel>();
                        foreach (var item in dataImages)
                        {
                            var newImageInfo = new AttachmentModel()
                            {
                                ID = item.ID,
                                FileName = item.FileName,
                                FilePath = _commonService.GetPathFile(item.FilePath),
                                PathThumbnail = _commonService.GetPathFile(item.PathThumbnail),
                                ScreenName = item.ScreenName,
                                FileType = item.FileType,
                            };
                            images.Add(newImageInfo);
                        }
                        entity.Images = images;
                    }
                }


                // Thông tin thiết bị cha
                if (!string.IsNullOrWhiteSpace(entity.AssetParentStr))
                {
                    var parentData = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.AssetCode.Equals(entity.AssetParentStr) && !x.IsDeleted).FirstOrDefault();
                    if (parentData != null)
                    {
                        entity.AssetParent = new ComboboxDataModel { Value = parentData.ID, Label = parentData.AssetDesc, Code = parentData.AssetCode };
                    }
                }
            }

            return entity;
        }

        public ResponseBaseModel GetDevices(AmDeviceSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => !x.IsDeleted);

            // Tên thiết bị or Mã thiết bị
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.AssetDesc.Contains(requestModel.SearchText) || x.AssetCode.Contains(requestModel.SearchText));
            }

            // Thiết bị cha
            if (requestModel.AssetParent != CommonConstant.ALL_KEY)
            {
                if (requestModel.AssetParent != CommonConstant.ZERO)
                {
                    var assetCode = _unitOfWork.ElogbookRepository<AmDevice>()
                    .GetQuery(x => x.ID == requestModel.AssetParent && !x.IsDeleted).Select(t => t.AssetCode).FirstOrDefault();
                    entities = entities.Where(x => x.AssetParent.Equals(assetCode));
                }
                else
                {
                    entities = entities.Where(x => x.AssetParent == null);
                }
            }

            // Khu vực
            var locations = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => !x.IsDeleted && x.OrganizationID == requestModel.CompanyID).ToList();

            var locationIds = locations.Select(x => x.ID).ToList();

            if (requestModel.SiteID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmLocationID == requestModel.SiteID);
            }
            else
            {
                entities = entities.Where(x => locationIds.Contains(x.AmLocationID.Value));
            }

            // Loại thiết bị
            if (requestModel.TypeDescID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceTypeID == requestModel.TypeDescID);
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            // Tình trạng thiết bị
            if (requestModel.DeviceStatusID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceStatusID == requestModel.DeviceStatusID);
            }

            var dataResult = entities.Select(
                e => new AmDeviceModel()
                {
                    ID = e.ID,
                    SiteDesc = new ComboboxDataModel { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc, Code = e.AmLocation.SiteCode },
                    AssetParentStr = e.AssetParent,
                    AssetDesc = e.AssetDesc,
                    AssetCode = e.AssetCode,
                    DeviceStatus = new ValueLabelCheckedModel { Value = e.AmDeviceStatus.ID, Label = e.AmDeviceStatus.StatusDesc },
                    Active = e.Active,
                    TypeDesc = e.AmDeviceTypeID != null ? new ComboboxDataModel { Value = e.AmDeviceType.ID, Label = e.AmDeviceType.TypeDesc, Code = e.AmDeviceType.TypeCode } : null,
                    Manufacturer = e.AmManufacturerID != null ? new ComboboxDataModel { Value = e.AmManufacturer.ID, Label = e.AmManufacturer.ManufacturerName, Code = e.AmManufacturer.ManufacturerCode } : null,
                    Supplier = e.AmSupplierID != null ? new ComboboxDataModel { Value = e.AmSupplier.ID, Label = e.AmSupplier.SupplierName, Code = e.AmSupplier.SupplierCode } : null,
                    CountryManufacture = e.AmCountryManufactureID != null ? new ComboboxDataModel { Value = e.AmCountryManufacture.ID, Label = e.AmCountryManufacture.CountryName, Code = e.AmCountryManufacture.CountryCode } : null,
                    Organization = e.OrganizationID != null ? new ValueLabelCheckedModel { Value = e.Organization.ID, Label = e.Organization.Name } : null,
                    DeviceCategory = e.AmDeviceCategoryID != null ? new ComboboxDataModel { Value = e.AmDeviceCategory.ID, Label = e.AmDeviceCategory.CategoryName, Code = e.AmDeviceCategory.CategoryCode } : null,
                    SerialNum = e.SerialNum,
                    AssetType = e.AssetType,
                    Meter = e.Meter,
                    Kss = e.Kss,
                    Khvh = e.Khvh,
                    Note = e.Note,
                    Description = e.Description,
                    Drawing = e.Drawing,
                    ListImageIds = e.Images,
                    InstallDate = e.InstallDate,
                    OperatingDate = e.OperatingDate,
                    GoogleMapUrl = e.GoogleMapUrl,
                    InsertedAt = e.InsertedAt
                })
                .OrderByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            var rootData = dataResult.ToList();
            if (rootData.Count > 0)
            {
                foreach (var item in rootData)
                {
                    if (!string.IsNullOrWhiteSpace(item.AssetParentStr))
                    {
                        var parentData = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.AssetCode == item.AssetParentStr && !x.IsDeleted).FirstOrDefault();
                        if (parentData != null)
                        {
                            item.AssetParent = new ComboboxDataModel { Value = parentData.ID, Label = parentData.AssetDesc, Code = parentData.AssetCode };
                            item.AssetDesc = SetTitleParent(rootData, item.AssetParentStr) + item.AssetDesc;
                        }
                    }
                }

                return HttpResponse.OkList(rootData, requestModel.PageNumber, requestModel.PageSize, dataResult.TotalRecords, dataResult.TotalPages);
            }

            return HttpResponse.OkList(new List<AmDeviceModel>(), requestModel.PageNumber, requestModel.PageSize);
        }

        public ResponseModel<bool> UpdateDevice(AmDeviceAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            if (requestModel.ID == requestModel.AssetParent)
            {
                responseModel.Data = false;
                responseModel.ErrorMessages.Add("Lỗi - Thiết bị cha đang chọn không khả dụng");
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                return responseModel;
            }
            var entity = _unitOfWork.ElogbookRepository<AmDevice>().GetById(requestModel.ID);

            if (entity != null)
            {
                string asetParent = null;
                if (requestModel.AssetParent.HasValue)
                {
                    var assetRefer = _unitOfWork.ElogbookRepository<AmDevice>()
                        .GetQuery(x => x.ID == requestModel.AssetParent && !x.IsDeleted).FirstOrDefault();

                    if (assetRefer != null)
                    {
                        asetParent = assetRefer.AssetCode;
                        if (entity.AssetCode.Equals(assetRefer.AssetParent))
                        {
                            responseModel.Data = false;
                            responseModel.ErrorMessages.Add("Lỗi - Thiết bị cha đang chọn không khả dụng");
                            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                            return responseModel;
                        }
                    }
                }

                entity.AmLocationID = requestModel.SiteID > 0 ? requestModel.SiteID : entity.AmLocationID;
                entity.AssetParent = asetParent;
                entity.AssetDesc = !string.IsNullOrWhiteSpace(requestModel.AssetDesc) ? requestModel.AssetDesc : entity.AssetDesc;
                entity.AmDeviceStatusID = requestModel.DeviceStatusID > 0 ? requestModel.DeviceStatusID : entity.AmDeviceStatusID;
                entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                entity.AmDeviceTypeID = requestModel.TypeDescID.HasValue ? requestModel.TypeDescID : entity.AmDeviceTypeID;
                entity.AmManufacturerID = requestModel.ManufacturerID.HasValue ? requestModel.ManufacturerID : entity.AmManufacturerID;
                entity.AmSupplierID = requestModel.SupplierID.HasValue ? requestModel.SupplierID : entity.AmSupplierID;
                entity.AmCountryManufactureID = requestModel.CountryManufactureID.HasValue ? requestModel.CountryManufactureID : entity.AmCountryManufactureID;
                entity.OrganizationID = requestModel.OrganizationID.HasValue ? requestModel.OrganizationID : entity.OrganizationID;
                entity.SerialNum = !string.IsNullOrWhiteSpace(requestModel.SerialNum) ? requestModel.SerialNum : entity.SerialNum;
                entity.AssetType = !string.IsNullOrWhiteSpace(requestModel.AssetType) ? requestModel.AssetType : entity.AssetType;
                entity.Meter = !string.IsNullOrWhiteSpace(requestModel.Meter) ? requestModel.Meter : entity.Meter;
                entity.Kss = !string.IsNullOrWhiteSpace(requestModel.Kss) ? requestModel.Kss : entity.Kss;
                entity.Khvh = !string.IsNullOrWhiteSpace(requestModel.Khvh) ? requestModel.Khvh : entity.Khvh;
                entity.Note = !string.IsNullOrWhiteSpace(requestModel.Note) ? requestModel.Note : entity.Note;
                entity.Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : entity.Description;
                entity.Drawing = !string.IsNullOrWhiteSpace(requestModel.Drawing) ? requestModel.Drawing : entity.Drawing;
                entity.Images = !string.IsNullOrWhiteSpace(requestModel.Images) ? requestModel.Images : entity.Images;
                entity.InstallDate = requestModel.InstallDate.HasValue ? requestModel.InstallDate : entity.InstallDate;
                entity.OperatingDate = requestModel.OperatingDate.HasValue ? requestModel.OperatingDate : entity.OperatingDate;
                entity.AmDeviceCategoryID = requestModel.CategoryID.HasValue ? requestModel.CategoryID : entity.AmDeviceCategoryID;
                entity.GoogleMapUrl = !string.IsNullOrWhiteSpace(requestModel.GoogleMapUrl) ? requestModel.GoogleMapUrl : entity.GoogleMapUrl;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AmDevice>().Update(entity);

                if (_unitOfWork.Complete() > 0)
                {
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                responseModel.Data = false;
                responseModel.ErrorMessages.Add("Cập nhật thiết bị thất bại");
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                return responseModel;
            }
            responseModel.Data = false;
            responseModel.ErrorMessages.Add("Đối tượng cập nhật không tồn tại");
            responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
            return responseModel;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            var entityJob = _unitOfWork.ElogbookRepository<PmisJobHistory>()
                     .GetQuery(e => !e.IsDeleted && e.Object == "AmDevice").OrderByDescending(t => t.LAST_RUN_DATE)
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
            jobHistory.Object = "AmDevice";

            try
            {
                _pmisParameterTask.Extract(jobHistory.ID, lastrun, PmisSyncType.AmDevice);
                _pmisParameterTask.Load(jobHistory.ID, PmisSyncType.AmDevice);
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

        public List<ComboboxDataModel> GetDeviceByLocationID(int locationID)
        {
            var devices = new List<ComboboxDataModel>();

            var entities = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

            if (locationID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmLocationID == locationID);
            }

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.AssetDesc,
                    Code = e.AssetCode
                }).ToList();

            if (listData.Count > 0)
            {
                devices.AddRange(listData);
            }

            return devices;
        }

        public List<ComboboxDataModel> GetParent(int locationID, int deviceTypeID)
        {
            var devices = new List<ComboboxDataModel>
                            {
                                new ComboboxDataModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                                new ComboboxDataModel { Value = CommonConstant.ZERO, Label = "Không có thiết bị cha" },
                            };

            var entities = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && x.AssetParent != null && !x.IsDeleted);

            if (locationID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmLocationID == locationID);
            }

            if (deviceTypeID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceTypeID == deviceTypeID);
            }

            List<string> lstParent = entities.Select(x => x.AssetParent).Distinct().ToList();

            entities = entities.Where(x => lstParent.Contains(x.AssetCode));

            var listData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.AssetDesc,
                    Code = e.AssetCode
                }).ToList();

            if (listData.Count > 0)
            {
                devices.AddRange(listData);
            }

            return devices;
        }

        public ResponseBaseModel QuickSearchDevices(AmDeviceQuickSearchModel requestModel)
        {
            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => !x.IsDeleted);

            // Tên thiết bị
            if (!string.IsNullOrWhiteSpace(requestModel.DeviceName))
            {
                entities = entities.Where(x => x.AssetDesc.Contains(requestModel.DeviceName));
            }

            // Mã thiết bị
            if (!string.IsNullOrWhiteSpace(requestModel.DeviceCode))
            {
                entities = entities.Where(x => x.AssetCode.Contains(requestModel.DeviceCode));
            }

            // Khu vực
            var locations = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => !x.IsDeleted && x.OrganizationID == requestModel.CompanyID).ToList();

            var locationIds = locations.Select(x => x.ID).ToList();

            if (requestModel.SiteID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmLocationID == requestModel.SiteID);
            }
            else
            {
                entities = entities.Where(x => locationIds.Contains(x.AmLocationID.Value));
            }

            // Loại thiết bị
            if (requestModel.TypeDescID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceTypeID == requestModel.TypeDescID);
            }

            if(requestModel.IsParent)
            {
                var devices = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => !x.IsDeleted && x.AssetParent != null);

                if (requestModel.SiteID != CommonConstant.ALL_KEY)
                {
                    devices = devices.Where(x => x.AmLocationID == requestModel.SiteID);
                }
                else
                {
                    devices = devices.Where(x => locationIds.Contains(x.AmLocationID.Value));
                }

                var lstParent = devices.Select(x => x.AssetParent).Distinct().ToList();

                entities = entities.Where(x => lstParent.Contains(x.AssetCode));
            }
            else
            {
                entities = entities.Where(x => x.Active == (int)AmEnumUsageStatus.Apply);
            }

            var dataResult = entities.Select(
                e => new AmDeviceBasicInfoModel()
                {
                    ID = e.ID,
                    SiteDesc = new ValueLabelCheckedModel { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc },
                    AssetParent = e.AssetParent,
                    AssetDesc = e.AssetDesc,
                    AssetCode = e.AssetCode,
                    DeviceStatus = new ValueLabelCheckedModel { Value = e.AmDeviceStatus.ID, Label = e.AmDeviceStatus.StatusDesc },
                    TypeDesc = e.AmDeviceTypeID != null ? new ValueLabelCheckedModel { Value = e.AmDeviceType.ID, Label = e.AmDeviceType.TypeDesc } : null,
                    Manufacturer = e.AmManufacturerID != null ? new ValueLabelCheckedModel { Value = e.AmManufacturer.ID, Label = e.AmManufacturer.ManufacturerName } : null,
                    Supplier = e.AmSupplierID != null ? new ValueLabelCheckedModel { Value = e.AmSupplier.ID, Label = e.AmSupplier.SupplierName } : null,
                    CountryManufacture = e.AmCountryManufactureID != null ? new ValueLabelCheckedModel { Value = e.AmCountryManufacture.ID, Label = e.AmCountryManufacture.CountryName } : null,
                })
                .OrderBy(t => t.AssetDesc)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(dataResult);
        }

        public ResponseBaseModel GetDevicesHierarchy(AmDeviceSearchModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.AssetDesc.Contains(requestModel.SearchText) || x.AssetCode.Contains(requestModel.SearchText));
            }

            if (requestModel.AssetParent != CommonConstant.ALL_KEY)
            {
                if (requestModel.AssetParent != CommonConstant.ZERO)
                {
                    var assetCode = _unitOfWork.ElogbookRepository<AmDevice>()
                    .GetQuery(x => x.ID == requestModel.AssetParent && !x.IsDeleted).Select(t => t.AssetCode).FirstOrDefault();
                    entities = entities.Where(x => x.AssetParent.Equals(assetCode));
                }
                else
                {
                    entities = entities.Where(x => x.AssetParent == null);
                }
            }

            var locations = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => !x.IsDeleted && x.OrganizationID == requestModel.CompanyID).ToList();

            var locationIds = locations.Select(x => x.ID).ToList();

            if (requestModel.SiteID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmLocationID == requestModel.SiteID);
            }
            else
            {
                entities = entities.Where(x => locationIds.Contains(x.AmLocationID.Value));
            }

            if (requestModel.TypeDescID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceTypeID == requestModel.TypeDescID);
            }

            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            if (requestModel.DeviceStatusID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceStatusID == requestModel.DeviceStatusID);
            }

            var matchedDevices = entities.Select(
                e => new AmDeviceModel()
                {
                    ID = e.ID,
                    AssetParentStr = e.AssetParent,
                    AssetDesc = e.AssetDesc,
                    AssetCode = e.AssetCode
                }).ToList();

            var allDevices = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => !x.IsDeleted && locationIds.Contains(x.AmLocationID.Value))
                .Select(e => new AmDeviceModel()
                {
                    ID = e.ID,
                    AssetParentStr = e.AssetParent,
                    AssetDesc = e.AssetDesc,
                    AssetCode = e.AssetCode
                }).ToList();

            var deviceCodes = new HashSet<string>(matchedDevices.Select(x => x.AssetCode));
            
            foreach (var device in matchedDevices)
            {
                var parentCode = device.AssetParentStr;
                while (!string.IsNullOrWhiteSpace(parentCode))
                {
                    deviceCodes.Add(parentCode);
                    parentCode = allDevices.FirstOrDefault(x => x.AssetCode == parentCode)?.AssetParentStr;
                }
            }
            
            var filteredDevices = allDevices.Where(x => deviceCodes.Contains(x.AssetCode)).ToList();

            foreach (var device in filteredDevices)
            {
                device.HasChildren = filteredDevices.Any(x => x.AssetParentStr == device.AssetCode);
            }
            var rootDevices = filteredDevices.Where(x => x.AssetParentStr == null).OrderBy(x => x.AssetDesc).ToList();
            
            var totalRecords = rootDevices.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)requestModel.PageSize);
            var pagedRootDevices = rootDevices.Skip((requestModel.PageNumber - 1) * requestModel.PageSize).Take(requestModel.PageSize).ToList();
            
            if (requestModel.IncludeChildren)
            {
                var hierarchyList = new List<AmDeviceHierachyModel>();
                foreach (var root in pagedRootDevices)
                {
                    hierarchyList.Add(new AmDeviceHierachyModel
                    {
                        Data = root,
                        Children = BuildChildren(filteredDevices, root.AssetCode)
                    });
                }
                return HttpResponse.OkList(hierarchyList, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
            }
            else
            {
                var rootList = pagedRootDevices.Select(root => new AmDeviceHierachyModel
                {
                    Data = root,
                    Children = new List<AmDeviceHierachyModel>()
                }).ToList();
                return HttpResponse.OkList(rootList, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
            }
        }

        public ResponseBaseModel GetRootDevices(AmDeviceSearchModel requestModel)
        {
            var locations = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => !x.IsDeleted && x.OrganizationID == requestModel.CompanyID).ToList();

            var locationIds = locations.Select(x => x.ID).ToList();

            bool hasSearchFilter = !string.IsNullOrWhiteSpace(requestModel.SearchText) 
                || requestModel.AssetParent != CommonConstant.ALL_KEY
                || requestModel.SiteID != CommonConstant.ALL_KEY
                || requestModel.TypeDescID != CommonConstant.ALL_KEY
                || requestModel.Active != CommonConstant.ALL_KEY
                || requestModel.DeviceStatusID != CommonConstant.ALL_KEY;

            if (hasSearchFilter)
            {
                var entities = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
                {
                    entities = entities.Where(x => x.AssetDesc.Contains(requestModel.SearchText) || x.AssetCode.Contains(requestModel.SearchText));
                }

                if (requestModel.AssetParent != CommonConstant.ALL_KEY)
                {
                    if (requestModel.AssetParent != CommonConstant.ZERO)
                    {
                        var assetCode = _unitOfWork.ElogbookRepository<AmDevice>()
                        .GetQuery(x => x.ID == requestModel.AssetParent && !x.IsDeleted).Select(t => t.AssetCode).FirstOrDefault();
                        entities = entities.Where(x => x.AssetParent.Equals(assetCode));
                    }
                    else
                    {
                        entities = entities.Where(x => x.AssetParent == null);
                    }
                }

                if (requestModel.SiteID != CommonConstant.ALL_KEY)
                {
                    entities = entities.Where(x => x.AmLocationID == requestModel.SiteID);
                }
                else
                {
                    entities = entities.Where(x => locationIds.Contains(x.AmLocationID.Value));
                }

                if (requestModel.TypeDescID != CommonConstant.ALL_KEY)
                {
                    entities = entities.Where(x => x.AmDeviceTypeID == requestModel.TypeDescID);
                }

                if (requestModel.Active != CommonConstant.ALL_KEY)
                {
                    entities = entities.Where(x => x.Active == requestModel.Active);
                }

                if (requestModel.DeviceStatusID != CommonConstant.ALL_KEY)
                {
                    entities = entities.Where(x => x.AmDeviceStatusID == requestModel.DeviceStatusID);
                }

                var matchedDevices = entities.Select(
                    e => new AmDeviceModel()
                    {
                        ID = e.ID,
                        AssetParentStr = e.AssetParent,
                        AssetDesc = e.AssetDesc,
                        AssetCode = e.AssetCode
                    }).ToList();

                var allDevices = _unitOfWork.ElogbookRepository<AmDevice>()
                    .GetQuery(x => !x.IsDeleted && locationIds.Contains(x.AmLocationID.Value))
                    .Select(e => new AmDeviceModel()
                    {
                        ID = e.ID,
                        AssetParentStr = e.AssetParent,
                        AssetDesc = e.AssetDesc,
                        AssetCode = e.AssetCode
                    }).ToList();

                var deviceCodes = new HashSet<string>(matchedDevices.Select(x => x.AssetCode));
                
                foreach (var device in matchedDevices)
                {
                    var parentCode = device.AssetParentStr;
                    while (!string.IsNullOrWhiteSpace(parentCode))
                    {
                        deviceCodes.Add(parentCode);
                        parentCode = allDevices.FirstOrDefault(x => x.AssetCode == parentCode)?.AssetParentStr;
                    }
                }
                
                var filteredDevices = allDevices.Where(x => deviceCodes.Contains(x.AssetCode)).ToList();

                foreach (var device in filteredDevices)
                {
                    device.HasChildren = allDevices.Any(x => x.AssetParentStr == device.AssetCode);
                }

                var rootDevices = filteredDevices.Where(x => x.AssetParentStr == null).OrderBy(x => x.AssetDesc).ToList();
                
                var totalRecords = rootDevices.Count;
                var totalPages = (int)Math.Ceiling(totalRecords / (double)requestModel.PageSize);
                var pagedRootDevices = rootDevices.Skip((requestModel.PageNumber - 1) * requestModel.PageSize).Take(requestModel.PageSize).ToList();

                return HttpResponse.OkList(pagedRootDevices, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
            }
            else
            {
                var entities = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => !x.IsDeleted && x.AssetParent == null);

                entities = entities.Where(x => locationIds.Contains(x.AmLocationID.Value));

                var rootDevices = entities.Select(
                    e => new AmDeviceModel()
                    {
                        ID = e.ID,
                        AssetParentStr = e.AssetParent,
                        AssetDesc = e.AssetDesc,
                        AssetCode = e.AssetCode
                    }).OrderBy(x => x.AssetDesc).ToList();

                var allDevicesCodes = _unitOfWork.ElogbookRepository<AmDevice>()
                    .GetQuery(x => !x.IsDeleted && locationIds.Contains(x.AmLocationID.Value))
                    .Select(e => new { AssetCode = e.AssetCode, AssetParent = e.AssetParent })
                    .ToList();

                foreach (var device in rootDevices)
                {
                    device.HasChildren = allDevicesCodes.Any(x => x.AssetParent == device.AssetCode);
                }

                var totalRecords = rootDevices.Count;
                var totalPages = (int)Math.Ceiling(totalRecords / (double)requestModel.PageSize);
                var pagedRootDevices = rootDevices.Skip((requestModel.PageNumber - 1) * requestModel.PageSize).Take(requestModel.PageSize).ToList();

                return HttpResponse.OkList(pagedRootDevices, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
            }
        }

        public List<BaseHierachyModel> GetLocationDeviceHierarchy()
        {
            List<BasicInfoModel> locations = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply)
                .Select(e => new BasicInfoModel()
                {
                    Id = e.ID,
                    Name = e.SiteDesc,
                    Code = e.SiteCode,
                    ParentCode = e.SiteParent,
                    Type = "Location"
                })
                .ToList();

            List<BasicInfoModel> devices = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && x.AmLocationID != null)
                .Select(e => new BasicInfoModel()
                {
                    Id = e.ID,
                    Name = e.AssetDesc,
                    Code = e.AssetCode,
                    ParentCode = e.AssetParent,
                    LocationId = e.AmLocationID.Value,
                    Type = "Device"
                })
                .ToList();

            var rootLocations = locations.Where(x => string.IsNullOrWhiteSpace(x.ParentCode)).OrderBy(x => x.Name).ToList();
            var hierarchyList = new List<BaseHierachyModel>();

            foreach (var location in rootLocations)
            {
                hierarchyList.Add(new BaseHierachyModel
                {
                    Data = new BasicInfoModel { Id = location.Id, Name = location.Name, Code = location.Code, Type = "Location" },
                    Children = BuildLocationChildren(locations, devices, location.Code, location.Id)
                });
            }

            return hierarchyList;
        }

        public ResponseBaseModel ChangeParent(ChangeParentLocationModel requestModel)
        {
            var device = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => x.AssetCode == requestModel.Code && !x.IsDeleted).FirstOrDefault();

            if (device == null)
                return HttpResponse.Error("Thiết bị không tồn tại");

            if (requestModel.Type == "Device")
            {
                device.AssetParent = requestModel.ParentCode;
            }
            else if (requestModel.Type == "Location")
            {
                var location = _unitOfWork.ElogbookRepository<AmLocation>()
                    .GetQuery(x => x.SiteCode == requestModel.ParentCode && !x.IsDeleted).FirstOrDefault();

                if (location == null)
                    return HttpResponse.Error("Khu vực không tồn tại");

                device.AssetParent = null;
                device.AmLocationID = location.ID;
            }
            else
            {
                return HttpResponse.Error("Type không hợp lệ");
            }

            _unitOfWork.ElogbookRepository<AmDevice>().Update(device);

            if (_unitOfWork.Complete() > 0)
                return HttpResponse.OK("Cập nhật thành công");

            return HttpResponse.Error("Cập nhật thất bại");
        }

        public List<AmDeviceModel> GetDeviceChildren(int id)
        {
            var device = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();

            if (device == null)
                return new List<AmDeviceModel>();

            var children = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => x.AssetParent == device.AssetCode && !x.IsDeleted)
                .ToList();

            return children.Select(e => ConvertToAmDeviceModel(e)).ToList();
        }
        #endregion

        #region Private Method

        private List<BaseHierachyModel> BuildLocationChildren(List<BasicInfoModel> allLocations, List<BasicInfoModel> allDevices, string parentCode, int locationId)
        {
            var result = new List<BaseHierachyModel>();
            var childLocations = allLocations.Where(x => x.ParentCode == parentCode).OrderBy(x => x.Name).ToList();

            foreach (var child in childLocations)
            {
                result.Add(new BaseHierachyModel
                {
                    Data = new BasicInfoModel { Id = child.Id, Name = child.Name, Code = child.Code, Type = "Location" },
                    Children = BuildLocationChildren(allLocations, allDevices, child.Code, child.Id)
                });
            }

            var rootDevices = allDevices.Where(x => x.LocationId == locationId && string.IsNullOrWhiteSpace(x.ParentCode)).OrderBy(x => x.Name).ToList();
            foreach (var device in rootDevices)
            {
                result.Add(new BaseHierachyModel
                {
                    Data = new BasicInfoModel { Id = device.Id, Name = device.Name, Code = device.Code, Type = "Device" },
                    Children = BuildDeviceChildrenForLocation(allDevices, device.Code)
                });
            }

            return result;
        }

        private List<BaseHierachyModel> BuildDeviceChildrenForLocation(List<BasicInfoModel> allDevices, string parentCode)
        {
            var children = allDevices.Where(x => x.ParentCode == parentCode).OrderBy(x => x.Name).ToList();

            if (!children.Any())
                return new List<BaseHierachyModel>();

            var result = new List<BaseHierachyModel>();

            foreach (var child in children)
            {
                result.Add(new BaseHierachyModel
                {
                    Data = new BasicInfoModel { Id = child.Id, Name = child.Name, Code = child.Code, Type = "Device" },
                    Children = BuildDeviceChildrenForLocation(allDevices, child.Code)
                });
            }

            return result;
        }

        private List<AmDeviceHierachyModel> BuildChildren(List<AmDeviceModel> allDevices, string parentCode)
        {
            var children = allDevices.Where(x => x.AssetParentStr == parentCode).ToList();
            
            if (!children.Any())
                return new List<AmDeviceHierachyModel>();

            var result = new List<AmDeviceHierachyModel>();

            foreach (var child in children)
            {
                result.Add(new AmDeviceHierachyModel
                {
                    Data = child,
                    Children = BuildChildren(allDevices, child.AssetCode)
                });
            }

            return result;
        }

        private AmDeviceModel ConvertToAmDeviceModel(AmDevice e)
        {
            var hasChildren = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => x.AssetParent == e.AssetCode && !x.IsDeleted).Any();

            return new AmDeviceModel()
            {
                ID = e.ID,
                AssetDesc = e.AssetDesc,
                AssetCode = e.AssetCode,
                AssetParentStr = e.AssetParent,
                Active = e.Active,
                SiteDesc = e.AmLocation != null ? new ComboboxDataModel { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc, Code = e.AmLocation.SiteCode } : null,
                DeviceStatus = e.AmDeviceStatus != null ? new ValueLabelCheckedModel { Value = e.AmDeviceStatus.ID, Label = e.AmDeviceStatus.StatusDesc } : null,
                TypeDesc = e.AmDeviceTypeID != null && e.AmDeviceType != null ? new ComboboxDataModel { Value = e.AmDeviceType.ID, Label = e.AmDeviceType.TypeDesc, Code = e.AmDeviceType.TypeCode } : null,
                Manufacturer = e.AmManufacturerID != null && e.AmManufacturer != null ? new ComboboxDataModel { Value = e.AmManufacturer.ID, Label = e.AmManufacturer.ManufacturerName, Code = e.AmManufacturer.ManufacturerCode } : null,
                Supplier = e.AmSupplierID != null && e.AmSupplier != null ? new ComboboxDataModel { Value = e.AmSupplier.ID, Label = e.AmSupplier.SupplierName, Code = e.AmSupplier.SupplierCode } : null,
                CountryManufacture = e.AmCountryManufactureID != null && e.AmCountryManufacture != null ? new ComboboxDataModel { Value = e.AmCountryManufacture.ID, Label = e.AmCountryManufacture.CountryName, Code = e.AmCountryManufacture.CountryCode } : null,
                Organization = e.OrganizationID != null && e.Organization != null ? new ValueLabelCheckedModel { Value = e.Organization.ID, Label = e.Organization.Name } : null,
                DeviceCategory = e.AmDeviceCategoryID != null && e.AmDeviceCategory != null ? new ComboboxDataModel { Value = e.AmDeviceCategory.ID, Label = e.AmDeviceCategory.CategoryName, Code = e.AmDeviceCategory.CategoryCode } : null,
                SerialNum = e.SerialNum,
                AssetType = e.AssetType,
                Meter = e.Meter,
                Kss = e.Kss,
                Khvh = e.Khvh,
                Note = e.Note,
                Description = e.Description,
                Drawing = e.Drawing,
                ListImageIds = e.Images,
                InstallDate = e.InstallDate,
                OperatingDate = e.OperatingDate,
                GoogleMapUrl = e.GoogleMapUrl,
                HasChildren = hasChildren
            };
        }
        private void GetChildByID(int amDeviceID, ref List<int> IDs)
        {
            var assetCurrent = _unitOfWork.ElogbookRepository<AmDevice>().GetById(amDeviceID);
            var listID = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => !x.IsDeleted && x.AssetParent == assetCurrent.AssetCode).Select(x => x.ID);
            if (listID.Any())
            {
                IDs.AddRange(listID);
                foreach (var item in listID)
                {
                    GetChildByID(item, ref IDs);
                }
            }
        }

        private bool CheckDataIsUse(int id)
        {
            if (_unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().GetQuery(x => x.AmDeviceID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            if (_unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.AmDeviceId == id && !x.IsDeleted).Any())
            {
                return true;
            }

            if (_unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.AmDeviceID == id && !x.IsDeleted).Any())
            {
                return true;
            }

            return false;
        }

        private string SetTitleParent(List<AmDeviceModel> rootData, string childAsset)
        {
            var amDeviceEntityTemp = rootData.Find(x => x.AssetCode == childAsset);

            if (amDeviceEntityTemp != null)
            {
                string parentAsset = amDeviceEntityTemp.AssetParentStr;

                if (string.IsNullOrWhiteSpace(parentAsset))
                {
                    return ">";
                }
                StringBuilder strTxt = new StringBuilder();

                while (!string.IsNullOrWhiteSpace(parentAsset))
                {
                    strTxt.Append(">");
                    childAsset = parentAsset;
                    parentAsset = rootData.Where(x => x.AssetCode == childAsset).Select(x => x.AssetParentStr).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(parentAsset))
                    {
                        strTxt.Append(">");
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
        #endregion
    }
}
