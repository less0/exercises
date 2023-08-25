using bowling_backend_core.DomainModel;

namespace bowling_backend_applicaton;

public class BowlingGameDto
{
    public Guid Id { get; init; }
    public FrameDto[][] Frames { get; init; }
    public string[] PlayerNames { get; init; }
    public string CurrentPlayer { get; init; }
    public bool IsInProgress { get; init; }
    public DateTime StartedAt { get; init; }

    public static explicit operator BowlingGameDto(BowlingGame game)
    {
        return new BowlingGameDto()
        {
            Id = game.Id,
            PlayerNames = game.PlayerNames,
            CurrentPlayer = game.CurrentPlayer,
            IsInProgress = !game.IsFinished,
            StartedAt = DateTime.MinValue, // TODO
            Frames = MapFrames(game.Frames)
        };
    }

    private static FrameDto[][] MapFrames(Frame[][] frames)
    {
        int playerCount = frames.Length;
        FrameDto[][] result = new FrameDto[playerCount][];

        for(int playerIndex = 0; playerIndex < frames.Length; playerIndex++)
        {
            var framesCountForPlayer = frames[playerIndex].Length;
            result[playerIndex] = new FrameDto[framesCountForPlayer];
            var cumulativeScore = 0;

            for(int frameIndex = 0; frameIndex < framesCountForPlayer; frameIndex++)
            {
                var frame = frames[playerIndex][frameIndex];
                cumulativeScore += frame.Score;
                var mappedFrame = (FrameDto)frame;
                mappedFrame.CumulativeScore = cumulativeScore;

                result[playerIndex][frameIndex] = (FrameDto)frames[playerIndex][frameIndex];
            }
        }

        return result;
    }
}