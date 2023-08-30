using editable_table_backend.Model;
using editable_table_backend.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace editable_table_backend.Controllers;

[ApiController]
public class PersonsController : Controller
{
    IPersonsRepository _repository;

    public PersonsController(IPersonsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("/persons")]
    public IActionResult GetAllPersons() => Ok(_repository.GetAll());

    [HttpPost("/persons")]
    public IActionResult PostNewPerson(Person person)
    {
        _repository.Add(person);
        return Ok();
    }

    [HttpPut("/persons/{id}")]
    public IActionResult UpdatePerson(string id, Person person)
    {
        if(_repository.GetAll().Any(p => p.Id == id))
        {
            _repository.Update(person);
        }
        else
        {
            _repository.Add(person);
        }

        return Ok();
    }
}