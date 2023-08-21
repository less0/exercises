using System.Collections.ObjectModel;

namespace bowling_backend_api.Model;

public class GameDetails
{
    public string Id { get; init; }

    public List<string> Players { get; init; }

    public DateTime StartedAt { get; init; }

    public Frame[][] Frames { get; init; }

    public bool IsInProgress { get; init; }

    public ReadOnlyDictionary<string, string> Links
    {
        get
        {
            Dictionary<string, string> result = new()
            {
                ["self"] = $"/games/{Id}"
            };

            if(IsInProgress)
            {
                result.Add("roll", $"/games/{Id}/roll");
            }

            return new(result);
        }
    }
}