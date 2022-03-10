using lab.RedisCacheSql.Config;
using lab.RedisCacheSql.Managers;
using Microsoft.Extensions.Options;

namespace lab.RedisCacheSql.Services
{
    public class AppBackgroundService : IAppBackgroundService
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger<AppBackgroundService> _logger;
        private readonly IPatientProfileManager _iPatientProfileManager;

        public AppBackgroundService(IOptions<AppConfig> appConfig, ILogger<AppBackgroundService> logger, IPatientProfileManager iPatientProfileManager)
        {
            _appConfig = appConfig.Value;
            _logger = logger;
            _iPatientProfileManager = iPatientProfileManager;
        }

        public void RunAutomaticPatientProfileAsync()
        {
            try
            {
                ThreadStart insertOrUpdatePatientProfileObj = new ThreadStart(RunInsertOrUpdatePatientProfileAsync);
                Thread insertOrUpdatePatientProfileThread = new Thread(insertOrUpdatePatientProfileObj);
                insertOrUpdatePatientProfileThread.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError("RunAutomaticPatientProfileAsync : " + ex);
            }
            finally { }

        }

        private void RunInsertOrUpdatePatientProfileAsync()
        {
            try
            {
                bool patientProfileCacheEnable = _appConfig.PatientProfileRedisEnable != null ? bool.Parse(_appConfig.PatientProfileRedisEnable) : false;

                while (patientProfileCacheEnable)
                {
                    int delayMilliseconds = (Convert.ToInt32(_appConfig.DelayInMinutes) * 60 * 1000);
                    Console.WriteLine($"RunInsertOrUpdatePatientProfileAsync: {DateTime.Now.ToString()}");
                    Thread.Sleep(delayMilliseconds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("RunInsertOrUpdatePatientProfileAsync : " + ex);
            }
            finally { }
        }
    }

    public interface IAppBackgroundService
    {
        void RunAutomaticPatientProfileAsync();
    }

}
