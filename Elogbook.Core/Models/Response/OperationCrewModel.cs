namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Kíp vận hành
    /// </summary>
    public class OperationCrewModel : EntityBaseModel
    {
        /// <summary>
        /// Tên Kíp vận hành
        /// </summary>
        public string OperationCrewName { get; set; }

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
