﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arad.Portal.UI.Shop.Controllers
{
    public class BasketController : BaseController
    {
        private readonly IBaske
        public BasketController(IHttpContextAccessor accessor):base(accessor)
        {
                
        }


        public IActionResult AddProToBasket(string productId)
        {
            var userId = base.CurrentUserId;

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
