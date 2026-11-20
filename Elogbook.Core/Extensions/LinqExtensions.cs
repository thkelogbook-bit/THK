using Elogbook.Core.Enums;
using Elogbook.Core.Helpers;
using Elogbook.Core.Helpers.ExpressionHelpers;
using Elogbook.Core.Models.BaseCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Extensions
{
    /// <summary>
    /// LinqExtensions
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Filter custom with 1 key - 1 value - 1 operator.
        ///     Key: any property must existing in entity type
        ///     Operator: the method to compare(contains, equals,...)
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="source">List to query</param>
        /// <param name="property">property name need to filter</param>
        /// <param name="value">value to filter</param>
        /// <param name="operator">method to filter</param>
        /// <returns>filtered source</returns>
        public static IQueryable<T> FilterCustom<T>(this IQueryable<T> source
            , string property
            , object value
            , string @operator = CompareOperators.Contains)
        {
            if (value == null)
                return source;
            AbstractExpressionHelper search;

            var parameter = Expression.Parameter(typeof(T), "a");
            Expression propertyExpress = Expression.Property(parameter, property);

            // Check property type
            if (propertyExpress.Type.IsNumericType())
                search = new NumericExpressionHelper(property, value, @operator: @operator);
            else if (propertyExpress.Type == typeof(DateTime))
                search = new DateTimeExpressionHelper(property, value, @operator: @operator);
            else if (propertyExpress.Type == typeof(Guid) || propertyExpress.Type == typeof(Guid?))
                search = new GuidExpressionHelper(property, value);
            else
                search = new TextExpressionHelper(property, value, @operator: @operator);

            ParameterExpression[] parameters = new[] { parameter };
            var lambda = Expression.Lambda<Func<T, bool>>(search.PropertyExpression<T>(parameter), parameters);
            return source.Where(lambda);
        }

        /// <summary>
        /// Filter data with list expressions,
        /// expressions can join by Or/And operator
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="source">data source</param>
        /// <param name="filters">filters to create expressions</param>
        /// <param name="joinType">or/and</param>
        /// <returns></returns>
        public static IQueryable<T> FilterCustom<T>(this IQueryable<T> source
            , IEnumerable<Filter> filters
            , ExpressionJoinType joinType = ExpressionJoinType.And)
        {
            if (filters != null && filters.Any())
            {
                Expression expression = null;

                AbstractExpressionHelper search;
                var parameter = Expression.Parameter(typeof(T), "a");

                foreach (var filter in filters)
                {
                    var filterOperator = filter.Operator ?? CompareOperators.Equal;

                    Expression propertyExpress = Expression.Property(parameter, filter.Prop);

                    // Check property type
                    if (propertyExpress.Type.IsNumericType())
                        search = new NumericExpressionHelper(filter.Prop, filter.Val, filter.Val2, @operator: filterOperator);
                    else if (propertyExpress.Type == typeof(DateTime))
                        search = new DateTimeExpressionHelper(filter.Prop, filter.Val, filter.Val2, @operator: filterOperator);
                    else if (propertyExpress.Type == typeof(Guid) || propertyExpress.Type == typeof(Guid?))
                        search = new GuidExpressionHelper(filter.Prop, filter.Val);
                    else
                        search = new TextExpressionHelper(filter.Prop, filter.Val, @operator: filterOperator);

                    /* check join type of expressions
                     */
                    if (expression == null)
                        expression = search.PropertyExpression<T>(parameter);
                    else
                    {
                        expression = joinType == ExpressionJoinType.And ?
                            Expression.And(expression, search.PropertyExpression<T>(parameter))
                            : Expression.Or(expression, search.PropertyExpression<T>(parameter));
                    }
                }

                if (expression != null)
                {
                    ParameterExpression[] parameters = new[] { parameter };
                    var lambda = Expression.Lambda<Func<T, bool>>(expression, parameters);

                    return source.Where(lambda);
                }
            }

            return source;
        }

        /// <summary>
        /// Paging the result in query
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="source">list objects</param>
        /// <param name="pageNo">page number</param>
        /// <param name="pageSize">size per page</param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source
            , int pageNo = DEFAULT_PAGE_NUMBER
            , int pageSize = DEFAULT_PAGE_SIZE)
        {
            return new PagedList<T>(source, pageNo, pageSize);
        }

        /// <summary>
        /// Paging the result in query
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="source">list objects</param>
        /// <param name="pageNo">page number</param>
        /// <param name="pageSize">size per page</param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this List<T> source
            , int pageNo = DEFAULT_PAGE_NUMBER
            , int pageSize = DEFAULT_PAGE_SIZE)
        {
            return new PagedList<T>(source, pageNo, pageSize);
        }

        /// <summary>
        /// dynamic sort object by sort field and sort type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">datasource before apply sort</param>
        /// <param name="properties">fields sort for this datasource</param>
        /// <param name="sortOrder">sort type: asc or desc</param>
        /// <returns>list data after apply sort</returns>
        public static IQueryable<T> OrderByCustom<T>(this IQueryable<T> source, string properties, string sortOrder)
        {
            try
            {
                // build OrderBy query
                var strOrderBy = string.Format("OrderBy{0}", sortOrder == SortType.DESC.ToString() ? "descending" : "");
                // define parameter for sort query
                var parameter = Expression.Parameter(source.ElementType, "p");
                // build sort fields
                MemberExpression memberAccess = null;
                foreach (var property in properties.Split('.'))
                {
                    memberAccess = Expression.Property(memberAccess ?? (parameter as Expression), property);
                }
                // final OrderBy query
                if (memberAccess != null)
                {
                    var orderByLambda = Expression.Lambda(memberAccess, parameter);
                    var result = Expression.Call(
                        typeof(Queryable),
                        strOrderBy,
                        new[] { source.ElementType, memberAccess.Type },
                        source.Expression,
                        Expression.Quote(orderByLambda));

                    // If has order by fields then return data after order by, if no return current data source
                    return source.Provider.CreateQuery<T>(result);
                }

                return source;
            }
            catch
            {
                // Incase build query order by has any error then return current datasource
                return source;
            }
        }
    }
}
