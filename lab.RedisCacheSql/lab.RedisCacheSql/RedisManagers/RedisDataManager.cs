using lab.RedisCacheSql.Helpers;
using StackExchange.Redis;

namespace lab.RedisCacheSql.RedisManagers
{
    public class RedisDataManager<T> : RedisBaseManager<T>, IRedisDataManager<T>
    {
        private readonly IServer _iServer;
        private readonly IDatabase _iDatabase;
        private readonly IRedisConnectionFactory _iRedisConnectionFactory;

        public RedisDataManager(IRedisConnectionFactory iRedisConnectionFactory)
        {
            this._iRedisConnectionFactory = iRedisConnectionFactory;
            this._iServer = this._iRedisConnectionFactory.Server();
            this._iDatabase = this._iRedisConnectionFactory.Database();
        }

        public void Delete(string key)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(key) || key.Contains("_")) throw new ArgumentException(MessageHelper.RedisInvalidKey);
                if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException(MessageHelper.RedisInvalidKey);

                //key = this.GenerateKey(key);
                this._iDatabase.KeyDelete(key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T? Get(string key)
        {
            try
            {
                //key = this.GenerateKey(key);
                var hash = this._iDatabase.HashGetAll(key);
                if (hash != null)
                {
                    return this.MapFromHash(hash);
                }
                else
                {
                    return default;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Save(string key, T obj)
        {
            try
            {
                if (obj != null)
                {
                    var hash = this.GenerateHash(obj);
                    //key = this.GenerateKey(key);

                    if (this._iDatabase.HashLength(key) == 0)
                    {
                        this._iDatabase.HashSet(key, hash);
                    }
                    else
                    {
                        var props = this.Properties;
                        foreach (var item in props)
                        {
                            if (this._iDatabase.HashExists(key, item.Name))
                            {
                                this._iDatabase.HashIncrement(key, item.Name, Convert.ToInt32(item.GetValue(obj)));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
   }

    public interface IRedisDataManager<T>
    {
        T? Get(string key);

        void Save(string key, T obj);

        void Delete(string key);
    }
}