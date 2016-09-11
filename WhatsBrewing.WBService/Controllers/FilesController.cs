using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.WindowsAzure.ServiceRuntime;
using WebApi.OutputCache.V2;
using WhatsBrewing.DAL;
using WhatsBrewing.Importer;


namespace WhatsBrewing.WBService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    public class FilesController : ApiController
    {
        public static string appDataFolder = HttpContext.Current.Server.MapPath("~/App_Data/");

        Context _context;
        IImporter _importer;

        public FilesController(IUnitOfwork Uow, IImporter importer)
        {
            _context = Uow.DBContext;
            _importer = importer;            
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                //return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "The request doesn't contain valid content!");
                return BadRequest("The request doesn't contain valid content!");
            }

            _context.Beers.RemoveRange(_context.Beers.AsEnumerable());
            _context.Breweries.RemoveRange(_context.Breweries.AsEnumerable());
            _context.Activities.RemoveRange(_context.Activities.AsEnumerable());
            _context.Rooms.RemoveRange(_context.Rooms.AsEnumerable());
            _context.TapSessions.RemoveRange(_context.TapSessions.AsEnumerable());
            await _context.SaveChangesAsync();

            try
            {

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var file = provider.Contents.FirstOrDefault();


                var dataStream = await file.ReadAsStreamAsync();

                var filename = string.Format(@"{0}\temp.xlsx", appDataFolder);

                if (RoleEnvironment.IsAvailable)
                {
                    filename = Path.Combine(RoleEnvironment.GetLocalResource("TempFiles").RootPath, "temp.xlsx");
                }

                // use the data stream to persist the data to the server (file system etc)

                using (FileStream SourceStream = File.Open(filename, FileMode.Create))
                {
                    SourceStream.Seek(0, SeekOrigin.End);
                    dataStream.CopyTo(SourceStream);
                }

                dataStream.Close();

                _importer.LoadExcelData(filename);

                _context.Configuration.AutoDetectChangesEnabled = false;

                _context.Rooms.AddRange(_importer.Rooms);
                _context.Breweries.AddRange(_importer.Breweries);
                _context.TapSessions.AddRange(_importer.Sessions);
                _context.Beers.AddRange(_importer.Beers);
                _context.Activities.AddRange(_importer.Activities);

                await _context.SaveChangesAsync();

                _context.Configuration.AutoDetectChangesEnabled = true;


                //}
                //var response = Request.CreateResponse(HttpStatusCode.OK);
                //response.Content = new StringContent("Successful upload", Encoding.UTF8, "text/plain");
                //response.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(@"text/html");
                //return response;

                var cache = Configuration.CacheOutputConfiguration().GetCacheOutputProvider(Request);

                //and invalidate cache for method "Get" of "TeamsController"
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((ActivitiesController t) => t.GetAll(0, 1000)));
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((ActivitiesController t) => t.GetAllTimeFrameGrp()));
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((BeersController t) => t.GetAll(0, 1000)));
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((BreweriesController t) => t.GetAll(0, 1000)));
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((RoomsController t) => t.GetAll(0, 1000)));
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((TapSessionsController t) => t.GetAll(0, 1000)));
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((TapSessionsController t) => t.GetAllRoomGrp()));
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((TapSessionsController t) => t.GetByName("any", "any")));


                return Ok("Cheers! Info was updated!");
            }
            catch (Exception e)
            {
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
                return InternalServerError(e);
            }
        }

        
    }
}

