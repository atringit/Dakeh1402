﻿using Dake.Models;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
    public interface ICityService
    {
        Task<object> GetCitiesAsync();

        PagedList<City> GetCities(int pageId = 1, string filterName = "");

        object GetProvinces(int cityId);
        object GetAreas(int provinceId);
    }
}
