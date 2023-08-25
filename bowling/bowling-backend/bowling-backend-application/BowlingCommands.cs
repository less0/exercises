using bowling_backend_applicaton.Interfaces;
using bowling_backend_core.DomainModel;

namespace bowling_backend_applicaton;

public class BowlingCommands : IBowlingCommands
{
    IRepository<BowlingGame> _repository;

    public BowlingCommands(IRepository<BowlingGame> repository)
    {
        _repository = repository;
    }

    public void Roll(string userId, Guid gameId, int numberOfPins)
    {
        var game = _repository.GetByEntityIdAndUserId(gameId, userId);
        game.AddRoll(numberOfPins);
        _repository.Save(game, userId);
    }

    public void StartGame(string userId, string[] players)
    {
        var game = BowlingGame.StartNew(players);
        _repository.Save(game, userId);
    }
}