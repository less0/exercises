namespace bowling_backend_core.DomainModel;

public class Frame
{
    private readonly List<int> _rolls;
    private readonly int _cumulativeScorePriorToFrame = 0;

    public static Frame FirstFrame(int pinsWithFirstRoll) => new(pinsWithFirstRoll);
    public static Frame MidgameFrame(int pinsWithFirstRoll, int cumulativeScore) => new(pinsWithFirstRoll, cumulativeScore);

    private Frame(int pinsWithFirstRoll)
    {
        _rolls = new() { pinsWithFirstRoll };
    }

    private Frame(int pinsWithFirstRoll, int cumulativeScore)
    {
        _rolls = new() { pinsWithFirstRoll };
        _cumulativeScorePriorToFrame = cumulativeScore;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public int[] Rolls => _rolls.ToArray();
    public bool IsFinished => _rolls.Count == 1 && _rolls[0] == 10 || _rolls.Count == 2;
    public bool IsStrike => _rolls[0] == 10;
    public bool IsSpare => _rolls.Sum() == 10;
    public int Score => _rolls.Sum() + _cumulativeScorePriorToFrame;

    public void AddRoll(int pins)
    {
        ThrowIfInvalidRoll(pins);
        _rolls.Add(pins);
    }

    private void ThrowIfInvalidRoll(int pins)
    {
        if (IsFinished)
        {
            throw new InvalidOperationException();
        }

        if (ExceedsMaximumPinsPerFrame(pins))
        {
            throw new InvalidOperationException();
        }
    }

    private bool ExceedsMaximumPinsPerFrame(int pins) => _rolls[0] + pins > 10;
}