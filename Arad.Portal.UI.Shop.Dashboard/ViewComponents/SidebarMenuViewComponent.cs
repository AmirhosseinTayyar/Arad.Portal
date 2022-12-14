using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Arad.Portal.DataLayer.Entities.General.User;
using Arad.Portal.DataLayer.Contracts.General.Permission;
using Arad.Portal.DataLayer.Models.Shared;
using System.Security.Claims;
using System.Text;
using AspNetCore.Identity.Mongo.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Routing;
using Arad.Portal.DataLayer.Models.Permission;
using Microsoft.Extensions.Configuration;

namespace Arad.Portal.UI.Shop.Dashboard.ViewComponents
{
    public class SidebarMenu : ViewComponent
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IConfiguration _configuration;

        public SidebarMenu(
            IHttpContextAccessor accessor,
            IPermissionRepository permissionRepository,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _accessor = accessor;
            _permissionRepository = permissionRepository;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menues = new List<PermissionTreeViewDto>();
            try
            {
                string userId = _accessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var route = _accessor.HttpContext.GetRouteData();

                var baseAddress = _configuration["BaseAddress"];
                //if(string.IsNullOrWhiteSpace(baseAddress))
                //{
                //    baseAddress = "/Admin";
                //}
                ViewBag.BasePath = baseAddress;
                var req = _accessor.HttpContext.Request;
                var obj = new RequestMenuModel()
                {
                    PathString = req.Path,
                    Domain = $"{_accessor.HttpContext.Request.Host}"
                };

                
                if (userId != null)
                {
                    menues = await _permissionRepository.GetMenus(userId,obj.PathString);
                }
            }
            catch (Exception e)
            {

            }
            return View(menues);
        }
    }
}
