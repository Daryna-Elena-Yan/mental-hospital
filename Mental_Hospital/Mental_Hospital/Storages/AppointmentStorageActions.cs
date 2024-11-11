using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class AppointmentStorageActions : IStorageAction<Appointment>
{
    private readonly Storage<Person> _personStorage;
    public AppointmentStorageActions(Storage<Person> personStorage)
    {
        _personStorage = personStorage;
    }
    public void OnDelete(Appointment appointment)
    {
        appointment.Therapist.Appointments.Remove(appointment);
        
        if(appointment.Patient is not null)
            appointment.Patient.Appointments.Remove(appointment);
        
        foreach (Guid qualifier in appointment.Prescriptions.Keys)
        {
            appointment.Prescriptions[qualifier].Appointment = null;
        }
    }

    public void OnAdd(Appointment item)
    {
        item.Therapist.Appointments.Add(item);
        if(item.Patient is not null)
            item.Patient.Appointments.Add(item);
    }

    public void OnRestore(Appointment item)
    {
        var therapist = _personStorage.FindBy(x => x.IdPerson == item.IdTherapist).First() as Therapist;
        item.Therapist = therapist!;
        therapist?.Appointments.Add(item);
        
        var patient = _personStorage.FindBy(x => x.IdPerson == item.IdPatient).First() as Patient;
        if (patient is not null)
        {
            item.Patient = patient;
            patient.Appointments.Add(item);
        }
    }
    
}