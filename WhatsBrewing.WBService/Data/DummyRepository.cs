using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WhatsBrewing.DAL.Models;

namespace WhatsBrewing.WBService.Data
{
    public class DummyRepository
    {
        public DummyRepository()
        {
            Breweries = new List<Brewery>()
            { 
                new Brewery(){Name = "BrewDog"},
                new Brewery(){Name = "Lervig"}
             };

            Rooms = new List<Room>() 
            {
                new Room(){ Name= "Room1", Description="This room is cool", Breweries = Breweries },
                new Room(){ Name= "Room2", Description="This room is cool", Breweries = Breweries },
                new Room(){ Name= "Room3", Description="This room is cool", Breweries = Breweries },
                new Room(){ Name= "Room4", Description="This room is cool", Breweries = Breweries }
            };           

        }

        public List<Room> Rooms { get; set; }
        public List<Brewery> Breweries { get; set; }
    }
}