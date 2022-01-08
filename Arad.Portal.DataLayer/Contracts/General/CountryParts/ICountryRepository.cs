﻿using Arad.Portal.DataLayer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arad.Portal.DataLayer.Contracts.General.CountryParts
{
    public interface ICountryRepository
    {
        //Task InsertMany(List<Entities.General.Country.Country> countries);

        List<SelectListModel> GetAllCountries();

        List<SelectListModel> GetStates(string countryId);

        List<SelectListModel> GetCities(string cityId);
    }
}
