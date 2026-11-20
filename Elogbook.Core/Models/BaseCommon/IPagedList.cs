using System.Collections.Generic;

namespace Elogbook.Core.Models.BaseCommon
{
    /// <summary>
    /// IPagedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T>
        : IEnumerable<T>
    {
        /// <summary>
        /// Tổng record
        /// </summary>
        int TotalRecords { get; }

        /// <summary>
        /// Số trang
        /// </summary>
        int PageNo { get; }

        /// <summary>
        /// PageSize
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Tổng trang
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// FirstPage
        /// </summary>
        bool IsFirstPage { get; }

        /// <summary>
        /// IsLastPage
        /// </summary>
        bool IsLastPage { get; }
    }
}
