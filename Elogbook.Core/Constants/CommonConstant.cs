using System;

namespace Elogbook.Core.Constants
{
    /// <summary>
    /// Common Constant
    /// </summary>
    public struct CommonConstant
    {
        /// <summary>
        /// Operators
        /// </summary>
        public static class CompareOperators
        {
            /// <summary>
            /// Operators
            /// </summary>
            public const string
                LessThan = "LessThan",
                LessThanOrEqual = "LessThanOrEqual",
                Equal = "Equal",
                GreaterThanOrEqual = "GreaterThanOrEqual",
                GreaterThan = "GreaterThan",
                InRange = "InRange",
                Contains = "Contains";
        }

        /// <summary>
        /// Data Type
        /// </summary>
        public static class TypeConstants
        {
            /// <summary>
            /// All Types
            /// </summary>
            public static readonly Type[] NumericType =
            {
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
            };
        }

        /// <summary>
        /// EntityFrameworkConfiguration
        /// </summary>
        public struct EntityFrameworkConfiguration
        {
            /// <summary>
            /// IsDeleted
            /// </summary>
            public const string FilterIsDelete = "IsDeleted";
        }

        /// <summary>
        /// Số dòng mặc định của mỗi page
        /// </summary>
        public const int DEFAULT_PAGE_SIZE = 100;

        /// <summary>
        /// Page mặc định
        /// </summary>
        public const int DEFAULT_PAGE_NUMBER = 1;

        /// <summary>
        /// Thông tin người dùng không chính xác
        /// </summary>
        public const string INVALID_ACCOUNT = "Tài khoản hoặc mật khẩu chưa chính xác. Vui lòng thử lại.";

        /// <summary>
        /// Khóa tài khoản người dùng
        /// </summary>
        public const string LOCK_ACCOUNT = "Tài khoản của bạn đã bị khóa do đăng nhập sai mật khẩu quá 5 lần, vui lòng liên hệ quản trị hệ thống để mở khóa tài khoản.";

        /// <summary>
        /// Người dùng không có quyền
        /// </summary>
        public const string NOT_HAS_PERMISSION = "Bạn không có quyền truy cập hệ thống. Vui lòng liên hệ Quản trị viên để được hỗ trợ.";

        /// <summary>
        /// Người dùng không có quyền
        /// </summary>
        public const string NOT_HAS_PERMISSION_MOBILE = "Bạn không có quyền truy cập trang ứng dụng. Vui lòng liên hệ Quản trị viên để được hỗ trợ.";

        /// <summary>
        /// Không tìm thấy người dùng AD
        /// </summary>
        public const string ACCOUNT_NOTFOUND = "Tên đăng nhập không đúng. Vui lòng liên hệ Admin.";

        /// <summary>
        /// Người dùng không tồn tại hoặc không có quyền truy cập ứng dụng.
        /// </summary>
        public const string ACCOUNT_INVALID = "Người dùng không tồn tại hoặc không có quyền truy cập ứng dụng.";

        /// <summary>
        /// Tài khoản đã tồn tại
        /// </summary>
        public const string ACCOUNT_EXISTING = "Tài khoản người dùng đã tồn tại trong hệ thống.";

        /// <summary>
        /// Lỗi khi làm việc với AD
        /// </summary>
        public const string AD_COMMON_ERROR = "Có lỗi xảy ra khi kết nối tới AD server để lấy thông tin người dùng. Vui lòng kiểm tra log để biết nguyên nhân.";

        /// <summary>
        /// Định nghĩa key cho giá trị all
        /// </summary>
        public const int ALL_KEY = -1;

        /// <summary>
        /// Định nghĩa value cho giá trị all
        /// </summary>
        public const string ALL_VALUE = "Tất cả";

        /// <summary>
        /// Tên Phân xưởng
        /// </summary>
        public const string PRODUCTION_PLANT_NAME_REQUIRED = "Tên phân xưởng là thông tin bắt buộc.";

        /// <summary>
        /// Nhà máy
        /// </summary>
        public const string FACTORY_REQUIRED = "Nhà máy là thông tin bắt buộc.";

        /// <summary>
        /// Nhà máy
        /// </summary>
        public const string FACTORY_NOT_EXISTS = "Nhà máy không tồn tại trong hệ thống.";

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public const string CYCLE_REQUIRED = "Chu kỳ là thông tin bắt buộc.";

        /// <summary>
        /// Tên nhà máy
        /// </summary>
        public const string FACTORY_NAME_REQUIRED = "Tên nhà máy là thông tin bắt buộc.";

        /// <summary>
        /// Thuộc công ty
        /// </summary>
        public const string FACTORY_COMPANY_REQUIRED = "Công ty là thông tin bắt buộc.";

        /// <summary>
        /// Thuộc quyền quản lý của phân xưởng
        /// </summary>
        public const string FACTORY_PRODUCTIONPLANT_REQUIRED = "Phân xưởng là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra phân xưởng thuộc công ty
        /// </summary>
        public const string COMPANY_PRODUCTIONPLANT_NOTFOUND = "Công ty, Phân xưởng không tồn tại hoặc Phân xưởng không thuộc công ty.";

        /// <summary>
        /// Trùng công ty trong cùng nhà máy
        /// </summary>
        public const string DUPLICATE_FACTORY_COMPANY = "Nhà máy đã tồn tại trong công ty.";

        /// <summary>
        /// Kiểm tra trùng Phân xưởng
        /// </summary>
        public const string DUPLICATE_PRODUCTION_PLANT = "Phân xưởng đã tồn tại trong Nhà máy.";

        /// <summary>
        /// Tên Tổ máy
        /// </summary>
        public const string POWERUNITNAME_REQUIRED = "Tên Tổ máy là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng Tổ máy
        /// </summary>
        public const string DUPLICATE_POWERUNIT = "Tổ máy đã tồn tại trong Nhà máy.";

        /// <summary>
        /// Số lượng là thông tin bắt buộc.
        /// </summary>
        public const string QUANTITY_REQUIRED = "Số lượng là thông tin bắt buộc.";

        /// <summary>
        /// Tên Nhóm thông số
        /// </summary>
        public const string PARAMETERGROUPNAME_REQUIRED = "Tên Nhóm thông số là thông tin bắt buộc.";

        /// <summary>
        /// QR Code
        /// </summary>
        public const string QRCODE_REQUIRED = "QR Code là thông tin bắt buộc.";

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public const string ATHENTICATION_TYPE = "Kiểu xác thực là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng Nhóm thông số
        /// </summary>
        public const string DUPLICATE_PARAMETERGROUP = "Nhóm thông số đã tồn tại trong Cương vị này.";

        /// <summary>
        /// Vai trò người dùng
        /// </summary>
        public const string USER_ROLE_REQUIRED = "Vai trò là thông tin bắt buộc.";

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public const string USERNAME_REQUIRED = "Tên đăng nhập là thông tin bắt buộc.";

        /// <summary>
        /// Họ và tên
        /// </summary>
        public const string FULLNAME_REQUIRED = "Họ và tên là thông tin bắt buộc.";

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public const string PASSWORD_REQUIRED = "Mật khẩu là thông tin bắt buộc.";

        /// <summary>
        /// Mật khẩu phải có độ dài tối thiểu 10 ký tự
        /// </summary>
        public const string PASSWORD_LENGTH = "Mật khẩu phải có độ dài tối thiểu 10 ký tự.";

        /// <summary>
        /// Mật khẩu phải có ít nhất một ký tự viết hoa
        /// </summary>
        public const string PASSWORD_UPPERCASE = "Mật khẩu phải có ít nhất một ký tự viết hoa.";

        /// <summary>
        /// Mật khẩu phải có ít nhất một ký tự viết thường
        /// </summary>
        public const string PASSWORD_LOWERCASE = "Mật khẩu phải có ít nhất một ký tự viết thường.";

        /// <summary>
        /// Mật khẩu phải có ít nhất một ký tự số
        /// </summary>
        public const string PASSWORD_NUMBER = "Mật khẩu phải có ít nhất một ký tự số.";

        /// <summary>
        /// Mật khẩu phải có ít nhất một ký tự đặc biệt
        /// </summary>
        public const string PASSWORD_SPECIAL_CHARACTER = "Mật khẩu phải có ít nhất một ký tự đặc biệt.";

        /// <summary>
        /// Kiểm tra tồn tại người dùng
        /// </summary>
        public const string DUPLICATE_USER = "Người dùng đã tồn tại.";

        /// <summary>
        /// Tên Cương vị
        /// </summary>
        public const string OPERATION_POSITION_NAME_REQUIRED = "Tên Cương vị là thông tin bắt buộc.";

        /// <summary>
        /// Tên Cương vị
        /// </summary>
        public const string PRODUCTION_PLANT_REQUIRED = "Phân xưởng là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng Nhóm thông số
        /// </summary>
        public const string DUPLICATE_OPERATION_POSITION = "Cương vị đã tồn tại trong Nhà máy.";

        /// <summary>
        /// Tên Cương vị trong ca
        /// </summary>
        public const string POSITION_IN_SHIFT_NAME_REQUIRED = "Tên Cương vị trong ca là bắt buộc.";

