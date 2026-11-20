
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin trình tự công việc thi hành hành lệnh
    /// </summary>
    public class WorkProcessSafetyMobileModel
    {
        /// <summary>
        /// Trình tự công việc
        /// </summary>
        public string ProcedureStep { get; set; }

        /// <summary>
        /// Điều kiện an toàn
        /// </summary>
        public string SafeCondition { get; set; }

        /// <summary>
        /// TThời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string EndTime { get; set; }

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
    }
}
