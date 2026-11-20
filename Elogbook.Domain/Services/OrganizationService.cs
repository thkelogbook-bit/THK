using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Quản lý Sơ đồ tổ chức
    /// </summary>
    public class OrganizationService : IOrganizationService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonService"></param>
        public OrganizationService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        /// <summary>
        /// Tìm kiếm Sơ đồ tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public List<OrganizationModel> GetOrganizations(SearchOrganizationModel requestModel)
        {
            List<Organization> data = new List<Organization>();
            // Lấy toàn bộ dữ liệu theo cương vị
            var entities = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && (string.IsNullOrEmpty(requestModel.Name) || x.Name.Contains(requestModel.Name))
                                                                                    && (requestModel.Active == CommonConstant.ALL_KEY || x.Active == requestModel.Active))
                .OrderBy(x => x.Name).ToList();

            foreach (var item in entities)
            {
                data.Add(item);
                //level nhóm
                if (item.Parent != null)
                {
                    data.Add(item.Parent);

                    //level phân xưởng
                    if (item.Parent.Parent != null)
                    {
                        data.Add(item.Parent.Parent);

                        //level công ty
                        if (item.Parent.Parent.Parent != null)
                        {
                            data.Add(item.Parent.Parent.Parent);
                        }
                    }
                }
            }
            // xoá data trùng
            data = data.GroupBy(x => x.ID).Select(x => x.First()).ToList();

            // Lấy dữ liệu công ty
            var companys = AppendOrganization(EnumExtensions.GetEnumDescription(OrganizationType.CT), 0, data);
            foreach (var company in companys)
            {
                // Lấy dữ liệu phân xưởng
                company.Children = AppendOrganization(EnumExtensions.GetEnumDescription(OrganizationType.PX), company.Data.ID, data);
                foreach (var productionPlant in company.Children)
                {
                    // Lấy dữ liệu tổ
                    productionPlant.Children = AppendOrganization(EnumExtensions.GetEnumDescription(OrganizationType.TO), productionPlant.Data.ID, data);
                    foreach (var team in productionPlant.Children)
                    {
                        // Lấy dữ liệu đội/nhóm
                        team.Children = AppendOrganization(EnumExtensions.GetEnumDescription(OrganizationType.DN), team.Data.ID, data);
                    }
                }
            }

            return companys;
        }

        /// <summary>
        /// Thêm mới Sơ đồ tổ chức
        /// </summary>
        /// <param name="level"></param>
        /// <param name="parrentID"></param>
        /// <param name="organizations"></param>
        /// <returns></returns>
        private List<OrganizationModel> AppendOrganization(string level, int? parrentID, List<Organization> organizations)
        {
            return organizations.Where(x => x.Level == level && (parrentID == 0 || x.ParentID == parrentID || (EnumExtensions.GetEnumDescription(OrganizationType.CT).Equals(x.Level, System.StringComparison.OrdinalIgnoreCase)
                             && x.ParentID == null))).Select(
                    e => new OrganizationModel()
                    {
                        Data = new OrganizationParrentModel()
                        {
                            ID = e.ID,
                            ParentID = parrentID > 0 ? parrentID : null,
                            Name = e.Name,
                            ShortName = e.ShortName,
                            Level = e.Level,
                            CompanyType = e.CompanyType,
                            ProductionPlantType = e.ProductionPlantType,
                            Active = e.Active,
                            FileAttachment = e.AttachmentID != null && e.AttachmentFile != null ? new AttachmentModel()
                            {
                                FilePath = _commonService.GetPathFile(e.AttachmentFile.FilePath),
                                PathThumbnail = _commonService.GetPathFile(e.AttachmentFile.PathThumbnail),
                                ID = e.AttachmentFile.ID
                            } : null
                        },
                        Children = new List<OrganizationModel>()
                    }).ToList();
        }

        /// <summary>
        /// Thêm mới Sơ đồ tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public int AddOrganization(AddEditOrganizationModel requestModel)
        {
            var entity = new Organization()
            {
                // Với các tổ chức Phân xưởng, Tổ, Đội/nhóm thì cần gán ParentID để xác định level cha của nó
                ParentID = requestModel.Level != EnumExtensions.GetEnumDescription(OrganizationType.CT) ? requestModel.ParentID : null,
                Name = requestModel.Name,
                ShortName = requestModel.ShortName,
                Level = requestModel.Level,
                CompanyType = requestModel.Level == EnumExtensions.GetEnumDescription(OrganizationType.CT) ? requestModel.CompanyType : string.Empty,
                ProductionPlantType = requestModel.Level == EnumExtensions.GetEnumDescription(OrganizationType.PX) ? requestModel.ProductionPlantType : string.Empty,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
                AttachmentID = requestModel.AttachmentID
            };

            _unitOfWork.ElogbookRepository<Organization>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                return entity.ID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Cập nhật Sơ đồ tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool UpdateOrganization(AddEditOrganizationModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                // Danh sách các tổ chức cần thay đổi trạng thái
                List<int> ids = new List<int>();
                // Nếu chuyển trạng thái sang Không áp dụng thì cập nhật tất cả level con
                // Nếu chuyển trạng thái sang Áp dụng thì việc chuyển các level con phụ thuộc vào yêu cầu của người dùng
                if (requestModel.Active == 2) requestModel.IsUpdateChildLevel = true;

                ids = GetOrganizationDeleteOrChangeStatus(entity, requestModel.IsUpdateChildLevel);
                var entitys = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => ids.Contains(x.ID)).ToList();
                foreach (var item in entitys)
                {
                    item.LastUpdatedUserID = requestModel.CurrentUserID;
                    // Parrent level
                    if (item.ID == requestModel.ID)
                    {
                        item.Name = requestModel.Name;
                        item.ShortName = requestModel.ShortName;
                        item.CompanyType = requestModel.Level == EnumExtensions.GetEnumDescription(OrganizationType.CT) ? requestModel.CompanyType : string.Empty;
                        item.ProductionPlantType = requestModel.Level == EnumExtensions.GetEnumDescription(OrganizationType.PX) ? requestModel.ProductionPlantType : string.Empty;
                        item.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;

                        if (item.AttachmentID != null && (item.AttachmentID != requestModel.AttachmentID))
                        {
                            DeleteFileAttachment((int)item.AttachmentID, requestModel.CurrentUserID);
                        }
                        item.AttachmentID = requestModel.AttachmentID;
                    }
                    // Child level
                    else
                    {
                        item.Active = requestModel.Active;
                    }

                    _unitOfWork.ElogbookRepository<Organization>().Update(item);
                }

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        /// <summary>
        /// Xóa Sơ đồ tổ chức
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UpdateStatus DeleteOrganization(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                // Danh sách các tổ chức cần xóa
                List<int> ids = new List<int>();
                ids = GetOrganizationDeleteOrChangeStatus(entity, true);

                if (CheckInUseOrganization(ids))
                {
                    return UpdateStatus.InUse;
                }
                else
                {
                    var entitys = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => ids.Contains(x.ID)).ToList();
                    _unitOfWork.ElogbookRepository<Organization>().Delete(entitys, userID);

                    // Xoá file
                    foreach (var item in
                        from item in entitys
                        where item.AttachmentID != null
                        select item)
                    {
                        DeleteFileAttachment((int)item.AttachmentID, userID);
                    }

                    if (_unitOfWork.Complete() > 0)
                        return UpdateStatus.Success;
                    else
                        return UpdateStatus.Error;
                }
            }

            return UpdateStatus.NotFound;
        }

        /// <summary>
        /// Xóa Sơ đồ tổ chức
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        private void DeleteFileAttachment(int id, int userID)
        {
            var entityFile = _unitOfWork.ElogbookRepository<Attachment>().GetById((id));
            if (entityFile != null)
            {
                // kiểm tra file đã tồn tại chưa và xoá
                var filePath = Path.Combine(AppSettings.SERVER_DIR, entityFile.FilePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // kiểm tra file Thumbnail đã tồn tại chưa và xoá
                var filePathThumbnail = Path.Combine(AppSettings.SERVER_DIR, entityFile.PathThumbnail);
                if (File.Exists(filePathThumbnail))
                {
                    File.Delete(filePathThumbnail);
                }

                _unitOfWork.ElogbookRepository<Attachment>().Delete(entityFile, userID);
            }
        }

        /// <summary>
        /// Kiểm tra Sơ đồ tổ chức
        /// </summary>
        /// <param name="level"></param>
        /// <param name="name"></param>
        /// <param name="shortName"></param>
        /// <param name="id"></param>
        /// <param name="parrentID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        public bool CheckDuplicateData(string level, string name, string shortName, int id, int? parrentID, bool isAddMode)
        {
            // Trường hợp thêm mới
            if (isAddMode)
            {
                var entities = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => (x.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase)
                                                                                    || x.ShortName.Equals(shortName, System.StringComparison.OrdinalIgnoreCase))
                                                                                    && level.Equals(x.Level, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted);
                switch (level.ToUpper())
                {
                    case "CÔNG TY":
                    case "PHÂN XƯỞNG":
                    case "TỔ":
                        return entities.Any();
                    case "ĐỘI/NHÓM":
                        return entities.Where(x => x.ParentID == parrentID).Any();
                }
            }
            // Trường hợp cập nhật
            else
            {
                var entities = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => (x.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase)
                                                                                    || x.ShortName.Equals(shortName, System.StringComparison.OrdinalIgnoreCase))
                                                                                    && level.Equals(x.Level, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && x.ID != id
                                                                                    && !x.IsDeleted);
                switch (level.ToUpper())
                {
                    case "CÔNG TY":
                        return entities.Any();
                    case "PHÂN XƯỞNG":
                    case "TỔ":
                    case "ĐỘI/NHÓM":
                        return entities.Where(x => x.ParentID == parrentID).Any();
                }
            }

            return false;
        }

        /// <summary>
        /// Cập nhật Sơ đồ tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public UpdateStatus ChangeStatusOrganization(ChangeOrganizationStatusModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                // Danh sách các tổ chức cần thay đổi trạng thái
                List<int> ids = new List<int>();
                // Nếu chuyển trạng thái sang Không áp dụng thì cập nhật tất cả level con
                // Nếu chuyển trạng thái sang Áp dụng thì việc chuyển các level con phụ thuộc vào yêu cầu của người dùng
                if (requestModel.Active == 2) requestModel.IsUpdateChildLevel = true;

                ids = GetOrganizationDeleteOrChangeStatus(entity, requestModel.IsUpdateChildLevel);
                var entitys = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => ids.Contains(x.ID)).ToList();
                foreach (var item in entitys)
                {
                    item.Active = requestModel.Active;
                    item.LastUpdatedUserID = requestModel.CurrentUserID;
                    _unitOfWork.ElogbookRepository<Organization>().Update(item);
                }

                if (_unitOfWork.Complete() > 0)
                    return UpdateStatus.Success;
                else
                    return UpdateStatus.Error;
            }

            return UpdateStatus.NotFound;
        }

        /// <summary>
        /// Kiểm tra dữ liệu Sơ đồ tổ chức
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private bool CheckInUseOrganization(List<int> ids)
        {
            // kiểm tra danh sách mã số trong Table User
            var entityUsers = _unitOfWork.ElogbookRepository<User>().GetQuery();
            foreach (var item in entityUsers)
            {
                if (ids.Contains(item.CompanyID) || ids.Contains(item.ProductionPlantID.GetValueOrDefault(0))
                    || (item.GroupID != null && ids.Contains((int)item.GroupID)) || (item.TeamID != null && ids.Contains((int)item.TeamID)))
                {
                    return true;
                }
            }

            // kiểm tra danh sách mã số trong Table phiếu công tác
            var existIdInRegistrationTickets = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => (x.RegistrationUnitID != null && ids.Contains((int)x.RegistrationUnitID))
                                            || ids.Contains(x.ReceiveUnitID)).Any();
            if (existIdInRegistrationTickets)
            {
                return true;
            }

            // kiểm tra danh sách mã số trong Table phiếu công tác
            var existIdInWorkTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ApprovedUnitID != null && ids.Contains((int)x.ApprovedUnitID)).Any();
            if (existIdInWorkTickets)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Trường hợp xóa thì xóa tất cả các level con
        /// Trường hợp thay đổi trạng thái, nếu chuyển sang Không áp dụng thì cũng chuyển tất cả level con
        /// Nếu chuyển sang áp dụng thì phụ thuộc vào yêu cầu của người dùng có muốn chuyển level con hay không
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isUpdateChildLevel"></param>
        /// <returns></returns>
        private List<int> GetOrganizationDeleteOrChangeStatus(Organization entity, bool isUpdateChildLevel)
        {
            // Danh sách các tổ chức cần xóa
            List<int> ids = new List<int>();
            var id = entity.ID;
            switch (entity.Level.Trim().ToUpper())
            {
                case "CÔNG TY":
                    // Công ty
                    ids.Add(id);
                    //Phân xưởng
                    if (isUpdateChildLevel)
                    {
                        List<int> productionPlantIDs = GetListIDOrganization(id);
                        ids.AddRange(productionPlantIDs);
                        foreach (var productionPlantID in productionPlantIDs)
                        {
                            // Tổ
                            List<int> teamID1s = GetListIDOrganization(productionPlantID);
                            ids.AddRange(teamID1s);
                            foreach (var teamID in teamID1s)
                            {
                                // Đội/nhóm
                                List<int> groupID1s = GetListIDOrganization(teamID);
                                ids.AddRange(groupID1s);
                            }
                        }
                    }
                    break;
                case "PHÂN XƯỞNG":
                    // Phân xưởng
                    ids.Add(id);
                    // Tổ
                    if (isUpdateChildLevel)
                    {
                        List<int> teamIDs = GetListIDOrganization(id);
                        ids.AddRange(teamIDs);
                        foreach (var teamID in teamIDs)
                        {
                            // Đội/nhóm
                            List<int> groupID2s = GetListIDOrganization(teamID);
                            ids.AddRange(groupID2s);
                        }
                    }
                    break;
                case "TỔ":
                    // Tổ
                    ids.Add(id);
                    // Đội/nhóm
                    if (isUpdateChildLevel)
                    {
                        List<int> groupIDs = GetListIDOrganization(id);
                        ids.AddRange(groupIDs);
                    }
                    break;
                case "ĐỘI/NHÓM":
                    // Đội/nhóm
                    ids.Add(id);
                    break;
            }

            return ids;
        }

        /// <summary>
        /// Lấy danh sách ID Sơ đồ tổ chức
        /// </summary>
        /// <param name="parrentID"></param>
        /// <returns></returns>
        private List<int> GetListIDOrganization(int parrentID)
        {
            return _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.ParentID == parrentID && !x.IsDeleted).Select(x => x.ID).ToList();
        }
    }
}
