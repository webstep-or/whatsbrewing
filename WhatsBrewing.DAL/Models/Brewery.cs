using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WhatsBrewing.DAL.Models
{
    public class Brewery : WBBase
    {
        [JsonProperty(Order = 4)]
        public string Country { get; set; }

        [JsonIgnore]
        public Guid? RoomId { get; set; } 

        //[JsonIgnore]
        [JsonProperty(Order = 5)]
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

        //[JsonIgnore]
        [JsonProperty(Order = 6)]
        public virtual ICollection<Beer> Beers { get; set; }

         
    }
}