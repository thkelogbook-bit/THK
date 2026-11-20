using Elogbook.Core.Models.Request.RegistrationTicket;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkTicket.PDF
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfInfoRegistrationTicketModel
    {

        /// <summary>
        /// Căn cứ biên bản số
        /// </summary>
        public string RegulationNumber { get; set; }

        /// <summary>
        /// Người đại diện đăng ký
        /// </summary>
        public UserInTicketModel RegisterUser { get; set; }

        /// <summary>
        /// chức vụ
        /// </summary>
        public string Position { get; set; }


        /// <summary>
        /// số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Địa điểm hoặc thiết bị tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Số lượng đơn vị công tác/Số lượng nhân viên đơn vị công tác
        /// </summary>
        public string UnitNumberAndUnitStaff { get; set; }

        /// <summary>
        /// Người lãnh đạo công việc (nếu có)
        /// </summary>
        public StakeholdersResponseModel LeaderUser { get; set; }

        /// <summary>
        /// Người giám sát an toàn điện (nếu có)
        /// </summary>
        public virtual List<StakeholdersResponseModel> Supervisors { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp (require)
        /// </summary>
        public virtual List<StakeholdersResponseModel> Commanders { get; set; }

        /// <summary>
        /// Người cảnh giới (Nếu có)
        /// </summary>
        public StakeholdersResponseModel GuardUser { get; set; }

        /// <summary>
        /// Danh sách những người được cử để thực hiện công việc
        /// </summary>
        public List<StakeholdersResponseModel> Workers { get; set; }

        /// <summary>
        /// Các đơn vị QLVH khác có liên quan
        /// </summary>
        public virtual List<string> Units { get; set; }

        /// <summary>
        /// Những yêu cầu khác
        /// </summary>
        public virtual List<OtherRequirementsModel> OtherRequirements { get; set; }

    }
}
