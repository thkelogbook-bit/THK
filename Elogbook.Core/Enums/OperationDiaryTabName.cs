namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Định nghĩa tên các tab cho phần nhật ký vận hành
    /// </summary>
    public enum OperationDiaryTabName
    {
        /// <summary>
        /// Tình hình vận hành
        /// </summary>
        Situation = 1,
        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        Incident = 2,
        /// <summary>
        /// Phương thức giao ca
        /// </summary>
        DeliveryShiftMethod = 3,
        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        ExistInShift = 4,
        /// <summary>
        /// Nhận xét thông số
        /// </summary>
        Reviewed = 5,
        /// <summary>
        /// Giao/nhận ca
        /// </summary>
        DeliveryReceiveShift = 6,
        /// <summary>
        /// Tình hình lúc nhận ca
        /// </summary>
        ShiftReceiving = 7,
        /// <summary>
        /// Công tác đang thực hiện
        /// </summary>
        ActiveWork = 8,
    }
}
