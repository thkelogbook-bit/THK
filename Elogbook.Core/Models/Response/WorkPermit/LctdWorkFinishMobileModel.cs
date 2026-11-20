using Newtonsoft.Json;
using System;


namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// 
    /// </summary>
    public class LctdWorkFinishMobileModel : EntityBaseModel
    {
        /// <summary>
        /// Id ticket hiện tại
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp (Người thi hành lệnh) đã báo cho ông (bà)
        /// </summary>
        public LctLeadershipUserModel RepresentativeUnitUser { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public LctLeadershipUserModel CommanderUser { get; set; }

        /// <summary>
        /// Người cấp lệnh
        /// </summary>
        public LctLeadershipUserModel ApproverUser { get; set; }

        /// <summary>
        /// TThời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Đơn vị công tác kết thúc, làm xong công việc lúc
        /// </summary>
        public string CompletionTime { get; set; }

        private DateTime? _completionTimeTemp;
        /// <summary>
        /// Đơn vị công tác kết thúc, làm xong công việc lúc
        /// </summary>
        [JsonIgnore]
        public DateTime? CompletionTimeTemp
        {
            get
            {
                return _completionTimeTemp;
            }
            set
            {
                _completionTimeTemp = value;

                if (_completionTimeTemp != null && _completionTimeTemp.Value != DateTime.MinValue)
                {
                    CompletionTime = _completionTimeTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    CompletionTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Đã kiểm tra hoàn thành Lệnh công tác lúc
        /// </summary>
        public string CheckCompletionTime { get; set; }

        private DateTime? _checkCompletionTimeTemp;
        /// <summary>
        /// Đã kiểm tra hoàn thành Lệnh công tác lúc
        /// </summary>
        [JsonIgnore]
        public DateTime? CheckCompletionTimeTemp
        {
            get
            {
                return _checkCompletionTimeTemp;
            }
            set
            {
                _checkCompletionTimeTemp = value;

                if (_checkCompletionTimeTemp != null && _checkCompletionTimeTemp.Value != DateTime.MinValue)
                {
                    CheckCompletionTime = _checkCompletionTimeTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    CheckCompletionTime = string.Empty;
                }
            }
        }
    }
}
