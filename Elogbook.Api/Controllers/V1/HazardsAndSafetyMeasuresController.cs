using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Mối nguy và biện pháp an toàn
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/hazardsAndSafetyMeasures")]
    public class HazardsAndSafetyMeasuresController : BaseApiController
    {
        private readonly IHazardsAndSafetyMeasuresBusiness _hazardsAndSafetyMeasuresBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hazardsAndSafetyMeasuresBusiness"></param>
        public HazardsAndSafetyMeasuresController(IHazardsAndSafetyMeasuresBusiness hazardsAndSafetyMeasuresBusiness)
        {
            _hazardsAndSafetyMeasuresBusiness = hazardsAndSafetyMeasuresBusiness;
        }

        /// <summary>
        /// Tìm kiếm Mối nguy và biện pháp an toàn theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLNNKTH
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetAlls([FromBody] HazardsAndSafetyMeasuresSearchModel requestModel)
        {
            return _hazardsAndSafetyMeasuresBusiness.GetAlls(requestModel);
        }

        /// <summary>
        /// Tìm kiếm Mối nguy và biện pháp an toàn theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLNNKTH
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<HazardsAndSafetyMeasuresModel> GetById([FromUri] int id)
        {
            return _hazardsAndSafetyMeasuresBusiness.GetById(id);
        }

        /// <summary>
        /// Thêm mới Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> Create([FromBody] HazardsAndSafetyMeasuresAddEditModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _hazardsAndSafetyMeasuresBusiness.Create(requestModel);
        }

        /// <summary>
        /// Cập nhập Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> Update([FromBody] HazardsAndSafetyMeasuresAddEditModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _hazardsAndSafetyMeasuresBusiness.Update(requestModel);
        }

        /// <summary>
        /// Xóa Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel Delete([FromUri] int id)
        {
            return _hazardsAndSafetyMeasuresBusiness.Delete(id, CurrentUser.ID);
        }

        /// <summary>
        /// Lấy phân loại
        /// </summary>
        /// <returns></returns>
        [Route("findContentType")]
        [HttpGet]
        public List<ValueLabelCheckedModel> GetContentType()
        {
            return new List<ValueLabelCheckedModel>()
            {
                new ValueLabelCheckedModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelCheckedModel { Value = (int)EnumHazardsAndSafetyMeasures.Hazards, Label = EnumExtensions.GetEnumDescription(EnumHazardsAndSafetyMeasures.Hazards) },
                new ValueLabelCheckedModel { Value = (int)EnumHazardsAndSafetyMeasures.SafetyMeasures, Label = EnumExtensions.GetEnumDescription(EnumHazardsAndSafetyMeasures.SafetyMeasures) },
            };
        }

        /// <summary>
        /// Lấy danh sách theo Mối nguy hoặc Biện pháp an toàn
        /// </summary>
        /// <returns></returns>
        [Route("findByContentType")]
        [HttpGet]
        public List<ValueLabelCheckedModel> FindByContentType([FromUri] int contentType)
        {
            return _hazardsAndSafetyMeasuresBusiness.FindByContentType(contentType);
        }
    }
}
