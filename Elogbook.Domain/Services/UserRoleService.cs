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
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private string _permissions = string.Empty;
        private bool _isAdmin = false;

        public UserRoleService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetUserRoles(RoleRequestModel requestModel)
        {
            // Filter data by condition
            if (requestModel.Active <= 0) requestModel.Active = CommonConstant.ALL_KEY;
            if (requestModel.DataPermission <= 0) requestModel.DataPermission = CommonConstant.ALL_KEY;
            if (string.IsNullOrWhiteSpace(requestModel.RoleName)) requestModel.RoleName = string.Empty;

            var entities = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.RoleName.Contains(requestModel.RoleName)
                                                                                && (requestModel.Active == CommonConstant.ALL_KEY || x.Active == requestModel.Active)
                                                                                && (requestModel.DataPermission == CommonConstant.ALL_KEY || x.DataPermission == requestModel.DataPermission)
                                                                                && !x.IsDeleted);

            // Return and paging data
            var page = entities.Select(
                e => new UserRoleModel()
                {
                    ID = e.ID,
                    RoleName = e.RoleName,
                    Description = e.Description,
                    DataPermission = e.DataPermission,
                    Active = e.Active,
                    InsertedAt = e.InsertedAt,
                    IsAdmin = e.IsAdmin,
                }).OrderByDescending(t => t.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public UserRoleModel GetUserRoleById(int id)
        {
            // Lấy thông tin của Vai trò
            var userRole = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(
                e => new UserRoleModel()
                {
                    ID = e.ID,
                    RoleName = e.RoleName,
                    Description = e.Description,
                    DataPermission = e.DataPermission,
                    Permissions = e.Permissions,
                    Active = e.Active,
                    IsAdmin = e.IsAdmin,
                }).FirstOrDefault();

            // Kiểm tra Vai trò có quyền thì lấy danh sách quyền
            if (userRole != null)
            {
                _isAdmin = userRole.IsAdmin;
                _permissions = userRole.Permissions;
                userRole.UserPermissions = GePermissions();
            }

            return userRole;
        }

        public int AddUserRole(AddEditUserRoleModel requestModel)
        {
            var entity = new UserRole
            {
                RoleName = !string.IsNullOrWhiteSpace(requestModel.RoleName) ? requestModel.RoleName.Trim() : string.Empty,
                Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description.Trim() : string.Empty,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                DataPermission = requestModel.DataPermission <= 0 ? (int)DataPermission.Limited : requestModel.DataPermission,
                Permissions = !string.IsNullOrWhiteSpace(requestModel.Permissions) ? requestModel.Permissions.Trim() : string.Empty,
                //IsAdmin = requestModel.IsAdmin,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };
            _unitOfWork.ElogbookRepository<UserRole>().Add(entity);
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

        public bool UpdateUserRole(AddEditUserRoleModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.RoleName = !string.IsNullOrWhiteSpace(requestModel.RoleName) ? requestModel.RoleName.Trim() : string.Empty;
                entity.Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description.Trim() : string.Empty;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.DataPermission = requestModel.DataPermission <= 0 ? (int)DataPermission.Limited : requestModel.DataPermission;
                entity.Permissions = !string.IsNullOrWhiteSpace(requestModel.Permissions) ? requestModel.Permissions.Trim() : string.Empty;
                //entity.IsAdmin = requestModel.IsAdmin;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<UserRole>().Update(entity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteUserRole(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.ID == id && !x.IsDeleted);

            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<UserRole>().Delete(entity, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool CheckInUseUserRole(int id)
        {
            return _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => x.UserRoleID == id && !x.IsDeleted).Any();
        }

        public bool CheckDuplicateData(int userRoleID, string roleName, bool isAddMode)
        {
            // Trường hợp thêm mới
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase)
                                                                                && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.ID != userRoleID
                                                                                        && x.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase)
                                                                                        && !x.IsDeleted).Any();
            }
        }

        public List<UserPermissionModel> GePermissions()
        {
            // Danh sách tất cả các quyền trong hệ thống
            var permissions = _unitOfWork.ElogbookRepository<UserPermission>().GetQuery(x => x.Active == (int)AccessStatus.Active && !x.IsDeleted).ToList();
            // Danh sách module phân quyền
            var modules = _unitOfWork.ElogbookRepository<Modules>().GetQuery(x => !x.IsDeleted).ToList();

            // Node Admin Portal
            var userPermissions = new List<UserPermissionModel>
            {
                AppendParrentNode(modules, permissions, CommonConstant.ADMIN_PORTAL),
            };

            // Node Mobile App
            userPermissions.Add(AppendParrentNode(modules, permissions, CommonConstant.MOBILE_APP));

            return userPermissions;
        }

        private List<UserPermissionModel> AppendPermissionForModule(string systemType, string moduleValue, List<UserPermission> permissions)
        {
            if (string.IsNullOrWhiteSpace(_permissions))
                _permissions = string.Empty;
            string[] listPermission = _permissions.Split(';');

            return permissions.Where(x => systemType.Equals(x.SystemType, StringComparison.OrdinalIgnoreCase)
                                                        && moduleValue.Equals(x.ModuleValue, StringComparison.OrdinalIgnoreCase))
                .Select(
                    e => new UserPermissionModel()
                    {
                        Data = new UserPermissionDetailModel()
                        {
                            Value = e.PermissionKey,
                            Label = e.PermissionName,
                            IsParrent = false,
                            IsSelected = Array.Exists(listPermission, x => x == e.PermissionKey) || _isAdmin,
                        },
                        Children = new List<UserPermissionModel>()
                    }).ToList();
        }

        private UserPermissionModel AppendParrentNode(List<Modules> modules, List<UserPermission> permissions, string parentNodeName)
        {
            var moduleNode = new UserPermissionModel()
            {
                Data = new UserPermissionDetailModel()
                {
                    Value = parentNodeName,
                    Label = parentNodeName,
                    IsParrent = true,
                    IsSelected = false,
                },
                Children = new List<UserPermissionModel>(),
            };

            // Danh sách tất cả module phân quyền cho Admin Portal
            moduleNode.Children = modules.Where(x => parentNodeName.Equals(x.SystemType, StringComparison.OrdinalIgnoreCase)).Select(
                e => new UserPermissionModel()
                {
                    Data = new UserPermissionDetailModel()
                    {
                        Value = e.ModuleValue,
                        Label = e.ModuleName,
                        IsParrent = true,
                        IsSelected = false,
                    },
                    Children = new List<UserPermissionModel>()
                }).ToList();
            foreach (var module in moduleNode.Children)
            {
                // Thêm các node quyền chức năng cho module
                module.Children = AppendPermissionForModule(parentNodeName, module.Data.Value, permissions);
            }

            return moduleNode;
        }

        public bool CheckPermission(string permissions)
        {
            var entities = _unitOfWork.ElogbookRepository<UserPermission>().GetQuery(x => x.Active == (int)AccessStatus.Active && !x.IsDeleted).ToList();
            var lstPermissions = entities.Select(x=>x.PermissionKey).ToList();
            string[] lstPermissionCheck = permissions.Split(';');
            bool isExist = true;
            foreach (var item in lstPermissionCheck)
            {
                if (!lstPermissions.Contains(item))
                {
                    isExist = false;
                    break; 
                }
            }

            return isExist;
        }
    }
}
