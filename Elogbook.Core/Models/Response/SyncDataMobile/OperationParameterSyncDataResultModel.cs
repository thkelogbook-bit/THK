using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh sách thống số vận hành
    /// </summary>
    public class OperationParameterSyncDataResultModel
    {
        /// <summary>
        /// Thông số vận hành
        /// </summary>
        public List<MobileOperationParametersInfo> OperationParameters { get; set; }

        /// <summary>
        /// Các mốc thời gian ghi thông số vận hành
        /// </summary>
        public List<MobileOperationTimeLineInfo> OperationTimeLines { get; set; }

        /// <summary>
        /// Thông tin setting cho thông số
        /// </summary>
        public List<MobileAlertDetailInfo> AlertDetail { get; set; }
    }
}
