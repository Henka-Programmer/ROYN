using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ROYN.Extensions
{
    internal static class ExpressionExtension
    {
        internal static PropertyInfo GetPropertyInfo<T, TValue>(this Expression<Func<T, TValue>> expression)
        {
            MemberExpression Exp = null;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            if (expression.Body is UnaryExpression)
            {
                var UnExp = (UnaryExpression)expression.Body;
                if (UnExp.Operand is MemberExpression)
                {
                    Exp = (MemberExpression)UnExp.Operand;
                }
                else
                    throw new ArgumentException();
            }
            else if (expression.Body is MemberExpression)
            {
                Exp = (MemberExpression)expression.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)Exp.Member;
        }

        internal static string GetPropertyPath<T, TValue>(this Expression<Func<T, TValue>> expr)
        {
            var pathSegmantes = new List<string>();
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expr.Body as UnaryExpression;
                    me = ue?.Operand as MemberExpression;
                    break;

                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            while (me != null)
            {
                string propertyName = me.Member.Name;
                pathSegmantes.Add(propertyName);
                me = me.Expression as MemberExpression;
            }
            pathSegmantes.Reverse();
            return string.Join(".", pathSegmantes);
        }
    }
}