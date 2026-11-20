using Core.System.Runtime.Constants;
using Core.System.Runtime.Managers;
using Elogbook.Core.Constants;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.PMIS;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;

namespace Elogbook.Domain.Services
{
    public class PmisService : IPmisService
    {
        private readonly string pmisSyncConnection = "Database={0}; Server={1}; user id={2}; password={3}; MultipleActiveResultSets=True; Connect Timeout = 1";
        private readonly PmisUnitOfWork _pmisUnitOfWork;
        private readonly PmisSyncUnitOfWork _pmisSyncUnitOfWork;
        private readonly ElogbookUnitOfWork _unitOfWork;

        public PmisService(PmisUnitOfWork pmisUnitOfWork, PmisSyncUnitOfWork pmisSyncUnitOfWork, ElogbookUnitOfWork unitOfWork)
        {
            _pmisUnitOfWork = pmisUnitOfWork;
            _pmisSyncUnitOfWork = pmisSyncUnitOfWork;
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetParameters(SearchParameterModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => !x.IsDeleted);
            // Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            // Chu kỳ
            if (requestModel.CycelID > 0)
            {
                entities = entities.Where(x => x.CycleID == requestModel.CycelID);
            }
            // Cương vị
            if (requestModel.OperationPositionID > 0)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }

            var page = entities.Select(
                e => new ParameterModel()
                {
                    ID = e.ID,
                    ParameterName = e.ParameterName,
                    KKSCode = e.KKSCode,
                    UnitMeasure = e.UnitMeasure,
                    QLKTCode = e.QLKTCode,
                    Position = e.Position,
                })
                .OrderBy(t => t.Position)
                .ThenBy(t => t.ID)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public ResponseBaseModel GetParameterPmiss(SearchParameterPmisModel requestModel)
        {
            var entities = (from dt in _pmisUnitOfWork.PmisRepository<OP_METER_TAB_DT>().GetQuery(x => x.OPTABID == requestModel.OPTABID)
                            join dy in _pmisUnitOfWork.PmisRepository<A_ASSET_METER_DYN>().GetQuery() on dt.ADMETERID equals dy.ADMETERID
                            join mt in _pmisUnitOfWork.PmisRepository<A_METER>().GetQuery() on dy.METERID equals mt.METERID
                            select new ParameterInfoModel()
                            {
                                ADMETERID = dt.ADMETERID,
                                METERDESC = mt.METERDESC,
                                UOMID = mt.UOMID,
                                ROWORD = dt.ROWORD
                            }).OrderBy(t => t.ROWORD)
                        .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(entities);
        }

        public List<SelectboxInfoModel> GetParameterGroups(bool isHasAll)
        {
            var parameterGroups = _pmisUnitOfWork.PmisRepository<OP_METER_TAB>().GetQuery(x => x.ENABLE == true)
                    .Select(
                        e => new SelectboxInfoModel()
                        {
                            Value = e.OPTABID,
                            Label = e.OPTABDESC
                        }).OrderBy(t => t.Label).ToList();

            if (isHasAll)
            {
                var list = new List<SelectboxInfoModel>
                            {
                                new SelectboxInfoModel { Value = string.Empty, Label = CommonConstant.ALL_VALUE },
                            };
                list.AddRange(parameterGroups);

                return list;
            }
            else
            {
                return parameterGroups;
            }
        }

