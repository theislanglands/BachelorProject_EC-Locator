namespace EC_locator.Core.Models;

public class Employee
{
    public string Name { get; set; }
    public string Id { get; set; }

    public Employee(string name, string id)
    {
        Name = name;
        Id = id;
    }
}