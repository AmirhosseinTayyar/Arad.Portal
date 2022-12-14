using Arad.Portal.DataLayer.Entities.General.Email;
using Arad.Portal.DataLayer.Entities.General.User;
using Arad.Portal.DataLayer.Models.DesignStructure;
using Arad.Portal.DataLayer.Models.Domain;
using Arad.Portal.DataLayer.Models.Shared;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Arad.Portal.DataLayer.Models.Shared.Enums;

namespace Arad.Portal.DataLayer.Entities.General.Domain
{
    public class Domain : BaseEntity
    {
        public Domain()
        {
            Prices = new();
            DomainPaymentProviders = new();
            HomePageDesign = new();
        }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string DomainId { get; set; }


        /// <summary>
        /// it is the address of domain without its schemas
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// this field extracted is site for page Title at the top of browser tabs
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// the userId of its owner
        /// </summary>
        public string OwnerUserId { get; set; }

        public string OwnerUserName { get; set; }


        /// <summary>
        /// eac domain has a defaullt language this is the id of that language in our language entity
        /// </summary>
        public string DefaultLanguageId { get; set; }


        /// <summary>
        /// the symbol of default language
        /// </summary>
        public string DefaultLangSymbol { get; set; }


        /// <summary>
        /// name of default language
        /// </summary>
        public string DefaultLanguageName { get; set; }

        /// <summary>
        /// each domain has a default currency for showing price or payment this is the id of that currency in our currency entity
        /// </summary>
        public string DefaultCurrencyId { get; set; }

        /// <summary>
        /// name of default currency
        /// </summary>
        public string DefaultCurrencyName { get; set; }


        /// <summary>
        /// cause this application has ability to work with multi domain it has one default domain that show with this field
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// if site has some cases which has to send email anywhere this object should be filled for information of email service 
        /// </summary>
        public SMTP SMTPAccount  { get; set; }

        public POP POPAccount { get; set; }

        /// <summary>
        /// determine whether this domain support multilingual contents or not
        /// </summary>
        public bool IsMultiLinguals { get; set; }


        /// <summary>
        /// determine whether this domain has worked as shopping site or not
        /// </summary>
        public bool IsShop { get; set; }

        /// <summary>
        /// here we have list of prices cause the price of domain can be change during time and we can have a history of its prices
        /// </summary>
        public List<Price> Prices { get; set; }

        /// <summary>
        /// this field is related to paymentgateways which domain can support usage of it
        /// </summary>
        public List<ProviderDetail> DomainPaymentProviders { get; set; } 
        /// <summary>
        /// it determine the way that a domain produces its invoiceNumbers (if domain is shopping)
        /// mainDomain can produce invoiceNumber or it can customize and start with special number
        /// </summary>
        public InvoiceNumberProcedure InvoiceNumberProcedure { get; set; }
        /// <summary>
        /// if InvoiceNumberProcedure = CustomFromMyInstance domain owner should fill this prop
        /// otherwise main domain will generate the invoice number for this domain
        /// </summary>
        public string InvoiceNumberInitializer { get; set; }

        /// <summary>
        /// the value which wanna increase in each sequential invoice numbers if InvoiceNumberProcedure = CustomFromMyInstance
        /// </summary>
        public int? IncreasementValue { get; set; }

        /// <summary>
        /// there is already two ShippingType considered in this application which exist in basicData entity with 'ShippingType' Group Key and this is the Value Field of one of them
        /// it can be extended based on developer need
        /// </summary>
        public int DefaultShippingTypeId { get; set; }

        /// <summary>
        /// dynamic content of home page of domain which is address by "domainName" or "domainName/home/index"
        /// it will be generated by admin of site based on their requirements 
        /// </summary>
        public List<PageDesignContent> HomePageDesign { get; set; }


        /// <summary>
        /// dynamic content of productList of domain which is address by "domainName/Products" 
        /// it will be generated by admin of site based on their requirements 
        /// </summary>
        public List<PageDesignContent> ProductPageDesign { get; set; }


        /// <summary>
        /// dynamic content of productList of domain which is address by "domainName/blog" 
        /// it will be generated by admin of site based on their requirements 
        /// </summary>
        public List<PageDesignContent> BlogPageDesign { get; set; }

    }
    public class ProviderDetail
    {
        public PspType PspType { get; set; }
        public string DomainValueProvider { get; set; }
    }
    public enum InvoiceNumberProcedure
    {
        FromMainDomain,
        CustomFromMyInstance
    }
    
}
