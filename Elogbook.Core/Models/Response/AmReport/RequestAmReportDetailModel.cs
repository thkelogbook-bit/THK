using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Báo cáo Yêu cầu bảo trì - Chi tiết
    /// </summary>
    public class RequestAmReportDetailModel
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
        /// Thiết bị
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
        /// Trạng thái phiếu yêu cầu
        /// </summary>
        [JsonIgnore]
        public int AmStatusID { get; set; }

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public ValueLabelCheckedModel Emergency { get; set; }

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
                    case (int)AmEnumReportStatus.UnSentPYC:
                        Status = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.UnSentPYC, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.UnSentPYC) };
                        break;
                    case (int)AmEnumReportStatus.InProgessPYC:
                        Status = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.InProgessPYC, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.InProgessPYC) };
                        break;
                    case (int)AmEnumReportStatus.StopPYC:
                        Status = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.StopPYC, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.StopPYC) };
                        break;
                    case (int)AmEnumReportStatus.ApprovePYC:
                        Status = new ValueLabelCheckedModel { Value = (int)AmEnumReportStatus.ApprovePYC, Label = EnumExtensions.GetEnumDescription(AmEnumReportStatus.ApprovePYC) };
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
        /// Phiếu đăng ký
        /// </summary>
        public ValueLabelCheckedModel RegisterTicket { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public ValueLabelCheckedModel WorkTicket { get; set; }
    }
}
