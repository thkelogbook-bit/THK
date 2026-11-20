using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// chi tiết lịch bảo trì
    /// </summary>
    public class AmMaintenanceScheduleModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên lịch trình
        /// </summary>
        public string MaintenanceScheduleName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public ValueLabelCheckedModel SiteDesc { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ValueLabelCheckedModel AssetDesc { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationPosition { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationShift { get; set; }

        private int _statusValue;
        /// <summary>
        /// Trạng thái 1: Có thể sử dụng, 2: Không thể sử dụng 
        /// </summary>
        [JsonIgnore]
        public int Active
        {
            get
            {
                return _statusValue;
            }
            set
            {
                _statusValue = value;
                switch (_statusValue)
                {
                    case 1:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply) };
                        break;
                    case 2:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply) };
                        break;
                    default:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelCheckedModel Status { get; set; }

        /// <summary>
        /// Danh sách công việc AM được thực hiện
        /// </summary>
        public List<AmMaintenanceWorkModel> MaintenanceWorks { get; set; }

        /// <summary>
        /// Ngày bắt đầu của 1 lịch trình
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc của 1 lịch trình
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Ngày kết thúc của 1 lịch trình
        /// </summary>
        public string EndDateStr { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelCheckedModel CreateUser { get; set; }

        /// <summary>
        /// Thời gian bắt đầu run cron job
        /// </summary>
        public string CronJobFormat { get; set; }

        /// <summary>
        /// Mô tả cron job
        /// </summary>
        public string CronJobDesc { get; set; }

        /// <summary>
        /// Thời gian thông báo (số phút)
        /// </summary>
        public int NotificationsTime { get; set; }

        /// <summary>
        /// Đã phát sinh dữ liệu AM
        /// </summary>
        public bool IsUseData { get; set; } = false;
    }
}
