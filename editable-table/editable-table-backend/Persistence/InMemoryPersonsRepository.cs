using editable_table_backend.Model;

namespace editable_table_backend.Persistence;

/// <summary>
/// Since the backend is a mere stub, we do not really care for persistence
/// between runs of the API, therefor the easiest way to implement the API 
/// was to introduce in memory repositories. 
/// </summary>
public class InMemoryPersonsRepository : IPersonsRepository
{
    public Dictionary<string, Person> _persons = new();

    public void Add(Person person) => _persons.Add(person.Id, person);
    public IEnumerable<Person> GetAll() => _persons.Values;
    public void Update(Person person) => _persons[person.Id] = person;
}