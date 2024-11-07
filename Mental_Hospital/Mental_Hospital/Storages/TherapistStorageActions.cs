using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class TherapistStorageActions:IStorageAction<Therapist>
{
    private readonly Storage<Appointment> _approintmentStorage;
    private readonly Storage<Therapist> _therapistStorage;

    public TherapistStorageActions(Storage<Appointment> approintmentStorage,Storage<Therapist> therapist)
    {
        this._approintmentStorage = approintmentStorage;
        _therapistStorage = therapist;
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
        if(item.Supervisor is not null)
            item.Supervisor.Subordinates.Add(item);
        else
        {
            foreach (var person in _therapistStorage.GetList())
            {
                if (person.IdPerson.Equals(item.IdSupervisor))
                {
                    item.Supervisor = person;
                    person.Subordinates.Add(item);
                }
            }
        }
    }
}