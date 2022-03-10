namespace lab.RedisCacheSql.RedisManagers
{
    public interface IRedisDataManager<T>
    {
        T? Get(string key);

        void Save(string key, T obj);

        void Delete(string key); 
    }
}