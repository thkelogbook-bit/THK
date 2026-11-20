using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi Cương vị trong ca
    /// </summary>
    public interface IPositionInShiftBusiness
    {
        /// <summary>
        /// Tìm kiếm tất cả Cương vị trong ca theo Cương vị ID
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetPositionInShifts(SearchPositionInShiftModel requestModel);

        /// <summary>
        /// Tìm kiếm một Cương vị trong ca theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<PositionInShiftModel> GetPositionInShiftById(int id);

        /// <summary>
        /// Thêm mới Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddPositionInShift(AddEditPositionInShiftModel requestModel);

        /// <summary>
        /// Cập nhật Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<PositionInShiftModel> UpdatePositionInShift(AddEditPositionInShiftModel requestModel);

        /// <summary>
        /// Xóa Cương vị trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeletePositionInShift(int id, int userID);

        /// <summary>
        /// Kiểm tra Cương vị trong ca này có được ghi nhận tại nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUsePositionInShift(int id);
    }
}
