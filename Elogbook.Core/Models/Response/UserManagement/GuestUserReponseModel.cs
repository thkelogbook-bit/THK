using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin người ngoài hệ thống
    /// </summary>
    public class GuestUserReponseModel
    {
        /// <summary>
        /// Mã người ngoài hệ thống
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Tên đại diện cho người dùng ngoài hệ thống
        /// </summary>
        public string UserName { get; set; }

        private DateTime? _tempDateOfBirth;
        /// <summary>
        /// Ngày sinh
        /// </summary>
        [JsonIgnore]
        public DateTime? TempDateOfBirth
        {
            get { return _tempDateOfBirth; }
            set
            {
                _tempDateOfBirth = value;
                if (_tempDateOfBirth.HasValue)
                    DateOfBirth = _tempDateOfBirth.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public string NationalID { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [JsonIgnore]
        public DateTime InsertedAt { get; set; }
    }
}
