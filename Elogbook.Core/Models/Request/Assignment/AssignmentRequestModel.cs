using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số nhận từ UI
    /// </summary>
    public class AssignmentRequestModel
    {
        /// <summary>
        /// ID nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// ID Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Danh sách Ca vận hành
        /// </summary>
        public List<AssignOperationShiftsRequestModel> OperationShifts { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }

    /// <summary>
    /// Thông tin Ca vận hành
    /// </summary>
    public class AssignOperationShiftsRequestModel
    {
        /// <summary>
        /// ID Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Danh sách thông tin chi tiết phân công
        /// </summary>
        public List<AssignmentDetailsRequestModel> AssignmentDetails { get; set; }
    }

    /// <summary>
    /// Thông tin chi tiết phân công
    /// </summary>
    public class AssignmentDetailsRequestModel
    {
        /// <summary>
        /// Ngày làm việc
        /// </summary>
        public string WorkDate { get; set; }

        /// <summary>
        /// Danh sách người dùng
        /// </summary>
        public List<AssignUserRequestModel> AssignedUsers { get; set; }
    }

    /// <summary>
    /// Thông tin người dùng
    /// </summary>
    public class AssignUserRequestModel
    {
        /// <summary>
        /// ID người dùng
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Phân loại người dùng : 1 - Nội bộ, 2 - Bên ngoài
        /// </summary>
        public bool IsInternal { get; set; }
    }

    /// <summary>
    /// Tham số lọc bảng phân công (View 1 - Quản lý phân công)
    /// </summary>
    public class GetAssignmentScheduleRequestModel
    {
        /// <summary>
        /// ID nhà máy (bắt buộc)
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Từ ngày (bắt buộc, định dạng dd/MM/yyyy)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày (bắt buộc, định dạng dd/MM/yyyy)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// ID cương vị (tùy chọn, 0 = lấy tất cả)
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// ID ca vận hành (tùy chọn, 0 = lấy tất cả)
        /// </summary>
        public int OperationShiftID { get; set; }
    }

    /// <summary>
    /// Tham số kiểm tra trùng lặp dữ liệu phân công
    /// </summary>
    public class CheckAssignmentDuplicateRequestModel
    {
        /// <summary>
        /// ID nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// ID cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// ID ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Ngày làm việc
        /// </summary>
        public string WorkDate { get; set; }
    }

    /// <summary>
    /// Tham số nhận từ UI
    /// </summary>
    public class CheckRangeAssignmentRequestModel
    {
        /// <summary>
        /// ID nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// ID Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Từ ngày (bắt buộc, định dạng dd/MM/yyyy)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày (bắt buộc, định dạng dd/MM/yyyy)
        /// </summary>
        public string ToDate { get; set; }
    }
}
