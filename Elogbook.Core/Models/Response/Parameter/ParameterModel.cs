using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông số
    /// </summary>
    public class ParameterModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public ValueLabelModel Cycle { get; set; }

        /// <summary>
        /// Mã RFID
        /// </summary>
        public string RFIDCode { get; set; }

        /// <summary>
        /// Mã QR Code
        /// </summary>
        public string QRCode { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Mã KKS
        /// </summary>
        public string KKSCode { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public string UnitMeasure { get; set; }

        /// <summary>
        /// Setting
        /// </summary>
        public string Setting { get; set; }

        /// <summary>
        /// Setting
        /// </summary>
        public string LastValue { get; set; }

        /// <summary>
        /// Giới hạn giá trị (từ)
        /// </summary>
        public float? FromRange { get; set; }

        /// <summary>
        /// Giới hạn giá trị (đến)
        /// </summary>
        public float? ToRange { get; set; }

        /// <summary>
        /// Kiểu dữ liệu
        /// </summary>
        public bool IsNumberType { get; set; }

        /// <summary>
        /// Mã QLKT
        /// </summary>
        public string QLKTCode { get; set; }

        /// <summary>
        /// Thuộc nhóm thông số
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public ValueLabelModel ParameterGroup { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Danh sách thông tin cảnh báo
        /// </summary>
        public List<AlertDetailModel> AlertDetails { get; set; }

        /// <summary>
        /// Phương thức xác thực
        /// </summary>
        public ValueLabelModel AuthenticationType { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [JsonIgnore]
        public new DateTime InsertedAt { get; set; }
    }
}
