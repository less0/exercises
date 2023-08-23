using bowling_backend_applicaton.Interfaces;
using bowling_backend_core.DomainModel;

namespace bowling_backend_persistence;

public class BowlingGameRepository : IRepository<BowlingGame>
{
    public void Save(BowlingGame entity, string userId)
    {
        throw new NotImplementedException();
    }

    public BowlingGame[] GetAllByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public BowlingGame GetByEntityIdAndUserId(Guid gameId, string userId)
    {
        throw new NotImplementedException();
    }
}