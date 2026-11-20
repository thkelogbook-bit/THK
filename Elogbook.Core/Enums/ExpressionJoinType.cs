namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Định nghĩa kiểu kết hợp điều kiện truy vấn
    /// </summary>
    public enum ExpressionJoinType
    {
        /// <summary>
        /// And giữa các điều kiện với nhau
        /// </summary>
        And = 1,
        /// <summary>
        /// Điều kiện này hoặc điều kiện kia
        /// </summary>
        Or = 2,
    }
}
