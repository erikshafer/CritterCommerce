namespace Inventory;

public record Name
{
    public string Value { get; internal init; } = string.Empty;

    internal Name() { }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Name value cannot be empty");

        if (value.Length <= 2)
            throw new DomainException("A product's name must exceed 2 characters");

        if (value.Length > 100)
            throw new DomainException("A product's name cannot exceed 100 characters");

        Value = value;
    }

    public bool HasSameValue(string another)
        => string.Compare(Value, another, StringComparison.CurrentCulture) == 0;

    public static implicit operator string(Name name)
        => name.Value;
}
