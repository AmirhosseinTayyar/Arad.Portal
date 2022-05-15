﻿using Arad.Portal.DataLayer.Contracts.General.Domain;
using Arad.Portal.DataLayer.Models.DesignStructure;
using Arad.Portal.DataLayer.Models.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Arad.Portal.UI.Shop.Helpers
{
    public class LayoutContentProcess
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly IWebHostEnvironment _environment;
        public LayoutModel LayoutModel = new LayoutModel();
        
        public LayoutContentProcess(IDomainRepository domainRepository,
            IWebHostEnvironment env,
            IHttpContextAccessor accessor)
        {
            _domainRepository = domainRepository;
            _accessor = accessor;
            _environment = env;
            CalculateLayoutContent();
        }

        public void CalculateLayoutContent()
        {
            string domainName;
            if(_environment.EnvironmentName != "Development")
            {
                domainName = $"{_accessor.HttpContext.Request.Scheme}://{_accessor.HttpContext.Request.Host}";
            }else
            {
                domainName = "http://localhost:17951";
            }
             
            var domainEntity = _domainRepository.FetchByName(domainName).ReturnValue;
            
            if(!string.IsNullOrEmpty(domainEntity.HeaderPart.PriorFixedContent) || !string.IsNullOrEmpty(domainEntity.HeaderPart.LatterFixedContent)
                 || domainEntity.HeaderPart.CustomizedContent.Count > 0 )
            {
                LayoutModel.HasCustomizedHeader = true;
                LayoutModel.HeaderPart = domainEntity.HeaderPart;
            }

            if(!string.IsNullOrEmpty(domainEntity.FooterPart.PriorFixedContent) || !string.IsNullOrEmpty(domainEntity.FooterPart.LatterFixedContent)
                || domainEntity.FooterPart.CustomizedContent.Count > 0)
            {
                LayoutModel.HasCustomizedFooter = true;
                LayoutModel.FooterPart = domainEntity.FooterPart;
            }
          
        }
    }
}
