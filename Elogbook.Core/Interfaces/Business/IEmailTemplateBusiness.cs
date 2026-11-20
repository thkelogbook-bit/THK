using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho email template
    /// </summary>
    public interface IEmailTemplateBusiness
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
        ResponseModel<EmailTemplateResponseModel> GetEmailTemplateById(int id);

        /// <summary>
        /// Thêm mới email template
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<EmailTemplateResponseModel> AddEmailTemplate(AddEditEmailTemplateRequestModel requestModel);

        /// <summary>
        /// Cập nhật email template
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<EmailTemplateResponseModel> UpdateEmailTemplate(AddEditEmailTemplateRequestModel requestModel);

        /// <summary>
        /// Xóa email template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteEmailTemplate(int id, int userID);

        /// <summary>
        /// Kiểm tra email template đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseEmailTemplate(int id);
    }
}
