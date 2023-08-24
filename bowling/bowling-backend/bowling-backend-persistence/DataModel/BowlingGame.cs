namespace bowling_backend_persistence.DataModel;

public class BowlingGame
{
    public Guid Id { get; set; }

    public string UserId { get; set; }

    public string[] PlayerNames { get; set; }

    public List<Frame> Frames { get; set; }
}