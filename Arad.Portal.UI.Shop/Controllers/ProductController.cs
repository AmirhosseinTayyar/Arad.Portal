﻿using Arad.Portal.DataLayer.Contracts.General.Comment;
using Arad.Portal.DataLayer.Contracts.General.Domain;
using Arad.Portal.DataLayer.Contracts.General.Language;
using Arad.Portal.DataLayer.Contracts.Shop.Product;
using Arad.Portal.DataLayer.Entities.General.User;
using Arad.Portal.DataLayer.Models.Comment;
using Arad.Portal.DataLayer.Models.Product;
using Arad.Portal.DataLayer.Models.Shared;
using Arad.Portal.UI.Shop.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Arad.Portal.GeneralLibrary.Utilities;
using Microsoft.AspNetCore.Localization;
using Arad.Portal.DataLayer.Contracts.General.User;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.StaticFiles;

namespace Arad.Portal.UI.Shop.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILanguageRepository _lanRepository;
        private readonly IDomainRepository _domainRepository;
        //private readonly EnyimMemcachedMethods<DataLayer.Entities.Shop.Transaction.Transaction> _enyimMemcachedMethods;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IConfiguration _configuration;
        private readonly string _domainName;

        public ProductController(IProductRepository productRepository, IHttpContextAccessor accessor,
            UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IWebHostEnvironment env, IUserRepository userRepository,
            ILanguageRepository lanRepository, IDomainRepository domainRepository, ICommentRepository commentRepository):base(accessor, domainRepository)
        {
            _productRepository = productRepository;
            _accessor = accessor;
            _lanRepository = lanRepository;
            _domainRepository = domainRepository;
            _userManager = userManager;
            _commentRepository = commentRepository;
            _domainName = this.DomainName;
            _configuration = configuration;
            _userRepository = userRepository;
            
            //_enyimMemcachedMethods = enyimMemcachedMethods;
        }

        [Route("{language?}/products")]
        public IActionResult Index()
        {
            ViewData["DomainTitle"] = this.DomainTitle;
            ViewData["PageTitle"] = Language.GetString("design_Products");
            return View();
        }

        [HttpGet]
        [Route("{language}/product/Download")]
        public async Task<IActionResult> Download([FromQuery]long code)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                var domaindto = _domainRepository.FetchByName(this.DomainName, false).ReturnValue;
                var entity = _productRepository.FetchByCode(code.ToString(), domaindto, userId);
                var localStaticFileStorageURL = _configuration["LocalStaticFileStorage"];
                var filePath = System.IO.Path.Combine(localStaticFileStorageURL, entity.ProductFileUrl);
               
                
                byte[] fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
                var test = GetMimeTypeForFileExtension(filePath);
                return File(fileContent, GetMimeTypeForFileExtension(filePath), entity.ProductFileName);
            }
            else
                return Json(null);
           
        }

        public string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }

        [Route("{language}/product/{**slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var isLoggedUser = HttpContext.User.Identity.IsAuthenticated;
            string userId = "";
            ViewData["DomainTitle"] = this.DomainTitle;
            userId = isLoggedUser ? HttpContext.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value : "";
            var domainEntity = _domainRepository.FetchByName(_domainName, false);
            ViewBag.Providers = domainEntity.ReturnValue.DomainPaymentProviders
                .Select(_ => new SelectListModel() { Text = _.PspType.ToString(), Value = ((int)_.PspType).ToString() });
            var lanIcon = _accessor.HttpContext.Request.Path.Value.Split("/")[1];
            string languageId;
            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            if(HttpContext.Request.Cookies[cookieName] != null)
            {
                var lanSymbol = HttpContext.Request.Cookies[cookieName];
                var defLangSymbol = lanSymbol.Split("|")[0][2..];
                languageId = _lanRepository.FetchBySymbol(defLangSymbol);
                
            }else
            {
                var lanEntity = _lanRepository.FetchLanguage(domainEntity.ReturnValue.DefaultLanguageId);
                languageId = lanEntity.LanguageId;
            }
             

            var entity = _productRepository.FetchByCode(slug, domainEntity.ReturnValue, userId);
            if (isLoggedUser && entity.ProductType == Enums.ProductType.File)
            {
                ViewBag.IsDownloadable = _productRepository.IsDownloadIconShowForCurrentUser(userId, entity.ProductId);
            }
            if (!string.IsNullOrEmpty(entity.ProductId))
            {
                var updateVisitCount = await _productRepository.UpdateVisitCount(entity.ProductId);
                if (isLoggedUser)
                {
                    #region check cookiepart for loggedUser
                    var userProductRateCookieName = $"{userId}_p{entity.ProductId}";
                    if (HttpContext.Request.Cookies[userProductRateCookieName] != null)
                    {
                        ViewBag.HasRateBefore = true;
                        ViewBag.PreRate = HttpContext.Request.Cookies[userProductRateCookieName];
                    }
                    else
                    {
                        ViewBag.HasRateBefore = false;
                    }
                    #endregion
                    #region FavoriteList
                    var userEntity = await _userManager.FindByIdAsync(userId);
                    var userFavoriteList = _userRepository.GetUserFavoriteList(userId, FavoriteType.Product);

                    if(userFavoriteList.Any(_=>_.EntityId == entity.ProductId))
                    {
                        ViewBag.Like = true;
                    }
                    else
                    {
                        ViewBag.Like = false;
                    }
                    #endregion
                }

                var lanId = _lanRepository.FetchBySymbol(lanIcon);
                ViewBag.LanIcon = lanIcon;
                ViewBag.CurCurrencyId = domainEntity.ReturnValue.DefaultCurrencyId;
                ViewBag.CurLanguageId = lanId;
                ViewData["PageTitle"] = entity.MultiLingualProperties.FirstOrDefault(_=>_.LanguageId == languageId).Name;
                return View(entity);
            }else
            {
                return Redirect($"~/{lanIcon}/ExceptionHandler/PageNotFound");
            }
            
        }

        //[HttpPost]
        //public async Task<IActionResult> RatingProduct([FromBody]RateProduct model)
        //{
        //    var userId = HttpContext.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
        //    string prevRate = ""; 
        //    var userProductRateCookieName = $"{userId}_p{model.ProductId}";
        //    if (!model.IsNew)//the user has rated before
        //    {
        //         prevRate = HttpContext.Request.Cookies[userProductRateCookieName];
        //    }
        //    int preS = !string.IsNullOrWhiteSpace(prevRate) ? Convert.ToInt32(prevRate) : 0;
           
        //    var res = await _productRepository.RateProduct(model.ProductId, model.Score,
        //            model.IsNew, preS);
        //    if (res.Succeeded)
        //    {
        //        //set its related cookie
        //        return
        //            Json(new
        //            {
        //                status = "Succeed",
        //                like = res.ReturnValue.LikeRate,
        //                dislike = res.ReturnValue.DisikeRate,
        //                half = res.ReturnValue.HalfLikeRate
        //            });
        //    }
        //    else
        //    {
        //        return Json(new { status = "error" });
        //    }
        //}
    }
}

