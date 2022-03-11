using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Diagnostics;

namespace lab.RedisCacheSql.RedisManagers
{
    public class RedisServiceManager : IRedisServiceManager
    {
        private readonly RedisConfig _redisConfig;

        public RedisServiceManager(IOptions<RedisConfig> redisConfig)
        {
            _redisConfig = redisConfig.Value;
        }

        public bool Start()
        {
            try
            {
                string? startcommand = _redisConfig.RedisStartCommand;

                ProcessStartInfo pi = new ProcessStartInfo();
                pi.FileName = "cmd.exe";
                pi.Arguments = startcommand;
                pi.UseShellExecute = false;
                pi.CreateNoWindow = true;
                pi.Verb = "runas";

                Process p = new Process();
                p.StartInfo = pi;
                p.Start();

                return true;
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        public bool Stop()
        {
            try
            {
                string? stopcommand = _redisConfig.RedisStopCommand;

                ProcessStartInfo pi = new ProcessStartInfo();
                pi.FileName = "cmd.exe";
                pi.Arguments = stopcommand;
                pi.UseShellExecute = false;
                pi.CreateNoWindow = true;
                pi.Verb = "runas";

                Process p = new Process();
                p.StartInfo = pi;
                p.Start();

                return true;
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }
    }

    public interface IRedisServiceManager
    {
        bool Start();
        bool Stop();
    }
}