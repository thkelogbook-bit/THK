using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class OperationPositionService : IOperationPositionService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public OperationPositionService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int AddOperationPosition(AddEditOperationPositionModel requestModel)
        {
            //Mapping thông tin cương vị thêm mới
            var entity = new OperationPosition
            {
                OperationPositionName = requestModel.OperationPositionName.Trim(),
                FactoryID = requestModel.FactoryID,
                Quantity = requestModel.Quantity,
                AllowWriteDiaryPortal = requestModel.AllowWriteDiaryPortal,
                AllowWriteDiaryMobile = requestModel.AllowWriteDiaryMobile,
                AllowWriteParameterPortal = requestModel.AllowWriteParameterPortal,
                AllowWriteParameterMobile = requestModel.AllowWriteParameterMobile,
                AuthenticationType = requestModel.AthenticationType,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                JobTitle = requestModel.JobTitle,
                IsUsingOnlyOneTablet = requestModel.IsUsingOnlyOneTablet,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
                TemplateDeliveryShiftMethod = requestModel.TemplateDeliveryShiftMethod,
                Position = requestModel.Position,
                IsConcurrentlyInShift = requestModel.IsConcurrentlyInShift,
                ShiftReceivingTemplate = requestModel.ShiftReceivingTemplate,
            };

            //Lưu cương vị
            _unitOfWork.ElogbookRepository<OperationPosition>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;

            //Sau khi tạo cương vị thành công thì tạo danh sách cương vị trong ca
            if (saveSuccess)
            {
                List<PositionInShift> lstPositionInShift = new List<PositionInShift>();
                // Toàn bộ cương vị trong nhà máy
                var entities = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID != entity.ID && x.FactoryID == requestModel.FactoryID && !x.IsDeleted).ToList();
                var position = 0;
                switch (requestModel.JobTitle)
                {
                    //Nếu cương vị là Trưởng ca/Trưởng kíp thì thêm toàn bộ các cương vị khác đã có vào danh sách cương vị trong ca
                    case (int)JobTitle.ShiftLeader:
                    case (int)JobTitle.TeamLeader:
                        var leaders = entities.Select(e => new PositionInShift
                        {
                            IsSystem = e.JobTitle == 1 || e.JobTitle == 2,
                            Quantity = e.Quantity,
                            OperationPositionID = e.ID
                        }).ToList();

                        // Thêm danh sách cương vị trong ca
                        foreach (var item in leaders)
                        {
                            position++;
                            lstPositionInShift.Add(AddPositionInShift(item, requestModel.CurrentUserID, entity.ID, position));
                        }

                        // Duyệt qua tất cả những cương vị khác để thêm cương vị Trưởng ca/Trưởng kíp vào danh sách cương vị trong ca
                        foreach (var item in entities)
                        {
                            var positionInShift = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.ParentID == item.ID && !x.IsDeleted).ToList();
                            if (!positionInShift.Exists(x => x.ParentID == entity.ID))
                            {
                                var maxPosition = positionInShift?.Max(x => x.Position);
                                if (!maxPosition.HasValue) maxPosition = 1;
                                else
                                    maxPosition++;

                                lstPositionInShift.Add(new PositionInShift
                                {
                                    ParentID = item.ID,
                                    IsSystem = true,
                                    Position = (int)maxPosition,
                                    Quantity = requestModel.Quantity,
                                    Active = (int)AccessStatus.Active,
                                    OperationPositionID = entity.ID,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                });
                            }
                        }
                        break;

                    case (int)JobTitle.Operator:
                        var operators = entities.Where(x => x.JobTitle == 1 || x.JobTitle == 2).Select(e => new PositionInShift
                        {
                            IsSystem = e.JobTitle == 1 || e.JobTitle == 2,
                            Quantity = e.Quantity,
                            OperationPositionID = e.ID
                        }).ToList();

                        foreach (var item in operators)
                        {
                            position++;
                            lstPositionInShift.Add(AddPositionInShift(item, requestModel.CurrentUserID, entity.ID, position));
                        }
                        break;
                }

                position++;
                lstPositionInShift.Add(new PositionInShift
                {
                    ParentID = entity.ID,
                    IsSystem = true,
                    Position = position,
                    Quantity = requestModel.Quantity,
                    Active = (int)AccessStatus.Active,
                    OperationPositionID = entity.ID,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                });

                _unitOfWork.ElogbookRepository<PositionInShift>().AddRange(lstPositionInShift);
                saveSuccess = _unitOfWork.Complete() > 0;
                if (saveSuccess) return entity.ID;
            }

            return 0;
        }

        public ValueLabelModel CheckDuplicateData(int operationPositionID, string operationPositionName, int factoryID, bool isAddMode, int jobTitle)
        {
            ValueLabelModel existing = null;
            // Trường hợp thêm mới thì check Tên Cương vị ko được trùng trong cùng 1 Nhà máy
            if (isAddMode)
            {
                existing = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.OperationPositionName.Equals(operationPositionName, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Select(e => new ValueLabelModel()
                                                                                    {
                                                                                        Value = -1,
                                                                                        Label = e.Factory.FactoryName
                                                                                    }).FirstOrDefault();
                if (existing == null && (jobTitle == (int)JobTitle.ShiftLeader || jobTitle == (int)JobTitle.TeamLeader))
                {
                    return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.FactoryID == factoryID
                                                                                     && x.JobTitle == jobTitle
                                                                                     && !x.IsDeleted).Select(e => new ValueLabelModel()
                                                                                     {
                                                                                         Value = e.ID,
                                                                                         Label = e.Factory.FactoryName
                                                                                     }).FirstOrDefault();
                }
                else
                {
                    return existing;
                }
            }
            // Trường hợp cập nhật thì check Tên Cương vị, Nhà máy và những Tổ máy khác Tổ máy đang cập nhật
            else
            {
                existing = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.ID != operationPositionID
                                                                                    && x.OperationPositionName.Equals(operationPositionName, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Select(e => new ValueLabelModel()
                                                                                    {
                                                                                        Value = -1,
                                                                                        Label = e.Factory.FactoryName
                                                                                    }).FirstOrDefault();

                if (existing == null && (jobTitle == (int)JobTitle.ShiftLeader || jobTitle == (int)JobTitle.TeamLeader))
                {
                    return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID != operationPositionID
                                                                                     && x.FactoryID == factoryID
                                                                                     && x.JobTitle == jobTitle
                                                                                     && !x.IsDeleted).Select(e => new ValueLabelModel()
                                                                                     {
                                                                                         Value = e.ID,
                                                                                         Label = e.Factory.FactoryName
                                                                                     }).FirstOrDefault();
                }
                else
                {
                    return existing;
                }
            }
        }

        public bool CheckInUseOperationPosition(int id)
        {
            return _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationPositionID == id && !x.IsDeleted).Any();
        }

        public bool DeleteOperationPosition(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID == id && !x.IsDeleted);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<OperationPosition>().Delete(entity, userID);

                var positionInShift = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.ParentID == id && !x.IsDeleted);
                _unitOfWork.ElogbookRepository<PositionInShift>().Delete(positionInShift, userID);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public OperationPositionModel GetOperationPositionById(int id)
        {
            return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new OperationPositionModel()
                {
                    ID = e.ID,
                    OperationPositionName = e.OperationPositionName,
                    Quantity = e.Quantity,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    Active = e.Active,
                    JobTitle = e.JobTitle,
                    IsUsingOnlyOneTablet = e.IsUsingOnlyOneTablet,
                    AuthenticationTypeValue = e.AuthenticationType,
                    AllowWriteDiaryMobile = e.AllowWriteDiaryMobile,
                    AllowWriteDiaryPortal = e.AllowWriteDiaryPortal,
                    AllowWriteParameterMobile = e.AllowWriteParameterMobile,
                    AllowWriteParameterPortal = e.AllowWriteParameterPortal,
                    TemplateDeliveryShiftMethod = e.TemplateDeliveryShiftMethod,
                    Position = e.Position,
                    ShiftReceivingTemplate = e.ShiftReceivingTemplate,
                    IsConcurrentlyInShift = e.IsConcurrentlyInShift,
                }).FirstOrDefault();
        }

        public ResponseBaseModel GetOperationPositions(SearchOperationPositionModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            // Filter data by condition
            var entities = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date
                                                                                && DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date
                                                                                && !x.IsDeleted);
            //Nhà máy
            if (requestModel.FactoryID > 0)
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            //Tên cương vị
            if (!string.IsNullOrWhiteSpace(requestModel.OperationPositionName))
                entities = entities.Where(x => x.OperationPositionName.Contains(requestModel.OperationPositionName.Trim()));
            //Trạng thái sử dụng
            if (requestModel.Active > 0)
                entities = entities.Where(x => x.Active == requestModel.Active);
            //Kiểu xác thực Sử dụng: 0/Không sử dụng, 1/RFID, 2/QR Code
            if (requestModel.AthenticationType >= 0)
                entities = entities.Where(x => x.AuthenticationType == requestModel.AthenticationType);

            // Return and paging data
            var page = (from p in entities
                        join ps in _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery() on p.ID equals ps.OperationPositionID into lj
                        from r in lj.DefaultIfEmpty()
                        select new OperationPositionModel()
                        {
                            ID = p.ID,
                            OperationPositionName = p.OperationPositionName,
                            Quantity = p.Quantity,
                            Factory = new ValueLabelModel { Value = p.Factory.ID, Label = p.Factory.FactoryName },
                            InsertedAt = p.InsertedAt,
                            JobTitle = p.JobTitle,
                            AuthenticationTypeValue = p.AuthenticationType,
                            IsHasPositionInShift = r != null && r.OperationPositionID > 0,
                            Active = p.Active,
                            Position = p.Position,
                            IsConcurrentlyInShift = p.IsConcurrentlyInShift,
                        }).Distinct()
                .OrderBy(t => t.Factory.Label)
                .ThenBy(t => t.Position)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool UpdateOperationPosition(AddEditOperationPositionModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID == requestModel.ID).FirstOrDefault();
            if (entity != null)
            {
                int factoryID = (int)entity.FactoryID;

                entity.OperationPositionName = requestModel.OperationPositionName.Trim();
                entity.FactoryID = requestModel.FactoryID;
                entity.Quantity = requestModel.Quantity;
                entity.JobTitle = requestModel.JobTitle;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.AuthenticationType = requestModel.AthenticationType;
                entity.IsUsingOnlyOneTablet = requestModel.IsUsingOnlyOneTablet;
                entity.AllowWriteDiaryMobile = requestModel.AllowWriteDiaryMobile;
                entity.AllowWriteDiaryPortal = requestModel.AllowWriteDiaryPortal;
                entity.AllowWriteParameterPortal = requestModel.AllowWriteParameterPortal;
                entity.AllowWriteParameterMobile = requestModel.AllowWriteParameterMobile;
                entity.TemplateDeliveryShiftMethod = requestModel.TemplateDeliveryShiftMethod;
                entity.Position = requestModel.Position;
                entity.IsConcurrentlyInShift = requestModel.IsConcurrentlyInShift;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.ShiftReceivingTemplate = requestModel.ShiftReceivingTemplate;

                _unitOfWork.ElogbookRepository<OperationPosition>().Update(entity);

                // Cập nhật những cương vị trong ca đã tồn tại theo cương vị
                var updateEntities = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.OperationPositionID == requestModel.ID && !x.IsDeleted).ToList();
                foreach (var item in updateEntities)
                {
                    item.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                    item.Quantity = requestModel.Quantity;
                    if (item.LastUpdatedUserID != requestModel.CurrentUserID)
                        item.LastUpdatedUserID = requestModel.CurrentUserID;

                    _unitOfWork.ElogbookRepository<PositionInShift>().Update(item);
                }

                if (factoryID != requestModel.FactoryID)
                {
                    int cycelID = 0;
                    // Cập nhật nhà máy cho những nhóm thông số, nếu nhà máy có thay đổi
                    var parameterGroupEntities = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.OperationPositionID == requestModel.ID && !x.IsDeleted).ToList();

                    foreach (var item in parameterGroupEntities)
                    {
                        if (item.CycleID.HasValue)
                        {
                            cycelID = (int)item.CycleID;
                        }
                        item.FactoryID = requestModel.FactoryID;
                        if (item.LastUpdatedUserID != requestModel.CurrentUserID)
                            item.LastUpdatedUserID = requestModel.CurrentUserID;

                        _unitOfWork.ElogbookRepository<ParameterGroup>().Update(item);
                    }

                    // Cập nhật nhà máy cho những thông số, nếu nhà máy có thay đổi
                    var parameterEntities = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.OperationPositionID == requestModel.ID && !x.IsDeleted).ToList();
                    foreach (var item in parameterEntities)
                    {
                        item.FactoryID = requestModel.FactoryID;
                        if (item.LastUpdatedUserID != requestModel.CurrentUserID)
                            item.LastUpdatedUserID = requestModel.CurrentUserID;

                        _unitOfWork.ElogbookRepository<Parameter>().Update(item);
                    }

                    // Cập nhật nhà máy cho chu kỳ nhà máy, nếu nhà máy có thay đổi
                    if (cycelID > 0)
                    {
                        var cycleFactory = _unitOfWork.ElogbookRepository<CycleFactory>().GetQuery(x => x.FactoryID == factoryID && x.CycleID == cycelID && !x.IsDeleted).FirstOrDefault();
                        if (cycleFactory != null)
                        {
                            cycleFactory.FactoryID = requestModel.FactoryID;
                            if (cycleFactory.LastUpdatedUserID != requestModel.CurrentUserID)
                                cycleFactory.LastUpdatedUserID = requestModel.CurrentUserID;

                            _unitOfWork.ElogbookRepository<CycleFactory>().Update(cycleFactory);
                        }
                    }
                }

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        private PositionInShift AddPositionInShift(PositionInShift item, int userID, int parentID, int position)
        {
            return new PositionInShift
            {
                ParentID = parentID,
                IsSystem = item.IsSystem,
                Position = position,
                Quantity = item.Quantity,
                Active = (int)AccessStatus.Active,
                OperationPositionID = item.OperationPositionID,
                CreatedUserID = userID,
                LastUpdatedUserID = userID,
            };
        }
    }
}
