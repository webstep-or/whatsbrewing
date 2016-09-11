using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhatsBrewing.HueClient;

namespace WhatsBrewing.Tests.HueClient
{
    [TestClass]
    public class WBHueClientTests
    {
        [TestMethod]
        public void WBHueClient_LocateBridge()
        {
            var hubClient = new WBHueClient();

            var isLocated = hubClient.Initialize().Result;

            Assert.IsTrue(isLocated);
        }

        [TestMethod]
        public void WBHueClient_RegisterClient()
        {
            var hubClient = new WBHueClient();

            var isLocated = hubClient.Initialize().Result;

            var isRegistered = isLocated ? hubClient.RegisterApplication().Result : false;

            Assert.IsTrue(isRegistered);
        }

        [TestMethod]
        public void WBHueClient_TurnOn()
        {
            var hubClient = new WBHueClient();

            var isLocated = hubClient.Initialize().Result;
            
            var test = hubClient.SetPower().Result;

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void WBHueClient_TurnOff()
        {
            var hubClient = new WBHueClient();

            hubClient.Initialize().Wait();
                       

            var test = hubClient.SetPower(false).Result;

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void WBHueClient_SetColor()
        {
            var hubClient = new WBHueClient();

            hubClient.Initialize().Wait();

            var test = hubClient.SetColor("14bf4b").Result;

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void WBHueClient_ColorLoop()
        {
            var hubClient = new WBHueClient();

            hubClient.Initialize().Wait();

            var test = hubClient.ColorLoop().Result;

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void WBHueClient_Blink()
        {
            var hubClient = new WBHueClient();

            hubClient.Initialize().Wait();

            var test = hubClient.Blink().Result;

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void WBHueClient_Blink_ID2()
        {
            var hubClient = new WBHueClient();

            hubClient.Initialize().Wait();

            var test = hubClient.Blink(new List<string>{"2"}).Result;

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void WBHueClient_Blink2()
        {
            var hubClient = new WBHueClient();

            hubClient.Initialize().Wait();

            var test = hubClient.Blink2("14bf4b", 5).Result;

            Assert.IsTrue(test);
        }
    }
}
