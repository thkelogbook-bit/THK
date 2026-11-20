using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức cho email template
    /// </summary>
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Tìm kiếm email template theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetEmailTemplates(SearchEmailTemplateRequestModel requestModel);

        /// <summary>
        /// Thông tin của email template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EmailTemplateResponseModel GetEmailTemplateById(int id);

        /// <summary>
        /// Thêm mới email template
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        EmailTemplateResponseModel AddEmailTemplate(AddEditEmailTemplateRequestModel requestModel);

        /// <summary>
        /// Cập nhật email template
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        EmailTemplateResponseModel UpdateEmailTemplate(AddEditEmailTemplateRequestModel requestModel);

        /// <summary>
        /// Xóa email template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteEmailTemplate(int id, int userID);

        /// <summary>
        /// Kiểm tra email template đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseEmailTemplate(int id);

        /// <summary>
        /// Kiểm tra email template đã tồn tại trong nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(AddEditEmailTemplateRequestModel requestModel, bool isAddMode);

        /// <summary>
        /// lấy email mẫu theo mã module và notification
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        EmailTemplateResponseModel GetTemplateByModuleAndNotification(int moduleId, int notificationId);
    }
}
