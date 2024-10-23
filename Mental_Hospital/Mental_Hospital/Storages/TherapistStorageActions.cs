using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class TherapistStorageActions:IStorageAction<Therapist>
{
    private readonly Storage<Appointment> _approintmentStorage;
    private readonly Storage<Prescription> _prescriptionStorage;
    public void OnDelete(Therapist item)
    {
        foreach (var appointment in item.Appointments.ToList())
        {
            for (int i = 0; i < appointment.Prescriptions.Count; i++)
            {
                appointment.Prescriptions[i].Appointment = null;
            }
            _approintmentStorage.Delete(appointment);
        }
        foreach (var patient in item.Patients)
        {
            patient.Therapists.Remove(item);
        }
    }

    public void OnAdd(Therapist item)
    {
    }
}