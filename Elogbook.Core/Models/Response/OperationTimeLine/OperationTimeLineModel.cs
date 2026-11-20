using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Nhóm thông số
    /// </summary>
    public class OperationTimeLineModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Chu ky
        /// </summary>
        public ValueLabelModel Cycle { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Tên khung giờ
        /// </summary>
        public int TimeLineName { get; set; }

        /// <summary>
        /// Tổng thông số mặc định
        /// </summary>
        public int TotalParametersDefault { get; set; }

        /// <summary>
        /// Tổng thông số đã ghi
        /// </summary>
        public int TotalParametersChecked { get; set; }

        /// <summary>
        /// Tổng thông số đã được đồng bộ
        /// </summary>
        public int TotalParametersSynchronized { get; set; }

        /// <summary>
        /// Ngày ghi thông số
        /// </summary>
        public new DateTime UpdatedAt { get; set; }

        private DateTime _insertedAt;

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public new DateTime InsertedAt
        {
            get { return _insertedAt; }
            set
            {
                _insertedAt = value;
                if (_insertedAt != DateTime.MinValue)
                    CreateDateForWeb = _insertedAt.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }

        /// <summary>
        /// Ngày phân nhóm dữ liệu
        /// </summary>
        [JsonIgnore]
        public DateTime? GroupDate { get; set; }

        /// <summary>
        /// Ngày tạo dữ liệu để ghi thông số
        /// </summary>
        public string CreateDateForWeb { get; set; }

        /// <summary>
        /// Key kiểm tra sync dữ liệu offline từ mobile
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }
    }
}
