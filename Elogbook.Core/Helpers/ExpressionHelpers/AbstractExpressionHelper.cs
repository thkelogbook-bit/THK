using System;
using System.Linq;
using System.Linq.Expressions;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Helpers.ExpressionHelpers
{
    /// <summary>
    /// Expression Helper
    /// </summary>
    public abstract class AbstractExpressionHelper
    {
        /// <summary>
        /// Value Compare
        /// </summary>
        public object CompareValue { get; set; }

        /// <summary>
        /// Part
        /// </summary>
        public string[] PropertyParts
        {
            get; private set;
        }

        /// <summary>
        /// property to search
        /// </summary>
        public string Property
        {
            get
            {
                var count = PropertyParts.Count();
                return PropertyParts.Length > 0 ? PropertyParts[count - 1] : string.Empty;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    PropertyParts = new string[] { };

                if (!string.IsNullOrWhiteSpace(value))
                    PropertyParts = value.Split('.');
            }
        }

        /// <summary>
        /// Opearator to search
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="operator"></param>
        protected AbstractExpressionHelper(string property, object value, string @operator = CompareOperators.Equal)
        {
            CompareValue = value;
            Operator = @operator;
            Property = property;
        }

        /// <summary>
        /// PropertyExpression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
#pragma warning disable S2326 // Unused type parameters should be removed
        public Expression PropertyExpression<T>(ParameterExpression parameter)
#pragma warning restore S2326 // Unused type parameters should be removed
        {
            return PropertyExpression(parameter, Property);
        }

        /// <summary>
        /// Get Expression for a property
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public Expression PropertyExpression(ParameterExpression parameter, string propName)
        {
            Expression property = Expression.Property(parameter, propName);
            if (property.Type.IsNullableType())
                property = NullableExpression(property);

            if (property.Type.IsCollectionType())
                property = CollectionExpression(property);
            else
                property = GetExpression(property);

            if (!property.Type.IsValueType && property.Type.IsNullableType())
                return Combine(ReferenceTypeNullableExpression(property), property);
            return property;
        }

        /// <summary>
        /// Get Expression for property
        /// </summary>
        /// <returns></returns>
        protected abstract Expression GetExpression(Expression parameter);

        /// <summary>
        /// Get expression when property is Nullable type
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression NullableExpression(Expression expression)
        {
            return Expression.Property(expression, "Value");
        }

        /// <summary>
        /// Get expression when property is ValueType and Nullable Type
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression ReferenceTypeNullableExpression(Expression expression)
        {
            return Expression.NotEqual(expression, Expression.Constant(null));
        }

        /// <summary>
        /// Get expression when property is Collection type
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression CollectionExpression(Expression expression)
        {
            var parameter = Expression.Parameter(expression.Type.GetGenericArguments()[0]);
            var searhExpression = GetExpression(parameter);
            if (searhExpression != null)
            {
                var asQueryable = (typeof(Queryable)).GetMethods()
                    .Single(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethod)
                    .MakeGenericMethod(expression.Type.GetGenericArguments());

                var anyMethod = (typeof(Queryable)).GetMethods()
                    .Single(m => m.Name == nameof(Queryable.Any) && m.GetParameters().Length == 2)
                    .MakeGenericMethod(expression.Type.GetGenericArguments());

                searhExpression = Expression.Call(null,
                    anyMethod,
                    Expression.Call(null, asQueryable, expression),
                    Expression.Lambda(searhExpression, parameter));
            }
            return searhExpression;
        }

        /// <summary>
        /// Combine two expressions
        /// </summary>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        private Expression Combine(Expression expression1, Expression expression2)
        {
            return expression1 == null ? expression2 : Expression.AndAlso(expression1, expression2);
        }

        /// <summary>
        /// Cast to any type if we want
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        protected object Cast(object value, Type valueType)
        {
            return Convert.ChangeType(value, valueType);
        }
    }
}
