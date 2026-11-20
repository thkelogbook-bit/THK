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
    /// 
    /// </summary>
    public class DeviceAbnormalDataDetailModel
    {
        /// <summary>
        /// Mã AM
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày có hoạt động / Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationPosition { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ValueLabelCheckedModel Device { get; set; }

        /// <summary>
        /// Lịch bảo trì
        /// </summary>
        public ValueLabelCheckedModel MaintenanceSchedule { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationShift { get; set; }

        /// <summary>
        /// Người hoàn thành
        /// </summary>
        public ValueLabelCheckedModel CompleterUser { get; set; }

        /// <summary>
        /// Thời gian thực hiện
        /// </summary>
        public DateTime? CompleteDate { get; set; }

        /// <summary>
        /// Danh sách công việc thực hiện
        /// </summary>
        [JsonIgnore]
        public List<AmWorkDetailModel> WorkDetails { get; set; }

        private int _statusValue;
        /// <summary>
        /// Trạng thái
        /// </summary>
        [JsonIgnore]
        public int StatusValue
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
                        Status = new ValueLabelCheckedModel { Value = 1, Label = "Đã tạo PYC" };
                        break;
                    case 2:
                        Status = new ValueLabelCheckedModel { Value = 1, Label = "Chưa tạo PYC" };
                        break;
                    default:
                        Status = new ValueLabelCheckedModel { Value = 0, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelCheckedModel Status { get; set; }

        private int _processStatusValue;
        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        [JsonIgnore]
        public int ProcessStatusValue
        {
            get
            {
                return _processStatusValue;
            }
            set
            {
                _processStatusValue = value;
                switch (_processStatusValue)
                {
                    case (int)AmEnumReportStatus.WaitForRegistPCT:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.WaitForRegistPCT, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.WaitForRegistPCT) };
                        break;
                    case (int)AmEnumReportStatus.RegisteredPCT:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.RegisteredPCT, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.RegisteredPCT) };
                        break;
                    case (int)AmEnumReportStatus.ApprovedPCT:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.ApprovedPCT, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.ApprovedPCT) };
                        break;
                    case (int)AmEnumReportStatus.OnGoingPCT:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.OnGoingPCT, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.OnGoingPCT) };
                        break;
                    case (int)AmEnumReportStatus.CompletedPCT:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.CompletedPCT, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.CompletedPCT) };
                        break;
                    case (int)AmEnumReportStatus.InProgessPYC:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.InProgessPYC, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.InProgessPYC) };
                        break;
                    case (int)AmEnumReportStatus.StopPYC:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.StopPYC, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.StopPYC) };
                        break;
                    default:
                        ProcessingStatus = new ValueLabelCheckedModel { Value = _processStatusValue, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public ValueLabelCheckedModel ProcessingStatus { get; set; }

        /// <summary>
        /// Phiếu yêu cầu
        /// </summary>
        [JsonIgnore]
        public int? AmRequestTicketID { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        [JsonIgnore]
        public int? AmDeviceAbnormalStatusID { get; set; }
    }
}
