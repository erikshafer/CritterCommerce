namespace Legacy.SkuManagement.Domain.SkuReservations;

public class SkuReservation
{
    public required int Unit { get; set; }
    public required bool IsReserved { get; set; }
    public required string ReservedByUsername { get; set; } = null!;
}
