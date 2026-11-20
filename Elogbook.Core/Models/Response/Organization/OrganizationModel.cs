using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tổ chức hiển thị trên portal
    /// </summary>
    public class OrganizationModel
    {
        /// <summary>
        /// Thông tin đối tượng cha
        /// </summary>
        public OrganizationParrentModel Data { get; set; }

        /// <summary>
        /// Thông tin đối tượng con
        /// </summary>
        public List<OrganizationModel> Children { get; set; }
    }
}