        public bool UpdateParameter(AddEditParameterModel requestModel)
        {
            var currentEntity = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (currentEntity != null)
            {
                currentEntity.QLKTCode = requestModel.QLKTCode;
                currentEntity.UpdatedAt = DateTime.Now;
                currentEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                _unitOfWork.ElogbookRepository<Parameter>().Update(currentEntity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool TestConnectionDatabaseSyncPMIS(DatabaseConnectionInfoModel requestModel)
        {
            var fullPmisSyncConnection = string.Format(pmisSyncConnection, requestModel.DatabaseName, requestModel.ServerName, requestModel.UserName, requestModel.Password);
            using (var conn = new SqlConnection(fullPmisSyncConnection))
            {
                conn.Open();
                conn.Close();
            }

            return true;
        }

        public bool SaveConnectionDatabasePMIS(DatabaseConnectionInfoModel requestModel)
        {
            var result = false;

            XmlDocument parentXmlDocument = new XmlDocument();
            //Load file Web.config of current location deploy
            parentXmlDocument.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            foreach (XmlElement parentXmlElement in parentXmlDocument.DocumentElement)
            {
                if (result) break;
                //Check config file has tag "connectionStrings"
                if ("connectionStrings".Equals(parentXmlElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    //Get value of "configSource" attribute
                    var configSource = parentXmlElement.Attributes["configSource"].Value;

                    XmlDocument childXmlDocument = new XmlDocument();
                    //Load external file config setting
                    childXmlDocument.Load(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + configSource);
                    var isEncrypt = false;
                    foreach (XmlElement childXmlElement in childXmlDocument.DocumentElement)
                    {
                        //Check whether the ElogbookConnection connection string is encrypted
                        if ("add".Equals(childXmlElement.Name, StringComparison.OrdinalIgnoreCase)
                            && "ElogbookConnection".Equals(childXmlElement.Attributes["name"].Value, StringComparison.OrdinalIgnoreCase))
                        {
                            isEncrypt = requestModel.Password.StartsWith(RuntimeConstants.Password_Prefix, StringComparison.OrdinalIgnoreCase);
                        }

                        //Check file has tag "add" and attribute "name" has value "PmisSyncConnection"
                        if ("add".Equals(childXmlElement.Name, StringComparison.OrdinalIgnoreCase)
                        && "PmisSyncConnection".Equals(childXmlElement.Attributes["name"].Value, StringComparison.OrdinalIgnoreCase))
                        {
                            //If isEncrypt is true, encrypt the password; otherwise, leave it unchanged
                            var password = isEncrypt ? RuntimeManager.Encrypt(requestModel.Password, true) : requestModel.Password;
                            result = true;
                            //Update value attribute "connectionString" and break loop
                            childXmlElement.Attributes["connectionString"].Value = $"Data Source={requestModel.ServerName};Initial Catalog={requestModel.DatabaseName};user id={requestModel.UserName};password={password};persist security info=True;MultipleActiveResultSets=True;";
                            break;
                        }
                    }
                    //Save new config file
                    childXmlDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + parentXmlElement.Attributes["configSource"].Value);
                }
            }

            return result;
        }

        public DatabaseConnectionInfoModel LoadConnectionDatabasePMIS()
        {
            DatabaseConnectionInfoModel databaseConnectionInfoModel = new DatabaseConnectionInfoModel();

            ConnectionStringSettingsCollection cStringsSettings = ConfigurationManager.ConnectionStrings;
            if (cStringsSettings != null)
            {
                foreach (var sqlConnectionStringBuilder in from ConnectionStringSettings cstrings in cStringsSettings
                                                           where "PmisSyncConnection".Equals(cstrings.Name, StringComparison.OrdinalIgnoreCase)
                                                           let sqlConnectionStringBuilder = new SqlConnectionStringBuilder(cstrings.ConnectionString)
                                                           select sqlConnectionStringBuilder)
                {
                    databaseConnectionInfoModel.ServerName = sqlConnectionStringBuilder.DataSource;
                    databaseConnectionInfoModel.DatabaseName = sqlConnectionStringBuilder.InitialCatalog;
                    databaseConnectionInfoModel.UserName = sqlConnectionStringBuilder.UserID;
                    databaseConnectionInfoModel.Password = sqlConnectionStringBuilder.Password;
                }
            }

            return databaseConnectionInfoModel;
        }

        public int AutoCreateTableByYear()
        {
            var sqlQuery = @"DECLARE @sql_create_table NVARCHAR(MAX) = '',
                            @params NVARCHAR(MAX) = '',
		                    @tableName NVARCHAR(50) = 'ZOP_METER',
		                    @tableKey NVARCHAR(50) = ''

                        SET @tableName = @tableName + CAST(YEAR(GETDATE()) AS VARCHAR(10))
                        SET @tableKey = @tableName + '_IDPK'

                        IF OBJECT_ID(@tableName) IS NULL
                        BEGIN
                            --build query tạo table
                            SET @sql_create_table = 'CREATE TABLE ' + @tableName + '([AMETERID] [NVARCHAR](50) NOT NULL,
                                                        [DTIME][DATETIME] NOT NULL,
                                                        [VAL_S] [NVARCHAR](255) NOT NULL,
                                                        [READMETHODID] [NVARCHAR](25) NOT NULL,
                                                        [USERID] [NVARCHAR](25) NULL,
	                                                    [USERDTIME] [DATETIME] NULL,
	                                                    [DELTA_VAL] [NVARCHAR](255) NULL,
	                                                    [FAILURE] [INT] NULL,
	                                                CONSTRAINT['+ @tableKey +'] PRIMARY KEY CLUSTERED
                                                   (
                                                       [AMETERID] ASC,
		                                                [DTIME] ASC
	                                                )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON) ON[PRIMARY]
	                                                ) ON[PRIMARY]'
                        END

                        --Kiểm tra nếu chưa tồn tại table PMIS theo năm thì tạo mới table
                        IF(ISNULL(@sql_create_table, '') != '')
                        BEGIN
                            EXECUTE sp_executesql @sql_create_table
                        END";

            return _pmisSyncUnitOfWork.DataContext.Database.ExecuteSqlCommand(sqlQuery);
        }

        public void SycnDataForPMIS(int timeGetLatestPramaterUpdated = -2)
        {
            //Config param time get latest parameters updated
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("Hour", System.Data.SqlDbType.Int)
                {
                    Value = timeGetLatestPramaterUpdated,
                },
            };
            //Get list parameters updated
            var parametersPmisModel = _unitOfWork.DataContext.Database.SqlQuery<ParametersPmisModel>("TSVH_GetDataForPMIS @Hour", sqlParams).ToList();

            if (parametersPmisModel.Count > 0)
            {
                //Mapping final list import to PMIS
                List<ParametersPmisModel> finalResults = new List<ParametersPmisModel>();
                foreach (var item in parametersPmisModel)
                {
                    //Check record exist in list then not append
                    if (finalResults.Exists(x => x.AMETERID == item.AMETERID && x.DTIME == item.DTIME))
                    {
                        continue;
                    }

                    //Append item to list
                    finalResults.Add(new ParametersPmisModel
                    {
                        RowIndex = item.RowIndex,
                        AMETERID = item.AMETERID,
                        DTIME = item.DTIME,
                        VAL_S = item.VAL_S,
                        READMETHODID = item.READMETHODID,
                        USERID = item.USERID,
                    });
                }

                if (finalResults.Count > 0)
                {
                    StringBuilder sqlFull = new StringBuilder();
                    var tableName = $"ZOP_METER" + DateTime.Now.Year;
                    foreach (var item in finalResults)
                    {
                        var sqlExist = $"SELECT pmis.AMETERID FROM dbo.{tableName} pmis WITH(NOLOCK) WHERE pmis.AMETERID = N'{item.AMETERID}' AND pmis.DTIME = N'{item.DTIME}'";
                        var isExist = _pmisSyncUnitOfWork.DataContext.Database.SqlQuery<string>(sqlExist).SingleOrDefault() != null;
                        //Check exist record on PMIS then update data
                        if (isExist)
                        {
                            var sqlUpdate = "\n" + $"UPDATE dbo.{tableName} SET VAL_S = N'{item.VAL_S}', USERDTIME = GETDATE() WHERE AMETERID = N'{item.AMETERID}' AND DTIME = N'{item.DTIME}'";
                            sqlFull.Append(sqlUpdate);
                        }
                        //If record not exist then insert
                        else
                        {
                            var sqlInsert = "\n" + $"INSERT INTO dbo.{tableName}(AMETERID, DTIME, VAL_S, READMETHODID, USERID, USERDTIME, DELTA_VAL, FAILURE) VALUES(N'{item.AMETERID}', N'{item.DTIME}', N'{item.VAL_S}', 'MANUAL', N'{item.USERID}', GETDATE(), NULL, 0)";
                            sqlFull.Append(sqlInsert);
                        }
                    }

                    //Execute Insert/Update for final query
                    _pmisSyncUnitOfWork.DataContext.Database.ExecuteSqlCommand(sqlFull.ToString());
                }
            }
        }
    }

}
