using GCSideLoading.Core;
using GCSideLoading.Web.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace GCSideLoading.Web.Controllers
{
    public class BaseController : Controller
    {
        #region Global Variable Declaration
        private readonly ILogger<BaseController> _logger;
        internal Result _result = new Result();

        #endregion

        #region Constructor
        public BaseController()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<BaseController>();
            _logger.LogInformation("BaseController instance created...");
        }
        #endregion

        #region Actions

        internal IActionResult ErrorView(Exception ex)
        {
            _logger.LogError(ex, "ErrorView");
            var errorPageViewModel = new ErrorPageViewModel();
            errorPageViewModel = ExceptionHelper.ExceptionErrorMessageFormat(ex);
            _logger.LogError(errorPageViewModel.ErrorMessage, "Error");
            return View("Error", errorPageViewModel);
        }

        internal IActionResult ErrorPartialView(Exception ex)
        {
            _logger.LogError(ex, "ErrorPartialView");
            var errorPageViewModel = new ErrorPageViewModel();
            errorPageViewModel = ExceptionHelper.ExceptionErrorMessageFormat(ex);
            _logger.LogError(errorPageViewModel.ErrorMessage, "Error");
            return PartialView("_ErrorModal", errorPageViewModel);
        }

        internal IActionResult ErrorView(ErrorPageViewModel errorPageViewModel)
        {
            _logger.LogError(errorPageViewModel.ErrorMessage, "Error");
            return View("Error", errorPageViewModel);
        }

        internal IActionResult ErrorPartialView(ErrorPageViewModel errorPageViewModel)
        {
            _logger.LogError(errorPageViewModel.ErrorMessage, "Error");
            return PartialView("_ErrorModal", errorPageViewModel);
        }

        internal IActionResult JsonResult(Exception ex)
        {
            _logger.LogError(ex, "JsonResult");
            return ModalHelper.JsonError(ex);
        }

        internal IActionResult JsonResult(Result result)
        {
            _logger.LogError(result.Error, "JsonResult");
            return ModalHelper.Json(result);
        }

        internal IActionResult JsonResult(ModelStateDictionary modelStateDictionary)
        {
            return ModalHelper.JsonModelError(modelStateDictionary);
        }

        internal IActionResult RedirectResult(string actionName)
        {
            return RedirectToAction(actionName);
        }

        internal IActionResult RedirectResult(string actionName, string controllerName)
        {
            return RedirectToAction(actionName, controllerName);
        }

        internal IActionResult RedirectResult(string actionName, string controllerName, string areaName)
        {
            return RedirectToAction(actionName, controllerName, new { @area = areaName });
        }

        internal bool IsAjaxRequest()
        {
            var request = Request;
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            return (request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }

        internal IActionResult RenderAlertResult(Result result)
        {
            _logger.LogError(result.Error, "RenderAlertResult");
            return PartialView("_ErrorRenderModal", result);
        }

        internal IActionResult RenderAlertResult(Exception ex)
        {
            _logger.LogError(ex, "RenderAlertResult");
            var result = ModalHelper.RenderAlertResult(ex);
            return PartialView("_ErrorRenderModal", result);
        }

        internal string GetIPAddress()
        {
            ConnectionInfo cnnection = HttpContext.Connection;
            string ip = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            if (!string.IsNullOrEmpty(ip))
            {
                if (ip == "::1")
                {
                    ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                }
                return ip;
            }
            ip = Response.HttpContext.Connection.LocalIpAddress.ToString();
            if (!string.IsNullOrEmpty(ip))
            {
                if (ip == "::1")
                {
                    ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                }
                return ip;
            }
            return ip;
        }

        internal string GetIPAddress2()
        {
            string ip;
            try
            {
                ip = Request.Headers["X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                {
                    ip = GetIPAddress();
                }
            }
            catch
            {
                ip = null;
            }
            return ip;
        }
        public string GetGeneratedGameUrl(string gameUrlSlug)
        {
            string url = Url.Action("Index", "Play", new { gameUrlSlug }, protocol: HttpContext.Request.Scheme);
            return url;
        }
        public string GetGeneratedGameUrl(string gameUrlSlug, string id)
        {
            string url = Url.Action("Index", "Play", new { gameUrlSlug, id }, protocol: HttpContext.Request.Scheme);
            return url;
        }
        public string GetGeneratedRewardUrl(string gameId,string playerId)
        {
            string url = Url.Action("PlayerGameReward", "Play", new { gameId,playerId }, protocol: HttpContext.Request.Scheme);
            return url;
        }

        #endregion
    }
}