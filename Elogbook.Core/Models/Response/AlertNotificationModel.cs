using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Chi tiết Thông báo
    /// </summary>
    public class AlertNotificationModel
    {
        /// <summary>
        /// Mã Thông báo
        /// </summary>
        public int ID { get; set; }

        private int _statusValue;
        /// <summary>
        /// Trạng thái 1: Đã đọc, 2: Chưa đọc
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
                    case (int)EnumNotification.Read:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = EnumExtensions.GetEnumDescription(EnumNotification.Read) };
                        break;
                    case (int)EnumNotification.Unread:
                        Status = new ValueLabelCheckedModel { Value = _statusValue, Label = EnumExtensions.GetEnumDescription(EnumNotification.Unread) };
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
        /// Tiêu đề thông báo
        /// </summary>
        public string NoticeTitle { get; set; }

        /// <summary>
        /// Nội dung thông báo
        /// </summary>
        public string NoticeContent { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsImportant { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