        /// <summary>
        /// Cương vị trong ca là thông tin bắt buộc
        /// </summary>
        public const string POSITION_IN_SHIFT_REQUIRED = "Cương vị trong ca là thông tin bắt buộc.";

        /// <summary>
        /// Cương vị trong ca không tồn tại trong hệ thống
        /// </summary>
        public const string POSITION_IN_SHIFT_NOT_EXISTS = "Cương vị trong ca không tồn tại trong hệ thống.";

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public const string POSITION_REQUIRED = "Thứ tự sắp xếp là bắt buộc";

        /// <summary>
        /// Cương vị
        /// </summary>
        public const string OPERATION_POSITION_REQUIRED = "Cương vị là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra tồn tại Cương vị trong hệ thống
        /// </summary>
        public const string OPERATION_POSITION_NOT_EXISTS = "Cương vị không tồn tại trong hệ thống.";

        /// <summary>
        /// Định dạng thời gian: ngày/tháng/năm
        /// </summary>
        public const string FORMAT_DATE_DDMMYYYY = "dd/MM/yyyy";

        /// <summary>
        /// Định dạng thời gian: ngày/tháng/năm giờ:phút
        /// </summary>
        public const string FORMAT_DATE_DDMMYYYYHHMM = "dd/MM/yyyy HH:mm";

        /// <summary>
        /// Định dạng thời gian: ngày/tháng/năm giờ:phút:giây
        /// </summary>
        public const string FORMAT_DATE_DDMMYYYYHHMMSS = "dd/MM/yyyy HH:mm:ss";

        /// <summary>
        /// Tên kíp vận hành
        /// </summary>
        public const string OPERATION_CREW_NAME_REQUIRED = "Tên kíp vận hành là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng Kíp vận hành
        /// </summary>
        public const string DUPLICATE_OPERATION_CREW = "Tên Kíp vận hành đã tồn tại trong Nhà máy.";

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public const string OPERATION_CREW_REQUIRED = "Kíp vận hành là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra tồn tại kíp vận hành trong hệ thống
        /// </summary>
        public const string OPERATION_CREW_NOT_EXISTS = "Kíp vận hành không tồn tại trong hệ thống.";

        /// <summary>
        /// Tên chu kỳ
        /// </summary>
        public const string CYCLE_NAME_REQUIRED = "Tên chu kỳ là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra tồn tại chu kỳ
        /// </summary>
        public const string DUPLICATE_CYCLE = "Chu kỳ đã tồn tại trong nhà máy.";

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public const string POISITION_REQUIRED = "Thứ tự sắp xếp là thông tin bắt buộc.";

        /// <summary>
        /// Tên vai trò
        /// </summary>
        public const string ROLE_NAME_REQUIRED = "Tên vai trò là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng tên Vai trò
        /// </summary>
        public const string DUPLICATE_ROLE_NAME = "Tên vai trò đã tồn tại.";

        /// <summary>
        /// Tên ca vận hành
        /// </summary>
        public const string OPERATION_SHIFT_NAME_REQUIRED = "Tên ca vận hành là thông tin bắt buộc.";

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public const string OPERATION_SHIFT_REQUIRED = "Ca vận hành là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm trả tồn tại Ca vận hành trong hệ thống
        /// </summary>
        public const string OPERATION_SHIFT_NOT_EXISTS = "Ca vận hành không tồn tại trong hệ thống.";

        /// <summary>
        /// Thời gian bắt đầu ca
        /// </summary>
        public const string OPERATION_SHIFT_START_TIME = "Thời gian bắt đầu ca là thông tin bắt buộc.";

        /// <summary>
        /// Thời gian kết thúc ca
        /// </summary>
        public const string OPERATION_SHIFT_END_TIME = "Thời gian kết thúc ca là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng ca vận hành
        /// </summary>
        public const string DUPLICATE_OPERATION_SHIFT = "Ca vận hành đã tồn tại trong nhà máy.";

        #region Nhật ký vận hành

        /// <summary>
        /// Có lỗi xảy ra khi lưu nhật ký vận hành
        /// </summary>
        public const string OPERATION_DIARY_COMMON = "Có lỗi xảy ra khi thao tác Nhật ký vận hành. Vui lòng liên hệ Admin để được hỗ trợ.";

        /// <summary>
        /// Tồn tại nhật ký vận hành trong ngày cùng ca vận hành
        /// </summary>
        public const string DUPLICATE_OPERATION_DIARY = "Nhật ký vận hành đã tồn tại.";

        /// <summary>
        /// Ngày ký
        /// </summary>
        public const string SIGNATUREDATE = "Ngày tạo nhật ký vận hành là thông tin bắt buộc.";

        /// <summary>
        /// Nhân viên thực hiện là thông tin bắt buộc
        /// </summary>
        public const string ASSIGNED_USER_REQUIRED = "Nhân viên thực hiện là thông tin bắt buộc.";

        /// <summary>
        /// Nhân viên thực hiện không tồn tại trong hệ thống
        /// </summary>
        public const string ASSIGNED_USER_NOT_EXISTS = "Nhân viên không tồn tại trong hệ thống.";

        /// <summary>
        /// Kiểm tra nhân viên nhân/giao ca
        /// </summary>
        public const string RECEIVE_DELIVERY_USER = "Nhân viên nhận ca không được trùng với nhân viên giao ca.";

        /// <summary>
        /// Trùng nhân viên nhân ca
        /// </summary>
        public const string RECEIVE_DUPLICATE_USER = "Nhân viên nhận ca không được phép trùng nhau.";

        /// <summary>
        /// Nhân viên giao ca không trùng với nhân viên được khái báo 
        /// </summary>
        public const string DELIVERY_NOTMAP_USER_DEFINE = "Nhân viên ký giao ca không trùng với nhân viên được khai báo khi tạo nhật ký vận hành.";

        /// <summary>
        /// Kiểm tra password
        /// </summary>
        public const string PASSWORD_INVALID = "Mật khẩu nhân viên không đúng.";

        /// <summary>
        /// Nhận ca
        /// </summary>
        public const string RECEIVED_COMPLETED = "Đã thực hiện nhận ca.";

        /// <summary>
        /// Giao ca
        /// </summary>
        public const string DELIVERED_COMPLETED = "Đã thực hiện giao ca.";

        /// <summary>
        /// Đã ký nhận ca
        /// </summary>
        public const string USER_RECEIVED_COMPLETED = "Người dùng đã ký nhận ca.";

        /// <summary>
        /// Đã ký giao ca
        /// </summary>
        public const string USER_DELIVERED_COMPLETED = "Người dùng đã ký giao ca.";

        /// <summary>
        /// Ca vận hành không tồn tại
        /// </summary>
        public const string OPERATION_DIARY_NOT_EXISTS = "Ca vận hành không tồn tại trong hệ thống.";

        /// <summary>
        /// Có lỗi xảy ra khi lưu nhật ký vận hành
        /// </summary>
        public const string PROBATIONARY_STAFF_COMMON = "Có lỗi xảy ra khi cập nhật nhân viên tập sự. Vui lòng liên hệ Admin để được hỗ trợ.";

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public const string TWO_SYSTEM = "Hai hệ thống";

        /// <summary>
        /// 
        /// </summary>
        public const string ADMIN_PORTAL = "Admin Portal";

        /// <summary>
        /// 
        /// </summary>
        public const string MOBILE_APP = "Mobile App";

        /// <summary>
        /// 
        /// </summary>
        public const string AD = "AD";

        /// <summary>
        /// Kiểm tra Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc
        /// </summary>
        public const string INVALID_TIME = "Thời gian kết thúc không được lớn hơn thời gian bắt đầu.";

        /// <summary>
        /// Thời gian ca vận hành của nhà máy này đang được sử dụng
        /// </summary>
        public const string TIME_IS_BEING_USED = "Thời gian ca vận hành trùng với ca khác đang sử dụng trong nhà máy.";

        /// <summary>
        /// Định dạng thời gian chưa đúng
        /// </summary>
        public const string FORMAT_TIME_INVALID = "Thời gian phải hợp lệ và kiểu string có định dạng là: hh:mm";

        /// <summary>
        /// Mật khẩu không đúng
        /// </summary>
        public const string OLD_PASSWORD_INVALID = "Mật khẩu cũ không đúng";

        /// <summary>
        /// Số phiếu đăng ký
        /// </summary>
        public const string TICKET_NUMBER = "Số phiếu đăng ký công tác là thông tin bắt buộc.";

        /// <summary>
        /// đơn vị nhận phiếu
        /// </summary>
        public const string RECEIVE_TICKET_UNIT = "Đơn vị nhận phiếu là thông tin bắt buộc.";

        /// <summary>
        /// Căn cứ biên bản số
        /// </summary>
        public const string DOCUMENT_TICKET = "Căn cứ biên bản số là thông tin bắt buộc.";

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public const string PRODUCTION_PLANT = "Đơn vị công tác đăng ký là thông tin bắt buộc.";

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public const string WORK_CONTENT = "Nội dung công việc là thông tin bắt buộc.";

        /// <summary>
        /// Địa điểm (hoặc thiết bị) tiến hành công tác
        /// </summary>
        public const string WORK_ADDRESS = "Địa điểm (hoặc thiết bị) tiến hành công tác là thông tin bắt buộc.";

