namespace Base.WebHelpers;

public class EnumDisplayAttribute : Attribute
{
    public string Name { get; }
    public Type ResourceType { get; }

    public EnumDisplayAttribute(string name, Type resourceType)
    {
        Name = name;
        ResourceType = resourceType;
    }
}