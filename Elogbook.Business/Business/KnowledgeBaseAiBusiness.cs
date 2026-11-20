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
using System.Net;
using System.Threading.Tasks;

namespace Elogbook.Business.Business
{
    /// <summary>
    /// Business tích hợp Gemini AI cho KnowledgeBase
    /// </summary>
    public class KnowledgeBaseAiBusiness : IKnowledgeBaseAiBusiness
    {
        private readonly IKnowledgeBaseAiService _knowledgeBaseAiService;

        public KnowledgeBaseAiBusiness(IKnowledgeBaseAiService knowledgeBaseAiService)
        {
            _knowledgeBaseAiService = knowledgeBaseAiService;
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<List<GeminiFileUploadResult>>> UploadFilesAsync(
            IEnumerable<KnowledgeBaseAiUploadItem> files)
        {
            var responseModel = new ResponseModel<List<GeminiFileUploadResult>>();
            try
            {
                responseModel = await _knowledgeBaseAiService.UploadFilesAsync(files);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.KnowledgeBaseAiBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }

        /// <inheritdoc/>
        public async Task<ResponseModel<KnowledgeBaseAiQueryResponse>> QueryAsync(
            KnowledgeBaseAiQueryRequest request)
        {
            var responseModel = new ResponseModel<KnowledgeBaseAiQueryResponse>();
            try
            {
                responseModel = await _knowledgeBaseAiService.QueryAsync(request);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.KnowledgeBaseAiBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(request) + Environment.NewLine + ex.ToString());
            }
            return responseModel;
        }
    }
}
