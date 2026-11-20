using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Dữ liệu cần đồng bộ của thông số vận hành
    /// </summary>
    public class OperationParametersSyncDataModel
    {
        /// <summary>Danh sách dữ liệu Thông số vận hành </summary>
        public List<OperationParametersSyncModel> OperationParameters { get; set; }

        /// <summary>Danh sách dữ liệu Khung giờ thông số vận hành </summary>
        public List<OperationTimeLineSyncModel> OperationTimeLines { get; set; }
    }
}
