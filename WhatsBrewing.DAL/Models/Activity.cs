
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Newtonsoft.Json;
namespace WhatsBrewing.DAL.Models
{
    public class Activity : WBBase
    {
        [JsonProperty(Order = 4)]
        public string StartDate { get; set; }
        [JsonProperty(Order = 5)]
        public string StartTime { get; set; }
        [JsonProperty(Order = 6)]
        public string EndTime { get; set; }

        [JsonIgnore]
        public Guid? RoomId { get; set; }

        //[JsonIgnore]
        [JsonProperty(Order = 7)]
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

        [JsonProperty(Order = 8)]
        public string Icon { get; set; }

        [NotMapped]
        [JsonProperty(Order = 9)]
        public string TimeFrame { get { return string.Format("{0} {1}-{2}", !string.IsNullOrEmpty(StartDate) ? DateTime.Parse(StartDate, CultureInfo.GetCultureInfo("nb-NO")).DayOfWeek : 0, !string.IsNullOrEmpty(StartTime) ? StartTime : "Unknown", !string.IsNullOrEmpty(EndTime) ? EndTime : "Unknown"); } }
        
        [NotMapped]
        [JsonProperty(Order = 10)]
        public string Day { get { return !string.IsNullOrEmpty(StartDate) ? DateTime.Parse(StartDate, CultureInfo.GetCultureInfo("nb-NO")).DayOfWeek.ToString() : "Unknown"; } }
    }
}
