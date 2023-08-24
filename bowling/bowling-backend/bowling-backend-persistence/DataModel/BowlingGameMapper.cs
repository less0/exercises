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
    {
        return game.Frames.SelectMany((frames, playerIndex) => frames.Select(f => MapFrame(f, playerIndex)))
                          .ToList();
    }

    private static Frame MapFrame(FrameDomain frame, int playerIndex)
    {
        return new Frame
        {
            Id = frame.Id,
            PlayerIndex = playerIndex
        };
    }
}