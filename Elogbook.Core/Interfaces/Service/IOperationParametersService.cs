using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Thông số vận hành
    /// </summary>
    public interface IOperationParametersService
    {
        /// <summary>
        /// Tìm kiếm tất cả Chu kỳ đang hoạt động theo nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        ResponseBaseModel GetCyclesManagements(int factoryID, int operationPositionID);

        /// <summary>
        /// Cập nhật thông số vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateOperationParameters(EditOperationParameterModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả Khung giờ theo ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationTimeLines(SearchOperationTimeLineModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả Khung giờ theo ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationTimeLinesMobile(SearchOperationTimeLineModel requestModel);

        /// <summary>
        /// Cập nhật khung giờ khi báo bận hoặc báo dừng máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateOperationTimeLine(EditOperationTimeLineModel requestModel);

        /// <summary>
        /// Cập nhật khung giờ khi báo bận hoặc báo dừng máy từng phần
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateOperationTimeLine(StopOperationTimeLineModel requestModel);

        /// <summary>
        /// Kiểm tra tồn tại khung giờ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckExistOperationTimeline(int id);

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
        /// <param name="isView"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationParametersPortal(SearchOperationParametersModel requestModel, bool isView = false);

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
        /// Sync dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel OperationParameterSyncData(OperationParametersSyncDataModel requestModel);

        /// <summary>
        /// Cập nhật các biến count
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        void UpdateCountData(OperationParametersSyncDataModel requestModel);


        /// <summary>
        /// Lấy dữ liệu sau khi Sync
        /// </summary>
        /// <param name="lstOperationTimeLineID"></param>
        /// <returns></returns>
        ResponseBaseModel GetDataAfterSyncing(List<int> lstOperationTimeLineID);


        /// <summary>
        /// Tạo danh sách thông số vận hành theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        void AddOperationParameterByTime(AddOperationParameterByTimeModel requestModel);

        /// <summary>
        /// kiểm tra danh sách thông số vận hành theo khung giờ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel ValidateOperationParameterByTime(AddOperationParameterByTimeModel requestModel);

        /// <summary>
        /// Lấy danh sách nhãn khung giờ
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
