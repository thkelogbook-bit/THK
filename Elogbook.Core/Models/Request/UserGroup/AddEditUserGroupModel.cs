using DocumentFormat.OpenXml.Spreadsheet;
using Elogbook.Core.Mapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin thêm mới
    /// </summary>
    public class AddEditUserGroupModel
    {
        /// <summary>
        /// Key Update
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên của nhóm người dùng
        /// </summary>
        public string UserGroupName { get; set; }

        /// <summary>
        /// Danh sách tài khoản người dùng
        /// </summary>
        public List<UserAccountsModel> UserAccounts { get; set; }

        /// <summary>
        /// Trạng thái sử dụng (True: được sử dụng, False: Không được sử dụng)
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
