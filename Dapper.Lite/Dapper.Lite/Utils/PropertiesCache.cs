using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Lite
{
    /// <summary>
    /// 缓存PropertyInfo集合
    /// </summary>
    public static class PropertiesCache
    {
        public static ConcurrentDictionary<Type, object> _dict = new ConcurrentDictionary<Type, object>();

        private static object _lock = new object();

        /// <summary>
        /// 获取对象
        /// </summary>
        public static T TryGet<T>(Type type, Func<Type, T> func)
        {
            lock (_lock)
            {
                if (_dict.TryGetValue(type, out object obj))
                {
                    return (T)obj;
                }
                else
                {
                    var instance = func(type);
                    _dict.TryAdd(type, instance);
                    return instance;
                }
            }
        }

    }
}
