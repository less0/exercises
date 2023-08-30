using editable_table_backend.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace editable_table_backend.Controllers;

[ApiController]
public class DepartmentsController : Controller
{
    private readonly IDepartmentsRepository _repository;

    public DepartmentsController(IDepartmentsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("/departments")]
    public IActionResult GetAllDepartments() => Ok(_repository.GetAll());
}