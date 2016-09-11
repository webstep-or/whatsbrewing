using System.Linq;
using System.Web.Http;
using WhatsBrewing.DAL;
using WebApi.OutputCache.V2.TimeAttributes;

namespace WhatsBrewing.WBService.Controllers
{
    public class BreweriesController : ApiController
    {

        Context _dbCtx;

        public BreweriesController(IUnitOfwork Uow)
        {
            _dbCtx = Uow.DBContext;
        }

        [HttpGet]
        [CacheOutputUntilThisMonth(30)]
        public IHttpActionResult GetAll([FromUri]int skip = 0, [FromUri]int take = 1000)
        {
            dynamic items = null;

            if (items == null)
            {

                items = _dbCtx.Breweries.OrderBy(p => p.Name).Skip(skip).Take(take).Select(brewery => new
                {
                    Name = brewery.Name,
                    Country = brewery.Country,
                    Description = brewery.Description

                }).ToList();
            }

            return Ok(items);            
        }
        
    }
}
