namespace Mental_Hospital.Models;

public class Room
{
    public Guid IdRoom { get; } = Guid.NewGuid();
    public int Quantity { get; set; }
    public virtual ICollection<Nurse> Nurses { get; } = new List<Nurse>();

    public virtual ICollection<Equipment> Equipments { get; } = new List<Equipment>();
    public virtual ICollection<RoomPatient> RoomPatients { get; } = new List<RoomPatient>();
}