
using Elogbook.Core.Enums;
using System;

namespace Elogbook.Core.Models.Response.InspectionReport
{
    /// <summary>
    /// User signing model
    /// </summary>
    public class UserSigningModel
    {
        /// <summary>
        /// Khóa chính của bảng dữ liệu. Chỉ cần dùng khi update.
        /// </summary>
        public int RelatedId { get; set; }

        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Hình ảnh chữ ký
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Trạng thái BBKSHT
        /// </summary>
        public InspectionReportStatus InspectionReportStatus { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public string CreatedDateStr { get; set; }

        private DateTime? _createdDateTime;
        /// <summary>
        /// Thời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public DateTime? CreatedDateTime
        {
            get
            {
                return _createdDateTime;
            }
            set
            {
                _createdDateTime = value;

                if (_createdDateTime != null && _createdDateTime.Value != DateTime.MinValue)
                {
                    CreatedDateStr = _createdDateTime.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    CreatedDateStr = string.Empty;
                }
            }
        }
    }
}
