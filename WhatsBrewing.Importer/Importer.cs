using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WhatsBrewing.DAL;
using WhatsBrewing.DAL.Models;

namespace WhatsBrewing.Importer
{
    public class Importer : WhatsBrewing.Importer.IImporter
    {
        private string _connectionStringBase = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=NO;IMEX=1;""";

        //private Context _dbCtx;

        public IdResolver _breweryResolver;
        //public RoomResolver _roomResolver;
        public IdResolver _roomResolver;
        public IdResolver _sessionResolver;

        //private List<string> _sheets = new List<string>() { "Rooms", "Breweries", "Sessions", "Beers" };
        //private Dictionary<Type,string> _typeToSheetNameMaps = new Dictionary<Type,string>()
        //{            
        //    {typeof(Brewery), "Breweries"}, 
        //    {typeof(Room), "Rooms"}, 
        //    {typeof(TapSession), "Sessions"}, 
        //    {typeof(Beer), "Beers"}
        //};

        //public Importer(Context dbCtx)
        public Importer()
        {
            //_dbCtx = dbCtx;

            _breweryResolver = new IdResolver();
            //_roomResolver = new RoomResolver();
            _roomResolver = new IdResolver();
            _sessionResolver = new IdResolver();

            var importTime = DateTime.UtcNow;

            Mapper.CreateMap<DataRow, WBBase>()
                .ForMember(d => d.Id, opt => opt.MapFrom(p => Guid.NewGuid()))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(p => importTime))
                .ForMember(d => d.Name, opt => opt.MapFrom((row) => row.Field<string>(0)))
                .ForMember(d => d.Description, opt => opt.MapFrom((row) => row.Field<string>(1)))
                .Include<DataRow, Brewery>()
                .Include<DataRow, Room>()
                .Include<DataRow, TapSession>()
                .Include<DataRow, Beer>()
                .Include<DataRow, Activity>()
                ;

            Mapper.CreateMap<DataRow, Room>();

            Mapper.CreateMap<DataRow, Brewery>()
                .ForMember(d => d.Country, opt => opt.MapFrom((row) => row.Field<string>(2)))
                .ForMember(d => d.RoomId, opt => opt.ResolveUsing(_roomResolver).FromMember((row) => row.Field<string>(3)))                
                //.ForMember(d => d.Room, opt => opt.ResolveUsing(_roomResolver).FromMember((row) => row.Field<string>(3)))
                //.AfterMap((d, s) => s.RoomId = s.Room != null ? s.Room.Id : new Nullable<Guid>())
                ;

            Mapper.CreateMap<DataRow, TapSession>()
                .ForMember(d => d.StartDate, opt => opt.MapFrom((row) => row.Field<string>(2)))
                .ForMember(d => d.StartTime, opt => opt.MapFrom((row) => row.Field<string>(3)))
                .ForMember(d => d.EndTime, opt => opt.MapFrom((row) => row.Field<string>(4)))
                ;

            Mapper.CreateMap<DataRow, Activity>()
                .ForMember(d => d.StartDate, opt => opt.MapFrom((row) => row.Field<string>(2)))
                .ForMember(d => d.StartTime, opt => opt.MapFrom((row) => row.Field<string>(3)))
                .ForMember(d => d.EndTime, opt => opt.MapFrom((row) => row.Field<string>(4)))
                .ForMember(d => d.RoomId, opt => opt.ResolveUsing(_roomResolver).FromMember((row) => row.Field<string>(5)))
                .ForMember(d => d.Icon, opt => opt.MapFrom((row) => row.Field<string>(6)))
                //.AfterMap((d, s) => s.RoomId = s.Room != null ? s.Room.Id : new Nullable<Guid>())
                ;

            Mapper.CreateMap<DataRow, Beer>()
                .ForMember(d => d.Style, opt => opt.MapFrom((row) => row.Field<string>(2)))
                .ForMember(d => d.Alcohol, opt => opt.MapFrom((row) => row.Field<string>(3)))
                .ForMember(d => d.BreweryId, opt => opt.ResolveUsing(_breweryResolver).FromMember((row) => row.Field<string>(4)))
                .ForMember(d => d.TapSessionId, opt => opt.ResolveUsing(_sessionResolver).FromMember((row) => row.Field<string>(5)))
                //.AfterMap((d, s) => s.BreweryId = s.Brewery != null ? s.Brewery.Id : new Nullable<Guid>())
                //.AfterMap((d, s) => s.TapSessionId = s.TapSession != null ? s.TapSession.Id : new Nullable<Guid>())
                ;            
               
        }

        public List<Brewery> Breweries { get; set; }
        public List<Room> Rooms { get; set; }
        public List<TapSession> Sessions { get; set; }
        public List<Beer> Beers { get; set; }
        public List<Activity> Activities { get; set; }


        public void LoadExcelData(string sourceFileName)
        {
            var importData = new DataSet() { Locale = CultureInfo.CurrentCulture };
            
            Rooms = ParseSheet<Room>("Rooms", sourceFileName, importData);
            _roomResolver.IncomingItems = Rooms.Select(p => new KeyValuePair<string,Guid?>(p.Name, new Nullable<Guid>(p.Id)));


            //AddOrUpdateRooms(rooms);

            Breweries = ParseSheet<Brewery>("Breweries", sourceFileName, importData);
            _breweryResolver.IncomingItems = Breweries.Select(p => new KeyValuePair<string, Guid?>(p.Name, new Nullable<Guid>(p.Id)));

            //AddOrUpdateBreweries(breweries);

            Sessions = ParseSheet<TapSession>("Sessions", sourceFileName, importData);
            _sessionResolver.IncomingItems = Sessions.Select(p => new KeyValuePair<string, Guid?>(p.Name, new Nullable<Guid>(p.Id))); 

            //AddOrUpdateSessions(sessions);

            Activities = ParseSheet<Activity>("Activities", sourceFileName, importData);
            //AddOrUpdateActivities(activities);

            Beers = ParseSheet<Beer>("Beers", sourceFileName, importData);
            //AddOrUpdateBeers(beers);
                        
        }

        private void AddOrUpdateBreweries(List<Brewery> breweries)
        {
            //foreach (var brewery in breweries)
            //{
            //    var existingItem = _dbCtx.Breweries.FirstOrDefault(p => p.Name == brewery.Name);

            //    if (existingItem != null)
            //    {
            //        // update existing
            //        existingItem.Description = brewery.Description;
            //        existingItem.Country = brewery.Country;
            //        existingItem.Room = brewery.Room;
            //    }
            //    else
            //    {
            //        _dbCtx.Breweries.Add(brewery);
            //    }
            //}

            //_dbCtx.Breweries.AddRange(breweries);
            //_dbCtx.SaveChanges();

            //_breweryResolver.ExistingItems = _dbCtx.Breweries.ToList();

        }

        private void AddOrUpdateRooms(List<Room> rooms)
        {
            //foreach (var room in rooms)
            //{
            //    var existingItem = _dbCtx.Rooms.FirstOrDefault(p => p.Name == room.Name);

            //    if (existingItem != null)
            //    {
            //        // update existing
            //        existingItem.Description = room.Description;
            //    }
            //    else
            //    {
            //        _dbCtx.Rooms.Add(room);
            //    }
            //}

            //_dbCtx.Rooms.AddRange(rooms);

            //_dbCtx.SaveChanges();

            //_roomResolver.ExistingItems = _dbCtx.Rooms.ToList();
        }

        private void AddOrUpdateSessions(List<TapSession> sessions)
        {
            //foreach (var session in sessions)
            //{
            //    var existingItem = _dbCtx.TapSessions.FirstOrDefault(p => p.Name == session.Name);

            //    if (existingItem != null)
            //    {
            //        // update existing
            //        existingItem.Description = session.Description;
            //        existingItem.StartDate = session.StartDate;
            //        existingItem.StartTime = session.StartTime;
            //        existingItem.EndTime = session.EndTime;                    
            //    }
            //    else
            //    {
            //        _dbCtx.TapSessions.Add(session);
            //    }
            //}

            //_dbCtx.TapSessions.AddRange(sessions);

            //_dbCtx.SaveChanges();

            //_sessionResolver.ExistingItems = _dbCtx.TapSessions.ToList();
        }

        private void AddOrUpdateActivities(List<Activity> activities)
        {
            //foreach (var activity in activities)
            //{
            //    var existingItem = _dbCtx.Activities.FirstOrDefault(p => p.Name == activity.Name);

            //    if (existingItem != null)
            //    {
            //        // update existing
            //        existingItem.Description = activity.Description;
            //    }
            //    else
            //    {
            //        _dbCtx.Activities.Add(activity);
            //    }
            //}

            //_dbCtx.Activities.AddRange(activities);

            //_dbCtx.SaveChanges();            
        }

        private void AddOrUpdateBeers(List<Beer> beers)
        {
            //foreach (var beer in beers)
            //{
            //    var existingItem = _dbCtx.Beers.FirstOrDefault(p => p.Name == beer.Name);

            //    if (existingItem != null)
            //    {
            //        // update existing
            //        existingItem.Style = beer.Style;
            //        existingItem.Description = beer.Description;
            //        existingItem.Brewery = beer.Brewery;
            //        existingItem.TapSession = beer.TapSession;
            //    }
            //    else
            //    {
            //        _dbCtx.Beers.Add(beer);
            //    }
            //}

            //_dbCtx.Beers.AddRange(beers);

            //_dbCtx.SaveChanges();
        }


        private List<T> ParseSheet<T>(string sheetName, string sourceFileName, DataSet importData)
        {
            var data = new List<T>();

            OleDbDataAdapter adapter = null;
            OleDbConnection olecon = null;
            var connectionString = string.Format(_connectionStringBase, sourceFileName);

            try
            {
                var table = new DataTable(sheetName);

                olecon = new OleDbConnection(connectionString);
                adapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}$]", sheetName), olecon);
                adapter.FillSchema(table, SchemaType.Mapped);

                foreach (DataColumn column in table.Columns)
                {
                    column.DataType = typeof(string);
                }

                adapter.Fill(table);

                importData.Tables.Add(table);

                // Load excel data                  
                data = CreateListFromDataTable<T>(table);

            }
            catch (OleDbException oleEx)
            {
                //throw new ClientException(oleEx.Message, oleEx);
                throw new Exception("OleDbException");
            }
            catch (Exception ex)
            {
                //throw new ClientException("Errors during import.", ex);
                throw new Exception("Generell Exception" + ex.Message);
            }
            finally
            {
                if (olecon != null)
                {
                    olecon.Dispose();
                }

                if (adapter != null)
                {
                    adapter.Dispose();
                }
                importData.Dispose();
            }

            return data;
        }

        private List<T> CreateListFromDataTable<T>(DataTable dataTable)
        {
            return dataTable
                .AsEnumerable()
                .Where(row => !string.IsNullOrEmpty(row.Field<string>(0)))
                .Select(Mapper.Map<T>)
                .ToList();
        }

        //public class BreweryResolver : ValueResolver<string, Brewery>
        //{
        //    public List<Brewery> IncomingItems { get; set; }
        //    public List<Brewery> ExistingItems { get; set; }

        //    public BreweryResolver()
        //    {
                
        //    }

        //    protected override Brewery ResolveCore(string name)
        //    {
        //        return !string.IsNullOrEmpty(name) && ExistingItems != null ? ExistingItems.FirstOrDefault(p => p.Name == name) : null;
        //    }
        //}

        public class IdResolver : ValueResolver<string, Guid?>
        {
            public IEnumerable<KeyValuePair<string, Guid?>> IncomingItems { get; set; }

            protected override Guid? ResolveCore(string name)
            {
                return !string.IsNullOrEmpty(name) && IncomingItems != null ? IncomingItems.Where(p=> p.Key == name).FirstOrDefault().Value : new Nullable<Guid>();
            }
        }
        

        //public class RoomResolver : ValueResolver<string, Room>
        //{
        //    public List<Room> IncomingItems { get; set; }
        //    public List<Room> ExistingItems { get; set; }

        //    public RoomResolver()
        //    {
                
        //    }

        //    protected override Room ResolveCore(string name)
        //    {
        //        return !string.IsNullOrEmpty(name) && ExistingItems != null ? ExistingItems.FirstOrDefault(p => p.Name == name) : null;
        //    }
        //}

        //public class SessionResolver : ValueResolver<string, TapSession>
        //{
        //    public List<TapSession> IncomingItems { get; set; }
        //    public List<TapSession> ExistingItems { get; set; }

        //    public SessionResolver()
        //    {
               
        //    }

        //    protected override TapSession ResolveCore(string name)
        //    {
        //        return !string.IsNullOrEmpty(name) && ExistingItems != null ? ExistingItems.FirstOrDefault(p => p.Name == name) : null;
        //    }
        //}



        //private List<Brewery> CreateListFromDataTable(DataTable dataTable)
        //{
        //    var rows = dataTable.AsEnumerable();

        //    var headerRow = rows.FirstOrDefault();

        //    return rows
        //        //.Skip(1) // skip header row
        //        .Select((row) => CreateSampleFromRow(row, headerRow))
        //        .ToList();
        //}

        public double? ParseNullableDouble(DataRow row, int elementIndex)
        {
            Double number = 0;
            return Double.TryParse(row.Field<string>(elementIndex), out number) ? new Nullable<Double>(number) : null;
        }



        public void Dispose()
        {
            Mapper.Reset();
        }
    }
}
