using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using WhatsBrewing.DAL;
using WebApi.OutputCache.V2.TimeAttributes;

namespace WhatsBrewing.WBService.Controllers
{
    public class ActivitiesController : ApiController
    {
         Context _dbCtx;
        
         public ActivitiesController(IUnitOfwork Uow)
         {
             _dbCtx = Uow.DBContext;
         }

         [HttpGet]
         //[ResponseType(typeof(IEnumerable<Activity>))]
         [CacheOutputUntilThisMonth(30)]
         public IHttpActionResult GetAll([FromUri]int skip = 0, [FromUri]int take = 1000)
         {
             dynamic items = null;
             
             if (items == null)
             {
                 items = _dbCtx.Activities
                  .Include(p => p.Room)
                  .OrderBy(p => p.StartDate)
                  .ThenBy(p => p.StartTime)
                  .ThenBy(p => p.EndTime)
                  .ToList()
                  .Skip(skip)
                  .Take(take)
                  .GroupBy(p => p.Day)
                  .Select(grp => new
                  {
                      Day = grp.Key,
                      Activities = grp.Select(act => new
                      {
                          Name = act.Name,
                          StartDate = act.StartDate,
                          StartTime = act.StartTime,
                          EndTime = act.EndTime,
                          Description = act.Description,
                          Room = act.Room != null ? act.Room.Name : "Unknown"
                      })
                  }
                  ).ToList();

             }

             return Ok(items);            

         }

         [HttpGet]
         //[ResponseType(typeof(IEnumerable<TapSession>))]
         [CacheOutputUntilThisMonth(30)]
         public IHttpActionResult GetAllTimeFrameGrp()
         {
             var test = _dbCtx.Activities
                 .OrderBy(p=> p.StartDate)
                 .ThenBy(p=> p.StartTime)
                 .ThenBy(p=> p.EndTime)
                 .ToList()
                 .GroupBy(p => p.Day)
                 .Select(grp =>
                     new
                     {
                         Day = grp.Key,
                         Activities = grp.Select(act => new
                         {
                             Name = act.Name,
                             StartDate = act.StartDate,
                             StartTime = act.StartTime,
                             EndTime = act.EndTime,
                             act.Description,
                             Room = act.Room != null ? act.Room.Name : "Unknown"
                         })
                     });


             if (test != null)
             {
                 return Ok(test);
             }
             else
             {
                 return InternalServerError();
             }
         }         
         
    }
}
