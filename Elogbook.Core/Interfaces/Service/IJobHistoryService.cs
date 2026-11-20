using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Lịch sử đồng bộ
    /// </summary>
    public interface IJobHistoryService
    {
        /// <summary>
        /// Tìm kiếm 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchJobHistoryModel requestModel);

    }
}
