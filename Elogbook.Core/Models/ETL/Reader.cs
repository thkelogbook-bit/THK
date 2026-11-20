using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.ETL
{
    /// <summary>
    /// Reader
    /// </summary>
    public abstract class Reader<T>
		{
        /// <summary>
        /// Câu query 
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Read
        /// </summary>
        public abstract IEnumerator<T> Read(int page, DateTime lastrun, int size);
        /// <summary>
        /// Read
        /// </summary>
        public abstract IEnumerator<T> Read(int page, int jobId, int size);
    }
}
