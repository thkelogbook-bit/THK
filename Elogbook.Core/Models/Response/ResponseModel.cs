using System.Collections.Generic;
using System.Net;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Định nghĩa dữ liệu trả về cho API tìm kiếm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T>
    {
        /// <summary>
        /// Hàm dựng mặc định
        /// </summary>
        public ResponseModel()
        {
            ErrorMessages = new List<string>();
        }

        /// <summary>
        /// Hàm dựng với dữ liệu và thông tin lỗi
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errors"></param>
        public ResponseModel(T data, List<string> errors)
        {
            this.Data = data;
            this.ErrorMessages = errors;
        }

        /// <summary>
        /// Hàm dựng với dữ liệu, thông tin lỗi và mã lỗi
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <param name="StatusCode"></param>
        public ResponseModel(T data, string error, int StatusCode)
        {
            this.Data = data;
            this.ErrorMessages = new List<string>
            {
                error
            };
            this.StatusCode = StatusCode;
        }

        /// <summary>
        /// Hàm dựng với dữ liệu
        /// </summary>
        /// <param name="data"></param>
        public ResponseModel(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Error message
        /// </summary>
        public List<string> ErrorMessages { get; set; }

        /// <summary>
        /// Returned data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Status code
        /// </summary>
        public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
    }
}
