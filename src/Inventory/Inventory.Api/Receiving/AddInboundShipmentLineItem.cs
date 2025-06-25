using Wolverine.Marten;

namespace Inventory.Api.Receiving;

public record AddInboundShipmentLineItem(Guid ShipmentId, string Sku, int ExpectedQuantity);

public static class AddInboundShipmentLineItemHandler
{

    [AggregateHandler]
    public static Events Handle(AddInboundShipmentLineItem command, InboundShipment shipment)
    {
        var events = new Events();
        events += new InboundShipmentLineItemAdded(command.Sku, command.ExpectedQuantity);
        return events;
    }
}
