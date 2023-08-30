using editable_table_backend.Model;

namespace editable_table_backend.Persistence;

public interface IDepartmentsRepository
{
    public IEnumerable<Department> GetAll();

    public void Add(Department department);
}