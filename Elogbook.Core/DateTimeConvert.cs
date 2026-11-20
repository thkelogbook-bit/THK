using System;
using System.Globalization;

namespace Elogbook.Core
{
    /// <summary>
    /// Định nghĩa các phương thức dùng cho cho kiểu ngày
    /// </summary>
    public static class DateTimeConvert
    {
        /// <summary>
        /// Chuyển đổi dữ liệu chuổi ngày tháng sang kiểu chuẩn DateTime dd/MM/yyyy
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="isToDate"></param>
        /// Nếu ToDate = MinValue thì gán = MaxValue
        /// Trường hợp do ToDate truyền vào không đúng
        /// <returns></returns>
        public static DateTime ConvertDateTimeFromString(string strDate, bool isToDate)
        {
            DateTime.TryParseExact(strDate, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);

            if (isToDate && dateTime == DateTime.MinValue)
            {
                return DateTime.MaxValue;
            }

            return dateTime == DateTime.MinValue ? System.Data.SqlTypes.SqlDateTime.MinValue.Value : dateTime;
        }

        private static readonly string[] DateTimeFormats =
        {
            "d/M/yyyy H:m",
            "d/M/yyyy H:m:s"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime ConvertDateAndTimeFromString(string strDate)
        {
            DateTime.TryParseExact(
                strDate?.Trim(),
                DateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dateTime);

            if (dateTime == DateTime.MinValue)
            {
                return DateTime.MaxValue;
            }

            return dateTime == DateTime.MinValue ? System.Data.SqlTypes.SqlDateTime.MinValue.Value : dateTime;
        }
    }
}
