using System;
using System.Linq.Expressions;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Helpers.ExpressionHelpers
{
    /// <summary>
    /// TextExpressionHelper
    /// </summary>
    public class TextExpressionHelper : AbstractExpressionHelper
    {
        /// <summary>
        /// TextExpressionHelper
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="operator"></param>
        public TextExpressionHelper(string property, object value, string @operator = CompareOperators.Equal)
            : base(property, value, @operator)
        {
        }

        /// <summary>
        /// Get Expression for text type
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override Expression GetExpression(Expression parameter)
        {
            if (Operator == CompareOperators.Contains || Operator == CompareOperators.Equal)
            {
                var dataType = CompareValue != null ? typeof(string) : typeof(object);
                if (Operator == CompareOperators.Equal)
                    Operator = nameof(string.Equals);
                var containsMethod = typeof(string).GetMethod(Operator, new[] { dataType });
                var tolowerMethod = typeof(string).GetMethod(nameof(string.ToLower), new Type[] { });
                var toLowerExp = Expression.Call(parameter, tolowerMethod);
                return Expression.Call(toLowerExp, containsMethod, Expression.Constant(CompareValue));
            }
            throw new InvalidOperationException("Operator not support");
        }
    }
}