        /// <summary>
        /// Điều kiện cần có để hoàn thành công việc
        /// </summary>
        public const string CONDITION_TO_WORK = "Điều kiện cần có để hoàn thành công việc là thông tin bắt buộc";

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public const string START_DATE = "Thời gian tiến hành công việc (bắt đầu) là thông tin bắt buộc";

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public const string END_DATE = "Thời gian tiến hành công việc (kết thúc) là thông tin bắt buộc";

        /// <summary>
        /// Thời gian bắt đầu hằng ngày
        /// </summary>
        public const string START_TIME_DAILY = "Thời gian hằng ngày (bắt đầu) là thông tin bắt buộc";

        /// <summary>
        /// Thời gian kết thúc hằng ngày
        /// </summary>
        public const string END_TIME_DAILY = "Thời gian hằng ngày (kết thúc) là thông tin bắt buộc";

        /// <summary>
        /// Kiểm tra số lượng nhân viên
        /// </summary>
        public const string UNIT_NUMBER_AND_UNIT_STAFFS = "Số lượng nhân viên bạn khai báo ở mục 11 ({0} người) không khớp với số lượng nhân viên bạn nhập ở mục 6 ({1} người)";

        /// <summary>
        /// Kiểm tra nhân viên có đang được chọn ở phiếu công tác khác
        /// </summary>
        public const string USER_IN_USE = "Nhân viên đang được chọn ở phiếu công tác khác";

        /// <summary>
        /// Số lượng đơn vị công tác
        /// </summary>
        public const string POSITION_QUANTITY_COUNT = "Số lượng đơn vị công tác phải lớn hơn hoặc bằng 1";

        /// <summary>
        /// Số lượng đơn vị công tác
        /// </summary>
        public const string POSITION_QUANTITYS = "Số lượng nhân viên đơn vị công tác là thông tin bắt buộc";

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public const string COMMANDER_USER = "Người chỉ huy trực tiếp là thông tin bắt buộc";

        /// <summary>
        /// Danh sách những người được cử để thực hiện công việc
        /// </summary>
        public const string WORKER_USER = "Danh sách những người được cử để thực hiện công việc là thông tin bắt buộc";

        /// <summary>
        /// Kiểm tra trùng Số phiếu đăng ký công tác
        /// </summary>
        public const string DUPLICATE_REGISTRAITON_TICKET = "Số phiếu đăng ký công tác '{0}' đã tồn tại.";

        /// <summary>
        /// Kiểm tra trùng Căn cứ biên bản số
        /// </summary>
        public const string REGULATION_TICKET = "Căn cứ biên bản số đã tồn tại.";

        /// <summary>
        /// Dữ liệu không tồn tại
        /// </summary>
        public const string DATA_DOES_NOT_EXIST = "Dữ liệu không tồn tại";

        /// <summary>
        /// Đã cấp phiếu công tác
        /// </summary>
        public const string APPROVED_TICKET = "Đã cấp phiếu công tác";

        /// <summary>
        /// Kế hoạch
        /// </summary>
        public const string TICKET_INSIDEPLAN = "Kế hoạch";

        /// <summary>
        /// Ngoài kế hoạch
        /// </summary>
        public const string TICKET_OUTSIDEPLAN = "Ngoài kế hoạch";

        /// <summary>
        /// Xử lý sự cố
        /// </summary>
        public const string TICKET_SPECIALCASE = "Xử lý sự cố";

        /// <summary>
        /// Phiếu đăng ký công tác là thông tin bắt buộc
        /// </summary>
        public const string REGISTRAITON_TICKET_REQUIRED = "Phiếu đăng ký công tác là thông tin bắt buộc.";

        /// <summary>
        /// Nội dung công tác là thông tin bắt buộc
        /// </summary>
        public const string CONTENT_OF_WORK_REQUIRED = "Nội dung công tác là thông tin bắt buộc.";

        /// <summary>
        /// số phiếu đăng ký công tác bắt buộ
        /// </summary>
        public const string NUMBER_WORD_TICKET_REQUIRED = "Số phiếu đăng ký công tác bắt buộc";

        /// <summary>
        /// Người chỉ huy trực tiếp bắt buộc
        /// </summary>
        public const string COMMANDER_REQUIRED = "Người chỉ huy trực tiếp lfa thông tin bắt buộc.";

        /// <summary>
        /// điều kiện tiến hành công việc bắt buộc
        /// </summary>
        public const string CONDITION_TO_WORK_REQUIRED = "Điều kiện tiến hành công việc bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng phiếu công tác
        /// </summary>
        public const string DUPLICATE_WORK_TICKET = "Phiếu công tác đã tồn tại.";

        /// <summary>
        /// Người cho phép là bắt buộc
        /// </summary>
        public const string ACCEPTOR_REQUIRED = "Người cho phép là thông tin bắt buộc.";

        /// <summary>
        /// Địa điểm tiến hành công tác là thông tin bắt buộc
        /// </summary>
        public const string WORK_PLACEMENT_REQUIRED = "Địa điểm tiến hành công tác là thông tin bắt buộc.";

        /// <summary>
        /// Người cấp phiếu là bắt buộc
        /// </summary>
        public const string APPROVER_REQUIRED = "Người cấp phiếu là thông tin bắt buộc.";

        /// <summary>
        /// Người đại diện đơn vị quản lý là bắt buộc
        /// </summary>
        public const string REPRESENTATIVE_UNIT_REQUIRED = "Người đại diện đơn vị quản lý là bắt buộc.";

        /// <summary>
        /// Có lỗi xảy ra khi lưu phiếu đăng ký công tác
        /// </summary>
        public const string REGISTRATION_TICKET_COMMON = "Có lỗi xảy ra khi lưu phiếu đăng ký công tác. Kiểm tra log để biết nguyên nhân.";

        /// <summary>
        /// Có lỗi xảy ra khi lưu phiếu thao tác
        /// </summary>
        public const string MANIPULATION_TICKET_COMMON = "Có lỗi xảy ra khi lưu phiếu thao tác. Kiểm tra log để biết nguyên nhân.";

        /// <summary>
        /// Có lỗi xảy ra khi lưu phiếu thao tác mẫu
        /// </summary>
        public const string MANIPULATION_TEMPLATE_TICKET_COMMON = "Có lỗi xảy ra khi lưu phiếu thao tác mẫu. Kiểm tra log để biết nguyên nhân.";

        /// <summary>
        /// Tên {0} là thông tin bắt buộc
        /// </summary>
        public const string ORGANIZATION_NAME = "Tên {0} là thông tin bắt buộc.";

        /// <summary>
        /// Tên viết tắt của {0} là thông tin bắt buộc.
        /// </summary>
        public const string ORGANIZATION_SHORTNAME = "Tên viết tắt của {0} là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra tồn tại dữ liệu
        /// </summary>
        public const string DUPLICATE_ORGANIZATION = "{0} đã tồn tại trong hệ thống.";

        /// <summary>
        /// Kiểm tra trùng Thông số
        /// </summary>
        public const string DUPLICATE_PARAMETER = "Thông số đã tồn tại.";

        /// <summary>
        /// Tên thông số
        /// </summary>
        public const string PARAMETERNAME_REQUIRED = "Tên thông số là thông tin bắt buộc.";

        /// <summary>
        /// Mã KKS
        /// </summary>
        public const string KKSCODE_REQUIRED = "Mã KKS là thông tin bắt buộc.";

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public const string UNITMEASURE_REQUIRED = "Đơn vị đo là thông tin bắt buộc.";

        /// <summary>
        /// lý do từ chối là thông tin bắt buộc
        /// </summary>
        public const string REASON_REQUIRED = "Lý do từ chối là thông tin bắt buộc";

        /// <summary>
        /// Nhóm thông số là bắt buộc
        /// </summary>
        public const string PARAMETER_GROUP_REQUIRED = "Nhóm thông số là bắt buộc";

        /// <summary>
        /// Khung giờ là bắt buộc
        /// </summary>
        public const string OPERATION_TIMELINEID_REQUIRED = "Khung giờ là bắt buộc";

        /// <summary>
        /// Thông số
        /// </summary>
        public const string PARAMETER_REQUIRED = "Thông số là thông tin bắt buộc.";

        /// <summary>
        /// Địa điểm/Thiết bị
        /// </summary>
        public const string LOCATION_DEVICE = "Địa điểm/Thiết bị là thông tin bắt buộc";

        /// <summary>
        /// Nhận diện mối nguy và biện pháp an toàn
        /// </summary>
        public const string IDENTIFICATION_SAFETY = "Nhận diện mối nguy và biện pháp an toàn là thông tin bắt buộc";

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public const string WORKER_USERS = "Danh sách người tham gia công việc là thông tin bắt buộc";

        /// <summary>
        /// Nhân viên giao ca không trùng với danh sách nhân viên trong cương vị
        /// </summary>
        public const string USER_DELIVERY_UNMATCH = "Nhân viên giao ca không trùng với danh sách nhân viên trong cương vị.";

        /// <summary>
        /// Mã xác thực truy cập API
        /// ELOGBOOK_API_KEY_VERSION: RUxPR0JPT0tfQVBJX0tFWV9WRVJTSU9O
        /// </summary>
        public const string API_KEY = "RUxPR0JPT0tfQVBJX0tFWV9WRVJTSU9O";

