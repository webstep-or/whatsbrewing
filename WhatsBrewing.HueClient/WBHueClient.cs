using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace WhatsBrewing.HueClient
{
    public class WBHueClient
    {
        Q42.HueApi.HueClient _client;
        string _bridgeIp = string.Empty;
        string _appKey = "279dee22271a619716b653e5dbba3";
        List<string> _allLights = new List<string>();
        string _appName = "whatsbrewingapp";
        //string _appKey = "whatsbrewingapp";                
        Timer _test;

        public event Action<Exception> OnConnectionError;

        public async Task<bool> Initialize(string appKey = null) 
        {
            if (await LocateBridge())
            {
                _client = new Q42.HueApi.HueClient(_bridgeIp);

                if (!string.IsNullOrEmpty(appKey))
                {
                    _appKey = appKey;
                }

                _client.Initialize(_appKey);

                if (_client.IsInitialized)
                {
                    try
                    {
                        var lights = await _client.GetLightsAsync();

                        _allLights = lights.Select(p => p.Id).ToList();
                    }
                    catch (Exception ex) 
                    {
                        OnConnectionError(ex);
                    }

                }
            }

            return _allLights.Count > 0;
        }

        private async Task<bool> LocateBridge()
        {
            IBridgeLocator locator = new HttpBridgeLocator();

            IEnumerable<string> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

            _bridgeIp = bridgeIPs.FirstOrDefault();

            return !string.IsNullOrEmpty(_bridgeIp);
        }


        public async Task<bool> RegisterApplication()
        {
            var isRegistered = false;

            if (!string.IsNullOrEmpty(_bridgeIp))
            {
                _client = new Q42.HueApi.HueClient(_bridgeIp);

                // Remember to press the bridge button
                isRegistered = await _client.RegisterAsync(_appName, _appKey);
            }

            return isRegistered;
        }
        
        /// <summary>
        /// If lightId is not provided, all lights will be turned on.
        /// </summary>
        /// <param name="lightId"></param>
        /// <returns></returns>
        public async Task<bool> SetPower(bool state = true, List<string> lights = null) 
        {
            var command = new LightCommand();
            command.On = state;
            command.TransitionTime = TimeSpan.FromMilliseconds(0);

            if (state)
            {
                command.Brightness = 255;
            }

            await _client.SendCommandAsync(command, lights != null ? lights : _allLights);
            
            return true;
        }

        public async Task<bool> SetColor(string hexColor, List<string> lights = null)
        {
            var command = new LightCommand();
            command.On = true;
            command.TransitionTime = TimeSpan.FromMilliseconds(0);
            command.Brightness = 255;

            //var color = Color.FromArgb(.FromName(colorName);
            //command.SetColor(color.R, color.G, color.B);
            command.SetColor(hexColor);

            await _client.SendCommandAsync(command, lights != null ? lights : _allLights);

            return true;
        }

        public async Task<bool> ColorLoop(List<string> lights = null)
        {
            var command = new LightCommand();
            command.On = true;
            command.Brightness = 255;
            command.Effect = Effect.ColorLoop;

            await _client.SendCommandAsync(command, lights != null ? lights : _allLights);

            return true;
        }

        public async Task<bool> Blink(List<string> lights = null)
        {
            var command = new LightCommand();
            command.Alert = Alert.Once; // blinks 15 times

            await _client.SendCommandAsync(command, lights != null ? lights : _allLights);

            return true;
        }

        public async Task<bool> Blink2(string hexColor, int duration, List<string> lights = null)
        {
            var dur = TimeSpan.FromSeconds(duration);
            _test = new Timer(dur.TotalMilliseconds);
            _test.Elapsed += (obj, a) => test_Elapsed(lights); 

            var command = new LightCommand();
            command.On = true;
            command.Brightness = 255;
            command.SetColor(hexColor);

            // Enable timer
            _test.Enabled = true;

            await _client.SendCommandAsync(command, lights != null ? lights : _allLights);
            
            
            return true;
        }

        void test_Elapsed(List<string> lights = null)
        {
            _test.Dispose();
            // Shut down lights
            SetPower(false, lights).Wait();            
        }


        public async Task<bool> SetBrightness(string level, List<string> lights = null)
        {
            var command = new LightCommand();
            command.Brightness = Byte.Parse(level);


            await _client.SendCommandAsync(command, lights != null ? lights : _allLights);

            return true;
        }
    }
}
