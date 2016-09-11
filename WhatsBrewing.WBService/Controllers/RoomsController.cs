using System.Linq;
using System.Data.Entity;
using System.Web.Http;
using WhatsBrewing.DAL;
using WebApi.OutputCache.V2.TimeAttributes;

namespace WhatsBrewing.WBService.Controllers
{
    public class RoomsController : ApiController
    {
        
        Context _dbCtx;

        public RoomsController(IUnitOfwork Uow)
        {
            _dbCtx = Uow.DBContext;
        }

        [HttpGet]
        [CacheOutputUntilThisMonth(30)]
        public IHttpActionResult GetAll([FromUri]int skip = 0, [FromUri]int take = 1000)
        {

            var items = _dbCtx.Rooms.Include(p => p.Breweries).ToList();

            return Ok(items);

        }        
    }
}
