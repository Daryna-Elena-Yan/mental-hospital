namespace Mental_Hospital.Models;

public class RoomPatient
{
    public DateTime DatePlaced { get; set; }
    public DateTime? DateDischarged { get; set; }
    public virtual Room Room { get; set; } = null!;
    public virtual Patient Patient { get; set;  } = null!;
}