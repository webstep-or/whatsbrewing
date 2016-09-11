using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using WebApi.OutputCache.V2.TimeAttributes;
using WhatsBrewing.DAL;
using WhatsBrewing.DAL.Models;

namespace WhatsBrewing.WBService.Controllers
{
    public class TapSessionsController : ApiController
    {
        Context _dbCtx;

        public TapSessionsController(IUnitOfwork Uow)
        {
            _dbCtx = Uow.DBContext;
        }

        [HttpGet]
        //[ResponseType(typeof(IEnumerable<TapSession>))]
        [CacheOutputUntilThisMonth(30)]
        public IHttpActionResult GetAll([FromUri]int skip = 0, [FromUri]int take = 1000)
        {
            var items = _dbCtx.TapSessions
                .OrderBy(p => p.Name)
                .Skip(skip)
                .Take(take)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate,
                    p.StartTime,
                    p.EndTime
                }).ToList();

            return Ok(items);
        }


        [HttpGet]
        [CacheOutputUntilThisMonth(30)]
        public IHttpActionResult GetAllRoomGrp()
        {
            var sessions = _dbCtx.TapSessions
                    .OrderBy(p => p.Name)
                    //.Where(p=> p.Beers.Any())
                    .Include(p => p.Beers.Select(b => b.Brewery.Room))
                    .ToList<TapSession>();

            CultureInfo culture = new CultureInfo("nb-NO");

            var result = sessions.Select(session => new
            {
                Name = session.Name,
                Description = session.Description,
                Rooms = session.Beers.Where(b=> b.BreweryId != null && b.Brewery.RoomId != null)
                    .OrderBy(p => p.Brewery.Room.Name, StringComparer.Create(culture, false))
                    .GroupBy(p => p.Brewery.Room.Name, StringComparer.Create(culture, false))
                    .Select(grp => new
                    {
                        Name = grp.Key,
                        Beers = grp.OrderBy(p => p.Brewery.Name, StringComparer.Create(culture, false)).Select(beer => new 
                        {
                            Name = beer.Name,
                            Style = beer.Style,
                            Alcohol = !string.IsNullOrEmpty(beer.Alcohol) ? beer.Alcohol : "? %",
                            Brewery = beer.Brewery != null ? beer.Brewery.Name : "Unknown",
                            Country = beer.Brewery != null ? beer.Brewery.Country : "Unknown"
                        })
                    })
            });
              

            return Ok(result);
        }

        [HttpGet]
        [CacheOutputUntilThisMonth(30)]
        public IHttpActionResult GetAllBreweryGrp()
        {
            var sessions = _dbCtx.TapSessions
                    .OrderBy(p => p.Name)
                    .Include(p => p.Beers.Select(b => b.Brewery.Room))
                    .ToList<TapSession>();

            var result = sessions.Select(session => new
            {
                Name = session.Name,
                Description = session.Description,
                Breweries = session.Beers
                    .Where(b => b.BreweryId != null && b.Brewery.RoomId != null)
                    .OrderBy(p => p.Brewery.Name)
                    .GroupBy(p => p.Brewery.Name)
                    .Select(grp => new
                    {
                        Name = grp.Key,
                        Beers = grp.OrderBy(p => p.Name).Select(beer => new
                        {
                            Name = beer.Name,
                            Style = beer.Style,
                            Alcohol = !string.IsNullOrEmpty(beer.Alcohol)? beer.Alcohol : "? %"
                        })
                    })
                    
            });

            return Ok(result);
        }


        [HttpGet]
        [CacheOutputUntilThisMonth(30)]
        //[ResponseType(typeof(IEnumerable<TapSession>))]
        public IHttpActionResult GetByName([FromUri]string name, [FromUri]string group = "room")
        {
            List<TapSession> items = null;

            if (items == null)
            {
                items = _dbCtx.TapSessions
                    .OrderBy(p => p.Name)
                    .Include(p => p.Beers.Select(b => b.Brewery.Room))
                    .ToList<TapSession>();
            }
            
            dynamic test = null;
                
            if("brewery".Equals(group))
            {
                test = GetByNameBreweryGrp(items.FirstOrDefault(p => p.Name == name));
            }
            else
            {
                test = GetByNameRoomGrp(items.FirstOrDefault(p => p.Name == name));
            }

            if (test != null) 
            {
                return Ok(test);
            }
            else
            {
                return InternalServerError();

            }
        }

        private dynamic GetByNameRoomGrp(TapSession tapSession) 
        {
            if (tapSession == null) 
            {
                return null;
            }

            //var tapSession = tapsessions.FirstOrDefault(p => p.Name == name);

            var rooms = tapSession.Beers.Where(p => p.Brewery != null && p.Brewery.Room != null).Select(p => p.Brewery.Room).Distinct().ToList();
            var beers = tapSession.Beers.ToList();

            return new
            {
                Name = tapSession.Name,
                Description = tapSession.Description,
                Rooms = rooms.Select(room => new
                {
                    Name = room.Name,
                    Beers = beers.Where(beer=> beer.Brewery.RoomId == room.Id).Select(beer => new
                    {
                        Name = beer.Name,                        
                        Style = beer.Style,
                        Alcohol = beer.Alcohol,
                        Brewery = beer.Brewery != null ? beer.Brewery.Name : "Unknown",
                        Country = beer.Brewery != null ? beer.Brewery.Country: "Unknown"
                    })
                }).ToList()
            };
        }

        private dynamic GetByNameBreweryGrp(TapSession tapSession)
        {

            if (tapSession == null)
            {
                return null;
            }

            var breweries = tapSession.Beers.ToList().Where(p => p.Brewery != null).Select(p => p.Brewery).Distinct().ToList();
            var beers = tapSession.Beers.ToList();

            return new
            {
                Name = tapSession.Name,
                Description = tapSession.Description,
                Breweries = breweries.Select(brewery => new
                {
                    Name = brewery.Name,
                    Country = brewery.Country,
                    @Room = brewery.Room != null ? brewery.Room.Name : "Unknown",
                    Beers = beers.Where(p => p.BreweryId == brewery.Id).Select(beer => new 
                    {
                        Name = beer.Name,
                        Style = beer.Style,
                        Alcohol = beer.Alcohol,
                    })
                }                    
                ).ToList()
            };
        }


        private IEnumerable<dynamic> GetAllRoomsGrp(IEnumerable<TapSession> tapsessions)
        {
            foreach (var tapSession in tapsessions)
            {
                var beers = tapSession.Beers.OrderBy(p=> p.Brewery.Name);
                var rooms = beers.Select(p => p.Brewery.Room).Distinct().OrderBy(p=> p.Name).ToList();
                

                yield return new
                {
                    Name = tapSession.Name,
                    Description = tapSession.Description,
                    Rooms = rooms.Select(room => new
                    {
                        Name = room.Name,
                        Beers = beers.Where(beer => beer.Brewery.RoomId == room.Id).Select(beer => new
                        {
                            Name = beer.Name,
                            Style = beer.Style,
                            Alcohol = beer.Alcohol,
                            Brewery = beer.Brewery != null ? beer.Brewery.Name : "Unknown",
                            Country = beer.Brewery != null ? beer.Brewery.Country : "Unknown"
                        }).ToList()
                    })
                };
            }
        }
    }
}
