namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Cương vị
    /// </summary>
    public class MobileOperationPositionInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên cương vị
        /// </summary>
        public string OperationPositionName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Phương thức xác thực
        /// </summary>
        public int AuthenticationType { get; set; }

        /// <summary>
        /// sử dụng máy tính bảng
        /// </summary>
        public bool IsUsingOnlyOneTablet { get; set; }

        /// <summary>
        /// được ghi nhật ký vận hành trên portal
        /// </summary>
        public bool AllowWriteDiaryPortal { get; set; }

        /// <summary>
        /// được ghi nhật ký vận hành trên moble
        /// </summary>
        public bool AllowWriteDiaryMobile { get; set; }

        /// <summary>
        /// được ghi thông số vận hành trên portal
        /// </summary>
        public bool AllowWriteParameterPortal { get; set; }

        /// <summary>
        /// được ghi thống số vận hành trên moble
        /// </summary>
        public bool AllowWriteParameterMobile { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public int JobTitle { get; set; }

        /// <summary>
        /// Mẫu ghi Phương thức giao ca - Nhật ký vận hành
        /// </summary>
        public string TemplateDeliveryShiftMethod { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Cho phép kiêm nhiệm nhiều vị trí trong ca vận hành
        /// </summary>
        public bool IsConcurrentlyInShift { get; set; }

        /// <summary>
        /// Mẫu tình hình lúc nhận ca cho Cương vị
        /// </summary>
        public string ShiftReceivingTemplate { get; set; }
    }
}
