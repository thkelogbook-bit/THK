using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.SyncAMs;
using System;
using System.Net;

namespace Elogbook.Business
{
    public class AMMobileSyncDataBusiness : IAmMobileDataSyncBusiness
    {
        private readonly IAmMobileSyncDataService _amMobileSyncDataService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manipulationTicketService"></param>
        public AMMobileSyncDataBusiness(IAmMobileSyncDataService amMobileSyncDataService)
        {
            _amMobileSyncDataService = amMobileSyncDataService;
        }

        public ResponseBaseModel SyncAM(MobileAutonomousMaintenanceSyncDataModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amMobileSyncDataService.SyncAM(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseBaseModel.Message = "Lỗi hệ thống. Vui lòng thử lại sau hoặc liên hệ Quản trị viên để được hỗ trợ.";
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.ManipulationTicketBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
