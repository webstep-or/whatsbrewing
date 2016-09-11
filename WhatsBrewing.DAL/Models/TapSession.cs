using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WhatsBrewing.DAL.Models
{
    public class TapSession : WBBase
    {
        [JsonProperty(Order = 4)]
        public virtual ICollection<Beer> Beers { get; set; }
        [JsonProperty(Order = 5)]
        public string StartDate { get; set; }
        [JsonProperty(Order = 6)]
        public string StartTime { get; set; }
        [JsonProperty(Order = 7)]
        public string EndTime { get; set; }
    }
}