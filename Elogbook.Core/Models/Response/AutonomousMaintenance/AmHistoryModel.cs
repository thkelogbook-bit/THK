using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// mô tả model lịch sử thiết bị
    /// </summary>
    public class AmHistoryModel
    {
        /// <summary>
        /// loại
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// thông tin
        /// </summary>
        public ValueLabelModel Info { get; set; }

        /// <summary>
        /// thời gian bắt đầu
        /// </summary>
        public string TimeStart { get; set; }

        /// <summary>
        /// thời gian kết thúc
        /// </summary>
        public string TimeEnd { get; set; }

        /// <summary>
        /// mô tả
        /// </summary>
        public string Description { get; set; }
    }
}