        /// <summary>
        /// API Key không tồn tại trong request
        /// </summary>
        public const string API_KEY_ERROR = "Thông tin API Key không đúng trong request.";

        /// <summary>
        /// Ngày ghi thông số
        /// </summary>
        public const string CHECKED_DATE = "Ngày ghi thông số là thông tin bắt buộc";

        /// <summary>
        /// Theo dõi tồn tại trong ca
        /// </summary>
        public const string ISSUE_OF_SHIFT_COMMON = "Có lỗi xảy ra khi thao xử lý theo dõi tồn tại trong ca. Vui lòng liên hệ Admin để được hỗ trợ.";

        /// <summary>
        /// Nhóm thông số này đã được sử dụng, không thể xóa.
        /// </summary>
        public const string MSG_CANNOT_DELETE_PARAMETER_GROUP = "Nhóm thông số này đã được sử dụng, không thể xóa.";

        /// <summary>
        /// Cương vị này đang được sử dụng không thể xóa.
        /// </summary>
        public const string MSG_CANNOT_DELETE_OPERATION_POSITION = "Cương vị này đang được sử dụng không thể xóa.";

        /// <summary>
        /// Cương vị trong ca này đang được sử dụng không thể xóa.
        /// </summary>
        public const string MSG_CANNOT_DELETE_POSITION_IN_SHIFT = "Cương vị trong ca này đang được sử dụng không thể xóa.";

        /// <summary>
        /// Kíp vận hành này đã được sử dụng không thể xóa.
        /// </summary>
        public const string MSG_CANNOT_DELETE_OPERATION_CREW = "Kíp vận hành này đã được sử dụng không thể xóa.";

        /// <summary>
        /// Phân xưởng này đã được sử dụng không thể xóa.
        /// </summary>
        public const string MSG_CANNOT_DELETE_PRODUCTION_PLANT = "Phân xưởng này đã được sử dụng không thể xóa.";

        /// <summary>
        /// Tài khoản này đang được sử dụng không thể xóa.
        /// </summary>
        public const string MSG_CANNOT_DELETE_USER = "Tài khoản này đang được sử dụng không thể xóa.";

        /// <summary>
        /// Tên người dùng hoặc mật khẩu không đúng.
        /// </summary>
        public const string INVALID_USER_PASSWORD = "Tên người dùng hoặc mật khẩu không đúng.";

        /// <summary>
        /// số phiếu đăng ký công tác đã tồn tại
        /// </summary>
        public const string NUMBER_WORD_TICKET_EXIST = "Số phiếu công tác đã tồn tại.";

        /// <summary>
        /// Người dùng đã tồn tại trong hệ thống
        /// </summary>
        public const string DUPLICATE_GUEST_USER = "Người dùng đã tồn tại trong hệ thống.";

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public const string DUPLICATE_NATIONALID = "Số CMND/CCCD đã tồn tại.";

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public const string DUPLICATE_ELECTRICRANKCARD = "Thẻ an toàn điện đã tồn tại.";

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public const string DUPLICATE_LABORCARD = "Thẻ an toàn lao động đã tồn tại.";

        /// <summary>
        /// Email đã tồn tại
        /// </summary>
        public const string DUPLICATE_EMAIL = "Email đã tồn tại.";

        /// <summary>
        /// Lỗi chung khi xử lý người ngoài hệ thống
        /// </summary>
        public const string GUESTUSER_COMMON_ERROR = "Có lỗi xảy ra khi thao xử lý người ngoài hệ thống. Vui lòng liên hệ Admin để được hỗ trợ.";

        /// <summary>
        /// Xóa dữ liệu đang sử dụng
        /// </summary>
        public const string COMMON_DELETE_INUSE_DATA = "Không thể xáo dữ liệu dang sử dụng.";

        /// <summary>
        /// Phạm vi là bắt buộc
        /// </summary>
        public const string WORK_SCOPE_REQUIRED = "Phạm vi công tác là bắt buộc.";

        /// <summary>
        /// Tính năng là bắt buộc
        /// </summary>
        public const string WORK_TICKET_FEATURE_REQUIRED = "Tính năng là bắt buộc. (Thủy, Cơ, Nhiệt, Hóa)";

        /// <summary>
        /// Đơn vị cấp phiếu là bắt buộc
        /// </summary>
        public const string APPROVE_UNIT_REQUIRED = "Đơn vị cấp phiếu là bắt buộc";

        /// <summary>
        /// Loại phiếu là bắt buộc
        /// </summary>
        public const string TICKET_TYPE_REQUIRED = "Loại phiếu là bắt buộc (Kế hoạch, Ngoài kế hoạch, Xử lý sự cố)";

        /// <summary>
        /// Số lượng người là bắt buộc
        /// </summary>
        public const string AMOUNT_PERSON_REQUIRED = "Số lượng người là bắt buộc";

        /// <summary>
        /// Đơn vị đang được xử lý
        /// </summary>
        public const string CHECK_STATUS_USE_ORGANIZATION = "Bạn không thể xoá dữ liệu này vì nó đang được sử dụng.";

        /// <summary>
        /// Tên phiếu thao tác là bắt buộc
        /// </summary>
        public const string MANIPULATION_TICKET_NAME = "Tên phiếu thao tác là bắt buộc.";

        /// <summary>
        /// Tên phiếu thao tác mẫu là bắt buộc
        /// </summary>
        public const string MANIPULATION_TEMPLATE_TICKET_NAME = "Tên phiếu thao tác mẫu là bắt buộc.";

        /// <summary>
        /// Người duyệt phiếu là bắt buộc
        /// </summary>
        public const string APPROVER_USER = "Người duyệt phiếu là bắt buộc.";

        /// <summary>
        /// Người duyệt phiếu là bắt buộc
        /// </summary>
        public const string SUPPERVISOR_USER = "Người giám sát là bắt buộc.";

        /// <summary>
        /// Người duyệt phiếu là bắt buộc
        /// </summary>
        public const string WORKERS_USER = "Người thao tác là bắt buộc.";

        /// <summary>
        /// Thời gian dự kiến (bắt đầu)
        /// </summary>
        public const string START_DATE_EXPECTED = "Thời gian dự kiến (bắt đầu) là thông tin bắt buộc";

        /// <summary>
        /// Thời gian dự kiến (kết thúc)
        /// </summary>
        public const string END_DATE_EXPECTED = "Thời gian dự kiến (kết thúc) là thông tin bắt buộc";

        /// <summary>
        /// Kiểm tra trùng tên phiếu thao tác
        /// </summary>
        public const string DUPLICATE_MANIPULATION_TICKET_NAME = "Tên phiếu thao tác đã tồn tại.";

        /// <summary>
        /// Kiểm tra phiếu thao tác mẫu
        /// </summary>
        public const string DUPLICATE_MANIPULATION_TEMPLATE_TICKET_NAME = "Phiếu thao tác mẫu đã tồn tại.";

        /// <summary>
        /// Kiểm tra trùng số phiếu thao tác
        /// </summary>
        public const string DUPLICATE_MANIPULATION_TICKET_NUMBER = "Số phiếu thao tác đã tồn tại.";

        /// <summary>
        /// Giao nhận nghiệm thu thiết bị là cần thiết
        /// </summary>
        public const string DELIVERY_ACCEPDEVICES = "Giao nhận nghiệm thu thiết bị là cần thiết";

        /// <summary>
        /// Thông tin người ký là bắt buộc
        /// </summary>
        public const string SIGNER_INFO = "Thông tin người ký là bắt buộc";

        /// <summary>
        /// Thông tin người ký là bắt buộc
        /// </summary>
        public const string ID_MANIPULATION_TICKET = "ID phiếu thao tác là bắt buộc";

        /// <summary>
        /// Key xác định dữ liệu thuộc nhóm
        /// </summary>
        public const int PARAMETERTYPE_GROUP = 1;

        /// <summary>
        /// Key xác định dữ liệu không thuộc nhóm thông số
        /// </summary>
        public const int PARAMETERTYPE_NOGROUP = 2;

        /// <summary>
        /// Key để so sánh trong các điều kiện
        /// </summary>
        public const int ZERO_KEY_COMPARE = 0;

        /// <summary>
        /// Trạng thái đã tồn tại trong cùng loại dữ liệu
        /// </summary>
        public const string DUPLICATE_STATUSCATEGORY = "Trạng thái đã tồn tại";

        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public const string STATUSNAME_NAME_REQUIRED = "Tên trạng thái là thông tin bắt buộc";

        /// <summary>
        /// Loại dữ liệu trạng thái
        /// </summary>
        public const string DATATYPE_REQUIRED = "Loại dữ liệu trạng thái là thông tin bắt buộc";

        /// <summary>
        /// Theo dõi bất thường sự cố
        /// </summary>
        public const string ABNORMAL_INCIDENT_COMMON = "Có lỗi xảy ra khi thao xử lý theo bất thường sự cố. Vui lòng liên hệ Admin để được hỗ trợ.";

        /// <summary>
        /// Xác định tài khoản ký cấp/khóa phiếu công tác bằng username/password
        /// </summary>
        public const string SIGNING_USERNAME_PASSWORD = "Tài khoản người dùng";

