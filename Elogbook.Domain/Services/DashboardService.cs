using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Dashboard;
using Elogbook.Data;
using Elogbook.Data.DataModels.Entities.Dashboard;
using Elogbook.Data.DataModels.Entities.Report;
using Elogbook.Data.Migrations;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO.Ports;
using System.Linq;
using System.Windows.Media;
using static iTextSharp.text.pdf.AcroFields;

namespace Elogbook.Domain.Services
{
    public class DashboardService : IDashboardService
    {
        private static readonly Random _random = new Random();

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public DashboardService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        public List<ListDashboardModuleView> GetDashboards(int userId, bool includeInactive = false)
        {
            var results =  _unitOfWork.ElogbookRepository<DashboardModule>().GetQuery(x => !x.IsDeleted && (x.IsActive || includeInactive)).Select(d => new ListDashboardModuleView()
            {
                ID = d.ID,
                Name = d.Name,
                ModuleValue = d.ModuleValue,
                Order = d.Order,
                IsActive = d.IsActive,
            }).ToList();
            var user = _commonService.GetUserRight(userId);
            if (user != null && user.UsersRight != null)
            {
                List<string> userRights = user.UsersRight.Select(u => u.PermissionKey).ToList();
                results = results.Where(d => d.ModuleValue.Equals("TQ") || userRights.Any(r => r.EndsWith("REPORT_" + d.ModuleValue))).OrderBy(r => r.Order).ToList();
                return results;
            }
            return new List<ListDashboardModuleView>();
        }

        public DashboardViewModel GetInfo(int reportId)
        {
            DashboardViewModel dashboardView = new DashboardViewModel();

            var reportEntity = _unitOfWork.ElogbookRepository<Report>().GetQuery(x => !x.IsDeleted && x.ID == reportId).FirstOrDefault();

            if (reportEntity == null) {
                dashboardView.Title = $"Report id: {reportId} không tồn tại hoặc trạng thái chưa được kích hoạt";
                return dashboardView;
            }

            // Thống kê trình trạng giao nhận ca trong ngày
            if (reportEntity.ModuleType.Equals("NKVH") 
                && reportEntity.Name.Equals("Thống kê trình trạng giao nhận ca trong ngày")){

               return GetPositionShiftStatus();
            }

            // Thống kê trình trạng xử lý tồn tại ca
            if (reportEntity.ModuleType.Equals("NKVH")
                && reportEntity.Name.Equals("Thống kê trình trạng xử lý tồn tại ca"))
            {
                return GetPlantShiftStatusStatistics();
            }

            // Thống kê số lượng tồn tại ca các tháng trong năm
            if (reportEntity.ModuleType.Equals("NKVH")
                && reportEntity.Name.Equals("Thống kê số lượng tồn tại ca các tháng trong năm"))
            {
                return GetMonthlyShiftCount();
            }

            // Thống kê top thiết bị có thông số báo lỗi nhiều nhất
            if (reportEntity.ModuleType.Equals("TSVH")
                && reportEntity.Name.Equals("Thống kê top thiết bị có thông số báo lỗi nhiều nhất"))
            {
                return GetTop10DeviceError();
            }

            // Thống kê top thông số có giá trị vượt ngưỡng nhiều nhất
            if (reportEntity.ModuleType.Equals("TSVH")
                && reportEntity.Name.Equals("Thống kê top thông số có giá trị vượt ngưỡng nhiều nhất"))
            {
                return GetTop10ThresholdExceededParameters();
            }

            return dashboardView;
        }

