using Elogbook.Core.Models;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức Import dữ liệu từ DCS
    /// </summary>
    public interface IDcsImportDataBusiness
    {
        /// <summary>
        /// Cập nhật dữ liệu thông số từ DCS sang Elogbook
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        ResponseModel<bool> ImportDataFromExcelFiles(int day);
    }
}
