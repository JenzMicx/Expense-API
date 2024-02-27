using Auth_API.Model.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    [Route("Get")]
    public IActionResult Get()
    {
        return Ok(Summaries);
    }

    [HttpGet]
    [Route("User-Roles")]
    [Authorize(Roles = UserRoles.USER)]
    public IActionResult GetUserRoles()
    {
        return Ok(Summaries);
    }

    [HttpGet]
    [Route("Admin-Roles")]
    [Authorize(Roles = UserRoles.ADMIN)]
    public IActionResult GetAdminRoles()
    {
        return Ok(Summaries);
    }

    [HttpGet]
    [Route("Owner-Roles")]
    [Authorize(Roles = UserRoles.OWNER)]
    public IActionResult GetOwnerRoles()
    {
        return Ok(Summaries);
    }

}
