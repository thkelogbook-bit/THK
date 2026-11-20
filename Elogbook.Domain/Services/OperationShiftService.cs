using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.OperationShift;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class OperationShiftService : IOperationShiftService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public OperationShiftService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetOperationShifts(SearchOperationShiftModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            if (requestModel.DateType <= 0) requestModel.DateType = CommonConstant.ALL_KEY;

            var operationShifts = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => !x.IsDeleted);
            // Tên ca vận hành
            if (!string.IsNullOrWhiteSpace(requestModel.OperationShiftName))
                operationShifts = operationShifts.Where(x => x.OperationShiftName.Contains(requestModel.OperationShiftName));
            // Nhà máy
            if (requestModel.FactoryID > 0)
                operationShifts = operationShifts.Where(x => x.FactoryID == requestModel.FactoryID);
            // Trạng thái sử dụng
            if (requestModel.Active > 0)
                operationShifts = operationShifts.Where(x => x.Active == requestModel.Active);

            //Tìm kiếm theo ngày sử dụng
            //Nếu có nhập Từ ngày hoặc Đến ngày thì thực hiện tìm kiếm theo khoảng thời gian sử dụng, ngược lại thì lấy tất cả dữ liệu ca vận hành theo điều kiện khác
            if (string.IsNullOrEmpty(requestModel.FromDate) && string.IsNullOrEmpty(requestModel.ToDate))
            {
                if (requestModel.DateType == 2)
                {
                    var usedTimes = _unitOfWork.ElogbookRepository<UsedTimes>().GetQuery(x => !x.IsDeleted).Select(u => new
                    {
                        OperationShiftID = (int)u.OperationShiftID,
                    }).Distinct();

                    var data = (from op in operationShifts
                                join ut in usedTimes
                                on op.ID equals ut.OperationShiftID
                                select new OperationShiftModel()
                                {
                                    ID = op.ID,
                                    OperationShiftName = op.OperationShiftName,
                                    Position = op.Position,
                                    Factory = new ValueLabelModel { Value = op.Factory.ID, Label = op.Factory.FactoryName },
                                    StartTime = SqlFunctions.StringConvert(op.StartTime, 6, 2).Trim().Replace(".", ":"),
                                    EndTime = SqlFunctions.StringConvert(op.EndTime, 6, 2).Trim().Replace(".", ":"),
                                    UsedTimes = op.UsedTimes.Select(
                                                t => new UsedTimesModel()
                                                {
                                                    ActiveTime = t.ActiveTime,
                                                    InactiveTime = t.InactiveTime
                                                }).ToList(),
                                    InsertedAt = op.InsertedAt,
                                    Active = op.Active,
                                }).OrderBy(t => t.Position)
                                .ThenBy(t => t.OperationShiftName)
                                .ThenByDescending(t => t.InsertedAt)
                                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                    return HttpResponse.OkList(data);
                }

                var page = operationShifts.Select(
                            e => new OperationShiftModel()
                            {
                                ID = e.ID,
                                OperationShiftName = e.OperationShiftName,
                                Position = e.Position,
                                Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                                StartTime = SqlFunctions.StringConvert(e.StartTime, 6, 2).Trim().Replace(".", ":"),
                                EndTime = SqlFunctions.StringConvert(e.EndTime, 6, 2).Trim().Replace(".", ":"),
                                UsedTimes = e.UsedTimes.Select(
                                                t => new UsedTimesModel()
                                                {
                                                    ActiveTime = t.ActiveTime,
                                                    InactiveTime = t.InactiveTime
                                                }).ToList(),
                                InsertedAt = e.InsertedAt,
                                Active = e.Active,
                            })
                            .OrderBy(t => t.Position)
                            .ThenBy(t => t.OperationShiftName)
                            .ThenByDescending(t => t.InsertedAt)
                            .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                return HttpResponse.OkList(page);
            }
            else
            {
                if (requestModel.DateType == 2 && (frDate != DateTime.MinValue || toDate != DateTime.MaxValue))
                {
                    var usedTimes = _unitOfWork.ElogbookRepository<UsedTimes>().GetQuery(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date
                                                                                               && DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date
                                                                                               && !x.IsDeleted).Select(u => new
                                                                                               {
                                                                                                   OperationShiftID = (int)u.OperationShiftID,
                                                                                               }).Distinct();

                    var page = (from op in operationShifts
                                join ut in usedTimes
                                on op.ID equals ut.OperationShiftID
                                select new OperationShiftModel()
                                {
                                    ID = op.ID,
                                    OperationShiftName = op.OperationShiftName,
                                    Position = op.Position,
                                    Factory = new ValueLabelModel { Value = op.Factory.ID, Label = op.Factory.FactoryName },
                                    StartTime = SqlFunctions.StringConvert(op.StartTime, 6, 2).Trim().Replace(".", ":"),
                                    EndTime = SqlFunctions.StringConvert(op.EndTime, 6, 2).Trim().Replace(".", ":"),
                                    UsedTimes = op.UsedTimes.Select(
                                                t => new UsedTimesModel()
                                                {
                                                    ActiveTime = t.ActiveTime,
                                                    InactiveTime = t.InactiveTime
                                                }).ToList(),
                                    InsertedAt = op.InsertedAt,
                                    Active = op.Active,
                                }).OrderBy(t => t.Position)
                                .ThenBy(t => t.OperationShiftName)
                                .ThenByDescending(t => t.InsertedAt)
                                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                    return HttpResponse.OkList(page);
                }
                //Tìm kiếm theo ngày tạo, nếu không nhập Từ ngày, Đến ngày thì tìm kiếm tất cả dữ liệu theo những điều kiện khác
                else
                {
                    var page = operationShifts?.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date && DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date)
                        .Select(
                            e => new OperationShiftModel()
                            {
                                ID = e.ID,
                                OperationShiftName = e.OperationShiftName,
                                Position = e.Position,
                                Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                                StartTime = SqlFunctions.StringConvert(e.StartTime, 6, 2).Trim().Replace(".", ":"),
                                EndTime = SqlFunctions.StringConvert(e.EndTime, 6, 2).Trim().Replace(".", ":"),
                                UsedTimes = e.UsedTimes.Select(
                                                t => new UsedTimesModel()
                                                {
                                                    ActiveTime = t.ActiveTime,
                                                    InactiveTime = t.InactiveTime
                                                }).ToList(),
                                InsertedAt = e.InsertedAt,
                                Active = e.Active,
                            })
                            .OrderBy(t => t.Position)
                            .ThenBy(t => t.OperationShiftName)
                            .ThenByDescending(t => t.InsertedAt)
                            .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                    return HttpResponse.OkList(page);
                }
            }
        }

        public OperationShiftModel GetOperationShiftById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
               e => new OperationShiftModel()
               {
                   ID = e.ID,
                   OperationShiftName = e.OperationShiftName,
                   Position = e.Position,
                   Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                   StartTime = SqlFunctions.StringConvert(e.StartTime, 6, 2).Trim().Replace(".", ":"),
                   EndTime = SqlFunctions.StringConvert(e.EndTime, 6, 2).Trim().Replace(".", ":"),
                   Active = e.Active,
               }).FirstOrDefault();

            return entity;
        }

        public OperationShiftModel AddOperationShift(AddEditOperationShiftModel requestModel)
        {
            double.TryParse(requestModel.StartTime.Replace(":", "."), out double startTime);
            double.TryParse(requestModel.EndTime.Replace(":", "."), out double endTime);

            var entity = new OperationShift
            {
                OperationShiftName = requestModel.OperationShiftName.Trim(),
                Position = requestModel.Position,
                FactoryID = requestModel.FactoryID,
                StartTime = startTime,
                EndTime = endTime,
                Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<OperationShift>().Add(entity);
            _unitOfWork.Complete();

            // Nếu tạo mới Ca vận hành thành công và trạng thái Có thể sử dụng thì thêm mới thời gian sử dụng Ca vận hành
            if (requestModel.Active == (int)AccessStatus.Active && entity.ID > 0)
            {
                var useTime = new UsedTimes
                {
                    OperationShiftID = entity.ID,
                    ActiveTime = DateTime.Now,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                };
                _unitOfWork.ElogbookRepository<UsedTimes>().Add(useTime);
                _unitOfWork.Complete();
            }

            return GetOperationShiftById(entity.ID);
        }

        public OperationShiftModel UpdateOperationShift(AddEditOperationShiftModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationShift>().GetById(requestModel.ID);
            if (entity != null)
            {
                var oldActive = entity.Active;
                double.TryParse(requestModel.StartTime.Replace(":", "."), out double startTime);
                double.TryParse(requestModel.EndTime.Replace(":", "."), out double endTime);

                entity.OperationShiftName = requestModel.OperationShiftName.Trim();
                entity.Position = requestModel.Position;
                entity.FactoryID = requestModel.FactoryID;
                entity.StartTime = startTime;
                entity.EndTime = endTime;
                entity.Active = requestModel.Active <= 0 ? (int)AccessStatus.InActive : requestModel.Active;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<OperationShift>().Update(entity);

                // Nếu thay đổi trạng thái sử dụng
                if (oldActive != requestModel.Active)
                {
                    // Nếu chuyển trạng thái từ Không thể sử dụng sang Có thể sử dụng thì thêm mới row dữ liệu để tracking
                    if (entity.Active == (int)AccessStatus.Active)
                    {
                        var useTime = new UsedTimes
                        {
                            OperationShiftID = entity.ID,
                            ActiveTime = DateTime.Now,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                        };
                        _unitOfWork.ElogbookRepository<UsedTimes>().Add(useTime);
                    }
                    // Nếu ngược lại thì cập nhật thời gian Không thể sử dụng vào row đã tồn tại
                    else
                    {
                        var useTime = _unitOfWork.ElogbookRepository<UsedTimes>().GetQuery(x => x.OperationShiftID == entity.ID && x.InactiveTime == null).FirstOrDefault();
                        if (useTime != null)
                        {
                            useTime.OperationShiftID = entity.ID;
                            useTime.InactiveTime = DateTime.Now;
                            useTime.LastUpdatedUserID = requestModel.CurrentUserID;
                            _unitOfWork.ElogbookRepository<UsedTimes>().Update(useTime);
                        }
                    }
                }

                _unitOfWork.Complete();

                return GetOperationShiftById(entity.ID);
            }

            return new OperationShiftModel();
        }

        public bool DeleteOperationShift(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationShift>().GetById(id);
            if (entity != null && _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationShiftID == id && !x.IsDeleted).Any())
            {
                _unitOfWork.ElogbookRepository<OperationShift>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckDuplicateData(int id, string operationShiftName, int factoryID, bool isAddMode)
        {
            // Trường hợp thêm mới thì check Tên ca vận hành ko được trùng trong cùng 1 Nhà máy
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.OperationShiftName.Equals(operationShiftName, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì check Tên Cương vị, Nhà máy và những Tổ máy khác Tổ máy đang cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.ID != id
                                                                                    && x.OperationShiftName.Equals(operationShiftName, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public bool CheckInUseOperationShift(int id)
        {
            return _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationShiftID == id && !x.IsDeleted).Any();
        }

        public bool CheckDuplicateTime(int id, int factoryID, double startTime, double endTime, bool isAddMode)
        {
            bool isDuplicate = false;
            if (startTime > endTime)
            {
                var query = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == factoryID
                                                                                  && x.Active == (int)AccessStatus.Active
                                                                                  && !x.IsDeleted
                                                                                  && (id > 0 ? x.ID != id : x.ID != 0));

                var listShifts = query.ToList();

                foreach (var itm in listShifts)
                {
                    if (itm.StartTime > itm.EndTime)
                    {
                        isDuplicate = true; break;
                    }
                    else
                    {
                        if (startTime < itm.EndTime || endTime > itm.StartTime)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                var query = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == factoryID
                                                                                  && x.Active == (int)AccessStatus.Active
                                                                                  && !x.IsDeleted
                                                                                  && (id > 0 ? x.ID != id : x.ID != 0));

                var listShifts = query.ToList();
                foreach (var itm in listShifts)
                {
                    if (itm.StartTime > itm.EndTime)
                    {
                        if (endTime > itm.StartTime || startTime < itm.EndTime)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                    else
                    {
                        if (startTime < itm.EndTime && endTime > itm.StartTime)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                }
            }

            return isDuplicate;
        }
    }
}
