namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// CommonDataComboboxList
    /// </summary>
    public class CommonDataComboboxList
    {
        /// <summary>
        /// Key của thuộc tính
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Mô tả thuộc tính
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Phương thức xác thực
        /// </summary>
        public ValueLabelModel AuthenticationType { get; set; }
    }
}
