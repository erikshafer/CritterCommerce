using JasperFx.Events;

namespace Inventory.Receiving;

/* events, the source of truth */

public record InboundOrderScheduled(Guid Id, string ShipmentNumber, DateTime ExpectedArrival);
public record InboundShipmentReceived(string ReceivedBy, DateTime ReceivedAt);
public record InboundShipmentLineItemAdded(string Sku, int ExpectedQuantity);
public record InboundShipmentPutaway(Guid LocationId, string PutawayBy, DateTime PutawayAt);
public record LineItemQuantityReceived(string Sku, int ReceivedQuantity);

public class InboundShipment
{
    public Guid Id { get; private set; }
    public string ShipmentNumber { get; private set; }
    public DateTime ExpectedArrival { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public string ReceivedBy { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public Guid? LocationId { get; private set; }
    public string PutawayBy { get; private set; }
    public DateTime? PutawayAt { get; private set; }

    public Dictionary<string, LineItem> LineItems { get; private set; } = new();

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
        Status = ShipmentStatus.Expected;
    }

    public void Apply(InboundShipmentLineItemAdded @event)
    {
        LineItems[@event.Sku] =
            new LineItem(Sku: @event.Sku, ExpectedQuantity: @event.ExpectedQuantity, ReceivedQuantity: 0);
    }

    public void Apply(InboundShipmentReceived @event)
    {
        Status = ShipmentStatus.Received;
        ReceivedBy = @event.ReceivedBy;
        ReceivedAt = @event.ReceivedAt;
    }

    public void Apply(LineItemQuantityReceived @event)
    {
        if (LineItems.TryGetValue(@event.Sku, out var lineItem))
        {
            lineItem = lineItem with { ReceivedQuantity = @event.ReceivedQuantity }; // TODO: debug this
        }
    }

    public void Apply(InboundShipmentPutaway @event)
    {
        Status = ShipmentStatus.Putaway;
        LocationId = @event.LocationId;
        PutawayBy = @event.PutawayBy;
        PutawayAt = @event.PutawayAt;
    }

    /* business logic methods */

    public void ReceiveShipment(string receivedBy)
    {
        if (Status != ShipmentStatus.Expected)
            throw new InvalidOperationException("Shipment can only be received when in Expected status");

        var @event = new InboundShipmentReceived(receivedBy, DateTime.UtcNow);
        Apply(@event);
    }

    public void RecordLineItemQuantity(string sku, int receivedQuantity)
    {
        if (Status != ShipmentStatus.Received)
            throw new InvalidOperationException("Can only record quantities for received shipments");

        if (!LineItems.ContainsKey(sku))
            throw new InvalidOperationException($"Sku {sku} is not part of this shipment");

        var @event = new LineItemQuantityReceived(sku, receivedQuantity);
        Apply(@event);
    }

    public void PutawayShipment(Guid locationId, string putawayBy)
    {
        if (Status != ShipmentStatus.Received)
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

public record LineItem(string Sku, int ExpectedQuantity, int ReceivedQuantity);

public enum ShipmentStatus
{
    Expected = 1,
    Received = 2,
    Putaway = 4
}
