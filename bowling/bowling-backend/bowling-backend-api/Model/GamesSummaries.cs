using System.Collections.ObjectModel;

namespace bowling_backend_api.Model;

public class GamesSummaries
{
    public ReadOnlyCollection<GameSummary> Games { get; init; }

    /// <summary>
    /// This property returns the links to implement (at least a basic) HATEOAS for the /games 
    /// endpoint. Instead of hard coding the links and implement the logic to determine whether 
    /// a new game can be started for instance, the frontend application can check whether "start"
    /// is available and follow the respective link, which decouples the frontend from the exact
    /// routes implemented by the backend.
    /// </summary>
    public ReadOnlyDictionary<string, string> Links { 
        get
        {
            Dictionary<string, string> result = new();
            if(Games.All(g => !g.IsInProgress))
            {
                result.Add("start", "/games/start");
            }

            return new(result);
        }
    }
}