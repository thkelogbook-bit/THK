using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Widget;
using Elogbook.Core.Models.Response.Widget;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWidgetService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        List<WidgetViewModel> FindByModule(string module);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addEditWidgetModel"></param>
        /// <returns></returns>
        WidgetViewModel Add(AddEditWidgetModel addEditWidgetModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addEditWidgetModel"></param>
        /// <returns></returns>
        WidgetViewModel Edit(AddEditWidgetModel addEditWidgetModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool Delete(int id, int currentUserId);

        bool DeleteByReportId(int reportId, int currentUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool AddToOverview(int id, int currentUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool RemoveFromOverview(int id, int currentUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        List<WidgetLabelModel> FindByModuleNotOverview(string module);

    }
}