        /// <summary>
        /// Xác định ký cấp/khóa phiếu công tác bằng Viettel MobileCA
        /// </summary>
        public const string SIGNING_VIETTEL_CA = "VIETTEL - CA";

        /// <summary>
        /// Xác định ký cấp/khóa phiếu công tác bằng VNPT SmartCA
        /// </summary>
        public const string SIGNING_VNPT_SMARTCA = "VINA PHONE - CA";

        /// <summary>
        /// Xác định ký cấp/khóa phiếu công tác bằng Viettel MySign (bất đồng bộ)
        /// </summary>
        public const string SIGNING_VIETTEL_MYSIGN = "VIETTEL - MYSIGN";

        /// <summary>
        /// Encrypt-Decrypt
        /// </summary>
        public const string CRYPT_KEY = "sblw-3hn8-sqoy19";

        /// <summary>
        /// Loại sự kiện
        /// </summary>
        public const string TICKETTYPE_REQUIRED = "Loại sự kiện là thông tin bắt buộc.";

        /// <summary>
        /// Nội dung sự kiện
        /// </summary>
        public const string TICKET_CONTENT_REQUIRED = "Nội dung sự kiện là thông tin bắt buộc.";

        /// <summary>
        /// Nội dung sự kiện
        /// </summary>
        public const string DUPLICATE_TICKET_EVENT = "Sự kiện đã tồn tại trong Nhà máy.";

        /// <summary>
        /// Phiếu thao tác
        /// </summary>
        public const string PTT = "PTT";

        /// <summary>
        /// Phiếu công tác Thủy Cơ Nhiệt Hóa
        /// </summary>
        public const string PCTTCNH = "PCTTCNH";

        /// <summary>
        /// Phiếu công tác Điện
        /// </summary>
        public const string PCTD = "PCTD";

        /// <summary>
        /// Format Phiếu thao tác mặc định
        /// </summary>
        public const string PTT_FORMAT_DEFAULT = "1,6,2,6,3,6,4,7,5";

        /// <summary>
        /// Format Phiếu công tác Thủy Cơ Nhiệt Hóa mặc định
        /// </summary>
        public const string PCTTCNH_FORMAT_DEFAULT = "1,6,2,6,3,7,4,7,5";

        /// <summary>
        /// Format Phiếu công tác Điện mặc định
        /// </summary>
        public const string PCTD_FORMAT_DEFAULT = "1,6,2,6,3,7,4,7,5";

        /// <summary>
        /// Kí tự /
        /// </summary>
        public const string SLASH = "/";

        /// <summary>
        /// Kí tự -
        /// </summary>
        public const string HYPHEN = "-";

        /// <summary>
        /// Phiếu công tác là bắt buộc
        /// </summary>
        public const string WORK_TICKET_REQUIRED = "Phiếu công tác là bắt buộc";

        /// <summary>
        /// Người cần thay đổi cho phép là bắt buộc
        /// </summary>
        public const string NEW_ALLOWR_USER_REQUIRED = "Người cần thay đổi cho phép là bắt buộc";

        /// <summary>
        /// Người yêu cầu thay đôi người cho phép
        /// </summary>
        public const string REQUESTER_USER_REQUIRED = "Người yêu cầu người cho phép là bắt buộc";

        /// <summary>
        /// Loại phiếu thông tin sửa chữa lớn
        /// </summary>
        public const string MAJOR_REPAIR_TYPE_REQUIRED = "Loại phiếu thông tin sửa chữa lớn là bắt buộc";

        /// <summary>
        /// Title
        /// </summary>
        public const string TITLE_REQUIRED = "Title là bắt buộc";

        /// <summary>
        /// Duplicate thông tin sửa chữa lớn
        /// </summary>
        public const string DUPLICATE_MAJOR_REPAIR = "Thông tin sửa chữa lớn đã tồn tại";

        /// <summary>
        /// Không thể update SCL đã hoặc đang tạo work ticket
        /// </summary>
        public const string MAJOR_REPAIR_NOT_AVAILABLE_UPDATE = "Sữa chữa lớn không thể sửa, quá trình tạo phiếu đang diễn ra";

        /// <summary>
        /// Mã thông tin sửa chữa lớn
        /// </summary>
        public const string MAJOR_REPAIR_ID_REQUIRED = "Mã thông tin sửa chữa lớn là bắt buộc";

        /// <summary>
        /// Format excel file
        /// </summary>
        public const string XLSX_EXTENTION_REQUIRED = "Vui lòng upload file excel có định dạng xlsx";

        /// <summary>
        /// Batch size
        /// </summary>
        public const int BATCH_SIZE = 100;

        /// <summary>
        /// Sửa chữa lớn
        /// </summary>
        public const string MAJOR_REPAIR = "MajorRepair";

        /// <summary>
        /// Tên đợt công tác bắt buộc
        /// </summary>
        public const string MAJOR_REPAIR_NAME_REQUIRED = "Tên đợt công tác là bắt buộc";

        /// <summary>
        /// File excel đính kèm là bắt buộc
        /// </summary>
        public const string ATTACHTMENT_EXECL = "File excel đính kèm là bắt buộc";

        /// <summary>
        /// Dữ liệu excel là bắt buộc
        /// </summary>
        public const string DATA_EXCEL_REQUIRED = "Dữ liệu excel là bắt buộc";

        /// <summary>
        /// Thời gian tạo phiếu
        /// </summary>
        public const string PLAN_CREATE_TICKET_TIME = "Thời gian tạo phiếu công tác phải lớn hơn thời gian hiện tại.";

        /// <summary>
        /// ID thiết bị là bắt buộc
        /// </summary>
        public const string DEVICE_ID_REQUIRED = "ID thiết bị là bắt buộc";

        /// <summary>
        /// Tên loại thiết bị là bắt buộc
        /// </summary>
        public const string DEVICE_TYPE_NAME_REQUIRED = "Tên loại thiết bị là bắt buộc";

        /// <summary>
        /// Mã loại thiết bị là bắt buộc
        /// </summary>
        public const string DEVICE_TYPE_CODE_REQUIRED = "Mã loại thiết bị là bắt buộc";

        /// <summary>
        /// ID loại thiết bị là bắt buộc
        /// </summary>
        public const string DEVICE_TYPE_ID_REQUIRED = "ID loại thiết bị là bắt buộc";

        /// <summary>
        /// ID khu vực là bắt buộc
        /// </summary>
        public const string LOCATION_ID_REQUIRED = "ID khu vực là bắt buộc";

        /// <summary>
        /// Mã thiết bị là bắt buộc
        /// </summary>
        public const string DEVICE_CODE_REQUIRED = "Mã thiết bị là bắt buộc";

        /// <summary>
        /// Tên khu vực là bắt buộc
        /// </summary>
        public const string LOCATION_NAME_REQUIRED = "Tên khu vực là bắt buộc";

        /// <summary>
        /// ID loại công việc là bắt buộc
        /// </summary>
        public const string WORK_TYPE_ID_REQUIRED = "ID loại công việc là bắt buộc";

        /// <summary>
        /// Tên loại công việc là bắt buộc
        /// </summary>
        public const string WORK_TYPE_NAME_REQUIRED = "Tên loại công việc là bắt buộc";

        /// <summary>
        /// ID loại công việc là bắt buộc
        /// </summary>
        public const string WORK_ID_REQUIRED = "ID công việc là bắt buộc";

        /// <summary>
        /// Tên loại công việc là bắt buộc
        /// </summary>
        public const string WORK_NAME_REQUIRED = "Tên công việc là bắt buộc";

        /// <summary>
        /// Loại công việc là bắt buộc
        /// </summary>
        public const string WORK_TYPE_REQUIRED = "Loại công việc là bắt buộc.";

        /// <summary>
        /// Mô tả công việc là bắt buộc
        /// </summary>
        public const string WORK_DESC_REQUIRED = "Mô tả công việc là bắt buộc.";

        /// <summary>
        /// Tên lịch bảo trì là bắt buộc
        /// </summary>
        public const string MAINTENANCE_SCHEDULE_NAME_REQUIRED = "Tên lịch bảo trì là bắt buộc";

        /// <summary>
        /// Danh sách công việc là bắt buộc
        /// </summary>
        public const string LIST_MAINTENANCE_WORK_REQUIRED = "Danh sách công việc là bắt buộc";

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public const string START_TIME_REQUIRED = "Thời gian bắt đầu là bắt buộc";

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public const string END_TIME_REQUIRED = "Thời gian kết thúc là bắt buộc";

        /// <summary>
        /// Schedule là bắt buộc
        /// </summary>
        public const string SCHEDULE_TYPE_REQUIRED = "Schedule là bắt buộc";

        /// <summary>
        /// Nguyên nhân là bắt buộc
        /// </summary>
        public const string REASON_STOP_REQUIRED = "Tên nguyên nhân là bắt buộc";

        /// <summary>
        /// mức độ khẩn cấp
        /// </summary>
        public const string EMERGENCY_MANAGEMENT_REQUIRED = "Mức độ khẩn cấp là thông tin bắt buộc.";

        /// <summary>
        /// Tên mức độ khẩn cấp
        /// </summary>
        public const string EMERGENCY_MANAGEMENT_NAME_REQUIRED = "Tên Mức độ khẩn cấp là thông tin bắt buộc.";

