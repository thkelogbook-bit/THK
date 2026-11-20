using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tạo mới/cập nhật BBKSHT
    /// </summary>
    public class AddEditInspectionReporModel
    {
        /// <summary>
        /// Mã BBKSHT
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên đơn vị, phân xưởng
        /// </summary>
        [JsonIgnore]
        public string ProductionPlantName { get; set; }

        /// <summary>
        /// Mã đơn vị, phân xưởng
        /// </summary>
        [JsonIgnore]
        public int? ProductionPlantID { get; set; }

        /// <summary>
        /// Thời gian khảo sát
        /// </summary>
        public DateTime SurveyTime { get; set; }

        /// <summary>
        /// Tại
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 1. Đại diện Đơn vị công tác
        /// </summary>
        public virtual List<RelatedUserModel> UnitRepresentatives { get; set; }

        /// <summary>
        /// 2. Đại diện (các) Đơn vị QLVH
        /// </summary>
        public virtual List<RelatedUserModel> OmUnitRepresentatives { get; set; }

        /// <summary>
        /// 3. Đại diện các đơn vị có liên quan
        /// </summary>
        public virtual List<RelatedUserModel> RelatedUnitRepresentatives{ get; set; }

        /// <summary>
        /// 4. Địa điểm (hoặc thiết bị) thực hiện công việc
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// 5. Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 6. Phạm vi làm việc
        /// </summary>
        public string InspectionScope { get; set; }

        /// <summary>
        /// 7. Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime? PlanStartDate { get; set; }

        /// <summary>
        /// 7. Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime? PlanEndDate { get; set; }

        /// <summary>
        /// 8. Trách nhiệm của các đơn vị liên quan
        /// a/ Đối với (các) Đơn vị QLVH
        /// </summary>
        public string OperationUnitResponsibilities { get; set; }

        /// <summary>
        /// b/ Đối với Đơn vị công tác
        /// </summary>
        public string WorkUnitResponsibilities { get; set; }

        /// <summary>
        /// c/ Đối với đơn vị điều độ
        /// </summary>
        public string DispatchUnitResponsibilities { get; set; }

        /// <summary>
        /// d/ Những nội dung khác có liên quan đến công việc
        /// </summary>
        public string OtherWorkRelatedMatters { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public virtual List<int> FileAttachments { get; set; }
    }
}
