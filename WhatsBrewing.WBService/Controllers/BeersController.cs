using System.Linq;
using System.Data.Entity;
using System.Web.Http;
using WhatsBrewing.DAL;
using WebApi.OutputCache.V2.TimeAttributes;


namespace WhatsBrewing.WBService.Controllers
{
    public class BeersController : ApiController
    {
        Context _dbCtx;

        public BeersController(IUnitOfwork Uow)
        {
            _dbCtx = Uow.DBContext;
        }

        [HttpGet]
        //[ResponseType(typeof(IEnumerable<Beer>))]
        [CacheOutputUntilThisMonth(30)]
        public IHttpActionResult GetAll([FromUri]int skip = 0, [FromUri]int take = 1000)
        {            

            dynamic items = null;

            if (items == null)
            {
                items = _dbCtx.Beers.Include(p => p.Brewery).OrderBy(p => p.Name).Skip(skip).Take(take).Select(beer => new
                {
                    Name = beer.Name,
                    Style = beer.Style,
                    Brewery = beer.Brewery != null ? beer.Brewery.Name : "Unknown",
                    Country = beer.Brewery != null ? beer.Brewery.Country : "Unknown",
                    Alcohol = beer.Alcohol,
                    Description = beer.Description
                }).ToList();
            }


            return Ok(items);            
        }                
    }
}
