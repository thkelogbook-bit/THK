using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkTicket
{
    /// <summary>
    /// thông tin để cập nhật tổ chức thực hiện
    /// </summary>
    public class UpdateReceiveWorkplaceModel
		{
        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// đơn vị công tác (cấp tổ)
        /// </summary>
        public int? WorkUnitID { get; set; }

        /// <summary>
        /// tên đơn vị công tác (trường hợp nhập)
        /// </summary>
        public string WorkUnitName { get; set; }

        /// <summary>
        /// số lượng người
        /// </summary>
        public string AmountPersons { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
				public int CurrentUserID { get; set; }
		}
}
