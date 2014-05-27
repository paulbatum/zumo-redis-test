using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using dotnetdb3Service.DataObjects;
using Autofac;
using ServiceStack.Redis;
using ServiceStack.Logging;
using System.Diagnostics;

namespace dotnetdb3Service
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            LogManager.LogFactory = new ServiceStack.Logging2.DebugLogFactory(debugEnabled: true);

            string redisKey = "";
            var redis = new PooledRedisClientManager(redisKey + "@pbtest.redis.cache.windows.net:6379");                                         
            
            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options, (httpconfig, autofac) =>
            {
                autofac.Register<IRedisClient>(componentContext => redis.GetClient())
                    .InstancePerDependency();
            }));            

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
        }
    }
}

