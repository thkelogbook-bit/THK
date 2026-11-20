using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một Phân xưởng
    /// </summary>
    public class AddEditProductionPlantModel
    {
        /// <summary>
        /// Mã Phân xưởng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên phân xưởng
        /// </summary>
        public string ProductionPlantName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
