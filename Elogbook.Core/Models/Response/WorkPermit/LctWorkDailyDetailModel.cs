using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Trả về danh sách thông tin nhân viên đơn vị công tác (881)
    /// </summary>
    public class LctWorkDailyDetailModel
    {
        /// <summary>
        /// STT
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// STT
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID Vị trí
        /// </summary>
        public int WorkLocationID { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public string WorkLocationText { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public string WorkDayText { get; set; }

        private DateTime _workDayTemp;
        /// <summary>
        /// Vị trí
        /// </summary>
        [JsonIgnore]
        public DateTime WorkDayTemp
        {
            get
            {
                return _workDayTemp;
            }
            set
            {
                _workDayTemp = value;

                if (_workDayTemp != DateTime.MinValue)
                {
                    WorkDayText = _workDayTemp.ToString("dd/MM/yyyy");
                }
                else
                {
                    WorkDayText = string.Empty;
                }
            }
        }

        /// <summary>
        /// Vị trí
        /// </summary>
        public DateTime WorkDay { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public List<LctStaffInformationsModel> StaffInformations { get; set; }

        /// <summary>
        /// Ngày phân nhóm dữ liệu
        /// </summary>
        [JsonIgnore]
        public DateTime? GroupDate { get; set; }

    }
}
