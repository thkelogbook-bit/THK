using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.OperationShift;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Quản lý ca vận hành
    /// </summary>
    public interface IOperationShiftBusiness
    {
        /// <summary>
        /// Tìm kiếm ca vận hành theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationShifts(SearchOperationShiftModel requestModel);

        /// <summary>
        /// Tìm kiếm ca vận hành theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<OperationShiftModel> GetOperationShiftById(int id);

        /// <summary>
        /// Thêm mới ca vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<OperationShiftModel> AddOperationShift(AddEditOperationShiftModel requestModel);

        /// <summary>
        /// Cập nhật ca vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<OperationShiftModel> UpdateOperationShift(AddEditOperationShiftModel requestModel);

        /// <summary>
        /// Xóa ca vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteOperationShift(int id, int userID);

        /// <summary>
        /// Kiểm tra ca vận hành đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseOperationShift(int id);
    }
}
