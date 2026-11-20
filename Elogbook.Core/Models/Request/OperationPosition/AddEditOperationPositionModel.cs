using Elogbook.Core.Models.Response;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một Cương vị
    /// </summary>
    public class AddEditOperationPositionModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên cương vị
        /// </summary>
        public string OperationPositionName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Kiểu xác thực Sử dụng 1/RFID - 2/QR Code
        /// </summary>
        public int AthenticationType { get; set; }

        /// <summary>
        /// Nếu số lượng (Quantity) >2 thì có sử dụng trên cùng máy tính bảng không?
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
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Mẫu tình hình lúc nhận ca cho Cương vị
        /// </summary>
        public string ShiftReceivingTemplate { get; set; }

    }
}
