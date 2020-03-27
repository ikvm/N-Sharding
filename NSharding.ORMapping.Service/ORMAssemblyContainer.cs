using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.ORMapping.Service
{
    /// <summary>
    /// ORM相关的程序集容器
    /// </summary>
    class ORMAssemblyContainer
    {
        private static ConcurrentDictionary<string, Type> assemblyDic;

        private static object syncObj = new object();

        //程序集容器实例
        private static ORMAssemblyContainer instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        private ORMAssemblyContainer()
        {
            assemblyDic = new ConcurrentDictionary<string, Type>();
        }

        /// <summary>
        /// 获取程序集容器实例
        /// </summary>
        /// <returns>程序集容器实例</returns>
        public static ORMAssemblyContainer GetInstance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new ORMAssemblyContainer();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 反射构造指定类型的实例
        /// </summary>        
        /// <param name="typeName">类型全称</param>
        /// <returns>指定类型的实例</returns>
        public object CreateInstance(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException("ORMAssemblyContainer.CreateInstance.typeName");

            Type type = null;
            if (!assemblyDic.ContainsKey(typeName))
            {
                lock (syncObj)
                {
                    if (!assemblyDic.ContainsKey(typeName))
                    {
                        type = Type.GetType(typeName);
                        if (type == null)
                            throw new TypeAccessException(typeName);

                        assemblyDic.TryAdd(typeName, type);
                    }
                    else
                    {
                        type = assemblyDic[typeName];
                    }
                }
            }
            else
            {
                type = assemblyDic[typeName];
            }

            if (type == null)
                throw new TypeAccessException(typeName);

            return type.Assembly.CreateInstance(type.FullName, true);
        }

        /// <summary>
        /// 获取对象类型
        /// </summary>
        /// <param name="typeName">类型全称</param>
        /// <returns>对象类型</returns>
        public Type GetObjectType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException("ORMAssemblyContainer.GetObjectType.typeName");

            Type type = null;
            if (!assemblyDic.ContainsKey(typeName))
            {
                lock (syncObj)
                {
                    if (!assemblyDic.ContainsKey(typeName))
                    {
                        type = Type.GetType(typeName);
                        if (type == null)
                            throw new TypeAccessException(typeName);

                        assemblyDic.TryAdd(typeName, type);
                    }
                    else
                    {
                        type = assemblyDic[typeName];
                    }
                }
            }
            else
            {
                type = assemblyDic[typeName];
            }

            return type;
        }
    }
}
}
