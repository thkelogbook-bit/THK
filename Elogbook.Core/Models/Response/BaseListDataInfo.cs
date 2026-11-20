using Elogbook.Core.Models.BaseCommon;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Dữ liệu trả về lưới danh sách
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseListDataInfo<T>
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public BaseListDataInfo()
        {
        }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="page"></param>
        public BaseListDataInfo(IPagedList<T> page)
        {
            TotalPage = page.TotalPages;
            TotalElements = page.TotalRecords;
            PageNo = page.PageNo;
            PageSize = page.PageSize;
            Data = page.ToList();
        }

        /// <summary>
        /// Danh sách dữ liệu của page
        /// </summary>
        public List<T> Data { get; protected set; }

        /// <summary>
        /// Tổng số page
        /// </summary>
        public int TotalPage { get; protected set; }

        /// <summary>
        /// Tổng số dòng dữ liệu tìm thấy
        /// </summary>
        public int TotalElements { get; protected set; }

        /// <summary>
        /// Page hiện tại
        /// </summary>
        public int PageNo { get; protected set; }

        /// <summary>
        /// Số dòng hiện tại trên page
        /// </summary>
        public int PageSize { get; protected set; }
    }
}
