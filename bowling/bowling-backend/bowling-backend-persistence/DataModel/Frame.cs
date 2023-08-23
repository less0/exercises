namespace bowling_backend_persistence.DataModel;

public class Frame 
{
    public Guid Id { get; set; }

    public int PlayerIndex { get; set; }

    public int[] Rolls { get; set; }

    public bool IsLastFrame { get; set; }

    public int BonusPoints { get; set; }
}