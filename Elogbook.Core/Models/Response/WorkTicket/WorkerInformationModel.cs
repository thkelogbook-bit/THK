using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân viên công tác
    /// </summary>
    public class WorkerInformationModel : EntityBaseModel
    {
        /// <summary>
        /// Tên công nhân
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Thời gian CheckIn
        /// </summary>
        public string CheckInTime
        {
            get
            {
                return (CheckInDateTime.Date != DateTime.MinValue.Date && CheckInDateTime.Date != DateTime.MaxValue.Date) ? CheckInDateTime.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        /// <summary>
        /// Thời gian CheckIn
        /// </summary>
        [JsonIgnore]
        public DateTime CheckInDateTime { get; set; }

        /// <summary>
        /// Ký CheckIn
        /// </summary>
        public string CheckInSignature { get; set; }

        /// <summary>
        /// Id chữ ký CheckIn
        /// </summary>
        public int? CheckInSignatureID { get; set; }

        /// <summary>
        /// Thời gian checkout
        /// </summary>
        public string CheckOutTime
        {
            get
            {
                return (CheckOutDateTime.Date != DateTime.MinValue.Date && CheckOutDateTime.Date != DateTime.MaxValue.Date) ? CheckOutDateTime.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        /// <summary>
        /// Thời gian checkout
        /// </summary>
        [JsonIgnore]
        public DateTime CheckOutDateTime { get; set; }

        /// <summary>
        /// Ký CheckOut
        /// </summary>
        public string CheckOutSignature { get; set; }

        /// <summary>
        /// Id chữ ký CheckOut
        /// </summary>
        public int? CheckOutSignatureID { get; set; }

        /// <summary>
        /// Ngày phân nhóm dữ liệu
        /// </summary>
        [JsonIgnore]
        public DateTime? GroupDate { get; set; }

        /// <summary>
        /// Tài khoản người dùng nội bộ/ngoài hệ thống
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Xác định người dùng nội bộ/ngoài hệ thống
        /// </summary>
        public bool IsExternal { get; set; }
    }
}
