using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auth_API.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // private readonly ILogger<AuthController> _logger;

        // public AuthController(ILogger<AuthController> logger)
        // {
        //     _logger = logger;
        // }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            return Ok(Summaries);
        }
    }
}