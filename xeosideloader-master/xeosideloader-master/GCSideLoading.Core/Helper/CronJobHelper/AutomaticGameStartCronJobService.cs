using GCSideLoading.Core.BLL.Redis;
using log4net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GCSideLoading.Core.Helper.CronJobHelper
{
    public class AutomaticGameStartCronJobService: CronJobService
    {
        private readonly ILog _log;
        //private AutomaticGameStartStopManager _automaticGameStartStopManager;

        public AutomaticGameStartCronJobService(IScheduleConfig<AutomaticGameStartCronJobService> config)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _log = LogManager.GetLogger(typeof(AutomaticGameStartCronJobService));
            //_automaticGameStartStopManager = new AutomaticGameStartStopManager();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _log.Info("AutomaticGameStartCronJobService starts.");
                Console.WriteLine("AutomaticGameStartCronJobService starts.");
                return base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _log.Error($"Error in AutomaticGameStartCronJobService StartAsync = {ex.StackTrace}");
            }
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                RedisDataReaderManager redisDataReaderManager = new RedisDataReaderManager();
                //redisDataReaderManager.readRedisDataAndStoreCosmosDB();

                //redisDataReaderManager.DataStatus();
                _log.Info($"{DateTime.Now:hh:mm:ss} AutomaticGameStartCronJobService is working.");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} AutomaticGameStartCronJobService is working.");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _log.Error($"Error in AutomaticGameStartCronJobService DoWork = {ex.StackTrace}");
            }
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _log.Info("AutomaticGameStartCronJobService is stopping.");
                Console.WriteLine("AutomaticGameStartCronJobService is stopping.");
                return base.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _log.Error($"Error in AutomaticGameStartCronJobService StopAsync = {ex.StackTrace}");
            }
            return base.StopAsync(cancellationToken);
        }
    }
}
