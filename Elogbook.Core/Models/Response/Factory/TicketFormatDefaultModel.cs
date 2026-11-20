using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Ticket format default model
    /// </summary>
    public class TicketFormatDefaultModel
    {
        /// <summary>
        /// Format phiếu công tác điện
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Phiếu thao tác format mặc định
        /// </summary>
        public List<ValueLabelModel> Formats { get; set; }
    }
}
