using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCSideLoading.Core.BLL.Redis
{
    public class RedisConnectionManager
    {
        private static string host = "xcite.redis.cache.windows.net";
        private static int port = 6380;
        private static string password = "cnwZlYoyfFfBC2LoOGHLwQYy19HRMJpXV27YDW+0bts=";
        private static string connectionString = host+":"+port+",password="+password+",ssl=True,abortConnect=False";
        public RedisConnectionManager()
        {

        }
        public static Lazy<ConnectionMultiplexer> DataConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            try
            {
                var options = ConfigurationOptions.Parse(connectionString);
                options.ConnectRetry = 5;
                options.AllowAdmin = true;
                options.SyncTimeout = 60000;
                var connect = ConnectionMultiplexer.Connect(options);
                return connect;
            }
            catch (Exception)
            {
                throw;
            }
        });
        public static IServer GetServer()
        {
            IServer server = DataConnection.Value.GetServer(host, port);
            return server;
        }
        public static IDatabase GetDatabase()
        {
            return DataConnection.Value.GetDatabase();
        }
    }
}
