﻿using Mental_Hospital.Models;

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
            item.Appointment.Prescriptions.Remove(item.GetHashCode());
    }

    public void OnAdd(Prescription item)
    {
        if(item.Appointment is not null)
            item.Appointment.Prescriptions.Add(item.GetHashCode(), item);
        else
        {
            foreach (var appointment in _appointmentStorage.GetList())
            {
                if (appointment.IdAppointment.Equals(item.IdAppointment))
                {
                    item.Appointment = appointment;
                    appointment.Prescriptions.Add(item.GetHashCode(), item);
                }
            }
        }
    }
}
