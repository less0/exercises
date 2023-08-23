using bowling_backend_core.DomainModel;
using bowling_backend_core.Interfaces;

namespace bowling_backend_applicaton.Interfaces;

public interface IRepository<T> 
    where T : Entity, IAggregateRoot
{
    public T GetById(Guid id);

    public void Save(T entity);
}