using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IHazardsAndSafetyMeasuresService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentText"></param>
        /// <param name="contentType"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        bool CheckDuplicate(int id, string contentText, int contentType, bool isCreate);

        /// <summary>
        /// Thêm mới Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool Create(HazardsAndSafetyMeasuresAddEditModel requestModel);

        /// <summary>
        /// Xóa Mối nguy và biện pháp an toàn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseBaseModel Delete(int id, int userId);
        
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetAlls(HazardsAndSafetyMeasuresSearchModel requestModel);
        
        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HazardsAndSafetyMeasuresModel GetById(int id);
        
        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool Update(HazardsAndSafetyMeasuresAddEditModel requestModel);

        /// <summary>
        /// Lấy danh sách theo loại
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        List<ValueLabelCheckedModel> FindByContentType(int contentType);
    }
}
