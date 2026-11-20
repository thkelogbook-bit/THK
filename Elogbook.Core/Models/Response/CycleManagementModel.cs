using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Chu kỳ
    /// </summary>
    public class CycleManagementModel : EntityBaseModel
    {
        /// <summary>
        /// Tên Chu kỳ
        /// </summary>
        public string CycleName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public List<ValueLabelModel> CycleFactories { get; set; }

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
        /// Sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public new DateTime? InsertedAt { get; set; }
    }
}
