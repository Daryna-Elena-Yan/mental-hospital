using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class AppointmentActions : IStorageAction<Appointment>
{
    public void OnDelete(Appointment item)
    {
        item.Therapist.Appointments.Remove(item);
        if(item.Patient is not null)
            item.Patient.Appointments.Remove(item);
    }

    public void OnAdd(Appointment item)
    {
        item.Therapist.Appointments.Add(item);
        if(item.Patient is not null)
            item.Patient.Appointments.Add(item);
    }
}