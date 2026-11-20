namespace Elogbook.Core.Constants
{
    /// <summary>
    /// MajorRepairConstant
    /// </summary>
    public struct MajorRepairConstant
    {
        /// <summary>
        /// Địa chỉ ô Nội dung công tác
        /// </summary>
        public const int WORK_CONTENT = 0;

        /// <summary>
        /// Vị trí / KKS
        /// </summary>
        public const int WORK_TICKET_LOCATION = 1;

        /// <summary>
        /// Loại Phiếu (Sử dụng để tạo Phiếu Điện hoặc TCNH)
        /// </summary>
        public const int WORK_TICKET_TYPE = 2;

        /// <summary>
        /// Tổ Máy / Khu vực
        /// </summary>
        public const int TICKET_UNIT_AREA = 3;

        /// <summary>
        /// Tạo mới đợt sửa chữa lớn
        /// </summary>
        public const string MSG_CREATE_SCL = "Tạo mới đợt công tác SCL";

        /// <summary>
        /// Tạo hàng loạt phiếu công tác
        /// </summary>
        public const string MSG_SYS_CREATE_PCT = "Tạo hàng loạt phiếu công tác";

        /// <summary>
        /// Chỉnh sửa đợt sửa chữa lớn
        /// </summary>
        public const string MSG_UPDATE_SCL = "Chỉnh sửa đợt công tác SCL";

        /// <summary>
        /// Xóa đợt sửa chữa lớn
        /// </summary>
        public const string MSG_DELETE_SCL = "Xóa đợt công tác SCL";

        /// <summary>
        /// Xóa phiếu công tác
        /// </summary>
        public const string MSG_DELETE_PCT = "Xóa phiếu công tác";

        /// <summary>
        /// Chỉnh sửa phiếu công tác
        /// </summary>
        public const string MSG_UPDATE_PCT = "Chỉnh sửa phiếu công tác";

        /// <summary>
        /// Hủy phiếu công tác
        /// </summary>
        public const string ANNULLED_DELETE_PCT = "Hủy phiếu công tác";

        /// <summary>
        /// Chờ vật tư
        /// </summary>
        public const string WAITINGMATERIALS_PCT = "Chờ vật tư";

        /// <summary>
        /// Không thực hiện
        /// </summary>
        public const string SKIPPED_PCT = "Không thực hiện";

        /// <summary>
        /// Khóa hết hạn
        /// </summary>
        public const string EXPRIREDLOCK_PCT = "Khóa hết hạn";
    }
}
