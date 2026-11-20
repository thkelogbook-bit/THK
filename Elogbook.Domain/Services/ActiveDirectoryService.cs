using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models.Request.ActiveDirectory;
using Elogbook.Core.Models.Response.ActiveDirectory;
using Elogbook.Data;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Triển khai các phương thức cấu hình Active Directory
    /// </summary>
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        #region Properties field

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public ActiveDirectoryService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public ActiveDirectoryResponseModel GetActiveDirectoryInfo()
        {
            return _unitOfWork.ElogbookRepository<ActiveDirectory>().GetQuery().Select(
                        e => new ActiveDirectoryResponseModel()
                        {
                            ID = e.ID,
                            IsEnableActiveDirectory = e.IsEnableActiveDirectory,
                            IPAddress = e.IPAddress,
                            DomainController = e.DomainController,
                            OrganizationalUnit = e.OrganizationalUnit,
                        }).FirstOrDefault();
        }

        public ActiveDirectoryResponseModel Save(ActiveDirectoryRequestModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<ActiveDirectory>().GetQuery(e => !e.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.IsEnableActiveDirectory = requestModel.IsEnableActiveDirectory;
                entity.IPAddress = requestModel.IPAddress;
                entity.DomainController = requestModel.DomainController;
                entity.OrganizationalUnit = requestModel.OrganizationalUnit;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<ActiveDirectory>().Update(entity);
            }
            else
            {
                entity = new ActiveDirectory
                {
                    IsEnableActiveDirectory = requestModel.IsEnableActiveDirectory,
                    IPAddress = requestModel.IPAddress,
                    DomainController = requestModel.DomainController,
                    OrganizationalUnit = requestModel.OrganizationalUnit,
                    CreatedUserID = requestModel.CurrentUserID
                };

                _unitOfWork.ElogbookRepository<ActiveDirectory>().Add(entity);
            }
            _unitOfWork.Complete();

            return GetActiveDirectoryInfo();
        }
    }
}
