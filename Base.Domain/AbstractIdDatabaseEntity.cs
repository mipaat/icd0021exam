namespace Base.Domain;

public class AbstractIdDatabaseEntity : IIdDatabaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}