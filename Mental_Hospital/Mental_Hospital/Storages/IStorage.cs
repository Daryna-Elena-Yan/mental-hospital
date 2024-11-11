namespace Mental_Hospital.Storages;

public interface IStorage
{
    public string Serialize();
    public void Deserialize(string json);
    public void RestoreAllConnections();
    
}