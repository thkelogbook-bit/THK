using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Service for SLA configuration management
    /// </summary>
    public class SlaConfigService : ISlaConfigService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        public SlaConfigService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get active email templates for SLA
        /// </summary>
        public List<SlaEmailTemplateModel> GetActiveEmailTemplates()
        {
            return _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => !x.IsDeleted && x.Active == (int)Core.Enums.AccessStatus.Active)
                .Select(e => new SlaEmailTemplateModel
                {
                    ID = e.ID,
                    EmailSubject = e.EmailSubject
                }).OrderBy(t => t.ID).ToList();
        }

        public bool EmailTemplateExists(int id)
        {
            if (id <= 0) return false;

            return _unitOfWork.ElogbookRepository<EmailTemplate>()
                .GetQuery(x => x.ID == id && !x.IsDeleted && x.Active == (int)Core.Enums.AccessStatus.Active)
                .Any();
        }

        /// <summary>
        /// Physically delete a SlaLevel by ID
        /// </summary>
        public bool DeleteSlaLevel(int id, int currentUserID)
        {
            var entity = _unitOfWork.ElogbookRepository<SlaLevel>().GetQuery(x => x.ID == id).FirstOrDefault();
            if (entity == null)
                return false;

            // delete related SlaLevelUserGroup entries first
            var userGroups = _unitOfWork.ElogbookRepository<SlaLevelUserGroup>().GetQuery(r => r.SlaLevelID == entity.ID).ToList();
            foreach (var r in userGroups)
            {
                _unitOfWork.ElogbookRepository<SlaLevelUserGroup>().Delete(r, currentUserID, true);
            }

            // then delete the SlaLevel
            _unitOfWork.ElogbookRepository<SlaLevel>().Delete(entity, currentUserID, true);
            _unitOfWork.Complete();

            return true;
        }

        /// <summary>
        /// Add a SlaLevel to a SlaConfig
        /// </summary>
        public int AddSlaLevel(AddEditSlaLevelModel model)
        {
            if (model == null) return 0;

            var slaConfig = _unitOfWork.ElogbookRepository<SlaConfig>().GetQuery(x => x.ID == model.SlaConfigID && !x.IsDeleted).FirstOrDefault();
            if (slaConfig == null) return 0;

            var lvlEntity = new SlaLevel
            {
                SlaConfigID = model.SlaConfigID,
                LevelOrder = model.LevelOrder,
                SlaConditionID = model.SlaConditionID,
                OffsetDays = model.OffsetDays,
                OffsetHours = model.OffsetHours,
                OffsetMinutes = model.OffsetMinutes,
                IncludeHolidays = model.IncludeHolidays,
                IncludeWeekends = model.IncludeWeekends,
                WorkingHoursOnly = model.WorkingHoursOnly,
                SendEmail = model.SendEmail,
                EmailTemplateID = model.EmailTemplateId > 0 ? model.EmailTemplateId : null,
                SlaLevelUserGroups = model.UserGroups?.Select(u => new SlaLevelUserGroup
                {
                    UserGroupsID = u.ID,
                    InsertedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedUserID = model.CurrentUserID,
                    LastUpdatedUserID = model.CurrentUserID,
                    IsDeleted = false
                }).ToList(),
                InsertedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedUserID = model.CurrentUserID,
                LastUpdatedUserID = model.CurrentUserID,
                IsDeleted = false
            };

            _unitOfWork.ElogbookRepository<SlaLevel>().Add(lvlEntity);
            _unitOfWork.Complete();

            return lvlEntity.ID;
        }

        /// <summary>
        /// Update a SlaLevel (replace userGroups)
        /// </summary>
        public bool UpdateSlaLevel(AddEditSlaLevelModel model)
        {
            if (model == null || model.ID <= 0) return false;

            var entity = _unitOfWork.ElogbookRepository<SlaLevel>().GetQuery(x => x.ID == model.ID && !x.IsDeleted).FirstOrDefault();
            if (entity == null) return false;

            entity.LevelOrder = model.LevelOrder;
            entity.SlaConditionID = model.SlaConditionID;
            entity.OffsetDays = model.OffsetDays;
            entity.OffsetHours = model.OffsetHours;
            entity.OffsetMinutes = model.OffsetMinutes;
            entity.IncludeHolidays = model.IncludeHolidays;
            entity.IncludeWeekends = model.IncludeWeekends;
            entity.WorkingHoursOnly = model.WorkingHoursOnly;
            entity.SendEmail = model.SendEmail;
            entity.EmailTemplateID = model.EmailTemplateId > 0 ? model.EmailTemplateId : null;
            entity.UpdatedAt = DateTime.Now;
            entity.LastUpdatedUserID = model.CurrentUserID;

            _unitOfWork.ElogbookRepository<SlaLevel>().Update(entity);

            // delete existing usergroup physically
            var existingUserGroups = _unitOfWork.ElogbookRepository<SlaLevelUserGroup>().GetQuery(r => r.SlaLevelID == entity.ID).ToList();
            foreach (var er in existingUserGroups)
            {
                _unitOfWork.ElogbookRepository<SlaLevelUserGroup>().Delete(er, model.CurrentUserID, true);
            }
            _unitOfWork.Complete();

            // add new usergroups
            if (model.UserGroups != null && model.UserGroups.Count > 0)
            {
                foreach (var r in model.UserGroups)
                {
                    var userGroupEntity = new SlaLevelUserGroup
                    {
                        SlaLevelID = entity.ID,
                        UserGroupsID = r.ID,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedUserID = model.CurrentUserID,
                        IsDeleted = false
                    };

                    _unitOfWork.ElogbookRepository<SlaLevelUserGroup>().Add(userGroupEntity);
                }

                _unitOfWork.Complete();
            }

            return true;
        }

        /// <summary>
        /// Get all SLA configurations with pagination and filtering
        /// </summary>
        public ResponseBaseModel GetSlaConfigs(SearchSlaConfigModel requestModel)
        {
            // Get active configurations
            var entities = _unitOfWork.ElogbookRepository<SlaConfig>().GetQuery(x => !x.IsDeleted);

            // Filter by name
            if (!string.IsNullOrWhiteSpace(requestModel.Name))
            {
                entities = entities.Where(x => x.Name.Contains(requestModel.Name));
            }

            // Filter by factory
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Filter by category
            if (requestModel.CategoryID > 0)
            {
                entities = entities.Where(x => x.SlaCategoryID == requestModel.CategoryID);
            }

            // Filter by active status
            if (requestModel.IsActive.HasValue)
            {
                entities = entities.Where(x => x.IsActive == requestModel.IsActive.Value);
            }

            // Search, sort and paginate
            var page = entities.Select(e => new SlaConfigDetailModel()
            {
                ID = e.ID,
                Factory = new SlaFactoryModel
                {
                    ID = e.Factory.ID,
                    Name = e.Factory.FactoryName
                },
                Category = new SlaCategoryModel
                {
                    ID = e.Category.ID,
                    Name = e.Category.Name,
                    Description = e.Category.Description
                },
                Name = e.Name,
                Description = e.Description,
                IsActive = e.IsActive,
                CreatedUser = new SlaUserModel()
                {
                    ID = e.CreatedUserID,
                    UserName = e.CreatedUser.UserName,
                    FullName = e.CreatedUser.FullName
                },
                CreatedAt = e.InsertedAt,
                UpdatedAt = e.UpdatedAt,
                Levels = e.SlaLevels.Where(sl => !sl.IsDeleted).Select(sl => new SlaLevelModel()
                {
                    ID = sl.ID,
                    LevelOrder = sl.LevelOrder,
                    ConditionType = sl.SlaCondition.Name,
                    OffsetDays = sl.OffsetDays,
                    OffsetHours = sl.OffsetHours,
                    OffsetMinutes = sl.OffsetMinutes,
                    IncludeHolidays = sl.IncludeHolidays,
                    IncludeWeekends = sl.IncludeWeekends,
                    WorkingHoursOnly = sl.WorkingHoursOnly,
                    SendEmail = sl.SendEmail,
                    EmailTemplateId = sl.EmailTemplateID,
                    UserGroups = sl.SlaLevelUserGroups.Where(u => !u.IsDeleted).Select(r => new SlaUserGroupModel
                    {
                        ID = r.ID,
                        UserGroupsID = r.UserGroupsID,
                        UserGroupName = r.UserGroups.UserGroupName,
                    }).ToList()
                }).ToList()
            }).OrderByDescending(x => x.CreatedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Get SLA configuration details by ID
        /// </summary>
        public SlaConfigDetailModel GetSlaConfigById(int id)
        {
            return _unitOfWork.ElogbookRepository<SlaConfig>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new SlaConfigDetailModel()
                {
                    ID = e.ID,
                    Category = new SlaCategoryModel
                    {
                        ID = e.Category.ID,
                        Name = e.Category.Name,
                        Description = e.Category.Description
                    },
                    Name = e.Name,
                    Description = e.Description,
                    IsActive = e.IsActive,
                    CreatedUser = new SlaUserModel()
                    {
                        ID = e.CreatedUserID,
                        UserName = e.CreatedUser.UserName,
                        FullName = e.CreatedUser.FullName
                    },
                    Factory = new SlaFactoryModel
                    {
                        ID = e.Factory.ID,
                        Name = e.Factory.FactoryName
                    },
                    CreatedAt = e.InsertedAt,
                    UpdatedAt = e.UpdatedAt,
                    Levels = e.SlaLevels.Where(sl => !sl.IsDeleted).Select(sl => new SlaLevelModel()
                    {
                        ID = sl.ID,
                        LevelOrder = sl.LevelOrder,
                        ConditionType = sl.SlaCondition.Name,
                        OffsetDays = sl.OffsetDays,
                        OffsetHours = sl.OffsetHours,
                        OffsetMinutes = sl.OffsetMinutes,
                        IncludeHolidays = sl.IncludeHolidays,
                        IncludeWeekends = sl.IncludeWeekends,
                        WorkingHoursOnly = sl.WorkingHoursOnly,
                        SendEmail = sl.SendEmail,
                        EmailTemplateId = sl.EmailTemplateID,
                        UserGroups = sl.SlaLevelUserGroups.Where(u => !u.IsDeleted).Select(r => new SlaUserGroupModel
                        {
                            ID = r.ID,
                            UserGroupsID = r.UserGroupsID,
                            UserGroupName = r.UserGroups.UserGroupName,
                        }).ToList()
                    }).ToList()
                }).FirstOrDefault();
        }

        /// <summary>
        /// Create new SLA configuration
        /// </summary>
        public int AddSlaConfig(AddEditSlaConfigModel requestModel)
        {
            var entity = new SlaConfig
            {
                FactoryID = requestModel.FactoryID,
                SlaCategoryID = requestModel.CategoryID,
                Name = requestModel.Name,
                Description = requestModel.Description,
                IsActive = requestModel.IsActive,
                SlaLevels = requestModel.Levels?.Select(lvl => new SlaLevel
                {
                    LevelOrder = lvl.LevelOrder,
                    SlaConditionID = lvl.SlaConditionID,
                    OffsetDays = lvl.OffsetDays,
                    OffsetHours = lvl.OffsetHours,
                    OffsetMinutes = lvl.OffsetMinutes,
                    IncludeHolidays = lvl.IncludeHolidays,
                    IncludeWeekends = lvl.IncludeWeekends,
                    WorkingHoursOnly = lvl.WorkingHoursOnly,
                    SendEmail = lvl.SendEmail,
                    EmailTemplateID = lvl.EmailTemplateId > 0 ? lvl.EmailTemplateId : null,
                    SlaLevelUserGroups = lvl.UserGroups?.Select(u => new SlaLevelUserGroup
                    {
                        UserGroupsID = u.ID,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        IsDeleted = false
                    }).ToList(),
                    InsertedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    IsDeleted = false
                }).ToList(),
                InsertedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
                IsDeleted = false
            };

            _unitOfWork.ElogbookRepository<SlaConfig>().Add(entity);
            _unitOfWork.Complete();

            return entity.ID;
        }

        /// <summary>
        /// Update existing SLA configuration
        /// </summary>
        public bool UpdateSlaConfig(AddEditSlaConfigModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<SlaConfig>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity == null)
                return false;

            entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
            entity.SlaCategoryID = requestModel.CategoryID > 0 ? requestModel.CategoryID : entity.SlaCategoryID;
            entity.Name = string.IsNullOrEmpty(requestModel.Name) ? entity.Name : requestModel.Name;
            entity.Description = string.IsNullOrEmpty(requestModel.Description) ? entity.Description : requestModel.Description;
            entity.IsActive = requestModel.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.LastUpdatedUserID = requestModel.CurrentUserID;

            _unitOfWork.ElogbookRepository<SlaConfig>().Update(entity);

            return true;
        }

        /// <summary>
        /// Delete SLA configuration
        /// </summary>
        public bool DeleteSlaConfig(int id, int currentUserID)
        {
            var entity = _unitOfWork.ElogbookRepository<SlaConfig>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity == null)
                return false;

            // mark related levels and usergroups as deleted
            var levels = _unitOfWork.ElogbookRepository<SlaLevel>().GetQuery(x => x.SlaConfigID == id && !x.IsDeleted).ToList();
            foreach (var lvl in levels)
            {
                var userGroups = _unitOfWork.ElogbookRepository<SlaLevelUserGroup>().GetQuery(r => r.SlaLevelID == lvl.ID && !r.IsDeleted).ToList();
                foreach (var rl in userGroups)
                {
                    rl.IsDeleted = true;
                    rl.LastUpdatedUserID = currentUserID;
                    rl.UpdatedAt = DateTime.Now;
                    _unitOfWork.ElogbookRepository<SlaLevelUserGroup>().Update(rl);
                }

                lvl.IsDeleted = true;
                lvl.LastUpdatedUserID = currentUserID;
                lvl.UpdatedAt = DateTime.Now;
                _unitOfWork.ElogbookRepository<SlaLevel>().Update(lvl);
            }

            entity.IsDeleted = true;
            entity.LastUpdatedUserID = currentUserID;

            _unitOfWork.ElogbookRepository<SlaConfig>().Update(entity);
            _unitOfWork.Complete();

            return true;
        }

        /// <summary>
        /// Toggle SLA configuration active status
        /// </summary>
        public bool ToggleSlaConfigStatus(int id, int currentUserID)
        {
            var entity = _unitOfWork.ElogbookRepository<SlaConfig>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity == null)
                return false;

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.LastUpdatedUserID = currentUserID;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.ElogbookRepository<SlaConfig>().Update(entity);
            _unitOfWork.Complete();

            return true;
        }

        /// <summary>
        /// <summary>
        /// Check whether an SLA configuration with the same name, category and factory exists.
        /// excludeId: optional ID to exclude (useful when updating)
        /// </summary>
        public bool ExistsByNameCategoryFactory(string name, int categoryId, int factoryId, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name) || categoryId <= 0 || factoryId <= 0)
                return false;

            var query = _unitOfWork.ElogbookRepository<SlaConfig>()
                .GetQuery(x => !x.IsDeleted && x.SlaCategoryID == categoryId && x.Name == name && x.FactoryID == factoryId);

            if (excludeId.HasValue && excludeId.Value > 0)
                query = query.Where(x => x.ID != excludeId.Value);

            return query.Any();
        }

        /// <summary>
        /// Get all SLA conditions
        /// </summary>
        public List<SlaConditionModel> GetSlaConditions()
        {
            return _unitOfWork.ElogbookRepository<SlaCondition>()
                .GetQuery(x => !x.IsDeleted)
                .Select(c => new SlaConditionModel { ID = c.ID, Name = c.Name, Description = c.Description })
                .OrderBy(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// Get all SLA roles
        /// </summary>
        public List<SlaRoleModel> GetSlaRoles() => _unitOfWork.ElogbookRepository<SlaRole>()
                .GetQuery(x => !x.IsDeleted)
                .Select(r => new SlaRoleModel { ID = r.ID, Name = r.Name, Description = r.Description })
                .OrderBy(x => x.Name)
                .ToList();

        /// <summary>
        /// Get all SLA categories as combobox
        /// </summary>
        public List<SlaCategoryModel> GetSlaCategories()
        {
            return _unitOfWork.ElogbookRepository<SlaCategory>()
                .GetQuery(x => !x.IsDeleted)
                .Select(c => new SlaCategoryModel { ID = c.ID, Name = c.Name, Description = c.Description })
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}
