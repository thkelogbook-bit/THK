using System;
using System.Linq.Expressions;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Helpers.ExpressionHelpers
{
    /// <summary>
    /// DateTimeExpressionHelper
    /// </summary>
    public class DateTimeExpressionHelper : AbstractExpressionHelper
    {
        /// <summary>
        /// DateTimeExpressionHelper
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <param name="operator"></param>
        public DateTimeExpressionHelper(string property, object value, object value2 = null, string @operator = CompareOperators.Equal)
            : base(property, value, @operator)
        {
            CompareValue2 = value?.ToString() ?? DateTime.MaxValue.ToString();
        }

        /// <summary>
        /// ActualComapareValue
        /// </summary>
        public DateTime ActualComapareValue { get { return Convert.ToDateTime(CompareValue); } }

        /// <summary>
        /// CompareValue2
        /// </summary>
        public string CompareValue2 { get; set; }

        /// <summary>
        /// ActualComapareValue2
        /// </summary>
        public DateTime ActualComapareValue2 { get { return Convert.ToDateTime(CompareValue2); } }

        /// <summary>
        /// Get expression for DateTime type
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override Expression GetExpression(Expression parameter)
        {
            switch (Operator)
            {
                case CompareOperators.Equal:
                    return Expression.Equal(parameter, Expression.Constant(ActualComapareValue));

                case CompareOperators.GreaterThan:
                    return Expression.GreaterThan(parameter, Expression.Constant(ActualComapareValue));

                case CompareOperators.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(parameter, Expression.Constant(ActualComapareValue));

                case CompareOperators.InRange:
                    return DateInRangeExpression(parameter);

                case CompareOperators.LessThan:
                    return Expression.LessThan(parameter, Expression.Constant(ActualComapareValue));

                case CompareOperators.LessThanOrEqual:
                    return Expression.LessThanOrEqual(parameter, Expression.Constant(ActualComapareValue));

                default:
                    throw new InvalidOperationException("Operator not support");
            }
        }

        /// <summary>
        /// When check date range,
        /// Property value must GreaterOrEqual fromDate and
        /// LessOrEqual ToDate
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private Expression DateInRangeExpression(Expression property)
        {
            var topRangeExp = Expression.GreaterThanOrEqual(property, Expression.Constant(ActualComapareValue));
            var bottonExp = Expression.LessThanOrEqual(property, Expression.Constant(ActualComapareValue2));
            return Expression.And(topRangeExp, bottonExp);
        }
    }
}
