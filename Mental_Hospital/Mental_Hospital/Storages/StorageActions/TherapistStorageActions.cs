using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class TherapistStorageActions : IStorageAction<Therapist>
{
    private readonly Storage<Appointment> _approintmentStorage;
    private readonly Storage<Person> _personStorage;

    public TherapistStorageActions(Storage<Appointment> approintmentStorage,Storage<Person> personStorage)
    {
        this._approintmentStorage = approintmentStorage;
        _personStorage = personStorage;
    }

    public void OnDelete(Therapist item)
    {

        foreach (var patient in item.Patients.ToList())
        {
            patient.Therapists.Remove(item);
        }

        foreach (var employee in item.Subordinates.ToList())
        {
            employee.Supervisor = null;
        }

        item.Supervisor?.Subordinates.Remove(item);
        foreach (var appointment in item.Appointments.ToList())
        {
            foreach (var prescription in appointment.Prescriptions.Values.ToList())
            {
                prescription.Appointment = null;
            }

            _approintmentStorage.Delete(appointment);
        }
    }

    public void OnAdd(Therapist item)
    {
        if (item.Supervisor != null)
        {
            item.Supervisor.Subordinates.Add(item);
        }
    }

    public void OnRestore(Therapist item)
    {
        var foundPerson = _personStorage.FindBy(x => x.Id == item.IdSupervisor).FirstOrDefault();
        if (foundPerson is not null)
        {
            var supervisor = foundPerson as Therapist;
            item.Supervisor = supervisor;
            supervisor.Subordinates.Add(item);
            
        }

    }
}
    
