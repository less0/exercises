namespace bowling_backend_persistence.DataModel;

public class BowlingGame
{
    public Guid Id { get; set; }

    public string[] PlayerNames { get; set; }

    public Frame[] Frames { get; set; }
}