namespace bowling_backend_applicaton.Interfaces;

public interface IBowlingCommands
{
    public void StartGame(string userId, string[] players);

    public void Roll(string userId, Guid gameId, int numberOfPins);
}