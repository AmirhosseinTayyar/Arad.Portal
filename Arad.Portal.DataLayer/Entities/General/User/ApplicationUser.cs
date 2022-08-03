﻿using Arad.Portal.DataLayer.Models.Shared;
using Arad.Portal.DataLayer.Models.User;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Entities.General.User
{
    public class ApplicationUser : MongoUser<string>
    {
        public ApplicationUser()
        {
            FavoriteList = new();
            LoginData = new();
            Profile = new();
            LoginData = new();
            Otp = new();
        }
        public bool IsSystemAccount { get; set; }
        public bool IsDomainAdmin { get; set; }
        public bool IsActive { get; set; }
        public bool IsSiteUser { get; set; }
        public Profile Profile { get; set; }
        public string UserRoleId { get; set; }
        public OTP Otp { get; set; }
        public bool IsDeleted { get; set; }
        public List<string> FavoriteList { get; set; }
        public string DomainId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreationDate { get; set; }
        public string CreatorId { get; set; }
        public string CreatorUserName { get; set; }
        public List<Modification> Modifications { get; set; }
        public DateTime LastLoginDate { get; set; }
        public List<LoginLogoutRecord> LoginData { get; set; } 
    }


    public class ApplicationRole : MongoRole<string>
    {

    }

}
