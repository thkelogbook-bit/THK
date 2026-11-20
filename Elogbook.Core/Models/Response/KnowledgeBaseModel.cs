using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tài liệu
    /// </summary>
    public class KnowledgeBaseModel : EntityBaseModel
    {
        /// <summary>
        /// Chủ đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Mô tả vắn tắt nội dung tài liệu
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Mã tài liệu
        /// </summary>
        public string ProcessDocumentCode { get; set; }

        /// <summary>
        /// Tài liệu đính kèm
        /// </summary>
        [JsonIgnore]
        public int? AttachmentID { get; set; }

        /// <summary>
        /// Tài liệu
        /// </summary>
        public virtual AttachmentModel FileAttachment { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        private DateTime _createDate;
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                _createDate = value;
                if (_createDate != DateTime.MinValue)
                    CreateDateForWeb = _createDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public string CreateDateForWeb { get; set; }

        /// <summary>
        /// Phiên bản tài liệu
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Giá trị module xác định tài liệu thuộc module nào
        /// </summary>
        public string ModuleValue { get; set; }
    }
}
