using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý Tổ máy
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/powerUnit")]
    public class PowerUnitController : BaseApiController
    {
        private readonly IPowerUnitBusiness _powerUnitBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="powerUnitBusiness"></param>
        /// <param name="commonBusiness"></param>
        public PowerUnitController(IPowerUnitBusiness powerUnitBusiness,
            ICommonBusiness commonBusiness)
        {
            _powerUnitBusiness = powerUnitBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Trả về tất cả Tổ máy theo điều kiện truyền vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_TM
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetPowerUnits([FromBody] SearchPowerUnitModel requestModel)
        {
            return _powerUnitBusiness.GetPowerUnits(requestModel);
        }

        /// <summary>
        /// Trả về Tổ máy theo mã Tổ máy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_TM
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<PowerUnitModel> GetPowerUnitById([FromUri] int id)
        {
            return _powerUnitBusiness.GetPowerUnitById(id);
        }

        /// <summary>
        /// Thêm mới Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<PowerUnitModel> AddPowerUnit([FromBody] AddEditPowerUnitModel requestModel)
        {
            ResponseModel<PowerUnitModel> responseModel = new ResponseModel<PowerUnitModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_TM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _powerUnitBusiness.AddPowerUnit(requestModel);
        }

        /// <summary>
        /// Cập nhập Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<PowerUnitModel> UpdatePowerUnit([FromBody] AddEditPowerUnitModel requestModel)
        {
            ResponseModel<PowerUnitModel> responseModel = new ResponseModel<PowerUnitModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_TM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _powerUnitBusiness.UpdatePowerUnit(requestModel);
        }

        /// <summary>
        /// Xóa Tổ máy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeletePowerUnit([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_TM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _powerUnitBusiness.DeletePowerUnit(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra Tổ máy có đang sử dụng hay chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        public ResponseModel<bool> CheckInUsePowerUnit([FromUri] int id)
        {
            return _powerUnitBusiness.CheckInUsePowerUnit(id);
        }
    }
}