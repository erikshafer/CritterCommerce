using JasperFx.Events;

namespace Inventory.Api.Receiving;

public sealed record InboundOrderScheduled(Guid Id, string ShipmentNumber, DateTime ExpectedArrival);
public sealed record InboundShipmentReceived(string ReceivedBy, DateTime ReceivedAt);
public sealed record InboundShipmentLineItemAdded(string Sku, int ExpectedQuantity);
public sealed record InboundShipmentPutaway(Guid LocationId, string PutawayBy, DateTime PutawayAt);
public sealed record LineItemQuantityReceived(string Sku, int ReceivedQuantity);

public sealed record InboundShipment
{
    public Guid Id { get; private set; }
    public string ShipmentNumber { get; private set; } = null!;
    public DateTime ExpectedArrival { get; private set; }
    public InboundShipmentStatus Status { get; private set; }
    public string ReceivedBy { get; private set; } = null!;
    public DateTime? ReceivedAt { get; private set; }
    public Guid? LocationId { get; private set; }
    public string PutawayBy { get; private set; } = null!;
    public DateTime? PutawayAt { get; private set; }

    public Dictionary<string, ReceivingLineItem> LineItems { get; private set; } = new();

    /* factory method for creation; can use metadata inside IEvent */

    public static InboundShipment Create(IEvent<InboundOrderScheduled> @event)
    {
        var shipment = new InboundShipment();
        shipment.Apply(@event.Data);
        return shipment;
    }

    /* event methods */

    public void Apply(InboundOrderScheduled @event)
    {
        Id = @event.Id;
        ShipmentNumber = @event.ShipmentNumber;
        ExpectedArrival = @event.ExpectedArrival;
        Status = InboundShipmentStatus.Expected;
    }

    public void Apply(InboundShipmentLineItemAdded @event)
    {
        LineItems[@event.Sku] = new ReceivingLineItem(@event.Sku, @event.ExpectedQuantity, 0);
    }

    public void Apply(InboundShipmentReceived @event)
    {
        Status = InboundShipmentStatus.Received;
        ReceivedBy = @event.ReceivedBy;
        ReceivedAt = @event.ReceivedAt;
    }

    public void Apply(LineItemQuantityReceived @event)
    {
        if (LineItems.TryGetValue(@event.Sku, out var lineItem))
            lineItem = lineItem with { ReceivedQuantity = @event.ReceivedQuantity }; // TODO: debug this
    }

    public void Apply(InboundShipmentPutaway @event)
    {
        Status = InboundShipmentStatus.Putaway;
        LocationId = @event.LocationId;
        PutawayBy = @event.PutawayBy;
        PutawayAt = @event.PutawayAt;
    }

    /* business logic methods */

    public void ReceiveShipment(string receivedBy)
    {
        if (Status != InboundShipmentStatus.Expected)
            throw new InvalidOperationException("Shipment can only be received when in Expected status");

        var @event = new InboundShipmentReceived(receivedBy, DateTime.UtcNow);
        Apply(@event);
    }

    public void RecordLineItemQuantity(string sku, int receivedQuantity)
    {
        if (Status != InboundShipmentStatus.Received)
            throw new InvalidOperationException("Can only record quantities for received shipments");

        if (!LineItems.ContainsKey(sku))
            throw new InvalidOperationException($"Sku {sku} is not part of this shipment");

        var @event = new LineItemQuantityReceived(sku, receivedQuantity);
        Apply(@event);
    }

    public void PutawayShipment(Guid locationId, string putawayBy)
    {
        if (Status != InboundShipmentStatus.Received)
            throw new InvalidOperationException("Can only putaway received shipments");

        if (!AllQuantitiesReceived())
            throw new InvalidOperationException("All line items must be received before putaway");

        var @event = new InboundShipmentPutaway(locationId, putawayBy, DateTime.UtcNow);
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
