
namespace bowling_backend_api.Model;

public class Frame
{
    public int[] Rolls { get; init; }

    public int BonusPoints { get; init; }

    public int TotalScore { get; init; }
}