        /// <summary>
        /// Thống kê trình trạng giao nhận ca trong ngày
        /// </summary>
        /// <returns></returns>
        private DashboardViewModel GetPositionShiftStatus()
        {
            DashboardViewModel dashboardView = new DashboardViewModel();

            DateTime today = DateTime.Today;
            DateTime startOfDay = today.Date;
            DateTime endOfDay = today.Date.AddDays(1).AddTicks(-1);

            // Dữ liệu Nhật ký vận hành đang sử dụng trong ngày hiện tại
            var entities = _unitOfWork.ElogbookRepository<OperationDiary>()
                .GetQuery(x => !x.IsDeleted
                                && DbFunctions.TruncateTime(x.CreatedDate) >= startOfDay
                                && DbFunctions.TruncateTime(x.CreatedDate) <= endOfDay);

            var grouped = entities
                .OrderBy(x => x.FactoryID)
                .GroupBy(x => x.FactoryID)
                .ToList();

            var resultDatas = grouped
                .Select(g =>
                {
                    int total = g.Count();
                    int totalDelivered = g.Count(x => x.IsDelivered && x.IsReceived);
                    int totalPending = g.Count(x => !(x.IsDelivered && x.IsReceived));
                    return new DashboardPositionShiftStatusModel
                    {
                        FactoryName = g.Select(x => x.Factory.FactoryName).FirstOrDefault(),
                        Total = total,
                        TotalShiftHandovers = totalDelivered,
                        TotalPendingShiftHandovers = totalPending
                    };
                }).ToList();

            List<string> labels = new List<string> { "Tổng", "Chưa giao/nhận ca", "Đã giao ca" };
            dashboardView.Title = "Thống kê trình trạng giao nhận ca trong ngày";

            if (resultDatas.Count > 0)
            {
                foreach (var item in resultDatas)
                {
                    var dashboardCategoryView = new DashboardCategoryViewModel()
                    {
                        Category = item.FactoryName,
                        Total = item.Total,
                        DataCount = new List<DashboardInfoViewModel>()
                    };

                    var colors = new List<string>();
                    string color = RandomColor(colors);
                    colors.Add(color);
                    dashboardCategoryView.DataCount.Add(
                    new DashboardInfoViewModel
                    {
                        Label = "Chưa giao/nhận ca",
                        Count = item.TotalPendingShiftHandovers,
                        Color = color,
                    });

                    color = RandomColor(colors);
                    colors.Add(color);
                    dashboardCategoryView.DataCount.Add(
                    new DashboardInfoViewModel
                    {
                        Label = "Đã giao ca",
                        Count = item.TotalShiftHandovers,
                        Color = color,
                    });

                    dashboardView.Data.Add(dashboardCategoryView);
                }
            }

            return dashboardView;
        }

        /// <summary>
        /// Thống kê trình trạng xử lý tồn tại ca
        /// </summary>
        /// <returns></returns>
        private DashboardViewModel GetPlantShiftStatusStatistics()
        {
            DashboardViewModel dashboardView = new DashboardViewModel();

            var tempDatas = _unitOfWork.ElogbookRepository<ExistInShift>()
                .GetQuery(x => !x.IsDeleted && !x.IsDeleteIssueOfShift);

            var grouped = tempDatas
                .OrderBy(x => x.FactoryID)
                .GroupBy(x => x.FactoryID)
                .ToList();

            var factoryIds = grouped
                .Where(x => x.Key.HasValue)
                .Select(x => x.Key.Value)
                .Distinct()
                .ToList();

            var factories = _unitOfWork.ElogbookRepository<Factory>()
                .GetQuery(x => !x.IsDeleted && factoryIds.Contains(x.ID))
                .ToDictionary(x => x.ID);

            var resultDatas = grouped
                .Select(g =>
                {
                    factories.TryGetValue(g.Key ?? 0, out var factory);

                    return new DashboardPlantShiftStatusModel
                    {
                        FactoryName = factory?.FactoryName ?? "",
                        TotalPendingCount = g.Count(x => x.StatusID == (int)IssueOfShiftStatus.NOT_PROCESS),
                        TotalProcessingCount = g.Count(x => x.StatusID == (int)IssueOfShiftStatus.IN_PROGRESS),
                        TotalProcessedCount = g.Count(x => x.StatusID == (int)IssueOfShiftStatus.DONE),
                    };
                }).ToList();

            List<string> labels = new List<string> { "Chưa xử lý", "Đang xử lý", "Đã xử lý" };
            dashboardView.Title = "Thống kê trình trạng xử lý tồn tại ca";

            if (resultDatas.Count > 0)
            {
                foreach (var item in resultDatas)
                {
                    var dashboardCategoryView = new DashboardCategoryViewModel()
                    {
                        Category = item.FactoryName,
                        Total = item.TotalPendingCount + item.TotalProcessingCount + item.TotalProcessedCount,
                        DataCount = new List<DashboardInfoViewModel>()
                    };

                    var colors = new List<string>();
                    string color = RandomColor(colors);
                    colors.Add(color);
                    dashboardCategoryView.DataCount.Add(
                    new DashboardInfoViewModel
                    {
                        Label = "Chưa xử lý",
                        Count = item.TotalPendingCount,
                        Color = color,
                    });

                    color = RandomColor(colors);
                    colors.Add(color);
                    dashboardCategoryView.DataCount.Add(
                    new DashboardInfoViewModel
                    {
                        Label = "Đang xử lý",
                        Count = item.TotalProcessingCount,
                        Color = color,
                    });

                    color = RandomColor(colors);
                    colors.Add(color);
                    dashboardCategoryView.DataCount.Add(
                    new DashboardInfoViewModel
                    {
                        Label = "Đã xử lý",
                        Count = item.TotalProcessedCount,
                        Color = color,
                    });

                    dashboardView.Data.Add(dashboardCategoryView);
                }
            }

            return dashboardView;
        }

