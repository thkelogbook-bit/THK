using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin giao/nhận ca
    /// </summary>
    public class DeliveryReceiveShiftModel
    {
        /// <summary>
        /// Mã thông tin giao/nhận ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mà nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Xác định cương vị trong ca được khai báo khi ghi nhật ký vận hành
        /// </summary>
        public int PositionShiftID { get; set; }

        /// <summary>
        /// Trưởng ca/Trưởng kíp
        /// </summary>
        public ValueLabelModel Supervisor { get; set; }

        /// <summary>
        /// Xác định người giao ca/nhận ca, true thì người đó giao ca và ngược lại
        /// </summary>
        public bool IsDeliveryShift { get; set; }

        /// <summary>
        /// Hình ảnh chữ ký
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Chữ ký hiển tị trên mobile
        /// </summary>
        public string SignatureMobile { get; set; }

        /// <summary>
        /// Ngày giao/nhận ca
        /// </summary>
        public string DeliverDate { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [JsonIgnore]
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Xác định các trường hợp lỗi
        /// </summary>
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
