namespace bowling_backend_core.DomainModel;

public class Frame
{
    private readonly List<int> _rolls;

    public static Frame StartFrame(int pinsWithFirstRoll) => new(pinsWithFirstRoll);

    private Frame(int pinsWithFirstRoll)
    {
        _rolls = new() { pinsWithFirstRoll };
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public int[] Rolls => _rolls.ToArray();
    public bool IsFinished => _rolls.Count == 1 && _rolls[0] == 10 || _rolls.Count == 2;
    public bool IsStrike => _rolls[0] == 10;
    public bool IsSpare => _rolls.Sum() == 10;
    public int Score => _rolls.Sum();

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