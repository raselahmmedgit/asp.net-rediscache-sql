using System;

namespace lab.RedisCacheSql.Config
{
    public class AppConfig
    {
        public static string Name = "AppConfig";
        public string PatientProfileRedisKey { get; set; }
        public string PatientProfileRedisEnable { get; set; }
        public string DelayInMinutes { get; set; }
    }
}
