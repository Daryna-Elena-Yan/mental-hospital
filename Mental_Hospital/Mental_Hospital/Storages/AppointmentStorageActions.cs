using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class AppointmentStorageActions : IStorageAction<Appointment>
{
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
        item.Therapist.Appointments.Add(item);
        if(item.Patient is not null)
            item.Patient.Appointments.Add(item);
    }
}