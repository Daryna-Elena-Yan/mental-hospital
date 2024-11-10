using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class AppointmentFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Appointment> _storage;

    public AppointmentFactory(IServiceProvider provider, Storage<Appointment> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public Appointment CreateNewAppointment(Therapist therapist, Patient? patient, DateTime dateOfAppointment, string description)
    {
        var appointment = _provider.GetRequiredService<Appointment>();
        appointment.IdAppointment = Guid.NewGuid();
        appointment.DateOfAppointment = dateOfAppointment;
        appointment.Description = description;
        appointment.Therapist = therapist;
        appointment.Patient = patient;
        
        _storage.RegisterNew(appointment);
        
        return appointment;
    }
}