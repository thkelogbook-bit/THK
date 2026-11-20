using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Elogbook.Business.Business
{
    /// <summary>
    /// Business tích hợp Open Notebook — hỏi AI trong notebook KnowledgeBase.
    /// Tầng này chịu trách nhiệm: validation input, exception wrap, logging.
    /// Business logic thực sự nằm trong OpenNotebookService.
    /// </summary>
    public class OpenNotebookBusiness : IOpenNotebookBusiness
    {
        private readonly IOpenNotebookService _openNotebookService;

        public OpenNotebookBusiness(IOpenNotebookService openNotebookService)
        {
            _openNotebookService = openNotebookService;
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<OpenNotebookModelItem>> GetDefaultModelAsync()
        {
            var responseModel = new ResponseModel<OpenNotebookModelItem>();

            try
            {
                var modelId = await _openNotebookService.GetDefaultLanguageModelIdAsync();

                if (string.IsNullOrWhiteSpace(modelId))
                {
                    responseModel.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    responseModel.ErrorMessages.Add(
                        "Không tìm được model mặc định. " +
                        "Hãy cấu hình OPEN_NOTEBOOK_DEFAULT_MODEL hoặc thêm model trong Open Notebook.");
                    return responseModel;
                }

                // Lấy tên hiển thị từ danh sách models
                var allModels = await _openNotebookService.GetLanguageModelsAsync();
                var matched   = allModels.FirstOrDefault(m =>
                    string.Equals(m.Id, modelId, StringComparison.OrdinalIgnoreCase));

                responseModel.Data = new OpenNotebookModelItem
                {
                    Id   = modelId,
                    Name = matched?.Name ?? modelId, // fallback dùng ID nếu không tìm thấy tên
                };
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(
                    TrackingLogLevel.ERROR,
                    Category.OpenNotebook.ToString(),
                    string.Format("GetDefaultModelAsync{0}{1}", Environment.NewLine, ex));
            }

            return responseModel;
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<List<OpenNotebookModelItem>>> GetLanguageModelsAsync()
        {
            var responseModel = new ResponseModel<List<OpenNotebookModelItem>>();

            try
            {
                responseModel.Data = await _openNotebookService.GetLanguageModelsAsync();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(
                    TrackingLogLevel.ERROR,
                    Category.OpenNotebook.ToString(),
                    string.Format("GetLanguageModelsAsync{0}{1}", Environment.NewLine, ex));
            }

            return responseModel;
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<OpenNotebookAskResponse>> AskKnowledgeBaseAsync(
            OpenNotebookAskRequest request)
        {
            var responseModel = new ResponseModel<OpenNotebookAskResponse>();

            try
            {
                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.Question))
                {
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.ErrorMessages.Add("Câu hỏi không được để trống.");
                    return responseModel;
                }

                if (request.Question.Trim().Length < 3)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.ErrorMessages.Add("Câu hỏi quá ngắn, vui lòng nhập ít nhất 3 ký tự.");
                    return responseModel;
                }

                // Normalize
                request.Question = request.Question.Trim();

                responseModel = await _openNotebookService.AskKnowledgeBaseAsync(request);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(
                    TrackingLogLevel.ERROR,
                    Category.OpenNotebook.ToString(),
                    string.Format("AskKnowledgeBaseAsync — question: {0}{1}{2}",
                        request?.Question, Environment.NewLine, ex));
            }

            return responseModel;
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<bool>> CheckConnectionAsync()
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = await _openNotebookService.CheckConnectionAsync();

                if (!responseModel.Data)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    responseModel.ErrorMessages.Add(
                        "Không thể kết nối tới Open Notebook. " +
                        "Kiểm tra OPEN_NOTEBOOK_BASE_URL và trạng thái dịch vụ.");
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(
                    TrackingLogLevel.ERROR,
                    Category.OpenNotebook.ToString(),
                    string.Format("CheckConnectionAsync{0}{1}", Environment.NewLine, ex));
            }

            return responseModel;
        }
    }
}
