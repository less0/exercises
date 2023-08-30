using editable_table_backend.Model;

namespace editable_table_backend.Persistence;

public class InMemoryDepartmentsRepository : IDepartmentsRepository
{
    private List<Department> _departments = new();

    public void Add(Department department) => _departments.Add(department);
    public IEnumerable<Department> GetAll() => _departments;
}