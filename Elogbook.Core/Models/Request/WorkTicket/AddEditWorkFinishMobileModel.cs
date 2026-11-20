using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin hoàn thành công tác
    /// </summary>
    public class AddEditWorkFinishMobileModel
    {
        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public List<AddWorkFinishPictureModel> WorkFinishPictures { get; set; }

        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public string ReturnWorkplaceTime { get; set; }

        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa phiếu
        /// </summary>
        public string CheckReceivedTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành phiếu công tác
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
        /// Chữ ký Người lảnh đạo công việc
        /// </summary>
        public SignatureModel CommanderSignature { get; set; }

        /// <summary>
        /// Chữ ký Người đóng phiếu
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
        /// Khóa phiếu công tác này (881)
        /// </summary>
        public bool? OperatorConfirmed3 { get; set; }

        /// <summary>
        /// Version API
        /// </summary>
        public string ApiVersion { get; set; }
    }
}
