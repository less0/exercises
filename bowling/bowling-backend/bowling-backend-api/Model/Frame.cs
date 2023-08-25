
using bowling_backend_applicaton;

namespace bowling_backend_api.Model;

public class Frame
{
    public int[] Rolls { get; init; }

    public int BonusPoints { get; init; }

    public int TotalScore { get; init; }

    public static explicit operator Frame(FrameDto dto)
    {
        return new Frame
        {
            Rolls = dto.Rolls,
            TotalScore = dto.CumulativeScore,
        };
    } 
}