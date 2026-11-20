using System.Configuration;

namespace Elogbook.Core
{
    /// <summary>
    /// Định nghĩa những thông tin được khai báo ở file config
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Cấu hình thông tin log4net
        /// </summary>
        public static readonly string LOG4NET_CONFIG_FILE = ConfigurationManager.AppSettings["LOG4NET_CONFIG_FILE"];

        /// <summary>
        /// Server AD để kết nối
        /// </summary>
        public static readonly string AD_LDAP = ConfigurationManager.AppSettings["AD_LDAP"];

        /// <summary>
        /// Tài khoản kết nối đến AD
        /// </summary>
        public static readonly string AD_USERNAME = ConfigurationManager.AppSettings["AD_USERNAME"];

        /// <summary>
        /// Mật khẩu tài khoản kết nói tới AD
        /// </summary>
        public static readonly string AD_PASSWORD = ConfigurationManager.AppSettings["AD_PASSWORD"];

        /// <summary>
        /// Đường dẫn lưu file data api
        /// </summary>
        public static readonly string FILE_STORAGE_SERVER_DIR = ConfigurationManager.AppSettings["FILE_STORAGE_SERVER_DIR"];

        /// <summary>
        /// Đường dẫn Server chứa file ảnh
        /// </summary>
        public static readonly string SERVER_DIR = ConfigurationManager.AppSettings["SERVER_DIR"];

        /// <summary>
        /// VIETTEL_CERT
        /// </summary>
        public static readonly string VIETTEL_CERT = ConfigurationManager.AppSettings["VIETTEL_CERT"];

        /// <summary>
        /// VIETTEL_KEY
        /// </summary>
        public static readonly string VIETTEL_KEY = ConfigurationManager.AppSettings["VIETTEL_KEY"];

        /// <summary>
        /// VIETTEL_APIID
        /// </summary>
        public static readonly string VIETTEL_APIID = ConfigurationManager.AppSettings["VIETTEL_APIID"];

        /// <summary>
        /// URL connect to api Viettel
        /// </summary>
        public static readonly string VIETTEL_CA_DOTNET_WEB_MobileCAWS_APWsImplService = ConfigurationManager.AppSettings["VIETTEL_CA_DOTNET_WEB_MobileCAWS_APWsImplService"];

        /// <summary>
        /// VNPT SmartCA SP ID
        /// </summary>
        public static readonly string VNPT_SP_ID = ConfigurationManager.AppSettings["VNPT_SP_ID"];

        /// <summary>
        /// VNPT SmartCA SP Password
        /// </summary>
        public static readonly string VNPT_SP_PASSWORD = ConfigurationManager.AppSettings["VNPT_SP_PASSWORD"];

        /// <summary>
        /// VNPT SmartCA Gateway URL
        /// </summary>
        public static readonly string VNPT_GATEWAY_URL = ConfigurationManager.AppSettings["VNPT_GATEWAY_URL"];

