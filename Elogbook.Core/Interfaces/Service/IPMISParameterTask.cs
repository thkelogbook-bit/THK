using System;
using System.Threading.Tasks;
using Elogbook.Core.Enums;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Khai báo các phương thức dùng chung
    /// </summary>
    public interface IPmisParameterTask
    {
        /// <summary>
        /// Run Batch
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="lastRun"></param>
        /// <returns></returns>
        Task Extract(int jobId, DateTime lastRun);

        /// <summary>
        /// Chạy theo loại
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="lastRun"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task Extract(int jobId, DateTime lastRun, PmisSyncType type);

        /// <summary>
        /// Run Batch Load Data
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task Load(int jobId);

        /// <summary>
        /// Run Batch Load Data
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task Load(int jobId, PmisSyncType type);

        /// <summary>
        /// Clean data
        /// </summary>
        /// <param name="jobId"></param>
        void CleanData(int jobId);

    }
}
