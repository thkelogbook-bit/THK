using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("TrackingSyncPmisData")]
    public partial class TrackingSyncPmisData : EntityBase
    {
        /// <summary>
        /// Table cần đồng bộ
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Xác định thời gian sync dữ liệu danh mục
        /// </summary>
        public DateTime SyncDate { get; set; } = DateTime.Now;
    }
}
