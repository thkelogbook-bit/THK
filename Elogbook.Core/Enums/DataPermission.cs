using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Quyền thao tác
    /// </summary>
    public enum DataPermission
    {
        /// <summary>
        /// Nội bộ
        /// </summary>
        [Description("Nội bộ")]
        Limited = 1,
        /// <summary>
        /// Không giới hạn
        /// </summary>
        [Description("Không giới hạn")]
        Unlimited,
    }

    /// <summary>
    /// Danh sách phân quyền module trong hệ thống
    /// </summary>
    public enum EnumModule
    {
        /// <summary>
        /// Quản lý tài khoản nhân viên
        /// </summary>
        [Description("DM_TKNV")]
        DM_TKNV = 1,
        /// <summary>
        /// Quản lý Người dùng ngoài hệ thống
        /// </summary>
        [Description("DM_NDNHT")]
        DM_NDNHT = 2,
        /// <summary>
        /// Quản lý vai trò
        /// </summary>
        [Description("DM_VT")]
        DM_VT = 3,
        /// <summary>
        /// Quản lý ca vận hành
        /// </summary>
        [Description("DM_CVH")]
        DM_CVH = 4,
        /// <summary>
        /// Quản lý kíp vận hành
        /// </summary>
        [Description("DM_KVH")]
        DM_KVH = 5,
        /// <summary>
        /// Quản lý chu kỳ
        /// </summary>
        [Description("DM_CK")]
        DM_CK = 6,
        /// <summary>
        /// Quản lý cương vị
        /// </summary>
        [Description("DM_CV")]
        DM_CV = 7,
        /// <summary>
        /// Quản lý nhóm thông số
        /// </summary>
        [Description("DM_NTS")]
        DM_NTS = 8,
        /// <summary>
        /// Quản lý thông số
        /// </summary>
        [Description("DM_TS")]
        DM_TS = 9,
        /// <summary>
        /// Quản lý tổ máy
        /// </summary>
        [Description("DM_TM")]
        DM_TM = 10,
        /// <summary>
        /// Sơ đồ tổ chức
        /// </summary>
        [Description("DM_SDTC")]
        DM_SDTC = 11,
        /// <summary>
        /// Quản lý nhà máy
        /// </summary>
        [Description("DM_NM")]
        DM_NM = 12,
        /// <summary>
        /// Theo dõi nhật ký vận hành
        /// </summary>
        [Description("ONLY_NKVH")]
        ONLY_NKVH = 13,
        /// <summary>
        /// Quản lý nhật ký vận hành
        /// </summary>
        [Description("NKVH")]
        NKVH = 14,
        /// <summary>
        /// Theo dõi thông số vận hành
        /// </summary>
        [Description("ONLY_TSVH")]
        ONLY_TSVH = 15,
        /// <summary>
        /// Quản lý thông số vận hành
        /// </summary>
        [Description("TSVH")]
        TSVH = 16,
        /// <summary>
        /// Quản lý Đăng ký công tác Điện
        /// </summary>
        [Description("ĐKCTĐ")]
        ĐKCTĐ = 17,
        /// <summary>
        /// Quản lý Cấp phiếu công tác Điện
        /// </summary>
        [Description("CPCTĐ")]
        CPCTĐ = 18,
        /// <summary>
        /// Quản lý Phiếu công tác Điện
        /// </summary>
        [Description("PCTĐ")]
        PCTĐ = 19,
        /// <summary>
        /// Quản lý Đăng ký công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("ĐKCTTCNH")]
        ĐKCTTCNH = 20,
        /// <summary>
        /// Quản lý Cấp phiếu công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("CPCTTCNH")]
        CPCTTCNH = 21,
        /// <summary>
        /// Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("PCTTCNH")]
        PCTTCNH = 22,
        /// <summary>
        /// Quản lý tồn tại trong ca
        /// </summary>
        [Description("TTTC")]
        TTTC = 23,
        /// <summary>
        /// Quản lý Phiếu thao tác mẫu
        /// </summary>
        [Description("PTTM")]
        PTTM = 24,
        /// <summary>
        /// Quản lý Tạo phiếu thao tác
        /// </summary>
        [Description("TPTT")]
        TPTT = 25,
        /// <summary>
        /// Quản lý Cấp phiếu thao tác
        /// </summary>
        [Description("CPTT")]
        CPTT = 26,
        /// <summary>
        /// Quản lý Phiếu thao tác
        /// </summary>
        [Description("PTT")]
        PTT = 27,
        /// <summary>
        /// Quản lý nhật ký vận hành
        /// </summary>
        [Description("NKVH_MOBILE")]
        NKVH_MOBILE = 28,
        /// <summary>
        /// Quản lý thông số vận hành
        /// </summary>
        [Description("TSVH_MOBILE")]
        TSVH_MOBILE = 29,
        /// <summary>
        /// Quyền cho phép công tác điện
        /// </summary>
        [Description("CTĐ_MOBILE")]
        CTĐ_MOBILE = 30,
        /// <summary>
        /// Quản lý Phiếu công tác Điện
        /// </summary>
        [Description("PCTĐ_MOBILE")]
        PCTĐ_MOBILE = 31,
        /// <summary>
        /// Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("CTTCNH_MOBILE")]
        CTTCNH_MOBILE = 32,
        /// <summary>
        /// Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("PCTTCNH_MOBILE")]
        PCTTCNH_MOBILE = 33,
        /// <summary>
        /// Quản lý tồn tại trong ca
        /// </summary>
        [Description("TTTC_MOBILE")]
        TTTC_MOBILE = 34,
        /// <summary>
        /// Quản lý Phiếu thao tác
        /// </summary>
        [Description("PTT_MOBILE")]
        PTT_MOBILE = 35,
        /// <summary>
        /// Cấu hình phiên bản
        /// </summary>
        [Description("APP_VER")]
        APP_VER = 36,
        /// <summary>
        /// Liên kết Mã QLKT
        /// </summary>
        [Description("PMIS")]
        PMIS = 37,
        /// <summary>
        /// Tìm kiếm
        /// </summary>
        [Description("QUICK_SEARCH")]
        QUICK_SEARCH = 38,
        /// <summary>
        /// Thiết lập lịch
        /// </summary>
        [Description("SCHEDULE")]
        SCHEDULE = 39,
        /// <summary>
        /// Cấu hình PMIS
        /// </summary>
        [Description("CH_PMIS")]
        CH_PMIS = 40,
        /// <summary>
        /// Quản lý sự kiện
        /// </summary>
        [Description("QLSK")]
        QLSK = 41,
        /// <summary>
        /// Quản lý Đợt công tác SCL
        /// </summary>
        [Description("QLSCL")]
        QLSCL = 42,
        /// <summary>
        /// Quản lý Danh sách Phiếu công tác Điện chờ đăng ký
        /// </summary>
        [Description("QLDS_PCTD_CDK")]
        QLDS_PCTD_CDK = 43,
        /// <summary>
        /// Quản lý Danh sách Phiếu công tác TCNH chờ đăng ký
        /// </summary>
        [Description("QLDS_PCTTCNH_CDK")]
        QLDS_PCTTCNH_CDK = 44,
        /// <summary>
        /// Quản lý Phiếu yêu cầu - Vận hành viên
        /// </summary>
        [Description("QLPYCVHV")]
        QLPYCVHV = 45,
        /// <summary>
        /// Quản lý Phiếu yêu cầu - Trưởng ca
        /// </summary>
        [Description("QLPYCTC")]
        QLPYCTC = 46,
        /// <summary>
        /// Quản lý Phiếu yêu cầu - Đơn vị sửa chữa
        /// </summary>
        [Description("QLPYCDVSC")]
        QLPYCDVSC = 47,
        /// <summary>
        /// Quản lý loại công việc AM
        /// </summary>
        [Description("QLLCVAM")]
        QLLCVAM = 48,
        /// <summary>
        /// Quản lý công việc AM
        /// </summary>
        [Description("QLCVAM")]
        QLCVA = 49,
        /// <summary>
        /// Quản lý phân loại thiết bị
        /// </summary>
        [Description("QLPLTB")]
        QLPLTB = 50,
        /// <summary>
        /// Quản lý thiết bị
        /// </summary>
        [Description("QLTB")]
        QLTB = 51,
        /// <summary>
        /// Quản lý loại thiết bị
        /// </summary>
        [Description("QLLTB")]
        QLLTB = 52,
        /// <summary>
        /// Quản lý mức độ khẩn cấp
        /// </summary>
        [Description("QLMDKC")]
        QLMDKC = 53,
        /// <summary>
        /// Quản lý nguyên nhân tạm dừng công việc
        /// </summary>
        [Description("QLNNTDCV")]
        QLNNTDCV = 54,
        /// <summary>
        /// Quản lý nguyên nhân không thực hiện
        /// </summary>
        [Description("QLNNKTH")]
        QLNNKTH = 55,
        /// <summary>
        /// Quản lý khu vực/địa điểm
        /// </summary>
        [Description("QLKVDD")]
        QLKVDD = 56,
        /// <summary>
        /// Quản lý lịch thực hiện AM
        /// </summary>
        [Description("QLLTHAM")]
        QLLTHAM = 57,
        /// <summary>
        /// Quản lý bảo trì tự quản
        /// </summary>
        [Description("QLBTTQ")]
        QLBTTQ = 58,
        /// <summary>
        /// Tổng quan
        /// </summary>
        [Description("DASHBOARD")]
        DASHBOARD = 59,
        /// <summary>
        /// Quản lý Báo cáo
        /// </summary>
        [Description("REPORT")]
        REPORT = 60,
        /// <summary>
        /// Cấu hình hộp thư
        /// </summary>
        [Description("CHHTEMAIL")]
        CHHTEMAIL = 61,
        /// <summary>
        /// Cấu hình thư mẫu
        /// </summary>
        [Description("EMAILT")]
        EMAILT = 62,
    }

    /// <summary>
    /// Danh sách phân quyền các chức năng trong hệ thống
    /// </summary>
    public enum EnumUserPermission
    {
        /// <summary>
        /// Danh sách tài khoản nhân viên
        /// </summary>
        [Description("VIEW_TKNV")]
        VIEW_TKNV = 1,
        /// <summary>
        /// Thêm mới tài khoản nhân viên
        /// </summary>
        [Description("ADD_TKNV")]
        ADD_TKNV = 2,
        /// <summary>
        /// Hiệu chỉnh tài khoản nhân viên
        /// </summary>
        [Description("EDIT_TKNV")]
        EDIT_TKNV = 3,
        /// <summary>
        /// Xóa tài khoản nhân viên
        /// </summary>
        [Description("DELETE_TKNV")]
        DELETE_TKNV = 4,
        /// <summary>
        /// Danh sách người dùng ngoài hệ thống
        /// </summary>
        [Description("VIEW_NDNHT")]
        VIEW_NDNHT = 5,
        /// <summary>
        /// Thêm mới người dùng ngoài hệ thống
        /// </summary>
        [Description("ADD_NDNHT")]
        ADD_NDNHT = 6,
        /// <summary>
        /// Hiệu chỉnh người dùng ngoài hệ thống
        /// </summary>
        [Description("EDIT_NDNHT")]
        EDIT_NDNHT = 7,
        /// <summary>
        /// Xóa người dùng ngoài hệ thống
        /// </summary>
        [Description("DELETE_NDNHT")]
        DELETE_NDNHT = 8,
        /// <summary>
        /// Danh sách vai trò
        /// </summary>
        [Description("VIEW_VT")]
        VIEW_VT = 9,
        /// <summary>
        /// Thêm mới vai trò
        /// </summary>
        [Description("ADD_VT")]
        ADD_VT = 10,
        /// <summary>
        /// Hiệu chỉnh vai trò
        /// </summary>
        [Description("EDIT_VT")]
        EDIT_VT = 11,
        /// <summary>
        /// Xóa vai trò
        /// </summary>
        [Description("DELETE_VT")]
        DELETE_VT = 12,
        /// <summary>
        /// Danh sách ca vận hành
        /// </summary>
        [Description("VIEW_CVH")]
        VIEW_CVH = 13,
        /// <summary>
        /// Thêm mới ca vận hành
        /// </summary>
        [Description("ADD_CVH")]
        ADD_CVH = 14,
        /// <summary>
        /// Hiệu chỉnh ca vận hành
        /// </summary>
        [Description("EDIT_CVH")]
        EDIT_CVH = 15,
        /// <summary>
        /// Xóa ca vận hành
        /// </summary>
        [Description("DELETE_CVH")]
        DELETE_CVH = 16,
        /// <summary>
        /// Danh sách kíp vận hành
        /// </summary>
        [Description("VIEW_KVH")]
        VIEW_KVH = 17,
        /// <summary>
        /// Thêm mới kíp vận hành
        /// </summary>
        [Description("ADD_KVH")]
        ADD_KVH = 18,
        /// <summary>
        /// Hiệu chỉnh kíp vận hành
        /// </summary>
        [Description("EDIT_KVH")]
        EDIT_KVH = 19,
        /// <summary>
        /// Xóa kíp vận hành
        /// </summary>
        [Description("DELETE_KVH")]
        DELETE_KVH = 20,
        /// <summary>
        /// Danh sách chu kỳ
        /// </summary>
        [Description("VIEW_CK")]
        VIEW_CK = 21,
        /// <summary>
        /// Thêm mới chu kỳ
        /// </summary>
        [Description("ADD_CK")]
        ADD_CK = 22,
        /// <summary>
        /// Hiệu chỉnh chu kỳ
        /// </summary>
        [Description("EDIT_CK")]
        EDIT_CK = 23,
        /// <summary>
        /// Xóa chu kỳ
        /// </summary>
        [Description("DELETE_CK")]
        DELETE_CK = 24,
        /// <summary>
        /// Danh sách cương vị
        /// </summary>
        [Description("VIEW_CV")]
        VIEW_CV = 25,
        /// <summary>
        /// Thêm mới cương vị
        /// </summary>
        [Description("ADD_CV")]
        ADD_CV = 26,
        /// <summary>
        /// Hiệu chỉnh cương vị
        /// </summary>
        [Description("EDIT_CV")]
        EDIT_CV = 27,
        /// <summary>
        /// Xóa cương vị
        /// </summary>
        [Description("DELETE_CV")]
        DELETE_CV = 28,
        /// <summary>
        /// Danh sách nhóm thông số
        /// </summary>
        [Description("VIEW_NTS")]
        VIEW_NTS = 29,
        /// <summary>
        /// Thêm mới nhóm thông số
        /// </summary>
        [Description("ADD_NTS")]
        ADD_NTS = 30,
        /// <summary>
        /// Hiệu chỉnh nhóm thông số
        /// </summary>
        [Description("EDIT_NTS")]
        EDIT_NTS = 31,
        /// <summary>
        /// Xóa nhóm thông số
        /// </summary>
        [Description("DELETE_NTS")]
        DELETE_NTS = 32,
        /// <summary>
        /// Danh sách thông số
        /// </summary>
        [Description("VIEW_TS")]
        VIEW_TS = 33,
        /// <summary>
        /// Thêm mới thông số
        /// </summary>
        [Description("ADD_TS")]
        ADD_TS = 34,
        /// <summary>
        /// Hiệu chỉnh thông số
        /// </summary>
        [Description("EDIT_TS")]
        EDIT_TS = 35,
        /// <summary>
        /// Xóa thông số
        /// </summary>
        [Description("DELETE_TS")]
        DELETE_TS = 36,
        /// <summary>
        /// Danh sách tổ máy
        /// </summary>
        [Description("VIEW_TM")]
        VIEW_TM = 37,
        /// <summary>
        /// Thêm mới tổ máy
        /// </summary>
        [Description("ADD_TM")]
        ADD_TM = 38,
        /// <summary>
        /// Hiệu chỉnh tổ máy
        /// </summary>
        [Description("EDIT_TM")]
        EDIT_TM = 39,
        /// <summary>
        /// Xóa tổ máy
        /// </summary>
        [Description("DELETE_TM")]
        DELETE_TM = 40,
        /// <summary>
        /// Danh sách sơ đồ tổ chức
        /// </summary>
        [Description("VIEW_SDTC")]
        VIEW_SDTC = 41,
        /// <summary>
        /// Thêm mới sơ đồ tổ chức
        /// </summary>
        [Description("ADD_SDTC")]
        ADD_SDTC = 42,
        /// <summary>
        /// Hiệu chỉnh sơ đồ tổ chức
        /// </summary>
        [Description("EDIT_SDTC")]
        EDIT_SDTC = 43,
        /// <summary>
        /// Xóa sơ đồ tổ chức
        /// </summary>
        [Description("DELETE_SDTC")]
        DELETE_SDTC = 44,
        /// <summary>
        /// Danh sách nhà máy
        /// </summary>
        [Description("VIEW_NM")]
        VIEW_NM = 45,
        /// <summary>
        /// Thêm mới nhà máy
        /// </summary>
        [Description("ADD_NM")]
        ADD_NM = 46,
        /// <summary>
        /// Hiệu chỉnh nhà máy
        /// </summary>
        [Description("EDIT_NM")]
        EDIT_NM = 47,
        /// <summary>
        /// Xóa nhà máy
        /// </summary>
        [Description("DELETE_NM")]
        DELETE_NM = 48,
        /// <summary>
        /// Chi tiết nhật ký vận hành
        /// </summary>
        [Description("VIEWONLY_NKVH")]
        VIEWONLY_NKVH = 49,
        /// <summary>
        /// Danh sách nhật ký vận hành
        /// </summary>
        [Description("VIEW_NKVH")]
        VIEW_NKVH = 50,
        /// <summary>
        /// Thêm mới nhật ký vận hành
        /// </summary>
        [Description("ADD_NKVH")]
        ADD_NKVH = 51,
        /// <summary>
        /// Hiệu chỉnh nhật ký vận hành
        /// </summary>
        [Description("EDIT_NKVH")]
        EDIT_NKVH = 52,
        /// <summary>
        /// Xóa nhật ký vận hành
        /// </summary>
        [Description("DELETE_NKVH")]
        DELETE_NKVH = 53,
        /// <summary>
        /// Cho phép Trưởng ca cập nhật/xóa nhật ký vận hành đã giao/nhận ca
        /// </summary>
        [Description("EDIT_DELETE_NKVH")]
        EDIT_DELETE_NKVH = 54,
        /// <summary>
        /// Chi tiết thông số vận hành
        /// </summary>
        [Description("VIEWONLY_TSVH")]
        VIEWONLY_TSVH = 55,
        /// <summary>
        /// Danh sách thông số vận hành
        /// </summary>
        [Description("VIEW_TSVH")]
        VIEW_TSVH = 56,
        /// <summary>
        /// Hiệu chỉnh thông số vận hành
        /// </summary>
        [Description("EDIT_TSVH")]
        EDIT_TSVH = 57,
        /// <summary>
        /// Danh sách đăng ký công tác điện
        /// </summary>
        [Description("VIEW_ĐKCTĐ")]
        VIEW_ĐKCTĐ = 58,
        /// <summary>
        /// Thêm mới đăng ký công tác điện
        /// </summary>
        [Description("ADD_ĐKCTĐ")]
        ADD_ĐKCTĐ = 59,
        /// <summary>
        /// Hiệu chỉnh đăng ký công tác điện
        /// </summary>
        [Description("EDIT_ĐKCTĐ")]
        EDIT_ĐKCTĐ = 60,
        /// <summary>
        /// Xóa đăng ký công tác điện
        /// </summary>
        [Description("DELETE_ĐKCTĐ")]
        DELETE_ĐKCTĐ = 61,
        /// <summary>
        /// Danh sách phiếu đăng ký công tác điện chờ cấp
        /// </summary>
        [Description("VIEW_CPCTĐ")]
        VIEW_CPCTĐ = 62,
        /// <summary>
        /// Cấp phiếu công tác điện mới
        /// </summary>
        [Description("ADD_CPCTĐ")]
        ADD_CPCTĐ = 63,
        /// <summary>
        /// Xử lý phiếu đăng ký công tác điện
        /// </summary>
        [Description("EDIT_CPCTĐ")]
        EDIT_CPCTĐ = 64,
        /// <summary>
        /// Danh sách phiếu công tác điện
        /// </summary>
        [Description("VIEW_PCTĐ")]
        VIEW_PCTĐ = 65,
        /// <summary>
        /// Nhập liệu phiếu công tác điện
        /// </summary>
        [Description("ADD_PCTĐ")]
        ADD_PCTĐ = 66,
        /// <summary>
        /// Kiểm tra hoàn thành phiếu công tác điện
        /// </summary>
        [Description("CLOSE_PCTĐ")]
        CLOSE_PCTĐ = 67,
        /// <summary>
        /// Danh sách đăng ký công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("VIEW_ĐKCTTCNH")]
        VIEW_ĐKCTTCNH = 68,
        /// <summary>
        /// Thêm mới đăng ký công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("ADD_ĐKCTTCNH")]
        ADD_ĐKCTTCNH = 69,
        /// <summary>
        /// Hiệu chỉnh đăng ký công tácThuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("EDIT_ĐKCTTCNH")]
        EDIT_ĐKCTTCNH = 70,
        /// <summary>
        /// Xóa đăng ký công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("DELETE_ĐKCTTCNH")]
        DELETE_ĐKCTTCNH = 71,
        /// <summary>
        /// Danh sách phiếu đăng ký công tác Thuỷ-cơ-nhiệt-hoá chờ cấp
        /// </summary>
        [Description("VIEW_CPCTTCNH")]
        VIEW_CPCTTCNH = 72,
        /// <summary>
        /// Cấp phiếu công tác Thuỷ-cơ-nhiệt-hoá mới
        /// </summary>
        [Description("ADD_CPCTTCNH")]
        ADD_CPCTTCNH = 73,
        /// <summary>
        /// Xử lý phiếu đăng ký công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("EDIT_CPCTTCNH")]
        EDIT_CPCTTCNH = 74,
        /// <summary>
        /// Danh sách phiếu công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("VIEW_PCTTCNH")]
        VIEW_PCTTCNH = 75,
        /// <summary>
        /// Nhập liệu phiếu công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("ADD_PCTTCNH")]
        ADD_PCTTCNH = 76,
        /// <summary>
        /// Danh sách tồn tại trong ca
        /// </summary>
        [Description("VIEW_TTTC")]
        VIEW_TTTC = 77,
        /// <summary>
        /// Hiệu chỉnh tồn tại trong ca
        /// </summary>
        [Description("EDIT_TTTC")]
        EDIT_TTTC = 78,
        /// <summary>
        /// Xóa tồn tại trong ca
        /// </summary>
        [Description("DELETE_TTTC")]
        DELETE_TTTC = 79,
        /// <summary>
        /// Danh sách phiếu mẫu được tạo
        /// </summary>
        [Description("VIEW_PTTM")]
        VIEW_PTTM = 80,
        /// <summary>
        /// Tạo phiếu mẫu
        /// </summary>
        [Description("ADD_PTTM")]
        ADD_PTTM = 81,
        /// <summary>
        /// Hiệu chỉnh phiếu mẫu
        /// </summary>
        [Description("EDIT_PTTM")]
        EDIT_PTTM = 82,
        /// <summary>
        /// Xóa phiếu mẫu
        /// </summary>
        [Description("DELETE_PTTM")]
        DELETE_PTTM = 83,
        /// <summary>
        /// Danh sách phiếu thao tác được tạo
        /// </summary>
        [Description("VIEW_TPTT")]
        VIEW_TPTT = 84,
        /// <summary>
        /// Tạo phiếu thao tác
        /// </summary>
        [Description("ADD_TPTT")]
        ADD_TPTT = 85,
        /// <summary>
        /// Hiệu chỉnh phiếu thao tác
        /// </summary>
        [Description("EDIT_TPTT")]
        EDIT_TPTT = 86,
        /// <summary>
        /// Xóa phiếu thao tác
        /// </summary>
        [Description("DELETE_TPTT")]
        DELETE_TPTT = 87,
        /// <summary>
        /// Danh sách phiếu thao tác chờ duyệt
        /// </summary>
        [Description("VIEW_CPTT")]
        VIEW_CPTT = 88,
        /// <summary>
        /// Xử lý phiếu thao tác chờ duyệt
        /// </summary>
        [Description("EDIT_CPTT")]
        EDIT_CPTT = 89,
        /// <summary>
        /// Danh sách phiếu thao tác
        /// </summary>
        [Description("VIEW_PTT")]
        VIEW_PTT = 90,
        /// <summary>
        /// Kiểm tra giao nhận sau thao tác
        /// </summary>
        [Description("CLOSE_PTT")]
        CLOSE_PTT = 91,
        /// <summary>
        /// Cho phép admin xóa phiếu thao tác
        /// </summary>
        [Description("DELETE_TPTT_ADMIN")]
        DELETE_TPTT_ADMIN = 92,
        /// <summary>
        /// Danh sách nhật ký vận hành
        /// </summary>
        [Description("VIEW_NKVH_MOBILE")]
        VIEW_NKVH_MOBILE = 93,
        /// <summary>
        /// Thêm mới nhật ký vận hành
        /// </summary>
        [Description("ADD_NKVH_MOBILE")]
        ADD_NKVH_MOBILE = 94,
        /// <summary>
        /// Hiệu chỉnh nhật ký vận hành
        /// </summary>
        [Description("EDIT_NKVH_MOBILE")]
        EDIT_NKVH_MOBILE = 95,
        /// <summary>
        /// Xóa nhật ký vận hành
        /// </summary>
        [Description("DELETE_NKVH_MOBILE")]
        DELETE_NKVH_MOBILE = 96,
        /// <summary>
        /// Danh sách thông số vận hành
        /// </summary>
        [Description("VIEW_TSVH_MOBILE")]
        VIEW_TSVH_MOBILE = 97,
        /// <summary>
        /// Hiệu chỉnh thông số vận hành
        /// </summary>
        [Description("EDIT_TSVH_MOBILE")]
        EDIT_TSVH_MOBILE = 98,
        /// <summary>
        /// Quyền cho phép công tác điện
        /// </summary>
        [Description("ALLOW_CTĐ_MOBILE")]
        ALLOW_CTĐ_MOBILE = 99,
        /// <summary>
        /// Danh sách phiếu công tác điện chờ cho phép
        /// </summary>
        [Description("VIEW_CPCTĐ_MOBILE")]
        VIEW_CPCTĐ_MOBILE = 100,
        /// <summary>
        /// Cho phép thực hiện công tác điện
        /// </summary>
        [Description("EDIT_CPCTĐ_MOBILE")]
        EDIT_CPCTĐ_MOBILE = 101,
        /// <summary>
        /// Danh sách phiếu công tác điện đang có
        /// </summary>
        [Description("VIEW_PCTĐ_MOBILE")]
        VIEW_PCTĐ_MOBILE = 102,
        /// <summary>
        /// Hiệu chỉnh phiếu công tác điện
        /// </summary>
        [Description("EDIT_PCTĐ_MOBILE")]
        EDIT_PCTĐ_MOBILE = 103,
        /// <summary>
        /// Khoá phiếu công tác điện
        /// </summary>
        [Description("LOCK_PCTĐ_MOBILE")]
        LOCK_PCTĐ_MOBILE = 104,
        /// <summary>
        /// Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("ALLOW_CTTCNH_MOBILE")]
        ALLOW_CTTCNH_MOBILE = 105,
        /// <summary>
        /// Danh sách phiếu công tác Thuỷ-cơ-nhiệt-hoá chờ cho phép
        /// </summary>
        [Description("VIEW_CPCTTCNH_MOBILE")]
        VIEW_CPCTTCNH_MOBILE = 106,
        /// <summary>
        /// Cho phép thực hiện công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("EDIT_CPCTTCNH_MOBILE")]
        EDIT_CPCTTCNH_MOBILE = 107,
        /// <summary>
        /// Danh sách phiếu công tác Thuỷ-cơ-nhiệt-hoá đang có
        /// </summary>
        [Description("VIEW_PCTTCNH_MOBILE")]
        VIEW_PCTTCNH_MOBILE = 108,
        /// <summary>
        /// Hiệu chỉnh phiếu công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("EDIT_PCTTCNH_MOBILE")]
        EDIT_PCTTCNH_MOBILE = 109,
        /// <summary>
        /// Khoá phiếu công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("LOCK_PCTTCNH_MOBILE")]
        LOCK_PCTTCNH_MOBILE = 110,
        /// <summary>
        /// Danh sách tồn tại trong ca
        /// </summary>
        [Description("VIEW_TTTC_MOBILE")]
        VIEW_TTTC_MOBILE = 111,
        /// <summary>
        /// Hiệu chỉnh tồn tại trong ca
        /// </summary>
        [Description("EDIT_TTTC_MOBILE")]
        EDIT_TTTC_MOBILE = 112,
        /// <summary>
        /// Xóa tồn tại trong ca
        /// </summary>
        [Description("DELETE_TTTC_MOBILE")]
        DELETE_TTTC_MOBILE = 113,
        /// <summary>
        /// Danh sách phiếu thao tác
        /// </summary>
        [Description("VIEW_PTT_MOBILE")]
        VIEW_PTT_MOBILE = 114,
        /// <summary>
        /// Hiệu chỉnh phiếu thao tác
        /// </summary>
        [Description("EDIT_PTT_MOBILE")]
        EDIT_PTT_MOBILE = 115,
        /// <summary>
        /// Xem phiên bản
        /// </summary>
        [Description("VIEW_CHHT")]
        VIEW_CHHT = 116,
        /// <summary>
        /// Cập nhật phiên bản
        /// </summary>
        [Description("EDIT_CHHT")]
        EDIT_CHHT = 117,
        /// <summary>
        /// Xem Mã QLKT
        /// </summary>
        [Description("VIEW_PMIS")]
        VIEW_PMIS = 118,
        /// <summary>
        /// Cập nhật Mã QLKT
        /// </summary>
        [Description("EDIT_PMIS")]
        EDIT_PMIS = 119,
        /// <summary>
        /// Nhật ký vận hành
        /// </summary>
        [Description("QUICK_SEARCH_NKVH")]
        QUICK_SEARCH_NKVH = 120,
        /// <summary>
        /// Thông số vận hành
        /// </summary>
        [Description("QUICK_SEARCH_TSVH")]
        QUICK_SEARCH_TSVH = 121,
        /// <summary>
        /// Xem lập lịch
        /// </summary>
        [Description("VIEW_SCHEDULE")]
        VIEW_SCHEDULE = 122,
        /// <summary>
        /// Thêm mới lập lịch
        /// </summary>
        [Description("ADD_SCHEDULE")]
        ADD_SCHEDULE = 123,
        /// <summary>
        /// Cập nhật lập lịch
        /// </summary>
        [Description("EDIT_SCHEDULE")]
        EDIT_SCHEDULE = 124,
        /// <summary>
        /// Xóa lập lịch
        /// </summary>
        [Description("DELETE_SCHEDULE")]
        DELETE_SCHEDULE = 125,
        /// <summary>
        /// Xem cấu hình PMIS
        /// </summary>
        [Description("VIEW_CH_PMIS")]
        VIEW_CH_PMIS = 126,
        /// <summary>
        /// Cập nhật cấu hình PMIS
        /// </summary>
        [Description("EDIT_CH_PMIS")]
        EDIT_CH_PMIS = 127,
        /// <summary>
        /// Xem sự kiện
        /// </summary>
        [Description("VIEW_QLSK")]
        VIEW_QLSK = 128,
        /// <summary>
        /// Thêm mới sự kiện
        /// </summary>
        [Description("ADD_QLSK")]
        ADD_QLSK = 129,
        /// <summary>
        /// Cập nhật sự kiện
        /// </summary>
        [Description("EDIT_QLSK")]
        EDIT_QLSK = 130,
        /// <summary>
        /// Xóa sự kiện
        /// </summary>
        [Description("DELETE_QLSK")]
        DELETE_QLSK = 131,
        /// <summary>
        /// Xem thông tin Đợt công tác SCL
        /// </summary>
        [Description("VIEW_QLSCL")]
        VIEW_QLSCL = 132,
        /// <summary>
        /// Thêm mới Đợt công tác SCL
        /// </summary>
        [Description("ADD_QLSCL")]
        ADD_QLSCL = 133,
        /// <summary>
        /// Cập nhật Đợt công tác SCL
        /// </summary>
        [Description("EDIT_QLSCL")]
        EDIT_QLSCL = 134,
        /// <summary>
        /// Xóa Đợt công tác SCL
        /// </summary>
        [Description("DELETE_QLSCL")]
        DELETE_QLSCL = 135,
        /// <summary>
        /// Xem Danh sách Phiếu công tác Điện chờ đăng ký
        /// </summary>
        [Description("VIEW_QLDS_PCTD_CDK")]
        VIEW_QLDS_PCTD_CDK = 136,
        /// <summary>
        /// Thực hiện đăng ký Phiếu công tác Điện chờ đăng ký
        /// </summary>
        [Description("EDIT_QLDS_PCTD_CDK")]
        EDIT_QLDS_PCTD_CDK = 137,
        /// <summary>
        /// Xem Danh sách Phiếu công tác TCNH chờ đăng ký
        /// </summary>
        [Description("VIEW_QLDS_PCTTCNH_CDK")]
        VIEW_QLDS_PCTTCNH_CDK = 138,
        /// <summary>
        /// Thực hiện đăng ký Phiếu công tác TCNH chờ đăng ký
        /// </summary>
        [Description("EDIT_QLDS_PCTTCNH_CDK")]
        EDIT_QLDS_PCTTCNH_CDK = 139,
        /// <summary>
        /// Xem thông tin Quản lý Phiếu yêu cầu - Vận hành viên
        /// </summary>
        [Description("VIEW_QLPYCVHV")]
        VIEW_QLPYCVHV = 140,
        /// <summary>
        /// Thêm mới Phiếu yêu cầu - Vận hành viên
        /// </summary>
        [Description("ADD_QLPYCVHV")]
        ADD_QLPYCVHV = 141,
        /// <summary>
        /// Cập nhật Phiếu yêu cầu - Vận hành viên
        /// </summary>
        [Description("EDIT_QLPYCVHV")]
        EDIT_QLPYCVHV = 142,
        /// <summary>
        /// Xóa Phiếu yêu cầu - Vận hành viên
        /// </summary>
        [Description("DELETE_QLPYCVHV")]
        DELETE_QLPYCVHV = 143,
        /// <summary>
        /// Xem thông tin Quản lý Phiếu yêu cầu - Trưởng ca
        /// </summary>
        [Description("VIEW_QLPYCTC")]
        VIEW_QLPYCTC = 144,
        /// <summary>
        /// Thêm mới Phiếu yêu cầu - Trưởng ca
        /// </summary>
        [Description("ADD_QLPYCTC")]
        ADD_QLPYCTC = 145,
        /// <summary>
        /// Cập nhật Phiếu yêu cầu - Trưởng ca
        /// </summary>
        [Description("EDIT_QLPYCTC")]
        EDIT_QLPYCTC = 146,
        /// <summary>
        /// Xóa Phiếu yêu cầu - Trưởng ca
        /// </summary>
        [Description("DELETE_QLPYCTC")]
        DELETE_QLPYCTC = 147,
        /// <summary>
        /// Xem thông tin Quản lý Phiếu yêu cầu - Đơn vị sửa chữa
        /// </summary>
        [Description("VIEW_QLPYCDVSC")]
        VIEW_QLPYCDVSC = 148,
        /// <summary>
        /// Thêm mới Phiếu yêu cầu - Đơn vị sửa chữa
        /// </summary>
        [Description("ADD_QLPYCDVSC")]
        ADD_QLPYCDVSC = 149,
        /// <summary>
        /// Cập nhật Phiếu yêu cầu - Đơn vị sửa chữa
        /// </summary>
        [Description("EDIT_QLPYCDVSC")]
        EDIT_QLPYCDVSC = 150,
        /// <summary>
        /// Xóa Phiếu yêu cầu - Đơn vị sửa chữa
        /// </summary>
        [Description("DELETE_QLPYCDVSC")]
        DELETE_QLPYCDVSC = 151,
        /// <summary>
        /// Xem thông tin loại công việc AM
        /// </summary>
        [Description("VIEW_QLLCVAM")]
        VIEW_QLLCVAM = 152,
        /// <summary>
        /// Thêm mới loại công việc AM
        /// </summary>
        [Description("ADD_QLLCVAM")]
        ADD_QLLCVAM = 153,
        /// <summary>
        /// Cập nhật loại công việc AM
        /// </summary>
        [Description("EDIT_QLLCVAM")]
        EDIT_QLLCVAM = 154,
        /// <summary>
        /// Xóa loại công việc AM
        /// </summary>
        [Description("DELETE_QLLCVAM")]
        DELETE_QLLCVAM = 155,
        /// <summary>
        /// Xem thông tin công việc AM
        /// </summary>
        [Description("VIEW_QLCVAM")]
        VIEW_QLCVAM = 156,
        /// <summary>
        /// Thêm mới công việc AM
        /// </summary>
        [Description("ADD_QLCVAM")]
        ADD_QLCVAM = 157,
        /// <summary>
        /// Cập nhật công việc AM
        /// </summary>
        [Description("EDIT_QLCVAM")]
        EDIT_QLCVAM = 158,
        /// <summary>
        /// Xóa công việc AM
        /// </summary>
        [Description("DELETE_QLCVAM")]
        DELETE_QLCVAM = 159,
        /// <summary>
        /// Xem thông tin phân loại thiết bị
        /// </summary>
        [Description("VIEW_QLPLTB")]
        VIEW_QLPLTB = 160,
        /// <summary>
        /// Thêm mới phân loại thiết bị
        /// </summary>
        [Description("ADD_QLPLTB")]
        ADD_QLPLTB = 161,
        /// <summary>
        /// Cập nhật phân loại thiết bị
        /// </summary>
        [Description("EDIT_QLPLTB")]
        EDIT_QLPLTB = 162,
        /// <summary>
        /// Xóa phân loại thiết bị
        /// </summary>
        [Description("DELETE_QLPLTB")]
        DELETE_QLPLTB = 163,
        /// <summary>
        /// Xem thông tin thiết bị
        /// </summary>
        [Description("VIEW_QLTB")]
        VIEW_QLTB = 164,
        /// <summary>
        /// Thêm mới thiết bị
        /// </summary>
        [Description("ADD_QLTB")]
        ADD_QLTB = 165,
        /// <summary>
        /// Cập nhật thiết bị
        /// </summary>
        [Description("EDIT_QLTB")]
        EDIT_QLTB = 166,
        /// <summary>
        /// Xóa thiết bị
        /// </summary>
        [Description("DELETE_QLTB")]
        DELETE_QLTB = 167,
        /// <summary>
        /// Xem thông tin loại thiết bị
        /// </summary>
        [Description("VIEW_QLLTB")]
        VIEW_QLLTB = 168,
        /// <summary>
        /// Thêm mới loại thiết bị
        /// </summary>
        [Description("ADD_QLLTB")]
        ADD_QLLTB = 169,
        /// <summary>
        /// Cập nhật loại thiết bị
        /// </summary>
        [Description("EDIT_QLLTB")]
        EDIT_QLLTB = 170,
        /// <summary>
        /// Xóa loại thiết bị
        /// </summary>
        [Description("DELETE_QLLTB")]
        DELETE_QLLTB = 171,
        /// <summary>
        /// Xem thông tin mức độ khẩn cấp
        /// </summary>
        [Description("VIEW_QLMDKC")]
        VIEW_QLMDKC = 172,
        /// <summary>
        /// Thêm mới mức độ khẩn cấp
        /// </summary>
        [Description("ADD_QLMDKC")]
        ADD_QLMDKC = 173,
        /// <summary>
        /// Cập nhật mức độ khẩn cấp
        /// </summary>
        [Description("EDIT_QLMDKC")]
        EDIT_QLMDKC = 174,
        /// <summary>
        /// Xóa mức độ khẩn cấp
        /// </summary>
        [Description("DELETE_QLMDKC")]
        DELETE_QLMDKC = 175,
        /// <summary>
        /// Xem thông tin nguyên nhân tạm dừng công việc
        /// </summary>
        [Description("VIEW_QLNNTDCV")]
        VIEW_QLNNTDCV = 176,
        /// <summary>
        /// Thêm mới nguyên nhân tạm dừng công việc
        /// </summary>
        [Description("ADD_QLNNTDCV")]
        ADD_QLNNTDCV = 177,
        /// <summary>
        /// Cập nhật nguyên nhân tạm dừng công việc
        /// </summary>
        [Description("EDIT_QLNNTDCV")]
        EDIT_QLNNTDCV = 178,
        /// <summary>
        /// Xóa nguyên nhân tạm dừng công việc
        /// </summary>
        [Description("DELETE_QLNNTDCV")]
        DELETE_QLNNTDCV = 179,
        /// <summary>
        /// Xem thông tin nguyên nhân không thực hiện
        /// </summary>
        [Description("VIEW_QLNNKTH")]
        VIEW_QLNNKTH = 180,
        /// <summary>
        /// Thêm mới nguyên nhân không thực hiện
        /// </summary>
        [Description("ADD_QLNNKTH")]
        ADD_QLNNKTH = 181,
        /// <summary>
        /// Cập nhật nguyên nhân không thực hiện
        /// </summary>
        [Description("EDIT_QLNNKTH")]
        EDIT_QLNNKTH = 182,
        /// <summary>
        /// Xóa nguyên nhân không thực hiện
        /// </summary>
        [Description("DELETE_QLNNKTH")]
        DELETE_QLNNKTH = 183,
        /// <summary>
        /// Xem thông tin khu vực/địa điểm
        /// </summary>
        [Description("VIEW_QLKVDD")]
        VIEW_QLKVDD = 184,
        /// <summary>
        /// Thêm mới khu vực/địa điểm
        /// </summary>
        [Description("ADD_QLKVDD")]
        ADD_QLKVDD = 185,
        /// <summary>
        /// Cập nhật khu vực/địa điểm
        /// </summary>
        [Description("EDIT_QLKVDD")]
        EDIT_QLKVDD = 186,
        /// <summary>
        /// Xóa khu vực/địa điểm
        /// </summary>
        [Description("DELETE_QLKVDD")]
        DELETE_QLKVDD = 187,
        /// <summary>
        /// Xem thông tin lịch thực hiện AM
        /// </summary>
        [Description("VIEW_QLLTHAM")]
        VIEW_QLLTHAM = 188,
        /// <summary>
        /// Thêm mới lịch thực hiện AM
        /// </summary>
        [Description("ADD_QLLTHAM")]
        ADD_QLLTHAM = 189,
        /// <summary>
        /// Cập nhật lịch thực hiện AM
        /// </summary>
        [Description("EDIT_QLLTHAM")]
        EDIT_QLLTHAM = 190,
        /// <summary>
        /// Xóa lịch thực hiện AM
        /// </summary>
        [Description("DELETE_QLLTHAM")]
        DELETE_QLLTHAM = 191,
        /// <summary>
        /// Xem danh sách bảo trì tự quản
        /// </summary>
        [Description("VIEW_QLBTTQ")]
        VIEW_QLBTTQ = 192,
        /// <summary>
        /// Cho phép thực hiện
        /// </summary>
        [Description("PROCESS_QLBTTQ")]
        PROCESS_QLBTTQ = 193,
        /// <summary>
        /// Không cho phép thực hiện
        /// </summary>
        [Description("NO_PROCESS_QLBTTQ")]
        NO_PROCESS_QLBTTQ = 194,
        /// <summary>
        /// Xem danh Tổng quan
        /// </summary>
        [Description("VIEW_DASHBOARD")]
        VIEW_DASHBOARD = 195,
        /// <summary>
        /// Xem báo cáo Tuân thủ hoạt động AM
        /// </summary>
        [Description("VIEW_REPORT_TTAM")]
        VIEW_REPORT_TTAM = 196,
        /// <summary>
        /// Xem báo cáo Bất thường thiết bị
        /// </summary>
        [Description("VIEW_REPORT_BTTB")]
        VIEW_REPORT_BTTB = 197,
        /// <summary>
        /// Xem báo cáo Thời gian xử lý
        /// </summary>
        [Description("VIEW_REPORT_TGXL")]
        VIEW_REPORT_TGXL = 198,
        /// <summary>
        /// Xem báo cáo Yêu cầu bảo trì
        /// </summary>
        [Description("VIEW_REPORT_YCBT")]
        VIEW_REPORT_YCBT = 199,
        /// <summary>
        /// Xem báo cáo Tiến trình xử lý
        /// </summary>
        [Description("VIEW_REPORT_TTXL")]
        VIEW_REPORT_TTXL = 200,
        /// <summary>
        /// Xem cấu hình hộp thư
        /// </summary>
        [Description("VIEW_CHHTEMAIL")]
        VIEW_CHHTEMAIL = 201,
        /// <summary>
        /// Cập nhật cấu hình hộp thư
        /// </summary>
        [Description("EDIT_CHHTEMAIL")]
        EDIT_CHHTEMAIL = 202,
        /// <summary>
        /// Xem thư mẫu
        /// </summary>
        [Description("VIEW_EMAILT")]
        VIEW_EMAILT = 203,
        /// <summary>
        /// Thêm mới thư mẫu
        /// </summary>
        [Description("ADD_EMAILT")]
        ADD_EMAILT = 204,
        /// <summary>
        /// Cập nhật thư mẫu
        /// </summary>
        [Description("EDIT_EMAILT")]
        EDIT_EMAILT = 205,
        /// <summary>
        /// Xóa thư mẫu
        /// </summary>
        [Description("DELETE_EMAILT")]
        DELETE_EMAILT = 206,
        /// <summary>
        /// Xem thông tin nhà cung cấp
        /// </summary>
        [Description("VIEW_QLNCC")]
        VIEW_QLNCC = 207,
        /// <summary>
        /// Thêm mới nhà cung cấp
        /// </summary>
        [Description("ADD_QLNCC")]
        ADD_QLNCC = 208,
        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        [Description("EDIT_QLNCC")]
        EDIT_QLNCC = 209,
        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        [Description("DELETE_QLNCC")]
        DELETE_QLNCC = 210,
        /// <summary>
        /// Xem thông tin nhà chế tạo
        /// </summary>
        [Description("VIEW_QLNCT")]
        VIEW_QLNCT = 211,
        /// <summary>
        /// Thêm mới nhà chế tạo
        /// </summary>
        [Description("ADD_QLNCT")]
        ADD_QLNCT = 212,
        /// <summary>
        /// Cập nhật nhà chế tạo
        /// </summary>
        [Description("EDIT_QLNCT")]
        EDIT_QLNCT = 213,
        /// <summary>
        /// Xóa nhà chế tạo
        /// </summary>
        [Description("DELETE_QLNCT")]
        DELETE_QLNCT = 214,
        /// <summary>
        /// Xem thông tin nước sản xuất
        /// </summary>
        [Description("VIEW_QLNSX")]
        VIEW_QLNSX = 215,
        /// <summary>
        /// Thêm mới nước sản xuất
        /// </summary>
        [Description("ADD_QLNSX")]
        ADD_QLNSX = 216,
        /// <summary>
        /// Cập nhật nước sản xuất
        /// </summary>
        [Description("EDIT_QLNSX")]
        EDIT_QLNSX = 217,
        /// <summary>
        /// Xóa nước sản xuất
        /// </summary>
        [Description("DELETE_QLNSX")]
        DELETE_QLNSX = 218,
        /// <summary>
        /// Thêm mới Phiếu công tác số lượng lớn
        /// </summary>
        [Description("ADD_ĐKPCT_SLL")]
        ADD_ĐKPCT_SLL = 219,
        /// <summary>
        /// Xem thông tin Quản lý Phiếu yêu cầu
        /// </summary>
        [Description("VIEW_QLPYC_MOBILE")]
        VIEW_QLPYC_MOBILE = 220,
        /// <summary>
        /// Thêm mới Phiếu yêu cầu
        /// </summary>
        [Description("ADD_QLPYC_MOBILE")]
        ADD_QLPYC_MOBILE = 221,
        /// <summary>
        /// Cập nhật Phiếu yêu cầu
        /// </summary>
        [Description("EDIT_QLPYC_MOBILE")]
        EDIT_QLPYC_MOBILE = 222,
        /// <summary>
        /// Xóa Phiếu yêu cầu
        /// </summary>
        [Description("DELETE_QLPYC_MOBILE")]
        DELETE_QLPYC_MOBILE = 223,
        /// <summary>
        /// Xem danh sách bảo trì tự quản
        /// </summary>
        [Description("VIEW_QLBTTQ_MOBILE")]
        VIEW_QLBTTQ_MOBILE = 224,
        /// <summary>
        /// Thực hiện bảo trì tự quản
        /// </summary>
        [Description("EDIT_QLBTTQ_MOBILE")]
        EDIT_QLBTTQ_MOBILE = 225,
        /// <summary>
        /// Cho phép đồng bộ dữ liệu PMIS
        /// </summary>
        [Description("SYNC_PMIS_QLKVDD")]
        SYNC_PMIS_QLKVDD = 226,
        /// <summary>
        /// Cho phép đồng bộ dữ liệu PMIS
        /// </summary>
        [Description("SYNC_PMIS_QLLTB")]
        SYNC_PMIS_QLLTB = 227,
        /// <summary>
        /// Cho phép đồng bộ dữ liệu PMIS
        /// </summary>
        [Description("SYNC_PMIS_QLPLTB")]
        SYNC_PMIS_QLPLTB = 228,
        /// <summary>
        /// Cho phép đồng bộ dữ liệu PMIS
        /// </summary>
        [Description("SYNC_PMIS_QLTB")]
        SYNC_PMIS_QLTB = 229,
        /// <summary>
        /// Hủy Phiếu công tác Điện chờ đăng ký
        /// </summary>
        [Description("ANNULLED_QLDS_PCTD_CDK")]
        ANNULLED_QLDS_PCTD_CDK = 230,
        /// <summary>
        /// Hủy Phiếu công tác TCNH chờ đăng ký
        /// </summary>
        [Description("ANNULLED_QLDS_PCTTCNH_CDK")]
        ANNULLED_QLDS_PCTTCNH_CDK = 231,
        /// <summary>
        /// Danh sách xuất file nhật ký vận hành
        /// </summary>
        [Description("VIEW_EXPORT_NKVH")]
        VIEW_EXPORT_NKVH = 232,
        /// <summary>
        /// Xóa xuất file nhật ký vận hành
        /// </summary>
        [Description("DELETE_EXPORT_NKVH")]
        DELETE_EXPORT_NKVH = 233,
        /// <summary>
        /// Xuất file nhật ký vận hành
        /// </summary>
        [Description("DOWNLOAD_EXPORT_NKVH")]
        DOWNLOAD_EXPORT_NKVH = 234,
        /// <summary>
        /// Danh sách nhóm người dùng
        /// </summary>
        [Description("VIEW_NND")]
        VIEW_NND = 235,
        /// <summary>
        /// Thêm mới nhóm người dùng
        /// </summary>
        [Description("ADD_NND")]
        ADD_NND = 236,
        /// <summary>
        /// Hiệu chỉnh nhóm người dùng
        /// </summary>
        [Description("EDIT_NND")]
        EDIT_NND = 237,
        /// <summary>
        /// Xóa nhóm người dùng
        /// </summary>
        [Description("DELETE_NND")]
        DELETE_NND = 238,
        /// <summary>
        /// Xóa phiếu công tác
        /// </summary>
        [Description("DELETE_PCTĐ")]
        DELETE_PCTĐ = 239,
        /// <summary>
        /// Xóa phiếu công tác
        /// </summary>
        [Description("DELETE_PCTTCNH")]
        DELETE_PCTTCNH = 240,
        /// <summary>
        /// Thêm mới biên bản khảo sát hiện trường
        /// </summary>
        [Description("ADD_BBKSHT")]
        ADD_BBKSHT = 241,
        /// <summary>
        /// Thêm mới biên bản khảo sát hiện trường
        /// </summary>
        [Description("ADD_BBKSHT_MOBILE")]
        ADD_BBKSHT_MOBILE = 242,
        /// <summary>
        /// Cấp lệnh công tác điện mới
        /// </summary>
        [Description("ADD_CLCTD")]
        ADD_CLCTD = 243,
        /// <summary>
        /// Cấp lệnh công tác Thuỷ-cơ-nhiệt-hoá mới
        /// </summary>
        [Description("ADD_CLCTTCNH")]
        ADD_CLCTTCNH = 244,
        /// <summary>
        /// Thêm mới đăng ký lệnh công tác điện
        /// </summary>
        [Description("ADD_DKLCTD")]
        ADD_DKLCTD = 245,
        /// <summary>
        /// Thêm mới đăng ký lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("ADD_DKLCTTCNH")]
        ADD_DKLCTTCNH = 246,
        /// <summary>
        /// Nhập liệu lệnh công tác điện
        /// </summary>
        [Description("ADD_LCTD")]
        ADD_LCTD = 247,
        /// <summary>
        /// Nhập liệu lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("ADD_LCTTCNH")]
        ADD_LCTTCNH = 248,
        /// <summary>
        /// Thêm Phân công ca vận hành
        /// </summary>
        [Description("ADD_PC_CVH")]
        ADD_PC_CVH = 249,
        /// <summary>
        /// Thêm báo cáo Bảo trì tự quản
        /// </summary>
        [Description("ADD_REPORT_BTTQ")]
        ADD_REPORT_BTTQ = 250,
        /// <summary>
        /// Thêm báo cáo Lệnh công tác
        /// </summary>
        [Description("ADD_REPORT_LCT")]
        ADD_REPORT_LCT = 251,
        /// <summary>
        /// Thêm báo cáo Nhật ký vận hành
        /// </summary>
        [Description("ADD_REPORT_NKVH")]
        ADD_REPORT_NKVH = 252,
        /// <summary>
        /// Thêm báo cáo Phiếu công tác
        /// </summary>
        [Description("ADD_REPORT_PCT")]
        ADD_REPORT_PCT = 253,
        /// <summary>
        /// Thêm báo cáo Phiếu thao tác
        /// </summary>
        [Description("ADD_REPORT_PTT")]
        ADD_REPORT_PTT = 254,
        /// <summary>
        /// Thêm báo cáo Phiếu yêu cầu
        /// </summary>
        [Description("ADD_REPORT_PYC")]
        ADD_REPORT_PYC = 255,
        /// <summary>
        /// Thêm báo cáo Quản lý tài sản
        /// </summary>
        [Description("ADD_REPORT_QLTS")]
        ADD_REPORT_QLTS = 256,
        /// <summary>
        /// Thêm báo cáo Sửa chữa lớn
        /// </summary>
        [Description("ADD_REPORT_SCL")]
        ADD_REPORT_SCL = 257,
        /// <summary>
        /// Thêm báo cáo Thông số vận hành
        /// </summary>
        [Description("ADD_REPORT_TSVH")]
        ADD_REPORT_TSVH = 258,
        /// <summary>
        /// Thêm mới cấu hình SLA
        /// </summary>
        [Description("ADD_SLA")]
        ADD_SLA = 259,
        /// <summary>
        /// Thêm mới thời gian làm việc
        /// </summary>
        [Description("ADD_TGLV")]
        ADD_TGLV = 260,
        /// <summary>
        /// Thêm mới tài liệu
        /// </summary>
        [Description("ADD_TL")]
        ADD_TL = 261,
        /// <summary>
        /// Cảnh báo
        /// </summary>
        [Description("ALERT_DASHBOARD")]
        ALERT_DASHBOARD = 262,
        /// <summary>
        /// Xóa dữ liệu backup
        /// </summary>
        [Description("DELETE_BACKUP_DATA")]
        DELETE_BACKUP_DATA = 263,
        /// <summary>
        /// Xóa biên bản khảo sát hiện trường
        /// </summary>
        [Description("DELETE_BBKSHT")]
        DELETE_BBKSHT = 264,
        /// <summary>
        /// Xóa biên bản khảo sát hiện trường
        /// </summary>
        [Description("DELETE_BBKSHT_MOBILE")]
        DELETE_BBKSHT_MOBILE = 265,
        /// <summary>
        /// Xóa dashboard
        /// </summary>
        [Description("DELETE_DASHBOARD")]
        DELETE_DASHBOARD = 266,
        /// <summary>
        /// Xóa đăng ký lệnh công tác điện
        /// </summary>
        [Description("DELETE_DKLCTD")]
        DELETE_DKLCTD = 267,
        /// <summary>
        /// Xóa đăng ký lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("DELETE_DKLCTTCNH")]
        DELETE_DKLCTTCNH = 268,
        /// <summary>
        /// Xóa Phân công ca vận hành
        /// </summary>
        [Description("DELETE_PC_CVH")]
        DELETE_PC_CVH = 269,
        /// <summary>
        /// Xóa báo cáo Bảo trì tự quản
        /// </summary>
        [Description("DELETE_REPORT_BTTQ")]
        DELETE_REPORT_BTTQ = 270,
        /// <summary>
        /// Xóa báo cáo Lệnh công tác
        /// </summary>
        [Description("DELETE_REPORT_LCT")]
        DELETE_REPORT_LCT = 271,
        /// <summary>
        /// Xóa báo cáo nhật ký vận hành
        /// </summary>
        [Description("DELETE_REPORT_NKVH")]
        DELETE_REPORT_NKVH = 272,
        /// <summary>
        /// Xóa báo cáo Phiếu công tác
        /// </summary>
        [Description("DELETE_REPORT_PCT")]
        DELETE_REPORT_PCT = 273,
        /// <summary>
        /// Xóa báo cáo Phiếu thao tác
        /// </summary>
        [Description("DELETE_REPORT_PTT")]
        DELETE_REPORT_PTT = 274,
        /// <summary>
        /// Xóa báo cáo Phiếu yêu cầu
        /// </summary>
        [Description("DELETE_REPORT_PYC")]
        DELETE_REPORT_PYC = 275,
        /// <summary>
        /// Xóa báo cáo Quản lý tài sản
        /// </summary>
        [Description("DELETE_REPORT_QLTS")]
        DELETE_REPORT_QLTS = 276,
        /// <summary>
        /// Xóa báo cáo Sửa chữa lớn
        /// </summary>
        [Description("DELETE_REPORT_SCL")]
        DELETE_REPORT_SCL = 277,
        /// <summary>
        /// Xóa báo cáo Thông số vận hành
        /// </summary>
        [Description("DELETE_REPORT_TSVH")]
        DELETE_REPORT_TSVH = 278,
        /// <summary>
        /// Xóa cấu hình SLA
        /// </summary>
        [Description("DELETE_SLA")]
        DELETE_SLA = 279,
        /// <summary>
        /// Xóa thời gian làm việc
        /// </summary>
        [Description("DELETE_TGLV")]
        DELETE_TGLV = 280,
        /// <summary>
        /// Xóa tài tài liệu
        /// </summary>
        [Description("DELETE_TL")]
        DELETE_TL = 281,
        /// <summary>
        /// Download dữ liệu backup
        /// </summary>
        [Description("DOWNLOAD_BACKUP_DATA")]
        DOWNLOAD_BACKUP_DATA = 282,
        /// <summary>
        /// Cập nhật biên bản khảo sát hiện trường
        /// </summary>
        [Description("EDIT_BBKSHT")]
        EDIT_BBKSHT = 283,
        /// <summary>
        /// Cập nhật biên bản khảo sát hiện trường
        /// </summary>
        [Description("EDIT_BBKSHT_MOBILE")]
        EDIT_BBKSHT_MOBILE = 284,
        /// <summary>
        /// Cập nhật cấu hình cảnh báo
        /// </summary>
        [Description("EDIT_CHCB")]
        EDIT_CHCB = 285,
        /// <summary>
        /// Xử lý lệnh đăng ký công tác điện
        /// </summary>
        [Description("EDIT_CLCTD")]
        EDIT_CLCTD = 286,
        /// <summary>
        /// Xử lý lệnh đăng ký công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("EDIT_CLCTTCNH")]
        EDIT_CLCTTCNH = 287,
        /// <summary>
        /// Chỉnh sữa dashboard
        /// </summary>
        [Description("EDIT_DASHBOARD")]
        EDIT_DASHBOARD = 288,
        /// <summary>
        /// Hiệu chỉnh đăng ký lệnh công tác điện
        /// </summary>
        [Description("EDIT_DKLCTD")]
        EDIT_DKLCTD = 289,
        /// <summary>
        /// Hiệu chỉnh đăng ký lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("EDIT_DKLCTTCNH")]
        EDIT_DKLCTTCNH = 290,
        /// <summary>
        /// Kiểm tra hoàn thành lệnh công tác điện
        /// </summary>
        [Description("EDIT_LCTD")]
        EDIT_LCTD = 291,
        /// <summary>
        /// Hiệu chỉnh lệnh công tác điện
        /// </summary>
        [Description("EDIT_LCTD_MOBILE")]
        EDIT_LCTD_MOBILE = 292,
        /// <summary>
        /// Hiệu chỉnh lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("EDIT_LCTTCNH_MOBILE")]
        EDIT_LCTTCNH_MOBILE = 293,
        /// <summary>
        /// Cập nhập Phân công ca vận hành
        /// </summary>
        [Description("EDIT_PC_CVH")]
        EDIT_PC_CVH = 294,
        /// <summary>
        /// Chỉnh sữa báo cáo Bảo trì tự quản
        /// </summary>
        [Description("EDIT_REPORT_BTTQ")]
        EDIT_REPORT_BTTQ = 295,
        /// <summary>
        /// Chỉnh sữa báo cáo Lệnh công tác
        /// </summary>
        [Description("EDIT_REPORT_LCT")]
        EDIT_REPORT_LCT = 296,
        /// <summary>
        /// Chỉnh sữa báo cáo nhật ký vận hành
        /// </summary>
        [Description("EDIT_REPORT_NKVH")]
        EDIT_REPORT_NKVH = 297,
        /// <summary>
        /// Chỉnh sữa báo cáo Phiếu công tác
        /// </summary>
        [Description("EDIT_REPORT_PCT")]
        EDIT_REPORT_PCT = 298,
        /// <summary>
        /// Chỉnh sữa báo cáo Phiếu thao tác
        /// </summary>
        [Description("EDIT_REPORT_PTT")]
        EDIT_REPORT_PTT = 299,
        /// <summary>
        /// Chỉnh sữa báo cáo Phiếu yêu cầu
        /// </summary>
        [Description("EDIT_REPORT_PYC")]
        EDIT_REPORT_PYC = 300,
        /// <summary>
        /// Chỉnh sữa báo cáo Quản lý tài sản
        /// </summary>
        [Description("EDIT_REPORT_QLTS")]
        EDIT_REPORT_QLTS = 301,
        /// <summary>
        /// Chỉnh sữa báo cáo Sửa chữa lớn
        /// </summary>
        [Description("EDIT_REPORT_SCL")]
        EDIT_REPORT_SCL = 302,
        /// <summary>
        /// Chỉnh sữa báo cáo Thông số vận hành
        /// </summary>
        [Description("EDIT_REPORT_TSVH")]
        EDIT_REPORT_TSVH = 303,
        /// <summary>
        /// Chỉnh sửa cấu hình SLA
        /// </summary>
        [Description("EDIT_SLA")]
        EDIT_SLA = 304,
        /// <summary>
        /// Hiệu chỉnh thời gian làm việc
        /// </summary>
        [Description("EDIT_TGLV")]
        EDIT_TGLV = 305,
        /// <summary>
        /// Hiệu chỉnh tài liệu
        /// </summary>
        [Description("EDIT_TL")]
        EDIT_TL = 306,
        /// <summary>
        /// Khoá lệnh công tác điện
        /// </summary>
        [Description("LOCK_LCTD_MOBILE")]
        LOCK_LCTD_MOBILE = 307,
        /// <summary>
        /// Khoá lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("LOCK_LCTTCNH_MOBILE")]
        LOCK_LCTTCNH_MOBILE = 308,
        /// <summary>
        /// Tài liệu quy trình vận hành
        /// </summary>
        [Description("QUICK_SEARCH_TL")]
        QUICK_SEARCH_TL = 309,
        /// <summary>
        /// View dữ liệu backup
        /// </summary>
        [Description("VIEW_BACKUP_DATA")]
        VIEW_BACKUP_DATA = 310,
        /// <summary>
        /// Xem biên bản khảo sát hiện trường
        /// </summary>
        [Description("VIEW_BBKSHT")]
        VIEW_BBKSHT = 311,
        /// <summary>
        /// Xem biên bản khảo sát hiện trường
        /// </summary>
        [Description("VIEW_BBKSHT_MOBILE")]
        VIEW_BBKSHT_MOBILE = 312,
        /// <summary>
        /// Xem hình cảnh báo
        /// </summary>
        [Description("VIEW_CHCB")]
        VIEW_CHCB = 313,
        /// <summary>
        /// Danh sách lệnh đăng ký công tác điện chờ cấp
        /// </summary>
        [Description("VIEW_CLCTD")]
        VIEW_CLCTD = 314,
        /// <summary>
        /// Danh sách lệnh đăng ký công tác Thuỷ-cơ-nhiệt-hoá chờ cấp
        /// </summary>
        [Description("VIEW_CLCTTCNH")]
        VIEW_CLCTTCNH = 315,
        /// <summary>
        /// Danh sách đăng ký lệnh công tác điện
        /// </summary>
        [Description("VIEW_DKLCTD")]
        VIEW_DKLCTD = 316,
        /// <summary>
        /// Danh sách đăng ký lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("VIEW_DKLCTTCNH")]
        VIEW_DKLCTTCNH = 317,
        /// <summary>
        /// Danh sách lệnh công tác điện
        /// </summary>
        [Description("VIEW_LCTD")]
        VIEW_LCTD = 318,
        /// <summary>
        /// Danh sách lệnh công tác điện
        /// </summary>
        [Description("VIEW_LCTD_MOBILE")]
        VIEW_LCTD_MOBILE = 319,
        /// <summary>
        /// Danh sách lệnh công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        [Description("VIEW_LCTTCNH")]
        VIEW_LCTTCNH = 320,
        /// <summary>
        /// Danh sách lệnh công tác Thuỷ-cơ-nhiệt-hoá đang có
        /// </summary>
        [Description("VIEW_LCTTCNH_MOBILE")]
        VIEW_LCTTCNH_MOBILE = 321,
        /// <summary>
        /// Xem Phân công ca vận hành
        /// </summary>
        [Description("VIEW_PC_CVH")]
        VIEW_PC_CVH = 322,
        /// <summary>
        /// Danh sách báo cáo Bảo trì tự quản
        /// </summary>
        [Description("VIEW_REPORT_BTTQ")]
        VIEW_REPORT_BTTQ = 323,
        /// <summary>
        /// Danh sách báo cáo Lệnh công tác
        /// </summary>
        [Description("VIEW_REPORT_LCT")]
        VIEW_REPORT_LCT = 324,
        /// <summary>
        /// Danh sách báo cáo nhật ký vận hành
        /// </summary>
        [Description("VIEW_REPORT_NKVH")]
        VIEW_REPORT_NKVH = 325,
        /// <summary>
        /// Danh sách báo cáo Phiếu công tác
        /// </summary>
        [Description("VIEW_REPORT_PCT")]
        VIEW_REPORT_PCT = 326,
        /// <summary>
        /// Danh sách báo cáo Phiếu thao tác
        /// </summary>
        [Description("VIEW_REPORT_PTT")]
        VIEW_REPORT_PTT = 327,
        /// <summary>
        /// Danh sách báo cáo Phiếu yêu cầu
        /// </summary>
        [Description("VIEW_REPORT_PYC")]
        VIEW_REPORT_PYC = 328,
        /// <summary>
        /// Danh sách báo cáo Quản lý tài sản
        /// </summary>
        [Description("VIEW_REPORT_QLTS")]
        VIEW_REPORT_QLTS = 329,
        /// <summary>
        /// Danh sách báo cáo Sửa chữa lớn
        /// </summary>
        [Description("VIEW_REPORT_SCL")]
        VIEW_REPORT_SCL = 330,
        /// <summary>
        /// Danh sách báo cáo Thông số vận hành
        /// </summary>
        [Description("VIEW_REPORT_TSVH")]
        VIEW_REPORT_TSVH = 331,
        /// <summary>
        /// Xem cấu hình SLA
        /// </summary>
        [Description("VIEW_SLA")]
        VIEW_SLA = 332,
        /// <summary>
        /// Danh thời gian làm việc
        /// </summary>
        [Description("VIEW_TGLV")]
        VIEW_TGLV = 333,
        /// <summary>
        /// Danh sách tài liệu
        /// </summary>
        [Description("VIEW_TL")]
        VIEW_TL = 334,
        /// <summary>
        /// Xem thông tin xuất dữ liệu thông số vận hành
        /// </summary>
        [Description("VIEW_XDL_TSVH")]
        VIEW_XDL_TSVH = 335,
    }
}
