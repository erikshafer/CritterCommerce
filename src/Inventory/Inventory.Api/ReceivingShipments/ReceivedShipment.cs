using System.Collections.Immutable;

namespace Inventory.Api.ReceivingShipments;

public sealed record ReceivedShipmentCreated(
    Guid ShipmentId
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
    Guid LocationId,
    DateTimeOffset PutawayAt
);

public sealed record ReceivedShipment(
    Guid Id,
    ReceivingShipmentStatus Status,
    IReadOnlyList<ShipmentLineItem> LineItems,
    Guid? PutawayLocationId,
    DateTime? PutawayAt,
    DateTime? ReceivedAt = null
)
{
    private ReceivedShipment() : this(
        Guid.Empty,
        ReceivingShipmentStatus.Created,
        ImmutableList<ShipmentLineItem>.Empty,
        null,
        null)
    { }

    public static ReceivedShipment Create(ReceivedShipmentCreated @event)
        => new (
            @event.ShipmentId,
            ReceivingShipmentStatus.Created,
            ImmutableList<ShipmentLineItem>.Empty,
            null,
            null
        );

    public static ReceivedShipment Apply(ReceivedShipment state, ReceivedShipmentLineItemAdded @event)
        => state with
        {
            Status = ReceivingShipmentStatus.Receiving,
            LineItems = state.LineItems
                .Append(new ShipmentLineItem
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
            PutawayLocationId = @event.LocationId,
            PutawayAt = @event.PutawayAt.UtcDateTime
        };

}
