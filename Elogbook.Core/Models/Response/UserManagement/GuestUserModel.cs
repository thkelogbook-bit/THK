using Elogbook.Core.Constants;
using Elogbook.Core.Models.Response;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin người ngoài hệ thống
    /// </summary>
    public class GuestUserModel : EntityBaseModel
    {
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
        /// Hình ảnh số CMND/CCCD
        /// </summary>
        [JsonIgnore]
        public int? NationalAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh số CMND/CCCD
        /// </summary>
        public AttachmentModel NationalAttachment { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public int? ElectricRank { get; set; }

        private string _electricRankStr;
        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRankStr
        {
            get
            {
                return _electricRankStr;
            }
            set
            {
                _electricRankStr = value;
                if (!string.IsNullOrWhiteSpace(_electricRankStr))
                {
                    var rank = _electricRankStr.Split('/');
                    if (rank != null && rank.Length > 0)
                    {
                        int.TryParse(rank[0], out int rankData);
                        ElectricRank = rankData > 5 ? 5 : rankData;
                    }
                }
            }
        }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        [JsonIgnore]
        public int? ElectricRankAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        public AttachmentModel ElectricRankAttachment { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public int? LaborRank { get; set; }

        private string _laborRankStr;
        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string LaborRankStr
        {
            get
            {
                return _laborRankStr;
            }
            set
            {
                _laborRankStr = value;
                if (!string.IsNullOrWhiteSpace(_laborRankStr))
                {
                    var rank = _laborRankStr.Split('/');
                    if (rank != null && rank.Length > 0)
                    {
                        int.TryParse(rank[0], out int rankData);
                        LaborRank = rankData > 5 ? 5 : rankData;
                    }
                }
            }
        }

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động
        /// </summary>
        [JsonIgnore]
        public int? LaborCardAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động
        /// </summary>
        public AttachmentModel LaborCardAttachment { get; set; }
    }
}
