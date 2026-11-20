using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thuộc tính của item
    /// </summary>
    public class MRValueLabelModel
    {
        /// <summary>
        /// Key của thuộc tính
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mô tả thuộc tính
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Giá trị thuộc tính
        /// </summary>
        public int Value { get; set; }
    }
}
