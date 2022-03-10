namespace lab.RedisCacheSql.RedisManagers
{
    public class RedisBaseModel
    {
        public bool HasData { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
}
