using Elogbook.Core.Models.Request.WorkPermit;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin chi tiết của lệnh đăng ký
    /// </summary>
    public class LctRegistrationDetailModel
    {
        /// ID lệnh công tác
        public int RegistrationTicketID { get; set; }

        /// Nhà máy
        public int FactoryID { get; set; }

        /// đơn vị nhận lệnh
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
        /// Danh sách vị trí & nhân viên công tác
        /// </summary>
        public List<LctContentPositionModel> WorkTicketLocations { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<LctStakeholdersResponseModel> Stakeholders { get; set; }
    }
}
