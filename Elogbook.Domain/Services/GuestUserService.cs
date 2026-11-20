using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Globalization;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class GuestUserService : IGuestUserService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        public GuestUserService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        public ResponseBaseModel GetGuestUsers(SearchGuestUserModel requestModel)
        {
            // Dữ liệu người dùng ngoài hệ thống đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => !x.IsDeleted);

            // Theo từ khóa
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => (x.FullName.Contains(requestModel.SearchText)
                                                    || x.NationalID.Contains(requestModel.SearchText)
                                                    || x.ElectricRankCard.Contains(requestModel.SearchText)
                                                    || x.LaborCard.Contains(requestModel.SearchText)));
            }
            // Bậc an toàn điện
            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                entities = entities.Where(x => x.ElectricRank == requestModel.ElectricRank);
            }
            // Bậc thợ
            if (!string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                entities = entities.Where(x => x.LaborRank == requestModel.LaborRank);
            }

            // Tìm kiếm và paging dữ liệu
            var page = entities.Select(
                e => new GuestUserReponseModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    TempDateOfBirth = e.DateOfBirth,
                    NationalID = e.NationalID,
                    ElectricRankCard = e.ElectricRankCard,
                    ElectricRank = e.ElectricRank,
                    LaborCard = e.LaborCard,
                    LaborRank = e.LaborRank,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(x => x.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public GuestUserModel GetGuestUserById(int id, bool isHasAttachent = true)
        {
            var guestUser = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new GuestUserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    TempDateOfBirth = e.DateOfBirth,
                    NationalID = e.NationalID,
                    NationalAttachmentID = e.NationalAttachmentID,
                    Notes = e.Notes,
                    ElectricRankCard = e.ElectricRankCard,
                    ElectricRankStr = e.ElectricRank,
                    ElectricRankAttachmentID = e.ElectricRankAttachmentID,
                    LaborCard = e.LaborCard,
                    LaborRankStr = e.LaborRank,
                    LaborCardAttachmentID = e.LaborCardAttachmentID,
                    IsDeleted = e.IsDeleted,
                    CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    InsertedAt = e.InsertedAt,
                    LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
                    UpdatedAt = e.UpdatedAt,
                }).FirstOrDefault();

            if (guestUser != null && isHasAttachent)
            {
                // Hình ảnh số CMND/CCCD
                if (guestUser.NationalAttachmentID.HasValue && guestUser.NationalAttachmentID.Value > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == guestUser.NationalAttachmentID.Value).FirstOrDefault();
                    if (attachment != null)
                    {
                        guestUser.NationalAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }

                // Hình ảnh thẻ an toàn điện
                if (guestUser.ElectricRankAttachmentID.HasValue && guestUser.ElectricRankAttachmentID.Value > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == guestUser.ElectricRankAttachmentID.Value).FirstOrDefault();
                    if (attachment != null)
                    {
                        guestUser.ElectricRankAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }

                // Hình ảnh thẻ an toàn lao động
                if (guestUser.LaborCardAttachmentID.HasValue && guestUser.LaborCardAttachmentID.Value > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == guestUser.LaborCardAttachmentID.Value).FirstOrDefault();
                    if (attachment != null)
                    {
                        guestUser.LaborCardAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = _commonService.GetPathFile(attachment.FilePath),
                            PathThumbnail = _commonService.GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }
            }

            return guestUser;
        }

        public GuestUserModel AddGuestUser(AddEditGuestUserModel requestModel)
        {
            var entity = new GuestUser
            {
                FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName.Trim() : null,
                NationalID = !string.IsNullOrWhiteSpace(requestModel.NationalID) ? requestModel.NationalID.Trim() : null,
                NationalAttachmentID = requestModel.NationalAttachmentID,
                Notes = !string.IsNullOrWhiteSpace(requestModel.Notes) ? requestModel.Notes.Trim() : null,
                ElectricRankCard = !string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) ? requestModel.ElectricRankCard.Trim() : null,
                ElectricRank = requestModel.ElectricRank,
                ElectricRankAttachmentID = requestModel.ElectricRankAttachmentID,
                LaborCard = !string.IsNullOrWhiteSpace(requestModel.LaborCard) ? requestModel.LaborCard.Trim() : null,
                LaborRank = requestModel.LaborRank,
                LaborCardAttachmentID = requestModel.LaborCardAttachmentID,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
            };

            if (string.IsNullOrWhiteSpace(requestModel.DateOfBirth))
            {
                entity.DateOfBirth = null;
            }
            else
            {
                DateTime.TryParseExact(requestModel.DateOfBirth, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                if (dateTime != DateTime.MinValue)
                    entity.DateOfBirth = dateTime;
                else
                {
                    entity.DateOfBirth = null;
                }
            }

            _unitOfWork.ElogbookRepository<GuestUser>().Add(entity);
            if (_unitOfWork.Complete() > 0)
            {
                entity.UserName = $"EU_{entity.ID}";
                _unitOfWork.ElogbookRepository<GuestUser>().Update(entity);
                _unitOfWork.Complete();
            }

            return GetGuestUserById(entity.ID, false);
        }

        public bool UpdateGuestUser(AddEditGuestUserModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.FullName = !string.IsNullOrWhiteSpace(requestModel.FullName) ? requestModel.FullName.Trim() : null;
                entity.NationalID = !string.IsNullOrWhiteSpace(requestModel.NationalID) ? requestModel.NationalID.Trim() : null;
                entity.NationalAttachmentID = requestModel.NationalAttachmentID;
                entity.Notes = !string.IsNullOrWhiteSpace(requestModel.Notes) ? requestModel.Notes.Trim() : null;
                entity.ElectricRankCard = !string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) ? requestModel.ElectricRankCard.Trim() : null;
                entity.ElectricRank = requestModel.ElectricRank;
                entity.ElectricRankAttachmentID = requestModel.ElectricRankAttachmentID;
                entity.LaborCard = !string.IsNullOrWhiteSpace(requestModel.LaborCard) ? requestModel.LaborCard.Trim() : null;
                entity.LaborRank = requestModel.LaborRank;
                entity.LaborCardAttachmentID = requestModel.LaborCardAttachmentID;

                if (string.IsNullOrWhiteSpace(requestModel.DateOfBirth))
                {
                    entity.DateOfBirth = null;
                }
                else
                {
                    DateTime.TryParseExact(requestModel.DateOfBirth, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                    if (dateTime != DateTime.MinValue)
                        entity.DateOfBirth = dateTime;
                    else
                    {
                        entity.DateOfBirth = null;
                    }
                }

                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<GuestUser>().Update(entity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteGuestUser(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<GuestUser>().Delete(entity, userID);
                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool CheckInUseGuestUser(int id)
        {
            var isInUse = false;
            return isInUse;
        }

        public bool CheckDuplicateData(int guestUserID, string description, bool isAddMode, int fieldType)
        {
            if (isAddMode)
            {
                switch (fieldType)
                {
                    // Số CMND/CCCD
                    case 1:
                        return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.NationalID.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn điện
                    case 2:
                        return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ElectricRankCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn lao động
                    case 3:
                        return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.LaborCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                }
            }
            else
            {
                switch (fieldType)
                {
                    // Số CMND/CCCD
                    case 1:
                        return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID != guestUserID
                                                                                    && x.NationalID.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn điện
                    case 2:
                        return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID != guestUserID
                                                                                    && x.ElectricRankCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                    // Thẻ an toàn lao động
                    case 3:
                        return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID != guestUserID
                                                                                    && x.LaborCard.Equals(description, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
                }
            }

            return false;
        }
    }
}
