using editable_table_backend.Model;

namespace editable_table_backend.Persistence;

public interface IPersonsRepository
{
    public IEnumerable<Person> GetAll();
    public void Add(Person person);
    public void Update(Person person);
}