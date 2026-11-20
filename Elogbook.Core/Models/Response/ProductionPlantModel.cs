namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Phân xưởng
    /// </summary>
    public class ProductionPlantModel
    {
        /// <summary>
        /// Mã Phân xưởng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên Phân xưởng
        /// </summary>
        public string ProductionPlantName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
