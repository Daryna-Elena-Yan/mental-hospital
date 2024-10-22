namespace Mental_Hospital.Models;

public class Room
{
    public int IdRoom { get; set; }
    public int Quantity { get; set; }
    public virtual ICollection<Equipment> Equipments { get; } = new List<Equipment>();
    public virtual ICollection<Room_Patient> RoomPatients { get; } = new List<Room_Patient>();
}