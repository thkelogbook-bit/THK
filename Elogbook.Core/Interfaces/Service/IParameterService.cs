using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Thông số
    /// </summary>
    public interface IParameterService
    {
        /// <summary>
        /// Lấy danh sách nhóm thông số và thông số của nhóm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetParameters(SearchParameterForStopModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả Thông số theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetParameters(SearchParameterModel requestModel);

        /// <summary>
        /// Tìm kiếm nhanh danh mục thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel QuickSearchParameters(SearchParameterModel requestModel);

        /// <summary>
        /// Lấy dữ liệu để vẽ chart cho TSVH
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ParametersChartModel GetParametersDrawChart(SearchParameterForChartModel requestModel);

        /// <summary>
        /// Tìm kiếm một Thông số theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ParameterModel GetParameterById(int id);

        /// <summary>
        /// Thêm mới Thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddParameter(AddEditParameterModel requestModel);

        /// <summary>
        /// Cập nhật Thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateParameter(AddEditParameterModel requestModel);

        /// <summary>
        /// Xóa Thông số
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteParameter(int id, int userID);

        /// <summary>
        /// Kiểm tra Thông số có đang sử dụng để cấu hình thông số không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseParameter(int id);

        /// <summary>
        /// Kiểu tra tồn tại Thông số trong Nhà máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(AddEditParameterModel requestModel, bool isAddMode);

        /// <summary>
        /// Danh sách cương vị trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        List<CommonDataComboboxList> GetOperationPositions(int factoryID);
    }
}
