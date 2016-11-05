using System;
using System.Linq.Expressions;

namespace BeyondQueries.Domain
{
    public class PropertySpecifier<T>
    {
        private string _propertyName;

        public PropertySpecifier(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression)
            {
                var me = expression.Body as MemberExpression;
                _propertyName = me.Member.Name;
            }
            else if (expression.Body is UnaryExpression)
            {
                var ue = expression.Body as UnaryExpression;
                var me = ue.Operand as MemberExpression;
                _propertyName = me.Member.Name;
            }
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }
    }
}