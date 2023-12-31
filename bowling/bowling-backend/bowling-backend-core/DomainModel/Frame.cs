namespace bowling_backend_core.DomainModel;

public class Frame : Entity
{
    private readonly List<int> _rolls = new();
    private readonly bool _isLastFrame;
    private int _bonusPoints;

    public static Frame CreateFrame(int pinsWithFirstRoll) => new(pinsWithFirstRoll);
    public static Frame CreateLastFrame(int pinsWithFirstRoll) => new(pinsWithFirstRoll, true);
    public static Frame Restore(Guid id, int[] rolls, int bonusPoints, bool isLastFrame) => new(id, rolls, bonusPoints, isLastFrame);

    private Frame(int pinsWithFirstRoll, bool isLastFrame = false)
    {
        ThrowIfInvalidRoll(pinsWithFirstRoll);
        _rolls.Add(pinsWithFirstRoll);
        _isLastFrame = isLastFrame;
    }

    private Frame(Guid id, int[] rolls, int bonusPoints, bool isLastFrame)
        : base(id)
    {
        _rolls = new(rolls);
        _bonusPoints = bonusPoints;
        _isLastFrame = isLastFrame;
    }

    public int[] Rolls => _rolls.ToArray();
    public bool IsFinished => IsNotLastFrameAndFinished || IsLastFrameFinished;
    public int Score => _rolls.Sum() + BonusPoints;
    public int BonusPoints { get => _bonusPoints; private set => _bonusPoints = value; }
    public bool IsLastFrame => _isLastFrame;
    private bool IsNotLastFrameAndFinished => !IsLastFrame && (_rolls.Count == 1 && _rolls[0] == 10 || _rolls.Count == 2);
    private bool IsLastFrameFinished => IsLastFrame && (((IsStrike || IsSpare) && _rolls.Count == 3) || !IsStrike && !IsSpare && _rolls.Count == 2);
    private bool IsSpare => _rolls.Count > 1 && _rolls.Take(2).Sum() == 10;
    private bool IsStrike => _rolls[0] == 10;

    public void AddRoll(int pins)
    {
        ThrowIfInvalidRoll(pins);
        _rolls.Add(pins);
    }

    public void AddBonusPoints(Frame nextFrame)
    {
        ThrowOnAddBonusPointsNotPossible(nextFrame);

        if(IsSpare)
        {
            BonusPoints = nextFrame.Rolls[0];
        }
        else if(IsStrike)
        {
            BonusPoints = nextFrame.Rolls.Take(2).Sum();
        }
    }

    private void ThrowOnAddBonusPointsNotPossible(Frame nextFrame)
    {
        if(IsLastFrame)
        {
            throw new InvalidOperationException();
        } 

        if(!IsFinished)
        {
            throw new InvalidOperationException();
        }

        if(!nextFrame.IsFinished)
        {
            throw new ArgumentException(string.Empty, nameof(nextFrame));
        }
    }

    private void ThrowIfInvalidRoll(int pins)
    {
        if (IsFinished)
        {
            throw new InvalidOperationException();
        }

        if(pins < 0)
        {
            throw new ArgumentException("Number of pins may not be null.", nameof(pins));
        }

        if (ExceedsMaximumPinsPerRoll(pins))
        {
            throw new ArgumentException(string.Empty, nameof(pins));
        }

        if (ExceedsMaximumPinsPerFrame(pins))
        {
            throw new InvalidOperationException();
        }
    }

    private static bool ExceedsMaximumPinsPerRoll(int pins) => pins > 10;

    private bool ExceedsMaximumPinsPerFrame(int pins) => !IsLastFrame && ExceedsMaximumPinsForNonLastFrame(pins);

    private bool ExceedsMaximumPinsForNonLastFrame(int pins) => _rolls.Any() ? _rolls[0] + pins > 10 : pins > 10;
}