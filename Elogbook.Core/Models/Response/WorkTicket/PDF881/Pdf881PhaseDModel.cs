namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// 
    /// </summary>
    public class Pdf881PhaseDModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tiêu đề 1
        /// </summary>
        public string Title1 { get; set; }

        /// <summary>
        /// Tiêu đề 2
        /// </summary>
        public string Title2 { get; set; }

        /// <summary>
        /// Đơn vị 1 xác nhận
        /// </summary>
        public string WorkUnitConfirm1 { get; set; }

        /// <summary>
        /// Đơn vị 2 xác nhận
        /// </summary>
        public string WorkUnitConfirm2 { get; set; }

        /// <summary>
        /// Đơn vị 3 xác nhận
        /// </summary>
        public string WorkUnitConfirm3 { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký người chỉ huy
        /// </summary>
        public string CommanderSignature { get; set; }

        /// <summary>
        /// Thời gian xác nhận
        /// </summary>
        public string SignedTime1 { get; set; }

        /// <summary>
        /// Operator Confirm 1
        /// </summary>
        public string WorkOperatorConfirm1 { get; set; }

        /// <summary>
        /// Operator Confirm 2
        /// </summary>
        public string WorkOperatorConfirm2 { get; set; }

        /// <summary>
        /// Operator Confirm 3
        /// </summary>
        public string WorkOperatorConfirm3 { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public string AllowerSignature { get; set; }

        /// <summary>
        /// Thời gian cho phép
        /// </summary>
        public string SignedTime2 { get; set; }
    }
}
