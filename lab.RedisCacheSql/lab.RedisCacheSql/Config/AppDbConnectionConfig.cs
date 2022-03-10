using System;

namespace lab.RedisCacheSql.Config
{
    public class AppDbConnectionConfig
    {
        public static string Name = "AppDbConnectionConfig";
        public string DefaultConnection { get; set; }
        public string DatabaseName { get; set; }
        public bool IsDatabaseCreate { get; set; }
        public bool IsMasterDataInsert { get; set; }
    }
}
