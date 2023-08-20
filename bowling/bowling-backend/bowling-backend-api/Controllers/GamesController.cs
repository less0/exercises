using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using bowling_backend_api.Model;

namespace bowling_backend_api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GamesController : Controller
{
    [HttpGet("/games")]
    public IActionResult Games()
    {
        List<GameSummary> allGames = new()
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                IsInProgress = false,
                StartedAt = DateTime.Now.AddDays(-2),
                NumberOfPlayers = 3
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                StartedAt = DateTime.Now.AddDays(-1),
                NumberOfPlayers = 4,
                IsInProgress = false,
            },
            new() 
            {
                Id = Guid.NewGuid().ToString(),
                StartedAt = DateTime.Now.AddHours(-1),
                NumberOfPlayers = 4,
                IsInProgress = Random.Shared.Next() % 2 == 0
            }
        };

        GamesSummaries games = new()
        {
            Games = new(allGames)
        };

        return Ok(games);
    }

    [HttpGet("/games/{id}")]
    public IActionResult GetGame(string id)
    {
        GameDetails details = new()
        {
            Id = id,
            Players = new List<string> { "Peter", "Paul", "Mary" },
            StartedAt = DateTime.Now.AddHours(-1),
            IsInProgress = true,
            Frames = new Frame[3][]
            {
                new Frame[]{ new Frame(){Rolls = new int[]{1,2}, TotalScore = 3}, new Frame(){Rolls = new int[]{10}, TotalScore = 10} },
                new Frame[]{ new Frame(){Rolls = new int[]{10}, TotalScore = 10}, new Frame(){Rolls = new int[]{9,0}, BonusPoints = 9, TotalScore = 19} },
                new Frame[]{ new Frame(){ Rolls = new int[]{0,3}, TotalScore = 3}, new Frame(){Rolls = new int[]{2, 6}, TotalScore = 8}},
            }
        };
        return Ok(details);
    }
}