using Elogbook.Core.Models;
using Elogbook.Core.Models.PMIS;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// PMIS
    /// </summary>
    public interface IPmisService
    {
        /// <summary>
        /// Tìm kiếm tất cả Thông số theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetParameters(SearchParameterModel requestModel);

        /// <summary>
        /// Danh sách thông số PMIS
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetParameterPmiss(SearchParameterPmisModel requestModel);

        /// <summary>
        /// Lấy danh sách nhóm thông số PMIS
        /// </summary>
        /// <param name="isHasAll"></param>
        /// <returns></returns>
        List<SelectboxInfoModel> GetParameterGroups(bool isHasAll);

        /// <summary>
        /// Cập nhật Thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateParameter(AddEditParameterModel requestModel);

        /// <summary>
        /// Test kết nối đến database server PMIS
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool TestConnectionDatabaseSyncPMIS(DatabaseConnectionInfoModel requestModel);

        /// <summary>
        /// Lưu thông tin connection database PMIS
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool SaveConnectionDatabasePMIS(DatabaseConnectionInfoModel requestModel);

        /// <summary>
        /// Load thông tin kết nối database server PMIS
        /// </summary>
        /// <returns></returns>
        DatabaseConnectionInfoModel LoadConnectionDatabasePMIS();

        /// <summary>
        /// Tạo table để lưu dữ liệu PMIS theo năm
        /// </summary>
        /// <returns></returns>
        int AutoCreateTableByYear();

        /// <summary>
        /// Đồng bộ dữ liệu sang PMIS
        /// </summary>
        /// <param name="timeGetLatestPramaterUpdated"></param>
        void SycnDataForPMIS(int timeGetLatestPramaterUpdated = -2);
    }
}
