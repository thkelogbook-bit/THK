using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackupDataBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ResponseModel<UploadFileModel> BackupData();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(BaseListFilter searchModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> Delete(int id, int userID);
    }
}
