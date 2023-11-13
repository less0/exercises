using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace beers_of_belgium.bob_backend.Controllers;

public class UsersController : Controller
{
    [HttpGet("users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetUsers()
    {
        return Ok();
    }
}