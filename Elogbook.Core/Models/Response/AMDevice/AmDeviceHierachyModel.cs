using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.AmDevice
{
    /// <summary>
    /// Thông tin thiết bị
    /// </summary>
    public class AmDeviceHierachyModel
    {
        /// <summary>
        /// Thông tin đối tượng cha
        /// </summary>
        public AmDeviceModel Data { get; set; }

        /// <summary>
        /// Thông tin đối tượng con
        /// </summary>
        public List<AmDeviceHierachyModel> Children { get; set; }
    }
}
