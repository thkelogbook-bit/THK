using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using System;
using System.Net;

namespace Elogbook.Business
{
    /// <summary>
    /// Đồng bộ dữ liệu PMIS
    /// </summary>
    public class BatchEtlBusiness : IBatchETLBusiness
    {
        private readonly IBatchEtlService _batchETLService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="batchETLService"></param>
        public BatchEtlBusiness(IBatchEtlService batchETLService)
        {
            _batchETLService = batchETLService;
        }

        public ResponseBaseModel RunBatch()
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                _batchETLService.RunBatch();
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = "Có lỗi xảy ra khi đồng bộ dữ liệu, vui lòng liên hệ quản trị hệ thống để được hỗ trợ.";
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "BatchEtlBusiness",
                    Environment.NewLine + CommonFunction.SerializeObjectToString("") + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
