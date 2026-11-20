namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các bên liên quan xử lý BBKSHT
    /// </summary>
    public class RelatedUserModel
    {
        /// <summary>
        /// Khóa chính của bảng dữ liệu. Chỉ cần dùng khi update.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int AssignToUserID { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Xác định người trong hệ thống/ngoài hệ thống
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Bậc ATĐ
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }

        /// <summary>
        /// Chức vụ trong ticket
        /// 1/Đại diện Đơn vị công tác 2/Đại diện (các) Đơn vị QLVH
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// Chức vụ của user
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public ValueLabelModel ProductionPlant { get; set; }
    }
}
