using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Data;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Elogbook.Domain.Services
{
    // These classes are intentionally empty for the purpose of this example.
    // They are simply marker classes for the purpose of demonstration,
    // contain no properties, and serve no other purpose.
    public class PmisParameterTask : IPmisParameterTask
    {
        #region Private fields

        private readonly PmisUnitOfWork _pmisUnitOfWork;
        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public PmisParameterTask(PmisUnitOfWork pmisUnitOfWork,
            ElogbookUnitOfWork unitOfWork)
        {
            _pmisUnitOfWork = pmisUnitOfWork;
            _unitOfWork = unitOfWork;
        }

        #endregion

        private async Task SyncOrganization(int jobId)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS org");

            var orgs = _pmisUnitOfWork.PmisRepository<S_ORGANIZATION>()
                .GetQuery(x => x.ACTIVE)
                .Select(
                    e => new PMIS_S_ORGANIZATION()
                    {
                        ORGID = e.ORGID,
                        UPD_LOGIN_CODE_TIME = e.UPD_LOGIN_CODE_TIME,
                        VERSION = e.VERSION,
                        PARENT_RPT_LEVEL = e.PARENT_RPT_LEVEL,
                        FUELTYPEID = e.FUELTYPEID,
                        ORGDESC = e.ORGDESC,
                        ORGCODE = e.ORGCODE,
                        ORGID_PARENT = e.ORGID_PARENT,
                        PARRELTYPEID = e.PARRELTYPEID,
                        ORGLEVEL = e.ORGLEVEL,
                        ACTIVE = e.ACTIVE,
                        APPURL = e.APPURL,
                        AUTO_LOGIN_CODE = e.AUTO_LOGIN_CODE,
                        ACTIVEDATE = e.ACTIVEDATE,
                        ISRUNNING = e.ISRUNNING,
                        ORGORD = e.ORGORD,
                        JobId = jobId
                    })
                .ToList();

            _unitOfWork.ElogbookRepository<PMIS_S_ORGANIZATION>().AddRange(orgs);
            _unitOfWork.Complete();

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS org done! >>");
        }

        private async Task SyncSite(int jobId)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS site");

            var orgs = _pmisUnitOfWork.PmisRepository<S_SITE>()
            .GetQuery()
            .Select(
                e => new PMIS_S_SITE()
                {
                    ORGID = e.ORGID,
                    SITEID = e.SITEID,
                    SITEDESC = e.SITEDESC,
                    ACTIVE = e.ACTIVE,
                    SITEID_PARENT = e.SITEID_PARENT,
                    JobId = jobId
                })
            .ToList();

            _unitOfWork.ElogbookRepository<PMIS_S_SITE>().AddRange(orgs);
            _unitOfWork.Complete();

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS SITE done! >>");
        }

        private async Task LoadSite(int jobId)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS site");

            try
            {
                List<PMIS_S_SITE> existingTypes = _unitOfWork.ElogbookRepository<PMIS_S_SITE>()
                    .GetQuery(x => !x.IsSync != true && x.JobId == jobId).ToList();

                if (existingTypes.Count > 0)
                {
                    for (var i = 0; i < existingTypes.Count; i++)
                    {
                        PMIS_S_SITE type = existingTypes[i];

                        Organization org = _unitOfWork.ElogbookRepository<Organization>()
                            .GetQuery(x => !x.IsDeleted && x.PMIS_ORGID == type.ORGID).FirstOrDefault();

                        AmLocation location = _unitOfWork.ElogbookRepository<AmLocation>()
                                .GetQuery(x => x.PMISId == type.SITEID).FirstOrDefault();

                        if (location != null)
                        {
                            location.SiteCode = type.SITEID;
                            location.SiteDesc = type.SITEDESC;
                            if (org != null)
                                location.OrganizationID = org.ID;

                            location.Active = type.ACTIVE ? (int)AccessStatus.Active : (int)AccessStatus.InActive;
                            location.SiteParent = type.SITEID_PARENT;
                            location.IsDeleted = false;

                            _unitOfWork.ElogbookRepository<AmLocation>().Update(location);
                        }
                        else
                        {
                            AmLocation locationNew = new AmLocation
                            {
                                SiteCode = type.SITEID,
                                SiteDesc = type.SITEDESC,
                                PMISId = type.SITEID,
                                Active = type.ACTIVE ? (int)AccessStatus.Active : (int)AccessStatus.InActive
                            };

                            if (org != null)
                                locationNew.OrganizationID = org.ID;
                            locationNew.SiteParent = type.SITEID_PARENT;
                            _unitOfWork.ElogbookRepository<AmLocation>().Add(locationNew);
                        }

                        type = _unitOfWork.ElogbookRepository<PMIS_S_SITE>().Refresh(type);
                        type.IsSync = true;
                        _unitOfWork.ElogbookRepository<PMIS_S_SITE>().Update(type);
                    }
                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PMISParameterTask.ToString() + Environment.NewLine + ex.ToString());
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS SITE done! >>");
        }

        private async Task LoadOrg(int jobId)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS org");

            List<PMIS_S_ORGANIZATION> existingTypes = _unitOfWork.ElogbookRepository<PMIS_S_ORGANIZATION>()
                    .GetQuery(x => x.IsSync != true && x.JobId == jobId).ToList();

            if (existingTypes.Count > 0)
            {
                for (var i = 0; i < existingTypes.Count; i++)
                {
                    PMIS_S_ORGANIZATION org = existingTypes[i];

                    Organization organization = _unitOfWork.ElogbookRepository<Organization>()
                            .GetQuery(x => x.PMIS_ORGID == org.ORGID).FirstOrDefault();

                    Organization organizationParent = _unitOfWork.ElogbookRepository<Organization>()
                            .GetQuery(x => x.PMIS_ORGID == org.ORGID_PARENT).FirstOrDefault();

                    if (organization != null)
                    {
                        if (organizationParent != null)
                            organization.ParentID = organizationParent.ID;

                        organization.IsDeleted = false;
                        _unitOfWork.ElogbookRepository<Organization>().Update(organization);
                    }
                    else
                    {
                        Organization organizationNew = new Organization();
                        if (organizationParent != null)
                            organizationNew.ParentID = organizationParent.ID;

                        organizationNew.Name = org.ORGDESC;
                        organizationNew.ShortName = org.ORGID;
                        organizationNew.PMIS_ORGID = org.ORGID;
                        organizationNew.PMIS_ORGCODE = org.ORGCODE;
                        organizationNew.PMIS_ORGID_PARENT = org.ORGID_PARENT;

                        organizationNew.Active = org.ACTIVE ? 1 : 0;
                        organizationNew.Level = org.ORGLEVEL == "P" || org.ORGLEVEL == null ? "Công ty" : org.ORGLEVEL;

                        _unitOfWork.ElogbookRepository<Organization>().Add(organizationNew);
                    }

                    org = _unitOfWork.ElogbookRepository<PMIS_S_ORGANIZATION>().Refresh(org);
                    org.IsSync = true;
                    _unitOfWork.ElogbookRepository<PMIS_S_ORGANIZATION>().Update(org);
                }

                _unitOfWork.Complete();
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS SITE done! >>");
        }

        private async Task SyncMaintainnance(DateTime lastRun, int jobId)
        {
            try
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS maintenance");

                string query = @"SELECT 
	                w.WORDERID,
	                NULL AS TASKID,
	                w.WDESC AS TASKDESC,
	                w.PLANSTARTDATE AS TACSTARTDATE,
	                w.PLANFINISHDATE AS TPLFINISHDATE,
	                w.ACTUALSTARTDATE AS TACSTARTDATE,
	                w.ACTUALFINISHDATE AS TACFINISHDATE,
	                w.WTYPEID,
	                wa.ASSETID,
                    @JobId AS JobId
                FROM dbo.W_WORDER w WITH(NOLOCK)
                LEFT JOIN dbo.W_WORDER_ASSET_LINK wa WITH(NOLOCK) ON w.WORDERID = wa.WORDERID 
                WHERE w.WSTATUSID_ALL IN ('COMPLETE', 'DRAFT', 'INEXEC', 'INPLAN', 'INPROG')
	                AND (w.WORDERID IN (SELECT WORDERID FROM dbo.W_WORDER_ASSET_LINK wa, dbo.A_ASSET a  WHERE wa.ASSETID = a.ASSETID )
	                OR w.WORDERID IN (SELECT WORDERID FROM dbo.W_WORDER_UNIT u, dbo.A_ASSET a WHERE u.ASSETID = a.assetid ) 
	                OR w.WORDERID IN (SELECT WORDERID FROM dbo.W_TASK t 
						                INNER JOIN dbo.W_TASK_ASSET_LINK ta ON t.TASKID = ta.TASKID 
						                INNER JOIN A_ASSET a ON ta.ASSETID = a.ASSETID
                )) AND USER_MDF_DTIME >= @date1
                UNION ALL
                SELECT 
	                w.WORDERID,
	                t.TASKID,
	                t.TASKDESC,
	                t.TPLSTARTDATE,
	                t.TPLFINISHDATE,
	                t.TACSTARTDATE,
	                t.TACFINISHDATE,
	                w.WTYPEID,
	                ta.ASSETID,
                    @JobId AS JobId
                FROM dbo.W_TASK t 
                INNER JOIN dbo.W_WORDER w ON t.WORDERID = w.WORDERID 
                LEFT JOIN dbo.W_TASK_ASSET_LINK ta on t.TASKID =ta.TASKID 
                WHERE TSTATUSID NOT IN ('1604') 
	                AND(t.taskid IN (SELECT TASKID FROM dbo.W_TASK_ASSET_LINK ta 
	                INNER JOIN dbo.A_ASSET a ON ta.ASSETID = a.ASSETID
                )) AND (t.USER_MDF_DTIME >= @date2 OR w.USER_MDF_DTIME >= @date3)";

                var data = _pmisUnitOfWork.DataContext.Database.SqlQuery
                    <PMIS_ASSET_MAINTENANCE>(query,
                    new SqlParameter("date1", lastRun),
                    new SqlParameter("date2", lastRun),
                    new SqlParameter("date3", lastRun),
                    new SqlParameter("JobId", jobId)).ToList();

                var assetHistories = CreateAssetHistories(data);
                using (var context = new ElogbookDataContext())
                {
                    var sqlParams = new SqlParameter[]
                    {
                        new SqlParameter
                        {
                            ParameterName = "@AssetHistories_Input",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.A_ASSET_HISTORY_TABLE_TYPE",
                            Value = assetHistories,
                        },
                    };

                    context.Database.ExecuteSqlCommand("EXEC [dbo].[SYNC_PMIS_A_ASSET_HISTORY] @AssetHistories_Input", sqlParams);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString() + Environment.NewLine + ">> init sync PMIS maintenance done! >>");
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PMISParameterTask.ToString(),
                    $"There was an error while synchronizing A_ASSET PMIS data. Please try again or contact the system administrator for support."
                    + Environment.NewLine + ex.ToString()
                    + Environment.NewLine + ex.StackTrace);
            }
        }

        private async Task SyncAssetType(int jobId)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> init sync PMIS asset type");

            var types = _pmisUnitOfWork.PmisRepository<A_LST_TYPE>()
                .GetQuery(x => x.TYPEID != null)
                .Select(
                    e => new PMIS_A_LST_TYPE()
                    {
                        TYPEID = e.TYPEID,
                        TYPEDESC = e.TYPEDESC,
                        TYPEORD = e.TYPEORD,
                        JobId = jobId
                    })
                .ToList();

            List<AmDeviceType> existingTypes = _unitOfWork.ElogbookRepository<AmDeviceType>()
                .GetQuery(x => !x.IsDeleted).ToList();
            var mapOrgs = new Dictionary<string, AmDeviceType>();

            for (var i = 0; i < existingTypes.Count; i++)
            {
                if (existingTypes[i].PMISId != null)
                    mapOrgs[existingTypes[i].PMISId] = existingTypes[i];
            }
            // Find existing data and save if it does not exist
            var toSave = new List<PMIS_A_LST_TYPE>();
            for (var i = 0; i < types.Count; i++)
            {
                if (!mapOrgs.ContainsKey(types[i].TYPEID))
                {
                    toSave.Add(types[i]);
                }
            }
            _unitOfWork.ElogbookRepository<PMIS_A_LST_TYPE>().AddRange(toSave);
            _unitOfWork.Complete();

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> init sync PMIS types done! >>");
        }

        private async Task LoadAssetType(int jobId)
        {
            List<PMIS_A_LST_TYPE> existingTypes = _unitOfWork.ElogbookRepository<PMIS_A_LST_TYPE>()
                    .GetQuery(x => x.IsSync != true && x.JobId == jobId).ToList();
            if (existingTypes.Count > 0)
            {
                for (var i = 0; i < existingTypes.Count; i++)
                {
                    PMIS_A_LST_TYPE type = existingTypes[i];

                    AmDeviceType eType = _unitOfWork.ElogbookRepository<AmDeviceType>()
                        .GetQuery(x => x.PMISId == type.TYPEID).FirstOrDefault();
                    if (eType != null)
                    {
                        eType.TypeDesc = type.TYPEDESC;
                        eType.IsDeleted = false;
                        _unitOfWork.ElogbookRepository<AmDeviceType>().Update(eType);
                    }
                    else
                    {
                        AmDeviceType eTypeNew = new AmDeviceType
                        {
                            TypeDesc = type.TYPEDESC,
                            PMISId = type.TYPEID,
                            TypeCode = type.TYPEID
                        };
                        _unitOfWork.ElogbookRepository<AmDeviceType>().Add(eTypeNew);
                    }

                    type = _unitOfWork.ElogbookRepository<PMIS_A_LST_TYPE>().Refresh(type);
                    type.IsSync = true;
                    _unitOfWork.ElogbookRepository<PMIS_A_LST_TYPE>().Update(type);
                }
                _unitOfWork.Complete();
            }
        }

        private async Task SyncCategories(int jobId)
        {
            try
            {
                var datas = CreateCategoryList(jobId);

                using (var context = new ElogbookDataContext())
                {
                    var sqlParams = new SqlParameter[]
                    {
                        new SqlParameter
                        {
                            ParameterName = "@Data_Input",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.A_LST_CATEGORY_TABLE_TYPE",
                            Value = datas
                        },
                    };

                    context.Database.ExecuteSqlCommand("EXEC [dbo].[SYNC_PMIS_A_LST_CATEGORY] @Data_Input", sqlParams);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), $"Load data A_LST_CATEGORY from PMIS: {datas.Rows}");
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PMISParameterTask.ToString(),
                    $"There was an error while synchronizing A_LST_CATEGORY PMIS data. Please try again or contact the system administrator for support."
                    + Environment.NewLine + ex.ToString()
                    + Environment.NewLine + ex.StackTrace);
            }
        }

        private async Task LoadCategories(int jobId)
        {
            try
            {
                using (var context = new ElogbookDataContext())
                {
                    var sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("JobId", SqlDbType.Int)
                        {
                            Value = jobId,
                        },
                    };

                    context.Database.ExecuteSqlCommand("EXEC [dbo].[SYNC_PMIS_AMDEVICECATEGORY] @JobId", sqlParams);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), $"LoadCategories: sync data success.");
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PMISParameterTask.ToString(),
                    $"There was an error while synchronizing PMIS data. Please try again or contact the system administrator for support."
                    + Environment.NewLine + ex.ToString()
                    + Environment.NewLine + ex.StackTrace);
            }
        }

        public async Task SyncAsset(int jobId)
        {
            try
            {
                var currentDate = DateTime.Now;
                var asset = CreateAssetLlist(jobId, currentDate);
                var company = CreateCompanyList(jobId, currentDate);
                var country = CreateCountryList(jobId, currentDate);

                using (var context = new ElogbookDataContext())
                {
                    var sqlParams = new SqlParameter[]
                    {
                        new SqlParameter
                        {
                            ParameterName = "@Asset_Input",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.A_ASSET_TABLE_TYPE",
                            Value = asset
                        },
                        new SqlParameter
                        {
                            ParameterName = "@Company_Input",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.S_COMPANY_TABLE_TYPE",
                            Value = company
                        },
                        new SqlParameter
                        {
                            ParameterName = "@Country_Input",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.G_ERP_COUNTRY_TABLE_TYPE",
                            Value = country
                        },
                    };

                    context.Database.ExecuteSqlCommand("EXEC [dbo].[SYNC_PMIS_A_ASSET] @Asset_Input, @Company_Input, @Country_Input", sqlParams);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), $"Load data A_ASSET from PMIS: {asset.Rows}");
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PMISParameterTask.ToString(),
                    $"There was an error while synchronizing A_ASSET PMIS data. Please try again or contact the system administrator for support."
                    + Environment.NewLine + ex.ToString()
                    + Environment.NewLine + ex.StackTrace);
            }
        }

        private async Task LoadAsset(int jobId)
        {
            try
            {
                using (var context = new ElogbookDataContext())
                {
                    var sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("JobId", SqlDbType.Int)
                        {
                            Value = jobId,
                        },
                    };

                    context.Database.ExecuteSqlCommand("EXEC [dbo].[SYNC_PMIS_AMDEVICE] @JobId", sqlParams);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), $"LoadAsset: sync data success.");
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PMISParameterTask.ToString(),
                    $"There was an error while synchronizing PMIS data. Please try again or contact the system administrator for support."
                    + Environment.NewLine + ex.ToString()
                    + Environment.NewLine + ex.StackTrace);
            }
        }

        public async Task Extract(int jobId, DateTime lastRun, PmisSyncType type)
        {
            var jobHistory = _unitOfWork.ElogbookRepository<PmisJobHistory>()
                    .GetQuery(e => e.ID == jobId)
                    .FirstOrDefault();
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting PMIS data! >>" + (int)type);

            switch (type)
            {
                case PmisSyncType.AmDeviceType:
                    _ = SyncAssetType(jobHistory.ID);
                    int count = _unitOfWork.ElogbookRepository<PMIS_A_LST_TYPE>().GetQuery(x => x.JobId == jobHistory.ID).Count();
                    jobHistory.NumRecord = count;
                    break;

                case PmisSyncType.AmLocation:
                    _ = SyncOrganization(jobHistory.ID);
                    _ = SyncSite(jobHistory.ID);
                    int countSite = _unitOfWork.ElogbookRepository<PMIS_S_SITE>().GetQuery(x => x.JobId == jobHistory.ID).Count();
                    jobHistory.NumRecord = countSite;
                    break;

                case PmisSyncType.AmDevice:
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting Device maintenance data.! >>");
                    // sync Device maintenance
                    await SyncMaintainnance(lastRun, jobHistory.ID);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting Device maintenance data.! >>");

                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">>extracting PMIS data.! >> type:PmisSyncType.AmDevice");
                    // sync Device
                    int totalItem = CountItem();
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">>Count:" + totalItem);
                    jobHistory.NumRecord = totalItem;
                    await SyncAsset(jobHistory.ID);
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting PMIS data.! >> type:PmisSyncType.AmDevice");
                    break;

                case PmisSyncType.AmDeviceCategory:
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">>extracting PMIS data.! >> type:PmisSyncType.AmDeviceCategory");
                    _ = SyncCategories(jobHistory.ID);
                    int countCategory = _unitOfWork.ElogbookRepository<PMIS_A_LST_CATEGORY>().GetQuery(x => x.JobId == jobHistory.ID).Count();
                    jobHistory.NumRecord = countCategory;
                    LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting PMIS data.! >> type:PmisSyncType.AmDeviceCategory");
                    break;
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting PMIS data! >>");
        }

        public async Task Extract(int jobId, DateTime lastRun)
        {
            var jobHistory = _unitOfWork.ElogbookRepository<PmisJobHistory>()
                    .GetQuery(e => e.ID == jobId)
                    .FirstOrDefault();

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting PMIS data.! >>");

            if (jobHistory != null)
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting ORG data.! >>");
                // sync Orgranization
                _ = SyncOrganization(jobHistory.ID);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting ORG data.! >>");

                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting SITE data.! >>");
                // sync Site
                _ = SyncSite(jobHistory.ID);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting SITE data.! >>");

                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting ASSET TYPE data.! >>");
                // sync Asset Type
                _ = SyncAssetType(jobHistory.ID);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting  ASSET TYPE data.! >>");

                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting Categories data.! >>");
                // sync Categories
                _ = SyncCategories(jobHistory.ID);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting Categories data.! >>");

                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting Device maintenance data.! >>");
                // sync Device maintenance
                _ = SyncMaintainnance(lastRun, jobHistory.ID);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting Device maintenance data.! >>");

                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> extracting Device data.! >>");
                // sync Device
                int totalItems = CountItem();
                jobHistory.NumRecord = totalItems;
                await SyncAsset(jobHistory.ID);
                LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting DEvice data.! >>");
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END extracting PMIS data.! >>");
        }

        public async Task Load(int jobId, PmisSyncType type)
        {
            switch (type)
            {
                case PmisSyncType.AmDeviceType:
                    // sync Asset Type
                    await LoadAssetType(jobId);
                    break;

                case PmisSyncType.AmLocation:
                    _ = LoadOrg(jobId);
                    _ = LoadSite(jobId);
                    break;

                case PmisSyncType.AmDevice:
                    // sync Device
                    await LoadAsset(jobId);
                    break;

                case PmisSyncType.AmDeviceCategory:
                    _ = LoadCategories(jobId);
                    break;
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> Clean data.! >>");
            //Clean data
            BackgroundJob.Enqueue<IPmisParameterTask>(x => x.CleanData(jobId));
            LogCustom.LogMessage(TrackingLogLevel.INFO, Category.PMISParameterTask.ToString(), ">> END Clean data.! >>");
        }

        public async Task Load(int jobId)
        {
            // sync Asset Type
            await LoadAssetType(jobId);
            // sync Org
            _ = LoadOrg(jobId);
            // sync Location
            _ = LoadSite(jobId);
            _ = LoadCategories(jobId);
            // sync Device
            await LoadAsset(jobId);

            BackgroundJob.Enqueue<IPmisParameterTask>(x => x.CleanData(jobId));
        }

        #region Clean Data

        public void CleanData(int jobId)
        {
            _unitOfWork.ElogbookRepository<PMIS_A_LST_TYPE>().Delete(x => x.JobId == jobId, 0, true);
            _unitOfWork.Complete();

            _unitOfWork.ElogbookRepository<PMIS_S_ORGANIZATION>().Delete(x => x.JobId == jobId, 0, true);
            _unitOfWork.Complete();

            _unitOfWork.ElogbookRepository<PMIS_S_SITE>().Delete(x => x.JobId == jobId, 0, true);
            _unitOfWork.Complete();
        }

        #endregion

        private int CountItem()
        {
            var totalCount = _pmisUnitOfWork.PmisRepository<A_ASSET>()
                .GetQuery(x => x.SITEID != null).Count();

            return totalCount;
        }

        private DataTable CreateAssetLlist(int jobId, DateTime currentDate)
        {
            var datas = _pmisUnitOfWork.PmisRepository<A_ASSET>()
                            .GetQuery(x => x.SITEID != null)
                            .OrderBy(t => t.ASSETID).ToList();

            DataTable result = new DataTable();
            result.Columns.Add("ASSETID", typeof(string));
            result.Columns.Add("ASSETID_PARENT", typeof(string));
            result.Columns.Add("ASSETID_ORG", typeof(string));
            result.Columns.Add("ASSETDESC", typeof(string));
            result.Columns.Add("ASSETORD", typeof(int));
            result.Columns.Add("SITEID", typeof(string));
            result.Columns.Add("CATEGORYID", typeof(string));
            result.Columns.Add("TYPEID", typeof(string));
            result.Columns.Add("MANDEPTIDLIST", typeof(string));
            result.Columns.Add("USESTATUS_LAST_ID", typeof(string));
            result.Columns.Add("USESTATUS_LAST_DTIME", typeof(DateTime));
            result.Columns.Add("SERIALNUM", typeof(string));
            result.Columns.Add("P_MANUFACTURERID", typeof(string));
            result.Columns.Add("P_VENDORID", typeof(string));
            result.Columns.Add("P_INSTALLDATE", typeof(DateTime));
            result.Columns.Add("P_PRICE", typeof(decimal));
            result.Columns.Add("P_PRICEUOM", typeof(string));
            result.Columns.Add("P_PRICE_INC_ALL_CHILD", typeof(bool));
            result.Columns.Add("PRIORITY", typeof(int));
            result.Columns.Add("USER_CR_ID", typeof(string));
            result.Columns.Add("USER_CR_DTIME", typeof(DateTime));
            result.Columns.Add("USER_MDF_ID", typeof(string));
            result.Columns.Add("USER_MDF_DTIME", typeof(DateTime));
            result.Columns.Add("USESTATUS_UPDATE_ID", typeof(string));
            result.Columns.Add("ASSETIDSORT", typeof(string));
            result.Columns.Add("CHILDCOUNT", typeof(int));
            result.Columns.Add("COUNTRY_ID", typeof(int));
            result.Columns.Add("ISFIX", typeof(bool));
            result.Columns.Add("P_OPERATIONDATE", typeof(DateTime));
            result.Columns.Add("NONE_ATTACH", typeof(bool));
            result.Columns.Add("NONE_MATERIAL", typeof(bool));
            result.Columns.Add("NONE_METER_DYN", typeof(bool));
            result.Columns.Add("NONE_SAFE", typeof(bool));
            result.Columns.Add("P_MAN_YEAR", typeof(int));
            result.Columns.Add("USER_DEL_DTIME", typeof(DateTime));
            result.Columns.Add("USER_DEL_ID", typeof(string));
            result.Columns.Add("ISDEL", typeof(bool));
            result.Columns.Add("CHILD_METER_DYN", typeof(bool));
            result.Columns.Add("CHILD_MATERIAL", typeof(bool));
            result.Columns.Add("JobId", typeof(int));
            result.Columns.Add("IsDeleted", typeof(bool));
            result.Columns.Add("InsertedAt", typeof(DateTime));
            result.Columns.Add("UpdatedAt", typeof(DateTime));

            foreach (var item in datas)
            {
                DataRow newRow = result.NewRow();

                newRow["ASSETID"] = item.ASSETID;

                if (!string.IsNullOrWhiteSpace(item.ASSETID_PARENT))
                {
                    newRow["ASSETID_PARENT"] = item.ASSETID_PARENT;
                }
                else
                {
                    newRow["ASSETID_PARENT"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.ASSETID_ORG))
                {
                    newRow["ASSETID_ORG"] = item.ASSETID_ORG;
                }
                else
                {
                    newRow["ASSETID_ORG"] = DBNull.Value;
                }

                newRow["ASSETDESC"] = item.ASSETDESC;

                if (item.ASSETORD.HasValue)
                {
                    newRow["ASSETORD"] = item.ASSETORD;
                }
                else
                {
                    newRow["ASSETORD"] = DBNull.Value;
                }

                newRow["SITEID"] = item.SITEID;

                if (!string.IsNullOrWhiteSpace(item.CATEGORYID))
                {
                    newRow["CATEGORYID"] = item.CATEGORYID;
                }
                else
                {
                    newRow["CATEGORYID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.TYPEID))
                {
                    newRow["TYPEID"] = item.TYPEID;
                }
                else
                {
                    newRow["TYPEID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.MANDEPTIDLIST))
                {
                    newRow["MANDEPTIDLIST"] = item.MANDEPTIDLIST;
                }
                else
                {
                    newRow["MANDEPTIDLIST"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USESTATUS_LAST_ID))
                {
                    newRow["USESTATUS_LAST_ID"] = item.USESTATUS_LAST_ID;
                }
                else
                {
                    newRow["USESTATUS_LAST_ID"] = DBNull.Value;
                }

                if (item.USESTATUS_LAST_DTIME.HasValue)
                {
                    newRow["USESTATUS_LAST_DTIME"] = item.USESTATUS_LAST_DTIME;
                }
                else
                {
                    newRow["USESTATUS_LAST_DTIME"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.SERIALNUM))
                {
                    newRow["SERIALNUM"] = item.SERIALNUM;
                }
                else
                {
                    newRow["SERIALNUM"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.P_MANUFACTURERID))
                {
                    newRow["P_MANUFACTURERID"] = item.P_MANUFACTURERID;
                }
                else
                {
                    newRow["P_MANUFACTURERID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.P_VENDORID))
                {
                    newRow["P_VENDORID"] = item.P_VENDORID;
                }
                else
                {
                    newRow["P_VENDORID"] = DBNull.Value;
                }

                if (item.P_INSTALLDATE.HasValue)
                {
                    newRow["P_INSTALLDATE"] = item.P_INSTALLDATE;
                }
                else
                {
                    newRow["P_INSTALLDATE"] = DBNull.Value;
                }

                if (item.P_PRICE.HasValue)
                {
                    newRow["P_PRICE"] = item.P_PRICE;
                }
                else
                {
                    newRow["P_PRICE"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.P_PRICEUOM))
                {
                    newRow["P_PRICEUOM"] = item.P_PRICEUOM;
                }
                else
                {
                    newRow["P_PRICEUOM"] = DBNull.Value;
                }

                if (item.P_PRICE_INC_ALL_CHILD.HasValue)
                {
                    newRow["P_PRICE_INC_ALL_CHILD"] = item.P_PRICE_INC_ALL_CHILD;
                }
                else
                {
                    newRow["P_PRICE_INC_ALL_CHILD"] = DBNull.Value;
                }

                if (item.PRIORITY.HasValue)
                {
                    newRow["PRIORITY"] = item.PRIORITY;
                }
                else
                {
                    newRow["PRIORITY"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USER_CR_ID))
                {
                    newRow["USER_CR_ID"] = item.USER_CR_ID;
                }
                else
                {
                    newRow["USER_CR_ID"] = DBNull.Value;
                }

                if (item.USER_CR_DTIME.HasValue)
                {
                    newRow["USER_CR_DTIME"] = item.USER_CR_DTIME;
                }
                else
                {
                    newRow["USER_CR_DTIME"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USER_MDF_ID))
                {
                    newRow["USER_MDF_ID"] = item.USER_MDF_ID;
                }
                else
                {
                    newRow["USER_MDF_ID"] = DBNull.Value;
                }

                if (item.USER_MDF_DTIME.HasValue)
                {
                    newRow["USER_MDF_DTIME"] = item.USER_MDF_DTIME;
                }
                else
                {
                    newRow["USER_MDF_DTIME"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USESTATUS_UPDATE_ID))
                {
                    newRow["USESTATUS_UPDATE_ID"] = item.USESTATUS_UPDATE_ID;
                }
                else
                {
                    newRow["USESTATUS_UPDATE_ID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.ASSETIDSORT))
                {
                    newRow["ASSETIDSORT"] = item.ASSETIDSORT;
                }
                else
                {
                    newRow["ASSETIDSORT"] = DBNull.Value;
                }

                if (item.CHILDCOUNT.HasValue)
                {
                    newRow["CHILDCOUNT"] = item.CHILDCOUNT;
                }
                else
                {
                    newRow["CHILDCOUNT"] = DBNull.Value;
                }

                if (item.COUNTRY_ID.HasValue)
                {
                    newRow["COUNTRY_ID"] = item.COUNTRY_ID;
                }
                else
                {
                    newRow["COUNTRY_ID"] = DBNull.Value;
                }

                if (item.ISFIX.HasValue)
                {
                    newRow["ISFIX"] = item.ISFIX;
                }
                else
                {
                    newRow["ISFIX"] = DBNull.Value;
                }

                if (item.P_OPERATIONDATE.HasValue)
                {
                    newRow["P_OPERATIONDATE"] = item.P_OPERATIONDATE;
                }
                else
                {
                    newRow["P_OPERATIONDATE"] = DBNull.Value;
                }

                if (item.NONE_ATTACH.HasValue)
                {
                    newRow["NONE_ATTACH"] = item.NONE_ATTACH;
                }
                else
                {
                    newRow["NONE_ATTACH"] = DBNull.Value;
                }

                if (item.NONE_MATERIAL.HasValue)
                {
                    newRow["NONE_MATERIAL"] = item.NONE_MATERIAL;
                }
                else
                {
                    newRow["NONE_MATERIAL"] = DBNull.Value;
                }

                if (item.NONE_METER_DYN.HasValue)
                {
                    newRow["NONE_METER_DYN"] = item.NONE_METER_DYN;
                }
                else
                {
                    newRow["NONE_METER_DYN"] = DBNull.Value;
                }

                if (item.NONE_SAFE.HasValue)
                {
                    newRow["NONE_SAFE"] = item.NONE_SAFE;
                }
                else
                {
                    newRow["NONE_SAFE"] = DBNull.Value;
                }

                if (item.P_MAN_YEAR.HasValue)
                {
                    newRow["P_MAN_YEAR"] = item.P_MAN_YEAR;
                }
                else
                {
                    newRow["P_MAN_YEAR"] = DBNull.Value;
                }

                if (item.USER_DEL_DTIME.HasValue)
                {
                    newRow["USER_DEL_DTIME"] = item.USER_DEL_DTIME;
                }
                else
                {
                    newRow["USER_DEL_DTIME"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USER_DEL_ID))
                {
                    newRow["USER_DEL_ID"] = item.USER_DEL_ID;
                }
                else
                {
                    newRow["USER_DEL_ID"] = DBNull.Value;
                }

                if (item.ISDEL.HasValue)
                {
                    newRow["ISDEL"] = item.ISDEL;
                }
                else
                {
                    newRow["ISDEL"] = DBNull.Value;
                }

                if (item.CHILD_METER_DYN.HasValue)
                {
                    newRow["CHILD_METER_DYN"] = item.CHILD_METER_DYN;
                }
                else
                {
                    newRow["CHILD_METER_DYN"] = DBNull.Value;
                }

                if (item.CHILD_MATERIAL.HasValue)
                {
                    newRow["CHILD_MATERIAL"] = item.CHILD_MATERIAL;
                }
                else
                {
                    newRow["CHILD_MATERIAL"] = DBNull.Value;
                }

                newRow["JobId"] = jobId;
                newRow["IsDeleted"] = false;
                newRow["InsertedAt"] = currentDate;
                newRow["UpdatedAt"] = currentDate;

                result.Rows.Add(newRow);
            }

            return result;
        }

        private DataTable CreateAssetHistories(List<PMIS_ASSET_MAINTENANCE> datas)
        {
            DataTable result = new DataTable();
            result.Columns.Add("WORDERID", typeof(string));
            result.Columns.Add("TASKID", typeof(string));
            result.Columns.Add("TASKDESC", typeof(string));
            result.Columns.Add("TPLSTARTDATE", typeof(DateTime));
            result.Columns.Add("TPLFINISHDATE", typeof(DateTime));
            result.Columns.Add("TACSTARTDATE", typeof(DateTime));
            result.Columns.Add("TACFINISHDATE", typeof(DateTime));
            result.Columns.Add("WTYPEID", typeof(string));
            result.Columns.Add("ASSETID", typeof(string));
            result.Columns.Add("JobId", typeof(int));

            foreach (var item in datas)
            {
                DataRow newRow = result.NewRow();

                if (!string.IsNullOrWhiteSpace(item.WORDERID))
                {
                    newRow["WORDERID"] = item.WORDERID;
                }
                else
                {
                    newRow["WORDERID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.TASKID))
                {
                    newRow["TASKID"] = item.TASKID;
                }
                else
                {
                    newRow["TASKID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.TASKDESC))
                {
                    newRow["TASKDESC"] = item.TASKDESC;
                }
                else
                {
                    newRow["TASKDESC"] = DBNull.Value;
                }

                if (item.TPLSTARTDATE.HasValue)
                {
                    newRow["TPLSTARTDATE"] = item.TPLSTARTDATE;
                }
                else
                {
                    newRow["TPLSTARTDATE"] = DBNull.Value;
                }

                if (item.TPLFINISHDATE.HasValue)
                {
                    newRow["TPLFINISHDATE"] = item.TPLFINISHDATE;
                }
                else
                {
                    newRow["TPLFINISHDATE"] = DBNull.Value;
                }

                if (item.TACSTARTDATE.HasValue)
                {
                    newRow["TACSTARTDATE"] = item.TACSTARTDATE;
                }
                else
                {
                    newRow["TACSTARTDATE"] = DBNull.Value;
                }

                if (item.TACFINISHDATE.HasValue)
                {
                    newRow["TACFINISHDATE"] = item.TACFINISHDATE;
                }
                else
                {
                    newRow["TACFINISHDATE"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.WTYPEID))
                {
                    newRow["WTYPEID"] = item.WTYPEID;
                }
                else
                {
                    newRow["WTYPEID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.ASSETID))
                {
                    newRow["ASSETID"] = item.ASSETID;
                }
                else
                {
                    newRow["ASSETID"] = DBNull.Value;
                }

                if (item.JobId.HasValue)
                {
                    newRow["JobId"] = item.JobId;
                }
                else
                {
                    newRow["JobId"] = DBNull.Value;
                }

                result.Rows.Add(newRow);
            }

            return result;
        }

        private DataTable CreateCompanyList(int jobId, DateTime currentDate)
        {
            var datas = _pmisUnitOfWork.PmisRepository<S_COMPANY>().GetQuery(x => x.COMPID != null).ToList();

            DataTable result = new DataTable();
            result.Columns.Add("COMPID", typeof(string));
            result.Columns.Add("COMPDESC", typeof(string));
            result.Columns.Add("ADDRESS", typeof(string));
            result.Columns.Add("EMAIL", typeof(string));
            result.Columns.Add("TEL", typeof(string));
            result.Columns.Add("FAX", typeof(string));
            result.Columns.Add("NOTE", typeof(string));
            result.Columns.Add("COMPID_PARENT", typeof(string));
            result.Columns.Add("ORGID", typeof(string));
            result.Columns.Add("TYPEIDLIST", typeof(string));
            result.Columns.Add("HOMEPAGE", typeof(string));
            result.Columns.Add("USER_CREATE_ID", typeof(string));
            result.Columns.Add("USER_CREATE_DTIME", typeof(DateTime));
            result.Columns.Add("USER_MODIFY_ID", typeof(string));
            result.Columns.Add("USER_MODIFY_DTIME", typeof(DateTime));
            result.Columns.Add("ACTIVE", typeof(bool));
            result.Columns.Add("MUCDICH_SD_TROXI", typeof(string));
            result.Columns.Add("JobId", typeof(int));
            result.Columns.Add("IsDeleted", typeof(bool));
            result.Columns.Add("InsertedAt", typeof(DateTime));
            result.Columns.Add("UpdatedAt", typeof(DateTime));

            foreach (var item in datas)
            {
                DataRow newRow = result.NewRow();

                newRow["COMPID"] = item.COMPID;
                newRow["COMPDESC"] = item.COMPDESC;

                if (!string.IsNullOrWhiteSpace(item.ADDRESS))
                {
                    newRow["ADDRESS"] = item.ADDRESS;
                }
                else
                {
                    newRow["ADDRESS"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.EMAIL))
                {
                    newRow["EMAIL"] = item.EMAIL;
                }
                else
                {
                    newRow["EMAIL"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.TEL))
                {
                    newRow["TEL"] = item.TEL;
                }
                else
                {
                    newRow["TEL"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.FAX))
                {
                    newRow["FAX"] = item.FAX;
                }
                else
                {
                    newRow["FAX"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.NOTE))
                {
                    newRow["NOTE"] = item.NOTE;
                }
                else
                {
                    newRow["NOTE"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.COMPID_PARENT))
                {
                    newRow["COMPID_PARENT"] = item.COMPID_PARENT;
                }
                else
                {
                    newRow["COMPID_PARENT"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.ORGID))
                {
                    newRow["ORGID"] = item.ORGID;
                }
                else
                {
                    newRow["ORGID"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.TYPEIDLIST))
                {
                    newRow["TYPEIDLIST"] = item.TYPEIDLIST;
                }
                else
                {
                    newRow["TYPEIDLIST"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.HOMEPAGE))
                {
                    newRow["HOMEPAGE"] = item.HOMEPAGE;
                }
                else
                {
                    newRow["HOMEPAGE"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USER_CREATE_ID))
                {
                    newRow["USER_CREATE_ID"] = item.USER_CREATE_ID;
                }
                else
                {
                    newRow["USER_CREATE_ID"] = DBNull.Value;
                }

                if (item.USER_CREATE_DTIME.HasValue)
                {
                    newRow["USER_CREATE_DTIME"] = item.USER_CREATE_DTIME;
                }
                else
                {
                    newRow["USER_CREATE_DTIME"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USER_MODIFY_ID))
                {
                    newRow["USER_MODIFY_ID"] = item.USER_MODIFY_ID;
                }
                else
                {
                    newRow["USER_MODIFY_ID"] = DBNull.Value;
                }

                if (item.USER_MODIFY_DTIME.HasValue)
                {
                    newRow["USER_MODIFY_DTIME"] = item.USER_MODIFY_DTIME;
                }
                else
                {
                    newRow["USER_MODIFY_DTIME"] = DBNull.Value;
                }

                if (item.ACTIVE.HasValue)
                {
                    newRow["ACTIVE"] = item.ACTIVE;
                }
                else
                {
                    newRow["ACTIVE"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.MUCDICH_SD_TROXI))
                {
                    newRow["MUCDICH_SD_TROXI"] = item.MUCDICH_SD_TROXI;
                }
                else
                {
                    newRow["MUCDICH_SD_TROXI"] = DBNull.Value;
                }

                newRow["JobId"] = jobId;
                newRow["IsDeleted"] = false;
                newRow["InsertedAt"] = currentDate;
                newRow["UpdatedAt"] = currentDate;

                result.Rows.Add(newRow);
            }

            return result;
        }

        private DataTable CreateCountryList(int jobId, DateTime currentDate)
        {
            var datas = _pmisUnitOfWork.PmisRepository<G_ERP_COUNTRY>().GetQuery(x => x.COUNTRY_CODE != null).ToList();

            DataTable result = new DataTable();
            result.Columns.Add("ID", typeof(string));
            result.Columns.Add("COUNTRY_CODE", typeof(string));
            result.Columns.Add("COUNTRY_NAME", typeof(string));
            result.Columns.Add("JobId", typeof(int));
            result.Columns.Add("IsDeleted", typeof(bool));
            result.Columns.Add("InsertedAt", typeof(DateTime));
            result.Columns.Add("UpdatedAt", typeof(DateTime));

            foreach (var item in datas)
            {
                DataRow newRow = result.NewRow();

                newRow["ID"] = item.ID;
                newRow["COUNTRY_CODE"] = item.COUNTRY_CODE;

                if (!string.IsNullOrWhiteSpace(item.COUNTRY_NAME))
                {
                    newRow["COUNTRY_NAME"] = item.COUNTRY_NAME;
                }
                else
                {
                    newRow["COUNTRY_NAME"] = DBNull.Value;
                }

                newRow["JobId"] = jobId;
                newRow["IsDeleted"] = false;
                newRow["InsertedAt"] = currentDate;
                newRow["UpdatedAt"] = currentDate;

                result.Rows.Add(newRow);
            }

            return result;
        }

        private DataTable CreateCategoryList(int jobId)
        {
            var datas = _pmisUnitOfWork.PmisRepository<A_LST_CATEGORY>().GetQuery().ToList();

            DataTable result = new DataTable();
            result.Columns.Add("CATEGORYID", typeof(string));
            result.Columns.Add("CATEGORYDESC", typeof(string));
            result.Columns.Add("CATEGORYID_PARENT", typeof(string));
            result.Columns.Add("ORGID", typeof(string));
            result.Columns.Add("CATEGORYORD", typeof(int));
            result.Columns.Add("ASSETIDPREFIX", typeof(string));
            result.Columns.Add("USER_CR_ID", typeof(string));
            result.Columns.Add("USER_CR_DTIME", typeof(DateTime));
            result.Columns.Add("USER_MDF_ID", typeof(string));
            result.Columns.Add("USER_MDF_DTIME", typeof(DateTime));
            result.Columns.Add("JobId", typeof(int));
            result.Columns.Add("IsDeleted", typeof(bool));
            result.Columns.Add("InsertedAt", typeof(DateTime));
            result.Columns.Add("UpdatedAt", typeof(DateTime));

            foreach (var item in datas)
            {
                DataRow newRow = result.NewRow();

                newRow["CATEGORYID"] = item.CATEGORYID;
                newRow["CATEGORYDESC"] = item.CATEGORYDESC;

                if (!string.IsNullOrWhiteSpace(item.CATEGORYID_PARENT))
                {
                    newRow["CATEGORYID_PARENT"] = item.CATEGORYID_PARENT;
                }
                else
                {
                    newRow["CATEGORYID_PARENT"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.ORGID))
                {
                    newRow["ORGID"] = item.ORGID;
                }
                else
                {
                    newRow["ORGID"] = DBNull.Value;
                }

                if (item.CATEGORYORD.HasValue)
                {
                    newRow["CATEGORYORD"] = item.CATEGORYORD;
                }
                else
                {
                    newRow["CATEGORYORD"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.ASSETIDPREFIX))
                {
                    newRow["ASSETIDPREFIX"] = item.ASSETIDPREFIX;
                }
                else
                {
                    newRow["ASSETIDPREFIX"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USER_CR_ID))
                {
                    newRow["USER_CR_ID"] = item.USER_CR_ID;
                }
                else
                {
                    newRow["USER_CR_ID"] = DBNull.Value;
                }

                if (item.USER_CR_DTIME.HasValue)
                {
                    newRow["USER_CR_DTIME"] = item.USER_CR_DTIME;
                }
                else
                {
                    newRow["USER_CR_DTIME"] = DBNull.Value;
                }

                if (!string.IsNullOrWhiteSpace(item.USER_MDF_ID))
                {
                    newRow["USER_MDF_ID"] = item.USER_MDF_ID;
                }
                else
                {
                    newRow["USER_MDF_ID"] = DBNull.Value;
                }

                if (item.USER_MDF_DTIME.HasValue)
                {
                    newRow["USER_MDF_DTIME"] = item.USER_MDF_DTIME;
                }
                else
                {
                    newRow["USER_MDF_DTIME"] = DBNull.Value;
                }

                newRow["JobId"] = jobId;
                newRow["IsDeleted"] = false;
                newRow["InsertedAt"] = DateTime.Now;
                newRow["UpdatedAt"] = DateTime.Now;

                result.Rows.Add(newRow);
            }

            return result;
        }

    }
}
