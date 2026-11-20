using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response.Manipulation
{
    /// <summary>
    /// Giao nhận, nghiệm thu đường dây, thiết bị điện trước và sau khi thao tác
    /// </summary>
    public class DeliveryAccepDeviceModel : EntityBaseModel
    {
        /// <summary>
        /// True nếu giao nhận, nghiệm thu trước. False nếu giao nhận, nghiệm thu sau
        /// </summary>
        public bool IsBeforeDelivery { get; set; }

        /// <summary>
        /// Thời gian
        /// </summary>
        public string DeliveryTime { get; set; }

        private DateTime _dtDeliveryTime;
        /// <summary>
        /// Thời gian
        /// </summary>
        [JsonIgnore]
        public DateTime DTDeliveryTime
        {
            get { return _dtDeliveryTime; }
            set
            {
                _dtDeliveryTime = value;
                if (_dtDeliveryTime != DateTime.MinValue)
                {
                    DeliveryTime = _dtDeliveryTime.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM);
                }
            }
        }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }
    }
}
