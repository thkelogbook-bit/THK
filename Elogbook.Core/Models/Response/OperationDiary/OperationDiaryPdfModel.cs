using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin người thực hiện
    /// </summary>
    public class WorkerModel
    {
        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Tên người thực hiện
        /// </summary>
        public string FullName { get; set; }
    }

    /// <summary>
    /// Thông tin NKVH
    /// </summary>
    public class OperationDiaryPdfModel
    {
        /// <summary>
        /// Header page
        /// </summary>
        public PdfHeaderOperationDiaryModel PdfHeaderOperationDiaryModel { get; set; }

        /// <summary>
        /// Tiêu đề nhật ký vận hành
        /// </summary>
        public string TitleOperationDiary { get; set; }

        /// <summary>
        /// Nhận xét
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Danh sách tồn tại trong ca
        /// </summary>
        public List<ExistInShiftModel> ExistInShifts { get; set; }

        /// <summary>
        /// Giờ ca vận hành đang ghi nhật ký
        /// </summary>
        public string OperationShiftTime { get; set; }

        /// <summary>
        /// Tên ca vận hành đang ghi nhật ký
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Danh sách nhân viên tham gia
        /// </summary>
        public List<WorkerModel> WorkerModel { get; set; }

        /// <summary>
        /// Danh sách tình hình vận hành trong ca vận hành
        /// </summary>
        public List<OperationSituationModel> OperationSituations { get; set; }

        /// <summary>
        /// Danh sách công tác đang thực hiện trong ca vận hành
        /// </summary>
        public List<ActiveWorkModel> ActiveWorks { get; set; }

        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        public string AbnormalIncident { get; set; }

        /// <summary>
        /// Phương thức giao ca
        /// </summary>
        public string DeliveryShiftMethod { get; set; }

        /// <summary>
        /// Thông tin giao/nhận ca
        /// </summary>
        public List<DeliveryReceiveShiftModel> DeliveryReceiveShiftInfo { get; set; }


        /// <summary>
        /// Danh sách cương vị ký giao ca/nhận ca
        /// </summary>
        public List<OperationDiaryPositionShiftModel> OperationDiaryPositionShifts { get; set; }


        /// <summary>
        /// Danh sách Cương vị trong ca
        /// </summary>
        public List<List<string>> PositionInShifts { get; set; }

        /// <summary>
        /// Tình hình lúc nhận ca
        /// </summary>
        public string ShiftReceiving { get; set; }
    }
}
