using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Lucene;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Controller dùng để test ILuceneSearchService — IndexFile, SearchInFile và các chức năng liên quan.
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/luceneSearch")]
    public class LuceneSearchController : BaseApiController
    {
        private readonly ILuceneSearchService _luceneSearchService;

        public LuceneSearchController(ILuceneSearchService luceneSearchService)
        {
            _luceneSearchService = luceneSearchService;
        }

        // ─────────────────────────────────────────────────────────
        // INDEX OPERATIONS
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// Index nội dung 1 file vào Lucene. Nếu đã index thì sẽ update (upsert).
        /// </summary>
        /// <param name="filePath">Đường dẫn đầy đủ tới file cần index.</param>
        /// <returns>Kết quả thành công/thất bại.</returns>
        [Route("indexFile")]
        [HttpPost]
        public ResponseModel<bool> IndexFile([FromBody] string filePath)
        {
            _luceneSearchService.IndexFile(filePath);
            return new ResponseModel<bool>(true);
        }

        /// <summary>
        /// Index toàn bộ file trong folder (bao gồm sub-folder).
        /// Dùng cho re-index hoặc khởi tạo lần đầu.
        /// </summary>
        /// <param name="folderPath">Đường dẫn folder cần index.</param>
        /// <returns>Số file đã index thành công.</returns>
        [Route("indexFolder")]
        [HttpPost]
        public ResponseModel<int> IndexFolder([FromBody] string folderPath)
        {
            var count = _luceneSearchService.IndexFolder(folderPath);
            return new ResponseModel<int>(count);
        }

        /// <summary>
        /// Xoá index của 1 file khỏi Lucene.
        /// </summary>
        [Route("removeIndex")]
        [HttpDelete]
        public ResponseModel<bool> RemoveIndex([FromUri] string filePath)
        {
            // Đọc raw để tránh + bị decode thành space
            var rawFilePath = HttpUtility.ParseQueryString(Request.RequestUri.Query)["filePath"] ?? filePath;
            _luceneSearchService.RemoveIndex(rawFilePath);
            return new ResponseModel<bool>(true);
        }

        // ─────────────────────────────────────────────────────────
        // SEARCH OPERATIONS
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// Kiểm tra xem nội dung file có match với query không.
        /// </summary>
        /// <param name="query">Nội dung cần tìm kiếm.</param>
        /// <param name="filePath">Đường dẫn đầy đủ tới file cần search.</param>
        /// <returns>True nếu file chứa nội dung match, False nếu không.</returns>
        [Route("searchInFile")]
        [HttpGet]
        public ResponseModel<bool> SearchInFile([FromUri] string query, [FromUri] string filePath)
        {
            // Đọc raw query string để tránh dấu + bị decode thành space bởi ASP.NET
            var qs = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var rawFilePath = qs["filePath"] ?? filePath;
            var rawQuery = qs["query"] ?? query;

            var result = _luceneSearchService.SearchInFile(rawQuery, rawFilePath);
            return new ResponseModel<bool>(result);
        }

        /// <summary>
        /// Tìm kiếm nội dung trong tất cả file đã index thuộc 1 folder.
        /// Kết quả sắp xếp theo điểm relevance giảm dần.
        /// </summary>
        /// <param name="query">Nội dung cần tìm kiếm.</param>
        /// <param name="folderPath">Đường dẫn folder cần search.</param>
        /// <param name="maxResults">Số kết quả tối đa (mặc định 50).</param>
        /// <returns>Danh sách file match.</returns>
        [Route("searchInFolder")]
        [HttpGet]
        public ResponseModel<List<FileSearchResult>> SearchInFolder(
            [FromUri] string query,
            [FromUri] string folderPath,
            [FromUri] int maxResults = 50)
        {
            var result = _luceneSearchService.SearchInFolder(query, folderPath, maxResults);
            return new ResponseModel<List<FileSearchResult>>(result);
        }

        /// <summary>
        /// Tìm kiếm file theo tên (hỗ trợ partial match).
        /// </summary>
        /// <param name="fileName">Tên file hoặc phần tên file cần tìm.</param>
        /// <param name="maxResults">Số kết quả tối đa (mặc định 20).</param>
        /// <returns>Danh sách file match.</returns>
        [Route("searchByFileName")]
        [HttpGet]
        public ResponseModel<List<FileSearchResult>> SearchByFileName(
            [FromUri] string fileName,
            [FromUri] int maxResults = 20)
        {
            var result = _luceneSearchService.SearchByFileName(fileName, maxResults);
            return new ResponseModel<List<FileSearchResult>>(result);
        }

        /// <summary>
        /// Tìm kiếm nội dung có phân trang.
        /// folderPath để trống thì search toàn bộ index.
        /// </summary>
        /// <param name="query">Nội dung cần tìm kiếm.</param>
        /// <param name="folderPath">Đường dẫn folder (để trống = search toàn bộ).</param>
        /// <param name="pageNumber">Số trang (bắt đầu từ 1).</param>
        /// <param name="pageSize">Số kết quả mỗi trang.</param>
        /// <returns>Kết quả phân trang.</returns>
        [Route("searchPaged")]
        [HttpGet]
        public ResponseModel<PagedSearchResult> SearchPaged(
            [FromUri] string query,
            [FromUri] string folderPath = null,
            [FromUri] int pageNumber = 1,
            [FromUri] int pageSize = 20)
        {
            var result = _luceneSearchService.SearchPaged(query, folderPath, pageNumber, pageSize);
            return new ResponseModel<PagedSearchResult>(result);
        }

        // ─────────────────────────────────────────────────────────
        // STATUS OPERATIONS
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// Kiểm tra trạng thái index của 1 file: đã index chưa, thời điểm index gần nhất.
        /// </summary>
        [Route("indexStatus")]
        [HttpGet]
        public ResponseModel<FileIndexStatus> GetIndexStatus([FromUri] string filePath)
        {
            var rawFilePath = HttpUtility.ParseQueryString(Request.RequestUri.Query)["filePath"] ?? filePath;
            var result = _luceneSearchService.GetIndexStatus(rawFilePath);
            return new ResponseModel<FileIndexStatus>(result);
        }

        // ─────────────────────────────────────────────────────────
        // DEBUG OPERATIONS — chỉ dùng khi test, không dùng production
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// [DEBUG] Extract text từ file và trả về raw content.
        /// </summary>
        [Route("debug/extractText")]
        [HttpGet]
        public ResponseModel<ExtractTextDebugResult> DebugExtractText([FromUri] string filePath)
        {
            var rawFilePath = HttpUtility.ParseQueryString(Request.RequestUri.Query)["filePath"] ?? filePath;
            var result = _luceneSearchService.DebugExtractText(rawFilePath);
            return new ResponseModel<ExtractTextDebugResult>(result);
        }

        /// <summary>
        /// [DEBUG] Liệt kê toàn bộ file đã index — kiểm tra path được lưu trong index.
        /// </summary>
        [Route("debug/allIndexedFiles")]
        [HttpGet]
        public ResponseModel<IndexedFileListResult> GetAllIndexedFiles([FromUri] int maxResults = 1000)
        {
            var result = _luceneSearchService.GetAllIndexedFiles(maxResults);
            return new ResponseModel<IndexedFileListResult>(result);
        }

        /// <summary>
        /// [DEBUG] Re-index file rồi search luôn — bypass mọi vấn đề cache/path.
        /// Dùng để xác nhận pipeline index→search hoạt động end-to-end.
        /// </summary>
        [Route("debug/indexAndSearch")]
        [HttpPost]
        public ResponseModel<IndexAndSearchDebugResult> DebugIndexAndSearch([FromBody] IndexAndSearchDebugRequest request)
        {
            var result = _luceneSearchService.DebugIndexAndSearch(request.FilePath, request.Query);
            return new ResponseModel<IndexAndSearchDebugResult>(result);
        }
    }
}
