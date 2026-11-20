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
    public class UserGroupService : IUserGroupService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        public UserGroupService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy tất cả người dùng trong hệ thống
        /// </summary>
        /// <returns></returns>
        public List<EmployeeInfoModel> GetAllUsers(bool isExternal)
        {
            return _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted)
                .Select(e => new EmployeeInfoModel()
                {
                    ID = e.ID,
                    UserName = e.UserName,
                    FullName = e.FullName,
                    IsExternal = false
                }).OrderBy(t => t.ID).ToList();
        }

        /// <summary>
        /// Kiểm tra tài khoản người dùng đã tồn tại
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="userId"></param>
        /// <param name="isExternal"></param>
        /// <returns></returns>
        public bool CheckDuplicateUser(int iD, int userId, bool isExternal)
        {
            return _unitOfWork.ElogbookRepository<UserInGroup>()
                .GetQuery(x => !x.IsDeleted
                                && x.UserGroupsID == iD
                                && x.UserID == userId
                                && x.IsExternal == isExternal)
                .Any();
        }

        /// <summary>
        /// Kiểm tra nhóm người dùng đã tồn tại
        /// </summary>
        /// <param name="factoryId"></param>
        /// <param name="userGroupName"></param>
        /// <returns></returns>
        public bool CheckDuplicateUserGroup(int factoryId, string userGroupName)
        {
            return _unitOfWork.ElogbookRepository<UserGroup>()
                .GetQuery(x => !x.IsDeleted
                                && x.FactoryID == factoryId
                                && x.UserGroupName.Equals(userGroupName))
                .Any();
        }

        /// <summary>
        /// Lấy thông tin nhóm người theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserGroupModel GetUserGroupById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<UserGroup>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new UserGroupModel()
                {
                    ID = e.ID,
                    UserGroupName = e.UserGroupName,
                    FactoryID = e.FactoryID,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                    UserInGroups = e.UserInGroups.Where(k => !k.IsDeleted).Select(t => new UserInGroupModel()
                    {
                        UserID = t.UserID,
                        IsExternal = t.IsExternal,
                    }).ToList(),
                }).FirstOrDefault();

            if (entity != null && entity.UserInGroups != null && entity.UserInGroups.Count > 0)
            {
                List<string> lstFullName = new List<string>();
                foreach (var user in entity.UserInGroups)
                {
                    if (user.IsExternal)
                    {
                        var userInfo = _unitOfWork.ElogbookRepository<GuestUser>()
                            .GetQuery(x => !x.IsDeleted && x.ID == user.UserID).FirstOrDefault();

                        if (userInfo != null)
                        {
                            user.FullName = userInfo.FullName;
                            lstFullName.Add(userInfo.FullName);
                        }
                    }

                    if (!user.IsExternal)
                    {
                        var userInfo = _unitOfWork.ElogbookRepository<User>()
                            .GetQuery(x => !x.IsDeleted && x.ID == user.UserID).FirstOrDefault();

                        if (userInfo != null)
                        {
                            user.FullName = userInfo.FullName;
                            lstFullName.Add(userInfo.FullName);
                        }
                    }
                }
                entity.FullNames = lstFullName;
            }

            return entity;
        }

        /// <summary>
        /// Tìm kiếm nhóm người dùng theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetUserGroups(SearchUserGroupModel requestModel)
        {
            // Dữ liệu đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<UserGroup>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID > 1)
            {
                entities = entities.Where(x => x.FactoryID == (requestModel.FactoryID));
            }
            // Tên Nhóm người dùng
            if (!string.IsNullOrWhiteSpace(requestModel.UserGroupName))
            {
                entities = entities.Where(x => x.UserGroupName.Contains(requestModel.UserGroupName));
            }
            // Trạng thái
            if (requestModel.Active > -1)
            {
                entities = entities.Where(x => x.Active == (requestModel.Active == 1));
            }

            // Tìm kiếm và paging dữ liệu
            var page = entities.Select(e => new UserGroupModel()
            {
                ID = e.ID,
                FactoryID = e.FactoryID,
                Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                UserGroupName = e.UserGroupName,
                Active = e.Active,
                UserInGroups = e.UserInGroups.Where(k => !k.IsDeleted).Select(t => new UserInGroupModel()
                {
                    UserID = t.UserID,
                    IsExternal = t.IsExternal,
                }).ToList()
            })
                .OrderBy(y => y.FactoryID)
                .ThenBy(y => y.UserGroupName);

            IQueryable<UserGroupModel> dataResult = page;

            IPagedList<UserGroupModel> pagelist = dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            foreach (var item in pagelist)
            {
                if (item.UserInGroups != null && item.UserInGroups.Count > 0)
                {
                    List<string> lstFullName = new List<string>();
                    foreach (var user in item.UserInGroups)
                    {
                        if (user.IsExternal)
                        {
                            var userInfo = _unitOfWork.ElogbookRepository<GuestUser>()
                                .GetQuery(x => !x.IsDeleted && x.ID == user.UserID).FirstOrDefault();

                            if (userInfo != null)
                            {
                                user.FullName = userInfo.FullName;
                                lstFullName.Add(userInfo.FullName);
                            }
                        }

                        if (!user.IsExternal)
                        {
                            var userInfo = _unitOfWork.ElogbookRepository<User>()
                                .GetQuery(x => !x.IsDeleted && x.ID == user.UserID).FirstOrDefault();

                            if (userInfo != null)
                            {
                                user.FullName = userInfo.FullName;
                                lstFullName.Add(userInfo.FullName);
                            }
                        }
                    }
                    item.FullNames = lstFullName;
                }
            }

            pagelist.GetEnumerator().Reset();

            return HttpResponse.OkList(pagelist);
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool AddUserGroup(AddEditUserGroupModel requestModel)
        {
            var userGroupEntity = new UserGroup
            {
                FactoryID = requestModel.FactoryID,
                UserGroupName = requestModel.UserGroupName,
                UserInGroups = requestModel.UserAccounts?.Select(e => new UserInGroup()
                {
                    UserID = e.UserID,
                    IsExternal = (bool)e.IsExternal,
                    IsDeleted = false,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList(),
                Active = requestModel.Active,
                IsDeleted = false,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
                InsertedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _unitOfWork.ElogbookRepository<UserGroup>().Add(userGroupEntity);
            return _unitOfWork.Complete() > 0;
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool UpdateUserGroup(AddEditUserGroupModel requestModel)
        {
            var userGroupEntity = _unitOfWork.ElogbookRepository<UserGroup>()
                .GetQuery(x => !x.IsDeleted && x.ID == requestModel.ID).FirstOrDefault();

            if (userGroupEntity != null)
            {
                userGroupEntity.Active = requestModel.Active;
                userGroupEntity.UpdatedAt = DateTime.Now;
                userGroupEntity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<UserGroup>().Update(userGroupEntity);
                _unitOfWork.Complete();

                if (requestModel.UserAccounts.Count > 0)
                {
                    foreach (var user in requestModel.UserAccounts)
                    {
                        // Thêm mới
                        if (user.IsInsert)
                        {
                            var userInsert = new UserInGroup
                            {
                                UserGroupsID = userGroupEntity.ID,
                                UserID = user.UserID,
                                IsExternal = (bool)user.IsExternal,
                                IsDeleted = false,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            _unitOfWork.ElogbookRepository<UserInGroup>().Add(userInsert);
                            _unitOfWork.Complete();
                        }
                        // Xóa
                        else
                        {
                            var userDelete = _unitOfWork.ElogbookRepository<UserInGroup>()
                                .GetQuery(x => !x.IsDeleted
                                            && x.UserGroupsID == userGroupEntity.ID
                                            && x.UserID == user.UserID
                                            && x.IsExternal == (bool)user.IsExternal)
                                .FirstOrDefault();

                            if (userDelete != null)
                            {
                                _unitOfWork.ElogbookRepository<UserInGroup>().Delete(userDelete, requestModel.CurrentUserID);
                                _unitOfWork.Complete();
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool DeleteUserGroup(int iD, int userID)
        {
            var userGroupDelete = _unitOfWork.ElogbookRepository<UserGroup>()
                .GetQuery(x => !x.IsDeleted && x.ID == iD).FirstOrDefault();

            if (userGroupDelete != null)
            {
                var userInGroupDelete = _unitOfWork.ElogbookRepository<UserInGroup>()
                    .GetQuery(x => x.UserGroupsID == userGroupDelete.ID).ToList();

                _unitOfWork.ElogbookRepository<UserInGroup>().Delete(userInGroupDelete, userID);

                _unitOfWork.ElogbookRepository<UserGroup>().Delete(userGroupDelete, userID);
                return _unitOfWork.Complete() > 0;
            }

            return false;
        }
    }
}
