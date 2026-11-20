using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IHazardsAndSafetyMeasuresBusiness
    {
        /// <summary>
        /// Tìm kiếm Mối nguy và biện pháp an toàn theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetAlls(HazardsAndSafetyMeasuresSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm Mối nguy và biện pháp an toàn theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<HazardsAndSafetyMeasuresModel> GetById(int id);

        /// <summary>
        /// Thêm mới Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> Create(HazardsAndSafetyMeasuresAddEditModel requestModel);

        /// <summary>
        /// Cập nhập Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> Update(HazardsAndSafetyMeasuresAddEditModel requestModel);

        /// <summary>
        /// Xóa Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseBaseModel Delete(int id, int userId);

        /// <summary>
        /// Lấy theo Mối nguy hoặc Biện pháp an toàn
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        List<ValueLabelCheckedModel> FindByContentType(int contentType);
    }
}
