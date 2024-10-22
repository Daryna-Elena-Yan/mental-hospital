namespace Mental_Hospital.Models;

public class Room_Patient
{
    public DateTime DatePlaced { get; set; }
    public DateTime? DateDischarged { get; set; }
    public virtual Room Room { get; } = null!;
    public virtual Patient Patient { get; } = null!;
}