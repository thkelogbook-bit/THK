namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Interface domain service cho chức năng HistoryCall —
    /// archive dữ liệu cũ từ OperationParameters sang OperationParametersHistoryCall.
    /// </summary>
    public interface IHistoryCallService
    {
        /// <summary>
        /// Thực hiện archive dữ liệu theo batch.
        /// Gọi SP ArchiveOperationParametersHistoryCall lặp lại cho đến khi
        /// hết dữ liệu cần archive hoặc đạt giới hạn số batch.
        /// </summary>
        /// <param name="batchSize">Số bản ghi tối đa mỗi lần gọi SP</param>
        /// <param name="maxBatches">Số batch tối đa trong một lần chạy job</param>
        void ArchiveData(int batchSize, int maxBatches);
    }
}
