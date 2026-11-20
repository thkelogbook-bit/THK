using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Dữ liệu cần đồng bộ của nhật ký vận hành
    /// </summary>
    public class OperationDiarySyncDataModel
    {
        /// <summary>Danh sách dữ liệu  Bất thường sự cố </summary>
        public List<AbnormalIncidentSyncModel> AbnormalIncidentSyncs { get; set; }

        /// <summary>Danh sách dữ liệu Giao/Nhận ca  </summary>
        public List<DeliveryReceiveShiftSyncModel> DeliveryReceiveShiftSyncs { get; set; }

        /// <summary> Danh sách dữ liệu Phương thức giao ca </summary>
        public List<DeliveryShiftMethodSyncModel> DeliveryShiftMethodSyncs { get; set; }

        /// <summary>Danh sách dữ liệu  Tồn tại trong ca  </summary>
        public List<ExistInShiftSyncModel> ExistInShiftSyncs { get; set; }

        /// <summary> Danh sách dữ liệu Nhật ký vận hành </summary>
        public List<OperationDiarySyncModel> OperationDiarySyncs { get; set; }

        /// <summary> Danh sách dữ liệu Tình hình vận hành </summary>
        public List<OperationSituationSyncModel> OperationSituationSyncs { get; set; }

        /// <summary> Danh sách dữ liệu Công tác đang thực hiện </summary>
        public List<ActiveWorkSyncModel> ActiveWorkSyncs { get; set; }

        /// <summary> Danh sách dữ liệu cương vị trong ca </summary>
        public List<OperationDiaryPositionShiftSyncModel> OperationDiaryPositionShiftSyncs { get; set; }

        /// <summary>
        /// Version API
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// Mã máy
        /// </summary>
        public string DeviceID { get; set; }
    }
}
