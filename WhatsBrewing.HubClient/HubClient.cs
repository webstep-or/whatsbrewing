using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace WhatsBrewing.HubClient
{
    public class HubClient
    {
        private string _url = "http://localhost:65409/";
        private string _hubName = "whatsbrewinghub";
        //private string _hubUsr;
        //private string _hubPwd;

        HubConnection _connection;
        IHubProxy _proxy;

        public HubClient(string hubUrl)
        {
            _url = hubUrl;            
        }

        public event Action<Exception> OnConnectionError;
        //public event Action<string> OnSetColor;

        public async Task<bool> Connect() 
        {
            _connection = new HubConnection(_url);   
            _proxy = _connection.CreateHubProxy(_hubName);
            await _connection.Start().ContinueWith(p =>
            {
                if (p.IsFaulted)
                {
                    OnConnectionError(p.Exception.GetBaseException());
                }
            });

            return true;
        }


        public void HandleBlink(string command, Action<string,int,string> handler)
        {
            _proxy.On<string, int, string>(command, (color, duration, lights) => handler(color, duration, lights));
        }

        public void HandleCommand(string command, Action<string,string> handler)
        {            
            _proxy.On<string,string>(command, (arg, lights) => handler(arg, lights));            
        }

        public async Task<bool> InvokeHub(string method, params object[] args)
        {
            using (var connection = new HubConnection(_url))
            {
                //connection.Credentials = new NetworkCredential() { UserName = _hubUsr, Password = _hubPwd };

                //if (!string.IsNullOrEmpty(globalId))
                //{
                //    connection.Headers.Add("globalId", globalId);
                //}

                var hub = connection.CreateHubProxy(_hubName);

                await connection.Start();

                await hub.Invoke(method, args);
            }

            return true;
        }
    }
}
