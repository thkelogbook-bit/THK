using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Class to define common methods for enum
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get enum description from enum value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}
