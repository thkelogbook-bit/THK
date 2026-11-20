namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức Import dữ liệu từ DCS
    /// </summary>
    public interface IDcsImportDataService
    {
        /// <summary>
        /// Cập nhật dữ liệu thông số từ DCS sang Elogbook
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        bool ImportDataFromExcelFiles(int day);

    }
}
