using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory.ReceivingShipments.Views;

public record ExpectedQuantityAnticipatedView
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int ExpectedQuantity { get; set; }
}

public class ExpectedQuantityAnticipatedProjection : SingleStreamProjection<ExpectedQuantityAnticipatedView, Guid>
{
    public static ExpectedQuantityAnticipatedView Create(IEvent<ReceivedShipmentCreated> @event) =>
        new()
        {
            Id = @event.Id,
            ExpectedQuantity = 0,
        };

    public static ExpectedQuantityAnticipatedView Apply(ReceivedShipmentLineItemAdded @event, ExpectedQuantityAnticipatedView anticipatedView) =>
        anticipatedView with
        {
            Sku = @event.Sku,
            ExpectedQuantity = @event.ExpectedQuantity
        };
}
