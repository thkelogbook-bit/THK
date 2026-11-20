using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkTicket
{
    /// <summary>
    /// Item Update Thông tin cho phép
    /// </summary>
    public class UpdateAllowedInfoModel
		{
				/// <summary>
				/// Mã phiếu công tác
				/// </summary>
				public int WorkTicketID { get; set; }

				/// <summary>
				/// Mã phiếu công tác
				/// </summary>
				public int ID { get; set; }

				/// <summary>
				/// Người chỉ huy. đối với phiếu thủy cơ nhiệt
				/// </summary>
				public LeadershipUserSigningModel Commander { get; set; }

				/// <summary>
				/// Người chỉ huy. đối với phiếu thủy cơ nhiệt
				/// </summary>
				public LeadershipUserSigningModel Allower { get; set; }

				/// <summary>
				/// Người dùng hiện tại
				/// </summary>
				[JsonIgnore]
				public int CurrentUserID { get; set; }
		}
}
