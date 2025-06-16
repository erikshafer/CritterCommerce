using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory.Receiving;

public record ExpectedQuantityAnticipated
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int ExpectedQuantity { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
}

public class ExpectedQuantityAnticipatedProjection : SingleStreamProjection<ExpectedQuantityAnticipated, Guid>
{
    public static ExpectedQuantityAnticipated Create(IEvent<InboundOrderScheduled> evt) =>
        new()
        {
            Id = evt.Id,
            ExpectedQuantity = 0,
            ShipmentNumber = evt.Data.ShipmentNumber
        };

    public static ExpectedQuantityAnticipated Apply(InboundShipmentLineItemAdded evt, ExpectedQuantityAnticipated anticipated) =>
        anticipated with
        {
            Sku = evt.Sku,
            ExpectedQuantity = evt.ExpectedQuantity
        };
}
