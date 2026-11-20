using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý Cương vị trong ca
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/positionInShift")]
    public class PositionInShiftController : BaseApiController
    {
        private readonly IPositionInShiftBusiness _positionInShiftBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="positionInShiftBusiness"></param>
        public PositionInShiftController(IPositionInShiftBusiness positionInShiftBusiness)
        {
            _positionInShiftBusiness = positionInShiftBusiness;
        }

        /// <summary>
        /// Trả về tất cả Cương vị trong ca theo Cương vị ID
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetPositionInShifts([FromBody] SearchPositionInShiftModel requestModel)
        {
            return _positionInShiftBusiness.GetPositionInShifts(requestModel);
        }

        /// <summary>
        /// Trả về Cương vị trong ca theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<PositionInShiftModel> GetPositionInShiftById([FromUri] int id)
        {
            return _positionInShiftBusiness.GetPositionInShiftById(id);
        }

        /// <summary>
        /// Thêm mới Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<int> AddPositionInShift([FromBody] AddEditPositionInShiftModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _positionInShiftBusiness.AddPositionInShift(requestModel);
        }

        /// <summary>
        /// Cập nhập Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<PositionInShiftModel> UpdatePositionInShift([FromBody] AddEditPositionInShiftModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _positionInShiftBusiness.UpdatePositionInShift(requestModel);
        }

        /// <summary>
        /// Xóa Cương vị trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeletePositionInShift([FromUri] int id)
        {
            return _positionInShiftBusiness.DeletePositionInShift(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra Cương vị trong ca có đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        public ResponseModel<bool> CheckInUsePositionInShift([FromUri] int id)
        {
            return _positionInShiftBusiness.CheckInUsePositionInShift(id);
        }
    }
}