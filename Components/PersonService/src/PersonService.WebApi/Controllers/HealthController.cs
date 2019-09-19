using System;

using Microsoft.AspNetCore.Mvc;

namespace PersonService.WebApi.Controllers
{
    [Route("")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly Func<DateTime> _now;

        public HealthController(Func<DateTime> now)
        {
            _now = now;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public string Ping()
        {
            return $"Pong at {_now():F}";
        }
    }
}
