using Core.System.Runtime.Managers;
using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.User;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        public UserService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        public ResponseBaseModel GetUsers(SearchUserModel requestModel)
        {
            // Filter data by condition
            var users = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted);
            // Theo từ khóa
            if (!string.IsNullOrWhiteSpace(requestModel.Name))
                users = users.Where(x => (x.UserName.Contains(requestModel.Name) || x.FullName.Contains(requestModel.Name) || x.NationalID.Contains(requestModel.Name)));
            // Công ty
            if (requestModel.CompanyID > 0)
                users = users.Where(x => x.CompanyID == requestModel.CompanyID);
            // Nhà máy
            if (requestModel.FactoryID > 0)
                users = users.Where(x => x.FactoryID == requestModel.FactoryID);
            // Trạng thái
            if (requestModel.Status.HasValue && requestModel.Status.Value > 0)
                users = users.Where(x => x.Status == requestModel.Status);
            // Loại tài khoản
            if (requestModel.AccountType.HasValue && requestModel.AccountType.Value > 0)
                users = users.Where(x => x.AccountType == requestModel.AccountType);
            // Nguồn tài khoản
            if (requestModel.SourceType.HasValue && requestModel.SourceType.Value > 0)
                users = users.Where(x => x.SourceType == requestModel.SourceType);
            // Vai trò
            if (requestModel.UserRoleID.HasValue && requestModel.UserRoleID.Value > 0)
                users = users.Where(x => x.RoleInUsers.Any(t => t.UserRoleID == requestModel.UserRoleID));

            // Return and paging data
            var page = users.Select(
                e => new UserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    Company = new ValueLabelModel { Value = e.Company.ID, Label = e.Company.Name },
                    ProductionPlant = new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                    Team = new ValueLabelModel { Value = e.Team.ID, Label = e.Team.Name },
                    Group = new ValueLabelModel { Value = e.Group.ID, Label = e.Group.Name },
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    AccountTypeValue = e.AccountType,
                    SourceType = new ValueLabelModel { Value = e.SourceType, Label = e.SourceType == (int)UserSource.ADUser ? CommonConstant.AD : CommonConstant.ADMIN_PORTAL },
                    RoleInUsers = e.RoleInUsers.Where(x => !x.IsDeleted).Select(
                                            t => new ValueLabelModel()
                                            {
                                                Value = t.UserRole.ID,
                                                Label = t.UserRole.RoleName,
                                            }).ToList(),
                    Status = e.Status,
                    DateOfBirth = e.DateOfBirth,
                    NationalID = e.NationalID,
                    ElectricRankStr = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    PositionName = e.PositionName,
                    LaborCard = e.LaborCard,
                    LaborRankStr = e.LaborRank,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,
                    InsertedAt = e.InsertedAt,
                    SigningMethod = new ValueLabelModel { Value = e.SigningMethod.ID, Label = e.SigningMethod.SigningMethodName },
                }).OrderByDescending(x => x.InsertedAt).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public UserModel GetUserById(int id)
        {
            var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new UserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    Company = new ValueLabelModel { Value = e.Company.ID, Label = e.Company.Name },
                    ProductionPlant = new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                    Team = new ValueLabelModel { Value = e.Team.ID, Label = e.Team.Name },
                    Group = new ValueLabelModel { Value = e.Group.ID, Label = e.Group.Name },
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    AccountTypeValue = e.AccountType,
                    SourceType = new ValueLabelModel { Value = e.SourceType, Label = e.SourceType == (int)UserSource.ADUser ? CommonConstant.AD : CommonConstant.ADMIN_PORTAL },
                    RoleInUsers = e.RoleInUsers.Where(x => !x.IsDeleted).Select(
                                            t => new ValueLabelModel()
                                            {
                                                Value = t.UserRole.ID,
                                                Label = t.UserRole.RoleName,
                                            }).ToList(),
                    Status = e.Status,
                    DateOfBirth = e.DateOfBirth,
                    NationalID = e.NationalID,
                    ElectricRankStr = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    Signature = e.Signature,
                    PositionName = e.PositionName,
                    NationalAttachmentID = e.NationalAttachmentID,
                    ElectricRankAttachmentID = e.ElectricRankAttachmentID,
                    LaborCard = e.LaborCard,
                    LaborRankStr = e.LaborRank,
                    LaborCardAttachmentID = e.LaborCardAttachmentID,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,
                    AvatarAttachmentID = e.AvatarAttachmentID,
                    SigningMethod = new ValueLabelModel { Value = e.SigningMethod.ID, Label = e.SigningMethod.SigningMethodName },
                }).FirstOrDefault();

            if (userInfo != null)
            {
                // Hình ảnh số CMND/CCCD
                if (userInfo.NationalAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.NationalAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.NationalAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }

                // Hình ảnh thẻ an toàn điện
                if (userInfo.ElectricRankAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.ElectricRankAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.ElectricRankAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }

                // Hình ảnh thẻ an toàn lao động
                if (userInfo.LaborCardAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.LaborCardAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.LaborCardAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }

                // Hình ảnh đại diện
                if (userInfo.AvatarAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.AvatarAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.AvatarAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }
            }

            return userInfo;
        }

        public CurrentUserModel GetCurrentUser(string username)
        {
            var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.UserName.Equals(username, System.StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Select(
                e => new CurrentUserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    Password = e.Password,
                    Company = new ValueLabelModel { Value = e.Company.ID, Label = e.Company.Name },
                    ProductionPlant = new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                    Team = new ValueLabelModel { Value = e.Team.ID, Label = e.Team.Name },
                    Group = new ValueLabelModel { Value = e.Group.ID, Label = e.Group.Name },
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    AccountTypeValue = e.AccountType,
                    SourceType = new ValueLabelModel { Value = e.SourceType, Label = e.SourceType == (int)UserSource.ADUser ? CommonConstant.AD : CommonConstant.ADMIN_PORTAL },
                    RoleInUsers = e.RoleInUsers.Where(x => !x.IsDeleted).Select(
                                            t => new ValueLabelModel()
                                            {
                                                Value = t.UserRole.ID,
                                                Label = t.UserRole.RoleName,
                                            }).ToList(),
                    IsExistActiveRole = e.RoleInUsers.Any(t => t.UserRole.Active == (int)AccessStatus.Active),
                    Status = e.Status,
                    DateOfBirth = e.DateOfBirth,
                    NationalID = e.NationalID,
                    ElectricRankStr = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    PositionName = e.PositionName,
                    LaborCard = e.LaborCard,
                    LaborRankStr = e.LaborRank,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,
                    AvatarAttachmentID = e.AvatarAttachmentID,
                    SigningMethod = new ValueLabelModel { Value = e.SigningMethod.ID, Label = e.SigningMethod.SigningMethodName },
                    SigningMethodID = e.SigningMethod.ID,
                    NumberLoginFailed = e.NumberLoginFailed,
                }).FirstOrDefault();

            //Hình ảnh đại diện

            if (userInfo != null)
            {
                if (userInfo?.AvatarAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.AvatarAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.AvatarImage = _commonService.GetPathFile(attachment.FilePath);
                    }
                }

                userInfo.IsRequiredSigningMobile = !userInfo.SigningMethod.Label.Equals(CommonConstant.SIGNING_USERNAME_PASSWORD, System.StringComparison.OrdinalIgnoreCase);
            }

            return userInfo;
        }

        public int AddUser(AddEditUserModel requestModel)
        {
            var entity = ToEntity(requestModel);

            if (!string.IsNullOrEmpty(requestModel.DateOfBirth))
            {
                var dateOfBirth = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false);
                if (DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false) != System.DateTime.MinValue)
                    entity.DateOfBirth = dateOfBirth;
                else
                    entity.DateOfBirth = null;
            }

            // Lưu Vai trò cho người dùng
            var roleIDs = requestModel.UserRoleIDs.Split(';').ToList();
            var roleInUsers = new List<RoleInUser>();
            foreach (var roleID in roleIDs)
            {
                int.TryParse(roleID, out int id);
                if (id > 0 && _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.ID == id && !x.IsDeleted).Any())
                {
                    var roleInUser = new RoleInUser
                    {
                        UserRoleID = id,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    };
                    roleInUsers.Add(roleInUser);
                }
            }
            entity.RoleInUsers = roleInUsers;

            entity.CreatedUserID = entity.LastUpdatedUserID = requestModel.CurrentUserID;

            _unitOfWork.ElogbookRepository<User>().Add(entity);
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

        public bool UpdateUserProfileWeb(UpdateUserProfileWebModel requestModel)
        {
            var currentEntity = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (currentEntity != null)
            {
                #region Cập nhật người dùng

                //Gán mặc định phương thức ký cấp/khóa phiếu bằng "Tài khoản người dùng"
                if (requestModel.SigningMethodID <= 0)
                {
                    var signingMethod = _unitOfWork.ElogbookRepository<SigningMethod>().GetQuery(x => x.SigningMethodName.Equals(CommonConstant.SIGNING_USERNAME_PASSWORD, System.StringComparison.OrdinalIgnoreCase)
                                                                                        && !x.IsDeleted && x.Active == (int)UserStatus.Active).FirstOrDefault();
                    if (signingMethod != null)
                    {
                        requestModel.SigningMethodID = signingMethod.ID;
                    }
                }

                currentEntity.FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName.Trim() : requestModel.FullName;
                currentEntity.SigningMethodID = requestModel.SigningMethodID;
                var dateOfBirth = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false);
                if (!string.IsNullOrEmpty(requestModel.DateOfBirth)
                    && DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false) != System.DateTime.MinValue)
                {
                    currentEntity.DateOfBirth = dateOfBirth;
                }
                currentEntity.NationalID = !string.IsNullOrWhiteSpace(requestModel.NationalID) ? requestModel.NationalID.Trim() : requestModel.NationalID;

                if (!requestModel.ElectricRank.Contains("/"))
                {
                    currentEntity.ElectricRank = requestModel.ElectricRank.Trim() + "/5";
                }
                else
                {
                    currentEntity.ElectricRank = requestModel.ElectricRank;
                }

                currentEntity.ElectricRankCard = !string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) ? requestModel.ElectricRankCard.Trim() : requestModel.ElectricRankCard;
                currentEntity.Signature = !string.IsNullOrWhiteSpace(requestModel.Signature) ? requestModel.Signature.Trim() : requestModel.Signature;
                currentEntity.NationalAttachmentID = requestModel.NationalAttachmentID;
                currentEntity.ElectricRankAttachmentID = requestModel.ElectricRankAttachmentID;
                currentEntity.LaborCard = !string.IsNullOrWhiteSpace(requestModel.LaborCard) ? requestModel.LaborCard.Trim() : requestModel.LaborCard;

                if (!requestModel.LaborRank.Contains("/"))
                {
                    currentEntity.LaborRank = requestModel.LaborRank.Trim() + "/7";
                }
                else
                {
                    currentEntity.LaborRank = requestModel.LaborRank;
                }

                currentEntity.LaborCardAttachmentID = requestModel.LaborCardAttachmentID;
                currentEntity.PhoneNumber = !string.IsNullOrWhiteSpace(requestModel.PhoneNumber) ? requestModel.PhoneNumber.Trim() : requestModel.PhoneNumber;
                currentEntity.Email = !string.IsNullOrWhiteSpace(requestModel.Email) ? requestModel.Email.Trim() : requestModel.Email;
                currentEntity.AvatarAttachmentID = requestModel.AvatarAttachmentID;

                currentEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                _unitOfWork.ElogbookRepository<User>().Update(currentEntity);

                #endregion

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool UpdateUserProfileMobile(UpdateUserProfileMobileModel requestModel)
        {
            var currentEntity = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (currentEntity != null)
            {
                #region Cập nhật người dùng

                currentEntity.FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName.Trim() : requestModel.FullName;
                var dateOfBirth = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false);
                if (!string.IsNullOrEmpty(requestModel.DateOfBirth)
                    && DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false) != System.DateTime.MinValue)
                {
                    currentEntity.DateOfBirth = dateOfBirth;
                }
                currentEntity.NationalID = !string.IsNullOrWhiteSpace(requestModel.NationalID) ? requestModel.NationalID.Trim() : requestModel.NationalID;

                if (!requestModel.ElectricRank.Contains("/"))
                {
                    currentEntity.ElectricRank = requestModel.ElectricRank.Trim() + "/5";
                }
                else
                {
                    currentEntity.ElectricRank = requestModel.ElectricRank;
                }

                currentEntity.ElectricRankCard = !string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) ? requestModel.ElectricRankCard.Trim() : requestModel.ElectricRankCard;
                currentEntity.Signature = !string.IsNullOrWhiteSpace(requestModel.Signature) ? requestModel.Signature.Trim() : requestModel.Signature;
                currentEntity.NationalAttachmentID = requestModel.NationalAttachmentID;
                currentEntity.ElectricRankAttachmentID = requestModel.ElectricRankAttachmentID;
                currentEntity.LaborCard = !string.IsNullOrWhiteSpace(requestModel.LaborCard) ? requestModel.LaborCard.Trim() : requestModel.LaborCard;

                if (!requestModel.LaborRank.Contains("/"))
                {
                    currentEntity.LaborRank = requestModel.LaborRank.Trim() + "/7";
                }
                else
                {
                    currentEntity.LaborRank = requestModel.LaborRank;
                }

                currentEntity.LaborCardAttachmentID = requestModel.LaborCardAttachmentID;
                currentEntity.PhoneNumber = !string.IsNullOrWhiteSpace(requestModel.PhoneNumber) ? requestModel.PhoneNumber.Trim() : requestModel.PhoneNumber;

                currentEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                _unitOfWork.ElogbookRepository<User>().Update(currentEntity);

                #endregion

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool UpdateUser(AddEditUserModel requestModel)
        {
            var currentEntity = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (currentEntity != null)
            {
                #region Cập nhật vai trò cho người dùng

                //Chỉ cho phép cập nhật thông tin vai trò từ web
                if (!requestModel.IsMobileSystem)
                {
                    var roleIDStr = requestModel.UserRoleIDs.Split(';').ToList();
                    var listRoleIDs = new List<int>();

                    foreach (var roleID in roleIDStr)
                    {
                        int.TryParse(roleID, out int id);
                        if (id > 0)
                        {
                            listRoleIDs.Add(id);
                            // Thêm mới vai trò cho người dùng nếu chưa tồn tại
                            if (!currentEntity.RoleInUsers.Any(x => x.UserRoleID == id)
                                && _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.ID == id && !x.IsDeleted).Any())
                            {
                                var roleInUser = new RoleInUser
                                {
                                    UserRoleID = id,
                                    UserID = requestModel.ID,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };
                                _unitOfWork.ElogbookRepository<RoleInUser>().Add(roleInUser);
                            }
                        }
                    }
                    // Xóa vai trò không còn gán cho người dùng
                    var userRolesDelete = currentEntity.RoleInUsers.Where(x => !listRoleIDs.Contains(x.UserRoleID)).ToList();
                    _unitOfWork.ElogbookRepository<RoleInUser>().Delete(userRolesDelete, requestModel.CurrentUserID, true);
                }

                #endregion

                #region Cập nhật người dùng

                //Gán mặc định phương thức ký cấp/khóa phiếu bằng "Tài khoản người dùng"
                if (requestModel.SigningMethodID <= 0)
                {
                    var signingMethod = _unitOfWork.ElogbookRepository<SigningMethod>().GetQuery(x => x.SigningMethodName.Equals(CommonConstant.SIGNING_USERNAME_PASSWORD, System.StringComparison.OrdinalIgnoreCase)
                                                                                        && !x.IsDeleted && x.Active == (int)UserStatus.Active).FirstOrDefault();
                    if (signingMethod != null)
                    {
                        requestModel.SigningMethodID = signingMethod.ID;
                    }
                }

                // Trong trường hợp login với người dùng AD thì lưu lại thông tin người dùng
                if (requestModel.IsLoginAD)
                {
                    currentEntity.UserName = !string.IsNullOrWhiteSpace(requestModel.UserName) ? requestModel.UserName.Trim() : requestModel.UserName;
                    currentEntity.FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName.Trim() : requestModel.FullName;
                    if (!string.IsNullOrWhiteSpace(requestModel.Password))
                        currentEntity.Password = RuntimeManager.GeneratePasswordHash(requestModel.Password);
                }
                else
                {
                    currentEntity.UserName = !string.IsNullOrWhiteSpace(requestModel.UserName) ? requestModel.UserName.Trim() : requestModel.UserName;
                    currentEntity.FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName.Trim() : requestModel.FullName;
                    if (!string.IsNullOrWhiteSpace(requestModel.Password))
                        currentEntity.Password = RuntimeManager.GeneratePasswordHash(requestModel.Password);
                    currentEntity.CompanyID = requestModel.CompanyID;
                    currentEntity.ProductionPlantID = requestModel.ProductionPlantID.GetValueOrDefault(0) > 0 ? requestModel.ProductionPlantID : null;
                    currentEntity.TeamID = requestModel.TeamID.GetValueOrDefault(0) > 0 ? requestModel.TeamID : null;
                    currentEntity.GroupID = requestModel.GroupID.GetValueOrDefault(0) > 0 ? requestModel.GroupID : null;
                    currentEntity.FactoryID = requestModel.FactoryID.GetValueOrDefault(0) > 0 ? requestModel.FactoryID : null;
                    currentEntity.AccountType = requestModel.AccountType <= 0 ? (int)UserType.TwoSystem : requestModel.AccountType;
                    currentEntity.SigningMethodID = requestModel.SigningMethodID;
                    currentEntity.SourceType = requestModel.SourceType <= 0 ? (int)UserSource.AdminPortal : requestModel.SourceType;
                    currentEntity.Status = requestModel.Status <= 0 ? (int)UserStatus.Active : requestModel.Status;
                    var dateOfBirth = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false);
                    if (!string.IsNullOrEmpty(requestModel.DateOfBirth)
                        && DateTimeConvert.ConvertDateTimeFromString(requestModel.DateOfBirth, false) != System.DateTime.MinValue)
                    {
                        currentEntity.DateOfBirth = dateOfBirth;
                    }
                    currentEntity.NationalID = !string.IsNullOrWhiteSpace(requestModel.NationalID) ? requestModel.NationalID.Trim() : requestModel.NationalID;

                    if (!requestModel.ElectricRank.Contains("/"))
                    {
                        currentEntity.ElectricRank = requestModel.ElectricRank.Trim() + "/5";
                    }
                    else
                    {
                        currentEntity.ElectricRank = requestModel.ElectricRank;
                    }

                    currentEntity.ElectricRankCard = !string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) ? requestModel.ElectricRankCard.Trim() : requestModel.ElectricRankCard;
                    currentEntity.Signature = !string.IsNullOrWhiteSpace(requestModel.Signature) ? requestModel.Signature.Trim() : requestModel.Signature;
                    currentEntity.PositionName = !string.IsNullOrWhiteSpace(requestModel.PositionName) ? requestModel.PositionName.Trim() : requestModel.PositionName;
                    currentEntity.NationalAttachmentID = requestModel.NationalAttachmentID;
                    currentEntity.ElectricRankAttachmentID = requestModel.ElectricRankAttachmentID;
                    currentEntity.LaborCard = !string.IsNullOrWhiteSpace(requestModel.LaborCard) ? requestModel.LaborCard.Trim() : requestModel.LaborCard;

                    if (!requestModel.LaborRank.Contains("/"))
                    {
                        currentEntity.LaborRank = requestModel.LaborRank.Trim() + "/7";
                    }
                    else
                    {
                        currentEntity.LaborRank = requestModel.LaborRank;
                    }

                    currentEntity.LaborCardAttachmentID = requestModel.LaborCardAttachmentID;
                    currentEntity.PhoneNumber = !string.IsNullOrWhiteSpace(requestModel.PhoneNumber) ? requestModel.PhoneNumber.Trim() : requestModel.PhoneNumber;
                    currentEntity.Email = !string.IsNullOrWhiteSpace(requestModel.Email) ? requestModel.Email.Trim() : requestModel.Email;
                    currentEntity.AvatarAttachmentID = requestModel.AvatarAttachmentID;
                }

                currentEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                currentEntity.NumberLoginFailed = 0;
                _unitOfWork.ElogbookRepository<User>().Update(currentEntity);

                #endregion

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteUser(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<User>().Delete(entity, userID);
                // Xóa vai trò của người dùng
                var roleInUsers = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => x.UserID == id && !x.IsDeleted).ToList();
                _unitOfWork.ElogbookRepository<RoleInUser>().Delete(roleInUsers, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool CheckExistUserByUserName(string userName)
        {
            return _unitOfWork.ElogbookRepository<User>().GetQuery(t => t.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase) && !t.IsDeleted).Any();
        }

        public bool ChangePassword(ChangePasswordModel requestModel)
        {
            var currentEntity = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == requestModel.CurrentUserID).FirstOrDefault();
            if (currentEntity != null && !string.IsNullOrWhiteSpace(requestModel.NewPassword))
            {
                currentEntity.Password = RuntimeManager.GeneratePasswordHash(requestModel.NewPassword);
                currentEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                currentEntity.NumberLoginFailed = 0;

                _unitOfWork.ElogbookRepository<User>().Update(currentEntity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool LogOut(int userID, string userName, string token)
        {
            var tokeExpire = new TokenExpire
            {
                UserName = userName,
                Token = token,
                CreatedUserID = userID,
                LastUpdatedUserID = userID,
                InsertedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _unitOfWork.ElogbookRepository<TokenExpire>().Add(tokeExpire);

            return _unitOfWork.Complete() > 0;
        }

        private User ToEntity(AddEditUserModel requestModel)
        {
            //Gán mặc định phương thức ký cấp/khóa phiếu bằng "Tài khoản người dùng"
            if (requestModel.SigningMethodID <= 0)
            {
                var signingMethod = _unitOfWork.ElogbookRepository<SigningMethod>().GetQuery(x => x.SigningMethodName.Equals(CommonConstant.SIGNING_USERNAME_PASSWORD, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted && x.Active == (int)UserStatus.Active).FirstOrDefault();
                if (signingMethod != null)
                {
                    requestModel.SigningMethodID = signingMethod.ID;
                }
            }

            return new User
            {
                FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName.Trim() : requestModel.FullName,
                UserName = !string.IsNullOrWhiteSpace(requestModel.UserName) ? requestModel.UserName.Trim() : requestModel.UserName,
                Password = !string.IsNullOrWhiteSpace(requestModel.Password) ? RuntimeManager.GeneratePasswordHash(requestModel.Password) : requestModel.Password,
                CompanyID = requestModel.CompanyID,
                ProductionPlantID = requestModel.ProductionPlantID.GetValueOrDefault(0) > 0 ? requestModel.ProductionPlantID : null,
                TeamID = requestModel.TeamID.GetValueOrDefault(0) > 0 ? requestModel.TeamID : null,
                GroupID = requestModel.GroupID.GetValueOrDefault(0) > 0 ? requestModel.GroupID : null,
                FactoryID = requestModel.FactoryID.GetValueOrDefault(0) > 0 ? requestModel.FactoryID : null,
                AccountType = requestModel.AccountType <= 0 ? (int)UserType.TwoSystem : requestModel.AccountType,
                SigningMethodID = requestModel.SigningMethodID,
                SourceType = requestModel.SourceType <= 0 ? (int)UserSource.AdminPortal : requestModel.SourceType,
                Status = requestModel.Status <= 0 ? (int)UserStatus.Active : requestModel.Status,
                NationalID = !string.IsNullOrWhiteSpace(requestModel.NationalID) ? requestModel.NationalID.Trim() : requestModel.NationalID,
                ElectricRank = requestModel.ElectricRank,
                ElectricRankCard = !string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) ? requestModel.ElectricRankCard.Trim() : requestModel.ElectricRankCard,
                Signature = !string.IsNullOrWhiteSpace(requestModel.Signature) ? requestModel.Signature.Trim() : requestModel.Signature,
                PositionName = !string.IsNullOrWhiteSpace(requestModel.PositionName) ? requestModel.PositionName.Trim() : requestModel.PositionName,
                NationalAttachmentID = requestModel.NationalAttachmentID,
                ElectricRankAttachmentID = requestModel.ElectricRankAttachmentID,
                LaborCard = !string.IsNullOrWhiteSpace(requestModel.LaborCard) ? requestModel.LaborCard.Trim() : requestModel.LaborCard,
                LaborRank = requestModel.LaborRank,
                LaborCardAttachmentID = requestModel.LaborCardAttachmentID,
                PhoneNumber = !string.IsNullOrWhiteSpace(requestModel.PhoneNumber) ? requestModel.PhoneNumber.Trim() : requestModel.PhoneNumber,
                Email = !string.IsNullOrWhiteSpace(requestModel.Email) ? requestModel.Email.Trim() : requestModel.Email,
                AvatarAttachmentID = requestModel.AvatarAttachmentID,
            };
        }

        public bool CheckInUseUser(int id)
        {
            // Tài khoản này không thể xóa
            //OperationDiaryPositionShift
            bool isUseOperationDiaryPositionShift = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>().GetQuery(x => x.AssignedUserID == id && !x.IsDeleted).Any();
            if (isUseOperationDiaryPositionShift) return true;

            //Nhân viên tập sự
            //ProbationaryStaff
            bool isUseProbationaryStaff = _unitOfWork.ElogbookRepository<ProbationaryStaff>().GetQuery(x => x.AssignedUserID == id && !x.IsDeleted).Any();
            if (isUseProbationaryStaff) return true;

            //DeliveryReceiveShift
            bool isUseDeliveryReceiveShift = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.SupervisorID == id && !x.IsDeleted).Any();
            if (isUseDeliveryReceiveShift) return true;

            //OperationSituation
            bool isUseOperationSituation = _unitOfWork.ElogbookRepository<OperationSituation>().GetQuery(x => (x.AssignedFromID == id || x.AssignedToID == id) && !x.IsDeleted).Any();
            if (isUseOperationSituation) return true;

            //RegistrationManipulationTicket
            bool isUseRegistrationManipulationTicket = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetQuery(x => (x.ApprovedUserID == id || x.CreatedUserID == id || x.LastUpdatedUserID == id) && !x.IsDeleted).Any();
            if (isUseRegistrationManipulationTicket) return true;

            //IssueOfShiftDetail
            bool isUseIssueOfShiftDetail = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().GetQuery(x => (x.CreatedUserID == id) && !x.IsDeleted).Any();
            if (isUseIssueOfShiftDetail) return true;

            //SequenceOperation
            bool isUseSequenceOperation = _unitOfWork.ElogbookRepository<SequenceOperation>().GetQuery(x => (x.CreatedUserID == id || x.LastUpdatedUserID == id) && !x.IsDeleted).Any();
            if (isUseSequenceOperation) return true;

            //ExistInShift
            bool isUseExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => (x.CreatedUserID == id) && !x.IsDeleted).Any();
            if (isUseExistInShift) return true;

            //OperationDiary
            bool isUseOperationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => (x.LastUpdatedUserID == id) && !x.IsDeleted).Any();
            if (isUseOperationDiary) return true;

            //RegistrationTicket
            bool isUseRegistrationTicket = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => (x.CreatedUserID == id) && !x.IsDeleted).Any();
            if (isUseRegistrationTicket) return true;

            //Stakeholders
            bool isUseStakeholders = _unitOfWork.ElogbookRepository<Stakeholders>().GetQuery(x => (x.AssignToUserID == id) && !x.IsDeleted).Any();
            if (isUseStakeholders) return true;

            //GuestUser
            bool isUseGuestUser = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => (x.CreatedUserID == id || x.LastUpdatedUserID == id) && !x.IsDeleted).Any();
            if (isUseGuestUser) return true;

            //TicketStatusHistory
            bool isUseTicketStatusHistory = _unitOfWork.ElogbookRepository<TicketStatusHistory>().GetQuery(x => (x.CreatedUserID == id) && !x.IsDeleted).Any();
            if (isUseTicketStatusHistory) return true;

            var userEntity = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == id).FirstOrDefault();
            if (userEntity != null)
            {
                var userName = userEntity.UserName;
                //WorkerInformation
                bool isUseWorkerInformation = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.UserName.Equals(userName) && !x.IsDeleted).Any();
                if (isUseWorkerInformation) return true;

                //StaffInformation
                bool isUseStaffInformation = _unitOfWork.ElogbookRepository<StaffInformation>().GetQuery(x => x.UserName.Equals(userName) && !x.IsDeleted).Any();
                if (isUseStaffInformation) return true;

                //LeadershipUser
                bool isUseLeadershipUser = _unitOfWork.ElogbookRepository<LeadershipUser>().GetQuery(x => x.UserName.Equals(userName) && !x.IsDeleted).Any();
                if (isUseLeadershipUser) return true;

                //SequenceOperationParticipant
                bool isUseSequenceOperationParticipant = _unitOfWork.ElogbookRepository<SequenceOperationParticipant>().GetQuery(x => x.UserName.Equals(userName) && !x.IsDeleted).Any();
                if (isUseSequenceOperationParticipant) return true;

                //SequenceOperationParticipant
                bool isUseParticipant = _unitOfWork.ElogbookRepository<Participant>().GetQuery(x => x.UserName.Equals(userName) && !x.IsDeleted).Any();
                if (isUseParticipant) return true;
            }

            return false;
        }

        public bool CheckDuplicateData(int userID, string description, bool isAddMode, int fieldType)
        {
            if (isAddMode)
            {
                switch (fieldType)
                {
                    // Số CMND/CCCD
                    case 1:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.NationalID.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn điện
                    case 2:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ElectricRankCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn lao động
                    case 3:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.LaborCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Email
                    case 4:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.Email.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                }
            }
            else
            {
                switch (fieldType)
                {
                    // Số CMND/CCCD
                    case 1:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID != userID
                                                                                    && x.NationalID.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn điện
                    case 2:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID != userID
                                                                                    && x.ElectricRankCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn lao động
                    case 3:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID != userID
                                                                                    && x.LaborCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Email
                    case 4:
                        return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID != userID
                                                                                    && x.Email.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                }
            }

            return false;
        }

        public bool MigrationPasswordOldSystem()
        {
            var users = _unitOfWork.ElogbookRepository<User>().GetQuery().ToList();
            foreach (var user in users)
            {
                if (!string.IsNullOrWhiteSpace(user.Password))
                    user.Password = RuntimeManager.GeneratePasswordHash(user.Password);
                _unitOfWork.ElogbookRepository<User>().Update(user);
            }

            return _unitOfWork.Complete() > 0;
        }

        public void HandleLoginFailed(int userID, bool isLoginFail)
        {
            var user = _unitOfWork.ElogbookRepository<User>().GetById(userID);
            if (user != null)
            {
                if (isLoginFail)
                {
                    user.NumberLoginFailed++;
                }
                else
                {
                    user.NumberLoginFailed = 0;
                }

                _unitOfWork.ElogbookRepository<User>().Update(user);
                _unitOfWork.Complete();
            }
        }

        public bool CheckValidSigningMethod(int signingMethodID)
        {
            return _unitOfWork.ElogbookRepository<SigningMethod>().GetQuery(x => x.ID == signingMethodID && x.Active == 1 && !x.IsDeleted).Any();
        }

        public bool CheckExistEmail(int userID)
        {
            var hasEmail = _unitOfWork.ElogbookRepository<User>()
                            .GetQuery(t => t.ID == userID
                                        && !t.IsDeleted
                                        && t.Email != null
                                        && t.Email.Trim() != "")
                            .Any();

            return hasEmail;
        }
    }
}
