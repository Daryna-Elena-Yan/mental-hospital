namespace Mental_Hospital.Models;

public class Equipment
{
    public Guid IdEquipment { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public DateTime ExpirationDate { get; set; }
    public virtual Room? Room { get; }
}