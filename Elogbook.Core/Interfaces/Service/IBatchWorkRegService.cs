using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Phương thức xử lý
    /// </summary>
    public interface IBatchWorkRegService
    {
        /// <summary>
        /// Tạo mới danh sách đăng ký công việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int CreateBatchWorkReg(WorkRegAddEditModel requestModel);

        /// <summary>
        /// Tải file
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        HttpResponseMessage DownloadFileTemplate(string type);

        /// <summary>
        /// Tạo Phiếu đăng ký hàng loạt
        /// </summary>
        /// <returns></returns>
        int InsertWorkRegFromSchedule();

        /// <summary>
        /// Upload file data lên server
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        WorkRegExcelDataValidationModel UploadFile(WorkRegUploadInfoModel requestModel);
    }
}
