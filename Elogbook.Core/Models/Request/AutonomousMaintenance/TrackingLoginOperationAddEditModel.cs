using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông số thêm mới thông tin login
    /// </summary>
    public class TrackingLoginOperationAddEditModel
    {
        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
