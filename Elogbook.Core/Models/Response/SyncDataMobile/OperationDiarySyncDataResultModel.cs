using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// OperationDiarySyncDataResultModel
    /// </summary>
    public class OperationDiarySyncDataResultModel
    {
        /// <summary>
        /// Nhật ký vận hành
        /// </summary>
        public List<MobileOperationDiaryInfo> OperationDiary { get; set; }

        /// <summary>
        /// Cương vị trong ca của Nhật ký vận hành
        /// </summary>
        public List<MobileOperationDiaryPositionShiftInfo> OperationDiaryPositionShift { get; set; }

        /// <summary>
        /// Tình hình vận hành
        /// </summary>
        public List<MobileOperationSituationInfo> OperationSituation { get; set; }

        /// <summary>
        /// Công tác đang thực hiện
        /// </summary>
        public List<MobileActiveWorkInfo> ActiveWork { get; set; }

        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        public List<MobileAbnormalIncidentInfo> AbnormalIncident { get; set; }

        /// <summary>
        /// Phương thức giao ca
        /// </summary>
        public List<MobileDeliveryShiftMethodInfo> DeliveryShiftMethod { get; set; }

        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        public List<MobileExistInShiftInfo> ExistInShift { get; set; }

        /// <summary>
        /// Giao/nhận ca
        /// </summary>
        public List<MobileDeliveryReceiveShiftInfo> DeliveryReceiveShift { get; set; }
    }
}
