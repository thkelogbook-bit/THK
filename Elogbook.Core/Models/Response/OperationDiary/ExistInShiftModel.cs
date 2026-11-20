using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tồn tại trong ca
    /// </summary>
    public class ExistInShiftModel
    {
        /// <summary>
        /// Mã tồn tại trong ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public TicketInforModel RegistrationTicket { get; set; }

        /// <summary>
        /// Nhóm trưởng
        /// </summary>
        public string TeamLeader { get; set; }

        /// <summary>
        /// Nhóm trưởng
        /// </summary>
        public ValueLabelModel TeamLeaderUser { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Nội dung tồn tại
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public virtual List<AttachmentModel> FileAttachments { get; set; }

        /// <summary>
        /// Danh sách chi tiết xử lý tồn tại
        /// </summary>
        public virtual List<IssueOfShiftDetailModel> ExistInShiftDetails { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Nhóm tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Trạng thái tồn tại trong ca
        /// </summary>
        public ValueLabelModel StatusObject { get; set; }

        /// <summary>
        /// Có thể chỉnh sửa
        /// </summary>
        public Boolean IsEditable { get; set; }

        private int _status;
        /// <summary>
        /// Trạng thái tồn tại trong ca
        /// </summary>
        public int Status
        {
            get { return _status; }
            set { StatusObject = LoadStatus(value); _status = value; }
        }

        /// <summary>
        /// Giá trị hiển thị của trạng thái tồn tại trong ca
        /// </summary>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        public ValueLabelModel LoadStatus(int statusValue)
        {
            if (statusValue == 0)
            {
                return new ValueLabelModel
                {
                    Value = statusValue,
                    Label = "Chưa xử lý"
                };
            }
            else if (statusValue == 1)
            {
                return new ValueLabelModel
                {
                    Value = statusValue,
                    Label = "Đang xử lý"
                };
            }
            else if (statusValue == 2)
            {
                return new ValueLabelModel
                {
                    Value = statusValue,
                    Label = "Đã xử lý"
                };
            }
            else
            {
                return null;
            }
        }
    }
}
