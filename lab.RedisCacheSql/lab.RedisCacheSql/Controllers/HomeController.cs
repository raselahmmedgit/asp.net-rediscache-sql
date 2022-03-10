using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using lab.RedisCacheSql.Helpers;
using lab.RedisCacheSql.Managers;
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

        public HomeController(ILogger<HomeController> logger, IAppBackgroundService iAppBackgroundService)
        {
            _logger = logger;
            _iAppBackgroundService = iAppBackgroundService;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}