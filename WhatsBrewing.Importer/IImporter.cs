using System;
namespace WhatsBrewing.Importer
{
    public interface IImporter : IDisposable
    {
        System.Collections.Generic.List<WhatsBrewing.DAL.Models.Activity> Activities { get; set; }
        System.Collections.Generic.List<WhatsBrewing.DAL.Models.Beer> Beers { get; set; }
        System.Collections.Generic.List<WhatsBrewing.DAL.Models.Brewery> Breweries { get; set; }
        void LoadExcelData(string sourceFileName);
        System.Collections.Generic.List<WhatsBrewing.DAL.Models.Room> Rooms { get; set; }
        System.Collections.Generic.List<WhatsBrewing.DAL.Models.TapSession> Sessions { get; set; }
    }
}
