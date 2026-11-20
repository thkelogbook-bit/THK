using Elogbook.Core.Constants;
using Elogbook.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Elogbook.Core
{
    /// <summary>
    /// Định nghĩa các phương thức dùng chung
    /// </summary>
    public static class CommonFunction
    {
        /// <summary>
        /// Lấy Base64String từ file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetBase64StringFromFile(string filePath)
        {
            var result = string.Empty;

            using (var binaryFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var buffer = new byte[1024];
                using (var ms = new MemoryStream())
                {
                    int read;
                    while ((read = binaryFile.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    binaryFile.Close();
                    result = Convert.ToBase64String(ms.ToArray());
                }
            }

            return result;
        }

        /// <summary>
        /// Lưu trữ file vào thư mục
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        public static void CreateFile(string filePath, byte[] data)
        {
            File.WriteAllBytes(filePath, data);
        }

        /// <summary>
        /// Tạo key mapping mobile app và server
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetSyncDataKey(GenerateSyncDataKeyInfo info)
        {
            string key = string.Empty;
            var date = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}";

            switch (info.KeyType)
            {
                case "NKVH":
                    break;
                case "TSVH_PARAMETER":
                    key = $"TSVH_P{info.ParameterID}_{info.FactoryID}_{info.CycleID}_{info.OperationPositionID}_{date}";
                    break;
                case "TSVH_HOUR":
                    key = $"TSVH_H{info.Hour}_{info.FactoryID}_{info.CycleID}_{info.OperationPositionID}_{date}";
                    break;
            }

            return key;
        }

        /// <summary>
        /// Serialize object log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        /// <returns></returns>
        public static string SerializeObjectToString<T>(this T toSerialize)
        {
            // Format object to json
            return JsonConvert.SerializeObject(toSerialize, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Kiểm tra xác thực api key
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ValidateApiKey(HttpRequestMessage request)
        {
            var errorMessage = CommonConstant.API_KEY_ERROR;

            try
            {
                if (request.Headers.TryGetValues("apiKey", out IEnumerable<string> requestHeaders) && requestHeaders != null)
                {
                    var apiKey = requestHeaders.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(apiKey)
                        && apiKey.Equals(CommonConstant.API_KEY))
                    {
                        errorMessage = string.Empty;
                    }
                }
            }
            catch
            {
                errorMessage = string.Empty;
            }

            return errorMessage;
        }

    }
}
