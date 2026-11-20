using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin địa điểm làm việc công tác hằng ngày
    /// </summary>
    public class WorkPlacementModel : EntityBaseModel
    {
        /// <summary>
        /// Địa điểm công tác
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// TThời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string StartTime { get; set; }

        private DateTime? _startTimeTemp;
        /// <summary>
        /// Thời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        [JsonIgnore]
        public DateTime? StartTimeTemp
        {
            get
            {
                return _startTimeTemp;
            }
            set
            {
                _startTimeTemp = value;

                if (_startTimeTemp != null && _startTimeTemp.Value != DateTime.MinValue)
                {
                    StartTime = _startTimeTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    StartTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Thời gian kết thúc xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string EndTime { get; set; }

        private DateTime? _endTimeTemp;
        /// <summary>
        /// Thời gian kết thúc xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        [JsonIgnore]
        public DateTime? EndTimeTemp
        {
            get
            {
                return _endTimeTemp;
            }
            set
            {
                _endTimeTemp = value;

                if (_endTimeTemp != null && _endTimeTemp.Value != DateTime.MinValue)
                {
                    EndTime = _endTimeTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    EndTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Mã số của Người chỉ huy trực tiếp
        /// </summary>
        public int? CommanderUserID { get; set; }

        /// <summary>
        /// Tên đăng nhập của người chỉ huy trực tiếp
        /// </summary>
        public string CommanderUserName { get; set; }

        /// <summary>
        /// Chức vụ của người chỉ huy trực tiếp
        /// </summary>
        public string CommanderPositionName { get; set; }

        /// <summary>
        /// ID chữ ký xác nhận bắt đầu làm việc của người chỉ huy trực tiếp
        /// </summary>
        public int? CommanderSignatureID { get; set; }

        /// <summary>
        /// Thông tin chữ ký xác nhận bắt đầu làm việc của người chỉ huy trực tiếp
        /// </summary>
        public SignatureInfo CommanderSignature { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Mã số của Người cho phép
        /// </summary>
        public int? AllowerUserID { get; set; }

        /// <summary>
        /// Tên đăng nhập của người cho phép
        /// </summary>
        public string AllowerUserName { get; set; }

        /// <summary>
        /// Chức vụ của người cho phép
        /// </summary>
        public string AllowerPositionName { get; set; }

        /// <summary>
        /// Thông tin chữ ký xác nhận bắt đầu làm việc của người cho phép
        /// </summary>
        public SignatureInfo AllowerSignature { get; set; }

        /// <summary>
        /// ID chữ ký xác nhận kết thúc làm việc của người cho phép
        /// </summary>
        public int? AllowerSignatureID { get; set; }

        /// <summary>
        /// Ngày phân nhóm dữ liệu
        /// </summary>
        [JsonIgnore]
        public DateTime? GroupDate { get; set; }
    }
}
