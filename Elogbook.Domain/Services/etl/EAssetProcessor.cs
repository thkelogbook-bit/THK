using Elogbook.Core.Models.ETL;
using Elogbook.Data;
using System.Linq;

namespace Elogbook.Domain.Services.etl
{
    public class EAssetProcessor : Processor<PMIS_A_ASSET, AmDevice>
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public EAssetProcessor(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region public

        public override AmDevice Process(PMIS_A_ASSET _asset, int jobId)
        {
            return null;
        }

        public override AmDevice Process(PMIS_A_ASSET _asset)
        {
            AmDevice asset = _unitOfWork.ElogbookRepository<AmDevice>()
                            .GetQuery(x => x.PMISId == _asset.ASSETID)
                            .FirstOrDefault();

            if (asset == null)
            {
                asset = new AmDevice();
            }
            else
            {
                return null;
            }

            if (_asset.ASSETID_PARENT != null)
            {
                asset.AssetParent = _asset.ASSETID_PARENT;
            }

            AmLocation location = _unitOfWork.ElogbookRepository<AmLocation>()
                            .GetQuery(x => !x.IsDeleted && x.SiteCode.Equals(_asset.SITEID)).FirstOrDefault();
            if (location != null)
            {
                asset.AmLocationID = location.ID;
            }

            AmDeviceType deviceType = null;
            if (_asset.TYPEID != null)
            {
                deviceType = _unitOfWork.ElogbookRepository<AmDeviceType>()
                            .GetQuery(x => !x.IsDeleted && x.PMISId == _asset.TYPEID)
                            .FirstOrDefault();
                if (deviceType != null)
                {
                    asset.AmDeviceTypeID = deviceType.ID;
                }
            }

            AmDeviceStatus status = null;
            if (_asset.ISDEL != null)
            {
                status = _unitOfWork.ElogbookRepository<AmDeviceStatus>()
                            .GetQuery(x => x.StatusDesc == ((_asset.ISDEL.HasValue
                                        && _asset.ISDEL.Value) ? "Tốt" : "Hỏng"))
                            .FirstOrDefault();

                if (status != null)
                {
                    asset.AmDeviceStatusID = status.ID;
                }
            }

            AmDeviceCategory category = null;
            if (_asset.CATEGORYID != null)
            {
                category = _unitOfWork.ElogbookRepository<AmDeviceCategory>()
                            .GetQuery(x => x.PMISId == _asset.CATEGORYID)
                            .FirstOrDefault();

                if (category != null)
                {
                    asset.AmDeviceCategoryID = category.ID;
                }
            }

            AmSupplier supplier = null;
            if (_asset.P_VENDORID != null)
            {
                supplier = _unitOfWork.ElogbookRepository<AmSupplier>()
                            .GetQuery(x => x.PMISId == _asset.P_VENDORID)
                            .FirstOrDefault();

                if (supplier != null)
                {
                    asset.AmSupplierID = supplier.ID;
                }
            }

            AmManufacturer manufacturer = null;
            if (_asset.P_MANUFACTURERID != null)
            {
                manufacturer = _unitOfWork.ElogbookRepository<AmManufacturer>()
                            .GetQuery(x => x.PMISId == _asset.P_MANUFACTURERID)
                            .FirstOrDefault();

                if (manufacturer != null)
                {
                    asset.AmManufacturerID = manufacturer.ID;
                }
            }

            AmCountryManufacture countryManufacture = null;
            if (_asset.COUNTRY_ID != null)
            {
                countryManufacture = _unitOfWork.ElogbookRepository<AmCountryManufacture>()
                            .GetQuery(x => x.PMISId == _asset.COUNTRY_ID.ToString())
                            .FirstOrDefault();

                if (countryManufacture != null)
                {
                    asset.AmCountryManufactureID = countryManufacture.ID;
                }
            }

            asset.PMISId = _asset.ASSETID;
            asset.AssetDesc = _asset.ASSETDESC;
            asset.AssetCode = _asset.ASSETID;
            asset.Khvh = _asset.KHVH;
            asset.Kss = _asset.KKS;
            asset.AssetType = _asset.ASSET_TYPE;
            asset.Meter = _asset.METER;
            asset.Drawing = _asset.DRAWING;
            asset.Note = _asset.NOTE;
            asset.IsDeleted = false;

            return asset;
        }

        #endregion
    }
}
