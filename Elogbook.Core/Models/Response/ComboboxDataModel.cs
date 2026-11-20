using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh sách các thuộc tính dùng combobox
    /// </summary>
    public class ComboboxDataModel
    {
        /// <summary>
        /// Key của thuộc tính
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Mô tả thuộc tính
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Loại/Mã
        /// </summary>
        public string Code { get; set; }
    }
}
