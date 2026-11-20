using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Lucene;using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Service full-text search nội dung file sử dụng Lucene.NET.
    /// IndexWriter được quản lý như singleton thông qua Autofac SingleInstance.
    /// Thread-safe: IndexWriter tự quản lý internal lock.
    /// </summary>
    public class LuceneSearchService : ILuceneSearchService, IDisposable
    {
        private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;


        private readonly FSDirectory _indexDirectory;
        private readonly IndexWriter _indexWriter;
        private readonly StandardAnalyzer _analyzer;
        private bool _disposed;

        /// <summary>
        /// Các extension file tài liệu hỗ trợ index.
        /// Text-based: đọc trực tiếp bằng File.ReadAllText.
        /// Binary (PDF, DOCX, XLSX): cần extract text qua thư viện chuyên dụng.
        /// </summary>
        private static readonly HashSet<string> TextBasedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".md", ".csv", ".json", ".xml", ".html", ".htm", ".rtf", ".log"
        };

        private static readonly HashSet<string> BinaryDocExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".docx", ".xlsx", ".xls"
        };

        private static readonly HashSet<string> AllSupportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Text-based
            ".txt", ".md", ".csv", ".json", ".xml", ".html", ".htm", ".rtf", ".log",
            // Binary documents
            ".pdf", ".docx", ".xlsx", ".xls"
        };

        public LuceneSearchService()
        {
            var indexPath = AppSettings.FILE_LUCENE_SERVER_DIR;
            if (!System.IO.Directory.Exists(indexPath))
            {
                System.IO.Directory.CreateDirectory(indexPath);
            }

            _indexDirectory = FSDirectory.Open(new DirectoryInfo(indexPath));
            _analyzer = new StandardAnalyzer(AppLuceneVersion);

            var config = new IndexWriterConfig(AppLuceneVersion, _analyzer)
            {
                OpenMode = OpenMode.CREATE_OR_APPEND
            };

            _indexWriter = new IndexWriter(_indexDirectory, config);

            LogCustom.LogMessage(TrackingLogLevel.INFO, $"FileSearchService: Khởi tạo thành công — index path: {indexPath}");
        }

        #region Index Operations

        /// <summary>
        /// Index nội dung file vào Lucene.  Nếu file đã được index, sẽ update (upsert).
        /// </summary>
        public void IndexFile(string filePath)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.IndexFile: File không tồn tại — {filePath}");
                return;
            }

            var extension = Path.GetExtension(filePath);
            if (!AllSupportedExtensions.Contains(extension))
            {
                LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"FileSearchService.IndexFile: File type không hỗ trợ — {extension} ({filePath})");
                return;
            }

            try
            {
                var content = ExtractTextContent(filePath, extension);
                if (string.IsNullOrWhiteSpace(content))
                {
                    LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.IndexFile: Không extract được nội dung — {filePath}");
                    return;
                }

                IndexFileInternal(filePath, content);
            }
            catch (IOException ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.IndexFile: Lỗi đọc file — {filePath}", ex);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.IndexFile: Lỗi không xác định — {filePath}", ex);
            }
        }

        /// <summary>
        /// Index tất cả file hỗ trợ trong folder (và sub-folder).
        /// Flush 1 lần duy nhất sau khi index xong batch — hiệu quả cho lần đầu khởi tạo.
        /// </summary>
        public int IndexFolder(string folderPath)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            if (!System.IO.Directory.Exists(folderPath))
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.IndexFolder: Folder không tồn tại — {folderPath}");
                return 0;
            }

            var indexedCount = 0;

            try
            {
                var files = System.IO.Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file);
                    if (!AllSupportedExtensions.Contains(extension))
                        continue;

                    try
                    {
                        var content = ExtractTextContent(file, extension);
                        if (string.IsNullOrWhiteSpace(content))
                            continue;

                        IndexFileInternal(file, content, flush: false);
                        indexedCount++;
                    }
                    catch (IOException ex)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.IndexFolder: Bỏ qua file lỗi — {file}", ex);
                    }
                }

                // Flush 1 lần sau khi index xong batch
                if (indexedCount > 0)
                {
                    _indexWriter.Flush(triggerMerge: true, applyAllDeletes: false);
                }

                LogCustom.LogMessage(TrackingLogLevel.INFO, $"FileSearchService.IndexFolder: Đã index {indexedCount} file từ {folderPath}");
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.IndexFolder: Lỗi — {folderPath}", ex);
            }

            return indexedCount;
        }

        /// <summary>
        /// Xoá index của file khi file bị xoá khỏi hệ thống.
        /// </summary>
        public void RemoveIndex(string filePath)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            try
            {
                var normalizedPath = NormalizePath(filePath);
                _indexWriter.DeleteDocuments(new Term("path", normalizedPath));
                _indexWriter.Flush(triggerMerge: false, applyAllDeletes: true);

                LogCustom.LogMessage(TrackingLogLevel.INFO, $"FileSearchService.RemoveIndex: Đã xoá index — {filePath}");
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.RemoveIndex: Lỗi xoá index — {filePath}", ex);
            }
        }

        #endregion

        #region Search Operations

        /// <summary>
        /// Search nội dung trong 1 file cụ thể.
        /// Trả về true nếu nội dung file match với query.
        /// </summary>
        public bool SearchInFile(string query, string filePath)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            try
            {
                var normalizedPath = NormalizePath(filePath);

                LogCustom.LogMessage(TrackingLogLevel.INFO, $"FileSearchService.SearchInFile: — {query} - {normalizedPath}");
                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);
                    var contentQuery = ParseContentQuery(query);

                    // Kết hợp: path phải khớp chính xác AND content phải match
                    var booleanQuery = new BooleanQuery
                    {
                        { new TermQuery(new Term("path", normalizedPath)), Occur.MUST },
                        { contentQuery, Occur.MUST }
                    };

                    var hits = searcher.Search(booleanQuery, 1);
                    return hits.TotalHits > 0;
                }
            }
            catch (ParseException ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.SearchInFile: Query parse error — {query}", ex);
                // Fallback: thử escape toàn bộ query rồi search lại
                return SearchInFileFallback(query, filePath);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.SearchInFile: Lỗi search — query='{query}', file='{filePath}'", ex);
                return false;
            }
        }

        /// <summary>
        /// Search nội dung trong nhiều file cụ thể.
        /// Dùng 1 lần mở IndexReader duy nhất cho toàn bộ danh sách.
        /// Trả về danh sách filePath của các file có nội dung match với query.
        /// </summary>
        public List<string> SearchInFiles(string query, List<string> filePaths)
        {
            ThrowIfDisposed();

            var matchedPaths = new List<string>();

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            if (filePaths == null || filePaths.Count == 0)
                return matchedPaths;

            try
            {
                var contentQuery = ParseContentQuery(query);

                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);

                    foreach (var filePath in filePaths)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.INFO, $"SearchInFiles: Tìm kiếm nội dung tệp — {query} - {filePath}");
                        if (string.IsNullOrWhiteSpace(filePath))
                            continue;

                        try
                        {
                            var normalizedPath = NormalizePath(filePath);

                            var booleanQuery = new BooleanQuery
                            {
                                { new TermQuery(new Term("path", normalizedPath)), Occur.MUST },
                                { contentQuery, Occur.MUST }
                            };

                            var hits = searcher.Search(booleanQuery, 1);
                            if (hits.TotalHits > 0)
                            {
                                matchedPaths.Add(filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.WARN,
                                $"FileSearchService.SearchInFile(List): Bỏ qua file lỗi — '{filePath}'", ex);
                        }
                    }
                }
            }
            catch (ParseException ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN,
                    $"FileSearchService.SearchInFile(List): Query parse error — '{query}'", ex);

                // Fallback: escape query rồi thử lại toàn bộ list
                return SearchInFileListFallback(query, filePaths);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR,
                    $"FileSearchService.SearchInFile(List): Lỗi — query='{query}'", ex);
            }

            return matchedPaths;
        }

        /// <summary>
        /// Search nội dung trong tất cả file đã index thuộc folder path.
        /// Trả về danh sách file match, sắp xếp theo score giảm dần.
        /// </summary>
        public List<FileSearchResult> SearchInFolder(string query, string folderPath, int maxResults = 50)
        {
            ThrowIfDisposed();

            var results = new List<FileSearchResult>();

            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            try
            {
                var normalizedFolder = NormalizePath(folderPath);

                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);
                    var contentQuery = ParseContentQuery(query);

                    // Folder filter: PrefixQuery match tất cả file trong folder và sub-folder
                    var folderQuery = new PrefixQuery(new Term("folder", normalizedFolder));

                    var booleanQuery = new BooleanQuery
                    {
                        { folderQuery, Occur.MUST },
                        { contentQuery, Occur.MUST }
                    };

                    var hits = searcher.Search(booleanQuery, maxResults);

                    foreach (var hit in hits.ScoreDocs)
                    {
                        var doc = searcher.Doc(hit.Doc);
                        results.Add(new FileSearchResult
                        {
                            FilePath = doc.Get("path"),
                            FileName = doc.Get("filename"),
                            Score = hit.Score
                        });
                    }
                }
            }
            catch (ParseException ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.SearchInFolder: Query parse error — {query}", ex);
                // Fallback: escape query rồi thử lại
                return SearchInFolderFallback(query, folderPath, maxResults);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.SearchInFolder: Lỗi search — query='{query}', folder='{folderPath}'", ex);
            }

            return results;
        }

        /// <summary>
        /// Search file theo tên (filename).  Hỗ trợ partial match.
        /// Dùng khi user biết tên file nhưng không nhớ nằm ở thư mục nào.
        /// </summary>
        public List<FileSearchResult> SearchByFileName(string fileName, int maxResults = 20)
        {
            ThrowIfDisposed();

            var results = new List<FileSearchResult>();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            try
            {
                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);

                    // Search trên field "filename" (TextField — có tokenize)
                    var parser = new QueryParser(AppLuceneVersion, "filename", _analyzer);
                    parser.DefaultOperator = Operator.AND;

                    // Escape để tránh lỗi với ký tự đặc biệt trong tên file
                    var escapedFileName = QueryParserBase.Escape(fileName);
                    var query = parser.Parse(escapedFileName);

                    var hits = searcher.Search(query, maxResults);

                    foreach (var hit in hits.ScoreDocs)
                    {
                        var doc = searcher.Doc(hit.Doc);
                        results.Add(new FileSearchResult
                        {
                            FilePath = doc.Get("path"),
                            FileName = doc.Get("filename"),
                            Score = hit.Score
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.SearchByFileName: Lỗi — fileName='{fileName}'", ex);
            }

            return results;
        }

        /// <summary>
        /// Search nội dung có phân trang.
        /// folderPath = null → search toàn bộ index.
        /// </summary>
        public PagedSearchResult SearchPaged(string query, string folderPath = null, int pageNumber = 1, int pageSize = 20)
        {
            ThrowIfDisposed();

            var result = new PagedSearchResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;

            try
            {
                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);
                    var contentQuery = ParseContentQuery(query);

                    Query finalQuery;

                    if (!string.IsNullOrWhiteSpace(folderPath))
                    {
                        // Có folder filter
                        var normalizedFolder = NormalizePath(folderPath);
                        var folderQuery = new PrefixQuery(new Term("folder", normalizedFolder));

                        finalQuery = new BooleanQuery
                        {
                            { folderQuery, Occur.MUST },
                            { contentQuery, Occur.MUST }
                        };
                    }
                    else
                    {
                        // Search toàn bộ index
                        finalQuery = contentQuery;
                    }

                    // Lấy tổng số kết quả (search với limit lớn để đếm)
                    var maxHits = pageNumber * pageSize + pageSize; // lấy đủ cho trang hiện tại
                    var hits = searcher.Search(finalQuery, maxHits);

                    result.TotalCount = hits.TotalHits;

                    // Skip + Take cho phân trang
                    var skip = (pageNumber - 1) * pageSize;
                    var take = Math.Min(pageSize, hits.ScoreDocs.Length - skip);

                    if (skip < hits.ScoreDocs.Length)
                    {
                        for (int i = skip; i < skip + take; i++)
                        {
                            var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                            result.Items.Add(new FileSearchResult
                            {
                                FilePath = doc.Get("path"),
                                FileName = doc.Get("filename"),
                                Score = hits.ScoreDocs[i].Score
                            });
                        }
                    }
                }
            }
            catch (ParseException ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.SearchPaged: Query parse error — {query}", ex);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.SearchPaged: Lỗi — query='{query}'", ex);
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra trạng thái index của 1 file.
        /// Trả về null nếu file chưa được index.
        /// </summary>
        public FileIndexStatus GetIndexStatus(string filePath)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            try
            {
                var normalizedPath = NormalizePath(filePath);

                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);
                    var query = new TermQuery(new Term("path", normalizedPath));
                    var hits = searcher.Search(query, 1);

                    if (hits.TotalHits == 0)
                    {
                        return new FileIndexStatus
                        {
                            FilePath = normalizedPath,
                            FileName = Path.GetFileName(filePath),
                            IsIndexed = false,
                            LastIndexedUtc = null,
                            FolderPath = null
                        };
                    }

                    var doc = searcher.Doc(hits.ScoreDocs[0].Doc);
                    var modifiedTicks = doc.GetField("modified")?.GetInt64Value();

                    return new FileIndexStatus
                    {
                        FilePath = doc.Get("path"),
                        FileName = doc.Get("filename"),
                        IsIndexed = true,
                        LastIndexedUtc = modifiedTicks.HasValue
                            ? new DateTime(modifiedTicks.Value, DateTimeKind.Utc)
                            : (DateTime?)null,
                        FolderPath = doc.Get("folder")
                    };
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.GetIndexStatus: Lỗi — {filePath}", ex);
                return null;
            }
        }

        #endregion

        #region Debug Operations

        /// <summary>
        /// [DEBUG] Extract text từ file, trả về raw content để troubleshoot.
        /// Không write vào index.
        /// </summary>
        public ExtractTextDebugResult DebugExtractText(string filePath)
        {
            var result = new ExtractTextDebugResult
            {
                FilePath = filePath,
                FileExists = File.Exists(filePath),
                Extension = Path.GetExtension(filePath)
            };

            if (!result.FileExists)
            {
                result.ErrorMessage = "File không tồn tại trên disk.";
                return result;
            }

            if (!AllSupportedExtensions.Contains(result.Extension))
            {
                result.ErrorMessage = $"Extension '{result.Extension}' không được hỗ trợ. Supported: {string.Join(", ", AllSupportedExtensions)}";
                return result;
            }

            try
            {
                var content = ExtractTextContent(filePath, result.Extension);

                if (string.IsNullOrWhiteSpace(content))
                {
                    result.ExtractSuccess = false;
                    result.ErrorMessage = "Extract thành công nhưng nội dung rỗng. PDF có thể là scan ảnh (image-based) hoặc có font encoding đặc biệt.";
                }
                else
                {
                    result.ExtractSuccess = true;
                    result.ExtractedLength = content.Length;
                    // Trả về 500 ký tự đầu để preview
                    result.PreviewText = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
                }
            }
            catch (Exception ex)
            {
                result.ExtractSuccess = false;
                result.ErrorMessage = $"Lỗi khi extract: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// [DEBUG] Re-index file rồi search luôn — xác nhận pipeline end-to-end.
        /// </summary>
        public IndexAndSearchDebugResult DebugIndexAndSearch(string filePath, string query)
        {
            var result = new IndexAndSearchDebugResult
            {
                RawFilePath = filePath,
                NormalizedFilePath = NormalizePath(filePath),
                RawQuery = query
            };

            try
            {
                // Bước 1: index lại file
                IndexFile(filePath);
                result.IndexSuccess = true;

                // Bước 2: kiểm tra indexStatus ngay sau khi index
                var status = GetIndexStatus(filePath);
                result.IsIndexedAfterIndex = status?.IsIndexed ?? false;
                result.PathStoredInIndex = status?.FilePath;

                // Bước 3: search
                result.SearchResult = SearchInFile(query, filePath);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// [DEBUG] Liệt kê toàn bộ file đã index — dùng để kiểm tra path lưu trong index.
        /// </summary>
        public IndexedFileListResult GetAllIndexedFiles(int maxResults = 1000)
        {
            ThrowIfDisposed();

            var result = new IndexedFileListResult();

            try
            {
                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);

                    // MatchAllDocsQuery lấy toàn bộ document trong index
                    var hits = searcher.Search(new Lucene.Net.Search.MatchAllDocsQuery(), maxResults);
                    result.TotalCount = hits.TotalHits;

                    foreach (var hit in hits.ScoreDocs)
                    {
                        var doc = searcher.Doc(hit.Doc);
                        var modifiedTicks = doc.GetField("modified")?.GetInt64Value();

                        result.Files.Add(new IndexedFileInfo
                        {
                            FilePath   = doc.Get("path"),
                            FileName   = doc.Get("filename"),
                            FolderPath = doc.Get("folder"),
                            LastIndexedUtc = modifiedTicks.HasValue
                                ? new System.DateTime(modifiedTicks.Value, System.DateTimeKind.Utc).ToString("yyyy-MM-dd HH:mm:ss")
                                : null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "FileSearchService.GetAllIndexedFiles: Lỗi", ex);
            }

            return result;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose IndexWriter và resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                _indexWriter?.Dispose();
                _analyzer?.Dispose();
                _indexDirectory?.Dispose();

                LogCustom.LogMessage(TrackingLogLevel.INFO, "FileSearchService: Disposed thành công.");
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "FileSearchService.Dispose: Lỗi khi dispose resources", ex);
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Internal method index 1 file — tách riêng để dùng chung cho IndexFile và IndexFolder.
        /// </summary>
        private void IndexFileInternal(string filePath, string content, bool flush = true)
        {
            var normalizedPath = NormalizePath(filePath);
            var folderPath = NormalizePath(Path.GetDirectoryName(filePath));

            var doc = new Document
            {
                // Path — dùng làm key cho upsert, không tokenize
                new StringField("path", normalizedPath, Field.Store.YES),

                // Folder — dùng cho search theo folder (prefix query)
                new StringField("folder", folderPath, Field.Store.YES),

                // Filename — có tokenize, cho phép search theo tên file
                new TextField("filename", Path.GetFileName(filePath), Field.Store.YES),

                // Content — field chính cho full-text search (không store để giảm size index)
                new TextField("content", content, Field.Store.NO),

                // Modified — timestamp lần index gần nhất
                new Int64Field("modified", File.GetLastWriteTimeUtc(filePath).Ticks, Field.Store.YES)
            };

            _indexWriter.UpdateDocument(new Term("path", normalizedPath), doc);

            if (flush)
            {
                _indexWriter.Flush(triggerMerge: false, applyAllDeletes: false);
            }
        }

        /// <summary>
        /// Parse query string thành Lucene Query object.
        /// Nếu query chứa Lucene operators (AND, OR, quotes) → parse nguyên bản.
        /// Nếu query là plain text → escape ký tự đặc biệt để tránh ParseException.
        /// </summary>
        private Query ParseContentQuery(string query)
        {
            var parser = new QueryParser(AppLuceneVersion, "content", _analyzer);
            parser.DefaultOperator = Operator.AND;

            // Nếu query có dấu hiệu Lucene syntax intentional → parse trực tiếp
            if (ContainsLuceneOperator(query))
            {
                return parser.Parse(query);
            }

            // Plain text → escape toàn bộ ký tự đặc biệt Lucene
            var escapedQuery = QueryParserBase.Escape(query);
            return parser.Parse(escapedQuery);
        }

        /// <summary>
        /// Fallback search trong file khi parse lỗi — escape toàn bộ query.
        /// </summary>
        private bool SearchInFileFallback(string query, string filePath)
        {
            try
            {
                var normalizedPath = NormalizePath(filePath);
                var parser = new QueryParser(AppLuceneVersion, "content", _analyzer);
                var escapedQuery = QueryParserBase.Escape(query);
                var contentQuery = parser.Parse(escapedQuery);

                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);
                    var booleanQuery = new BooleanQuery
                    {
                        { new TermQuery(new Term("path", normalizedPath)), Occur.MUST },
                        { contentQuery, Occur.MUST }
                    };

                    var hits = searcher.Search(booleanQuery, 1);
                    return hits.TotalHits > 0;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.SearchInFileFallback: Fallback cũng lỗi — {query}", ex);
                return false;
            }
        }

        /// <summary>
        /// Fallback search trong folder khi parse lỗi — escape toàn bộ query.
        /// </summary>
        private List<FileSearchResult> SearchInFolderFallback(string query, string folderPath, int maxResults)        {
            var results = new List<FileSearchResult>();

            try
            {
                var normalizedFolder = NormalizePath(folderPath);
                var parser = new QueryParser(AppLuceneVersion, "content", _analyzer);
                var escapedQuery = QueryParserBase.Escape(query);
                var contentQuery = parser.Parse(escapedQuery);

                var folderQuery = new PrefixQuery(new Term("folder", normalizedFolder));

                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);
                    var booleanQuery = new BooleanQuery
                    {
                        { folderQuery, Occur.MUST },
                        { contentQuery, Occur.MUST }
                    };

                    var hits = searcher.Search(booleanQuery, maxResults);

                    foreach (var hit in hits.ScoreDocs)
                    {
                        var doc = searcher.Doc(hit.Doc);
                        results.Add(new FileSearchResult
                        {
                            FilePath = doc.Get("path"),
                            FileName = doc.Get("filename"),
                            Score = hit.Score
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"FileSearchService.SearchInFolderFallback: Fallback cũng lỗi — {query}", ex);
            }

            return results;
        }

        /// <summary>
        /// Kiểm tra query có chứa Lucene operator intentional hay không.
        /// </summary>
        private static bool ContainsLuceneOperator(string query)
        {
            return query.Contains("\"")
                || query.Contains(" AND ")
                || query.Contains(" OR ")
                || query.Contains(" NOT ")
                || query.Contains("+")
                || query.Contains("*")
                || query.Contains("?");
        }

        /// <summary>
        /// Chuẩn hoá path về dạng lowercase, forward slash, không có trailing slash.
        /// Xử lý cả mixed slash (/ và \), double slash, và các dạng path không nhất quán.
        /// </summary>
        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            // Replace tất cả dạng backslash (kể cả \\) về forward slash trước
            var normalized = path
                .Replace("\\\\", "/")   // double backslash trước
                .Replace("\\", "/")     // single backslash
                .Replace("//", "/")     // double forward slash (trừ phần http://)
                .TrimEnd('/')
                .ToLowerInvariant();

            return normalized;
        }

        /// <summary>
        /// Extract text content từ file dựa trên extension.
        /// Text-based files: đọc trực tiếp.
        /// Binary documents (PDF, DOCX, XLSX): dùng thư viện chuyên dụng.
        /// </summary>
        private string ExtractTextContent(string filePath, string extension)
        {
            try
            {
                if (TextBasedExtensions.Contains(extension))
                {
                    return File.ReadAllText(filePath);
                }

                if (BinaryDocExtensions.Contains(extension))
                {
                    switch (extension.ToLowerInvariant())
                    {
                        case ".pdf":
                            return ExtractTextFromPdf(filePath);
                        case ".docx":
                        case ".doc":
                            return ExtractTextFromWord(filePath);

                        case ".xlsx":
                        case ".xls":
                            return ExtractTextFromExcel(filePath);
                        default:
                            return null;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.WARN, $"FileSearchService.ExtractTextContent: Lỗi extract — {filePath}", ex);
                return null;
            }
        }

        /// <summary>
        /// Extract text từ PDF sử dụng iTextSharp (project đã có sẵn).
        /// </summary>
        private string ExtractTextFromPdf(string filePath)
        {
            var sb = new System.Text.StringBuilder();

            using (var reader = new iTextSharp.text.pdf.PdfReader(filePath))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    var text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        sb.AppendLine(text);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Extract text từ Word (.docx) sử dụng NPOI (project đã có sẵn).
        /// Hỗ trợ cả .doc (HWPF) và .docx (XWPF).
        /// </summary>
        private string ExtractTextFromWord(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var doc = new NPOI.XWPF.UserModel.XWPFDocument(fs);
                var sb = new System.Text.StringBuilder();

                foreach (var paragraph in doc.Paragraphs)
                {
                    sb.AppendLine(paragraph.Text);
                }

                // Cũng lấy text từ tables
                foreach (var table in doc.Tables)
                {
                    foreach (var row in table.Rows)
                    {
                        foreach (var cell in row.GetTableCells())
                        {
                            sb.Append(cell.GetText()).Append(" ");
                        }
                        sb.AppendLine();
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Extract text từ Excel (.xlsx/.xls) sử dụng NPOI (project đã có sẵn).
        /// Đọc tất cả sheet, tất cả cell có giá trị.
        /// </summary>
        private string ExtractTextFromExcel(string filePath)
        {
            var sb = new System.Text.StringBuilder();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var workbook = NPOI.SS.UserModel.WorkbookFactory.Create(fs);

                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    var sheet = workbook.GetSheetAt(sheetIndex);
                    sb.AppendLine($"[Sheet: {sheet.SheetName}]");

                    for (int rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var row = sheet.GetRow(rowIndex);
                        if (row == null) continue;

                        for (int cellIndex = row.FirstCellNum; cellIndex < row.LastCellNum; cellIndex++)
                        {
                            var cell = row.GetCell(cellIndex);
                            if (cell == null) continue;

                            var cellValue = GetCellValueAsString(cell);
                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                sb.Append(cellValue).Append(" ");
                            }
                        }
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Chuyển giá trị cell Excel thành string (hỗ trợ tất cả cell type).
        /// </summary>
        private static string GetCellValueAsString(NPOI.SS.UserModel.ICell cell)
        {
            switch (cell.CellType)
            {
                case NPOI.SS.UserModel.CellType.String:
                    return cell.StringCellValue;
                case NPOI.SS.UserModel.CellType.Numeric:
                    return NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell)
                        ? cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss")
                        : cell.NumericCellValue.ToString();
                case NPOI.SS.UserModel.CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case NPOI.SS.UserModel.CellType.Formula:
                    try { return cell.StringCellValue; }
                    catch { return cell.NumericCellValue.ToString(); }
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Fallback search trong list file khi parse lỗi — escape toàn bộ query.
        /// </summary>
        private List<string> SearchInFileListFallback(string query, List<string> filePaths)
        {
            var matchedPaths = new List<string>();

            try
            {
                var parser = new QueryParser(AppLuceneVersion, "content", _analyzer);
                var escapedQuery = QueryParserBase.Escape(query);
                var contentQuery = parser.Parse(escapedQuery);

                using (var reader = _indexWriter.GetReader(applyAllDeletes: true))
                {
                    var searcher = new IndexSearcher(reader);

                    foreach (var filePath in filePaths)
                    {
                        if (string.IsNullOrWhiteSpace(filePath))
                            continue;

                        try
                        {
                            var normalizedPath = NormalizePath(filePath);
                            var booleanQuery = new BooleanQuery
                            {
                                { new TermQuery(new Term("path", normalizedPath)), Occur.MUST },
                                { contentQuery, Occur.MUST }
                            };

                            var hits = searcher.Search(booleanQuery, 1);
                            if (hits.TotalHits > 0)
                                matchedPaths.Add(filePath);
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.WARN,
                                $"FileSearchService.SearchInFileListFallback: Bỏ qua file lỗi — '{filePath}'", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR,
                    $"FileSearchService.SearchInFileListFallback: Fallback cũng lỗi — '{query}'", ex);
            }

            return matchedPaths;
        }

        /// <summary>
        /// Kiểm tra service đã bị dispose chưa.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LuceneSearchService));
            }
        }

        #endregion
    }
}
