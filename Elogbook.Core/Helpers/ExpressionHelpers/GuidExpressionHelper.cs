using System;
using System.Linq.Expressions;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Core.Helpers.ExpressionHelpers
{
    /// <summary>
    /// Expression Helper
    /// </summary>
    public class GuidExpressionHelper : AbstractExpressionHelper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public GuidExpressionHelper(string property, object value)
            : base(property, value, CompareOperators.Equal)
        {
        }

        /// <summary>
        /// Get expression for Guid type
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override Expression GetExpression(Expression parameter)
        {
            var compareVal = Guid.Parse(CompareValue.ToString());
            return Expression.Equal(parameter, Expression.Constant(compareVal));
        }
    }
}
