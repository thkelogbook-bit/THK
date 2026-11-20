using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// DataSyncModel
    /// </summary>
    public class DataSyncModel
    {
        /// <summary>
        /// DataSyncs
        /// </summary>
        public List<TrackingLoginSyncModel> DataSyncs { get; set; }
    }


    /// <summary>
    /// Thông tin đồng bộ
    /// </summary>
    public class TrackingLoginSyncModel
    {
        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
