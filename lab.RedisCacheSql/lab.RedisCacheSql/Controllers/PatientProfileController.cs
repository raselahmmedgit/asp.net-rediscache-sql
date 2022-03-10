using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using lab.RedisCacheSql.Core;
using lab.RedisCacheSql.Helpers;
using lab.RedisCacheSql.Managers;
using lab.RedisCacheSql.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace lab.RedisCacheSql.Controllers
{
    public class PatientProfileController : BaseController
    {
        #region Global Variable Declaration
        private readonly ILogger<PatientProfileController> _logger;
        private readonly IPatientProfileManager _iPatientProfileManager;

        #endregion

        #region Constructor
        public PatientProfileController(IPatientProfileManager iPatientProfileManager)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<PatientProfileController>();
            _iPatientProfileManager = iPatientProfileManager;
        }
        #endregion

        #region Actions

        // GET: PatientProfile
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return ErrorView(ex);
            }
        }

        [HttpGet]
        [Route("/PatientProfile/GetDataTableAjax")]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> GetDataTableAjax(IDataTablesRequest request)
        {
            try
            {
                DataTablesResponse response = await _iPatientProfileManager.GetDataTablesResponseAsync(request);
                return new DataTablesJsonResult(response, true);
            }
            catch (Exception ex)
            {
                return ErrorView(ex);
            }
        }


        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> AddAjax()
        {
            try
            {
                var viewModel = new PatientProfileViewModel();
                if (viewModel != null)
                {
                    return PartialView("~/Views/PatientProfile/_AddOrEdit.cshtml", viewModel);
                }
                else
                {
                    return ErrorPartialView(ExceptionHelper.ExceptionErrorMessageForNullObject());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMessageHelper.FormateMessageForException(ex, "AddAjax[GET]"));
                return ErrorPartialView(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditAjax(long id)
        {
            try
            {
                if (id > 0)
                {
                    var viewModel = await _iPatientProfileManager.GetPatientProfileAsync(id);
                    if (viewModel != null)
                    {
                        return PartialView("~/Views/PatientProfile/_AddOrEdit.cshtml", viewModel);
                    }
                    else
                    {
                        return ErrorPartialView(ExceptionHelper.ExceptionErrorMessageForNullObject());
                    }
                }
                else
                {
                    return ErrorPartialView(ExceptionHelper.ExceptionErrorMessageForNullObject());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMessageHelper.FormateMessageForException(ex, "EditAjax[GET]"));
                return ErrorPartialView(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAjax(long id)
        {
            try
            {
                if (id > 0)
                {
                    var viewModel = await _iPatientProfileManager.GetPatientProfileAsync(id);
                    if (viewModel != null)
                    {
                        return PartialView("~/Views/PatientProfile/_Details.cshtml", viewModel);
                    }
                    else
                    {
                        return ErrorPartialView(ExceptionHelper.ExceptionErrorMessageForNullObject());
                    }
                }
                else
                {
                    return ErrorPartialView(ExceptionHelper.ExceptionErrorMessageForNullObject());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMessageHelper.FormateMessageForException(ex, "DetailsAjax[GET]"));
                return ErrorPartialView(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAjax(PatientProfileViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.PatientProfileId == 0) //add
                    {
                        _result = await _iPatientProfileManager.InsertPatientProfileAsync(viewModel);
                    }
                    else if (viewModel.PatientProfileId > 0) //edit
                    {
                        _result = await _iPatientProfileManager.UpdatePatientProfileAsync(viewModel);
                    }
                }
                else
                {
                    _result = Result.Fail(ExceptionHelper.ModelStateErrorFirstFormat(ModelState));
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMessageHelper.FormateMessageForException(ex, "SaveAjax[POST]"));
                _result = Result.Fail(MessageHelper.UnhandledError);
            }

            return JsonResult(_result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(long id)
        {
            try
            {
                if (id > 0)
                {
                    _result = await _iPatientProfileManager.DeletePatientProfileAsync(id);
                }
                else
                {
                    _result = Result.Fail(MessageHelper.DeleteFail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMessageHelper.FormateMessageForException(ex, "DeleteAjax[POST]"));
                _result = Result.Fail(MessageHelper.UnhandledError);
            }

            return JsonResult(_result);
        }


        #endregion
    }
}
