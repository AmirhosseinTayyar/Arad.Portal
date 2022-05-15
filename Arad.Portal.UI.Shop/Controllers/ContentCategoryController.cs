﻿using Arad.Portal.DataLayer.Contracts.General.ContentCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Arad.Portal.UI.Shop.Controllers
{
    public class ContentCategoryController : BaseController
    {
        private readonly IContentCategoryRepository _categoryRepository;
      
        public ContentCategoryController(IContentCategoryRepository categoryRepository,
            IWebHostEnvironment env,
            IHttpContextAccessor accessor):base(accessor, env)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("{language}/category/{**slug}")]
        public IActionResult Details(long slug)
        {
            
            var entity = _categoryRepository.FetchByCode(slug);
            return View(entity);
        }
    }
}
