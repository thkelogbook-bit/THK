using System;
using System.Collections.Generic;
using System.Linq;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Helpers
{
    /// <summary>
    /// TypeHelpers
    /// </summary>
    public static class TypeHelpers
    {
        /// <summary>
        /// check is nullable for a property
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Check is collection for a property
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollectionType(this Type type)
        {
            return type.IsGenericType && typeof(IEnumerable<>).MakeGenericType(type.GetGenericArguments())
                .IsAssignableFrom(type);
        }

        /// <summary>
        /// Check is numeric type for a property
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            return TypeConstants.NumericType.Contains(type);
        }
    }
}
