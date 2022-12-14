using Arad.Portal.DataLayer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Models.Domain
{
    public class DomainViewModel
    {
        public DomainViewModel()
        {
            Prices = new();
        }
        public string DomainId { get; set; }

        public string DomainName { get; set; }

        public string Title { get; set; }

        public string OwnerUserId { get; set; }

        public string OwnerUserName { get; set; }

        public string DefaultLanguageId { get; set; }

        public string DefaultLanguageName { get; set; }

        public string DefaultCurrencyName { get; set; }

        public string DefaultCurrencyId { get; set; }

        public bool IsDefault { get; set; }

        //public Price  DomainPrice { get; set; }

        public bool IsDeleted { get; set; }

        public List<Price> Prices { get; set; }
    }
}
