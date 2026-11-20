using AutoMapper;
using System.ComponentModel;

namespace Elogbook.Core.Mapper
{
    /// <summary>
    /// Ignore mapping
    /// </summary>
    public static class IgnoreNoMapExtensions
    {
        /// <summary>
        /// Function to ignore properties in model use AutoMapper when set attribute MapperIgnoreAttribute for each property
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> IgnoreNoMap<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            foreach (var property in sourceType.GetProperties())
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(sourceType)[property.Name];
                MapperIgnoreAttribute attribute = (MapperIgnoreAttribute)descriptor.Attributes[typeof(MapperIgnoreAttribute)];
                try
                {
                    if (attribute != null)
                        expression.ForMember(property.Name, opt => opt.Ignore());
                }
                catch
                {
                    // Ignore case
                }
            }

            return expression;
        }
    }
}
