using System;
using System.Linq.Expressions;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Helpers.ExpressionHelpers
{
    /// <summary>
    /// NumericExpressionHelper
    /// </summary>
    public class NumericExpressionHelper : AbstractExpressionHelper
    {
        /// <summary>
        /// NumericExpressionHelper
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <param name="operator"></param>
        public NumericExpressionHelper(string property, object value, object value2 = null, string @operator = CompareOperators.Equal)
            : base(property, value, @operator)
        {
            CompareValue2 = value2?.ToString() ?? double.MaxValue.ToString();
        }

        /// <summary>
        /// CompareValue2
        /// </summary>
        public string CompareValue2 { get; set; }

        /// <summary>
        /// Get Expression for numeric type
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override Expression GetExpression(Expression parameter)
        {
            var compareValue = Cast(CompareValue, parameter.Type);
            switch (Operator)
            {
                case CompareOperators.Equal:
                    return Expression.Equal(parameter, Expression.Constant(compareValue));

                case CompareOperators.GreaterThan:
                    return Expression.GreaterThan(parameter, Expression.Constant(compareValue));

                case CompareOperators.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(parameter, Expression.Constant(compareValue));

                case CompareOperators.InRange:
                    return DateInRangeExpression(parameter);

                case CompareOperators.LessThan:
                    return Expression.LessThan(parameter, Expression.Constant(compareValue));

                case CompareOperators.LessThanOrEqual:
                    return Expression.LessThanOrEqual(parameter, Expression.Constant(compareValue));

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
            var compareValue = Cast(CompareValue, property.Type);
            var compareValue2 = Cast(CompareValue2, property.Type);

            var topRangeExp = Expression.GreaterThanOrEqual(property, Expression.Constant(compareValue));
            var bottonExp = Expression.LessThanOrEqual(property, Expression.Constant(compareValue2));
            return Expression.And(topRangeExp, bottonExp);
        }
    }
}
