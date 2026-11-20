using Elogbook.Core.Models.Request.Widget;
using Elogbook.Core.Models.Response.Widget;
using Elogbook.Core.Models;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWidgetBusiness
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        ResponseModel<List<WidgetViewModel>> FindByModule(string module);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addEditWidgetModel"></param>
        /// <returns></returns>
        ResponseModel<WidgetViewModel> Add(AddEditWidgetModel addEditWidgetModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addEditWidgetModel"></param>
        /// <returns></returns>
        ResponseModel<WidgetViewModel> Edit(AddEditWidgetModel addEditWidgetModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> Delete(int id, int currentUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> AddToOverview(int id, int currentUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> RemoveFromOverview(int id, int currentUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        ResponseModel<List<WidgetLabelModel>> FindByModuleNotOverview(string module);
    }
}
