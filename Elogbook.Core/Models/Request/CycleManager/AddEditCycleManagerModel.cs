using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số để thêm mới một Chu kỳ
    /// </summary>
    public class AddEditCycleManagerModel
    {
        /// <summary>
        /// Mã Chu kỳ
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên Chu kỳ
        /// </summary>
        public string CycleName { get; set; }

        /// <summary>
        /// Danh sách nhà máy sử dụng Chu kỳ
        /// </summary>
        public List<int> FactoryIDs { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Khoá nhập liệu trước (phút)
        /// </summary>
        public int BlockInputPreviousTime { get; set; }

        /// <summary>
        /// khoá nhập liệu sau (phút)
        /// </summary>
        public int BlockInputNextTime { get; set; }

        /// <summary>
        /// Mốc thời gian
        /// </summary>
        public string TimeLine { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
