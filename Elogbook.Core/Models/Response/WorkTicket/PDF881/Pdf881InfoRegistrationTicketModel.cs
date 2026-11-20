using Elogbook.Core.Models.Request.RegistrationTicket;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkTicket.PDF881
{
    /// <summary>
    /// 
    /// </summary>
    public class Pdf881InfoRegistrationTicketModel
    {
        /// <summary>
        /// Địa điểm/Thiết bị
        /// </summary>
        public string LocationDevice { get; set; }

        /// <summary>
        /// Loại phiếu (PCT điện) / Lý do (PCT thuỷ cơ nhiệt hoá)
        /// 1/kế hoạch  2/Ngoài kế hoạch  3/Xử lý sự cố
        /// </summary>
        public ValueLabelModel TicketType { get; set; }

        /// <summary>
        /// Loại nội dung
        /// </summary>
        public ValueLabelModel ContentType { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Nhận diện mối nguy và biện pháp an toàn
        /// </summary>
        public virtual List<IdentificationAndSafetyMeasuresModel> IdentificationAndSafetyMeasures { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// True: hằng ngày --  False: Liên tục trong thời gian đăng ký
        /// </summary>
        public bool IsWorkDaily { get; set; }

        /// <summary>
        /// Giờ bắt đầu hằng ngày
        /// </summary>
        public string StartTimeDaily { get; set; }

        /// <summary>
        /// Chọn thời gian làm việc hàng ngày
        /// </summary>
        public string EndTimeDaily { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<ContentPositionModel> ContentPositions { get; set; }

        /// <summary>
        /// Những yêu cầu khác
        /// </summary>
        public string OtherContents { get; set; }

        /// <summary>
        /// Người đại diện đăng ký
        /// </summary>
        public UserInTicketModel RegisterUser { get; set; }

        /// <summary>
        /// chức vụ
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime RegistrationDate { get; set; }
    }
}
