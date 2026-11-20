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
    public class CycleManagementService : ICycleManagementService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public CycleManagementService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int AddCycleManagement(AddEditCycleManagerModel requestModel)
        {
            var entity = new CycleManagement
            {
                CycleName = requestModel.CycleName,
                Position = requestModel.Position,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                OpenInputBefore = requestModel.BlockInputPreviousTime,
                BlockInputAfter = requestModel.BlockInputNextTime,
                TimeLine = requestModel.TimeLine,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            var cycleFactories = new List<CycleFactory>();
            if (requestModel.FactoryIDs != null && requestModel.FactoryIDs.Count > 0)
            {
                foreach (var factoryID in requestModel.FactoryIDs)
                {
                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == factoryID && !x.IsDeleted && x.Status == (int)AccessStatus.Active).FirstOrDefault();
                    if (factory != null)
                    {
                        cycleFactories.Add(new CycleFactory()
                        {
                            FactoryID = factoryID,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                        });
                    }
                }
            }
            if (cycleFactories.Count > 0)
                entity.CycleFactories = cycleFactories;

            _unitOfWork.ElogbookRepository<CycleManagement>().Add(entity);
            _unitOfWork.Complete();

            // Thêm mới mốc thời gian cho mỗi Chu kỳ
            AddDeleteTimeLineCycle(entity.ID, entity.TimeLine, requestModel.CurrentUserID, true);

            return entity.ID;
        }

        public ResponseBaseModel GetCycleManagements(SearchCycleManagementModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            // Dữ liệu đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => !x.IsDeleted);

            // Tên chu kỳ
            if (!string.IsNullOrWhiteSpace(requestModel.CycleName))
            {
                entities = entities.Where(x => x.CycleName.Contains(requestModel.CycleName));
            }
            // Ngày tạo (từ ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.FromDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date);
            }
            // Ngày tạo (đến ngày)
            if (!string.IsNullOrWhiteSpace(requestModel.ToDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date);
            }
            // Trạng thái sử dụng. 
            if (requestModel.Active > 0)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }
            // Trường hợp tìm kiếm theo Nhà máy
            if (requestModel.FactoryID > 0)
            {
                entities = entities.Where(x => x.CycleFactories.Any(t => t.FactoryID == requestModel.FactoryID));
            }

            // Return and paging data
            var page = entities.Select(
                e => new CycleManagementModel()
                {
                    ID = e.ID,
                    CycleName = e.CycleName,
                    CycleFactories = e.CycleFactories.Select(t => new ValueLabelModel() { Value = t.Factory.ID, Label = t.Factory.FactoryName }).ToList(),
                    Position = e.Position,
                    Active = e.Active,
                    BlockInputPreviousTime = e.OpenInputBefore,
                    BlockInputNextTime = e.BlockInputAfter,
                    TimeLine = e.TimeLine,
                    InsertedAt = e.InsertedAt
                }).OrderBy(t => t.Position)
                .ThenBy(t => t.CycleName)
                .ThenByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public CycleManagementModel GetCycleManagementById(int id)
        {
            return _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new CycleManagementModel()
                {
                    ID = e.ID,
                    CycleName = e.CycleName,
                    CycleFactories = e.CycleFactories.Select(t => new ValueLabelModel() { Value = t.Factory.ID, Label = t.Factory.FactoryName }).ToList(),
                    Position = e.Position,
                    Active = e.Active,
                    BlockInputPreviousTime = e.OpenInputBefore,
                    BlockInputNextTime = e.BlockInputAfter,
                    TimeLine = e.TimeLine
                }).FirstOrDefault();
        }

        public bool UpdateCycleManagement(AddEditCycleManagerModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<CycleManagement>().GetById(requestModel.ID);
            if (entity != null)
            {
                entity.CycleName = requestModel.CycleName;
                entity.Position = requestModel.Position;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.OpenInputBefore = requestModel.BlockInputPreviousTime;
                entity.BlockInputAfter = requestModel.BlockInputNextTime;
                entity.TimeLine = requestModel.TimeLine;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<CycleManagement>().Update(entity);

                string[] hours = requestModel.TimeLine.Split(',').Select(str => str.Trim()).ToArray();
                // Xóa mốc thời gian không còn sử dụng của mỗi chu kỳ
                var deletes = _unitOfWork.ElogbookRepository<TimeLineCycle>().GetQuery().Where(x => x.CycleID == entity.ID
                                                                                                    && !x.IsDeleted
                                                                                                    && x.Active == (int)AccessStatus.Active
                                                                                                    && !hours.Contains(x.Hour.ToString()));
                _unitOfWork.ElogbookRepository<TimeLineCycle>().Delete(deletes, requestModel.CurrentUserID, true);

                if (requestModel.FactoryIDs != null && requestModel.FactoryIDs.Count > 0)
                {
                    foreach (var factoryID in requestModel.FactoryIDs)
                    {
                        var cycleFactory = _unitOfWork.ElogbookRepository<CycleFactory>().GetQuery(x => x.CycleID == entity.ID && x.FactoryID == factoryID).FirstOrDefault();
                        // Thêm nhà máy sử dụng Chu kỳ
                        if (cycleFactory == null)
                        {
                            var factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == factoryID && !x.IsDeleted && x.Status == (int)AccessStatus.Active).FirstOrDefault();
                            if (factory != null)
                            {
                                cycleFactory = new CycleFactory
                                {
                                    CycleID = entity.ID,
                                    FactoryID = factoryID,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID
                                };
                                _unitOfWork.ElogbookRepository<CycleFactory>().Add(cycleFactory);
                            }
                        }
                    }

                    // Xóa Nhà máy không sử dụng Chu kỳ
                    var cycleFactories = entity.CycleFactories.Where(x => !requestModel.FactoryIDs.Contains(x.FactoryID) && x.CycleID == entity.ID).ToList();
                    _unitOfWork.ElogbookRepository<CycleFactory>().Delete(cycleFactories, requestModel.CurrentUserID, true);
                }

                // Thêm mới mốc thời gian cho mỗi chu kỳ
                AddDeleteTimeLineCycle(entity.ID, entity.TimeLine, requestModel.CurrentUserID, false);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteCycleManagement(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<CycleManagement>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<CycleManagement>().Delete(entity, userID);

                var deletes = _unitOfWork.ElogbookRepository<TimeLineCycle>().GetQuery().Where(x => x.CycleID == entity.ID
                                                                                                    && !x.IsDeleted
                                                                                                    && x.Active == (int)AccessStatus.Active);

                _unitOfWork.ElogbookRepository<TimeLineCycle>().Delete(deletes, userID);

                var saveSuccess = _unitOfWork.Complete() > 0;
                if (saveSuccess) return true;
                return false;
            }

            return false;
        }

        public bool CheckInUseCycleManagement(int id)
        {
            var isInUse = false;
            isInUse = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.CycleID == id && !x.IsDeleted).Any();
            if (!isInUse)
                _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.CycleID == id && !x.IsDeleted).Any();
            if (!isInUse)
                _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.CycleID == id && !x.IsDeleted).Any();
            if (!isInUse)
                _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == id && !x.IsDeleted).Any();

            return isInUse;
        }

        public bool CheckDuplicateData(int cycleID, string cycleName, bool isAddMode)
        {
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => x.CycleName.Equals(cycleName, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => x.ID != cycleID
                                                                                    && x.CycleName.Equals(cycleName, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        private void AddDeleteTimeLineCycle(int id, string timeLines, int userID, bool isNew = false)
        {
            string[] hours = timeLines.Split(',').Select(str => str.Trim()).ToArray();
            if (id > 0 && hours.Length > 0)
            {
                foreach (var item in hours)
                {
                    int.TryParse(item.Trim(), out int hour);
                    if ((isNew && hour > 0)
                        || (hour > 0 && !_unitOfWork.ElogbookRepository<TimeLineCycle>().GetQuery(x => x.CycleID == id && !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.Hour == hour).Any()))
                    {
                        var timeLine = new TimeLineCycle
                        {
                            Hour = hour,
                            CycleID = id,
                            Active = (int)AccessStatus.Active,
                            CreatedUserID = userID,
                            LastUpdatedUserID = userID,
                        };

                        _unitOfWork.ElogbookRepository<TimeLineCycle>().Add(timeLine);
                    }
                }

                if (isNew)
                    _unitOfWork.Complete();
            }
        }
    }
}
