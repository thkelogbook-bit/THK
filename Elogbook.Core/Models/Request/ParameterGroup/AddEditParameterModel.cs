using Elogbook.Core.Models.Request;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Các thuộc tính để thêm mới/cập nhật Thông số
    /// </summary>
    public class AddEditParameterModel
    {
        /// <summary>
        /// ID Thông số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Thuộc nhóm thông số hoặc thông số lẻ
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int? CycleID { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public int? ParameterGroupID { get; set; }

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
        /// Giới hạn giá trị (từ)
        /// </summary>
        public float? FromRange { get; set; }

        /// <summary>
        /// Giới hạn giá trị (đến)
        /// </summary>
        public float? ToRange { get; set; }

        /// <summary>
        /// Kiểu dữ liệu(số or chữ)
        /// </summary>
        public bool IsNumberType { get; set; }

        /// <summary>
        /// Mã QLKT
        /// </summary>
        public string QLKTCode { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Danh sách Cảnh báo
        /// </summary>
        public List<AddEditAlertDetailModel> AlertDetails { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
