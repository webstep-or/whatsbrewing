using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Azure;

namespace WhatsBrewing.WBService.Controllers
{
    public class LightsController : ApiController
    {
        //private string _signalRHubURL = "http://localhost:65409/";
        private string _signalRHubURL; //= "http://whatsbrewing.cloudapp.net/";

        public LightsController()
        {
            _signalRHubURL = CloudConfigurationManager.GetSetting("SignalRHubURL");
        }

        //// POST: api/Lights/?color=RED
        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> SetColor([FromUri]string color, [FromUri]string lights = "")
        {
            var ok = false;
            
            var client = new HubClient.HubClient(_signalRHubURL);


            if (!string.IsNullOrEmpty(color))
            {
                ok = await client.InvokeHub("SetColor", color, lights);
            }


            if (ok)
            {
                return Ok();
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> SetPower([FromUri]string state, [FromUri]string lights = "")
        {
            var client = new HubClient.HubClient(_signalRHubURL);

            var ok = await client.InvokeHub("SetPower", state, lights);

            if (ok)
            {
                return Ok();
            }
            else 
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Blink([FromUri]string color, [FromUri]int seconds, [FromUri]string lights = "")
        {
            var client = new HubClient.HubClient(_signalRHubURL);

            var ok = await client.InvokeHub("Blink", color, seconds, lights);

            if (ok)
            {
                return Ok();
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> SetBrightness([FromUri]string level, [FromUri]string lights = "")
        {
            var client = new HubClient.HubClient(_signalRHubURL);

            var ok = await client.InvokeHub("SetBrightness", level, lights);

            if (ok)
            {
                return Ok();
            }
            else
            {
                return InternalServerError();
            }
        }
    }
}