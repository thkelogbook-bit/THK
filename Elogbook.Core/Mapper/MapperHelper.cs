using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Core.Mapper
{
    /// <summary>
    /// MapperHelper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MapperHelper<T> where T : new()
    {
        /// <summary>
        /// Custom function to ignore properties when mapping current model to new model
        /// Set attribute MapperIgnoreAttribute for each property need ignore
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Map(T source)
        {
            var result = new T();

            if (source == null)
                return result;

            var props = typeof(T)
                .GetProperties()
                .Where(p => p.CanWrite)
                .Where(p => !p.CustomAttributes.Any(a => a.AttributeType == typeof(MapperIgnoreAttribute)))
                .ToList();

            foreach (var prop in props)
            {
                prop.SetValue(result, prop.GetValue(source));
            }

            return result;
        }

        /// <summary>
        /// Custom function to ignore properties for list model
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map(IEnumerable<T> source)
        {
            if (source == null)
            {
                return default(IEnumerable<T>);
            }

            List<T> result = new List<T>();

            foreach (T item in source)
            {
                result.Add(Map(item));
            }

            return result;
        }
    }
}
