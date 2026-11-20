using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số trả lên UI
    /// </summary>
    public class AssignmentResponseModel
    {
    }

    /// <summary>
    /// Tham số trả lên UI của nhà máy
    /// </summary>
    public class APFactoryResponseModel
    {
        /// <summary>
        /// ID nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên nhà máy
        /// </summary>
        public string FactoryName { get; set; }
    }

    /// <summary>
    /// Tham số trả lên UI của cương vị
    /// </summary>
    public class APOperationPositionResponseModel
    {
        /// <summary>
        /// ID cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Tên cương vị
        /// </summary>
        public string OperationPositionName { get; set; }
    }

    /// <summary>
    /// Tham số trả lên UI của ca vận hành
    /// </summary>
    public class APOperationShiftResponseModel
    {
        /// <summary>
        /// ID ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Tên ca vận hành
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Giờ bắt đầu (số nguyên, VD: 6)
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// Giờ kết thúc (số nguyên, VD: 14)
        /// </summary>
        public double EndTime { get; set; }
    }

    /// <summary>
    /// Tham số trả lên UI của nhân viên — ánh xạ từ AssignmentEmployee join User
    /// </summary>
    public class AssignUserResponseModel
    {
        /// <summary>
        /// ID bản ghi AssignmentEmployee
        /// </summary>
        public int AssignmentEmployeeID { get; set; }

        /// <summary>
        /// ID người dùng — ánh xạ từ AssignmentEmployee.UserID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Tên đăng nhập — ánh xạ từ User.UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên — ánh xạ từ User.FullName
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Loại tài khoản — ánh xạ từ User.AccountType
        /// (dùng để phân biệt nội bộ / bên ngoài phía FE nếu cần)
        /// </summary>
        public int AccountType { get; set; }
    }

    // -------------------------------------------------------
    // Raw result — ánh xạ 1-1 với flat row trả về từ SP PCON_GetAssignmentSchedule
    // C# dùng class này để GroupBy thành nested response
    // -------------------------------------------------------

    /// <summary>
    /// Flat row trả về từ SP PCON_GetAssignmentSchedule
    /// </summary>
    public class APAssignmentScheduleRawResult
    {
        /// <summary>
        /// ID bản ghi AssignmentSlot
        /// </summary>
        public int AssignmentSlotID { get; set; }

        /// <summary>
        /// ID cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Tên cương vị
        /// </summary>
        public string OperationPositionName { get; set; }

        /// <summary>
        /// ID ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Tên ca vận hành
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Thứ tự ca (dùng để sort: Ca sáng → Ca chiều → Ca đêm)
        /// </summary>
        public int ShiftPosition { get; set; }

        /// <summary>
        /// Giờ bắt đầu ca
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// Giờ kết thúc ca
        /// </summary>
        public double EndTime { get; set; }

        /// <summary>
        /// Ngày làm việc
        /// </summary>
        public DateTime WorkDate { get; set; }

        /// <summary>
        /// Thứ trong tuần (0: CN, 1: T2, ..., 6: T7)
        /// </summary>
        public int DayOfWeekCode { get; set; }

        /// <summary>
        /// true nếu slot đã có ít nhất 1 bản ghi trong AssignmentEmployeeHistory
        /// </summary>
        public bool HasOverride { get; set; }

        /// <summary>
        /// ID bản ghi AssignmentEmployee
        /// </summary>
        public int AssignmentEmployeeID { get; set; }

        /// <summary>
        /// ID người dùng được phân công
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Loại tài khoản (phân biệt nội bộ / bên ngoài)
        /// </summary>
        public int AccountType { get; set; }
    }

    // -------------------------------------------------------
    // Response models cho getAssignmentSchedule
    // -------------------------------------------------------

    /// <summary>
    /// Bảng phân công theo cương vị (cấp 1)
    /// </summary>
    public class AssignmentScheduleResponseModel
    {
        /// <summary>
        /// ID cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Tên cương vị
        /// </summary>
        public string OperationPositionName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp cương vị — dùng để order phía FE
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Danh sách ca vận hành trong cương vị này
        /// </summary>
        public List<AssignmentShiftResponseModel> OperationShifts { get; set; }
    }

    /// <summary>
    /// Ca vận hành trong bảng phân công (cấp 2)
    /// </summary>
    public class AssignmentShiftResponseModel
    {
        /// <summary>
        /// ID ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Tên ca vận hành
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Giờ bắt đầu
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// Giờ kết thúc
        /// </summary>
        public double EndTime { get; set; }

        /// <summary>
        /// Danh sách chi tiết phân công theo ngày
        /// </summary>
        public List<AssignmentDetailResponseModel> AssignmentDetails { get; set; }
    }

    /// <summary>
    /// Chi tiết phân công theo ngày (cấp 3 — tương ứng 1 ô trong bảng, ánh xạ từ AssignmentSlot)
    /// </summary>
    public class AssignmentDetailResponseModel
    {
        /// <summary>
        /// ID bản ghi AssignmentSlot — dùng khi gọi update hoặc lấy audit log
        /// </summary>
        public int AssignmentSlotID { get; set; }

        /// <summary>
        /// Ngày làm việc (dd/MM/yyyy) — ánh xạ từ AssignmentSlot.WorkDate
        /// </summary>
        public string WorkDate { get; set; }

        /// <summary>
        /// Thứ trong tuần (0: CN, 1: T2, ..., 6: T7) — tính từ WorkDate
        /// </summary>
        public int DayOfWeekCode { get; set; }

        /// <summary>
        /// true nếu ô này đã có ít nhất 1 bản ghi trong AssignmentEmployeeHistory
        /// </summary>
        public bool HasOverride { get; set; }

        /// <summary>
        /// Danh sách nhân sự được phân công — ánh xạ từ AssignmentEmployee
        /// </summary>
        public List<AssignUserResponseModel> AssignedUsers { get; set; }
    }
}
