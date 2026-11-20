using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin báo cáo xử lý bất thường sự cố
    /// </summary>
    public class DashboardAbnormalIncidentModel
    {
        /// <summary>
        /// Mã thông tin xử lý
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày hoạt động
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// nhà máy
        /// </summary>
        public int FactoryId { get; set; }

        /// <summary>
        /// nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Id thiết bị
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// thiết bị
        /// </summary>
        public ValueLabelModel Device { get; set; }

        /// <summary>
        /// Ca
        /// </summary>
        public int ShiftID { get; set; }

        /// <summary>
        /// Ca
        /// </summary>
        public ValueLabelModel Shift { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelModel Status { get; set; }

        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public int StatusProcessingID { get; set; }

        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public ValueLabelModel StatusProcessing { get; set; }


        /// <summary>
        /// Người dùng
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public ValueLabelModel User { get; set; }


    }
}
