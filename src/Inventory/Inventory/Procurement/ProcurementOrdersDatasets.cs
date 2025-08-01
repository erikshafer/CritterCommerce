namespace Inventory.Procurement;

public static class ProcurementOrdersDatasets
{
    public static readonly ProcurementOrder[] Data =
    {
        new() { Id = 1001, VendorId = 101, TrackingNumber = "1234567890001", Origin = "AustinTxUsa", Destination = "OmahaNeUsa", OrderedAt = DateTime.UtcNow, RecordedAt = DateTime.UtcNow.AddMinutes(-5) },
        new() { Id = 1002, VendorId = 102, TrackingNumber = "1234567890002", Origin = "OmahaNeUsa", Destination = "BellevueWaUsa", OrderedAt = DateTime.UtcNow, RecordedAt = DateTime.UtcNow.AddDays(-2).AddMinutes(321) },
        new() { Id = 1003, VendorId = 103, TrackingNumber = "1234567890003", Origin = "OmahaNeUsa", Destination = "PhiladelphiaPaUsa", OrderedAt = DateTime.UtcNow, RecordedAt = DateTime.UtcNow.AddDays(-1).AddMinutes(123) },
        new() { Id = 1004, VendorId = 104, TrackingNumber = "1234567890004", Origin = "OmahaNeUsa", Destination = "MemphisTnUsa", OrderedAt = DateTime.UtcNow, RecordedAt = DateTime.UtcNow.AddMinutes(-3) },
        new() { Id = 1005, VendorId = 101, TrackingNumber = "1234567890005", Origin = "OmahaNeUsa", Destination = "SanBernardinoCaUsa", OrderedAt = DateTime.UtcNow, RecordedAt = DateTime.UtcNow.AddMinutes(-2) },
        new() { Id = 1006, VendorId = 101, TrackingNumber = "1234567890006", Origin = "BellevueWaUsa", Destination = "TorontoOnCa", OrderedAt = DateTime.UtcNow, RecordedAt = DateTime.UtcNow.AddMinutes(-2) },
        new() { Id = 1007, VendorId = 101, TrackingNumber = "1234567890007", Origin = "AustinTxUsa", Destination = "TepotzotlanEmMex", OrderedAt = DateTime.UtcNow, RecordedAt = DateTime.UtcNow.AddMinutes(-1) }
    };
}
