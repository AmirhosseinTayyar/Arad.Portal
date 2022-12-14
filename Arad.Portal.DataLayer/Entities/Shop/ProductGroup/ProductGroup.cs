using Arad.Portal.DataLayer.Entities.Shop.Product;
using Arad.Portal.DataLayer.Models.Shared;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Entities.Shop.ProductGroup
{
    public class ProductGroup : BaseEntity
    {
        public ProductGroup()
        {
            MultiLingualProperties = new List<MultiLingualProperty>();
            GroupImage = new Image();
        }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string ProductGroupId { get; set; }

        /// <summary>
        /// language, Name and urlFriend will be filled here
        /// </summary>
        public List<MultiLingualProperty> MultiLingualProperties { get; set; }

        /// <summary>
        /// a GroupCode is a unique code between Product, ProductGroup, Content and ContentCategory entity
        /// which generated by application and lastCode will be store in BasicData Entity with GroupKey ='lastid' 
        /// with group/GroupCode we can access this entity in store part
        /// </summary>

        public long GroupCode { get; set; }


        /// <summary>
        /// the primary key Of parent entity
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// this entity can have a photo if it has a photo detail information of it store here
        /// </summary>
        public Image GroupImage { get; set; }

        /// <summary>
        /// if there is any available and valid promotion which assign to this productGroup it is that reference
        /// </summary>
        public Promotion.Promotion Promotion { get; set; }

    }
}
