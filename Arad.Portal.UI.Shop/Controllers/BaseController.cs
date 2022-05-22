﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Arad.Portal.UI.Shop.Controllers
{
    public class BaseController : Controller
    {
        public readonly string DomainName;
        public readonly string CurrentUserName;
        public readonly string CurrentUserId;
        private readonly IHttpContextAccessor _accessor;
        private readonly IWebHostEnvironment _env;
    
        public BaseController(IHttpContextAccessor accessor, IWebHostEnvironment environment)
        {
            _accessor = accessor;
            _env = environment;
            if (_env.EnvironmentName == "Development")
            {
                DomainName = "http://localhost:17951";
            }
            else
            {
                DomainName = $"{_accessor.HttpContext.Request.Scheme}://{_accessor.HttpContext.Request.Host}";
            }

            if ( User != null && User.Identity.IsAuthenticated)
            {
                CurrentUserId = accessor.HttpContext.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                CurrentUserName = accessor.HttpContext.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name).Value;
            }
            else
            {
                CurrentUserId = "";
                CurrentUserName = "";
            }
           
        }
    }
}
