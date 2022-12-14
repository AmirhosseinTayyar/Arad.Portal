using Arad.Portal.DataLayer.Contracts.General.Domain;
using Microsoft.AspNetCore.Hosting;
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
        public readonly string DomainTitle;
        public readonly string CurrentUserId;
        private readonly IHttpContextAccessor _accessor;
        private readonly IDomainRepository _domainRepository;
     
        public BaseController(IHttpContextAccessor accessor, IDomainRepository domainRepository)
        {
            _accessor = accessor;
            _domainRepository = domainRepository;
            DomainName = $"{_accessor.HttpContext.Request.Host}";

            DomainTitle = _domainRepository.FetchDomainTitle(DomainName);
            if (User != null && User.Identity.IsAuthenticated)
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
