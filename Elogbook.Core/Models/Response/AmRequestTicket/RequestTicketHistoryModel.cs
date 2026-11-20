using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// RequestTicketHistoryModel
    /// </summary>
    public class RequestTicketHistoryModel
    {
        /// <summary>
        /// thời gian
        /// </summary>
        public DateTime Time { get; set; } 

        /// <summary>
        /// người thực hiện
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// hành động
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// comment
        /// </summary>
        public string Comment { get; set; }
    }
}
