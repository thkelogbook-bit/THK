using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi Thông số vận hành
    /// </summary>
    public interface IOperationParametersBusiness
    {
        /// <summary>
        /// Tìm kiếm tất cả chu kỳ đang hoạt động theo nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        ResponseBaseModel GetCyclesManagements(int factoryID, int operationPositionID);

        /// <summary>
        /// Cập nhật Thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateOperationParameters(EditOperationParameterModel requestModel);

        /// <summary>
        /// Lấy danh sách khung giờ điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationTimeLines(SearchOperationTimeLineModel requestModel);

        /// <summary>
        /// Lấy danh sách khung giờ điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationTimeLinesMobile(SearchOperationTimeLineModel requestModel);

        /// <summary>
        /// Cập nhật khung giờ khi báo bận hoặc báo dừng máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateOperationTimeLine(EditOperationTimeLineModel requestModel);

        /// <summary>
        /// Cập nhật khung giờ khi báo bận từng phần
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateOperationTimeLine(StopOperationTimeLineModel requestModel);

        /// <summary>
        /// Lấy danh sách nhóm thông số và số lượng thông số
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <param name="cycleID"></param>
        /// <param name="operationTimeLineID"></param>
        /// <returns></returns>
        ResponseBaseModel GetParameterGroups(int factoryID, int operationPositionID, int cycleID, int operationTimeLineID);

        /// <summary>
        /// Lấy danh sách thông số cần ghi thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetParameters(SearchParametersModel requestModel);

        /// <summary>
        /// Lấy danh sách thông số vận hành đã ghi theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationParameters(SearchOperationParametersModel requestModel);

        /// <summary>
        /// Xem thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel ViewOperationParameters(ViewOperationParametersModel requestModel);

        /// <summary>
        /// Lấy danh sách thông số vận hành theo portal
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationParametersPortal(SearchOperationParametersModel requestModel);

        /// <summary>
        /// In file PDF
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(PdfExportOPModel requestModel);

        /// <summary>
        /// In file excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportOperationParametersExcel(PdfExportOPModel requestModel);

        /// <summary>
        /// Đồng bộ dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel OperationParameterSyncData(OperationParametersSyncDataModel requestModel);

        /// <summary>
        /// Tạo danh sách thông số vận hành theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddOperationParameterByTime(AddOperationParameterByTimeModel requestModel);

        /// <summary>
        /// Lấy danh sách khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetTimeLineLabels(SearchOperationTimeLineModel requestModel);

        /// <summary>
        /// Lấy danh sách thông số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetTimeLineDetail(SearchOperationTimeLineDetailModel requestModel);

        /// <summary>
        /// Kiểm tra người dùng có phải là người tạo khung giờ không
        /// </summary>
        bool IsCreator(int operationTimeLineID, int userID);

        /// <summary>
        /// Đồng bộ dữ liệu TSVH Elogbok sang PMIS
        /// </summary>
        void SyncParametersPmis();

        /// <summary>
        /// Lấy danh sách thông vượt ngưỡng giá trị bị cảnh báo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationParameterWarning(SearchOperationParameterWarningModel requestModel);

        /// <summary>
        /// Đồng bộ dữ liệu thông số để hiển thị cảnh báo
        /// </summary>
        void SyncDataOperationParameterWarning();

        /// <summary>
        /// Lấy thông tin chi tiết cảnh báo AI của 1 thông số vận hành.
        /// Trả về thông tin thiết bị (từ Parameters) và lịch sử giá trị ghi nhận (từ OperationParameterHistory).
        /// </summary>
        /// <param name="parameterId"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationParameterWarningDetail(int parameterId);
    }
}
