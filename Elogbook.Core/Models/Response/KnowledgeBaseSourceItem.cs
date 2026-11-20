namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tối giản của một tài liệu — dùng cho Sources list ở UI Hỏi đáp AI
    /// </summary>
    public class KnowledgeBaseSourceItem
    {
        /// <summary>
        /// ID bản ghi tài liệu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã tài liệu (VD: DOC-001)
        /// </summary>
        public string ProcessDocumentCode { get; set; }

        /// <summary>
        /// Tiêu đề / tên quy trình
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tên file đính kèm (VD: quy-trinh-van-hanh.pdf)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Đường dẫn tải/xem file (đã qua GetPathFile)
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Source ID trong Open Notebook (dạng "source:xxx").
        /// Dùng để map reference trong câu trả lời AI với tên file.
        /// </summary>
        public string OpenNotebookSourceId { get; set; }
    }
}
