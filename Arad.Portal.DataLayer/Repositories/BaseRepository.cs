﻿using Arad.Portal.DataLayer.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Repositories
{
    public class BaseRepository
    {
        protected  IHttpContextAccessor _httpContextAccessor;
        public BaseRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected Modification GetCurrentModification(string modificationReason)
        {
            return new Modification()
            {
                DateTime = DateTime.Now,
                UserId = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value,
                UserName = _httpContextAccessor.HttpContext.User.Claims
                 .FirstOrDefault(_ => _.Type == ClaimTypes.Name).Value,
                ModificationReason = modificationReason
            };
        }
    }
}
