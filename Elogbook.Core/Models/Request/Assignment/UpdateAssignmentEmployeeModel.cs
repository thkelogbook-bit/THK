namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân sự thay đổi
    /// Nếu không thay đổi thì truyền object lên bình thường
    /// Nếu có thay đổi thì object giữ ID, thay đổi giá trị UserID
    /// Nếu add mới nhân sự thì truyền lên object với ID = 0 và UserID
    /// Nếu xóa nhân sự thì truyền object lên và gán flag IsRemove = true
    /// </summary>
    public class UpdateAssignmentEmployeeModel
    {
        /// <summary>
        /// Mã vị trí nhân sự
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã nhân sự
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Đánh dấu nhân sự bị xóa
        /// </summary>
        public bool IsRemove { get; set; }
    }
}
