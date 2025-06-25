using Wolverine.Marten;

namespace Inventory.Api.Receiving;

public record RecordLineItemQuantity(Guid ShipmentId, string Sku, int ReceivedQuantity);

public static class RecordLineItemQuantityHandler
{
    [AggregateHandler]
    public static Events Handle(RecordLineItemQuantity command, InboundShipment shipment)
    {
        var events = new Events();

        shipment.RecordLineItemQuantity(command.Sku, command.ReceivedQuantity);
        events += new LineItemQuantityReceived(command.Sku, command.ReceivedQuantity);

        return events;
    }
}