        /// <summary>
        /// Thông tin chữ lý là bắt buộc
        /// </summary>
        public const string SIGNATURE_INFO_REQUIRED = "Thông tin chữ lý là bắt buộc";

        /// <summary>
        /// Lịch lịch bảo trì này đã tồn tại dữ liệu tạm dừng.
        /// </summary>
        public const string DUPLICATE_AM_PENDING = "Lịch lịch bảo trì này đã tồn tại dữ liệu tạm dừng.";

        /// <summary>
        /// ID thực hiện AM
        /// </summary>
        public const string AM_WORKING_REQUIRED = "ID thực hiện AM";

        /// <summary>
        /// ID lý do không thực hiện là bắt buộc
        /// </summary>
        public const string AM_REASON_STOP_REQUIRED = "ID lý do không thực hiện là bắt buộc";

        /// <summary>
        /// ID bảo trì tự quản là bắt buộc
        /// </summary>
        public const string AM_REQUIRED = "ID bảo trì tự quản là bắt buộc";

        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        public const string AUTONOMOUS_MAINTENANCE = "AutonomousMaintenance";

        /// <summary>
        /// Thông tin chi tiết công việc là bắt buộc
        /// </summary>
        public const string AM_WORKDETAIL_REQUIRED = "Thông tin chi tiết công việc là bắt buộc";

        /// <summary>
        /// Thời gian thực hiện là bắt buộc
        /// </summary>
        public const string ACTUAL_DATE_REQUIRED = "Thời gian thực hiện là bắt buộc";

        /// <summary>
        /// Thông tin chi tiết công việc là bắt buộc
        /// </summary>
        public const string STCIK_TIME_REQUIRED = "Thời gian đánh dấu tạm dừng là bắt buộc";

        /// <summary>
        /// Loại nguyên nhân
        /// </summary>
        public const string REASON_TYPE_REQUIRED = "Loại nguyên nhân không hợp lệ.";
        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        public const string REQUEST_CONTENT_REQUIRED = "Nội dung yêu cầu là thông tin bắt buộc.";

        /// <summary>
        /// Kiểm tra trùng Mức độ khẩn cấp
        /// </summary>
        public const string DUPLICATE_REQUEST_TICKET = "Phiếu yêu cầu đã tồn tại trong Nhà máy.";

        /// <summary>
        /// ID Tình trạng thiết bị là bắt buộc
        /// </summary>
        public const string DEVICE_STATUS_ID_REQUIRED = "ID tình trạng thiết bị là bắt buộc";

        /// <summary>
        /// Số file hình ảnh không được vượt quá 5
        /// </summary>
        public const string IMAGE_LIMITED = "Số file hình ảnh không được vượt quá 5";

        /// <summary>
        /// Mã khu vực là bắt buộc
        /// </summary>
        public const string LOCATION_CODE_REQUIRED = "Mã khu vực là bắt buộc";

        /// <summary>
        /// Tên phân xưởng or công ty
        /// </summary>
        public const string ORGANIZATION_ID_REQUIRED = "{0} là thông tin bắt buộc.";

        /// <summary>
        /// Đang xử lý đồng bộ dữ liệu
        /// </summary>
        public const string MSG_SYNC_DATA_PMIS = "Đồng bộ dữ liệu thành công.";

        /// <summary>
        /// {0} này đang được sử dụng không thể xóa
        /// </summary>
        public const string MSG_WARNING_DELETE = "{0} này đang được sử dụng không thể xóa";

        /// <summary>
        /// Xóa {0} thành công
        /// </summary>
        public const string MSG_DELETE_SUCCESS = "Xóa {0} thành công";

        /// <summary>
        /// Xóa {0} thất bại
        /// </summary>
        public const string MSG_DELETE_FAILD = "Xóa {0} thất bại";

        /// <summary>
        /// {0} không tồn tại
        /// </summary>
        public const string MSG_DOES_NOT_EXISTS = "{0} này không tồn tại";

        /// <summary>
        /// loại bảo trì tự quản
        /// </summary>
        public const string TYPE_BAO_TRI_TU_QUAN = "BAOTRI/TUQUAN";

        /// <summary>
        /// bảo trì tự quản
        /// </summary>
        public const string BAO_TRI_TU_QUAN = "Bảo trì tự quản";

        /// <summary>
        /// Link truy cập tác vụ AM
        /// </summary>
        public const string AM_ROUTERLINK = "/autonomous-maintenance/detail/";

        /// <summary>
        /// sữa chữa/công việc lớn
        /// </summary>
        public const string SUA_CHUA_CONG_VIEC = "Sửa chữa/công việc";

        /// <summary>
        /// Cron Schedule
        /// </summary>
        public const string AM_CRON_SCHEDULE_REQUIRED = "Cron schedule config là bắt buộc.";

        /// <summary>
        /// ID quản lý lịch là bắt buộc
        /// </summary>
        public const string MAINTENANCE_SCHEDULE_ID_REQUIRED = "ID quản lý lịch là bắt buộc";

        /// <summary>
        /// Lỗi lịch sử đồng bộ
        /// </summary>
        public const string JOB_HISTORY_COMMON = "Có lỗi xảy ra khi thao xử lý lịch sử đồng bộ. Vui lòng liên hệ Admin để được hỗ trợ.";
        /// <summary>
        /// Lỗi báo cáo
        /// </summary>
        public const string INCIDENT = "Có lỗi xảy ra khi thao tác. Vui lòng liên hệ Admin để được hỗ trợ.";

        /// <summary>
        /// ID người hoàn thành
        /// </summary>
        public const string COMPLETE_USERID_REQUIRED = "UserID người hoàn thành là bắt buộc";

        /// <summary>
        /// ID người thực hiện
        /// </summary>
        public const string IMPLEMENT_USERID_REQUIRED = "UserID người thực hiện là bắt buộc";

        /// <summary>
        /// {0} cha không thể là trùng với đối tượng được cập nhật
        /// </summary>
        public const string MSG_DUPLICATE_PARENT_ID = "{0} cha không thể là trùng với đối tượng được cập nhật";

        /// <summary>
        /// tên phân loại thiết bị
        /// </summary>
        public const string NAME_DEVICE_CATEGORY_REQUIRED = "Tên phân loại thiết bị là thông tin bắt buộc.";

        /// <summary>
        /// mã phân loại thiết bị
        /// </summary>
        public const string CODE_DEVICE_CATEGORY_REQUIRED = "Mã phân loại thiết bị là thông tin bắt buộc.";

        /// <summary>
        /// Mã {0} đã tồn tại
        /// </summary>
        public const string MSG_ERROR_AM_CODE_DUPLICATE = "Mã {0} đã tồn tại";

        /// <summary>
        /// Tên {0} đã tồn tại
        /// </summary>
        public const string MSG_ERROR_AM_NAME_DUPLICATE = "Tên {0} đã tồn tại";

        /// <summary>
        /// Folder Report AM
        /// </summary>
        public const string FOLDER_REPORT_AM = "ReportAM";

        /// <summary>
        /// Tiến trình xử lý - Tổng hợp
        /// </summary>
        public const string FILE_NAME_RPT_GENERAL = "Bao cao tien trinh xu ly tong hop";

        /// <summary>
        /// Tiến trình xử lý - Chi tiết
        /// </summary>
        public const string FILE_NAME_RPT_DETAIL = "Bao cao tien trinh xu ly chi tiet";

        /// <summary>
        /// Yêu cầu bảo trì - Chi tiết
        /// </summary>
        public const string FILE_NAME_R_AM_DETAIL = "Bao cao yeu cau bao tri chi tiet";

        /// <summary>
        /// Yêu cầu bảo trì - Tổng hợp
        /// </summary>
        public const string FILE_NAME_R_AM_GENERAL = "Bao cao yeu cau bao tri tong hop";

        /// <summary>
        /// Yêu cầu bảo trì - Mức độ khẩn cấp
        /// </summary>
        public const string FILE_NAME_R_AM_EMERGENCY = "Bao cao yeu cau bao tri muc do khan cap";

        /// <summary>
        /// Bất thường thiết bị - Tổng hợp
        /// </summary>
        public const string FILE_DEVICE_ABNORMAL_GENERAL = "Bat thuong thiet bi tong hop";

        /// <summary>
        /// Bất thường thiết bị - Chi tiết
        /// </summary>
        public const string FILE_DEVICE_ABNORMAL_DETAIL = "Bat thuong thiet bi chi tiet";

        /// <summary>
        /// Phiếu công tác điện
        /// </summary>
        public const string FILE_NAME_PCT_DIEN = "Phieu cong tac dien";

        /// <summary>
        /// Phiếu công tác thủy cơ nhiệt hóa
        /// </summary>
        public const string FILE_NAME_PCT_TCNH = "Phieu cong tac thuy co nhiet hoa";

        /// <summary>
        /// Phiếu thao tác
        /// </summary>
        public const string FILE_NAME_PTT = "Phieu thao tac";

        /// <summary>
        /// Giá trị 0
        /// </summary>
        public const int ZERO = 0;

        /// <summary>
        /// Thời gian Timeout khi gửi email
        /// </summary>
        public const int TIMEOUT_SENDMAIL = 20000;

        /// <summary>
        /// Giá trị bắt đầu
        /// </summary>
        public const int START_NUMBER = 1;

