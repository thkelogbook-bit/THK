using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// model chi tiết báo cáo tuân thủ hoạt động AM
    /// </summary>
    public class DetailOperationalComplianceModel : EntityBaseModel
    {
        /// <summary>
        /// Ngày có hoạt động / Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationPosition { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ValueLabelCheckedModel Device { get; set; }

        /// <summary>
        /// Lịch bảo trì
        /// </summary>
        public ValueLabelCheckedModel MaintenanceSchedule { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public ValueLabelCheckedModel OperationShift { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public AmStatusInfoModel Status { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public ValueLabelCheckedModel DeviceAbnormalStatus { get; set; }

        /// <summary>
        /// Người hoàn thành
        /// </summary>
        public ValueLabelCheckedModel CompleterUser { get; set; }

        /// <summary>
        /// Thời gian thực hiện
        /// </summary>
        public DateTime? CompleteDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Danh sách công việc thực hiện
        /// </summary>
        [JsonIgnore]
        public List<AmWorkDetailModel> WorkDetails { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// tình trạng công việc
        /// </summary>
        public string WorkStatus { get; set; }

        /// <summary>
        /// Thông tin nguyên nhân không thực hiện
        /// </summary>
        public ValueLabelCheckedModel ReasonStopInfo { get; set; }
    }
}
