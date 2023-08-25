using bowling_backend_applicaton.Interfaces;
using bowling_backend_core.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace bowling_backend_persistence;

public class BowlingGameRepository : IRepository<BowlingGame>
{
    private readonly BowlingDataContext _dataContext;
    private readonly DataModel.BowlingGameMapper _mapper;

    public BowlingGameRepository(BowlingDataContext dataContext)
    {
        _dataContext = dataContext;
        _mapper = new DataModel.BowlingGameMapper();
    }

    public void Save(BowlingGame game, string userId)
    {
        var gameData = _mapper.Map(game);
        gameData.UserId = userId;

        var existingGame = _dataContext.Set<DataModel.BowlingGame>().FirstOrDefault(g => g.Id == game.Id && g.UserId == userId);

        if(existingGame == null)
        {
            _dataContext.Add(gameData);
        }
        else
        {
            UpdateFrames(gameData);

            _dataContext.Entry(existingGame).State = EntityState.Detached;
            _dataContext.Attach(gameData);
            _dataContext.Entry(gameData).State = EntityState.Modified;
        }

        _dataContext.SaveChanges();
    }

    public BowlingGame[] GetAllByUserId(string userId) => _dataContext.Set<DataModel.BowlingGame>()
                                                                      .Where(g => g.UserId == userId)
                                                                      .Include(g => g.Frames)
                                                                      .Select(_mapper.ReverseMap)
                                                                      .ToArray();

    public BowlingGame GetByEntityIdAndUserId(Guid gameId, string userId)
    {
        var gameData = _dataContext.Set<DataModel.BowlingGame>()
                                   .Where(g => g.Id == gameId && g.UserId == userId)
                                   .Include(g => g.Frames)
                                   .SingleOrDefault();
        return _mapper.ReverseMap(gameData);
    }

    private void UpdateFrames(DataModel.BowlingGame gameData)
    {
        foreach(var frameData in gameData.Frames)
        {
            var existingFrame = _dataContext.Set<DataModel.Frame>().FirstOrDefault(f => f.Id == frameData.Id);
            if(existingFrame == null)
            {
                _dataContext.Add(frameData);
            }
            else
            {
                _dataContext.Entry(existingFrame).State = EntityState.Detached;
                _dataContext.Attach(frameData);
                _dataContext.Entry(frameData).State = EntityState.Modified;
            }
        }
    }
}