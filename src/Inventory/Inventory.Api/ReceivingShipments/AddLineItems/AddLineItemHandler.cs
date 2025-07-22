using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.AddLineItems;

public sealed record AddLineItem(Guid ShipmentId, string Sku, int ExpectedQuantity);

public static class AddLineItemHandler
{
    [WolverinePost("/api/receiving-shipments/{shipmentId}/line-items/add")]
    [AggregateHandler]
    public static object Handle(AddLineItem command)
    {
        // Business logic for adding a line item.
        // To be implemented.
        return new { Success = true };
    }
}
