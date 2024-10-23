using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class AppointmentAction : IStorageAction<Appointment>
{
    public void OnDelete(Appointment item)
    {
        item.Therapist.Appointments.Remove(item);
        item.Patient.Appointments.Remove(item);
    }

    public void OnAdd(Appointment item)
    {
        item.Therapist.Appointments.Add(item);
        item.Patient.Appointments.Add(item);
    }
}