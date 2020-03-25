using NSharding.DomainModel.Spi;
using NSharding.ORMapping.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.ORMapping.Service
{
    class ORMPluginFactory
    {
        private static Dictionary<string, IORMPlugin> plugins = new Dictionary<string, IORMPlugin>();
        private IORMPlugin defaultPlugin = null;

        private ORMPluginFactory()
        {
            defaultPlugin = new ORMDefaultImpl();
        }

        private static ORMPluginFactory instance;
        private static object syncObj = new object();

        public static ORMPluginFactory GetInstance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new ORMPluginFactory();
                    }
                }
            }

            return instance;
        }

        public IORMPlugin GetOrCreatePlugin(NSharding.DomainModel.Spi.DomainModel domainModel)
        {
            if (domainModel == null)
                throw new ArgumentNullException("ORMPluginFactory.GetOrCreatePlugin.domainModel");
            if (string.IsNullOrEmpty(domainModel.DataLoaderConfig))
                return defaultPlugin;

            Type type = null;
            if (!plugins.ContainsKey(domainModel.ID))
            {
                lock (syncObj)
                {
                    if (!plugins.ContainsKey(domainModel.ID))
                    {
                        type = Type.GetType(domainModel.DataLoaderConfig);
                        if (type == null)
                            throw new TypeAccessException(domainModel.DataLoaderConfig);

                        var pluginObj = type.Assembly.CreateInstance(type.FullName, true);
                        if (pluginObj == null)
                        {
                            throw new Exception("反射创建类型为空:" + type.FullName);
                        }
                        var plugin = pluginObj as IORMPlugin;
                        if (plugin == null)
                        {
                            throw new Exception("反射创建对象未实现IORMPlugin:" + pluginObj.GetType().FullName);
                        }

                        plugins.Add(domainModel.ID, plugin);
                    }
                }
            }

            return plugins[domainModel.ID];
        }
    }
}
