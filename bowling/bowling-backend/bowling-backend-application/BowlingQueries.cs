using bowling_backend_applicaton.Interfaces;
using bowling_backend_core.DomainModel;

namespace bowling_backend_applicaton;

public class BowlingQueries : IBowlingQueries
{
    IRepository<BowlingGame> _repository;

    public BowlingQueries(IRepository<BowlingGame> repository)
    {
        _repository = repository;
    }

    public IEnumerable<BowlingGameDto> GetAllGames(string userId)
    {
        var games = _repository.GetAllByUserId(userId);
        return games.Select(g => (BowlingGameDto)g);
    }

    public BowlingGameDto GetGameById(string userId, Guid gameId)
    {
        return (BowlingGameDto)_repository.GetByEntityIdAndUserId(gameId, userId);
    }
}