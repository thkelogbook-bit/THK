using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh sách dữ liệu dùng chung
    /// </summary>
    public class SystemDataModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public List<ValueLabelModel> Factories { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public List<ValueLabelModel> ProductionPlants { get; set; }

        /// <summary>
        /// Danh sách người dùng hệ thống
        /// </summary>
        public List<UserInfo> Users { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        public List<ValueLabelModel> UserStatus { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public List<ValueLabelModel> UserTypes { get; set; }

        /// <summary>
        /// Vai trò
        /// </summary>
        public List<ValueLabelModel> UserRoles { get; set; }

        /// <summary>
        /// Nguồn của tài khoản người dùng
        /// </summary>
        public List<ValueLabelModel> UserSources { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public List<ValueLabelModel> AccessStatus { get; set; }

        /// <summary>
        /// Quyền Nhân viên
        /// </summary>
        public List<UserPermissionModel> UserPermission { get; set; }

        /// <summary>
        /// Quyền thao tác
        /// </summary>
        public List<ValueLabelModel> DataPermission { get; set; }

        /// <summary>
        /// Tổ máy
        /// </summary>
        public List<ValueLabelModel> PowerUnit { get; set; }

        /// <summary>
        /// Sử dụng thẻ RFID/QR Code
        /// </summary>
        public List<ValueLabelModel> RFIDQR { get; set; }

        /// <summary>
        /// Chọn này
        /// </summary>
        public List<ValueLabelModel> DateType { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public List<ValueLabelModel> OperationShift { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public List<ValueLabelModel> OperationCrew { get; set; }

        /// <summary>
        /// Mẫu tình hình giao nhận ca
        /// </summary>
        public ValueLabelModel DeliveryShiftMethod { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public List<ValueLabelModel> Units { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public List<ValueLabelModel> TicketTypes { get; set; }

        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public List<ValueLabelModel> TicketStatus { get; set; }

        /// <summary>
        /// Danh sách cương vị
        /// </summary>
        public List<ValueLabelModel> OperationPositions { get; set; }

        /// <summary>
        /// Danh sách chu kỳ
        /// </summary>
        public List<ValueLabelModel> Cycles { get; set; }

        /// <summary>
        /// Danh sách nhóm thông số
        /// </summary>
        public List<ValueLabelModel> ParameterGroups { get; set; }

        /// <summary>
        /// Danh sách công ty
        /// </summary>
        public List<ValueLabelModel> Companies { get; set; }

        /// <summary>
        /// Danh sách bậc an toàn điện
        /// </summary>
        public List<ValueLabelModel> Electricranks { get; set; }

        /// <summary>
        /// Danh sách bậc thợ
        /// </summary>
        public List<ValueLabelModel> LaborRanks { get; set; }

        /// <summary>
        /// Danh sách bậc an toàn điện
        /// </summary>
        public List<ValueStringLabelModel> ElectricranksStr { get; set; }

        /// <summary>
        /// Danh sách bậc thợ
        /// </summary>
        public List<ValueStringLabelModel> LaborRanksStr { get; set; }

        /// <summary>
        /// Lỗi thiết bị
        /// </summary>
        public List<ValueLabelModel> DeviceErrors { get; set; }

        /// <summary>
        /// Tình trạng xử lý
        /// </summary>
        public List<ValueLabelModel> ProcessingStatus { get; set; }

        /// <summary>
        /// Danh mục thủy cơ nhiệt hóa
        /// </summary>
        public List<ValueLabelModel> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Vị trí làm việc
        /// </summary>
        public List<ValueLabelModel> WorkLocation { get; set; }

        /// <summary>
        /// Sự kiện gửi email
        /// </summary>
        public List<ValueLabelModel> EmailNotifications { get; set; }

        /// <summary>
        /// Các chức năng có sự kiện gửi email
        /// </summary>
        public List<ValueLabelModel> ModuleNames { get; set; }
    }
}
