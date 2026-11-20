namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Khai báo các phương thức dùng chung
    /// </summary>
    public interface IBatchEtlService
    {
        /// <summary>
        /// Run Batch
        /// </summary>
        /// <returns></returns>
        void RunBatch();
    }
}
