using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Utils
{
    /// <summary>
    /// 服务帮助类
    /// </summary>
    public partial class ServiceHelper
    {
        #region 变量
        /// <summary>
        /// 接口的对象集合
        /// </summary>
        private static ConcurrentDictionary<Type, object> _dict = new ConcurrentDictionary<Type, object>();

        private static object _lock = new object();
        #endregion

        #region Get 获取实例
        /// <summary>
        /// 获取实例
        /// </summary>
        public static T Get<T>()
        {
            Type type = typeof(T);

            lock (_lock)
            {
                if (_dict.TryGetValue(type, out object obj))
                {
                    return (T)obj;
                }
                else
                {
                    var instance = Activator.CreateInstance(type);
                    _dict.TryAdd(type, instance);
                    return (T)instance;
                }
            }
        }
        #endregion

        #region Get 通过Func获取实例
        /// <summary>
        /// 获取实例
        /// </summary>
        public static T Get<T>(Func<T> func)
        {
            Type type = typeof(T);

            lock (_lock)
            {
                if (_dict.TryGetValue(type, out object obj))
                {
                    return (T)obj;
                }
                else
                {
                    var instance = func();
                    _dict.TryAdd(type, instance);
                    return instance;
                }
            }
        }
        #endregion

    }
}
