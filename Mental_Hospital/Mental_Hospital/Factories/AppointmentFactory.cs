using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class AppointmentFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Appointment> _storage;

    public AppointmentFactory(IServiceProvider provider, Storage<Appointment> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public Appointment CreateNewAppointment( DateTime dateOfAppointment, string description)
    {
        var appointment = _provider.GetRequiredService<Appointment>();
        appointment.DateOfAppointment = dateOfAppointment;
        appointment.Description = description;
        
        _storage.RegisterNew(appointment);
        
        return appointment;
    }
}