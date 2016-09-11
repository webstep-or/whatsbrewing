using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WhatsBrewing.DAL.Models
{
    public class Beer : WBBase
    {
        [JsonProperty(Order = 4)]
        public string Style { get; set; }

        [JsonProperty(Order = 5)]
        public string Alcohol { get; set; }

        // Foreign key 
        [JsonIgnore]
        public Guid? BreweryId { get; set; } 

        [JsonIgnore]
        [ForeignKey("BreweryId")]
        public virtual Brewery Brewery { get; set; }


        // Foreign key 
        [JsonIgnore]
        public Guid? TapSessionId { get; set; } 

        //[JsonIgnore]
        //public virtual Room Room { get; set; }
        [JsonIgnore]
        [ForeignKey("TapSessionId")]
        public virtual TapSession TapSession { get; set; }
    }
}