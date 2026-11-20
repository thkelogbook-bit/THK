namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Interface business cho chức năng HistoryCall —
    /// điều phối quá trình archive dữ liệu cũ thông số vận hành.
    /// </summary>
    public interface IHistoryCallBusiness
    {
        /// <summary>
        /// Khởi chạy quá trình archive dữ liệu.
        /// Đọc cấu hình batchSize và maxBatches từ AppSettings rồi delegate xuống service.
        /// </summary>
        void ArchiveData();
    }
}
