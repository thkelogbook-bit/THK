using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackupDataService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        UploadFileModel BackupData();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        ResponseBaseModel Find(BaseListFilter filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool Delete(int id, int userID);
    }
}
