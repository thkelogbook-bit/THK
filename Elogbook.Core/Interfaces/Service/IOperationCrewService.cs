using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Kíp vận hành
    /// </summary>
    public interface IOperationCrewService
    {
        /// <summary>
        /// Tìm kiếm tất cả Kíp vận hành theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationCrews(SearchOperationCrewModel requestModel);

        /// <summary>
        /// Tìm kiếm Kíp vận hành theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OperationCrewModel GetOperationCrewById(int id);

        /// <summary>
        /// Thêm mới Kíp vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        OperationCrewModel AddOperationCrew(AddEditOperationCrewModel requestModel);

        /// <summary>
        /// Cập nhật Kíp vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        OperationCrewModel UpdateOperationCrew(AddEditOperationCrewModel requestModel);

        /// <summary>
        /// Xóa Kíp vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteOperationCrew(int id, int userID);

        /// <summary>
        /// Kiểm tra đã sử dụng kíp vận hành để nhập nhật ký vận hành nào chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseOperationCrew(int id);

        /// <summary>
        /// Kiểu tra tồn tại Kíp vận hành trong Nhà máy
        /// </summary>
        /// <param name="operationCrewID"></param>
        /// <param name="operationCrewName"></param>
        /// <param name="factoryID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int operationCrewID, string operationCrewName, int factoryID, bool isAddMode);
    }
}
