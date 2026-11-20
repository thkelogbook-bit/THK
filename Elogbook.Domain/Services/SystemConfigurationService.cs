using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request.SystemConfiguration;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.SystemConfiguration;
using Elogbook.Data;
using Elogbook.Data.DataModels.Entities.SystemConfiguration;
using System;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Triển khai các phương thức sử dụng cho cấu hình hệ thống
    /// </summary>
    public class SystemConfigurationService : ISystemConfigurationService
    {
        #region Properties field

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        #endregion

        #region Contructor

        public SystemConfigurationService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        #endregion

        public AppVersionModel GetAppVersion()
        {
            var toReturn = _unitOfWork.ElogbookRepository<AppVersion>().GetQuery().Select(
                        e => new AppVersionModel()
                        {
                            ID = e.ID,
                            Version = e.Version,
                            ForceUpdate = e.ForceUpdate,
                            AppTitle = e.AppTitle,
                            LogoImageID = e.LogoImage,
                            BackgroundImageAfterLogin = e.BackgroundImageAfterLogin,
                            BackgroundImageBeforeLogin = e.BackgroundImageBeforeLogin,
                            BackgroundImageMobile = e.BackgroundImageMobile,
                            BackgroundLoginImage = e.BackgroundLoginImage,
                            LogoImageMobile = e.LogoImageMobile,
                            Copyright = e.Copyright,
                            LogoImageIOSMobile = e.LogoImageIOSMobile,
                            BackgroundImageIOSMobile = e.BackgroundImageIOSMobile,
                            WebVersion = e.WebVersion,
                            LeftMenuLogoIcon = e.LeftMenuLogoIcon,
                            IsShowShiftReceiving = AppSettings.IS_SHOW_SHIFT_RECEIVING,
                            IsShowNotification = AppSettings.IS_SHOW_NOTIFICATION,
                            AssignmentMaxCreateDays = AppSettings.ASSIGNMENT_MAX_CREATE_DAYS,
                            AssignmentUpdateDeadlineHours = AppSettings.ASSIGNMENT_UPDATE_DEADLINE_HOURS,
                        }).FirstOrDefault();


            if (toReturn != null)
            {
                if (toReturn.LogoImageID.HasValue)
                {
                    toReturn.LogoAttachment = GetAttachment(toReturn.LogoImageID);
                }

                if (toReturn.BackgroundImageAfterLogin.HasValue)
                {
                    toReturn.BackgroundImageAfterLoginAttachment = GetAttachment(toReturn.BackgroundImageAfterLogin);
                }

                if (toReturn.BackgroundImageBeforeLogin.HasValue)
                {
                    toReturn.BackgroundImageBeforeLoginAttachment = GetAttachment(toReturn.BackgroundImageBeforeLogin);
                }

                if (toReturn.BackgroundImageMobile.HasValue)
                {
                    toReturn.BackgroundImageMobileAttachment = GetAttachment(toReturn.BackgroundImageMobile);
                }

                if (toReturn.BackgroundLoginImage.HasValue)
                {
                    toReturn.BackgroundLoginImageAttachment = GetAttachment(toReturn.BackgroundLoginImage);
                }

                if (toReturn.LogoImageMobile.HasValue)
                {
                    toReturn.LogoImageMobileAttachment = GetAttachment(toReturn.LogoImageMobile);
                }

                if (toReturn.LogoImageIOSMobile.HasValue)
                {
                    toReturn.LogoImageIOSMobileAttachment = GetAttachment(toReturn.LogoImageIOSMobile);
                }

                if (toReturn.BackgroundImageIOSMobile.HasValue)
                {
                    toReturn.BackgroundImageIOSMobileAttachment = GetAttachment(toReturn.BackgroundImageIOSMobile);
                }

                if (toReturn.LeftMenuLogoIcon.HasValue)
                {
                    toReturn.LeftMenuLogoIconAttachment = GetAttachment(toReturn.LeftMenuLogoIcon);
                }

                var systemConfig = GetSystemConfiguration();
                if (systemConfig != null)
                {
                    toReturn.RefreshIntervalDashboard = systemConfig.RefreshIntervalDashboard;
                    toReturn.MaxFileSizeUpload = systemConfig.MaxFileSizeUpload;
                }
            }

            return toReturn;
        }

        private AttachmentModel GetAttachment(int? id)
        {
            if (id.HasValue)
            {
                var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == id).FirstOrDefault();
                if (att != null)
                {
                    return new AttachmentModel
                    {
                        ID = att.ID,
                        FileName = att.FileName,
                        FilePath = _commonService.GetPathFile(att.FilePath),
                        PathThumbnail = _commonService.GetPathFile(att.PathThumbnail),
                        ScreenName = att.ScreenName,
                        FileType = att.FileType,
                    };
                }
            }

            return null;
        }

        public AppVersionModel Save(AppVersionInputModel requestModel)
        {
            var data = _unitOfWork.ElogbookRepository<AppVersion>().GetQuery(e => !e.IsDeleted).FirstOrDefault();
            if (data != null)
            {
                data.ForceUpdate = requestModel.ForceUpdate;
                data.Version = requestModel.Version;
                data.AppTitle = requestModel.AppTitle;
                data.LogoImage = requestModel.LogoImage;
                data.BackgroundImageAfterLogin = requestModel.BackgroundImageAfterLogin;
                data.BackgroundImageBeforeLogin = requestModel.BackgroundImageBeforeLogin;
                data.BackgroundImageMobile = requestModel.BackgroundImageMobile;
                data.BackgroundLoginImage = requestModel.BackgroundLoginImage;
                data.LogoImageMobile = requestModel.LogoImageMobile;
                data.Copyright = requestModel.Copyright;
                data.LogoImageIOSMobile = requestModel.LogoImageIOSMobile;
                data.LeftMenuLogoIcon = requestModel.LeftMenuLogoIcon;
                data.BackgroundImageIOSMobile = requestModel.BackgroundImageIOSMobile;
                data.WebVersion = requestModel.WebVersion;
                _unitOfWork.ElogbookRepository<AppVersion>().Update(data);
            }
            else
            {
                data = new AppVersion()
                {
                    ForceUpdate = requestModel.ForceUpdate,
                    Version = requestModel.Version,
                    AppTitle = requestModel.AppTitle,
                    LogoImage = requestModel.LogoImage,
                    BackgroundImageAfterLogin = requestModel.BackgroundImageAfterLogin,
                    BackgroundImageBeforeLogin = requestModel.BackgroundImageBeforeLogin,
                    BackgroundImageMobile = requestModel.BackgroundImageMobile,
                    BackgroundLoginImage = requestModel.BackgroundLoginImage,
                    LogoImageMobile = requestModel.LogoImageMobile,
                    Copyright = requestModel.Copyright,
                    LogoImageIOSMobile = requestModel.LogoImageIOSMobile,
                    LeftMenuLogoIcon = requestModel.LeftMenuLogoIcon,
                    BackgroundImageIOSMobile = requestModel.BackgroundImageIOSMobile,
                    WebVersion = requestModel.WebVersion,
                };
                _unitOfWork.ElogbookRepository<AppVersion>().Add(data);
            }
            _unitOfWork.Complete();

            return GetAppVersion();
        }

        public SystemConfigurationModel GetSystemConfiguration()
        {
            try
            {
                var data = _unitOfWork.ElogbookRepository<SystemConfiguration>().GetQuery(e => !e.IsDeleted).ToList();
                var refreshIntervalDashboard = data.Where(e => e.ConfigKey == "RefreshIntervalDashboard").Select(e => e.ConfigValue).FirstOrDefault();
                var maxFileSizeUpload = data.Where(e => e.ConfigKey == "MaxFileSizeUpload").Select(e => e.ConfigValue).FirstOrDefault();
                return new SystemConfigurationModel
                {
                    FormatDate = data.Where(e => e.ConfigKey == "FormatDate").Select(e => e.ConfigValue).FirstOrDefault(),
                    FormatNumber = data.Where(e => e.ConfigKey == "FormatNumber").Select(e => e.ConfigValue).FirstOrDefault(),
                    DeleteLogCron = data.Where(e => e.ConfigKey == "DeleteLogCron").Select(e => e.ConfigValue).FirstOrDefault(),
                    RefreshIntervalDashboard = refreshIntervalDashboard == null ? 0 : int.Parse(refreshIntervalDashboard),
                    MaxFileSizeUpload = maxFileSizeUpload == null ? 0 : int.Parse(maxFileSizeUpload)
                };
            } 
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "GetSystemConfiguration", $"Error getting system configuration: {ex.Message}");
            }
            return null;

        }

        public SystemConfigurationModel UpdateSystemConfiguration(UpdateSystemConfigurationModel requestModel)
        {
            var data = _unitOfWork.ElogbookRepository<SystemConfiguration>().GetQuery(e => !e.IsDeleted).ToList();

            UpsertConfig(data, "FormatDate", requestModel.FormatDate);
            UpsertConfig(data, "FormatNumber", requestModel.FormatNumber);
            UpsertConfig(data, "DeleteLogCron", requestModel.DeleteLogCron);
            UpsertConfig(data, "RefreshIntervalDashboard", requestModel.RefreshIntervalDashboard.ToString());
            UpsertConfig(data, "MaxFileSizeUpload", requestModel.MaxFileSizeUpload.ToString());

            _unitOfWork.Complete();
            return GetSystemConfiguration();
        }

        private void UpsertConfig(System.Collections.Generic.List<SystemConfiguration> data, string key, string value)
        {
            var config = data.FirstOrDefault(e => e.ConfigKey == key);
            if (config != null)
            {
                config.ConfigValue = value;
                _unitOfWork.ElogbookRepository<SystemConfiguration>().Update(config);
            }
            else
            {
                _unitOfWork.ElogbookRepository<SystemConfiguration>().Add(new SystemConfiguration { ConfigKey = key, ConfigValue = value });
            }
        }
    }
}
