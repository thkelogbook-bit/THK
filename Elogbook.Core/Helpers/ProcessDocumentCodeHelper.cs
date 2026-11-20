namespace Elogbook.Core.Helpers
{
    /// <summary>
    /// Định nghĩa các phương thức sinh mã cho quản lý tài liệu
    /// </summary>
    public static class ProcessDocumentCodeHelper
    {
        /// <summary>
        /// Tiền tố của mã tài liệu
        /// </summary>
        public const string PREFIX = "DOC";

        /// <summary>
        /// Số ký tự định dạng số trong mã tài liệu
        /// </summary>
        public const int LENGTH = 5;

        /// <summary>
        /// Sinh mã từ số
        /// </summary>
        private static string Generate(string prefix, int number, int length)
        {
            return $"{prefix}-{number.ToString($"D{length}")}";
        }

        /// <summary>
        /// Tách phần số từ chuỗi mã
        /// DOC-00015 => 15
        /// </summary>
        public static int GetNumber(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return 0;
            }

            var index = code.LastIndexOf('-');
            if (index < 0)
            {
                return 0;
            }

            return int.Parse(code.Substring(index + 1));
        }

        /// <summary>
        /// Sinh mã tiếp theo từ mã hiện tại
        /// DOC-00015 => DOC-00016
        /// </summary>
        public static string GenerateCode(string prefix, string currentCode, int length)
        {
            return Generate(prefix, GetNumber(currentCode) + 1, length);
        }
    }
}
