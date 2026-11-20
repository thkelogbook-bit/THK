using Elogbook.Core.Models.Request;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Elogbook.Core.Models.Response.InspectionReport
{
    /// <summary>
    /// Thông tin chi tiết BBKSHT hiển thị trên lưới
    /// </summary>
    public class InspectionReportDetailsModel : GeneralInspectionReportModel
    {

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Đone vị
        /// </summary>
        public int? ProductionPlantID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        ///Đone vị
        /// </summary>
        public ValueLabelModel ProductionPlant { get; set; }

        /// <summary>
        /// Thời gian khảo sát
        /// </summary>
        public string SurveyTime { get; set; }

        private DateTime? _surveyDateTime;
        /// <summary>
        /// Thời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public DateTime? SurveyDateTime
        {
            get
            {
                return _surveyDateTime;
            }
            set
            {
                _surveyDateTime = value;

                if (_surveyDateTime != null && _surveyDateTime.Value != DateTime.MinValue)
                {
                    SurveyTime = _surveyDateTime.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    SurveyTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Tại
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 1. Đại diện Đơn vị công tác
        /// </summary>
        public List<RelatedUserModel> UnitRepresentatives { get; set; } = new List<RelatedUserModel>();

        /// <summary>
        /// 2. Đại diện (các) Đơn vị QLVH
        /// </summary>
        public List<RelatedUserModel> OmUnitRepresentatives { get; set; } = new List<RelatedUserModel>();

        /// <summary>
        /// 3. Đại diện các đơn vị có liên quan
        /// </summary>
        public List<RelatedUserModel> RelatedUnitRepresentatives { get; set; } = new List<RelatedUserModel>();

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

        /// <summary>
        /// Người update
        /// </summary>
        public ValueLabelModel UpdatedUser { get; set; }

        /// <summary>
        /// 1. Đại diện Đơn vị công tác ký
        /// </summary>
        public List<UserSigningModel> UnitRepresentativesSigning { get; set; } = new List<UserSigningModel>();

        /// <summary>
        /// 2. Đại diện (các) Đơn vị QLVH ký
        /// </summary>
        public List<UserSigningModel> OmUnitRepresentativesSigning { get; set; } = new List<UserSigningModel>();

        /// <summary>
        /// 3. Đại diện các bên liên quan
        /// </summary>
        public List<UserSigningModel> RelatedUnitRepresentativesSigning { get; set; } = new List<UserSigningModel>();

        /// <summary>
        /// 
        /// </summary>
        public List<AttachmentModel> Attachments { get; set; }

    }
}
