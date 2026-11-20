using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// IssueOfShiftDetailSyncDataResultModel
    /// </summary>
    public class IssueOfShiftDetailSyncDataResultModel
    {
        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        public List<MobileExistInShiftInfo> ExistInShift { get; set; }

        /// <summary>
        /// Chi tiết tồn tại trong ca
        /// </summary>
        public List<MobileIssueOfShiftDetailInfo> IssueOfShiftDetail { get; set; }
    }
}
