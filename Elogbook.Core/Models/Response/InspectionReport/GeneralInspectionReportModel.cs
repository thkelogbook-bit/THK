using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin BBKSHT hiển thị trên lưới
    /// </summary>
    public class GeneralInspectionReportModel
    {
        /// <summary>
        /// Mã BBKSHT
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Số BBKSHT
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string ProductionPlantName { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Trạng thái BBKSHT
        /// </summary>
        public InspectionReportStatus StatusValue { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public string CompletedDate { get; set; }

        private DateTime? _completedDateTime;
        /// <summary>
        /// Thời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public DateTime? CompletedDateTime
        {
            get
            {
                return _completedDateTime;
            }
            set
            {
                _completedDateTime = value;

                if (_completedDateTime != null && _completedDateTime.Value != DateTime.MinValue)
                {
                    CompletedDate = _completedDateTime.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    CompletedDate = string.Empty;
                }
            }
        }
    }
}
