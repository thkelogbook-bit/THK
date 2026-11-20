using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Nhật ký vận hành
    /// </summary>
    public class OperationDiaryModel
    {
        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int ID { get; set; }

        private DateTime _createDate;
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                _createDate = value;
                if (_createDate != DateTime.MinValue)
                    CreateDateForWeb = _createDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }

        /// <summary>
        /// Ngày phân nhóm dữ liệu
        /// </summary>
        [JsonIgnore]
        public DateTime? GroupDate { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public string CreateDateForWeb { get; set; }

        /// <summary>
        /// Trạng thái nhật ký vận hành
        /// </summary>
        public string StatusLableForWeb { get; set; }

        /// <summary>
        /// Hiển thị nhật ký vận hành
        /// </summary>
        public string Description { get; set; }

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
        /// Thứ tự sắp xếp ca vận hành
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Trạng thái nhận ca
        /// </summary>
        public bool IsReceived { get; set; }

        private bool _isDelivered;
        /// <summary>
        /// Trạng thái giao ca
        /// </summary>
        public bool IsDelivered
        {
            get { return _isDelivered; }
            set
            {
                _isDelivered = value;
                if (_isDelivered)
                    StatusLableForWeb = "Đã giao ca";
                else if (IsReceived)
                    StatusLableForWeb = "Đã nhận ca";
                else
                    StatusLableForWeb = "Chưa giao/nhận ca";
            }
        }

        /// <summary>
        /// Có thể chỉnh sửa dữ liệu nhật ký vận hành
        /// </summary>
        public bool IsCanEdit { get; set; }

        /// <summary>
        /// Có thể xóa dữ liệu nhật ký vận hành
        /// </summary>
        public bool IsCanDelete { get; set; }

        /// <summary>
        /// Cương vị trong ca
        /// </summary>
        [JsonIgnore]
        public List<OperationDiaryPositionShiftModel> OperationDiaryPositionShifts { get; set; }

        /// <summary>
        /// Cương vị ghi NKVH
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// SyncDataKey - {Ngày}_{Nhà máy}_{Cương vị}_{Ca} (Ex: 07042022_1_1_1)
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Cương vị Id
        /// </summary>
        [JsonIgnore]
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Cương vị Id
        /// </summary>
        [JsonIgnore]
        public int? FactoryID { get; set; }
    }
}
