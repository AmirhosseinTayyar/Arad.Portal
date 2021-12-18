﻿
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Entities.Shop.Setting
{
    public class ShippingSetting: BaseEntity
    {
        public ShippingSetting()
        {
            AllowedShippingTypes = new();
            ShippingCoupon = new();
        }
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string ShippingSettingId { get; set; }
        
        public List<ShippingTypeDetail> AllowedShippingTypes { get; set; }

        public ShippingCoupon ShippingCoupon { get; set; }

    }

    public class ShippingTypeDetail
    {
        /// <summary>
        /// the value property of basicData class with groupkey equal to  'ShippingType'
        /// </summary>
        public int ShippingTypeId { get; set; }

        /// <summary>
        /// the text propert of BasicData class with groupkey equals to 'ShippingType'
        /// </summary>
        public string ShippingTypeText { get; set; }

        public bool HasFixedExpense { get; set; }

        public decimal FixedExpenseValue { get; set; }

        public string ProviderId { get; set; }
        
    }

    public class ShippingCoupon
    {
        //public string ShippingCouponId { get; set; }

        public decimal FromInvoiceExpense { get; set; }

        /// <summary>
        /// if shipping expense equal zero it means shipping is free
        /// </summary>
        public decimal ShippingExpense { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? EndDate { get; set; }
    }


}
