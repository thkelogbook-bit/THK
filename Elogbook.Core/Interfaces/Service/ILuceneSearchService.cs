using Elogbook.Core.Models.Lucene;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Service quản lý full-text search nội dung file sử dụng Lucene.NET.
    /// Cung cấp khả năng index, remove index, và search nội dung file.
    /// </summary>
    public interface ILuceneSearchService
    {
        #region Index Operations

        /// <summary>
        /// Index nội dung file vào Lucene.  Nếu file đã được index, sẽ update (upsert).
        /// </summary>
        /// <param name="filePath">Đường dẫn đầy đủ tới file cần index.</param>
        void IndexFile(string filePath);

        /// <summary>
        /// Index tất cả file hỗ trợ trong folder (và sub-folder).
        /// Dùng khi khởi tạo index lần đầu hoặc re-index.
        /// </summary>
        /// <param name="folderPath">Đường dẫn folder cần index.</param>
        /// <returns>Số file đã index thành công.</returns>
        int IndexFolder(string folderPath);

        /// <summary>
        /// Xoá index của file khi file bị xoá khỏi hệ thống.
        /// </summary>
        /// <param name="filePath">Đường dẫn đầy đủ tới file cần xoá index.</param>
        void RemoveIndex(string filePath);

        #endregion

        #region Search Operations

        /// <summary>
        /// Search nội dung trong 1 file cụ thể.
        /// </summary>
        /// <param name="query">Nội dung cần tìm kiếm.</param>
        /// <param name="filePath">Đường dẫn file cần search.</param>
        /// <returns>True nếu nội dung file match với query, ngược lại False.</returns>
        bool SearchInFile(string query, string filePath);

        /// <summary>
        /// Search nội dung trong nhiều file cụ thể.
        /// </summary>
        /// <param name="query">Nội dung cần tìm kiếm.</param>
        /// <param name="filePaths">Danh sách đường dẫn file cần search.</param>
        /// <returns>Danh sách kết quả tìm kiếm trong các file.</returns>
        List<string> SearchInFiles(string query, List<string> filePaths);

        /// <summary>
        /// Search nội dung trong tất cả file đã index thuộc folder path.
        /// </summary>
        /// <param name="query">Nội dung cần tìm kiếm.</param>
        /// <param name="folderPath">Đường dẫn folder cần search.</param>
        /// <param name="maxResults">Số kết quả tối đa trả về (mặc định 50).</param>
        /// <returns>Danh sách file match, sắp xếp theo score giảm dần.</returns>
        List<FileSearchResult> SearchInFolder(string query, string folderPath, int maxResults = 50);

        /// <summary>
        /// Search file theo tên (filename).  Hỗ trợ partial match.
        /// Dùng khi user biết tên file nhưng không nhớ nằm ở thư mục nào.
        /// </summary>
        /// <param name="fileName">Tên file hoặc phần tên file cần tìm (VD: "PCT_2026", "báo_cáo").</param>
        /// <param name="maxResults">Số kết quả tối đa trả về (mặc định 20).</param>
        /// <returns>Danh sách file match theo tên.</returns>
        List<FileSearchResult> SearchByFileName(string fileName, int maxResults = 20);

        /// <summary>
        /// Search nội dung có phân trang.
        /// Dùng khi kết quả search nhiều, cần hiển thị theo từng trang.
        /// </summary>
        /// <param name="query">Nội dung cần tìm kiếm.</param>
        /// <param name="folderPath">Đường dẫn folder cần search (null = search toàn bộ index).</param>
        /// <param name="pageNumber">Số trang (1-based, mặc định 1).</param>
        /// <param name="pageSize">Số kết quả mỗi trang (mặc định 20).</param>
        /// <returns>Kết quả phân trang gồm items, tổng số, thông tin trang.</returns>
        PagedSearchResult SearchPaged(string query, string folderPath = null, int pageNumber = 1, int pageSize = 20);

        #endregion

        #region Status Operations

        /// <summary>
        /// Kiểm tra trạng thái index của 1 file: đã index chưa, lần index gần nhất.
        /// Dùng cho troubleshooting khi search không ra kết quả.
        /// </summary>
        /// <param name="filePath">Đường dẫn file cần kiểm tra.</param>
        /// <returns>Thông tin trạng thái index, hoặc null nếu chưa index.</returns>
        FileIndexStatus GetIndexStatus(string filePath);

        #endregion

        #region Debug Operations

        /// <summary>[DEBUG] Extract text từ file, không write vào index.</summary>
        ExtractTextDebugResult DebugExtractText(string filePath);

        /// <summary>[DEBUG] Re-index file rồi search luôn — xác nhận pipeline end-to-end.</summary>
        IndexAndSearchDebugResult DebugIndexAndSearch(string filePath, string query);

        /// <summary>[DEBUG] Liệt kê toàn bộ file đã index — kiểm tra path lưu trong index.</summary>
        IndexedFileListResult GetAllIndexedFiles(int maxResults = 1000);

        #endregion
    }
}
