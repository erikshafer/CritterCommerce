using JasperFx.Events;
using Marten.Schema;

namespace Inventory.Api.Receiving;

public sealed record InboundShipmentReceived(Guid ReceivedShipmentId, string ShipmentNumber, Guid InboundShipmentId, string ReceivedBy, DateTime ReceivedAt);
public sealed record ReceivedShipmentLineItemAdded(string Sku, int ExpectedQuantity);
public sealed record ReceivedShipmentPutaway(Guid LocationId, string PutawayBy, DateTime PutawayAt);
public sealed record ReceivedLineItemQuantityRecorded(string Sku, int ReceivedQuantity);

public sealed record ReceivedShipment
{
    [Identity]
    public Guid Id { get; private set; }
    public string ShipmentNumber { get; private set; } = null!;
    public Guid InboundShipmentId { get; set; }
    public InboundShipmentStatus Status { get; private set; }
    public string ReceivedBy { get; private set; } = null!;
    public DateTime? ReceivedAt { get; private set; }
    public Guid? LocationId { get; private set; }
    public string PutawayBy { get; private set; } = null!;
    public DateTime? PutawayAt { get; private set; }
    public Dictionary<string, LineItem> LineItems { get; private set; } = new();

    /* factory method for creation; can use metadata inside IEvent */

    public static ReceivedShipment Create(IEvent<InboundShipmentReceived> @event)
    {
        var shipment = new ReceivedShipment();
        shipment.Apply(@event.Data);
        return shipment;
    }

    /* event methods */

    public void Apply(InboundShipmentReceived @event)
    {
        Id = @event.ReceivedShipmentId;
        ShipmentNumber = @event.ShipmentNumber;
        InboundShipmentId = @event.InboundShipmentId;
        Status = InboundShipmentStatus.Received;
        ReceivedBy = @event.ReceivedBy;
        ReceivedAt = @event.ReceivedAt;
    }

    public void Apply(ReceivedShipmentLineItemAdded @event)
    {
        LineItems[@event.Sku] = new LineItem(@event.Sku, @event.ExpectedQuantity, 0);
    }

    public void Apply(ReceivedLineItemQuantityRecorded @event)
    {
        if (LineItems.TryGetValue(@event.Sku, out var lineItem))
            lineItem = lineItem with { ReceivedQuantity = @event.ReceivedQuantity }; // TODO: debug this
    }

    public void Apply(ReceivedShipmentPutaway @event)
    {
        Status = InboundShipmentStatus.Putaway;
        LocationId = @event.LocationId;
        PutawayBy = @event.PutawayBy;
        PutawayAt = @event.PutawayAt;
    }

    /* business logic methods */

    public void RecordLineItemQuantity(string sku, int receivedQuantity)
    {


        var @event = new ReceivedLineItemQuantityRecorded(sku, receivedQuantity);
        Apply(@event);
    }

    public void PutawayShipment(Guid locationId, string putawayBy)
    {
        if (Status != InboundShipmentStatus.Received)
            throw new InvalidOperationException("Can only putaway received shipments");

        if (!AllQuantitiesReceived())
            throw new InvalidOperationException("All line items must be received before putaway");

        var @event = new ReceivedShipmentPutaway(locationId, putawayBy, DateTime.UtcNow);
        Apply(@event);
    }

    private bool AllQuantitiesReceived()
    {
        return LineItems.Values.All(item => item.ReceivedQuantity > 0);
    }
}

public enum InboundShipmentStatus
{
    Expected = 1,
    Received = 2,
    Putaway = 4
}
