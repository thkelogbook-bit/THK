namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin thông số
    /// </summary>
    public class OperationParameterWarningModel
    {
        /// <summary>
        /// Mã thông số
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Số lượng thông số bị cảnh báo mức 1
        /// </summary>
        public int TotalWarningLevel1 { get; set; }

        /// <summary>
        /// Số lượng thông số bị cảnh báo mức 2
        /// </summary>
        public int TotalWarningLevel2 { get; set; }

        /// <summary>
        /// Số lượng thông số bị cảnh báo mức 3
        /// </summary>
        public int TotalWarningLevel3 { get; set; }

        /// <summary>
        /// Mã thiết bị
        /// </summary>
        public string KKSCode { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Tên nhóm thông số
        /// </summary>
        public string ParameterGroupName { get; set; }

        /// <summary>
        /// Nội dung cảnh báo
        /// </summary>
        public string WarningMessage { get; set; }

        /// <summary>
        /// Mức độ cảnh báo
        /// </summary>
        public int WarningLevel { get; set; }
    }
}
