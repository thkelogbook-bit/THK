using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhóm người dùng
    /// </summary>
    public class UserGroupModel : EntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên của Nhóm người dùng
        /// </summary>
        public string UserGroupName { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Danh sách người dùng
        /// </summary>
        public List<UserInGroupModel> UserInGroups { get; set; }

        /// <summary>
        /// Danh sách tên người dùng
        /// </summary>
        public List<string> FullNames { get; set; }
    }
}
