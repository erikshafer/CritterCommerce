using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory.Api.Receiving.Views;

public record ExpectedQuantityAnticipatedView
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int ExpectedQuantity { get; set; }
}

public class ExpectedQuantityAnticipatedProjection : SingleStreamProjection<ExpectedQuantityAnticipatedView, Guid>
{
    public static ExpectedQuantityAnticipatedView Create(IEvent<InboundShipmentReceived> @event) =>
        new()
        {
            Id = @event.Id,
            ShipmentNumber = @event.Data.ShipmentNumber,
            ExpectedQuantity = 0,
        };

    public static ExpectedQuantityAnticipatedView Apply(ReceivedShipmentLineItemAdded @event, ExpectedQuantityAnticipatedView anticipatedView) =>
        anticipatedView with
        {
            Sku = @event.Sku,
            ExpectedQuantity = @event.ExpectedQuantity
        };
}
