using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Cương vị
    /// </summary>
    public class OperationPositionModel : EntityBaseModel
    {
        /// <summary>
        /// Tên Cương vị
        /// </summary>
        public string OperationPositionName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Sử dụng
        /// </summary>
        public int Active { get; set; }

        private int _authenticationTypeValue;
        /// <summary>
        /// Kiểu xác thực 1/RFID 2/QR Code
        /// </summary>
        [JsonIgnore]
        public int AuthenticationTypeValue
        {
            get
            {
                return _authenticationTypeValue;
            }
            set
            {
                _authenticationTypeValue = value;
                switch (_authenticationTypeValue)
                {
                    case 1:
                        AuthenticationType = new ValueLabelModel { Value = _authenticationTypeValue, Label = "RFID" };
                        break;
                    case 2:
                        AuthenticationType = new ValueLabelModel { Value = _authenticationTypeValue, Label = "QR Code" };
                        break;
                    default:
                        AuthenticationType = new ValueLabelModel { Value = _authenticationTypeValue, Label = string.Empty };
                        break;
                }
            }
        }

        /// <summary>
        /// Kiểu xác thực 1/RFID 2/QR Code
        /// </summary>
        public ValueLabelModel AuthenticationType { get; set; }

        /// <summary>
        /// Kiểm tra tồn tại cương vị trong ca
        /// </summary>
        public bool IsHasPositionInShift { get; set; } = false;

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public new DateTime? InsertedAt { get; set; }

        /// <summary>
        /// Trưởng ca/Trưởng kíp/Vân hành viên(cương vị chung)
        /// </summary>
        public int JobTitle { get; set; }

        /// <summary>
        /// Sử dụng máy tính bảng chung/riêng
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