        /// <summary>
        /// VNPT SmartCA - Số lần polling tối đa chờ user confirm
        /// </summary>
        public static int VNPT_MAX_POLLING_COUNT
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["VNPT_MAX_POLLING_COUNT"], out int value);
                if (value <= 0) value = 30;
                return value;
            }
        }

        /// <summary>
        /// VNPT SmartCA - Thời gian chờ giữa mỗi lần polling (milliseconds)
        /// </summary>
        public static int VNPT_POLLING_INTERVAL_MS
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["VNPT_POLLING_INTERVAL_MS"], out int value);
                if (value <= 0) value = 10000;
                return value;
            }
        }

        // =====================================================================
        // Viettel MySign (ký bất đồng bộ)
        // =====================================================================

        /// <summary>
        /// Base URL của Viettel MySign API (VD: https://mysign.viettel.vn)
        /// </summary>
        public static readonly string VIETTEL_MYSIGN_BASE_URL = ConfigurationManager.AppSettings["VIETTEL_MYSIGN_BASE_URL"];

        /// <summary>
        /// Client ID ứng dụng đăng ký với Viettel MySign
        /// </summary>
        public static readonly string VIETTEL_MYSIGN_CLIENT_ID = ConfigurationManager.AppSettings["VIETTEL_MYSIGN_CLIENT_ID"];

        /// <summary>
        /// Client Secret ứng dụng đăng ký với Viettel MySign
        /// </summary>
        public static readonly string VIETTEL_MYSIGN_CLIENT_SECRET = ConfigurationManager.AppSettings["VIETTEL_MYSIGN_CLIENT_SECRET"];

        /// <summary>
        /// Profile ID người ký (tùy chọn, theo tài liệu Viettel MySign)
        /// </summary>
        public static readonly string VIETTEL_MYSIGN_PROFILE_ID = ConfigurationManager.AppSettings["VIETTEL_MYSIGN_PROFILE_ID"];

        /// <summary>
        /// Số lần polling tối đa chờ người dùng xác nhận trên app MySign (default: 30)
        /// </summary>
        public static int VIETTEL_MYSIGN_MAX_POLLING_COUNT
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["VIETTEL_MYSIGN_MAX_POLLING_COUNT"], out int value);
                if (value <= 0) value = 30;
                return value;
            }
        }

        /// <summary>
        /// Thời gian chờ giữa mỗi lần polling MySign (milliseconds, default: 5000)
        /// </summary>
        public static int VIETTEL_MYSIGN_POLLING_INTERVAL_MS
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["VIETTEL_MYSIGN_POLLING_INTERVAL_MS"], out int value);
                if (value <= 0) value = 5000;
                return value;
            }
        }

        /// <summary>
        /// Token expire của người dùng
        /// </summary>
        public static int DEFAULT_TOKEN_EXPIRE_MINUTES
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["DEFAULT_TOKEN_EXPIRE_MINUTES"], out int value);
                // Mặc định 30 phút nếu không cấu hình hoặc cấu hình không đúng
                if (value <= 0) value = 30;

                return value;
            }
        }

        /// <summary>
        /// Cho phép migrate password từ hệ thống cũ
        /// </summary>
        public static bool ALLOW_MIGRATE_PASSWORD
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["ALLOW_MIGRATE_PASSWORD"], out bool value);

                return value;
            }
        }

        /// <summary>
        /// Cấu hình thời gian lấy dữ liệu offline
        /// </summary>
        public static int DATE_SYNC_DATA_MOBILE
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["DATE_SYNC_DATA_MOBILE"], out int value);
                // Mặc định lấy dữ liệu 3 ngày gần nhất
                if (value <= 0) value = 3;

                return value;
            }
        }

        /// <summary>
        /// Cấu hình chỉ cho phép lấy ca vận hành theo giờ thực tế hoặc lấy tất cả ca vận hành của nhà máy
        /// </summary>
        public static bool ALLOW_GET_SHIFT_BY_CURRENT_TIME
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["ALLOW_GET_SHIFT_BY_CURRENT_TIME"], out bool value);

                return value;
            }
        }

        /// <summary>
        /// Thời gian trigger đồng bộ dữ liệu offline khi thiết bị có internet, giá trị tính bằng phút
        /// </summary>
        public static int SYNC_OFFLINE_TIME
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["SYNC_OFFLINE_TIME"], out int value);
                // Mặc định 5 phút sẽ thực hiện sync data
                if (value <= 0) value = 5;

                return value;
            }
        }

        /// <summary>
        /// Định nghĩa thời gian xóa dữ liệu offline ở thiết bị, giá trị tính bằng ngày
        /// </summary>
        public static int DELETE_OFFLINE
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["DELETE_OFFLINE"], out int value);
                // Mặc định 7 ngày sẽ thực hiện xóa dữ liệu ở thiết bị
                if (value <= 0) value = 7;

                return value;
            }
        }

        /// <summary>
        /// Kiểm tra tính khả thi sử dụng của các thư viện hỗ trợ
        /// </summary>
        public static readonly string VALIDATION_KEY = ConfigurationManager.AppSettings["VALIDATION_KEY"];

        /// <summary>
        /// Tăng ngày tạo NKVH
        /// </summary>
        public static bool NKVH_IS_EXTENDTIME
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["NKVH_ISEXTENDTIME"], out bool value);

                return value;
            }
        }

        /// <summary>
        /// Tăng ngày tạo NKVH
        /// </summary>
        public static bool IS_TURN_OFF_LOG_QUARTZ
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["IS_TURN_OFF_LOG_QUARTZ"], out bool value);

                return value;
            }
        }

        /// <summary>
        /// Schedule chạy đồng bộ dữ liệu sang PMIS
        /// </summary>
        public static readonly string CRON_SCHEDULE_PMIS = ConfigurationManager.AppSettings["CRON_SCHEDULE_PMIS"];

        /// <summary>
        /// Schedule chạy đồng bộ dữ liệu từ PMIS sang Elogbook
        /// </summary>
        public static readonly string CRON_SCHEDULE_PMIS_ELOGBOOK = ConfigurationManager.AppSettings["CRON_SCHEDULE_PMIS_ELOGBOOK"];

        /// <summary>
        /// Schedule chạy tự động tạo work ticket
        /// </summary>
        public static readonly string CRON_SCHEDULE_WORK_TICKET = ConfigurationManager.AppSettings["CRON_SCHEDULE_WORK_TICKET"];

        /// <summary>
        /// Schedule chạy tự động backup data
        /// </summary>
        public static readonly string CRON_SCHEDULE_BACKUP_DATA = ConfigurationManager.AppSettings["CRON_SCHEDULE_BACKUP_DATA"];

        /// <summary>
        /// Schedule chạy tự động tạo AutonomousMaintenance
        /// </summary>
        public static readonly string CRON_SCHEDULE_AUTONOMOUS_MAINTENANCE = ConfigurationManager.AppSettings["CRON_SCHEDULE_AUTONOMOUS_MAINTENANCE"];

        /// <summary>
        /// Định nghĩa thời gian lấy dữ liệu thay đổi để đồng bộ sang PMIS, mặc định lấy những thông số cập nhật trong khoảng 2 giờ trước đó
        /// </summary>
        public static int TIME_GET_LATEST_DATA_PMIS
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["TIME_GET_LATEST_DATA_PMIS"], out int value);
                // Mặc định lấy dữ liệu cập nhật trong khoảng 2 giờ trước đó
                if (value == 0) value = -2;

                return value;
            }
        }

        /// <summary>
        /// Schedule reset số PCT không dử dụng
        /// </summary>
        public static readonly string CRON_SCHEDULE_TICKET_NUMBER = ConfigurationManager.AppSettings["CRON_SCHEDULE_TICKET_NUMBER"];

        /// <summary>
        /// Định nghĩa thời gian xóa file log
        /// </summary>
        public static int LAST_TIME_DELETE_LOG
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["LAST_TIME_DELETE_LOG"], out int value);
                // Mặc định sẽ xóa những file của 7 ngày trước đó
                if (value == 0) value = -7;

                return value;
            }
        }

        /// <summary>
        /// Schedule xóa file log
        /// </summary>
        public static readonly string CRON_SCHEDULE_DELETE_LOG = ConfigurationManager.AppSettings["CRON_SCHEDULE_DELETE_LOG"];

        /// <summary>
        /// Schedule xử lý gửi mail
        /// </summary>
        public static readonly string CRON_SCHEDULE_SEND_MAIL = ConfigurationManager.AppSettings["CRON_SCHEDULE_SEND_MAIL"];

        /// <summary>
        /// domain name
        /// </summary>
        public static readonly string DOMAIN = ConfigurationManager.AppSettings["DOMAIN"];

        /// <summary>
        /// Thư mục chứa file excel dữ liệu tự động export từ hệ thống DCS
        /// </summary>
        public static readonly string EXCEL_ROOT_FOLDER_PATH = ConfigurationManager.AppSettings["EXCEL_ROOT_FOLDER_PATH"];

        /// <summary>
        /// Thư mục chứa dữ liệu Elogbook đọc và cập nhật vào hệ thống
        /// </summary>
        public static readonly string EXCEL_DESTINATION_FOLDER_PATH = ConfigurationManager.AppSettings["EXCEL_DESTINATION_FOLDER_PATH"];

        /// <summary>
        /// TÊN FILE EXCEL THÔNG SỐ ĐỘ RUNG - ĐỘ ĐẢO
        /// </summary>
        public static readonly string EXCEL_FILENAME_RUNG_DAO = ConfigurationManager.AppSettings["EXCEL_FILENAME_RUNG_DAO"];

        /// <summary>
        /// TÊN FILE EXCEL THÔNG SỐ VẬN HÀNH TỔ MÁY H1
        /// </summary>
        public static readonly string EXCEL_FILENAME_TMH1 = ConfigurationManager.AppSettings["EXCEL_FILENAME_TMH1"];

        /// <summary>
        /// TÊN FILE EXCEL THÔNG SỐ VẬN HÀNH TỔ MÁY H2
        /// </summary>
        public static readonly string EXCEL_FILENAME_TMH2 = ConfigurationManager.AppSettings["EXCEL_FILENAME_TMH2"];

        /// <summary>
        /// TÊN FILE EXCEL THÔNG SỐ VẬN HÀNH THIẾT BỊ ĐIỆN TRUNG TÂM
        /// </summary>
        public static readonly string EXCEL_FILENAME_TRUNG_TAM = ConfigurationManager.AppSettings["EXCEL_FILENAME_TRUNG_TAM"];

        /// <summary>
        /// Số ngày cần đọc dữ liệu DCS so với ngày hiện tại trở về trước
        /// </summary>
        public static int DCS_DAY_READ_DATA
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["DCS_DAY_READ_DATA"], out int value);
                // Mặc định sẽ đọc dữ liệu 3 ngày gần nhất
                if (value == 0) value = 3;

                return value;
            }
        }

        /// <summary>
        /// Cho phép hiển thị Tình hình lúc nhận ca cho nhật ký vận hành hay không
        /// </summary>
        public static bool IS_SHOW_SHIFT_RECEIVING
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["IS_SHOW_SHIFT_RECEIVING"], out bool value);

                return value;
            }
        }

        /// <summary>
        /// Cho phép hiển thị Tình hình lúc nhận ca cho nhật ký vận hành hay không
        /// </summary>
        public static bool IS_SHOW_NOTIFICATION
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["IS_SHOW_NOTIFICATION"], out bool value);

                return value;
            }
        }

        /// <summary>
        /// Đường dẫn lưu file data api
        /// </summary>
        public static readonly string FILE_LUCENE_SERVER_DIR = ConfigurationManager.AppSettings["FILE_LUCENE_SERVER_DIR"];

        /// <summary>
        /// Khi flag này true, thì với cương vị Trưởng ca sẽ luôn yêu cầu có một Trưởng kíp, ngược lại thì theo logic chung của ứng dụng
        /// </summary>
        public static bool IS_REQUIRED_TEAMLEADER_IN_SHIFTLEADER
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["IS_REQUIRED_TEAMLEADER_IN_SHIFTLEADER"], out bool value);

                return value;
            }
        }

        /// <summary>
        /// Cấu hình đồng bộ dữ liệu thông số sang table để lấy thông tin cảnh báo
        /// </summary>
        public static readonly string CRON_SCHEDULE_SYNC_DATA_OPERATION_PARAMETER_WARNING = ConfigurationManager.AppSettings["CRON_SCHEDULE_SYNC_DATA_OPERATION_PARAMETER_WARNING"];

        /// <summary>
        /// Định nghĩa thời gian xóa dữ liệu xuất báo cáo nhật ký vận hành, mặc định xóa dữ liệu của 30 ngày trước
        /// </summary>
        public static int LAST_TIME_DELETE_REPORT_NKVH
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["LAST_TIME_DELETE_REPORT_NKVH"], out int value);
                // Mặc định xóa dữ liệu của 30 ngày trước
                if (value == 0)
                {
                    value = -30;
                }

                return value;
            }
        }

        /// <summary>
        /// Phân công ca: khoảng thời gian tối đa cho phép mỗi lần tạo bảng phân công, giá trị tính bằng ngày.
        /// Mặc định 31 ngày.
        /// </summary>
        public static int ASSIGNMENT_MAX_CREATE_DAYS
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["ASSIGNMENT_MAX_CREATE_DAYS"], out int value);
                if (value <= 0) value = 31;

                return value;
            }
        }

        /// <summary>
        /// Phân công ca: thời gian cho phép được cập nhật nhân sự trong một slot,
        /// tính bằng số giờ trước giờ bắt đầu ca. Mặc định 24 giờ.
        /// Ví dụ: value = 24 → chỉ được update trước khi ca bắt đầu ít nhất 24 giờ.
        /// </summary>
        public static int ASSIGNMENT_UPDATE_DEADLINE_HOURS
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["ASSIGNMENT_UPDATE_DEADLINE_HOURS"], out int value);
                if (value <= 0) value = 24;

                return value;
            }
        }

        /// <summary>
        /// Google Gemini API Key dùng cho KnowledgeBaseAi
        /// </summary>
        public static readonly string GEMINI_API_KEY = ConfigurationManager.AppSettings["GEMINI_API_KEY"];

        /// <summary>
        /// Google Gemini model name, ví dụ: gemini-3.6-flash, gemini-1.5-pro
        /// Mặc định: gemini-3.6-flash
        /// </summary>
        public static string GEMINI_MODEL
        {
            get
            {
                var model = ConfigurationManager.AppSettings["GEMINI_MODEL"];
                return string.IsNullOrWhiteSpace(model) ? "gemini-3.6-flash" : model;
            }
        }

        /// <summary>
        /// Cron expression cho HistoryCallJob (archive TSVH cũ hơn 12 tháng).
        /// Mặc định: Chủ nhật 2:00 AM (0 0 2 ? * SUN)
        /// </summary>
        public static readonly string CRON_SCHEDULE_HISTORY_CALL = ConfigurationManager.AppSettings["CRON_SCHEDULE_HISTORY_CALL"];

        /// <summary>
        /// Số bản ghi tối đa mỗi batch khi archive OperationParameters sang HistoryCall.
        /// Mặc định: 50000
        /// </summary>
        public static int HISTORY_CALL_BATCH_SIZE
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["HISTORY_CALL_BATCH_SIZE"], out int value);
                if (value <= 0) value = 50000;
                return value;
            }
        }

        /// <summary>
        /// Số batch tối đa được xử lý trong một lần chạy HistoryCallJob.
        /// Mặc định: 10
        /// </summary>
        public static int HISTORY_CALL_MAX_BATCHES
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["HISTORY_CALL_MAX_BATCHES"], out int value);
                if (value <= 0) value = 10;
                return value;
            }
        }

        /// <summary>
        /// Các mốc thời gian để thông báo phiếu công tác sắp đến thời gian theo kế hoạch nhưng chưa kết thúc
        /// </summary>
        public static readonly string WORK_TICKET_BEFORE_COMPLETION_NOTIFICATION_HOURS = ConfigurationManager.AppSettings["WORK_TICKET_BEFORE_COMPLETION_NOTIFICATION_HOURS"];

        /// <summary>
        /// Các mốc thời gian để thông báo phiếu công tác đã quá thời gian theo kế hoạch nhưng chưa kết thúc
        /// </summary>
        public static readonly string WORK_TICKET_AFTER_COMPLETION_NOTIFICATION_HOURS = ConfigurationManager.AppSettings["WORK_TICKET_AFTER_COMPLETION_NOTIFICATION_HOURS"];

        /// <summary>
        /// Các mốc thời gian để thông báo Phiếu công tác có nhân viên chưa rời khỏi vị trí làm việc
        /// </summary>
        public static readonly string WORK_TICKET_NOTSIGNEDOUT_NOTIFICATION_HOURS = ConfigurationManager.AppSettings["WORK_TICKET_NOTSIGNEDOUT_NOTIFICATION_HOURS"];

        /// <summary>
        /// Cấu hình thời gian chạy job để gửi thông báo nhắc nhở, cảnh báo Phiếu công tác có nhân viên chưa rời khỏi vị trí làm việc
        /// </summary>
        public static readonly string CRON_SCHEDULE_WORK_TICKET_NOTSIGNEDOUT_NOTIFICATION_HOURS = ConfigurationManager.AppSettings["CRON_SCHEDULE_WORK_TICKET_NOTSIGNEDOUT_NOTIFICATION_HOURS"];

        /// <summary>
        /// Định nghĩa khoảng thời gian quanh mốc cần lấy danh sách phiếu để gửi thông báo
        /// </summary>
        public static int WORK_PERMIT_NOTIFICATION_WINDOW_MINUTES
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["WORK_PERMIT_NOTIFICATION_WINDOW_MINUTES"], out int value);
                if (value == 0)
                {
                    value = 5;
                }

                return value;
            }
        }

        /// <summary>
        /// Cấu hình thời gian chạy job để gửi thông báo nhắc nhở, cảnh báo phiếu công tác chưa hoàn thành theo kế hoạch
        /// </summary>
        public static readonly string CRON_SCHEDULE_WORK_TICKET_COMPLETION_NOTIFICATION_HOURS = ConfigurationManager.AppSettings["CRON_SCHEDULE_WORK_TICKET_COMPLETION_NOTIFICATION_HOURS"];

        // =====================================================================
        // Open Notebook Integration
        // =====================================================================

        /// <summary>
        /// Base URL của Open Notebook API.
        /// Ví dụ: http://192.168.1.100:5055
        /// Cần đặt trỏ thẳng tới FastAPI backend (port 5055), không qua Next.js frontend.
        /// </summary>
        public static readonly string OPEN_NOTEBOOK_BASE_URL = ConfigurationManager.AppSettings["OPEN_NOTEBOOK_BASE_URL"];

        /// <summary>
        /// Mật khẩu xác thực Open Notebook (PasswordAuthMiddleware).
        /// Tương ứng với biến môi trường APP_PASSWORD phía Open Notebook.
        /// Để trống nếu Open Notebook chạy không cần password (dev mode).
        /// </summary>
        public static readonly string OPEN_NOTEBOOK_PASSWORD = ConfigurationManager.AppSettings["OPEN_NOTEBOOK_PASSWORD"];

        /// <summary>
        /// Timeout (giây) cho mỗi request upload file lên Open Notebook.
        /// Mặc định: 300 giây (5 phút) — phù hợp cho file PDF/DOCX lớn.
        /// </summary>
        public static int OPEN_NOTEBOOK_UPLOAD_TIMEOUT_SECONDS
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["OPEN_NOTEBOOK_UPLOAD_TIMEOUT_SECONDS"], out int value);
                if (value <= 0) value = 300;
                return value;
            }
        }

        /// <summary>
        /// Model ID mặc định trong Open Notebook (SurrealDB record ID, dạng "model:xxx").
        /// Dùng khi gọi ask workflow mà không truyền model override.
        /// Lấy giá trị từ GET /api/models trên Open Notebook UI.
        /// </summary>
        public static readonly string OPEN_NOTEBOOK_DEFAULT_MODEL = ConfigurationManager.AppSettings["OPEN_NOTEBOOK_DEFAULT_MODEL"];

        /// <summary>
        /// Timeout (giây) cho mỗi request ask tới Open Notebook (LangGraph workflow).
        /// Mặc định: 120 giây — ask có thể mất vài chục giây tuỳ model.
        /// </summary>
        public static int OPEN_NOTEBOOK_ASK_TIMEOUT_SECONDS
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["OPEN_NOTEBOOK_ASK_TIMEOUT_SECONDS"], out int value);
                if (value <= 0) value = 120;
                return value;
            }
        }

        /// <summary>
        /// Cấu hình thời gian chạy job để tạo phiếu đăng ký công việc từ file excel
        /// </summary>
        public static readonly string CRON_SCHEDULE_REGIST_WORK_TICKET = ConfigurationManager.AppSettings["CRON_SCHEDULE_REGIST_WORK_TICKET"];
    }
}
