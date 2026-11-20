using Elogbook.Core.Mapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin cho phép của lệnh công tác
    /// </summary>
    public class LctAllowedInfoModel
    {
        /// <summary>
        /// Mã
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// các thông tin cho phép
        /// </summary>
        public List<LctAllowedInfoDetailModel> AllowedInfoDetails { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public LctLeadershipUserModel Acceptor { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        [MapperIgnore]
        public int? AllowedSignatureID { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public SignatureInfo AllowedSignature { get; set; }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public string AllowedTime { get; set; }

        private DateTime _allowedTimeTemp;
        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        [JsonIgnore]
        public DateTime AllowedTimeTemp
        {
            get
            {
                return _allowedTimeTemp;
            }
            set
            {
                _allowedTimeTemp = value;

                if (_allowedTimeTemp != DateTime.MinValue)
                {
                    AllowedTime = _allowedTimeTemp.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    AllowedTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public LctLeadershipUserModel Commander { get; set; }

        /// <summary>
        /// Chữ ký Người chỉ huy
        /// </summary>
        [MapperIgnore]
        public int? CommanderSignatureID { get; set; }

        /// <summary>
        /// Chữ ký Người chỉ huy
        /// </summary>
        public SignatureInfo CommanderSignature { get; set; }


        /// <summary>
        /// Thời gian người chỉ huy ký
        /// </summary>
        public string TimeCommanderSigned { get; set; }

        private DateTime _timeCommanderSignedTemp;
        /// <summary>
        /// Thời gian người chỉ huy ký
        /// </summary>
        [JsonIgnore]
        public DateTime TimeCommanderSignedTemp
        {
            get
            {
                return _timeCommanderSignedTemp;
            }
            set
            {
                _timeCommanderSignedTemp = value;

                if (_timeCommanderSignedTemp != DateTime.MinValue)
                {
                    TimeCommanderSigned = _timeCommanderSignedTemp.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    TimeCommanderSigned = string.Empty;
                }
            }
        }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        [JsonIgnore]
        public DateTime AllowerSigned { get; set; }

        /// <summary>
        /// Thời gian người chỉ huy ký
        /// </summary>
        [JsonIgnore]
        public DateTime CommanderSigned { get; set; }
    }
}
