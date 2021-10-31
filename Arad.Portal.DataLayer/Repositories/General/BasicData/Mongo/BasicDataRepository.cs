﻿using Arad.Portal.DataLayer.Contracts.General.BasicData;
using Arad.Portal.DataLayer.Models.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Repositories.General.BasicData.Mongo
{
    public class BasicDataRepository: BaseRepository, IBasicDataRepository
    {
        private readonly IMapper _mapper;
        private readonly BasicDataContext _context;
        public BasicDataRepository(IHttpContextAccessor httpContextAccessor,
            IMapper mapper, BasicDataContext basicDataContext):base(httpContextAccessor)
        {
            _mapper = mapper;
            _context = basicDataContext;
        }

        public List<BasicDataModel> GetList(string groupKey)
        {
            var result = new List<BasicDataModel>();
            var lst = _context.Collection
                .Find(_ => _.GroupKey.ToLower() == groupKey.ToLower()).ToList();

            result = _mapper.Map<List<BasicDataModel>>(lst);
            return result;
        }
       
        public bool HasLastID()
        {
            var result = false;
            if (_context.Collection.Find(_ => _.GroupKey.ToLower() == "lastid").Any())
            {
                result = true;
            }
            return result;
        }

        public void InsertOne(Entities.General.BasicData.BasicData entity)
        {
            _context.Collection.InsertOne(entity);
        }

        public bool SaveLastId(long id)
        {
            var result = false;
            var entity = _context.Collection.Find(_ => _.GroupKey.ToLower() == "lastid").FirstOrDefault();
            entity.Text = id.ToString();
            entity.Value = id.ToString();
            var updateResult = _context.Collection.ReplaceOne(_ => _.BasicDataId == entity.BasicDataId, entity);
            if (updateResult.IsAcknowledged)
            {
                result =  true;
            }
            return result; 

        }
    }
}