        /// <summary>
        /// Sô lần tối đa cho phép gửi lại email
        /// </summary>
        public const int RETRY_NUMBER = 3;

        /// <summary>
        /// Tiêu đề log chức năng thêm mới PYC
        /// </summary>
        public const string ADD_TICKET = "AmRequestTicketService: Create AmEnumRequestTicket";

        /// <summary>
        /// Chờ đăng ký
        /// </summary>
        public const string PYC_WAITING_STATUS = "Chờ đăng ký";

        /// <summary>
        /// Phiếu đăng ký
        /// </summary>
        public const string PDK = "Phiếu đăng ký";

        /// <summary>
        /// Giá trị xác định loại phiếu công tác 1: Phiếu công tác điện, 2: Phiếu công tác thủy-cơ-nhiệt-hóa
        /// </summary>
        public const int TYPE_PCTD = 1;

        /// <summary>
        /// Link truy cập phiếu đăng ký công tác điện
        /// </summary>
        public const string PDKD_ROUTERLINK = "/work-ticket/register/view/";

        /// <summary>
        /// Link truy cập phiếu đăng ký công tác thủy-cơ-nhiệt-hóa
        /// </summary>
        public const string PDKTCNH_ROUTERLINK = "/work-ticket-heat/register/view/";

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public const string PCT = "Phiếu công tác";

        /// <summary>
        /// Link truy cập phiểu công tác điện
        /// </summary>
        public const string PCTD_ROUTERLINK = "/work-ticket/issue/detail/";

        /// <summary>
        /// Link truy cập phiếu công tác thủy-cơ-nhiệt-hóa
        /// </summary>
        public const string PCTTCNH_ROUTERLINK = "/work-ticket-heat/issue/detail/";

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public const string MAIL_MERGE_TITLE = "[He thong Elogbook]";

        /// <summary>
        /// Thông tin người nhận mail
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_USER = "[Account User]";

        /// <summary>
        /// Đơn vị công tác đăng ký
        /// </summary>
        public const string MAIL_MERGE_DON_VI_CONG_TAC_DANG_KY = "[Don vi cong tac dang ky]";

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public const string MAIL_MERGE_NOI_DUNG_CONG_VIEC = "[Noi dung cong viec]";

        /// <summary>
        /// Địa điểm
        /// </summary>
        public const string MAIL_MERGE_DIA_DIEM = "[Dia diem]";

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public const string MAIL_MERGE_THOI_GIAN_BAT_DAU = "[Thoi gian bat dau]";

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public const string MAIL_MERGE_THOI_GIAN_KET_THUC = "[Thoi gian ket thuc]";

        /// <summary>
        /// Người tạo
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_TAO = "[Account tao]";

        /// <summary>
        /// Người thu hồi
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_THU_HOI = "[Account thu hoi]";

        /// <summary>
        /// Thời gian thu hồi
        /// </summary>
        public const string MAIL_MERGE_THOI_GIAN_THU_HOI = "[Thoi gian thu hoi]";

        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public const string MAIL_MERGE_SO_PHIEU_CONG_TAC = "[So phieu cong tac]";

        /// <summary>
        /// Người cấp
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_CAP = "[Account cap]";

        /// <summary>
        /// Lý do từ chối
        /// </summary>
        public const string MAIL_MERGE_LY_DO_TU_CHOI = "[Ly do tu choi]";

        /// <summary>
        /// Lý do dừng xử lý
        /// </summary>
        public const string MAIL_MERGE_LY_DO_DUNG_XU_LY = "[Ly do dung xu ly]";

        /// <summary>
        /// Người từ chối
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_TU_CHOI = "[Account tu choi]";

        /// <summary>
        /// Link hệ thống
        /// </summary>
        public const string MAIL_MERGE_LINK_HE_THONG = "[Link he thong]";

        /// <summary>
        /// Link xem thông tin phiếu đăng ký điện
        /// </summary>
        public const string MAIL_MERGE_PDKD = "work-ticket/register/view/";

        /// <summary>
        /// Link xem thông tin phiếu đăng ký thủy-cơ-nhiệt-hóa
        /// </summary>
        public const string MAIL_MERGE_PDKD_TCNH = "work-ticket-heat/register/view/";

        /// <summary>
        /// Từ chối cấp phiếu
        /// </summary>
        public const string MAIL_MERGE_REJECT = "Từ chối phiếu đăng ký.";

        /// <summary>
        /// Người cho phép
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_CHO_PHEP = "[Account cho phep]";

        /// <summary>
        /// Thời gian cho phép
        /// </summary>
        public const string MAIL_MERGE_THOI_GIAN_CHO_PHEP = "[Thoi gian cho phep]";

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public const string MAIL_MERGE_THOI_GIAN_HOAN_THANH = "[Thoi gian hoan thanh]";

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public const string MAIL_MERGE_LOAI_PHIEU = "[Loai phieu]";

        /// <summary>
        /// Khu vực
        /// </summary>
        public const string MAIL_MERGE_KHU_VUC = "[Khu vuc]";

        /// <summary>
        /// Thiết bị
        /// </summary>
        public const string MAIL_MERGE_THIET_BI = "[Thiet bi]";

        /// <summary>
        /// Tài khoản thực hiện AM
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_THUC_HIEN_AM = "[Account thuc hien AM]";

        /// <summary>
        /// Ngày của hoạt động AM
        /// </summary>
        public const string MAIL_MERGE_NGAY_CUA_HOAT_DONG_AM = "[Ngay cua hoat dong AM]";

        /// <summary>
        /// Tên lịch bảo trì
        /// </summary>
        public const string MAIL_MERGE_TEN_LICH_BAO_TRI = "[Ten lich bao tri]";

        /// <summary>
        /// Cương vị
        /// </summary>
        public const string MAIL_MERGE_CUONG_VI = "[Cuong vi]";

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public const string MAIL_MERGE_CA = "[Ca]";

        /// <summary>
        /// Nội dung bất thường thiết bị
        /// </summary>
        public const string MAIL_MERGE_NOI_DUNG_BAT_THUONG_THIET_BI = "[Noi dung bat thuong, thiet bi]";

        /// <summary>
        /// Link truy cập tác vụ bảo trì tự quản
        /// </summary>
        public const string MAIL_MERGE_AM_LINK = "/autonomous-maintenance/detail/";

        /// <summary>
        /// Số phiếu yêu cầu
        /// </summary>
        public const string MAIL_MERGE_SO_PHIEU_YEU_CAU = "[So phieu yeu cau]";

        /// <summary>
        /// Mức độ khẩn cấp
        /// </summary>
        public const string MAIL_MERGE_MUC_DO_KHAN_CAP = "[Muc do khan cap]";

        /// <summary>
        /// Nhà máy
        /// </summary>
        public const string MAIL_MERGE_NHA_MAY = "[Nha may]";

        /// <summary>
        /// Người phê duyệt
        /// </summary>
        public const string MAIL_MERGE_ACCOUNT_PHE_DUYET = "[Account phe duyet]";

        /// <summary>
        /// Từ chối
        /// </summary>
        public const string MAIL_MERGE_REJECT_STATUS = "Từ chối yêu cầu";

        /// <summary>
        /// Dừng xử lý
        /// </summary>
        public const string MAIL_MERGE_STOP_STATUS = "Dừng yêu cầu";

        /// <summary>
        /// Link truy cập phiếu công tác
        /// </summary>
        public const string MAIL_MERGE_TICKET_LINK = "/request-ticket/approve-list/";

        /// <summary>
        /// Cho phép cấp phiếu công tác điện
        /// </summary>
        public const string PERMISSION_ADD_CPCTD = "ADD_CPCTĐ";

        /// <summary>
        /// Cho phép công tác thủy-cơ-nhiệt-hóa
        /// </summary>
        public const string PERMISSION_ADD_CPCTTCNH = "ADD_CPCTTCNH";

        /// <summary>
        /// Đăng ký phiếu công tác điện
        /// </summary>
        public const string PERMISSION_ADD_PCTD = "ADD_PCTĐ";

        /// <summary>
        /// Đăng ký phiếu công tác thủy-cơ-nhiệt-hóa
        /// </summary>
        public const string PERMISSION_ADD_PCTTCNH = "ADD_PCTTCNH";

        /// <summary>
        /// Cho phép Trưởng ca cập nhật PYC
        /// </summary>
        public const string PERMISSION_EDIT_QLPYCTC = "EDIT_QLPYCTC";

        /// <summary>
        /// Cho phép cập nhật PYC đơn vị sữa chửa
        /// </summary>
        public const string PERMISSION_EDIT_QLPYCDVSC = "EDIT_QLPYCDVSC";

        /// <summary>
        /// Thông báo quyền người dùng
        /// </summary>
        public const string USER_RIGHT = "Tài khoản của bạn không có quyền thực hiện chức năng này, vui lòng liên hệ quản trị viên để được hỗ trợ.";

        /// <summary>
        /// Độ dài của tên tệp tin
        /// </summary>
        public const int FILENAME_LENGTH = 255;

        /// <summary>
        /// Lỗi kiểm tra độ dài tên tệp tin
        /// </summary>
        public const string FILENAME_LENGTH_ERROR = "Độ dài của tên tệp tin không được vượt quá {0} kí tự.";

