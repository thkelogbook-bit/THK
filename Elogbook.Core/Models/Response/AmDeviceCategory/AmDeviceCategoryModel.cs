using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// model dữ liệu trả về
    /// </summary>
    public class AmDeviceCategoryModel : EntityBaseModel
    {
        /// <summary>
        /// Công ty
        /// </summary>
        public ComboboxDataModel Organization { get; set; }

        /// <summary>
        /// mã số phân loại thiết bị cha
        /// </summary>
        public int? OrgID { get; set; }

        /// <summary>
        /// mã số phân loại thiết bị cha
        /// </summary>
        public int? CategoryParent { get; set; }

        /// <summary>
        /// tên
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// mã
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// Trạng thái 1: Áp dụng, 2 Không áp dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Phân loại thiết bị cha
        /// </summary>
        public ComboboxDataModel DeviceCategoryParent { get; set; }
    }
}
