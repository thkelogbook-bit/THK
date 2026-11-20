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
using System.Data.SqlClient;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class ParameterService : IParameterService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public ParameterService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetParameters(SearchParameterForStopModel requestModel)
        {

            // Danh sách nhóm thông số theo chu kỳ
            var parameterGroups = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.CycleID == requestModel.CycelID
                                                                                            && x.FactoryID == requestModel.FactoryID
                                                                                            && x.OperationPositionID == requestModel.OperationPositionID
                                                                                            && x.Status == (int)AccessStatus.Active
                                                                                            && !x.IsDeleted).Select(e => new { e.ID, e.ParameterGroupName }).ToList();

            // Thông số của cac nhóm thông số
            var parameters = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => !x.IsDeleted && x.Status == 1
                                            && x.FactoryID == requestModel.FactoryID
                                            && x.CycleID == requestModel.CycelID
                                            && x.OperationPositionID == requestModel.OperationPositionID
                                            ).Select(e => new { e.ID, e.ParameterName, e.ParameterGroupID, e.IsGroup }).ToList();

            var lstParameterGroup = new List<ParameterGroupDetailModel>();
            foreach (var item in parameterGroups)
            {
                var parameterOfGroups = parameters.Where(x => x.ParameterGroupID == item.ID).ToList();
                if (parameterOfGroups.Count > 0)
                {
                    // Tổng thông số
                    int totalParameters = parameterOfGroups.Count;

                    ParameterGroupDetailModel opg = new ParameterGroupDetailModel()
                    {
                        ParameterGroupID = item.ID,
                        ParameterGroupName = item.ParameterGroupName,
                        TotalParameters = totalParameters,
                        Parameters = parameterOfGroups.Select(x => new ValueLabelCheckedModel() { Label = x.ParameterName, Value = x.ID }).ToList()
                    };
                    lstParameterGroup.Add(opg);
                }

            }

            var otherParameterOfGroups = parameters.Where(x => !x.IsGroup).ToList();
            if (otherParameterOfGroups.Count > 0)
            {
                ParameterGroupDetailModel other = new ParameterGroupDetailModel()
                {
                    ParameterGroupID = -1,
                    ParameterGroupName = "Khác",
                    TotalParameters = otherParameterOfGroups.Count,
                    Parameters = otherParameterOfGroups.Select(x => new ValueLabelCheckedModel() { Label = x.ParameterName, Value = x.ID }).ToList()
                };
                lstParameterGroup.Add(other);
            }

            return HttpResponse.OkList(lstParameterGroup);
        }

        public ResponseBaseModel GetParameters(SearchParameterModel requestModel)
        {
            // Tất cả thông số theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => !x.IsDeleted);
            // Tên thông số
            if (!string.IsNullOrWhiteSpace(requestModel.Name))
            {
                entities = entities.Where(x => x.ParameterName.Contains(requestModel.Name));
            }
            // Nhà máy
            if (requestModel.FactoryID > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }
            // Chu kỳ
            if (requestModel.CycelID > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.CycleID == requestModel.CycelID);
            }
            // Cương vị
            if (requestModel.OperationPositionID > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }
            //Trạng thái sử dụng
            if (requestModel.Status > CommonConstant.ZERO_KEY_COMPARE)
            {
                entities = entities.Where(x => x.Status == requestModel.Status);
            }
            // Thêm điều kiện thuộc nhóm thông số
            if (requestModel.ParameterType == CommonConstant.PARAMETERTYPE_GROUP)
            {
                entities = entities.Where(x => x.IsGroup && (x.ParameterGroupID == requestModel.ParameterGroupID || requestModel.ParameterGroupID == CommonConstant.ALL_KEY));
            }
            // Thêm điều kiện tìm theo thông số lẻ
            else if (requestModel.ParameterType == CommonConstant.PARAMETERTYPE_NOGROUP)
            {
                entities = entities.Where(x => !x.IsGroup);
            }

            var page = entities.Select(
                e => new ParameterModel()
                {
                    ID = e.ID,
                    IsGroup = e.IsGroup,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                    ParameterGroup = new ValueLabelModel { Value = e.ParameterGroup.ID, Label = e.ParameterGroup.ParameterGroupName },
                    RFIDCode = e.OperationPosition != null && e.OperationPosition.AuthenticationType == 1 ? e.RFIDCode : string.Empty,
                    QRCode = e.OperationPosition != null && e.OperationPosition.AuthenticationType == 2 ? e.QRCode : string.Empty,
                    ParameterName = e.ParameterName,
                    KKSCode = e.KKSCode,
                    UnitMeasure = e.UnitMeasure,
                    Setting = e.Setting,
                    FromRange = e.FromRange,
                    ToRange = e.ToRange,
                    IsNumberType = e.IsNumberType,
                    QLKTCode = e.QLKTCode,
                    Position = e.Position,
                    Status = e.Status,
                    InsertedAt = e.InsertedAt,
                })
                .OrderBy(t => t.Position)
                .ThenBy(t => t.ID)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public ResponseBaseModel QuickSearchParameters(SearchParameterModel requestModel)
        {
            // Tất cả thông số theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => !x.IsDeleted);
            // Nhà máy
            entities = entities.Where(x => x.FactoryID == requestModel.FactoryID || requestModel.FactoryID == CommonConstant.ALL_KEY);
            // Tên thông số
            string strHighlight = string.Empty;
            if (!string.IsNullOrWhiteSpace(requestModel.Name))
            {
                strHighlight = "<span class=\"selected\">" + requestModel.Name + "</span>";
                entities = entities.Where(x => x.ParameterName.Contains(requestModel.Name) || x.KKSCode.Contains(requestModel.Name));
            }
            if (string.IsNullOrWhiteSpace(strHighlight)) strHighlight = string.Empty;

            var page = entities.Select(
                e => new ParameterModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                    ParameterName = strHighlight.Length > 0 ? e.ParameterName.Replace(requestModel.Name, strHighlight) : e.ParameterName,
                    KKSCode = strHighlight.Length > 0 ? e.KKSCode.Replace(requestModel.Name, strHighlight) : e.KKSCode,
                    UnitMeasure = e.UnitMeasure,
                    Position = e.Position,
                })
                .OrderBy(t => t.Position)
                .ThenBy(t => t.ID)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public ParametersChartModel GetParametersDrawChart(SearchParameterForChartModel requestModel)
        {
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("ParameterID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.ParameterID,
                },
                new SqlParameter("FromDate", System.Data.SqlDbType.DateTime)
                {
                    Value = requestModel.FromDate,
                },
                new SqlParameter("ToDate", System.Data.SqlDbType.DateTime)
                {
                    Value = requestModel.ToDate,
                },
            };

            var parametersChartModel = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ID == requestModel.ParameterID).Select(
                    e => new ParametersChartModel()
                    {
                        ID = e.ID,
                        OperationPosition_Factory_Name = e.OperationPosition.OperationPositionName + " - " + e.Factory.FactoryName,
                        ParameterName = e.ParameterName,
                        KKSCode = e.KKSCode,
                        UnitMeasure = e.UnitMeasure
                    }).FirstOrDefault();

            decimal minY = 0M;
            decimal maxY = 0M;

            if (requestModel.IsSearch)
            {
                var parametersChartDetailModel = _unitOfWork.DataContext.Database
                    .SqlQuery<ParametersChartDetailModel>("TSVH_GetValuesForChart @ParameterID, @FromDate, @ToDate", sqlParams).ToList();

                if (parametersChartModel != null && parametersChartDetailModel.Count > 0)
                {
                    parametersChartModel.ParametersChartDetails = parametersChartDetailModel;

                    //Lấy giá trị min của trục hoành
                    var firstItem = parametersChartDetailModel.FirstOrDefault();
                    if (firstItem != null)
                    {
                        parametersChartModel.MinX = firstItem.XValue;
                    }
                    //Lấy giá trị max của trục hoành
                    var lastItem = parametersChartDetailModel.LastOrDefault();
                    if (lastItem != null)
                    {
                        parametersChartModel.MaxX = lastItem.XValue;
                    }

                    parametersChartModel.MinDateX = parametersChartDetailModel.Select(x => x.CycleDate).Min();
                    parametersChartModel.MaxDateX = parametersChartDetailModel.Select(x => x.CycleDate).Max();

                    //Lấy giá trị min của trục tung
                    var tempMinY = parametersChartModel.ParametersChartDetails.Select(x => x.YValue).Min();
                    decimal.TryParse(tempMinY, out minY);
                    //Lấy giá trị max của trục hoành
                    var tempMaxY = parametersChartModel.ParametersChartDetails.Select(x => x.YValue).Max();
                    decimal.TryParse(tempMaxY, out maxY);

                    if (minY >= 0)
                        minY = 0M;
                    else
                        minY += -(minY * 5 / 100);
                    parametersChartModel.MinY = minY;
                    parametersChartModel.MaxY = maxY + (maxY * 5 / 100);

                    if (parametersChartDetailModel.Count > 15)
                    {
                        parametersChartModel.Xaxis = new List<string>();
                        //Lấy giá trị min của trục hoành
                        if (firstItem != null)
                        {
                            parametersChartModel.Xaxis.Add(firstItem.XValue);
                        }

                        var loopIndex = (double)parametersChartDetailModel.Count / 10;
                        for (int i = 0; i < 15; i++)
                        {
                            if (i != 0 && i != 14)
                            {
                                var index = (int)Math.Round(loopIndex * i);
                                if (index <= parametersChartDetailModel.Count - 1)
                                {
                                    var item = parametersChartDetailModel[index];
                                    if (!parametersChartModel.Xaxis.Contains(item.XValue))
                                    {
                                        parametersChartModel.Xaxis.Add(item.XValue);
                                    }
                                }
                            }
                        }

                        //Lấy giá trị max của trục hoành
                        if (lastItem != null && !parametersChartModel.Xaxis.Contains(lastItem.XValue))
                        {
                            parametersChartModel.Xaxis.Add(lastItem.XValue);
                        }
                    }
                    else
                    {
                        parametersChartModel.Xaxis = parametersChartDetailModel.Select(x => x.XValue).ToList();
                    }
                }
            }

            return parametersChartModel ?? new ParametersChartModel();
        }

        public ParameterModel GetParameterById(int id)
        {
            return _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new ParameterModel()
                {
                    ID = e.ID,
                    IsGroup = e.IsGroup,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    AuthenticationType = new ValueLabelModel { Value = e.OperationPosition.AuthenticationType, Label = e.OperationPosition.AuthenticationType == 1 ? "RFID" : (e.OperationPosition.AuthenticationType == 2 ? "QR Code" : "Không sử dụng") },
                    Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                    ParameterGroup = new ValueLabelModel { Value = e.ParameterGroup.ID, Label = e.ParameterGroup.ParameterGroupName },
                    RFIDCode = e.RFIDCode,
                    QRCode = e.QRCode,
                    ParameterName = e.ParameterName,
                    KKSCode = e.KKSCode,
                    UnitMeasure = e.UnitMeasure,
                    Setting = e.Setting,
                    FromRange = e.FromRange,
                    ToRange = e.ToRange,
                    IsNumberType = e.IsNumberType,
                    QLKTCode = e.QLKTCode,
                    Position = e.Position,
                    Status = e.Status,
                    AlertDetails = e.AlertDetails.Where(x => !x.IsDeleted).Select(t => new AlertDetailModel
                    {
                        ID = t.ID,
                        Alert = t.Alert,
                        Operator = t.Operator,
                        Value = t.Value,
                        Color = t.Color,
                    }).ToList()
                }).FirstOrDefault();
        }

        public int AddParameter(AddEditParameterModel requestModel)
        {
            var entity = ToEntity(requestModel);

            // Danh sách Cảnh báo
            entity.AlertDetails = requestModel.AlertDetails?
                 .Select(e => new AlertDetail()
                 {
                     Alert = !string.IsNullOrWhiteSpace(e.Alert) ? e.Alert.Trim() : null,
                     Operator = !string.IsNullOrWhiteSpace(e.Operator) ? e.Operator.Trim() : null,
                     Value = e.Value,
                     Color = !string.IsNullOrWhiteSpace(e.Color) ? e.Color.Trim() : null,
                     CreatedUserID = requestModel.CurrentUserID,
                     LastUpdatedUserID = requestModel.CurrentUserID,
                     SyncedDate = DateTime.Now,
                 }).ToList();

            entity.CreatedUserID = entity.LastUpdatedUserID = requestModel.CurrentUserID;

            _unitOfWork.ElogbookRepository<Parameter>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess) return entity.ID;
            return 0;
        }

        public bool UpdateParameter(AddEditParameterModel requestModel)
        {
            var currentEntity = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (currentEntity != null)
            {
                List<int> lstAlertDetailId = new List<int>();

                // cập nhật danh sách cảnh báo
                if (requestModel.AlertDetails != null && requestModel.AlertDetails.Count > 0)
                {
                    foreach (var item in requestModel.AlertDetails)
                    {
                        // cập nhật cảnh báo
                        if (item.ID > 0 && item.ID != null)
                        {
                            var alertDetailUpdate = _unitOfWork.ElogbookRepository<AlertDetail>().GetQuery(x => x.ID == item.ID && !x.IsDeleted).FirstOrDefault();
                            if (alertDetailUpdate != null)
                            {
                                alertDetailUpdate.Alert = !string.IsNullOrWhiteSpace(item.Alert) ? item.Alert.Trim() : null;
                                alertDetailUpdate.Operator = !string.IsNullOrWhiteSpace(item.Operator) ? item.Operator.Trim() : null;
                                alertDetailUpdate.Value = item.Value;
                                alertDetailUpdate.Color = !string.IsNullOrWhiteSpace(item.Color) ? item.Color.Trim() : null;
                                alertDetailUpdate.UpdatedAt = DateTime.Now;
                                alertDetailUpdate.LastUpdatedUserID = requestModel.CurrentUserID;
                                alertDetailUpdate.SyncedDate = DateTime.Now;

                                _unitOfWork.ElogbookRepository<AlertDetail>().Update(alertDetailUpdate);

                                lstAlertDetailId.Add((int)item.ID);
                            }
                        }
                        // thêm mới cảnh báo
                        else
                        {
                            var alertDetailNew = new AlertDetail()
                            {
                                Alert = !string.IsNullOrWhiteSpace(item.Alert) ? item.Alert.Trim() : null,
                                ParameterID = requestModel.ID,
                                Operator = !string.IsNullOrWhiteSpace(item.Operator) ? item.Operator.Trim() : null,
                                Value = item.Value,
                                Color = !string.IsNullOrWhiteSpace(item.Color) ? item.Color.Trim() : null,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                SyncedDate = DateTime.Now,
                            };
                            _unitOfWork.ElogbookRepository<AlertDetail>().Add(alertDetailNew);
                        }
                    }
                }

                // xóa danh sách cảnh báo
                var deletes = _unitOfWork.ElogbookRepository<AlertDetail>().GetQuery().Where(x => x.ParameterID == requestModel.ID
                                                                                                    && !x.IsDeleted
                                                                                                    && !lstAlertDetailId.Contains(x.ID));

                foreach (var item in deletes)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<AlertDetail>().Delete(deletes, requestModel.CurrentUserID);

                // cập nhập thông số
                var updateEntity = ToEntity(requestModel);
                updateEntity.ExcelAIColIndex = currentEntity.ExcelAIColIndex;
                updateEntity.ID = requestModel.ID;
                updateEntity.UpdatedAt = DateTime.Now;
                updateEntity.LastUpdatedUserID = updateEntity.CreatedUserID = requestModel.CurrentUserID;
                updateEntity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<Parameter>().Update(updateEntity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteParameter(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                // xóa danh sách cảnh báo
                var deletes = _unitOfWork.ElogbookRepository<AlertDetail>().GetQuery().Where(x => x.ParameterID == id
                                                                                                    && !x.IsDeleted);
                foreach (var item in deletes)
                {
                    item.SyncedDate = DateTime.Now;
                }
                _unitOfWork.ElogbookRepository<AlertDetail>().Delete(deletes, userID);

                // xóa thông số
                entity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<Parameter>().Delete(entity, userID);

                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckDuplicateData(AddEditParameterModel requestModel, bool isAddMode)
        {
            var factoryID = requestModel.FactoryID.GetValueOrDefault(0);
            var operationPositionID = requestModel.OperationPositionID.GetValueOrDefault(0);
            var cycleID = requestModel.CycleID.GetValueOrDefault(0);

            // Trường hợp thêm mới thì check Tên Thông số ko được trùng và Nhà máy có tồn tại
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.OperationPositionID == operationPositionID
                                                                                    && x.CycleID == cycleID
                                                                                    && x.ParameterName.Equals(requestModel.ParameterName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì check Tên Thông số, Nhà máy và những Tổ máy khác Tổ máy đang cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.FactoryID == factoryID
                                                                                    && x.OperationPositionID == operationPositionID
                                                                                    && x.CycleID == cycleID
                                                                                    && x.ID != requestModel.ID
                                                                                    && x.ParameterName.Equals(requestModel.ParameterName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public bool CheckInUseParameter(int id)
        {
            return _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.ParameterID == id && !x.IsDeleted).Any();
        }

        public List<CommonDataComboboxList> GetOperationPositions(int factoryID)
        {
            return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                                                    && x.FactoryID == factoryID)
                            .Select(
                                e => new CommonDataComboboxList()
                                {
                                    Value = e.ID,
                                    Label = e.OperationPositionName,
                                    AuthenticationType = new ValueLabelModel { Value = e.AuthenticationType, Label = e.AuthenticationType == 1 ? "RFID" : (e.AuthenticationType == 2 ? "QR Code" : "Không sử dụng") },
                                }).OrderBy(t => t.Value).ToList();
        }

        private Parameter ToEntity(AddEditParameterModel requestModel)
        {
            var entity = new Parameter
            {
                FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : null,
                OperationPositionID = requestModel.OperationPositionID > 0 ? requestModel.OperationPositionID : null,
                RFIDCode = !string.IsNullOrWhiteSpace(requestModel.RFIDCode) ? requestModel.RFIDCode.Trim() : requestModel.RFIDCode,
                QRCode = !string.IsNullOrWhiteSpace(requestModel.QRCode) ? requestModel.QRCode.Trim() : requestModel.QRCode,
                IsGroup = requestModel.IsGroup,
                ParameterName = !string.IsNullOrWhiteSpace(requestModel.ParameterName) ? requestModel.ParameterName.Trim() : requestModel.ParameterName,
                KKSCode = !string.IsNullOrWhiteSpace(requestModel.KKSCode) ? requestModel.KKSCode.Trim() : requestModel.KKSCode,
                UnitMeasure = !string.IsNullOrWhiteSpace(requestModel.UnitMeasure) ? requestModel.UnitMeasure.Trim() : requestModel.UnitMeasure,
                Setting = !string.IsNullOrWhiteSpace(requestModel.Setting) ? requestModel.Setting.Trim() : requestModel.Setting,
                IsNumberType = requestModel.IsNumberType,
                QLKTCode = !string.IsNullOrWhiteSpace(requestModel.QLKTCode) ? requestModel.QLKTCode.Trim() : requestModel.QLKTCode,
                Position = requestModel.Position,
                Status = requestModel.Status <= 0 ? (int)AccessStatus.InActive : requestModel.Status,
            };

            if (requestModel.IsNumberType)
            {
                entity.FromRange = requestModel.FromRange;
                entity.ToRange = requestModel.ToRange;
            }
            else
            {
                entity.FromRange = entity.ToRange = null;
            }

            // Thông số thuộc nhóm thông số
            if (requestModel.IsGroup)
            {
                entity.CycleID = null;
                entity.ParameterGroupID = requestModel.ParameterGroupID;
                if (requestModel.ParameterGroupID > 0)
                {
                    var parameterGroup = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.ID == requestModel.ParameterGroupID).FirstOrDefault();
                    if (parameterGroup != null && parameterGroup.ID > 0)
                    {
                        entity.ParameterGroupID = parameterGroup.ID;
                        entity.CycleID = parameterGroup.CycleID;
                    }
                }
            }
            // Thông số lẻ
            else
            {
                entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : null;
                entity.OperationPositionID = requestModel.OperationPositionID > 0 ? requestModel.OperationPositionID : null;
                entity.CycleID = requestModel.CycleID > 0 ? requestModel.CycleID : null;
                entity.ParameterGroupID = null;
            }

            entity.SyncedDate = DateTime.Now;

            return entity;
        }
    }
}
