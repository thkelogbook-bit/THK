using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;
namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Báo cáo tiến trình xử lý yêu cầu - Chi tiết
    /// </summary>
    public class PrtReportDetailModel
    {
        /// <summary>
        /// ID Phiếu yêu cầu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// số phiếu
        /// </summary>
        public string RequestTicketNumber { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [JsonIgnore]
        public int FactoryID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Device { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelCheckedModel CreaterUser { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public ValueLabelCheckedModel Emergency { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        [JsonIgnore]
        public int? EmergencyID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [JsonIgnore]
        public int StatusValue { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelCheckedModel Status { get; set; }

        private int _processStatusValue;
        /// <summary>
        /// Trạng thái
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
        /// Id phiếu đăng ký
        /// </summary>
        [JsonIgnore]
        public int? RegisterTicketID { get; set; }

        /// <summary>
        /// Id phiếu đăng ký
        /// </summary>
        [JsonIgnore]
        public int? WorkTicketID { get; set; }

        /// <summary>
        /// Month
        /// </summary>
        [JsonIgnore]
        public int Month { get; set; }
    }
}
