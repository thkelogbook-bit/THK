using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Data;
using Elogbook.Domain.Models;
using ExcelDataReader;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Triển khai các phương thức cấu hình Active Directory
    /// </summary>
    public class ExportImportService : IExportImportService
    {
        #region Properties field

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly XssfHelper xssfHelper = new XssfHelper();

        #endregion

        #region Contructor

        public ExportImportService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Method
        public HttpResponseMessage ExportFileTemplate(int categoryType, int companyId)
        {
            var _rootPath = AppSettings.FILE_STORAGE_SERVER_DIR;
            try
            {
                //Create folder
                if (!Directory.Exists(Path.Combine(_rootPath, CommonConstant.FOLDER_EXCEL_TEMPLATE)))
                {
                    Directory.CreateDirectory(Path.Combine(_rootPath, CommonConstant.FOLDER_EXCEL_TEMPLATE));
                }

                string outputFile = GetFileTemplateName(categoryType, _rootPath);

                bool isCreatexlsx = CreateFileTemplate(categoryType, outputFile, companyId);

                if (isCreatexlsx)
                {
                    var fileBytes = File.ReadAllBytes(outputFile);
                    if (fileBytes.Length > 0)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                        return Download(new ByteArrayContent(fileBytes), Path.GetFileName(outputFile));
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportImportService: ExportTemplate", ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public ResponseModel<bool> ImportDataFromExcelFile(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            // Lưu vào file tạm
            if (File.Exists(requestModel.FilePath))
            {
                File.Delete(requestModel.FilePath);
            }
            File.Move(requestModel.LocalFileName, requestModel.FilePath);

            FileInfo fileInfo = new FileInfo(requestModel.FilePath);
            long fileSizeInBytes = fileInfo.Length;
            long fileSizeCheck = 1024 * 1024 * 5;

            if (fileSizeInBytes > fileSizeCheck)
            {
                responseModel.ErrorMessages.Add("Dung lượng file data vượt quá 5M");
                responseModel.Data = false;
                return responseModel;
            }

            return ImportDataProcess(requestModel, categoryType);

        }
        #endregion

        #region Private Method

        #region Import
        private ResponseModel<bool> ImportDataProcess(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            switch (categoryType)
            {
                case (int)EnumAmCategories.ReasonForStop:
                    return ImportData01(requestModel, categoryType);

                case (int)EnumAmCategories.ReasonForPending:
                case (int)EnumAmCategories.Emergency:
                case (int)EnumAmCategories.WorkType:
                    return ImportData02(requestModel, categoryType);

                case (int)EnumAmCategories.DeviceCategory:
                case (int)EnumAmCategories.Location:
                    return ImportData03(requestModel, categoryType);

                case (int)EnumAmCategories.Work:
                    return ImportData04(requestModel, categoryType);

                case (int)EnumAmCategories.Manufacturer:
                case (int)EnumAmCategories.Supplier:
                case (int)EnumAmCategories.CountryManufacture:
                case (int)EnumAmCategories.DeviceType:
                    return ImportData05(requestModel, categoryType);

                case (int)EnumAmCategories.Device:
                    return ImportData06(requestModel, categoryType);

                default:
                    responseModel.ErrorMessages.Add("Danh mục không hợp lệ");
                    responseModel.Data = false;
                    return responseModel;
            }
        }

        private ResponseModel<bool> ImportData01(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Đọc file excel data
                    using (var stream = File.Open(requestModel.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                        {
                            // Read the data from the Excel file
                            DataSet result = reader.AsDataSet();

                            // Access the first sheet
                            DataTable dataInput = result.Tables[0];

                            // Get the last row index
                            int lastRowIndex = dataInput.Rows.Count;
                            List<string> lstNames = new List<string>();

                            for (int i = 1; i < lastRowIndex; i++)
                            {
                                string inputFactory = dataInput.Rows[i][0].ToString().Trim();
                                string inputName = dataInput.Rows[i][4].ToString().Trim();
                                string inputType = dataInput.Rows[i][7].ToString().Trim();
                                string inputStatus = dataInput.Rows[i][9].ToString().Trim();
                                int status = 0;
                                int reasonType = 0;

                                //Check stop
                                if (string.IsNullOrWhiteSpace(inputFactory) && string.IsNullOrWhiteSpace(inputName)
                                    && string.IsNullOrWhiteSpace(inputStatus) && string.IsNullOrWhiteSpace(inputType))
                                {
                                    break;
                                }

                                // Check empty
                                // Tên
                                if (string.IsNullOrWhiteSpace(inputName))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Tên đang bị trống");
                                    continue;
                                }

                                // Nhà máy
                                if (string.IsNullOrWhiteSpace(inputFactory))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Nhà máy đang bị trống");
                                    continue;
                                }

                                // Trạng thái
                                if (string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Trạng thái đang bị trống");
                                    continue;
                                }

                                if (string.IsNullOrWhiteSpace(inputType))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Loại nguyên nhân đang bị trống");
                                    continue;
                                }

                                // Check factory exist
                                int factoryId = CheckExistsFactory(i, inputFactory,ref responseModel);
                                if (factoryId == 0)
                                {
                                    continue;
                                }

                                // Check name exist
                                if (!CheckExistsName(i, inputName, ref lstNames, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstNames.Add(inputName);
                                }

                                // Check Status
                                if (!CheckValidStatus(i, inputStatus, ref status, ref responseModel))
                                {
                                    continue;
                                }

                                // Check key
                                int typeKey = 0;
                                if (inputType.Equals(EnumExtensions.GetEnumDescription(AmEnumReasonType.StopAM)))
                                {
                                    typeKey = (int)AmEnumReasonType.StopAM;
                                }

                                if (inputType.Equals(EnumExtensions.GetEnumDescription(AmEnumReasonType.StopWork)))
                                {
                                    typeKey = (int)AmEnumReasonType.StopWork;
                                }

                                // Check trạng thái hợp lệ
                                if (typeKey == 0)
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Loại nguyên nhân không hợp lệ");
                                    continue;
                                }

                                // Insert Data
                                var entity = new AmReasonForStop
                                {
                                    ReasonDesc = inputName,
                                    ReasonType = reasonType,
                                    FactoryID = factoryId,
                                    Active = status,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };

                                _unitOfWork.ElogbookRepository<AmReasonForStop>().Add(entity);
                            }
                            _unitOfWork.Complete();
                        }
                    }

                    transaction.Commit();
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                catch (Exception ex)
                {
                    responseModel.ErrorMessages.Add(ex.ToString());
                    responseModel.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    responseModel.Data = false;
                    transaction.Rollback();
                    return responseModel;
                }
            }
        }

        private ResponseModel<bool> ImportData02(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Đọc file excel data
                    using (var stream = File.Open(requestModel.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                        {
                            // Read the data from the Excel file
                            DataSet result = reader.AsDataSet();

                            // Access the first sheet
                            DataTable dataInput = result.Tables[0];

                            // Get the last row index
                            int lastRowIndex = dataInput.Rows.Count;
                            List<string> lstNames = new List<string>();

                            for (int i = 1; i < lastRowIndex; i++)
                            {
                                string inputFactory = dataInput.Rows[i][0].ToString().Trim();
                                string inputName = dataInput.Rows[i][4].ToString().Trim();
                                string inputStatus = dataInput.Rows[i][7].ToString().Trim();
                                int status = 0;

                                //Check stop
                                if (string.IsNullOrWhiteSpace(inputFactory) && string.IsNullOrWhiteSpace(inputName) && string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    break;
                                }

                                // Check empty
                                // Tên
                                if (string.IsNullOrWhiteSpace(inputName))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Tên đang bị trống");
                                    continue;
                                }

                                // Nhà máy
                                if (string.IsNullOrWhiteSpace(inputFactory))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Nhà máy đang bị trống");
                                    continue;
                                }

                                // Trạng thái
                                if (string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Trạng thái đang bị trống");
                                    continue;
                                }

                                // Check factory exist
                                int factoryId = CheckExistsFactory(i, inputFactory, ref responseModel);
                                if (factoryId == 0)
                                {
                                    continue;
                                }

                                // Check name exist
                                if (!CheckExistsName(i, inputName, ref lstNames, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstNames.Add(inputName);
                                }

                                // Check Status
                                if (!CheckValidStatus(i, inputStatus, ref status, ref responseModel))
                                {
                                    continue;
                                }

                                // Insert Data
                                InsertDataToDB02(factoryId, inputName, status, requestModel.CurrentUserID, categoryType);
                            }

                            _unitOfWork.Complete();
                        }
                    }

                    transaction.Commit();
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                catch (Exception ex)
                {
                    responseModel.ErrorMessages.Add(ex.ToString());
                    responseModel.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    responseModel.Data = false;
                    transaction.Rollback();
                    return responseModel;
                }
            }
        }

        private ResponseModel<bool> ImportData03(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Đọc file excel data
                    using (var stream = File.Open(requestModel.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                        {
                            // Read the data from the Excel file
                            DataSet result = reader.AsDataSet();

                            // Access the first sheet
                            DataTable dataInput = result.Tables[0];

                            // Get the last row index
                            int lastRowIndex = dataInput.Rows.Count;
                            List<string> lstCodes = new List<string>();
                            List<string> lstNames = new List<string>();

                            for (int i = 1; i < lastRowIndex; i++)
                            {
                                string inputCompany = dataInput.Rows[i][0].ToString().Trim();
                                string inputName = dataInput.Rows[i][4].ToString().Trim();
                                string inputCode = dataInput.Rows[i][8].ToString().Trim();
                                string inputStatus = dataInput.Rows[i][10].ToString().Trim();
                                int status = 0;

                                #region Validate data input

                                if (string.IsNullOrWhiteSpace(inputCompany)
                                    && string.IsNullOrWhiteSpace(inputName)
                                    && string.IsNullOrWhiteSpace(inputStatus)
                                    && string.IsNullOrWhiteSpace(inputCode))
                                {
                                    break;
                                }

                                // Check empty
                                // Tên
                                if (string.IsNullOrWhiteSpace(inputName))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Tên đang bị trống");
                                    continue;
                                }

                                // Mã
                                if (string.IsNullOrWhiteSpace(inputCode))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Mã đang bị trống");
                                    continue;
                                }

                                // Trạng thái
                                if (string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Trạng thái đang bị trống");
                                    continue;
                                }

                                // Công ty
                                if (string.IsNullOrWhiteSpace(inputCompany))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Công ty đang bị trống");
                                    continue;
                                }

                                // Check name exist
                                if (!CheckExistsName(i, inputName, ref lstNames, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstNames.Add(inputName);
                                }

                                // Check code exist
                                if (!CheckExistsCode(i, inputCode, ref lstCodes, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstCodes.Add(inputCode);
                                }

                                // Check Status
                                if (!CheckValidStatus(i, inputStatus, ref status, ref responseModel))
                                {
                                    continue;
                                }

                                // Check company exist
                                int conpanyId = CheckExistsCompany(i, inputCompany, ref responseModel);
                                if (conpanyId == 0)
                                {
                                    continue;
                                }

                                #endregion

                                #region Insert DB
                                if (categoryType == (int)EnumAmCategories.Location)
                                {
                                    var entity = new AmLocation
                                    {
                                        OrganizationID = conpanyId,
                                        SiteDesc = inputName,
                                        SiteCode = inputCode,
                                        Active = status,
                                        CreatedUserID = requestModel.CurrentUserID,
                                        LastUpdatedUserID = requestModel.CurrentUserID,
                                    };

                                    _unitOfWork.ElogbookRepository<AmLocation>().Add(entity);
                                }

                                if (categoryType == (int)EnumAmCategories.DeviceCategory)
                                {
                                    var entity = new AmDeviceCategory
                                    {
                                        OrgID = conpanyId,
                                        CategoryName = inputName,
                                        CategoryCode = inputCode,
                                        Active = status,
                                        CreatedUserID = requestModel.CurrentUserID,
                                        LastUpdatedUserID = requestModel.CurrentUserID,
                                    };

                                    _unitOfWork.ElogbookRepository<AmDeviceCategory>().Add(entity);
                                }
                                #endregion
                            }
                            _unitOfWork.Complete();
                        }
                    }

                    transaction.Commit();
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                catch (Exception ex)
                {
                    responseModel.ErrorMessages.Add(ex.ToString());
                    responseModel.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    responseModel.Data = false;
                    transaction.Rollback();
                    return responseModel;
                }
            }
        }

        private ResponseModel<bool> ImportData04(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Đọc file excel data
                    using (var stream = File.Open(requestModel.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                        {
                            // Read the data from the Excel file
                            DataSet result = reader.AsDataSet();

                            // Access the first sheet
                            DataTable dataInput = result.Tables[1];

                            // Get the last row index
                            int lastRowIndex = dataInput.Rows.Count;
                            List<string> lstNames = new List<string>();

                            for (int i = 1; i < lastRowIndex; i++)
                            {
                                string inputFactory = dataInput.Rows[i][0].ToString().Trim();
                                string inputType = dataInput.Rows[i][4].ToString().Trim();
                                string inputName = dataInput.Rows[i][7].ToString().Trim();
                                string inputContent = dataInput.Rows[i][10].ToString().Trim();
                                string inputStatus = dataInput.Rows[i][13].ToString().Trim();
                                int status = 0;

                                //Check stop
                                if (string.IsNullOrWhiteSpace(inputFactory) && string.IsNullOrWhiteSpace(inputName)
                                    && string.IsNullOrWhiteSpace(inputStatus) && string.IsNullOrWhiteSpace(inputType))
                                {
                                    break;
                                }

                                // Check empty
                                // Tên
                                if (string.IsNullOrWhiteSpace(inputName))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Tên đang bị trống");
                                    continue;
                                }

                                // Nhà máy
                                if (string.IsNullOrWhiteSpace(inputFactory))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Nhà máy đang bị trống");
                                    continue;
                                }

                                // Trạng thái
                                if (string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Trạng thái đang bị trống");
                                    continue;
                                }

                                // Loại công việc
                                if (string.IsNullOrWhiteSpace(inputType))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Loại công việc đang bị trống");
                                    continue;
                                }

                                // Check factory exist
                                int factoryId = CheckExistsFactory(i, inputFactory, ref responseModel);
                                if (factoryId == 0)
                                {
                                    continue;
                                }

                                // Check name exist
                                if (!CheckExistsName(i, inputName, ref lstNames, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstNames.Add(inputName);
                                }

                                // Check Status
                                if (!CheckValidStatus(i, inputStatus, ref status, ref responseModel))
                                {
                                    continue;
                                }

                                // Check WorkType
                                int workTypeId = CheckExistWorkType(i, inputType, ref responseModel);
                                if (workTypeId == 0)
                                {
                                    continue;
                                }

                                // Insert Data
                                var entity = new AmWork
                                {
                                    WorkName = inputName,
                                    FactoryID = factoryId,
                                    AmWorkTypeID = workTypeId,
                                    WorkDesc = !string.IsNullOrWhiteSpace(inputContent) ? inputContent : string.Empty,
                                    Active = status,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };
                                _unitOfWork.ElogbookRepository<AmWork>().Add(entity);
                            }
                            _unitOfWork.Complete();
                        }
                    }

                    transaction.Commit();
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                catch (Exception ex)
                {
                    responseModel.ErrorMessages.Add(ex.ToString());
                    responseModel.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    responseModel.Data = false;
                    transaction.Rollback();
                    return responseModel;
                }
            }
        }

        private ResponseModel<bool> ImportData05(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Đọc file excel data
                    using (var stream = File.Open(requestModel.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                        {
                            // Read the data from the Excel file
                            DataSet result = reader.AsDataSet();

                            // Access the first sheet
                            DataTable dataInput = result.Tables[0];

                            // Get the last row index
                            int lastRowIndex = dataInput.Rows.Count;
                            List<string> lstCodes = new List<string>();
                            List<string> lstNames = new List<string>();

                            for (int i = 1; i < lastRowIndex; i++)
                            {
                                string inputName = dataInput.Rows[i][0].ToString().Trim();
                                string inputCode = dataInput.Rows[i][4].ToString().Trim();
                                string inputStatus = dataInput.Rows[i][6].ToString().Trim();
                                int status = 0;

                                //Check stop
                                if (string.IsNullOrWhiteSpace(inputName) && string.IsNullOrWhiteSpace(inputCode) && string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    break;
                                }

                                // Check empty
                                // Tên
                                if (string.IsNullOrWhiteSpace(inputName))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Tên đang bị trống");
                                    continue;
                                }

                                // Mã
                                if (string.IsNullOrWhiteSpace(inputCode))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Mã đang bị trống");
                                    continue;
                                }

                                // Trạng thái
                                if (string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Trạng thái đang bị trống");
                                    continue;
                                }

                                // Check Name exist
                                if (!CheckExistsName(i, inputName, ref lstNames, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstNames.Add(inputName);
                                }

                                // Check Code exist
                                if (!CheckExistsCode(i, inputCode, ref lstCodes, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstCodes.Add(inputName);
                                }

                                // Check Status
                                if (!CheckValidStatus(i, inputStatus, ref status, ref responseModel))
                                {
                                    continue;
                                }

                                // Insert Data
                                InsertDataToDB05(inputName, inputCode, status, requestModel.CurrentUserID, categoryType);
                            }

                            _unitOfWork.Complete();
                        }
                    }

                    transaction.Commit();
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                catch (Exception ex)
                {
                    responseModel.ErrorMessages.Add(ex.ToString());
                    responseModel.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    responseModel.Data = false;
                    transaction.Rollback();
                    return responseModel;
                }
            }
        }

        private ResponseModel<bool> ImportData06(FileUploadInfoModel requestModel, int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Đọc file excel data
                    using (var stream = File.Open(requestModel.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                        {
                            // Read the data from the Excel file
                            DataSet result = reader.AsDataSet();

                            // Access the first sheet
                            DataTable dataInput = result.Tables[0];

                            // Get the last row index
                            int lastRowIndex = dataInput.Rows.Count;
                            List<string> lstCodes = new List<string>();
                            List<string> lstNames = new List<string>();

                            for (int i = 1; i < lastRowIndex; i++)
                            {
                                #region Define value
                                string inputLocation = dataInput.Rows[i][0].ToString().Trim();
                                string inputName = dataInput.Rows[i][4].ToString().Trim();
                                string inputCode = dataInput.Rows[i][8].ToString().Trim();
                                string inputOrganization = dataInput.Rows[i][10].ToString().Trim();
                                string inputDeviceStatus = dataInput.Rows[i][13].ToString().Trim();
                                string inputStatus = dataInput.Rows[i][15].ToString().Trim();
                                string inputSerialNum = dataInput.Rows[i][17].ToString().Trim();
                                string inputAssetType = dataInput.Rows[i][20].ToString().Trim();
                                string inputMeter = dataInput.Rows[i][23].ToString().Trim();
                                string inputKss = dataInput.Rows[i][26].ToString().Trim();
                                string inputKhvh = dataInput.Rows[i][29].ToString().Trim();
                                string inputNote = dataInput.Rows[i][32].ToString().Trim();
                                string inputDescription = dataInput.Rows[i][36].ToString().Trim();
                                string inputDrawing = dataInput.Rows[i][40].ToString().Trim();
                                string inputInstallDate = dataInput.Rows[i][43].ToString().Trim();
                                string inputOperatingDate = dataInput.Rows[i][45].ToString().Trim();
                                int status = 0;
                                #endregion

                                #region Validate data input

                                if (string.IsNullOrWhiteSpace(inputLocation) && string.IsNullOrWhiteSpace(inputName)
                                    && string.IsNullOrWhiteSpace(inputCode) && string.IsNullOrWhiteSpace(inputOrganization)
                                    && string.IsNullOrWhiteSpace(inputDeviceStatus) && string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    break;
                                }

                                // Check empty
                                // Tên
                                if (string.IsNullOrWhiteSpace(inputName))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Tên đang bị trống");
                                    continue;
                                }

                                // Mã
                                if (string.IsNullOrWhiteSpace(inputCode))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Mã đang bị trống");
                                    continue;
                                }

                                // Trạng thái
                                if (string.IsNullOrWhiteSpace(inputStatus))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Trạng thái đang bị trống");
                                    continue;
                                }

                                // Khu vực
                                if (string.IsNullOrWhiteSpace(inputLocation))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Khu vực đang bị trống");
                                    continue;
                                }

                                // Tình trạng thiết bị
                                if (string.IsNullOrWhiteSpace(inputDeviceStatus))
                                {
                                    responseModel.ErrorMessages.Add("Row: " + i.ToString() + " -Tình trạng thiết bị đang bị trống");
                                    continue;
                                }

                                // Check name exist
                                if (!CheckExistsName(i, inputName, ref lstNames, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstNames.Add(inputName);
                                }

                                // Check code exist
                                if (!CheckExistsCode(i, inputCode, ref lstCodes, categoryType, ref responseModel))
                                {
                                    continue;
                                }
                                else
                                {
                                    lstCodes.Add(inputCode);
                                }

                                // Check Status
                                if (!CheckValidStatus(i, inputStatus, ref status, ref responseModel))
                                {
                                    continue;
                                }

                                // Check phân xưởng exist
                                int organizationId = CheckExistsProductionPlant(i, inputOrganization, requestModel.CurrentCompanyID, ref responseModel);
                                if (organizationId == 0)
                                {
                                    continue;
                                }

                                // Check khu vực exist
                                int locationId = CheckExistsLocation(i, inputLocation, requestModel.CurrentCompanyID, ref responseModel);
                                if (locationId == 0)
                                {
                                    continue;
                                }

                                // Check tình trạng exist
                                int deviceStatusId = CheckExistsDeviceStatus(i, inputDeviceStatus, ref responseModel);
                                if (deviceStatusId == 0)
                                {
                                    continue;
                                }

                                #endregion

                                #region Insert DB
                                DateTime? installDate = null;
                                if (!string.IsNullOrWhiteSpace(inputInstallDate))
                                {
                                    inputInstallDate = inputInstallDate.Remove(inputInstallDate.Length - 9);
                                    installDate = DateTimeConvert.ConvertDateTimeFromString(inputInstallDate, false);
                                }
                                DateTime? operatingDate = null;
                                if (!string.IsNullOrWhiteSpace(inputOperatingDate))
                                {
                                    inputOperatingDate = inputOperatingDate.Remove(inputOperatingDate.Length - 9);
                                    operatingDate = DateTimeConvert.ConvertDateTimeFromString(inputOperatingDate,false);
                                }

                                var entity = new AmDevice
                                {
                                    OrganizationID = organizationId,
                                    AmLocationID = locationId,
                                    AmDeviceStatusID = deviceStatusId,
                                    AssetCode = inputCode,
                                    AssetDesc = inputName,
                                    SerialNum = inputSerialNum,
                                    AssetType = inputAssetType,
                                    Meter = inputMeter,
                                    Kss = inputKss,
                                    Description = inputDescription,
                                    Khvh = inputKhvh,
                                    Note = inputNote,
                                    Drawing = inputDrawing,
                                    InstallDate = installDate,
                                    OperatingDate = operatingDate,
                                    Active = status,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                };

                                _unitOfWork.ElogbookRepository<AmDevice>().Add(entity);
                                #endregion
                            }
                            _unitOfWork.Complete();
                        }
                    }

                    transaction.Commit();
                    responseModel.Data = true;
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    return responseModel;
                }
                catch (Exception ex)
                {
                    responseModel.ErrorMessages.Add(ex.ToString());
                    responseModel.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    responseModel.Data = false;
                    transaction.Rollback();
                    return responseModel;
                }
            }
        }
        #endregion

        #region Method Export
        private string GetFileTemplateName(int categoryType, string _rootPath)
        {
            string currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            string fileName;
            switch (categoryType)
            {
                case (int)EnumAmCategories.WorkType:
                    fileName = CommonConstant.FILE_TEMP_WORKTYPE;
                    break;
                case (int)EnumAmCategories.Work:
                    fileName = CommonConstant.FILE_TEMP_WORK;
                    break;
                case (int)EnumAmCategories.Emergency:
                    fileName = CommonConstant.FILE_TEMP_EMERGENCY;
                    break;
                case (int)EnumAmCategories.Location:
                    fileName = CommonConstant.FILE_TEMP_LOCATION;
                    break;
                case (int)EnumAmCategories.ReasonForPending:
                    fileName = CommonConstant.FILE_TEMP_REASON_PENDING;
                    break;
                case (int)EnumAmCategories.ReasonForStop:
                    fileName = CommonConstant.FILE_TEMP_REASON_STOP;
                    break;
                case (int)EnumAmCategories.DeviceType:
                    fileName = CommonConstant.FILE_TEMP_DEVICE_TYPE;
                    break;
                case (int)EnumAmCategories.DeviceCategory:
                    fileName = CommonConstant.FILE_TEMP_DEVICE_CATEGORY;
                    break;
                case (int)EnumAmCategories.Device:
                    fileName = CommonConstant.FILE_TEMP_DEVICE;
                    break;
                case (int)EnumAmCategories.CountryManufacture:
                    fileName = CommonConstant.FILE_TEMP_COUNTRY;
                    break;
                case (int)EnumAmCategories.Supplier:
                    fileName = CommonConstant.FILE_TEMP_SUPPLIER;
                    break;
                case (int)EnumAmCategories.Manufacturer:
                    fileName = CommonConstant.FILE_TEMP_MANUFACTURE;
                    break;
                default:
                    fileName = "TemplateFile";
                    break;
            }

            string outputFile = Path.Combine(_rootPath, CommonConstant.FOLDER_EXCEL_TEMPLATE, $"{fileName}_{currentDate}.xlsx");

            return outputFile;
        }

        private bool CreateFileTemplate(int categoryType, string outputFile, int companyId)
        {
            switch (categoryType)
            {
                case (int)EnumAmCategories.ReasonForStop:
                    return GenerateFile01(outputFile);

                case (int)EnumAmCategories.ReasonForPending:
                case (int)EnumAmCategories.Emergency:
                case (int)EnumAmCategories.WorkType:
                    return GenerateFile02(outputFile, categoryType);

                case (int)EnumAmCategories.DeviceCategory:
                case (int)EnumAmCategories.Location:
                    return GenerateFile03(outputFile, categoryType);

                case (int)EnumAmCategories.Work:
                    return GenerateFile04(outputFile);

                case (int)EnumAmCategories.Manufacturer:
                case (int)EnumAmCategories.Supplier:
                case (int)EnumAmCategories.CountryManufacture:
                case (int)EnumAmCategories.DeviceType:
                    return GenerateFile05(outputFile, categoryType);

                case (int)EnumAmCategories.Device:
                    return GenerateFile06(outputFile, companyId);
                default:
                    return false;
            }
        }

        private bool GenerateFile01(string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();

            try
            {
                var factoryData = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted).Select(t => t.FactoryName).ToList();

                string[] factories = factoryData.ToArray();
                string[] statusKeys = new string[] {
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply),
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply)
                };
                string[] typeKeys = new string[] {
                    EnumExtensions.GetEnumDescription(AmEnumReasonType.StopAM),
                    EnumExtensions.GetEnumDescription(AmEnumReasonType.StopWork)
                };
                ISheet tempDataSheet = workbook.CreateSheet("Data");

                IRow headerData = tempDataSheet.CreateRow(0);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 0, 3), headerData, "Nhà máy");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 4, 6), headerData, "Tên nguyên nhân");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 7, 8), headerData, "Loại nguyên nhân");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 9, 10), headerData, "Trạng thái");
                xssfHelper.SetStyleTableHeader(tempDataSheet, new CellRangeAddress(0, 0, 0, 10), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                IRow bodyData = tempDataSheet.CreateRow(1);
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 0, 3), factories);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 4, 6), bodyData, "Tên xxxx");
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 7, 8), typeKeys);
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 9, 10), statusKeys);

                for (int i = 2; i <= 100; i++)
                {
                    IRow bodyTempData = tempDataSheet.CreateRow(i);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 0, 3), factories);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 4, 6), bodyTempData, string.Empty);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 7, 8), typeKeys);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 9, 10), statusKeys);
                }
                xssfHelper.SetStyleTableBody(tempDataSheet, new CellRangeAddress(1, 100, 0, 10), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                xssfHelper.SetStyleFinish(tempDataSheet, new CellRangeAddress(0, 100, 0, 10), 130, 30);

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportImportService: GenerateFile06", ex.ToString());
                return false;
            }
        }

        private bool GenerateFile02(string outputFile, int categoryType)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();

            try
            {
                var factoryData = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted).Select(t => t.FactoryName).ToList();

                string[] factories = factoryData.ToArray();
                string[] statusKeys = new string[] {
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply),
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply)
                };

                string titleName = "Tên";

                if (categoryType == (int)EnumAmCategories.WorkType)
                {
                    titleName += " loại công việc";
                }

                if (categoryType == (int)EnumAmCategories.Emergency)
                {
                    titleName += " mức độ khẩn cấp";
                }

                if (categoryType == (int)EnumAmCategories.ReasonForPending)
                {
                    titleName += " nguyên nhân";
                }

                ISheet tempDataSheet = workbook.CreateSheet("Data");

                IRow headerData = tempDataSheet.CreateRow(0);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 0, 3), headerData, "Nhà máy");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 4, 6), headerData, titleName);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 7, 8), headerData, "Trạng thái");
                xssfHelper.SetStyleTableHeader(tempDataSheet, new CellRangeAddress(0, 0, 0, 8), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                IRow bodyData = tempDataSheet.CreateRow(1);
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 0, 3), factories);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 4, 6), bodyData, "Tên xxxx");
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 7, 8), statusKeys);

                for (int i = 2; i <= 100; i++)
                {
                    IRow bodyTempData = tempDataSheet.CreateRow(i);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 0, 3), factories);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 4, 6), bodyTempData, string.Empty);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 7, 8), statusKeys);
                }
                xssfHelper.SetStyleTableBody(tempDataSheet, new CellRangeAddress(1, 100, 0, 8), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                xssfHelper.SetStyleFinish(tempDataSheet, new CellRangeAddress(0, 100, 0, 8), 130, 28);

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportImportService: GenerateFile02", ex.ToString());
                return false;
            }
        }

        private bool GenerateFile03(string outputFile, int categoryType)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();

            try
            {
                var orgData = _unitOfWork.ElogbookRepository<Organization>()
                    .GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && "Công ty".Equals(x.Level, StringComparison.OrdinalIgnoreCase))
                    .Select(t => t.Name).ToList();

                string[] companys = orgData.ToArray();
                string[] statusKeys = new string[] {
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply),
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply)
                };

                string titleName = string.Empty;

                if (categoryType == (int)EnumAmCategories.Location)
                {
                    titleName = " khu vực";
                }

                if (categoryType == (int)EnumAmCategories.DeviceCategory)
                {
                    titleName = " phân loại";
                }

                ISheet tempDataSheet = workbook.CreateSheet("Data");

                IRow headerData = tempDataSheet.CreateRow(0);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 0, 3), headerData, "Công ty");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 4, 7), headerData, "Tên" + titleName);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 8, 9), headerData, "Mã" + titleName);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 10, 11), headerData, "Trạng thái");
                xssfHelper.SetStyleTableHeader(tempDataSheet, new CellRangeAddress(0, 0, 0, 11), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                IRow bodyData = tempDataSheet.CreateRow(1);
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 0, 3), companys);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 4, 7), bodyData, "Tên xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 8, 9), bodyData, "Mã xxxx");
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 10, 11), statusKeys);

                for (int i = 2; i <= 100; i++)
                {
                    IRow bodyTempData = tempDataSheet.CreateRow(i);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 0, 3), companys);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 4, 7), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 8, 9), bodyTempData, string.Empty);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 10, 11), statusKeys);
                }
                xssfHelper.SetStyleTableBody(tempDataSheet, new CellRangeAddress(1, 100, 0, 11), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                xssfHelper.SetStyleFinish(tempDataSheet, new CellRangeAddress(0, 100, 0, 11), 130, 31);

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmLocationService: GenerateFile03", ex.ToString());
                return false;
            }
        }

        private bool GenerateFile04(string outputFile)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();

            try
            {
                // Create Sheet Config Data
                ISheet configDataSheet = workbook.CreateSheet("Config");

                var amWorkType = _unitOfWork.ElogbookRepository<AmWorkType>()
                    .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply).OrderBy(t => t.FactoryID).ToList();

                var lstFactory = amWorkType.Select(x => x.FactoryID).Distinct().ToList();

                int indexRow = 0;

                IRow headerRowConfig = configDataSheet.CreateRow(indexRow);
                headerRowConfig.CreateCell(0).SetCellValue("Nhà máy");
                headerRowConfig.CreateCell(1).SetCellValue("Loại công việc");
                xssfHelper.SetStyleTableHeader(configDataSheet, new CellRangeAddress(0, 0, 0, 1), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                if (lstFactory.Count > 0)
                {
                    foreach (var facId in lstFactory)
                    {
                        var lstWorkType = amWorkType.FindAll(x => x.FactoryID == facId).ToList();

                        if (lstWorkType.Count > 0)
                        {
                            foreach (var wt in lstWorkType)
                            {
                                indexRow += 1;
                                IRow headerBodyConfig = configDataSheet.CreateRow(indexRow);
                                headerBodyConfig.CreateCell(0).SetCellValue(wt.Factory.FactoryName);
                                headerBodyConfig.CreateCell(1).SetCellValue(wt.TypeDesc);
                            }
                        }
                    }
                }

                xssfHelper.SetStyleTableBody(configDataSheet, new CellRangeAddress(1, indexRow, 0, 1), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                xssfHelper.SetStyleFinish(configDataSheet, new CellRangeAddress(0, indexRow, 0, 1), indexRow + 100, 65);

                var factoryData = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted).Select(t => t.FactoryName).ToList();

                string[] factories = factoryData.ToArray();
                string[] statusKeys = new string[] {
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply),
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply)
                };

                // Create Sheet Template Data
                ISheet tempDataSheet = workbook.CreateSheet("Data");

                // Create Define Name
                XSSFName indexIndirect = (XSSFName)workbook.CreateName();
                indexIndirect.NameName = "index_indirect";
                indexIndirect.RefersToFormula = "INDIRECT(\"Data!$A$\"&ROW())";

                XSSFName factoryRange = (XSSFName)workbook.CreateName();
                factoryRange.NameName = "ds_factories";
                factoryRange.RefersToFormula = "Config!$A$1:$A$" + indexRow.ToString();

                XSSFName workTypeRange = (XSSFName)workbook.CreateName();
                workTypeRange.NameName = "ds_work_type";
                workTypeRange.RefersToFormula = "OFFSET(Config!$B$1,MATCH(index_indirect,ds_factories,0) - 1,0,COUNTIF(ds_factories,index_indirect),1)";

                IRow headerData = tempDataSheet.CreateRow(0);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 0, 3), headerData, "Nhà máy");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 4, 6), headerData, "Loại công việc");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 7, 9), headerData, "Tên công việc");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 10, 12), headerData, "Nội dung");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 13, 14), headerData, "Trạng thái");
                xssfHelper.SetStyleTableHeader(tempDataSheet, new CellRangeAddress(0, 0, 0, 14), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                IRow bodyData = tempDataSheet.CreateRow(1);
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 0, 3), factories);
                xssfHelper.CreateDataValidation02(tempDataSheet, new CellRangeAddress(1, 1, 4, 6), "ds_work_type");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 7, 9), bodyData, "Tên xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 10, 12), bodyData, "<p>Nội dung xxxx</p>");
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 13, 14), statusKeys);

                for (int i = 2; i <= 100; i++)
                {
                    IRow bodyTempData = tempDataSheet.CreateRow(i);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 0, 3), factories);
                    xssfHelper.CreateDataValidation02(tempDataSheet, new CellRangeAddress(i, i, 4, 6), "ds_work_type");
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 7, 9), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 10, 12), bodyTempData, string.Empty);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 13, 14), statusKeys);
                }
                xssfHelper.SetStyleTableBody(tempDataSheet, new CellRangeAddress(1, 100, 0, 14), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                xssfHelper.SetStyleFinish(tempDataSheet, new CellRangeAddress(0, 100, 0, 14), 130, 34);

                configDataSheet.AutoSizeColumn(0);
                configDataSheet.AutoSizeColumn(1);

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportImportService: GenerateFile04", ex.ToString());
                return false;
            }
        }

        private bool GenerateFile05(string outputFile, int categoryType)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();

            try
            {
                string[] statusKeys = new string[] {
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply),
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply)
                };

                string title = string.Empty;

                if(categoryType == (int)EnumAmCategories.Manufacturer)
                {
                    title = " nhà chế tạo";
                }

                if (categoryType == (int)EnumAmCategories.Supplier)
                {
                    title = " nhà cung cấp";
                }

                if (categoryType == (int)EnumAmCategories.CountryManufacture)
                {
                    title = " nước sản xuất";
                }

                if (categoryType == (int)EnumAmCategories.DeviceType)
                {
                    title = " loại thiết bị";
                }

                ISheet tempDataSheet = workbook.CreateSheet("Data");

                IRow headerData = tempDataSheet.CreateRow(0);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 0, 3), headerData, "Tên" + title);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 4, 5), headerData, "Mã" + title);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 6, 7), headerData, "Trạng thái");
                xssfHelper.SetStyleTableHeader(tempDataSheet, new CellRangeAddress(0, 0, 0, 7), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                IRow bodyData = tempDataSheet.CreateRow(1);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 0, 3), bodyData, "Tên xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 4, 5), bodyData, "Mã xxxx");
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 6, 7), statusKeys);

                for (int i = 2; i <= 100; i++)
                {
                    IRow bodyTempData = tempDataSheet.CreateRow(i);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 0, 3), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 4, 5), bodyTempData, string.Empty);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 6, 7), statusKeys);
                }
                xssfHelper.SetStyleTableBody(tempDataSheet, new CellRangeAddress(1, 100, 0, 7), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                xssfHelper.SetStyleFinish(tempDataSheet, new CellRangeAddress(0, 100, 0, 7), 130, 27);

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportImportService: GenerateFile01", ex.ToString());
                return false;
            }
        }

        private bool GenerateFile06(string outputFile, int companyId)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();

            try
            {
                var locationData = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => !x.IsDeleted && x.OrganizationID == companyId && x.Active == (int)AmEnumUsageStatus.Apply)
                .Select(x => x.SiteDesc).ToList();

                var organizationData = _unitOfWork.ElogbookRepository<Organization>()
                    .GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && "Phân xưởng".Equals(x.Level, StringComparison.OrdinalIgnoreCase) && x.ParentID == companyId)
                    .Select(t => t.Name).ToList();

                var deviceStatusData = _unitOfWork.ElogbookRepository<AmDeviceStatus>().GetQuery(x => !x.IsDeleted).Select(x => x.StatusDesc).ToList();

                string[] locations = locationData.ToArray();
                string[] organizations = organizationData.ToArray();
                string[] deviceStatus = deviceStatusData.ToArray();
                string[] statusKeys = new string[] {
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply),
                    EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply)
                };

                // Create Sheet Template Data
                ISheet tempDataSheet = workbook.CreateSheet("Data");

                IRow headerData = tempDataSheet.CreateRow(0);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 0, 3), headerData, "Khu vực");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 4, 7), headerData, "Tên thiết bị");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 8, 9), headerData, "Mã thiết bị");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 10, 12), headerData, "Phân xưởng quản lý");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 13, 14), headerData, "Tình trạng thiết bị");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 15, 16), headerData, "Trạng thái");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 17, 19), headerData, "Số S/N");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 20, 22), headerData, "Chủng loại/Kiểu");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 23, 25), headerData, "Thông số kỹ thuật");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 26, 28), headerData, "Mã số KKS");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 29, 31), headerData, "Ký hiệu vận hành");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 32, 35), headerData, "Ghi chú");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 36, 39), headerData, "Mô tả");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 40, 42), headerData, "Bản vẽ liên quan");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 43, 44), headerData, "Ngày lắp đặt");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(0, 0, 45, 46), headerData, "Ngày đưa vận hành");
                xssfHelper.SetStyleTableHeader(tempDataSheet, new CellRangeAddress(0, 0, 0, 46), new XSSFColor(new byte[] { 169, 208, 142 }), BorderStyle.Thin);

                IRow bodyData = tempDataSheet.CreateRow(1);

                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 0, 3), locations);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 4, 7), bodyData, "Tên thiết bị xxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 8, 9), bodyData, "Mã thiết bị xxx");
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 10, 12), organizations);
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 13, 14), deviceStatus);
                xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(1, 1, 15, 16), statusKeys);
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 17, 19), bodyData, "Số S/N xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 20, 22), bodyData, "Chủng loại/Kiểu xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 23, 25), bodyData, "Thông số kỹ thuật xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 26, 28), bodyData, "Mã số KKS xxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 29, 31), bodyData, "Ký hiệu vận hành xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 32, 35), bodyData, "Ghi chú xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 36, 39), bodyData, "Mô tả xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 40, 42), bodyData, "Bản vẽ liên quan xxxx");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 43, 44), bodyData, "dd/MM/yyyy");
                xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(1, 1, 45, 46), bodyData, "dd/MM/yyyy");

                for (int i = 2; i <= 100; i++)
                {
                    IRow bodyTempData = tempDataSheet.CreateRow(i);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 0, 3), locations);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 4, 7), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 8, 9), bodyTempData, string.Empty);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 10, 12), organizations);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 13, 14), deviceStatus);
                    xssfHelper.CreateDataValidation01(tempDataSheet, new CellRangeAddress(i, i, 15, 16), statusKeys);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 17, 19), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 20, 22), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 23, 25), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 26, 28), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 29, 31), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 32, 35), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 36, 39), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 40, 42), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 43, 44), bodyTempData, string.Empty);
                    xssfHelper.CreateCellTypeStr(tempDataSheet, new CellRangeAddress(i, i, 45, 46), bodyTempData, string.Empty);
                }
                xssfHelper.SetStyleTableBody(tempDataSheet, new CellRangeAddress(1, 100, 0, 46), new XSSFColor(new byte[] { 221, 235, 247 }), BorderStyle.Thin);
                xssfHelper.SetStyleFinish(tempDataSheet, new CellRangeAddress(0, 100, 0, 46), 130, 66);

                //Save the workbook to a file
                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                workbook.Close();

                return true;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ExportImportService: GenerateFile05", ex.ToString());
                return false;
            }
        }
        #endregion

        #region Check Validate

        private int CheckExistsFactory(int indexRow, string inputFactory, ref ResponseModel<bool> responseModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Factory>()
                .GetQuery(x => x.FactoryName.Equals(inputFactory, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).FirstOrDefault();

            if (entity == null)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Nhà máy không tồn tại");
                return 0;
            }

            return entity.ID;
        }

        private bool CheckExistsName(int indexRow, string inputName, ref List<string> lstNames, int categoryType, ref ResponseModel<bool> responseModel)
        {
            bool isValid = true;
            bool isExistRecord = false;

            if (categoryType == (int)EnumAmCategories.DeviceType)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmDeviceType>()
                                    .GetQuery(x => x.TypeDesc.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Manufacturer)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmManufacturer>()
                                    .GetQuery(x => x.ManufacturerName.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Supplier)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmSupplier>()
                                    .GetQuery(x => x.SupplierName.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.CountryManufacture)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmCountryManufacture>()
                                    .GetQuery(x => x.CountryName.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.WorkType)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmWorkType>()
                                    .GetQuery(x => x.TypeDesc.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Location)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmLocation>()
                                    .GetQuery(x => x.SiteDesc.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.ReasonForStop)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmReasonForStop>()
                                    .GetQuery(x => x.ReasonDesc.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.ReasonForPending)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmReasonForPending>()
                                    .GetQuery(x => x.ReasonDesc.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Emergency)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmEmergency>()
                                    .GetQuery(x => x.Name.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Device)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmDevice>()
                                    .GetQuery(x => x.AssetDesc.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.DeviceCategory)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmDeviceCategory>()
                                    .GetQuery(x => x.CategoryName.Equals(inputName, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (isExistRecord)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Tên đã tồn tại");
                isValid = false;
            }

            if (lstNames.Contains(inputName))
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Tên đã tồn tại");
                isValid = false;
            }

            return isValid;
        }

        private bool CheckExistsCode(int indexRow, string inputCode, ref List<string> lstCodes, int categoryType, ref ResponseModel<bool> responseModel)
        {
            bool isValid = true;
            bool isExistRecord = false;

            if (categoryType == (int)EnumAmCategories.DeviceType)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmDeviceType>()
                                    .GetQuery(x => x.TypeCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Manufacturer)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmManufacturer>()
                                    .GetQuery(x => x.ManufacturerCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Supplier)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmSupplier>()
                                    .GetQuery(x => x.SupplierCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.CountryManufacture)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmCountryManufacture>()
                                    .GetQuery(x => x.CountryCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Location)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmLocation>()
                                    .GetQuery(x => x.SiteCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.Device)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmDevice>()
                                    .GetQuery(x => x.AssetCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (categoryType == (int)EnumAmCategories.DeviceCategory)
            {
                isExistRecord = _unitOfWork.ElogbookRepository<AmDeviceCategory>()
                                    .GetQuery(x => x.CategoryCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).Any();
            }

            if (isExistRecord)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Mã đã tồn tại");
                isValid = false;
            }

            if (lstCodes.Contains(inputCode))
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Mã đã tồn tại");
                isValid = false;
            }

            return isValid;
        }

        private bool CheckValidStatus(int indexRow, string inputStatus,ref int status, ref ResponseModel<bool> responseModel)
        {
            bool isValid = true;
            if (inputStatus.Equals(EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply)))
            {
                status = (int)AmEnumUsageStatus.Apply;
            }

            if (inputStatus.Equals(EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply)))
            {
                status = (int)AmEnumUsageStatus.UnApply;
            }

            // Check trạng thái hợp lệ
            if (status == 0)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Trạng thái không hợp lệ");
                isValid = false;
            }

            return isValid;
        }

        private int CheckExistWorkType(int indexRow, string inputType, ref ResponseModel<bool> responseModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkType>()
                .GetQuery(x => x.TypeDesc.Equals(inputType, StringComparison.OrdinalIgnoreCase) && !x.IsDeleted).FirstOrDefault();

            if (entity == null)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Loại công việc không tồn tại");
                return 0;
            }

            return entity.ID;
        }

        private int CheckExistsProductionPlant(int indexRow, string inputProductionPlant, int companyId, ref ResponseModel<bool> responseModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Organization>()
                                    .GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                    && "Phân xưởng".Equals(x.Level, StringComparison.OrdinalIgnoreCase)
                                                    && inputProductionPlant.Equals(x.Name, StringComparison.OrdinalIgnoreCase)
                                                    && x.ParentID == companyId).FirstOrDefault();

            if (entity == null)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Phân xưởng không tồn tại");
                return 0;
            }

            return entity.ID;
        }

        private int CheckExistsCompany(int indexRow, string inputCompany, ref ResponseModel<bool> responseModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Organization>()
                                    .GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                    && "Công ty".Equals(x.Level, StringComparison.OrdinalIgnoreCase)
                                                    && inputCompany.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (entity == null)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Công ty không tồn tại");
                return 0;
            }

            return entity.ID;
        }

        private int CheckExistsDeviceStatus(int indexRow, string inputDeviceStatus, ref ResponseModel<bool> responseModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmDeviceStatus>()
                .GetQuery(x => !x.IsDeleted && inputDeviceStatus.Equals(x.StatusDesc, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (entity == null)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Tình trạng thiết bị không tồn tại");
                return 0;
            }

            return entity.ID;
        }

        private int CheckExistsLocation(int indexRow, string inputLocation, int company, ref ResponseModel<bool> responseModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                && inputLocation.Equals(x.SiteDesc, StringComparison.OrdinalIgnoreCase) && x.OrganizationID == company).FirstOrDefault();

            if (entity == null)
            {
                responseModel.ErrorMessages.Add("Row: " + indexRow.ToString() + " -Khu vực không tồn tại");
                return 0;
            }

            return entity.ID;
        }
        #endregion

        #region Insert Data
        private void InsertDataToDB02(int factoryId, string inputName, int status, int currentUserID, int categoryType)
        {
            if (categoryType == (int)EnumAmCategories.WorkType)
            {
                var entity = new AmWorkType
                {
                    TypeDesc = inputName,
                    FactoryID = factoryId,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };

                _unitOfWork.ElogbookRepository<AmWorkType>().Add(entity);
            }

            if (categoryType == (int)EnumAmCategories.Emergency)
            {
                var entity = new AmEmergency
                {
                    Name = inputName,
                    FactoryID = factoryId,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };

                _unitOfWork.ElogbookRepository<AmEmergency>().Add(entity);
            }

            if (categoryType == (int)EnumAmCategories.ReasonForPending)
            {
                var entity = new AmReasonForPending
                {
                    ReasonDesc = inputName,
                    FactoryID = factoryId,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };
                _unitOfWork.ElogbookRepository<AmReasonForPending>().Add(entity);
            }
        }

        private void InsertDataToDB05(string inputName, string inputCode, int status, int currentUserID, int categoryType)
        {
            if (categoryType == (int)EnumAmCategories.DeviceType)
            {
                var entity = new AmDeviceType
                {
                    TypeDesc = inputName,
                    TypeCode = inputCode,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };

                _unitOfWork.ElogbookRepository<AmDeviceType>().Add(entity);
            }

            if (categoryType == (int)EnumAmCategories.DeviceType)
            {
                var entity = new AmDeviceType
                {
                    TypeDesc = inputName,
                    TypeCode = inputCode,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };

                _unitOfWork.ElogbookRepository<AmDeviceType>().Add(entity);
            }

            if (categoryType == (int)EnumAmCategories.CountryManufacture)
            {
                var entity = new AmCountryManufacture
                {
                    CountryName = inputName,
                    CountryCode = inputCode,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };

                _unitOfWork.ElogbookRepository<AmCountryManufacture>().Add(entity);
            }

            if (categoryType == (int)EnumAmCategories.Manufacturer)
            {
                var entity = new AmManufacturer
                {
                    ManufacturerName = inputName,
                    ManufacturerCode = inputCode,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };

                _unitOfWork.ElogbookRepository<AmManufacturer>().Add(entity);
            }

            if (categoryType == (int)EnumAmCategories.Supplier)
            {
                var entity = new AmSupplier
                {
                    SupplierName = inputName,
                    SupplierCode = inputCode,
                    Active = status,
                    CreatedUserID = currentUserID,
                    LastUpdatedUserID = currentUserID,
                };

                _unitOfWork.ElogbookRepository<AmSupplier>().Add(entity);
            }
        }
        #endregion

        #region Download
        protected HttpResponseMessage Download(HttpContent content, string fileName = "")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return response;
        }
        #endregion

        #endregion
    }
}
