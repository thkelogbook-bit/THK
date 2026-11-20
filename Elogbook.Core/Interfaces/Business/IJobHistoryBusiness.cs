using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Khai báo các phương thức dùng chung
    /// </summary>
    public interface IJobHistoryBusiness
    {
        /// <summary>
        /// Lấy danh sách lịch sử đồng bộ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchJobHistoryModel requestModel);
    }
}
