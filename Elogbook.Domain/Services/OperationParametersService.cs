using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Helpers;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseCommon;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Elogbook.Domain.Services
{
    public class OperationParametersService : IOperationParametersService
    {
        public class Result
        {
            public string Message { get; set; }
        }

        private int stt = 1;
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fBold = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont f_bold_italic = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbi.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        public OperationParametersService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Danh sách Chu kỳ của Nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetCyclesManagements(int factoryID, int operationPositionID)
        {
            var lstCycles = new List<CyclesModel>();
            var cycles = _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => x.CycleFactories.Any(y => y.FactoryID == factoryID)
                                                                                          && x.Active == (int)AccessStatus.Active
                                                                                          && !x.IsDeleted).ToList();

            foreach (var cycle in cycles)
            {
                // Tổng số thông số của mỗi chu kỳ
                int totalParams = GetCountParamaterByCycleID(cycle.ID, operationPositionID, factoryID);
                if (totalParams > 0)
                {
                    CyclesModel cyclesResponse = new CyclesModel()
                    {
                        ID = cycle.ID,
                        CycleName = cycle.CycleName,
                        OpenInputBeforeStartTime = cycle.OpenInputBefore,
                        LockInputBeforeEndTime = cycle.BlockInputAfter,
                        TotalParameters = totalParams,
                        IsAdd = CheckExistOperationParameterByCurrentDate(factoryID, operationPositionID, cycle.ID),
                    };
                    lstCycles.Add(cyclesResponse);
                }
            }

            return HttpResponse.OkList(lstCycles);
        }

        /// <summary>
        /// Cập nhật thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool UpdateOperationParameters(EditOperationParameterModel requestModel)
        {
            // Cập nhật thông số vận hành
            var operationParameter = _unitOfWork.ElogbookRepository<OperationParameters>().GetById(requestModel.ID);
            if (operationParameter != null)
            {
                // Cập nhật dữ liệu được ghi cho khung giờ
                var operationTimeLine = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetById(requestModel.OperationTimeLineID);
                if (operationTimeLine != null)
                {
                    if (!operationParameter.IsChecked)
                    {
                        operationTimeLine.TotalParametersChecked += 1;
                        operationTimeLine.TotalParametersSynchronized += 1;
                    }
                    operationTimeLine.LastUpdatedUserID = requestModel.CurrentUserID;
                    operationTimeLine.SyncedDate = DateTime.Now;
                    _unitOfWork.ElogbookRepository<OperationTimeLine>().Update(operationTimeLine);
                }
                operationParameter.CurrentValue = !string.IsNullOrWhiteSpace(requestModel.CurrentValue) ? requestModel.CurrentValue : null;
                operationParameter.ErrorCode = (requestModel.ErrorCode < 1 && requestModel.ErrorCode > 5) ? 0 : requestModel.ErrorCode;
                operationParameter.IsChecked = true;
                operationParameter.LastUpdatedUserID = requestModel.CurrentUserID;
                operationParameter.CheckedDate = DateTime.Now;
                operationParameter.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<OperationParameters>().Update(operationParameter);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        /// <summary>
        /// Cập nhật khung giờ khi báo bận hoặc báo dừng máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool UpdateOperationTimeLine(EditOperationTimeLineModel requestModel)
        {
            if (requestModel.ListID == null)
            {
                requestModel.ListID = new List<int>();
            }

            if (requestModel.ListID != null && requestModel.ListID.Count > 0)
            {
                //Danh sách ID của những khung giờ Báo bận công tác hoặc Báo dừng máy
                var operationTimeLineIDs = string.Join(",", requestModel.ListID);
                if (!string.IsNullOrWhiteSpace(operationTimeLineIDs))
                {
                    var sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("FactoryID", SqlDbType.Int)
                        {
                            Value = requestModel.FactoryID,
                        },
                        new SqlParameter("OperationPositionID", SqlDbType.Int)
                        {
                            Value = requestModel.OperationPositionID,
                        },
                        new SqlParameter("CycleID", SqlDbType.Int)
                        {
                            Value = requestModel.CycleID,
                        },
                        new SqlParameter("OperationTimeLineIDs", SqlDbType.NVarChar)
                        {
                            Value = operationTimeLineIDs,
                        },
                        new SqlParameter("ErrorCode", SqlDbType.Int)
                        {
                            Value = requestModel.ErrorCode,
                        },
                        new SqlParameter("CurrentUserID", SqlDbType.Int)
                        {
                            Value = requestModel.CurrentUserID,
                        },
                    };

                    var result = _unitOfWork.DataContext.Database
                        .SqlQuery<Result>("TSVH_UpdateOperationTimeLine @FactoryID, @OperationPositionID, @CycleID, @OperationTimeLineIDs, @ErrorCode, @CurrentUserID", sqlParams).ToList();

                    if (result.Count > 0 && result[0].Message == "SUCCESS")
                    {
                        //ignore error when create success
                        return true;
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "UpdateOperationTimeLine", "Báo bận công tác hoặc Báo dừng máy không thành công!");
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Cập nhật khung giờ khi báo bận hoặc báo dừng máy từng phần
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool UpdateOperationTimeLine(StopOperationTimeLineModel requestModel)
        {
            if (requestModel.OperationTimeLineIds != null && requestModel.OperationTimeLineIds.Count > 0)
            {
                var errorCode = requestModel.ErrorCode < 1 && requestModel.ErrorCode > 5 ? 0 : requestModel.ErrorCode;

                //Danh sách ID của những khung giờ Báo bận công tác hoặc Báo dừng máy
                var operationTimeLineIDs = string.Join(",", requestModel.OperationTimeLineIds);

                if (!string.IsNullOrWhiteSpace(operationTimeLineIDs))
                {
                    var sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("FactoryID", SqlDbType.Int)
                        {
                            Value = requestModel.FactoryID,
                        },
                        new SqlParameter("OperationPositionID", SqlDbType.Int)
                        {
                            Value = requestModel.OperationPositionID,
                        },
                        new SqlParameter("CycleID", SqlDbType.Int)
                        {
                            Value = requestModel.CycleID,
                        },
                        new SqlParameter("OperationTimeLineIDs", SqlDbType.NVarChar)
                        {
                            Value = operationTimeLineIDs,
                        },
                        new SqlParameter("ErrorCode", SqlDbType.Int)
                        {
                            Value = errorCode,
                        },
                        new SqlParameter("CurrentUserID", SqlDbType.Int)
                        {
                            Value = requestModel.CurrentUserID,
                        },
                        // Danh sách group dừng tất cả thông số

                        new SqlParameter("StopParameterIDs", SqlDbType.NVarChar)
                        {
                            Value = string.Join(",", requestModel.StopParameterIds)
                        }
                    };

                    var result = _unitOfWork.DataContext.Database
                        .SqlQuery<Result>("TSVH_UpdateOperationTimeLinePartial @FactoryID, @OperationPositionID, @CycleID, @OperationTimeLineIDs, @StopParameterIDs, @ErrorCode, @CurrentUserID", sqlParams.ToArray()).ToList();

                    if (result.Count > 0 && result[0].Message == "SUCCESS")
                    {
                        //ignore error when create success
                        return true;
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "UpdateOperationTimeLine", "Báo bận công tác hoặc Báo dừng máy không thành công!");
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Lấy danh sách khung giờ cho chu kỳ theo từ ngày đến ngày
        /// Đã thay thế bằng API GetTimeLineLabels, GetTimeLineDetail để tối ưu performance
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationTimeLines(SearchOperationTimeLineModel requestModel)
        {
            // Filter data by condition
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            var entities = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && x.FactoryID == requestModel.FactoryID
                                                                                        && !x.IsDeleted);
            var masterData = entities;
            IPagedList<OperationTimeLineModel> dataGroup = entities.Select(
               e => new OperationTimeLineModel()
               {
                   GroupDate = DbFunctions.TruncateTime(e.InsertedAt),
               }).Distinct().OrderByDescending(x => x.GroupDate)
               .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            List<BaseGroupDataModel<OperationTimeLineModel>> results = (from item in dataGroup
                                                                        let groupDate = item.GroupDate.GetValueOrDefault(DateTime.MinValue).ToString(CommonConstant.FORMAT_DATE_DDMMYYYY)
                                                                        select new BaseGroupDataModel<OperationTimeLineModel>()
                                                                        {
                                                                            GroupDate = groupDate,
                                                                            MasterDetail = masterData.Where(x => DbFunctions.TruncateTime(x.InsertedAt) == item.GroupDate.Value).Select(
                                                                                                        e => new OperationTimeLineModel()
                                                                                                        {
                                                                                                            ID = e.ID,
                                                                                                            Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                                                                                                            OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                                                                                                            Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                                                                                                            TimeLineName = e.TimeLineName,
                                                                                                            TotalParametersDefault = e.TotalParametersDefault,
                                                                                                            TotalParametersChecked = e.TotalParametersChecked,
                                                                                                            TotalParametersSynchronized = e.TotalParametersSynchronized,
                                                                                                            UpdatedAt = e.UpdatedAt,
                                                                                                            InsertedAt = e.InsertedAt,
                                                                                                        }).OrderBy(x => x.TimeLineName).ToList()
                                                                        }).ToList();
            return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, dataGroup.TotalRecords, dataGroup.TotalPages);
        }

        /// <summary>
        /// Lấy danh sách khung giờ cho chu kỳ theo từ ngày đến ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationTimeLinesMobile(SearchOperationTimeLineModel requestModel)
        {
            // Filter data by condition
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            var entities = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && x.FactoryID == requestModel.FactoryID
                                                                                        && !x.IsDeleted);

            // Return and paging data
            var page = entities.Select(
                e => new OperationTimeLineModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                    TimeLineName = e.TimeLineName,
                    TotalParametersDefault = e.TotalParametersDefault,
                    TotalParametersChecked = e.TotalParametersChecked,
                    TotalParametersSynchronized = e.TotalParametersSynchronized,
                    UpdatedAt = e.UpdatedAt,
                    InsertedAt = e.InsertedAt,
                    AppID = e.AppID,
                    SyncDataKey = e.TimeLineSyncDataKey,
                })
                .OrderByDescending(t => t.InsertedAt)
                .ThenByDescending(t => t.TimeLineName)
                .ThenByDescending(t => t.ID)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Danh sách nhóm thông số và số lượng thông số
        /// </summary>
        /// <param name="cycleID"></param>
        /// <returns></returns>
        public ResponseBaseModel GetParameterGroups(int factoryID, int operationPositionID, int cycleID, int operationTimeLineID)
        {
            var lstParameterGroup = new List<OperationParameterGroupModel>();

            // Danh sách nhóm thông số theo chu kỳ
            var parameterGroups = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.CycleID == cycleID
                                                                                            && x.FactoryID == factoryID
                                                                                            && x.OperationPositionID == operationPositionID
                                                                                            && x.Status == (int)AccessStatus.Active
                                                                                            && !x.IsDeleted).Select(e => new { e.ID, e.ParameterGroupName }).ToList();
            foreach (var item in parameterGroups)
            {
                // Thông số của 1 nhóm thông số
                var operationParameters = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.OperationTimeLineID == operationTimeLineID
                                                                                                    && x.CycleID == cycleID
                                                                                                    && x.FactoryID == factoryID
                                                                                                    && x.OperationPositionID == operationPositionID
                                                                                                    && x.ParameterGroupID == item.ID
                                                                                                    && !x.IsDeleted).Select(e => new { e.ID, e.IsChecked }).ToList();
                // Tổng thông số
                int totalParameters = operationParameters.Count;
                // Thông số đã ghi dữ liệu
                int totalParametersChecked = operationParameters.Count(x => x.IsChecked);

                OperationParameterGroupModel opg = new OperationParameterGroupModel()
                {
                    ParameterGroupID = item.ID,
                    ParameterGroupName = item.ParameterGroupName,
                    TotalParameters = totalParameters,
                    TotalParametersChecked = totalParametersChecked,
                };
                lstParameterGroup.Add(opg);
            }

            // Thông số lẻ
            var otherOperationParameters = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.OperationTimeLineID == operationTimeLineID
                                                                                                        && x.CycleID == cycleID
                                                                                                        && x.FactoryID == factoryID
                                                                                                        && x.OperationPositionID == operationPositionID
                                                                                                        && (x.ParameterGroupID == null || x.ParameterGroupID == 0)
                                                                                                        && !x.IsDeleted).Select(e => new { e.ID, e.IsChecked }).ToList();
            // Tổng số thông số lẻ
            int totalOtherParameters = otherOperationParameters.Count;
            // Số thông số lẻ đã ghi dữ liệu
            int totalOtherParametersChecked = otherOperationParameters.Count(x => x.IsChecked);

            if (totalOtherParameters > 0)
            {
                OperationParameterGroupModel other = new OperationParameterGroupModel()
                {
                    ParameterGroupName = "Khác",
                    TotalParameters = totalOtherParameters,
                    TotalParametersChecked = totalOtherParametersChecked,
                };
                lstParameterGroup.Add(other);
            }

            return HttpResponse.OkList(lstParameterGroup);
        }

        /// <summary>
        /// Danh sách thông số vận hành cần ghi dữ liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetParameters(SearchParametersModel requestModel)
        {
            // Phương thức xác thực của Cương vị
            var operationPosition = _unitOfWork.ElogbookRepository<OperationPosition>().GetById(requestModel.OperationPositionID);
            var authenticationType = operationPosition?.AuthenticationType;

            // Mobile App không cần phân trang dữ liệu
            if (requestModel.IsMobile)
            {
                requestModel.PageNumber = 1;
                requestModel.PageSize = int.MaxValue;
            }

            // Nếu từ Mobile app và phương thức xác thực RFID/QRCode thì cần quét trước khi trả về dữ liệu
            // Danh sách toàn bộ thông số của khung giờ cần ghi dữ liệu
            var data = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                    && x.OperationPositionID == requestModel.OperationPositionID
                                                                                    && x.OperationTimeLineID == requestModel.OperationTimeLineID
                                                                                    && x.CycleID == requestModel.CycleID
                                                                                    && !x.IsDeleted);

            if (requestModel.IsMobile && authenticationType.GetValueOrDefault(0) != 0)
            {
                // Danh sách thông số sau khi quét mã
                List<int> listParametersID = ScanPrameters(requestModel, authenticationType.GetValueOrDefault(0));
                data = data.Where(x => listParametersID.Contains(x.ParameterID));

                var page = data.Select(item => new OperationParametersModel()
                {
                    ID = item.ID,
                    IsGroup = requestModel.ParameterGroupID != null && requestModel.ParameterGroupID > 0,
                    ParameterID = item.ParameterID,
                    ParameterName = item.ParameterName,
                    KKSCode = item.KKSCode,
                    UnitMeasure = item.UnitMeasure,
                    PreviousValue = item.PreviousValue,
                    CurrentValue = item.CurrentValue,
                    ErrorCode = (requestModel.IsMobile || item.ErrorCode != 5) ? item.ErrorCode : null,
                    FromRange = item.FromRange,
                    ToRange = item.ToRange,
                    IsNumberType = item.IsNumberType,
                    ParameterGroupID = item.ParameterGroupID,
                    OprerationTimeLineID = item.OperationTimeLineID,
                    IsChecked = item.IsChecked,
                    AlertDetails = item.AlertDetails,
                    CheckedDate = item.CheckedDate,
                }).OrderBy(x => x.ID);

                IPagedList<OperationParametersModel> pagelist = page.ToPagedList(requestModel.PageNumber, requestModel.PageSize);
                // Lấy giá trị thông số của 4 khung giờ trước
                if (pagelist != null && pagelist.TotalRecords > 0)
                {
                    foreach (var entity in pagelist)
                    {
                        entity.PreviousValue = GetPreviousValue(requestModel.FactoryID, requestModel.OperationPositionID, requestModel.CycleID, requestModel.OperationTimeLineID, entity.ParameterID);
                        var parameter = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ID == entity.ParameterID).FirstOrDefault();

                        entity.Setting = GetAlarmSetting(parameter);
                        entity.FromRange = parameter?.FromRange;
                        entity.ToRange = parameter?.ToRange;
                    }
                    pagelist.GetEnumerator().Reset();
                }

                return HttpResponse.OkList(page);
            }
            else
            {
                var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("FactoryID", SqlDbType.Int)
                    {
                        Value = requestModel.FactoryID,
                    },
                    new SqlParameter("CycleID", SqlDbType.Int)
                    {
                        Value = requestModel.CycleID,
                    },
                    new SqlParameter("ParameterGroupID", SqlDbType.Int)
                    {
                        Value = requestModel.ParameterGroupID.GetValueOrDefault(0),
                    },
                    new SqlParameter("OperationPositionID", SqlDbType.Int)
                    {
                        Value = requestModel.OperationPositionID,
                    },
                    new SqlParameter("OperationTimeLineID", SqlDbType.Int)
                    {
                        Value = requestModel.OperationTimeLineID,
                    },
                    new SqlParameter("PageNumber", SqlDbType.Int)
                    {
                        Value = requestModel.PageNumber,
                    },
                    new SqlParameter("PageSize", SqlDbType.Int)
                    {
                        Value = requestModel.PageSize,
                    }
                };

                var results = _unitOfWork.DataContext.Database
                    .SqlQuery<OperationParametersModel>("TSVH_GetParameters @FactoryID, @CycleID, @ParameterGroupID, @OperationPositionID, @OperationTimeLineID, @PageNumber, @PageSize", sqlParams).ToList();

                var totalRecords = 0;
                var totalPages = 0;
                if (results.Count > 0)
                {
                    totalRecords = results[0].TotalRecords;
                    totalPages = totalRecords / requestModel.PageSize + (totalRecords % requestModel.PageSize > 0 ? 1 : 0);
                }

                return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
            }
        }

        private string GetAlarmSetting(Parameter parameter)
        {
            StringBuilder alarmSetting = new StringBuilder();
            if (parameter.AlertDetails != null && parameter.AlertDetails.Count > 0)
            {
                foreach (var al in parameter.AlertDetails.OrderBy(x => x.ID))
                {
                    if (!al.IsDeleted)
                    {
                        alarmSetting.Append(al.Alert.Trim() + ": " + al.Operator.Trim() + " " + al.Value + "; ");
                    }
                }
            }

            string alarmSettingStr = alarmSetting.ToString();
            if (!string.IsNullOrWhiteSpace(alarmSettingStr) && alarmSettingStr.Length > 2)
            {
                alarmSettingStr = alarmSettingStr.Substring(0, alarmSettingStr.Length - 2);
            }

            return alarmSettingStr;
        }

        /// <summary>
        /// Lấy danh sách thông số vận hành theo khung giờ trên Mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationParameters(SearchOperationParametersModel requestModel)
        {
            var operationParameters = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.OperationTimeLineID == requestModel.OperationTimeLineID
                                                                                                        && x.CycleID == requestModel.CycleID
                                                                                                        && x.FactoryID == requestModel.FactoryID
                                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                                        && !x.IsDeleted);
            if (requestModel.TypeGet == 0)
            {
                operationParameters = operationParameters.Where(x => !x.IsChecked);
            }
            else if (requestModel.TypeGet == 1)
            {
                operationParameters = operationParameters.Where(x => x.IsChecked);
            }

            var results = new List<OperationParametersMobile>();
            var lstOperationParameters = operationParameters.ToList();
            if (lstOperationParameters.Count > 0)
            {
                foreach (var item in lstOperationParameters)
                {
                    OperationParametersMobile operationParametersMobile = new OperationParametersMobile()
                    {
                        ID = item.ID,
                        ParameterName = item.ParameterName + "\n" + item.KKSCode,
                        CurrentValue = item.CurrentValue,
                        UnitMeasure = item.UnitMeasure,
                        ErrorCode = item.ErrorCode,
                        Status = !item.IsChecked ? 0 : 1,
                    };

                    if (item.IsNumberType)
                    {
                        var prEntity = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.ID == item.ParameterID && !x.IsDeleted)
                            .Select(e => new ParameterModel()
                            {
                                ID = e.ID,
                                AlertDetails = e.AlertDetails.Where(x => !x.IsDeleted).Select(t => new AlertDetailModel
                                {
                                    ID = t.ID,
                                    Alert = t.Alert,
                                    Operator = t.Operator,
                                    Value = t.Value,
                                    Color = t.Color,
                                }).ToList()
                            }).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(item.CurrentValue))
                        {
                            operationParametersMobile.Color = GetColorAlarm(prEntity, item.CurrentValue);
                        }
                        operationParametersMobile.ColorCode = GetColorCode(operationParametersMobile.Color);
                    }
                    results.Add(operationParametersMobile);
                }
            }

            return HttpResponse.OkList(results);
        }

        /// <summary>
        /// Xem thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel ViewOperationParameters(ViewOperationParametersModel requestModel)
        {
            SearchOperationParametersModel request = new SearchOperationParametersModel
            {
                SearchDate = requestModel.SearchDate,
                FactoryID = requestModel.FactoryID,
                OperationPositionID = requestModel.OperationPositionID,
                CycleID = requestModel.CycleID,
                ParameterGroupID = -1, //Lấy tất cả thông số
                PageNumber = requestModel.PageNumber,
                PageSize = requestModel.PageSize,
            };

            return GetOperationParametersPortal(request, true);
        }

        /// <summary>
        /// Lấy danh sách thông số vận hành theo web
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isView"></param>
        /// <returns></returns>
        public ResponseBaseModel GetOperationParametersPortal(SearchOperationParametersModel requestModel, bool isView = false)
        {
            var lstOperationParameters = new List<OperationParametersWebModel>();
            DateTime searchDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.SearchDate, true);
            var listOperationTimeLineID = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) == searchDate.Date
                                                                                        && x.FactoryID == requestModel.FactoryID
                                                                                        && !x.IsDeleted).Select(x => x.ID).ToList();

            if (listOperationTimeLineID == null || (listOperationTimeLineID != null && listOperationTimeLineID.Count == 0))
            {
                return HttpResponse.OkList(lstOperationParameters, requestModel.PageNumber, requestModel.PageSize, 0, 0);
            }

            // Trường hợp view dữ liệu thì mặc định lấy chu kỳ đầu tiên
            List<ValueLabelModel> cycles = null;
            if (isView && requestModel.CycleID <= 0)
            {
                // Danh sách Chu kỳ của Nhà máy
                List<int> cycleIDs = _unitOfWork.ElogbookRepository<CycleFactory>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                              && !x.IsDeleted).Select(e => e.CycleID).ToList() ?? new List<int>();
                cycles = _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => cycleIDs.Contains(x.ID)
                                                                                              && x.Active == (int)AccessStatus.Active
                                                                                              && !x.IsDeleted).Select(
                            e => new ValueLabelModel()
                            {
                                Value = e.ID,
                                Label = e.CycleName
                            }).ToList();

                if (cycles != null && cycles.Count > 0 && requestModel.CycleID <= 0)
                {
                    requestModel.CycleID = cycles[0].Value.Value;
                }
            }

            // Danh sách khung giờ theo chu kỳ
            var timeLineCycles = _unitOfWork.ElogbookRepository<TimeLineCycle>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                && x.Active == (int)AccessStatus.Active
                                                                                && !x.IsDeleted).ToList();

            var lstParameterID = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                            && x.OperationPositionID == requestModel.OperationPositionID
                                                                            && x.FactoryID == requestModel.FactoryID
                                                                            && x.Status == (int)AccessStatus.Active
                                                                            && !x.IsDeleted).Select(x => x.ID).ToList() ?? new List<int>();
            // Danh sách tất cả thông số theo chu kỳ
            var parametersEntities = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                && x.OperationPositionID == requestModel.OperationPositionID
                                                                                && x.CycleID == requestModel.CycleID
                                                                                && (lstParameterID.Contains((int)x.ParameterGroupID) || x.ParameterGroupID == null)
                                                                                && x.Status == (int)AccessStatus.Active
                                                                                && !x.IsDeleted);

            // Nhóm thông số
            if (requestModel.ParameterGroupID > 0)
            {
                parametersEntities = parametersEntities.Where(x => x.IsGroup && x.ParameterGroupID == requestModel.ParameterGroupID);
            }

            // Thông số lẻ
            if (requestModel.ParameterGroupID == 0)
            {
                parametersEntities = parametersEntities.Where(x => !x.IsGroup);
            }

            IPagedList<ParameterModel> page = parametersEntities.Select(
                e => new ParameterModel()
                {
                    ID = e.ID,
                    ParameterName = e.ParameterName,
                    IsGroup = e.IsGroup,
                    KKSCode = e.KKSCode,
                    UnitMeasure = e.UnitMeasure,
                    Setting = e.Setting,
                    FromRange = e.FromRange,
                    ToRange = e.ToRange,
                    IsNumberType = e.IsNumberType,
                    AlertDetails = e.AlertDetails.Where(x => !x.IsDeleted).Select(t => new AlertDetailModel
                    {
                        ID = t.ID,
                        Alert = t.Alert,
                        Operator = t.Operator,
                        Value = t.Value,
                        Color = t.Color,
                    }).ToList()
                })
                .OrderBy(t => t.ID)
                .ThenBy(t => t.ParameterName)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            var checkedEntities = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                                    && x.CycleID == requestModel.CycleID
                                                                                                    && x.OperationPositionID == requestModel.OperationPositionID
                                                                                                    && listOperationTimeLineID.Contains(x.OperationTimeLineID != null ? (int)x.OperationTimeLineID : 0)
                                                                                                    && !x.IsDeleted);
            // trường hợp thuộc nhóm thông số
            if (requestModel.ParameterGroupID > 0)
            {
                checkedEntities = checkedEntities.Where(x => x.ParameterGroupID == requestModel.ParameterGroupID);
            }

            // trường hợp thông số lẻ
            if (requestModel.ParameterGroupID == 0)
            {
                checkedEntities = checkedEntities.Where(x => x.ParameterGroupID == null);
            }

            if (page != null)
            {
                foreach (var pr in page)
                {
                    // tạo Dictionary khung giở và giá trị
                    Dictionary<string, ParameterValueModel> timeLineCycleDict = new Dictionary<string, ParameterValueModel>();
                    foreach (var tl in timeLineCycles)
                    {
                        timeLineCycleDict.Add(tl.Hour.ToString(), null);
                    }

                    List<ParameterValueModel> parameterValueModels = new List<ParameterValueModel>();

                    var masterData = checkedEntities.Where(x => x.ParameterID == pr.ID).ToList();

                    // So sánh giá trị MIN, MAX
                    List<double> lstValue = new List<double>();
                    foreach (var op in masterData)
                    {
                        string prValue = string.Empty;
                        if (op.CurrentValue == null)
                        {
                            if (op.ErrorCode != null)
                            {
                                switch (op.ErrorCode)
                                {
                                    // Thiết bị bị hỏng (F)
                                    case (int)DeviceErrorCode.Error:
                                        prValue = "F";
                                        break;
                                    // Dừng thiết bị (-)
                                    case (int)DeviceErrorCode.Stop:
                                        prValue = "-";
                                        break;
                                    // Sim thiết bị (S)
                                    case (int)DeviceErrorCode.Sim:
                                        prValue = "S";
                                        break;
                                    // Jump thiết bị (J)
                                    case (int)DeviceErrorCode.Jump:
                                        prValue = "J";
                                        break;
                                    // Bận công tác
                                    case (int)DeviceErrorCode.Busy:
                                        prValue = "X";
                                        break;
                                }
                            }
                            else
                            {
                                prValue = null;
                            }
                        }
                        else
                        {
                            prValue = op.CurrentValue;
                        }
                        var prValueColor = new ParameterValueModel()
                        {
                            ParameterValue = prValue
                        };

                        if (pr.IsNumberType && op.CurrentValue != null)
                        {
                            if (!string.IsNullOrWhiteSpace(op.CurrentValue))
                            {
                                prValueColor.Color = !string.IsNullOrWhiteSpace(GetColorAlarm(pr, op.CurrentValue)) ? GetColorAlarm(pr, op.CurrentValue) : null;
                            }

                            double.TryParse(op.CurrentValue, out double value);
                            lstValue.Add(value);
                        }
                        timeLineCycleDict[op.OperationTimeLine.TimeLineName.ToString()] = prValueColor;
                    }

                    // thêm giá trị được ghi và khung giờ
                    foreach (KeyValuePair<string, ParameterValueModel> item in timeLineCycleDict)
                    {
                        var parameterValueModel = new ParameterValueModel()
                        {
                            OpeartionTimeLineName = item.Key,
                            ParameterValue = item.Value?.ParameterValue,
                            Color = item.Value?.Color
                        };

                        parameterValueModel.ColorCode = GetColorCode(parameterValueModel.Color);
                        parameterValueModels.Add(parameterValueModel);
                    }

                    // add 1 dữ liệu vào list output
                    OperationParametersWebModel operationParametersWebModel = new OperationParametersWebModel()
                    {
                        ParamaterID = pr.ID,
                        ParameterName = pr.ParameterName,
                        UnitMeasure = pr.UnitMeasure,
                        KKSCode = pr.KKSCode,
                        MinValue = lstValue.Count > 0 ? lstValue.Min().ToString() : null,
                        MaxValue = lstValue.Count > 0 ? lstValue.Max().ToString() : null,
                        ParameterValues = parameterValueModels,
                        IsNumberType = pr.IsNumberType,
                    };

                    if (pr.FromRange != null)
                    {
                        operationParametersWebModel.FromRange = (float)pr.FromRange;
                    }
                    else
                    {
                        operationParametersWebModel.FromRange = null;
                    }

                    if (pr.ToRange != null)
                    {
                        operationParametersWebModel.ToRange = (float)pr.ToRange;
                    }
                    else
                    {
                        operationParametersWebModel.ToRange = null;
                    }

                    lstOperationParameters.Add(operationParametersWebModel);
                }
            }

            return HttpResponse.OkList(lstOperationParameters, requestModel.PageNumber, requestModel.PageSize, page == null ? 0 : page.TotalRecords, page == null ? 0 : page.TotalPages);
        }

        /// <summary>
        /// Kiểm tra tồn tại khung giờ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckExistOperationTimeline(int id)
        {
            return _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.ID == id && !x.IsDeleted).Any();
        }

        public ResponseBaseModel GetDataAfterSyncing(List<int> lstOperationTimeLineID)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            OperationParameterSyncDataResultModel masterDatas = new OperationParameterSyncDataResultModel();
            if (lstOperationTimeLineID != null && lstOperationTimeLineID.Count > 0)
            {
                masterDatas.OperationTimeLines = _unitOfWork.ElogbookRepository<OperationTimeLine>()
                                    .GetQuery(x => lstOperationTimeLineID.Contains(x.ID))
                                    .Select(
                                        e => new MobileOperationTimeLineInfo()
                                        {
                                            AppID = e.AppID,
                                            TimeLineSyncDataKey = e.TimeLineSyncDataKey,
                                            ID = e.ID,
                                            TimeLineName = e.TimeLineName,
                                            FactoryID = e.FactoryID,
                                            OperationPositionID = e.OperationPositionID,
                                            CycleID = e.CycleID,
                                            TotalParametersDefault = e.TotalParametersDefault,
                                            TotalParametersChecked = e.TotalParametersChecked,
                                            TotalParametersSynchronized = e.TotalParametersSynchronized,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt,
                                        }).OrderBy(t => t.ID).ToList();

                masterDatas.OperationParameters = _unitOfWork.ElogbookRepository<OperationParameters>()
                                        .GetQuery(x => lstOperationTimeLineID.Contains((int)x.OperationTimeLineID))
                                        .Select(
                                            e => new MobileOperationParametersInfo()
                                            {
                                                AppID = e.AppID,
                                                SyncDataKey = e.SyncDataKey,
                                                ID = e.ID,
                                                ParameterName = e.ParameterName,
                                                ParameterID = e.ParameterID,
                                                ParameterGroupID = e.ParameterGroupID,
                                                OperationTimeLineID = e.OperationTimeLineID,
                                                FactoryID = e.FactoryID,
                                                OperationPositionID = e.OperationPositionID,
                                                CycleID = e.CycleID,
                                                CurrentValue = e.CurrentValue,
                                                KKSCode = e.KKSCode,
                                                UnitMeasure = e.UnitMeasure,
                                                Setting = e.Setting,
                                                FromRange = e.FromRange,
                                                ToRange = e.ToRange,
                                                IsNumberType = e.IsNumberType,
                                                PreviousValue = e.PreviousValue,
                                                ErrorCode = e.ErrorCode,
                                                IsChecked = e.IsChecked,
                                                AlertDetails = e.AlertDetails,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt,
                                                CheckedDate = e.CheckedDate,
                                            }).OrderBy(t => t.ID).ToList();
            }

            responseModel.Content = masterDatas;

            return responseModel;
        }

        public void UpdateCountData(OperationParametersSyncDataModel requestModel)
        {
            if (requestModel != null && requestModel.OperationTimeLines != null && requestModel.OperationTimeLines.Count > 0)
            {
                List<string> dataExist = new List<string>();
                foreach (var timeLine in requestModel.OperationTimeLines)
                {
                    var timelineUpdate = _unitOfWork.ElogbookRepository<OperationTimeLine>()
                                    .GetQuery(x => x.TimeLineSyncDataKey.Equals(timeLine.SyncDataKey)
                                                    && !x.IsDeleted
                                                    && x.CycleID == timeLine.CycleID
                                                    && x.FactoryID == timeLine.FactoryID
                                                    && x.TimeLineName == timeLine.TimeLineName
                                                    && x.OperationPositionID == timeLine.OperationPositionID).FirstOrDefault();

                    if (timelineUpdate != null && !dataExist.Contains(timeLine.SyncDataKey + "::" + timelineUpdate.ID))
                    {
                        //check if
                        var _b = _unitOfWork.DataContext.OperationTimeLine.Find(timelineUpdate.ID);

                        if (_b != null)
                        {
                            dataExist.Add(timeLine.SyncDataKey + "::" + timelineUpdate.ID);
                            var entityCount = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.SyncDataKey.Equals(timeLine.SyncDataKey)
                                                                                                        && x.CycleID == timeLine.CycleID
                                                                                                        && x.FactoryID == timeLine.FactoryID
                                                                                                        && x.OperationTimeLineID == timelineUpdate.ID
                                                                                                        && (x.CurrentValue != null || x.ErrorCode.HasValue)
                                                                                                            && !x.IsDeleted).Select(x => x.ID);

                            _b.TotalParametersSynchronized = entityCount.Count();
                            _b.TotalParametersChecked = entityCount.Count();
                            _b.SyncedDate = DateTime.Now;
                            _unitOfWork.ElogbookRepository<OperationTimeLine>().Update(_b);
                        }
                    }
                }

                _unitOfWork.Complete();
            }
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel OperationParameterSyncData(OperationParametersSyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            List<int> lstOperationTimeLineID = new List<int>();
            List<string> dataExist = new List<string>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                _unitOfWork.DataContext.Database.CommandTimeout = 3600;
                LogCustom.LogMessage(TrackingLogLevel.DEBUG, "OperationParametersService",
                Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine);

                try
                {
                    if (requestModel != null && requestModel.OperationTimeLines != null && requestModel.OperationTimeLines.Count > 0)
                    {
                        //Các khung giờ ghi thông số vận hành
                        foreach (var timeLine in requestModel.OperationTimeLines)
                        {
                            var operationTimeLineID = 0;
                            OperationTimeLine operationTimeLine = null;
                            bool isHasChanged = false;
                            //Tạo mới khung giờ ghi thông số
                            if (timeLine.IsNew)
                            {
                                if (!string.IsNullOrEmpty(timeLine.SyncDataKey))
                                {
                                    var operationTimeLineUpdate = _unitOfWork.ElogbookRepository<OperationTimeLine>()
                                    .GetQuery(x => x.TimeLineSyncDataKey.Equals(timeLine.SyncDataKey)
                                                    && !x.IsDeleted
                                                    && x.CycleID == timeLine.CycleID
                                                    && x.FactoryID == timeLine.FactoryID
                                                    && x.OperationPositionID == timeLine.OperationPositionID).FirstOrDefault();

                                    //Kiểm tra khung giờ đã tồn tại thì cập nhật
                                    if (operationTimeLineUpdate != null)
                                    {
                                        double diffTimeUpdate = (timeLine.UpdatedAt - operationTimeLineUpdate.UpdatedAt).TotalMilliseconds;
                                        //Chỉ cập nhất nếu thời gian thay đổi lớn hơn thời gian đã cập nhật trước đó
                                        if (diffTimeUpdate > 0)
                                        {
                                            operationTimeLineUpdate.AppID = timeLine.AppID;
                                            operationTimeLineUpdate.TotalParametersDefault = timeLine.TotalParametersDefault;
                                            operationTimeLineUpdate.TotalParametersChecked = timeLine.TotalParametersChecked;
                                            operationTimeLineUpdate.TotalParametersSynchronized = timeLine.TotalParametersSynchronized;
                                            operationTimeLineUpdate.IsDeleted = timeLine.IsDeleted;
                                            if (timeLine.LastUpdatedUserID != null)
                                            {
                                                operationTimeLineUpdate.LastUpdatedUserID = timeLine.LastUpdatedUserID;
                                            }
                                            operationTimeLineUpdate.UpdatedAt = timeLine.UpdatedAt;
                                            operationTimeLineUpdate.SyncedDate = DateTime.Now;
                                            _unitOfWork.ElogbookRepository<OperationTimeLine>().Update(operationTimeLineUpdate);
                                            _unitOfWork.Complete();
                                        }

                                        lstOperationTimeLineID.Add(operationTimeLineUpdate.ID);
                                        operationTimeLineID = operationTimeLineUpdate.ID;
                                    }
                                    //Nếu chưa tồn tại khung giờ thì thêm mới để tạo dữ liêu
                                    else
                                    {
                                        var count = 0;
                                        foreach (var item in requestModel.OperationParameters)
                                        {
                                            if (item.IsNew && !string.IsNullOrEmpty(item.SyncDataKey) && (item.CurrentValue != null || item.ErrorCode.HasValue))
                                            {
                                                var entity = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.SyncDataKey.Equals(item.SyncDataKey)
                                                                                                                        && x.ParameterID == item.ParameterID
                                                                                                                        && !x.IsDeleted).FirstOrDefault();
                                                //kiểm tra tồn tại
                                                if (entity == null)
                                                {
                                                    count += 1;
                                                }
                                            }
                                        }
                                        operationTimeLine = AddOperationTimeLine(timeLine, count);
                                        operationTimeLineID = operationTimeLine.ID;
                                        lstOperationTimeLineID.Add(operationTimeLine.ID);
                                    }
                                }
                            }
                            //Cập nhật khung giờ đã tồn tại
                            else
                            {
                                operationTimeLineID = UpdateOperationTimeLine(timeLine);
                                lstOperationTimeLineID.Add(operationTimeLineID);
                            }

                            //Danh sách dữ liệu Thông số vận hành
                            if (requestModel.OperationParameters != null && requestModel.OperationParameters.Count > 0)
                            {
                                foreach (var item in requestModel.OperationParameters)
                                {
                                    //Thêm mới dữ liệu
                                    if (item.IsNew)
                                    {
                                        if (!string.IsNullOrEmpty(item.SyncDataKey) && !dataExist.Contains(item.SyncDataKey + "=" + item.ParameterID))
                                        {
                                            var entity = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.SyncDataKey.Equals(item.SyncDataKey)
                                                                                                                    && x.ParameterID == item.ParameterID
                                                                                                                    && !x.IsDeleted).FirstOrDefault();
                                            //Nếu đã tồn tại thì cập nhật
                                            if (entity != null)
                                            {
                                                double diffTimeUpdate = (item.CheckedDate.GetValueOrDefault(System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                                                                            - entity.CheckedDate.GetValueOrDefault(System.Data.SqlTypes.SqlDateTime.MinValue.Value)).TotalMilliseconds;
                                                //Chỉ cập nhất nếu thời gian thay đổi lớn hơn thời gian đã cập nhật trước đó
                                                if (diffTimeUpdate > 0)
                                                {
                                                    entity.OperationTimeLineID = operationTimeLineID;
                                                    entity.ParameterName = !string.IsNullOrEmpty(item.ParameterName) ? item.ParameterName : string.Empty;
                                                    entity.ParameterID = item.ParameterID;
                                                    entity.ParameterGroupID = item.ParameterGroupID != null ? item.ParameterGroupID : null;
                                                    entity.FactoryID = item.FactoryID;
                                                    entity.OperationPositionID = item.OperationPositionID != null ? item.OperationPositionID : null;
                                                    entity.CycleID = item.CycleID != null ? item.CycleID : null;
                                                    entity.KKSCode = !string.IsNullOrEmpty(item.KKSCode) ? item.KKSCode : null;
                                                    entity.UnitMeasure = !string.IsNullOrEmpty(item.UnitMeasure) ? item.UnitMeasure : string.Empty;
                                                    entity.Setting = !string.IsNullOrEmpty(item.Setting) ? item.Setting : null;
                                                    entity.FromRange = item.FromRange != null ? item.FromRange : null;
                                                    entity.ToRange = item.ToRange != null ? item.ToRange : null;
                                                    entity.IsNumberType = item.IsNumberType;
                                                    entity.PreviousValue = !string.IsNullOrEmpty(item.PreviousValue) ? item.PreviousValue : null;
                                                    entity.IsChecked = item.IsChecked;
                                                    entity.AlertDetails = !string.IsNullOrEmpty(item.AlertDetails) ? item.AlertDetails : null;
                                                    entity.IsDeleted = item.IsDeleted;
                                                    if (item.LastUpdatedUserID != null)
                                                    {
                                                        entity.LastUpdatedUserID = item.LastUpdatedUserID;
                                                    }
                                                    entity.UpdatedAt = item.UpdatedAt;
                                                    entity.SyncedDate = DateTime.Now;
                                                    dataExist.Add(item.SyncDataKey + "=" + item.ParameterID);

                                                    entity.AppID = item.AppID;
                                                    if (item.CheckedDate.HasValue)
                                                    {
                                                        entity.CheckedDate = item.CheckedDate;
                                                    }

                                                    if (!string.IsNullOrWhiteSpace(item.CurrentValue))
                                                    {
                                                        entity.CurrentValue = item.CurrentValue;
                                                    }

                                                    if (item.ErrorCode.HasValue)
                                                    {
                                                        entity.ErrorCode = item.ErrorCode.Value;
                                                        entity.CurrentValue = null;
                                                    }
                                                    else
                                                    {
                                                        entity.ErrorCode = null;
                                                    }
                                                }

                                                _unitOfWork.ElogbookRepository<OperationParameters>().Update(entity);
                                                isHasChanged = true;
                                            }
                                            //Thêm mới thông số
                                            else
                                            {
                                                OperationParameters operationParameters = new OperationParameters()
                                                {
                                                    AppID = item.AppID,
                                                    SyncDataKey = !string.IsNullOrEmpty(item.SyncDataKey) ? item.SyncDataKey : null,
                                                    ParameterName = !string.IsNullOrEmpty(item.ParameterName) ? item.ParameterName : string.Empty,
                                                    ParameterID = item.ParameterID,
                                                    ParameterGroupID = item.ParameterGroupID != null ? item.ParameterGroupID : null,
                                                    OperationTimeLineID = operationTimeLineID,
                                                    FactoryID = item.FactoryID,
                                                    OperationPositionID = item.OperationPositionID != null ? item.OperationPositionID : null,
                                                    CycleID = item.CycleID != null ? item.CycleID : null,
                                                    KKSCode = !string.IsNullOrEmpty(item.KKSCode) ? item.KKSCode : null,
                                                    UnitMeasure = !string.IsNullOrEmpty(item.UnitMeasure) ? item.UnitMeasure : string.Empty,
                                                    Setting = !string.IsNullOrEmpty(item.Setting) ? item.Setting : null,
                                                    FromRange = item.FromRange != null ? item.FromRange : null,
                                                    ToRange = item.ToRange != null ? item.ToRange : null,
                                                    IsNumberType = item.IsNumberType,
                                                    PreviousValue = !string.IsNullOrEmpty(item.PreviousValue) ? item.PreviousValue : null,
                                                    IsChecked = item.IsChecked,
                                                    AlertDetails = !string.IsNullOrEmpty(item.AlertDetails) ? item.AlertDetails : null,
                                                    IsDeleted = false,
                                                    InsertedAt = item.InsertedAt,
                                                    UpdatedAt = item.UpdatedAt,
                                                    SyncedDate = DateTime.Now
                                                };

                                                if (!string.IsNullOrWhiteSpace(item.CurrentValue))
                                                {
                                                    operationParameters.CurrentValue = item.CurrentValue;
                                                }

                                                if (item.ErrorCode.HasValue)
                                                {
                                                    operationParameters.ErrorCode = item.ErrorCode.Value;
                                                    operationParameters.CurrentValue = null;
                                                }
                                                else
                                                {
                                                    operationParameters.ErrorCode = null;
                                                }

                                                if (item.CheckedDate.HasValue)
                                                {
                                                    operationParameters.CheckedDate = item.CheckedDate;
                                                }

                                                if (item.CreatedUserID != null)
                                                {
                                                    operationParameters.CreatedUserID = item.CreatedUserID;
                                                }

                                                if (item.LastUpdatedUserID != null)
                                                {
                                                    operationParameters.LastUpdatedUserID = item.LastUpdatedUserID;
                                                }

                                                _unitOfWork.ElogbookRepository<OperationParameters>().Add(operationParameters);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isHasChanged = true;
                                        UpdateOperationParameters(item, operationTimeLineID);
                                    }
                                }

                                _unitOfWork.Complete();
                            }

                            if (isHasChanged)
                            {
                                var timelineUpdate = _unitOfWork.ElogbookRepository<OperationTimeLine>()
                                .GetQuery(x => x.TimeLineSyncDataKey.Equals(timeLine.SyncDataKey)
                                                && !x.IsDeleted
                                                && x.CycleID == timeLine.CycleID
                                                && x.FactoryID == timeLine.FactoryID
                                                && x.TimeLineName == timeLine.TimeLineName
                                                && x.OperationPositionID == operationTimeLineID).FirstOrDefault();

                                if (timelineUpdate != null)
                                {

                                    var entityCount = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.SyncDataKey.Equals(timeLine.SyncDataKey)
                                                                                                                && x.CycleID == timeLine.CycleID
                                                                                                                && x.FactoryID == timeLine.FactoryID
                                                                                                                && x.OperationTimeLineID == timeLine.ID
                                                                                                                && (x.CurrentValue != null || x.ErrorCode.HasValue)
                                                                                                                && !x.IsDeleted).Count();

                                    timelineUpdate.TotalParametersSynchronized = entityCount;
                                    timelineUpdate.SyncedDate = DateTime.Now;
                                    _unitOfWork.ElogbookRepository<OperationTimeLine>().Update(timelineUpdate);
                                    _unitOfWork.Complete();
                                }
                            }
                        }
                    }
                    transaction.Commit();
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    responseModel.Message = "Đồng bộ dữ liệu thành công";
                    responseModel.Content = lstOperationTimeLineID;
                }
                catch (Exception ex)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationParametersService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    responseModel.Message = "Đồng bộ dữ liệu thất bại. Vui lòng liên hệ quản trị viên để kiểm tra lỗi.\n" + ex.ToString();
                    transaction.Rollback();

                    return responseModel;
                }
            }

            return responseModel;
        }

        private int UpdateOperationTimeLine(OperationTimeLineSyncModel timeLine)
        {
            OperationTimeLine entity;
            if (!timeLine.IsNew && timeLine.ID > 0)
            {
                entity = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.ID == timeLine.ID && !x.IsDeleted).FirstOrDefault();
            }
            else
            {
                entity = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.TimeLineSyncDataKey.Equals(timeLine.SyncDataKey) && !x.IsDeleted).FirstOrDefault();
            }

            if (entity != null)
            {
                entity.AppID = timeLine.AppID;
                entity.TotalParametersDefault = timeLine.TotalParametersDefault;
                entity.TotalParametersChecked = timeLine.TotalParametersChecked;
                entity.IsDeleted = timeLine.IsDeleted;
                if (timeLine.LastUpdatedUserID != null)
                {
                    entity.LastUpdatedUserID = timeLine.LastUpdatedUserID;
                }
                entity.UpdatedAt = timeLine.UpdatedAt;
                entity.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<OperationTimeLine>().Update(entity);
                _unitOfWork.Complete();

                return entity.ID;
            }

            return 0;
        }

        private OperationTimeLine AddOperationTimeLine(OperationTimeLineSyncModel timeLine, int count)
        {
            OperationTimeLine operationTimeLine = new OperationTimeLine()
            {
                AppID = timeLine.AppID,
                TimeLineSyncDataKey = timeLine.SyncDataKey,
                TimeLineName = timeLine.TimeLineName,
                FactoryID = timeLine.FactoryID,
                OperationPositionID = timeLine.OperationPositionID != null ? timeLine.OperationPositionID : null,
                CycleID = timeLine.CycleID != null ? timeLine.CycleID : null,
                TotalParametersDefault = timeLine.TotalParametersDefault,
                TotalParametersChecked = timeLine.TotalParametersChecked,
                TotalParametersSynchronized = count,
                IsDeleted = false,
                UpdatedAt = timeLine.UpdatedAt,
                SyncedDate = DateTime.Now
            };

            if (timeLine.CreatedUserID != null)
            {
                operationTimeLine.CreatedUserID = timeLine.CreatedUserID;
            }

            if (timeLine.LastUpdatedUserID != null)
            {
                operationTimeLine.LastUpdatedUserID = timeLine.LastUpdatedUserID;
            }

            operationTimeLine.InsertedAt = timeLine.InsertedAt.Date;
            _unitOfWork.ElogbookRepository<OperationTimeLine>().Add(operationTimeLine);
            _unitOfWork.Complete();

            return operationTimeLine;
        }

        private void UpdateOperationParameters(OperationParametersSyncModel parameter, int operationTimeLineID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.SyncDataKey.Equals(parameter.SyncDataKey)
                                                                                                                        && x.ParameterID == parameter.ParameterID
                                                                                                                        && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                double diffTimeUpdate = (parameter.CheckedDate.GetValueOrDefault(System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                                            - entity.CheckedDate.GetValueOrDefault(System.Data.SqlTypes.SqlDateTime.MinValue.Value)).TotalMilliseconds;
                if (diffTimeUpdate > 0)
                {
                    entity.AppID = parameter.AppID;
                    entity.OperationTimeLineID = operationTimeLineID;
                    entity.ParameterName = !string.IsNullOrEmpty(parameter.ParameterName) ? parameter.ParameterName : string.Empty;
                    entity.ParameterID = parameter.ParameterID;
                    entity.ParameterGroupID = parameter.ParameterGroupID != null ? parameter.ParameterGroupID : null;
                    entity.FactoryID = parameter.FactoryID;
                    entity.OperationPositionID = parameter.OperationPositionID != null ? parameter.OperationPositionID : null;
                    entity.CycleID = parameter.CycleID != null ? parameter.CycleID : null;
                    entity.KKSCode = !string.IsNullOrEmpty(parameter.KKSCode) ? parameter.KKSCode : null;
                    entity.UnitMeasure = !string.IsNullOrEmpty(parameter.UnitMeasure) ? parameter.UnitMeasure : string.Empty;
                    entity.Setting = !string.IsNullOrEmpty(parameter.Setting) ? parameter.Setting : null;
                    entity.FromRange = parameter.FromRange != null ? parameter.FromRange : null;
                    entity.ToRange = parameter.ToRange != null ? parameter.ToRange : null;
                    entity.IsNumberType = parameter.IsNumberType;
                    entity.PreviousValue = !string.IsNullOrEmpty(parameter.PreviousValue) ? parameter.PreviousValue : null;
                    entity.IsChecked = parameter.IsChecked;
                    entity.AlertDetails = !string.IsNullOrEmpty(parameter.AlertDetails) ? parameter.AlertDetails : null;
                    entity.IsDeleted = parameter.IsDeleted;
                    entity.SyncedDate = DateTime.Now;
                    if (parameter.LastUpdatedUserID != null)
                    {
                        entity.LastUpdatedUserID = parameter.LastUpdatedUserID;
                    }

                    entity.UpdatedAt = parameter.UpdatedAt;

                    if (parameter.CheckedDate.HasValue)
                    {
                        entity.CheckedDate = parameter.CheckedDate;
                    }

                    if (!string.IsNullOrWhiteSpace(parameter.CurrentValue))
                    {
                        entity.CurrentValue = parameter.CurrentValue;
                    }

                    if (parameter.ErrorCode.HasValue)
                    {
                        entity.ErrorCode = parameter.ErrorCode.Value;
                        entity.CurrentValue = null;
                    }
                    else
                    {
                        entity.ErrorCode = null;
                    }

                    _unitOfWork.ElogbookRepository<OperationParameters>().Update(entity);
                }
            }
        }

        private bool CheckExistOperationParameterByCurrentDate(int factoryID, int operationPositionID, int cycleID)
        {
            DateTime currentDate = DateTimeConvert.ConvertDateTimeFromString(DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/"), true);
            return _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == cycleID
                                                                                        && x.OperationPositionID == operationPositionID
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) == currentDate
                                                                                        && x.FactoryID == factoryID
                                                                                        && !x.IsDeleted).Any();
        }

        private string GetColorCode(string colorKey)
        {
            var colorCode = string.Empty;

            var alarmSetting = _unitOfWork.ElogbookRepository<AlarmSetting>().GetQuery(x => x.SettingType == 3 && x.SettingName.Equals(colorKey) && !x.IsDeleted).FirstOrDefault(); //3: cấu hình màu sắc
            if (alarmSetting != null)
            {
                colorCode = alarmSetting.ColorCode;
            }
            else
            {
                switch (colorKey)
                {
                    case "Red":
                        colorCode = "#FB1F00";
                        break;
                    case "Blue":
                        colorCode = "#06509B";
                        break;
                    case "Yellow":
                        colorCode = "#FFC700";
                        break;
                }
            }

            return colorCode;
        }

        /// <summary>
        /// Tổng số thông số của mỗi chu kỳ
        /// </summary>
        /// <param name="cycleID"></param>
        /// <returns></returns>
        private int GetCountParamaterByCycleID(int cycleID, int operationPositionID, int factoryID)
        {
            //Tổng thông số của Nhà máy, Cương vị, Chu kỳ
            return _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.CycleID == cycleID
                                                                             && x.OperationPositionID == operationPositionID
                                                                             && x.FactoryID == factoryID
                                                                             && x.Status == (int)AccessStatus.Active
                                                                             && !x.IsDeleted).Count();
        }

        private List<int> ScanPrameters(SearchParametersModel requestModel, int authenticationType)
        {
            // Tất cả thông số của Nhà máy, Cương vị, Chu kỳ
            var entities = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                && x.OperationPositionID == requestModel.OperationPositionID
                                                                                && x.CycleID == requestModel.CycleID
                                                                                && x.Status == (int)AccessStatus.Active
                                                                                && !x.IsDeleted);

            // Trường hợp thông số thuộc nhóm thông số
            if (requestModel.ParameterGroupID.GetValueOrDefault(0) > 0)
            {
                entities = entities.Where(x => x.IsGroup && x.ParameterGroupID == requestModel.ParameterGroupID);
            }
            // Trường hợp thông số lẽ
            else
            {
                entities = entities.Where(x => !x.IsGroup);
            }

            switch (authenticationType)
            {
                // Dùng thẻ RFID
                case (int)AthenticationType.RFID:
                    entities = entities.Where(x => x.RFIDCode == requestModel.AuthenticationCode);
                    break;
                // Scan QR Code
                case (int)AthenticationType.QrCode:
                    entities = entities.Where(x => x.QRCode == requestModel.AuthenticationCode);
                    break;
            }

            return entities.Select(x => x.ID).ToList();
        }

        private string GetColorAlarm(ParameterModel prEntity, string currentValue)
        {
            string color = string.Empty;
            if (prEntity != null)
            {
                var alertDetailModels = prEntity.AlertDetails;
                float.TryParse(currentValue, out float value);

                Dictionary<int, string> opreratorColor = new Dictionary<int, string>();
                List<int> lstPriority = new List<int>();
                if (alertDetailModels != null)
                {
                    foreach (var al in alertDetailModels)
                    {
                        switch (al.Operator)
                        {
                            case "=":
                                if (value == al.Value && !opreratorColor.ContainsKey(1))
                                {
                                    opreratorColor.Add(1, al.Color); lstPriority.Add(1);
                                }
                                break;
                            case ">=":
                                if (value >= al.Value && !opreratorColor.ContainsKey(2))
                                {
                                    opreratorColor.Add(2, al.Color); lstPriority.Add(2);
                                }
                                break;
                            case "<=":
                                if (value <= al.Value && !opreratorColor.ContainsKey(3))
                                {
                                    opreratorColor.Add(3, al.Color); lstPriority.Add(3);
                                }
                                break;
                            case ">":
                                if (value > al.Value && !opreratorColor.ContainsKey(4))
                                {
                                    opreratorColor.Add(4, al.Color); lstPriority.Add(4);
                                }
                                break;
                            case "<":
                                if (value < al.Value && !opreratorColor.ContainsKey(5))
                                {
                                    opreratorColor.Add(5, al.Color); lstPriority.Add(5);
                                }
                                break;
                        }
                    }
                }

                if (opreratorColor.Count > 0)
                {
                    foreach (KeyValuePair<int, string> dict in opreratorColor)
                    {
                        if (dict.Key == lstPriority.Min())
                        {
                            color = dict.Value;
                            break;
                        }
                    }
                }
            }

            return color;
        }

        private string GetPreviousValue(int factoryID, int operationPositionID, int cycleID, int operationTimeLineID, int parameterID)
        {
            var operationParameters = _unitOfWork.ElogbookRepository<OperationParameters>().GetQuery(x => x.FactoryID == factoryID
                                                                                && x.OperationPositionID == operationPositionID
                                                                                && x.CycleID == cycleID
                                                                                && x.OperationTimeLineID < operationTimeLineID
                                                                                && x.ParameterID == parameterID
                                                                                && !x.IsDeleted)
                                                                                .OrderByDescending(i => i.OperationTimeLineID)
                                                                                .ToPagedList(1, 5)
                                                                                .Select(t => new { t.CurrentValue, t.ErrorCode, t.OperationTimeLineID })
                                                                                .ToList();

            StringBuilder previousValue = new StringBuilder();
            var totalRecords = operationParameters.Count;

            var tempParams = operationParameters.OrderBy(x => x.OperationTimeLineID).ToList();
            var index = 0;
            foreach (var item in tempParams)
            {
                index++;
                var currentValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(item.CurrentValue))
                {
                    currentValue = item.CurrentValue;
                }

                if (item.ErrorCode.HasValue)
                {
                    switch (item.ErrorCode)
                    {
                        // Thiết bị bị hỏng (F)
                        case (int)DeviceErrorCode.Error:
                            currentValue = "F";
                            break;
                        // Dừng thiết bị (-)
                        case (int)DeviceErrorCode.Stop:
                            currentValue = "-";
                            break;
                        // Sim thiết bị (S)
                        case (int)DeviceErrorCode.Sim:
                            currentValue = "S";
                            break;
                        // Jump thiết bị (J)
                        case (int)DeviceErrorCode.Jump:
                            currentValue = "J";
                            break;
                        // Bận công tác
                        case (int)DeviceErrorCode.Busy:
                            currentValue = "x";
                            break;
                    }
                }

                if (totalRecords == index)
                {
                    previousValue.Append(currentValue);
                }
                else
                {
                    previousValue.Append(currentValue + " : ");
                }
            }

            return previousValue.Length > 0 ? previousValue.ToString() : null;
        }

        /// <summary>
        /// Lấy dữ liệu để ghi file PDF
        /// </summary>
        private PdfOperationParameters CreateDataPdf(PdfExportOPModel requestModel)
        {
            PdfOperationParameters model = new PdfOperationParameters
            {
                // Tạo bảng Tiêu đề
                Header = GetPdfHeaderInfo(requestModel),

                // Tạo danh sách bảng thông số vận hành
                ListCycle = GetOperationParametersPrintV2(requestModel),

                // Tạo bảng nhật ký vận hành
                ListOperationDiaryInfo = GetOperationDiaryInfo(requestModel)
            };

            return model;
        }

        /// <summary>
        /// Tạo file PDF
        /// </summary>
        private bool CreatePDFContent(PdfOperationParameters pdfDataModel, string outputFile)
        {
            try
            {
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    //Create document
                    Document pdfDocument = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                    pdfDocument.SetMargins(5, 5, 5, 5);

                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, fileStream);

                    HeaderFooterOperationParameter tevent = new HeaderFooterOperationParameter();
                    pdfWriter.PageEvent = tevent;
                    // Open the document to enable you to write to the document
                    pdfDocument.Open();

                    try
                    {
                        PdfPTable mainTable = new PdfPTable(5);
                        float[] floatArray = { 1.2f, 1, 1, 1, 1.2f };
                        mainTable.SetWidths(floatArray);
                        mainTable.TotalWidth = 830;
                        mainTable.LockedWidth = true;
                        mainTable.HeaderRows = 5;
                        // set dữ liệu tiêu đề file
                        AppendTableHeader(mainTable, pdfDataModel.Header);

                        // set dữ liệu thông số vận hành
                        if (pdfDataModel.ListCycle != null)
                        {
                            foreach (var item in pdfDataModel.ListCycle)
                            {
                                if (item.ListOperationParameters != null && item.ListOperationParameters.Count > 0)
                                {
                                    AppendTableOperationParameter(mainTable, item);
                                }
                            }
                        }
                        pdfDocument.Add(mainTable);
                        pdfDocument.NewPage();
                        // set dữ liệu nhật ký vận hành
                        if (pdfDataModel.ListOperationDiaryInfo != null)
                        {
                            PdfPTable mainTable2 = new PdfPTable(5);
                            mainTable2.SetWidths(floatArray);
                            mainTable2.TotalWidth = 830;
                            mainTable2.LockedWidth = true;
                            mainTable2.HeaderRows = 5;

                            // set dữ liệu tiêu đề file
                            AppendTableHeader(mainTable2, pdfDataModel.Header);
                            PdfPTable tblODInfo = new PdfPTable(4) { TotalWidth = 830, };
                            tblODInfo.SetWidths(new float[] { 1, 1, 2.5f, 1 });
                            tblODInfo.LockedWidth = (true);
                            tblODInfo.SplitRows = false;
                            int fSize = 8;
                            tblODInfo.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Ca trực", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            tblODInfo.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Họ và tên VHV", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            tblODInfo.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Nhận xét thông số cập nhật trong ca", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                            tblODInfo.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Ký tên", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                            PdfPCell pdfPCell = new PdfPCell
                            {
                                Colspan = 5,
                                Padding = 0
                            };
                            PdfPCell cellMerge = pdfPCell;
                            cellMerge.AddElement(tblODInfo);
                            mainTable2.AddCell(cellMerge);

                            foreach (var item in pdfDataModel.ListOperationDiaryInfo)
                            {
                                AppendTableOperationDiaryInfo(mainTable2, item);
                            }
                            pdfDocument.Add(mainTable2);
                        }

                        pdfDocument.Close();
                    }
                    finally
                    {
                        pdfDocument.Close();
                        pdfWriter.Close();
                        fileStream.Close();
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Lấy dữ liệu Thông số vận hành
        /// </summary>
        private List<PdfCycleModel> GetOperationParametersPrintV2(PdfExportOPModel requestModel)
        {
            DateTime createDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreateDate, true);
            // lấy tất cả chu kỳ được ghi theo ngày, theo cương vị
            // loop chu kỳ lấy thông số vận hành theo cương vị, chu kỳ, ngày ghi
            var lstCycleOperationParameter = new List<PdfCycleModel>();

            var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                    {
                        Value = requestModel.FactoryID,
                    },
                    new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                    {
                        Value = requestModel.OperationPositionID,
                    },
                    new SqlParameter("InsertedAt", System.Data.SqlDbType.Date)
                    {
                        Value = createDate,
                    },
                };

            //Danh sách ghi thông số
            var parameters = _unitOfWork.DataContext.Database
            .SqlQuery<ParameterValueModel>("TSVH_GetParameterValuesPrint @FactoryID, @OperationPositionID, @InsertedAt", sqlParams).ToList();

            List<int> lstCycleID = new List<int>();
            if (parameters != null && parameters.Count > 0)
            {
                lstCycleID = parameters.Select(x => x.CycleID).Distinct().ToList();
            }

            foreach (var cycleID in lstCycleID)
            {
                PdfCycleModel cycleOperationParameterModel = new PdfCycleModel();
                // lấy danh sách nhóm thông số theo nhà máy, cương vị, chu kỳ
                var parameterGroup = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => x.CycleID == cycleID
                                                                            && x.OperationPositionID == requestModel.OperationPositionID
                                                                            && x.FactoryID == requestModel.FactoryID
                                                                            && x.Status == (int)AccessStatus.Active
                                                                            && !x.IsDeleted).ToList();

                List<int> lstParameterID = new List<int>();
                if (parameterGroup != null && parameterGroup.Count > 0)
                {
                    lstParameterID = parameterGroup.Select(x => x.ID).ToList();
                }

                // lấy danh sách tất cả thông số theo chu kỳ
                var parametersEntities = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                    && x.OperationPositionID == requestModel.OperationPositionID
                                                                                    && x.CycleID == cycleID
                                                                                    && (lstParameterID.Contains((int)x.ParameterGroupID) || x.ParameterGroupID == null)
                                                                                    && x.Status == (int)AccessStatus.Active
                                                                                    && !x.IsDeleted);

                // lấy danh sách khung giờ theo chu kỳ
                var timeLineCycleEntities = _unitOfWork.ElogbookRepository<TimeLineCycle>().GetQuery(x => x.CycleID == cycleID
                                                                                    && x.Active == (int)AccessStatus.Active
                                                                                    && !x.IsDeleted).OrderBy(x => x.Hour).ToList();

                var lstOperationParameters = new List<OperationParametersWebModel>();
                foreach (var pr in parametersEntities.ToList())
                {
                    List<ParameterValueModel> parameterValueModels = new List<ParameterValueModel>();
                    if (createDate.Year > 2022)
                    {
                        parameterValueModels = parameters.Where(x => x.ParameterID == pr.ID && x.CycleID == cycleID)
                                .Select(e => new ParameterValueModel
                                {
                                    OpeartionTimeLineName = e.OpeartionTimeLineName,
                                    ParameterValue = e.ParameterValue
                                }).ToList();
                    }
                    //Fix issue print pdf not correct with old data from application NKVH
                    else
                    {
                        foreach (var (tl, items) in from tl in timeLineCycleEntities
                                                    let items = parameters.Find(x => x.ParameterID == pr.ID && x.CycleID == cycleID && tl.Hour.ToString() == x.OpeartionTimeLineName)
                                                    select (tl, items))
                        {
                            if (items != null)
                            {
                                parameterValueModels.Add(new ParameterValueModel
                                {
                                    OpeartionTimeLineName = items.OpeartionTimeLineName,
                                    ParameterValue = items.ParameterValue,
                                });
                            }
                            else
                            {
                                parameterValueModels.Add(new ParameterValueModel
                                {
                                    OpeartionTimeLineName = tl.Hour.ToString(),
                                    ParameterValue = string.Empty,
                                });
                            }
                        }
                    }

                    // add 1 dữ liệu vào list output
                    OperationParametersWebModel operationParametersWebModel = new OperationParametersWebModel()
                    {
                        ParamaterID = pr.ID,
                        ParameterName = pr.ParameterName,
                        UnitMeasure = pr.UnitMeasure,
                        KKSCode = pr.KKSCode,
                        ParameterValues = parameterValueModels
                    };

                    if (pr.FromRange != null)
                    {
                        operationParametersWebModel.FromRange = (float)pr.FromRange;
                    }
                    else
                    {
                        operationParametersWebModel.FromRange = null;
                    }

                    if (pr.ToRange != null)
                    {
                        operationParametersWebModel.ToRange = (float)pr.ToRange;
                    }
                    else
                    {
                        operationParametersWebModel.ToRange = null;
                    }

                    lstOperationParameters.Add(operationParametersWebModel);
                }

                cycleOperationParameterModel.ListOperationParameters = lstOperationParameters;
                lstCycleOperationParameter.Add(cycleOperationParameterModel);
            }

            return lstCycleOperationParameter;
        }

        /// <summary>
        /// Lấy dữ liệu cho bảng OperationDiaryInfo
        /// </summary>
        private List<PdfOperationDiaryInfoModel> GetOperationDiaryInfo(PdfExportOPModel requestModel)
        {
            DateTime createDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreateDate, true);
            DateTime prevCreateDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreateDate, true).AddDays(-1);
            var lstOperationDiaryInfo = new List<PdfOperationDiaryInfoModel>();

            //trả về tất cả các ca được ghi trong ngày
            var odEntites = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationPositionID == requestModel.OperationPositionID
                                                                                                && x.FactoryID == requestModel.FactoryID
                                                                                                && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(createDate)
                                                                                                && !x.IsDeleted).OrderBy(t => t.ID).ToList();

            //đối với các nhà máy đi 3 ca thì vẫn hiển thị 3 ca thôi
            //chỉ có trưởng hợp 2 ca như BK mới cần thêm ca đêm vào
            if (odEntites.Count == 2)
            {
                var operationShift = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                                                            && x.FactoryID == requestModel.FactoryID)
                                                                                .OrderByDescending(t => t.StartTime).FirstOrDefault();
                //trả về tất cả các ca được ghi trong ca đêm của ngày trước
                var odEntitesPrev = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationPositionID == requestModel.OperationPositionID
                                                                                                    && x.FactoryID == requestModel.FactoryID
                                                                                                    && x.OperationShiftID == operationShift.ID
                                                                                                    && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(prevCreateDate)
                                                                                                    && !x.IsDeleted).OrderBy(t => t.ID).ToList();
                foreach (var item in odEntitesPrev)
                {
                    // lấy danh sách những người giao ca
                    var drsEntites = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.OperationDiaryID == item.ID
                                                                                                    && x.IsDeliveryShift
                                                                                                    && !x.IsDeleted).ToList();
                    var lstUser = drsEntites.Select(e => new PdfDeliveryReceiveShiftUserInfo()
                    {
                        FullName = e.Supervisor.FullName,
                        Signature = e.Supervisor.Signature
                    }).ToList();

                    PdfOperationDiaryInfoModel pdfOperationDiaryInfoModel = new PdfOperationDiaryInfoModel()
                    {
                        ShiftName = item.OperationShift.OperationShiftName,
                        Comments = item.Comments,
                        UserInfo = lstUser
                    };

                    lstOperationDiaryInfo.Add(pdfOperationDiaryInfoModel);
                }
            }

            foreach (var item in odEntites)
            {
                // lấy danh sách những người giao ca
                var drsEntites = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>().GetQuery(x => x.OperationDiaryID == item.ID
                                                                                                && x.IsDeliveryShift
                                                                                                && !x.IsDeleted).ToList();
                var lstUser = drsEntites.Select(e => new PdfDeliveryReceiveShiftUserInfo()
                {
                    FullName = e.Supervisor.FullName,
                    Signature = e.Supervisor.Signature
                }).ToList();

                PdfOperationDiaryInfoModel pdfOperationDiaryInfoModel = new PdfOperationDiaryInfoModel()
                {
                    ShiftName = item.OperationShift.OperationShiftName,
                    Comments = item.Comments,
                    UserInfo = lstUser
                };

                lstOperationDiaryInfo.Add(pdfOperationDiaryInfoModel);
            }

            return lstOperationDiaryInfo;
        }

        /// <summary>
        /// Lấy dữ liệu cho bảng Header
        /// </summary>
        private PdfHeaderOperationDiaryModel GetPdfHeaderInfo(PdfExportOPModel requestModel)
        {
            DateTime createDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.CreateDate, true);
            var entity = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID == requestModel.OperationPositionID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                PdfHeaderOperationDiaryModel pdfHeaderModel = new PdfHeaderOperationDiaryModel()
                {
                    OperationPositionName = entity.OperationPositionName,
                    FactoryName = entity.Factory.FactoryName,
                    ItemISO = "",
                    VersionRelease = "",
                    EffectiveDate = "",
                    DocumentCode = "",
                    PageNumber = " ",
                    CreateDate = "Ngày " + createDate.Day.ToString() + " tháng " + createDate.Month.ToString() + " năm " + createDate.Year.ToString(),
                    PathLogo = GetOrganizationLogo(requestModel.FactoryID)
                };

                return pdfHeaderModel;
            }

            return null;
        }

        private string GetOrganizationLogo(int? factoryId)
        {
            if (factoryId != null)
            {
                var entity = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == factoryId && !x.IsDeleted).FirstOrDefault();
                if (entity != null && entity.Company != null && entity.Company.AttachmentFile != null)
                {
                    return entity.Company.AttachmentFile.FilePath;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Chèn bảng Header vào file PDF
        /// </summary>
        private void AppendTableHeader(PdfPTable tblHeader, PdfHeaderOperationDiaryModel headerModel)
        {
            int fSize = 6;
            // Header Table.
            tblHeader.AddCell(PdfHelper.GetCellImageLogo(headerModel.PathLogo, 1, 4, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 75, true));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize + 3, "Bảng ghi thông số cương vị " + headerModel.OperationPositionName + " - " + headerModel.FactoryName, 3, 2, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, "Mã tài liệu ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, headerModel.DocumentCode, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, "Mục ISO: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, "Ban hành lần thứ: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, "Ngày hiệu lực: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, "Trang số: ", 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, headerModel.ItemISO, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, headerModel.VersionRelease, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, headerModel.EffectiveDate, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, headerModel.PageNumber, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblHeader.AddCell(PdfHelper.GetCellFixedHeight(f_bold_italic, fSize + 2, headerModel.CreateDate, 5, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
        }

        /// <summary>
        /// Chèn bảng Thông số vận hành vào file PDF
        /// </summary>
        private void AppendTableOperationParameter(PdfPTable mainTable, PdfCycleModel lstOperationParameter)
        {
            int fSize = 7;
            int dataRow = lstOperationParameter.ListOperationParameters.Count + 2;

            int colTable = lstOperationParameter.ListOperationParameters[0].ParameterValues.Count + 5;
            float[] floatArray = new float[colTable];
            floatArray[0] = 1;
            floatArray[1] = 6;
            floatArray[2] = 2;
            floatArray[3] = 3.5f;
            floatArray[4] = 2;
            PdfPTable tblOP = new PdfPTable(colTable)
            {
                TotalWidth = 830,
                LockedWidth = (true),
                SplitRows = false
            };
            for (int i = 0; i < lstOperationParameter.ListOperationParameters[0].ParameterValues.Count; i++)
            {
                floatArray[i + 5] = 1;
            }
            tblOP.SetWidths(floatArray);
            // Append Table
            for (int i = 0; i < dataRow; i++)
            {
                // Insert Header
                if (i == 0)
                {
                    InsertHeaderTableOperationParameter(tblOP, lstOperationParameter.ListOperationParameters);
                }
                // Insert Footer
                else if (i == dataRow - 1)
                {
                    tblOP.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, " ", colTable, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                }
                // Insert Content
                else
                {
                    tblOP.AddCell(PdfHelper.GetCell(_fNomal, fSize, stt.ToString(), 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblOP.AddCell(PdfHelper.GetCell(_fNomal, fSize, lstOperationParameter.ListOperationParameters[i - 1].ParameterName, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
                    tblOP.AddCell(PdfHelper.GetCell(_fNomal, fSize, lstOperationParameter.ListOperationParameters[i - 1].UnitMeasure, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblOP.AddCell(PdfHelper.GetCell(_fNomal, fSize, lstOperationParameter.ListOperationParameters[i - 1].KKSCode, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    tblOP.AddCell(PdfHelper.GetCell(_fNomal, fSize, lstOperationParameter.ListOperationParameters[i - 1].FromRange.ToString() + " - " + lstOperationParameter.ListOperationParameters[i - 1].ToRange.ToString(), 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                    foreach (var op in lstOperationParameter.ListOperationParameters[i - 1].ParameterValues)
                    {
                        tblOP.AddCell(PdfHelper.GetCell(_fNomal, fSize, op.ParameterValue, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    }
                    stt += 1;
                }
            }

            PdfPCell cellMerge = new PdfPCell
            {
                Colspan = 7,
                Padding = 0
            };
            cellMerge.AddElement(tblOP);
            mainTable.AddCell(cellMerge);

            stt = 1;
        }

        private void InsertHeaderTableOperationParameter(PdfPTable tblOP, List<OperationParametersWebModel> ListOperationParameters)
        {
            // Insert Header
            int fSize = 7;
            tblOP.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Stt", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblOP.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Thông số", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblOP.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Đơn vị", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblOP.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Mã KKS", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            tblOP.AddCell(PdfHelper.GetCellFixedHeight(_fBold, fSize, "Range", 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

            foreach (var item in ListOperationParameters[0].ParameterValues)
            {
                tblOP.AddCell(PdfHelper.GetCell(_fBold, fSize, item.OpeartionTimeLineName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
            }
        }

        private void AppendTableOperationDiaryInfo(PdfPTable mainTable, PdfOperationDiaryInfoModel lstOperationDiaryInfo)
        {
            if (lstOperationDiaryInfo.UserInfo != null && lstOperationDiaryInfo.UserInfo.Count > 0)
            {
                PdfPTable tblODInfo = new PdfPTable(4) { TotalWidth = 830, };
                tblODInfo.SetWidths(new float[] { 1, 1, 2.5f, 1 });
                tblODInfo.LockedWidth = (true);
                tblODInfo.SplitRows = false;
                int countRow = lstOperationDiaryInfo.UserInfo.Count;
                int fSize = 8;

                tblODInfo.AddCell(PdfHelper.GetCell(_fNomal, fSize, lstOperationDiaryInfo.ShiftName, 1, countRow, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));

                for (int i = 0; i < lstOperationDiaryInfo.UserInfo.Count; i++)
                {
                    tblODInfo.AddCell(PdfHelper.GetCellFixedHeight(_fNomal, fSize, lstOperationDiaryInfo.UserInfo[i].FullName, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 31));
                    if (i == 0)
                    {
                        tblODInfo.AddCell(PdfHelper.GetCell(_fNomal, fSize, lstOperationDiaryInfo.Comments, 1, countRow, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE));
                    }
                    tblODInfo.AddCell(GetCellImageSignature(lstOperationDiaryInfo.UserInfo[i].Signature, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 31));
                }
                mainTable.CompleteRow();
                PdfPCell cellMerge = new PdfPCell
                {
                    Colspan = 5,
                    Padding = 0,
                    BorderColor = BaseColor.WHITE
                };
                cellMerge.AddElement(tblODInfo);
                mainTable.AddCell(cellMerge);
            }
        }

        private PdfPCell GetCellImageSignature(string signatureContent, int colSpan, int rowSpan, int hAlignment, int vAlignment, int fixHeight = 21)
        {
            if (string.IsNullOrWhiteSpace(signatureContent))
            {
                PdfPCell cell = new PdfPCell(new Phrase(" "))
                {
                    HorizontalAlignment = hAlignment,
                    VerticalAlignment = vAlignment,
                    Rowspan = rowSpan,
                    Colspan = colSpan,
                    FixedHeight = fixHeight,
                    Border = 1,
                    BorderColor = BaseColor.GRAY
                };
                return cell;
            }

            string signatureBase64 = signatureContent.Remove(0, signatureContent.IndexOf(',') + 1);
            byte[] imgBytes = Convert.FromBase64String(signatureBase64);

            Image img = Image.GetInstance(imgBytes);
            float imgHeight = 25;
            float imgWidth = (img.Width * imgHeight) / img.Height;
            img.ScaleAbsolute(imgWidth, imgHeight);

            PdfPCell imageCell = new PdfPCell(img)
            {
                Colspan = colSpan,
                Rowspan = rowSpan,
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                BorderColor = BaseColor.GRAY
            };

            return imageCell;
        }

        /// <summary>
        /// In file PDF
        /// </summary>
        public HttpResponseMessage ExportPdfFile(PdfExportOPModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            // Lấy dữ liệu để tạo file pdf
            PdfOperationParameters pdfDataModel = CreateDataPdf(requestModel);

            // tạo thư mục lưu file pdf
            if (!Directory.Exists(Path.Combine(_rootPath, "OperationParameter", "PDF")))
            {
                Directory.CreateDirectory(Path.Combine(_rootPath, "OperationParameter", "PDF"));
            }

            string fileName = $"OperationParameter_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string outputFile = Path.Combine(_rootPath, "OperationParameter", "PDF", fileName);

            bool isCreatePDF = CreatePDFContent(pdfDataModel, outputFile);

            if (isCreatePDF)
            {
                var fileBytes = File.ReadAllBytes(outputFile);
                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Tải file
        /// </summary>
        protected HttpResponseMessage Download(HttpContent content, string fileName = "")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return response;
        }

        /// <summary>
        /// In file excel
        /// </summary>
        public HttpResponseMessage ExportOperationParametersExcel(PdfExportOPModel requestModel)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;

            try
            {
                PdfOperationParameters pdfDataModel = CreateDataPdf(requestModel);
                if (pdfDataModel?.ListCycle == null || pdfDataModel.ListCycle.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }

                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx")))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx"));
                }

                string fileName = $"Thong_so_van_hanh_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_REPORT_AM, "xlsx", fileName);

                bool isCreated = CreateOperationParametersFile(pdfDataModel, outputFile);
                if (isCreated)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    File.Delete(outputFile);
                    if (fileBytes.Length > 0)
                        return Download(new ByteArrayContent(fileBytes), fileName);
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationParametersService: ExportOperationParametersExcel", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private bool CreateOperationParametersFile(PdfOperationParameters pdfDataModel, string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            try
            {
                ISheet sheet = workbook.CreateSheet("Thông số vận hành");
                int indexRow = 0;

                // Tính số cột tối đa từ bảng data: STT(1) + Tên(1) + Đơn vị(1) + KKS(1) + Range(1) + số khung giờ
                int timelineCols = 0;
                if (pdfDataModel.ListCycle != null)
                {
                    foreach (var cycle in pdfDataModel.ListCycle)
                    {
                        if (cycle.ListOperationParameters?.Count > 0)
                        {
                            int cols = cycle.ListOperationParameters[0].ParameterValues?.Count ?? 0;
                            if (cols > timelineCols) timelineCols = cols;
                        }
                    }
                }
                // col 1 = STT, col 2 = Tên, col 3 = Đơn vị, col 4 = KKS, col 5 = Range, col 6..N = khung giờ
                int lastCol = 5 + timelineCols; // cột cuối cùng (1-based)

                // ---- HEADER (same layout as PDF, span to lastCol) ----
                if (pdfDataModel.Header != null)
                {
                    var h = pdfDataModel.Header;
                    int hStartRow = indexRow;

                    // Row 0: Logo (col 1, rowspan 4) | Title (col 2 ~ lastCol-1, rowspan 2) | Mã tài liệu label (lastCol)
                    IRow row0 = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, hStartRow, hStartRow + 3, 1, 1, row0, ""); // logo placeholder
                    CreateCellTypeStr(sheet, hStartRow, hStartRow + 1, 2, lastCol - 1, row0,
                        $"Bảng ghi thông số cương vị {h.OperationPositionName} - {h.FactoryName}");
                    SetBoldweight(sheet, indexRow, 2, true, false, 13, HorizontalAlignment.Center);
                    CreateCellTypeStr(sheet, hStartRow, hStartRow, lastCol, lastCol, row0, "Mã tài liệu");

                    // Row 1: (logo cont) | (title cont) | DocumentCode
                    indexRow++;
                    IRow row1 = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, lastCol, lastCol, row1, h.DocumentCode);

                    // Row 2: (logo cont) | Mục ISO | Ban hành lần thứ | Ngày hiệu lực | Trang số — chia đều từ col 2 đến lastCol
                    indexRow++;
                    IRow row2 = sheet.CreateRow(indexRow);
                    int span = (lastCol - 2 + 1) / 4; // chia đều 4 phần
                    int c2s = 2, c2e = 2 + span - 1;
                    int c3s = c2e + 1, c3e = c3s + span - 1;
                    int c4s = c3e + 1, c4e = c4s + span - 1;
                    int c5s = c4e + 1, c5e = lastCol;
                    CreateCellTypeStr(sheet, indexRow, indexRow, c2s, c2e, row2, "Mục ISO:");
                    CreateCellTypeStr(sheet, indexRow, indexRow, c3s, c3e, row2, "Ban hành lần thứ:");
                    CreateCellTypeStr(sheet, indexRow, indexRow, c4s, c4e, row2, "Ngày hiệu lực:");
                    CreateCellTypeStr(sheet, indexRow, indexRow, c5s, c5e, row2, "Trang số:");
                    SetCellAlignment(sheet, indexRow, c2s, HorizontalAlignment.Left);
                    SetCellAlignment(sheet, indexRow, c3s, HorizontalAlignment.Left);
                    SetCellAlignment(sheet, indexRow, c4s, HorizontalAlignment.Left);
                    SetCellAlignment(sheet, indexRow, c5s, HorizontalAlignment.Left);

                    // Row 3: (logo cont) | values
                    indexRow++;
                    IRow row3 = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, c2s, c2e, row3, h.ItemISO);
                    CreateCellTypeStr(sheet, indexRow, indexRow, c3s, c3e, row3, h.VersionRelease);
                    CreateCellTypeStr(sheet, indexRow, indexRow, c4s, c4e, row3, h.EffectiveDate);
                    CreateCellTypeStr(sheet, indexRow, indexRow, c5s, c5e, row3, h.PageNumber);
                    SetCellAlignment(sheet, indexRow, c2s, HorizontalAlignment.Left);
                    SetCellAlignment(sheet, indexRow, c3s, HorizontalAlignment.Left);
                    SetCellAlignment(sheet, indexRow, c4s, HorizontalAlignment.Left);
                    SetCellAlignment(sheet, indexRow, c5s, HorizontalAlignment.Left);

                    // Row 4: CreateDate (span toàn bộ, align right + in nghiêng)
                    indexRow++;
                    IRow row4 = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, lastCol, row4, h.CreateDate);
                    SetBoldweight(sheet, indexRow, 1, false, true, 11, HorizontalAlignment.Right);

                    SetStyleTableHeader(sheet, new CellRangeAddress(hStartRow, indexRow, 1, lastCol),
                        new XSSFColor(new byte[] { 255, 255, 255 }), BorderStyle.Thin);

                    indexRow += 2;
                }

                // ---- THÔNG SỐ VẬN HÀNH theo từng chu kỳ ----
                if (pdfDataModel.ListCycle != null)
                {
                    foreach (var cycle in pdfDataModel.ListCycle)
                    {
                        if (cycle.ListOperationParameters == null || cycle.ListOperationParameters.Count == 0)
                            continue;

                        // Lấy danh sách khung giờ từ thông số đầu tiên
                        List<string> timelineColumns = cycle.ListOperationParameters[0].ParameterValues
                            .Select(pv => pv.OpeartionTimeLineName).ToList();

                        // Header row
                        IRow headerRow = sheet.CreateRow(indexRow);
                        int colIndex = 1;
                        int stColIndex = colIndex;

                        headerRow.CreateCell(colIndex++).SetCellValue("STT");
                        CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, headerRow, "Tên thông số"); colIndex++;
                        CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, headerRow, "Đơn vị"); colIndex++;
                        CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, headerRow, "Mã KKS"); colIndex++;
                        CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, headerRow, "Range"); colIndex++;

                        foreach (var tl in timelineColumns)
                        {
                            CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, headerRow, tl);
                            colIndex++;
                        }

                        int edColIndex = colIndex - 1;
                        SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColIndex, edColIndex),
                            new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);
                        indexRow++;

                        int stRowIndex = indexRow;
                        int stt = 1;
                        foreach (var item in cycle.ListOperationParameters)
                        {
                            IRow bodyRow = sheet.CreateRow(indexRow);
                            colIndex = 1;

                            bodyRow.CreateCell(colIndex).SetCellValue(stt.ToString());
                            SetAlignmentCenter(sheet, indexRow, colIndex); colIndex++;

                            CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, bodyRow, item.ParameterName); colIndex++;
                            CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, bodyRow, item.UnitMeasure); colIndex++;
                            CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, bodyRow, item.KKSCode); colIndex++;
                            CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, bodyRow,
                                $"{item.FromRange} - {item.ToRange}"); colIndex++;

                            foreach (var tl in timelineColumns)
                            {
                                var pv = item.ParameterValues?.FirstOrDefault(p => p.OpeartionTimeLineName == tl);
                                string val = pv?.ParameterValue ?? string.Empty;
                                CreateCellTypeStr(sheet, indexRow, indexRow, colIndex, colIndex, bodyRow, val);
                                if (!string.IsNullOrWhiteSpace(pv?.ColorCode))
                                    ApplyCellColor(sheet, indexRow, colIndex, pv.ColorCode);
                                colIndex++;
                            }

                            stt++;
                            indexRow++;
                        }

                        SetStyleTableBody(sheet, new CellRangeAddress(stRowIndex, indexRow - 1, stColIndex, edColIndex),
                            new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                        indexRow++; // khoảng cách giữa các chu kỳ
                    }
                }

                // ---- NHẬT KÝ VẬN HÀNH ----
                if (pdfDataModel.ListOperationDiaryInfo != null && pdfDataModel.ListOperationDiaryInfo.Count > 0)
                {
                    indexRow++;
                    IRow diaryTitleRow = sheet.CreateRow(indexRow);
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 4, diaryTitleRow, "NHẬT KÝ VẬN HÀNH");
                    SetBoldweight(sheet, indexRow, 1, true, false, 12);
                    indexRow++;

                    IRow diaryHeaderRow = sheet.CreateRow(indexRow);
                    int stColDiary = 1;
                    CreateCellTypeStr(sheet, indexRow, indexRow, 1, 1, diaryHeaderRow, "Ca trực");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 2, 2, diaryHeaderRow, "Họ và tên VHV");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 3, 3, diaryHeaderRow, "Nhận xét thông số cập nhật trong ca");
                    CreateCellTypeStr(sheet, indexRow, indexRow, 4, 4, diaryHeaderRow, "Ký tên");
                    SetStyleTableHeader(sheet, new CellRangeAddress(indexRow, indexRow, stColDiary, 4),
                        new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);
                    indexRow++;

                    int stRowDiary = indexRow;
                    foreach (var diary in pdfDataModel.ListOperationDiaryInfo)
                    {
                        if (diary.UserInfo == null || diary.UserInfo.Count == 0) continue;
                        int startRow = indexRow;
                        for (int i = 0; i < diary.UserInfo.Count; i++)
                        {
                            IRow diaryRow = sheet.CreateRow(indexRow);
                            if (i == 0)
                            {
                                CreateCellTypeStr(sheet, startRow, startRow + diary.UserInfo.Count - 1, 1, 1, diaryRow, diary.ShiftName);
                                CreateCellTypeStr(sheet, startRow, startRow + diary.UserInfo.Count - 1, 3, 3, diaryRow, diary.Comments);
                            }
                            CreateCellTypeStr(sheet, indexRow, indexRow, 2, 2, diaryRow, diary.UserInfo[i].FullName);
                            indexRow++;
                        }
                    }

                    if (indexRow > stRowDiary)
                        SetStyleTableBody(sheet, new CellRangeAddress(stRowDiary, indexRow - 1, stColDiary, 4),
                            new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                }

                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }
                workbook.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationParametersService: CreateOperationParametersFile", ex.ToString());
                return false;
            }
        }

        private void SetBoldweight(ISheet sheet, int row, int col, bool isBold = false, bool isItalic = false, int fontSize = 11, HorizontalAlignment alignment = HorizontalAlignment.Center)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();
            var font = (XSSFFont)xssfWorkbook.CreateFont();
            cellStyle.SetFillForegroundColor(new XSSFColor(new byte[] { 255, 255, 255 }));
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.Alignment = alignment;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            font.FontName = "Times New Roman";
            font.IsBold = isBold;
            font.IsItalic = isItalic;
            font.FontHeightInPoints = fontSize;
            cellStyle.SetFont(font);
            ICell currentCell = sheet.GetRow(row).GetCell(col);
            currentCell.CellStyle = cellStyle;
        }
        private object GetPropertyValue(object obj, string propertyName)
        {
            try
            {
                var prop = obj.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    return prop.GetValue(obj, null);
                }
            }
            catch
            {
                // Property doesn't exist
            }
            return null;
        }

        private void ApplyCellColor(ISheet sheet, int row, int col, string colorCode)
        {
            try
            {
                XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
                var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

                // Parse hex color code (e.g., "#FF0000" or "FF0000")
                if (colorCode.StartsWith("#"))
                {
                    colorCode = colorCode.Substring(1);
                }

                byte[] colorBytes = new byte[3];
                for (int i = 0; i < 3; i++)
                {
                    colorBytes[i] = Convert.ToByte(colorCode.Substring(i * 2, 2), 16);
                }

                var xssfColor = new XSSFColor(colorBytes);
                cellStyle.SetFillForegroundColor(xssfColor);
                cellStyle.FillPattern = FillPattern.SolidForeground;

                IRow currentRow = sheet.GetRow(row);
                ICell currentCell = currentRow.GetCell(col);
                if (currentCell != null)
                {
                    currentCell.CellStyle = cellStyle;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "OperationParametersService: ApplyCellColor", ex.ToString());
            }
        }

        private void CreateCellTypeStr(ISheet sheet, int startRow, int endRow, int startCol, int endCol, IRow headerRow, string value)
        {
            headerRow.CreateCell(startCol).SetCellValue(value ?? "");
            MergeColumnCellsTypeStr(sheet, startRow, endRow, startCol, endCol);
        }

        private void MergeColumnCellsTypeStr(ISheet sheet, int startRow, int endRow, int startCol, int endCol)
        {
            // Ensure the start row exists
            IRow row = sheet.GetRow(startRow) ?? sheet.CreateRow(startRow);
            ICell cell = row.GetCell(startCol, MissingCellPolicy.CREATE_NULL_AS_BLANK);

            if (cell != null)
            {
                cell.SetCellType(CellType.String);
                // Only add merged region when it spans more than one cell
                bool isSingleCell = (startRow == endRow) && (startCol == endCol);
                if (!isSingleCell)
                {
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startCol, endCol));
                }
            }
        }

        private void SetStyleTableHeader(ISheet sheet, CellRangeAddress cellRange, XSSFColor xSSFColor, BorderStyle borderStyle)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;

            for (int row = cellRange.FirstRow; row <= cellRange.LastRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                if (currentRow == null) continue;
                for (int col = cellRange.FirstColumn; col <= cellRange.LastColumn; col++)
                {
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

                    // Giữ nguyên alignment hiện tại của cell
                    cellStyle.Alignment = currentCell.CellStyle?.Alignment ?? HorizontalAlignment.Center;
                    cellStyle.VerticalAlignment = currentCell.CellStyle?.VerticalAlignment ?? VerticalAlignment.Center;

                    cellStyle.SetFillForegroundColor(xSSFColor);
                    cellStyle.FillPattern = FillPattern.SolidForeground;

                    var font = (XSSFFont)xssfWorkbook.CreateFont();
                    font.IsBold = true;
                    font.FontName = "Times New Roman";
                    cellStyle.SetFont(font);

                    cellStyle.BorderTop = borderStyle;
                    cellStyle.BorderLeft = borderStyle;
                    cellStyle.BorderRight = borderStyle;
                    cellStyle.BorderBottom = borderStyle;

                    currentCell.CellStyle = cellStyle;
                }
            }
        }

        private void SetStyleTableBody(ISheet sheet, CellRangeAddress cellRange, XSSFColor xSSFColor, BorderStyle borderStyle)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

            for (int row = cellRange.FirstRow; row <= cellRange.LastRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                for (int col = cellRange.FirstColumn; col <= cellRange.LastColumn; col++)
                {
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    cellStyle.SetFillForegroundColor(xSSFColor);
                    cellStyle.FillPattern = FillPattern.SolidForeground;

                    var font = (XSSFFont)xssfWorkbook.CreateFont();
                    font.FontName = "Times New Roman";
                    cellStyle.SetFont(font);

                    cellStyle.BorderTop = borderStyle;
                    cellStyle.BorderLeft = borderStyle;
                    cellStyle.BorderRight = borderStyle;
                    cellStyle.BorderBottom = borderStyle;

                    cellStyle.Alignment = HorizontalAlignment.Left;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    currentCell.CellStyle = cellStyle;
                }
            }
        }

        private void SetAlignmentCenter(ISheet sheet, int row, int col)
        {
            SetCellAlignment(sheet, row, col, HorizontalAlignment.Center);
        }

        private void SetCellAlignment(ISheet sheet, int row, int col, HorizontalAlignment alignment)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();
            var font = (XSSFFont)xssfWorkbook.CreateFont();
            font.FontName = "Times New Roman";
            cellStyle.SetFont(font);
            cellStyle.Alignment = alignment;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            ICell currentCell = sheet.GetRow(row).GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            currentCell.CellStyle = cellStyle;
        }

        private void SetStyleFinish(ISheet sheet, int ignoreStRow, int ignoreEdRow, int ignoreStCol, int ignoreEdCol, int endRow, int endCol)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();
            for (int row = 0; row < endRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                if (currentRow == null)
                {
                    currentRow = sheet.CreateRow(row);
                }
                for (int col = 0; col < endCol; col++)
                {
                    if (row >= ignoreStRow && row <= ignoreEdRow && col >= ignoreStCol && col <= ignoreEdCol)
                    {
                        continue;
                    }
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    if (currentCell.StringCellValue == "")
                    {
                        var font = (XSSFFont)xssfWorkbook.CreateFont();
                        font.FontName = "Times New Roman";
                        cellStyle.SetFont(font);
                        cellStyle.SetFillForegroundColor(new XSSFColor(new byte[] { 255, 255, 255 }));
                        cellStyle.FillPattern = FillPattern.SolidForeground;
                        currentCell.CellStyle = cellStyle;
                    }
                }
            }
        }

        public void AddOperationParameterByTime(AddOperationParameterByTimeModel requestModel)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                _unitOfWork.DataContext.Database.CommandTimeout = 3600;
                try
                {
                    var sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                        {
                            Value = requestModel.FactoryID,
                        },
                        new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                        {
                            Value = requestModel.OperationPositionID,
                        },
                        new SqlParameter("CycleID", System.Data.SqlDbType.Int)
                        {
                            Value = requestModel.CycleID,
                        },
                        new SqlParameter("DateCreate", System.Data.SqlDbType.Date)
                        {
                            Value = requestModel.DateCreate,
                        },
                        new SqlParameter("CurrentUserID", System.Data.SqlDbType.Int)
                        {
                            Value = requestModel.CurrentUserID,
                        },
                    };

                    var result = _unitOfWork.DataContext.Database
                        .SqlQuery<Result>("TSVH_AddOperationParameterByTime @FactoryID, @OperationPositionID, @CycleID, @DateCreate, @CurrentUserID", sqlParams).ToList();
                    transaction.Commit();

                    if (result.Count > 0 && result[0].Message == "SUCCESS")
                    {
                        //ignore error when create success
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "AddOperationParameterByTime", "Tạo dữ liệu không thành công!");
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public ResponseBaseModel ValidateOperationParameterByTime(AddOperationParameterByTimeModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            // Danh sách TimeLineCycle của chu kỳ
            var timeLineCycles = _unitOfWork.ElogbookRepository<TimeLineCycle>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                                && x.Active == (int)AccessStatus.Active
                                                                                                && !x.IsDeleted).OrderBy(x => x.Hour).ToList();

            // Lấy dữ liệu của ngày hiện tại theo Nhà máy, Cương vị, Chu ky
            var totalData = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) == requestModel.DateCreate.Date
                                                                                        && x.FactoryID == requestModel.FactoryID
                                                                                        && !x.IsDeleted).Count();

            // Danh sách tất cả thông số theo Nhà máy, Cương vị, Chu kỳ
            var parameters = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && x.CycleID == requestModel.CycleID
                                                                                        && x.Status == (int)AccessStatus.Active
                                                                                        && !x.IsDeleted).Count();

            if (timeLineCycles.Count > 0 && totalData > 0)
            {
                responseBaseModel.Message = "Dữ liệu đã tồn tại";
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                return responseBaseModel;
            }

            if (parameters == 0)
            {
                responseBaseModel.Message = "Danh sách thông số bị trống";
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                return responseBaseModel;
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetTimeLineLabels(SearchOperationTimeLineModel requestModel)
        {
            // Filter data by condition
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);
            var entities = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && x.FactoryID == requestModel.FactoryID
                                                                                        && !x.IsDeleted);

            IPagedList<OperationTimeLineModel> dataGroup = entities.Select(
               e => new OperationTimeLineModel()
               {
                   GroupDate = DbFunctions.TruncateTime(e.InsertedAt),
               }).Distinct().OrderByDescending(x => x.GroupDate)
               .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            List<BaseGroupDataModel<OperationTimeLineModel>> results = new List<BaseGroupDataModel<OperationTimeLineModel>>();
            foreach (var item in dataGroup)
            {
                var groupDate = item.GroupDate.GetValueOrDefault(DateTime.MinValue).ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
                results.Add(new BaseGroupDataModel<OperationTimeLineModel>()
                {
                    GroupDate = groupDate,
                });
            }

            return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, dataGroup.TotalRecords, dataGroup.TotalPages);
        }

        public bool IsCreator(int operationTimeLineID, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetById(operationTimeLineID);
            return entity != null && entity.CreatedUserID == userID;
        }

        public ResponseBaseModel GetTimeLineDetail(SearchOperationTimeLineDetailModel requestModel)
        {
            DateTime dateGroup = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateGroup, false);
            // Filter data by condition
            var entities = _unitOfWork.ElogbookRepository<OperationTimeLine>().GetQuery(x => x.CycleID == requestModel.CycleID
                                                                                        && DbFunctions.TruncateTime(x.InsertedAt) == dateGroup
                                                                                        && x.OperationPositionID == requestModel.OperationPositionID
                                                                                        && x.FactoryID == requestModel.FactoryID
                                                                                        && !x.IsDeleted)
                .Select(e => new OperationTimeLineModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    OperationPosition = new ValueLabelModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    Cycle = new ValueLabelModel { Value = e.Cycle.ID, Label = e.Cycle.CycleName },
                    TimeLineName = e.TimeLineName,
                    TotalParametersDefault = e.TotalParametersDefault,
                    TotalParametersChecked = e.TotalParametersChecked,
                    TotalParametersSynchronized = e.TotalParametersSynchronized,
                    UpdatedAt = e.UpdatedAt,
                    InsertedAt = e.InsertedAt,
                    CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                }).OrderBy(x => x.TimeLineName).ToList();

            return HttpResponse.OkList(entities);
        }

        /// <summary>
        /// Đồng bộ dữ liệu TSVH Elogbok sang PMIS
        /// </summary>
        public void SyncParametersPmis()
        {
            try
            {
                var result = _unitOfWork.DataContext.Database
                        .SqlQuery<Result>("TSVH_SyncDataPMIS").ToList();

                if (result.Count > 0)
                {
                    if (result[0].Message == "SUCCESS")
                    {
                        //ignore error when create success
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                            "Đông bộ dữ liệu PMIS không thành công!" + Environment.NewLine + result[0].Message);
                    }
                }
                else
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(), "Đông bộ dữ liệu PMIS không thành công!");
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    "Đồng bộ dữ liệu PMIS không thành công!" + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Đồng bộ dữ liệu thông số để hiển thị cảnh báo
        /// </summary>
        public void SyncDataOperationParameterWarning()
        {
            try
            {
                var result = _unitOfWork.DataContext.Database
                        .SqlQuery<Result>("SyncDataOperationParameterWarning").ToList();

                if (result.Count > 0)
                {
                    if (result[0].Message == "SUCCESS")
                    {
                        //ignore error when create success
                    }
                    else
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                            "Đông bộ dữ liệu thông số không thành công!" + Environment.NewLine + result[0].Message);
                    }
                }
                else
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(), "Đông bộ dữ liệu thông số không thành công!");
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationParameterBusiness.ToString(),
                    "Đồng bộ dữ liệu thông số không thành công!" + Environment.NewLine + ex.ToString());
            }
        }

        public ResponseBaseModel GetOperationParameterWarning(SearchOperationParameterWarningModel requestModel)
        {
            // Mức cảnh báo
            if (requestModel.WarningLevel <= 0)
            {
                requestModel.WarningLevel = CommonConstant.ALL_KEY;
            }

            List<SqlParameter> sqlParams = new List<SqlParameter>
            {
                new SqlParameter("WarningLevel", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.WarningLevel,
                },
                new SqlParameter("PageNumber", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageNumber,
                },
                new SqlParameter("PageSize", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.PageSize,
                },
            };

            var datas = _unitOfWork.DataContext.Database
                .SqlQuery<OperationParameterWarningResult>("GetOperationParameterWarning @WarningLevel, @PageNumber, @PageSize", sqlParams.ToArray()).ToList();

            var totalRecords = 0;
            var totalPages = 0;
            List<OperationParameterWarningModel> results = new List<OperationParameterWarningModel>();
            if (datas.Count > 0)
            {
                foreach (var item in datas)
                {
                    var operationParameterInfo = new OperationParameterWarningModel()
                    {
                        ParameterID = item.ParameterID,
                        TotalWarningLevel1 = item.TotalWarningLevel1,
                        TotalWarningLevel2 = item.TotalWarningLevel2,
                        TotalWarningLevel3 = item.TotalWarningLevel3,
                        KKSCode = item.KKSCode,
                        ParameterName = item.ParameterName,
                        ParameterGroupName = item.ParameterGroupName,
                        WarningMessage = item.WarningMessage,
                        WarningLevel = item.WarningLevel,
                    };

                    results.Add(operationParameterInfo);
                }

                totalRecords = datas[0].TotalRecords;
                totalPages = totalRecords / requestModel.PageSize + (totalRecords % requestModel.PageSize > 0 ? 1 : 0);
            }

            return HttpResponse.OkList(results, requestModel.PageNumber, requestModel.PageSize, totalRecords, totalPages);
        }

        /// <summary>
        /// Lấy thông tin chi tiết cảnh báo AI của 1 thông số vận hành.
        /// Gọi SP GetOperationParameterWarningDetail — 2 result set:
        ///   RS1: thông tin thiết bị + nội dung cảnh báo
        ///   RS2: lịch sử 50 giá trị ghi nhận gần nhất
        /// </summary>
        public ResponseBaseModel GetOperationParameterWarningDetail(int parameterId)
        {
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("ParameterID", System.Data.SqlDbType.Int) { Value = parameterId }
            };

            OperationParameterWarningDetailModel detail = null;
            var historyValues = new List<OperationParameterHistoryModel>();

            var conn = _unitOfWork.DataContext.Database.Connection;
            var wasClosed = conn.State == System.Data.ConnectionState.Closed;
            if (wasClosed) conn.Open();

            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetOperationParameterWarningDetail";

                    var tx = _unitOfWork.DataContext.Database.CurrentTransaction;
                    if (tx != null) cmd.Transaction = tx.UnderlyingTransaction;

                    cmd.Parameters.AddRange(sqlParams);

                    using (var reader = cmd.ExecuteReader())
                    {
                        // RS1: thông tin thiết bị + cảnh báo
                        if (reader.Read())
                        {
                            detail = new OperationParameterWarningDetailModel
                            {
                                ParameterID = reader.GetInt32(reader.GetOrdinal("ParameterID")),
                                ParameterName = reader["ParameterName"]?.ToString(),
                                KKSCode = reader["KKSCode"]?.ToString(),
                                UnitMeasure = reader["UnitMeasure"]?.ToString(),
                                FromRange = reader["FromRange"] == DBNull.Value ? (float?)null : Convert.ToSingle(reader["FromRange"]),
                                ToRange = reader["ToRange"] == DBNull.Value ? (float?)null : Convert.ToSingle(reader["ToRange"]),
                                ParameterGroupName = reader["ParameterGroupName"]?.ToString(),
                                FactoryName = reader["FactoryName"]?.ToString(),
                                CuongVi = reader["CuongVi"]?.ToString(),
                                WarningLevel = reader.GetInt32(reader.GetOrdinal("WarningLevel")),
                                WarningMessage = reader["WarningMessage"] == DBNull.Value ? null : reader["WarningMessage"].ToString(),
                            };
                        }

                        // RS2: lịch sử giá trị
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                historyValues.Add(new OperationParameterHistoryModel
                                {
                                    ID = Convert.ToInt64(reader["ID"]),
                                    RecordDate = reader.GetDateTime(reader.GetOrdinal("RecordDate")),
                                    TimeSlot = reader["TimeSlot"] == DBNull.Value ? 0 : reader.GetInt32(reader.GetOrdinal("TimeSlot")),
                                    RangeFrom = reader["RangeFrom"] == DBNull.Value ? (float?)null : Convert.ToSingle(reader["RangeFrom"]),
                                    RangeTo = reader["RangeTo"] == DBNull.Value ? (float?)null : Convert.ToSingle(reader["RangeTo"]),
                                    CurrentValue = reader["CurrentValue"]?.ToString(),
                                    IsOutOfRange = reader.GetInt32(reader.GetOrdinal("IsOutOfRange")) == 1,
                                });
                            }
                        }
                    }
                }
            }
            finally
            {
                if (wasClosed) conn.Close();
            }

            if (detail == null)
                return HttpResponse.Error("Không tìm thấy thông số");

            detail.HistoryValues = historyValues;
            return HttpResponse.OK(detail);
        }

    }

    public class OperationParameterWarningResult
    {
        public int ParameterID { get; set; }

        public int TotalWarningLevel1 { get; set; }

        public int TotalWarningLevel2 { get; set; }

        public int TotalWarningLevel3 { get; set; }

        public string KKSCode { get; set; }

        public string ParameterName { get; set; }

        public string ParameterGroupName { get; set; }

        public string WarningMessage { get; set; }

        public int WarningLevel { get; set; }

        public int TotalRecords { get; set; }

    }
}
