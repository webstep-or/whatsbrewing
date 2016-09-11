using System.Collections.Generic;
using Newtonsoft.Json;

namespace WhatsBrewing.DAL.Models
{
    public class Room : WBBase
    {        
        //public string BeaconId { get; set; }
        //[JsonProperty(Order = 4)]
        [JsonIgnore]
        public virtual ICollection<Brewery> Breweries { get; set; }
    }
}