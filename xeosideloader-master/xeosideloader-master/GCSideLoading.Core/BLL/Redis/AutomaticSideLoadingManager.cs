using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GCSideLoading.Core.BLL.Redis
{
    public class AutomaticSideLoadingManager
    {
        public RedisDataReaderManager redisDataReaderManager;
        private ILog log;

        public AutomaticSideLoadingManager()
        {
            redisDataReaderManager =  new RedisDataReaderManager();
            log = LogManager.GetLogger(typeof(AutomaticSideLoadingManager));

        }
        public void RunAutomaticRedisDataReader()
        {
            try
            {
                ThreadStart notifyObj = new ThreadStart(NotifyRedisDataReader);
                Thread notifyThread = new Thread(notifyObj);
                notifyThread.Start();
            }
            catch (Exception ex)
            {
                log.Error("Exception in RunAutomaticNotifySurgicalConceirgeStage " + ex);
            }
            finally { }

        }

        public async void NotifyRedisDataReader()
        {
            try
            {
                while (true)
                {
                    await redisDataReaderManager.readRedisDataAndStoreCosmosDB();
                    Thread.Sleep(1 * 60 * 1000);
                }

            }
            catch (Exception ex)
            {
                log.Error("Exception in NotifyToPatientAttendeeOfSurgicalConceirgeStage " + ex);
            }
            finally { }
        }
    }
}
