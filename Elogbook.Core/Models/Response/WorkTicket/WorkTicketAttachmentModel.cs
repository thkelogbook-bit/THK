namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin file đính kèm
    /// </summary>
    public class WorkTicketAttachmentModel : EntityBaseModel
    {
        /// <summary>
        /// Attachment
        /// </summary>
        public AttachmentModel Attachment { get; set; }

        /// <summary>
        /// Vị trí của hình ảnh (3.1, 3.2, 3.3)
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// nội dung file
        /// </summary>
        public string FileContent { get; set; }
    }
}
