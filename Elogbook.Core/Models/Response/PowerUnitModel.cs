namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tổ máy
    /// </summary>
    public class PowerUnitModel : EntityBaseModel
    {
        /// <summary>
        /// Mã Tổ máy
        /// </summary>
        public new int ID { get; set; }

        /// <summary>
        /// Tên Tổ máy
        /// </summary>
        public string PowerUnitName { get; set; }

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