        /// <summary>
        /// Thống kê số lượng tồn tại ca các tháng trong năm
        /// </summary>
        /// <returns></returns>
        private DashboardViewModel GetMonthlyShiftCount()
        {
            DashboardViewModel dashboardView = new DashboardViewModel();
            int currentYear = DateTime.Now.Year;

            var tempDatas = _unitOfWork.ElogbookRepository<ExistInShift>()
                .GetQuery(x => !x.IsDeleted && !x.IsDeleteIssueOfShift && x.InsertedAt.Year == currentYear);

            var grouped = tempDatas
                .OrderBy(x => x.FactoryID)
                .GroupBy(x => x.FactoryID)
                .ToList();

            var factoryIds = grouped
                .Where(x => x.Key.HasValue)
                .Select(x => x.Key.Value)
                .Distinct()
                .ToList();

            var factories = _unitOfWork.ElogbookRepository<Factory>()
                .GetQuery(x => !x.IsDeleted && factoryIds.Contains(x.ID))
                .ToDictionary(x => x.ID);

            dashboardView.Title = "Thống kê số lượng tồn tại ca các tháng trong năm";

            var resultDatas = grouped.Select(g =>
            {
                factories.TryGetValue(g.Key ?? 0, out var factory);
                var monthCounts = Enumerable.Range(1, 12)
                    .Select(m => new
                    {
                        Month = m,
                        Count = g.Count(x => x.InsertedAt.Month == m)
                    }).ToList();

                return new
                {
                    FactoryName = factory?.FactoryName ?? "",
                    MonthCounts = monthCounts
                };
            }).ToList();

            if (resultDatas.Count > 0)
            {
                foreach (var item in resultDatas)
                {
                    var dashboardCategoryView = new DashboardCategoryViewModel()
                    {
                        Category = item.FactoryName,
                        DataCount = new List<DashboardInfoViewModel>()
                    };

                    var colors = new List<string>();
                    int total = 0;

                    foreach (var mc in item.MonthCounts)
                    {
                        string color = RandomColor(colors);
                        colors.Add(color);
                        dashboardCategoryView.DataCount.Add(new DashboardInfoViewModel
                        {
                            Label = $"Tháng {mc.Month}",
                            Count = mc.Count,
                            Color = color
                        });
                        total += mc.Count;
                    }

                    dashboardCategoryView.Total = total;
                    dashboardView.Data.Add(dashboardCategoryView);
                }
            }

            return dashboardView;
        }

