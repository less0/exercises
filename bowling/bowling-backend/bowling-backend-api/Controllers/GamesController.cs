using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}