namespace Elogbook.Core.Models
{
    /// <summary>
    /// Thông tin tạo key mapping local mobile app
    /// </summary>
    public class GenerateSyncDataKeyInfo
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Thời gian trong chu kỳ
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Mã thông số
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Loại key
        /// </summary>
        public string KeyType { get; set; }
    }
}
