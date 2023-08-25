using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using bowling_backend_api.Model;
using bowling_backend_applicaton.Interfaces;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace bowling_backend_api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GamesController : Controller
{
    private readonly IBowlingCommands _commands;
    private readonly IBowlingQueries _queries;

    public GamesController(IBowlingCommands commands, IBowlingQueries queries)
    {
        _commands = commands;
        _queries = queries;
    }

    private string UserId => User.FindFirstValue("sub");

    [HttpGet("/games")]
    public IActionResult Games()
    {
        var games = _queries.GetAllGames(UserId);

        GamesSummaries result = new()
        {
            Games = new ReadOnlyCollection<GameSummary>(games.Select(g => (GameSummary)g).ToList())
        };

        return Ok(games);
    }

    [HttpGet("/games/{id}")]
    public IActionResult GetGame(string id)
    {
        var game = _queries.GetGameById(UserId, Guid.Parse(id));
        return Ok((GameDetails)game);
    }

    [HttpPost("/games/{id}/roll")]
    public IActionResult AddRoll(string id, [FromBody] Roll roll)
    {
        _commands.Roll(UserId, Guid.Parse(id), roll.Pins);
        return Ok();
    }

    [HttpPost("/games/start")]
    public IActionResult StartGame([FromBody] string[] playerNames)
    {
        _commands.StartGame(UserId, playerNames);
        return Ok();
    }
}