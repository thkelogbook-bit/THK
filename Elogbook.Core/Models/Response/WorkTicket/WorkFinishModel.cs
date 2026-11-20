using Elogbook.Core.Mapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// kết thúc công tác, trao trả nơi làm việc
    /// </summary>
    public class WorkFinishModel : EntityBaseModel
    {
        /// <summary>
        /// Danh sách hình ảnh kết thúc công tác
        /// </summary>
        public List<WorkFinishPictureModel> WorkFinishPictures { get; set; }

        /// <summary>
        /// Mọi biển báo, cảnh báo an toàn đã gỡ hết
        /// </summary>
        public bool? OperatorConfirmed1 { get; set; }

        /// <summary>
        /// Thiết bị đủ điều kiện đưa trở lại vận hành
        /// </summary>
        public bool? OperatorConfirmed2 { get; set; }

        /// <summary>
        /// Khóa phiếu công tác này
        /// </summary>
        public bool? OperatorConfirmed3 { get; set; }

        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public string ReturnWorkplaceTime { get; set; }

        private DateTime? _returnWorkplaceTimeTemp;
        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        [JsonIgnore]
        public DateTime? ReturnWorkplaceTimeTemp
        {
            get
            {
                return _returnWorkplaceTimeTemp;
            }
            set
            {
                _returnWorkplaceTimeTemp = value;

                if (_returnWorkplaceTimeTemp != null && _returnWorkplaceTimeTemp.Value != DateTime.MinValue)
                {
                    ReturnWorkplaceTime = _returnWorkplaceTimeTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    ReturnWorkplaceTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        [JsonIgnore]
        public DateTime? ReturnTime { get; set; }

        /// <summary>
        /// Đại diện đơn vị quản lý
        /// </summary>
        public LeadershipUserModel RepresentativeUnitUser { get; set; }

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
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa phiếu
        /// </summary>
        public string CheckReceivedTime { get; set; }

        private DateTime? _checkReceivedTimeTemp;
        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa phiếu
        /// </summary>
        [JsonIgnore]
        public DateTime? CheckReceivedTimeTemp
        {
            get
            {
                return _checkReceivedTimeTemp;
            }
            set
            {
                _checkReceivedTimeTemp = value;

                if (_checkReceivedTimeTemp != null && _checkReceivedTimeTemp.Value != DateTime.MinValue)
                {
                    CheckReceivedTime = _checkReceivedTimeTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    CheckReceivedTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa phiếu
        /// </summary>
        [JsonIgnore]
        public DateTime? CheckedTime { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public LeadershipUserModel AllowedUser { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        [MapperIgnore]
        public int? AllowedSignatureID { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public SignatureInfo AllowedSignature { get; set; }

        /// <summary>
        /// Người cấp phiếu
        /// </summary>
        public LeadershipUserModel ApproverUser { get; set; }

        /// <summary>
        /// Chữ ký người cấp phiếu
        /// </summary>
        [MapperIgnore]
        public int? ApproverSignatureID { get; set; }

        /// <summary>
        /// Chữ ký người cấp phiếu
        /// </summary>
        public SignatureInfo ApproverSignature { get; set; }

        /// <summary>
        /// Thời gian hoàn thành phiếu công tác
        /// </summary>
        public string CheckCompletedTime { get; set; }

        private DateTime? _checkCompletedTimeTemp;
        /// <summary>
        /// Thời gian hoàn thành phiếu công tác
        /// </summary>
        [JsonIgnore]
        public DateTime? CheckCompletedTimeTemp
        {
            get
            {
                return _checkCompletedTimeTemp;
            }
            set
            {
                _checkCompletedTimeTemp = value;

                if (_checkCompletedTimeTemp != null && _checkCompletedTimeTemp.Value != DateTime.MinValue)
                {
                    CheckCompletedTime = _checkCompletedTimeTemp.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    CheckCompletedTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// Thời gian hoàn thành phiếu công tác
        /// </summary>
        [MapperIgnore]
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// Chức vụ Người đại diện đơn vị quản lý
        /// </summary>
        public string RepresentativeUnitPositionName { get; set; }

        /// <summary>
        /// Người đại diện đơn vị quản lý
        /// </summary>
        public string RepresentativeUnit { get; set; }
    }
}
