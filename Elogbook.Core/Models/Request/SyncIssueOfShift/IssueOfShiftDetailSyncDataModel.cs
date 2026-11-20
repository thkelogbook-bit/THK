using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Danh sách Thông tin tồn tại trong ca
    /// </summary>
    public class IssueOfShiftDetailSyncDataModel
    {
        /// <summary>
        /// Thông tin tồn tại trong ca
        /// </summary>
        public List<IssueOfShiftDetailSyncModel> IssueOfShiftDetails { get; set; }

        /// <summary>Danh sách dữ liệu  Tồn tại trong ca  </summary>
        public List<ExistInShiftSyncModel> ExistInShiftSyncs { get; set; }
    }
}
