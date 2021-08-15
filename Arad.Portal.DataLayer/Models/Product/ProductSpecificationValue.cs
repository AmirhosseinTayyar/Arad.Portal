﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Models.Product
{
    public class ProductSpecificationValue
    {

        public string LanguageId { get; set; }
        public string SpecificationId { get; set; }
        public string SpecificationGroupId { get; set; }
        public string LanguageName { get; set; }
        public string SpecificationGroupName { get; set; }
        public string SpecificationName { get; set; }
        /// <summary>
        ///  | seperated values
        /// </summary>
        public string Values { get; set; }
    }
}
