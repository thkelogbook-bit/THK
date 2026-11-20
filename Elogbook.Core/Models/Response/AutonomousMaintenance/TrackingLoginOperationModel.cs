using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin responese
    /// </summary>
    public class TrackingLoginOperationModel : EntityBaseModel
    {
        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelCheckedModel OperationPosition { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public ValueLabelCheckedModel UserLogin { get; set; }

        /// <summary>
        /// Thời gian login cuối cùng
        /// </summary>
        public DateTime LoginLastTime { get; set; }
    }
}
