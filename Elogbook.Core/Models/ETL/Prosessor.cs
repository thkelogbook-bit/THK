using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.ETL
{
    /// <summary>
    /// Reader
    /// </summary>
    public abstract class Processor<T, R>
		{
        /// <summary>
        /// Câu query 
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Read
        /// </summary>
        public abstract R Process(T t, int jobId);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="_asset"></param>
        /// <returns></returns>
        public abstract R Process(T _asset);
    }
}
