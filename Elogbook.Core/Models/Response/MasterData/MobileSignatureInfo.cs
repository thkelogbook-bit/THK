
using Newtonsoft.Json;
using System.Text;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chữ ký
    /// </summary>
    public class MobileSignatureInfo
    {
        /// <summary>
        /// Mã chữ ký
        /// </summary>
        public int ID { get; set; }

        private string _signatureContent;
        /// <summary>
        /// Nội dung
        /// </summary>
        public string SignatureContent
        {
            get
            {
                if (_signatureContent != null)
                {
                    return _signatureContent;
                }

                if (SignatureContentByte != null)
                {
                    return Encoding.UTF8.GetString(SignatureContentByte);
                }
                return null;
            }
            set { _signatureContent = value; }
        }

        /// <summary>
        /// Nội dung byte
        /// </summary>
        [JsonIgnore]
        public byte[] SignatureContentByte { get; set; }

        /// <summary>
        /// Loại chữ ký
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
