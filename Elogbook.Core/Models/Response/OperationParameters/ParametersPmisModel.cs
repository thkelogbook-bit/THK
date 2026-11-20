using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Dữ liệu thông số vận hành đồng bộ sang PMIS
    /// </summary>
    public class ParametersPmisModel
    {
        /// <summary>
        /// RowIndex
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// AMETERID
        /// </summary>
        public string AMETERID { get; set; }

        /// <summary>
        /// DTIME
        /// </summary>
        public DateTime DTIME { get; set; }

        /// <summary>
        /// VAL_S
        /// </summary>
        public string VAL_S { get; set; }

        /// <summary>
        /// READMETHODID
        /// </summary>
        public string READMETHODID { get; set; }

        /// <summary>
        /// USERID
        /// </summary>
        public string USERID { get; set; }
    }
}
