namespace bowling_backend_applicaton.Interfaces;

public interface IBowlingQueries
{
    IEnumerable<BowlingGameDto> GetAllGames(string userId);

    BowlingGameDto GetGameById(string userId, Guid gameId);
}