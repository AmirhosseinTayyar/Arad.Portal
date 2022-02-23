﻿using Arad.Portal.DataLayer.Contracts.General.Currency;
using Arad.Portal.DataLayer.Contracts.General.Domain;
using Arad.Portal.DataLayer.Contracts.General.Language;
using Arad.Portal.DataLayer.Contracts.Shop.Product;
using Arad.Portal.DataLayer.Entities.General.DesignStructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Arad.Portal.UI.Shop.Dashboard.ViewComponents
{
    public class SpecialProductViewComponent : ViewComponent
    {
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILanguageRepository _lanRepository;
        private readonly IDomainRepository _domainRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public SpecialProductViewComponent(IProductRepository productRepository,ICurrencyRepository currencyRepository,
            IHttpContextAccessor accessor, ILanguageRepository lanRepository, IDomainRepository domainRepository)
        {
            _productRepository = productRepository;
            _accessor = accessor;
            _lanRepository = lanRepository;
            _domainRepository = domainRepository;
            _currencyRepository = currencyRepository;
        }

        public  IViewComponentResult Invoke(ProductOrContentType productType, ProductTemplateDesign selectionTemplate, int count)
        {
            var defaultCulture = _accessor.HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            var defLangSymbol = defaultCulture.Split("|")[0][2..];
            CultureInfo currentCultureInfo = new(defLangSymbol, false);
            var ri = new RegionInfo(currentCultureInfo.LCID);
            var currencyPrefix = ri.ISOCurrencySymbol;
            var currencyDto = _currencyRepository.GetCurrencyByItsPrefix(currencyPrefix);
            ViewBag.CurrencySymbol = currencyDto.Symbol;

            var langId = _lanRepository.FetchBySymbol(defLangSymbol);
            ViewBag.CurLangId = langId;
            
            var lst = _productRepository.GetSpecialProducts(count, currencyDto.CurrencyId, productType);
            return selectionTemplate switch
            {
                ProductTemplateDesign.First => View("First", lst),
                _ => View(lst),
            };
        }
    }
}