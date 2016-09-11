using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WhatsBrewing.WBService.SignalR
{
    [HubName("whatsbrewinghub")]
    public class WhatsBrewingHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void SetColor(string colorName, string lights = "")
        {
            Clients.All.setColor(colorName, lights);
        }

        public void SetPower(string state, string lights = "")
        {
            Clients.All.setPower(state, lights);
        }

        public void SetBrightness(string level, string lights = "")
        {
            Clients.All.setBrightness(level, lights);
        }

        public void Blink(string color, int seconds, string lights = "")
        {
            Clients.All.blink(color, seconds, lights);
        }

        
    }
}