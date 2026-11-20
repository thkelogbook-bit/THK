using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thêm mới nhật ký vận hành
    /// </summary>
    public class AddOperationDiary
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ngày vận hành
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public int OperationCrewID { get; set; }

        /// <summary>
        /// Danh sách cương vị trong ca
        /// </summary>
        public List<AddOperationDiaryPositionShiftModel> OperationDiaryPositionShift { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
