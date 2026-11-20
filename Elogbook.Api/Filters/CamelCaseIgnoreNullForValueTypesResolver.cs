using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Elogbook.Api.Filters
{
    /// <summary>
    /// CamelCaseIgnoreNullForValueTypesResolver
    /// </summary>
    public class CamelCaseIgnoreNullForValueTypesResolver : CamelCasePropertyNamesContractResolver
    {
        /// <summary>
        /// Bỏ qua các giá trị null trong request
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != null
                && property.PropertyType.IsValueType
                && Nullable.GetUnderlyingType(property.PropertyType) == null)
            {
                property.NullValueHandling = NullValueHandling.Ignore;
            }

            return property;
        }
    }
}