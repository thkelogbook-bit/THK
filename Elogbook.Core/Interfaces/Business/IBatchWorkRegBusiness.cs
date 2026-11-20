using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa phương thức Quản lý Phiếu đăng ký công tác số lượng lớn
    /// </summary>
    public interface IBatchWorkRegBusiness
    {
        /// <summary>
        /// Tạo mới Phiếu đăng ký công tác số lượng lớn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> CreateBatchWorkReg(WorkRegAddEditModel requestModel);

        /// <summary>
        /// Tải file mẫu Phiếu đăng ký
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        HttpResponseMessage DownloadFileTemplate(string type);

        /// <summary>
        /// Tạo Phiếu công tác hàng loạt
        /// </summary>
        /// <returns></returns>
        int InsertWorkRegFromSchedule();

        /// <summary>
        /// Upload file data lên server
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<WorkRegExcelDataValidationModel> UploadFile(WorkRegUploadInfoModel requestModel);
    }
}
