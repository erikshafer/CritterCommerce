namespace Inventory.Api.FulfillmentCenters;

public sealed class FulfillmentCenter
{
    /// <summary>
    /// The concatenated name of a Fulfillment Center. Should include the
    /// city's name along with the region, state, or province. The country's
    /// abbreviation is at the end.
    /// </summary>
    /// <example>
    /// The location of "Omaha, Nebraska, United States of America", which consists
    /// of a city, state, and country, is abbreviated as "OmahaNeUsa".
    /// </example>
    public string Name { get; }

    /// <summary>
    /// The "code", or shorthand, that can be used as a more concise identifier
    /// than the name. Is typically 3 or 4 characters long.
    /// Locations in the USA do not have an identifier at the end of the code
    /// unlike the other (international) locations do, such as "MX" at the end of
    /// "TPMX" for the location (name) "TepotzotlanEmMex".
    /// </summary>
    public string Shorthand { get; }

    /// <summary>
    /// The integer identifier for the smart enum. Somewhat comparable to the
    /// underlying integral numeric type that C# (CLR) enumeration types use.
    /// </summary>
    public int Id { get; }

    private FulfillmentCenter(string name, string shorthand, int id)
    {
        Name = name;
        Shorthand = shorthand;
        Id = id;
    }

    public static readonly FulfillmentCenter OmahaNeUsa          = new("OmahaNeUsa", "OMA", 1);
    public static readonly FulfillmentCenter AustinTxUsa         = new("AustinTxUsa", "ATX", 2);
    public static readonly FulfillmentCenter BellevueWaUsa       = new("BellevueWaUsa", "BLV", 3);
    public static readonly FulfillmentCenter PhiladelphiaPaUsa   = new("PhiladelphiaPaUsa", "PHL", 4);
    public static readonly FulfillmentCenter MemphisTnUsa        = new("MemphisTnUsa", "MPH", 5);
    public static readonly FulfillmentCenter SanBernardinoCaUsa  = new("SanBernardinoCaUsa", "SBN", 6);
    public static readonly FulfillmentCenter TorontoOnCa         = new("TorontoOnCa", "TOCA", 7);
    public static readonly FulfillmentCenter TepotzotlanEmMex    = new("TepotzotlanEmMex", "TPMX", 8);
    public static readonly FulfillmentCenter ColumbusOhUsa       = new("ColumbusOhUsa", "COLO", 9);

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

    /*
     * These may be useful?
     * Possibly overengineered?
     * An example?
     * Why not all of the above?
     */

    public static IEnumerable<FulfillmentCenter> UnitedStatesOfAmerica() => _usa;
    private static readonly List<FulfillmentCenter> _usa = new()
    {
        OmahaNeUsa, AustinTxUsa, BellevueWaUsa, PhiladelphiaPaUsa, MemphisTnUsa,
        SanBernardinoCaUsa, ColumbusOhUsa
    };

    public static IEnumerable<FulfillmentCenter> OutsideTheUnitedStatesOfAmerica() => _outsideTheUsa;
    private static readonly List<FulfillmentCenter> _outsideTheUsa = new()
    {
        TorontoOnCa, TepotzotlanEmMex
    };

    public static IEnumerable<FulfillmentCenter> Canada() => _canada;
    private static readonly List<FulfillmentCenter> _canada = new()
    {
        TorontoOnCa
    };

    public static IEnumerable<FulfillmentCenter> Mexico() => _mexico;
    private static readonly List<FulfillmentCenter> _mexico = new()
    {
        TepotzotlanEmMex
    };
}
