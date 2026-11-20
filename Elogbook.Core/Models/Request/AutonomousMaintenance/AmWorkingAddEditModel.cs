using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tạo Thực hiện AM
    /// </summary>
    public class AmWorkingAddEditModel
    {
        /// <summary>
        /// Mã bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int AmLocationID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int AmStatusID { get; set; }

        /// <summary>
        /// File hình ảnh trước khi thực hiện AM
        /// </summary>
        public string BeforeImages { get; set; }

        /// <summary>
        /// File hình ảnh sau khi thực hiện AM
        /// </summary>
        public string AfterImages { get; set; }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public DateTime CompleteDate { get; set; }

        /// <summary>
        /// Người hoành thành
        /// </summary>
        public int? CompleterUserID { get; set; }

        /// <summary>
        /// Chữ ký Người hoành thành
        /// </summary>
        public int? CompleterSignatureID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
