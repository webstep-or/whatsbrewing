using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WhatsBrewing.DAL.Models
{
    public class WBBase
    {
        [JsonProperty(Order = 1)]
        public Guid Id { get; set; }

        //[JsonProperty(Order = 1)]
        //public int Id { get; set; }
        [JsonProperty(Order = 2)]
        public string Name { get; set; }
        [JsonProperty(Order = 3)]
        public string Description { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
    }
}