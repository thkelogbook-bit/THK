using Elogbook.Core.Models.Response;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Cập nhật nhật ký vận hành
    /// </summary>
    public class EditOperationDiary
    {
        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// 1.Tình hình vận hành
        /// </summary>
        public AddEditOperationSituationModel OperationSituation { get; set; }

        /// <summary>
        /// Công tác đang thực hiện
        /// </summary>
        public AddEditActiveWorkModel ActiveWork { get; set; }

        /// <summary>
        /// 2.Bất thường sự cố
        /// </summary>
        public AddEditAbnormalIncident AbnormalIncident { get; set; }

        /// <summary>
        /// 3.Phương thức giao ca
        /// </summary>
        public AddEditDeliveryShiftMethod DeliveryShiftMethod { get; set; }

        /// <summary>
        /// 4.Tồn tại trong ca
        /// </summary>
        public AddEditExistInShiftModel ExistInShift { get; set; }

        /// <summary>
        /// 5.Nhận xet thông số
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Tab cần cập nhật thông tin
        /// </summary>
        public int TabIndex { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// edit data cho mobile
        /// </summary>
        public bool IsMobile { get; set; }

        /// <summary>
        /// Thông tin khai báo cương vị trong ca
        /// </summary>
        public List<OperationDiaryPositionShiftModel> PositionInShifts { get; set; }

        /// <summary>
        /// Tình hình lúc nhận ca
        /// </summary>
        public string ShiftReceiving { get; set; }
    }
}
