using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using lab.RedisCacheSql.Core;
using lab.RedisCacheSql.Helpers;
using lab.RedisCacheSql.Managers;
using lab.RedisCacheSql.RedisManagers;
using lab.RedisCacheSql.Services;
using lab.RedisCacheSql.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace lab.RedisCacheSql.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppBackgroundService _iAppBackgroundService;
        private readonly IRedisServiceManager _iRedisServiceManager;

        public HomeController(ILogger<HomeController> logger
            , IAppBackgroundService iAppBackgroundService
            , IRedisServiceManager iRedisServiceManager)
        {
            _logger = logger;
            _iAppBackgroundService = iAppBackgroundService;
            _iRedisServiceManager = iRedisServiceManager;
        }

        public IActionResult Index()
        {
            try
            {
                _iAppBackgroundService.RunAutomaticPatientProfileAsync();
                return View();
            }
            catch (Exception ex)
            {
                return ErrorView(ex);
            }
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> RedisServiceStartAjax()
        {
            try
            {
                bool isStart = _iRedisServiceManager.Start();

                if (isStart)
                {
                    _result = Result.Ok(("Redis Service Start " + MessageHelper.Success));
                }
                else
                {
                    _result = Result.Fail(MessageHelper.Fail);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMessageHelper.FormateMessageForException(ex, "RedisServiceStartAjax[GET]"));
                _result = Result.Fail(MessageHelper.UnhandledError);
            }

            return JsonResult(_result);
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> RedisServiceStopAjax()
        {
            try
            {
                bool isStop = _iRedisServiceManager.Stop();

                if (isStop)
                {
                    _result = Result.Ok(("Redis Service Stop " + MessageHelper.Success));
                }
                else
                {
                    _result = Result.Fail(MessageHelper.Fail);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMessageHelper.FormateMessageForException(ex, "RedisServiceStopAjax[GET]"));
                _result = Result.Fail(MessageHelper.UnhandledError);
            }

            return JsonResult(_result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}