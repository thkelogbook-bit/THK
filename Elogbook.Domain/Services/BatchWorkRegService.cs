using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Elogbook.Domain.Services
{
    public class BatchWorkRegService : IBatchWorkRegService
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;

        #endregion

        #region Contructor

        public BatchWorkRegService(ElogbookUnitOfWork unitOfWork, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Tải file mẫu Phiếu đăng ký (Điện - Thủy Cơ Nhiệt Hóa)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HttpResponseMessage DownloadFileTemplate(string type)
        {
            string fileName = "PhieuDK_Dien_Template.xlsx"; // File mẫu Điện

            if (type.Equals("TCNH"))
            {
                fileName = "PhieuDK_Tcnh_Template.xlsx"; // File mẫu Thủy cơ nhiệt hóa
            }

            var pathCurrent = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var outputFile = Path.Combine(pathCurrent.Substring(6), "Template", fileName);

            if (File.Exists(outputFile))
            {
                var fileBytes = File.ReadAllBytes(outputFile);

                if (fileBytes.Length > 0)
                {
                    return Download(new ByteArrayContent(fileBytes), fileName);
                }
            }
            else
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BatchWorkRegBusiness.ToString(),
                    Environment.NewLine + "File not found: " + outputFile);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Upload file lên server
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public WorkRegExcelDataValidationModel UploadFile(WorkRegUploadInfoModel requestModel)
        {
            if (requestModel.RegisType != 1 && requestModel.RegisType != 2)
            {
                var results = new WorkRegExcelDataValidationModel
                {
                    IsValid = false,
                    ErrorMessage = "Loại phiếu đăng ký công tác là bắt buộc"
                };
                return results;
            }

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
                var results = new WorkRegExcelDataValidationModel
                {
                    IsValid = false,
                    ErrorMessage = "Dung lượng file phải nhỏ hơn hoặc 5M"
                };
                File.Delete(requestModel.FilePath);
                File.Delete(requestModel.LocalFileName);
                return results;
            }

            var data = ProcessExcelData(requestModel);

            File.Delete(requestModel.LocalFileName);

            // valid
            if (!data.IsValid)
            {
                File.Delete(requestModel.FilePath);
                return data;
            }
            else
            {
                // lưu vào table Attachment
                var attachment = new Attachment
                {
                    FileName = requestModel.FileName,
                    FilePath = requestModel.FilePath,
                    ScreenName = CommonConstant.BATCH_WORK_REG,
                    FileType = "xlsx",
                    CreatedUserID = requestModel.CurrentUserID,
                    LastUpdatedUserID = requestModel.CurrentUserID
                };

                _unitOfWork.ElogbookRepository<Attachment>().Add(attachment);
                if (_unitOfWork.Complete() > 0)
                {
                    // return id
                    data.AttachmentId = attachment.ID;
                    return data;
                }
            }

            return data;
        }

        /// <summary>
        /// Tạo batch phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CreateBatchWorkReg(WorkRegAddEditModel requestModel)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    int totalTicket = 0;
                    if (requestModel.ScreenType == (int)TicketTypePCT.DIEN)
                    {
                        totalTicket = requestModel.WorkRegDienExcelDatas.Count;
                    }

                    if (requestModel.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                    {
                        totalTicket = requestModel.WorkRegTcnhExcelDatas.Count;
                    }

                    var entity = new BatchWorkReg
                    {
                        FactoryID = requestModel.FactoryID,
                        ScreenType = requestModel.ScreenType,
                        CreatedDate = DateTime.Now,
                        PlanCreateTicketTime = requestModel.PlanCreateTicketTime,
                        TotalTicket = totalTicket,
                        TotalSuccess = 0,
                        BatchWorkRegStatusID = (int)EnumWorkRegStatus.Waiting,
                        AttachmentIds = requestModel.AttachmentID.ToString(),
                        AttachmentID = requestModel.AttachmentID,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    _unitOfWork.ElogbookRepository<BatchWorkReg>().Add(entity);
                    var saveSuccess = _unitOfWork.Complete() > 0;

                    if (saveSuccess)
                    {
                        // Save dữ liệu tạo PĐK vào table BatchWorkRegTemps
                        InsertBatchWorkRegExcelDataTemps(requestModel, entity.ID);

                        // Save log khi tạo mới Batch đăng ký phiếu công tác
                        BatchWorkRegHistory batchWorkRegHistory = new BatchWorkRegHistory
                        {
                            BatchWorkRegID = entity.ID,
                            Content = CommonConstant.MSG_CREATE_PĐK_SLL,
                            AttachmentID = entity.AttachmentID,
                            BatchWorkRegStatusID = (int)EnumWorkRegStatus.Waiting,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID
                        };
                        _unitOfWork.ElogbookRepository<BatchWorkRegHistory>().Add(batchWorkRegHistory);
                        _unitOfWork.Complete();

                        transaction.Commit();
                        return entity.ID;
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "BatchWorkRegService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                }

                return 0;
            }
        }

        public int InsertWorkRegFromSchedule()
        {
            // Lấy danh sách đợt tạo Phiếu đăng ký công tác
            var batchWorkRegs = _unitOfWork.ElogbookRepository<BatchWorkReg>()
                .GetQuery(x => !x.IsDeleted && (x.BatchWorkRegStatusID == (int)EnumWorkRegStatus.Waiting
                || x.BatchWorkRegStatusID == (int)EnumWorkRegStatus.Error) && x.PlanCreateTicketTime <= DateTime.Now).ToList();

            if (batchWorkRegs != null && batchWorkRegs.Count > 0)
            {
                //List ID đợt sửa chữa lớn
                var batchWorkRegIds = batchWorkRegs.Select(x => x.ID).ToList();

                //List phiếu công tác từ table tạm
                var excelDatas = _unitOfWork.ElogbookRepository<BatchWorkRegTemp>()
                    .GetQuery(x => !x.IsDeleted && batchWorkRegIds.Contains(x.BatchWorkRegID)).ToList();

                if (excelDatas != null && excelDatas.Count > 0)
                {
                    foreach (var item in batchWorkRegs)
                    {
                        var history = new ScheduleHistory()
                        {
                            ScheduleName = "Đăng ký Phiếu công tác hàng loạt Id: " + item.ID.ToString(),
                            Status = 1,
                            ScheduleTime = DateTime.Now
                        };

                        int countSuccess = 0;
                        int countError = 0;

                        var excelDataOfBatchWorkRegs = excelDatas.Where(x => x.BatchWorkRegID == item.ID);
                        List<int> lstTempId = new List<int>();

                        if (excelDataOfBatchWorkRegs != null)
                        {
                            // Set status for BatchWorkReg
                            item.BatchWorkRegStatusID = (int)EnumWorkRegStatus.Doing;
                            _unitOfWork.ElogbookRepository<BatchWorkReg>().Update(item);
                            _unitOfWork.Complete();

                            // Save log khi hệ thống tạo hàng loạt phiếu công tác
                            BatchWorkRegHistory batchWorkRegHistory = new BatchWorkRegHistory
                            {
                                BatchWorkRegID = item.ID,
                                Content = CommonConstant.MSG_CREATE_PĐK_SLL,
                                AttachmentID = item.AttachmentID,
                                BatchWorkRegStatusID = (int)EnumWorkRegStatus.Doing
                            };
                            _unitOfWork.ElogbookRepository<BatchWorkRegHistory>().Add(batchWorkRegHistory);
                            _unitOfWork.Complete();
                            Thread.Sleep(500);

                            // Get Factory Info
                            var facInfo = _unitOfWork.ElogbookRepository<Factory>().GetById(item.FactoryID);

                            // Get User Info
                            var userEntity = _unitOfWork.ElogbookRepository<User>().GetById((int)item.CreatedUserID);

                            string ticketType = item.ScreenType == (int)TicketTypePCT.DIEN ? "DKPCTD" : "DKPCTTCNH";
                            foreach (var it in excelDataOfBatchWorkRegs)
                            {
                                using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
                                {
                                    try
                                    {
                                        var ticketNumber = _commonService.GetRegistrationTicketNumber((int)item.CreatedUserID, ticketType);

                                        // Tạo phiếu đăng ký công tác điện
                                        if (it.ScreenType == (int)TicketTypePCT.DIEN)
                                        {
                                            var newRegiDien = new RegistrationTicket
                                            {
                                                // Thông tin chung
                                                TicketType = it.TicketType,
                                                FactoryID = it.FactoryID,
                                                TicketNumber = ticketNumber,
                                                RegistrationDate = DateTime.Now,
                                                ReceiveUnitID = facInfo.ProductionPlantID,
                                                RegulationNumber = string.Empty,
                                                RegistrationUnitID = userEntity.ProductionPlantID.Value,
                                                Position = userEntity.PositionName,
                                                PhoneNumber = userEntity.PhoneNumber,
                                                Email = userEntity.Email,
                                                // Thông tin đăng ký
                                                WorkContent = it.WorkContent,
                                                WorkPlacement = it.WorkPlacement,
                                                WorkCondition = it.WorkCondition,
                                                StartDate = it.StartDate,
                                                EndDate = it.EndDate,
                                                StatusValue = (int)EnumTicketStatus.Unsent,
                                                CreatedUserID = (int)it.CreatedUserID,
                                                LastUpdatedUserID = (int)it.CreatedUserID,
                                                InsertedAt = DateTime.Now,
                                                UpdatedAt = DateTime.Now,
                                                RegistrationTicketType = (int)TicketTypePCT.DIEN,
                                                BatchWorkRegID = item.ID,
                                                RegistrationUnitView = userEntity.ProductionPlant.Name,
                                            };

                                            newRegiDien.RegistrationUnitView = newRegiDien.RegistrationUnitView ?? _commonService.GetRegistrationUnitView((int)newRegiDien.CreatedUserID);

                                            _unitOfWork.ElogbookRepository<RegistrationTicket>().Add(newRegiDien);
                                            _unitOfWork.Complete();
                                        }

                                        // Tạo phiếu đăng ký công tác thủy cơ nhiệt hóa
                                        if (it.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                                        {
                                            var newRegiTcnh = new RegistrationTicket
                                            {
                                                // Thông tin chung
                                                TicketType = it.TicketType,
                                                FactoryID = it.FactoryID,
                                                TicketNumber = ticketNumber,
                                                RegistrationDate = DateTime.Now,
                                                RegistrationUnitID = userEntity.ProductionPlantID.Value,
                                                ReceiveUnitID = facInfo.ProductionPlantID,
                                                Position = userEntity.PositionName,
                                                PhoneNumber = userEntity.PhoneNumber,
                                                Email = userEntity.Email,
                                                // Thông tin đăng ký
                                                WorkContent = it.WorkContent,
                                                LocationDevice = it.WorkPlacement,
                                                WorkPlacement = it.WorkPlacement,
                                                WorkCondition = string.Empty,
                                                ContentType = it.ReasonType,
                                                StartDate = it.StartDate,
                                                EndDate = it.EndDate,
                                                StatusValue = (int)EnumTicketStatus.Unsent,
                                                CreatedUserID = (int)it.CreatedUserID,
                                                LastUpdatedUserID = (int)it.CreatedUserID,
                                                InsertedAt = DateTime.Now,
                                                UpdatedAt = DateTime.Now,
                                                RegistrationTicketType = (int)TicketTypePCT.COTHUYNHIETHOA,
                                                BatchWorkRegID = item.ID,
                                                RegistrationUnitView = userEntity.ProductionPlant.Name,
                                            };

                                            newRegiTcnh.RegistrationUnitView = newRegiTcnh.RegistrationUnitView ?? _commonService.GetRegistrationUnitView((int)newRegiTcnh.CreatedUserID);

                                            _unitOfWork.ElogbookRepository<RegistrationTicket>().Add(newRegiTcnh);
                                            _unitOfWork.Complete();
                                        }

                                        transaction.Commit();
                                        countSuccess++;
                                        lstTempId.Add(it.ID);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "BatchWorkRegService: InsertWorkTicketFromSchedule ID=" + it.ID.ToString(), Environment.NewLine + ex.ToString());
                                        transaction.Rollback();
                                        countError++;
                                    }
                                }
                            }

                            if (countError > 0)
                            {
                                item.BatchWorkRegStatusID = (int)EnumWorkRegStatus.Error;
                                history.Status = 1;
                                batchWorkRegHistory.BatchWorkRegStatusID = (int)EnumWorkRegStatus.Error;
                            }
                            else
                            {
                                history.Status = 2;
                                batchWorkRegHistory.BatchWorkRegStatusID = (int)EnumWorkRegStatus.Finish;
                                item.BatchWorkRegStatusID = (int)EnumWorkRegStatus.Finish;
                            }

                            // Save lịch sử khi tạo Phiếu đăng ký công tác hàng loạt
                            _unitOfWork.ElogbookRepository<BatchWorkRegHistory>().Add(batchWorkRegHistory);

                            // Delete all record  to BatchWorkRegTemp
                            if (lstTempId.Count > 0)
                            {
                                _unitOfWork.ElogbookRepository<BatchWorkRegTemp>().Delete(x => lstTempId.Contains(x.ID), 0, true);
                            }

                            // Set status for BatchWorkReg
                            item.TotalSuccess += countSuccess;

                            item.UpdatedAt = DateTime.Now;
                            _unitOfWork.ElogbookRepository<BatchWorkReg>().Update(item);

                            // Save history
                            history.ScheduleName = history.ScheduleName + ". Thành công: " + countSuccess.ToString() + ". Thất bại: " + countError.ToString();
                            _unitOfWork.ElogbookRepository<ScheduleHistory>().Add(history);

                            _unitOfWork.Complete();
                        }
                    }
                }
            }

            return 0;
        }

        #endregion

        #region Private Method

        private void InsertBatchWorkRegExcelDataTemps(WorkRegAddEditModel requestModel, int batchWorkRegID)
        {
            var listBatchWorkRegExcelDataTemps = new List<BatchWorkRegTemp>();
            if (requestModel.ScreenType == (int)TicketTypePCT.DIEN)
            {
                // Tạo phiếu con lưu vào bảng tạm
                foreach (var item in requestModel.WorkRegDienExcelDatas)
                {
                    var newTicket = new BatchWorkRegTemp()
                    {
                        BatchWorkRegID = batchWorkRegID,
                        RowIndex = item.RowIndex,
                        TicketType = item.TicketType,
                        FactoryID = requestModel.FactoryID,
                        ScreenType = requestModel.ScreenType,
                        WorkContent = ReplaceWhiteSpace(item.WorkContent),
                        WorkPlacement = ReplaceWhiteSpace(item.WorkPlacement),
                        WorkCondition = ReplaceWhiteSpace(item.WorkCondition),
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        UpdatedAt = DateTime.Now,
                        InsertedAt = DateTime.Now,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID
                    };

                    listBatchWorkRegExcelDataTemps.Add(newTicket);
                }
                _unitOfWork.ElogbookRepository<BatchWorkRegTemp>().AddRange(listBatchWorkRegExcelDataTemps);
                _unitOfWork.Complete();
            }

            if (requestModel.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
            {
                // Tạo phiếu con lưu vào bảng tạm
                foreach (var item in requestModel.WorkRegTcnhExcelDatas)
                {
                    var newTicket = new BatchWorkRegTemp()
                    {
                        BatchWorkRegID = batchWorkRegID,
                        RowIndex = item.RowIndex,
                        TicketType = item.TicketType,
                        FactoryID = requestModel.FactoryID,
                        ScreenType = requestModel.ScreenType,
                        WorkContent = ReplaceWhiteSpace(item.WorkContent),
                        WorkPlacement = ReplaceWhiteSpace(item.WorkPlacement),
                        ReasonType = item.ReasonType,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        UpdatedAt = DateTime.Now,
                        InsertedAt = DateTime.Now,
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID
                    };

                    listBatchWorkRegExcelDataTemps.Add(newTicket);
                }
                _unitOfWork.ElogbookRepository<BatchWorkRegTemp>().AddRange(listBatchWorkRegExcelDataTemps);
                _unitOfWork.Complete();
            }
        }

        private static string ReplaceWhiteSpace(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                sb.Append(Char.IsWhiteSpace(c) ? ' ' : c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Xử lý dữ liệu trong file excel, kiểm tra dữ liệu có hợp lệ hay không
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private WorkRegExcelDataValidationModel ProcessExcelData(WorkRegUploadInfoModel requestModel)
        {
            var results = new WorkRegExcelDataValidationModel();
            var workRegDienChilds = new List<WorkRegDienExcelDataModel>();
            var workRegDienChildErros = new List<WorkRegDienExcelDataModel>();

            var workRegTcnhChilds = new List<WorkRegTcnhExcelDataModel>();
            var workRegTcnhChildErros = new List<WorkRegTcnhExcelDataModel>();

            int totalRecord = 0;
            if (requestModel.BatchWorkRegID > 0)
            {
                var entity = _unitOfWork.ElogbookRepository<BatchWorkReg>()
                    .GetQuery(x => x.ID == requestModel.BatchWorkRegID && x.BatchWorkRegStatusID == (int)EnumWorkRegStatus.Finish && !x.IsDeleted)
                    .FirstOrDefault();
                if (entity != null)
                {
                    totalRecord = entity.TotalTicket.HasValue ? (int)entity.TotalTicket : 0;
                }
            }

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

                    if (totalRecord > 0 && lastRowIndex - 1 <= totalRecord)
                    {
                        results.IsValid = false;
                        results.ErrorMessage = "Tổng số phiếu trong file excel mới phải lớn hơn số phiếu đã tồn tại";
                        return results;
                    }

                    if (!CheckTitleFile(dataInput, requestModel.RegisType))
                    {
                        results.IsValid = false;
                        results.ErrorMessage = "Nội dung file chưa đúng!";
                        return results;
                    }

                    if (requestModel.RegisType == (int)TicketTypePCT.DIEN)
                    {
                        workRegDienChilds = new List<WorkRegDienExcelDataModel>();
                        workRegDienChildErros = new List<WorkRegDienExcelDataModel>();
                        for (int j = totalRecord + 1; j < lastRowIndex; j++)
                        {
                            WorkRegDienExcelDataModel workRegDienChild = new WorkRegDienExcelDataModel();
                            if (!IsValidExcelDienData(dataInput, j, workRegDienChild))
                            {
                                workRegDienChildErros.Add(workRegDienChild);
                            }
                            else
                            {
                                workRegDienChilds.Add(workRegDienChild);
                            }
                        }
                    }

                    if (requestModel.RegisType == (int)TicketTypePCT.COTHUYNHIETHOA)
                    {
                        workRegTcnhChilds = new List<WorkRegTcnhExcelDataModel>();
                        workRegTcnhChildErros = new List<WorkRegTcnhExcelDataModel>();
                        for (int j = totalRecord + 1; j < lastRowIndex; j++)
                        {
                            WorkRegTcnhExcelDataModel workRegTcnhChild = new WorkRegTcnhExcelDataModel();
                            if (!IsValidExcelTcnhData(dataInput, j, workRegTcnhChild))
                            {
                                workRegTcnhChildErros.Add(workRegTcnhChild);
                            }
                            else
                            {
                                workRegTcnhChilds.Add(workRegTcnhChild);
                            }
                        }
                    }
                }
            }

            if (workRegDienChildErros.Count > 0)
            {
                results.IsValid = false;
                results.WorkRegDienExcelDatas = workRegDienChildErros;
            }
            else if (workRegTcnhChildErros.Count > 0)
            {
                results.IsValid = false;
                results.WorkRegTcnhExcelDatas = workRegTcnhChildErros;
            }
            else
            {
                results.IsValid = true;
                results.WorkRegDienExcelDatas = workRegDienChilds;
                results.WorkRegTcnhExcelDatas = workRegTcnhChilds;
            }

            return results;
        }

        /// <summary>
        /// Kiểm tra title của file dữ liệu mẫu
        /// </summary>
        /// <param name="dataInput"></param>
        /// <param name="regisType"></param>
        /// <returns></returns>
        private static bool CheckTitleFile(DataTable dataInput, int regisType)
        {
            var row = dataInput.Rows[0];
            string[] expected;
            bool rs = false;

            if (regisType == (int)TicketTypePCT.DIEN)
            {
                expected = new[]
                {
                    "Loại phiếu",
                    "Nội dung công việc",
                    "Địa điểm (hoặc thiết bị) tiến hành công tác",
                    "Điều kiện cần có để thực hiện công tác",
                    "Thời gian tiến hành công việc (bắt đầu)",
                    "Thời gian tiến hành công việc (kết thúc)"
                };
            }
            else if (regisType == (int)TicketTypePCT.COTHUYNHIETHOA)
            {
                expected = new[]
                {
                    "Loại phiếu",
                    "Địa điểm/Thiết bị",
                    "Lý do",
                    "Nội dung",
                    "Thời gian (bắt đầu)",
                    "Thời gian (kết thúc)"
                };
            }
            else
            {
                return rs;
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (!row[ExcelConstant.COLUMN_A + i].ToString().Trim().Equals(expected[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra dữ liệu trong file excel có hợp lệ hay không với phiếu Điện
        /// </summary>
        /// <param name="dataInput"></param>
        /// <param name="indexRow"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static bool IsValidExcelDienData(DataTable dataInput, int indexRow, WorkRegDienExcelDataModel model)
        {
            model.Errors = new List<string>();
            model.RowIndex = indexRow;

            // Read cells
            var row = dataInput.Rows[indexRow];
            string ticketTypeStr = row[ExcelConstant.COLUMN_A].ToString().Trim();
            string workContent = row[ExcelConstant.COLUMN_B].ToString().Trim();
            string workPlacement = row[ExcelConstant.COLUMN_C].ToString().Trim();
            string workCondition = row[ExcelConstant.COLUMN_D].ToString().Trim();
            string startStr = row[ExcelConstant.COLUMN_E].ToString().Trim();
            string endStr = row[ExcelConstant.COLUMN_F].ToString().Trim();

            model.WorkContent = workContent;
            model.WorkPlacement = workPlacement;
            model.WorkCondition = workCondition;

            bool parsedTicket = false;
            if (!string.IsNullOrEmpty(ticketTypeStr)
                && (int.TryParse(ticketTypeStr, out int intVal)))
            {
                parsedTicket = Enum.IsDefined(typeof(TicketType), intVal);
                if (parsedTicket && intVal != (int)TicketType.MajorRepairCase)
                {
                    parsedTicket = true;
                }
            }

            if (!parsedTicket)
            {
                model.Errors.Add("Loại phiếu không hợp lệ hoặc để trống. Giá trị hợp lệ: Kế hoạch, Ngoài kế hoạch, Xử lý sự cố");
            }
            else
            {
                model.TicketType = int.Parse(ticketTypeStr);
                model.StartDate = ValidDateDien(startStr, model);
                model.EndDate = ValidDateDien(endStr, model);

                if (model.StartDate > DateTime.MinValue && model.EndDate > DateTime.MinValue && model.StartDate > model.EndDate)
                {
                    model.Errors.Add("Thời gian kết thúc phải lớn hơn thời gian bắt đầu ");
                }
            }

            return model.Errors.Count <= 0;
        }

        /// <summary>
        /// kiểm tra dữ liệu trong file excel có hợp lệ hay không với phiếu Thủy cơ nhiệt hóa
        /// </summary>
        /// <param name="dataInput"></param>
        /// <param name="indexRow"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static bool IsValidExcelTcnhData(DataTable dataInput, int indexRow, WorkRegTcnhExcelDataModel model)
        {
            model.Errors = new List<string>();
            model.RowIndex = indexRow;

            var row = dataInput.Rows[indexRow];
            string ticketTypeStr = row[ExcelConstant.COLUMN_A].ToString().Trim();
            string placement = row[ExcelConstant.COLUMN_B].ToString().Trim();
            string reasonStr = row[ExcelConstant.COLUMN_C].ToString().Trim();
            string content = row[ExcelConstant.COLUMN_D].ToString().Trim();
            string startStr = row[ExcelConstant.COLUMN_E].ToString().Trim();
            string endStr = row[ExcelConstant.COLUMN_F].ToString().Trim();

            model.WorkPlacement = placement;
            model.WorkContent = content;

            bool parsedTicket = false;
            if (!string.IsNullOrEmpty(ticketTypeStr)
                && (int.TryParse(ticketTypeStr, out int intValue1)))
            {
                parsedTicket = Enum.IsDefined(typeof(TicketType), intValue1);
                if (parsedTicket && intValue1 != (int)TicketType.MajorRepairCase)
                {
                    parsedTicket = true;
                }
            }

            if (!parsedTicket)
            {
                model.Errors.Add("Loại phiếu không hợp lệ hoặc để trống. Giá trị hợp lệ: 1:Kế hoạch, 2:Ngoài kế hoạch, 3:Xử lý sự cố");
            }
            else
            {
                model.TicketType = int.Parse(ticketTypeStr);
            }

            bool parsedReason = false;
            if (!string.IsNullOrEmpty(reasonStr)
                && (int.TryParse(ticketTypeStr, out int intValue2)))
            {
                parsedReason = Enum.IsDefined(typeof(EnumReasonTcnh), intValue2);
            }

            if (!parsedReason)
            {
                model.Errors.Add("Lý do không hợp lệ hoặc để trống. Giá trị hợp lệ: Sửa chữa, Thí nghiệm, Xây dựng");
            }
            else
            {
                model.ReasonType = int.Parse(reasonStr);
                model.StartDate = ValidDateTcnh(startStr, model);
                model.EndDate = ValidDateTcnh(endStr, model);

                if (model.StartDate > DateTime.MinValue && model.EndDate > DateTime.MinValue && model.StartDate > model.EndDate)
                {
                    model.Errors.Add("Thời gian kết thúc phải lớn hơn thời gian bắt đầu ");
                }
            }

            return model.Errors.Count <= 0;
        }

        private static DateTime ValidDateDien(string dateStr, WorkRegDienExcelDataModel model)
        {
            DateTime rsDate = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateStr))
            {
                bool isEndDateValid = true;
                if (DateTime.TryParseExact(dateStr, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tmp))
                {
                    rsDate = tmp;

                    switch (model.TicketType)
                    {
                        case (int)TicketType.InsidePlan: // Kế hoạch -> after 7 days
                            if (rsDate < DateTime.Now.AddDays(7))
                            {
                                model.Errors.Add("Loại phiếu 'Kế hoạch' yêu cầu Thời gian bắt đầu phải sau ít nhất 7 ngày kể từ thời điểm tạo phiếu");
                                isEndDateValid = false;
                            }
                            break;
                        case (int)TicketType.OutsidePlan: // Ngoài kế hoạch -> after 1 day
                            if (rsDate < DateTime.Now.AddDays(1))
                            {
                                model.Errors.Add("Loại phiếu 'Ngoài kế hoạch' yêu cầu Thời gian bắt đầu phải sau ít nhất 1 ngày kể từ thời điểm tạo phiếu");
                                isEndDateValid = false;
                            }
                            break;
                        case (int)TicketType.SpecialCase: // Xử lý sự cố -> after 2 hours
                            if (rsDate < DateTime.Now.AddHours(2))
                            {
                                model.Errors.Add("Loại phiếu 'Xử lý sự cố' yêu cầu Thời gian bắt đầu phải sau ít nhất 2 giờ kể từ thời điểm tạo phiếu");
                                isEndDateValid = false;
                            }
                            break;
                    }

                    if (isEndDateValid)
                    {
                        return rsDate;
                    }
                }
                else
                {
                    model.Errors.Add($"Định dạng Thời gian kết thúc không hợp lệ. Định dạng đúng: dd/MM/yyyy HH:mm");
                }
            }
            else
            {
                switch (model.TicketType)
                {
                    case (int)TicketType.InsidePlan:
                        rsDate = DateTime.Now.AddDays(7);
                        break;
                    case (int)TicketType.OutsidePlan:
                        rsDate = DateTime.Now.AddDays(1);
                        break;
                    case (int)TicketType.SpecialCase:
                        rsDate = DateTime.Now.AddHours(2);
                        break;
                }
            }

            return rsDate;
        }

        private static DateTime ValidDateTcnh(string dateStr, WorkRegTcnhExcelDataModel model)
        {
            DateTime rsDate = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateStr))
            {
                DateTime tmp;
                bool isEndDateValid = true;
                if (DateTime.TryParseExact(dateStr, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp))
                {
                    rsDate = tmp;

                    switch (model.TicketType)
                    {
                        case (int)TicketType.InsidePlan: // Kế hoạch -> after 7 days
                            if (rsDate < DateTime.Now.AddDays(7))
                            {
                                model.Errors.Add("Loại phiếu 'Kế hoạch' yêu cầu Thời gian bắt đầu phải sau ít nhất 7 ngày kể từ thời điểm tạo phiếu");
                                isEndDateValid = false;
                            }
                            break;
                        case (int)TicketType.OutsidePlan: // Ngoài kế hoạch -> after 1 day
                            if (rsDate < DateTime.Now.AddDays(1))
                            {
                                model.Errors.Add("Loại phiếu 'Ngoài kế hoạch' yêu cầu Thời gian bắt đầu phải sau ít nhất 1 ngày kể từ thời điểm tạo phiếu");
                                isEndDateValid = false;
                            }
                            break;
                        case (int)TicketType.SpecialCase: // Xử lý sự cố -> after 2 hours
                            if (rsDate < DateTime.Now.AddHours(2))
                            {
                                model.Errors.Add("Loại phiếu 'Xử lý sự cố' yêu cầu Thời gian bắt đầu phải sau ít nhất 2 giờ kể từ thời điểm tạo phiếu");
                                isEndDateValid = false;
                            }
                            break;
                    }

                    if (isEndDateValid)
                    {
                        return rsDate;
                    }
                }
                else
                {
                    model.Errors.Add($"Định dạng Thời gian kết thúc không hợp lệ. Định dạng đúng: dd/MM/yyyy HH:mm");
                }
            }
            else
            {
                switch (model.TicketType)
                {
                    case (int)TicketType.InsidePlan:
                        rsDate = DateTime.Now.AddDays(7);
                        break;
                    case (int)TicketType.OutsidePlan:
                        rsDate = DateTime.Now.AddDays(1);
                        break;
                    case (int)TicketType.SpecialCase:
                        rsDate = DateTime.Now.AddHours(2);
                        break;
                }
            }

            return rsDate;
        }

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
    }
}
