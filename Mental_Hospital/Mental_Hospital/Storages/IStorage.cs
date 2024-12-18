namespace Mental_Hospital.Storages;

public interface IStorage
{
    public string Serialize();
    public void Deserialize(string json);
    public void RestoreAllConnections();

    public object? GetById(Guid id);

    public void DeleteById(Guid id);

}