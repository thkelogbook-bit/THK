using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.ETL
{
    /// <summary>
    /// Reader
    /// </summary>
    public abstract class Writer<T>
		{
        /// <summary>
        /// Câu query 
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Read
        /// </summary>
        public abstract void Write(List<T> parameter);
    }
}
