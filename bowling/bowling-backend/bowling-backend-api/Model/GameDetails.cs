using System.Collections.ObjectModel;
using bowling_backend_applicaton;

namespace bowling_backend_api.Model;

public class GameDetails
{
    public string Id { get; init; }

    public List<string> Players { get; init; }

    public string CurrentPlayer { get; init; }

    public DateTime StartedAt { get; init; }

    public Frame[][] Frames { get; init; }

    public bool IsInProgress { get; init; }

    public static explicit operator GameDetails(BowlingGameDto dto)
    {
        return new()
        {
            Id = dto.Id.ToString(),
            Players = dto.PlayerNames.ToList(),
            CurrentPlayer = dto.CurrentPlayer,
            StartedAt = dto.StartedAt,
            Frames = MapFrames(dto.Frames),
            IsInProgress = dto.IsInProgress
        };
    }

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

    private static Frame[][] MapFrames(FrameDto[][] frames)
    {
        var numberOfPlayers = frames.Length;
        var result = new Frame[numberOfPlayers][];

        for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
        {
            var numberOfFrames = frames[playerIndex].Length;
            result[playerIndex] = new Frame[numberOfFrames];

            for (int frameIndex = 0; frameIndex < numberOfFrames; frameIndex++)
            {
                result[playerIndex][frameIndex] = (Frame)frames[playerIndex][frameIndex];
            }
        }

        return result;
    }
}