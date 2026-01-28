namespace Yact.Domain.Primitives;

public abstract record ValueObjectId<TId> where TId : ValueObjectId<TId>
{
    public Guid Value { get; protected set; }
    protected ValueObjectId(Guid value)
    {
        Value = value;
    }

    private static TId CreateInstance(Guid value)
    {
        return (TId)Activator.CreateInstance(
            typeof(TId),
            new object[] { value })!;
    }

    public static TId NewId() => CreateInstance(Guid.NewGuid());
    public static TId From(Guid value)
    {
        return CreateInstance(value);
    }
}
