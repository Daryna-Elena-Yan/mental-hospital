using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class AppointmentStorageActions : IStorageAction<Appointment>
{
    private readonly Storage<Patient> _patientStorage;
    private readonly Storage<Therapist> _therapistStorage;


    public AppointmentStorageActions(Storage<Patient> personStorage,Storage<Therapist> s)
    {
        _patientStorage = personStorage;
        _therapistStorage = s;
    }
    public void OnDelete(Appointment appointment)
    {
        appointment.Therapist.Appointments.Remove(appointment);
        
        if(appointment.Patient is not null)
            appointment.Patient.Appointments.Remove(appointment);
        
        foreach (int hash in appointment.Prescriptions.Keys)
        {
            appointment.Prescriptions[hash].Appointment = null;
        }
    }

    public void OnAdd(Appointment item)
    {
        if (item.Therapist is not null)
        {
            item.Therapist.Appointments.Add(item);
        }
        else
        {
            foreach (var person in _therapistStorage.GetList())
            {
                if (person.IdPerson.Equals(item.IdTherapist))
                { 
                    item.Therapist = person;
                    person.Appointments.Add(item);
                }
            }
        }
        if(item.Patient is not null)
            item.Patient.Appointments.Add(item);
        else
        {
            foreach (var person in _patientStorage.GetList())
            {
                if (person.IdPerson.Equals(item.IdPatient))
                {
                    item.Patient = person;
                    person.Appointments.Add(item);
                }
            }
        }
    }
}