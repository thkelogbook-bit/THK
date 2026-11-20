using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Ca vận hành
    /// </summary>
    public class OperationShiftModel : EntityBaseModel
    {
        /// <summary>
        /// Tên ca vận hành
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Thời gian bắt đầu ca
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian bắt đầu ca làm việc
        /// </summary>
        public double StartTimeNumber { get; set; }

        /// <summary>
        /// Thời gian kết thúc ca
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc ca làm việc
        /// </summary>
        [JsonIgnore]
        public double EndTimeNumber { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Thời gian sử dụng
        /// </summary>
        public List<UsedTimesModel> UsedTimes { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public string CreatedDate { get; set; }

        private DateTime _insertedAt;
        /// <summary>
        /// Ngày sinh
        /// </summary>
        [JsonIgnore]
        public new DateTime InsertedAt
        {
            get { return _insertedAt; }
            set
            {
                _insertedAt = value;
                if (_insertedAt != DateTime.MinValue)
                    CreatedDate = _insertedAt.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }
    }
}
