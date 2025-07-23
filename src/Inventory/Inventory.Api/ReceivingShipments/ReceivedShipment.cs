using System.Collections.Immutable;
using JasperFx.Events;

namespace Inventory.Api.ReceivingShipments;

public sealed record ReceivedShipmentCreated(
    string ShippingNumber,
    string Facility,
    DateTime DeliveredAt
);

public sealed record ReceivedShipmentLineItemAdded(
    string Sku,
    int ExpectedQuantity
);

public sealed record ReceivedShipmentLineItemQuantityRecorded(
    string Sku,
    int ReceivedQuantity
);

public sealed record ReceivedShipmentMarkedAsReceived(
    DateTimeOffset ReceivedAt
);

public sealed record ReceivedShipmentPutAway(
    string PutawayLotId,
    DateTimeOffset PutawayAt
);

public sealed record ReceivedShipment(
    Guid Id,
    ReceivingShipmentStatus Status,
    string? ShippingNumber,
    string? Facility,
    DateTime? DeliveredAt,
    IReadOnlyList<LineItem> LineItems,
    string? PutawayLotId,
    DateTime? PutawayAt,
    DateTime? ReceivedAt = null
)
{
    public static ReceivedShipment Create(IEvent<ReceivedShipmentCreated> @event)
        => new (
            @event.StreamId,
            ReceivingShipmentStatus.Created,
            @event.Data.ShippingNumber,
            @event.Data.Facility,
            @event.Data.DeliveredAt,
            ImmutableList<LineItem>.Empty,
            null,
            null
        );

    public static ReceivedShipment Apply(ReceivedShipment state, ReceivedShipmentLineItemAdded @event)
        => state with
        {
            Status = ReceivingShipmentStatus.Receiving,
            LineItems = state.LineItems
                .Append(new LineItem
                {
                    Sku = @event.Sku,
                    ExpectedQuantity = @event.ExpectedQuantity,
                    ReceivedQuantity = null
                })
                .ToImmutableList()
        };

    public static ReceivedShipment Apply(ReceivedShipment state, ReceivedShipmentLineItemQuantityRecorded @event)
    {
        var updatedLineItems = state.LineItems
            .Select(li => li.Sku == @event.Sku
                ? li with { ReceivedQuantity = @event.ReceivedQuantity }
                : li)
            .ToImmutableList();

        // Optional: Status could be changed to 'Received' if all line items now have ReceivedQuantity filled.
        var allReceived = updatedLineItems.All(li => li.ReceivedQuantity.HasValue);

        return state with
        {
            LineItems = updatedLineItems,
            Status = allReceived ? ReceivingShipmentStatus.Received : state.Status
        };
    }

    public static ReceivedShipment Apply(ReceivedShipment state, ReceivedShipmentMarkedAsReceived @event)
        => state with
        {
            Status = ReceivingShipmentStatus.Received,
            ReceivedAt = @event.ReceivedAt.UtcDateTime
        };

    public static ReceivedShipment Apply(ReceivedShipment state, ReceivedShipmentPutAway @event)
        => state with
        {
            Status = ReceivingShipmentStatus.PutAway,
            PutawayLotId = @event.PutawayLotId,
            PutawayAt = @event.PutawayAt.UtcDateTime
        };
}
