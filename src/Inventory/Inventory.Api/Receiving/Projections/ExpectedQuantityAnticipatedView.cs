using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory.Api.Receiving.Projections;

public record ExpectedQuantityAnticipatedView
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int ExpectedQuantity { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
}

public class ExpectedQuantityAnticipatedProjection : SingleStreamProjection<ExpectedQuantityAnticipatedView, Guid>
{
    public static ExpectedQuantityAnticipatedView Create(IEvent<InboundOrderScheduled> @event) =>
        new()
        {
            Id = @event.Id,
            ExpectedQuantity = 0,
            ShipmentNumber = @event.Data.ShipmentNumber
        };

    public static ExpectedQuantityAnticipatedView Apply(InboundShipmentLineItemAdded @event, ExpectedQuantityAnticipatedView anticipatedView) =>
        anticipatedView with
        {
            Sku = @event.Sku,
            ExpectedQuantity = @event.ExpectedQuantity
        };
}
