using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Nhật ký của ca vận hành
    /// </summary>
    public class ViewOpeartionDiaryModel
    {
        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Ngày ghi tạo nhật ký vận hành
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public ValueLabelModel OperationShift { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public ValueLabelModel OperationCrew { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public ValueLabelModel LastUpdatedUser { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Danh sách tình hình vận hành trong ca vận hành
        /// </summary>
        public List<OperationSituationModel> OperationSituations { get; set; }

        /// <summary>
        /// Danh sách công tác đang thực hiện
        /// </summary>
        public List<ActiveWorkModel> ActiveWorks { get; set; }

        /// <summary>
        /// Danh sách cương vị ký giao ca/nhận ca
        /// </summary>
        public List<OperationDiaryPositionShiftModel> OperationDiaryPositionShifts { get; set; }

        /// <summary>
        /// Danh sách Cương vị trong ca
        /// </summary>
        public List<OperationDiaryPositionShiftModel> PositionInShifts { get; set; }

        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        public AbnormalIncidentModel AbnormalIncident { get; set; }

        /// <summary>
        /// Phương thức giao ca
        /// </summary>
        public DeliveryShiftMethodModel DeliveryShiftMethod { get; set; }

        /// <summary>
        /// Thông tin tồn tại trong ca
        /// </summary>
        public List<ExistInShiftModel> ExistInShiftInfo { get; set; }

        /// <summary>
        /// Nhận xét thông số
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Thông tin giao/nhận ca
        /// </summary>
        public List<DeliveryReceiveShiftModel> DeliveryReceiveShiftInfo { get; set; }

        /// <summary>
        /// Trạng thái nhận ca
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// Trạng thái nhận ca
        /// </summary>
        public bool IsDelivered { get; set; }

        /// <summary>
        /// Ngày ghi tạo nhật ký vận hành
        /// </summary>
        [JsonIgnore]
        public DateTime? TimeDeliveryCaching { get; set; }

        /// <summary>
        /// Ngày ghi tạo nhật ký vận hành
        /// </summary>
        public string TimeDelivery { get; set; }

        /// <summary>
        /// Danh sách người dùng được chọn trong nhật ký vận hành
        /// </summary>
        public List<InternalExternalUserModel> AvailableUsers { get; set; }

        /// <summary>
        /// Thông tin ca vận hành
        /// </summary>
        [JsonIgnore]
        public OperationShiftModel OperationShiftInfo { get; set; }

        /// <summary>
        /// Tình hình lúc nhận ca
        /// </summary>
        public string ShiftReceiving { get; set; }

        /// <summary>
        /// PDd file path sau khi ký số
        /// </summary>
        public int? PdfExportId { get; set; }
    }
}
