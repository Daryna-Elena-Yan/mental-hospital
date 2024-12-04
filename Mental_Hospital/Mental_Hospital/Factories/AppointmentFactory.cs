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
        appointment.IdAppointment = Guid.NewGuid();
        appointment.DateOfAppointment = dateOfAppointment;
        appointment.Description = description;
        appointment.Therapist = therapist;
        appointment.Patient = patient;
        
        _storage.RegisterNew(appointment);
        
        return appointment;
    }

    public Appointment CreateAppointmentCopy(Appointment appointment)
    {
        var appointmentCopy = _provider.GetRequiredService<Appointment>();
        appointmentCopy.IdAppointment = appointment.IdAppointment;
        appointmentCopy.DateOfAppointment = appointment.DateOfAppointment;
        appointmentCopy.Description = appointment.Description;
        appointmentCopy.Therapist = appointment.Therapist;
        appointmentCopy.Patient = appointment.Patient;

        return appointmentCopy;
    }
    
    
}