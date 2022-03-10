using AspNetCore.Identity.DocumentDb;
using GCSideLoading.Core;
using GCSideLoading.Core.DBHelper;
using GCSideLoading.Core.EntityModel;
using GCSideLoading.Core.Helper;
using GCSideLoading.Core.Util;
using GCSideLoading.Services;
using GCSideLoading.Web.Models.AccountViewModels;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace GCSideLoading.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        #region Global Variable Declaration
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<DocumentDbIdentityRole> _roleManager;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILog _log;
        
        #endregion

        #region Constructor
        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<DocumentDbIdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _log = LogManager.GetLogger(typeof(AccountController));            
        }
        #endregion

        #region Actions
        
        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("Login[POST]", User.GetLoggedInUserId(), $"UserEmail: {model.Email}"));
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    

                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        //var data = await GetUserSignInOutHistoryLocal(model);
                        //await _userLogInOutHistoryManager.CreateLogInOutHistoryAsync(data);
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("Login[POST]", User.GetLoggedInUserId(), $"User logged in, UserEmail: {model.Email}"));
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        return RedirectToLocal(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("Login[POST]", User.GetLoggedInUserId(), $"User account locked out, UserEmail: {model.Email}"));
                        return View("Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("Login[POST]", User.GetLoggedInUserId(), $"Invalid login attempt, UserEmail: {model.Email}"));
                        return View(model);
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "Login[POST]", User.GetLoggedInUserId()));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAjax(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("LoginAjax[POST]", User.GetLoggedInUserId(), $"UserEmail: {model.Email}"));
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {


                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        //var data = await GetUserSignInOutHistoryLocal(model);
                        //await _userLogInOutHistoryManager.CreateLogInOutHistoryAsync(data);
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("LoginAjax[POST]", User.GetLoggedInUserId(), $"User logged in, UserEmail: {model.Email}"));
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        return Json(new { Message = "Login Success", IsSuccess = true });
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("LoginAjax[POST]", User.GetLoggedInUserId(), $"User account locked out, UserEmail: {model.Email}"));
                        return View("Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("LoginAjax[POST]", User.GetLoggedInUserId(), $"Invalid login attempt, UserEmail: {model.Email}"));
                        return Json(new { Message = "Invalid login attempt.", IsSuccess = false });
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "LoginAjax[POST]", User.GetLoggedInUserId()));
            }

            // If we got this far, something failed, redisplay form
            return Json(new { Message = "Login Failed.", IsSuccess = false });
        }

        //
        // GET: /Account/PlayerRegister
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> PlayerRegister(string returnUrl = null, string publishGameId = null)
        //{
        //    try
        //    {
        //        ViewData["ReturnUrl"] = returnUrl;
        //        PlayerRegisterViewModel model = new PlayerRegisterViewModel();
        //        model = await _playGameManager.PlayerRegisterAsync(publishGameId, User, Request);
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "PlayerRegister[GET]", User.GetLoggedInUserId()));
        //        return ErrorView(ex);
        //    }
        //}

        //
        // POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> PlayerRegister(PlayerRegisterViewModel model, string returnUrl = null)
        //{
        //    try
        //    {
        //        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("PlayerRegister[POST]", User.GetLoggedInUserId()));
        //        ViewData["ReturnUrl"] = returnUrl;

        //        if (ModelState.IsValid)
        //        {
        //            var playerProfileViewModel = await GenaretePlayerProfileViewModelAsync(model);

        //            if (playerProfileViewModel != null)
        //            {
        //                playerProfileViewModel.IsGoogleAuthorize = false;
        //                playerProfileViewModel.IsFacebookAuthorize = false;

        //                if (string.IsNullOrEmpty(model.Email))
        //                {
        //                    ModelState.AddModelError(string.Empty, "Email is required.");
        //                    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"Email is required, UserEmail: {model.Email}"));
        //                    return View(model);
        //                }



        //                if (model.Password != null && model.Password.Length > 0)
        //                {
        //                    playerProfileViewModel.Password = model.Password;
        //                    var game = await _gameManager.GetItemAsync(model.GameId);
        //                    playerProfileViewModel.ClientProfileId = game.ClientProfileId;
        //                    Result result = await _playerProfileManager.CreateAsync(playerProfileViewModel, null, _userManager, null);
        //                    if (result.Success)
        //                    {
        //                        var user = await _userManager.FindByIdAsync(result.Id);
        //                        await _signInManager.SignInAsync(user, isPersistent: false);
        //                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"User created a new account with password, UserEmail:{model.Email}"));
        //                        return RedirectToPlayer(returnUrl);
        //                    }
        //                    else
        //                    {
        //                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"User creation failed, UserEmail:{model.Email}"));
        //                        ModelState.AddModelError(string.Empty, result.Error);
        //                    }
        //                }
        //                else
        //                {
        //                    var resultPlayerProfile = await _playGameManager.CreatePlayerProfileAsync(playerProfileViewModel, model.GameId);

        //                    if (resultPlayerProfile.Success)
        //                    {
        //                        SessionCookiesHelper.SetCookiees(Response, "visPlayerProfileId", resultPlayerProfile.Error, 24 * 60);
        //                        SessionCookiesHelper.SetCookiees(Response, "visPublishGameId", model.PublishGameId, 24 * 60);
        //                        return RedirectToPlayer(returnUrl);
        //                    }
        //                    else
        //                    {
        //                        ModelState.AddModelError(string.Empty, resultPlayerProfile.Error);
        //                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"{resultPlayerProfile.Error}, UserEmail: {model.Email}"));
        //                        return View(model);
        //                    }

        //                }
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, MessageHelper.Error);
        //        _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "PlayerRegister[POST]", User.GetLoggedInUserId()));
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> PlayerRegisterAjax(PlayerRegisterViewModel model, string returnUrl = null)
        //{
        //    try
        //    {
        //        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("PlayerRegister[POST]", User.GetLoggedInUserId()));
        //        ViewData["ReturnUrl"] = returnUrl;

        //        if (ModelState.IsValid)
        //        {
        //            var playerProfileViewModel = await GenaretePlayerProfileViewModelAsync(model);

        //            if (playerProfileViewModel != null)
        //            {
        //                playerProfileViewModel.IsGoogleAuthorize = false;
        //                playerProfileViewModel.IsFacebookAuthorize = false;

        //                if (string.IsNullOrEmpty(model.Email))
        //                {
        //                    ModelState.AddModelError(string.Empty, "Email is required.");
        //                    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"Email is required, UserEmail: {model.Email}"));
        //                    return Json(new { Message = "Email is required.", IsSuccess = false });
        //                }

        //                if (model.Password != null && model.Password.Length > 0)
        //                {
        //                    playerProfileViewModel.Password = model.Password;
        //                    var game = await _gameManager.GetItemAsync(model.GameId);
        //                    playerProfileViewModel.ClientProfileId = game.ClientProfileId;
        //                    Result result = await _playerProfileManager.CreateAsync(playerProfileViewModel, null, _userManager, null);
        //                    if (result.Success)
        //                    {
        //                        var user = await _userManager.FindByIdAsync(result.Id);
        //                        await _signInManager.SignInAsync(user, isPersistent: false);
        //                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"User created a new account with password, UserEmail:{model.Email}"));
        //                        return Json(new { Message = "Registration successfull", IsSuccess = true });
        //                    }
        //                    else
        //                    {
        //                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"User creation failed, UserEmail:{model.Email}"));
        //                        ModelState.AddModelError(string.Empty, result.Error);
        //                        return Json(new { Message = result.Error, IsSuccess = false });

        //                    }
        //                }
        //                else
        //                {
        //                    var resultPlayerProfile = await _playGameManager.CreatePlayerProfileAsync(playerProfileViewModel, model.GameId);

        //                    if (resultPlayerProfile.Success)
        //                    {
        //                        SessionCookiesHelper.SetCookiees(Response, "visPlayerProfileId", resultPlayerProfile.Error, 24 * 60);
        //                        SessionCookiesHelper.SetCookiees(Response, "visPublishGameId", model.PublishGameId, 24 * 60);
        //                        return Json(new { Message = "Registration successfull", IsSuccess = true });
        //                    }
        //                    else
        //                    {
        //                        ModelState.AddModelError(string.Empty, resultPlayerProfile.Error);
        //                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("PlayerRegister[POST]", User.GetLoggedInUserId(), $"{resultPlayerProfile.Error}, UserEmail: {model.Email}"));
        //                        return Json(new { Message = resultPlayerProfile.Error, IsSuccess = false });
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
        //                if(allErrors != null && allErrors.Any())
        //                {
        //                    return Json(new { Message = allErrors.FirstOrDefault().ErrorMessage, IsSuccess = false });
        //                }    
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, MessageHelper.Error);
        //        _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "PlayerRegister[POST]", User.GetLoggedInUserId()));
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return Json(new { Message = "Registration Failed", IsSuccess = false });
        //}

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            RegisterViewModel model = new RegisterViewModel();
            model.RoleName = AppConstants.AppUserRole.MasterAdmin.ToString();
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("Register[POST]", User.GetLoggedInUserId()));
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    string userDisplayName = ((model.Email).Split('@')[0]).Trim(); // you are get here username.

                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        DisplayName = userDisplayName,
                        Roles = AspNetIdentityRolesConstants.MasterAdmin,
                        IsSystemAdmin = true,
                        //isPlayer = false
                    };

                    _result = await IsEmailExists(user);
                    if (!_result.Success)
                    {
                        ModelState.AddModelError(string.Empty, _result.Error);
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("Register[POST]", User.GetLoggedInUserId(), _result.Error));
                        return View(model);
                    }
                    
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {                       
                        //await _clientProfileManager.CreateMasterAdminClientAsync(user.Id);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("Register[POST]", User.GetLoggedInUserId(), $"User created a new account with password, UserEmail:{model.Email}"));

                        return RedirectToLocal(returnUrl);
                    }
                    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("Register[POST]", User.GetLoggedInUserId(), $"User creation failed, UserEmail:{model.Email}"));
                    AddErrors(result);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "Register[POST]", User.GetLoggedInUserId()));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task<Result> IsEmailExists(ApplicationUser user)
        {
            try
            {
                var isExists = await _userManager.FindByEmailAsync(user.Email);

                if (isExists != null)
                {
                    string isEmailExistsMessage = string.Format(MessageHelper.IsEmailExists, user.Email);
                    return Result.Fail(isEmailExistsMessage);
                }
                else
                {
                    return Result.Ok();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "IsEmailExists", User.GetLoggedInUserId()));
                return Result.Fail(MessageHelper.Error);
            }
        }

        //
        //// POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LogOff()
        //{
        //    await _signInManager.SignOutAsync();
        //    _logger.LogInformation(4, "User logged out.");
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}

        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("LogOff", User.GetLoggedInUserId(), $"User:{User.Identity.Name}"));
                await _signInManager.SignOutAsync();
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("LogOff", User.GetLoggedInUserId(), $"User logged out"));
            }
            catch (Exception ex)
            {
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "LogOff", User.GetLoggedInUserId()));
            }
            return RedirectToAction("Login", "Account");
        }
        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string gameId = null, string returnUrl = null)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("ExternalLogin[POST]", User.GetLoggedInUserId(), $"Provider: {provider}"));

                // Request a redirect to the external login provider.
                var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { gameId = gameId, returnUrl = returnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "ExternalLogin[POST]", User.GetLoggedInUserId()));
                return ErrorView(ex);
            }
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string gameId = null, string returnUrl = null, string remoteError = null)
        {
            try
            {
                if (remoteError != null)
                {
                    ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                    return View(nameof(Login));
                }
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                // Sign in the user with this external login provider if the user already has a login.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                if (result.Succeeded)
                {
                    // Update any authentication tokens if login succeeded
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalLoginCallback", User.GetLoggedInUserId(), $"User logged in with {info.LoginProvider} provider,User:{User.Identity.Name}"));
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    // If the user does not have an account, then ask the user to create an account.
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData["LoginProvider"] = info.LoginProvider;
                    var roleName = AppConstants.AppUserRole.Admin.ToString();

                    var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var mobilePhone = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);

                    var model = new ExternalPlayerLoginConfirmationViewModel();
                    model.GameId = gameId;
                    model.RoleName = roleName;
                    model.UserName = name;
                    model.Email = email;
                    model.IsEmail = string.IsNullOrEmpty(email) ? false : true;
                    model.MobilePhone = mobilePhone;
                    model.IsMobilePhone = string.IsNullOrEmpty(mobilePhone) ? false : true;

                    return View("ExternalPlayerLoginConfirmation", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "ExternalLoginCallback[POST]", User.GetLoggedInUserId()));
                return ErrorView(ex);
            }

        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalPlayerLoginConfirmation(ExternalPlayerLoginConfirmationViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get the information about the user from the external login provider
                    var info = await _signInManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    ViewData["LoginProvider"] = info.LoginProvider;
                    if (string.IsNullOrEmpty(model.GameId))
                    {
                        ModelState.AddModelError(string.Empty, "Game not found.");
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalPlayerLoginConfirmation[POST]", User.GetLoggedInUserId(), $"Email is required, UserEmail: {model.Email}"));
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.UserName))
                    {
                        ModelState.AddModelError(string.Empty, "User Name is required.");
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalPlayerLoginConfirmation[POST]", User.GetLoggedInUserId(), $"User Name is required, UserName: {model.UserName}"));
                        return View(model);
                    }

                    if (model.IsEmail)
                    {
                        if (string.IsNullOrEmpty(model.Email))
                        {
                            ModelState.AddModelError(string.Empty, "Email is required.");
                            _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalPlayerLoginConfirmation[POST]", User.GetLoggedInUserId(), $"Email is required, UserEmail: {model.Email}"));
                            return View(model);
                        }
                    }

                    if (model.IsMobilePhone)
                    {
                        if (string.IsNullOrEmpty(model.MobilePhone))
                        {
                            ModelState.AddModelError(string.Empty, "Mobile no is required.");
                            _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalPlayerLoginConfirmation[POST]", User.GetLoggedInUserId(), $"Mobile no is required, UserMobilePhone: {model.MobilePhone}"));
                            return View(model);
                        }
                    }

                    //PlayerProfileViewModel playerProfileViewModel = new PlayerProfileViewModel();
                    //playerProfileViewModel.PlayerName = model.UserName;
                    //playerProfileViewModel.EmailAddress = model.Email;
                    //playerProfileViewModel.PhoneNo = model.MobilePhone;

                    //if (info.LoginProvider == Enums.LoginProviderEnum.Google.ToDescriptionAttr())
                    //{
                    //    playerProfileViewModel.IsGoogleAuthorize = true;
                    //    playerProfileViewModel.IsFacebookAuthorize = false;
                    //}
                    //else if (info.LoginProvider == Enums.LoginProviderEnum.Facebook.ToDescriptionAttr())
                    //{
                    //    playerProfileViewModel.IsGoogleAuthorize = false;
                    //    playerProfileViewModel.IsFacebookAuthorize = true;
                    //}

                    //var game = await _gameManager.GetItemAsync(model.GameId);
                    //playerProfileViewModel.ClientProfileId = game.ClientProfileId;

                    //var resultPlayerProfile = await _playGameManager.CreatePlayerProfileAsync(playerProfileViewModel, model.GameId);

                    //if (resultPlayerProfile.Success)
                    //{
                    //    // Update any authentication tokens as well
                    //    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                    //    SessionCookiesHelper.SetCookiees(Response, "visPlayerProfileId", resultPlayerProfile.Error, 24 * 60);
                    //    SessionCookiesHelper.SetCookiees(Response, "visPublishGameId", game.Id, 24 * 60);
                    //    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalPlayerLoginConfirmation[POST]", User.GetLoggedInUserId(), $"User created a new account with password, UserEmail:{model.Email}"));
                    //    return RedirectToPlayer(returnUrl);
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError(string.Empty, resultPlayerProfile.Error);
                    //    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalPlayerLoginConfirmation[POST]", User.GetLoggedInUserId(), $"{resultPlayerProfile.Error}, UserEmail: {model.Email}"));
                    //}
                }

                ViewData["ReturnUrl"] = returnUrl;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "ExternalPlayerLoginConfirmation[POST]", User.GetLoggedInUserId()));
            }
            
            return View(model);
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get the information about the user from the external login provider
                    var info = await _signInManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ExternalLoginCallback", User.GetLoggedInUserId(), $"User created an account using {info.LoginProvider} provider,User:{User.Identity.Name}"));

                            // Update any authentication tokens as well
                            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }

                ViewData["ReturnUrl"] = returnUrl;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "ExternalLogin[POST]", User.GetLoggedInUserId()));
            }
            
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("ForgotPassword[POST]", User.GetLoggedInUserId(), $"UserEmail: {model.Email}"));

                if (ModelState.IsValid)
                {
                    bool isValid = true;
                    var user = await _userManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        isValid = false;
                        // Don't reveal that the user does not exist
                        ModelState.AddModelError(string.Empty, "Invalid email.");
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ForgotPassword[POST]", User.GetLoggedInUserId(), $"Invalid email, UserEmail: {model.Email}"));
                        return View(model);
                    }

                    //var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                    //if (!isEmailConfirmed)
                    //{
                    //    isValid = false;
                    //    // Don't reveal that the user email is not confirmed
                    //    ModelState.AddModelError(string.Empty, "Email is not confirmed.");
                    //    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ForgotPassword[POST]", User.GetLoggedInUserId(), $"Email is not confirmed, UserEmail: {model.Email}"));
                    //    return View(model);
                    //}

                    if(isValid)
                    {

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        var message = await GenareteForgotPasswordEmailTemplateAsync(user);
                        await _emailSender.SendEmailBySendGridAsync(user.Id, model.Email, "Reset Password", message);
                        return View("ForgotPasswordConfirmation");

                    }

                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "ForgotPassword[POST]", User.GetLoggedInUserId()));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string email, string code = null)
        {
            if (userId == null || email == null || code == null)
            {
                return View("Error");
            }
            else
            {
                Models.AccountViewModels.ResetPasswordViewModel model = new Models.AccountViewModels.ResetPasswordViewModel() { Code = HttpUtility.HtmlDecode(code).Trim(), Email = HttpUtility.HtmlDecode(email).Trim() };
                return View(model);
            }
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(Models.AccountViewModels.ResetPasswordViewModel model)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("ResetPassword[POST]", User.GetLoggedInUserId(), $"UserEmail: {model.Email}"));

                if (ModelState.IsValid)
                {
                    bool isValid = true;
                    var user = await _userManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        isValid = false;
                        // Don't reveal that the user does not exist
                        ModelState.AddModelError(string.Empty, "Invalid email.");
                        _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ResetPassword[POST]", User.GetLoggedInUserId(), $"Invalid email, UserEmail: {model.Email}"));
                        return View(model);
                    }

                    //var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                    //if (!isEmailConfirmed)
                    //{
                    //    isValid = false;
                    //    // Don't reveal that the user email is not confirmed
                    //    ModelState.AddModelError(string.Empty, "Email is not confirmed.");
                    //    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("ResetPassword[POST]", User.GetLoggedInUserId(), $"Email is not confirmed, UserEmail: {model.Email}"));
                    //    return View(model);
                    //}

                    if (isValid)
                    {
                        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                        if (result.Succeeded)
                        {
                            return View("ResetPasswordConfirmation");
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "ResetPassword[POST]", User.GetLoggedInUserId()));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("SendCode[POST]", User.GetLoggedInUserId(), $"SelectedProvider: {model.SelectedProvider}"));

                if (!ModelState.IsValid)
                {
                    return View();
                }

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return View("Error");
                }

                // Generate the token and send it
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
                if (string.IsNullOrWhiteSpace(code))
                {
                    return View("Error");
                }

                var message = "Your security code is: " + code;
                if (model.SelectedProvider == "Email")
                {
                    await _emailSender.SendEmailBySendGridAsync(user.Id, await _userManager.GetEmailAsync(user), "Security Code", message);
                }
                else if (model.SelectedProvider == "Phone")
                {
                    await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
                }

                return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "SendCode[POST]", User.GetLoggedInUserId()));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            try
            {
                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestStart("VerifyCode[POST]", User.GetLoggedInUserId(), $"Code: {model.Code}"));

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // The following code protects for brute force attacks against the two factor codes.
                // If a user enters incorrect codes for a specified amount of time then the user account
                // will be locked out for a specified amount of time.
                var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
                if (result.Succeeded)
                {
                    return RedirectToLocal(model.ReturnUrl);
                }
                if (result.IsLockedOut)
                {
                    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("VerifyCode", User.GetLoggedInUserId(), $"User account locked out,User:{User.Identity.Name}"));

                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid code.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, MessageHelper.Error);
                _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "VerifyCode[POST]", User.GetLoggedInUserId()));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToMasterAdmin(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private IActionResult RedirectToAdmin(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private IActionResult RedirectToPlayer(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private async Task<string> GenareteForgotPasswordEmailTemplateAsync(ApplicationUser user)
        {
            string htmlTemplate = string.Empty;

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            string link = Url.Action("ResetPassword", "Account", new { userId = user.Id, email = user.Email, code = passwordResetToken }, protocol: HttpContext.Request.Scheme);

            string title = "Please reset your password by clicking here:";
            string linkText = "Forgot Password";

            //htmlTemplate = "Please reset your password by clicking here: <a target='_blank' href=\"" + link + "\">link</a>";
            htmlTemplate = EmailTemplateHelper.GetEmailTemplate(title, link, linkText);

            return htmlTemplate;
        }

        //private async Task<PlayerProfileViewModel> GenaretePlayerProfileViewModelAsync(PlayerRegisterViewModel model)
        //{
        //    try
        //    {
        //        PlayerProfileViewModel playerProfileViewModel = new PlayerProfileViewModel();
        //        if (model.PlayerProfileViewModel != null)
        //        {
        //            if (model.PlayerProfileViewModel.IsPlayerNameRequired && !string.IsNullOrEmpty(model.PlayerProfileViewModel.PlayerName))
        //            {
        //                playerProfileViewModel.PlayerName = model.PlayerProfileViewModel.PlayerName;
        //            }
        //            else if(model.PlayerProfileViewModel.IsPlayerNameRequired)
        //            {
        //                ModelState.AddModelError(string.Empty, "Username is required.");
        //                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("GenaretePlayerProfileViewModel", User.GetLoggedInUserId(), $"Email is required, UserEmail: {model.Email}"));
        //                return null;
        //            }
        //            if (model.PlayerProfileViewModel.EmailAddressRequired && !string.IsNullOrEmpty(model.PlayerProfileViewModel.EmailAddress))
        //            {
        //                playerProfileViewModel.EmailAddress = model.PlayerProfileViewModel.EmailAddress;
        //                model.Email = model.PlayerProfileViewModel.EmailAddress;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, "Email is required.");
        //                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("GenaretePlayerProfileViewModel", User.GetLoggedInUserId(), $"Email is required, UserEmail: {model.Email}"));
        //                return null;
        //            }

        //            if (model.PlayerProfileViewModel.FirstNameRequired && !string.IsNullOrEmpty(model.PlayerProfileViewModel.FirstName))
        //            {
        //                playerProfileViewModel.FirstName = model.PlayerProfileViewModel.FirstName;
        //            }
        //            else if(model.PlayerProfileViewModel.FirstNameRequired)
        //            {
        //                ModelState.AddModelError(string.Empty, "First Name is required.");
        //                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("GenaretePlayerProfileViewModel", User.GetLoggedInUserId(), $"First Name is required, UserEmail: {model.Email}"));
        //                return null;
        //            }

        //            if (model.PlayerProfileViewModel.LastNameRequired && !string.IsNullOrEmpty(model.PlayerProfileViewModel.LastName))
        //            {
        //                playerProfileViewModel.LastName = model.PlayerProfileViewModel.LastName;
        //            }
        //            else if(model.PlayerProfileViewModel.LastNameRequired)
        //            {
        //                ModelState.AddModelError(string.Empty, "Last Name is required.");
        //                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("GenaretePlayerProfileViewModel", User.GetLoggedInUserId(), $"Last Name is required, UserEmail: {model.Email}"));
        //                return null;
        //            }

        //            if (model.PlayerProfileViewModel.PhoneNoRequired && !string.IsNullOrEmpty(model.PlayerProfileViewModel.PhoneNo))
        //            {

        //                playerProfileViewModel.PhoneCode = model.PlayerProfileViewModel.PhoneCode;
        //                playerProfileViewModel.PhoneNo = model.PlayerProfileViewModel.PhoneNo;
        //            }
        //            else if(model.PlayerProfileViewModel.PhoneNoRequired)
        //            {
        //                ModelState.AddModelError(string.Empty, "Phone No is required.");
        //                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("GenaretePlayerProfileViewModel", User.GetLoggedInUserId(), $"Phone No is required, UserEmail: {model.Email}"));
        //                return null;
        //            }

        //            if (model.PlayerProfileViewModel.AddressRequired && !string.IsNullOrEmpty(model.PlayerProfileViewModel.Address))
        //            {
        //                playerProfileViewModel.Address = model.PlayerProfileViewModel.Address;
        //            }
        //            else if(model.PlayerProfileViewModel.AddressRequired)
        //            {
        //                ModelState.AddModelError(string.Empty, "Address is required.");
        //                _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("GenaretePlayerProfileViewModel", User.GetLoggedInUserId(), $"Address is required, UserEmail: {model.Email}"));
        //                return null;
        //            }
        //            if (model.PlayerProfileViewModel.IsOverAgeRequired)
        //            {
        //                playerProfileViewModel.IsOverAge = model.PlayerProfileViewModel.IsOverAge;
        //            }
        //            if (model.PlayerProfileViewModel.IsAvatarRequired)
        //            {
        //                playerProfileViewModel.Avatar = model.PlayerProfileViewModel.Avatar;
        //            }
        //            //else
        //            //{
        //            //    ModelState.AddModelError(string.Empty, "Over Age is required.");
        //            //    _log.Info(Log4NetMessageHelper.LogFormattedMessageForRequestSuccess("GenaretePlayerProfileViewModel", User.GetLoggedInUserId(), $"Over Age is required, UserEmail: {model.Email}"));
        //            //    return null;
        //            //}

        //            if (model.PlayerProfileViewModel.CustomFields != null)
        //            {
        //                playerProfileViewModel.CustomFields = model.PlayerProfileViewModel.CustomFields;
        //            }
        //        }

        //        return playerProfileViewModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, MessageHelper.Error);
        //        _log.Error(Log4NetMessageHelper.FormateMessageForException(ex, "GenaretePlayerProfileViewModel", User.GetLoggedInUserId()));
        //        return null;
        //    }
        //}

        #endregion


        #endregion
    }
}
