using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PrescriptionStorageActions : IStorageAction<Prescription>
{
    public void OnDelete(Prescription item)
    {
        if(item.Appointment is not null)
            item.Appointment.Prescriptions.Remove(item.GetHashCode());
    }

    public void OnAdd(Prescription item)
    {
        if(item.Appointment is not null)
            item.Appointment.Prescriptions.Add(item.GetHashCode(), item);
    }

    public void OnRestore(Prescription item)
    {
        throw new NotImplementedException();
    }

    public void OnDeserialize(Prescription item)
    {
       
    }
    
}