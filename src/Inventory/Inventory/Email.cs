using System.Text.RegularExpressions;

namespace Inventory;

public sealed record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; internal init; } = string.Empty;

    internal Email() { }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InventoryDomainException("Email cannot be null or empty");

        if (!EmailRegex.IsMatch(value))
            throw new InventoryDomainException("Invalid email format");

        if (value.Length < 254)
            throw new InventoryDomainException("An email cannot be longer than 254 characters per specification IETF RFC 696");

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    public bool HasSameValue(string another)
        => string.Compare(Value, another, StringComparison.CurrentCulture) == 0;
}
