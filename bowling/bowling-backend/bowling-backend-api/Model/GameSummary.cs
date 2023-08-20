using System.Collections.ObjectModel;

public class GameSummary
{
    public string Id { get; init; }

    public DateTime StartedAt { get; init; }

    public int NumberOfPlayers { get; init; }

    public bool IsInProgress { get; init; }

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