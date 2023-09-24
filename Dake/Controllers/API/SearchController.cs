using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Newtonsoft.Json.Linq;
using Dake.Utility;

namespace Dake.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly Context _context;
        private INotice _notice;

        public SearchController(Context context,INotice notice)
        {
            _context = context;
            _notice = notice;
        }

        // GET: api/Search
        //[HttpGet("{categoryId}/{page}/{pagesize}")]
        //public object GetSearch([FromRoute]int categoryId,[FromRoute]int page,[FromRoute] int pagesize)
        //{
        //     var data = _product.GetSearch(categoryId,page,pagesize);
        //    return data;
        //}
       

        //[HttpPost]
        //public object GetSearch(ProductSearch2 searchProduct)
        //{
            
        //    var result = _product.GetProductsByTitle(searchProduct);
        //    return result;
        //}
       
       
    }
}