using Elogbook.Core.Enums;
using Elogbook.Core;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using System.Net;
using System;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Business.Business
{
    public class BackupDataBusiness : IBackupDataBusiness
    {
        private readonly IBackupDataService _backupDataService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="backupDataService"></param>
        public BackupDataBusiness(IBackupDataService backupDataService)
        {
            _backupDataService = backupDataService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ResponseModel<UploadFileModel> BackupData()
        {
            _backupDataService.BackupData();
            return new ResponseModel<UploadFileModel> {};
        }


        public ResponseBaseModel Find(BaseListFilter searchModel)
        {
            var responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _backupDataService.Find(searchModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BackupDataBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(searchModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }


        public ResponseModel<bool> Delete(int id, int userID)
        {
            var responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _backupDataService.Delete(id, userID);

            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.BackupDataBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(id) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
