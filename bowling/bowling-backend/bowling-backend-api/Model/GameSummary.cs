using System.Collections.ObjectModel;
using bowling_backend_applicaton;

namespace bowling_backend_api.Model;

public class GameSummary
{
    public string Id { get; init; }

    public DateTime StartedAt { get; init; }

    public int NumberOfPlayers { get; init; }

    public bool IsInProgress { get; init; }

    public static explicit operator GameSummary(BowlingGameDto dto)
    {
        return new GameSummary()
        {
            Id = dto.Id.ToString(),
            StartedAt = dto.StartedAt,
            NumberOfPlayers = dto.PlayerNames.Length,
            IsInProgress = dto.IsInProgress
        };
    }

    /// <summary>
    /// This property returns the links to implement HATEOAS for the /games endpoint. Clients
    /// can follow these links rather than hard code where to find the games details.
    /// </summary>
    public ReadOnlyDictionary<string, string> Links 
    {
        get
        {
            Dictionary<string, string> result = new()
            {
                ["details"] = $"/games/{Id}"
            };
            return new(result);
        }
    }
}