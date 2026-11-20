using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm thông số
    /// </summary>
    public class SearchParametersModel : BaseListFilter
    {
        /// <summary>
        /// True: gọi từ mobile, False: gọi từ web portal
        /// </summary>
        public bool IsMobile { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public int? ParameterGroupID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Mã QRCode/ RFID
        /// </summary>
        public string AuthenticationCode { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int OperationTimeLineID { get; set; }

        /// <summary>
        /// True: khi thực hiện scan, False: khi click vào dòng trên lưới(Nhóm thông số)
        /// </summary>
        public bool IsScanQRCodeRFID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
