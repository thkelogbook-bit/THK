using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới một thông tin hoàn thành công tác
    /// </summary>
    public class LctAddWorkFinishModel
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
        /// Chức vụ người đại diện quản lý
        /// </summary>
        public string RepresentativeUnitPositionName { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chức vụ người chỉ huy trực tiếp
        /// </summary>
        public string CommanderPositionName { get; set; }

        /// <summary>
        /// Mọi biển báo, cảnh báo an toàn đã gỡ hết
        /// </summary>
        public bool? OperatorConfirmed1 { get; set; }

        /// <summary>
        /// Thiết bị đủ điều kiện đưa trở lại vận hành
        /// </summary>
        public bool? OperatorConfirmed2 { get; set; }

        /// <summary>
        /// Khóa lệnh công tác này
        /// </summary>
        public bool? OperatorConfirmed3 { get; set; }
    }
}
