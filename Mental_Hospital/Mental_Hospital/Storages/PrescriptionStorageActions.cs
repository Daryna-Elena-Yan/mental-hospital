using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PrescriptionStorageActions : IStorageAction<Prescription>
{
    private readonly Storage<Appointment> _appointmentStorage;
    public PrescriptionStorageActions(Storage<Appointment> appointmentStorage)
    {
        _appointmentStorage = appointmentStorage;
    }
    public void OnDelete(Prescription item)
    {
        if(item.Appointment is not null)
            item.Appointment.Prescriptions.Remove(item.IdPrescription);
    }

    public void OnAdd(Prescription item)
    {
        if(item.Appointment is not null)
            item.Appointment.Prescriptions.Add(item.IdPrescription, item);
    }

    public void OnRestore(Prescription item)
    {
        var appointment = _appointmentStorage.FindBy(x => x.IdAppointment == item.IdAppointment).First();
        item.Appointment = appointment;
        appointment.Prescriptions.Add(item.IdPrescription, item);
    }


    
}