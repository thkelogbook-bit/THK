using System;

namespace Elogbook.Core.Models.Response.OperationDiary
{
    /// <summary>
    /// Export operation diary response model
    /// </summary>
    public class ExportOperationDiaryResponseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public ValueLabelModel OperationShift { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelModel Status { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

    }
}
