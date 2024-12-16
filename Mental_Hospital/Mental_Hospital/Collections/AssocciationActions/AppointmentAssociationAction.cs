using Mental_Hospital.Models;

namespace Mental_Hospital.Collections.AssocciationActions;

public class AppointmentAssociationAction : IAssociationAction<Appointment>
{
    public void OnAdd(Appointment item)
    {
        //implemented on creation
    }

    public void OnDelete(Appointment item)
    {
        
    }

    public void OnClear(Appointment item)
    {
       
    }
}