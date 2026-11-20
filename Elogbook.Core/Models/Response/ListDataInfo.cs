using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseCommon;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// ListDataInfo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListDataInfo<T> : BaseListDataInfo<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="page"></param>
        public ListDataInfo(IPagedList<T> page) : base(page)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        public ListDataInfo(IEnumerable<T> page,
            int pageNo = CommonConstant.DEFAULT_PAGE_NUMBER,
            int pageSize = CommonConstant.DEFAULT_PAGE_SIZE)
        {
            TotalElements = page.Count();
            PageNo = pageNo;
            PageSize = pageSize;

            int total;
            if (TotalElements == 0)
            {
                total = 0;
            }
            else
            {
                total = (TotalElements / PageSize + (TotalElements % PageSize > 0 ? 1 : 0));
            }
            TotalPage = total;
            Data = page.Skip(Skip).Take(PageSize).ToList();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <param name="totalPages"></param>
        public ListDataInfo(IEnumerable<T> page,
            int pageNumber,
            int pageSize,
            int totalRecords,
            int totalPages)
        {
            TotalElements = totalRecords;
            PageNo = pageNumber;
            PageSize = pageSize;
            TotalPage = totalPages;
            Data = page.ToList();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public ListDataInfo(IQueryable<T> page,
            int pageNumber = CommonConstant.DEFAULT_PAGE_NUMBER,
            int pageSize = CommonConstant.DEFAULT_PAGE_SIZE)
        {
            TotalElements = page.Count();
            PageNo = pageNumber;
            PageSize = pageSize;

            int total;
            if (TotalElements == 0)
            {
                total = 0;
            }
            else
            {
                total = (TotalElements / PageSize + (TotalElements % PageSize > 0 ? 1 : 0));
            }

            TotalPage = total;
            Data = page.Skip(Skip).Take(PageSize).ToList();
        }

        private int Skip { get { return (PageNo - 1) * PageSize; } }
    }
}
