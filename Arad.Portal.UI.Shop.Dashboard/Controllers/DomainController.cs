﻿using Arad.Portal.DataLayer.Contracts.General.Domain;
using Arad.Portal.DataLayer.Contracts.General.Language;
using Arad.Portal.DataLayer.Models.Domain;
using Arad.Portal.DataLayer.Models.Shared;
using Arad.Portal.UI.Shop.Dashboard.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Arad.Portal.GeneralLibrary.Utilities;
using Arad.Portal.DataLayer.Entities.General.User;
using Microsoft.AspNetCore.Identity;
using Arad.Portal.DataLayer.Contracts.General.Currency;
using Microsoft.AspNetCore.Authorization;
using Arad.Portal.DataLayer.Contracts.General.Services;
using static Arad.Portal.DataLayer.Models.Shared.Enums;
using System.Reflection;
using Arad.Portal.DataLayer.Entities.General.Domain;

namespace Arad.Portal.UI.Shop.Dashboard.Controllers
{
    [Authorize(Policy = "Role")]
    public class DomainController : Controller
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IPermissionView _permissionViewManager;
        private readonly IProviderRepository _providerRepository;
        private readonly ILanguageRepository _lanRepository;
        private readonly ICurrencyRepository _curRepository;
        private readonly UserManager<ApplicationUser> _userManager;
       
