namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tổ chức
    /// </summary>
    public class OrganizationParrentModel
    {
        /// <summary>
        /// ID của đối tượng cha, ví dụ: Phân xưởng thuộc công ty nào thì Phân xưởng sẽ chức ID của công ty đó
        /// </summary>
        public int? ParentID { get; set; }

        /// <summary>
        /// Mã tổ chức
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên Công ty, Phân xưởng, Tổ, Đội/Nhóm
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên viết tắt Công ty, Phân xưởng, Tổ, Đội/Nhóm
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Loại tổ chức
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Phân loại công ty
        /// </summary>
        public string CompanyType { get; set; }

        /// <summary>
        /// Phân loại phân xưởng(Vận hành/Sửa chữa)
        /// </summary>
        public string ProductionPlantType { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public AttachmentModel FileAttachment { get; set; }
    }
}
