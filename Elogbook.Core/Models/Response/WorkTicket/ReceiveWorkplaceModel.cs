using Elogbook.Core.Mapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// các thông tin tiếp nhận nơi làm việc
    /// </summary>
    public class ReceiveWorkplaceModel
    {
        /// <summary>
        /// Mã
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Các biện pháp
        /// </summary>
        public List<SafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Đơn vị công tác (cấp phân xưởng)
        /// </summary>
        public ValueLabelModel WorkUnitLv1 { get; set; }

        /// <summary>
        /// Đơn vị công tác (cấp tổ)
        /// </summary>
        public ValueLabelModel WorkUnitLv2 { get; set; }

        /// <summary>
        /// Tên đơn vị công tác
        /// </summary>
        public string WorkUnitName { get; set; }

        /// <summary>
        /// Số lượng người
        /// </summary>
        public string AmountPersons { get; set; }

        /// <summary>
        /// Thời gian bắt đầu tiến hành công việc
        /// </summary>
        public string StartWorkingDate { get; set; }

        private DateTime _startWorkingDateTemp;
        /// <summary>
        /// Thời gian bắt đầu tiến hành công việc
        /// </summary>
        [JsonIgnore]
        public DateTime StartWorkingDateTemp
        {
            get
            {
                return _startWorkingDateTemp;
            }
            set
            {
                _startWorkingDateTemp = value;

                if (_startWorkingDateTemp != DateTime.MinValue)
                {
                    StartWorkingDate = _startWorkingDateTemp.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    StartWorkingDate = string.Empty;
                }
            }
        }

        /// <summary>
        /// Thời gian bắt đầu tiến hành công việc
        /// </summary>
        [JsonIgnore]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public LeadershipUserModel LeaderUser { get; set; }

        /// <summary>
        /// Chữ ký người lãnh đạo
        /// </summary>
        [MapperIgnore]
        public int? LeaderSignatureID { get; set; }

        /// <summary>
        /// Chữ ký người lãnh đạo
        /// </summary>
        public SignatureInfo LeaderSignature { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public LeadershipUserModel CommanderUser { get; set; }

        /// <summary>
        /// Chữ ký người chỉ huy
        /// </summary>
        [MapperIgnore]
        public int? CommanderSignatureID { get; set; }

        /// <summary>
        /// Chữ ký người chỉ huy
        /// </summary>
        public SignatureInfo CommanderSignature { get; set; }

        /// <summary>
        /// Người giám sát
        /// </summary>
        public LeadershipUserModel SupervisorUser { get; set; }

        /// <summary>
        /// Chữ ký người giám sát
        /// </summary>
        [MapperIgnore]
        public int? SupervisorSignatureID { get; set; }

        /// <summary>
        /// Chữ ký người giám sát
        /// </summary>
        public SignatureInfo SupervisorSignature { get; set; }
    }
}
