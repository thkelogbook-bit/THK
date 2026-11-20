using Newtonsoft.Json;
using System;

namespace Elogbook.Core
{
    /// <summary>
    /// Định nghĩa đối chuyển đổi dữ liệu kiểu chuổi
    /// </summary>
    public class TrimmingConverter : JsonConverter
    {
        /// <summary>
        /// Xác định tối tượng có thế chuyển đổi
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        /// <summary>
        /// Chuyển đổi dữ liệu vào
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                if (reader.Value != null)
                {
                    var currentValue = (reader.Value as string);
                    if (!string.IsNullOrWhiteSpace(currentValue))
                    {
                        return currentValue.Trim();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }

            return reader.Value;
        }

        /// <summary>
        /// Chuyển đổi dữ liệu ra
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var outputValue = (string)value;
            if (outputValue == null)
                writer.WriteNull();
            else
                writer.WriteValue(outputValue.Trim());
        }
    }
}
