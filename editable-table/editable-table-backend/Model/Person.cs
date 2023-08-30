using System.Collections.ObjectModel;

namespace editable_table_backend.Model;

public class Person
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Department? Department { get; init; }
    public DateTime DateOfBirth { get; init; }

    public ReadOnlyDictionary<string, string> Links 
    {
        get
        {
            Dictionary<string, string> result = new()
            {
                ["self"] = $"/persons/{Id}"
            };
            return new(result);
        }
    }
}