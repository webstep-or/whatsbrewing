using System.Web.Http;

namespace WhatsBrewing.WBService.Controllers
{
    public class BeaconsController : ApiController
    {
        public IHttpActionResult GetAccuracy()
        {
            return Ok(new {  B = "0.05", W1 = "2", W2 = "2" });
        }
    }
}
