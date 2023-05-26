using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dapper.Lite
{
    /// <summary>
    /// 数据库实现工厂
    /// </summary>
    public class ProviderFactory
    {
        #region 变量属性
        private static ConcurrentDictionary<DBType, IProvider> _providers = new ConcurrentDictionary<DBType, IProvider>();
        private static ConcurrentDictionary<Type, IProvider> _providersByType = new ConcurrentDictionary<Type, IProvider>();
        #endregion

        #region 创建数据库 Provider
        /// <summary>
        /// 创建数据库 Provider
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        internal static IProvider CreateProvider(DBType dbType)
        {
            IProvider provider;
            _providers.TryGetValue(dbType, out provider);
            return provider;
        }

        /// <summary>
        /// 创建数据库 Provider
        /// </summary>
        /// <param name="providerType">数据库提供者类型</param>
        internal static IProvider CreateProvider(Type providerType)
        {
            IProvider provider;
            _providersByType.TryGetValue(providerType, out provider);
            return provider;
        }
        #endregion

        #region 注册Provider
        /// <summary>
        /// 注册数据库Provider(注册的数据库Provider需要继承相应的数据库提供者基类和IDBProvider,并重写IDBProvider中的接口实现)
        /// </summary>
        internal static void RegisterDBProvider(DBType dbType, IProvider provider)
        {
            _providers.TryAdd(dbType, provider);
        }
        #endregion

        #region 注册Provider
        /// <summary>
        /// 注册数据库Provider(注册的数据库Provider必须实现IProvider接口)
        /// </summary>
        internal static void RegisterDBProvider(Type providerType, IProvider provider)
        {
            _providersByType.TryAdd(providerType, provider);
        }
        #endregion

        #region 初始化 数据库字段名与实体类属性名映射
        /// <summary>
        /// 初始化 数据库字段名与实体类属性名映射
        /// </summary>
        public static void SetTypeMap(Assembly assembly)
        {
            DateTime dt = DateTime.Now;

            Type[] types = assembly.GetTypes();
            var modelTypeList = types.Where(x => x.GetCustomAttributes<TableAttribute>().Any());
            foreach (var type in modelTypeList)
            {
                var map = new CustomPropertyTypeMap(type, (modelType, columnName) =>
                {
                    PropertyInfoEx[] props = DbSession.GetEntityProperties(modelType);

                    PropertyInfoEx propEx = props.FirstOrDefault(prop =>
                    {
                        if (prop.FieldNameUpper == columnName.ToUpper())
                        {
                            return true;
                        }

                        return false;
                    });

                    if (propEx != null)
                    {
                        return propEx.PropertyInfo;
                    }
                    else
                    {
                        return null;
                    }
                });

                if (map != null)
                {
                    SqlMapper.SetTypeMap(type, map);
                }
            }

            double d = DateTime.Now.Subtract(dt).TotalSeconds;
            Console.WriteLine("数据库字段名与实体类属性名映射，耗时：" + d.ToString("0.000") + " 秒");
        }
        #endregion

    }
}
