using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseCommon;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// HttpResponse
    /// </summary>
    public static class HttpResponse
    {
        /// <summary>
        /// OK
        /// </summary>
        /// <param name="data"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseBaseModel OK(object data = null,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ResponseBaseModel()
            {
                Content = data,
                StatusCode = (int)statusCode,
                Message = string.Empty
            };
        }

        /// <summary>
        /// OK
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseBaseModel<T> OK<T>(T data = default,
            string message = "",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (!string.IsNullOrWhiteSpace(message))
                message = "";

            return new ResponseBaseModel<T>()
            {
                Content = data,
                StatusCode = (int)statusCode,
                Message = message
            };
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseBaseModel Error(string errorMessage = "",
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new ResponseBaseModel()
            {
                Content = null,
                Message = errorMessage,
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="errorMessage"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseListModel<T> Error<T>(string errorMessage = "",
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new ResponseListModel<T>()
            {
                Content = null,
                Message = errorMessage,
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// OkList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseBaseModel<T> OkList<T>(IPagedList<T> page,
            string message = "",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ResponseListModel<T>()
            {
                Content = new ListDataInfo<T>(page),
                Message = message,
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// OkList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseBaseModel<T> OkList<T>(List<T> page,
            string message = "",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ResponseListModel<T>()
            {
                Content = new ListDataInfo<T>(page),
                Message = message,
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// OkList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <param name="totalPages"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseBaseModel<T> OkList<T>(IEnumerable<T> page,
            int pageNumber,
            int pageSize,
            int totalRecords,
            int totalPages,
            string message = "",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ResponseListModel<T>()
            {
                Content = new ListDataInfo<T>(page,
                    pageNumber,
                    pageSize,
                    totalRecords,
                    totalPages),
                Message = message,
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// OkList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ResponseBaseModel<T> OkList<T>(IEnumerable<T> page,
            int pageNo = CommonConstant.DEFAULT_PAGE_NUMBER,
            int pageSize = CommonConstant.DEFAULT_PAGE_SIZE,
            string message = "",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ResponseListModel<T>()
            {
                Content = new ListDataInfo<T>(page, pageNo, pageSize),
                Message = message,
                StatusCode = (int)statusCode
            };
        }
    }
}
