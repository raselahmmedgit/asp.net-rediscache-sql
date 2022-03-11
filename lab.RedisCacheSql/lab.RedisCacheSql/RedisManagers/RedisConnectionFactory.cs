using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace lab.RedisCacheSql.RedisManagers
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly RedisConfig _redisConfig;

        public RedisConnectionFactory(IOptions<RedisConfig> redisConfig)
        {
            _redisConfig = redisConfig.Value;
        }

        public ConnectionMultiplexer Connection()
        {
            try
            {
                string connectionString = _redisConfig.Host + ":" + _redisConfig.Port;

                if (!string.IsNullOrEmpty(_redisConfig.Password))
                {
                    connectionString += ",password=" + _redisConfig.Password + ",ssl=True,abortConnect=False";
                }

                int syncTimeoutSeconds = (Convert.ToInt32(_redisConfig.SyncTimeoutInMinutes) * 60);

                var options = ConfigurationOptions.Parse(connectionString);
                options.ConnectRetry = Convert.ToInt32(_redisConfig.ConnectRetry);
                options.AllowAdmin = true;
                options.SyncTimeout = syncTimeoutSeconds;
                var connect = ConnectionMultiplexer.Connect(options);
                return connect;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IServer Server()
        {
            IServer server = this.Connection().GetServer(_redisConfig.Host, Convert.ToInt32(_redisConfig.Port));
            return server;
        }

        public IDatabase Database()
        {
            IDatabase database = this.Connection().GetDatabase();
            return database;
        }

        public bool IsConnected()
        {
            try
            {
                return this.Connection().IsConnected;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void DeleteAllDatabase()
        {
            this.Server().FlushAllDatabases();
        }
    }

    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
        IServer Server();
        IDatabase Database();
        bool IsConnected();
        void DeleteAllDatabase();
    }
}