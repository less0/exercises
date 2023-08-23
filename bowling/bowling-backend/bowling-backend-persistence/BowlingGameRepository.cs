using bowling_backend_applicaton.Interfaces;
using bowling_backend_core.DomainModel;

namespace bowling_backend_persistence;

public class BowlingGameRepository : IRepository<BowlingGame>
{
    public BowlingGame GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Save(BowlingGame entity)
    {
        throw new NotImplementedException();
    }
}