using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsBrewing.CAHubClient.Properties;

namespace WhatsBrewing.CAHubClient
{
    class Program
    {

        private static HueClient.WBHueClient _hueClient;

        static void Main(string[] args)
        {
            //var client = new HubClient.HubClient("http://localhost:65409/");
            var settings = GetSettings();

            _hueClient = new HueClient.WBHueClient();
            _hueClient.OnConnectionError += HandleException;

            if (_hueClient.Initialize(settings.HueAppKey).Result)
            {
                var signalRClient = new HubClient.HubClient(settings.SignalRHubURL);
                signalRClient.OnConnectionError += HandleException;

                signalRClient.Connect().Wait();

                //client.OnSetColor += (color) => Console.WriteLine(string.Format("color changed to {0}", color));
                signalRClient.HandleCommand("SetColor", Client_OnSetColor);
                signalRClient.HandleCommand("SetPower", Client_OnSetPower);
                signalRClient.HandleCommand("SetBrightness", Client_OnSetBrightness);
                signalRClient.HandleBlink("Blink", Client_OnBlink);

                Console.WriteLine("Listening to signalR hub... Press key to exit.");
            }
            else 
            {
                Console.WriteLine("Could not connect to Hue Bridge.");
            }

            Console.ReadLine();
        }

        static HubClientAppSettings GetSettings() 
        {
            var settings = new HubClientAppSettings()
            {
                SignalRHubURL = Settings.Default.SignalRHubURL,
                HueAppKey = Settings.Default.HueAppKey,
            };

            return settings;
        }

        private static void HandleException(Exception ex)
        {
            Console.WriteLine(string.Format("Error: {0}", ex));
        }

        static void Client_OnSetPower(string state, string lights)
        {
            //var hueClient = new HueClient.WBHueClient();
            //hueClient.Initialize().Wait();
            _hueClient.SetPower("on".Equals(state.ToLower()), !string.IsNullOrEmpty(lights) ? lights.Split(',').ToList() : null).Wait();

            Console.WriteLine(string.Format("Lights ({0}) State changed to {1}", string.IsNullOrEmpty(lights) ? "All": lights, state));
        }

        static void Client_OnSetColor(string hexColor, string lights)
        {
            //var hueClient = new HueClient.WBHueClient();
            //hueClient.Initialize().Wait();
            _hueClient.SetColor(hexColor, !string.IsNullOrEmpty(lights) ? lights.Split(',').ToList() : null).Wait();

            Console.WriteLine(string.Format("Lights ({0}) Color changed to {1}", string.IsNullOrEmpty(lights) ? "All" : lights, hexColor));
        }

        static void Client_OnSetBrightness(string level, string lights)
        {
            //var hueClient = new HueClient.WBHueClient();
            //hueClient.Initialize().Wait();
            _hueClient.SetBrightness(level, !string.IsNullOrEmpty(lights) ? lights.Split(',').ToList() : null).Wait();

            Console.WriteLine(string.Format("Lights ({0}) Brightness changed to {1}", string.IsNullOrEmpty(lights) ? "All" : lights, level));
        }

        static void Client_OnBlink(string hexColor, int seconds, string lights)
        {
            //var hueClient = new HueClient.WBHueClient();
            //hueClient.Initialize().Wait();
            _hueClient.Blink2(hexColor, seconds, !string.IsNullOrEmpty(lights) ? lights.Split(',').ToList() : null).Wait();

            Console.WriteLine(string.Format("Lights ({0}) Blinked for {1} seconds", string.IsNullOrEmpty(lights) ? "All" : lights, seconds));
        }
    }
}
