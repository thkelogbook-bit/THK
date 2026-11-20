using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tài khoản người dùng
    /// </summary>
    public class UserInGroupModel
    {
        /// <summary>
        /// Người dùng trong hệ thống
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Người dùng trong hệ thống
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Loại người dùng (True: Người ngoài hệ thống, False: Người nội bộ )
        /// </summary>
        public bool IsExternal { get; set; }
    }
}
