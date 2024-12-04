using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class AppointmentFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Appointment> _storage;
    private readonly AppointmentValidator _validator;

    public AppointmentFactory(IServiceProvider provider, Storage<Appointment> storage, AppointmentValidator validator)
    {
        _provider = provider;
        _storage = storage;
        _validator = validator;
    }

    public Appointment CreateNewAppointment(Therapist therapist, Patient? patient, DateTime dateOfAppointment, string description)
    {
        var appointment = _provider.GetRequiredService<Appointment>();
        appointment.Id = Guid.NewGuid();
        appointment.DateOfAppointment = dateOfAppointment;
        appointment.Description = description;
        appointment.Therapist = therapist;
        appointment.Patient = patient;
        
        _validator.ValidateAndThrow(appointment);
        _storage.RegisterNew(appointment);
        
        return appointment;
    }
}