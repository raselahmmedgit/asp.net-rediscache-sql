namespace lab.RedisCacheSql.RedisManagers
{
    public class RedisConfig
    {
        public static string Name = "RedisConfig";
        public string? Host { get; set; }
        public string? Port { get; set; }
        public string? Password { get; set; }
        public string? InstanceName { get; set; }
        public string? ConnectRetry { get; set; }
        public string? SyncTimeoutInMinutes { get; set; }
        public string? RedisStartCommand { get; set; }
        public string? RedisStopCommand { get; set; }
    }
}
