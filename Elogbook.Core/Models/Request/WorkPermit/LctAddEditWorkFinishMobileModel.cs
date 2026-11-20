using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin hoàn thành công tác
    /// </summary>
    public class LctAddEditWorkFinishMobileModel
    {
        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public List<LctAddWorkFinishPictureModel> WorkFinishPictures { get; set; }

        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public string ReturnWorkplaceTime { get; set; }

        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa lệnh
        /// </summary>
        public string CheckReceivedTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành lệnh công tác
        /// </summary>
        public string CheckCompletedTime { get; set; }

        /// <summary>
        /// Người đại diện đơn vị quản lý
        /// </summary>
        public string RepresentativeUnit { get; set; }

        /// <summary>
        /// Chức vụ Người đại diện đơn vị quản lý
        /// </summary>
        public string RepresentativeUnitPositionName { get; set; }

        /// <summary>
        /// Chữ ký Người lảnh đạo công việc
        /// </summary>
        public SignatureModel LeaderSignature { get; set; }
        /// <summary>
        /// Chữ ký người chỉ huy kết thúc công việc
        /// </summary>
        public LctLeadershipUserSigningModel Commander { get; set; }
        /// <summary>
        /// Chữ ký Người đóng lệnh
        /// </summary>
        public SignatureModel CloserSignature { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Id ticket hiện tại
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Mọi biển báo, cảnh báo an toàn đã gỡ hết (881)
        /// </summary>
        public bool? OperatorConfirmed1 { get; set; }

        /// <summary>
        /// Thiết bị đủ điều kiện đưa trở lại vận hành (881)
        /// </summary>
        public bool? OperatorConfirmed2 { get; set; }

        /// <summary>
        /// Khóa lệnh công tác này (881)
        /// </summary>
        public bool? OperatorConfirmed3 { get; set; }
    }
}
