using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin hoàn thành công tác
    /// </summary>
    public class AddWorkFinishModel
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
        /// Chức vụ người đại diện quản lý
        /// </summary>
        public string RepresentativeUnitPositionName { get; set; }

        /// <summary>
        /// Mọi biển báo, cảnh báo an toàn đã gỡ hết
        /// </summary>
        public bool? OperatorConfirmed1 { get; set; }

        /// <summary>
        /// Thiết bị đủ điều kiện đưa trở lại vận hành
        /// </summary>
        public bool? OperatorConfirmed2 { get; set; }

        /// <summary>
        /// Khóa phiếu công tác này
        /// </summary>
        public bool? OperatorConfirmed3 { get; set; }
    }
}
