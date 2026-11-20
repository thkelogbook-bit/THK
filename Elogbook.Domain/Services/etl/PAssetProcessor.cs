using Elogbook.Core.Models.ETL;
using Elogbook.Data;

namespace Elogbook.Domain.Services.etl
{
    public class PAssetProcessor : Processor<A_ASSET, PMIS_A_ASSET>
    {
        #region Private fields

        #endregion

        #region Contructor

        public PAssetProcessor()
        {
        }

        #endregion

        #region public

        public override PMIS_A_ASSET Process(A_ASSET _asset, int jobId)
        {
            PMIS_A_ASSET asset = new PMIS_A_ASSET
            {
                ASSETID = _asset.ASSETID,
                ASSETID_PARENT = _asset.ASSETID_PARENT,
                ASSETID_ORG = _asset.ASSETID_ORG,
                ASSETDESC = _asset.ASSETDESC,
                ASSETORD = _asset.ASSETORD,
                SITEID = _asset.SITEID,
                CATEGORYID = _asset.CATEGORYID,
                TYPEID = _asset.TYPEID,
                MANDEPTIDLIST = _asset.MANDEPTIDLIST,
                USESTATUS_LAST_ID = _asset.USESTATUS_LAST_ID,
                USESTATUS_LAST_DTIME = _asset.USESTATUS_LAST_DTIME,
                SERIALNUM = _asset.SERIALNUM,
                P_MANUFACTURERID = _asset.P_MANUFACTURERID,
                P_VENDORID = _asset.P_VENDORID,
                P_INSTALLDATE = _asset.P_INSTALLDATE,
                P_PRICE = _asset.P_PRICE,
                P_PRICEUOM = _asset.P_PRICEUOM,
                P_PRICE_INC_ALL_CHILD = _asset.P_PRICE_INC_ALL_CHILD,
                PRIORITY = _asset.PRIORITY,
                USER_CR_ID = _asset.USER_CR_ID,
                USER_CR_DTIME = _asset.USER_CR_DTIME,
                USER_MDF_ID = _asset.USER_MDF_ID,
                USER_MDF_DTIME = _asset.USER_MDF_DTIME,
                USESTATUS_UPDATE_ID = _asset.USESTATUS_UPDATE_ID,
                ASSETIDSORT = _asset.ASSETIDSORT,
                CHILDCOUNT = _asset.CHILDCOUNT,
                COUNTRY_ID = _asset.COUNTRY_ID,
                ISFIX = _asset.ISFIX,
                P_OPERATIONDATE = _asset.P_OPERATIONDATE,
                NONE_ATTACH = _asset.NONE_ATTACH,
                NONE_MATERIAL = _asset.NONE_MATERIAL,
                NONE_METER_DYN = _asset.NONE_METER_DYN,
                NONE_SAFE = _asset.NONE_SAFE,
                P_MAN_YEAR = _asset.P_MAN_YEAR,
                CHILD_MATERIAL = _asset.CHILD_MATERIAL,
                CHILD_METER_DYN = _asset.CHILD_METER_DYN,
                ISDEL = _asset.ISDEL,
                USER_DEL_DTIME = _asset.USER_DEL_DTIME,
                USER_DEL_ID = _asset.USER_DEL_ID,
                JobId = jobId
            };

            return asset;
        }

        public override PMIS_A_ASSET Process(A_ASSET _asset)
        {
            return null;
        }

        #endregion
    }
}
