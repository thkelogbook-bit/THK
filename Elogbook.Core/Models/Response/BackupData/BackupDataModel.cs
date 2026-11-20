using System;

namespace Elogbook.Core.Models.Response.BackupData
{
    /// <summary>
    /// 
    /// </summary>
    public class BackupDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID;

        /// <summary>
        /// 
        /// </summary>
        public string Name;

        /// <summary>
        /// 
        /// </summary>
        public string Status;

        /// <summary>
        /// 
        /// </summary>
        public int AttachmentId;

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }
    }
}
