using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PrescriptionActions : IStorageAction<Prescription>
{
    public void OnDelete(Prescription item)
    {
        item.Appointment.Prescriptions.Remove(item.GetHashCode());
    }

    public void OnAdd(Prescription item)
    {
        item.Appointment.Prescriptions.Add(item.GetHashCode(), item);
    }
}
