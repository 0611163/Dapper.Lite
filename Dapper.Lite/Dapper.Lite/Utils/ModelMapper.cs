using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 相同类型的实体类映射
    /// </summary>
    internal static class ModelMapper<T>
    {
        private static Func<T, T> _func = null;

        private static object _lock = new object();

        /// <summary>
        /// 实体类映射
        /// </summary>
        public static object Map(T source)
        {
            lock (_lock)
            {
                if (_func == null)
                {
                    _func = CreateMapFunc();
                }
            }
            return _func.Invoke(source);
        }

        private static Func<T, T> CreateMapFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "t");

            List<MemberBinding> memberBindings = new List<MemberBinding>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    MemberExpression propertyExpr = Expression.Property(parameterExpression, propertyInfo);
                    MemberBinding memberBinding = Expression.Bind(propertyInfo, propertyExpr);
                    memberBindings.Add(memberBinding);
                }
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(T)), memberBindings);

            Expression<Func<T, T>> lamada = Expression.Lambda<Func<T, T>>(memberInitExpression, parameterExpression);
            return lamada.Compile();
        }

    }
}
