using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân viên đơn vị công tác
    /// </summary>
    public class PdfStaffInfoModel
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Thời gian vào làm việc
        /// </summary>
        public DateTime CheckInDateTime { get; set; }
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
        /// Chữ ký vào làm việc
        /// </summary>
        public string CheckInSignature { get; set; }

        /// <summary>
        /// Thời gian ra về
        /// </summary>
        public DateTime CheckOutDateTime { get; set; }

        /// <summary>
        /// Chữ ký ra về
        /// </summary>
        public string CheckOutSignature { get; set; }
    }
}