        /// <summary>
        /// Thống kê top thiết bị có thông số báo lỗi nhiều nhất
        /// </summary>
        /// <returns></returns>
        private DashboardViewModel GetTop10DeviceError()
        {
            DashboardViewModel dashboardView = new DashboardViewModel();
            int currentYear = DateTime.Now.Year;

            var operationParameters = _unitOfWork.ElogbookRepository<OperationParameters>()
                .GetQuery(x => !x.IsDeleted
                    && x.InsertedAt.Year == currentYear
                    && x.ErrorCode != null 
                    && (x.ErrorCode == (int)DeviceErrorCode.Error 
                        || x.ErrorCode == (int)DeviceErrorCode.Stop 
                        || x.ErrorCode == (int)DeviceErrorCode.Sim 
                        || x.ErrorCode == (int)DeviceErrorCode.Jump));

            // Group by KKSCode and count occurrences, exclude null/empty KKSCode
            var topDevices = operationParameters
                .Where(x => x.KKSCode != null)
                .GroupBy(x => x.KKSCode)
                .Select(g => new { KKS = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            dashboardView.Title = "Thống kê top thiết bị có thông số báo lỗi nhiều nhất";

            if (topDevices.Any())
            {
                foreach (var item in topDevices)
                {
                    var dashboardCategoryView = new DashboardCategoryViewModel()
                    {
                        Category = item.KKS,
                        Total = item.Count,
                        DataCount = new List<DashboardInfoViewModel>()
                    };

                    var colors = new List<string>();
                    dashboardCategoryView.DataCount.Add(new DashboardInfoViewModel
                    {
                        Label = "Số lần báo lỗi",
                        Count = item.Count,
                        Color = RandomColor(colors)
                    });

                    dashboardView.Data.Add(dashboardCategoryView);
                }
            }

            return dashboardView;
        }


        /// <summary>
        /// Thống kê top thông số có giá trị vượt ngưỡng nhiều nhất
        /// </summary>
        /// <returns></returns>
        private DashboardViewModel GetTop10ThresholdExceededParameters()
        {
            DashboardViewModel dashboardView = new DashboardViewModel();
            int currentYear = DateTime.Now.Year;

            var query = _unitOfWork.ElogbookRepository<OperationParameters>()
                .GetQuery(x => !x.IsDeleted
                               && x.IsChecked
                               && x.IsNumberType
                               && x.FromRange.HasValue
                               && x.ToRange.HasValue
                               && x.CurrentValue != null
                               && x.CurrentValue != ""
                               && x.InsertedAt.Year == currentYear);

            var items = query.ToList()
                .Where(x =>
                {
                    double val;
                    if (!double.TryParse(x.CurrentValue, out val)) return false;
                    var from = x.FromRange ?? double.MinValue;
                    var to = x.ToRange ?? double.MaxValue;
                    return val < from || val > to;
                })
                .ToList();

            // Group by ParameterID and count occurrences
            var topParams = items
                .GroupBy(x => x.ParameterID)
                .Select(g => new
                {
                    ParameterID = g.Key,
                    ParameterName = g.Select(s => s.ParameterName).FirstOrDefault(),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            dashboardView.Title = "Thống kê top thông số có giá trị vượt ngưỡng nhiều nhất";

            if (topParams.Any())
            {
                foreach (var item in topParams)
                {
                    var dashboardCategoryView = new DashboardCategoryViewModel()
                    {
                        Category = string.IsNullOrEmpty(item.ParameterName) ? item.ParameterID.ToString() : item.ParameterName,
                        Total = item.Count,
                        DataCount = new List<DashboardInfoViewModel>()
                    };

                    var colors = new List<string>();
                    dashboardCategoryView.DataCount.Add(new DashboardInfoViewModel
                    {
                        Label = "Số lần vượt ngưỡng",
                        Count = item.Count,
                        Color = RandomColor(colors)
                    });

                    dashboardView.Data.Add(dashboardCategoryView);
                }
            }

            return dashboardView;
        }

        private static double RandomDoubleBetween()
        {
            return _random.Next(200, 801);
        }

        private static string RandomColor(List<string> colors)
        {
            var labels = new List<string>{
                "#AEC6CF", // Light Blue
                "#FFB347", // Light Orange
                "#B39EB5", // Light Purple
                "#77DD77", // Light Green
                "#FF6961", // Light Red
                "#FDFD96", // Light Yellow
                "#CFCFC4", // Light Gray
                "#779ECB", // Sky Blue
                "#CB99C9", // Pink Lavender
                "#966FD6"  // Lilac
            };

            // Loại bỏ các màu đã dùng
            var availableColors = labels.Except(colors).ToList();
            if (!availableColors.Any())
            {
                availableColors = labels;
            }
            var random = new Random();
            return availableColors[random.Next(availableColors.Count)];
        }

        public ListDashboardModuleView Update(EditDashboardModel dashboardModel)
        {
            var dashboardModule = _unitOfWork.ElogbookRepository<DashboardModule>().GetQuery(x => !x.IsDeleted && x.ID == dashboardModel.ID).FirstOrDefault();
            if (dashboardModule != null) {
                dashboardModule.Name = dashboardModel.Name;
                dashboardModule.IsActive = dashboardModel.IsActive;
                dashboardModule.Order = dashboardModel.Order;
                _unitOfWork.ElogbookRepository<DashboardModule>().Update(dashboardModule);
                _unitOfWork.Complete();
                return new ListDashboardModuleView()
                {
                    ID = dashboardModule.ID,
                    Name = dashboardModule.Name,
                    ModuleValue = dashboardModule.ModuleValue,
                    Order = dashboardModule.Order,
                    IsActive = dashboardModule.IsActive,
                };
            }
            return null;

        }
    }
}
