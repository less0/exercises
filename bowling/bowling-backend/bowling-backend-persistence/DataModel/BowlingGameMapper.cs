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
            Frames = MapFrames(game)
        };
    }

    private static List<Frame> MapFrames(BowlingGameDomain game)
        => game.Frames.SelectMany((frames, playerIndex) => frames.Select(f => MapFrame(f, playerIndex)))
                      .ToList();

    private static Frame MapFrame(FrameDomain frame, int playerIndex)
    {
        return new Frame
        {
            Id = frame.Id,
            PlayerIndex = playerIndex,
            BonusPoints = frame.BonusPoints,
            Rolls = frame.Rolls,
            IsLastFrame = frame.IsLastFrame
        };
    }

    public BowlingGameDomain ReverseMap(BowlingGame game)
    {
        return BowlingGameDomain.Restore(game.Id, game.PlayerNames, ReverseMapFrames(game.Frames, game.PlayerNames.Length));
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
            result[framesGroup.Key] = framesGroup.Select(ReverseMapFrame)
                                                 .ToList();
        }

        return result;
    }

    private FrameDomain ReverseMapFrame(Frame frame)
    {
        return FrameDomain.Restore(frame.Id, frame.Rolls, frame.BonusPoints, frame.IsLastFrame);
    }
}