        /// <summary>
        /// Kiểm tra loại tệp tin cho phép
        /// </summary>
        public const string FILE_EXTENTION_ERROR = "Không cho phép đính kèm tệp tin có định dạng {0}. Bạn chỉ được phép upload tệp tin có định dạng {1}.";

        /// <summary>
        /// Kiểm tra dung lượng tệp tin upload
        /// </summary>
        public const string FILE_SIZE_ERROR = "Bạn chỉ được phép upload tệp tin có dung lượng tối đa {0}.";

        /// <summary>
        /// Dung lương tệp tin cho phép
        /// </summary>
        public const int ALLOWED_FILESIZE = 1024 * 1024 * 30;

        /// <summary>
        /// Dung lương tệp tin cho phép
        /// </summary>
        public const string ALLOWED_FILE_EXTENTION = "flv, mp4, m4a, mov, wmv, mkv, 3gp, png, jpeg, jpg, xlsx, docx, xls, doc, pdf";

        /// <summary>
        /// Kiểm tra tệp tin upload không đúng định dạng
        /// </summary>
        public const string FILE_SIGNATURE_ERROR = "Tệp tin upload không đúng định dạng {0}.";

        /// <summary>
        /// Tệp tin upload không hợp lệ, vui lòng kiểm tra lại tệp tin.
        /// </summary>
        public const string FILE_UPLOAD_ERROR = "Tệp tin upload không hợp lệ, vui lòng kiểm tra lại tệp tin.";

        /// <summary>
        /// Nội dung văn bản có thể chứa dữ liệu mã độc
        /// </summary>
        public const string MALICIOUS_CODE = "Nội dung văn bản có thể chứa dữ liệu mã độc";

        /// <summary>
        /// Folder Template
        /// </summary>
        public const string FOLDER_EXCEL_TEMPLATE = "TemplateExcelFile";

        /// <summary>
        /// Tempalte file data loại thiết bị
        /// </summary>
        public const string FILE_TEMP_DEVICE_TYPE = "AM Loai thiet bi";

        /// <summary>
        /// Tempalte file data khu vực
        /// </summary>
        public const string FILE_TEMP_LOCATION = "AM Khu vuc";

        /// <summary>
        /// Tempalte file data loại công việc
        /// </summary>
        public const string FILE_TEMP_WORKTYPE = "AM Loai cong viec";

        /// <summary>
        /// Tempalte file data công việc
        /// </summary>
        public const string FILE_TEMP_WORK = "AM Cong viec";

        /// <summary>
        /// Tempalte file data Mức độ khẩn cấp
        /// </summary>
        public const string FILE_TEMP_EMERGENCY = "AM Muc do khan cap";

        /// <summary>
        /// Tempalte file data Nguyên nhân tạm dừng công viêc
        /// </summary>
        public const string FILE_TEMP_REASON_PENDING = "AM Nguyen nhan tam dung cong viec";

        /// <summary>
        /// Tempalte file data Nguyên nhân không thực hiện
        /// </summary>
        public const string FILE_TEMP_REASON_STOP = "AM Nguyen nhan khong thuc hien";

        /// <summary>
        /// Tempalte file data thiết bị
        /// </summary>
        public const string FILE_TEMP_DEVICE = "AM Thiet bi";

        /// <summary>
        /// Tempalte file data Phân loại thiết bị
        /// </summary>
        public const string FILE_TEMP_DEVICE_CATEGORY = "AM Phan loai thiet bi";

        /// <summary>
        /// Tempalte file data Nước sản xuất
        /// </summary>
        public const string FILE_TEMP_COUNTRY = "AM Nuoc san xuat";

        /// <summary>
        /// Tempalte file data Nhà chế tạo
        /// </summary>
        public const string FILE_TEMP_MANUFACTURE = "AM Nha che tao";

        /// <summary>
        /// Tempalte file data Nhà cung cấp
        /// </summary>
        public const string FILE_TEMP_SUPPLIER = "AM Nha cung cap";

        /// <summary>
        /// Đăng ký Phiếu công tác số lượng lớn
        /// </summary>
        public const string BATCH_WORK_REG = "BatchWorkReg";

        /// <summary>
        /// Tạo mới batch phiếu đăng ký công tác số lượng lớn
        /// </summary>
        public const string MSG_CREATE_PĐK_SLL = "Tạo mới batch phiếu đăng ký công tác số lượng lớn";

        /// <summary>
        /// Kiểm tra trùng Số BBKSHT
        /// </summary>
        public const string DUPLICATE_BBKSHT_TICKET = "Số biên bản khảo sát hiện trường '{0}' đã tồn tại.";

        /// <summary>
        /// Kiểm tra trùng Số lệnh đăng ký công tác
        /// </summary>
        public const string DUPLICATE_REGISTRAITON_WORK_PERMIT = "Số lệnh đăng ký công tác '{0}' đã tồn tại.";

        /// <summary>
        /// Có lỗi xảy ra khi lưu lệnh đăng ký công tác
        /// </summary>
        public const string REGISTRATION_WORK_PERMIT = "Có lỗi xảy ra khi lưu lệnh đăng ký công tác. Kiểm tra log để biết nguyên nhân.";

        /// <summary>
        /// Số lệnh đăng ký
        /// </summary>
        public const string WORK_PERMIT_NUMBER = "Số lệnh đăng ký công tác là thông tin bắt buộc.";

        /// <summary>
        /// Đơn vị nhận lệnh
        /// </summary>
        public const string RECEIVE_WORK_PERMIT_UNIT = "Đơn vị nhận lệnh là thông tin bắt buộc.";

        /// BBKSHT không tồn tại
        /// </summary>
        public const string BBKSHT_NOT_FOUND = "Không tìm thấy biên bảo khảo sát hiện trường.";

        /// <summary>
        /// User/ Pass không đúng
        /// </summary>
        public const string USERNAME_PASS_NOT_CORRECT = "Tên đăng nhập hoặc mật khẩu không đúng.";

        /// <summary>
        /// Số CMND/CCCD là thông tin bắt buộc.
        /// </summary>
        public const string NATIONALID_REQUIRED = "Số CMND/CCCD là thông tin bắt buộc.";

        /// <summary>
        /// Hình ảnh số CMND/CCCD là thông tin bắt buộc.
        /// </summary>
        public const string NATIONALATTACHMENTID_REQUIRED = "Hình ảnh số CMND/CCCD là thông tin bắt buộc.";

        /// <summary>
        /// Thẻ an toàn điện là thông tin bắt buộc.
        /// </summary>
        public const string ELECTRICRANKCARD_REQUIRED = "Thẻ an toàn điện là thông tin bắt buộc.";

        /// <summary>
        /// Bậc an toàn điện là thông tin bắt buộc.
        /// </summary>
        public const string ELECTRICRANK_REQUIRED = "Bậc an toàn điện là thông tin bắt buộc.";

        /// <summary>
        /// Hình ảnh thẻ an toàn điện là thông tin bắt buộc.
        /// </summary>
        public const string ELECTRICRANKATTACHMENTID_REQUIRED = "Hình ảnh thẻ an toàn điện là thông tin bắt buộc.";

        /// <summary>
        /// Thẻ an toàn lao động là thông tin bắt buộc.
        /// </summary>
        public const string LABORCARD_REQUIRED = "Thẻ an toàn lao động là thông tin bắt buộc.";

        /// <summary>
        /// Bậc thợ là thông tin bắt buộc.
        /// </summary>
        public const string LABORRANK_REQUIRED = "Bậc thợ là thông tin bắt buộc.";

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động là thông tin bắt buộc.
        /// </summary>
        public const string LABORCARDATTACHMENTID_REQUIRED = "Hình ảnh thẻ an toàn lao động là thông tin bắt buộc.";

        /// <summary>
        /// Chữ ký là thông tin bắt buộc.
        /// </summary>
        public const string SIGNATURE_REQUIRED = "Chữ ký là thông tin bắt buộc.";

        /// <summary>
        /// Phương thức ký Cấp/Hoàn thành/Kết thúc PCT là bắt buộc.
        /// </summary>
        public const string SIGNINGMETHODID_REQUIRED = "Phương thức ký Cấp/Hoàn thành/Kết thúc PCT là bắt buộc.";

        /// <summary>
        /// Phương thức ký Cấp/Hoàn thành/Kết thúc PCT không hợp lệ.
        /// </summary>
        public const string SIGNINGMETHOD_NOT_VALID = "Phương thức ký Cấp/Hoàn thành/Kết thúc PCT không hợp lệ.";

        /// <summary>
        /// Bậc an toàn điện không hợp lệ.
        /// </summary>
        public const string ELECTRICRANK_NOT_VALID = "Bậc an toàn điện không hợp lệ.";

        /// <summary>
        /// Bậc thợ không hợp lệ.
        /// </summary>
        public const string LABORRANK_NOT_VALID = "Bậc thợ không hợp lệ.";

        /// <summary>
        /// Chức vụ là thông tin bắt buộc.
        /// </summary>
        public const string POSITIONNAME_REQUIRED = "Chức vụ là thông tin bắt buộc.";

        /// <summary>
        /// Nhật ký vận hành
        /// </summary>
        public const string OPERATION_DIARY = "OperationDiary";
    }
}
