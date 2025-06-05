namespace Inventory.FulfillmentCenters;

public sealed class FulfillmentCenter
{
    public string Name { get; }
    public int Id { get; }

    private FulfillmentCenter(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public static readonly FulfillmentCenter OmahaNeUsa          = new("OmahaNeUsa", 1);
    public static readonly FulfillmentCenter AustinTxUsa         = new("AustinTxUsa", 2);
    public static readonly FulfillmentCenter BellevueWaUsa       = new("BellevueWaUsa", 3);
    public static readonly FulfillmentCenter PhiladelphiaPaUsa   = new("PhiladelphiaPaUsa", 4);
    public static readonly FulfillmentCenter MemphisTnUsa        = new("MemphisTnUsa", 5);
    public static readonly FulfillmentCenter SanBernardinoCaUsa  = new("SanBernardinoCaUsa", 6);
    public static readonly FulfillmentCenter TorontoOnCa         = new("TorontoOnCa", 7);
    public static readonly FulfillmentCenter TepotzotlanEmMex    = new("TepotzotlanEmMex", 8);
    public static readonly FulfillmentCenter ColumbusOhUsa       = new("ColumbusOhUsa", 9);

    private static readonly List<FulfillmentCenter> _all = new()
    {
        OmahaNeUsa, AustinTxUsa, BellevueWaUsa, PhiladelphiaPaUsa, MemphisTnUsa,
        SanBernardinoCaUsa, TorontoOnCa, TepotzotlanEmMex, ColumbusOhUsa
    };

    public static IEnumerable<FulfillmentCenter> List() => _all;

    public static FulfillmentCenter? FromName(string name) =>
        _all.FirstOrDefault(fc => string.Equals(fc.Name, name, StringComparison.OrdinalIgnoreCase));

    public static FulfillmentCenter? FromId(int id) =>
        _all.FirstOrDefault(fc => fc.Id == id);

    public override string ToString() => Name;

    public override bool Equals(object? obj) =>
        obj is FulfillmentCenter other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    /* This may be useful? Overengineering? Example? Why not all of the above? */

    private static readonly List<FulfillmentCenter> _usa = new()
    {
        OmahaNeUsa, AustinTxUsa, BellevueWaUsa, PhiladelphiaPaUsa, MemphisTnUsa,
        SanBernardinoCaUsa, ColumbusOhUsa
    };

    public static IEnumerable<FulfillmentCenter> UnitedStatesOfAmerica() => _usa;

    private static readonly List<FulfillmentCenter> _outsideTheUsa = new()
    {
        TorontoOnCa, TepotzotlanEmMex
    };

    public static IEnumerable<FulfillmentCenter> OutsideTheUnitedStatesOfAmerica() => _outsideTheUsa;
}

