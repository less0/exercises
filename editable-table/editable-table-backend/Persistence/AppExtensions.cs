using editable_table_backend.Model;

namespace editable_table_backend.Persistence;

static class AppExtensions
{
    public static void CreateDummyData(this WebApplication app)
    {
        var departmentsRepository = app.Services.GetRequiredService<IDepartmentsRepository>();
        var personsRepository = app.Services.GetRequiredService<IPersonsRepository>();

        departmentsRepository.Add(new Department()
        {
            Name = "MCU"
        });
        departmentsRepository.Add(new Department()
        {
            Name = "DCEU"
        });
        departmentsRepository.Add(new Department
        {
            Name = "Game of Thrones"
        });

        personsRepository.Add(new Person
        {
            LastName = "Parker",
            FirstName = "Peter",
            Department = new()
            {
                Name = "MCU"
            },
            DateOfBirth = new DateTime(2001, 8, 10),
        });
        personsRepository.Add(new Person
        {
            LastName = "Banner",
            FirstName = "Bruce",
            Department = new()
            {
                Name = "MCU"
            },
            DateOfBirth = new DateTime(1969, 12, 18)
        });
        personsRepository.Add(new Person()
        {
            LastName = "Stark",
            FirstName = "Anthony Edward",
            Department = new()
            {
                Name = "MCU"
            },
            DateOfBirth = new(1970, 5, 29)
        });
        personsRepository.Add(new()
        {
            LastName = "Wayne",
            FirstName = "Bruce",
            Department = new()
            {
                Name = "DCEU"
            },
            DateOfBirth = new(1972, 2, 19)
        });
        personsRepository.Add(new()
        {
            LastName = "Kent",
            FirstName = "Clark Joseph",
            Department = new()
            {
                Name = "DCEU"
            },
            DateOfBirth = new(1980, 2, 29)
        });
    }
}