        public DomainController(IDomainRepository domainRepository, UserManager<ApplicationUser> userManager,
            IProviderRepository providerRepository,
            IPermissionView permissionView, ILanguageRepository lanRepository,
            ICurrencyRepository currencyRepository)
        {
            _domainRepository = domainRepository;
            _permissionViewManager = permissionView;
            _lanRepository = lanRepository;
            _userManager = userManager;
            _curRepository = currencyRepository;
            _providerRepository = providerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            PagedItems<DomainViewModel> result = new PagedItems<DomainViewModel>();
            var dicKey = await _permissionViewManager.PermissionsViewGet(HttpContext);
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Permissions = dicKey;
            try
            {
                result = await _domainRepository.AllDomainList(Request.QueryString.ToString());
                ViewBag.DefLangId = _lanRepository.GetDefaultLanguage(currentUserId).LanguageId;
                ViewBag.LangList = _lanRepository.GetAllActiveLanguage();
            }
            catch (Exception)
            {
            }
            return View(result);
        }
        public async Task<IActionResult> AddEdit(string id = "")
        {
            var model = new DomainDTO();
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDB = await _userManager.FindByIdAsync(currentUserId);
            if (userDB.IsSystemAccount)
            {
                var vendorList = await _userManager.GetUsersForClaimAsync(new Claim("AppRole", "True"));
                var vendors = vendorList.Select(_ => new SelectListModel()
                {
                    Text = _.Profile.FullName,
                    Value = _.Id
                }).ToList();
                vendors.Insert(0, new SelectListModel() { Text = Language.GetString("AlertAndMessage_Choose"), Value = "-1" });
                ViewBag.Vendors = vendors;
            }
            //var paymentProviders = _providerRepository.GetProvidersPerType(DataLayer.Entities.General.Service.ProviderType.Payment);
            var paymentProviders = _domainRepository.GetPspTypesEnum();
            ViewBag.Providers = paymentProviders;

            if (!string.IsNullOrWhiteSpace(id))
            {
                model = _domainRepository.FetchDomain(id).ReturnValue;
            }
            
            var lan = _lanRepository.GetDefaultLanguage(currentUserId);
            ViewBag.LangId = lan.LanguageId;
            ViewBag.LangList = _lanRepository.GetAllActiveLanguage();

            var currencyList = _curRepository.GetAllActiveCurrency();
            ViewBag.CurrencyList = currencyList;
            ViewBag.DefCurrency = _curRepository.GetDefaultCurrency(currentUserId).ReturnValue.CurrencyId;

            var invoiceNumberEnum = _domainRepository.GetInvoiceNumberProcedureEnum();
            ViewBag.InvoiceNumberEnum = invoiceNumberEnum;

            return View(model);
        }
       
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DomainDTO dto)
        {

            JsonResult result;
            if (!ModelState.IsValid)
            {
                var errors = new List<AjaxValidationErrorModel>();

                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    errors.AddRange(modelStateVal.Errors
                        .Select(error => new AjaxValidationErrorModel { Key = modelStateKey, ErrorMessage = error.ErrorMessage }));
                }
                result = Json(new { Status = "ModelError", ModelStateErrors = errors });
            }
            else
            {
                foreach (var item in dto.DomainPaymentProviders)
                {
                    item.PspType = (PspType)Enum.Parse(typeof(PspType), item.Type);
                } 
                foreach (var item in dto.Prices)
                {
                    var cur = _curRepository.FetchCurrency(item.CurrencyId);

                    item.PriceId = Guid.NewGuid().ToString();
                    item.Symbol = cur.ReturnValue.Symbol;
                    item.Prefix = cur.ReturnValue.Symbol;
                    item.SDate = DateHelper.ToEnglishDate(item.StartDate);
                }
                Result saveResult = await _domainRepository.AddDomain(dto);
                result = Json(saveResult.Succeeded ? new { Status = "Success", saveResult.Message }
                : new { Status = "Error", saveResult.Message });
            }
            return result;

        }

        [HttpPost]
        public IActionResult GetProviderParams([FromQuery] int pspVal)
        {
            var htmlResult = "";
            var psp = (PspType)pspVal;
            switch (psp)
            {
                case PspType.IranKish:
                    htmlResult = GenerateForm(typeof(IrankishModel).GetProperties());
                    break;
                case PspType.Saman:
                    htmlResult = GenerateForm(typeof(SamanModel).GetProperties());
                    break;
                case PspType.Parsian:
                    htmlResult = GenerateForm(typeof(ParsianModel).GetProperties());
                    break;
            }
            return Content(htmlResult);
        }

        private string GenerateForm(PropertyInfo[] properties)
        {
            var finalHTML = "";
            foreach (var prop in properties)
            {
                finalHTML += $"<div class='form-group col-md-3'><label for='{prop.Name}'>{prop.Name}</label><br/><input type='text' id='{prop.Name}' class='form-control gatewayPar' value='' /></div>";
            }
            
            return finalHTML;
        }

        [HttpGet]
        public async Task<IActionResult> Restore(string id)
        {
            JsonResult result;
            try
            {
                var dto = _domainRepository.FetchDomain(id);
                if (dto == null)
                {
                    result = new JsonResult(new
                    {
                        Status = "error",
                        Message = Language.GetString("AlertAndMessage_EntityNotFound")
                    });
                }
                else
                {
                    var res = await _domainRepository.Restore(id);
                    if (res.Succeeded)
                    {
                        result = new JsonResult(new
                        {
                            Status = "success",
                            Message = Language.GetString("AlertAndMessage_EditionDoneSuccessfully")
                        });
                    }
                    else
                    {
                        result = new JsonResult(new
                        {
                            Status = "error",
                            Message = Language.GetString("AlertAndMessage_TryLator")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                result = new JsonResult(new
                {
                    Status = "error",
                    Message = Language.GetString("AlertAndMessage_TryLator")
                });
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] DomainDTO dto)
        {
            JsonResult result;
            DomainDTO model;
            if (!ModelState.IsValid)
            {
                var errors = new List<AjaxValidationErrorModel>();

                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    errors.AddRange(modelStateVal.Errors.Select(error => new AjaxValidationErrorModel { Key = modelStateKey, ErrorMessage = error.ErrorMessage }));
                }

                result = Json(new { Status = "ModelError", ModelStateErrors = errors });
            }
            else
            {
                foreach (var item in dto.DomainPaymentProviders)
                {
                    item.PspType = (PspType)Enum.Parse(typeof(PspType), item.Type);
                }
                foreach (var item in dto.Prices)
                {
                    var cur = _curRepository.FetchCurrency(item.CurrencyId);

                    item.PriceId = Guid.NewGuid().ToString();
                    item.Symbol = cur.ReturnValue.Symbol;
                    item.Prefix = cur.ReturnValue.Symbol;
                    item.SDate = DateHelper.ToEnglishDate(item.StartDate);
                }
                model = _domainRepository.FetchDomain(dto.DomainId).ReturnValue;
                if (model == null)
                {
                    return RedirectToAction("PageOrItemNotFound", "Account");
                }
            }
            Result saveResult = await _domainRepository.EditDomain(dto);

            result = Json(saveResult.Succeeded ? new { Status = "Success", saveResult.Message }
            : new { Status = "Error", saveResult.Message });
            return result;
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            Result opResult = await _domainRepository.DeleteDomain(id, "delete");
            return Json(opResult.Succeeded ? new { Status = "Success", opResult.Message }
            : new { Status = "Error", opResult.Message });
        }
    }
}
