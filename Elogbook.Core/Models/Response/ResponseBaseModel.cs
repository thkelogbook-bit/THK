using System.Net;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Định nghĩa thông tin dữ liệu trả về cho người dùng
    /// </summary>
    public class ResponseBaseModel
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public ResponseBaseModel()
        {
            Content = null;
        }

        /// <summary>
        /// Trạng thái của API, mặc định thành công
        /// </summary>
        public int StatusCode { get; set; } = (int)HttpStatusCode.OK;

        /// <summary>
        /// Lỗi nhận được từ api
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public virtual object Content { get; set; }
    }

    /// <summary>
    /// Định nghĩa dữ liệu trả về cho người dùng
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseBaseModel<T> : ResponseBaseModel
    {
        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public new T Content { get; set; }
    }

    /// <summary>
    /// Định nghĩa danh sách dữ liệu trả về cho người dùng
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseListModel<T> : ResponseBaseModel<T>
    {
        /// <summary>
        /// Danh sách dữ liệu trả về
        /// </summary>
        public new ListDataInfo<T> Content { get; set; }
    }
}
