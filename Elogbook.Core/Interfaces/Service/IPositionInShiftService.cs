using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Cương vị trong ca
    /// </summary>
    public interface IPositionInShiftService
    {
        /// <summary>
        /// Tìm kiếm tất cả Cương vị trong ca theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetPositionInShifts(SearchPositionInShiftModel requestModel);

        /// <summary>
        /// Tìm kiếm một Cương vị trong ca theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PositionInShiftModel GetPositionInShiftById(int id);

        /// <summary>
        /// Thêm mới Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddPositionInShift(AddEditPositionInShiftModel requestModel);

        /// <summary>
        /// Cập nhật Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        PositionInShiftModel UpdatePositionInShift(AddEditPositionInShiftModel requestModel);

        /// <summary>
        /// Xóa Cương vị trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeletePositionInShift(int id, int userID);

        /// <summary>
        /// Kiểm tra Kiểm tra Cương vị trong ca này có được ghi nhận tại nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUsePositionInShift(int id);

        /// <summary>
        /// Kiểm tra dữ liệu đã tồn tại
        /// </summary>
        /// <param name="operationPositionID"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int operationPositionID, int parentID);
    }
}
