using Microsoft.AspNetCore.Mvc;
using System;

namespace DemoPartnerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {
            var model = new
            {
                Value = "pong",
                Timestamp = DateTime.UtcNow,
                Server = Environment.MachineName,
                OS = Environment.OSVersion
            };

            return model;
        }
    }
}
