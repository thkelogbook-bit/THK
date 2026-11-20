using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Nhật ký vận hành
    /// </summary>
    public interface IOperationDiaryBusiness
    {
        /// <summary>
        /// Tìm kiếm nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationDiarys(SearchOperationDiaryModel requestModel);

        /// <summary>
        /// Xem thông tin nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<ViewOpeartionDiaryModel> ViewOperationDiary(ViewOperationDiaryRequestModel requestModel);

        /// <summary>
        /// Lấy thông tin nhật ký vận hành theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<ViewOpeartionDiaryModel> GetOperationDiaryById(int id);

        /// <summary>
        /// Thêm mới nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddOperationDiary(AddOperationDiary requestModel);

        /// <summary>
        /// Cập nhật nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateOperationDiary(EditOperationDiary requestModel);

        /// <summary>
        /// Kiểm tra người dùng có phải là người tạo nhật ký vận hành không
        /// </summary>
        bool IsCreator(int operationDiaryID, int userID);

        /// <summary>
        /// Kiểm tra người dùng có phải là người tạo tình hình vận hành không
        /// </summary>
        bool IsCreatorByOperationSituation(int id, int userID);

        /// <summary>
        /// Kiểm tra người dùng có phải là người tạo công tác đang thực hiện không
        /// </summary>
        bool IsCreatorByActiveWork(int id, int userID);

        /// <summary>
        /// Kiểm tra người dùng có phải là người tạo tồn tại trong ca không
        /// </summary>
        bool IsCreatorByExistInShift(int id, int userID);

        /// <summary>
        /// Giao/Nhân ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<DeliveryReceiveShiftModel> DeliveryReceiveShift(AddEditDeliveryReceiveShiftModel requestModel);

        /// <summary>
        /// Xóa nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteOperationDiary(int id, int userID);

        /// <summary>
        /// tìm kiếm nhật ký vận hành theo điều kiện (Web)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationDiarysWeb(SearchOperationDiaryWebModel requestModel);

        /// <summary>
        /// Tìm kiếm nhanh thông tin NKVH
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel QuickSearchOperationDiarys(QuickSearchOperationDiaryModel requestModel);

        /// <summary>
        /// Xóa tình hình vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteOperationSituation(int id, int userID);

        /// <summary>
        /// Xóa công tác đang thực hiện
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteActiveWork(int id, int userID);

        /// <summary>
        /// Xóa tồn tại trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteExistInShift(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại ca ghi nhật ký vận hành đang mở
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        ResponseModel<ValidateOpenShiftModel> ValidateOpenShift(int factoryID, int operationPositionID);

        /// <summary>
        /// In tài liệu nhật ký vận hành ra file PDF
        /// </summary>
        /// <param name="id"></param>
        /// <param name="curentUserID"></param>
        /// <returns></returns>
        HttpResponseMessage ExportFile(int id, int curentUserID);

        /// <summary>
        /// Sync dữ liệu với mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel OperationDiarySyncData(OperationDiarySyncDataModel requestModel);

        /// <summary>
        /// Lấy nhãn nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationDiaryLabels(SearchOperationDiaryWebModel requestModel);

        /// <summary>
        /// Lấy chi tiết nhật ký vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetOperationDiaryDetails(SearchOperationDiaryWebModel requestModel);

        /// <summary>
        /// Tạo yêu cầu export nhật ký vận hành
        /// </summary>
        ResponseModel<bool> ExportOperationDiary(ExportOperationDiaryRequestModel requestModel);

        /// <summary>
        /// Lấy danh sách yêu cầu export nhật ký vận hành
        /// </summary>
        ResponseBaseModel GetExportOperationDiary(BaseListFilter requestModel);

        /// <summary>
        /// Xóa export nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteExportOperationDiary(int id, int userID);

        /// <summary>
        /// Download file từ ExportOperationDiary
        /// </summary>
        /// <param name="exportOperationDiaryId">Id của ExportOperationDiary</param>
        /// <returns>HttpResponseMessage chứa file hoặc error message</returns>
        HttpResponseMessage DownloadExportOperationDiaryFile(int exportOperationDiaryId);

        /// <summary>
        /// Xóa dữ liệu báo cáo nhật ký vận hành đã lâu ngày không sử dụng
        /// </summary>
        void DeleteReportOperationDiary();

        /// <summary>
        /// Xuất dữ liệu nhân sự ca vận hành ra file Excel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        HttpResponseMessage ExportPersonnelData(ExportOperationDiaryModel requestModel);

    }
}
