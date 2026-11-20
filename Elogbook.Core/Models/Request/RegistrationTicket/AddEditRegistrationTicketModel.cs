using Elogbook.Core.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.RegistrationTicket
{
    /// <summary>
    /// Thông tin tạo mới/cập nhật đăng ký phiếu công tác
    /// </summary>
    public class AddEditRegistrationTicketModel
    {
        /// <summary>
        /// Mã đăng ký phiếu công tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 1/kế hoạch 2/Ngoài kế hoạch 3/Xử lý sự cố
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Số phiếu đăng ký công tác
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Đơn vị nhận phiếu
        /// </summary>
        public int ReceiveUnitID { get; set; }

        /// <summary>
        /// Căn cứ biên bản số
        /// </summary>
        public string RegulationNumber { get; set; }

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public int ProductionPlantID { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 1/ Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 2/ Địa điểm (hoặc thiết bị) tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// 3/ Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// 4/ Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 4/ Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 5-6/ Số lượng đơn vị công tác/Số lượng nhân viên đơn vị công tác
        /// </summary>
        public string UnitNumberAndUnitStaff { get; set; }

        /// <summary>
        /// 7/ Người lãnh đạo công việc trong hệ thống
        /// </summary>
        public StakeholdersModel LeaderUser { get; set; }

        /// <summary>
        /// 8/ Người giám sát an toàn điện (nếu có)
        /// </summary>
        public virtual List<StakeholdersModel> Supervisors { get; set; }

        /// <summary>
        /// 9/ Người chỉ huy trực tiếp
        /// </summary>
        public virtual List<StakeholdersModel> Commanders { get; set; }

        /// <summary>
        /// 10/ Người cảnh giới trong hệ thống (nếu có)
        /// </summary>
        public StakeholdersModel GuardUser { get; set; }

        /// <summary>
        /// 11/ Danh sách những người được cử để thực hiện công việc
        /// </summary>
        public virtual List<StakeholdersModel> Workers { get; set; }

        /// <summary>
        /// 12/ Các đơn vị QLVH khác có liên quan (nếu có)
        /// TO DO
        /// </summary>
        public virtual List<string> Units { get; set; }

        /// <summary>
        /// 13/ Những yêu cầu khác
        /// </summary>
        public virtual List<OtherRequirementsModel> OtherRequirements { get; set; }

        /// <summary>
        /// True: khi thực hiện chức năng Lưu thông tin, False: khi thực hiện chức năng gửi phiếu đăng ký
        /// </summary>
        public bool IsSave { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public virtual List<int> FileAttachments { get; set; }

        /// <summary>
        /// Địa điểm/Thiết bị
        /// </summary>
        public string LocationDevice { get; set; }

        /// <summary>
        /// lý do
        /// </summary>
        public int Reason { get; set; }

        /// <summary>
        /// Loại nội dung  1/ sửa chữa   2/ thí nghiệm   3/xây dựng
        /// </summary>
        public int ContentType { get; set; }

        /// <summary>
        /// Nhận diện mối nguy hại và biện pháp an toàn
        /// </summary>
        public List<IdentificationAndSafetyMeasuresModel> IdentificationAndSafetyMeasuresModels { get; set; }

        /// <summary>
        /// True: hằng ngày --  False: Liên tục trong thời gian đăng ký
        /// </summary>
        public bool IsWorkDaily { get; set; }

        /// <summary>
        /// Giờ bắt đầu hằng ngày
        /// </summary>
        public string StartTimeDaily { get; set; }

        /// <summary>
        /// Giờ kết thúc hằng ngày
        /// </summary>
        public string EndTimeDaily { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<ContentPositionModel> ContentPositions { get; set; }

        /// <summary>
        /// Những yêu cầu khác
        /// </summary>
        public string OtherContents { get; set; }

        /// <summary>
        /// 1/ PCT điện - 2/ PCT thuỷ cơ nhiệt hoá
        /// </summary>
        public int? RegistrationTicketType { get; set; }

        /// <summary>
        /// Hiển thị đơn vị công tác đăng ký
        /// </summary>
        public string RegistrationUnitView { get; set; }

        /// <summary>
        /// Mã phiếu công tác (Phiếu thuộc đợt sửa chữa lớn)
        /// </summary>
        public int? WorkTicketID { get; set; }

        /// <summary>
        /// Mã Sửa chửa lớn
        /// </summary>
        public int? MajorRepairID { get; set; }

        /// <summary>
        /// id phiếu yêu cầu
        /// </summary>
        public int? RequestTicketID { get; set; }

        /// <summary>
        /// Mã BBKSHT
        /// </summary>
        public int? InspectionReportID { get; set; }
    }
}
