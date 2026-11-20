using Elogbook.Core.Models;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Nhật ký vận hành
    /// </summary>
    public interface IBatchETLBusiness
    {
        /// <summary>
        /// ETL
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel RunBatch();
    }
}
