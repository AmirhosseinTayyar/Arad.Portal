﻿using Arad.Portal.DataLayer.Entities.General.User;
using Arad.Portal.DataLayer.Models.Shared;
using Arad.Portal.DataLayer.Models.User;
using Arad.Portal.GeneralLibrary.Utilities;
using Arad.Portal.UI.Shop.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Arad.Portal.UI.Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
                    
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity != null &&
                HttpContext.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }
            var viewModel = new LoginViewModel
            {
                ReturnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl,
                RememberMe = false
            };
            ViewBag.Message = string.Empty;
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Register()
        {
            RegisterDTO registerDto = new();

            return View(registerDto);
        }

        [HttpGet]
        public ActionResult CheckCaptcha(string captcha)
        {
            return Ok(HttpContext.Session.ValidateCaptcha(captcha) ? new { Status = "success" } : new { Status = "error" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterDTO model)
        {
            #region Validate
            if (!HttpContext.Session.ValidateCaptcha(model.Captcha))
            {
                ModelState.AddModelError("Captcha", Language.GetString("AlertAndMessage_CaptchaIncorrectOrExpired"));
            }

            model.FullCellPhoneNumber = model.FullCellPhoneNumber.Replace("+", "");
            model.FullCellPhoneNumber = model.FullCellPhoneNumber.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(model.FullCellPhoneNumber))
            {
                ModelState.AddModelError("CellPhoneNumber", Language.GetString("Validation_EnterMobileNumber"));
            }
            else
            {
                PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();

                PhoneNumber phoneNumber = phoneUtil.Parse(model.FullCellPhoneNumber, "IR");

                if (!phoneUtil.IsValidNumber(phoneNumber))
                {
                    ModelState.AddModelError("CellPhoneNumber", Language.GetString("Validation_MobileNumberInvalid1"));
                }
                else
                {
                    PhoneNumberType numberType = phoneUtil.GetNumberType(phoneNumber); // Produces Mobile , FIXED_LINE 

                    if (numberType != PhoneNumberType.MOBILE)
                    {
                        ModelState.AddModelError("CellPhoneNumber", Language.GetString("Validation_MobileNumberInvalid2"));
                    }
                }
            }

            if (_userManager.Users.Any(c => c.PhoneNumber == model.FullCellPhoneNumber))
            {
                ModelState.AddModelError("CellPhoneNumber", Language.GetString("Validation_MobileNumberAlreadyRegistered"));
            }

            if (string.IsNullOrWhiteSpace(model.SecurityCode))
            {
                ModelState.AddModelError("SecurityCode", Language.GetString("AlertAndMessage_ProfileConfirmPhoneError"));
            }

            OTP otp = OtpHelper.Get(model.FullCellPhoneNumber);

            if (otp == null)
            {
                ModelState.AddModelError("SecurityCode", Language.GetString("AlertAndMessage_ProfileConfirmPhoneError"));
            }
            else
            {
                if (otp.ExpirationDate >= DateTime.Now.AddMinutes(3))
                {
                    ModelState.AddModelError("SecurityCode", Language.GetString("AlertAndMessage_ProfileConfirmPhoneTimeOut"));
                }

                if (!string.IsNullOrWhiteSpace(model.SecurityCode) && !model.SecurityCode.Equals(otp.Code))
                {
                    ModelState.AddModelError("SecurityCode", Language.GetString("AlertAndMessage_ProfileConfirmPhoneTimeOut"));
                }
            }

            if (!ModelState.IsValid)
            {
                List<AjaxValidationErrorModel> errors = new();

                foreach (string modelStateKey in ModelState.Keys)
                {
                    ModelStateEntry modelStateVal = ModelState[modelStateKey];
                    errors.AddRange(modelStateVal.Errors.Select(error => new AjaxValidationErrorModel { Key = modelStateKey, ErrorMessage = error.ErrorMessage }));
                }

                return Ok(new { Status = "ModelError", ModelStateErrors = errors });
            }
            #endregion

            string id = Guid.NewGuid().ToString();

            #region Set claim
            List<IdentityUserClaim<string>> claims = new()
            {
                new() { ClaimType = ClaimTypes.GivenName, ClaimValue = model.FullCellPhoneNumber },
                new() { ClaimType = "IsActive", ClaimValue = true.ToString() },
                new() { ClaimType = "IsStaff", ClaimValue = false.ToString() },
                new() { ClaimType = "IsSystemAccount", ClaimValue = false.ToString() }
            };
            #endregion

            //UserGroup userGroup = await _userGroupRepository.GetDefault();

            ApplicationUser user = new()
            {
                UserName = model.FullCellPhoneNumber,
                IsSystemAccount = false,
                Id = id,
                CreatorId = id,
                CreatorUserName = model.FullCellPhoneNumber,
                CreationDate = DateTime.Now,
                IsActive = true,
                PhoneNumber = model.FullCellPhoneNumber,
                PhoneNumberConfirmed = true,
                Modifications = new(),
                Claims = claims,
            };

            string pass = AppUtil.GenerateRandomPassword(new() { RequireDigit = true, RequireLowercase = true, RequireNonAlphanumeric = false, RequireUppercase = true, RequiredLength = 6, RequiredUniqueChars = 0 });
            IdentityResult insertResult = await _userManager.CreateAsync(user, pass);

            if (insertResult.Succeeded)
            {
                Result result = await _createNotification.ClientSignUp("ClientSignupEmail", user, pass);

                if (!result.Succeeded)
                {
                    ErrorLog errorLog = new() { Error = result.Message, Source = @"Account\Register", Ip = Request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() };
                    await _errorLogRepository.Add(errorLog);
                }

                await _signInManager.PasswordSignInAsync(user, pass, false, false);
            }

            return Ok(insertResult.Succeeded ? new { Status = "Success", Message = Language.GetString("AlertAndMessage_OperationSuccess") } : new { Status = "Error", Message = insertResult.Errors.First().Description });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if (!HttpContext.Session.ValidateCaptcha(model.Captcha))
            {
                ModelState.AddModelError("Captcha", Language.GetString("AlertAndMessage_CaptchaIsExpired"));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await HttpContext.SignOutAsync();
            ApplicationUser user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || await _userManager.CheckPasswordAsync(user, model.Password) != true)
            {
                ViewBag.Message = Language.GetString("AlertAndMessage_InvalidUsernameOrPassword");
                return View(model);
            }

            if (!user.IsActive)
            {
                ViewBag.Message = Language.GetString("AlertAndMessage_InActiveUserAccount");
                return View(model);
            }
            await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            user.LastLoginDate = DateTime.Now;
            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && model.ReturnUrl != "/")
            {
                return Redirect(model.ReturnUrl);
            }

            TempData["LoginUser"] = $"{user.Profile.FirstName} {user.Profile.LastName} {Language.GetString("AlertAndMessage_Welcome")}";
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ChangeLang(string langId)
        {
          
            if (CultureInfo.CurrentCulture.Name != langId)
            {
                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(langId))
                    , new CookieOptions()
                    {
                        Expires = DateTimeOffset.Now.AddYears(1)
                    });

                return Ok(true);
            }

            return Ok(false);
        }

    }
}
