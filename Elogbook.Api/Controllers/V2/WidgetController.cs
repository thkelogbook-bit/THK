using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Microsoft.Web.Http;
using System.Web.Http;
using Hangfire;
using Elogbook.Core.Models.Request.Widget;
using Elogbook.Core.Models.Response.Widget;
using System.Collections.Generic;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Widget
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/widget")]
    public class WidgetController : BaseApiController
    {
        private readonly IWidgetBusiness _widgetBusiness;


        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="widgetBusiness"></param>
        public WidgetController(IWidgetBusiness widgetBusiness)
        {
            _widgetBusiness = widgetBusiness;
        }

        /// <summary>
        /// Find by module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseModel<List<WidgetViewModel>> FindByModule([FromParameter("module")] string module)
        {
            return _widgetBusiness.FindByModule(module);
        }

        /// <summary>
        /// Tạo widget
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<WidgetViewModel> Add([FromBody] AddEditWidgetModel requestModel)
        {

            requestModel.CurrentUserID = CurrentUser.ID;
            return _widgetBusiness.Add(requestModel);
        }

        /// <summary>
        /// Update widget
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<WidgetViewModel> Update([FromBody] AddEditWidgetModel requestModel)
        {

            requestModel.CurrentUserID = CurrentUser.ID;
            return _widgetBusiness.Edit(requestModel);
        }

        /// <summary>
        /// Update widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> Delete([FromUri] int id)
        {
            return _widgetBusiness.Delete(id, CurrentUser.ID);
        }

        /// <summary>
        /// Update widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("addToOverview/{id}")]
        [HttpPut]
        public ResponseModel<bool> AddToOverview([FromUri] int id)
        {
            return _widgetBusiness.AddToOverview(id, CurrentUser.ID);
        }

        /// <summary>
        /// Update widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("removeFromOverview/{id}")]
        [HttpPut]
        public ResponseModel<bool> RemoveFromOverview([FromUri] int id)
        {
            return _widgetBusiness.RemoveFromOverview(id, CurrentUser.ID);
        }

        /// <summary>
        /// Find by module not in overview
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [Route("findNotOverview")]
        [HttpGet]
        public ResponseModel<List<WidgetLabelModel>> FindByModuleNotOverview([FromParameter("module")] string module)
        {
            return _widgetBusiness.FindByModuleNotOverview(module);
        }
    }
}