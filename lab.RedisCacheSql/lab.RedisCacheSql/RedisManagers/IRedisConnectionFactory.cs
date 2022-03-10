using StackExchange.Redis;

namespace lab.RedisCacheSql.RedisManagers
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
        IServer Server();
        IDatabase Database();
        void DeleteAllDatabase();
    }
}