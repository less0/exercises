namespace bowling_backend_persistence.DataModel;

using BowlingGameDomain = bowling_backend_core.DomainModel.BowlingGame;
using FrameDomain = bowling_backend_core.DomainModel.Frame;

public class BowlingGameMapper
{
    public BowlingGame Map(BowlingGameDomain game)
    {
        return new BowlingGame
        {
            Id = game.Id,
            PlayerNames = game.PlayerNames,
            UserId = string.Empty,
            Frames = MapFrames(game),
            StartedAt = game.StartedAt
        };
    }

    private static List<Frame> MapFrames(BowlingGameDomain game)
        => game.Frames.SelectMany((frames, playerIndex) => frames.Select((f, frameIndex) => MapFrame(f, playerIndex, frameIndex)))
                      .ToList();

    private static Frame MapFrame(FrameDomain frame, int playerIndex, int frameIndex)
    {
        return new Frame
        {
            Id = frame.Id,
            PlayerIndex = playerIndex,
            FrameIndex = frameIndex,
            BonusPoints = frame.BonusPoints,
            Rolls = frame.Rolls,
            IsLastFrame = frame.IsLastFrame
        };
    }

    public BowlingGameDomain ReverseMap(BowlingGame game)
    {
        // Since SQL does stores neither the kind nor the timezone, we are always using UTC
        // time and set the kind after loading it.
        DateTime startedAt = DateTime.SpecifyKind(game.StartedAt, DateTimeKind.Utc);
        return BowlingGameDomain.Restore(game.Id, game.PlayerNames, ReverseMapFrames(game.Frames, game.PlayerNames.Length), startedAt);
    }

    private List<FrameDomain>[] ReverseMapFrames(List<Frame> frames, int numberOfPlayers)
    {
        var result = new List<FrameDomain>[numberOfPlayers];
        for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
        {
            result[playerIndex] = new();
        }

        var groupedByPlayerIndex = frames.GroupBy(f => f.PlayerIndex)
                                         .OrderBy(g => g.Key);

        foreach(var framesGroup in groupedByPlayerIndex)
        {
            result[framesGroup.Key] = framesGroup.OrderBy(f => f.FrameIndex)
                                                 .Select(ReverseMapFrame)
                                                 .ToList();
        }

        return result;
    }

    private FrameDomain ReverseMapFrame(Frame frame)
    {
        return FrameDomain.Restore(frame.Id, frame.Rolls, frame.BonusPoints, frame.IsLastFrame);
    }
}