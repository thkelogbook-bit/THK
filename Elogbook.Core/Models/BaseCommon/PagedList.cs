using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Models.BaseCommon
{
    /// <summary>
    /// PagedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : IPagedList<T>
    {
        private readonly IEnumerator<T> _source;

        /// <summary>
        /// Total records for source
        /// </summary>
        public int TotalRecords { get; private set; }

        /// <summary>
        /// Page Index for current page
        /// </summary>
        public int PageNo { get; private set; } = 1;

        /// <summary>
        /// Page size per page
        /// </summary>
        public int PageSize { get; private set; } = DEFAULT_PAGE_SIZE;

        /// <summary>
        /// Total page for source
        /// </summary>
        public int TotalPages
        {
            get
            {
                int total;
                if (TotalRecords == 0)
                {
                    total = 0;
                }
                else
                {
                    total = (TotalRecords / PageSize + (TotalRecords % PageSize > 0 ? 1 : 0));
                }

                return total;
            }
        }

        /// <summary>
        /// IsFirstPage
        /// </summary>
        public bool IsFirstPage { get { return PageNo == 1; } }

        /// <summary>
        /// IsLastPage
        /// </summary>
        public bool IsLastPage { get { return PageNo == TotalPages; } }

        private int _skip
        {
            get
            {
                return (PageNo - 1) * PageSize;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="pageNumber">Page current index for this source</param>
        /// <param name="pageSize">Size of records per page</param>
        public PagedList(int pageNumber = 1,
            int pageSize = DEFAULT_PAGE_SIZE)
        {
            PageNo = pageNumber < 1 ? 1 : pageNumber;
            PageSize = (pageSize < 0 || pageSize > DEFAULT_PAGE_SIZE) ? DEFAULT_PAGE_SIZE : pageSize;
        }

        /// <summary>
        /// When to list based on IQueryable
        /// </summary>
        /// <param name="source">Source for page</param>
        /// <param name="pageNumber">Page current index for this source</param>
        /// <param name="pageSize">Size of records per page</param>
        public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
            : this(pageNumber, pageSize)
        {
            TotalRecords = source.Count();
            _source = source?
                .Skip(_skip)
                .Take(PageSize)
                .ToList()
                .GetEnumerator();
        }

        /// <summary>
        /// When to list based on IEnumerable
        /// </summary>
        /// <param name="source">Source for page</param>
        /// <param name="pageNumber">Page current index for this source</param>
        /// <param name="pageSize">Size of records per page</param>
        public PagedList(IEnumerable<T> source, int pageNumber, int pageSize)
            : this(pageNumber, pageSize)
        {
            TotalRecords = source.Count();
            _source = source?
                .Skip(_skip)
                .Take(PageSize)
                .ToList()
                .GetEnumerator();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _source;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source;
        }
    }
}
