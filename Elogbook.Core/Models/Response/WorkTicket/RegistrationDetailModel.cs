using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết của phiếu đăng ký
    /// </summary>
    public class RegistrationDetailModel
    {
        /// ID phiếu công tác
        public int RegistrationTicketID { get; set; }

        /// Nhà máy
        public int FactoryID { get; set; }

        /// đơn vị nhận phiếu
        public int ReceiveUnitID { get; set; }

        /// đơn vị công tác cấp phân xưởng
        public ValueLabelModel WorkUnitLv1 { get; set; }

        /// đơn vị công tác cấp tổ
        public ValueLabelModel WorkUnitLv2 { get; set; }

        /// <summary>
        /// Hiển thị đơn vị công tác đăng ký
        /// </summary>
        public string RegistrationUnitView { get; set; }

        /// <summary>
        /// Danh sách vị trí và nhân viên công tác
        /// </summary>
        public List<ContentPositionModel> WorkTicketLocations { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<StakeholdersResponseModel> Stakeholders { get; set; }

        /// <summary>
        /// id phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }
    }
}
