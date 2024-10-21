using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PersonStorage
{
    private List<Person> _persons = [];
    

    public void RegisterNewPerson(Person person)
    {
        _persons.Add(person);
    }

    public int Count => _persons.Count;
}