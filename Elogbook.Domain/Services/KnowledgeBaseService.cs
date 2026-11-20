using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Helpers;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Elogbook.Domain.Services
{
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly ILuceneSearchService _luceneSearchService;
        private readonly IOpenNotebookService _openNotebookService;

        private static readonly object _lockAdd    = new object();
        private static readonly object _lockUpdate = new object();
        private static readonly object _lockDelete = new object();

        public KnowledgeBaseService(
            ElogbookUnitOfWork unitOfWork,
            ICommonService commonService,
            ILuceneSearchService luceneSearchService,
            IOpenNotebookService openNotebookService)
        {
            _unitOfWork           = unitOfWork;
            _commonService        = commonService;
            _luceneSearchService  = luceneSearchService;
            _openNotebookService  = openNotebookService;
        }

        public List<KnowledgeBaseSourceItem> GetAllSources()
        {
            var knowledgeBaseRepo = _unitOfWork.ElogbookRepository<KnowledgeBase>();
            var attachmentRepo    = _unitOfWork.ElogbookRepository<Attachment>();

            // Chỉ lấy tài liệu đang active và chưa xoá
            var items = knowledgeBaseRepo
                .GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                .Select(x => new
                {
                    x.ID,
                    x.ProcessDocumentCode,
                    x.Title,
                    x.AttachmentID,
                })
                .OrderBy(x => x.ProcessDocumentCode)
                .ToList();

            if (!items.Any())
                return new List<KnowledgeBaseSourceItem>();

            // Batch-load attachments một lần
            var attIds = items
                .Where(x => x.AttachmentID.HasValue)
                .Select(x => x.AttachmentID.Value)
                .Distinct()
                .ToList();

            var attachmentMap = attachmentRepo
                .GetQuery(x => !x.IsDeleted && attIds.Contains(x.ID))
                .Select(x => new { x.ID, x.FileName, x.FilePath, x.OpenNotebookSourceId })
                .ToList()
                .ToDictionary(x => x.ID);

            var result = items.Select(item =>
            {
                string fileName             = null;
                string filePath             = null;
                string openNotebookSourceId = null;

                if (item.AttachmentID.HasValue
                    && attachmentMap.TryGetValue(item.AttachmentID.Value, out var att))
                {
                    fileName             = att.FileName;
                    filePath             = _commonService.GetPathFile(att.FilePath);
                    openNotebookSourceId = att.OpenNotebookSourceId;
                }

                return new KnowledgeBaseSourceItem
                {
                    ID                   = item.ID,
                    ProcessDocumentCode  = item.ProcessDocumentCode,
                    Title                = item.Title,
                    FileName             = fileName,
                    FilePath             = filePath,
                    OpenNotebookSourceId = openNotebookSourceId,
                };
            }).ToList();

            return result;
        }

        public ResponseBaseModel SearchFullText(KnowledgeBaseRequestModel requestModel)
        {
            var repository = _unitOfWork.ElogbookRepository<KnowledgeBase>();
            List<int> attIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText)) {
                // Tìm kiếm các tệp đính kèm có chứa từ khóa tìm kiếm
                attIds = SearchContentFiles(requestModel.SearchText);
            }


            var entities = repository.GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active);

            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.Context.Contains(requestModel.SearchText) || (x.AttachmentID != null && attIds.Contains(x.AttachmentID.Value)));
            }

            // Tìm kiếm và paging dữ liệu
            var page = entities.Select(
                e => new KnowledgeBaseModel()
                {
                    ID = e.ID,
                    ProcessDocumentCode = e.ProcessDocumentCode,
                    Title = e.Title,
                    Context = e.Context,
                    CreateDate = e.InsertedAt,
                    AttachmentID = e.AttachmentID,
                    Active = e.Active,
                }).ToList();

            if (page.Count > 0)
            {
                foreach (var item in page)
                {
                    item.FileAttachment = GetAttachmentInfo(item.AttachmentID.GetValueOrDefault());
                }

                // Mặc định sắp xếp giảm dần theo mã tài liệu
                if (string.IsNullOrWhiteSpace(requestModel.SortColumn))
                {
                    List<KnowledgeBaseModel> dataResult = page.OrderBy(x => x.ProcessDocumentCode).ToList();
                    return HttpResponse.OkList(dataResult.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
                }
            }

            return HttpResponse.OkList(page);
        }

        private List<int> SearchContentFiles(string query)
        {
            try {
                var matchedPaths = new List<int>();
                List<int> attIds = _unitOfWork.ElogbookRepository<KnowledgeBase>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.AttachmentID != null).Select(x => x.AttachmentID.Value).ToList();
                if (attIds.Count > 0)
                {
                    var atts = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.FilePath != null && !x.IsDeleted && attIds.Contains(x.ID)).ToList();
                    List<string> filePaths = atts.Select(x => Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, x.FilePath)).ToList();
                    var luceneResults = _luceneSearchService.SearchInFiles(query, filePaths);
                    matchedPaths = atts.Where(x => luceneResults.Any(y => y.Contains(x.FilePath))).Select(x => x.ID).ToList();
                }
                return matchedPaths;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, $"SearchContentFiles: Lỗi tìm kiếm nội dung tệp — {query}", ex);
                return new List<int>();
            }
        }

        public ResponseBaseModel GetKnowledgeBases(KnowledgeBaseRequestModel requestModel)
        {
            var repository = _unitOfWork.ElogbookRepository<KnowledgeBase>();

            var entities = repository.GetQuery(x => !x.IsDeleted);

            if (requestModel.Active > 0)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.Title.Contains(requestModel.SearchText));
            }

            if (!string.IsNullOrWhiteSpace(requestModel.ModuleValue))
            {
                entities = entities.Where(x => x.ModuleValue == requestModel.ModuleValue);
            }

            var masterDetail = entities;

            var dataGroup = entities
                .Select(e => new KnowledgeBaseModel
                {
                    ProcessDocumentCode = e.ProcessDocumentCode
                })
                .Distinct()
                .OrderBy(x => x.ProcessDocumentCode)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            var results = dataGroup
                .Select(item => CreateKnowledgeBaseGroup(item.ProcessDocumentCode, masterDetail))
                .ToList();

            LoadAttachments(results);

            return HttpResponse.OkList(
                results,
                requestModel.PageNumber,
                requestModel.PageSize,
                dataGroup.TotalRecords,
                dataGroup.TotalPages);
        }

        public KnowledgeBaseModel GetKnowledgeBaseById(int id)
        {
            var repository = _unitOfWork.ElogbookRepository<KnowledgeBase>();
            // Lấy thông tin của tài liệu
            var knowledgeBase = repository.GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(
                e => new KnowledgeBaseModel()
                {
                    ID = e.ID,
                    ProcessDocumentCode = e.ProcessDocumentCode,
                    Title = e.Title,
                    Context = e.Context,
                    AttachmentID = e.AttachmentID,
                    Active = e.Active,
                    ModuleValue = e.ModuleValue,
                }).FirstOrDefault();

            if (knowledgeBase != null)
            {
                knowledgeBase.FileAttachment = GetAttachmentInfo(knowledgeBase.AttachmentID.GetValueOrDefault());
            }

            return knowledgeBase;
        }

        public int AddKnowledgeBase(AddEditKnowledgeBaseModel requestModel)
        {
            lock (_lockAdd)
            {
                var repository = _unitOfWork.ElogbookRepository<KnowledgeBase>();

                var entity = new KnowledgeBase
                {
                    Title = Normalize(requestModel.Title),
                    Context = Normalize(requestModel.Context),
                    AttachmentID = requestModel.AttachmentID,
                    VersionNumber = 1,
                    Active = GetActiveStatus(requestModel.Active),
                    ModuleValue = requestModel.ModuleValue,
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                };

                var maxProcessDocumentCode = repository.GetQuery()
                    .OrderByDescending(x => x.ProcessDocumentCode)
                    .Select(x => x.ProcessDocumentCode)
                    .FirstOrDefault();

                entity.ProcessDocumentCode = ProcessDocumentCodeHelper.GenerateCode(ProcessDocumentCodeHelper.PREFIX,
                    maxProcessDocumentCode,
                    ProcessDocumentCodeHelper.LENGTH);

                repository.Add(entity);
                // Nếu có tệp đính kèm thì index nội dung tệp đính kèm vào lucene
                var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == requestModel.AttachmentID).FirstOrDefault();
                if (attachment != null && !string.IsNullOrWhiteSpace(attachment.FilePath))
                {
                    _luceneSearchService.IndexFile(Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, attachment.FilePath));
                }

                var newId = _unitOfWork.Complete() > 0 ? entity.ID : 0;

                // Upload file lên Open Notebook và lưu SourceId vào Attachment
                if (newId > 0 && attachment != null && !string.IsNullOrWhiteSpace(attachment.FilePath))
                {
                    var fullPath     = Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, attachment.FilePath);
                    var displayTitle = string.Format("{0} — {1}", entity.ProcessDocumentCode, entity.Title);
                    var attachmentId = attachment.ID;
                    Task.Run(async () =>
                    {
                        var sourceId = await _openNotebookService.UploadKnowledgeBaseFileAndGetIdAsync(
                            fullPath, attachment.FileName, displayTitle);
                        if (!string.IsNullOrWhiteSpace(sourceId))
                        {
                            var attRepo = _unitOfWork.ElogbookRepository<Attachment>();
                            var att     = attRepo.GetQuery(x => x.ID == attachmentId).FirstOrDefault();
                            if (att != null)
                            {
                                att.OpenNotebookSourceId = sourceId;
                                attRepo.Update(att);
                                _unitOfWork.Complete();
                            }
                        }
                    });
                }

                return newId;
            }
        }

        public bool UpdateKnowledgeBase(AddEditKnowledgeBaseModel requestModel)
        {
            lock (_lockUpdate)
            {
                var repository = _unitOfWork.ElogbookRepository<KnowledgeBase>();

                var entity = repository.GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                if (entity != null)
                {
                    var isAdded = false;
                    // Nếu có thay đổi về nội dung vắn tắt hoặc tệp đính kèm thì tạo record mới
                    if (requestModel.Context != entity.Context
                        || requestModel.AttachmentID.GetValueOrDefault() > 0)
                    {
                        var oldAttachmentID = entity.AttachmentID.GetValueOrDefault();
                        var attachmentID = requestModel.AttachmentID.GetValueOrDefault() > 0
                            ? requestModel.AttachmentID : entity.AttachmentID;

                        if (requestModel.AttachmentID.GetValueOrDefault() == -1)
                        {
                            attachmentID = null;
                        }

                        var newEntity = new KnowledgeBase
                        {
                            Title = Normalize(requestModel.Title),
                            Context = Normalize(requestModel.Context),
                            AttachmentID = attachmentID,
                            VersionNumber = entity.VersionNumber + 1,
                            Active = GetActiveStatus(requestModel.Active),
                            ProcessDocumentCode = entity.ProcessDocumentCode,
                            ModuleValue = requestModel.ModuleValue,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                        };

                        isAdded = true;
                        repository.Add(newEntity);

                        // Nếu có thay đổi về tệp đính kèm thì index nội dung tệp đính kèm vào lucene
                        if (requestModel.AttachmentID.GetValueOrDefault() > 0 && requestModel.AttachmentID != oldAttachmentID)
                        {
                            // Nếu có tệp đính kèm cũ thì xóa index lucene của tệp cũ
                            var attachmentOld = _unitOfWork.ElogbookRepository<Data.Attachment>().GetQuery(x => !x.IsDeleted && x.ID == oldAttachmentID).FirstOrDefault();
                            if (attachmentOld != null && !string.IsNullOrWhiteSpace(attachmentOld.FilePath))
                            {
                                _luceneSearchService.RemoveIndex(attachmentOld.FilePath);
                            }
                            // Index tệp đính kèm mới
                            var attachment = _unitOfWork.ElogbookRepository<Data.Attachment>().GetQuery(x => !x.IsDeleted && x.ID == attachmentID).FirstOrDefault();
                            if (attachment != null && !string.IsNullOrWhiteSpace(attachment.FilePath))
                            {
                                _luceneSearchService.IndexFile(attachment.FilePath);

                                // Upload file mới lên Open Notebook và lưu SourceId vào Attachment
                                var fullPath     = Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, attachment.FilePath);
                                var displayTitle = string.Format("{0} — {1} (v{2})",
                                    newEntity.ProcessDocumentCode, newEntity.Title, newEntity.VersionNumber);
                                var attachmentId = attachment.ID;
                                Task.Run(async () =>
                                {
                                    var sourceId = await _openNotebookService.UploadKnowledgeBaseFileAndGetIdAsync(
                                        fullPath, attachment.FileName, displayTitle);
                                    if (!string.IsNullOrWhiteSpace(sourceId))
                                    {
                                        var attRepo = _unitOfWork.ElogbookRepository<Attachment>();
                                        var att     = attRepo.GetQuery(x => x.ID == attachmentId).FirstOrDefault();
                                        if (att != null)
                                        {
                                            att.OpenNotebookSourceId = sourceId;
                                            attRepo.Update(att);
                                            _unitOfWork.Complete();
                                        }
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        entity.Title = Normalize(requestModel.Title);
                        entity.Active = GetActiveStatus(requestModel.Active);
                        entity.ModuleValue = requestModel.ModuleValue;
                        entity.LastUpdatedUserID = requestModel.CurrentUserID;

                        repository.Update(entity);
                    }

                    // Nếu thêm mới bản sao thì disable bản cũ
                    if (isAdded)
                    {
                        entity.Active = (int)AccessStatus.InActive;
                        entity.LastUpdatedUserID = requestModel.CurrentUserID;
                        repository.Update(entity);
                    }

                    return _unitOfWork.Complete() > 0;
                }

                return false;
            }
        }

        public bool DeleteKnowledgeBase(int id, int userID)
        {
            lock (_lockDelete)
            {
                var repository = _unitOfWork.ElogbookRepository<KnowledgeBase>();

                var entity = repository.GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
                if (entity != null)
                {
                    //Remove index lucene nếu có tệp đính kèm
                    if (entity.AttachmentID != null && entity.AttachmentID > 0)
                    {
                        var att = GetAttachmentInfo(entity.AttachmentID.Value);
                        if (att != null && !string.IsNullOrWhiteSpace(att.FilePath))
                        {
                            _luceneSearchService.RemoveIndex(att.FilePath);
                        }
                    }
                    repository.Delete(entity, userID);

                    if (_unitOfWork.Complete() > 0)
                    {
                        // Sau khi xóa thì active tài liệu gần nhất nếu tồn tại
                        var updateEntity = repository.GetQuery(x => x.ProcessDocumentCode == entity.ProcessDocumentCode
                            && !x.IsDeleted)
                            .OrderByDescending(x => x.VersionNumber).FirstOrDefault();

                        if (updateEntity != null)
                        {
                            updateEntity.Active = (int)AccessStatus.Active;
                            updateEntity.LastUpdatedUserID = userID;
                            repository.Update(updateEntity);

                            return _unitOfWork.Complete() > 0;
                        }
                    }
                }

                return false;
            }
        }

        private BaseGroupDataModel<KnowledgeBaseModel> CreateKnowledgeBaseGroup(string processDocumentCode,
            IQueryable<KnowledgeBase> masterDetail)
        {
            return new BaseGroupDataModel<KnowledgeBaseModel>
            {
                ProcessDocumentCode = processDocumentCode,
                MasterDetail = masterDetail
                    .Where(x => x.ProcessDocumentCode == processDocumentCode)
                    .Select(e => new KnowledgeBaseModel
                    {
                        ID = e.ID,
                        ProcessDocumentCode = e.ProcessDocumentCode,
                        Title = e.Title,
                        Context = e.Context,
                        CreateDate = e.InsertedAt,
                        VersionNumber = e.VersionNumber,
                        AttachmentID = e.AttachmentID,
                        Active = e.Active,
                        ModuleValue = e.ModuleValue,
                    })
                    .OrderByDescending(x => x.VersionNumber)
                    .ToList()
            };
        }

        private void LoadAttachments(IEnumerable<BaseGroupDataModel<KnowledgeBaseModel>> groups)
        {
            var attachmentIds = groups
                .SelectMany(x => x.MasterDetail)
                .Where(x => x.AttachmentID.HasValue)
                .Select(x => x.AttachmentID.Value)
                .Distinct()
                .ToList();

            if (!attachmentIds.Any())
            {
                foreach (var detail in groups.SelectMany(x => x.MasterDetail))
                {
                    detail.FileAttachment = new AttachmentModel();
                }

                return;
            }

            var attachments = _unitOfWork
                    .ElogbookRepository<Attachment>()
                    .GetQuery(x => attachmentIds.Contains(x.ID))
                    .ToList();

            var attachmentDictionary = attachments.ToDictionary(
                x => x.ID,
                x => new AttachmentModel
                {
                    ID = x.ID,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    FileType = x.FileType,
                    Description = x.Description,
                    PathThumbnail = x.PathThumbnail,
                    ScreenName = x.ScreenName,
                    ModuleValue = x.ModuleValue,
                });

            foreach (var detail in groups.SelectMany(x => x.MasterDetail))
            {
                if (detail.AttachmentID.HasValue
                    && attachmentDictionary.TryGetValue(detail.AttachmentID.Value, out var attachment))
                {
                    detail.FileAttachment = new AttachmentModel
                    {
                        ID = attachment.ID,
                        FileName = attachment.FileName,
                        FilePath = _commonService.GetPathFile(attachment.FilePath, attachment.ModuleValue),
                        PathThumbnail = "",
                        ScreenName = attachment.ScreenName,
                        FileType = attachment.FileType,
                        Description = attachment.Description,
                    };
                }
                else
                {
                    detail.FileAttachment = new AttachmentModel();
                }
            }
        }

        private AttachmentModel GetAttachmentInfo(int attachmentID)
        {
            var repository = _unitOfWork.ElogbookRepository<Attachment>();
            var attachment = repository.GetQuery(x => !x.IsDeleted && x.ID == attachmentID).FirstOrDefault();

            if (attachment != null)
            {
                return new AttachmentModel
                {
                    ID = attachment.ID,
                    FileName = attachment.FileName,
                    FilePath = _commonService.GetPathFile(attachment.FilePath),
                    PathThumbnail = "",
                    ScreenName = attachment.ScreenName,
                    FileType = attachment.FileType,
                    Description = attachment.Description,
                };
            }

            return new AttachmentModel();
        }

        private static string Normalize(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Trim();
        }

        private static int GetActiveStatus(int active)
        {
            return active <= 0
                ? (int)AccessStatus.InActive
                : active;
        }
    }
}
